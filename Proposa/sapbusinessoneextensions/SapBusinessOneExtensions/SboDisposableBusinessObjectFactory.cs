using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NLog;
using SAPbobsCOM;
using SAPbouiCOM;

namespace SapBusinessOneExtensions
{
    public sealed class SboDisposableBusinessObjectFactory : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private List<object> _businessObjects = new List<object>();
        private Dictionary<Enum, object> _singleBusinessObjects = new Dictionary<Enum, object>();

        public T CreateSingle<T>(BoObjectTypes objectType)
        {
            if (!_singleBusinessObjects.ContainsKey(objectType))
                _singleBusinessObjects[objectType] = (T) SboAddon.Instance.Company.GetBusinessObject(objectType);

            return (T) _singleBusinessObjects[objectType];
        }

        public T Create<T>(BoObjectTypes objectType)
        {
            var bo = (T)SboAddon.Instance.Company.GetBusinessObject(objectType);
            _businessObjects.Add(bo);
            return bo;
        }

        public T Create<T>(BoCreatableObjectType objectType)
        {
            var bo = (T)SboAddon.Instance.Application.CreateObject(objectType);
            _businessObjects.Add(bo);
            return bo;
        }

        public CompanyService GetCompanyService()
        {
            return SboAddon.Instance.Company.GetCompanyService();
        }

        public void Dispose()
        {
            if (_businessObjects != null)
            {
                foreach (var businessObject in _businessObjects)
                {
                    try
                    {
                        Marshal.ReleaseComObject(businessObject);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, $"Error releasing COM businessobject {businessObject}");
                    }
                }
                _businessObjects.Clear();
                _businessObjects = null;

                GC.SuppressFinalize(this);
            }
            if (_singleBusinessObjects != null)
            {
                foreach (var businessObject in _singleBusinessObjects)
                {
                    try
                    {
                        Marshal.ReleaseComObject(businessObject.Value);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, $"Error releasing singleton COM businessobject {businessObject}");
                    }
                }
                _singleBusinessObjects.Clear();
                _singleBusinessObjects = null;

                GC.SuppressFinalize(this);
            }
        }

        ~SboDisposableBusinessObjectFactory()
        {
            Dispose();
        }
    }
}
