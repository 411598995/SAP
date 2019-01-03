using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Threading;
using System.Configuration;
using System.Security;

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

        public static DIClass SboAPI;
        public static DataServices dsLTD;
        public static DataServices dsINC;

        //NopCommerceSettings



        public static string strConNOP;
        public static string strConSAP;

        public static string LTDDBUID;
        public static string LTDDBPWD;
        public static string INCDB;
        public static string LTDDB;

        public static string strConWeb;
        public static string isSel;
        public static UDClass objHrmsUI;
        public static Encryption encriptor = new Encryption();
      
        [STAThread]
        static void Main()
        {
            
            Program.isSel = "N";
            string sConnectionString = "0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056";
            try
            {
                sConnectionString = Environment.GetCommandLineArgs().GetValue(1).ToString();
            }
            catch { }

            objHrmsUI = new UDClass(sConnectionString);
            objHrmsUI.loadSettings();
            LTDDBUID = Program.objHrmsUI.getSetting("UID");
            LTDDBPWD = "SAPB1Admin"; //Program.objHrmsUI.getSetting("PWD");
            if(objHrmsUI.oCompany.Server.ToUpper() == "UBAID-PC") LTDDBPWD = "super"; //Program.objHrmsUI.getSetting("PWD");

            INCDB = Program.objHrmsUI.getSetting("INCDB");
            LTDDB = Program.objHrmsUI.getSetting("LTDDB");

           string LTDConstr = "Data Source=" + objHrmsUI.oCompany.Server + ";Initial Catalog=" + LTDDB + ";Integrated Security=False;Persist Security Info=False;User ID=" + LTDDBUID + ";Password=" + LTDDBPWD + "";
           string INCConstr = "Data Source=" + objHrmsUI.oCompany.Server + ";Initial Catalog=" + INCDB + ";Integrated Security=False;Persist Security Info=False;User ID=" + LTDDBUID + ";Password=" + LTDDBPWD + "";

           dsLTD = new DataServices(LTDConstr);
            dsINC = new DataServices(INCConstr);

           System.Threading.Thread.CurrentThread.SetApartmentState( ApartmentState.STA);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run();
        }


        public static string GetMagiConnectLogin()
        {
            return System.Configuration.ConfigurationManager.AppSettings["MagiConnectLogin"].ToString();
        }

        public static string GetMagiConnectPassword()
        {
            return System.Configuration.ConfigurationManager.AppSettings["MagiConnectPassword"].ToString();
        }






    }
}
