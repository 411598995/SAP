using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropAPI.SAP
{
    public interface IDataServices
    {
        string constr { get; set; }








        object getScallerValue(string strSql);
        long getMaxId(string tblName, string idCol);

        DataTable getDataTable(string strsql);
        DataTable getDataTable(string strsql, Hashtable pms);


        string ExecuteNonQuery(string strsql);

        string ExecuteNonQuery(string strsql, Hashtable sqP);

        string getConStatus();









    }
}
