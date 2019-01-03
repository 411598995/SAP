using System;
using System.Collections.Generic;
using System.Collections;
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


namespace WarehouseTransfer.Sales
{
    class clsSalesOrder
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
       private static List<string> ColsFromHistory = new List<string>();
      
        #region "ItemEvent"
        public static void clsSalesOrder_ItemEvent(ref SAPbouiCOM.Application oApplication, ref SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oSetupForm, ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {

            oForm = oSetupForm;
            oHDBDataSource = oForm.DataSources.DBDataSources.Item("ORDR");
            oChildDataSource = oForm.DataSources.DBDataSources.Item("RDR1");
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
                                oApplication.StatusBar.SetText("Select the Customer....", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
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
                       //Utilities.UtilitiesCls.CreateButtonItem(oForm, "btnSH", oForm.Items.Item("2").Left + oForm.Items.Item("2").Width + 6, oForm.Items.Item("2").Top, oForm.Items.Item("2").Width, oForm.Items.Item("2").Height, "Sales History", true, 0, "");
                       Utilities.UtilitiesCls.CreateButtonItem(oForm, "btnSH", oForm.Items.Item("46").Left, oForm.Items.Item("46").Top + 20, oForm.Items.Item("46").Width, oForm.Items.Item("2").Height, "Sales History", true, 0, "46");
                       ColsFromHistory = new List<string> { "1", "11","14","1470002145" };
                       break;
                    case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED:
                        if (pVal.ItemUID == "1")
                        {
                            if (oForm.Mode != BoFormMode.fm_FIND_MODE)
                            {
                                if (pVal.ActionSuccess)
                                {
                                    if (oWaitForm != null)
                                    {
                                        try
                                        {                                            
                                            oWaitForm.Close();
                                        }
                                        catch (Exception ex)
                                        {
                                            
                                        }
                                        oWaitForm = null;
                                    }
                                }
                            }
                        }

                        if (pVal.ItemUID == "btnSH")
                          {
                            try
                            {
                                oWaitForm.Close();
                                oWaitForm = null;
                            }
                            catch
                            {

                            }
                              try
                              {
                                  if (oWaitForm != null)
                                  {
                                      string strCardCode1 = ((SAPbouiCOM.EditText)oForm.Items.Item("4").Specific).Value.Trim();
                                      string strCardCode2 = ((SAPbouiCOM.EditText)oWaitForm.Items.Item("6").Specific).Value.Trim();

                                      if (strCardCode1 != strCardCode2)
                                      {
                                          try
                                          {
                                              oWaitForm.Close();
                                          }
                                          catch (Exception ex)
                                          {

                                          }
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
                                oWaitForm = clsSBO.LoadForm("EJ_OCSH.srf", "EJ_OCSH", oApplication);
                                try
                                {

                                    oWaitForm.Settings.MatrixUID = "4";
                                    oWaitForm.Settings.Enabled = true;
                                }
                                catch { }

                                oWaitForm.State = BoFormStateEnum.fs_Maximized;

                                ((SAPbouiCOM.EditText)oWaitForm.Items.Item("6").Specific).Value = ((SAPbouiCOM.EditText)oForm.Items.Item("4").Specific).Value.Trim();
                                ((SAPbouiCOM.EditText)oWaitForm.Items.Item("8").Specific).Value = ((SAPbouiCOM.EditText)oForm.Items.Item("54").Specific).Value.Trim();
                                oWaitForm.Freeze(true);
                                oWaitForm.DataSources.DataTables.Add("@dtTemp");
                                SAPbouiCOM.Grid oGrid;
                                oGrid = (SAPbouiCOM.Grid)oWaitForm.Items.Item("4").Specific;

                                oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                                strQry = "EXEC dbo.EJ_CustomerSalesHistory  '" + ((SAPbouiCOM.EditText)oForm.Items.Item("4").Specific).Value.Trim() + "','" + ((SAPbouiCOM.EditText)oForm.Items.Item("8").Specific).Value.Trim() + "'";
                                System.Data.DataTable dtHistory = clsSBO.getDataTable(strQry, "History", oCompany);
                                oWaitForm.DataSources.DataTables.Item(0).ExecuteQuery(strQry);

                              
                                oGrid.DataTable = oWaitForm.DataSources.DataTables.Item(0);
                                string strCardCode = ((SAPbouiCOM.EditText)oForm.Items.Item("4").Specific).Value.Trim();
                                int i = 0;
                                //foreach (System.Data.DataRow dr in dtHistory.Rows)
                                //{
                                //    try
                                //    {
                                //        if (dr["ItemCode"].ToString() != "")
                                //        {
                                //            SAPbobsCOM.ItemPriceReturnParams itemPrice = clsSBO.getUnitPriceSys(oCompany,dr["ItemCode"].ToString().Trim(), strCardCode , 1, DateTime.Now.Date, dr["UoM"].ToString().Trim());
                                //            oGrid.DataTable.SetValue("Current Selling Price", i, itemPrice.Price);
                                //            i++;
                                //        }
                                //    }
                                //    catch { }

                                //}
                               
                                oGrid.Columns.Item(0).Editable = true;
                                SAPbouiCOM.EditTextColumn oEditTxt = ((SAPbouiCOM.EditTextColumn)oGrid.Columns.Item(0));
                                oEditTxt.ChooseFromListUID = "CFL_2";
                                oEditTxt.ChooseFromListAlias = "ItemCode";
                                oGrid.Columns.Item(1).Editable = false;
                                string showName = clsSBO.getColVisibility("EJ_OCSH", "ItemName", oCompany.UserName, oCompany);
                                if(showName=="N") oGrid.Columns.Item(1).Visible=false;

                                oGrid.Columns.Item(2).Editable = true;
                                oGrid.Columns.Item(3).Editable = true;
                                oEditTxt = ((SAPbouiCOM.EditTextColumn)oGrid.Columns.Item(3));
                                oEditTxt.ChooseFromListUID = "CFL_3";
                                oEditTxt.ChooseFromListAlias = "UomCode";
                                oGrid.Columns.Item(4).Editable = true;


                                for (int j = 5; j < oGrid.Columns.Count; j++)
                                {
                                    oGrid.Columns.Item(j).Editable = false;
                                }
                                oGrid.Columns.Item(1).Editable = false;
                                oGrid.Columns.Item(0).Editable = true;

                                oGrid.AutoResizeColumns();
                                oGrid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Auto;
                                oGrid.DataTable.Rows.Add(1);

                                clsCustomerSalesHistory.oForm = oWaitForm;
                                clsCustomerSalesHistory.oSOForm = oForm;
                                clsCustomerSalesHistory.ORDR = oForm.DataSources.DBDataSources.Item("ORDR");
                                clsCustomerSalesHistory.cardCode = ((SAPbouiCOM.EditText)oForm.Items.Item("4").Specific).Value.Trim();
                                clsCustomerSalesHistory.oCompany = oCompany;
                                clsCustomerSalesHistory.oApplication = oApplication;
                                clsCustomerSalesHistory.isAdding = oForm.Mode == BoFormMode.fm_ADD_MODE ? true : false;
                                clsCustomerSalesHistory.CallingForm = "SO";
                                clsCustomerSalesHistory.myDocNum = Convert.ToInt32( ((SAPbouiCOM.EditText)oForm.Items.Item("8").Specific).Value.Trim());
                                oWaitForm.Freeze(false);

                            }
                            else
                            {

                                SAPbouiCOM.Grid oGrid;
                                oGrid = (SAPbouiCOM.Grid)oWaitForm.Items.Item("4").Specific;

                                try
                                {
                                    for (int i = 1; i < oMatrix.RowCount; i++)
                                    {
                                        bool boolAvailable = false;
                                        for (int j = 0; j < oGrid.Rows.Count; j++)
                                        {
                                            if (((SAPbouiCOM.EditText)oMatrix.Columns.Item("1").Cells.Item(i).Specific).Value.Trim() == oGrid.DataTable.GetValue("ItemCode", j).ToString().Trim())
                                            {
                                                boolAvailable = true;
                                                oGrid.DataTable.SetValue("Order Qty", j, Convert.ToInt32(Convert.ToDouble(((SAPbouiCOM.EditText)oMatrix.Columns.Item("11").Cells.Item(i).Specific).Value.Trim())));
                                                break;
                                            }
                                        }
                                        if (boolAvailable == false)
                                        {
                                            int intGridRow = 0;
                                            if (oGrid.DataTable.GetValue("ItemCode", oGrid.Rows.Count - 2).ToString().Trim() == "")
                                            {
                                                intGridRow = oGrid.Rows.Count - 2;
                                            }
                                            else
                                            {
                                                intGridRow = oGrid.Rows.Count - 1;
                                            }

                                            oGrid.DataTable.SetValue("ItemCode", intGridRow, ((SAPbouiCOM.EditText)oMatrix.Columns.Item("1").Cells.Item(i).Specific).Value.Trim());
                                            oGrid.DataTable.SetValue("ItemName", intGridRow, ((SAPbouiCOM.EditText)oMatrix.Columns.Item("3").Cells.Item(i).Specific).Value.Trim());
                                            oGrid.DataTable.SetValue("Order Qty", intGridRow, Convert.ToInt32(Convert.ToDouble(((SAPbouiCOM.EditText)oMatrix.Columns.Item("11").Cells.Item(i).Specific).Value.Trim())));
                                            oGrid.DataTable.SetValue("UoM", intGridRow, ((SAPbouiCOM.EditText)oMatrix.Columns.Item("1470002145").Cells.Item(i).Specific).Value.Trim());
                                            string input = ((SAPbouiCOM.EditText)oMatrix.Columns.Item("14").Cells.Item(i).Specific).Value.Trim();
                                            SAPbobsCOM.ItemPriceReturnParams itemPrice =  clsSBO. getUnitPriceSys(oCompany, oGrid.DataTable.GetValue("ItemCode", pVal.Row).ToString().Trim(),  ((SAPbouiCOM.EditText)oForm.Items.Item("4").Specific).Value.Trim(), 1, DateTime.Now.Date, ((SAPbouiCOM.EditText)oMatrix.Columns.Item("1470002145").Cells.Item(i).Specific).Value.Trim());
                                            oGrid.DataTable.SetValue("Current Selling Price", pVal.Row, itemPrice.Price);


                                            //oGrid.DataTable.SetValue("Current Selling Price", intGridRow, new string(input.Where(c => (Char.IsDigit(c) || c == '.' || c == ',')).ToArray()));

                                            oGrid.DataTable.Rows.Add();

                                        }
                                    }
                                }
                                catch (Exception)
                                {

                                }


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
