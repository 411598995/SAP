using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using SAPbobsCOM;
using System.Collections;
using SAPbouiCOM;
using SapBusinessOneExtensions;
using System.Windows.Forms;
using Utilities;

using System.Linq;

using System.Text.RegularExpressions;



namespace WarehouseTransfer.Sales
{
    class clsCustomerSalesHistory
    {
        public static SAPbouiCOM.Form oForm;
        public static SAPbouiCOM.Form oSOForm;
       public static  SAPbobsCOM.Company oCompany;
        public static SAPbouiCOM.Application oApplication;
        public static int myDocNum=0;
        public static SAPbouiCOM.DBDataSource ORDR;
        public static string cardCode;
        private static SAPbouiCOM.Matrix oMatrix;
        public static SAPbouiCOM.DataTable oDataTable;
        public static System.Data.DataTable dtActionItems;
        private static SAPbobsCOM.Recordset oRecordSet = null;
        public static System.Threading.Timer aTimer;
        public static bool busy = false;
        public static bool isAdding = false;
        private static bool boolAdded = false;
        public static string CallingForm = "";
      
        #region "ItemEvent"
        public static void clsCustomerSalesHistory_ItemEvent(ref SAPbouiCOM.Application oApplication, ref SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oSetupForm, ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            if (oForm != null)
            {
               

                SAPbouiCOM.Grid oGrid;
                oGrid = (SAPbouiCOM.Grid)oForm.Items.Item("4").Specific;
               // oSOForm.Freeze(true);
                oMatrix = (SAPbouiCOM.Matrix)oSOForm.Items.Item("38").Specific;
             //   oMatrix.LoadFromDataSource();
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

                                                    if (oGrid.DataTable.GetValue("Order Qty", i).ToString().Trim() != "")
                                                    {
                                                        if (oGrid.DataTable.GetValue("Order Qty", i).ToString().Trim() != "0")
                                                        {
                                                            string strItemCode = oGrid.DataTable.GetValue("ItemCode", i).ToString().Trim();
                                                            string strQty = oGrid.DataTable.GetValue("Order Qty", i).ToString().Trim();
                                                            string strPrice = oGrid.DataTable.GetValue("Current Selling Price", i).ToString().Trim();
                                                            string strUoM = oGrid.DataTable.GetValue("UoM", i).ToString().Trim();

                                                            string strExistingQty = "";
                                                            string strExistingPrice = "";
                                                            string strExistingUoM = "";

                                                            bool boolItemExist = false;
                                                            int intMatrixLine = 0;

                                                            /* disabling placing item on order - Debug My Work

                                                            for (int m = 1; m <= oMatrix.RowCount; m++)
                                                            {
                                                                if (((SAPbouiCOM.EditText)oMatrix.Columns.Item("1").Cells.Item(m).Specific).Value.Trim() == strItemCode)
                                                                {
                                                                    boolItemExist = true;
                                                                    intMatrixLine = m;

                                                                    strExistingQty = ((SAPbouiCOM.EditText)oMatrix.Columns.Item("11").Cells.Item(m).Specific).Value.Trim();
                                                                    strExistingPrice = ((SAPbouiCOM.EditText)oMatrix.Columns.Item("14").Cells.Item(m).Specific).Value.Trim();
                                                                    strExistingUoM = ((SAPbouiCOM.EditText)oMatrix.Columns.Item("1470002145").Cells.Item(m).Specific).Value.Trim();
                                                                }
                                                            }

                                                            if (boolItemExist == false)
                                                            {
                                                                int j = oMatrix.RowCount;
                                                                if (j == 0)
                                                                {
                                                                    oMatrix.AddRow(1, -1);
                                                                }
                                                                intMatrixLine = oMatrix.RowCount;
                                                            }

                                                            if (boolItemExist == false)
                                                            {
                                                                ((SAPbouiCOM.EditText)oMatrix.Columns.Item("1").Cells.Item(intMatrixLine).Specific).Value = strItemCode;
                                                            }
                                                            ((SAPbouiCOM.EditText)oMatrix.Columns.Item("11").Cells.Item(intMatrixLine).Specific).Value = strQty;
                                                            try
                                                            {
                                                                ((SAPbouiCOM.EditText)oMatrix.Columns.Item("1470002145").Cells.Item(intMatrixLine).Specific).Value = strUoM;
                                                            }
                                                            catch (Exception)
                                                            {
                                                            }
                                                            try
                                                            {
                                                                ((SAPbouiCOM.EditText)oMatrix.Columns.Item("14").Cells.Item(intMatrixLine).Specific).Value = strPrice;
                                                            }
                                                            catch (Exception)
                                                            {
                                                            }
                                                             * 
                                                           */

                                                        }
                                                    }

                                                }
                                            }
                                        }
                                    }
                                    oForm.Freeze(false);
                                }
                            }

                             //string s = "";
                             //s = oGrid.Columns.Item(pVal.ColUID).TitleObject.Caption.ToString();
                             //string[] words = s.Split('-');
                             //if (words.Length == 2)
                             //{
                             //    string strDocNum = "";
                             //    strDocNum = words.GetValue(1).ToString();

                             //    oMatrix.Clear();
                             //    int j = oMatrix.RowCount;
                             //    if (j == 0)
                             //        oMatrix.AddRow(1, -1);
                             //    j = oMatrix.RowCount;

                             //    oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                             //    string strQry =  "SELECT T0.\"DocNum\",T1.\"ItemCode\",T1.\"Quantity\",T1.\"Price\",T1.\"UomCode\" From ORDR T0 Inner Join RDR1 T1 On T0.\"DocEntry\"  = T1.\"DocEntry\" Where T0.\"DocNum\" = '"+ strDocNum.ToString() +"'";
                             //    oRecordSet.DoQuery(strQry);
                             //    while (!oRecordSet.EoF)
                             //    {

                             //        try
                             //        {
                             //            ((SAPbouiCOM.EditText)oMatrix.Columns.Item("1").Cells.Item(j).Specific).Value = oRecordSet.Fields.Item("ItemCode").Value.ToString().Trim();
                             //            ((SAPbouiCOM.EditText)oMatrix.Columns.Item("11").Cells.Item(j).Specific).Value = oRecordSet.Fields.Item("Quantity").Value.ToString().Trim();
                             //            try
                             //            {
                             //                ((SAPbouiCOM.EditText)oMatrix.Columns.Item("1470002145").Cells.Item(j).Specific).Value = oRecordSet.Fields.Item("UomCode").Value.ToString().Trim(); 
                             //            }
                             //            catch (Exception)
                             //            {
                             //            }
                             //            try
                             //            {
                             //                ((SAPbouiCOM.EditText)oMatrix.Columns.Item("14").Cells.Item(j).Specific).Value = oRecordSet.Fields.Item("Price").Value.ToString().Trim(); 
                             //            }
                             //            catch (Exception)
                             //            {
                             //            }
                             //            j++;
                             //        }
                             //        catch (Exception)
                             //        {
                             //            j++;
                             //        }
                             //        oRecordSet.MoveNext();
                             //    }

                             //    oForm.Visible = false;
                             //}
                            break;

                        case BoEventTypes.et_ITEM_PRESSED:
                            if (pVal.ItemUID == "2")
                            {
                                BubbleEvent = false;
                                oForm.Visible = false;
                            }
                            if (pVal.ItemUID == "3")
                            {

                                if (CallingForm == "SO")
                                {

                                    if (isAdding)
                                    {
                                        int orderAdded = flushAll();
                                        if (orderAdded == 1)
                                        {
                                            oForm.Visible = false;

                                            oSOForm.Mode = BoFormMode.fm_OK_MODE;
                                            oSOForm.Mode = BoFormMode.fm_FIND_MODE;
                                            
                                            oApplication.SendKeys(myDocNum.ToString());
                                            oSOForm.ActiveItem = "1";
                                            oApplication.SendKeys("{ENTER}");
                                            //System.Windows.Forms.SendKeys.Send("{ENTER}");
                                        }
                                    }
                                    else
                                    {
                                        int orderAdded = flushAll();
                                        if (orderAdded == 1)
                                        {
                                            oForm.Visible = false;

                                            oSOForm.Mode = BoFormMode.fm_OK_MODE;
                                            oApplication.ActivateMenuItem("1304");
                                        }
                                    }
                                }
                                else
                                {

                                    flushAllCN();
                                    BubbleEvent = false;
                                    oForm.Visible = false;
                                    return;
                                }
                                //  oSOForm.Freeze(true);

                                // oSOForm.Freeze(false);
                            }
                            if (pVal.ItemUID == "38")
                            {
                                //SAPbouiCOM.Grid oGrid;
                                //oGrid = (SAPbouiCOM.Grid)oForm.Items.Item("4").Specific;
                                //oMatrix = (SAPbouiCOM.Matrix)oSOForm.Items.Item("38").Specific;

                                

                                //oMatrix.Clear();

                                for (int m = oMatrix.RowCount; m >= 1; m--)
                                {
                                    if (((SAPbouiCOM.EditText)oMatrix.Columns.Item("11").Cells.Item(m).Specific).Value.Trim() == ((SAPbouiCOM.EditText)oMatrix.Columns.Item("32").Cells.Item(m).Specific).Value.Trim())
                                    {
                                        oMatrix.DeleteRow(m);
                                    }
                                }
                                
                                int j = oMatrix.RowCount;
                                if (j == 0)
                                {
                                    oMatrix.AddRow(1, -1);
                                }
                                else
                                {
                                    oMatrix.AddRow(1, -1);
                                }
                                    
                                j = oMatrix.RowCount;

                                for (int i = 0; i < oGrid.Rows.Count; i++)
                                {
                                    if (oGrid.DataTable.GetValue("Order Qty", i).ToString().Trim() != "")
                                    {
                                        if (oGrid.DataTable.GetValue("Order Qty", i).ToString().Trim() != "0")
                                        {
                                            string strItemCode = oGrid.DataTable.GetValue("ItemCode", i).ToString().Trim();
                                            string strQty = oGrid.DataTable.GetValue("Order Qty", i).ToString().Trim();
                                            string strPrice = oGrid.DataTable.GetValue("Current Selling Price", i).ToString().Trim();
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
                                    } oMatrix.SetLineData(i);                                
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
                                    oCon.Alias = "SellItem";
                                    oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                                    oCon.CondVal = "Y";

                                    oCon.Relationship = BoConditionRelationship.cr_AND;
                                    oCon = oCons.Add();
                                    oCon.Alias = "frozenFor";
                                    oCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_EQUAL;
                                    oCon.CondVal = "Y";


                                    int intCnt = 1;
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
                        case SAPbouiCOM.BoEventTypes.et_GOT_FOCUS:

                            if (pVal.ItemUID == "4" && pVal.ColUID == "Order Qty" && pVal.Row > 0 && pVal.Row < oGrid.Rows.Count)
                            {
                                oForm.Freeze(true);
                                try
                                {
                                    oGrid.Rows.SelectedRows.Clear();
                                    oGrid.Rows.SelectedRows.Add(pVal.Row);

                                    SAPbobsCOM.ItemPriceReturnParams itemPrice = clsSBO.getUnitPriceSys(oCompany, oGrid.DataTable.GetValue("ItemCode", pVal.Row).ToString().Trim(), cardCode, 1, DateTime.Now.Date, oGrid.DataTable.GetValue("UoM", pVal.Row).ToString().Trim());
                                    double price = itemPrice.Price;
                                    double discount = itemPrice.Discount;
                                    double discountedPrice = price - (price * discount / 100);

                                    oGrid.DataTable.SetValue("Current Selling Price", pVal.Row, discountedPrice);
                                }
                                catch(Exception ex) {
                                    oApplication.SetStatusBarMessage(ex.Message);
                                }
                                oForm.Freeze(false);
                            }



                            break;

                        case SAPbouiCOM.BoEventTypes.et_LOST_FOCUS:
                           
                            if (pVal.ItemUID == "4")
                            {
                               

                               return;
                                if (((SAPbouiCOM.EditText)oSOForm.Items.Item("4").Specific).Value.Trim() != "")
                                {
                                    if (pVal.ColUID == "Order Qty")
                                    {
                                        if (oGrid.DataTable.GetValue("Order Qty", pVal.Row).ToString().Trim() == "" || oGrid.DataTable.GetValue("Order Qty", pVal.Row).ToString().Trim() == "0")
                                        {
                                            string strItemCode = oGrid.DataTable.GetValue("ItemCode", pVal.Row).ToString().Trim();
                                            for (int m = oMatrix.RowCount; m >= 1; m--)
                                            {
                                                if (((SAPbouiCOM.EditText)oMatrix.Columns.Item("1").Cells.Item(m).Specific).Value.Trim() == strItemCode)
                                                {
                                                    oMatrix.DeleteRow(m);
                                                }
                                            }
                                        }
                                    }
                                }

                            
                                if (cardCode != "")
                                {
                                    if (pVal.ColUID == "Order Qty" || pVal.ColUID == "UoM" || pVal.ColUID == "Current Selling Price")
                                    {
                                        if (oGrid.DataTable.GetValue("Order Qty", pVal.Row).ToString().Trim() != "")
                                        {
                                            if (oGrid.DataTable.GetValue("Order Qty", pVal.Row).ToString().Trim() != "0")
                                            {
                                                string strItemCode = oGrid.DataTable.GetValue("ItemCode", pVal.Row).ToString().Trim();
                                                string strQty = oGrid.DataTable.GetValue("Order Qty", pVal.Row).ToString().Trim();
                                                string strPrice = oGrid.DataTable.GetValue("Current Selling Price", pVal.Row).ToString().Trim();
                                                string strUoM = oGrid.DataTable.GetValue("UoM", pVal.Row).ToString().Trim();

                                                string strExistingQty = "";
                                                string strExistingPrice = "";
                                                string strExistingUoM = "";

                                                bool boolItemExist = false;
                                                int intMatrixLine = 0;

                                                // Debug My Work 



                                                for (int m = 1; m <= oMatrix.RowCount; m++)
                                                {
                                                    string strMatitemCode = ((SAPbouiCOM.EditText)oMatrix.GetCellSpecific("1", m)).Value.Trim();
                                                    if (strMatitemCode == strItemCode)
                                                    {
                                                        boolItemExist = true;
                                                        intMatrixLine = m;

                                                        strExistingQty = ((SAPbouiCOM.EditText)oMatrix.GetCellSpecific("11", m)).Value.Trim();
                                                        strExistingPrice = ((SAPbouiCOM.EditText)oMatrix.GetCellSpecific("14", m)).Value.Trim();
                                                        strExistingUoM = ((SAPbouiCOM.EditText)oMatrix.GetCellSpecific("1470002145", m)).Value.Trim();
                                                    }
                                                }
                                                oSOForm.Freeze(true);
                                             
                                                if (boolItemExist == false)
                                                {
                                                    int j = oMatrix.RowCount;
                                                    if (j == 0)
                                                    {
                                                        oMatrix.AddRow(1, -1);
                                                    }
                                                    intMatrixLine = oMatrix.RowCount;
                                                   
                                                }
                                                if (boolItemExist == false)
                                                {
                                                    dtActionItems.Rows.Add(dtActionItems.Rows.Count, strItemCode, strQty,strPrice, strUoM, "Add");
                                                   oSOForm.Freeze(false);
                                                    return;
                                                    ((SAPbouiCOM.EditText)oMatrix.GetCellSpecific("1", intMatrixLine)).Value = strItemCode;
                                                }
                                             
                                                ((SAPbouiCOM.EditText)oMatrix.GetCellSpecific("11", intMatrixLine)).Value = strQty;
                                           

                                                try
                                                {
                                                    ((SAPbouiCOM.EditText)oMatrix.GetCellSpecific("1470002145", intMatrixLine)).Value = strUoM;
                                           
                                                }
                                                catch (Exception)
                                                {
                                                }
                                                try
                                                {
                                                   ((SAPbouiCOM.EditText)oMatrix.GetCellSpecific("14", intMatrixLine)).Value = strPrice;
                                           
                                          
                                                }
                                                catch (Exception)
                                                {
                                                }
                                                oSOForm.Freeze(false);

                                               
                                            }
                                        }
                                    }
                                }
                                oForm.Freeze(false);
                            }
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
                                                try
                                                {
                                                    oGrid.DataTable.SetValue("ItemCode", pVal.Row, strItemCode);
                                                    oGrid.DataTable.SetValue("ItemName", pVal.Row, oRecordSet.Fields.Item("ItemName").Value.ToString());
                                                    oGrid.DataTable.SetValue("Order Qty", pVal.Row, "");
                                                    oGrid.DataTable.SetValue("UoM", pVal.Row, oRecordSet.Fields.Item("UoM").Value.ToString());

                                                    SAPbobsCOM.ItemPriceReturnParams itemPrice = clsSBO.getUnitPriceSys(oCompany, strItemCode, cardCode, 1, DateTime.Now.Date, oRecordSet.Fields.Item("UoM").Value.ToString());

                                                    double price = itemPrice.Price;
                                                    double discount = itemPrice.Discount;
                                                    double discountedPrice = price - (price * discount / 100);


                                                    oGrid.DataTable.SetValue("Current Selling Price", pVal.Row, discountedPrice);
                                                }
                                                catch (Exception ex)
                                                {
                                                    oApplication.SetStatusBarMessage(ex.Message);
                                                }
                                                // oGrid.DataTable.SetValue("Current Selling Price", pVal.Row, oRecordSet.Fields.Item("Current Price").Value.ToString());

                                            }
                                        }

                                        oGrid.DataTable.SetValue("ItemCode", pVal.Row, strItemCode);
                                        int j = oGrid.Rows.Count;
                                        if (j == pVal.Row + 1)
                                            oGrid.DataTable.Rows.Add();
                                    }
                                    if (pVal.ColUID == "UoM")
                                    {
                                        try
                                        {
                                            oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                                            string strUoMCode = "";
                                            oDataTable = UtilitiesCls.DataTable(ref oApplication, ref oCompany, oForm, ref pVal);
                                            if (oDataTable != null)
                                            {
                                                strUoMCode = oDataTable.GetValue("UomCode", 0).ToString();
                                            }
                                            oGrid.DataTable.SetValue("UoM", pVal.Row, strUoMCode);



                                            SAPbobsCOM.ItemPriceReturnParams itemPrice = clsSBO.getUnitPriceSys(oCompany, oGrid.DataTable.GetValue("ItemCode", pVal.Row).ToString().Trim(), cardCode, 1, DateTime.Now.Date, strUoMCode);
                                            double price = itemPrice.Price;
                                            double discount = itemPrice.Discount;
                                            double discountedPrice = price - (price * discount / 100);
                                            oGrid.DataTable.SetValue("Current Selling Price", pVal.Row, discountedPrice);
                                        }
                                        catch (Exception ex)
                                        {
                                            oApplication.SetStatusBarMessage(ex.Message);
                                        }

                                    }
                                }
                            }
                            break;

                        case BoEventTypes.et_ITEM_PRESSED:
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



        private static int flushAll()
        {

            int result = -1;
            SAPbouiCOM.Grid oGrid;
            oGrid = (SAPbouiCOM.Grid)oForm.Items.Item("4").Specific;
            SAPbouiCOM.DataTable dtHistory = oGrid.DataTable;
            SAPbouiCOM.StaticText lblStatus = (SAPbouiCOM.StaticText)oForm.Items.Item("lblStatus").Specific;
            oSOForm.Freeze(true);

            try
            {
                using (var progress = SboProgressBar.Create("Please Wait while system prepare document for selected item(s) ", 100, oForm, 400))
                {
                   



                    System.Data.DataTable dtSelItems = new System.Data.DataTable();
                    dtSelItems.Columns.Add("ItemCode");
                    dtSelItems.Columns.Add("Quantity");
                    dtSelItems.Columns.Add("Price");
                    dtSelItems.Columns.Add("UOM");



                    if (cardCode != "")
                    {
                        int rowCount = 1;

                        for (int dtrown = 0; dtrown < dtHistory.Rows.Count; dtrown++)
                        {
                            string strQty = dtHistory.GetValue("Order Qty", dtrown).ToString().Trim();

                            if (strQty != "0" && strQty != "")
                            {
                                string strItemCode = oGrid.DataTable.GetValue("ItemCode", dtrown).ToString().Trim();
                                string strUoM = oGrid.DataTable.GetValue("UoM", dtrown).ToString().Trim();

                                string strPrice = oGrid.DataTable.GetValue("Current Selling Price", dtrown).ToString().Trim();  

                               dtSelItems.Rows.Add(strItemCode, strQty, strPrice, strUoM);



                                rowCount++;

                            }
                        }

                        if (dtSelItems.Rows.Count > 0)
                        {
                            /*
                            for (int m = oMatrix.RowCount; m >= 1; m--)
                            {

                                oMatrix.DeleteRow(1);

                            }
                            */
                            progress.Value += 10;
                            int incrmentVal = 90 / dtSelItems.Rows.Count;

                            /*
                           rowCount = 1;
                           oMatrix.AddRow(dtSelItems.Rows.Count);
                           

                           foreach (System.Data.DataRow dr in dtSelItems.Rows)
                           {
                               if (progress.Value <= 100) progress.Value += incrmentVal;
                               progress.Text = "Please Wait while system prepare sales order for selected item(s) " + dr["ItemCode"].ToString();
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

                           */

                            string outStr = "";
                            SAPbobsCOM.Documents Doc = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
                            SAPbobsCOM.BusinessPartners bp = (SAPbobsCOM.BusinessPartners)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
                            ORDR = oSOForm.DataSources.DBDataSources.Item("ORDR");

                            if (!isAdding)
                            {
                                int DE = getDOcEntry(myDocNum.ToString());

                                Doc.GetByKey(DE);
                                int oldLineCnt = Doc.Lines.Count;
                                for (int oldLine = 0; oldLine < oldLineCnt; oldLine++)
                                {
                                    Doc.Lines.SetCurrentLine(0);
                                    Doc.Lines.Delete();
                                }
                            }
                            DateTime dtdel = DateTime.Now.Date;

                            Doc.CardCode = cardCode;
                            Doc.DocDate = DateTime.Now;
                            string numAtCard = ORDR.GetValue("NumAtCard", 0).ToString();
                            string strdocDueDate = ORDR.GetValue("DocDueDate", 0).ToString();
                            string strdocDate = ORDR.GetValue("DocDate", 0).ToString();

                            DateTime dueDate = strdocDueDate==""?DateTime.Now.Date:   DateTime.ParseExact(strdocDueDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                            DateTime docdate = strdocDate == "" ? DateTime.Now.Date : DateTime.ParseExact(strdocDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);


                            Doc.NumAtCard = ORDR.GetValue("NumAtCard", 0).ToString();
                            Doc.DocDueDate = dueDate;
                            Doc.DocDate = docdate;
                            //Doc.DocDueDate = dtdel.Date;
                            
                            
                           foreach (System.Data.DataRow dr in dtSelItems.Rows)
                            {
                                if (progress.Value <= 100) progress.Value += incrmentVal;
                                int existItemLine = -1;
                                string itemCode = Convert.ToString(dr["ItemCode"].ToString());
                                if (itemCode != "" && Convert.ToDouble(dr["Quantity"]) > 0)
                                {

                                    if (!isAdding)
                                    {

                                        existItemLine = getCurrentLine(Doc, itemCode);
                                        existItemLine = -1;
                                        if (existItemLine > -1)
                                        {
                                            Doc.Lines.SetCurrentLine(existItemLine);
                                            Doc.Lines.Quantity = Convert.ToDouble(dr["Quantity"]);
                                            Doc.Lines.UnitPrice = Convert.ToDouble(dr["Price"]);
                                            Doc.Lines.UoMEntry = clsSBO. getUomEntry(oCompany, dr["UOM"].ToString());
                                            
                                        }
                                        else
                                        {
                                          //  Doc.Lines.SetCurrentLine(Doc.Lines.Count-1);
                                            Doc.Lines.ItemCode = Convert.ToString(itemCode);
                                            Doc.Lines.Quantity = Convert.ToDouble(dr["Quantity"]);
                                            Doc.Lines.UnitPrice = Convert.ToDouble(dr["Price"]);
                                            Doc.Lines.UoMEntry = clsSBO.getUomEntry(oCompany, dr["UOM"].ToString());
                                            Doc.Lines.Add();


                                        }
                                    }
                                    else
                                    {
                                        Doc.Lines.ItemCode = Convert.ToString(itemCode);
                                        Doc.Lines.Quantity = Convert.ToDouble(dr["Quantity"]);
                                        Doc.Lines.UnitPrice = Convert.ToDouble(dr["Price"]);
                                        Doc.Lines.UoMEntry = clsSBO.getUomEntry(oCompany, dr["UOM"].ToString());
                                        Doc.Lines.Add();
                                    }
                                 
                                   
                                }
                            }

                            int docResult = isAdding == true ? Doc.Add() : Doc.Update();

                            if (docResult != 0)
                            {
                                int erroCode = 0;
                                string errDescr = "";
                                oCompany.GetLastError(out erroCode, out errDescr);
                                outStr = "Error:" + errDescr + outStr;
                                oApplication.StatusBar.SetText("Failed to add Order  : " + errDescr);

                            }
                            else
                            {
                                result = 1;
                                if (isAdding)
                                {
                                    outStr = Convert.ToString(oCompany.GetNewObjectKey());
                                    Doc.GetByKey(Convert.ToInt32(outStr));
                                    int DocNum = Doc.DocNum;
                                    myDocNum = DocNum;
                                }

                                if (isAdding)
                                {

                                    oApplication.SetStatusBarMessage("Order " + outStr + " Added Successfully", SAPbouiCOM.BoMessageTime.bmt_Short, false);
                                }
                                else
                                {
                                    oApplication.SetStatusBarMessage("Order " + myDocNum + " Updated Successfully", SAPbouiCOM.BoMessageTime.bmt_Short, false);

                                }

                            }



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

            return result;

        }

        public static int getCurrentLine(Documents order , string itemCode)
        {
            int result = -1;

            int lineCnt = order.Lines.Count;

            for (int i = 0; i <= order.Lines.Count - 1; i++)
            {
                order.Lines.SetCurrentLine(i);
                if(order.Lines.ItemCode == itemCode)
                {
                    result = i;
                    break;
                }

            }

            return result;

        }

       
        public static int getDOcEntry(string DocNum)
        {
            int result = -1;

            System.Data.DataTable dt = clsSBO.getDataTable("  select DocEntry from ORDR where DocNum = '" + DocNum + "'", "getDocEntry", oCompany);
            if (dt != null && dt.Rows.Count > 0)
            {
                try
                {
                    result = Convert.ToInt32(dt.Rows[0]["DocEntry"]);
                }
                catch { }
            }
            return result;
        }
        private static void addItemsInSO()
        {
          
            SAPbouiCOM.Grid oGrid;
            oGrid = (SAPbouiCOM.Grid)oForm.Items.Item("4").Specific;
            SAPbouiCOM.DataTable dtHistory = oGrid.DataTable;
            oSOForm.Freeze(true);

            try
            {

                foreach (System.Data.DataRow dr in dtActionItems.Rows)
                {
                    if (dr["Action"].ToString() == "Add")
                    {
                        string strItemCode = dr["ItemCode"].ToString();
                        string strPrice = dr["Price"].ToString();
                        string strQty = dr["Quantity"].ToString();
                        string strUoM = dr["UOM"].ToString();
                        oMatrix.AddRow(1);
                        ((SAPbouiCOM.EditText)oMatrix.GetCellSpecific("1", oMatrix.RowCount-1)).Value = strItemCode;
                        ((SAPbouiCOM.EditText)oMatrix.GetCellSpecific("11", oMatrix.RowCount-1)).Value = strQty;
                        try
                        {
                            ((SAPbouiCOM.EditText)oMatrix.GetCellSpecific("1470002145", oMatrix.RowCount-1)).Value = strUoM;

                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            ((SAPbouiCOM.EditText)oMatrix.GetCellSpecific("14", oMatrix.RowCount-1)).Value = strPrice;


                        }
                        catch (Exception)
                        {
                        }
                        dr["Action"] = "Done";
                        break;
                           
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


        private static void flushAllCN()
        {

            SAPbouiCOM.Grid oGrid;
            oGrid = (SAPbouiCOM.Grid)oForm.Items.Item("4").Specific;
            SAPbouiCOM.DataTable dtHistory = oGrid.DataTable;
            oSOForm.Freeze(true);

            try
            {
                using (var progress = SboProgressBar.Create("Please Wait while system prepare document for selected item(s) ", 100, oForm, 400))
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
                            string strPrice = oGrid.DataTable.GetValue("Current Selling Price", dtrown).ToString().Trim();
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




        #endregion

    }
}
