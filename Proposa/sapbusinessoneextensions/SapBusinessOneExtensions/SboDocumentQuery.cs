using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbobsCOM;

namespace SapBusinessOneExtensions
{
    public class SboDocumentQuery
    {
        public static Dictionary<string, object> DocumentHead(BoObjectTypes type, int docEntry, params string[] fields)
        {
            if (fields == null || fields.Length == 0)
                fields = new[] {"*"};
            else
            {
                var temp = fields.ToList();
                if (!temp.Contains("DocEntry", StringComparer.OrdinalIgnoreCase))
                    temp.Add("DocEntry");
                if (!temp.Contains("ObjType", StringComparer.OrdinalIgnoreCase))
                    temp.Add("ObjType");
                fields = temp.ToArray();
            }

            var query = string.Format(@"
SELECT {2}
FROM
	(SELECT {2} FROM OQUT WHERE ObjType = '{0}'
	 UNION ALL
     SELECT {2} FROM ORDR WHERE ObjType = '{0}'
	 UNION ALL
     SELECT {2} FROM ODLN WHERE ObjType = '{0}'
	 UNION ALL
     SELECT {2} FROM ORDN WHERE ObjType = '{0}'
	 UNION ALL
     SELECT {2} FROM OINV WHERE ObjType = '{0}'
	 UNION ALL
	 SELECT {2} FROM ODPI WHERE ObjType = '{0}'
	 UNION ALL
	 SELECT {2} FROM ORIN WHERE ObjType = '{0}'
	 UNION ALL
	 SELECT {2} FROM OPOR WHERE ObjType = '{0}'
) DOC
WHERE DOC.ObjType = '{0}' AND DOC.DocEntry = '{1}'
            ", (int) type, docEntry, String.Join(",", fields));

            return SboDiUtils.QueryList(query).FirstOrDefault();
        }

        public static Dictionary<string, object> DocumentCard(BoObjectTypes type, int docEntry, params string[] fields)
        {
            if (fields == null || fields.Length == 0)
                fields = new[] { "*" };

            var query = string.Format(@"
SELECT {2}
FROM
	(SELECT ObjType, DocEntry, CardCode FROM OQUT WHERE ObjType = '{0}'
	 UNION ALL
     SELECT ObjType, DocEntry, CardCode FROM ORDR WHERE ObjType = '{0}'
	 UNION ALL
     SELECT ObjType, DocEntry, CardCode FROM ODLN WHERE ObjType = '{0}'
	 UNION ALL
     SELECT ObjType, DocEntry, CardCode FROM ORDN WHERE ObjType = '{0}'
	 UNION ALL
     SELECT ObjType, DocEntry, CardCode FROM OINV WHERE ObjType = '{0}'
	 UNION ALL
	 SELECT ObjType, DocEntry, CardCode FROM ODPI WHERE ObjType = '{0}'
	 UNION ALL
	 SELECT ObjType, DocEntry, CardCode FROM ORIN WHERE ObjType = '{0}'
	 UNION ALL
	 SELECT ObjType, DocEntry, CardCode FROM OPOR WHERE ObjType = '{0}'
) DOC
INNER JOIN OCRD BP ON DOC.CardCode = BP.CardCode
WHERE DOC.ObjType = '{0}' AND DOC.DocEntry = '{1}'
            ", (int)type, docEntry, String.Join(",", fields.Select(f => "BP." + f)));

            return SboDiUtils.QueryList(query).FirstOrDefault();
        }

        public static Dictionary<string, object> DocumentContact(BoObjectTypes type, int docEntry, params string[] fields)
        {
            if (fields == null || fields.Length == 0)
                fields = new[] { "*" };

            var query = string.Format(@"
SELECT {2}
FROM
	(SELECT ObjType, DocEntry, CntctCode FROM OQUT WHERE ObjType = '{0}'
	 UNION ALL
     SELECT ObjType, DocEntry, CntctCode FROM ORDR WHERE ObjType = '{0}'
	 UNION ALL
     SELECT ObjType, DocEntry, CntctCode FROM ODLN WHERE ObjType = '{0}'
	 UNION ALL
     SELECT ObjType, DocEntry, CntctCode FROM ORDN WHERE ObjType = '{0}'
	 UNION ALL
     SELECT ObjType, DocEntry, CntctCode FROM OINV WHERE ObjType = '{0}'
	 UNION ALL
	 SELECT ObjType, DocEntry, CntctCode FROM ODPI WHERE ObjType = '{0}'
	 UNION ALL
	 SELECT ObjType, DocEntry, CntctCode FROM ORIN WHERE ObjType = '{0}'
	 UNION ALL
	 SELECT ObjType, DocEntry, CntctCode FROM OPOR WHERE ObjType = '{0}'
) DOC
INNER JOIN OCPR CPR ON DOC.CntctCode = CPR.CntctCode
WHERE DOC.ObjType = '{0}' AND DOC.DocEntry = '{1}'
            ", (int)type, docEntry, String.Join(",", fields.Select(f => "CPR." + f)));

            return SboDiUtils.QueryList(query).FirstOrDefault();
        }
    }
}
