using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SAPbobsCOM;

namespace SapBusinessOneExtensions
{
    public sealed class SboTransaction
    {
        public static void Start()
        {
            int start = 0;
            while (SboAddon.Instance.Company.InTransaction)
            {
                if (start++ >= 20)
                    throw new Exception("Could not get global transaction in 5 secs");

                Task.Delay(250).Wait();
            }

            SboAddon.Instance.Company.StartTransaction();
        }

        public static void Commit()
        {
            if (SboAddon.Instance.Company.InTransaction)
                SboAddon.Instance.Company.EndTransaction(BoWfTransOpt.wf_Commit);
        }

        public static void Rollback()
        {
            if (SboAddon.Instance.Company.InTransaction)
                SboAddon.Instance.Company.EndTransaction(BoWfTransOpt.wf_RollBack);
        }
    }
}
