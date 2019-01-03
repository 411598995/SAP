using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace ACHR.Screen
{

    class frm_SS : HRMSBaseForm
    {
        SHDocVw.WebBrowser oWebX;
        SHDocVw.WebBrowser oWebX2;
        string JesonStr = "";

        public bool isForLoading = false;
        SAPbouiCOM.Folder tbORDR1, tbORDR2, tbORDR3, tbORDR4, tbOpr1, tbOpr2, tbOpr3, tbOpr4, tbOCRD1, tbStock1;
        SAPbouiCOM.Matrix mtSOP, mtTOR, mtSTO, mtORI, mtStock, mtRoute, mtSP, mtGroup, mtCust, mtCustRP, mtMSP, mtCRDRP, mtSPO, mtBB, mtAI;
        SAPbouiCOM.DataTable dtRoute, dtSP, dtCG, dtCust, dtGroup, dtHead, dtTSO, dtCustPr, dtMSP, dtSTSO, dtRDR1, dtCRDRP, dtStock, dtTodSO, dtSPO, dtBB, dtAI, dtTSA, dtCard;
        SAPbouiCOM.EditText txGT, txDT, txTT, txTotal, txRem, txCalNote, txNAC;
        SAPbouiCOM.StaticText lbvQtyToProduce, lbvQtyStock, lbvQtyOnOrder, lbvQtyATM;

        SAPbouiCOM.Item dashboard;

        string isCFLCall = "N", lastCFL = "", cflMT = "";
        int cflMTRow = 0;
        string callId = "0";
        System.Data.DataRow drSetting;

        public override void CreateForm(SAPbouiCOM.Application SboApp, string strXml, SAPbobsCOM.Company cmp, string frmId)
        {
            base.CreateForm(SboApp, strXml, cmp, frmId);
            oForm.EnableMenu("1282", false);  // Add New Record
            oForm.EnableMenu("1288", false);  // Next Record
            oForm.EnableMenu("1289", false);  // Pevious Record
            oForm.EnableMenu("1290", false);  // First Record
            oForm.EnableMenu("1291", false);  // Last record 
            oForm.EnableMenu("1281", false);  // Find record 

            InitiallizeForm();
            try
            {

                oForm.Settings.MatrixUID = "mtORI";
                oForm.Settings.Enabled = true;
            }
            catch { }


        }
        public override void etFormAfterLoad(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etFormAfterLoad(ref pVal, ref BubbleEvent);
            //   loadDashBoard();
        }
        public override void etAfterKeyDown(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterKeyDown(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == "txFCard")
            {
                fillCust();
            }
        }

        public override void etAfterLostFocus(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterLostFocus(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == "txCdate")
            {

                string strDate = Convert.ToString(dtHead.GetValue("CDate", 0));
                if (strDate != "")
                {
                    DateTime dateStr = Convert.ToDateTime(strDate);
                    executeScheduler(dateStr);

                    fillTeleSale(dateStr);
                }
            }
        }

        public override void etAfterValidate(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {

            base.etAfterValidate(ref pVal, ref BubbleEvent);
            try
            {

                //if (pVal.ItemUID == "txCdate")
                //{
                //    string strDate = Convert.ToString(dtHead.GetValue("CDate", 0));
                //    DateTime dateStr = Convert.ToDateTime(strDate);
                //    executeScheduler(dateStr);

                //    fillTeleSale(dateStr);
                //}

                if (pVal.ItemUID == mtORI.Item.UniqueID && (pVal.ColUID == "Qty") && pVal.Row > 0)
                {

                    oForm.Freeze(true);
                    try
                    {
                      
                      

                        mtORI.FlushToDataSource();
                        double oldPrice = Convert.ToDouble(dtRDR1.GetValue("Price", pVal.Row - 1));
                        if (oldPrice == 0)
                        {
                            SAPbobsCOM.ItemPriceReturnParams itemPrice = getUnitPriceSys(dtRDR1.GetValue("ItemCode", pVal.Row - 1).ToString(), Convert.ToString(dtHead.GetValue("CardCode", 0)), Convert.ToDouble(dtRDR1.GetValue("Quantity", pVal.Row - 1)), Convert.ToDateTime(dtHead.GetValue("CDate", 0)));
                            dtRDR1.SetValue("Price", pVal.Row - 1, itemPrice.Price);
                            dtRDR1.SetValue("Discount", pVal.Row - 1, itemPrice.Discount);
                            mtORI.LoadFromDataSource();
                        }
                        setTotal(pVal.Row, pVal.ColUID);
                        mtORI.SelectRow(pVal.Row, true, false);
             

                    }
                    catch (Exception ex)
                    {
                        oApplication.SetStatusBarMessage("Error : " + ex.Message);
                    }
                    finally
                    {
                        oForm.Freeze(false);
                    }

                }

                if (pVal.ItemUID == mtORI.Item.UniqueID && (pVal.ColUID == "Price") && pVal.Row > 0)
                {

                    oForm.Freeze(true);
                    try
                    {
                        double oldPrice = Convert.ToDouble(dtRDR1.GetValue("Price", pVal.Row - 1));
                        double newPrice = Convert.ToDouble(((SAPbouiCOM.EditText)mtORI.GetCellSpecific("Price", pVal.Row)).Value);
                        string itemCode = Convert.ToString(((SAPbouiCOM.EditText)mtORI.GetCellSpecific("ItemCode", pVal.Row)).Value);
                        string bpCode = Convert.ToString(dtHead.GetValue("CardCode", 0));

                        string PL = dtHead.GetValue("PLN", 0).ToString();

                        mtORI.FlushToDataSource();


                        setTotal(pVal.Row, pVal.ColUID);

                        if (oldPrice != newPrice)
                        {
                            // oApplication.MessageBox("Price changed");

                            showSaveOptions(itemCode, newPrice, bpCode, PL);

                        }
                        mtORI.SelectRow(pVal.Row, true, false);
             
                    }
                    catch (Exception ex)
                    {
                        oApplication.SetStatusBarMessage("Error : " + ex.Message);
                    }
                    finally
                    {
                        oForm.Freeze(false);
                    }

                }



                if (pVal.ItemUID == "txCalNote")
                {
                    string remarks = dtHead.GetValue("CallNotes", 0).ToString().Replace("'", "''");

                    Hashtable hsp = new Hashtable();
                    hsp.Add("U_Remarks", remarks);
                    hsp.Add("CODE", callId);


                    string strUpdate = "Update [@B1_SCHCALL] set u_remarks='" + remarks + "' where code = " + callId + "";
                    Program.objHrmsUI.ExecQuery(strUpdate, "Updating call remarks");

                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
        }



        private void loadDashBoard()
        {
            //SHDocVw.InternetExplorer oWebX2;
            if (drSetting["U_DBPATH"] == null || drSetting["U_DBPATH"].ToString() == "") return;

            // Create the new activeX control
            SAPbouiCOM.ActiveX AcXTree = (SAPbouiCOM.ActiveX)dashboard.Specific;
            AcXTree.ClassID = "Shell.Explorer.2";
            oWebX2 = (SHDocVw.WebBrowser)AcXTree.Object;
            oWebX2.Navigate2(drSetting["U_DBPATH"].ToString());

            while (oWebX2.Busy)
            {
                System.Windows.Forms.Application.DoEvents();
            }
            // loadKPI(oWebX2);


            string strSql = @"select 
                    sum(  case when (CANCELED='N' and   t0.DocDueDate  = CONVERT(date, getdate())) then 1 else 0 end)  as ThisWeek   ,
                    sum(  case when (CANCELED='N' and   t0.DocDueDate  = DATEADD(WW,-1, CONVERT(date, getdate()))) then 1 else 0 end) as previousWeek   
                    from   ORDR t0 where t0.docduedate >= DATEADD(WW,-2,GETDATE())


                    union all
                    select 
                                        sum(  case when (CANCELED='N' and   t0.DocDate  = CONVERT(date, getdate())) then 1 else 0 end)  as ThisWeek   ,
                                        sum(  case when (CANCELED='N' and   t0.DocDate  = DATEADD(WW,-1, CONVERT(date, getdate()))) then 1 else 0 end) as previousWeek   
                                        from   ORDR t0 where t0.DocDate >= DATEADD(WW,-2,GETDATE())

                    union all
                    select 
                                        sum(  case when (CANCELED='N' and  t0.DocDate between  dateadd(WW,datediff(ww,0,getdate()),0) and convert(date,getdate() )) then 1 else 0 end)  as ThisWeek   
                                      , sum(  case when (CANCELED='N' and  t0.DocDate between dateadd(WW,datediff(ww,0, DATEADD(WW,-1, getdate())),0) and DATEADD(WW,-1, convert(date,getdate() ))) then 1 else 0 end)  as PrevWeek   
                   
					                    from   ORDR t0 where t0.DocDate >= DATEADD(WW,-2,GETDATE())

                    union all	
                    select 
                                        sum(  case when (CANCELED='N' and t0.DocStatus = 'O' and    t0.DocDueDate  = CONVERT(date, getdate())) then 1 else 0 end)  as ThisWeek
                                      , 0 as PrevWeek   
                   
					                    from   ORDR t0 where t0.DocDate >= DATEADD(WW,-2,GETDATE())
                    union all
                    select 
                                        sum(  case when (CANCELED='N' and   t0.DocDate  = CONVERT(date, getdate())) then t0.DocTotal else 0 end)  as ThisWeek   ,
                                        sum(  case when (CANCELED='N' and   t0.DocDate  = DATEADD(WW,-1, CONVERT(date, getdate()))) then t0.DocTotal else 0 end) as previousWeek   
                                        from   ORDR t0 where t0.DocDate >= DATEADD(WW,-2,GETDATE())";





            JesonStr = " [";
            System.Data.DataTable TSSchedule = Program.objHrmsUI.getDataTable(strSql, "Filling AI");
            int j = 0;
            foreach (System.Data.DataRow dr in TSSchedule.Rows)
            {
                double Val1 = Convert.ToDouble(dr[0]);
                double Val2 = Convert.ToDouble(dr[1]);
                string diff = (Val1 > Val2) ? "+" + (Val1 - Val2).ToString() : (Val1 - Val2).ToString();
                string dir = Val1 < Val2 ? "down" : "up";
                if (Val1 == Val2) dir = "uk";
                switch (j)
                {

                    case 0:

                        JesonStr += " { Nums: '5,6,7,2,0,4,2,4,8,2,3,3,2', KPIVal: '" + Val1 + "', Change: '" + diff + "', KPITitle: 'Order Due', Color: 'red', direction: '" + dir + "' },";

                        break;
                    case 1:
                        JesonStr += " { Nums: '5,6,7,2,6,4,2,4,8,2,3,3,12', KPIVal: '" + Val1 + "', Change: '" + diff + "', KPITitle: 'Order Placed', Color: 'yellow', direction: '" + dir + "' }, ";

                        break;
                    case 2:
                        JesonStr += " { Nums: '5,6,7,2,6,4,2,4,8,2,3,3,21', KPIVal: '" + Val1 + "', Change: '" + diff + "', KPITitle: 'Weekly Trend', Color: 'blue', direction: '" + dir + "' }, ";

                        break;
                    case 3:
                        JesonStr += " { Nums: '5,6,7,2,6,4,2,4,8,1,3,3,21', KPIVal: '" + Val1 + "', Change: '" + diff + "', KPITitle: 'Outstanding Orders', Color: 'green', direction: '" + dir + "' }, ";

                        break;
                    case 4:
                        JesonStr += " { Nums: '5,6,7,2,6,4,2,4,8,1,3,3,21', KPIVal: '" + Val1 + "', Change: '', KPITitle: 'Value Day', Color: 'light blue', direction: 'uk' }, ";

                        break;
                }

                j++;



            }



            JesonStr += " ]";

            ExecuteJavaScript();



        }

        private void ExecuteJavaScript()
        {
            System.Threading.Thread aThread = null;
            aThread = new Thread(ExecuteJavaScriptWorker);
            aThread.SetApartmentState(ApartmentState.STA);


            // Thread.Sleep(5000);


            aThread.Start();


        }

        private void ExecuteJavaScriptWorker()
        {
            try
            {
                oWebX2.Document.parentWindow.execScript("sboTest(" + JesonStr + ")", "javascript");

            }
            catch (Exception ex)
            {
                oApplication.SetStatusBarMessage(ex.Message);
            }
            finally
            {
                GC.Collect();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oWebX2);
                System.Windows.Forms.Application.ExitThread();
            }


        }

        public override void etAfterClick(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);

            if (pVal.ColUID != null)
            {
                //try
                //{
                //    oForm.Settings.MatrixUID = pVal.ItemUID;
                //}
                //catch { }
            }

            if (pVal.ItemUID == mtMSP.Item.UniqueID && pVal.Row > 0 && pVal.Row <= mtMSP.RowCount)
            {

                string itemcode = Convert.ToString(dtMSP.GetValue("ItemCode", pVal.Row - 1));
                string itemName = Convert.ToString(dtMSP.GetValue("ItemName", pVal.Row - 1));
                fillAI(itemcode);
            }




            if (pVal.ItemUID == mtCust.Item.UniqueID)
            {


                if (pVal.ColUID == "sel" && pVal.Row == 0)
                {
                    selectAllCust();
                }
            }

            if (pVal.ItemUID == "btNoSale")
            {
                noSale(callId);
                callId = "0";
            }

            if (pVal.ItemUID == mtGroup.Item.UniqueID)
            {


                if (pVal.ColUID == "sel")
                {
                    if (pVal.Row == 0)
                    {
                        selectAllGroup();
                    }

                    fillCust();

                }
            }

            if (pVal.ItemUID == "mtSP")
            {


                if (pVal.ColUID == "sel")
                {
                    if (pVal.Row == 0)
                    {
                        selectAllSlp();
                    }
                    fillCust();
                }
            }
            if (pVal.ItemUID == "mtRoute")
            {


                if (pVal.ColUID == "sel")
                {
                    if (pVal.Row == 0)
                    {
                        selectAllRoute();
                    }
                    fillCust();
                }
            }
            if (pVal.ItemUID.Contains("tbOpr"))
            {
                ShowhideOpr(pVal.ItemUID);
            }

            if (pVal.ItemUID.Contains("tbSTOCK"))
            {
                showHideStock(pVal.ItemUID);
            }

            if (pVal.ItemUID.Contains("tbORDR"))
            {
                ShowhideOR(pVal.ItemUID);
            }

            if (pVal.ItemUID.Contains("tbOCRD"))
            {
                ShowhideCRD(pVal.ItemUID);
            }


            if (pVal.ItemUID == "btExecSch")
            {
                string strDate = Convert.ToString(dtHead.GetValue("CDate", 0));

                DateTime dateStr = Convert.ToDateTime(strDate);

                executeScheduler(dateStr);
            }
            if (pVal.ItemUID == "btLoad")
            {
                string strDate = Convert.ToString(dtHead.GetValue("CDate", 0));
                fillTeleSale(Convert.ToDateTime(strDate));
            }
            if (pVal.ItemUID == "mtSOP" && pVal.Row <= mtSOP.RowCount && pVal.ColUID == "Id")
            {
                callId = Convert.ToString(dtTSO.GetValue("CallId", pVal.Row - 1));
                fillCustomerData(pVal.Row, "TS");
            }

            if (pVal.ItemUID == "mtSTO" && pVal.Row <= mtSTO.RowCount && pVal.ColUID == "Id")
            {
                callId = Convert.ToString(dtSTSO.GetValue("CallId", pVal.Row - 1));
                fillCustomerData(pVal.Row, "ST");
            }

            if (pVal.ItemUID == "mtCust" && pVal.Row <= mtCust.RowCount && pVal.ColUID == "Id")
            {
                //callId = Convert.ToString(dtSTSO.GetValue("CallId", pVal.Row - 1));
                callId = "0";
                fillCustomerData(pVal.Row, "CST");
            }

            if (pVal.ItemUID == mtORI.Item.UniqueID && pVal.Row <= mtORI.RowCount && pVal.ColUID == "Id" && pVal.Row > 0)
            {
                string itemid = Convert.ToString(dtRDR1.GetValue("ItemCode", pVal.Row - 1));
                fillStockVal(itemid);
                fillAI(itemid);
            }


            if (pVal.ItemUID == mtStock.Item.UniqueID && pVal.Row <= mtStock.RowCount && pVal.ColUID == "V_-1" && pVal.Row > 0)
            {
               
                string Batch = Convert.ToString(dtStock.GetValue("BN", pVal.Row - 1));
                string BB = Convert.ToString(dtStock.GetValue("BB", pVal.Row - 1));
                string Location = Convert.ToString(dtStock.GetValue("UOM", pVal.Row - 1));
                string freeText = Location + "-" + Batch + "-" + BB;
                int selInd = mtSelRow(mtORI);
                mtORI.SelectRow(selInd, true, false);
             
                dtRDR1.SetValue("Freetxt", selInd - 1, freeText);
                mtORI.LoadFromDataSource();
                mtORI.SelectRow(selInd, true, false);
               // mtORI.SetLineData(selInd);
                // oApplication.MessageBox(freeText);
            }



            if (pVal.ItemUID == "btAdd")
            {
                doPost();
            }
            if (pVal.ItemUID == "btClear")
            {
                clearItems();
                addEmptyRow(mtORI, dtRDR1, "ItemCode");
            }

        }

        private void fillStdOrders(string cardCode)
        {

            string strQuery = @"select t0.U_ItemCode as ItemCode , t1.ItemName , t0.U_Qty as Quantity
                                from [@B1_SO] t0 inner join oitm t1 on t0.U_ItemCode = t1.ItemCode";
            strQuery += " where U_SCCode = 'SO_" + cardCode + "'";
            System.Data.DataTable dtSO = Program.objHrmsUI.getDataTable(strQuery, "Getting Order");
            foreach (System.Data.DataRow dr in dtSO.Rows)
            {
                addItemInRDR(dr["ItemCode"].ToString(), dr["ItemName"].ToString(), Convert.ToDouble(dr["Quantity"]), 0.00);
            }
        }
        public override void etBeforeCfl(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etBeforeCfl(ref pVal, ref BubbleEvent);
            string cardCode = Convert.ToString(dtHead.GetValue("CardCode", 0));
            if (cardCode == "")
            {
                oApplication.MessageBox("Select a customer/Call/Standing Order to add an order for");
                BubbleEvent = false;
            }
        }
        public override void etAfterCfl(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCfl(ref pVal, ref BubbleEvent);

            int rowind = pVal.Row;
            mtORI.FlushToDataSource();
            SAPbouiCOM.IChooseFromListEvent oCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pVal;
            SAPbouiCOM.DataTable dtSel = oCFLEvento.SelectedObjects;
            if (dtSel != null)
            {
                if (pVal.ItemUID == "mtORI")
                {
                    string ItemCode = dtSel.GetValue("ItemCode", 0).ToString();
                    string ItemName = dtSel.GetValue("ItemName", 0).ToString();
                    string priceList = dtHead.GetValue("PLN", 0).ToString();


                    dtRDR1.SetValue("ItemCode", rowind - 1, ItemCode);
                    dtRDR1.SetValue("ItemName", rowind - 1, ItemName);
                    SAPbobsCOM.ItemPriceReturnParams itemPrice = getUnitPriceSys(ItemCode, Convert.ToString(dtHead.GetValue("CardCode", 0)), 1, Convert.ToDateTime(dtHead.GetValue("CDate", 0)));
                    string freeText = getCustSpecs(ItemCode, Convert.ToString(dtHead.GetValue("CardCode", 0)));
                    dtRDR1.SetValue("Freetxt", rowind - 1, freeText);

                    //  dtRDR1.SetValue("Price", rowind - 1, getUnitPrice(ItemCode,priceList));
                    dtRDR1.SetValue("Price", rowind - 1, itemPrice.Price);
                    dtRDR1.SetValue("Discount", rowind - 1, itemPrice.Discount);

                    dtRDR1.SetValue("ItemName", rowind - 1, ItemName);
                    dtRDR1.SetValue("Quantity", rowind - 1, "1");

                    mtORI.LoadFromDataSource();
                    addEmptyRow(mtORI, dtRDR1, "ItemCode");
                    isCFLCall = "Y";
                    cflId = oCFLEvento.ChooseFromListUID;
                    cflMT = "mtORI";
                    cflMTRow = rowind;
                    setTotal(rowind, pVal.ColUID);
                    // mtORI.SetCellFocus(rowind, 3);
                    // mtORI.SelectRow(rowind,false,false);

                }
            }
        }

        public override void etAfterDoubleClick(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterDoubleClick(ref pVal, ref BubbleEvent);

            if (pVal.ItemUID == mtMSP.Item.UniqueID && pVal.Row > 0 && pVal.Row <= mtMSP.RowCount)
            {
                string itemcode = Convert.ToString(dtMSP.GetValue("ItemCode", pVal.Row - 1));
                string itemName = Convert.ToString(dtMSP.GetValue("ItemName", pVal.Row - 1));

                addItemInRDR(itemcode, itemName, 1, 0.00);
            }

            if (pVal.ItemUID == mtBB.Item.UniqueID && pVal.Row > 0 && pVal.Row <= mtBB.RowCount)
            {
                if (dtHead.GetValue("CardCode", 0).ToString() != "")
                {
                    string itemcode = Convert.ToString(dtBB.GetValue("ItemCode", pVal.Row - 1));
                    string itemName = Convert.ToString(dtBB.GetValue("ItemName", pVal.Row - 1));

                    addItemInRDR(itemcode, itemName, 1, 0.00);
                }
                else
                {
                    oApplication.MessageBox("Please select a customer first");
                }
            }

            if (pVal.ItemUID == mtCustRP.Item.UniqueID && pVal.Row > 0 && pVal.Row <= mtCustRP.RowCount)
            {
                string itemcode = Convert.ToString(dtCustPr.GetValue("ItemCode", pVal.Row - 1));
                string itemName = Convert.ToString(dtCustPr.GetValue("Product", pVal.Row - 1));

                addItemInRDR(itemcode, itemName, 1, 0.00);
            }

            if (pVal.ItemUID == mtCRDRP.Item.UniqueID && pVal.Row > 0 && pVal.Row <= mtCRDRP.RowCount)
            {
                string itemcode = Convert.ToString(dtCRDRP.GetValue("ItemCode", pVal.Row - 1));
                string itemName = Convert.ToString(dtCRDRP.GetValue("ItemName", pVal.Row - 1));

                addItemInRDR(itemcode, itemName, 1, 0.00);
            }

            if (pVal.ItemUID == mtSPO.Item.UniqueID && pVal.Row > 0 && pVal.Row <= mtSPO.RowCount)
            {
                string itemcode = Convert.ToString(dtSPO.GetValue("ItemCode", pVal.Row - 1));
                string itemName = Convert.ToString(dtSPO.GetValue("ItemName", pVal.Row - 1));
                double quantity = Convert.ToDouble(dtSPO.GetValue("Quantity", pVal.Row - 1));
                double discount = Convert.ToDouble(dtSPO.GetValue("DiscPer", pVal.Row - 1));

                addItemInRDR(itemcode, itemName, quantity, discount);
            }



            if (pVal.ItemUID == mtSTO.Item.UniqueID && pVal.Row > 0 && pVal.Row <= mtSTO.RowCount)
            {
                clearItems();
                fillStdOrders(Convert.ToString(dtHead.GetValue("CardCode", 0)));
            }

            //if (pVal.ItemUID == mt.Item.UniqueID && pVal.Row <= mtMSP.RowCount)
            //{
            //    string itemcode = Convert.ToString(dtMSP.GetValue("ItemCode", pVal.Row - 1));
            //    string itemName = Convert.ToString(dtMSP.GetValue("ItemName", pVal.Row - 1));

            //    addItemInRDR(itemcode, itemName, 1);
            //}

        }

        public override void etFormAfterActivate(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etFormAfterActivate(ref pVal, ref BubbleEvent);
            if (isCFLCall == "Y")
            {
                isCFLCall = "N";
                if (cflId == "cflItem")
                {
                    mtORI.SetCellFocus(cflMTRow, 4);
                }


            }
        }

        private void InitiallizeForm()
        {


            string strExisting = @"SELECT         *
                                        FROM            [@B1_CONFIG] ";
            strExisting += "Where Code='0001'";

            System.Data.DataTable dtSetting = Program.objHrmsUI.getDataTable(strExisting, "Getting Setting");
            if (dtSetting.Rows.Count > 0)
            {
                drSetting = dtSetting.Rows[0];
            }
            else
            {
                string strInsert = " insert into  [@B1_CONFIG]  (Code, Name, U_SchDays, U_NLastOrdr, U_NMSI, U_NDTH,U_NRP) ";
                strInsert += " Values ('0001','0001','30','10','10','7','10')";
                Program.objHrmsUI.ExecQuery(strInsert, "Adding Setting");

                dtSetting = Program.objHrmsUI.getDataTable(strExisting, "Getting Setting");
                if (dtSetting.Rows.Count > 0)
                {
                    drSetting = dtSetting.Rows[0];
                }

            }

            //SAPbouiCOM.Item lbl = oForm.Items.Add("lblOTN", SAPbouiCOM.BoFormItemTypes.it_STATIC);
            //  int greenColor = System.Drawing.Color.Green.R | (System.Drawing.Color.Green.G << 8) | (System.Drawing.Color.Green.B << 16);

            //lbl.ForeColor =greenColor;
            //lbl.Top = 20;

            dtRoute = oForm.DataSources.DataTables.Item("dtRoute");
            dtSP = oForm.DataSources.DataTables.Item("dtSP");
            dtGroup = oForm.DataSources.DataTables.Item("dtGroup");
            dtHead = oForm.DataSources.DataTables.Item("dtHead");
            dtCust = oForm.DataSources.DataTables.Item("dtCust");
            dtTSO = oForm.DataSources.DataTables.Item("dtTSO");
            dtCustPr = oForm.DataSources.DataTables.Item("dtCustPr");
            dtMSP = oForm.DataSources.DataTables.Item("dtMSP");
            dtSTSO = oForm.DataSources.DataTables.Item("dtSTSO");
            dtRDR1 = oForm.DataSources.DataTables.Item("dtRDR1");
            dtCRDRP = oForm.DataSources.DataTables.Item("dtCRDRP");
            dtStock = oForm.DataSources.DataTables.Item("dtStock");
            dtSP = oForm.DataSources.DataTables.Item("dtSP");
            dtSPO = oForm.DataSources.DataTables.Item("dtSPO");
            dtTodSO = oForm.DataSources.DataTables.Item("dtTodSO");
            dtBB = oForm.DataSources.DataTables.Item("dtBB");
            dtAI = oForm.DataSources.DataTables.Item("dtAI");
            dtTSA = oForm.DataSources.DataTables.Item("dtTSA");
            dtCard = oForm.DataSources.DataTables.Item("dtCard");
            dtCard.Rows.Add(1);



            dashboard = oForm.Items.Add("browser", SAPbouiCOM.BoFormItemTypes.it_ACTIVE_X);
            dashboard.Left = 5;
            dashboard.Top = 55;
            dashboard.Width = 1024;
            dashboard.Height = 495;
            dashboard.Visible = false;



            mtSOP = (SAPbouiCOM.Matrix)oForm.Items.Item("mtSOP").Specific;
            mtTOR = (SAPbouiCOM.Matrix)oForm.Items.Item("mtTOR").Specific;
            mtSTO = (SAPbouiCOM.Matrix)oForm.Items.Item("mtSTO").Specific;

            mtORI = (SAPbouiCOM.Matrix)oForm.Items.Item("mtORI").Specific;
            mtORI.Columns.Item("Qty").Editable = true;
            mtRoute = (SAPbouiCOM.Matrix)oForm.Items.Item("mtRoute").Specific;
            mtSP = (SAPbouiCOM.Matrix)oForm.Items.Item("mtSP").Specific;
            mtGroup = (SAPbouiCOM.Matrix)oForm.Items.Item("mtGroup").Specific;
            mtCust = (SAPbouiCOM.Matrix)oForm.Items.Item("mtCust").Specific;
            mtCustRP = (SAPbouiCOM.Matrix)oForm.Items.Item("mtCustRP").Specific;
            mtMSP = (SAPbouiCOM.Matrix)oForm.Items.Item("mtMSP").Specific;
            mtCRDRP = (SAPbouiCOM.Matrix)oForm.Items.Item("mtCRDRP").Specific;
            mtSPO = (SAPbouiCOM.Matrix)oForm.Items.Item("mtSPO").Specific;
            mtBB = (SAPbouiCOM.Matrix)oForm.Items.Item("mtBB").Specific;
            mtAI = (SAPbouiCOM.Matrix)oForm.Items.Item("mtAI").Specific;


            txRem = (SAPbouiCOM.EditText)oForm.Items.Item("txRem").Specific;
            txNAC = (SAPbouiCOM.EditText)oForm.Items.Item("txNAC").Specific;
            txCalNote = (SAPbouiCOM.EditText)oForm.Items.Item("txCalNote").Specific;

            lbvQtyToProduce = (SAPbouiCOM.StaticText)oForm.Items.Item("lbvToProd").Specific;
            lbvQtyStock = (SAPbouiCOM.StaticText)oForm.Items.Item("lbvOnStock").Specific;
            lbvQtyOnOrder = (SAPbouiCOM.StaticText)oForm.Items.Item("lbvOnOrder").Specific;
            lbvQtyATM = (SAPbouiCOM.StaticText)oForm.Items.Item("lbvATP").Specific;

            dtHead.Rows.Add(1);
            dtTSA.Rows.Add(1);


            mtStock = (SAPbouiCOM.Matrix)oForm.Items.Item("mtStock").Specific;
            oForm.Freeze(true);
            createTabs();
            setCtrlPositions();
            fillRoute();
            fillSP();
            fillCust();

            oForm.Freeze(false);

            dtHead.SetValue("cDate", 0, DateTime.Now.Date);
            dtHead.SetValue("dDate", 0, DateTime.Now.Date);

            tbORDR1.Item.Click();
            tbOpr1.Item.Click();
            tbOCRD1.Item.Click();
            tbStock1.Item.Click();
            addEmptyRow(mtORI, dtRDR1, "ItemCode");



            fillTOR();

            oForm.Freeze(false);
            string strDate = Convert.ToString(dtHead.GetValue("CDate", 0));
            executeScheduler(Convert.ToDateTime(strDate));
            fillTeleSale(Convert.ToDateTime(strDate));
            fillNearExpiry();
            fillTelesaleAnalysis();
            try
            {
                loadDashBoard();
            }
            catch { }



            oForm.Freeze(false);
            oForm.Height = 830;
        }

        private void createTabs()
        {

            SAPbouiCOM.Item newTabItem;
            SAPbouiCOM.Folder folder;

            for (int i = 0; i < 5; i++)
            {
                newTabItem = oForm.Items.Add("tbOpr" + i.ToString(), SAPbouiCOM.BoFormItemTypes.it_FOLDER);
                newTabItem.Left = 100 * i;
                newTabItem.Top = 185;
                newTabItem.Width = 130;
                newTabItem.Height = 19;
                newTabItem.FromPane = 0;
                newTabItem.ToPane = 0;
                folder = (SAPbouiCOM.Folder)newTabItem.Specific;
                folder.AutoPaneSelection = true;
                folder.DataBind.SetBound(true, "", "FOlderDS");
                if (i == 0) folder.Select();
                else
                    folder.GroupWith("tbOpr" + (i - 1).ToString());


                switch (i)
                {
                    case 0:
                        folder.Caption = "Sales Opportunity";
                        tbOpr1 = folder;
                        break;
                    case 1:
                        folder.Caption = "Standing Orders";
                        tbOpr2 = folder;
                        break;
                    case 2:
                        folder.Caption = "Todays Orders";
                        tbOpr3 = folder;
                        break;
                    case 3:
                        folder.Caption = "Tele Sales Analysis";
                        tbOpr4 = folder;
                        break;
                    case 4:
                        folder.Caption = "Sales Analysis ";
                        break;
                }
            }



            for (int i = 0; i < 2; i++)
            {
                newTabItem = oForm.Items.Add("tbORDR" + i.ToString(), SAPbouiCOM.BoFormItemTypes.it_FOLDER);
                newTabItem.Left = (100 * i);
                newTabItem.Top = 420;
                newTabItem.Width = 100;
                newTabItem.Height = 19;
                newTabItem.FromPane = 0;
                newTabItem.ToPane = 0;
                folder = (SAPbouiCOM.Folder)newTabItem.Specific;
                folder.AutoPaneSelection = true;

                folder.DataBind.SetBound(true, "", "FOlderDS2");
                if (i == 0) folder.Select();
                else
                    folder.GroupWith("tbORDR" + (i - 1).ToString());


                switch (i)
                {
                    case 0:
                        folder.Caption = "Ordered";
                        tbORDR1 = folder;
                        break;
                    case 1:
                        folder.Caption = "Logistic / Accounting";
                        tbORDR2 = folder;
                        break;

                }
            }


            for (int i = 0; i < 4; i++)
            {
                newTabItem = oForm.Items.Add("tbOCRD" + i.ToString(), SAPbouiCOM.BoFormItemTypes.it_FOLDER);
                newTabItem.Left = 855 + (100 * i);
                newTabItem.Top = 18;
                newTabItem.Width = 100;
                newTabItem.Height = 19;
                newTabItem.FromPane = 0;
                newTabItem.ToPane = 0;
                folder = (SAPbouiCOM.Folder)newTabItem.Specific;
                folder.AutoPaneSelection = true;
                folder.DataBind.SetBound(true, "", "FOlderDS3");
                if (i == 0) folder.Select();
                else
                    folder.GroupWith("tbOCRD" + (i - 1).ToString());


                switch (i)
                {
                    case 0:
                        folder.Caption = "Recent Product";
                        tbOCRD1 = folder;
                        break;
                    case 1:
                        folder.Caption = "Most Sold Products";

                        break;

                    case 2:
                        folder.Caption = "Last Orders";

                        break;
                    case 3:
                        folder.Caption = "Special Offers";

                        break;

                }
            }

            for (int i = 0; i < 3; i++)
            {
                newTabItem = oForm.Items.Add("tbSTOCK" + i.ToString(), SAPbouiCOM.BoFormItemTypes.it_FOLDER);
                newTabItem.Left = 855 + (100 * i);
                newTabItem.Top = 570;
                newTabItem.Width = 100;
                newTabItem.Height = 19;
                newTabItem.FromPane = 0;
                newTabItem.ToPane = 0;
                folder = (SAPbouiCOM.Folder)newTabItem.Specific;

                folder.DataBind.SetBound(true, "", "FOlderDS4");
                if (i == 0) folder.Select();
                else
                    folder.GroupWith("tbSTOCK" + (i - 1).ToString());

                switch (i)
                {
                    case 0:
                        folder.Caption = "Stock List";
                        tbStock1 = folder;

                        break;
                    case 1:
                        folder.Caption = "Complimentary Products";

                        break;


                    case 2:
                        folder.Caption = "Short Date Stock";

                        break;

                }
            }


        }


        private void setCtrlPositions()
        {
            // tab Operation
            mtSOP.Item.Top = 212;
            mtSOP.Item.Left = 22;

            mtTOR.Item.Top = 212;
            mtTOR.Item.Left = 22;

            mtSTO.Item.Top = 212;
            mtSTO.Item.Left = 22;

            dashboard.Top = 212;
            dashboard.Left = 50;
            dashboard.Width = oForm.Width - 100;  // mtSTO.Item.Width+200;
            dashboard.Height = mtSTO.Item.Height;


            // tab Order
            mtORI.Item.Top = 448;
            mtORI.Item.Left = 22;

            mtCustRP.Item.Top = 70;
            mtCustRP.Item.Left = 860;

            mtMSP.Item.Top = 70;
            mtMSP.Item.Left = 860;

            mtCRDRP.Item.Top = 70;
            mtCRDRP.Item.Left = 860;

            mtSPO.Item.Top = 70;
            mtSPO.Item.Left = 860;


            mtBB.Item.Top = mtStock.Item.Top;
            mtBB.Item.Left = mtStock.Item.Left;
            mtBB.Item.Width = mtStock.Item.Width;
            mtBB.Item.Height = mtStock.Item.Height;

            mtAI.Item.Top = mtStock.Item.Top;
            mtAI.Item.Left = mtStock.Item.Left;
            mtAI.Item.Width = mtStock.Item.Width;
            mtAI.Item.Height = mtStock.Item.Height;



        }
        private void ShowhideOpr(string tabId)
        {
            //hide all
            mtSOP.Item.Visible = false;
            mtTOR.Item.Visible = false;
            mtSTO.Item.Visible = false;
            dashboard.Visible = false;
            for (int i = 0; i < 13; i++)
            {
                oForm.Items.Item("lbTS" + i.ToString()).Visible = false;
            }

            for (int i = 0; i < 10; i++)
            {
                oForm.Items.Item("txTS" + i.ToString()).Visible = false;
            }
            switch (tabId)
            {
                case "tbOpr0":
                    mtSOP.Item.Visible = true;
                    break;
                case "tbOpr1":
                    mtSTO.Item.Visible = true;
                    //  oApplication.ActivateMenuItem("39699");
                    break;
                case "tbOpr2":
                    mtTOR.Item.Visible = true;
                    break;
                case "tbOpr3":
                    for (int i = 0; i < 13; i++)
                    {
                        oForm.Items.Item("lbTS" + i.ToString()).Visible = true;
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        oForm.Items.Item("txTS" + i.ToString()).Visible = true;
                    }

                    break;
                case "tbOpr4":
                    dashboard.Visible = true;
                    break;
            }



        }

        private void ShowhideOR(string tabId)
        {
            //hide all
            mtORI.Item.Visible = false;

            for (int i = 0; i < 9; i++)
            {
                oForm.Items.Item("lbOR" + i.ToString()).Visible = false;
            }
            for (int i = 0; i < 9; i++)
            {
                oForm.Items.Item("ctOR" + i.ToString()).Visible = false;
            }



            switch (tabId)
            {
                case "tbORDR0":
                    mtORI.Item.Visible = true;
                    break;
                case "tbORDR1":
                    for (int i = 0; i < 9; i++)
                    {
                        oForm.Items.Item("lbOR" + i.ToString()).Visible = true;
                    }
                    for (int i = 0; i < 9; i++)
                    {
                        oForm.Items.Item("ctOR" + i.ToString()).Visible = true;
                    }
                    break;


            }



        }


        private void ShowhideCRD(string tabId)
        {
            //hide all
            mtMSP.Item.Visible = false;
            mtCustRP.Item.Visible = false;
            mtCRDRP.Item.Visible = false;
            mtSPO.Item.Visible = false;

            try
            {
                switch (tabId)
                {
                    case "tbOCRD0":
                        mtCRDRP.Item.Visible = true;
                        break;
                    case "tbOCRD1":
                        mtMSP.Item.Visible = true;
                        break;
                    case "tbOCRD2":
                        mtCustRP.Item.Visible = true;
                        break;
                    case "tbOCRD3":
                        mtSPO.Item.Visible = true;
                        break;


                }
            }
            catch { }



        }


        private void showHideStock(string tabId)
        {
            mtStock.Item.Visible = false;
            mtBB.Item.Visible = false;
            mtAI.Item.Visible = false;

            try
            {
                switch (tabId)
                {
                    case "tbSTOCK0":
                        mtStock.Item.Visible = true;
                        break;
                    case "tbSTOCK1":
                        mtAI.Item.Visible = true;
                        break;
                    case "tbSTOCK2":
                        mtBB.Item.Visible = true;
                        break;


                }
            }
            catch { }


        }
        private void fillCustomers()
        {

        }


        private void fillRoute()
        {
            dtRoute.Rows.Clear();
            string strProp = "SELECT        TOP (200) Code, Name FROM            [@B1_CUSTROUTE]";
            System.Data.DataTable Prop = Program.objHrmsUI.getDataTable(strProp, "Filling Route");
            int j = 0;
            foreach (System.Data.DataRow dr in Prop.Rows)
            {
                dtRoute.Rows.Add(1);
                dtRoute.SetValue("Id", j, dr["Code"].ToString());
                dtRoute.SetValue("sel", j, "N");
                dtRoute.SetValue("Route", j, dr["Name"].ToString());




                j++;
            }
            mtRoute.LoadFromDataSource();

        }
        private void fillSP()
        {
            dtSP.Rows.Clear();
            string strProp = "SELECT        slpCode,slpName FROM            OSLP";
            System.Data.DataTable Prop = Program.objHrmsUI.getDataTable(strProp, "Filling SP");
            int j = 0;
            foreach (System.Data.DataRow dr in Prop.Rows)
            {
                dtSP.Rows.Add(1);
                dtSP.SetValue("Id", j, dr["slpCode"].ToString());
                dtSP.SetValue("sel", j, "N");
                dtSP.SetValue("SP", j, dr["slpName"].ToString());




                j++;
            }
            mtSP.LoadFromDataSource();


            string strGroup = " select GroupCode,GroupName from ocrg where grouptype='C' ";
            System.Data.DataTable grp = Program.objHrmsUI.getDataTable(strGroup, "Filling Group");
            j = 0;
            foreach (System.Data.DataRow dr in grp.Rows)
            {
                dtGroup.Rows.Add(1);
                dtGroup.SetValue("Id", j, dr["GroupCode"].ToString());
                dtGroup.SetValue("sel", j, "N");
                dtGroup.SetValue("Group", j, dr["GroupName"].ToString());




                j++;
            }
            mtGroup.LoadFromDataSource();

        }

        private void fillCust()
        {

            dtCust.Rows.Clear();
            string todayDay = DateTime.Now.DayOfWeek.ToString().Substring(0, 3);

            string strGroupCri = getGroupCri();
            string strRouteCri = getRouteCri();
            string strSPCri = getSPCri();
            //  string fCard = dtHead.GetValue("FCard", 0).ToString();

            string fCard = ((SAPbouiCOM.EditText)oForm.Items.Item("txFCard").Specific).Value.ToString();


            string strSelect = "Select CardCode,CardName from ocrd where CardType = 'C' and (CardCode like '" + fCard + "%' or CardName Like '" + fCard + "%' ) ";

            if (strGroupCri != "") strSelect += " And ocrd.groupCode in (" + strGroupCri + ")";
            if (strSPCri != "") strSelect += " And ocrd.slpcode in (" + strSPCri + ")";

            if (strRouteCri != "") strSelect += " And isnull(ocrd.U_Route" + todayDay + ",'')  in (" + strRouteCri + ")";

            System.Data.DataTable grp = Program.objHrmsUI.getDataTable(strSelect, "Filling Customer");

            //  oForm.Freeze(true);
            int rowCnt = grp.Rows.Count;
            dtCust.Rows.Add(rowCnt);
            try
            {
                int j = 0;
                foreach (System.Data.DataRow dr in grp.Rows)
                {
                    //  dtCust.Rows.Add(1);
                    dtCust.SetValue("Id", j, (j + 1).ToString());
                    //    dtCust.SetValue("sel", j, "N");
                    dtCust.SetValue("CardCode", j, dr["CardCode"].ToString());
                    dtCust.SetValue("CardName", j, dr["CardName"].ToString());


                    j++;
                }
                mtCust.LoadFromDataSource();

                string strDate = Convert.ToString(dtHead.GetValue("CDate", 0));
                if (strDate != "")
                {
                    DateTime dateStr = Convert.ToDateTime(strDate);
                    fillTeleSale(dateStr);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                //  oForm.(false);

            }

        }

        private string getGroupCri()
        {
            string cri = "";
            int selCnt = 0;
            mtGroup.FlushToDataSource();
            for (int i = 0; i < dtGroup.Rows.Count; i++)
            {


                if (Convert.ToString(dtGroup.GetValue("sel", i)) == "Y")
                {
                    if (selCnt == 0)
                    {
                        cri = "'" + Convert.ToString(dtGroup.GetValue("Id", i)) + "'";
                    }
                    else
                    {
                        cri += ",'" + Convert.ToString(dtGroup.GetValue("Id", i)) + "'";
                    }

                    selCnt++;
                }
            }

            return cri;
        }

        private string getRouteCri()
        {
            string cri = "";
            int selCnt = 0;
            mtRoute.FlushToDataSource();
            for (int i = 0; i < dtRoute.Rows.Count; i++)
            {


                if (Convert.ToString(dtRoute.GetValue("sel", i)) == "Y")
                {
                    if (selCnt == 0)
                    {
                        cri = "'" + Convert.ToString(dtRoute.GetValue("Id", i)) + "'";
                    }
                    else
                    {
                        cri += ",'" + Convert.ToString(dtRoute.GetValue("Id", i)) + "'";
                    }

                    selCnt++;
                }
            }

            return cri;
        }

        private string getSPCri()
        {
            string cri = "";
            int selCnt = 0;
            mtSP.FlushToDataSource();
            for (int i = 0; i < dtSP.Rows.Count; i++)
            {


                if (Convert.ToString(dtSP.GetValue("sel", i)) == "Y")
                {
                    if (selCnt == 0)
                    {
                        cri = "'" + Convert.ToString(dtSP.GetValue("Id", i)) + "'";
                    }
                    else
                    {
                        cri += ",'" + Convert.ToString(dtSP.GetValue("Id", i)) + "'";
                    }

                    selCnt++;
                }
            }

            return cri;
        }

        private string getCustCri()
        {
            string cri = "";

            int selCnt = 0;

            string todayDay = DateTime.Now.DayOfWeek.ToString().Substring(0, 3);


            string strGroupCri = getGroupCri();
            string strRouteCri = getRouteCri();
            string strSPCri = getSPCri();
            //  string fCard = dtHead.GetValue("FCard", 0).ToString();

            string fCard = ((SAPbouiCOM.EditText)oForm.Items.Item("txFCard").Specific).Value.ToString();


            string strSelect = "Select CardCode from ocrd where CardType = 'C' and (CardCode like '" + fCard + "%' or CardName Like '" + fCard + "%' ) ";

            if (strGroupCri != "") strSelect += " And ocrd.groupCode in (" + strGroupCri + ")";
            if (strSPCri != "") strSelect += " And ocrd.slpcode in (" + strSPCri + ")";

            if (strRouteCri != "") strSelect += " And isnull(ocrd.U_Route" + todayDay + ",'')  in (" + strRouteCri + ")";

            cri = strSelect;


            return cri;
        }



        private void noSale(string callId)
        {
            if (callId == "0")
            {
                oApplication.MessageBox("Please select a Call");
            }
            else
            {
                string strDeleteOldSch = "update  [@B1_SCHCALL] set U_Status='Called'  where CODE='" + callId + "' and isnull(U_Status,'Open') ='Open' ";
                Program.objHrmsUI.ExecQuery(strDeleteOldSch, "Old Schedule");
                string strDate = Convert.ToString(dtHead.GetValue("CDate", 0));
                fillTeleSale(Convert.ToDateTime(strDate));


            }

        }
        private void executeScheduler(DateTime fromDt)
        {


            /*
            string strOldSche = "Select * from  [@B1_SCHCALL] where convert(date, U_Date)  ='" + fromDt.ToString("yyyyMMdd") + "'";

            System.Data.DataTable dtOldSch = Program.objHrmsUI.getDataTable(strOldSche, "CheckDailySch");
            if ( dtOldSch!=null &&  dtOldSch.Rows.Count > 0)
            {
                return;
            }
            */

            //// One time schedule entries for selected date

            int currentDay = 1;
            StringBuilder strSchInsert = new StringBuilder();
            int code = Convert.ToInt32(Program.objHrmsUI.getMaxId("[@B1_SCHCALL]", "CODE"));



            string strOT = @"SELECT        OT.Code, OT.Name, OT.U_SCCode, OT.U_Date, OT.U_Time
                            FROM           dbo. [@B1_SCHOT] OT
                            LEFT OUTER JOIN  dbo.[@B1_SCHCALL] ON dbo.[@B1_SCHCALL].U_SCCode = OT.U_sccode AND dbo.[@B1_SCHCALL].U_DATE = '" + fromDt.ToString("yyyyMMdd") + @"'
                            where OT.U_Date =  '" + fromDt.ToString("yyyyMMdd") + @"' AND (dbo.[@B1_SCHCALL].U_DATE IS NULL) ";

            System.Data.DataTable dtOT = Program.objHrmsUI.getDataTable(strOT, "DailySch");

            foreach (System.Data.DataRow dr in dtOT.Rows)
            {
                DateTime dt = fromDt;
                string schCode = dr["U_SCCode"].ToString();
                string RecType = "T";
                if (schCode.Substring(0, 2) == "SO") RecType = "O";

                string SchTime = dr["U_Time"].ToString();
                string CardCode = schCode.Replace("TS_", "").Replace("SO_", "");

                string todayDay = dt.DayOfWeek.ToString().Substring(0, 3);
                string strCardDetail = "Select cardname, isnull(u_Route" + todayDay + ",'') as Route from ocrd where cardcode = '" + CardCode + "'";
                string cardName = "";
                string route = "";
                string status = "Open";

                System.Data.DataTable dtCardDetail = Program.objHrmsUI.getDataTable(strCardDetail, "Card Detail");
                foreach (System.Data.DataRow drcrd in dtCardDetail.Rows)
                {
                    cardName = drcrd["CardName"].ToString();
                    route = drcrd["Route"].ToString();
                }
                string strCode = code.ToString().PadLeft(8, '0');
                strSchInsert.AppendLine(" Insert into   [@B1_SCHCALL] (Code, Name, U_SCCode, U_CardCode, U_CardName, U_Route, U_Time,U_DATE, U_Status, U_DocEntry, U_DocNum, U_Remarks, U_RecType) ");
                strSchInsert.Append(" Values ('" + strCode + "','" + strCode + "','" + schCode + "','" + CardCode + "','" + cardName.Replace("'", "''") + "','" + route + "','" + SchTime + "','" + dt.Date.ToString("yyyyMMdd") + "','" + status + "','0','0','','" + RecType + "');");
                code++;



            }





            string strDailySetup = @"SELECT   dbo.[@B1_CRDSCH].Code, dbo.[@B1_CRDSCH].Name, dbo.[@B1_CRDSCH].U_SchType, dbo.[@B1_CRDSCH].U_Active, dbo.[@B1_CRDSCH].U_Intrvl, dbo.[@B1_CRDSCH].U_W4, 
                                                         dbo.[@B1_CRDSCH].U_W5, dbo.[@B1_CRDSCH].U_W6, dbo.[@B1_CRDSCH].U_W7, dbo.[@B1_CRDSCH].U_CallTime, dbo.[@B1_CRDSCH].U_W1, dbo.[@B1_CRDSCH].U_W2, dbo.[@B1_CRDSCH].U_W3, 
                                                         dbo.[@B1_CRDSCH].U_slpCode
                                FROM            dbo.[@B1_CRDSCH] LEFT OUTER JOIN
                                                         dbo.[@B1_SCHCALL] ON dbo.[@B1_SCHCALL].U_SCCode = dbo.[@B1_CRDSCH].Code AND dbo.[@B1_SCHCALL].U_DATE = '" + fromDt.ToString("yyyyMMdd") + @"'
                                WHERE        (dbo.[@B1_CRDSCH].U_Intrvl = 'D') AND (ISNULL(dbo.[@B1_CRDSCH].U_Active, '') = 'Y') AND (dbo.[@B1_SCHCALL].U_DATE IS NULL) ";

            System.Data.DataTable dtSch = Program.objHrmsUI.getDataTable(strDailySetup, "DailySch");

            foreach (System.Data.DataRow dr in dtSch.Rows)
            {
                DateTime dt = fromDt;
                string schCode = dr["Code"].ToString();
                string RecType = dr["u_schType"].ToString();
                currentDay = 0;
                string SchTime = dr["U_CallTime"].ToString();
                string CardCode = schCode.Replace("TS_", "").Replace("SO_", "");





                string todayDay = dt.DayOfWeek.ToString().Substring(0, 3);
                string strCardDetail = "Select cardname, isnull(u_Route" + todayDay + ",'') as Route from ocrd where cardcode = '" + CardCode + "'";
                string cardName = "";
                string route = "";
                string status = "Open";

                System.Data.DataTable dtCardDetail = Program.objHrmsUI.getDataTable(strCardDetail, "Card Detail");
                foreach (System.Data.DataRow drcrd in dtCardDetail.Rows)
                {
                    cardName = drcrd["CardName"].ToString();
                    route = drcrd["Route"].ToString();
                }
                string strCode = code.ToString().PadLeft(8, '0');
                strSchInsert.AppendLine(" Insert into   [@B1_SCHCALL] (Code, Name, U_SCCode, U_CardCode, U_CardName, U_Route, U_Time,U_DATE, U_Status, U_DocEntry, U_DocNum, U_Remarks, U_RecType) ");
                strSchInsert.Append(" Values ('" + strCode + "','" + strCode + "','" + schCode + "','" + CardCode + "','" + cardName.Replace("'", "''") + "','" + route + "','" + SchTime + "','" + dt.Date.ToString("yyyyMMdd") + "','Open','0','0','','" + RecType + "');");
                dt = dt.AddDays(1);
                currentDay++;
                code++;



            }



            // Weekly Schedul



            strDailySetup = @"SELECT   dbo.[@B1_CRDSCH].Code, dbo.[@B1_CRDSCH].Name, dbo.[@B1_CRDSCH].U_SchType, dbo.[@B1_CRDSCH].U_Active, dbo.[@B1_CRDSCH].U_Intrvl, dbo.[@B1_CRDSCH].U_W4, 
                                                         dbo.[@B1_CRDSCH].U_W5, dbo.[@B1_CRDSCH].U_W6, dbo.[@B1_CRDSCH].U_W7, dbo.[@B1_CRDSCH].U_CallTime, dbo.[@B1_CRDSCH].U_W1, dbo.[@B1_CRDSCH].U_W2, dbo.[@B1_CRDSCH].U_W3, 
                                                         dbo.[@B1_CRDSCH].U_slpCode , isnull(U_EWN,1) U_EWN, U_EWD , datediff(WW , U_EWD,'" + fromDt.ToString("yyyyMMdd") + @"') as WeekNums 
                                FROM            dbo.[@B1_CRDSCH] LEFT OUTER JOIN
                                                         dbo.[@B1_SCHCALL] ON dbo.[@B1_SCHCALL].U_SCCode = dbo.[@B1_CRDSCH].Code AND dbo.[@B1_SCHCALL].U_DATE = '" + fromDt.ToString("yyyyMMdd") + @"'
                                WHERE  isnull(U_EWD,'1/1/1900') <= '" + fromDt.ToString("yyyyMMdd") + "' and   isnull(U_EWD,'1/1/1900') > '1/1/2016' and      (dbo.[@B1_CRDSCH].U_Intrvl = 'W') AND (ISNULL(dbo.[@B1_CRDSCH].U_Active, '') = 'Y') AND (dbo.[@B1_SCHCALL].U_DATE IS NULL) ";

            dtSch = Program.objHrmsUI.getDataTable(strDailySetup, "Weekly Sch");

            foreach (System.Data.DataRow dr in dtSch.Rows)
            {
                DateTime dt = fromDt;
                int EWN = Convert.ToInt32(dr["U_EWN"]);
                DateTime SchFrom = Convert.ToDateTime(dr["U_EWD"]);

                int weekBetween = Convert.ToInt32(dr["WeekNums"]);
                string schCode = dr["Code"].ToString();
                string RecType = dr["u_schType"].ToString();

                currentDay = 0;
                string SchTime = dr["U_CallTime"].ToString();
                string CardCode = schCode.Replace("TS_", "").Replace("SO_", "");

                string todayDay = dt.DayOfWeek.ToString().Substring(0, 3);

                string addSchedule = "N";
                if (weekBetween % EWN == 0)
                {
                    if (todayDay == "Mon" && dr["U_W1"].ToString() == "Y") addSchedule = "Y";
                    if (todayDay == "Tue" && dr["U_W2"].ToString() == "Y") addSchedule = "Y";
                    if (todayDay == "Wed" && dr["U_W3"].ToString() == "Y") addSchedule = "Y";
                    if (todayDay == "Thu" && dr["U_W4"].ToString() == "Y") addSchedule = "Y";
                    if (todayDay == "Fri" && dr["U_W5"].ToString() == "Y") addSchedule = "Y";
                    if (todayDay == "Sat" && dr["U_W6"].ToString() == "Y") addSchedule = "Y";
                    if (todayDay == "Sun" && dr["U_W7"].ToString() == "Y") addSchedule = "Y";
                }

                if (addSchedule == "Y")
                {
                    string strCardDetail = "Select cardname, isnull(u_Route" + todayDay + ",'') as Route from ocrd where cardcode = '" + CardCode + "'";
                    string cardName = "";
                    string route = "";
                    string status = "Open";

                    System.Data.DataTable dtCardDetail = Program.objHrmsUI.getDataTable(strCardDetail, "Card Detail");
                    foreach (System.Data.DataRow drcrd in dtCardDetail.Rows)
                    {
                        cardName = drcrd["CardName"].ToString();
                        route = drcrd["Route"].ToString();
                    }
                    string strCode = code.ToString().PadLeft(8, '0');
                    strSchInsert.AppendLine(" Insert into   [@B1_SCHCALL] (Code, Name, U_SCCode, U_CardCode, U_CardName, U_Route, U_Time,U_DATE, U_Status, U_DocEntry, U_DocNum, U_Remarks,U_RecType) ");
                    strSchInsert.Append(" Values ('" + strCode + "','" + strCode + "','" + schCode + "','" + CardCode + "','" + cardName.Replace("'", "''") + "','" + route + "','" + SchTime + "','" + dt.Date.ToString("yyyyMMdd") + "','Open','0','0','','" + RecType + "');");
                    code++;
                }
                dt = dt.AddDays(1);
                currentDay++;


            }




            // Monthly Date  Schedul


            string strGetSch = @"SELECT        TOP (200) Code, Name, U_SchType, U_Active, U_Intrvl, U_W4, U_W5, U_W6, U_W7, U_CallTime, U_W1, U_W2, U_W3, u_slpcode
                                        FROM            [@B1_CRDSCH]  where  isnull(U_Active,'') = 'Y' and U_Intrvl = 'M' and isnull(U_MOType,'') = 'DT'   ";



            strGetSch = @"SELECT   dbo.[@B1_CRDSCH].Code, dbo.[@B1_CRDSCH].Name, dbo.[@B1_CRDSCH].U_SchType, dbo.[@B1_CRDSCH].U_Active, dbo.[@B1_CRDSCH].U_Intrvl, dbo.[@B1_CRDSCH].U_W4, 
                                                         dbo.[@B1_CRDSCH].U_W5, dbo.[@B1_CRDSCH].U_W6, dbo.[@B1_CRDSCH].U_W7, dbo.[@B1_CRDSCH].U_CallTime, dbo.[@B1_CRDSCH].U_W1, dbo.[@B1_CRDSCH].U_W2, dbo.[@B1_CRDSCH].U_W3, 
                                                         dbo.[@B1_CRDSCH].U_slpCode
                                FROM            dbo.[@B1_CRDSCH] LEFT OUTER JOIN
                                                         dbo.[@B1_SCHCALL] ON dbo.[@B1_SCHCALL].U_SCCode = dbo.[@B1_CRDSCH].Code AND dbo.[@B1_SCHCALL].U_DATE = '" + fromDt.ToString("yyyyMMdd") + @"'
                                WHERE        (U_Intrvl = 'M' and isnull(U_MOType,'') = 'DT') AND (ISNULL(dbo.[@B1_CRDSCH].U_Active, '') = 'Y') AND (dbo.[@B1_SCHCALL].U_DATE IS NULL) ";


            dtSch = Program.objHrmsUI.getDataTable(strGetSch, "Montyly Date Sch");

            foreach (System.Data.DataRow dr in dtSch.Rows)
            {
                DateTime dt = fromDt;
                string schCode = dr["Code"].ToString();
                string RecType = dr["u_schType"].ToString();

                currentDay = 0;
                string SchTime = dr["U_CallTime"].ToString();
                string CardCode = schCode.Replace("TS_", "").Replace("SO_", "");

                System.Data.DataTable dtDates = Program.objHrmsUI.getDataTable("select u_mdates [Dates] from [@B1_SCHMDT] where u_sCCode='" + schCode + "'", "getDates");
                if (dtDates.Rows.Count == 0)
                {

                }
                else
                {

                    string todayDay = dt.DayOfWeek.ToString().Substring(0, 3);
                    // string todayWeekDayNum = dt.DayOfWeek.
                    string todayDate = dt.Date.Day.ToString();
                    string addSchedule = "N";
                    System.Data.DataRow[] dtrows = dtDates.Select("Dates='" + todayDate + "'");
                    if (dtrows.Count() > 0) addSchedule = "Y";
                    if (addSchedule == "Y")
                    {
                        string strCardDetail = "Select cardname, isnull(u_Route" + todayDay + ",'') as Route from ocrd where cardcode = '" + CardCode + "'";
                        string cardName = "";
                        string route = "";

                        System.Data.DataTable dtCardDetail = Program.objHrmsUI.getDataTable(strCardDetail, "Card Detail");
                        foreach (System.Data.DataRow drcrd in dtCardDetail.Rows)
                        {
                            cardName = drcrd["CardName"].ToString();
                            route = drcrd["Route"].ToString();
                        }
                        string strCode = code.ToString().PadLeft(12, '0');
                        strSchInsert.AppendLine(" Insert into   [@B1_SCHCALL] (Code, Name, U_SCCode, U_CardCode, U_CardName, U_Route, U_Time,U_DATE, U_Status, U_DocEntry, U_DocNum, U_Remarks,U_RecType) ");
                        strSchInsert.Append(" Values ('" + strCode + "','" + strCode + "','" + schCode + "','" + CardCode + "','" + cardName.Replace("'", "''") + "','" + route + "','" + SchTime + "','" + dt.Date.ToString("yyyyMMdd") + "','Open','0','0','','" + RecType + "');");
                        code++;
                    }
                    dt = dt.AddDays(1);
                    currentDay++;



                }
            }



            // Monthly Day of Week  Schedul


            strGetSch = @"SELECT        TOP (200) Code, Name, U_SchType, U_Active, U_Intrvl, U_W4, U_W5, U_W6, U_W7, U_CallTime, U_W1, U_W2, U_W3, u_slpcode
                                        FROM            [@B1_CRDSCH]  where  isnull(U_Active,'') = 'Y' and U_Intrvl = 'M' and isnull(U_MOType,'') = 'DY'   ";

            strGetSch = @"SELECT   dbo.[@B1_CRDSCH].Code, dbo.[@B1_CRDSCH].Name, dbo.[@B1_CRDSCH].U_SchType, dbo.[@B1_CRDSCH].U_Active, dbo.[@B1_CRDSCH].U_Intrvl, dbo.[@B1_CRDSCH].U_W4, 
                                                         dbo.[@B1_CRDSCH].U_W5, dbo.[@B1_CRDSCH].U_W6, dbo.[@B1_CRDSCH].U_W7, dbo.[@B1_CRDSCH].U_CallTime, dbo.[@B1_CRDSCH].U_W1, dbo.[@B1_CRDSCH].U_W2, dbo.[@B1_CRDSCH].U_W3, 
                                                         dbo.[@B1_CRDSCH].U_slpCode
                                FROM            dbo.[@B1_CRDSCH] LEFT OUTER JOIN
                                                         dbo.[@B1_SCHCALL] ON dbo.[@B1_SCHCALL].U_SCCode = dbo.[@B1_CRDSCH].Code AND dbo.[@B1_SCHCALL].U_DATE = '" + fromDt.ToString("yyyyMMdd") + @"'
                                WHERE        (U_Intrvl = 'M' and isnull(U_MOType,'') = 'DY') AND (ISNULL(dbo.[@B1_CRDSCH].U_Active, '') = 'Y') AND (dbo.[@B1_SCHCALL].U_DATE IS NULL) ";

            dtSch = Program.objHrmsUI.getDataTable(strGetSch, "Montyly Day of Week Sch");

            foreach (System.Data.DataRow dr in dtSch.Rows)
            {
                DateTime dt = fromDt;
                string schCode = dr["Code"].ToString();
                string RecType = dr["u_schType"].ToString();

                currentDay = 0;
                string SchTime = dr["U_CallTime"].ToString();
                string CardCode = schCode.Replace("TS_", "").Replace("SO_", "");



                System.Data.DataTable dtDates = Program.objHrmsUI.getDataTable("SELECT   Code, Name, U_SCCode, U_Day as WeekDay, U_WeekNum as WeekNum FROM [@B1_SCHMDY] where u_sCCode='" + schCode + "'", "getWeekDays");
                if (dtDates.Rows.Count == 0)
                {

                }
                else
                {

                    string todayDay = dt.DayOfWeek.ToString().Substring(0, 3);
                    string todayDate = dt.Date.Day.ToString();
                    DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                    Calendar cal = dfi.Calendar;
                    DateTime firstDateofMonth = new DateTime(dt.Year, dt.Month, 1);
                    int weekofMonth = cal.GetWeekOfYear(dt, dfi.CalendarWeekRule, dfi.FirstDayOfWeek) - cal.GetWeekOfYear(firstDateofMonth, dfi.CalendarWeekRule, dfi.FirstDayOfWeek) + 1;

                    string addSchedule = "N";
                    System.Data.DataRow[] dtrows = dtDates.Select("WeekDay='" + todayDay + "' and WeekNum = '" + weekofMonth.ToString() + "'");
                    if (dtrows.Count() > 0) addSchedule = "Y";
                    if (addSchedule == "Y")
                    {
                        string strCardDetail = "Select cardname, isnull(u_Route" + todayDay + ",'') as Route from ocrd where cardcode = '" + CardCode + "'";
                        string cardName = "";
                        string route = "";

                        System.Data.DataTable dtCardDetail = Program.objHrmsUI.getDataTable(strCardDetail, "Card Detail");
                        foreach (System.Data.DataRow drcrd in dtCardDetail.Rows)
                        {
                            cardName = drcrd["CardName"].ToString();
                            route = drcrd["Route"].ToString();
                        }

                        string strCode = code.ToString().PadLeft(8, '0');
                        strSchInsert.AppendLine(" Insert into   [@B1_SCHCALL] (Code, Name, U_SCCode, U_CardCode, U_CardName, U_Route, U_Time,U_DATE, U_Status, U_DocEntry, U_DocNum, U_Remarks,U_RecType) ");
                        strSchInsert.Append(" Values ('" + strCode + "','" + strCode + "','" + schCode + "','" + CardCode + "','" + cardName.Replace("'", "''") + "','" + route + "','" + SchTime + "','" + dt.Date.ToString("yyyyMMdd") + "','Open','0','0','','" + RecType + "');");

                        code++;
                    }
                    dt = dt.AddDays(1);
                    currentDay++;




                }

            }
            string strQuer = strSchInsert.ToString();
            if (strQuer != "")
            {
                Program.objHrmsUI.ExecQuery(strQuer, "Monthly Date Schedule Insert");
            }
        }

        private void fillTeleSale(DateTime dtDate)
        {

            dtTSO.Rows.Clear();
            dtSTSO.Rows.Clear();
            //     oForm.Freeze(true);

            try
            {
                string custCri = getCustCri();

                string strSql = @"SELECT         Code, Name, U_SCCode, U_CardCode, U_CardName, U_Route, U_Time, U_Status, U_DocEntry, U_DocNum, U_Remarks, u_Date , U_RecType
                                FROM            [@B1_SCHCALL] where u_Status='Open' and   u_Date='" + dtDate.ToString("yyyyMMdd") + "'";


                if (custCri != "")
                {
                    strSql += " and U_CardCode in (" + custCri + ") ";
                }
                System.Data.DataTable TSSchedule = Program.objHrmsUI.getDataTable(strSql, "Filling TSO");
                int j = 0;
                int k = 0;

                foreach (System.Data.DataRow dr in TSSchedule.Rows)
                {
                    string recType = dr["U_RecType"].ToString();
                    if (recType == "T")
                    {
                        dtTSO.Rows.Add(1);
                        dtTSO.SetValue("Id", j, (j + 1).ToString());
                        dtTSO.SetValue("CardCode", j, dr["U_CardCode"].ToString());
                        dtTSO.SetValue("CardName", j, dr["U_CardName"].ToString());
                        dtTSO.SetValue("Route", j, dr["U_Route"].ToString());
                        dtTSO.SetValue("CallTime", j, Program.objHrmsUI.getStrTime(Convert.ToInt32(dr["U_Time"])).ToString());
                        dtTSO.SetValue("Status", j, dr["U_Status"].ToString());
                        dtTSO.SetValue("CallId", j, dr["Code"].ToString());
                        if (dr["U_DocNum"].ToString() != "0")
                        {
                            dtTSO.SetValue("Order", j, dr["U_DocEntry"].ToString());
                        }

                        j++;
                    }
                    else
                    {
                        dtSTSO.Rows.Add(1);
                        dtSTSO.SetValue("Id", k, (k + 1).ToString());
                        dtSTSO.SetValue("CardCode", k, dr["U_CardCode"].ToString());
                        dtSTSO.SetValue("CardName", k, dr["U_CardName"].ToString());
                        dtSTSO.SetValue("Route", k, dr["U_Route"].ToString());
                        dtSTSO.SetValue("Status", k, dr["U_Status"].ToString());
                        dtSTSO.SetValue("CallId", k, dr["Code"].ToString());
                        if (dr["U_DocNum"].ToString() != "0")
                        {
                            dtSTSO.SetValue("DocNum", k, dr["U_DocEntry"].ToString());
                        }

                        k++;
                    }




                }
                mtSOP.LoadFromDataSource();
                mtSTO.LoadFromDataSource();
                tbOpr2.Caption = "Standing Orders (" + k.ToString() + ")";
                tbOpr1.Caption = "Sales Opportunity (" + j.ToString() + ")";
            }
            catch (Exception ex)
            {
                oApplication.SetStatusBarMessage(ex.Message);

            }

            //   oForm.Freeze(false);
        }


        private void fillNearExpiry()
        {

            dtBB.Rows.Clear();

            string strSql = @"select 
                                    OIBT.ItemCode,oitm.ItemName,OIBT.Quantity, OIBT.ExpDate ,oibt.WhsCode
                                    from OIBT inner join oitm on oitm.itemcode = OIBT.ItemCode

                                    where ExpDate between getdate() and GETDATE() + " + drSetting["U_SED"].ToString() + @"
                                    and OIBT.Quantity>0
                                    and oibt.WhsCode = '01'
                    ";


            System.Data.DataTable TSSchedule = Program.objHrmsUI.getDataTable(strSql, "Filling TSO");
            int j = 0;
            foreach (System.Data.DataRow dr in TSSchedule.Rows)
            {

                dtBB.Rows.Add(1);
                dtBB.SetValue("Id", j, (j + 1).ToString());
                dtBB.SetValue("ItemCode", j, dr["ItemCode"].ToString());
                dtBB.SetValue("ItemName", j, dr["ItemName"].ToString());
                dtBB.SetValue("Quantity", j, dr["Quantity"].ToString());
                dtBB.SetValue("BB", j, Convert.ToDateTime(dr["ExpDate"]));

                j++;



            }





            mtBB.LoadFromDataSource();

        }



        private void fillTelesaleAnalysis()
        {
            string strSql = @" 

                   
                    
                    select 
                    sum(  case when (u_status<>'open' and  (DATEPART(WW, t0.u_date) = datepart(WW, CONVERT(date, getdate())))) then 1 else 0 end)  as ThisWeek   ,
                    sum( case when (u_status<>'open' and (DATEPART(WW, t0.u_date) = datepart(ww, dateadd(WW,-1, CONVERT(date, getdate()))))) then 1 else 0 end)  as previousWeek   
                    from   [@B1_SCHCALL] t0  where t0.u_date>=DATEADD(WW,-2,GETDATE())

                    union all



                    select 
                    sum(  case when (canceled= 'N' and  (DATEPART(WW, t0.DocDate) = datepart(WW, CONVERT(date, getdate())))) then DocTotal else 0 end)  as ThisWeek   ,
                    sum( case when (canceled= 'N'  and (DATEPART(WW, t0.DocDate) = datepart(ww, dateadd(WW,-1, CONVERT(date, getdate()))))) then DocTotal else 0 end)  as previousWeek   
                    from   ordr t0 where t0.docdate>=DATEADD(WW,-2,GETDATE())

                    union all


                    select 
                    sum(  case when (u_status<>'open' and  ( t0.u_date >  dateadd(d, -7 , getdate()))) then 1 else 0 end)  as ThisWeek   ,
                    sum(  case when (U_DocEntry<>'0' and  ( t0.u_date >  dateadd(d, -7 , getdate()))) then 1 else 0 end)    as previousWeek   
                    from   [@B1_SCHCALL] t0 where t0.u_date>=DATEADD(WW,-2,GETDATE())
                    
                    union all

                    select 
                    sum(  case when (u_status<>'open' and  convert(date,t0.u_date) = CONVERT(date, getdate())) then 1 else 0 end)  as ThisWeek   ,
                      sum(  case when (u_status<>'Order Added' and  convert(date,t0.u_date) = CONVERT(date, getdate())) then 1 else 0 end) previousWeek   
                    from   [@B1_SCHCALL] t0 where t0.u_date>=DATEADD(WW,-2,GETDATE())

                    ";

            System.Data.DataTable TSSchedule = Program.objHrmsUI.getDataTable(strSql, "Filling AI");
            int j = 0;
            foreach (System.Data.DataRow dr in TSSchedule.Rows)
            {
                double Val1 = Convert.ToDouble(dr[0]);
                double Val2 = Convert.ToDouble(dr[1]);
                double difPrcnt = 0;
                if (Val1 > 0)
                {
                    difPrcnt = 100 * Val2 / Val1;
                }
                else
                {
                    difPrcnt = 0.00;
                }

                switch (j)
                {

                    case 0:

                        dtTSA.SetValue("TS11", 0, Val1.ToString());
                        dtTSA.SetValue("TS12", 0, Val2.ToString());
                        dtTSA.SetValue("TS13", 0, difPrcnt.ToString());
                        break;
                    case 1:
                        dtTSA.SetValue("TS21", 0, Val1.ToString());
                        dtTSA.SetValue("TS22", 0, Val2.ToString());
                        dtTSA.SetValue("TS23", 0, difPrcnt.ToString());

                        break;
                    case 2:
                        dtTSA.SetValue("TS31", 0, Val1.ToString());
                        dtTSA.SetValue("TS32", 0, Val2.ToString());
                        if (Val1 > 0)
                        {
                            difPrcnt = 100 * Val2 / Val1;
                        }
                        else
                        {
                            difPrcnt = 0.00;
                        }

                        dtTSA.SetValue("TS33", 0, difPrcnt.ToString());

                        break;
                    case 3:

                        dtTSA.SetValue("TS31", 0, Val1.ToString());
                        dtTSA.SetValue("TS32", 0, Val2.ToString());
                        if (Val1 > 0)
                        {
                            difPrcnt = 100 * Val2 / Val1;
                        }
                        else
                        {
                            difPrcnt = 0.00;
                        }

                        dtTSA.SetValue("TS33", 0, difPrcnt.ToString());

                        dtTSA.SetValue("NCMT", 0, dr[0].ToString());
                        //   dtTSA.SetValue("TS32", 0, dr[1].ToString());
                        // dtTSA.SetValue("TS33", 0, dr[0].ToString());

                        break;
                }

                j++;



            }



        }
        private void fillAI(string itemCode)
        {

            dtAI.Rows.Clear();

            string strSql = @"select OrigItem,AltItem,Match , oitm.ItemName,oitm.onhand
                                from OALI inner join oitm on oitm.itemcode = oali.OrigItem
                                where OrigItem = '" + itemCode + @"'
                    ";


            System.Data.DataTable TSSchedule = Program.objHrmsUI.getDataTable(strSql, "Filling AI");
            int j = 0;
            foreach (System.Data.DataRow dr in TSSchedule.Rows)
            {

                dtAI.Rows.Add(1);
                dtAI.SetValue("Id", j, (j + 1).ToString());
                dtAI.SetValue("ItemCode", j, dr["AltItem"].ToString());
                dtAI.SetValue("ItemName", j, dr["ItemName"].ToString());
                dtAI.SetValue("Quantity", j, dr["onhand"].ToString());
                dtAI.SetValue("match", j, dr["Match"].ToString());

                j++;



            }





            mtAI.LoadFromDataSource();

        }


        private void fillCustomerData(int selRow, string callType)
        {
            string cardCode = "";
            // System.Data.DataTable dtProducts = new System.Data.DataTable();
            if (callType == "ST")
            {
                cardCode = Convert.ToString(dtSTSO.GetValue("CardCode", selRow - 1));

            }
            else if (callType == "TS")
            {
                cardCode = Convert.ToString(dtTSO.GetValue("CardCode", selRow - 1));
            }
            else if (callType == "CST")
            {
                cardCode = Convert.ToString(dtCust.GetValue("CardCode", selRow - 1));
            }
            if (cardCode == "")
            {
                oApplication.MessageBox("Select Tele Sale Call or Standing Order to get customer detail");
                return;
            }


            string strCardDetail = @"
                    select 
                    CardName,
                    isnull(ocrd.Phone1,'') as Phone,
                    ocrd.E_Mail,
                    ocrd.Balance,
                    ocrd.ListNum,
                    CreditLine ,
                    opln.ListName,
                    CntctPrsn,
                    free_text ,
                    tcall.u_remarks
                   ,OSHP.TrnspName, crd1.Street,crd1.Block ,crd1.address, crd1.Address2 , crd1.City , crd1.State, crd1.ZipCode, octg.PymntGroup , opym.Descript as [pmtMthod]
                  
                    from ocrd inner join opln on opln.ListNum = ocrd.ListNum inner join  [@B1_SCHCALL] tCall on tcall.u_CardCode = ocrd.cardcode and tCall.Code='" + callId + "'";


            if (callId == "0")
            {
                strCardDetail = @"
                    select 
                    CardName,
                    isnull(ocrd.Phone2,'') as Phone,
                    ocrd.E_Mail,
                    ocrd.Balance,
                    ocrd.ListNum,
                    CreditLine ,
                    opln.ListName,
                    CntctPrsn,
                    free_text ,
                   '' as  u_remarks
                     ,OSHP.TrnspName, crd1.Street,crd1.Block ,crd1.address, crd1.Address2 , crd1.City , crd1.State, crd1.ZipCode, octg.PymntGroup , opym.Descript as [pmtMthod]
                  
                    from ocrd inner join opln on opln.ListNum = ocrd.ListNum ";

            }
            strCardDetail += @" left outer join crd1 on ocrd.CardCode = crd1.CardCode and crd1.Address = ocrd.ShipToDef
					            left outer join OCTG on ocrd.GroupNum = octg.GroupNum
					            left outer  join OPYM on opym.PayMethCod = ocrd.PymCode 
                                left outer join oshp on ocrd.ShipType = OSHP.TrnspCode";
            strCardDetail += "  where ocrd.cardcode = '" + cardCode + "'";
            System.Data.DataTable dt = Program.objHrmsUI.getDataTable(strCardDetail, "Get card detail");

            foreach (System.Data.DataRow dr in dt.Rows)
            {
                try
                {
                    dtHead.SetValue("CardCode", 0, cardCode);
                    dtHead.SetValue("CardName", 0, dr["CardName"].ToString());

                    dtHead.SetValue("CP", 0, dr["CntctPrsn"].ToString());

                    dtHead.SetValue("Tel", 0, dr["Phone"].ToString());
                    dtHead.SetValue("PL", 0, dr["ListName"].ToString());
                    dtHead.SetValue("PLN", 0, dr["ListNum"].ToString());
                    dtHead.SetValue("CL", 0, dr["CreditLine"].ToString());

                    dtHead.SetValue("Balance", 0, dr["Balance"].ToString());
                    dtHead.SetValue("Email", 0, dr["E_Mail"].ToString());

                    dtHead.SetValue("CustNotes", 0, dr["free_text"].ToString().Length > 254 ? dr["free_text"].ToString().Substring(0, 254) : dr["free_text"].ToString());
                    dtHead.SetValue("CallNotes", 0, dr["u_remarks"].ToString().Length > 254 ? dr["u_remarks"].ToString().Substring(0, 254) : dr["u_remarks"].ToString());



                    dtCard.SetValue("Shipto", 0, dr["address"].ToString());
                    dtCard.SetValue("Addr1", 0, dr["Street"].ToString());
                    dtCard.SetValue("Addr2", 0, dr["Block"].ToString());
                    dtCard.SetValue("City", 0, dr["City"].ToString());
                    dtCard.SetValue("State", 0, dr["State"].ToString());
                    dtCard.SetValue("Zip", 0, dr["ZipCode"].ToString());
                    dtCard.SetValue("ShpType", 0, dr["TrnspName"].ToString());
                    dtCard.SetValue("PmtTrm", 0, dr["PymntGroup"].ToString());
                    dtCard.SetValue("PmtMthd", 0, dr["pmtMthod"].ToString());
                }
                catch (Exception ex)
                {
                }

            }



            try { fillCustOrderList(cardCode); }
            catch { }
            try { fillCustMSP(cardCode); }
            catch { }
            try { fillCRDRP(cardCode); }
            catch { }
            try { fillOffers(cardCode); }
            catch { }
        }
        private void fillCustOrderList(string cardCode)
        {
            dtCustPr.Rows.Clear();
            string listType = "";
            string strSelect = "";




            strSelect = @"select  ordr.docnum [Order],ordr.docdate [Date],rdr1.itemcode ,rdr1.Dscription as  [Product] ,rdr1.quantity [Quantity] , 0 [WT] ,rdr1.Price [Price] 
                            from ordr inner join rdr1 on ordr.docentry = rdr1.DocEntry
                            where ordr.docentry in ( select top " + drSetting["U_NLastOrdr"].ToString() + @" topOrders.Docentry from ORDR topOrders where topOrders.CardCode = '" + cardCode + @"'   order by topOrders.DocDate desc) 
                            
                          ";



            dtCustPr.Rows.Clear();
            System.Data.DataTable dtProducts = Program.objHrmsUI.getDataTable(strSelect, "Filling Product");
            int j = 0;
            foreach (System.Data.DataRow dr in dtProducts.Rows)
            {
                dtCustPr.Rows.Add(1);
                dtCustPr.SetValue("Id", j, (j + 1).ToString());
                dtCustPr.SetValue("Order", j, dr["Order"].ToString());
                dtCustPr.SetValue("Date", j, Convert.ToDateTime(dr["Date"]));
                dtCustPr.SetValue("Product", j, dr["Product"].ToString());
                dtCustPr.SetValue("Quantity", j, dr["Quantity"].ToString());
                dtCustPr.SetValue("WT", j, dr["WT"].ToString());
                dtCustPr.SetValue("Price", j, dr["Price"].ToString());
                dtCustPr.SetValue("ItemCode", j, dr["ItemCode"].ToString());



                j++;
            }
            mtCustRP.LoadFromDataSource();



        }



        private void fillOffers(string cardCode)
        {
            dtSPO.Rows.Clear();
            string strSelect = "";

            string listNum = dtHead.GetValue("PLN", 0).ToString();



            strSelect = @"SELECT        dbo.OSPP.ItemCode, dbo.OITM.ItemName, ISNULL(dbo.SPP2.Amount, 1) AS Quantity, ISNULL(dbo.SPP2.Price, isnull( dbo.SPP1.Price,ospp.Price)) AS Price, ISNULL(dbo.SPP2.Discount, isnull(dbo.SPP1.Discount,ospp.Discount)) AS Discount , OSPP.CardCode
                            FROM            OSPP inner join    dbo.OITM ON dbo.OSPP.ItemCode = dbo.OITM.ItemCode  
							left outer join SPP1 on dbo.SPP1.ItemCode = dbo.OSPP.ItemCode and OSPP.ListNum = spp1.ListNum
							and isnull(dbo.spp1.FromDate ,'20100101') < getdate() and isnull(dbo.SPP1.ToDate ,'20300101') > getdate()  
                                                   
													  LEFT OUTER JOIN
                                                     dbo.SPP2 ON dbo.SPP1.ItemCode = dbo.SPP2.ItemCode AND dbo.SPP1.LINENUM = dbo.SPP2.SPP1LNum ";
                           //where   dbo.OSPP.Valid='Y' and  isnull(dbo.ospp.ValidFrom ,'20100101') < getdate() and isnull(dbo.ospp.ValidFrom ,'20300101') > getdate() "; --#AO these fields are not available for SAP 9.1

            strSelect += " where dbo.OSPP.ListNum = '" + listNum + "' and (ospp.CardCode = '" + cardCode + "' or ospp.CardCode = '*' + '" + listNum + "') ";
								



            dtSPO.Rows.Clear();
            System.Data.DataTable dtProducts = Program.objHrmsUI.getDataTable(strSelect, "Filling Product");
            int j = 0;
            foreach (System.Data.DataRow dr in dtProducts.Rows)
            {
                double discPercent = Convert.ToDouble(dr["Discount"]);
                if (discPercent > 0)
                {
                    dtSPO.Rows.Add(1);
                    dtSPO.SetValue("Id", j, (j + 1).ToString());
                    dtSPO.SetValue("ItemCode", j, dr["ItemCode"].ToString());
                    dtSPO.SetValue("ItemName", j, dr["ItemName"].ToString());
                    dtSPO.SetValue("Quantity", j, dr["Quantity"].ToString());
                    //    dtSPO.SetValue("Price", j, dr["Price"].ToString());
                    dtSPO.SetValue("DiscPer", j, dr["Discount"].ToString());
                    j++;
                }
            }
            mtSPO.LoadFromDataSource();



        }




        private void fillStockVal(string itemCode)
        {
            dtStock.Rows.Clear();
            string strSelect = "";
            double totalStockQty = 0;



            strSelect = "	select PMX_OSSL.Name , PMX_INVT.ItemCode , pmx_itri.batchNumber,  pmx_itri.BestBeforeDate , sum(PMX_INVT.Quantity) as Quantity ,sum(PMX_INVT.QuantityUom2) UOM2QTY   ";
            strSelect += " from PMX_INVT  inner join PMX_OSSL on PMX_INVT.StorLocCode = PMX_OSSL.Code inner join pmx_itri on pmx_itri.internalkey = PMX_INVT.ItemTransactionalInfoKey ";
            strSelect += " where PMX_INVT.ItemCode='" + itemCode + "' ";
            strSelect += " group by PMX_OSSL.Name , PMX_INVT.ItemCode , pmx_itri.batchNumber, pmx_itri.BestBeforeDate ";
            strSelect += "having sum(PMX_INVT.Quantity)>0 or   sum(PMX_INVT.QuantityUom2)>0 order by pmx_itri.BestBeforeDate  ";



            System.Data.DataTable dtProducts = Program.objHrmsUI.getDataTable(strSelect, "Filling Product");
            int j = 0;
            foreach (System.Data.DataRow dr in dtProducts.Rows)
            {
                dtStock.Rows.Add(1);
                dtStock.SetValue("Id", j, (j + 1).ToString());
                dtStock.SetValue("onhand", j, dr["Quantity"].ToString());
                dtStock.SetValue("comited", j, dr["UOM2QTY"].ToString());
                dtStock.SetValue("UOM", j, dr["Name"].ToString());
                DateTime bb = Convert.ToDateTime(dr["BestBeforeDate"]);
                dtStock.SetValue("BN", j, dr["batchNumber"].ToString());
                dtStock.SetValue("BB", j, bb.ToString("yyyy.MM.dd"));
                totalStockQty = totalStockQty + Convert.ToDouble(dr["Quantity"].ToString());

                j++;
            }
            mtStock.LoadFromDataSource();

            //Get total planned quantity for the due date until u_state change to 1 and ordet status is not completed
            string strDate = Convert.ToString(dtHead.GetValue("dDate", 0));
            string strDate2 = Convert.ToString(dtHead.GetValue("CDate", 0));
            DateTime dtdel = Convert.ToDateTime(strDate);

            StringBuilder sb = new StringBuilder();
            sb.Append("	select SUM(isnull(wor1.PlannedQty,0)) as QtyToProduce\r\n");
            sb.Append("	from wor1 \r\n");
            sb.Append("	left join owor on owor.DocEntry = wor1.DocEntry\r\n");
            sb.Append("	where \r\n");
            sb.AppendFormat("	owor.DueDate = '{0}'\r\n", dtdel.ToString("yyy-MM-dd"));
            sb.Append("	and \r\n");
            sb.AppendFormat("	wor1.ItemCode = '{0}'\r\n", itemCode);
            sb.Append("	and\r\n");
            sb.Append("	(owor.Status = 'P' or owor.Status = 'R')\r\n");
            sb.Append("	and \r\n");
            sb.Append("	ISNULL(owor.U_Status,0) = 0\r\n");
            sb.Append("    Group by owor.DueDate, wor1.ItemCode\r\n");
            double QtyToProduce = 0;
            string sQtyToProduce = Program.objHrmsUI.getScallerValue(sb.ToString());
            if (!string.IsNullOrEmpty(sQtyToProduce))
                QtyToProduce = Convert.ToDouble(sQtyToProduce);

            //When the Order is soft Close then
            //Get total produced qty looking to orders still not closed by sap but soft closed by dmx
            sb = new StringBuilder();
            sb.Append("	select SUM(wor1.IssuedQty) as QtyProduced \r\n");
            sb.Append("	from wor1 \r\n");
            sb.Append("	left join owor on owor.DocEntry = wor1.DocEntry \r\n");
            sb.Append("	where \r\n");
            sb.AppendFormat("	owor.DueDate = '{0}' \r\n", dtdel.ToString("yyy-MM-dd"));
            sb.Append("	and \r\n");
            sb.AppendFormat("	wor1.ItemCode = '{0}' \r\n", itemCode);
            sb.Append("	and\r\n");
            sb.Append("	(owor.Status = 'P' or owor.Status = 'R') \r\n");
            sb.Append("	and \r\n");
            sb.Append("	ISNULL(owor.U_Status,0) = 1\r\n");
            sb.Append("    Group by owor.DueDate, wor1.ItemCode \r\n");
            double QtyProduced = 0;
            string sQtyProduced = Program.objHrmsUI.getScallerValue(sb.ToString());
            if (!string.IsNullOrEmpty(sQtyProduced))
                QtyProduced = Convert.ToDouble(sQtyProduced);

            //Commited to a Order
            sb = new StringBuilder();
            sb.Append("	Select IsNull(sum(openqty * NumPerMsr), 0) \r\n");
            sb.Append("	from rdr1 \r\n");
            sb.Append("	inner join ordr on ordr.docentry = rdr1.docentry \r\n");
            sb.AppendFormat("	where rdr1.itemcode = '{0}' \r\n", itemCode);
            sb.AppendFormat("	and  ordr.docdate = '{0}' \r\n", dtdel.ToString("yyy-MM-dd"));
            double QtyOnOrder = 0;
            string sQtyOnOrder = Program.objHrmsUI.getScallerValue(sb.ToString());
            if (!string.IsNullOrEmpty(sQtyOnOrder))
                QtyOnOrder = Convert.ToDouble(sQtyOnOrder);

            lbvQtyToProduce.Caption = Convert.ToString(QtyToProduce);
            lbvQtyStock.Caption = Convert.ToString(totalStockQty + QtyProduced);
            lbvQtyOnOrder.Caption = Convert.ToString(QtyOnOrder);
            lbvQtyATM.Caption = Convert.ToString(QtyToProduce + totalStockQty + QtyProduced - QtyOnOrder);

        }



        private void fillCustMSP(string cardCode)
        {
            dtMSP.Rows.Clear();
            string strSelect = "";




            strSelect = @"select top " + drSetting["U_NMSI"].ToString() + @"  rdr1.itemcode,oitm.ItemName, sum(rdr1.quantity) as Quantity

                    from  rdr1 inner join oitm on oitm.itemcode = rdr1.itemcode inner join ordr on ordr.docentry = rdr1.docentry

                    where ordr.CardCode = '" + cardCode + @"'
                    group by rdr1.itemcode,oitm.ItemName
                    order by sum(quantity) desc

                          ";



            System.Data.DataTable dtProducts = Program.objHrmsUI.getDataTable(strSelect, "Filling Product");
            int j = 0;
            foreach (System.Data.DataRow dr in dtProducts.Rows)
            {
                dtMSP.Rows.Add(1);
                dtMSP.SetValue("Id", j, (j + 1).ToString());
                dtMSP.SetValue("ItemCode", j, dr["ItemCode"].ToString());
                dtMSP.SetValue("ItemName", j, dr["ItemName"].ToString());
                dtMSP.SetValue("Quantity", j, dr["Quantity"].ToString());




                j++;
            }
            mtMSP.LoadFromDataSource();



        }


        private void fillCRDRP(string cardCode)
        {
            dtCRDRP.Rows.Clear();
            string strSelect = "";




            strSelect = @"select top " + drSetting["U_NRP"].ToString() + @" rdr1.itemcode,oitm.itemname, max(rdr1.docdate)   as LOD , max(rdr1.Quantity) Quantity from rdr1 inner join oitm on oitm.itemcode = rdr1.ItemCode inner join 
                            ordr on ordr.docentry = rdr1.docentry
                            where ordr.cardcode = '" + cardCode + @"'
							group by rdr1.itemcode,oitm.itemname
							order by max(rdr1.docdate) desc
                          ";



            System.Data.DataTable dtProducts = Program.objHrmsUI.getDataTable(strSelect, "Filling Recent Products");
            int j = 0;
            foreach (System.Data.DataRow dr in dtProducts.Rows)
            {
                dtCRDRP.Rows.Add(1);
                dtCRDRP.SetValue("Id", j, (j + 1).ToString());
                dtCRDRP.SetValue("ItemCode", j, dr["ItemCode"].ToString());
                dtCRDRP.SetValue("ItemName", j, dr["ItemName"].ToString());
                dtCRDRP.SetValue("LOQ", j, dr["Quantity"].ToString());
                dtCRDRP.SetValue("LOD", j, Convert.ToDateTime(dr["LOD"].ToString()));




                j++;
            }
            mtCRDRP.LoadFromDataSource();



        }

        private void fillTOR()
        {
            dtTodSO.Rows.Clear();
            string strSelect = "";




            strSelect = @"select CardCOde,CardName , isnull(tblcall.U_Route,'') as U_Route ,ordr.DocDueDate,ordr.DocTotal,ordr.DocNum,ordr.DocStatus,ordr.docentry
                            from ordr left outer join [@B1_SCHCALL] tblCall on tblCall.Code = ordr.U_SchID
                            where docdate =  convert(date, '" + Convert.ToDateTime(dtHead.GetValue("CDate", 0)).ToString("yyyyMMdd") + @"',101)
                          ";



            System.Data.DataTable dtTodOrders = Program.objHrmsUI.getDataTable(strSelect, "Filling Todays Order");
            int j = 0;
            foreach (System.Data.DataRow dr in dtTodOrders.Rows)
            {
                string strStatus = "Open";
                switch (dr["DocStatus"].ToString())
                {
                    case "O":
                        strStatus = "Open";
                        break;
                    case "C":
                        strStatus = "Close";
                        break;
                }
                dtTodSO.Rows.Add(1);
                dtTodSO.SetValue("Id", j, (j + 1).ToString());
                dtTodSO.SetValue("CardCode", j, dr["CardCode"].ToString());
                dtTodSO.SetValue("CardName", j, dr["CardName"].ToString());
                dtTodSO.SetValue("Route", j, dr["U_Route"].ToString());
                dtTodSO.SetValue("delDate", j, Convert.ToDateTime(dr["DocDueDate"].ToString()));
                dtTodSO.SetValue("DocTotal", j, dr["DocTotal"].ToString());
                dtTodSO.SetValue("DocNum", j, dr["DocNum"].ToString());
                dtTodSO.SetValue("DocEntry", j, dr["DocEntry"].ToString());
                dtTodSO.SetValue("Status", j, strStatus);




                j++;
            }
            mtTOR.LoadFromDataSource();

            tbOpr3.Caption = "Todays Orders (" + (j).ToString() + ")";


        }




        private int mtSelRow(SAPbouiCOM.Matrix mt)
        {
            int selectedrow = 0;

            for (int i = 1; i <= mt.RowCount; i++)
            {
                if (mt.IsRowSelected(i))
                {
                    selectedrow = i;
                    return i;
                }
            }
            return selectedrow;

        }





        private void addEmptyRow(SAPbouiCOM.Matrix mt, SAPbouiCOM.DataTable dt, string firstCol)
        {


            if (dt.Rows.Count == 0)
            {
                dt.Rows.Add(2);
                dt.Rows.Remove(1);
                dt.SetValue("Id", 0, "1");
                mt.AddRow(1, mt.RowCount + 1);
            }
            else
            {
                if (dt.GetValue(firstCol, dt.Rows.Count - 1) == "")
                {
                }
                else
                {
                    dt.Rows.Add(1);

                    dt.SetValue("Id", dt.Rows.Count - 1, dt.Rows.Count.ToString());
                    dt.SetValue(firstCol, dt.Rows.Count - 1, "");
                    mt.AddRow(1, mt.RowCount + 1);

                }

            }
            mt.LoadFromDataSource();

        }



        private void setTotal(int rowIndex, string ColId)
        {

            mtORI.FlushToDataSource();
            double price = Convert.ToDouble(dtRDR1.GetValue("Price", rowIndex - 1));
            double quantity = Convert.ToDouble(dtRDR1.GetValue("Quantity", rowIndex - 1));
            double discount = Convert.ToDouble(dtRDR1.GetValue("Discount", rowIndex - 1));
            double docGross = 0.00;
            double lineTotal = (price - price * discount / 100) * quantity;
            dtRDR1.SetValue("LineTotal", rowIndex - 1, lineTotal.ToString());


            mtORI.LoadFromDataSource();

            setDocTotals();

            if (ColId == "Qty")
            {
                mtORI.SetCellFocus(rowIndex, 4);
            }
            else if (ColId == "Price")
            {
                mtORI.SetCellFocus(rowIndex, 3);
            }
            else if (ColId == "Disc")
            {
                mtORI.SetCellFocus(rowIndex + 1, 0);
            }
            else if (ColId == "ItemCode")
            {

            }
            else
            {
                mtORI.SetCellFocus(rowIndex, 5);
            }

        }

        private SAPbobsCOM.ItemPriceReturnParams getUnitPriceSys(string itemCode, string CardCode, double quantity, DateTime pdate)
        {
            double price = 0;

            SAPbobsCOM.ItemPriceParams priceParam;
            SAPbobsCOM.ItemPriceReturnParams ItemPrice;
            SAPbobsCOM.CompanyService cmpSvc = oCompany.GetCompanyService();
            priceParam = (SAPbobsCOM.ItemPriceParams)cmpSvc.GetDataInterface(SAPbobsCOM.CompanyServiceDataInterfaces.csdiItemPriceParams);
            priceParam.CardCode = CardCode;
            priceParam.ItemCode = itemCode;
            priceParam.UoMQuantity = quantity;
            //  priceParam.InventoryQuantity = quantity;
            priceParam.Date = pdate;

            ItemPrice = (SAPbobsCOM.ItemPriceReturnParams)cmpSvc.GetDataInterface(SAPbobsCOM.CompanyServiceDataInterfaces.csdiItemPriceReturnParams);
            ItemPrice = cmpSvc.GetItemPrice(priceParam);





            return ItemPrice;

        }


        private double getUnitPrice(string itemCode, string PriceList)
        {

            double price = 0;

            price = Convert.ToDouble(Program.objHrmsUI.getScallerValue("Select price from itm1 where itemcode = '" + itemCode + "' and pricelist='" + PriceList + "'"));

            return price;

        }

        private string getCustSpecs(string itemCode, string cardCode)
        {

            string outResult = "";

            outResult = Convert.ToString(Program.objHrmsUI.getScallerValue("select ISNULL(oscn.U_CutSpec,'') as CustSpecs  from oscn where itemcode = '" + itemCode + "' and cardcode = '" + cardCode + "'"));

            return outResult;

        }


        private void doPost()
        {
            string cardCode = Convert.ToString(dtHead.GetValue("CardCode", 0));
            if (cardCode != "")
            {
                postOrder(cardCode, callId);
            }
            else
            {
                oApplication.MessageBox("Select a customer/Call/Standing order to add an order");
            }

        }
        private void postOrder(string cardCode, string callID)
        {

            string outStr = "";
            SAPbobsCOM.Documents Doc = (SAPbobsCOM.Documents)Program.objHrmsUI.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
            SAPbobsCOM.BusinessPartners bp = (SAPbobsCOM.BusinessPartners)Program.objHrmsUI.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);

            string strDate = Convert.ToString(dtHead.GetValue("dDate", 0));

            DateTime dtdel = Convert.ToDateTime(strDate);

            Doc.CardCode = cardCode;
            Doc.DocDate = DateTime.Now;
            Doc.DocDueDate = dtdel.Date;

            Doc.UserFields.Fields.Item("U_SchID").Value = callID;

            Doc.Comments = txRem.Value.ToString();
            Doc.NumAtCard = txNAC.Value.ToString();
            for (int i = 0; i < dtRDR1.Rows.Count; i++)
            {
                string itemCode = Convert.ToString(dtRDR1.GetValue("ItemCode", i));
                if (itemCode != "" && Convert.ToDouble(dtRDR1.GetValue("Quantity", i)) > 0)
                {
                    Doc.Lines.ItemCode = Convert.ToString(dtRDR1.GetValue("ItemCode", i));
                    Doc.Lines.Quantity = Convert.ToDouble(dtRDR1.GetValue("Quantity", i));
                    if (Doc.Lines.Quantity == 0) Doc.Lines.Quantity = 0.01;

                    Doc.Lines.UnitPrice = Convert.ToDouble(dtRDR1.GetValue("Price", i));
                    Doc.Lines.DiscountPercent = Convert.ToDouble(dtRDR1.GetValue("Discount", i));
                    Doc.Lines.FreeText = Convert.ToString(dtRDR1.GetValue("Freetxt", i));
                    Doc.Lines.Add();
                }
            }

            if (Doc.Add() != 0)
            {
                int erroCode = 0;
                string errDescr = "";
                Program.objHrmsUI.oCompany.GetLastError(out erroCode, out errDescr);
                outStr = "Error:" + errDescr + outStr;
                oApplication.StatusBar.SetText("Failed to add Order  : " + errDescr);
            }
            else
            {
                outStr = Convert.ToString(Program.objHrmsUI.oCompany.GetNewObjectKey());

                Doc.GetByKey(Convert.ToInt32(outStr));
                string updateCall = "Update [@B1_SCHCALL] set U_Status='Order Added',  u_Docnum='" + Doc.DocEntry.ToString() + "', u_DocEntry='" + Doc.DocNum.ToString() + "' where code='" + callID + "'";
                int result = Program.objHrmsUI.ExecQuery(updateCall, "Update Call Record");

                oApplication.SetStatusBarMessage("Order Added Successfully", SAPbouiCOM.BoMessageTime.bmt_Short, false);
                fillTeleSale(Convert.ToDateTime(dtHead.GetValue("CDate", 0)));
                clearItems();
                fillTOR();
                fillTelesaleAnalysis();
            }

        }
        private void addItemInRDR(string itemCode, string itemName, double qty, double discount)
        {
            oForm.Freeze(true);

            addEmptyRow(mtORI, dtRDR1, "ItemCode");
            int rowind = dtRDR1.Rows.Count - 1;
            double linetotal = 0.00;
            double unitPrice = 0.00;
            dtRDR1.SetValue("ItemCode", rowind, itemCode);
            dtRDR1.SetValue("ItemName", rowind, itemName);
            dtRDR1.SetValue("Quantity", rowind, qty);
            string freeText = getCustSpecs(itemCode, Convert.ToString(dtHead.GetValue("CardCode", 0)));
            dtRDR1.SetValue("Freetxt", rowind, freeText);


            //  unitPrice = getUnitPrice(itemCode, Convert.ToString(dtHead.GetValue("PLN", 0)));
            SAPbobsCOM.ItemPriceReturnParams itemPrice = getUnitPriceSys(itemCode, Convert.ToString(dtHead.GetValue("CardCode", 0)), qty, Convert.ToDateTime(dtHead.GetValue("CDate", 0)));

            unitPrice = itemPrice.Price;

            dtRDR1.SetValue("Price", rowind, unitPrice);
            dtRDR1.SetValue("Discount", rowind, itemPrice.Discount);
            discount = itemPrice.Discount;
            dtRDR1.SetValue("LineTotal", rowind, (unitPrice - (unitPrice * discount / 100)) * qty);




            mtORI.LoadFromDataSource();

            setDocTotals();

            addEmptyRow(mtORI, dtRDR1, "ItemCode");
            oForm.Freeze(false);

        }

        private void setDocTotals()
        {
            double docGross = 0.00;


            for (int i = 0; i < dtRDR1.Rows.Count; i++)
            {
                docGross += Convert.ToDouble(dtRDR1.GetValue("LineTotal", i));

            }
            dtHead.SetValue("grosTot", 0, docGross.ToString());
            dtHead.SetValue("DocTotal", 0, docGross.ToString());


        }
        private void clearItems()
        {
            dtRDR1.Rows.Clear();
            mtORI.LoadFromDataSource();
            addEmptyRow(mtORI, dtRDR1, "ItemCode");
            txRem.Value = "";
            txNAC.Value = "";
            dtHead.SetValue("grosTot", 0, "0.00");
            dtHead.SetValue("DocTotal", 0, "0.00");

            lbvQtyToProduce.Caption = "";
            lbvQtyStock.Caption = "";
            lbvQtyOnOrder.Caption = "";
            lbvQtyATM.Caption = "";

        }

        private void selectAllSlp()
        {
            try
            {

                //    oForm.Freeze(true);
                SAPbouiCOM.Column col = mtSP.Columns.Item("sel");

                if (col.TitleObject.Caption == "✓")
                {
                    for (int i = 0; i < dtSP.Rows.Count; i++)
                    {

                        dtSP.SetValue("sel", i, "N");
                        col.TitleObject.Caption = "";
                    }
                }
                else
                {
                    for (int i = 0; i < dtSP.Rows.Count; i++)
                    {
                        dtSP.SetValue("sel", i, "Y");
                        col.TitleObject.Caption = "✓";
                    }
                }
                mtSP.LoadFromDataSource();
                //      oForm.Freeze(false);
            }
            catch (Exception ex)
            {
                oApplication.SetStatusBarMessage("Error occured " + ex.Message);
                // iniSalaryDetail();
            }
        }
        private void selectAllRoute()
        {
            try
            {

                // oForm.Freeze(true);
                SAPbouiCOM.Column col = mtRoute.Columns.Item("sel");

                if (col.TitleObject.Caption == "✓")
                {
                    for (int i = 0; i < dtRoute.Rows.Count; i++)
                    {

                        dtRoute.SetValue("sel", i, "N");
                        col.TitleObject.Caption = "";
                    }
                }
                else
                {
                    for (int i = 0; i < dtRoute.Rows.Count; i++)
                    {
                        dtRoute.SetValue("sel", i, "Y");
                        col.TitleObject.Caption = "✓";
                    }
                }
                mtRoute.LoadFromDataSource();
                //  oForm.Freeze(false);
            }
            catch (Exception ex)
            {
                oApplication.SetStatusBarMessage("Error occured " + ex.Message);
                // iniSalaryDetail();
            }
        }
        private void selectAllGroup()
        {
            try
            {

                //  oForm.Freeze(true);
                SAPbouiCOM.Column col = mtGroup.Columns.Item("sel");

                if (col.TitleObject.Caption == "✓")
                {
                    for (int i = 0; i < dtGroup.Rows.Count; i++)
                    {

                        dtGroup.SetValue("sel", i, "N");
                        col.TitleObject.Caption = "";
                    }
                }
                else
                {
                    for (int i = 0; i < dtGroup.Rows.Count; i++)
                    {
                        dtGroup.SetValue("sel", i, "Y");
                        col.TitleObject.Caption = "✓";
                    }
                }
                mtGroup.LoadFromDataSource();
                //    oForm.Freeze(false);
            }
            catch (Exception ex)
            {
                oApplication.SetStatusBarMessage("Error occured " + ex.Message);
                // iniSalaryDetail();
            }
        }
        private void selectAllCust()
        {
            try
            {

                // oForm.Freeze(true);
                SAPbouiCOM.Column col = mtCust.Columns.Item("sel");

                if (col.TitleObject.Caption == "✓")
                {
                    for (int i = 0; i < dtCust.Rows.Count; i++)
                    {

                        dtCust.SetValue("sel", i, "N");
                        col.TitleObject.Caption = "";
                    }
                }
                else
                {
                    for (int i = 0; i < dtCust.Rows.Count; i++)
                    {
                        dtCust.SetValue("sel", i, "Y");
                        col.TitleObject.Caption = "✓";
                    }
                }
                mtCust.LoadFromDataSource();
                //    oForm.Freeze(false);
            }
            catch (Exception ex)
            {
                oApplication.SetStatusBarMessage("Error occured " + ex.Message);
                // iniSalaryDetail();
            }
        }
        private void showSaveOptions(string itemCode, double newPrice, string BP, string priceList)
        {
            System.Data.DataTable dtOptions = new System.Data.DataTable();
            dtOptions.Columns.Add("optId");
            dtOptions.Columns.Add("OptText");

            dtOptions.Rows.Add("opt01", "Update Price List for that BP");
            dtOptions.Rows.Add("opt02", "Update Special Price Lis for that BP");
            dtOptions.Rows.Add("opt03", "Update Base Brice List for all BP");
            dtOptions.Rows.Add("opt04", "Cancel");
            optPicker pic = new optPicker(oApplication, dtOptions);
            System.Data.DataTable st = pic.ShowInput("Change Price", "Select an option to save price");
            pic = null;
            if (st.Rows.Count > 0)
            {

                string selectedOption = st.Rows[0][0].ToString();
                switch (selectedOption)
                {
                    case "opt01":
                        updatePriceList(itemCode, priceList, newPrice);
                        break;
                    case "opt02":
                        updateSpecialPrice(itemCode, BP, newPrice, priceList);
                        break;
                    case "opt03":
                        updateBasePriceList(itemCode, priceList, newPrice);
                        break;
                    case "opt04":
                        break;
                }


            }
        }
        private void updatePriceList(string itemCode, string PriceList, double price)
        {
            SAPbobsCOM.Items oitm = (SAPbobsCOM.Items)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems);
            oitm.GetByKey(itemCode);
            for (int i = 0; i < oitm.PriceList.Count; i++)
            {
                oitm.PriceList.SetCurrentLine(i);
                if (oitm.PriceList.PriceList == Convert.ToInt32(PriceList))
                {
                    oitm.PriceList.Price = price;
                  
                }
            }
            int result = oitm.Update();
            if (result != 0)
            {
                int v_ErrCode = 0;
                string v_ErrMsg = "";
                oCompany.GetLastError(out v_ErrCode, out v_ErrMsg);
                string err = v_ErrMsg;
                oApplication.SetStatusBarMessage(v_ErrMsg);

            }
            else
            {

            }
        }
        private void updateBasePriceList(string itemCode, string PriceList, double price)
        {

            string strBasePriceList = Convert.ToString( Program.objHrmsUI.getScallerValue("SELECT T0.[BASE_NUM] FROM OPLN T0 WHERE T0.[ListNum] ='" + PriceList + "' "));
            SAPbobsCOM.Items oitm = (SAPbobsCOM.Items)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems);
            oitm.GetByKey(itemCode);
            for (int i = 0; i < oitm.PriceList.Count; i++)
            {
                oitm.PriceList.SetCurrentLine(i);
                if (oitm.PriceList.PriceList == Convert.ToInt32(strBasePriceList))
                {
                    oitm.PriceList.Price = price;
                }
            }
           int result =  oitm.Update();
            
      
        }

        private void updateSpecialPrice(string itemcode, string bp, double price,string priceList)
        {

            SAPbobsCOM.SpecialPrices spprice = (SAPbobsCOM.SpecialPrices)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oSpecialPrices);
            bool exist = spprice.GetByKey(itemcode, bp);

            if (exist)
            {

                int remresult = spprice.Remove();
            }
            spprice = (SAPbobsCOM.SpecialPrices)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oSpecialPrices); 
      
            string oldPrice = spprice.PriceListNum.ToString();
            spprice.PriceListNum = Convert.ToInt32(priceList);
            spprice.CardCode = bp;
            spprice.ItemCode = itemcode;
            spprice.Price = price;

            {
                spprice.AutoUpdate = SAPbobsCOM. BoYesNoEnum.tYES;
                int result = spprice.Add();

                if (result != 0)
                {
                    int v_ErrCode = 0;
                    string v_ErrMsg = "";
                    oCompany.GetLastError(out v_ErrCode, out v_ErrMsg);
                    string err = v_ErrMsg;
                    oApplication.SetStatusBarMessage(v_ErrMsg);

                }
                else
                {

                }


            }



        }

        private void updateUOMPrice(string PriceList ,  string itemCode,double price,int uomEntry,double reducedby)
        {



          
          
            System.Data.DataTable dt = new System.Data.DataTable();
            string filePath = @"D:\Clients\Adiles\ImportFile.txt";
            using (StreamReader file = new StreamReader(filePath))
            {
                string line = "";
                string[] pastrts;

                line = file.ReadLine();
                pastrts = line.Split('\t');
                foreach (string colName in pastrts)
                {
                    dt.Columns.Add(colName);
                }
                line = file.ReadLine();
                line = file.ReadLine();
                string oldPriceList = "0";
                int PriceRowCnt = 0;

                while ("a" == "a")
                {
                    line = file.ReadLine();
                    if (line == null) break;
                    pastrts = line.Split('\t');
                    string priceList = pastrts[0].ToString();
                    if (priceList == "") continue;
                    dt.Rows.Add(pastrts);
               }
       
            }

            foreach (System.Data.DataRow dr in dt.Rows)
            {

            //  Convert.ToInt32(  dr["ParentKey"]).ToString()
                SAPbobsCOM.Items oitm = (SAPbobsCOM.Items)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems);
                oitm.GetByKey(itemCode);
                oApplication.MessageBox("Updating Item " + itemCode);
                bool uomFound = false;
              
                int uomCnt = 0;
                int currentRow = 0;
                for (int i = 0; i < oitm.PriceList.Count; i++)
                {
                    currentRow++;
                    if(currentRow>50)
                    {
                        currentRow=0;
                        oApplication.MessageBox("Updating " + itemCode ,1,"Yes");
                    }
                    oitm.PriceList.SetCurrentLine(i);
                    if (oitm.PriceList.PriceList == Convert.ToInt32(PriceList))
                    {
                        for (int k = 0; k < oitm.PriceList.UoMPrices.Count; k++)
                        {
                            uomCnt++;
                            oitm.PriceList.UoMPrices.SetCurrentLine(k);
                            if (oitm.PriceList.UoMPrices.UoMEntry == Convert.ToInt32(uomEntry))
                            {
                                oitm.PriceList.UoMPrices.ReduceBy = reducedby;
                                oitm.PriceList.UoMPrices.Auto = SAPbobsCOM.BoYesNoEnum.tYES;

                                uomFound = true;

                            }


                        }
                        if (!uomFound)
                        {
                            oitm.PriceList.UoMPrices.Add();
                            oitm.PriceList.UoMPrices.SetCurrentLine(uomCnt);
                            oitm.PriceList.UoMPrices.UoMEntry = uomEntry;
                            oitm.PriceList.UoMPrices.ReduceBy = reducedby;
                            oitm.PriceList.UoMPrices.Auto = SAPbobsCOM.BoYesNoEnum.tYES;

                            

                        }
                    }
                }
                int result = oitm.Update();
                if (result != 0)
                {
                    int v_ErrCode = 0;
                    string v_ErrMsg = "";
                    oCompany.GetLastError(out v_ErrCode, out v_ErrMsg);
                    string err = v_ErrMsg;
                    oApplication.SetStatusBarMessage(v_ErrMsg);

                }
                else
                {

                }
            }
        }
    }
}
