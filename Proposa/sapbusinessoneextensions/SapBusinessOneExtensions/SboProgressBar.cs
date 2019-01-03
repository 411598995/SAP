using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using NLog;
using SAPbouiCOM;

namespace SapBusinessOneExtensions
{
    public sealed class SboProgressBar : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Form Form { get; set; }
        public int MaxValue { get; set; }
        private int _value;
        public int Value { get { return _value; } 
            set { 
                _value = value;
                try
                {
                    Form.Items.Item("bar").Width = (int) Math.Round((Form.ClientWidth - 10d)*(_value/(double) MaxValue));
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Error updating progress bar");
                }

                // Check if process is taking a long time. If so, process windows messages
                var longRunningTime = DateTime.Now - LongRunningStartTime;
                if (longRunningTime.TotalMinutes > 0.25)
                {
                    LongRunningStartTime = DateTime.Now;
                    Logger.Trace("Long running process ({0}) ({1}/{2}) ({3} left), processing Windows messages", DateTime.Now - StartTime,
                        _value, MaxValue, TimeSpan.FromTicks((DateTime.Now - StartTime).Ticks/Math.Max(_value, 1)*(MaxValue - _value)));
                    SboAddon.Instance.Application.RemoveWindowsMessage(BoWindowsMessageType.bo_WM_TIMER, true);
                    SboAddon.Instance.LastEvent = DateTime.Now;
                }
            } }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                try
                {
                    Form.Items.Item<StaticText>("text").Caption = _text;
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Error updating progress text");
                }
            }
        }

        private IProgress<SboJobProgress> _progress;
        public IProgress<SboJobProgress> Progress
        {
            get
            {
                if (_progress == null)
                {
                    _progress = new SboProgress(progress =>
                    {
                        if (!string.IsNullOrWhiteSpace(progress.Message))
                            Text = progress.Message;
                        Value = (int) Math.Round((MaxValue/100M)*progress.PercentageOfCompletion);
                    });
                }

                return _progress;
            }
        }

        private DateTime LongRunningStartTime { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan RunTime => DateTime.Now - StartTime;

        private SboProgressBar(string text, int maxValue, Form relativeTo = null,int width=200)
        {
            Logger.Trace("Creating new progress bar form with text '{0}', max value {1}", text, maxValue);

            StartTime = LongRunningStartTime = DateTime.Now;

            var formCreation = (FormCreationParams) SboAddon.Instance.Application.CreateObject(BoCreatableObjectType.cot_FormCreationParams);
            formCreation.UniqueID = formCreation.FormType = Guid.NewGuid().ToString().Replace("-", "");
            formCreation.BorderStyle = BoFormBorderStyle.fbs_Floating;
            Form = SboAddon.Instance.Application.Forms.AddEx(formCreation);

            Form.Width = width;
            Form.Height = 50;
            Form.Settings.Enabled = false;
            if (relativeTo == null)
            {
                Form.Top = (int) ((SboAddon.Instance.Application.Desktop.Height - Form.Height)/3d);
                Form.Left = (int) ((SboAddon.Instance.Application.Desktop.Width - Form.Width)/2d);
            }
            else
            {
                Form.Top = (int)(relativeTo.Top + ((relativeTo.Height - Form.Height) / 2d));
                Form.Left = (int)(relativeTo.Left + ((relativeTo.Width - Form.Width) / 2d));
            }

            Item textItem = Form.Items.Add("text", BoFormItemTypes.it_STATIC);
            textItem.Top = 5;
            textItem.Left = 5;
            textItem.Width = Form.ClientWidth - 10;

            Item barItem = Form.Items.Add("bar", BoFormItemTypes.it_STATIC);
            barItem.Top = 25;
            barItem.Left = 5;
            barItem.Height = 20;
            barItem.Width = 0;
            barItem.BackColor = Color.CornflowerBlue.ToSapColor();

            Text = text;
            Value = 0;
            MaxValue = maxValue;

            Form.Visible = true;
            Thread.Sleep(100);
        }

        public static SboProgressBar Create(string text, int maxValue, Form relativeTo = null,int width=200)
        {
            return new SboProgressBar(text, maxValue, relativeTo,width);
        }

        public void Dispose()
        {
            if (Form != null)
            {
                if (Logger != null)
                    Logger.Trace("Disposing of progress bar.");

                Thread.Sleep(100);
                try
                {
                    Form.Close();
                }
                catch (Exception e)
                {
                    if (Logger != null)
                        Logger.Error(e, "Error closing progress bar form");
                }
                finally
                {
                    Marshal.ReleaseComObject(Form);
                    Form = null;

                    GC.SuppressFinalize(this);
                }
            }
        }

        ~SboProgressBar()
        {
            Dispose();
        }

    }

    public sealed class SboBackgroundProgress : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private IProgress<SboJobProgress> _progress;
        public IProgress<SboJobProgress> Progress
        {
            get
            {
                return _progress ?? (_progress = new SboProgress(progress =>
                       {
                           // Check if process is taking a long time. If so, process windows messages
                           var longRunningTime = DateTime.Now - LongRunningStartTime;
                           if (longRunningTime.TotalMinutes > 0.5)
                           {
                               LongRunningStartTime = DateTime.Now;
                               Logger.Trace("Long running process ({0}), processing Windows messages", DateTime.Now - StartTime);
                               SboAddon.Instance.Application.RemoveWindowsMessage(BoWindowsMessageType.bo_WM_TIMER, true);
                               SboAddon.Instance.LastEvent = DateTime.Now;
                           }
                       }));
            }
        }

        private DateTime LongRunningStartTime { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan RunTime => DateTime.Now - StartTime;

        private SboBackgroundProgress()
        {
            Logger.Trace("Creating new background progress reporter");

            StartTime = LongRunningStartTime = DateTime.Now;
        }

        public static SboBackgroundProgress Create()
        {
            return new SboBackgroundProgress();
        }

        public void Dispose()
        {
            _progress = null;
        }

        ~SboBackgroundProgress()
        {
            Dispose();
        }

    }

    public class SboProgress : IProgress<SboJobProgress>
    {
        private readonly Action<SboJobProgress> _action;
        public SboProgress(Action<SboJobProgress> action)
        {
            _action = action;
        }
             
        public void Report(SboJobProgress value)
        {
            _action(value);
        }
    }

    public class SboJobProgress
    {
        public SboJobProgress(string message, int percentageOfCompletion)
        {
            Message = message;
            PercentageOfCompletion = percentageOfCompletion;
        }

        public SboJobProgress(string message, int current, int count)
        {
            Message = message;
            PercentageOfCompletion = (int) (current/(double) count*100d);
        }

        public SboJobProgress(int percentageOfCompletion)
        {
            PercentageOfCompletion = percentageOfCompletion;
        }

        public SboJobProgress(int current, int count)
        {
            PercentageOfCompletion = (int) (current/(double) count*100d);
        }

        public string Message { get; set; }
        public int PercentageOfCompletion { get; set; }
    }
}
