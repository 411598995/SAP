using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace PropAPI.SAP
{

    public class PMDIApi
    {
        public IDataServices DataService;
        public string dbType = "SQL";
        public string constr = "";
        public PMDIApi()
        {
            string constr = ConStr();
            if (constr.Contains("SERVERNODE"))
            {
                DataService = new DataServicesHANA(ConStr());
                dbType = "HANA";
            }
            else
            {

                DataService = new DataServicesSQL(ConStr());
            }

        }

        private string ConStr()
        {

            string ConStr = ConfigurationManager.ConnectionStrings["ConStr"].ToString();
            constr = ConStr;
            return ConStr;
        }

        

    }
}