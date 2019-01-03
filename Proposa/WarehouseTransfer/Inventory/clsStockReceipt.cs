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

namespace WarehouseTransfer.Inventory
{
    class clsStockReceipt
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

        public clsStockReceipt(ref SAPbouiCOM.Application oApplication, ref SAPbobsCOM.Company oCompany)
        {
            try
            {
                //blnAuthorize = clsSBO.CheckAuthorization(ref oCompany, "EJ_OSTS", oCompany.UserSignature);
                blnAuthorize = true;
                if (blnAuthorize)
                {
                    oForm = clsSBO.LoadForm("EJ_OSTR.srf", "EJ_OSTR", oApplication);
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
        public static void clsStockReceipt_ItemEvent(ref SAPbouiCOM.Application oApplication, ref SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oSetupForm, ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            if (oForm != null)
            {
                oForm = oSetupForm;
                if (blnInitialize != null && blnInitialize.Length >= oForm.TypeCount)
                {
                    if (blnInitialize != null && blnInitialize[oForm.TypeCount - 1])
                    {
                        oHeaderDataSource = oForm.DataSources.DBDataSources.Item("@EJ_OSTR");
                        oChildDataSource = oForm.DataSources.DBDataSources.Item("@EJ_STR1");
                    }
                }

                if (pVal.BeforeAction)
                {
                    switch (pVal.EventType)
                    {
                        case SAPbouiCOM.BoEventTypes.et_VALIDATE:
                            if (pVal.ItemUID == "22" && pVal.ColUID == "V_3")
                            {
                                Double dblAvlQty = Convert.ToDouble(((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_5").Cells.Item(pVal.Row).Specific).Value, System.Globalization.CultureInfo.InvariantCulture);
                                Double dblRecQty = Convert.ToDouble(((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_3").Cells.Item(pVal.Row).Specific).Value, System.Globalization.CultureInfo.InvariantCulture);
                                if (dblAvlQty < dblRecQty)
                                {
                                    BubbleEvent = false;
                                    oApplication.StatusBar.SetText("Received Qty Should Not Be Greater than Available Qty ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                }
                            }
                            break;
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
                                if (((SAPbouiCOM.EditText)oForm.Items.Item("28").Specific).Value == "")
                                {
                                    oApplication.StatusBar.SetText("Select the Shipment Number....", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    BubbleEvent = false;
                                }
                            }
                            break;
                        case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST:
                            if (pVal.ItemUID == "22" && pVal.ColUID == "V_0")
                            {
                                if (((SAPbouiCOM.EditText)oForm.Items.Item("6").Specific).Value == "")
                                {
                                    oApplication.StatusBar.SetText("Select the From Warehouse....", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    BubbleEvent = false;
                                }
                            }

                            if (pVal.ItemUID == "28")
                            {
                                string strToWhs = ((SAPbouiCOM.EditText)oForm.Items.Item("8").Specific).Value.Trim();
                                string squery = "Select Distinct T0.\"DocNum\" From \"@EJ_OSTS\" T0 Inner Join \"@EJ_STS1\" T1 On T0.\"DocEntry\" = T1.\"DocEntry\"  Where IfNull(T1.\"U_ItemCode\",'') <> '' ANd (IfNull(T1.\"U_Qty\",0) - IfNull(T1.\"U_RecQty\",0) > 0) And ('" + strToWhs + "' = '' Or T0.\"U_ToWhs\" = '" + strToWhs + "')";
                                Utilities.UtilitiesCls.CFLConditionQuery(ref oApplication, ref oCompany, oForm, ref pVal, squery, "DocNum", "CFL_7", "DocNum", false, false, "", "", false);
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
                                if (pVal.ItemUID == "28")
                                {
                                    string strDocNum = "";
                                    oDataTable = UtilitiesCls.DataTable(ref oApplication, ref oCompany, oForm, ref pVal);
                                    if (oDataTable != null)
                                    {
                                        strDocNum = oDataTable.GetValue("DocNum", 0).ToString();
                                    }

                                    FillShipment(oApplication, oCompany,strDocNum);
                                }
                            }
                            break;
                        case SAPbouiCOM.BoEventTypes.et_LOST_FOCUS:
                            
                            break;
                        case SAPbouiCOM.BoEventTypes.et_VALIDATE:
                            if (pVal.ItemUID == "28")
                            {
                                if (((SAPbouiCOM.EditText)oForm.Items.Item("28").Specific).Value.Trim() == "")
                                {
                                    oMatrix.FlushToDataSource();
                                    oHeaderDataSource.SetValue("U_CardCode", oHeaderDataSource.Offset, "");
                                    oHeaderDataSource.SetValue("U_FromWhs", oHeaderDataSource.Offset, "");
                                    oHeaderDataSource.SetValue("U_TrnWhs", oHeaderDataSource.Offset, "");
                                    oHeaderDataSource.SetValue("U_SlpCode", oHeaderDataSource.Offset, "");
                                    oChildDataSource.Clear();
                                    oForm.Update();
                                    oMatrix.LoadFromDataSource();
                                }                                
                            }
                            if (pVal.ItemUID == "22" && pVal.ColUID == "V_3")
                            {
                                Double dblAvlQty = Convert.ToDouble(((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_5").Cells.Item(pVal.Row).Specific).Value, System.Globalization.CultureInfo.InvariantCulture);
                                Double dblRecQty = Convert.ToDouble(((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_3").Cells.Item(pVal.Row).Specific).Value, System.Globalization.CultureInfo.InvariantCulture);
                                ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_18").Cells.Item(pVal.Row).Specific).Value = (dblAvlQty - dblRecQty).ToString();
                                oForm.Update();
                            }
                            break;
                        case SAPbouiCOM.BoEventTypes.et_KEY_DOWN:
                            if (pVal.ItemUID == "12" && ((SAPbouiCOM.EditText)oForm.Items.Item("12").Specific).Value.ToString() != "" && pVal.CharPressed == 13)
                            {
                                string strQry = "";
                                string strItemCode = "";
                                oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                                strQry = "Select T0.\"ItemCode\",T0.\"ItemName\" From \"OITM\" T0 Where (T0.\"ItemCode\" = '" + ((SAPbouiCOM.EditText)oForm.Items.Item("12").Specific).Value.ToString() + "' Or T0.\"CodeBars\" = '" + ((SAPbouiCOM.EditText)oForm.Items.Item("12").Specific).Value.ToString() + "') ";
                                oRecordSet.DoQuery(strQry);
                                if (!oRecordSet.EoF)
                                {
                                    strItemCode = oRecordSet.Fields.Item("ItemCode").Value.ToString();
                                }

                                oMatrix.FlushToDataSource();

                                if (oChildDataSource.Size > 0)
                                {
                                    for (int iIndex = 0; iIndex <= oChildDataSource.Size - 1; iIndex++)
                                    {
                                        if (oChildDataSource.GetValue("U_ItemCode", iIndex).Trim() == strItemCode)
                                        {
                                            double dblNewQty = Convert.ToDouble(oChildDataSource.GetValue("U_Qty", iIndex), System.Globalization.CultureInfo.InvariantCulture) + 1;

                                            double dblAvlQty = Convert.ToDouble(oChildDataSource.GetValue("U_BalQty", iIndex), System.Globalization.CultureInfo.InvariantCulture);
                                            if (dblAvlQty < dblNewQty)
                                            {                                                
                                                oApplication.StatusBar.SetText("Received Qty Should Not Be Greater than Available Qty ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                                return;
                                            }
                                            oChildDataSource.SetValue("U_Qty", iIndex, dblNewQty.ToString());
                                            oChildDataSource.SetValue("U_DiffQty", iIndex,(dblAvlQty - dblNewQty).ToString());
                                            oMatrix.LoadFromDataSource();
                                            oForm.Freeze(true);
                                            ((SAPbouiCOM.EditText)oForm.Items.Item("12").Specific).Value = "";
                                            ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_3").Cells.Item(oMatrix.RowCount).Specific).Active = true;
                                            oForm.ActiveItem = "12";
                                            oForm.Freeze(false);
                                            return;
                                        }
                                    }
                                    oApplication.StatusBar.SetText("Item Not Found....", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                }                               
                            }
                            break;
                        case BoEventTypes.et_ITEM_PRESSED:
                            if (pVal.ItemUID == "1" && oForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                            {
                                
                                if (pVal.ActionSuccess)
                                {
                                    Initialize(ref oApplication, ref oCompany, ref oForm);
                                }
                            }
                            if (oForm.Mode != SAPbouiCOM.BoFormMode.fm_FIND_MODE)
                            {
                                if (pVal.ItemUID == "31")
                                {
                                    SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                    oRS.DoQuery("SELECT \"MenuUID\" FROM \"OCMN\" WHERE \"Name\" = 'StockReceipt' AND \"Type\" = 'C'");
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
                                if (pVal.ItemUID == "32")
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
                                else if (pVal.ItemUID == "39" && oForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                                {
                                    //for (int i = 1; i <= oMatrix.RowCount; i++)
                                    //{
                                    //    if (((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_0").Cells.Item(i).Specific).Value != "")
                                    //    {
                                    //        ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_3").Cells.Item(i).Specific).Value = ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_5").Cells.Item(i).Specific).Value.Trim();
                                    //    }
                                    //}
                                    oMatrix.FlushToDataSource();
                                    for (int i = 0; i < oChildDataSource.Size-1; i++)
                                    {
                                        oChildDataSource.SetValue("U_Qty", i, oChildDataSource.GetValue("U_BalQty",i).Trim());
                                        oChildDataSource.SetValue("U_DiffQty", i, "0");
                                    }
                                    oMatrix.LoadFromDataSource();
                                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_3").Cells.Item(oMatrix.RowCount).Specific).Value = ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_5").Cells.Item(oMatrix.RowCount).Specific).Value.Trim();
                                    //oForm.Update();
                                }
                                else if (pVal.ItemUID == "40" && oForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                                {
                                    oForm.Items.Item("34").Visible = true;
                                    oForm.Items.Item("35").Visible = true;
                                    oForm.Items.Item("36").Visible = true;
                                    oForm.Items.Item("37").Visible = true;
                                    oForm.Items.Item("40").Enabled = false;
                                }
                                else if (pVal.ItemUID == "37" && oForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                                {
                                    ((SAPbouiCOM.EditText)oForm.Items.Item("35").Specific).Value = "";
                                    ((SAPbouiCOM.EditText)oForm.Items.Item("24").Specific).Active = true;
                                    oForm.Items.Item("34").Visible = false;
                                    oForm.Items.Item("35").Visible = false;
                                    oForm.Items.Item("36").Visible = false;
                                    oForm.Items.Item("37").Visible = false;
                                    oForm.Items.Item("40").Enabled = true;
                                }
                                else if (pVal.ItemUID == "36" && oForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                                {
                                    if (((SAPbouiCOM.EditText)oForm.Items.Item("35").Specific).Value.Trim() != "")
                                    {
                                        string strDocNum = "";
                                        string strQry = "";
                                        SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                        oRS.DoQuery("Select IfNull(Max(\"DocNum\"),0) + 1 \"DocNum\" From \"STS_Park\"");
                                        if (oRS.RecordCount > 0)
                                        {
                                            strDocNum = oRS.Fields.Item("DocNum").Value.ToString();
                                            strQry = "Insert Into STR_Park Values('" + strDocNum + "','" + ((SAPbouiCOM.EditText)oForm.Items.Item("4").Specific).Value.Trim() + "','" + ((SAPbouiCOM.EditText)oForm.Items.Item("28").Specific).Value.Trim() + "'";
                                            strQry += ",'" + ((SAPbouiCOM.EditText)oForm.Items.Item("6").Specific).Value.Trim() + "','" + ((SAPbouiCOM.EditText)oForm.Items.Item("8").Specific).Value.Trim() + "'";
                                            strQry += ",'" + ((SAPbouiCOM.EditText)oForm.Items.Item("24").Specific).Value.Trim() + "','" + ((SAPbouiCOM.ComboBox)oForm.Items.Item("42").Specific).Value.Trim() + "','" + ((SAPbouiCOM.EditText)oForm.Items.Item("35").Specific).Value.Trim() + "')";

                                            SAPbobsCOM.Recordset oRS1 = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                            oRS1.DoQuery(strQry);

                                            int intSno = 1;
                                            for (int i = 1; i <= oMatrix.RowCount; i++)
                                            {
                                                if (((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_0").Cells.Item(i).Specific).Value != "")
                                                {
                                                    strQry = "Insert Into STR_ParkDetail Values('" + strDocNum + "','" + intSno + "','" + ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_0").Cells.Item(i).Specific).Value + "'";
                                                    strQry += ",'" + ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_2").Cells.Item(i).Specific).Value + "','" + ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_3").Cells.Item(i).Specific).Value + "'";
                                                    strQry += ",'" + ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_7").Cells.Item(i).Specific).Value + "','" + ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_6").Cells.Item(i).Specific).Value + "')";
                                                    oRS1.DoQuery(strQry);
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
                                        oForm.Items.Item("40").Enabled = true;
                                        Initialize(ref oApplication, ref oCompany, ref oForm);
                                    }
                                    else
                                    {
                                        oApplication.SetStatusBarMessage("Please Put Some Park Remarks...", BoMessageTime.bmt_Short, true);
                                    }
                                }

                            }
                            break;
                        case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT:
                            if (pVal.ItemUID == "41" && oForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                            {
                                oForm.Freeze(true);
                                SAPbouiCOM.ButtonCombo oBCombo = (SAPbouiCOM.ButtonCombo)oForm.Items.Item(pVal.ItemUID).Specific;
                                SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                oRS.DoQuery("Select CardCode,FromWhs,ToWhs,Remarks,Receiver,ShpNum  From STR_Park Where DocNum = '" + oBCombo.Selected.Value + "'");
                                if (oRS.RecordCount > 0)
                                {
                                    ((SAPbouiCOM.EditText)oForm.Items.Item("4").Specific).Value = oRS.Fields.Item("CardCode").Value.ToString();
                                    ((SAPbouiCOM.EditText)oForm.Items.Item("6").Specific).Value = oRS.Fields.Item("FromWhs").Value.ToString();
                                    ((SAPbouiCOM.EditText)oForm.Items.Item("8").Specific).Value = oRS.Fields.Item("ToWhs").Value.ToString();
                                    ((SAPbouiCOM.EditText)oForm.Items.Item("24").Specific).Value = oRS.Fields.Item("Remarks").Value.ToString();
                                    ((SAPbouiCOM.ComboBox)oForm.Items.Item("42").Specific).Select(oRS.Fields.Item("Receiver").Value.ToString(),BoSearchKey.psk_ByValue);
                                    ((SAPbouiCOM.EditText)oForm.Items.Item("28").Specific).Value = oRS.Fields.Item("ShpNum").Value.ToString();
                                }

                                string strQry = "";
                                strQry = "Select T0.ItemCode,T2.ItemName,T2.InvntryUom,T0.ShpQty,T0.Qty,T0.BaseDE,T0.BaseLn,IsNull(T3.U_Qty,0) - IsNull(T3.U_RecQty,0) [BalQty]  From STR_ParkDetail T0 ";
                                strQry += " Inner Join STR_Park T1 ON T0.DocNum = T1.DocNum Inner Join OITM T2 ON T0.ItemCode = T2.ItemCode ";
                                strQry += " Inner Join [@EJ_STS1] T3 On T0.BaseDE = T3.DocEntry And T0.BaseLn = T3.LineId Where T0.DocNum = '" + oBCombo.Selected.Value + "' Order By T0.Sno Desc";
                                oRS.DoQuery(strQry);

                                oMatrix.Clear();
                                oChildDataSource.Clear();
                                oMatrix.FlushToDataSource();
                                int introw = 0;
                                while (!oRS.EoF)
                                {

                                    oChildDataSource.SetValue("LineId", oChildDataSource.Offset, (introw + 1).ToString());
                                    oChildDataSource.SetValue("U_ItemCode", oChildDataSource.Offset, oRS.Fields.Item("ItemCode").Value.ToString());
                                    oChildDataSource.SetValue("U_ItemName", oChildDataSource.Offset, oRS.Fields.Item("ItemName").Value.ToString());
                                    oChildDataSource.SetValue("U_UOMCode", oChildDataSource.Offset, oRS.Fields.Item("InvntryUom").Value.ToString());
                                    oChildDataSource.SetValue("U_BaseDE", oChildDataSource.Offset, oRS.Fields.Item("BaseDE").Value.ToString());
                                    oChildDataSource.SetValue("U_BaseLn", oChildDataSource.Offset, oRS.Fields.Item("BaseLn").Value.ToString());
                                    oChildDataSource.SetValue("U_ShpQty", oChildDataSource.Offset, oRS.Fields.Item("ShpQty").Value.ToString());
                                    oChildDataSource.SetValue("U_BalQty", oChildDataSource.Offset, oRS.Fields.Item("BalQty").Value.ToString());
                                    oChildDataSource.SetValue("U_Qty", oChildDataSource.Offset, oRS.Fields.Item("Qty").Value.ToString());

                                    oMatrix.AddRow(1, oMatrix.RowCount - 1);
                                    oRS.MoveNext();
                                    introw += 1;
                                }

                                oForm.Update();
                                oMatrix.FlushToDataSource();                               

                                oRS.DoQuery("Delete From STR_Park Where DocNum = '" + oBCombo.Selected.Value + "'");
                                oRS.DoQuery("Delete From STR_ParkDetail Where DocNum = '" + oBCombo.Selected.Value + "'");
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
        public static void clsStockReceipt_FormDataEvent(ref SAPbouiCOM.Application oApplication, ref SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.BusinessObjectInfo oBusinessObjectInfo, ref  bool BubbleEvent)
        {
            if (blnInitialize != null && blnInitialize.Length >= oForm.TypeCount)
            {
                if (blnInitialize != null && blnInitialize[oForm.TypeCount - 1])
                {
                    oHeaderDataSource = oForm.DataSources.DBDataSources.Item("@EJ_OSTR");
                    oChildDataSource = oForm.DataSources.DBDataSources.Item("@EJ_STR1");
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
                            oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                            string strQry;
                            strQry = "Update T10 Set T10.\"U_RecQty\" = T11.\"Qty\" From \"@EJ_STS1\" T10 Inner Join ";
                            strQry += " (	Select T1.\"U_BaseDE\",T1.\"U_BaseLn\",SUM(T1.\"U_Qty\") \"Qty\" From ";
                            strQry += " 	\"@EJ_STR1\" T1 Inner Join \"@EJ_OSTR\" T2 On T1.\"DocEntry\" = T2.\"DocEntry\"";
                            strQry += "	    Where T2.\"U_ShpNum\" = '" + oHeaderDataSource.GetValue("U_ShpNum", 0).Trim() + "' Group By T1.\"U_BaseDE\",T1.\"U_BaseLn\"";
                            strQry += " ) T11 On T10.\"DocEntry\" = T11.\"U_BaseDE\" And T10.\"LineId\" = T11.\"U_BaseLn\"";
                            oRecordSet.DoQuery(strQry);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region "MenuEvents"
        public static void clsStockReceipt_MenuEvent(ref SAPbouiCOM.Application oApplication, ref SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.MenuEvent pVal, ref bool BubbleEvent)
        {
            try
            {
                oForm.Freeze(true);
                if (blnInitialize != null && blnInitialize.Length >= oForm.TypeCount)
                {
                    if (blnInitialize != null && blnInitialize[oForm.TypeCount - 1])
                    {
                        oHeaderDataSource = oForm.DataSources.DBDataSources.Item("@EJ_OSTR");
                        oHeaderDataSource = oForm.DataSources.DBDataSources.Item("@EJ_STR1");
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
        public static void clsStockReceipt_RightClickEvent(ref SAPbouiCOM.Application oApplication,ref SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.ContextMenuInfo oEventInfo, ref bool BubbleEvent)
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
                string strQryComboFill = "Select \"Code\",\"Name\",\"U_RptName\",\"U_DfltPL\" From \"@EJ_OBCT\" Where \"U_Type\" = 'STR' Order By IsNull(\"U_Dflt\",'N') Desc";
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

        static void Initialize(ref SAPbouiCOM.Application oApplication, ref SAPbobsCOM.Company oCompany, ref SAPbouiCOM.Form oForm)
        {
            try
            {
                string strQry = "";
                string strSlpCode = "-1";
                oForm.Freeze(true);
                SAPbouiCOM.Column oColumn;

                oForm.DataBrowser.BrowseBy = "15";
                oHeaderDataSource = oForm.DataSources.DBDataSources.Item("@EJ_OSTR");
                oChildDataSource = oForm.DataSources.DBDataSources.Item("@EJ_STR1");
                oForm.Items.Item("4").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 1, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                oForm.Items.Item("6").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 1, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                oForm.Items.Item("8").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 1, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
              //  oForm.Items.Item("21").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 1, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                oForm.Items.Item("22").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 1, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                oForm.Items.Item("12").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 1, SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                oForm.Items.Item("15").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, 4, SAPbouiCOM.BoModeVisualBehavior.mvb_True);


                oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                oMatrix = (SAPbouiCOM.Matrix)oForm.Items.Item("22").Specific;
                oMatrix.Columns.Item("V_6").Visible = false;
                oMatrix.Columns.Item("V_7").Visible = false;

                oColumn = oMatrix.Columns.Item("V_0");
                SAPbouiCOM.LinkedButton oLink = (SAPbouiCOM.LinkedButton)oColumn.ExtendedObject;
                oLink.LinkedObject = SAPbouiCOM.BoLinkedObject.lf_Items;

                //For the DocNum
                //strQry = "SELECT \"AutoKey\",\"DfltSeries\" FROM \"ONNM\" WHERE \"ObjectCode\" = 'EJ_OSTR'";
                strQry = "Select IfNull(Max(\"DocNum\"),0) + 1 \"DocNum\" From \"@EJ_OSTR\"";
                oRecordSet.DoQuery(strQry);
                if (!oRecordSet.EoF)
                {
                    oHeaderDataSource.SetValue("DocNum", oHeaderDataSource.Offset, oRecordSet.Fields.Item("DocNum").Value.ToString());
                }

                //strQry = "Select WhsCode From OWHS Where U_TranWhs = 'Y'";
                //oRecordSet.DoQuery(strQry);
                //if (!oRecordSet.EoF)
                //{
                //    oHeaderDataSource.SetValue("U_TrnWhs", oHeaderDataSource.Offset, oRecordSet.Fields.Item("WhsCode").Value.ToString());
                //}

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
                            strSlpCode = oRecordSet.Fields.Item("SalePerson").Value.ToString();
                        }
                        oForm.Items.Item("8").Enabled = true;  
                    }
                    else
                    {
                        strQry = "Select T0.\"Warehouse\",T1.\"WhsName\",T0.\"SalePerson\" FROM \"OUDG\" T0 LEFT JOIN \"OWHS\" T1 ON T1.\"WhsCode\" = T0.\"Warehouse\" ";
                        strQry += " Where T0.\"Code\" = (Select \"DfltsGroup\" FROM \"OUSR\" WHERE \"USERID\" = '" + oCompany.UserSignature.ToString() + "')";
                        oRecordSet.DoQuery(strQry);

                        if (!oRecordSet.EoF)
                        {
                            oHeaderDataSource.SetValue("U_ToWhs", oHeaderDataSource.Offset, oRecordSet.Fields.Item(0).Value.ToString());
                            strSlpCode = oRecordSet.Fields.Item("SalePerson").Value.ToString();
                        }
                        oForm.Items.Item("8").Enabled = false;  
                    }
                }

                oMatrix.AddRow(1, -1);
                ((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_-1").Cells.Item(oMatrix.RowCount).Specific).Value = oMatrix.RowCount.ToString();
                oMatrix.FlushToDataSource();
                oHeaderDataSource.SetValue("U_DocDate", oHeaderDataSource.Offset, System.DateTime.Now.ToString("yyyyMMdd"));

                oMatrix.Columns.Item("V_3").ColumnSetting.SumType = SAPbouiCOM.BoColumnSumType.bst_Auto;

                SAPbouiCOM.ButtonCombo oBCombo = (SAPbouiCOM.ButtonCombo)oForm.Items.Item("41").Specific;

                int pscount = oBCombo.ValidValues.Count;
                for (int j = pscount - 1; j >= 0; j--)
                {
                    oBCombo.ValidValues.Remove(j, SAPbouiCOM.BoSearchKey.psk_Index);
                }

                //strQry = "Select DocNum,ParkRemarks  From STR_Park ";
                //oRecordSet.DoQuery(strQry);
                //while (!oRecordSet.EoF)
                //{
                //    oBCombo.ValidValues.Add(oRecordSet.Fields.Item(0).Value.ToString(), oRecordSet.Fields.Item(1).Value.ToString());
                //    oRecordSet.MoveNext();
                //}

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

        private static bool Validate(SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm)
        {
            try
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
                else if (((SAPbouiCOM.EditText)oForm.Items.Item("17").Specific).Value == "")
                {
                    oApplication.StatusBar.SetText("Select the Posting Date....", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    return false;
                }
                else if (((SAPbouiCOM.ComboBox)oForm.Items.Item("42").Specific).Value == "")
                {
                    oApplication.StatusBar.SetText("Select the Receiver....", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    return false;
                }
                if (oMatrix.RowCount == 0)
                {
                    oApplication.StatusBar.SetText("Add the Items to Proceed....", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    return false;
                }
                for (int i = 1; i <= oMatrix.RowCount; i++)
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
            catch (Exception)
            {              
 
                return false;
            }

        }

        private static void Inventory_Transfer(SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {
            try
            {
                int RetVal;
                string strBranchID = "";
                oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                
                SAPbobsCOM.Documents oTransfer;
                oTransfer = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry);

                string strQry = "Select IfNull(\"BPLid\",0) \"BranchID\" From OWHS Where \"WhsCode\" = '" + ((SAPbouiCOM.EditText)oForm.Items.Item("8").Specific).Value.ToString() + "' ";
                oRecordSet.DoQuery(strQry);
                if (!oRecordSet.EoF)
                {
                    strBranchID = oRecordSet.Fields.Item("BranchID").Value.ToString();
                }

                oTransfer.BPL_IDAssignedToInvoice = Convert.ToInt32(strBranchID);
                oTransfer.UserFields.Fields.Item("U_TrnsType").Value = "R";
                oTransfer.UserFields.Fields.Item("U_TrnsDN").Value = oHeaderDataSource.GetValue("DocNum", 0).Trim().ToString();
                oTransfer.UserFields.Fields.Item("U_TrnsFW").Value = ((SAPbouiCOM.EditText)oForm.Items.Item("6").Specific).Value.ToString();
                oTransfer.UserFields.Fields.Item("U_TrnsTW").Value = ((SAPbouiCOM.EditText)oForm.Items.Item("8").Specific).Value.ToString();

                for (int iIndex = 0; iIndex <= oChildDataSource.Size - 1; iIndex++)
                {
                    String strBillWHS = ((SAPbouiCOM.EditText)oForm.Items.Item("8").Specific).Value.ToString();

                    if (oChildDataSource.GetValue("U_ItemCode", iIndex).Trim() != "")
                    {
                        double dbQty = Convert.ToDouble(oChildDataSource.GetValue("U_Qty", iIndex), System.Globalization.CultureInfo.InvariantCulture);
                        oTransfer.Lines.ItemCode = oChildDataSource.GetValue("U_ItemCode", iIndex).Trim();
                        oTransfer.Lines.WarehouseCode = strBillWHS.Trim();
                        oTransfer.Lines.Quantity = Convert.ToDouble(dbQty);
                        oTransfer.Lines.UserFields.Fields.Item("U_BaseDE").Value = oChildDataSource.GetValue("DocEntry", iIndex).Trim();
                        oTransfer.Lines.UserFields.Fields.Item("U_BaseLn").Value = oChildDataSource.GetValue("LineId", iIndex).Trim();

                        if (oChildDataSource.GetValue("U_ITRDE", iIndex).Trim() != "" && oChildDataSource.GetValue("U_ITRLN", iIndex).Trim() != "")
                        {
                            oTransfer.Lines.UserFields.Fields.Item("U_ITRDE").Value = oChildDataSource.GetValue("U_ITRDE", iIndex).Trim().ToString();
                            oTransfer.Lines.UserFields.Fields.Item("U_ITRLN").Value = oChildDataSource.GetValue("U_ITRLN", iIndex).Trim().ToString();
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
                    String DocEntry = oCompany.GetNewObjectKey();
                    oHeaderDataSource.SetValue("U_TrnsNum", 0, DocEntry);
                    oApplication.StatusBar.SetText("Inventory Transfer Completed Sucessfully", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void FillShipment(SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany, string strDocNum)
        {
            try
            {
                oForm.Freeze(true);
                int RetVal;
                string strQry;
                oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);



                strQry = "Select \"U_CardCode\",\"U_FromWhs\",\"U_ToWhs\",\"U_TrnWhs\",\"U_SlpCode\",\"U_Driver\",\"U_Brand\" From \"@EJ_OSTS\" ";
                strQry += " Where \"DocNum\" = '" + strDocNum + "'";
                oRecordSet.DoQuery(strQry);

                if (!oRecordSet.EoF)
                {
                    oHeaderDataSource.SetValue("U_CardCode", oHeaderDataSource.Offset, oRecordSet.Fields.Item("U_CardCode").Value.ToString());
                    oHeaderDataSource.SetValue("U_FromWhs", oHeaderDataSource.Offset, oRecordSet.Fields.Item("U_FromWhs").Value.ToString());
                    oHeaderDataSource.SetValue("U_ToWhs", oHeaderDataSource.Offset, oRecordSet.Fields.Item("U_ToWhs").Value.ToString());
                    oHeaderDataSource.SetValue("U_TrnWhs", oHeaderDataSource.Offset, oRecordSet.Fields.Item("U_TrnWhs").Value.ToString());
                    oHeaderDataSource.SetValue("U_Driver", oHeaderDataSource.Offset, oRecordSet.Fields.Item("U_Driver").Value.ToString());
                    oHeaderDataSource.SetValue("U_Brand", oHeaderDataSource.Offset, oRecordSet.Fields.Item("U_Brand").Value.ToString());
                }

                oMatrix.Clear();
                strQry = "Select T1.\"DocEntry\",T1.\"LineId\",T1.\"U_ItemCode\",T1.\"U_ItemName\",T1.\"U_Barcode\",T1.\"U_UOMCode\",T1.\"U_Qty\",IfNull(T1.\"U_Qty\",0) - IfNull(T1.\"U_RecQty\",0) \"BalQty\",T1.\"U_BaseDE\",T1.\"U_BaseLn\" From  ";
                strQry += " \"@EJ_OSTS\" T0 Inner Join \"@EJ_STS1\" T1 On T0.\"DocEntry\" = T1.\"DocEntry\" ";
                strQry += " Where T0.\"DocNum\" = '" + strDocNum + "' And IfNull(T1.\"U_ItemCode\",'') <> '' And (IfNull(T1.\"U_Qty\",0) - IfNull(T1.\"U_RecQty\",0) > 0) Order By \"LineId\" Desc";
                oRecordSet.DoQuery(strQry);
                oChildDataSource.Clear();
                oMatrix.FlushToDataSource();
                int intRow;
                while (!oRecordSet.EoF)                
                {
                    intRow = oMatrix.RowCount;
                    oChildDataSource.SetValue("LineId", oChildDataSource.Offset, intRow.ToString());
                    oChildDataSource.SetValue("U_ItemCode", oChildDataSource.Offset, oRecordSet.Fields.Item("U_ItemCode").Value.ToString());
                    oChildDataSource.SetValue("U_ItemName", oChildDataSource.Offset, oRecordSet.Fields.Item("U_ItemName").Value.ToString());
                    oChildDataSource.SetValue("U_UOMCode", oChildDataSource.Offset, oRecordSet.Fields.Item("U_UOMCode").Value.ToString());
                    oChildDataSource.SetValue("U_ShpQty", oChildDataSource.Offset, oRecordSet.Fields.Item("U_Qty").Value.ToString());
                    oChildDataSource.SetValue("U_BalQty", oChildDataSource.Offset, oRecordSet.Fields.Item("BalQty").Value.ToString());
                    oChildDataSource.SetValue("U_DiffQty", oChildDataSource.Offset, oRecordSet.Fields.Item("BalQty").Value.ToString());
                    oChildDataSource.SetValue("U_BarCode", oChildDataSource.Offset, oRecordSet.Fields.Item("U_Barcode").Value.ToString());
                    

                    oChildDataSource.SetValue("U_Qty", oChildDataSource.Offset, "0");
                    oChildDataSource.SetValue("U_BaseDE", oChildDataSource.Offset, oRecordSet.Fields.Item("DocEntry").Value.ToString());
                    oChildDataSource.SetValue("U_BaseLn", oChildDataSource.Offset, oRecordSet.Fields.Item("LineId").Value.ToString());

                    oChildDataSource.SetValue("U_ITRDE", oChildDataSource.Offset, oRecordSet.Fields.Item("U_BaseDE").Value.ToString());
                    oChildDataSource.SetValue("U_ITRLN", oChildDataSource.Offset, oRecordSet.Fields.Item("U_BaseLn").Value.ToString());

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

        #endregion

    }
}
