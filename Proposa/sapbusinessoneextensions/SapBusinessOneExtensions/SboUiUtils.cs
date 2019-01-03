using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbouiCOM;

namespace SapBusinessOneExtensions
{
    public class SboUiUtils
    {
        public static Form WaitForFormActivation(string formType, long waitMillis)
        {
            long startMillis = DateTime.Now.Ticks / 10000;
            long currentMillis = startMillis;

            while (!SboAddon.Instance.Application.Forms.ActiveForm.TypeEx.Equals(formType) && currentMillis != startMillis + waitMillis)
                System.Threading.Thread.Sleep((int)waitMillis / 10);

            if (SboAddon.Instance.Application.Forms.ActiveForm.TypeEx.Equals(formType))
                return SboAddon.Instance.Application.Forms.ActiveForm;

            return null;
        }
    }
}
