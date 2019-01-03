using System;
using System.Collections.Generic;
using System.Text;
using SAPbobsCOM;
using System.Collections;
using SAPbouiCOM;
using SapBusinessOneExtensions;
using System.Windows.Forms;
using Utilities;

namespace WarehouseTransfer.Purchase
{
    class clsSupplierPurchaseHistory
    {
        public static SAPbouiCOM.Form oForm;
        public static SAPbouiCOM.Form oSOForm;
        private static SAPbouiCOM.Matrix oMatrix;
        public static SAPbouiCOM.DataTable oDataTable;
        private static SAPbobsCOM.Recordset oRecordSet = null;

        #region "ItemEvent"
        public static void clsSupplierPurchaseHistory_ItemEvent(ref SAPbouiCOM.Application oApplication, ref SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oSetupForm, ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            if (oForm != null)
            {
                oForm = oSetupForm;
                SAPbouiCOM.Grid oGrid;
                oGrid = (SAPbouiCOM.Grid)oForm.Items.Item("4").Specific;
                oMatrix = (SAPbouiCOM.Matrix)oSOForm.Items.Item("38").Specific;
                if (pVal.BeforeAction)
                {
                    switch (pVal.EventType)
                    {
                        //case BoEventTypes.et_FORM_CLOSE:
                        //    BubbleEvent = false;
                        //    oForm.Visible = false;
                        //    break;

                        case BoEventTypes.et_DOUBLE_CLICK:
                            if (pVal.ItemUID == "4")
                            {
                                string strSelCol = "";
                                strSelCol = oGrid.Columns.Item(pVal.ColUID).TitleObject.Caption.ToString();
                                string[] words = strSelCol.Split('-');
                                string[] words1;
                                if (words.Length == 2)
                                {
                                    oForm.Freeze(true);
                                    for (int i = 0; i < oGrid.Rows.Count; i++)
                                    {
                                        if (oGrid.DataTable.GetValue(strSelCol, i).ToString().Trim() != "")
                                        {
                                            if (oGrid.DataTable.GetValue(strSelCol, i).ToString().Trim() != "0")
                                            {
                                                words1 = oGrid.DataTable.GetValue(strSelCol, i).ToString().Trim().Split(' ');
                                                if (words1[0].Trim() != "")
                                                {
                                                    oGrid.DataTable.SetValue("Order Qty", i, words1[0].Trim());
                                                }

                                            }
                                        }
                                    }
                                    oForm.Freeze(false);
                                }

                            }
                            break;

                        case BoEventTypes.et_ITEM_PRESSED:
                            if (pVal.ItemUID == "2")
                            {
                                BubbleEvent = false;
                                oForm.Visible = false;
                            }
                            if (pVal.ItemUID == "3")
                            {

                                flushAll();
                                BubbleEvent = false;
                                oForm.Visible = false;
                                return;
                                oMatrix.Clear();
                                int j = oMatrix.RowCount;
                                if (j == 0)
                                    oMatrix.AddRow(1, -1);
                                j = oMatrix.RowCount;

                                for (int i = 0; i < oGrid.Rows.Count; i++)
                                {
                                    if (oGrid.DataTable.GetValue("Order Qty", i).ToString().Trim() != "")
                                    {
                                        if (oGrid.DataTable.GetValue("Order Qty", i).ToString().Trim() != "0")
                                        {
                                            string strItemCode = oGrid.DataTable.GetValue("ItemCode", i).ToString().Trim();
                                            string strQty = oGrid.DataTable.GetValue("Order Qty", i).ToString().Trim();
                                            string strPrice = oGrid.DataTable.GetValue("Current Purchase Price", i).ToString().Trim();
                                            string strUoM = oGrid.DataTable.GetValue("UoM", i).ToString().Trim();
                                            try
                                            {
                                                ((SAPbouiCOM.EditText)oMatrix.Columns.Item("1").Cells.Item(j).Specific).Value = strItemCode;
                                                ((SAPbouiCOM.EditText)oMatrix.Columns.Item("11").Cells.Item(j).Specific).Value = strQty;
                                                try
                                                {
                                                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("1470002145").Cells.Item(j).Specific).Value = strUoM;
                                                }
                                                catch (Exception)
                                                {
                                                }
                                                try
                                                {
                                                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item("14").Cells.Item(j).Specific).Value = strPrice;
                                                }
                                                catch (Exception)
                                                {
                                                }
                                                j++;
                                            }
                                            catch (Exception)
                                            {
                                                j++;
                                            }
                                        }
                                    }
                                }

                                //oForm.Close();
                                oForm.Visible = false;
                            }
                            break;

                        case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST:
                            if (pVal.ItemUID == "4")
                            {
                                if (pVal.ColUID == "UoM")
                                {
                                    if (oGrid.DataTable.GetValue("ItemCode", pVal.Row).ToString().Trim() != "")
                                    {
                                        string squery = "Select T2.\"UomCode\",T2.\"UomName\" From OITM T0 Inner Join UGP1 T1 On T0.\"UgpEntry\" = T1.\"UgpEntry\" Inner Join OUOM T2 On T1.\"UomEntry\" = T2.\"UomEntry\" Where T0.\"ItemCode\" = '" + oGrid.DataTable.GetValue("ItemCode", pVal.Row).ToString().Trim() + "'";
                                        Utilities.UtilitiesCls.CFLConditionQuery(ref oApplication, ref oCompany, oForm, ref pVal, squery, "UomCode", "10010198", "UomCode", false, true, "4", "UoM", false);
                                    }
                                }
                                if (pVal.ColUID == "ItemCode")
                                {
                                    SAPbouiCOM.ChooseFromListCollection oCFLs = null;
                                    SAPbouiCOM.Conditions oCons = null;
                                    SAPbouiCOM.Condition oCon = null;
                                    oCFLs = oForm.ChooseFromLists;
                                    SAPbouiCOM.ChooseFromList oCFL = null;
                                    oCFL = oCFLs.Item("CFL_2");

                                    oCons = new Conditions();
                                    oCFL.SetConditions(oCons);


                                    oCons = oCFL.GetConditions();




                                    oCon = oCons.Add();
                                    oCon.Alias = "PrchseItem";
                                    oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                                    oCon.CondVal = "Y";

                                    oCon.Relationship = BoConditionRelationship.cr_AND;
                                    oCon = oCons.Add();
                                    oCon.Alias = "frozenFor";
                                    oCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_EQUAL;
                                    oCon.CondVal = "Y";


                                    int intCnt = 2;
                                    for (int i = 0; i < oGrid.Rows.Count; i++)
                                    {
                                        if (oGrid.DataTable.GetValue("ItemCode", i).ToString().Trim() != "")
                                        {
                                            if (intCnt > 0)
                                            {
                                                oCon.Relationship = BoConditionRelationship.cr_AND;
                                            }
                                            oCon = oCons.Add();
                                            oCon.Alias = "ItemCode";
                                            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_EQUAL;
                                            oCon.CondVal = oGrid.DataTable.GetValue("ItemCode", i).ToString().Trim();
                                            intCnt += 1;
                                        }
                                    }

                                    oCFL.SetConditions(oCons);
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

                        case BoEventTypes.et_ITEM_PRESSED:
                            break;
                        case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST:
                            if (oForm.Mode != SAPbouiCOM.BoFormMode.fm_FIND_MODE)
                            {
                                if (pVal.ItemUID == "4")
                                {
                                    if (pVal.ColUID == "ItemCode")
                                    {
                                        oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                                        string strQry = "";
                                        string strItemCode = "";
                                        oDataTable = UtilitiesCls.DataTable(ref oApplication, ref oCompany, oForm, ref pVal);
                                        if (oDataTable != null)
                                        {
                                            strItemCode = oDataTable.GetValue("ItemCode", 0).ToString();
                                        }
                                        if (strItemCode != "")
                                        {
                                            strQry = " Exec EJ_LoadBPItemDetails '" + ((SAPbouiCOM.EditText)oForm.Items.Item("6").Specific).Value.Trim() + "', '" + strItemCode + "'";
                                            oRecordSet.DoQuery(strQry);
                                            if (!oRecordSet.EoF)
                                            {
                                                oGrid.DataTable.SetValue("ItemCode", pVal.Row, strItemCode);
                                                oGrid.DataTable.SetValue("ItemName", pVal.Row, oRecordSet.Fields.Item("ItemName").Value.ToString());
                                                oGrid.DataTable.SetValue("Order Qty", pVal.Row, "");
                                                oGrid.DataTable.SetValue("UoM", pVal.Row, oRecordSet.Fields.Item("UoM").Value.ToString());
                                                oGrid.DataTable.SetValue("Current Purchase Price", pVal.Row, oRecordSet.Fields.Item("Current Price").Value.ToString());
                                            }
                                        }

                                        oGrid.DataTable.SetValue("ItemCode", pVal.Row, strItemCode);
                                        int j = oGrid.Rows.Count;
                                        if (j == oGrid.Rows.Count)
                                            oGrid.DataTable.Rows.Add();
                                    }

                                    if (pVal.ColUID == "UoM")
                                    {
                                        oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                                        string strUoMCode = "";
                                        oDataTable = UtilitiesCls.DataTable(ref oApplication, ref oCompany, oForm, ref pVal);
                                        if (oDataTable != null)
                                        {
                                            strUoMCode = oDataTable.GetValue("UomCode", 0).ToString();
                                        }
                                        oGrid.DataTable.SetValue("UoM", pVal.Row, strUoMCode);
                                        string strQry = " Exec EJ_LoadUoMPrice '" + ((SAPbouiCOM.EditText)oForm.Items.Item("6").Specific).Value.Trim() + "','" + oGrid.DataTable.GetValue("ItemCode", pVal.Row).ToString().Trim() + "' ,'" + strUoMCode + "'";
                                        oRecordSet.DoQuery(strQry);
                                        if (!oRecordSet.EoF)
                                        {
                                            oGrid.DataTable.SetValue("Current Purchase Price", pVal.Row, oRecordSet.Fields.Item("Current Price").Value.ToString());
                                        }
                                    }
                                }
                            }
                            break;
                        case BoEventTypes.et_COMBO_SELECT:

                            break;
                        case BoEventTypes.et_FORM_CLOSE:
                            oForm = null;
                            break;
                        default:
                            break;

                    }
                }
            }
        }
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

        #endregion


        private static void flushAll()
        {

            SAPbouiCOM.Grid oGrid;
            oGrid = (SAPbouiCOM.Grid)oForm.Items.Item("4").Specific;
            SAPbouiCOM.DataTable dtHistory = oGrid.DataTable;
            oSOForm.Freeze(true);

            try
            {
                using (var progress = SboProgressBar.Create("Please Wait while system prepare sales order for selected item(s) ", 100, oForm, 400))
                {
                   



                    System.Data.DataTable dtSelItems = new System.Data.DataTable();
                    dtSelItems.Columns.Add("ItemCode");
                    dtSelItems.Columns.Add("Quantity");
                    dtSelItems.Columns.Add("Price");
                    dtSelItems.Columns.Add("UOM");




                    int rowCount = 1;

                    for (int dtrown = 0; dtrown < dtHistory.Rows.Count; dtrown++)
                    {
                        string strQty = dtHistory.GetValue("Order Qty", dtrown).ToString().Trim();

                        if (strQty != "0" && strQty != "")
                        {
                            string strItemCode = oGrid.DataTable.GetValue("ItemCode", dtrown).ToString().Trim();
                            string strPrice = oGrid.DataTable.GetValue("Current Purchase Price", dtrown).ToString().Trim();
                            string strUoM = oGrid.DataTable.GetValue("UoM", dtrown).ToString().Trim();

                            dtSelItems.Rows.Add(strItemCode, strQty, strPrice, strUoM);



                            rowCount++;

                        }
                    }

                    if (dtSelItems.Rows.Count > 0)
                    {
                        for (int m = oMatrix.RowCount; m >= 1; m--)
                        {

                            oMatrix.DeleteRow(1);

                        }

                        progress.Value += 10;

                        rowCount = 1;
                        oMatrix.AddRow(dtSelItems.Rows.Count);
                        int incrmentVal = 90 / dtSelItems.Rows.Count;

                        foreach (System.Data.DataRow dr in dtSelItems.Rows)
                        {

                            
                            if (progress.Value <= 100) progress.Value += incrmentVal;
                            progress.Text = "Please Wait while system prepare document for selected item(s) " + dr["ItemCode"].ToString();
                            ((SAPbouiCOM.EditText)oMatrix.GetCellSpecific("1", rowCount)).Value = dr["ItemCode"].ToString();
                            ((SAPbouiCOM.EditText)oMatrix.GetCellSpecific("11", rowCount)).Value = dr["Quantity"].ToString();
                            try
                            {
                                ((SAPbouiCOM.EditText)oMatrix.GetCellSpecific("1470002145", rowCount)).Value = dr["UOM"].ToString();

                            }
                            catch (Exception)
                            {
                            }
                            try
                            {
                                ((SAPbouiCOM.EditText)oMatrix.GetCellSpecific("14", rowCount)).Value = dr["Price"].ToString();


                            }
                            catch (Exception)
                            {
                            }
                            
                            rowCount++;
                        }

                       

                           
                    }

                }



            }
            catch (Exception ex)
            {

            }
            finally
            {
                oSOForm.Freeze(false);
            }

        }

    }
}
