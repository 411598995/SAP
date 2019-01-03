using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbobsCOM;
using System.Collections;
using SAPbouiCOM;
using System.Windows.Forms;
using System.Data;
using Utilities;
using System.IO;
using System.Text.RegularExpressions;

namespace WarehouseTransfer.Purchase
{
    class clsPO
    {

        public static SAPbouiCOM.Form oForm;
        public static SAPbouiCOM.Form oWaitForm;
        private static SAPbobsCOM.Recordset oRecordSet = null;
        private static SAPbobsCOM.Recordset oRecordSet1 = null;
        private static SAPbouiCOM.DBDataSource oHDBDataSource;
        private static SAPbouiCOM.DBDataSource oChildDataSource;
        private static SAPbouiCOM.Matrix oMatrix;
        private static string strDocEntry = "";
        private static string strDocType = "";

        #region "ItemEvent"
        public static void clsPO_ItemEvent(ref SAPbouiCOM.Application oApplication, ref SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oSetupForm, ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {

            oForm = oSetupForm;
            oHDBDataSource = oForm.DataSources.DBDataSources.Item("OPOR");
            oChildDataSource = oForm.DataSources.DBDataSources.Item("POR1");
            oMatrix = (SAPbouiCOM.Matrix)oForm.Items.Item("38").Specific;

            if (pVal.BeforeAction)
            {
                switch (pVal.EventType)
                {
                                     
                    case SAPbouiCOM.BoEventTypes.et_CLICK:
                        if (pVal.ItemUID == "38" && (pVal.ColUID == "U_Variance" || pVal.ColUID == "U_DiscExc"))
                        {
                            BubbleEvent = false;
                        }
                        if (oForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE || oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE)
                        {
                            if (pVal.ItemUID == "U_DocType")
                            {
                                BubbleEvent = false;
                            }
                        }
                        break;
                    case SAPbouiCOM.BoEventTypes.et_PICKER_CLICKED:
                        if (pVal.ItemUID == "38" && pVal.ColUID == "U_Variance")
                        {
                            BubbleEvent = false;
                        }
                        break;
                    case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED:
                        if (pVal.ItemUID == "btnSH")
                        {
                            if (((SAPbouiCOM.EditText)oForm.Items.Item("4").Specific).Value.Trim() == "")
                            {
                                oApplication.StatusBar.SetText("Select the Supplier....", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                BubbleEvent = false;
                            }

                            if (oHDBDataSource.GetValue("DocStatus", oHDBDataSource.Offset).ToString().Trim() != "O")
                            {
                                BubbleEvent = false;
                            }

                           // strDocEntry = oHDBDataSource.GetValue("DocEntry", 0).Trim();
                            //strDocType = oHDBDataSource.GetValue("U_DocType", 0).Trim();
                        }                            
                        break;
                    default:
                        break;
                }
            }

            else if (pVal.BeforeAction == false)
            {
                switch (pVal.EventType)
                {
                    case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                       //Utilities.UtilitiesCls.CreateButtonItem(oForm, "btnSH", oForm.Items.Item("2").Left + oForm.Items.Item("2").Width + 120, oForm.Items.Item("2").Top, oForm.Items.Item("2").Width + 20, oForm.Items.Item("2").Height, "Purchase History", true, 0, "");
                        Utilities.UtilitiesCls.CreateButtonItem(oForm, "btnSH", oForm.Items.Item("46").Left, oForm.Items.Item("46").Top + 20, oForm.Items.Item("46").Width, oForm.Items.Item("2").Height, "Purchase History", true, 0, "46");
                        break;
                    case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED:
                        if (pVal.ItemUID == "1")
                        {
                            if (pVal.ActionSuccess)
                            {
                                if (oWaitForm != null)
                                {
                                    oWaitForm.Close();
                                    oWaitForm = null;
                                }
                            }
                        }

                        if (pVal.ItemUID == "btnSH")
                          {
                              try
                              {
                                  if (oWaitForm != null)
                                  {
                                      string strCardCode1 = ((SAPbouiCOM.EditText)oForm.Items.Item("4").Specific).Value.Trim();
                                      string strCardCode2 = ((SAPbouiCOM.EditText)oWaitForm.Items.Item("6").Specific).Value.Trim();

                                      if (strCardCode1 != strCardCode2)
                                      {
                                          oWaitForm.Close();
                                          oWaitForm = null;
                                      }
                                  }
                              }
                              catch (Exception)
                              {
                                  oWaitForm = null;
                              }

                              if (oWaitForm == null)
                              {
                                  string strQry;
                                  oWaitForm = clsSBO.LoadForm("EJ_OSSH.srf", "EJ_OSSH", oApplication);
                                  oWaitForm.State = BoFormStateEnum.fs_Maximized;

                                  ((SAPbouiCOM.EditText)oWaitForm.Items.Item("6").Specific).Value = ((SAPbouiCOM.EditText)oForm.Items.Item("4").Specific).Value.Trim();
                                  ((SAPbouiCOM.EditText)oWaitForm.Items.Item("8").Specific).Value = ((SAPbouiCOM.EditText)oForm.Items.Item("54").Specific).Value.Trim();
                                  oWaitForm.Freeze(true);
                                  oWaitForm.DataSources.DataTables.Add("@dtTemp");
                                  SAPbouiCOM.Grid oGrid;
                                  oGrid = (SAPbouiCOM.Grid)oWaitForm.Items.Item("4").Specific;
                                  oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                                  strQry = "Exec \"EJ_SupplierPurchaseHistory\"  '" + ((SAPbouiCOM.EditText)oForm.Items.Item("4").Specific).Value.Trim() + "'";
                                  oWaitForm.DataSources.DataTables.Item(0).ExecuteQuery(strQry);
                                  oGrid.DataTable = oWaitForm.DataSources.DataTables.Item(0);
                                  oGrid.Columns.Item(0).Editable = true;
                                  SAPbouiCOM.EditTextColumn oEditTxt = ((SAPbouiCOM.EditTextColumn)oGrid.Columns.Item(0));
                                  oEditTxt.ChooseFromListUID = "CFL_2";
                                  oEditTxt.ChooseFromListAlias = "ItemCode";
                                  oGrid.Columns.Item(1).Editable = false;
                                  oGrid.Columns.Item(2).Editable = true;
                                  oGrid.Columns.Item(3).Editable = true;
                                  oEditTxt = ((SAPbouiCOM.EditTextColumn)oGrid.Columns.Item(3));
                                  oEditTxt.ChooseFromListUID = "CFL_3";
                                  oEditTxt.ChooseFromListAlias = "UomCode";
                                  oGrid.Columns.Item(4).Editable = true;
                                  oGrid.Columns.Item(0).Editable = true;

                                  for (int j = 5; j < oGrid.Columns.Count; j++)
                                  {
                                      oGrid.Columns.Item(j).Editable = false;
                                  }

                                  oGrid.AutoResizeColumns();
                                  oGrid.DataTable.Rows.Add(1);

                                  clsSupplierPurchaseHistory.oForm = oWaitForm;
                                  clsSupplierPurchaseHistory.oSOForm = oForm;
                                  oWaitForm.Freeze(false);
                              }
                              else
                              {
                                  oWaitForm.Visible = true;
                              }
                          }
                        break;
                   
                    case SAPbouiCOM.BoEventTypes.et_LOST_FOCUS:
                        if (oForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE || oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE)
                        {
                            if (pVal.ItemUID == "12")
                            {
                                //oForm.DefButton = "btnSH";
                                // oForm.ActiveItem = "btnSH";
                                ((SAPbouiCOM.Button)oForm.Items.Item("btnSH").Specific).Item.Click(BoCellClickType.ct_Regular);
                            }
                        }
                        break;

                    case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                        if (oWaitForm != null)
                        {
                            try
                            {
                                oWaitForm.Close();
                            }
                            catch (Exception)
                            {

                            }

                            try
                            {

                            }
                            catch (Exception)
                            {
                                oWaitForm = null;
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
        }
        #endregion

        #region "MenuEvents"
        public static void clsSalesOrder_MenuEvent(ref SAPbouiCOM.Application oApplication, ref SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.MenuEvent pVal, ref bool BubbleEvent)
        {
            try
            {
                oHDBDataSource = oForm.DataSources.DBDataSources.Item("ORDR");
                oChildDataSource = oForm.DataSources.DBDataSources.Item("RDR1");
                oMatrix = (SAPbouiCOM.Matrix)oForm.Items.Item("38").Specific;  

                if (pVal.BeforeAction)
                {
                    if (pVal.MenuUID == "1299")
                    {
                                            
                    }
                    if (pVal.MenuUID == "DeleteRow" && oForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                    {
                        
                    }
                }                
            }
            catch (Exception ex)
            {                
                oApplication.MessageBox(ex.Message.ToString() + "/" + oCompany.GetLastErrorDescription().ToString(), 1, "OK", "", "");
            }

        }
        #endregion

    }
}
