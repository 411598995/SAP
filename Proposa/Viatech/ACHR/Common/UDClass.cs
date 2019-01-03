using System;

	
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
	
using System.Diagnostics;
using System.Threading;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Management;
using Microsoft.CSharp;
using System.Security.Cryptography;
using System.Text;

using System.Reflection;
using System.IO;
using SAPbobsCOM;
using SAPbouiCOM;
using System.Resources;

using System.Net;


namespace ACHR
{


    public class UDClass
    {
        long v_RetVal;
        int v_ErrCode;
        string v_ErrMsg = "";
        public SAPbobsCOM.Company oCompany;
        public SAPbobsCOM.Company oDiCompany;
        public  Hashtable settings = new Hashtable();
        string FileName;
        string defFileName;
        public Hashtable StringMessages = new Hashtable();
        public Hashtable DemModules = new Hashtable();
        public System.Data.DataTable LOVs = new System.Data.DataTable();
        public System.Data.DataTable AllLovs = new System.Data.DataTable();
        public SAPbouiCOM.Application oApplication;
        public string HRMSDbName = "";
        public string HRMSDbServer = "";
        public string HRMSDBuid = "";
        public string HRMSDbPwd = "";
        public string HRMSLicHash = "";
        public string hrConstr = "";
        public string HRMServerType = "";
        public bool CalculateTax = false;
        public bool isDIConnected = false;
        public string showReportCode = "";
        public bool isSystemReport = false;
        public string rptCritaria = "";
        private static System.Timers.Timer aTimer;
        public static bool busy = false;
        List<string> frmList = new List<string>();
        List<string> UDOList = new List<string>();

      
        public System.Data.DataTable dtRetScaned;
        public UDClass(string connectString)
        {
            string errmsg = "";
            try
            {
                SboGuiApi sboApi = new SboGuiApi();
                if (connectString == "")
                {
                    MessageBox.Show("Add-on must be run from SAP Business One.");
                }
                

                
                sboApi.Connect(connectString);
                 oApplication = sboApi.GetApplication();
                oCompany = new SAPbobsCOM.Company();
                string sCookies = oCompany.GetContextCookie();
                string conStr = oApplication.Company.GetConnectionContext(sCookies);

              
                int ret = oCompany.SetSboLoginContext(conStr);
                int ret2 = 0;//; oCompany.Connect();
                
              oCompany = (SAPbobsCOM.Company) oApplication.Company.GetDICompany();
                if (ret2 == 0)
                {

                   

                    oApplication.StatusBar.SetText("Addon Connected Successfully.!", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    oApplication.MenuEvent += new _IApplicationEvents_MenuEventEventHandler(oApplication_MenuEvent);
                    oApplication.AppEvent += new _IApplicationEvents_AppEventEventHandler(oApplication_appEvent);
                    oApplication.ItemEvent += new _IApplicationEvents_ItemEventEventHandler(oApplication_ItemEvent);
                    oApplication.FormDataEvent += new _IApplicationEvents_FormDataEventEventHandler(oApplication_FormDataEvent);
                    loadAddons();
                  //  execTimer();
                }

                else
                {
                     MessageBox.Show(oCompany.GetLastErrorDescription());
                     MessageBox.Show("UI Not connected");
                     Environment.Exit(0);
                        
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to connect company!" + ex.Message);

                Environment.Exit(0);
            }
            
        }


        public void execTimer()
        {
            aTimer = new System.Timers.Timer(1);
            aTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 1 * 60 * 1000;
            aTimer.Enabled = true;
        }
        public void loadAddons()
        {
            
                    string lang = oApplication.Language.ToString();
                    Program.sboLanguage = lang;
                    try
                    {
                        int langnum = Convert.ToInt16(oApplication.Language.ToString());
                        lang = "_" + lang;
                    }
                    catch
                    {

                    }
                  
                  

                    createConfigurationTables();
                    if (lang.Contains("English"))
                    {
                        lang = "ln_English";
                    }


                    Program.sboLanguage = lang;
                    loadMenu(lang);
                    Program.sboLanguage = lang;
                    frmList.Clear();

                    frmList = new List<string>();

                    frmList.Add("mnu_ModSetup");

                    frmList.Add("mnu_ProdList");

                    if (DemModules.ContainsKey("SubGrp"))
                    {

                        if (DemModules["SubGrp"].ToString() == "Y")
                        {
                            frmList.Add("134");
                            frmList.Add("63");
                            frmList.Add("150");
                        }
                    }

                    if (DemModules.ContainsKey("ProdCost"))
                    {

                        if (DemModules["ProdCost"].ToString() == "Y")
                        {
                            frmList.Add("65211");
                           
                        }
                    }
                    if (DemModules.ContainsKey("CRM"))
                    {

                        if (DemModules["CRM"].ToString() == "Y")
                        {

                            frmList.Add("mnu_SS");
                            frmList.Add("mnu_SO");
                            frmList.Add("mnu_TSS");
                            frmList.Add("mnu_GS");
                            frmList.Add("mnu_Setting");

                        }
                    }

                    if (DemModules.ContainsKey("MRPD"))
                    {

                        if (DemModules["MRPD"].ToString() == "Y")
                        {
                            frmList.Add("mnu_MRPD");

                        }
                    }


                    if (DemModules.ContainsKey("SaleOrder"))
                    {
                        //if (DemModules["SaleOrder"].ToString() == "Y")
                        //{
                            frmList.Add("139");
                            frmList.Add("1284");
                            frmList.Add("1286");

                        //}
                    }
            if (DemModules.ContainsKey("ABCommerce"))
            {
                if (DemModules["ABCommerce"].ToString() == "Y")
                {
                    frmList.Add("mnu_WO");
                    frmList.Add("mnu_ABGS");
                    UDOList.Add("B1_ABC_PAYMETHOD");

                }
            }

        }
        public void createConfigurationTables()
        {

            string result = "Creating Configuration Table";

            AddTable("B1_MODULES", "Demmex Modules", BoUTBTableType.bott_NoObject);
            AddColumns("@B1_MODULES", "Enabled", "Enabled", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_None);
            AddColumns("@B1_MODULES", "LicenseKey", "License Key", SAPbobsCOM.BoFieldTypes.db_Alpha, 100, SAPbobsCOM.BoFldSubTypes.st_None);

            AddTable("B1_SETTING", "B1 Settings", BoUTBTableType.bott_NoObjectAutoIncrement);
            AddColumns("@B1_SETTING", "Value", "Value", SAPbobsCOM.BoFieldTypes.db_Alpha, 254, SAPbobsCOM.BoFldSubTypes.st_None, "");


            addModulesRows();

            System.Data.DataTable dtModules = getDataTable("Select * from [@B1_MODULES] ", "Getting Modules");
            DemModules.Clear();
            foreach (DataRow dr in dtModules.Rows)
            {
                DemModules.Add(dr["Code"].ToString(), dr["U_Enabled"].ToString());
            }



            if (DemModules.ContainsKey("ProdCost"))
            {

                if (DemModules["ProdCost"].ToString() == "Y")
                {
                    createConfigurationTablesProdCost();
                }

            }

            if (DemModules.ContainsKey("SubGrp"))
            {

                if (DemModules["SubGrp"].ToString() == "Y")
                {
                    createConfigurationTablesSubGrp();
                }

            }

            if (DemModules.ContainsKey("ProdSeq"))
            {

                if (DemModules["ProdSeq"].ToString() == "Y")
                {

                    createConfigurationTableProdSeq();
                }

            }
            if (DemModules.ContainsKey("CRM"))
            {
                if (DemModules["CRM"].ToString() == "Y")
                {

                    createConfigurationTablesCRM();
                }
            }

            if (DemModules.ContainsKey("LaborReg"))
            {
                if (DemModules["LaborReg"].ToString() == "Y")
                {

                    createConfigurationTablesLaborReg();
                }
            }

            if (DemModules.ContainsKey("MRPD"))
            {
                if (DemModules["MRPD"].ToString() == "Y")
                {

                    createConfigurationTablesMRPD();
                }
            }

            if (DemModules.ContainsKey("SaleOrder"))
            {
                if (DemModules["SaleOrder"].ToString() == "Y")
                {

                    createConfigurationTablesSaleOrder();
                }
            }

            if (DemModules.ContainsKey("ABCommerce"))
            {
                if (DemModules["ABCommerce"].ToString() == "Y")
                {

                    createConfigurationTablesABCommerce();
                }
            }

        }
        private void addModulesRows()
        {
            int cntExist = Convert.ToInt32(getScallerValue("Select count(*) from [@B1_MODULES] where Code = 'SubGrp'"));
            if (cntExist == 0)
            {
                string strInsert = "Insert into [@B1_MODULES](Code,Name,U_Enabled,U_LicenseKey) values ('SubGrp','Sub Group Module','N','NA')";
                ExecQuery(strInsert, "ModuleRow");


            }


             cntExist = Convert.ToInt32(getScallerValue("Select count(*) from [@B1_MODULES] where Code = 'ABCommerce'"));
            if (cntExist == 0)
            {
                string strInsert = "Insert into [@B1_MODULES](Code,Name,U_Enabled,U_LicenseKey) values ('ABCommerce','AB Commerce Integration','N','NA')";
                ExecQuery(strInsert, "ModuleRow");


            }
           

       

        }
        public void loadSettings()
        {
            settings.Clear();
            string strSetting = "Select * from  \"@B1_SETTING\" ";
            System.Data.DataTable dtSettings = getDataTable(strSetting, "Getting Settings");
            foreach (DataRow dr in dtSettings.Rows)
            {
                try
                {
                    settings.Add(dr["Name"].ToString(), dr["U_Value"].ToString());
                }
                catch { }
            }
        }
        public string getSetting(string name)
        {
            string settingVal = "";
            if (settings.Contains(name))
            {
                settingVal = settings[name].ToString();
            }
            else
            {
                SaveSetting(name, "");
            }
            return settingVal;
        }
        public void SaveSetting(string name, string value)
        {

            string strInsert = "";
            strInsert = "IF  EXISTS(SELECT * FROM \"@B1_SETTING\"  WHERE \"Name\" ='" + name + "') UPDATE  \"@B1_SETTING\"   SET \"U_Value\" =  '" + value.Replace("'", "''") + "'  WHERE  \"Name\" =  '" + name + "';";
            ExecQuerySilent(strInsert, "Add Default");
            strInsert = "IF NOT EXISTS(SELECT * FROM \"@B1_SETTING\"  WHERE \"Name\" ='" + name + "') INSERT INTO \"@B1_SETTING\" ( \"Name\", \"U_Value\") VALUES ( '" + name.Replace("'", "''") + "', '" + value.Replace("'", "''") + "');";
            ExecQuerySilent(strInsert, "Add Default");


        }
        private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            if (!busy)
            {
                busy = true;
                busy = false;
            }
            //sendMessage();
        }

        public void loadMenu(string lang)
        {

            SAPbouiCOM.Menus mnus = oApplication.Menus;

            string strMenuFile = "";

            if (mnus.Exists("B1_ADV"))
            {
                mnus.RemoveEx("B1_ADV");
            }
            mnus.Item("43523").SubMenus.Add("B1_ADV", "Viatec Advance", BoMenuType.mt_POPUP, 12);
            mnus.Item("B1_ADV").SubMenus.Add("mnu_ModSetup", "Module Setup", BoMenuType.mt_STRING, 2);

            if (mnus.Exists("B1_STK"))
            {
                mnus.RemoveEx("B1_STK");
            }
            if (mnus.Exists("B1_ABC"))
            {
                mnus.RemoveEx("B1_ABC");
            }
            if (mnus.Exists("mnu_ProdList"))
            {
                mnus.RemoveEx("mnu_ProdList");
            }
            if (mnus.Exists("mnu_MRPD"))
            {
                mnus.RemoveEx("mnu_MRPD");
            }

            try
            {

                if (DemModules.ContainsKey("ABCommerce"))
                {
                    if (DemModules["ABCommerce"].ToString() == "Y")
                    {
                        strMenuFile = "ACHR.XMLScreen." + lang + ".PayrollMenu.xml";
                        LoadMenuFromXML(strMenuFile, "");
                        mnus.Item("B1_ABC").SubMenus.Add("mnu_ABGS", "General Setting", BoMenuType.mt_STRING, 1);
                        mnus.Item("B1_ABC").SubMenus.Add("B1_ABC_PAYMETHOD", "Payment Method Association", BoMenuType.mt_STRING, 2);


                        mnus.Item("B1_ABC").SubMenus.Add("mnu_WO", "Web Orders", BoMenuType.mt_STRING, 3);


                    }
                }

            }
            catch (Exception ex)
            {
                oApplication.MessageBox(ex.Message);
            }

            if (DemModules.ContainsKey("ProdSeq"))
            {
                if (DemModules["ProdSeq"].ToString() == "Y")
                {
                    mnus.Item("4352").SubMenus.Add("mnu_ProdList", "Production Sequencer", BoMenuType.mt_STRING, 2);
                }
            }

            if (DemModules.ContainsKey("MRPD"))
            {
                if (DemModules["MRPD"].ToString() == "Y")
                {
                    mnus.Item("43543").SubMenus.Add("mnu_MRPD", "Material Planning for Disassembly", BoMenuType.mt_STRING, 2);
                }
            }


        }



        public void createConfigurationTablesCRM()
        {

            string result = "Creating Configuration Table for CRM";

            AddTable("B1_CustRoute", "Customer Routes", BoUTBTableType.bott_NoObject);
            AddColumns("OCRD", "RouteMon", "Monday Route", SAPbobsCOM.BoFieldTypes.db_Alpha, 8, SAPbobsCOM.BoFldSubTypes.st_None, "B1_CustRoute");
            AddColumns("OCRD", "RouteTue", "Tuesday Route", SAPbobsCOM.BoFieldTypes.db_Alpha, 8, SAPbobsCOM.BoFldSubTypes.st_None, "B1_CustRoute");
            AddColumns("OCRD", "RouteWed", "Wednesday Route", SAPbobsCOM.BoFieldTypes.db_Alpha, 8, SAPbobsCOM.BoFldSubTypes.st_None, "B1_CustRoute");
            AddColumns("OCRD", "RouteThu", "Thursday Route", SAPbobsCOM.BoFieldTypes.db_Alpha, 8, SAPbobsCOM.BoFldSubTypes.st_None, "B1_CustRoute");
            AddColumns("OCRD", "RouteFri", "Friday Route", SAPbobsCOM.BoFieldTypes.db_Alpha, 8, SAPbobsCOM.BoFldSubTypes.st_None, "B1_CustRoute");
            AddColumns("OCRD", "RouteSat", "Saturday Route", SAPbobsCOM.BoFieldTypes.db_Alpha, 8, SAPbobsCOM.BoFldSubTypes.st_None, "B1_CustRoute");
            AddColumns("OCRD", "RouteSun", "Sunday Route", SAPbobsCOM.BoFieldTypes.db_Alpha, 8, SAPbobsCOM.BoFldSubTypes.st_None, "B1_CustRoute");

            AddColumns("ORDR", "SchID", "Scheduled ID", SAPbobsCOM.BoFieldTypes.db_Alpha, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");


            AddTable("B1_Config", "Sales Toolkit Configuration", BoUTBTableType.bott_NoObject);
            AddColumns("@B1_Config", "SchDays", "Sch Days", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_Config", "NLastOrdr", "N PrvOrders", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_Config", "NMSI", "Rows in MOP", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_Config", "NRP", "Rows in RP", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");

            AddColumns("@B1_Config", "NDTH", "NOD History", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_Config", "SED", "Short Exp DT", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");

            AddColumns("@B1_Config", "DBPATH", "Dashboard Path", SAPbobsCOM.BoFieldTypes.db_Alpha, 254, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_Config", "AlwPriceCh", "Allow to change price", SAPbobsCOM.BoFieldTypes.db_Alpha, 254, SAPbobsCOM.BoFldSubTypes.st_None, "");




            AddTable("B1_CRDSCH", "Customer Schedule", BoUTBTableType.bott_NoObject);
            AddColumns("@B1_CRDSCH", "SchType", "Schedule Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_CRDSCH", "Active", "Sale Active", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_CRDSCH", "slpCode", "Sale Agent", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");

            AddColumns("@B1_CRDSCH", "Intrvl", "Sale Interval", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_CRDSCH", "W1", "Mon", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_CRDSCH", "W2", "Tue", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_CRDSCH", "W3", "Wed", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_CRDSCH", "W4", "Thu", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_CRDSCH", "W5", "Fri", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_CRDSCH", "W6", "Sat", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_CRDSCH", "W7", "Sun", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_CRDSCH", "CallTime", "Call Time", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_CRDSCH", "MOType", "Sale Interval", SAPbobsCOM.BoFieldTypes.db_Alpha, 2, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_CRDSCH", "EWN", "Every N Week", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_CRDSCH", "EWD", "Start Date", SAPbobsCOM.BoFieldTypes.db_Date);




            AddTable("B1_SCHMDT", "Sch Dates", BoUTBTableType.bott_NoObject);
            AddColumns("@B1_SCHMDT", "SCCode", "Schedule Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_SCHMDT", "MDates", "Month Dates", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_SCHMDT", "upd", "Finalized", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");




            AddTable("B1_SCHMDY", "Sch Month Days", BoUTBTableType.bott_NoObject);
            AddColumns("@B1_SCHMDY", "SCCode", "Schedule Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_SCHMDY", "Day", "Week Day", SAPbobsCOM.BoFieldTypes.db_Alpha, 15, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_SCHMDY", "WeekNum", "Week Number", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_SCHMDY", "upd", "Finalized", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");


            AddTable("B1_SO", "Standing Orders Line", BoUTBTableType.bott_NoObject);
            AddColumns("@B1_SO", "SCCode", "Schedule Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_SO", "ItemCode", "Item Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_SO", "Qty", "Quantity", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_Quantity, "");
            AddColumns("@B1_SO", "upd", "Finalized", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");


            AddTable("B1_SCHOT", "One time Schedule", BoUTBTableType.bott_NoObject);
            AddColumns("@B1_SCHOT", "SCCode", "Schedule Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_SCHOT", "Date", "Date", SAPbobsCOM.BoFieldTypes.db_Date);
            AddColumns("@B1_SCHOT", "Time", "Time", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_SCHOT", "upd", "Finalized", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");


            AddTable("B1_SCHCALL", "Scheduled Call", BoUTBTableType.bott_NoObject);
            AddColumns("@B1_SCHCALL", "SCCode", "Schedule Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_SCHCALL", "CardCode", "Card Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_SCHCALL", "CardName", "Card Name", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_SCHCALL", "Route", "Route", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_SCHCALL", "DATE", "Call Date", SAPbobsCOM.BoFieldTypes.db_Date, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");

            AddColumns("@B1_SCHCALL", "Time", "Call Time", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_SCHCALL", "Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_SCHCALL", "DocEntry", "DocEntry", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_SCHCALL", "DocNum", "DocNum", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_SCHCALL", "Remarks", "Remarks", SAPbobsCOM.BoFieldTypes.db_Alpha, 254, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_SCHCALL", "RecType", "Record Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");


            AddColumns("OSCN", "CutSpec", "Cutting Specs", SAPbobsCOM.BoFieldTypes.db_Memo, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("OSCN", "PecSpec", "Cutting Specs", SAPbobsCOM.BoFieldTypes.db_Memo, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("OSCN", "CheSpec", "Cutting Specs", SAPbobsCOM.BoFieldTypes.db_Memo, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("OSCN", "Img", "Product Image", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Image, "");

            AddColumns("OSCN", "SL", "Shelf Life", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None, "");



        }


        public void createConfigurationTablesProdCost()
        {
            AddColumns("OWOR", "CostAP", "Cost AP Invoice", SAPbobsCOM.BoFieldTypes.db_Numeric, 8, SAPbobsCOM.BoFldSubTypes.st_None);
        }


        public void createConfigurationTablesSubGrp()
        {
            AddTable("B1_ITB", "Sub Groups", BoUTBTableType.bott_NoObject);
            AddColumns("@B1_ITB", "SubGrp", "Sub Group", SAPbobsCOM.BoFieldTypes.db_Alpha, 100, SAPbobsCOM.BoFldSubTypes.st_None);
            AddColumns("@B1_ITB", "Father", "Father Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 8, SAPbobsCOM.BoFldSubTypes.st_None);
            AddColumns("@B1_ITB", "Level", "Level", SAPbobsCOM.BoFieldTypes.db_Alpha, 8, SAPbobsCOM.BoFldSubTypes.st_None);
          
            AddColumns("OITM", "SubGrp", "Sub Group", SAPbobsCOM.BoFieldTypes.db_Alpha, 8, SAPbobsCOM.BoFldSubTypes.st_None, "B1_ITB");
            AddColumns("OITM", "SubGrp2", "Sub Group2", SAPbobsCOM.BoFieldTypes.db_Alpha, 8, SAPbobsCOM.BoFldSubTypes.st_None, "B1_ITB");

        }

        public void createConfigurationTablesABCommerce()
        {
            AddTable("B1_ABC_ORDR", "Web Orders", BoUTBTableType.bott_NoObject);
            AddColumns("@B1_ABC_ORDR", "OrderID", "OrderID", SAPbobsCOM.BoFieldTypes.db_Alpha, 100, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "OrdNo", "OrdNo", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "PONo", "PONo", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "StatID", "StatID", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "StatCode", "StatCode", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "Status", "Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "Date", "Date", SAPbobsCOM.BoFieldTypes.db_Date);
            AddColumns("@B1_ABC_ORDR", "CurrCode", "CurrCode", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "Curr", "Curr", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "NoItems", "NoItems", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "ItemsTotal", "ItemsTotal", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "Total", "Total", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "AmtPaid", "AmtPaid", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "Shipping", "Shipping", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "ShippingFee", "ShippingFee", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "Discount", "Discount", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "Email", "Email", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "DelType", "DelType", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "OutletCode", "OutletCode", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "Outlet", "Outlet", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "OutletID", "OutletID", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "ShipFirstName", "ShipFirstName", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "ShipLastName", "ShipLastName", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "ShipCompany", "ShipCompany", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "ShipAdd1", "ShipAdd1", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "ShipAdd2", "ShipAdd2", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "ShipAdd3", "ShipAdd3", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "ShipAdd4", "ShipAdd4", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "ShipPostCode", "ShipPostCode", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "ShipRegion", "ShipRegion", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "ShipRegionID", "ShipRegionID", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "ShipMobile", "ShipMobile", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "ShipPhoneDay", "ShipPhoneDay", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "ShipPhoneEve", "ShipPhoneEve", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "ShipEmail", "ShipEmail", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "BillFirstName", "BillFirstName", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "BillLastName", "BillLastName", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "BillCompany", "BillCompany", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "BillAdd1", "BillAdd1", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "BillAdd2", "BillAdd2", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "BillAdd3", "BillAdd3", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "BillAdd4", "BillAdd4", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "BillPostCode", "BillPostCode", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "BillRegion", "BillRegion", SAPbobsCOM.BoFieldTypes.db_Memo, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "BillMobile", "BillMobile", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "BillPhoneDay", "BillPhoneDay", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "BillPhoneEve", "BillPhoneEve", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "PaymentTypeID", "PaymentTypeID", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "PaymentType", "PaymentType", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "TrackRef", "TrackRef", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "StatusMsg", "StatusMsg", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "GiftMsg", "GiftMsg", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "ContID", "ContID", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "CustCode", "CustCode", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "CustCodeNo", "CustCodeNo", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "Company", "Company", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "ContType", "ContType", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "IsSpecial", "IsSpecial", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "PrimCustCode", "PrimCustCode", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "PrimCustCodeNo", "PrimCustCodeNo", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "PrimContID", "PrimContID", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "SUFirstName", "SUFirstName", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "SULastName", "SULastName", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "SUContID", "SUContID", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "Comments", "Comments", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "PayProvRef", "PayProvRef", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "SBOPosted", "Posted", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "SBOError", "Posting Error", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_ORDR", "SBOPostDT", "Posting Date Time", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");



            AddTable("B1_ABC_RDR1", "Web Orders Detail", BoUTBTableType.bott_NoObject);
            AddColumns("@B1_ABC_RDR1", "OrderID", "OrderID", SAPbobsCOM.BoFieldTypes.db_Alpha, 100, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_RDR1", "ItemID", "ItemID", SAPbobsCOM.BoFieldTypes.db_Alpha, 100, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_RDR1", "OrdNo", "OrdNo", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_RDR1", "Code", "Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_RDR1", "Title", "Title", SAPbobsCOM.BoFieldTypes.db_Alpha, 100, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_RDR1", "Price", "Price", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_RDR1", "PriceDisc", "PriceDisc", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_RDR1", "Qty", "Qty", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_RDR1", "Total", "Total", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_RDR1", "OrigPrice", "OrigPrice", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_RDR1", "PromText", "PromText", SAPbobsCOM.BoFieldTypes.db_Alpha, 250, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_RDR1", "Weight", "Weight", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");


            AddColumns("ORDR", "B1_ABC_WEBID", "AB Commerce ID", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("CRD1", "PhoneNum", "PhoneNum", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("CRD1", "EMail", "EMail", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");

            AddColumns("ORDR", "B1_ABC_DELIVERED", "Delivered Status Updated on WEB Order", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("ORDR", "B1_ABC_COMPLETED", "Completed Status Updated on WEB Order", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("ORDR", "B1_ABC_PAYMETHOD", "Payment Method", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("ORDR", "B1_ABC_PAYREF", "Payment Refference", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");

            AddColumns("ORCT", "B1_ABC_PAYMETHOD", "Payment Method", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("ORCT", "B1_ABC_PAYREF", "Payment Refference", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("ORCT", "B1_ABC_WEBNUM", "Web Order Number", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "");


            AddTable("B1_ABC_PAYMETHOD", "PAYMENT METHODS", BoUTBTableType.bott_NoObjectAutoIncrement);
            AddColumns("@B1_ABC_PAYMETHOD", "GL", "GL ACCOUNT", SAPbobsCOM.BoFieldTypes.db_Alpha, 100, SAPbobsCOM.BoFldSubTypes.st_None, "");
            AddColumns("@B1_ABC_PAYMETHOD", "CUR", "Doc Currency", SAPbobsCOM.BoFieldTypes.db_Alpha, 100, SAPbobsCOM.BoFldSubTypes.st_None, "");



        }

        public void createConfigurationTableProdSeq()
        {
            AddTable("PMX_OSPL", "Production Line", BoUTBTableType.bott_NoObject);
            AddTable("B1_Label", "Label", BoUTBTableType.bott_NoObject);

            AddColumns("OWOR", "PMX_PLCD", "Production Line", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "PMX_OSPL");
            AddColumns("OWOR", "B1_Label", "Label", SAPbobsCOM.BoFieldTypes.db_Alpha, 8, SAPbobsCOM.BoFldSubTypes.st_None, "B1_Label");
            AddColumns("OWOR", "B1_FrTxt", "Free Text", SAPbobsCOM.BoFieldTypes.db_Memo);
            AddColumns("OWOR", "B1_Seq", "Sequence", SAPbobsCOM.BoFieldTypes.db_Alpha, 8, SAPbobsCOM.BoFldSubTypes.st_None);
            AddColumns("OWOR", "B1_DispPos", "Display Position", SAPbobsCOM.BoFieldTypes.db_Numeric, 8);

            AddColumns("OITT", "B1_dfltPL", "Default Production Line", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None, "PMX_OSPL");


            AddTable("B1_KeyVal", "Label", BoUTBTableType.bott_NoObject);
            AddColumns("@B1_KeyVal", "Key", "Key", SAPbobsCOM.BoFieldTypes.db_Alpha, 8, SAPbobsCOM.BoFldSubTypes.st_None);
            AddColumns("@B1_KeyVal", "Val", "Value", SAPbobsCOM.BoFieldTypes.db_Alpha, 8, SAPbobsCOM.BoFldSubTypes.st_None);

            string strSqlRptId = "SELECT rptHash  FROM RDOC T0 WHERE T0.DocName = 'Production Line Sequence Report'";
            System.Data.DataTable dtRptId = getDataTable(strSqlRptId, "getting rpt id");
            if (dtRptId != null)
            {
                if (dtRptId.Rows.Count > 0)
                {
                   // printMenuId = dtRptId.Rows[0]["rptHash"].ToString();
                }
                else
                {
                    importRpt();


                }
            }

        }


        public void createConfigurationTablesLaborReg()
        {

            string result = "Creating Configuration Table";

            AddTable("CSPRODJOBTIME", "Production Time Log", BoUTBTableType.bott_NoObject);
            AddColumns("@CSPRODJOBTIME", "JOBID", "Job ID", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, SAPbobsCOM.BoFldSubTypes.st_None);
            AddColumns("@CSPRODJOBTIME", "TDate", "Time Stamp", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddColumns("@CSPRODJOBTIME", "TType", "Time Log Type", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddColumns("@CSPRODJOBTIME", "emp", "Employee", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddColumns("@CSPRODJOBTIME", "StDate", "Started On", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddColumns("@CSPRODJOBTIME", "FnDate", "Stopped On", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);




            AddColumns("OUSR", "B1_LAC", "Labor App Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddColumns("OWOR", "Status", "Production Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddColumns("WOR1", "Status", "Production Line Status", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddColumns("WOR1", "StDate", "Start On", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddColumns("WOR1", "FnDate", "Finished On", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddColumns("WOR1", "withEmp", "With Employee", SAPbobsCOM.BoFieldTypes.db_Alpha, 30);
            AddColumns("WOR1", "wrkHrs", "Consumed Hours", SAPbobsCOM.BoFieldTypes.db_Float, 8, BoFldSubTypes.st_Quantity);




        }

        public void createConfigurationTablesMRPD()
        {

            string result = "Creating Configuration Table";

            AddTable("DEM_AnimalType", "Animal Types", BoUTBTableType.bott_NoObject);
            AddColumns("@DEM_ANIMALTYPE", "isCat", "Is Category", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, BoFldSubTypes.st_None);
          
            AddColumns("OITM", "AnimalType", "Animal type", SAPbobsCOM.BoFieldTypes.db_Alpha, 30, BoFldSubTypes.st_None, "DEM_AnimalType");
            AddColumns("OITM", "MRPPer", "MR Percentage", SAPbobsCOM.BoFieldTypes.db_Float, 30, BoFldSubTypes.st_Percentage);
            AddColumns("OITM", "MRPPerF", "MR Percentage Future", SAPbobsCOM.BoFieldTypes.db_Float, 30, BoFldSubTypes.st_Percentage);
            AddColumns("OWOR", "PONum", "Supplier Purchase Order", SAPbobsCOM.BoFieldTypes.db_Numeric, 8,BoFldSubTypes.st_None);

            AddColumns("OITT", "Yield", "Yield %", SAPbobsCOM.BoFieldTypes.db_Float, 30, BoFldSubTypes.st_Percentage);



        }


        public void createConfigurationTablesSaleOrder()
        {

            string result = "Creating Configuration Table";

        }
    
        public virtual void oApplication_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {


            BubbleEvent = true;


            try
            {
                if (pVal.EventType == BoEventTypes.et_FORM_LOAD && pVal.Before_Action == false)
                {
                    if (frmList.Contains(pVal.FormTypeEx) )
                    {
                        try
                        {
                            Type oFormType = Type.GetType("ACHR.Screen." + "frm_" + pVal.FormTypeEx);
                            Screen.SysBaseForm objScr = ((Screen.SysBaseForm)oFormType.GetConstructor(System.Type.EmptyTypes).Invoke(null));
                            objScr.CreateForm(oApplication, oCompany, pVal.FormUID);
                            objScr.etFormAfterLoad(ref pVal, ref BubbleEvent);
                        }
                        catch { }
                    }

                }

            }
            catch (Exception ex)
            {
                oApplication.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Medium, true);
            }
        }
        private void fillDtFromTemplate(System.Data.DataTable dt , string pfileName )
        {
            string fileName = pfileName;
            using (StreamReader file = new StreamReader(fileName))
            {
                string line = "";
                string[] pastrts;
              
               line= file.ReadLine();
                pastrts = line.Split('\t');
                foreach (string colName in pastrts)
                {
                    dt.Columns.Add(colName);
                }
                while ("a" == "a")
                {
                    line = file.ReadLine();
                    if (line == null) break;
                    pastrts = line.Split('\t');
                    dt.Rows.Add(pastrts);
                    // dt.Rows.Add(pastrts(0), pastrts(1), pastrts(2), pastrts(3), pastrts(4), pastrts(5), pastrts(6), pastrts(7), pastrts(8), pastrts(9), pastrts(10), pastrts(11))
                }
            }
        }
       
    
        private void oApplication_appEvent(BoAppEventTypes EventType)
        {

            try
            {
                SAPbouiCOM.Menus mnus = oApplication.Menus;

            
                switch (EventType)
                {
                               case BoAppEventTypes.aet_CompanyChanged:
                       

                        if (mnus.Exists("B1_STK"))
                        {
                            mnus.RemoveEx("B1_STK");
                        }
                        System.Windows.Forms.Application.Exit();
                        break;
                    case BoAppEventTypes.aet_ShutDown:
                       
                        if (mnus.Exists("B1_STK"))
                        {
                            mnus.RemoveEx("B1_STK");
                        }
                        System.Windows.Forms.Application.Exit();
                        break;
                    case BoAppEventTypes.aet_LanguageChanged:
                        string lang = oApplication.Language.ToString();
                        Program.sboLanguage = lang;
                        try
                        {
                            int langnum = Convert.ToInt16(oApplication.Language.ToString());
                            lang = "_" + lang;
                        }
                        catch
                        {

                        }
                        loadMenu(lang);
                        break;


                }
            }
            catch (Exception ex)
            {
                oApplication.SetStatusBarMessage(ex.Message);
            }
        }
        public string getAcctName(string strAcctCode)
        {
            string strOut = "";

            SAPbobsCOM.Recordset oRecSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

            oRecSet.DoQuery("select acctname from oact where oact.acctcode='" + strAcctCode + "'");
            if (oRecSet.EoF)
            {
                strOut = "Not Found";
                return strOut;
            }
            strOut = oRecSet.Fields.Item("AcctName").Value.ToString();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecSet);
            oRecSet = null;
           

            return strOut;
        }
        public string getCardCode(string strCardCode)
        {
            string strOut = "";

            SAPbobsCOM.Recordset oRecSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

            oRecSet.DoQuery("select cardcode from ocrd where ocrd.cardcode='" + strCardCode + "'");
            if (oRecSet.EoF)
            {
                strOut = "Not Found";
                return strOut;
            }
            strOut = oRecSet.Fields.Item("CardCode").Value.ToString();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecSet);
            oRecSet = null;


            return strOut;
        }
        public string doSAPNonQuery(string strSql)
        {
            string result = "Ok";



            SAPbobsCOM.Recordset oRecSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

            try
            {
                oRecSet.DoQuery(strSql);
              
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            finally
            {

                System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecSet);
                oRecSet = null;
            }
            return result;
        }
        
        
      
        
        public string getStrMsg(string strKey)
        {
            string outStr = "Un-Known Message";

            try
            {
                outStr = StringMessages[strKey].ToString();
            }
            catch { }

            return outStr;
        }
        
        public void oApplication_FormDataEvent(ref BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
        
           
       

        }
        private void createCouponReport(SAPbouiCOM.Form couponForm )
        {
            
           


        }
       
        public void oApplication_MenuEvent(ref MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
           
            string mnuFrm = pVal.MenuUID;
       
            if (!frmList.Contains(pVal.MenuUID) && !UDOList.Contains(pVal.MenuUID))
            {
                return;
            }
           


            try
            {
                if (!pVal.BeforeAction)
                {
                    if (UDOList.Contains(pVal.MenuUID))
                    {
                        SAPbouiCOM.Menus mnus = oApplication.Menus.Item("51200").SubMenus;
                        foreach (SAPbouiCOM.MenuItem mnu in mnus)
                        {
                            string menuTitel = mnu.String;
                            if (menuTitel.Contains(pVal.MenuUID))
                            {


                                mnus.Item(mnu.UID).Activate();
                                return;

                            }
                        }
                        return;
                    }

                    if (pVal.MenuUID.Contains("mnu_"))
                    {
                        string strLang = oApplication.Language.ToString();

                        strLang = "ln_English";

                        string comName = pVal.MenuUID.Replace("mnu_", "");
                        try
                        {
                            oApplication.Forms.Item("frm_" + comName).Select();
                            oApplication.Forms.Item("frm_" + comName).Visible = true;
                        }
                        catch
                        {
                            Type oFormType = Type.GetType("ACHR.Screen." + "frm_" + comName);
                            Screen.HRMSBaseForm objScr = ((Screen.HRMSBaseForm)oFormType.GetConstructor(System.Type.EmptyTypes).Invoke(null));

                            List<string> checkValues = new List<string> { "EmpVL", "Conf" };

                            if (checkValues.Contains(comName))
                            {
                                objScr.CreateForm(oApplication, "ACHR.XMLScreen." + strLang + ".xml_" + comName + ".srf", oCompany, "frm_" + comName);
                            }

                            else
                            {
                                objScr.CreateForm(oApplication, "ACHR.XMLScreen." + strLang + ".xml_" + comName + ".xml", oCompany, "frm_" + comName);
                            }
                        }
                    }


                }
                else
                {
                    if (pVal.MenuUID.Contains("1284") || pVal.MenuUID.Contains("1286"))
                    {
                        if (oApplication.Forms.ActiveForm.TypeEx == "139")
                        {
                            SAPbouiCOM.EditText ORDRDocNum = (SAPbouiCOM.EditText)oApplication.Forms.ActiveForm.Items.Item("8").Specific;
                            string ordrNum = ORDRDocNum.Value.ToString();
                            string ordrEntry = null;
                            SAPbobsCOM.Recordset oRecSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                            oRecSet.DoQuery("Select DocEntry from ORDR where docnum='" + ordrNum + "'");
                            if (!oRecSet.EoF)
                            {
                                ordrEntry = Convert.ToString(oRecSet.Fields.Item("DocEntry").Value);
                                oRecSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                                oRecSet.DoQuery("Select PcName from DEM_TempInv where BaseType = 17 and BaseEntry = " + ordrEntry);
                                if (!oRecSet.EoF)
                                {
                                    int ithReturnValue;
                                    ithReturnValue = oApplication.MessageBox("The order has currently stock allocated. If you close the order the stock will go missing. Do you want to continue anyway?", 2, "Yes", "No");
                                    if (ithReturnValue == 1)
                                    {
                                        BubbleEvent = true;

                                    }
                                    else
                                    {
                                        BubbleEvent = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Unhandeled Exception Caught at General Class:" + ex.Message);
            }
        }

        public UDClass(ref SAPbobsCOM.Company comp, ref SAPbouiCOM.Application app)
        {
            oCompany = comp;
            oApplication = app;

        }
        private void importRpt()
        {
            ReportLayoutsService oLayoutService = (ReportLayoutsService)oCompany.GetCompanyService().GetBusinessService(SAPbobsCOM.ServiceTypes.ReportLayoutsService);

            ReportLayout oReport = (ReportLayout)oLayoutService.GetDataInterface(ReportLayoutsServiceDataInterfaces.rlsdiReportLayout);

            //Initialize critical properties

            //

            // Use TypeCode "RCRI" to specify a Crystal Report.

            // Use other TypeCode to specify a layout for a document type.

            // List of TypeCode types are in table RTYP.

            oReport.Name = "Production Line Sequence Report";
            oReport.TypeCode = "RCRI";

            oReport.Author = oCompany.UserName;
            oReport.Category = ReportLayoutCategoryEnum.rlcCrystal;

            string newReportCode;

            try
            {

                // Add new object

                ReportLayoutParams oNewReportParams = oLayoutService.AddReportLayoutToMenu(oReport, "43542");

                // Get code of the added ReportLayout object

                newReportCode = oNewReportParams.LayoutCode;

            }

            catch (System.Exception err)
            {

                string errMessage = err.Message;

                return;

            }

            // Wpload .rpt file using SetBlob interface

            string rptFilePath = @"ProductionLine.rpt";

            CompanyService oCompanyService = oCompany.GetCompanyService();

            // Specify the table and field to update

            BlobParams oBlobParams = (BlobParams)oCompanyService.GetDataInterface(SAPbobsCOM.CompanyServiceDataInterfaces.csdiBlobParams);

            oBlobParams.Table = "RDOC";

            oBlobParams.Field = "Template";


            // Specify the record whose blob field is to be set

            BlobTableKeySegment oKeySegment = oBlobParams.BlobTableKeySegments.Add();

            oKeySegment.Name = "DocCode";

            oKeySegment.Value = newReportCode;

            Blob oBlob = (Blob)oCompanyService.GetDataInterface(CompanyServiceDataInterfaces.csdiBlob);

            // Put the rpt file into buffer


            FileStream oFile = new FileStream(rptFilePath, System.IO.FileMode.Open);

            int fileSize = (int)oFile.Length;

            byte[] buf = new byte[fileSize];

            oFile.Read(buf, 0, fileSize);

            oFile.Close();

            // Convert memory buffer to Base64 string

            oBlob.Content = Convert.ToBase64String(buf, 0, fileSize);

            try
            {

                //Upload Blob to database

                oCompanyService.SetBlob(oBlobParams, oBlob);

            }

            catch (System.Exception ex)
            {

                string errmsg = ex.Message;

            }
        }

        public bool ColumnExists(string TableName, string FieldID)
        {
            bool oFlag = true;
            try
            {
                SAPbobsCOM.Recordset rsetField = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                string s = "Select 1 from [CUFD] Where TableID='" + TableName.Trim() + "' and AliasID='" + FieldID.Trim() + "'";
                rsetField.DoQuery("Select 1 from [CUFD] Where TableID='" + TableName.Trim() + "' and AliasID='" + FieldID.Trim() + "'");
                if (rsetField.EoF)
                    oFlag = false;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rsetField);
                rsetField = null;
                GC.Collect();
                return oFlag;
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed to Column Exists : " + ex.Message);
            }
            finally
            {
            }
            return oFlag;
        }
        public int ExecQuerySilent(string sql, string CallerRef)
        {
            int result = 0;
            SAPbobsCOM.Recordset rs = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {
                rs.DoQuery(sql);


            }
            catch (Exception ex)
            {
                //  oApplication.StatusBar.SetText("Failed in Exec Query on " + CallerRef + " : " + ex.Message);
                result = -1;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
                rs = null;

            }
            return result;
        }

        public int  ExecQuery(string sql, string CallerRef)
        {

            
            int result = 0;
            SAPbobsCOM.Recordset rs = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {
                rs.DoQuery(sql);
               

            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed in Exec Query on " + CallerRef + " : " + ex.Message);
                result = -1;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
                rs = null;

            }
            return result;
        }

        public System.Data.DataTable getDataTable(string sql, string CallerRef)
        {
            System.Data.DataTable dtOut = new System.Data.DataTable();
            SAPbobsCOM.Recordset rs = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {
                rs.DoQuery(sql);
               
                    for (int i=0;i<rs.Fields.Count;i++)
                    {
                        dtOut.Columns.Add(rs.Fields.Item(i).Description);
                    }
               

                while (!rs.EoF)
                {
                    DataRow nr = dtOut.NewRow();
                    for (int i = 0; i < rs.Fields.Count; i++)
                    {
                        nr[i] = rs.Fields.Item(i).Value;
                    }
                    dtOut.Rows.Add(nr);
                    rs.MoveNext();
                }

            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed in Exec Query on " + CallerRef + " : " + ex.Message);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
                rs = null;

            }
            return dtOut;
        }

        public string getScallerValue(string strSql)
        {
            string strResult = "";
            System.Data.DataTable dtScaller = getDataTable(strSql,"GetScaller of UDClass");

            if (dtScaller.Rows.Count > 0)
            {
                strResult = dtScaller.Rows[0][0].ToString();
            }

            return strResult;
        }
      
        public long getMaxId(string tblName, string idCol)
        {
            long nextId = 1;
            string strSql = " Select isnull(max(convert(int," + idCol + ")),0)  as nextId from " + tblName;
            try
            {
                nextId = Convert.ToInt32(getScallerValue(strSql)) +1;
            }
            catch { }
            return nextId;
        }
        public void ExecFileQuery(string filePath, string callerRef)
        {

            try
            {

                System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath);
                StreamReader reader = new StreamReader(stream);

                string strSql = reader.ReadToEnd();

                ExecQuery(strSql, callerRef);


            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed to execute pat" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            finally
            {
            }
        }

        public bool AddColumns(string TableName, string Name, string Description, SAPbobsCOM.BoFieldTypes Type, int Size = 0, SAPbobsCOM.BoFldSubTypes SubType = SAPbobsCOM.BoFldSubTypes.st_None, string LinkedTable = "", string[,] LOV = null, string DefV = "")
        {
            bool outResult = false;
            try
            {
                SAPbobsCOM.UserFieldsMD v_UserField = default(SAPbobsCOM.UserFieldsMD);

                if (TableName.StartsWith("@") == true)
                {
                    if (!ColumnExists(TableName, Name))
                    {
                        v_UserField = (SAPbobsCOM.UserFieldsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
                        v_UserField.TableName = TableName;
                        v_UserField.Name = Name;
                        if (!string.IsNullOrEmpty(DefV))
                        {
                            v_UserField.DefaultValue = DefV;
                        }

                        if (LOV == null)
                        {
                        }
                        else
                        {
                            for (int k = 0; k <= LOV.Length - 1; k++)
                            {
                                v_UserField.ValidValues.Value = LOV[k, 0];
                                v_UserField.ValidValues.Value = LOV[k, 1];
                                v_UserField.ValidValues.Add();
                            }

                        }

                        v_UserField.Description = Description;
                        v_UserField.Type = Type;
                        if (Type != SAPbobsCOM.BoFieldTypes.db_Date)
                        {
                            if (Size != 0)
                            {
                                v_UserField.Size = Convert.ToInt16(Size);
                                v_UserField.EditSize = Convert.ToInt16(Size);
                            }
                        }
                        if (SubType != SAPbobsCOM.BoFldSubTypes.st_None)
                        {
                            v_UserField.SubType = SubType;
                        }
                        if (!string.IsNullOrEmpty(LinkedTable))
                            v_UserField.LinkedTable = LinkedTable;
                        v_RetVal = v_UserField.Add();
                        if (v_RetVal != 0)
                        {
                            oCompany.GetLastError(out v_ErrCode, out v_ErrMsg);
                            oApplication.StatusBar.SetText("Failed to add UserField " + Description + " - " + v_ErrCode + " " + v_ErrMsg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            return false;
                        }
                        else
                        {
                            oApplication.StatusBar.SetText("[@" + TableName + "] - " + Description + " added successfully!!!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                            outResult = true;
                            return true;
                        }
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UserField);
                        v_UserField = null;
                    }
                    else
                    {
                        return false;
                    }
                }


                if (TableName.StartsWith("@") == false)
                {
                    if (!UDFExists(TableName, Name))
                    {
                        v_UserField = (SAPbobsCOM.UserFieldsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
                        v_UserField.TableName = TableName;
                        v_UserField.Name = Name;
                        if (!string.IsNullOrEmpty(DefV))
                        {
                            v_UserField.DefaultValue = DefV;
                        }

                        if (LOV == null)
                        {
                        }
                        else
                        {
                            for (int k = 0; k <= LOV.Length / 2 - 1; k++)
                            {
                                v_UserField.ValidValues.Value = LOV[k, 0];
                                v_UserField.ValidValues.Description = LOV[k, 1];
                                v_UserField.ValidValues.Add();
                            }

                        }
                        v_UserField.Description = Description;
                        v_UserField.Type = Type;
                        if (Type != SAPbobsCOM.BoFieldTypes.db_Date)
                        {
                            if (Size != 0)
                            {
                                v_UserField.Size = Size;
                                v_UserField.EditSize = Size;
                            }
                        }
                        if (SubType != SAPbobsCOM.BoFldSubTypes.st_None)
                        {
                            v_UserField.SubType = SubType;
                        }
                        if (!string.IsNullOrEmpty(LinkedTable))
                            v_UserField.LinkedTable = LinkedTable;
                        v_RetVal = v_UserField.Add();
                        if (v_RetVal != 0)
                        {
                            oCompany.GetLastError(out v_ErrCode, out v_ErrMsg);
                            oApplication.StatusBar.SetText("Failed to add UserField " + Description + " - " + v_ErrCode + " " + v_ErrMsg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            return false;
                        }
                        else
                        {
                            oApplication.StatusBar.SetText("[@" + TableName + "] - " + Description + " added successfully!!!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                            outResult = true;
                            return true;
                        }
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UserField);
                        v_UserField = null;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed to Add Columns : " + ex.Message);
            }
            finally
            {
            }
            return outResult;
        }
        
        public void AddXML(string pathstr)
        {
            try
            {
                System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
                System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(pathstr);
                System.IO.StreamReader streamreader = new System.IO.StreamReader(stream, true);
                xmldoc.LoadXml(streamreader.ReadToEnd());
                streamreader.Close();
                oApplication.LoadBatchActions(xmldoc.InnerXml);
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed to Load XML,AddXMl Method Failed" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            finally
            {
            }
        }
        
        public void CopyStream(ref Stream input, ref Stream output)
        {
            // Insert null checking here for production
            byte[] buffer = new byte[8193];
            int bytesRead = 1;
            while ((bytesRead > 0))
            {
                bytesRead = input.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    output.Write(buffer, 0, bytesRead);
                }

            }
        }
        
        public void DownloadEmbFile(string pathstr)
        {
            try
            {
                string strFileName = SaveFile(pathstr);


                if (!string.IsNullOrEmpty(strFileName))
                {

                    System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(pathstr);

                    byte[] buf = new byte[stream.Length + 1];
                    stream.Read(buf, 0, buf.Length);
                    File.WriteAllBytes(strFileName, buf);
                    oApplication.MessageBox("File saved successfully !");
                    //streamwriter.WriteLine(streamreader.ReadLine())
                }

            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed to Load XML,AddXMl Method Failed" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            finally
            {
            }
        }
        
        public bool UDOExists(string code)
        {
            bool outResult = false;
            try
            {
                SAPbobsCOM.UserObjectsMD v_UDOMD = default(SAPbobsCOM.UserObjectsMD);
                bool v_ReturnCode = false;

                GC.Collect();
                v_UDOMD = (SAPbobsCOM.UserObjectsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);
                v_ReturnCode = v_UDOMD.GetByKey(code);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UDOMD);
                v_UDOMD = null;
                outResult = v_ReturnCode;
                return v_ReturnCode;
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed to UDO Exists : " + ex.Message);
            }
            finally
            {
            }
            return outResult;
        }
        
        public bool registerUDO(string UDOCode, string UDOName, SAPbobsCOM.BoUDOObjType UDOType, string[,] findAliasNDescription, string parentTableName, string childTable1 = "", string childTable2 = "", string childTable3 = "", string childTable4 = "", SAPbobsCOM.BoYesNoEnum LogOption = SAPbobsCOM.BoYesNoEnum.tNO, string MenuId = "", int parrentId = 0)
        {
            bool functionReturnValue = false;

            try
            {
                bool actionSuccess = false;
                SAPbobsCOM.UserObjectsMD v_udoMD = default(SAPbobsCOM.UserObjectsMD);

                functionReturnValue = false;
                v_udoMD = (SAPbobsCOM.UserObjectsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);
                v_udoMD.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES;
                v_udoMD.CanClose = SAPbobsCOM.BoYesNoEnum.tYES;
                v_udoMD.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO;
                v_udoMD.CanDelete = SAPbobsCOM.BoYesNoEnum.tYES;
                v_udoMD.CanFind = SAPbobsCOM.BoYesNoEnum.tYES;
                v_udoMD.CanLog = SAPbobsCOM.BoYesNoEnum.tNO;
                v_udoMD.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tYES;
                v_udoMD.ManageSeries = SAPbobsCOM.BoYesNoEnum.tYES;
                if (!string.IsNullOrEmpty(MenuId))
                {
                    v_udoMD.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tYES;
                    //v_udoMD.RebuildEnhancedForm = SAPbobsCOM.BoYesNoEnum.tYES
                    v_udoMD.MenuItem = SAPbobsCOM.BoYesNoEnum.tYES;

                    v_udoMD.MenuUID = MenuId;
                    v_udoMD.MenuCaption = UDOName;
                    // v_udoMD.EnableEnhancedForm = SAPbobsCOM.BoYesNoEnum.tYES
                    v_udoMD.FatherMenuID = parrentId;
                    v_udoMD.Position = 2;
                }

                v_udoMD.Code = UDOCode;
                v_udoMD.Name = UDOName;
                v_udoMD.TableName = parentTableName;
                if (LogOption == SAPbobsCOM.BoYesNoEnum.tYES)
                {
                    v_udoMD.CanLog = SAPbobsCOM.BoYesNoEnum.tYES;
                    v_udoMD.LogTableName = "A" + parentTableName;
                }
                v_udoMD.ObjectType = UDOType;
                for (Int16 i = 0; i <= findAliasNDescription.GetLength(0) - 1; i++)
                {
                    if (i > 0)
                        v_udoMD.FindColumns.Add();
                    v_udoMD.FindColumns.ColumnAlias = findAliasNDescription[i, 0];
                    v_udoMD.FindColumns.ColumnDescription = findAliasNDescription[i, 1];
                }
                if (!string.IsNullOrEmpty(childTable1))
                {
                    v_udoMD.ChildTables.TableName = childTable1;
                    v_udoMD.ChildTables.Add();
                }
                if (!string.IsNullOrEmpty(childTable2))
                {
                    v_udoMD.ChildTables.TableName = childTable2;
                    v_udoMD.ChildTables.Add();
                }
                if (!string.IsNullOrEmpty(childTable3))
                {
                    v_udoMD.ChildTables.TableName = childTable3;
                    v_udoMD.ChildTables.Add();
                }
                if (!string.IsNullOrEmpty(childTable4))
                {
                    v_udoMD.ChildTables.TableName = childTable4;
                    v_udoMD.ChildTables.Add();
                }
                if (v_udoMD.Add() == 0)
                {
                    functionReturnValue = true;
                    oApplication.StatusBar.SetText("Successfully Registered UDO >" + UDOCode.ToString() + ">" + UDOName.ToString() + " >" + oCompany.GetLastErrorDescription(), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }
                else
                {
                    oApplication.StatusBar.SetText("Failed to Register UDO >" + UDOCode + ">" + UDOName + " >" + oCompany.GetLastErrorDescription(), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    functionReturnValue = false;
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(v_udoMD);
                v_udoMD = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed to UDO Register : " + ex.Message);
            }
            finally
            {
            }
            return functionReturnValue;
        }
        
        public bool TableExists(string TableName)
        {
            bool outResult = false;
            try
            {
                SAPbobsCOM.UserTablesMD oTables = default(SAPbobsCOM.UserTablesMD);
                bool oFlag = false;

                oTables = (SAPbobsCOM.UserTablesMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);
                oFlag = oTables.GetByKey(TableName);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oTables);
                outResult = oFlag;
                return oFlag;
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed to Table Exists : " + ex.Message);
            }
            finally
            {
            }
            return outResult;
        }
        
        public void addQryCategor(string catName)
        {
            try
            {
                SAPbobsCOM.QueryCategories qCat = default(SAPbobsCOM.QueryCategories);
                qCat = (SAPbobsCOM.QueryCategories)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oQueryCategories);
                qCat.Name = catName;
                qCat.Add();

            }
            catch { }


        }
        
        public int addQuery(string strQuery, string queryName)
        {
            int queryId = 0;
            int catId = 0;

            SAPbobsCOM.Recordset oRecSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            oRecSet.DoQuery("select CategoryId from OQCN where CatName='HRMS Payroll'");
            if (!oRecSet.EoF)
            {
                catId = Convert.ToInt32( oRecSet.Fields.Item("CategoryId").Value);


            }
            else
            {
                addQryCategor("HRMS Payroll");
                oRecSet.DoQuery("select CategoryId from OQCN where CatName='HRMS Payroll'");
                if (!oRecSet.EoF)
                {
                    catId = Convert.ToInt32( oRecSet.Fields.Item("CategoryId").Value);


                }
            }

            oRecSet.DoQuery("select intrnalKey as qId from ouqr where QName ='" + queryName + "'");
            if (!oRecSet.EoF)
            {
                queryId = Convert.ToInt32(oRecSet.Fields.Item("qId").Value);
            }
            else
            {
                oRecSet.DoQuery("select isnull(max(intrnalKey),0) +1 as newId from ouqr");
                queryId = Convert.ToInt32(oRecSet.Fields.Item("newId").Value);
                string sQuery = " insert into ouqr ([IntrnalKey] ,[QCategory] ,[QName] ,[QString] ,[QType] ) ";
                sQuery += " values ('" + queryId.ToString() + "','" + catId.ToString() + "','" + queryName + "','" + strQuery + "','W')";
                oRecSet.DoQuery(sQuery);
            }
            oRecSet = null;

            return queryId;
        }
        
        public void addFms(string frmId, string itmId, string colID, string query)
        {

            int queryId = 0;
            int fmsId = 0;

            SAPbobsCOM.Recordset oRecSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                oRecSet.DoQuery("select QueryId,IndexID from CSHS where formId='" + frmId + "' and ItemId='" + itmId + "' and colID='" + colID + "'");
                if (!oRecSet.EoF)
                {
                    queryId = Convert.ToInt32(oRecSet.Fields.Item("QueryId").Value);
                    fmsId = Convert.ToInt32(oRecSet.Fields.Item("IndexID").Value);
                    oRecSet.DoQuery("update OUQR set qString='" + query + "' where intrnalKey='" + queryId.ToString() + "'");

                }
                else
                {
                    oRecSet.DoQuery("select isnull(max(IndexID),0) +1 as fmsId from CSHS");
                    fmsId = Convert.ToInt32(oRecSet.Fields.Item("fmsId").Value);
                    queryId = addQuery(query, "Fms_" + frmId + "_" + itmId + "_" + colID);

                    string strS = "INSERT into [CSHS] ([FormID] ,[ItemID] ,[ColID] ,[ActionT] ,[QueryId] ,[IndexID] ,[Refresh]  ,[FrceRfrsh] ,[ByField]) ";
                    strS += " Values ('" + frmId + "','" + itmId + "','" + colID + "','2','" + queryId.ToString() + "','" + fmsId.ToString() + "','N','N','N')";
                    oRecSet.DoQuery(strS);
                }

                oRecSet = null;
            }
            catch (Exception ex)
            {
                oApplication.SetStatusBarMessage("Error in creating formatted search" + "Fms_" + frmId + "_" + itmId + "_" + colID + ex.Message);
            }

        }
        
        public bool AddTable(string TableName, string TableDescription, SAPbobsCOM.BoUTBTableType TableType)
        {
            bool outResult = false;
            try
            {

                SAPbobsCOM.UserTablesMD v_UserTableMD = default(SAPbobsCOM.UserTablesMD);

                GC.Collect();
                if (!TableExists(TableName))
                {
                    oApplication.StatusBar.SetText("Creating Table " + TableName + " ...................", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                    v_UserTableMD = (SAPbobsCOM.UserTablesMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);
                    v_UserTableMD.TableName = TableName;
                    v_UserTableMD.TableDescription = TableDescription;
                    v_UserTableMD.TableType = TableType;
                    v_RetVal = v_UserTableMD.Add();
                    if (v_RetVal != 0)
                    {
                        oCompany.GetLastError(out v_ErrCode, out  v_ErrMsg);
                        oApplication.StatusBar.SetText("Failed to Create Table " + TableName + v_ErrCode + " " + v_ErrMsg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UserTableMD);
                        v_UserTableMD = null;
                        GC.Collect();
                        return false;
                    }
                    else
                    {
                        oApplication.StatusBar.SetText("[@" + TableName + "] - " + TableDescription + " created successfully!!!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UserTableMD);
                        v_UserTableMD = null;
                        outResult = true;
                        GC.Collect();
                        return true;
                    }
                }
                else
                {
                    GC.Collect();
                    return false;
                }
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed to Add Table : " + ex.Message);
            }
            finally
            {
            }
            return outResult;
        }
        
        public bool UDFExists(string TableName, string FieldID)
        {
            bool outResult = false;
            try
            {
                SAPbobsCOM.Recordset rsetUDF = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                bool oFlag = true;

                rsetUDF.DoQuery("Select 1 from [CUFD] Where TableID='" + TableName.Trim() + "' and AliasID='" + FieldID.Trim() + "'");
                if (rsetUDF.EoF)
                    oFlag = false;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rsetUDF);
                rsetUDF = null;
                outResult = oFlag;
                GC.Collect();
                return oFlag;
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed to UDF Exisits : " + ex.Message);
            }
            finally
            {
            }
            return outResult;
        }
        
        public class WindowWrapper : System.Windows.Forms.IWin32Window
        {

            private IntPtr _hwnd;
            public WindowWrapper(IntPtr handle)
            {
                _hwnd = handle;
            }

            public System.IntPtr Handle
            {
                get { return _hwnd; }
            }

        }
        
        
        
        public string FindFile()
        {

            System.Threading.Thread ShowFolderBrowserThread = null;
            try
            {
                ShowFolderBrowserThread = new System.Threading.Thread(ShowFolderBrowser);
                if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Unstarted)
                {
                    ShowFolderBrowserThread.SetApartmentState(System.Threading.ApartmentState.STA);
                    ShowFolderBrowserThread.Start();
                    ShowFolderBrowserThread.Join();

                }
                else if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    ShowFolderBrowserThread.Start();
                    ShowFolderBrowserThread.Join();

                }
                Thread.Sleep(5000);
                while (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Running)
                {
                    System.Windows.Forms.Application.DoEvents();
                }


                if (!string.IsNullOrEmpty(FileName))
                {
                    return FileName;
                }
            }
            catch (Exception ex)
            {
                oApplication.MessageBox("FileFile" + ex.Message);
            }

            return "";

        }
        
        public string SaveFile(string defName)
        {

            defFileName = defName;
            System.Threading.Thread ShowFolderBrowserThread = null;
            try
            {
                ShowFolderBrowserThread = new System.Threading.Thread (SaveFileBrowser);

                if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Unstarted)
                {
                    ShowFolderBrowserThread.SetApartmentState(System.Threading.ApartmentState.STA);
                    ShowFolderBrowserThread.Start();
                }
                else if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    ShowFolderBrowserThread.Start();
                    ShowFolderBrowserThread.Join();

                }
                Thread.Sleep(5000);

                while (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Running)
                {
                    System.Windows.Forms.Application.DoEvents();
                }
                if (!string.IsNullOrEmpty(FileName))
                {
                    return FileName;
                }
            }
            catch (Exception ex)
            {
                oApplication.MessageBox("FileFile" + ex.Message);
            }

            return "";

        }
        
        public void ShowFolderBrowser()
        {
            System.Diagnostics.Process[] MyProcs = null;
            dynamic UserName = Environment.UserName;

            FileName = "";
            OpenFileDialog OpenFile = new OpenFileDialog();

            try
            {
                OpenFile.Multiselect = false;
                OpenFile.Filter = "All files(*.)|*.*";
                int filterindex = 0;
                try
                {
                    filterindex = 0;
                }
                catch (Exception ex)
                {
                }

                OpenFile.FilterIndex = filterindex;

                OpenFile.RestoreDirectory = true;
                MyProcs = System.Diagnostics.Process.GetProcessesByName("SAP Business One");

                for (int i = 0; i <= MyProcs.GetLength(0); i++)
                {

                    if (GetProcessUserName(MyProcs[i]) == UserName)
                    {
                        goto NEXT_STEP;
                    }
                }
                oApplication.MessageBox("Unable to determine Running processes by UserName!");
                OpenFile.Dispose();
                GC.Collect();
                return;
            NEXT_STEP:
                if (MyProcs.Length == 1)
                {

                    for (int i = 0; i <= MyProcs.Length - 1; i++)
                    {
                        WindowWrapper MyWindow = new WindowWrapper(MyProcs[i].MainWindowHandle);
                        DialogResult ret = OpenFile.ShowDialog(MyWindow);

                        if (ret == DialogResult.OK)
                        {
                            FileName = OpenFile.FileName;
                            OpenFile.Dispose();
                        }
                        else
                        {
                            System.Windows.Forms.Application.ExitThread();
                        }
                    }
                }
                else
                {
                    oApplication.MessageBox("More than 1 SAP B1 is started!");
                }
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText(ex.Message);
                FileName = "";
            }
            finally
            {
                OpenFile.Dispose();
                GC.Collect();
            }

        }
        
        public void SaveFileBrowser()
        {
            System.Diagnostics.Process[] MyProcs = null;
            dynamic UserName = Environment.UserName;

            FileName = "";
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.FileName = defFileName;
            try
            {
                MyProcs = System.Diagnostics.Process.GetProcessesByName("SAP Business One");

                for (int i = 0; i <= MyProcs.GetLength(1); i++)
                {
                    if (GetProcessUserName(MyProcs[i]) == UserName)
                    {
                        goto NEXT_STEP;
                    }
                }
                oApplication.MessageBox("Unable to determine Running processes by UserName!");
                saveFile.Dispose();
                GC.Collect();
                return;
            NEXT_STEP:
                if (MyProcs.Length == 1)
                {

                    for (int i = 0; i <= MyProcs.Length - 1; i++)
                    {
                        WindowWrapper MyWindow = new WindowWrapper(MyProcs[i].MainWindowHandle);
                        DialogResult ret = saveFile.ShowDialog(MyWindow);

                        if (ret == DialogResult.OK)
                        {
                            FileName = saveFile.FileName;
                            saveFile.Dispose();
                        }
                        else
                        {
                            System.Windows.Forms.Application.ExitThread();
                        }
                    }
                }
                else
                {
                    oApplication.MessageBox("More than 1 SAP B1 is started!");
                }
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText(ex.Message);
                FileName = "";
            }
            finally
            {
                saveFile.Dispose();
                GC.Collect();
            }

        }
        
        private string GetProcessUserName(System.Diagnostics.Process Process)
        {
            string strResult = "";
            ObjectQuery sq = new ObjectQuery("Select * from Win32_Process Where ProcessID = '" + Process.Id + "'");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(sq);


            if (searcher.Get().Count == 0)
                return null;

            foreach (ManagementObject oReturn in searcher.Get())
            {
                string[] o = new string[2];

                //Invoke the method and populate the o var with the user name and domain                         
                oReturn.InvokeMethod("GetOwner", (object[])o);
                strResult = o[0];
                return o[0];
            }
            return strResult;


        }
        
        private void LoadMenuFromXML(string FileName, string iconPath)
        {
            try
            {
                string sPath = null;
                System.Reflection.Assembly thisExe = null;
                thisExe = System.Reflection.Assembly.GetExecutingAssembly();
                System.IO.Stream file = thisExe.GetManifestResourceStream(FileName);
                string xml = null;

                // Using 
                System.IO.StreamReader sr = new System.IO.StreamReader(file);

                try
                {
                    xml = sr.ReadToEnd();

                }
                catch (Exception EX)
                {
                }
                finally
                {
                    ((IDisposable)sr).Dispose();
                }

                sPath = System.Windows.Forms.Application.StartupPath + "\\";
                xml = xml.Replace("Payroll.bmp", sPath + "Payroll.bmp");
                //'// load the form to the SBO application in one batch
                oApplication.LoadBatchActions(xml);
                sPath = oApplication.GetLastBatchResults();


            }
            catch (Exception ex)
            {
                oApplication.MessageBox(ex.Message + " " +  FileName);
            }
        }

        public  void ftpFile(string URL, string uid, string pwd, string Folder, string fileName)
        {
            // Get the object used to communicate with the server.



            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(URL + "/" + fileName);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(uid, pwd);

            // Copy the contents of the file to the request stream.
          //  StreamReader sourceStream = new StreamReader(Folder + "\\" + fileName);
            byte[] fileContents = System.IO.File.ReadAllBytes(Folder + "\\" + fileName);

           // byte[] fileContents = Encoding.ASCII.GetBytes(sourceStream.ReadToEnd());
           // sourceStream.Close();
            request.ContentLength = fileContents.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
          
            response.Close();
        }
        public string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        } 
        public string getFileName(string filter)
        {
            string fileName = "";
            using (ACHR.Common.GetFileNameClass oGetFileName = new ACHR.Common.GetFileNameClass())
            {
                if (filter != "")
                {
                    oGetFileName.Filter = filter;
                }
                oGetFileName.InitialDirectory = "c:";

                Thread threadGetFile = new Thread(new ThreadStart(oGetFileName.GetFileName));
                try
                {
                    threadGetFile.ApartmentState = ApartmentState.STA;

                    threadGetFile.Start();

                    Thread.Sleep(1000);                    // Wait a sec more
                    while (threadGetFile.IsAlive) ;
                    threadGetFile.Join();

                    // Use file name as you will here
                    if (oGetFileName.FileName != string.Empty)
                    {
                        fileName = oGetFileName.FileName;
                    }
                }
                catch (Exception ex)
                {
                    oApplication.MessageBox(ex.Message, 1, "OK", "", "");
                }
            }
            return fileName;

        }

        public string getStrTime(int intTime)
        {
            string outtime = "12:00 AM";
            if (intTime == 0) return outtime;
            try
            {
                string strAP = " AM";
                string hr = intTime.ToString().PadLeft(4,'0').Substring(0, 2);

                string mn = intTime.ToString().PadLeft(4, '0').Substring(2, 2);

                if (Convert.ToInt16(hr) > 12)
                {
                    hr = (Convert.ToInt16(hr) - 12).ToString();
                    strAP = " PM";
                }

                outtime = hr.PadLeft(2,'0') + ":" + mn.PadLeft(2,'0') + strAP;




            }
            catch { }

            return outtime;
        }


        public int getIntTime(string strTime)
        {
            int outtime = 0;

            try
            {
                string strAP = strTime.Substring(strTime.Length - 2);
                string strTP = strTime.Replace(strAP, "").Replace(":", "").Replace(" ", "").PadLeft(4, '0');
                int hr = Convert.ToInt16(strTP.Substring(0, 2));
                int mn = Convert.ToInt16(strTP.Substring(2, 2));

                if (strAP.ToUpper() == "PM") hr += 12;

                outtime = hr * 100 + mn;





            }
            catch { }

            return outtime;
        }
        public string getFileName2(string FileType)
        {
            string fileName = "";

            using (ACHR.Common.GetFileNameClass oGetFileName = new ACHR.Common.GetFileNameClass())
            {
                oGetFileName.Filter = FileType + " files (."+ FileType + ")|*." + FileType + "|All files (*.*)|*.*";
                oGetFileName.InitialDirectory = "c:";

                Thread threadGetFile = new Thread(new ThreadStart(oGetFileName.GetFileName));
                try
                {
                    threadGetFile.ApartmentState = ApartmentState.STA;

                    threadGetFile.Start();

                    Thread.Sleep(1000);                    // Wait a sec more
                    while (threadGetFile.IsAlive) ;
                    threadGetFile.Join();

                    // Use file name as you will here
                    if (oGetFileName.FileName != string.Empty)
                    {

                        fileName = oGetFileName.FileName;
                    }
                }
                catch (Exception ex)
                {
                    oApplication.MessageBox(ex.Message, 1, "OK", "", "");
                }
            }
            return fileName;

        }
        public string ExecQuery(string sql, Hashtable hp, string CallerRef)
        {


            string result = "OK";
            SAPbobsCOM.Recordset rs = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {
                foreach (string key in hp.Keys)
                {

                    try
                    {
                        sql = sql.Replace(key, "'" + hp[key].ToString().Replace("'", "''") + "'");

                    }
                    catch (Exception ex)
                    {
                        result = ex.Message;
                    }

                }


                rs.DoQuery(sql);


            }
            catch (Exception ex)
            {
                // oApplication.StatusBar.SetText("Failed in Exec Query on " + CallerRef + " : " + ex.Message);
                result = ex.Message;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
                rs = null;

            }
            return result;
        }





    }
}
