using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using NLog;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;
using System.Collections.Generic;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;

namespace SapBusinessOneExtensions
{
    public static class SboAddonTracker
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static string _myIp;
        private static TelemetryClient Client { get; set; }
        
        public static string DistinctId
        {
            get
            {
                var bytes = Encoding.UTF8.GetBytes(
                    string.Format(
                        $"{SboAddon.Instance.Application.Company.SystemId}|{SboAddon.Instance.Application.Company.InstallationId}|{SboAddon.Instance.Application.Company.UserName.ToLowerInvariant()}"));
                return
                    BitConverter.ToString(((HashAlgorithm) CryptoConfig.CreateFromName("SHA256")).ComputeHash(bytes))
                        .Replace("-", string.Empty)
                        .ToLower();
            }
        }

        public static string MyIp
        {
            get
            {
                try
                {
                    if (_myIp == null)
                        _myIp = new WebClient().DownloadString("http://icanhazip.com").Trim();

                    return _myIp;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        public static void Initialize(string token)
        {
            try
            {
                TelemetryConfiguration.Active.InstrumentationKey = token;
                TelemetryConfiguration.Active.TelemetryProcessorChainBuilder.Use(next => new ExcludeLoggerFilter(next));
                TelemetryConfiguration.Active.TelemetryProcessorChainBuilder.Build();

                Client = new TelemetryClient();
                Client.Context.Session.Id = Guid.NewGuid().ToString();
                Client.Context.User.AccountId = SboAddon.Instance.Application.Company.SystemId;
                Client.Context.User.AuthenticatedUserId = DistinctId;
                Client.Context.User.Id = DistinctId;
                Client.Context.Component.Version = SboAddon.Instance.Version;
            }
            catch (Exception e)
            {
                Logger.Warn(e);
            }

            TrackEvent("Launch");
        }

        public static void TrackEvent(string @event, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            try
            {
                Client?.TrackEvent(@event, properties, metrics);
            } catch (Exception) { }
        }

        public static void TrackPageView(string @url)
        {
            try
            {
                Client?.TrackPageView(@url);
            }
            catch (Exception) { }
        }

        public static void TrackException(Exception @exception, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            try
            {
                Client?.TrackException(@exception, properties, metrics);
            }
            catch (Exception) { }
        }
    }

    public class ExcludeLoggerFilter : ITelemetryProcessor
    {

        private ITelemetryProcessor Next { get; set; }

        // Link processors to each other in a chain.
        public ExcludeLoggerFilter(ITelemetryProcessor next)
        {
            this.Next = next;
        }
        public void Process(ITelemetry item)
        {
            if (((item as DependencyTelemetry)?.Data?.Contains("loggly.com/inputs")).GetValueOrDefault())
                return;

            this.Next.Process(item);
        }
    }
}