using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Security.Cryptography;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Configuration;


namespace ACHR
{
    static class Program
    {
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        public static string companyDb;
        public static string SboUID;
        public static string SboPwd;
        public static string DbUserName;
        public static string DbPassword;
        public static string ServerType;
        public static string SboServer;
        public static string sboLanguage;
        public static DateTime StartTime;
        public static DateTime EndTime;

        public static string productImageFolder;
        public static string categorImageFolder;
        public static string standardPricelist;
        public static string whsCode;

        
        public static DIClass SboAPI;

        //NopCommerceSettings

        public static string NopDbUserName;
        public static string NopDbPassword;
        public static string NopDbName;
        public static string NopSServer;

        public static string strConNOP;
        public static string strConSAP;
        public static string strConMena;

        public static UDClass objHrmsUI;

        [STAThread]
        static void Main()
        {
            string sConnectionString = "0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056";
            try
            {
                sConnectionString = Environment.GetCommandLineArgs().GetValue(1).ToString();
            }
            catch { }
           
            objHrmsUI = new UDClass(sConnectionString);

            readSettings();

            strConMena = "Data Source=" + Program.NopSServer + ";Initial Catalog=" + Program.NopDbName + ";Integrated Security=False;Persist Security Info=False;User ID=" + Program.NopDbUserName + ";Password=" + Program.NopDbPassword + "";
            strConSAP = "Data Source=" + Program.SboServer + ";Initial Catalog=" + Program.companyDb + ";Integrated Security=False;Persist Security Info=False;User ID=" + Program.DbUserName + ";Password=" + Program.DbPassword + "";
            SboAPI = new DIClass(companyDb, SboUID, SboPwd, DbUserName, DbPassword, ServerType, SboServer);
           

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run();
        }
        
        
        public static void SetReport(ReportDocument rep)
        {
            // rep.SetDatabaseLogon(

            rep.SetDatabaseLogon(objHrmsUI.HRMSDBuid, objHrmsUI.HRMSDbPwd, objHrmsUI.HRMSDbServer, objHrmsUI.HRMSDbName, true);

            foreach (CrystalDecisions.CrystalReports.Engine.Table Table in rep.Database.Tables)
            {
                CrystalDecisions.Shared.TableLogOnInfo Logon;
                Logon = Table.LogOnInfo;
                Logon.ConnectionInfo.DatabaseName = objHrmsUI.HRMSDbName;
                Logon.ConnectionInfo.ServerName = objHrmsUI.HRMSDbServer;
                Logon.ConnectionInfo.Password = Program.objHrmsUI.HRMSDbPwd;
                Logon.ConnectionInfo.UserID = Program.objHrmsUI.HRMSDBuid;
                Table.ApplyLogOnInfo(Logon);

            }



            foreach (ReportDocument rpt in rep.Subreports )
            {
                rpt.SetDatabaseLogon(objHrmsUI.HRMSDBuid, objHrmsUI.HRMSDbPwd, objHrmsUI.HRMSDbServer, objHrmsUI.HRMSDbName, true);

                foreach (CrystalDecisions.CrystalReports.Engine.Table Table in rpt.Database.Tables)
                {

                    CrystalDecisions.Shared.TableLogOnInfo Logon;
                    Logon = Table.LogOnInfo;
                    Logon.ConnectionInfo.DatabaseName = objHrmsUI.HRMSDbName;
                    Logon.ConnectionInfo.ServerName = objHrmsUI.HRMSDbServer;
                    Logon.ConnectionInfo.Password = Program.objHrmsUI.HRMSDbPwd;
                    Logon.ConnectionInfo.UserID = Program.objHrmsUI.HRMSDBuid;
                    Table.ApplyLogOnInfo(Logon);


                }

            }
           
        }

        public static void readSettings()
        {
            DataTable dtConf = objHrmsUI.getDataTable("Select * from \"@MENA_CONFIG\"", "Reading Settings");

            if (dtConf.Rows.Count > 0)
            {

                NopSServer = dtConf.Rows[0]["U_server"].ToString();
                NopDbName = dtConf.Rows[0]["U_db"].ToString();
                NopDbUserName = dtConf.Rows[0]["U_uid"].ToString();
                NopDbPassword = dtConf.Rows[0]["U_pwd"].ToString();
              


            }
            else
            {

              
            }

            strConMena = "Data Source=" + Program.NopSServer + ";Initial Catalog=" + Program.NopDbName + ";Integrated Security=False;Persist Security Info=False;User ID=" + Program.NopDbUserName + ";Password=" + Program.NopDbPassword + "";
               

        }
        public static void updateSetting()
        {

          
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configuration.AppSettings.Settings["SboConstr"] != null)
            {
                strConMena = "Data Source=" + Program.NopSServer + ";Initial Catalog=" + Program.NopDbName + ";Integrated Security=False;Persist Security Info=False;User ID=" + Program.NopDbUserName + ";Password=" + Program.NopDbPassword + "";
                strConSAP = "Data Source=" + Program.SboServer + ";Initial Catalog=" + Program.companyDb + ";Integrated Security=False;Persist Security Info=False;User ID=" + Program.DbUserName + ";Password=" + Program.DbPassword + "";

                configuration.AppSettings.Settings["SboConstr"].Value = strConSAP;
                configuration.AppSettings.Settings["NopConstr"].Value = strConNOP;

            }

            configuration.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("appSettings");
        }

    }
}
