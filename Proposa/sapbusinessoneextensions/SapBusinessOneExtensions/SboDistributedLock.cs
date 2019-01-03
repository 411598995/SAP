using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace SapBusinessOneExtensions
{
    class SboDistributedLock
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger(); 

        public static int GetLock(string resource, string mode = "Exclusive", string owner = "Transaction", TimeSpan? timeout = null)
        {
            if (String.IsNullOrWhiteSpace(resource))
                throw new ArgumentException();
            if (owner.Equals("Transaction") && !SboAddon.Instance.Company.InTransaction)
                throw new Exception("Cannot get lock with owner transaction without a running transaction");
            if (SboAddon.Instance.Company.DbServerType.Equals(SAPbobsCOM.BoDataServerTypes.dst_HANADB))
            {
                // TODO: Ugly hack for HANA - fix soon
                Task.Delay(TimeSpan.FromMilliseconds(new Random().NextDouble()*2500d)).Wait();
                return 0;
            }

            try
            {
                var result =
                    SboDiUtils.QueryValue<int>(
                        "DECLARE @res INT; EXEC @res = sp_getapplock @Resource = '{0}', @LockMode = '{1}', @LockOwner = '{2}', @LockTimeout = {3}; SELECT @res",
                        resource,
                        mode,
                        owner,
                        (int) (timeout ?? TimeSpan.Zero).TotalMilliseconds);

                return result;
            }
            catch (Exception)
            {
                Logger.Warn("Error requesting distributed lock for resource {0}, mode {1}, owner {2} and timeout {3}",
                    resource,
                    mode,
                    owner,
                    timeout);

                return -999;
            }
        }
    }
}
