using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NCrontab;
using NLog;
using SAPbobsCOM;

namespace SapBusinessOneExtensions
{
    public class SboTaskScheduler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, ScheduledTask> _scheduledTasks;
        private readonly TimeSpan _frequency;
        private Timer _timer;

        public SboTaskScheduler(TimeSpan frequency)
        {
            _scheduledTasks = new Dictionary<string, ScheduledTask>();
            _frequency = frequency;
            _timer = new Timer(Callback, null, _frequency, _frequency);
        }

        private void Callback(object state)
        {
            if (!_scheduledTasks.Any() || SboAddon.Instance.IdleTime < TimeSpan.FromMinutes(1.5))
                return;

            Logger.Trace("SAP Business One has been idle for {0} - running scheduled tasks", SboAddon.Instance.IdleTime);

            var tasks = (from t in _scheduledTasks.Values
                let nextOccurrence = t.Schedule.GetNextOccurrence(t.LastOccurrence ?? DateTime.MinValue)
                where nextOccurrence < DateTime.Now
                select t).ToList();

            if (!tasks.Any())
                return;

            foreach (var task in tasks)
            {
                try
                {
                    using (var progressReporter = SboBackgroundProgress.Create())
                    {
                         Run(task, progressReporter.Progress).Wait(TimeSpan.FromHours(1));
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e, $"Error running task {task.Name}");
                }
            }
        }

        public Task<bool> Run(string taskName, bool force = false)
        {
            return Run(_scheduledTasks[taskName], null, force);
        }

        public Task<bool> Run(ScheduledTask task, IProgress<SboJobProgress> progress = null, bool force = false)
        {
            Task<bool> returnTask = Task.FromResult(false);

            try
            {
                SboTransaction.Start();
                var lockResult = SboDistributedLock.GetLock($"SboTaskScheduler.{task.Name}");
                if (lockResult != 0)
                {
                    SboTransaction.Rollback();
                    return returnTask;
                }

                var lastOccurrence = SboAddon.Instance.Settings.GetValueOrDefault<DateTime?>("taskscheduler.lastoccurrence." + task.Name);
                var nextOccurrence = SboAddon.Instance.Settings.GetValueOrDefault<DateTime?>("taskscheduler.nextoccurrence." + task.Name);
                var synchronicity = task.IsAsync ? "asynchronous" : "synchronous";
                Logger.Trace(
                    $"Checking if task {task.Name} is applicable, last occurrence {lastOccurrence} - next occurrence {nextOccurrence}");

                if (force || !nextOccurrence.HasValue || nextOccurrence.Value < DateTime.Now)
                {
                    // Set new values
                    lastOccurrence = DateTime.Now;
                    nextOccurrence = task.Schedule.GetNextOccurrence(lastOccurrence.Value);
                    SboAddon.Instance.Settings.SetValue("taskscheduler.lastoccurrence." + task.Name, lastOccurrence.Value);
                    SboAddon.Instance.Settings.SetValue("taskscheduler.nextoccurrence." + task.Name, nextOccurrence.Value);

                    SboTransaction.Commit();

                    // Run task
                    var cancellationToken = CancellationToken.None;
                    var task1 = task;
                    var progressObject = progress;
                    returnTask = Task.Factory.StartNew( async () =>
                        {
                            Logger.Info($@"Running {synchronicity} task {task.Name}.");

                            var stopwatch = Stopwatch.StartNew();

                            try
                            {
                                task.Action?.Invoke(progressObject);

                                if (task.AsyncAction != null)
                                    await task.AsyncAction(progressObject);
                            }
                            catch (Exception e)
                            {
                                Logger.Error(e, $"Unhandled exception occurred running scheduled task {task.Name}");
                            }

                            stopwatch.Stop();

                            Logger.Info(
                                $"Completed task {task1.Name} in {stopwatch.Elapsed}. Next occurrence {nextOccurrence}");

                            SboAddonTracker.TrackEvent("ScheduledTaskRun", new Dictionary<string, string> {["TaskName"] = task1.Name},
                                new Dictionary<string, double> {["Duration"] = stopwatch.ElapsedMilliseconds});

                            return true;

                        }, cancellationToken, TaskCreationOptions.None,
                        task.IsAsync ? TaskScheduler.Default : SboAddon.Instance.UiTaskScheduler).Unwrap();
                }
                else if (SboAddon.Instance.Company.InTransaction)
                    SboTransaction.Commit();

                task.LastOccurrence = lastOccurrence;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Unhandled exception occurred running scheduled tasks");
                try { SboTransaction.Rollback(); } catch (Exception) { }
            }

            return returnTask;
        }

        public void Schedule(ScheduledTask task)
        {
            _scheduledTasks.Add(task.Name, task);
        }

        public void Schedule(String name, Action<IProgress<SboJobProgress>> action, string schedule)
        {
            Schedule(new ScheduledTask { Name = name, Action = action, ScheduleString = schedule });
        }

        public void UnSchedule(ScheduledTask task)
        {
            UnSchedule(task.Name);
        }

        public void UnSchedule(String name)
        {
            if (_scheduledTasks.ContainsKey(name))
                _scheduledTasks.Remove(name);
        }

        public class ScheduledTask
        {
            public string Name { get; set; }
            public Action<IProgress<SboJobProgress>>  Action { get; set; }
            public Func<IProgress<SboJobProgress>, Task> AsyncAction { get; set; }
            public string ScheduleString { set { Schedule = CrontabSchedule.Parse(value); } }
            internal CrontabSchedule Schedule { get; private set; }
            public DateTime? LastOccurrence { get; set; }
            public bool IsAsync { get; set; } = false;
        }
    }
}
