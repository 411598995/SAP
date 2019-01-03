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
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Threading;
using System.Xml.XPath;
using System.Xml;
//using excel = Microsoft.Office.Interop.Excel;

namespace WarehouseTransfer.Inventory
{
    class clsStockShipment
    {

        #region Declarations
        private static SAPbouiCOM.Form oForm;
        private static SAPbouiCOM.DBDataSource oHeaderDataSource;
        private static SAPbouiCOM.DBDataSource oChildDataSource;
        private static SAPbouiCOM.DataTable oDataTable;
        private static SAPbouiCOM.Matrix oMatrix;
        private static SAPbobsCOM.Recordset oRecordSet = null;
        private static bool[] blnInitialize = null;
        private static bool blnAuthorize = false;
        #endregion

        #region Constructor

        public clsStockShipment(ref SAPbouiCOM.Application oApplication, ref SAPbobsCOM.Company oCompany)
        {
            try
            {
                //blnAuthorize = clsSBO.CheckAuthorization(ref oCompany, "EJ_OSTS", oCompany.UserSignature);
                blnAuthorize = true;
                if (blnAuthorize)
                {
                    oForm = clsSBO.LoadForm("EJ_OSTS.srf", "EJ_OSTS", oApplication);
                    if (oForm != null)
                        Initialize(ref oApplication, ref oCompany, ref oForm);
                    if (oForm.TypeCount == 1)
                    {
                        blnInitialize = new bool[1];
                    }
                    else if (oForm.TypeCount > 1)
                    {
                        blnInitialize = (bool[])ResizeArray(blnInitialize, oForm.TypeCount);
                    }
                    blnInitialize[oForm.TypeCount - 1] = true;
                }
                else
                {
                    oApplication.MessageBox("Not Authorized to View This Screen ", 0, "", "", "");
                }                  
            }
            catch (Exception e)
            {
                oApplication.MessageBox(e.Message.ToString() + "/" + oCompany.GetLastErrorDescription().ToString(), 1, "OK", "", "");
            }
        }

        #endregion

        #region "Events"

        #region "ItemEvent"
        public static void clsStockShipment_ItemEvent(ref SAPbouiCOM.Application oApplication, ref SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oSetupForm, ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            if (oForm != null)
            {
                oForm = oSetupForm;
                if (blnInitialize != null && blnInitialize.Length >= oForm.TypeCount)
                {
                    if (blnInitialize != null && blnInitialize[oForm.TypeCount - 1])
                    {
                        oHeaderDataSource = oForm.DataSources.DBDataSources.Item("@EJ_OSTS");
                        oChildDataSource = oForm.DataSources.DBDataSources.Item("@EJ_STS1");
                    }
                }

                if (pVal.BeforeAction)
                {
                    switch (pVal.EventType)
                    {
                        case BoEventTypes.et_ITEM_PRESSED:
                            if (pVal.ItemUID == "1" && (oForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE || oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE))
                            {
                                if (!Validate(oApplication, oCompany, oForm))
                                    BubbleEvent = false;
                            }       
                            break;
                        case BoEventTypes.et_KEY_DOWN:
                            if (pVal.ItemUID == "12" && ((SAPbouiCOM.EditText)oForm.Items.Item("12").Specific).Value.ToString() != "" && pVal.CharPressed == 13)
                            {
                                if (((SAPbouiCOM.EditText)oForm.Items.Item("6").Specific).Value == "")
                                {
                                    oApplication.StatusBar.SetText("Select the From Warehouse....", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    BubbleEvent = false;
                                }
                            }
                            break;
                        case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST:
                            if (pVal.ItemUID == "6")
                            {
                              //  string squery = "Select * From \"OWHS\" Where \"U_Brand\" = '" + ((SAPbouiCOM.ComboBox)oForm.Items.Item("53").Specific).Value.ToString().Trim() + "' Union All Select * From \"OWHS\" Where \"U_WhsType\" = 'CW'";
                              //  Utilities.UtilitiesCls.CFLConditionQuery(ref oApplication, ref oCompany, oForm, ref pVal, squery, "WhsCode", "10063", "WhsCode", false, true, "7", "V_0", false);
                            }
                            if (pVal.ItemUID == "8")
                            {
                              //  string squery = "Select * From \"OWHS\" Where \"U_Brand\" = '" + ((SAPbouiCOM.ComboBox)oForm.Items.Item("53").Specific).Value.ToString().Trim() + "' Union All Select * From \"OWHS\" Where \"U_WhsType\" = 'CW'";
                              //  Utilities.UtilitiesCls.CFLConditionQuery(ref oApplication, ref oCompany, oForm, ref pVal, squery, "WhsCode", "10063", "WhsCode", false, true, "7", "V_0", false);
                            }
                            if (pVal.ItemUID == "48")
                            {
                                string strToWhs = ((SAPbouiCOM.EditText)oForm.Items.Item("8").Specific).Value.Trim();
                                string squery = "";
                                //squery = "Select Distinct T0.\"DocNum\" From \"@EJ_OSTS\" T0 Inner Join \"@EJ_STS1\" T1 On T0.\"DocEntry\" = T1.\"DocEntry\"  Where IfNull(T1.\"U_ItemCode\",'') <> '' ANd (IfNull(T1.\"U_Qty\",0) - IfNull(T1.\"U_RecQty\",0) > 0) And ('" + strToWhs + "' = '' Or T0.\"U_ToWhs\" = '" + strToWhs + "')";
                                squery = "Select Distinct T0.\"DocNum\" From \"OWTQ\" T0  Inner Join \"WTQ1\" T1 On T0.\"DocEntry\" = T1.\"DocEntry\" Where (IfNull(T1.\"Quantity\",0) - IfNull(T1.\"U_RecQty\",0) > 0)";
                                Utilities.UtilitiesCls.CFLConditionQuery(ref oApplication, ref oCompany, oForm, ref pVal, squery, "DocNum", "CFL_7", "DocNum", false, false, "", "", false);
                            }
                            if (pVal.ItemUID == "50")
                            {
                                string strFrmWhs = ((SAPbouiCOM.EditText)oForm.Items.Item("6").Specific).Value.Trim();
                                string strToWhs = ((SAPbouiCOM.EditText)oForm.Items.Item("8").Specific).Value.Trim();

                                if (strFrmWhs == "")
                                {
                                    oApplication.StatusBar.SetText("Select the From Warehouse....", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    BubbleEvent = false;
                                }                                

                                string squery = "";                                
                                //squery = "Select Distinct \"AbsEntry\" From PKL1 Where \"BaseObject\" = '1250000001' And \"PickStatus\" = 'Y'"
                                squery = "Select Distinct T0.\"AbsEntry\" From PKL1 T0 Inner Join OPKL T1 On T0.\"AbsEntry\" = T1.\"AbsEntry\" Inner Join OWTQ T2 On T0.\"OrderEntry\" = T2.\"DocEntry\" Where T0.\"BaseObject\" = '1250000001' And T0.\"PickStatus\" In ('Y','P') And \"Status\" <> 'C' And \"Filler\" = '"+ strFrmWhs +"' ";
                                Utilities.UtilitiesCls.CFLConditionQuery(ref oApplication, ref oCompany, oForm, ref pVal, squery, "AbsEntry", "CFL_8", "AbsEntry", false, false, "", "", false);
                            }
                            if (pVal.ItemUID == "22" && pVal.ColUID == "V_0")
                            {
                                if (((SAPbouiCOM.EditText)oForm.Items.Item("6").Specific).Value == "")
                                {
                                    oApplication.StatusBar.SetText("Select the From Warehouse....", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    BubbleEvent = false;
                                }
                                else
                                {
                                    if (((SAPbouiCOM.EditText)oForm.Items.Item("12").Specific).Value != "")
                                    {
                                        string squery = "CALL \"EJ_FindItem\" ('" + ((SAPbouiCOM.EditText)oForm.Items.Item("12").Specific).Value + "')";
                                        Utilities.UtilitiesCls.CFLConditionQuery(ref oApplication, ref oCompany, oForm, ref pVal, squery, "ItemCode", "10003", "ItemCode", false, true, "22", "V_0", false);
                                    }
                                    else
                                    {
                                        string squery = "CALL \"EJ_FindItem\" ('" + ((SAPbouiCOM.EditText)oForm.Items.Item("12").Specific).Value + "')";
                                        Utilities.UtilitiesCls.ClearCFLConditionQuery(ref oApplication, ref oCompany, oForm, ref pVal, "", "ItemCode", "10003", "ItemCode", false, true, "22", "V_0", true);
                                    }
                                }
                            }
                            break;

                        case SAPbouiCOM.BoEventTypes.et_VALIDATE:
                            if (pVal.ItemUID == "22" && pVal.ColUID == "V_3")
                            {
                                string strAvlQty = ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_2").Cells.Item(pVal.Row).Specific).Value.Trim();
                                string strRecQty = ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_3").Cells.Item(pVal.Row).Specific).Value.Trim();
                                //Decimal dblAvlQty = Decimal.Parse(((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_2").Cells.Item(pVal.Row).Specific).Value.Trim());
                                //Decimal dblRecQty = Decimal.Parse(((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_3").Cells.Item(pVal.Row).Specific).Value.Trim());                               
                                double dblAvlQty = Convert.ToDouble(strAvlQty, System.Globalization.CultureInfo.InvariantCulture);
                                double dblRecQty = Convert.ToDouble(strRecQty, System.Globalization.CultureInfo.InvariantCulture);
                                if (dblAvlQty < dblRecQty)
                                {
                                    BubbleEvent = false;
                                    oApplication.StatusBar.SetText("Shipment Qty Should Not Be Greater than Available Qty ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                }
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
                        case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST:
                            if (oForm.Mode != SAPbouiCOM.BoFormMode.fm_FIND_MODE)
                            {

                                if (oForm.Mode != SAPbouiCOM.BoFormMode.fm_FIND_MODE)
                                {
                                    if (pVal.ItemUID == "48")
                                    {
                                        string strDocNum = "";
                                        oDataTable = UtilitiesCls.DataTable(ref oApplication, ref oCompany, oForm, ref pVal);
                                        if (oDataTable != null)
                                        {
                                            strDocNum = oDataTable.GetValue("DocNum", 0).ToString();
                                        }

                                        if (strDocNum != "")
                                        {
                                            FillRequest(oApplication, oCompany, strDocNum);
                                        }
                                        
                                    }

                                    if (pVal.ItemUID == "50")
                                    {
                                        string strDocNum = "";
                                        oDataTable = UtilitiesCls.DataTable(ref oApplication, ref oCompany, oForm, ref pVal);
                                        if (oDataTable != null)
                                        {
                                            strDocNum = oDataTable.GetValue("AbsEntry", 0).ToString();
                                        }

                                        if (strDocNum != "")
                                        {
                                            FillPickList(oApplication, oCompany, strDocNum);
                                        }

                                    }
                                }

                                if (pVal.ItemUID == "22" && pVal.ColUID == "V_0")
                                {
                                    string strQry = "";
                                    string strDftWhs = "";
                                    string strItemCode = "";
                                    oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                                    oDataTable = UtilitiesCls.DataTable(ref oApplication, ref oCompany, oForm, ref pVal);
                                    if (oDataTable != null)
                                    {
                                        strItemCode = oDataTable.GetValue("ItemCode", 0).ToString(); 
                                    }

                                    if (strItemCode != "")
                                    {
                                        strQry = "Select T0.\"ItemCode\",T0.\"ItemName\",T0.\"InvntryUom\",T1.\"OnHand\",T0.\"CodeBars\" From \"OITM\" T0 Inner Join \"OITW\" T1 On T0.\"ItemCode\" = '" + strItemCode + "' ";
                                        strQry += " And T0.\"ItemCode\" = T1.\"ItemCode\" And T1.\"WhsCode\" = '" + ((SAPbouiCOM.EditText)oForm.Items.Item("6").Specific).Value.Trim() + "'";
                                        oRecordSet.DoQuery(strQry);
                                        
                                        if (!oRecordSet.EoF)
                                        {
                                            oMatrix.FlushToDataSource();
                                            strDftWhs = oRecordSet.Fields.Item(0).Value.ToString();
                                            oChildDataSource.SetValue("LineId", pVal.Row - 1, oMatrix.RowCount.ToString());
                                            oChildDataSource.SetValue("U_ItemCode", pVal.Row - 1, oRecordSet.Fields.Item("ItemCode").Value.ToString());
                                            oChildDataSource.SetValue("U_ItemName", pVal.Row - 1, oRecordSet.Fields.Item("ItemName").Value.ToString());
                                            oChildDataSource.SetValue("U_Qty", pVal.Row - 1, "0");
                                            oChildDataSource.SetValue("U_UOMCode", pVal.Row - 1, oRecordSet.Fields.Item("InvntryUom").Value.ToString());
                                            oChildDataSource.SetValue("U_OnHand", pVal.Row - 1, oRecordSet.Fields.Item("OnHand").Value.ToString());
                                            oChildDataSource.SetValue("U_BarCode", pVal.Row - 1, oRecordSet.Fields.Item("CodeBars").Value.ToString());

                                            if (pVal.Row == oMatrix.RowCount)
                                            {
                                                oMatrix.AddRow(1, oMatrix.RowCount - 1);
                                            }
                                            oMatrix.FlushToDataSource();
                                            oForm.Update(); 
                                            if (Convert.ToDouble(oRecordSet.Fields.Item("OnHand").Value.ToString()) == 0)
                                            {                                                
                                                oApplication.StatusBar.SetText("No Stock Available For This Item...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                            }
                                        }                                                                                                                       
                                    }                                   
                                }
                            }
                            break;
                        
                        case SAPbouiCOM.BoEventTypes.et_KEY_DOWN:
                            if (pVal.ItemUID == "12" && ((SAPbouiCOM.EditText)oForm.Items.Item("12").Specific).Value.ToString() != "" && pVal.CharPressed == 13)
                            {
                                string strQry = "";
                                string strDftWhs = "";
                                string strItemCode = "";
                                oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                                strQry = "Select T0.\"ItemCode\",T0.\"ItemName\" From \"OITM\" T0 Where (T0.\"ItemCode\" = '" + ((SAPbouiCOM.EditText)oForm.Items.Item("12").Specific).Value.ToString() + "' Or T0.\"CodeBars\" = '" + ((SAPbouiCOM.EditText)oForm.Items.Item("12").Specific).Value.ToString() + "') ";                                
                                oRecordSet.DoQuery(strQry);
                                if (!oRecordSet.EoF)
                                {
                                    strItemCode = oRecordSet.Fields.Item("ItemCode").Value.ToString();
                                }

                                if (strItemCode != "")
                                {
                                    oMatrix.FlushToDataSource();

                                    for (int iIndex = 0; iIndex <= oChildDataSource.Size - 1; iIndex++)
                                    {
                                        if (oChildDataSource.GetValue("U_ItemCode", iIndex).Trim() == strItemCode)
                                        {
                                            double dblNewQty = Convert.ToDouble(oChildDataSource.GetValue("U_Qty", iIndex), System.Globalization.CultureInfo.InvariantCulture) + 1;

                                            double dblAvlQty = Convert.ToDouble(oChildDataSource.GetValue("U_OnHand", iIndex), System.Globalization.CultureInfo.InvariantCulture);
                                            if (dblAvlQty < dblNewQty)
                                            {
                                                oApplication.StatusBar.SetText("Shipment Qty Should Not Be Greater than Available Qty ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                                return;
                                            }

                                            oChildDataSource.SetValue("U_Qty", iIndex, dblNewQty.ToString());
                                            oMatrix.LoadFromDataSource();
                                            ((SAPbouiCOM.EditText)oForm.Items.Item("12").Specific).Value = "";
                                            oForm.Update();
                                            oForm.Freeze(true);
                                            ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_3").Cells.Item(oMatrix.RowCount).Specific).Value = "0";
                                            oForm.ActiveItem = "12";
                                            oForm.Freeze(false);
                                            return;
                                        }
                                    }

                                    strQry = "Select T0.\"ItemCode\",T0.\"ItemName\",T0.\"InvntryUom\",T1.\"OnHand\",T0.\"CodeBars\" From \"OITM\" T0 Inner Join \"OITW\" T1 On T0.\"ItemCode\" = '" + strItemCode + "' ";
                                    strQry += " And T0.\"ItemCode\" = T1.\"ItemCode\" And T1.\"WhsCode\" = '" + oHeaderDataSource.GetValue("U_FromWhs", 0).Trim() + "'";
                                    oRecordSet.DoQuery(strQry);

                                    if (!oRecordSet.EoF)
                                    {
                                        if (Convert.ToDouble(oRecordSet.Fields.Item("OnHand").Value.ToString()) > 0)
                                        {
                                            strDftWhs = oRecordSet.Fields.Item(0).Value.ToString();
                                            oChildDataSource.SetValue("LineId", oChildDataSource.Offset, oMatrix.RowCount.ToString());
                                            oChildDataSource.SetValue("U_ItemCode", oChildDataSource.Offset, oRecordSet.Fields.Item("ItemCode").Value.ToString());
                                            oChildDataSource.SetValue("U_ItemName", oChildDataSource.Offset, oRecordSet.Fields.Item("ItemName").Value.ToString());
                                            oChildDataSource.SetValue("U_UOMCode", oChildDataSource.Offset, oRecordSet.Fields.Item("InvntryUom").Value.ToString());
                                            oChildDataSource.SetValue("U_OnHand", oChildDataSource.Offset, oRecordSet.Fields.Item("OnHand").Value.ToString());
                                            oChildDataSource.SetValue("U_Qty", oChildDataSource.Offset, "1");

                                            oChildDataSource.SetValue("U_BarCode", oChildDataSource.Offset, oRecordSet.Fields.Item("CodeBars").Value.ToString());

                                            ((SAPbouiCOM.EditText)oForm.Items.Item("12").Specific).Value = "";
                                            oForm.Update();
                                            oMatrix.AddRow(1, oMatrix.RowCount - 1);
                                            oMatrix.FlushToDataSource();
                                            oForm.Freeze(true);
                                            ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_3").Cells.Item(oMatrix.RowCount).Specific).Value = "0";
                                            oForm.ActiveItem = "12";
                                            oForm.Freeze(false);
                                        }
                                        else
                                        {
                                            ((SAPbouiCOM.EditText)oForm.Items.Item("12").Specific).Value = "";
                                            oApplication.StatusBar.SetText("No Stock Available For This Item...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                        }                                        
                                    }
                                    else
                                    {
                                        ((SAPbouiCOM.EditText)oForm.Items.Item("12").Specific).Value = "";
                                        oApplication.StatusBar.SetText("Item Details Not Found...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                    }
                                }
                                else
                                {
                                    ((SAPbouiCOM.EditText)oForm.Items.Item("12").Specific).Value = "";
                                    oApplication.StatusBar.SetText("Invalid ItemCode...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                }                                                      
                            }
                            break;
                        case BoEventTypes.et_ITEM_PRESSED:
                            if (oForm.Mode != SAPbouiCOM.BoFormMode.fm_FIND_MODE)
                            {

                                if (pVal.ItemUID == "45")
                                {
                                    string fileToCopy = System.Windows.Forms.Application.StartupPath + @"\STSUpload.xls";
                                    string destinationDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\";

                                    File.Copy(fileToCopy, destinationDirectory + Path.GetFileName(fileToCopy));

                                    oApplication.StatusBar.SetText("STS Template 'STSUpload.xls' is saved in your desktop ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                                }
                                if (pVal.ItemUID == "46")
                                {
                                    if (((SAPbouiCOM.EditText)oForm.Items.Item("6").Specific).Value != "")
                                    {
                                        FileBrowser file = new FileBrowser();
                                        file.oApplication = oApplication;
                                        file.BrowseFileDialogAbsent();
                                    }
                                    else
                                    {
                                        oApplication.StatusBar.SetText("Select the From Warehouse ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                    }                                    
                                }
                                if (pVal.ItemUID == "47")
                                {
                                    if (((SAPbouiCOM.EditText)oForm.Items.Item("6").Specific).Value != "")
                                    {
                                        uploadExcel(((SAPbouiCOM.EditText)(oForm.Items.Item("EdImport").Specific)).Value.Trim(), oForm, oCompany, oApplication);
                                    }
                                    else
                                    {
                                        oApplication.StatusBar.SetText("Select the From Warehouse ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                    } 
                                }

                                if (pVal.ItemUID == "29")
                                {
                                    SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                    oRS.DoQuery("SELECT \"MenuUID\" FROM \"OCMN\" WHERE \"Name\" = 'StockShipment' AND \"Type\" = 'C'");
                                    if (oRS.RecordCount == 0)
                                    {
                                        oApplication.MessageBox("Report layout not found.", 0, "OK", null, null);
                                    }
                                    else
                                    {
                                        SAPbouiCOM.Form oNewform = null;
                                        oApplication.ActivateMenuItem(oRS.Fields.Item(0).Value.ToString());
                                        oNewform = oApplication.Forms.ActiveForm;
                                        oNewform.Visible = false;
                                        ((SAPbouiCOM.EditText)oNewform.Items.Item("1000003").Specific).String = oHeaderDataSource.GetValue("DocEntry", 0).Trim();
                                        oNewform.Items.Item("1").Click(SAPbouiCOM.BoCellClickType.ct_Regular); // abrir reporte
                                        oNewform.Close();
                                    }
                                }
                                else if (pVal.ItemUID == "38")
                                {
                                    string strQry;

                                    //if (clsGBarcode.oForm == null)
                                    //{
                                    //    SAPbouiCOM.Form oWaitForm = clsSBO.LoadForm("EJ_ORBC.srf", "EJ_ORBC", oApplication);
                                    //    // ((SAPbouiCOM.EditText)oWaitForm.Items.Item("3").Specific).Value = "";
                                    //    string strGRPODE = oHeaderDataSource.GetValue("DocEntry", oHeaderDataSource.Offset).ToString();
                                    //    InitChildForm(ref oApplication, ref oCompany, ref oWaitForm, ref strGRPODE);
                                    //}
                                }
                                else if (pVal.ItemUID == "30" && oForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                                {
                                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific).Active = true;
                                    //((SAPbouiCOM.EditText)oForm.Items.Item("12").Specific).Active = true;
                                    //oForm.Items.Item("12").Click(BoCellClickType.ct_Regular);
                                   // oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Click(BoCellClickType.ct_Regular);
                                    
                                    oMatrix.SetCellFocus(oMatrix.RowCount, 1);                                    
                                    oApplication.SendKeys("{TAB}");
                                    
                                    //oApplication.SendKeys("{TAB}");
                                }
                                else if (pVal.ItemUID == "31" && oForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                                {

                                    oForm.Items.Item("34").Visible = true;
                                    oForm.Items.Item("35").Visible = true;
                                    oForm.Items.Item("36").Visible = true;
                                    oForm.Items.Item("37").Visible = true;
                                    oForm.Items.Item("31").Enabled = false;
                                    
                                }
                                else if (pVal.ItemUID == "37" && oForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                                {
                                    ((SAPbouiCOM.EditText)oForm.Items.Item("35").Specific).Value = "";
                                    ((SAPbouiCOM.EditText)oForm.Items.Item("24").Specific).Active = true;
                                    oForm.Items.Item("34").Visible = false;
                                    oForm.Items.Item("35").Visible = false;
                                    oForm.Items.Item("36").Visible = false;
                                    oForm.Items.Item("37").Visible = false;
                                    oForm.Items.Item("31").Enabled = true;

                                }
                                else if (pVal.ItemUID == "36" && oForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                                {
                                    if (((SAPbouiCOM.EditText)oForm.Items.Item("35").Specific).Value.Trim() != "")
                                    {
                                        string strDocNum = "";
                                        string strQry = "";
                                        SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                        oRS.DoQuery("Select IfNull(Max(\"Code\"),0) + 1 \"DocNum\" From \"@EJ_OSSP\"");
                                        if (oRS.RecordCount > 0)
                                        {
                                            strDocNum = oRS.Fields.Item("DocNum").Value.ToString().Trim();
                                            //strQry = "Insert Into STS_Park Values('" + strDocNum + "','" + ((SAPbouiCOM.EditText)oForm.Items.Item("4").Specific).Value.Trim() + "'";
                                            //strQry += ",'" + ((SAPbouiCOM.EditText)oForm.Items.Item("6").Specific).Value.Trim() + "','" + ((SAPbouiCOM.EditText)oForm.Items.Item("8").Specific).Value.Trim() + "'";
                                            //strQry += ",'" + ((SAPbouiCOM.EditText)oForm.Items.Item("24").Specific).Value.Trim() + "','" + ((SAPbouiCOM.EditText)oForm.Items.Item("21").Specific).Value.Trim() + "','" + ((SAPbouiCOM.EditText)oForm.Items.Item("35").Specific).Value.Trim() + "')";

                                            SAPbobsCOM.UserTable oUserTable;
                                            oUserTable = (SAPbobsCOM.UserTable)oCompany.UserTables.Item("EJ_OSSP");
                                            oUserTable.Code = strDocNum;
                                            oUserTable.Name = strDocNum;
                                            oUserTable.UserFields.Fields.Item("U_CardCode").Value = ((SAPbouiCOM.EditText)oForm.Items.Item("4").Specific).Value.Trim();
                                            oUserTable.UserFields.Fields.Item("U_FromWhs").Value = ((SAPbouiCOM.EditText)oForm.Items.Item("6").Specific).Value.Trim();
                                            oUserTable.UserFields.Fields.Item("U_ToWhs").Value = ((SAPbouiCOM.EditText)oForm.Items.Item("8").Specific).Value.Trim();
                                            oUserTable.UserFields.Fields.Item("U_Remarks").Value = ((SAPbouiCOM.EditText)oForm.Items.Item("24").Specific).Value.Trim();
                                            oUserTable.UserFields.Fields.Item("U_Sender").Value = ((SAPbouiCOM.ComboBox)oForm.Items.Item("42").Specific).Value.Trim();
                                            oUserTable.UserFields.Fields.Item("U_ParkRemarks").Value = ((SAPbouiCOM.EditText)oForm.Items.Item("35").Specific).Value.Trim();
                                            oUserTable.Add();


                                           // SAPbobsCOM.Recordset oRS1 = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                            //oRS1.DoQuery(strQry);

                                            int intSno = 1;
                                            for (int i = 1; i < oMatrix.RowCount; i++)
                                            {
                                                if (((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_0").Cells.Item(i).Specific).Value != "")
                                                {
                                                    //strQry = "Insert Into STS_ParkDetail Values('" + strDocNum + "','" + intSno + "','" + ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_0").Cells.Item(i).Specific).Value + "'";
                                                    //strQry += ",'" + ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_3").Cells.Item(i).Specific).Value + "')";
                                                    //oRS1.DoQuery(strQry);

                                                    oUserTable = (SAPbobsCOM.UserTable)oCompany.UserTables.Item("EJ_SSP1");
                                                    oUserTable.Name = strDocNum;
                                                    oUserTable.UserFields.Fields.Item("U_Sno").Value = intSno.ToString(); ;
                                                    oUserTable.UserFields.Fields.Item("U_ItemCode").Value = ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_0").Cells.Item(i).Specific).Value.Trim();
                                                    oUserTable.UserFields.Fields.Item("U_Qty").Value = ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_3").Cells.Item(i).Specific).Value.Trim();
                                                    oUserTable.Add();

                                                }
                                                intSno += 1;
                                            }
                                        }
                                        oApplication.SetStatusBarMessage("Document Parked Successfully...", BoMessageTime.bmt_Short, false);
                                        oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE;
                                        oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE;
                                        ((SAPbouiCOM.EditText)oForm.Items.Item("35").Specific).Value = "";
                                        ((SAPbouiCOM.EditText)oForm.Items.Item("24").Specific).Active = true;
                                        oForm.Items.Item("34").Visible = false;
                                        oForm.Items.Item("35").Visible = false;
                                        oForm.Items.Item("36").Visible = false;
                                        oForm.Items.Item("37").Visible = false;
                                        oForm.Items.Item("31").Enabled = true;
                                        //SAPbouiCOM.ButtonCombo oBCombo = (SAPbouiCOM.ButtonCombo)oForm.Items.Item(pVal.ItemUID).Specific;
                                        //oBCombo.Select("", BoSearchKey.psk_ByValue);
                                        Initialize(ref oApplication, ref oCompany, ref oForm);
                                    }
                                    else
                                    {
                                        oApplication.SetStatusBarMessage("Please Put Some Park Remarks...", BoMessageTime.bmt_Short, true);
                                    }
                                }
                            }
                            if (pVal.ItemUID == "1" && oForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                            {
                                if (pVal.ActionSuccess)
                                {
                                    Initialize(ref oApplication, ref oCompany, ref oForm);
                                }
                            }
                            break;
                        
                        case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT:
                            if (pVal.ItemUID == "32" && oForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                            {
                                oForm.Freeze(true);
                                 SAPbouiCOM.ButtonCombo oBCombo = (SAPbouiCOM.ButtonCombo)oForm.Items.Item(pVal.ItemUID).Specific;
                                 SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                 oRS.DoQuery("Select \"U_CardCode\",\"U_FromWhs\",\"U_ToWhs\",\"U_Remarks\",\"U_Sender\"  From \"@EJ_OSSP\" Where \"Code\" = '" + oBCombo.Selected.Value + "'");
                                 if (oRS.RecordCount > 0)
                                 {
                                     ((SAPbouiCOM.EditText)oForm.Items.Item("4").Specific).Value = oRS.Fields.Item("U_CardCode").Value.ToString();
                                     ((SAPbouiCOM.EditText)oForm.Items.Item("6").Specific).Value = oRS.Fields.Item("U_FromWhs").Value.ToString();
                                     ((SAPbouiCOM.EditText)oForm.Items.Item("8").Specific).Value = oRS.Fields.Item("U_ToWhs").Value.ToString();
                                     ((SAPbouiCOM.EditText)oForm.Items.Item("24").Specific).Value = oRS.Fields.Item("U_Remarks").Value.ToString();
                                     ((SAPbouiCOM.ComboBox)oForm.Items.Item("42").Specific).Select(oRS.Fields.Item("U_Sender").Value.ToString(),BoSearchKey.psk_ByValue);
                                 }

                                 string strQry = "";
                                 strQry = "Select T0.\"U_ItemCode\",T2.\"ItemName\",T2.\"CodeBars\",T2.\"InvntryUom\",T3.\"OnHand\",T0.\"U_Qty\",T2.\"U_SpclCode\" \"ArtNo\",T2.\"U_Size\" \"Size\",T2.\"U_Color\",T2.\"U_SubGroup\",T2.\"U_SubBrand\",T2.\"U_Class\",T2.\"U_SubClass\",T2.\"U_Season\",T2.\"U_Gender\"  From \"@EJ_SSP1\" T0 ";
                                 strQry += " Inner Join \"@EJ_OSSP\" T1 ON T0.\"Name\" = T1.\"Code\" Inner Join \"OITM\" T2 ON T0.\"U_ItemCode\" = T2.\"ItemCode\"";
                                 strQry += " Inner Join \"OITW\" T3 ON T0.\"U_ItemCode\" = T3.\"ItemCode\" And T3.\"WhsCode\" = T1.\"U_FromWhs\" Where T0.\"Name\" = '" + oBCombo.Selected.Value + "' Order By T0.\"U_Sno\" Desc";
                                 oRS.DoQuery(strQry);

                                 oMatrix.Clear();
                                 oChildDataSource.Clear();
                                // oMatrix.AddRow(oRS.RecordCount + 1, oMatrix.RowCount);
                                 oMatrix.FlushToDataSource();
                                 int introw = 0;
                                 while (!oRS.EoF)
                                 {
                                     
                                     oChildDataSource.SetValue("LineId", oChildDataSource.Offset, (introw + 1).ToString());
                                     oChildDataSource.SetValue("U_ItemCode", oChildDataSource.Offset, oRS.Fields.Item("U_ItemCode").Value.ToString());
                                     oChildDataSource.SetValue("U_ItemName", oChildDataSource.Offset, oRS.Fields.Item("ItemName").Value.ToString());
                                     oChildDataSource.SetValue("U_ArtNo", oChildDataSource.Offset, oRS.Fields.Item("ArtNo").Value.ToString());
                                     oChildDataSource.SetValue("U_Size", oChildDataSource.Offset, oRS.Fields.Item("Size").Value.ToString());
                                     oChildDataSource.SetValue("U_UOMCode", oChildDataSource.Offset, oRS.Fields.Item("InvntryUom").Value.ToString());
                                     oChildDataSource.SetValue("U_OnHand", oChildDataSource.Offset, oRS.Fields.Item("OnHand").Value.ToString());
                                     oChildDataSource.SetValue("U_Qty", oChildDataSource.Offset, oRS.Fields.Item("U_Qty").Value.ToString());

                                     oChildDataSource.SetValue("U_BarCode", oChildDataSource.Offset, oRS.Fields.Item("CodeBars").Value.ToString());
                                     oChildDataSource.SetValue("U_SubGroup", oChildDataSource.Offset, oRS.Fields.Item("U_SubGroup").Value.ToString());
                                     oChildDataSource.SetValue("U_SubBrand", oChildDataSource.Offset, oRS.Fields.Item("U_SubBrand").Value.ToString());
                                     oChildDataSource.SetValue("U_Class", oChildDataSource.Offset, oRS.Fields.Item("U_Class").Value.ToString());
                                     oChildDataSource.SetValue("U_SubClass", oChildDataSource.Offset, oRS.Fields.Item("U_SubClass").Value.ToString());
                                     oChildDataSource.SetValue("U_Color", oChildDataSource.Offset, oRS.Fields.Item("U_Color").Value.ToString());
                                     oChildDataSource.SetValue("U_Season", oChildDataSource.Offset, oRS.Fields.Item("U_Season").Value.ToString());
                                     oChildDataSource.SetValue("U_Gender", oChildDataSource.Offset, oRS.Fields.Item("U_Gender").Value.ToString());

                                     oMatrix.AddRow(1, oMatrix.RowCount - 1);
                                     oRS.MoveNext();
                                     introw += 1;
                                 }

                                 oForm.Update();
                                oMatrix.FlushToDataSource();
                                 
                                 oMatrix.AddRow(1, oMatrix.RowCount-1);
                                 ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_-1").Cells.Item(oMatrix.RowCount).Specific).Value = oMatrix.RowCount.ToString();
                                 ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_1").Cells.Item(oMatrix.RowCount).Specific).Value = "";
                                 ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_2").Cells.Item(oMatrix.RowCount).Specific).Value = "0";
                                 ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_3").Cells.Item(oMatrix.RowCount).Specific).Value = "0";
                                 ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_4").Cells.Item(oMatrix.RowCount).Specific).Value = "";
                                 ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific).Value = "";
                                 ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_5").Cells.Item(oMatrix.RowCount).Specific).Value = "";
                                 ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_6").Cells.Item(oMatrix.RowCount).Specific).Value = "";                                 

                                 SAPbobsCOM.UserTable oUserTable;
                                 oUserTable = (SAPbobsCOM.UserTable)oCompany.UserTables.Item("EJ_OSSP");
                                 if (oUserTable.GetByKey(oBCombo.Selected.Value.Trim()))
                                 {
                                     oUserTable.Remove();
                                 }

                                 oRS.DoQuery("Select \"Code\" From \"@EJ_SSP1\" Where \"Name\" = '" + oBCombo.Selected.Value.Trim() + "'");
                                 while (!oRS.EoF)
                                 {
                                     oUserTable = (SAPbobsCOM.UserTable)oCompany.UserTables.Item("EJ_SSP1");
                                     if (oUserTable.GetByKey(oRS.Fields.Item(0).Value.ToString()))
                                     {
                                         oUserTable.Remove();
                                     }
                                     oRS.MoveNext();
                                 }

                                 oForm.Items.Item("34").Enabled = false;
                                 oForm.Freeze(false);    
                            }
                            break;
                        case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                            blnInitialize[oForm.TypeCount - 1] = false;
                            break;

                        default:
                            break;
                    }
                }
            }
        }
        #endregion

        #region "DataEvents"
        public static void clsStockShipment_FormDataEvent(ref SAPbouiCOM.Application oApplication, ref SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.BusinessObjectInfo oBusinessObjectInfo, ref  bool BubbleEvent)
        {
            if (blnInitialize != null && blnInitialize.Length >= oForm.TypeCount)
            {
                if (blnInitialize != null && blnInitialize[oForm.TypeCount - 1])
                {
                    oHeaderDataSource = oForm.DataSources.DBDataSources.Item("@EJ_OSTS");
                    oChildDataSource = oForm.DataSources.DBDataSources.Item("@EJ_STS1");
                }
            }

            if (oBusinessObjectInfo.BeforeAction)
            {
                switch (oBusinessObjectInfo.EventType)
                {
                    case SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD:
                        try
                        {
                            Inventory_Transfer(oApplication, oCompany);
                        }
                        catch (Exception ex)
                        {
                            BubbleEvent = false;
                            oApplication.MessageBox(ex.Message.ToString(), 1, "OK", string.Empty, string.Empty);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (oBusinessObjectInfo.EventType)
                {                    
                    case BoEventTypes.et_FORM_DATA_ADD:
                        if (oBusinessObjectInfo.ActionSuccess)
                        {
                            //string strQry = "";
                            //strQry = "Update T10 Set T10.\"U_RecQty\" = T11.\"Qty\" From \"WTQ1\" T10 Inner Join ";
                            //strQry += " (	Select T1.\"U_BaseDE\",T1.\"U_BaseLn\",SUM(T1.\"U_Qty\") \"Qty\" From ";
                            //strQry += " 	\"@EJ_STS1\" T1 Inner Join \"@EJ_OSTS\" T2 On T1.\"DocEntry\" = T2.\"DocEntry\"";
                            //strQry += "	    Where T2.\"U_ReqNum\" = '" + oHeaderDataSource.GetValue("U_reqNum", 0).Trim() + "' Group By T1.\"U_BaseDE\",T1.\"U_BaseLn\"";
                            //strQry += " ) T11 On T10.\"DocEntry\" = T11.\"U_BaseDE\" And T10.\"LineNum\" = T11.\"U_BaseLn\"";
                            //oRecordSet.DoQuery(strQry);

                            string strITRDE = "";

                            for (int iIndex = 0; iIndex <= oChildDataSource.Size - 1; iIndex++)
                            {
                                if (strITRDE.Trim() == "")
                                {
                                    strITRDE = oChildDataSource.GetValue("U_BaseDE", iIndex).Trim().ToString();
                                }
                                else
                                {
                                    break;
                                }
                            }
                            oRecordSet.DoQuery("Call \"EJ_UpdateStockTransferRequest\" ('" + strITRDE + "')");

                            oRecordSet.DoQuery("Select Count(*) \"Cnt\" From WTQ1 Where \"DocEntry\" = '" + strITRDE + "' And \"Quantity\" > IfNull(\"U_TrsfQty\",0)");
                            if (oRecordSet.RecordCount > 0)
                            {
                                if (oRecordSet.Fields.Item("Cnt").Value.ToString() == "0")
                                {
                                    SAPbobsCOM.StockTransfer oTransfer;
                                    oTransfer = (SAPbobsCOM.StockTransfer)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest);
                                    if (oTransfer.GetByKey(Convert.ToInt32(strITRDE)))
                                    {
                                        oTransfer.Close();
                                    }
                                }                                
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
        public static void clsStockShipment_MenuEvent(ref SAPbouiCOM.Application oApplication, ref SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.MenuEvent pVal, ref bool BubbleEvent)
        {
            try
            {
                oForm.Freeze(true);
                if (blnInitialize != null && blnInitialize.Length >= oForm.TypeCount)
                {
                    if (blnInitialize != null && blnInitialize[oForm.TypeCount - 1])
                    {
                        oHeaderDataSource = oForm.DataSources.DBDataSources.Item("@EJ_OSTS");
                        oHeaderDataSource = oForm.DataSources.DBDataSources.Item("@EJ_STS1");
                    }
                }

                if (pVal.BeforeAction)
                {
                    if (pVal.MenuUID == "1283" || pVal.MenuUID == "1285")
                    {
                        oApplication.SetStatusBarMessage("Cannot Remove/Restore the Document...", BoMessageTime.bmt_Medium, true);
                        BubbleEvent = false;
                    }
                    if (pVal.MenuUID == "DeleteRow" && oForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                    {
                        RemarksDeleteRow(oApplication, oCompany, ref pVal, ref BubbleEvent);
                    }
                }
                else
                {
                    if (pVal.MenuUID == "1282")
                    {
                        Initialize(ref oApplication, ref oCompany, ref oForm);
                    }                   
                }
                oForm.Freeze(false);
            }
            catch (Exception ex)
            {
                oForm.Freeze(false);
                oApplication.MessageBox(ex.Message.ToString() + "/" + oCompany.GetLastErrorDescription().ToString(), 1, "OK", "", "");
            }

        }
        #endregion

        #region "RightClick"
        public static void clsStockShipment_RightClickEvent(ref SAPbouiCOM.Application oApplication, ref SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.ContextMenuInfo oEventInfo, ref bool BubbleEvent)
        {
            SAPbouiCOM.ItemEvent pVal = null;
            if (oEventInfo.BeforeAction == true && oEventInfo.ItemUID == "22")
            {
                RemarksRightClick(oApplication, oCompany, oForm, ref BubbleEvent, ref oEventInfo, ref pVal);
            }
        }
        #endregion

        #endregion

        #region "Functions"

        public static System.Array ResizeArray(System.Array oldArray, int newSize)
        {
            int oldSize = oldArray.Length;
            System.Type elementType = oldArray.GetType().GetElementType();
            System.Array newArray = System.Array.CreateInstance(elementType, newSize);
            int preserveLength = System.Math.Min(oldSize, newSize);
            if (preserveLength > 0)
                System.Array.Copy(oldArray, newArray, preserveLength);
            return newArray;
        }

        static void Initialize(ref SAPbouiCOM.Application oApplication, ref SAPbobsCOM.Company oCompany, ref SAPbouiCOM.Form oForm)
        {
            try
            {
                string strQry = "";
                string strSlpCode = "-1";
                oForm.Freeze(true);
                SAPbouiCOM.Column oColumn;

                oForm.DataBrowser.BrowseBy = "15";
                oHeaderDataSource = oForm.DataSources.DBDataSources.Item("@EJ_OSTS");
                oChildDataSource = oForm.DataSources.DBDataSources.Item("@EJ_STS1");
                oForm.Items.Item("4").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 1, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                oForm.Items.Item("6").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 1, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                oForm.Items.Item("8").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 1, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
             //   oForm.Items.Item("21").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 1, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                oForm.Items.Item("22").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 1, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                oForm.Items.Item("12").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 1, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                oForm.Items.Item("15").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_True);
                oForm.Items.Item("53").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 1, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                
                oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                oMatrix = (SAPbouiCOM.Matrix)oForm.Items.Item("22").Specific;

                oForm.Items.Item("34").Visible = false;
                oForm.Items.Item("35").Visible = false;
                oForm.Items.Item("36").Visible = false;
                oForm.Items.Item("36").Visible = false;

                oColumn = oMatrix.Columns.Item("V_0");
                SAPbouiCOM.LinkedButton oLink = (SAPbouiCOM.LinkedButton)oColumn.ExtendedObject;
                oLink.LinkedObject = SAPbouiCOM.BoLinkedObject.lf_Items;

                //For the DocNum
               // strQry = "SELECT \"AutoKey\",\"DfltSeries\" FROM \"ONNM\" WHERE \"ObjectCode\" = 'EJ_OSTS'";

                strQry = "Select IfNull(Max(\"DocNum\"),0) + 1 \"DocNum\" From \"@EJ_OSTS\"";
                oRecordSet.DoQuery(strQry);
                if (!oRecordSet.EoF)
                {
                    oHeaderDataSource.SetValue("DocNum", oHeaderDataSource.Offset, oRecordSet.Fields.Item("DocNum").Value.ToString());
                }

                strQry = "Select \"WhsCode\" From \"OWHS\" Where \"U_TranWhs\" = 'Y'";
                oRecordSet.DoQuery(strQry);
                if (!oRecordSet.EoF)
                {
                    oHeaderDataSource.SetValue("U_TrnWhs", oHeaderDataSource.Offset, oRecordSet.Fields.Item("WhsCode").Value.ToString());
                }

                oRecordSet.DoQuery("Select \"SUPERUSER\" From \"OUSR\" Where \"USERID\"	= '" + oCompany.UserSignature.ToString() + "'");
                if (!oRecordSet.EoF)
                {
                    if (oRecordSet.Fields.Item(0).Value.ToString() == "Y")
                    {
                        strQry = "Select T0.\"Warehouse\",T1.\"WhsName\",T0.\"SalePerson\" FROM \"OUDG\" T0 LEFT JOIN \"OWHS\" T1 ON T1.\"WhsCode\" = T0.\"Warehouse\" ";
                        strQry += " Where T0.\"Code\" = (Select \"DfltsGroup\" FROM \"OUSR\" WHERE \"USERID\" = '" + oCompany.UserSignature.ToString() + "')";
                        oRecordSet.DoQuery(strQry);

                        if (!oRecordSet.EoF)
                        {
                            oHeaderDataSource.SetValue("U_FromWhs", oHeaderDataSource.Offset, oRecordSet.Fields.Item(0).Value.ToString());
                            strSlpCode = oRecordSet.Fields.Item("SalePerson").Value.ToString();
                        }
                        oForm.Items.Item("6").Enabled = true;
                    }
                    else
                    {
                        strQry = "Select T0.\"Warehouse\",T1.\"WhsName\",T0.\"SalePerson\" FROM \"OUDG\" T0 LEFT JOIN \"OWHS\" T1 ON T1.\"WhsCode\" = T0.\"Warehouse\" ";
                        strQry += " Where T0.\"Code\" = (Select \"DfltsGroup\" FROM \"OUSR\" WHERE \"USERID\" = '" + oCompany.UserSignature.ToString() + "')";
                        oRecordSet.DoQuery(strQry);

                        if (!oRecordSet.EoF)
                        {
                            oHeaderDataSource.SetValue("U_FromWhs", oHeaderDataSource.Offset, oRecordSet.Fields.Item(0).Value.ToString());
                            strSlpCode = oRecordSet.Fields.Item("SalePerson").Value.ToString();
                        }
                        oForm.Items.Item("6").Enabled = true;
                    }
                }
                
                oMatrix.AddRow(1, -1);                
                ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_-1").Cells.Item(oMatrix.RowCount).Specific).Value = oMatrix.RowCount.ToString();
                oMatrix.FlushToDataSource();
                oHeaderDataSource.SetValue("U_DocDate", oHeaderDataSource.Offset, System.DateTime.Now.ToString("yyyyMMdd"));

                oMatrix.Columns.Item("V_3").ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto;

                SAPbouiCOM.ButtonCombo oBCombo = (SAPbouiCOM.ButtonCombo)oForm.Items.Item("32").Specific;

                int pscount = oBCombo.ValidValues.Count;
                for (int j = pscount - 1; j >= 0; j--)
                {
                    oBCombo.ValidValues.Remove(j, SAPbouiCOM.BoSearchKey.psk_Index);
                }

                strQry = "Select \"Code\",\"U_ParkRemarks\"  From \"@EJ_OSSP\" ";
                oRecordSet.DoQuery(strQry);
                while (!oRecordSet.EoF)
                {
                    oBCombo.ValidValues.Add(oRecordSet.Fields.Item(0).Value.ToString(), oRecordSet.Fields.Item(1).Value.ToString());
                    oRecordSet.MoveNext();
                }
                //oBCombo.Select("-1", BoSearchKey.psk_ByValue);

                SAPbouiCOM.ComboBox oCombo = (SAPbouiCOM.ComboBox)oForm.Items.Item("42").Specific;
                pscount = oCombo.ValidValues.Count;
                for (int j = pscount - 1; j >= 0; j--)
                {
                    oCombo.ValidValues.Remove(j, SAPbouiCOM.BoSearchKey.psk_Index);
                }
                strQry = "Select \"SlpCode\",\"SlpName\" From \"OSLP\"";
                oRecordSet.DoQuery(strQry);
                while (!oRecordSet.EoF)
                {
                    oCombo.ValidValues.Add(oRecordSet.Fields.Item(0).Value.ToString(), oRecordSet.Fields.Item(1).Value.ToString());
                    oRecordSet.MoveNext();
                }
                oCombo.Select(strSlpCode, BoSearchKey.psk_ByValue);

                oCombo = (SAPbouiCOM.ComboBox)oForm.Items.Item("43").Specific;
                pscount = oCombo.ValidValues.Count;
                for (int j = pscount - 1; j >= 0; j--)
                {
                    oCombo.ValidValues.Remove(j, SAPbouiCOM.BoSearchKey.psk_Index);
                }

                strQry = "Select \"Code\",\"Name\" From \"@EJ_ODRM\"";
                oRecordSet.DoQuery(strQry);
                while (!oRecordSet.EoF)
                {
                    oCombo.ValidValues.Add(oRecordSet.Fields.Item(0).Value.ToString(), oRecordSet.Fields.Item(1).Value.ToString());
                    oRecordSet.MoveNext();
                }

                oCombo = (SAPbouiCOM.ComboBox)oForm.Items.Item("1000005").Specific;
                pscount = oCombo.ValidValues.Count;
                for (int j = pscount - 1; j >= 0; j--)
                {
                    oCombo.ValidValues.Remove(j, SAPbouiCOM.BoSearchKey.psk_Index);
                }

                strQry = "Select \"Code\",\"Name\" From \"@EJ_OVHM\"";
                oRecordSet.DoQuery(strQry);
                while (!oRecordSet.EoF)
                {
                    oCombo.ValidValues.Add(oRecordSet.Fields.Item(0).Value.ToString(), oRecordSet.Fields.Item(1).Value.ToString());
                    oRecordSet.MoveNext();
                }

                oCombo = (SAPbouiCOM.ComboBox)oForm.Items.Item("53").Specific;
                pscount = oCombo.ValidValues.Count;
                for (int j = pscount - 1; j >= 0; j--)
                {
                    oCombo.ValidValues.Remove(j, SAPbouiCOM.BoSearchKey.psk_Index);
                }

                strQry = "Select \"OcrCode\",\"OcrName\" From \"OOCR\" Where \"DimCode\" = '4'";
                oRecordSet.DoQuery(strQry);
                while (!oRecordSet.EoF)
                {
                    oCombo.ValidValues.Add(oRecordSet.Fields.Item(0).Value.ToString(), oRecordSet.Fields.Item(1).Value.ToString());
                    oRecordSet.MoveNext();
                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordSet);

                oForm.Freeze(false);
            }
            catch (Exception e)
            {
                oApplication.MessageBox(e.Message.ToString() + "/" + oCompany.GetLastErrorDescription().ToString(), 1, "OK", "", "");
            }
            finally
            {
                oForm.Freeze(false);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordSet);
            }
        }

        private static void RemarksRightClick(SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref bool BubbleEvent, ref SAPbouiCOM.ContextMenuInfo eventInfo, ref SAPbouiCOM.ItemEvent pVal)
        {
            SAPbouiCOM.MenuItem oMenuItem = null;
            SAPbouiCOM.Menus oMenus = null;
            try
            {
                SAPbouiCOM.MenuCreationParams oCreationPackage = null;
                oCreationPackage = ((SAPbouiCOM.MenuCreationParams)(oApplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)));

                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                oCreationPackage.UniqueID = "DeleteRow";
                oCreationPackage.String = "Delete Row";
                oCreationPackage.Enabled = true;
                oMenuItem = oApplication.Menus.Item("1280");
                oMenus = oMenuItem.SubMenus;
                oMenus.Item("1283").Enabled = false;
                bool blexist = oMenus.Exists("DeleteRow");
                if (blexist == false)
                {
                    oMenus.AddEx(oCreationPackage);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static void RemarksDeleteRow(SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany, ref SAPbouiCOM.MenuEvent pVal, ref bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                for (int i = 1; i <= oMatrix.RowCount; i++)
                {
                    if (oMatrix.IsRowSelected(i))
                    {
                        int intmsg = oApplication.MessageBox("Are you sure you want to delete?", 2, "Yes", "No", "Cancel");
                        if (intmsg == 1)
                        {
                            oMatrix.DeleteRow(i);
                            if (oForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE)
                                oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE;
                        }
                    }
                    //((SAPbouiCOM.EditText)oMatrix.Columns.Item("col_f").Cells.Item(i).Specific).Value = i.ToString();
                }

                //for (int i = 1; i <= oMatrix.RowCount; i++)
                //{
                //    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("col_f").Cells.Item(i).Specific).Value = i.ToString();
                //}
                oMatrix.FlushToDataSource();
                oForm.Update();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void FillRequest(SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany, string strDocNum)
        {
            try
            {
                oForm.Freeze(true);
                int RetVal;
                string strQry;
                oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);



                strQry = "Select  \"Filler\" \"U_FromWhs\",\"ToWhsCode\" \"U_ToWhs\" From \"OWTQ\" ";
                strQry += " Where \"DocNum\" = '" + strDocNum + "'";
                oRecordSet.DoQuery(strQry);

                if (!oRecordSet.EoF)
                {

                    oHeaderDataSource.SetValue("U_FromWhs", oHeaderDataSource.Offset, oRecordSet.Fields.Item("U_FromWhs").Value.ToString());
                    oHeaderDataSource.SetValue("U_ToWhs", oHeaderDataSource.Offset, oRecordSet.Fields.Item("U_ToWhs").Value.ToString());
                }

                oMatrix.Clear();
                strQry = "Select T1.\"DocEntry\",T1.\"LineNum\",T1.\"WhsCode\",T1.\"ItemCode\",T1.\"Dscription\" \"ItemName\",T1.\"CodeBars\",T1.\"UomCode\",T1.\"Quantity\",IfNull(T1.\"Quantity\",0) - IfNull(T1.\"U_RecQty\",0) \"BalQty\",T2.\"OnHand\" From   ";
                strQry += " \"OWTQ\" T0 Inner Join \"WTQ1\" T1 On T0.\"DocEntry\" = T1.\"DocEntry\"  ";
                strQry += " Inner Join \"OITW\" T2 On T1.\"ItemCode\" = T2.\"ItemCode\" And T2.\"WhsCode\" = T0.\"Filler\"  ";
                strQry += " Where T0.\"DocNum\" = '" + strDocNum + "' And  IfNull(T1.\"ItemCode\",'') <> '' And (IfNull(T1.\"Quantity\",0) - IfNull(T1.\"U_RecQty\",0) > 0) Order By \"LineNum\" Desc";
                oRecordSet.DoQuery(strQry);
                oChildDataSource.Clear();
                oMatrix.FlushToDataSource();

                if (oRecordSet.RecordCount > 0)
                {
                    oHeaderDataSource.SetValue("U_ToWhs", oHeaderDataSource.Offset, oRecordSet.Fields.Item("WhsCode").Value.ToString());
                }       

                oMatrix.AddRow(1, oMatrix.RowCount - 1);

                int intRow;
                while (!oRecordSet.EoF)
                {
                    intRow = oMatrix.RowCount;
                    oChildDataSource.SetValue("LineId", oChildDataSource.Offset, intRow.ToString());
                    oChildDataSource.SetValue("U_ItemCode", oChildDataSource.Offset, oRecordSet.Fields.Item("ItemCode").Value.ToString());
                    oChildDataSource.SetValue("U_ItemName", oChildDataSource.Offset, oRecordSet.Fields.Item("ItemName").Value.ToString());
                    oChildDataSource.SetValue("U_UOMCode", oChildDataSource.Offset, oRecordSet.Fields.Item("UomCode").Value.ToString());
                    //oChildDataSource.SetValue("U_Qty", oChildDataSource.Offset, oRecordSet.Fields.Item("U_Qty").Value.ToString());
                    oChildDataSource.SetValue("U_ReqQty", oChildDataSource.Offset, oRecordSet.Fields.Item("BalQty").Value.ToString());
                    oChildDataSource.SetValue("U_OnHand", oChildDataSource.Offset, oRecordSet.Fields.Item("OnHand").Value.ToString());
                    oChildDataSource.SetValue("U_BarCode", oChildDataSource.Offset, oRecordSet.Fields.Item("CodeBars").Value.ToString());


                    oChildDataSource.SetValue("U_Qty", oChildDataSource.Offset, "0");
                    oChildDataSource.SetValue("U_BaseDE", oChildDataSource.Offset, oRecordSet.Fields.Item("DocEntry").Value.ToString());
                    oChildDataSource.SetValue("U_BaseLn", oChildDataSource.Offset, oRecordSet.Fields.Item("LineNum").Value.ToString());
                    oMatrix.AddRow(1, oMatrix.RowCount - 1);
                    oRecordSet.MoveNext();
                }

                

                oForm.Update();
                oMatrix.FlushToDataSource();

                //oMatrix.AddRow(1, oMatrix.RowCount - 1);

                //oChildDataSource.SetValue("U_ItemCode", oChildDataSource.Size-2,"");
                //oChildDataSource.SetValue("U_ItemName", oChildDataSource.Size-1, "");
                //oChildDataSource.SetValue("U_UOMCode", oChildDataSource.Offset, "");
                ////oChildDataSource.SetValue("U_Qty", oChildDataSource.Offset, oRecordSet.Fields.Item("U_Qty").Value.ToString());
                //oChildDataSource.SetValue("U_ReqQty", oChildDataSource.Offset, "0");
                //oChildDataSource.SetValue("U_OnHand", oChildDataSource.Offset, "0");
                //oChildDataSource.SetValue("U_BarCode", oChildDataSource.Offset, "");
                //oChildDataSource.SetValue("U_Qty", oChildDataSource.Offset, "0");
                //oChildDataSource.SetValue("U_BaseDE", oChildDataSource.Offset, "");
                //oChildDataSource.SetValue("U_BaseLn", oChildDataSource.Offset, "");


                //oForm.Update();
                //oMatrix.FlushToDataSource();
                

                oForm.Freeze(false);
            }
            catch (Exception)
            {
                oForm.Freeze(false);
                throw;
            }
        }

        private static void FillPickList(SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany, string strDocNum)
        {
            try
            {
                oForm.Freeze(true);
                int RetVal;
                string strQry;
                oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);



                //strQry = "Select  \"Filler\" \"U_FromWhs\",\"ToWhsCode\" \"U_ToWhs\" From \"OWTQ\" ";
                //strQry += " Where \"DocNum\" = '" + strDocNum + "'";
                //oRecordSet.DoQuery(strQry);

                //if (!oRecordSet.EoF)
                //{

                //    oHeaderDataSource.SetValue("U_FromWhs", oHeaderDataSource.Offset, oRecordSet.Fields.Item("U_FromWhs").Value.ToString());
                //    oHeaderDataSource.SetValue("U_ToWhs", oHeaderDataSource.Offset, oRecordSet.Fields.Item("U_ToWhs").Value.ToString());
                //}

                oMatrix.Clear();
                strQry = "Select T2.\"WhsCode\",T2.\"DocEntry\",T2.\"LineNum\",T2.\"ItemCode\",T2.\"Dscription\" \"ItemName\",T3.\"OnHand\",T2.\"CodeBars\",T2.\"UomCode\",T2.\"PickOty\" \"Quantity\",(T2.\"OpenQty\" - T2.\"PickOty\") \"BalQty\" From PKL1 T0 Inner Join OPKL T1 On T0.\"AbsEntry\" = T1.\"AbsEntry\"  ";
                strQry += " Inner Join WTQ1 T2 On T0.\"OrderEntry\" = T2.\"DocEntry\"  And T0.\"OrderLine\" = T2.\"LineNum\"";
                strQry += "Inner Join OITW T3 On T2.\"ItemCode\" = T3.\"ItemCode\" And T2.\"FromWhsCod\" = T3.\"WhsCode\" ";
                //strQry += "Inner Join \"EJ_LOADPENDINGTRANSFERREQUEST\" T4 On T2.\"DocEntry\" = T4.\"DocEntry\" And T2.\"LineNum\" = T4.\"LineNum\" ";
                strQry += " Where T0.\"BaseObject\" = '1250000001' And T0.\"AbsEntry\" = '"+ strDocNum +"' And T2.\"PickOty\" > 0";
                oRecordSet.DoQuery(strQry);
                oChildDataSource.Clear();
                oMatrix.FlushToDataSource();

                if (oRecordSet.RecordCount > 0)
                {
                    oHeaderDataSource.SetValue("U_ToWhs", oHeaderDataSource.Offset, oRecordSet.Fields.Item("WhsCode").Value.ToString());
                }                

                oMatrix.AddRow(1, oMatrix.RowCount - 1);

                int intRow;
                while (!oRecordSet.EoF)
                {
                    intRow = oMatrix.RowCount;
                    oChildDataSource.SetValue("LineId", oChildDataSource.Offset, intRow.ToString());
                    oChildDataSource.SetValue("U_ItemCode", oChildDataSource.Offset, oRecordSet.Fields.Item("ItemCode").Value.ToString());
                    oChildDataSource.SetValue("U_ItemName", oChildDataSource.Offset, oRecordSet.Fields.Item("ItemName").Value.ToString());
                    oChildDataSource.SetValue("U_UOMCode", oChildDataSource.Offset, oRecordSet.Fields.Item("UomCode").Value.ToString());
                    //oChildDataSource.SetValue("U_Qty", oChildDataSource.Offset, oRecordSet.Fields.Item("U_Qty").Value.ToString());
                    oChildDataSource.SetValue("U_ReqQty", oChildDataSource.Offset, oRecordSet.Fields.Item("Quantity").Value.ToString());
                    oChildDataSource.SetValue("U_OnHand", oChildDataSource.Offset, oRecordSet.Fields.Item("OnHand").Value.ToString());
                    oChildDataSource.SetValue("U_BarCode", oChildDataSource.Offset, oRecordSet.Fields.Item("CodeBars").Value.ToString());


                    oChildDataSource.SetValue("U_Qty", oChildDataSource.Offset, oRecordSet.Fields.Item("Quantity").Value.ToString());
                    oChildDataSource.SetValue("U_BaseDE", oChildDataSource.Offset, oRecordSet.Fields.Item("DocEntry").Value.ToString());
                    oChildDataSource.SetValue("U_BaseLn", oChildDataSource.Offset, oRecordSet.Fields.Item("LineNum").Value.ToString());
                    oMatrix.AddRow(1, oMatrix.RowCount - 1);
                    oRecordSet.MoveNext();
                }



                oForm.Update();
                oMatrix.FlushToDataSource();

         


                oForm.Freeze(false);
            }
            catch (Exception)
            {
                oForm.Freeze(false);
                throw;
            }
        }

        private static bool Validate(SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm)
        {
            if (((SAPbouiCOM.EditText)oForm.Items.Item("6").Specific).Value == "")
            {
                oApplication.StatusBar.SetText("Select the From Warehouse....", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return false;
            }
            if (((SAPbouiCOM.EditText)oForm.Items.Item("8").Specific).Value == "")
            {
                oApplication.StatusBar.SetText("Select the To Warehouse....", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return false;
            }
            //if (((SAPbouiCOM.ComboBox)oForm.Items.Item("53").Specific).Value == "")
            //{
            //    oApplication.StatusBar.SetText("Select the Brand....", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            //    return false;
            //}
            else if (((SAPbouiCOM.EditText)oForm.Items.Item("17").Specific).Value == "")
            {
                oApplication.StatusBar.SetText("Select the Posting Date....", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return false;
            }
            else if (((SAPbouiCOM.ComboBox)oForm.Items.Item("42").Specific).Value == "")
            {
                oApplication.StatusBar.SetText("Select the Sender....", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return false;
            }
            //else if (((SAPbouiCOM.ComboBox)oForm.Items.Item("43").Specific).Value == "")
            //{
            //    oApplication.StatusBar.SetText("Select the Driver....", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            //    return false;
            //}
            if (oMatrix.RowCount == 1)
            {
                oApplication.StatusBar.SetText("Add the Items to Proceed....", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return false;
            }
            for (int i = 1; i < oMatrix.RowCount; i++)
            {
                if (((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_0").Cells.Item(i).Specific).Value != "")
                {
                    if (Convert.ToDouble(((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_3").Cells.Item(i).Specific).Value.ToString(), System.Globalization.CultureInfo.InvariantCulture) <= 0)
                    {
                        oApplication.StatusBar.SetText("Quantity Should be Greater than Zero....", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        return false;
                    }
                }
            }
            return true;
        }

        static void InitChildForm(ref SAPbouiCOM.Application oApplication, ref SAPbobsCOM.Company oCompany, ref SAPbouiCOM.Form oWaitForm, ref string strGRPODE)
        {
            try
            {
                oWaitForm.Freeze(true);
                string strQry;
                string strDftPL = "";
                oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                SAPbouiCOM.ComboBox oCombo = (SAPbouiCOM.ComboBox)oWaitForm.Items.Item("5").Specific;
                oWaitForm.Items.Item("7").Enabled = false;

                int pscount = oCombo.ValidValues.Count;
                for (int j = pscount - 1; j >= 0; j--)
                {
                    oCombo.ValidValues.Remove(j, SAPbouiCOM.BoSearchKey.psk_Index);
                }
                int intDfltCnt = 0;
                string strQryComboFill = "Select \"Code\",\"Name\",\"U_RptName\",\"U_DfltPL\" From \"@EJ_OBCT\" Where \"U_Type\" = 'STS' Order By IsNull(\"U_Dflt\",'N') Desc";
                oRecordSet.DoQuery(strQryComboFill);
                while (!oRecordSet.EoF)
                {
                    oCombo.ValidValues.Add(oRecordSet.Fields.Item("Code").Value.ToString(), oRecordSet.Fields.Item("Name").Value.ToString());
                    if (intDfltCnt == 0)
                    {
                        strDftPL = oRecordSet.Fields.Item("U_DfltPL").Value.ToString();
                    }
                    intDfltCnt = intDfltCnt + 1;
                    oRecordSet.MoveNext();
                }
                oCombo.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);

                oCombo = (SAPbouiCOM.ComboBox)oWaitForm.Items.Item("9").Specific;

                pscount = oCombo.ValidValues.Count;
                for (int j = pscount - 1; j >= 0; j--)
                {
                    oCombo.ValidValues.Remove(j, SAPbouiCOM.BoSearchKey.psk_Index);
                }
                strQryComboFill = "Select \"Code\",\"Name\" From \"@EJ_OBCF\" Order By IsNull(\"U_Dflt\",'N') Desc";
                oRecordSet.DoQuery(strQryComboFill);
                while (!oRecordSet.EoF)
                {
                    oCombo.ValidValues.Add(oRecordSet.Fields.Item("Code").Value.ToString(), oRecordSet.Fields.Item("Name").Value.ToString());
                    oRecordSet.MoveNext();
                }
                oCombo.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);

                oCombo = (SAPbouiCOM.ComboBox)oWaitForm.Items.Item("13").Specific;

                pscount = oCombo.ValidValues.Count;
                for (int j = pscount - 1; j >= 0; j--)
                {
                    oCombo.ValidValues.Remove(j, SAPbouiCOM.BoSearchKey.psk_Index);
                }
                strQryComboFill = "Select \"ListNum\",\"ListName\" From \"OPLN\"";
                oRecordSet.DoQuery(strQryComboFill);
                while (!oRecordSet.EoF)
                {
                    oCombo.ValidValues.Add(oRecordSet.Fields.Item("ListNum").Value.ToString(), oRecordSet.Fields.Item("ListName").Value.ToString());
                    oRecordSet.MoveNext();
                }
                if (strDftPL != "")
                {
                    oCombo.Select(strDftPL, SAPbouiCOM.BoSearchKey.psk_ByValue);
                }
                else
                {
                    oCombo.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
                }

                ((SAPbouiCOM.EditText)oWaitForm.Items.Item("7").Specific).Value = strGRPODE;

                strQry = "Select \"U_Size\" From \"@EJ_OBCF\" Where \"Code\" = '" + ((SAPbouiCOM.ComboBox)oWaitForm.Items.Item("9").Specific).Value.ToString() + "'";
                oRecordSet.DoQuery(strQry);
                if (!oRecordSet.EoF)
                {
                    ((SAPbouiCOM.EditText)oWaitForm.Items.Item("11").Specific).Value = oRecordSet.Fields.Item("U_Size").Value.ToString();
                }

                oWaitForm.Freeze(false);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordSet);
            }
            catch (Exception e)
            {
                oApplication.MessageBox(e.Message.ToString() + "/" + oCompany.GetLastErrorDescription().ToString(), 1, "OK", "", "");
            }
            finally
            {
                oWaitForm.Freeze(false);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordSet);
            }
        }

        private static void Inventory_Transfer(SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {
            try
            {

                oMatrix.FlushToDataSource();
                int RetVal;
                string strITRDE = "";
                string strBranchID = "";
                oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                SAPbobsCOM.Documents oTransfer;
                oTransfer = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit);

                string strQry = "Select IfNull(\"BPLid\",0) \"BranchID\" From OWHS Where \"WhsCode\" = '" + ((SAPbouiCOM.EditText)oForm.Items.Item("6").Specific).Value.ToString() + "' ";
                oRecordSet.DoQuery(strQry);
                if (!oRecordSet.EoF)
                {
                    strBranchID = oRecordSet.Fields.Item("BranchID").Value.ToString();
                }             

                oTransfer.BPL_IDAssignedToInvoice = Convert.ToInt32(strBranchID);
                oTransfer.UserFields.Fields.Item("U_TrnsType").Value = "S";
                oTransfer.UserFields.Fields.Item("U_TrnsDN").Value = oHeaderDataSource.GetValue("DocNum", 0).Trim().ToString();
                oTransfer.UserFields.Fields.Item("U_TrnsFW").Value = ((SAPbouiCOM.EditText)oForm.Items.Item("6").Specific).Value.ToString();
                oTransfer.UserFields.Fields.Item("U_TrnsTW").Value = ((SAPbouiCOM.EditText)oForm.Items.Item("8").Specific).Value.ToString();                

                for (int iIndex = 0; iIndex <= oChildDataSource.Size - 1; iIndex++)
                {
                    String strBillWHS = ((SAPbouiCOM.EditText)oForm.Items.Item("6").Specific).Value.ToString();

                    if (strITRDE.Trim() == "")
                    {
                        strITRDE = oChildDataSource.GetValue("U_BaseDE", iIndex).Trim().ToString();
                    }

                    if (oChildDataSource.GetValue("U_ItemCode", iIndex).Trim() != "")
                    {
                        Double dbQty = Convert.ToDouble(oChildDataSource.GetValue("U_Qty", iIndex), System.Globalization.CultureInfo.InvariantCulture);
                        oTransfer.Lines.ItemCode = oChildDataSource.GetValue("U_ItemCode", iIndex).Trim();
                        oTransfer.Lines.WarehouseCode = strBillWHS.Trim();                        
                        oTransfer.Lines.Quantity = Convert.ToDouble(dbQty);                        

                        oTransfer.Lines.UserFields.Fields.Item("U_BaseDE").Value = oChildDataSource.GetValue("DocEntry", iIndex).Trim();
                        oTransfer.Lines.UserFields.Fields.Item("U_BaseLn").Value = (iIndex +1).ToString().Trim();
                        //oTransfer.Lines.UserFields.Fields.Item("U_BaseLn").Value = oChildDataSource.GetValue("LineId", iIndex).Trim();

                        if (oChildDataSource.GetValue("U_BaseDE", iIndex).Trim() != "" && oChildDataSource.GetValue("U_BaseLn", iIndex).Trim() != "")
                        {
                            oTransfer.Lines.UserFields.Fields.Item("U_ITRDE").Value = oChildDataSource.GetValue("U_BaseDE", iIndex).Trim().ToString();
                            oTransfer.Lines.UserFields.Fields.Item("U_ITRLN").Value = oChildDataSource.GetValue("U_BaseLn", iIndex).Trim().ToString();
                        }                        

                        oTransfer.Lines.Add();
                    }
                    
                }
                RetVal = oTransfer.Add();
                if (RetVal != 0)
                {
                    string strError = "Error Transfering : " + oCompany.GetLastErrorDescription();
                    throw new Exception(strError);
                }
                else
                {
                   oApplication.StatusBar.SetText("Inventory Transfer Completed Sucessfully", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);                    

                   SAPbobsCOM.PickLists oPickLists2 = null;                                      
                   oPickLists2 = (SAPbobsCOM.PickLists)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPickLists);
                   if (oPickLists2.GetByKey(Convert.ToInt32(oHeaderDataSource.GetValue("U_PickNum", 0).Trim().ToString())))
                   {
                       oPickLists2.Close();
                   }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static bool uploadExcel(string AttchPath, SAPbouiCOM.Form oForm, SAPbobsCOM.Company oCompany, SAPbouiCOM.Application oApplication)
        {
            bool bubval = true;
            string excelFilePath = "";
            IFormatProvider ifp = new System.Globalization.CultureInfo("en-US", true);
            string Filename = "";
            string company = "", SystemName = "";
            bool contin = true;

            try
            {
                company = oCompany.CompanyDB;
                SystemName = System.Environment.UserDomainName;
                SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                SAPbobsCOM.Recordset oRS1 = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                excelFilePath = AttchPath;
                if (AttchPath != "")
                {
                    string[] AttchPath2 = AttchPath.Split('\\');
                    Filename = AttchPath2[(AttchPath2.Length - 1)];
                }

                string myExcelDataQuery = string.Format("Select [BarCode],[Quantity] FROM [Sheet1$]");

                //string sExcelConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                //   "Data Source = " + excelFilePath + ";" +
                //    "Extended Properties = Excel 8.0;";

                string sExcelConnectionString = "Provider = Microsoft.Jet.OLEDB.4.0;" +
                  "Data Source = " + excelFilePath + ";" +
                   "Extended Properties = Excel 8.0;";


                OleDbConnection OleDbConn = new OleDbConnection(sExcelConnectionString);
                OleDbCommand OleDbCmd = new OleDbCommand(myExcelDataQuery, OleDbConn);
                DataSet ds = new DataSet();
                ds.DataSetName = "Stock";
                OleDbDataAdapter OleDBAdapter = new OleDbDataAdapter(myExcelDataQuery, OleDbConn);
                try
                {
                    OleDBAdapter.Fill(ds);
                    contin = true;
                }
                catch (Exception ex)
                {
                    oApplication.SetStatusBarMessage("Not Uploaded - " + ex.Message.ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, true);
                    contin = false;
                }
                if (contin == true)
                {
                    // oForm.Freeze(true);
                    System.Data.DataTable DtTable = new System.Data.DataTable();
                    System.Data.DataTable oErrorDT = new System.Data.DataTable();
                    oErrorDT.Columns.Add("BarCode", typeof(String));
                    oErrorDT.Columns.Add("Quantity", typeof(Double));
                    oErrorDT.Columns.Add("Details", typeof(String));
                    DtTable = ds.Tables[0];


                    oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                    oMatrix.Clear();
                    oChildDataSource.Clear();
                    // oMatrix.AddRow(oRS.RecordCount + 1, oMatrix.RowCount);
                    oMatrix.FlushToDataSource();
                    int introw = 0;

                    int intTotalRecords = DtTable.Rows.Count;
                    int intProcessed = 0;

                    ((SAPbouiCOM.StaticText)oForm.Items.Item("51").Specific).Caption = intTotalRecords.ToString();

                    
                    
                    for (int i = 0; i <= DtTable.Rows.Count - 1; i++)
                    {
                        //string strQry = "Select \"ItemCode\" From \"OITM\" Where \"CodeBars\" = '" + DtTable.Rows[i]["BarCode"].ToString().Trim() + "' And \"ItmsGrpCod\" = '" + DtTable.Rows[i]["BrandCode"].ToString().Trim() + "'";
                        string strQry = "";
                        strQry = "Select T0.\"ItemCode\",T0.\"ItemName\",T0.\"InvntryUom\",T1.\"OnHand\",T0.\"U_SpclCode\" \"ArtNo\",T0.\"U_Size\" \"Size\",T0.\"CodeBars\",T0.\"U_Color\",T0.\"U_SubGroup\",T0.\"U_SubBrand\",T0.\"U_Class\",T0.\"U_SubClass\",T0.\"U_Season\",T0.\"U_Gender\" From \"OITM\" T0 Inner Join \"OITW\" T1 On T0.\"CodeBars\" = '" + DtTable.Rows[i]["BarCode"].ToString().Trim() + "' ";
                        strQry += " And T0.\"ItemCode\" = T1.\"ItemCode\" And T1.\"WhsCode\" = '" + oHeaderDataSource.GetValue("U_FromWhs", 0).Trim() + "'";
                        oRecordSet.DoQuery(strQry);
                        if (!oRecordSet.EoF)
                        {
                            if (oRecordSet.RecordCount == 1)
                            {
                                if (Convert.ToInt32(DtTable.Rows[i]["Quantity"].ToString().Trim()) <= Convert.ToInt32(oRecordSet.Fields.Item("OnHand").Value.ToString().Trim()))
                                {
                                    try
                                    {

                                        oChildDataSource.SetValue("LineId", oChildDataSource.Offset, (introw + 1).ToString());
                                        oChildDataSource.SetValue("U_ItemCode", oChildDataSource.Offset, oRecordSet.Fields.Item("ItemCode").Value.ToString());
                                        oChildDataSource.SetValue("U_ItemName", oChildDataSource.Offset, oRecordSet.Fields.Item("ItemName").Value.ToString());
                                        oChildDataSource.SetValue("U_ArtNo", oChildDataSource.Offset, oRecordSet.Fields.Item("ArtNo").Value.ToString());
                                        oChildDataSource.SetValue("U_Size", oChildDataSource.Offset, oRecordSet.Fields.Item("Size").Value.ToString());
                                        oChildDataSource.SetValue("U_UOMCode", oChildDataSource.Offset, oRecordSet.Fields.Item("InvntryUom").Value.ToString());
                                        oChildDataSource.SetValue("U_OnHand", oChildDataSource.Offset, oRecordSet.Fields.Item("OnHand").Value.ToString());
                                        oChildDataSource.SetValue("U_Qty", oChildDataSource.Offset, DtTable.Rows[i]["Quantity"].ToString().Trim());

                                        oChildDataSource.SetValue("U_BarCode", oChildDataSource.Offset, oRecordSet.Fields.Item("CodeBars").Value.ToString());
                                        oChildDataSource.SetValue("U_SubGroup", oChildDataSource.Offset, oRecordSet.Fields.Item("U_SubGroup").Value.ToString());
                                        oChildDataSource.SetValue("U_SubBrand", oChildDataSource.Offset, oRecordSet.Fields.Item("U_SubBrand").Value.ToString());
                                        oChildDataSource.SetValue("U_Class", oChildDataSource.Offset, oRecordSet.Fields.Item("U_Class").Value.ToString());
                                        oChildDataSource.SetValue("U_SubClass", oChildDataSource.Offset, oRecordSet.Fields.Item("U_SubClass").Value.ToString());
                                        oChildDataSource.SetValue("U_Color", oChildDataSource.Offset, oRecordSet.Fields.Item("U_Color").Value.ToString());
                                        oChildDataSource.SetValue("U_Season", oChildDataSource.Offset, oRecordSet.Fields.Item("U_Season").Value.ToString());
                                        oChildDataSource.SetValue("U_Gender", oChildDataSource.Offset, oRecordSet.Fields.Item("U_Gender").Value.ToString());

                                        oMatrix.AddRow(1, oMatrix.RowCount - 1);
                                        oRecordSet.MoveNext();
                                        introw += 1;
                                    }
                                    catch (Exception)
                                    {
                                        introw += 1;
                                    }
                                }
                                else
                                {
                                    DataRow dr = default(DataRow);
                                    dr = oErrorDT.NewRow();
                                    dr["BarCode"] = DtTable.Rows[i]["BarCode"].ToString();
                                    dr["Quantity"] = DtTable.Rows[i]["Quantity"].ToString();
                                    dr["Details"] = "Transfer Qty is Bigger Than Available Qty";
                                    oErrorDT.Rows.Add(dr);
                                }                                
                            }
                            else
                            {
                                DataRow dr = default(DataRow);
                                dr = oErrorDT.NewRow();
                                dr["BarCode"] = DtTable.Rows[i]["BarCode"].ToString();
                                dr["Quantity"] = DtTable.Rows[i]["Quantity"].ToString();
                                dr["Details"] = "Duplicate barcode";
                                oErrorDT.Rows.Add(dr);
                            }
                        }
                        else
                        {
                            DataRow dr = default(DataRow);
                            dr = oErrorDT.NewRow();
                            dr["BarCode"] = DtTable.Rows[i]["BarCode"].ToString();
                            dr["Quantity"] = DtTable.Rows[i]["Quantity"].ToString();
                            dr["Details"] = "Barcode not found";
                            oErrorDT.Rows.Add(dr);
                        }
                        intProcessed = intProcessed + 1;
                        ((SAPbouiCOM.StaticText)oForm.Items.Item("52").Specific).Caption = intProcessed.ToString();
                    }

                    oForm.Update();
                    oMatrix.FlushToDataSource();

                    oMatrix.AddRow(1, oMatrix.RowCount - 1);
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_-1").Cells.Item(oMatrix.RowCount).Specific).Value = oMatrix.RowCount.ToString();
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_1").Cells.Item(oMatrix.RowCount).Specific).Value = "";
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_2").Cells.Item(oMatrix.RowCount).Specific).Value = "0";
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_3").Cells.Item(oMatrix.RowCount).Specific).Value = "0";
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_4").Cells.Item(oMatrix.RowCount).Specific).Value = "";
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_0").Cells.Item(oMatrix.RowCount).Specific).Value = "";
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_5").Cells.Item(oMatrix.RowCount).Specific).Value = "";
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_6").Cells.Item(oMatrix.RowCount).Specific).Value = "";
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_7").Cells.Item(oMatrix.RowCount).Specific).Value = "";
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_8").Cells.Item(oMatrix.RowCount).Specific).Value = "";
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_9").Cells.Item(oMatrix.RowCount).Specific).Value = "";
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_10").Cells.Item(oMatrix.RowCount).Specific).Value = "";
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_11").Cells.Item(oMatrix.RowCount).Specific).Value = "";
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_12").Cells.Item(oMatrix.RowCount).Specific).Value = "";
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_13").Cells.Item(oMatrix.RowCount).Specific).Value = "";
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_15").Cells.Item(oMatrix.RowCount).Specific).Value = "";                                

                    

                    oForm.Refresh();
                    string strFolderName = Path.GetDirectoryName(AttchPath);
                    string strFileName = "\\STS Upload Failure" + DateTime.Now.ToShortTimeString().Replace(":", "") + ".txt";
                    string filepath = strFolderName + "\\STS Upload Failure" + DateTime.Now.ToShortTimeString().Replace(":", "");
                    if (oErrorDT.Rows.Count > 0)
                    {                        
                        exportToNotepad(oErrorDT, strFolderName, strFileName);
                        oApplication.SetStatusBarMessage("Check Error Log", SAPbouiCOM.BoMessageTime.bmt_Short, true);
                    }


                   

                    // oForm.Freeze(false);
                }
            }
            catch (Exception e)
            {
                oForm.Freeze(false);
                if (oCompany.InTransaction)
                    oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

                oApplication.SetStatusBarMessage("Not Uploaded - " + e.Message.ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
            return bubval;
        }

        private static void exportToNotepad(System.Data.DataTable oDt, string folderName, string fileName)
        {
            string strDateTime = System.DateTime.Now.ToString();
            string strContent = "";
            string strFile = fileName;
            string strPath = folderName + strFile;

            for (int i = 0; i <= oDt.Rows.Count - 1; i++)
            {
                strContent += oDt.Rows[i]["BarCode"].ToString() + "-" + oDt.Rows[i]["Details"].ToString() + Environment.NewLine;
            }

            if (!File.Exists(strPath))
            {
                FileStream fs = new FileStream(strPath, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine(strContent);
                sw.Flush();
                sw.Close();
            }
            else
            {
                FileStream fs = new FileStream(strPath, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine(strContent);
                sw.Flush();
                sw.Close();
            }
        }


        #endregion
    }
}
