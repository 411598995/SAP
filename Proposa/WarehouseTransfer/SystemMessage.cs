using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
namespace WarehouseTransfer
{
    class SystemMessage
    {
        #region Events
        public static void SystemMessage_ItemEvent(ref SAPbouiCOM.Application oApplication, ref SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            if (pVal.Before_Action)
            {
                if (pVal.ItemUID == "1")
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(clsStartup.strType) && clsStartup.strType == "133" && clsStartup.intFormCount > 0)
                        {
                            SAPbouiCOM.Form oInvForm = oApplication.Forms.GetForm(clsStartup.strType, clsStartup.intFormCount);
                            if (oInvForm != null)
                            {   
                                clsStartup.strType = string.Empty;
                                clsStartup.intFormCount = 0;
                            }
                        }
                        else if (!string.IsNullOrEmpty(clsStartup.strType) && clsStartup.strType == "60090" && clsStartup.intFormCount > 0)
                        {
                            SAPbouiCOM.Form oInvForm = oApplication.Forms.GetForm(clsStartup.strType, clsStartup.intFormCount);
                            if (oInvForm != null)
                            {
                                clsStartup.strType = string.Empty;
                                clsStartup.intFormCount = 0;
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (pVal.ItemUID == "2")
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(clsStartup.strType) && clsStartup.strType == "133" && clsStartup.intFormCount > 0)
                        {
                            SAPbouiCOM.Form oInvForm = oApplication.Forms.GetForm(clsStartup.strType, clsStartup.intFormCount);
                            if (oInvForm != null)
                            {
                                //ARInvoice.cancelInventoryTransfer(ref oApplication, ref oCompany, oInvForm);
                                clsStartup.strType = string.Empty;
                                clsStartup.intFormCount = 0;
                            }
                        }
                    }
                    catch (Exception)
                    {   
                        
                    }
                }
            }
            else
            {
                switch (pVal.EventType)
                {  
                    default:
                        break;
                }
            }
        }
        #endregion
    }
}
