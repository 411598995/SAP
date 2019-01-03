using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using NLog;
using SAPbouiCOM;
using Company = SAPbobsCOM.Company;

namespace SapBusinessOneExtensions
{
    public interface ISboAddon
    {
        string ConnectionString { get; }
        string Namespace { get; }
        string Name { get; }
        Application Application { get; }
        Company Company { get; }
        TaskScheduler UiTaskScheduler { get; }
        SboTaskScheduler TaskScheduler { get; }
        ObjectCache Cache { get; }
        DateTime? StartTime { get; }
        DateTime? LastEvent { get; set; }
        TimeSpan IdleTime { get; }
        SboAddonEvents Events { get; }
        ISboSettingsTableManager Settings { get; }
        ISboSettingsTableManager UserSettings { get; }
        string Version { get; }
    }

    public class SboAddon : ISboAddon
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger(); 

        private SboAddon(string nameSpace, string name)
        {
            Namespace = nameSpace;
            Name = name;
            UiTaskScheduler = new StaTaskScheduler(1);
            TaskScheduler = new SboTaskScheduler(TimeSpan.FromSeconds(30));
            Events = new SboAddonEvents();
            Cache = new MemoryCache(Name);
        }

        public static ISboAddon Instance { get; set; }
        public string ConnectionString { get; private set; }

        public string Namespace { get; }
        public string Name { get; }
        public Application Application { get; private set; }
        public Company Company { get; private set; }
        public TaskScheduler UiTaskScheduler { get; }
        public SboTaskScheduler TaskScheduler { get; }
        public ObjectCache Cache { get; }

        public DateTime? StartTime { get; } = DateTime.Now;
        public DateTime? LastEvent { get; set; }
        public TimeSpan IdleTime { get { return DateTime.Now - LastEvent.GetValueOrDefault(DateTime.Now);} }

        public SboAddonEvents Events { get; private set; }
        
        private SboSettingsTableManager _settings;
        public ISboSettingsTableManager Settings { get { return _settings ?? (_settings = new SboSettingsTableManager(Namespace + "_SETTINGS", Name, null, Cache)); } }

        private SboSettingsTableManager _userSettings;
        public ISboSettingsTableManager UserSettings { get { return _userSettings ?? (_userSettings = new SboSettingsTableManager(Namespace + "_SETTINGS", Name, Instance.Application.Company.UserName, Cache));  } }

        public string Version { get
        {
            try
            {
                    var versInfo = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
                    return versInfo.FileVersion;
            }
            catch (Exception e)
            {
                Logger.Warn(e, "Error fetching addon version for {0}/{1}", Namespace, Name);
                return null;
            }
        } }

        public static ISboAddon Create(string nameSpace, string name,SAPbouiCOM.Application inApplication , SAPbobsCOM.Company inCompany )
        {
            if (Instance != null)
                throw new Exception("Addon instance already exists");

            var _addon = new SboAddon(nameSpace, name);
            Instance = _addon;

            _addon.Initialize(inApplication,inCompany);
            _addon.Events.Initialize();

            _addon.SetUiLanguage();
            _addon.Application.AppEvent += type =>
                {
                    Logger.Info("Application event triggered: {0}", type);
                    switch (type)
                    {
                        case BoAppEventTypes.aet_ShutDown:
                            break;
                        case BoAppEventTypes.aet_CompanyChanged:
                            break;
                        case BoAppEventTypes.aet_LanguageChanged:
                            _addon.SetUiLanguage();
                            break;
                        case BoAppEventTypes.aet_ServerTerminition:
                            break;
                        case BoAppEventTypes.aet_FontChanged:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("type");
                    }
                };

            var testUiConnection = new Action(async () =>
                {
                    while (true)
                    {
                        try
                        {
                            var appId = Instance.Application.AppId;

                            await Task.Delay(TimeSpan.FromSeconds(15 + (appId * 0)));
                        }
                        catch (Exception)
                        {
                            Logger.Info("Lost connection to application. Exiting.");
                            Environment.Exit(1);
                        }
                    }
                });

            Task.Factory.StartNew(testUiConnection, CancellationToken.None, TaskCreationOptions.None, Instance.UiTaskScheduler);

            Instance.LastEvent = DateTime.Now;

            return Instance;
        }

        internal void Initialize( SAPbouiCOM.Application inApplication , SAPbobsCOM.Company inCompany)
        {
            Application = inApplication;
            Company = inCompany;
            
        }


        private void ConnectUi()
        {
            try
            {
                Logger.Info("Initializing UI API for SAP Business One Addon - {0}", Name);
                if (Environment.GetCommandLineArgs() == null || Environment.GetCommandLineArgs().Length < 2)
                    throw new ArgumentException("No connectionstring passed to executable");

                var link = new SboGuiApi();
                link.Connect(Environment.GetCommandLineArgs().ElementAt(1));

                Application = link.GetApplication();                

                Logger.Info("Addon {0} initialized and connected to SAP Business One UI API", Name);
            }
            catch (Exception e)
            {
                throw new Exception("Error connecting to UI API", e);
            }
        }

        private void ConnectDi()
        {
            try
            {
                Logger.Info("Initializing DI API for SAP Business One Addon - {0}", Name);

                if (Company == null)
                    Company = new Company();

                string cookie = Company.GetContextCookie();

                ConnectionString = Application.Company.GetConnectionContext(cookie);

                // ////////////////My Code

                int ret = Company.SetSboLoginContext(ConnectionString);
                int result = 0;//; oCompany.Connect();
                Company = (SAPbobsCOM.Company)Application.Company.GetDICompany();

                /////////////



                /// oldCode /////////////

                //if (Company.Connected)
                //    Company.Disconnect();

                //int result = Company.SetSboLoginContext(ConnectionString);
                //if (result != 0)
                //    throw new Exception(string.Format("Error setting login context: {0} - {1} - {2}", result, Company.GetLastErrorCode(), Company.GetLastErrorDescription()));

                //Logger.Info("Connecting to company database (type: {0}, server: {1}, licenseserver: {2}, database: {3})", Company.DbServerType, Company.Server, Company.LicenseServer, Company.CompanyDB);
                //result = Company.Connect();

                //// end of old code 



                if (result != 0 && !Company.Connected)
                {
                    throw new Exception(string.Format("Could not connect to company: {0} - {1} - {2}",
                        result,
                        Company.GetLastErrorCode(),
                        Company.GetLastErrorDescription()));
                }
                else
                {
                    Application.SetStatusBarMessage("Connected!",BoMessageTime.bmt_Short,false);
                }


                Logger.Info("Addon {0} (version {1}) initialized and connected to SAP Business One DI API version {2}.", Name, Version, Company.Version);
            }
            catch (Exception e)
            {
                throw new Exception("Error connecting to DI API", e);
            }
        }


        private void SetUiLanguage()
        {
            switch (Application.Language)
            {
                case BoLanguages.ln_Norwegian:
                    Logger.Info("Setting addon language to Norwegian Bokmål");
                    CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("nb");
                    break;
                case BoLanguages.ln_Swedish:
                    Logger.Info("Setting addon language to Swedish");
                    CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("sv");
                    break;
                default:
                    Logger.Info("Setting addon language to English");
                    CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en");
                    break;
            }
        }
    }
    
    public class SboAddonEvents
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger(); 

        internal _IApplicationEvents_ItemEventEventHandler ItemEventDelegate;
        public event _IApplicationEvents_ItemEventEventHandler ItemEvent
        {
            add { ItemEventDelegate += value; }
            remove { ItemEventDelegate -= value; }
        }

        internal _IApplicationEvents_FormDataEventEventHandler FormDataEventDelegate;
        public event _IApplicationEvents_FormDataEventEventHandler FormDataEvent
        {
            add { FormDataEventDelegate += value; }
            remove { FormDataEventDelegate -= value; }
        }

        internal _IApplicationEvents_MenuEventEventHandler MenuEventDelegate;
        public event _IApplicationEvents_MenuEventEventHandler MenuEvent
        {
            add { MenuEventDelegate += value; }
            remove { MenuEventDelegate -= value; }
        }

        internal _IApplicationEvents_ProgressBarEventEventHandler ProgressBarEventDelegate;
        public event _IApplicationEvents_ProgressBarEventEventHandler ProgressBarEvent
        {
            add { ProgressBarEventDelegate += value; }
            remove { ProgressBarEventDelegate -= value; }
        }

        internal _IApplicationEvents_RightClickEventEventHandler RightClickEventDelegate;
        public event _IApplicationEvents_RightClickEventEventHandler RightClickEvent
        {
            add { RightClickEventDelegate += value; }
            remove { RightClickEventDelegate -= value; }
        }

        internal _IApplicationEvents_AppEventEventHandler AppEventDelegate;
        public event _IApplicationEvents_AppEventEventHandler AppEvent
        {
            add { AppEventDelegate += value; }
            remove { AppEventDelegate -= value; }
        }


        public void Initialize()
        {
            SboAddon.Instance.Application.AppEvent += type => 
            {
                SboAddon.Instance.LastEvent = DateTime.Now;

                if (AppEventDelegate == null) return;
                var invocationList = AppEventDelegate.GetInvocationList();
                foreach (_IApplicationEvents_AppEventEventHandler i in invocationList)
                {
                    try
                    {
                        i.Invoke(type);
                    }
                    catch (Exception e)
                    {
                        Logger.Warn(e, "Unhandled exception occurred in application event handler");
                    }
                }
            };

            SboAddon.Instance.Application.ItemEvent += (string uid, ref ItemEvent val, out bool @event) =>
                {
                    @event = true;

                    SboAddon.Instance.LastEvent = DateTime.Now;

                    if (ItemEventDelegate == null) return;
                    var invocationList = ItemEventDelegate.GetInvocationList();
                    foreach (_IApplicationEvents_ItemEventEventHandler i in invocationList)
                    {
                        try
                        {
                            i.Invoke(uid, ref val, out @event);
                            if (@event == false)
                                return;
                        }
                        catch (Exception e)
                        {
                            Logger.Warn(e, "Unhandled exception occurred in item event handler");
                        }
                    }
                };

            SboAddon.Instance.Application.MenuEvent += (ref MenuEvent val, out bool @event) =>
                {
                    @event = true;

                    SboAddon.Instance.LastEvent = DateTime.Now;

                    if (MenuEventDelegate == null) return;
                    var invocationList = MenuEventDelegate.GetInvocationList();
                    foreach (_IApplicationEvents_MenuEventEventHandler i in invocationList)
                    {
                        try
                        {
                            i.Invoke(ref val, out @event);
                            if (@event == false)
                                return;
                        }
                        catch (Exception e)
                        {
                            Logger.Warn(e, "Unhandled exception occurred in menu event handler");
                        }
                    }
                };

            SboAddon.Instance.Application.FormDataEvent += (ref BusinessObjectInfo info, out bool @event) => 
            {
                @event = true;

                SboAddon.Instance.LastEvent = DateTime.Now;

                if (FormDataEventDelegate == null) return;
                var invocationList = FormDataEventDelegate.GetInvocationList();
                foreach (_IApplicationEvents_FormDataEventEventHandler i in invocationList)
                {
                    try
                    {
                        i.Invoke(ref info, out @event);
                        if (@event == false)
                            return;
                    }
                    catch (Exception e)
                    {
                        Logger.Warn(e, "Unhandled exception occurred in form data event handler");
                    }
                }
            };

            SboAddon.Instance.Application.ProgressBarEvent += (ref ProgressBarEvent val, out bool @event) => 
            {
                @event = true;

                SboAddon.Instance.LastEvent = DateTime.Now;

                if (ProgressBarEventDelegate == null) return;
                var invocationList = ProgressBarEventDelegate.GetInvocationList();
                foreach (_IApplicationEvents_ProgressBarEventEventHandler i in invocationList)
                {
                    try
                    {
                        i.Invoke(ref val, out @event);
                        if (@event == false)
                            return;
                    }
                    catch (Exception e)
                    {
                        Logger.Warn(e, "Unhandled exception occurred in progress bar event handler");
                    }
                }
            };

            SboAddon.Instance.Application.RightClickEvent += (ref ContextMenuInfo info, out bool @event) =>
            {
                @event = true;

                SboAddon.Instance.LastEvent = DateTime.Now;

                if (RightClickEventDelegate == null) return;
                var invocationList = RightClickEventDelegate.GetInvocationList();
                foreach (_IApplicationEvents_RightClickEventEventHandler i in invocationList)
                {
                    try
                    {
                        i.Invoke(ref info, out @event);
                        if (@event == false)
                            return;
                    }
                    catch (Exception e)
                    {
                        Logger.Warn(e, "Unhandled exception occurred in right click event handler");
                    }
                }
            };
        }
    }
}