using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapBusinessOneExtensions
{
    public static class SboVersion
    {
        public static bool IsVersion91OrMore()
        {
            return
                SboDiUtils.QueryValue<int>(SboAddon.Instance.Company.DbServerType.Equals(SAPbobsCOM.BoDataServerTypes.dst_HANADB) ?
                @"SELECT COUNT(*) FROM ""SYS"".""TABLE_COLUMNS"" WHERE ""SCHEMA_NAME"" = '{0}' AND ""TABLE_NAME"" = 'OCPR' AND ""COLUMN_NAME"" = 'EmlGrpCode'" :
                    "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'OCPR' AND COLUMN_NAME = 'EmlGrpCode'", SboAddon.Instance.Company.CompanyDB) > 0;
        }

        public static bool HasColumn(string table, string column)
        {
            return
                SboDiUtils.QueryValue<int>(SboAddon.Instance.Company.DbServerType.Equals(SAPbobsCOM.BoDataServerTypes.dst_HANADB) ?
                $@"SELECT COUNT(*) FROM ""SYS"".""TABLE_COLUMNS"" WHERE ""SCHEMA_NAME"" = '{SboAddon.Instance.Company.CompanyDB}' AND ""TABLE_NAME"" = '{table}' AND ""COLUMN_NAME"" = '{column}'" :
                    $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{table}' AND COLUMN_NAME = '{column}'") > 0;
        }
    }
}
