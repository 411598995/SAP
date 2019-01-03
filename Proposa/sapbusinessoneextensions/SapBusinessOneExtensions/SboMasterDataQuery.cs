using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapBusinessOneExtensions
{
    public class SboMasterDataQuery
    {
        public static Dictionary<string, object> AdminInfo()
        {
            var query = "SELECT * FROM OADM ORDER BY UpdateDate DESC";

            return SboDiUtils.QueryList(query).FirstOrDefault();
        }
        public static Dictionary<string, object> AdminInfo1()
        {
            var query = "SELECT ADM1.* FROM OADM INNER JOIN ADM1 ON OADM.Code = ADM1.Code ORDER BY OADM.UpdateDate DESC";

            return SboDiUtils.QueryList(query).FirstOrDefault();
        } 
    }
}
