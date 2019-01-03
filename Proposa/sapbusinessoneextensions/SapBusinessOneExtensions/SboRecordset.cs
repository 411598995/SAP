using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NLog;
using SAPbobsCOM;

namespace SapBusinessOneExtensions
{
    public sealed class SboEagerRecordset : List<Dictionary<string, dynamic>>, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Recordset _recordset;

        public SboEagerRecordset(string query)
        {
            Logger.Trace("Creating eager recordset for query: {0}", query);

            _recordset = (Recordset) SboAddon.Instance.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            _recordset.DoQuery(query);

            Logger.Trace("Fetched {0} records, copying to internal structure", _recordset.RecordCount);
            while (!_recordset.EoF)
            {
                var record = new Dictionary<string, dynamic>();
                foreach (Field field in _recordset.Fields)
                {
                    record[field.Name] = field.Value;
                }
                Add(record);

                _recordset.MoveNext();
            }
        }

        public void Dispose()
        {
            if (_recordset != null)
            {
                Logger.Trace("Disposing of eager recordset. Cleaning up COM objects and clearing internal data");
                Marshal.ReleaseComObject(_recordset);
                _recordset = null;
                Clear();

                GC.SuppressFinalize(this);
            }
        }
    }
}
