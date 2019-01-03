using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ABCommerce.ABCModals;
using SAPbobsCOM;
//using Mag =  ACHR.com.thefruitcompany.www;

using SAPbouiCOM;

namespace ACHR.Screen
{
    class frm_WO : HRMSBaseForm
    {

      
        public bool isForLoading = false;
        SAPbouiCOM.DataTable dtST, dtIt;
        SAPbouiCOM.Matrix mtWO, mtOD;
        SAPbouiCOM.ComboBox cbOS, cbBsUnt;
        SAPbouiCOM.EditText txFrom, txTo, txWO, txWODT, txTotO, txTotSel, txCR, txOS, txNC, tx4D, txAutID, txCCAmt, txCCTD, txCCED, txCCBZ, txDC, txWD, txWDA, txMMI, txDPM, txOP, txCCode, txCTel, txCEmail, txCNCode;
        SAPbouiCOM.EditText txCNTel, txCNEmail, txSName, txSSPO, txSCity, txSState, txSZip, txSPhone, txSEmail, txSSComp;
        SAPbouiCOM.Button btPost, btVoid, btGet;
        SAPbouiCOM.StaticText lblStatus;
        int rowNum = 0;
        public string rootGroup;
        public string rootGroupName;

        SAPbouiCOM.DataTable dtWebO, dtCard, dtHead, dtORDR, dtRDR1;
        string itemClicked = "";
        string selOrderNum = "";

        private bool initiallizing = false;
        string printMenuId = "";
        public override void CreateForm(SAPbouiCOM.Application SboApp, string strXml, SAPbobsCOM.Company cmp, string frmId)
        {
            base.CreateForm(SboApp, strXml, cmp, frmId);

            oForm.EnableMenu("1282", false);  // Add New Record
            oForm.EnableMenu("1288", false);  // Next Record
            oForm.EnableMenu("1289", false);  // Pevious Record
            oForm.EnableMenu("1290", false);  // First Record
            oForm.EnableMenu("1291", false);  // Last record 
            oForm.EnableMenu("1281", false);  // Find record 
            oForm.Settings.MatrixUID = "mtOD";
            oForm.Settings.Enabled = true;
            InitiallizeForm();




        }
        public override void etAfterKeyDown(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterKeyDown(ref pVal, ref BubbleEvent);


        }
        public override void etAfterCmbSelect(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCmbSelect(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == cbOS.Item.UniqueID && !initiallizing )
            {
                FillOrderMatrix();
            }
        }
        public override void etAfterClick(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);

            if (pVal.ItemUID ==mtWO.Item.UniqueID)
            {


                if (pVal.ColUID == "V_3" && pVal.Row == 0)
                {
                    selectAllWO();
                }
            }

            if (pVal.ItemUID == btGet.Item.UniqueID)
            {
                if (txFrom.Value.ToString() == "")
                    return;
                if (txTo.Value.ToString() == "")
                    return;
                getOrders();
            }

            if (pVal.ItemUID =="btST")
            {
               // addShipment( Convert.ToString( dtORDR.GetValue("WBID", 0)));
            }


            if (pVal.ItemUID == mtWO.Item.UniqueID && pVal.ColUID=="V_-1" &&  pVal.Row > 0 && pVal.Row <= mtWO.RowCount)
            {
                string incrementId = Convert.ToString(dtWebO.GetValue("WebNum", pVal.Row - 1));
                addWebOrder(incrementId);
            }

            if (pVal.ItemUID == "btPost")
            {
                int confirm = oApplication.MessageBox("Are you sure you want to Post Order To SBO", 2, "Yes", "No");
                if (confirm != 1) return;
                ProcessPosting();
                //postDocument();
            }

            if (pVal.ItemUID == "btDel")
            {
                int confirm = oApplication.MessageBox("Are you sure you want to update all delivered order on web?", 2, "Yes", "No");
                if (confirm != 1) return;
                updateShipped();
            }
            if (pVal.ItemUID == "btCmp")
            {
                int confirm = oApplication.MessageBox("Are you sure you want to update all completed order on web?", 2, "Yes", "No");
                if (confirm != 1) return;
                updateInvoiced();
            }
            if (pVal.ItemUID == "btUS")
            {
                int confirm = oApplication.MessageBox("Are you sure you want to update stock level web?", 2, "Yes", "No");
                if (confirm != 1) return;
                updateStock();
            }

            
        }


        private bool saveOrder(DataExtractOrder weborder)
        {
            bool outresult=false;


            DateTime shipDate = DateTime.Now.AddDays(1);
            DateTime DeliveryDate = DateTime.Now.AddDays(1);

            string WebID = weborder.OrderID;
            DateTime utcDate = Convert.ToDateTime(weborder.Date);
            utcDate = DateTime.SpecifyKind(utcDate, DateTimeKind.Utc);
            DateTime localDT = utcDate.ToLocalTime();

            string OrderDate = localDT.ToString();

          

            
            string strInsert = @"INSERT INTO [dbo].[@B1_ABC_ORDR]
           ([Code]
           ,[Name]
           ,[U_OrderID]
           ,[U_OrdNo]
           ,[U_PONo]
           ,[U_StatID]
           ,[U_StatCode]
           ,[U_Status]
           ,[U_Date]
           ,[U_CurrCode]
           ,[U_Curr]
           ,[U_NoItems]
           ,[U_ItemsTotal]
           ,[U_Total]
           ,[U_AmtPaid]
           ,[U_Shipping]
           ,[U_ShippingFee]
           ,[U_Discount]
           ,[U_Email]
           ,[U_DelType]
           ,[U_OutletCode]
           ,[U_Outlet]
           ,[U_OutletID]
           ,[U_ShipFirstName]
           ,[U_ShipLastName]
           ,[U_ShipCompany]
           ,[U_ShipAdd1]
           ,[U_ShipAdd2]
           ,[U_ShipAdd3]
           ,[U_ShipAdd4]
           ,[U_ShipPostCode]
           ,[U_ShipRegion]
           ,[U_ShipRegionID]
           ,[U_ShipMobile]
           ,[U_ShipPhoneDay]
           ,[U_ShipPhoneEve]
           ,[U_ShipEmail]
           ,[U_BillFirstName]
           ,[U_BillLastName]
           ,[U_BillCompany]
           ,[U_BillAdd1]
           ,[U_BillAdd2]
           ,[U_BillAdd3]
           ,[U_BillAdd4]
           ,[U_BillPostCode]
           ,[U_BillRegion]
           ,[U_BillMobile]
           ,[U_BillPhoneDay]
           ,[U_BillPhoneEve]
           ,[U_PaymentTypeID]
           ,[U_PaymentType]
           ,[U_TrackRef]
           ,[U_StatusMsg]
           ,[U_GiftMsg]
           ,[U_ContID]
           ,[U_CustCode]
           ,[U_CustCodeNo]
           ,[U_Company]
           ,[U_ContType]
           ,[U_IsSpecial]
           ,[U_PrimCustCode]
           ,[U_PrimCustCodeNo]
           ,[U_PrimContID]
           ,[U_SUFirstName]
           ,[U_SULastName]
           ,[U_SUContID]
           ,[U_Comments]
           ,[U_PayProvRef]
           )
     VALUES
           (@Code!
           ,@Name!
           ,@U_OrderID!
           ,@U_OrdNo!
           ,@U_PONo!
           ,@U_StatID!
           ,@U_StatCode!
           ,@U_Status!
           ,@U_Date!
           ,@U_CurrCode!
           ,@U_Curr!
           ,@U_NoItems!
           ,@U_ItemsTotal!
           ,@U_Total!
           ,@U_AmtPaid!
           ,@U_Shipping!
           ,@U_ShippingFee!
           ,@U_Discount!
           ,@U_Email!
           ,@U_DelType!
           ,@U_OutletCode!
           ,@U_Outlet!
           ,@U_OutletID!
           ,@U_ShipFirstName!
           ,@U_ShipLastName!
           ,@U_ShipCompany!
           ,@U_ShipAdd1!
           ,@U_ShipAdd2!
           ,@U_ShipAdd3!
           ,@U_ShipAdd4!
           ,@U_ShipPostCode!
           ,@U_ShipRegion!
           ,@U_ShipRegionID!
           ,@U_ShipMobile!
           ,@U_ShipPhoneDay!
           ,@U_ShipPhoneEve!
           ,@U_ShipEmail!
           ,@U_BillFirstName!
           ,@U_BillLastName!
           ,@U_BillCompany!
           ,@U_BillAdd1!
           ,@U_BillAdd2!
           ,@U_BillAdd3!
           ,@U_BillAdd4!
           ,@U_BillPostCode!
           ,@U_BillRegion!
           ,@U_BillMobile!
           ,@U_BillPhoneDay!
           ,@U_BillPhoneEve!
           ,@U_PaymentTypeID!
           ,@U_PaymentType!
           ,@U_TrackRef!
           ,@U_StatusMsg!
           ,@U_GiftMsg!
           ,@U_ContID!
           ,@U_CustCode!
           ,@U_CustCodeNo!
           ,@U_Company!
           ,@U_ContType!
           ,@U_IsSpecial!
           ,@U_PrimCustCode!
           ,@U_PrimCustCodeNo!
           ,@U_PrimContID!
           ,@U_SUFirstName!
           ,@U_SULastName!
           ,@U_SUContID!
           ,@U_Comments!
           ,@U_PayProvRef!
            )";

            Hashtable hsp = new Hashtable();
            hsp.Add("@Code!", weborder.OrderID);
            hsp.Add("@Name!", weborder.OrderID);
            hsp.Add("@U_OrderID!", weborder.OrderID);
            hsp.Add("@U_OrdNo!", weborder.OrdNo);
            hsp.Add("@U_PONo!", weborder.PONo);
            hsp.Add("@U_StatID!", weborder.StatID);
            hsp.Add("@U_StatCode!", weborder.StatCode);
            hsp.Add("@U_Status!", weborder.Status);
            hsp.Add("@U_Date!", weborder.Date);
            hsp.Add("@U_CurrCode!", weborder.CurrCode);
            hsp.Add("@U_Curr!", weborder.Curr);
            hsp.Add("@U_NoItems!", weborder.NoItems);
            hsp.Add("@U_ItemsTotal!", weborder.ItemsTotal);
            hsp.Add("@U_Total!", weborder.Total);
            hsp.Add("@U_AmtPaid!", weborder.AmtPaid);
            hsp.Add("@U_Shipping!", weborder.Shipping);
            hsp.Add("@U_ShippingFee!", weborder.ShippingFee);
            hsp.Add("@U_Discount!", weborder.Discount);
            hsp.Add("@U_Email!", weborder.Email);
            hsp.Add("@U_DelType!", weborder.DelType);
            hsp.Add("@U_OutletCode!", weborder.OutletCode);
            hsp.Add("@U_Outlet!", weborder.Outlet);
            hsp.Add("@U_OutletID!", weborder.OutletID);
            hsp.Add("@U_ShipFirstName!", weborder.ShipFirstName);
            hsp.Add("@U_ShipLastName!", weborder.ShipLastName);
            hsp.Add("@U_ShipCompany!", weborder.ShipCompany);
            hsp.Add("@U_ShipAdd1!", weborder.ShipAdd1);
            hsp.Add("@U_ShipAdd2!", weborder.ShipAdd2);
            hsp.Add("@U_ShipAdd3!", weborder.ShipAdd3);
            hsp.Add("@U_ShipAdd4!", weborder.ShipAdd4);
            hsp.Add("@U_ShipPostCode!", weborder.ShipPostCode);
            hsp.Add("@U_ShipRegion!", weborder.ShipRegion);
            hsp.Add("@U_ShipRegionID!", weborder.ShipRegionID);
            hsp.Add("@U_ShipMobile!", weborder.ShipMobile);
            hsp.Add("@U_ShipPhoneDay!", weborder.ShipPhoneDay);
            hsp.Add("@U_ShipPhoneEve!", weborder.ShipPhoneEve);
            hsp.Add("@U_ShipEmail!", weborder.ShipEmail);
            hsp.Add("@U_BillFirstName!", weborder.BillFirstName);
            hsp.Add("@U_BillLastName!", weborder.BillLastName);
            hsp.Add("@U_BillCompany!", weborder.BillCompany);
            hsp.Add("@U_BillAdd1!", weborder.BillAdd1);
            hsp.Add("@U_BillAdd2!", weborder.BillAdd2);
            hsp.Add("@U_BillAdd3!", weborder.BillAdd3);
            hsp.Add("@U_BillAdd4!", weborder.BillAdd4);
            hsp.Add("@U_BillPostCode!", weborder.BillPostCode);
            hsp.Add("@U_BillRegion!", weborder.BillRegion);
            hsp.Add("@U_BillMobile!", weborder.BillMobile);
            hsp.Add("@U_BillPhoneDay!", weborder.BillPhoneDay);
            hsp.Add("@U_BillPhoneEve!", weborder.BillPhoneEve);
            hsp.Add("@U_PaymentTypeID!", weborder.PaymentTypeID);
            hsp.Add("@U_PaymentType!", weborder.PaymentType);
            hsp.Add("@U_TrackRef!", weborder.TrackRef);
            hsp.Add("@U_StatusMsg!", weborder.StatusMsg);
            hsp.Add("@U_GiftMsg!", weborder.GiftMsg);
            hsp.Add("@U_ContID!", weborder.ContID);
            hsp.Add("@U_CustCode!", weborder.CustCode);
            hsp.Add("@U_CustCodeNo!", weborder.CustCodeNo);
            hsp.Add("@U_Company!", weborder.Company);
            hsp.Add("@U_ContType!", weborder.ContType);
            hsp.Add("@U_IsSpecial!", weborder.IsSpecial);
            hsp.Add("@U_PrimCustCode!", weborder.PrimCustCode);
            hsp.Add("@U_PrimCustCodeNo!", weborder.PrimCustCodeNo);
            hsp.Add("@U_PrimContID!", weborder.PrimContID);
            hsp.Add("@U_SUFirstName!", weborder.SUFirstName);
            hsp.Add("@U_SULastName!", weborder.SULastName);
            hsp.Add("@U_SUContID!", weborder.SUContID);
            hsp.Add("@U_Comments!", weborder.Comments);
            hsp.Add("@U_PayProvRef!", weborder.PayProvRef);


            string orderREsult = Program.objHrmsUI.ExecQuery(strInsert, hsp, "addORDR");
            if (orderREsult != "OK") return false;
            int i = 0;

            foreach (DataExtractOrderOrderItem item in weborder.OrderItem)
            {
              
              


                hsp.Clear();
                hsp.Add("@Code!", item.ItemID);
                hsp.Add("@Name!", item.ItemID);
                hsp.Add("@U_OrderID!", item.OrderID);
                hsp.Add("@U_ItemID!", item.ItemID);
                hsp.Add("@U_OrdNo!", item.OrdNo);
                hsp.Add("@U_Code!", item.Code);
                hsp.Add("@U_Title!", item.Title);
                hsp.Add("@U_Price!", item.Price);
                hsp.Add("@U_PriceDisc!", item.PriceDisc);
                hsp.Add("@U_Qty!", item.Qty.ToString());
                hsp.Add("@U_Total!", item.Total.ToString());
                hsp.Add("@U_OrigPrice!", item.OrigPrice.ToString());
                hsp.Add("@U_PromText!", item.PromText);
                hsp.Add("@U_Weight!", item.Weight.ToString());


                i++;

                string strRdr1Insert = @" 
                                INSERT INTO [dbo].[@B1_ABC_RDR1]
                                           ([Code]
                                           ,[Name]
                                           ,[U_OrderID]
                                           ,[U_ItemID]
                                           ,[U_OrdNo]
                                           ,[U_Code]
                                           ,[U_Title]
                                           ,[U_Price]
                                           ,[U_PriceDisc]
                                           ,[U_Qty]
                                           ,[U_Total]
                                           ,[U_OrigPrice]
                                           ,[U_PromText]
                                           ,[U_Weight])
                                     VALUES
                                           (@Code!
                                           ,@Name!
                                           ,@U_OrderID!
                                           ,@U_ItemID!
                                           ,@U_OrdNo!
                                           ,@U_Code!
                                           ,@U_Title!
                                           ,@U_Price!
                                           ,@U_PriceDisc!
                                           ,@U_Qty!
                                           ,@U_Total!
                                           ,@U_OrigPrice!
                                           ,@U_PromText!
                                           ,@U_Weight!)";


                string detailResult = Program.objHrmsUI.ExecQuery(strRdr1Insert, hsp, "Add Row");
                if (detailResult != "OK") return false;

            }



            return true;
        }

        private void getOrders()
        {
            DateTime OrdersSinceCreateDate = Convert.ToDateTime(dtHead.GetValue("From", 0));
            DateTime OrdersToCreateDate = Convert.ToDateTime(dtHead.GetValue("To", 0));

            string Login = Program.GetMagiConnectLogin();
            string Password = Program.GetMagiConnectPassword();
            Guid OrderStatus_NewOrder = new Guid(System.Configuration.ConfigurationManager.AppSettings["MagiConnectOrderStatus_NewOrder"]);

            MagiConnect_Sales.sales SalesWebService = new MagiConnect_Sales.sales();

            string OrdersXML = SalesWebService.ExportOrders(Login, Password, OrderStatus_NewOrder);

            DataExtract result = new DataExtract();
            XmlSerializer serializer = new XmlSerializer(typeof(DataExtract));
            using (TextReader reader = new StringReader(OrdersXML))
            {
                result = (DataExtract)serializer.Deserialize(reader);

            }







            lblStatus.Caption = "Please wait while loading!!!";

            dtHead.SetValue("TotOrdr", 0, result.Order.Count().ToString());

            Guid OrderStatus_Downloaded = new Guid("EAB0ED46-686F-4B57-8F16-124B03714F16");

            foreach (DataExtractOrder weborder in result.Order)
            {
                bool isDownloaded = saveOrder(weborder);

                if (isDownloaded)
                {

                    string OrderTrackingReference = "";
                    string OrderStatusMessage = "Order Downloaded into SAP";

                    string Results = SalesWebService.UpdateOrderStatus(Login, Password,
                       new Guid(weborder.OrderID), OrderStatus_NewOrder, OrderStatus_Downloaded, OrderTrackingReference, OrderStatusMessage);

                }
            }


            OrdersXML = SalesWebService.ExportOrders(Login, Password, OrderStatus_Downloaded);

            using (TextReader reader = new StringReader(OrdersXML))
            {
                result = (DataExtract)serializer.Deserialize(reader);

            }



            FillOrderMatrix();

        }

        private void FillOrderMatrix()
        {
           

            oForm.Freeze(true);
            dtWebO.Rows.Clear();
            int i = 0;
            DateTime OrdersSinceCreateDate = Convert.ToDateTime(dtHead.GetValue("From", 0));
            DateTime OrdersToCreateDate = Convert.ToDateTime(dtHead.GetValue("To", 0));

            string strSql = "Select * from [@B1_ABC_ORDR] where Convert(date, U_Date  ,101) between '" + OrdersSinceCreateDate.ToString("yyyyMMdd") + "' and '" + OrdersToCreateDate.ToString("yyyyMMdd") + "' order by U_OrdNo";

            System.Data.DataTable dtOrders = Program.objHrmsUI.getDataTable(strSql, "gettingOrders");

            string selStatus = cbOS.Selected.Value.ToString();
            foreach ( System.Data.DataRow  dr in dtOrders.Rows)
            {
                string orderStatus = "O";
                string strId = dr["U_OrdNo"].ToString();

                string orderNumber = getDocNum(strId, "ORDR");

                if (orderNumber != "" && orderNumber != "0") orderStatus = "C";

                if (selStatus == "01" || (selStatus == "02" && orderStatus == "O") || (selStatus == "03" && orderStatus == "C"))
                {
                    dtWebO.Rows.Add(1);

                    dtWebO.SetValue("Id", i, (i + 1).ToString());
                    dtWebO.SetValue("WebNum", i, strId);
                    dtWebO.SetValue("RdrNum", i, orderNumber);
                    DateTime utcDate = Convert.ToDateTime(dr["U_Date"]);
                    utcDate = DateTime.SpecifyKind(utcDate, DateTimeKind.Utc);
                    DateTime localDT = utcDate.ToLocalTime();

                    dtWebO.SetValue("RdrDT", i, localDT.ToString());

                    dtWebO.SetValue("DlnNum", i, getDocNum(strId, "ODLN"));
                    dtWebO.SetValue("InvNum", i, getDocNum(strId, "OINV"));

                    i++;
                }
            }

            mtWO.LoadFromDataSource();
            oForm.Freeze(false);
            lblStatus.Caption = "";
            updateRetMatrix();
        }

       
        private void updatelnInfo(string incrementId)
        {
            string strDlnSelect = "Select dln1.DocEntry, dln1.Quantity,odln.TrackNo,dln1.shipdate, oitm.itmsGrpCod as [GRP] from dln1 inner join odln on dln1.docentry = odln.docentry inner join oitm on dln1.itemcode = oitm.itemcode where odln.numatcard = '" + incrementId + "'";
            System.Data.DataTable dtDln = Program.objHrmsUI.getDataTable(strDlnSelect, "Shipment");
            if (dtDln.Rows.Count > 0)
            {
                string grp =  dtDln.Rows[0]["GRP"].ToString();


                for (int i = 0; i < dtRDR1.Rows.Count; i++)
                {
                    string ItemCode = Convert.ToString(  dtRDR1.GetValue("ItemCode", i));
                    System.Data.DataRow[] shipRow = dtDln.Select("ItemCode = '" + ItemCode + "'");
                    if (grp == "170")
                    {
                        shipRow = dtDln.Select("TrackNo <> ''");

                    }
                    if (shipRow.Length > 0)
                    {
                        string dlnEntry = shipRow[0]["DocEntry"].ToString();
                        string qty = shipRow[0]["Quantity"].ToString();
                        string trackNum = shipRow[0]["TrackNo"].ToString();
                        string shipdate =  shipRow[0]["shipdate"].ToString();

                        dtRDR1.SetValue("dlnEntry", i, dlnEntry);
                        dtRDR1.SetValue("dlnQty", i, qty);
                        dtRDR1.SetValue("dlnDate", i, shipdate);
                        dtRDR1.SetValue("TrackNum", i, trackNum);
                        break;

                    }
                }
                
            }

        }

        private void addWebOrder(string orderNum)
        {


            System.Data.DataTable dtWO = Program.objHrmsUI.getDataTable("SELECT * From [@B1_ABC_ORDR] WHERE U_OrdNo = '" + orderNum + "'", "Getting Order from UDT");

            if (dtWO == null || dtWO.Rows.Count == 0) return;

            System.Data.DataRow weborder = dtWO.Rows[0];
            string mLogin = "";



            DateTime shipDate = DateTime.Now.AddDays(1);
            DateTime DeliveryDate = DateTime.Now.AddDays(1);

            string WebID = weborder["U_OrderID"].ToString(); ;
            DateTime utcDate = Convert.ToDateTime(weborder["U_Date"]);
            utcDate = DateTime.SpecifyKind(utcDate, DateTimeKind.Utc);
            DateTime localDT = utcDate.ToLocalTime();

            string OrderDate = localDT.ToString();

            string CCode = "";

            string OrderID = weborder["U_OrderID"].ToString();
            string OrdNo = weborder["U_OrdNo"].ToString();
            string PONo = weborder["U_PONo"].ToString();
            string StatID = weborder["U_StatID"].ToString();
            string StatCode = weborder["U_StatCode"].ToString();
            string Status = weborder["U_Status"].ToString();
            DateTime Date = utcDate;
            string CurrCode = weborder["U_CurrCode"].ToString();
            string Curr = weborder["U_Curr"].ToString();
            int NoItems = Convert.ToInt32(weborder["U_NoItems"]);
            decimal ItemsTotal = Convert.ToDecimal(weborder["U_ItemsTotal"]);
            decimal Total = Convert.ToDecimal(weborder["U_Total"]);
            decimal AmtPaid = Convert.ToDecimal(weborder["U_AmtPaid"]);
            decimal Shipping = Convert.ToDecimal(weborder["U_Shipping"]);
            decimal ShippingFee = Convert.ToDecimal(weborder["U_ShippingFee"]);
            decimal Discount = Convert.ToDecimal(weborder["U_Discount"]);
            string Email = weborder["U_Email"].ToString();
            string DelType = weborder["U_DelType"].ToString();
            string OutletCode = weborder["U_OutletCode"].ToString();
            string Outlet = weborder["U_Outlet"].ToString();
            string OutletID = weborder["U_OutletID"].ToString();
            string ShipFirstName = weborder["U_ShipFirstName"].ToString();
            string ShipLastName = weborder["U_ShipLastName"].ToString();
            string ShipCompany = weborder["U_ShipCompany"].ToString();
            string ShipAdd1 = weborder["U_ShipAdd1"].ToString();
            string ShipAdd2 = weborder["U_ShipAdd2"].ToString();
            string ShipAdd3 = weborder["U_ShipAdd3"].ToString();
            string ShipAdd4 = weborder["U_ShipAdd4"].ToString();
            string ShipPostCode = weborder["U_ShipPostCode"].ToString();
            string ShipRegion = weborder["U_ShipRegion"].ToString();
            string ShipRegionID = weborder["U_ShipRegionID"].ToString();
            string ShipMobile = weborder["U_ShipMobile"].ToString();
            string ShipPhoneDay = weborder["U_ShipPhoneDay"].ToString();
            string ShipPhoneEve = weborder["U_ShipPhoneEve"].ToString();
            string ShipEmail = weborder["U_ShipEmail"].ToString();
            string BillFirstName = weborder["U_BillFirstName"].ToString();
            string BillLastName = weborder["U_BillLastName"].ToString();
            string BillCompany = weborder["U_BillCompany"].ToString();
            string BillAdd1 = weborder["U_BillAdd1"].ToString();
            string BillAdd2 = weborder["U_BillAdd2"].ToString();
            string BillAdd3 = weborder["U_BillAdd3"].ToString();
            string BillAdd4 = weborder["U_BillAdd4"].ToString();
            string BillPostCode = weborder["U_BillPostCode"].ToString();
            string BillRegion = weborder["U_BillRegion"].ToString();
            string BillMobile = weborder["U_BillMobile"].ToString();
            string BillPhoneDay = weborder["U_BillPhoneDay"].ToString();
            string BillPhoneEve = weborder["U_BillPhoneEve"].ToString();
            string PaymentTypeID = weborder["U_PaymentTypeID"].ToString();
            string PaymentType = weborder["U_PaymentType"].ToString();
            string TrackRef = weborder["U_TrackRef"].ToString();
            string StatusMsg = weborder["U_StatusMsg"].ToString();
            string GiftMsg = weborder["U_GiftMsg"].ToString();
            string ContID = weborder["U_ContID"].ToString();
            string CustCode = weborder["U_CustCode"].ToString();
            string CustCodeNo = weborder["U_CustCodeNo"].ToString();
            string Company = weborder["U_Company"].ToString();
            string ContType = weborder["U_ContType"].ToString();
            string IsSpecial = weborder["U_IsSpecial"].ToString();
            string PrimCustCode = weborder["U_PrimCustCode"].ToString();
            string PrimCustCodeNo = weborder["U_PrimCustCodeNo"].ToString();
            string PrimContID = weborder["U_PrimContID"].ToString();
            string SUFirstName = weborder["U_SUFirstName"].ToString();
            string SULastName = weborder["U_SULastName"].ToString();
            string SUContID = weborder["U_SUContID"].ToString();
            string Comments = weborder["U_Comments"].ToString();
            string PayProvRef = weborder["U_PayProvRef"].ToString();
            string SBOPosted = "N";
            string SBOError = "";



            dtCard.Rows.Clear();
            dtCard.Rows.Add(1);
            CCode = "99999999";


            dtCard.SetValue("CardCode", 0, Program.objHrmsUI.settings["WebCardCode"].ToString());


            dtCard.SetValue("DocCur", 0, CurrCode);

            dtCard.SetValue("BName", 0, BillFirstName + " " + BillLastName);
            dtCard.SetValue("BStPO", 0, BillAdd1 + " " + BillAdd2);
            dtCard.SetValue("BCity", 0, BillAdd4);
            dtCard.SetValue("BState", 0, BillRegion);
            dtCard.SetValue("BZip", 0, BillPostCode);
            dtCard.SetValue("BTel", 0, BillPhoneDay);
            dtCard.SetValue("BEmail", 0, Email);
            dtCard.SetValue("pMethod", 0, PaymentType);
            dtCard.SetValue("payRef", 0, PayProvRef);


            dtCard.SetValue("SName", 0, ShipFirstName + " " + ShipLastName);
            dtCard.SetValue("SStPO", 0, ShipAdd1 + " " + ShipAdd2);
            dtCard.SetValue("SCity", 0, ShipAdd4);
            dtCard.SetValue("SState", 0, ShipRegion);
            dtCard.SetValue("SZip", 0, ShipPostCode);
            dtCard.SetValue("STel", 0, ShipPhoneDay);
            dtCard.SetValue("SEmail", 0, ShipEmail);

            dtORDR.Rows.Clear();
            dtORDR.Rows.Add(1);
            dtORDR.SetValue("NumAtCard", 0, orderNum);

            dtORDR.SetValue("WBDT", 0, localDT.ToString());
            dtORDR.SetValue("WBID", 0, OrdNo);





            dtRDR1.Rows.Clear();
            int i = 0;

            mtOD.Clear();

            System.Data.DataTable dtRows = Program.objHrmsUI.getDataTable("Select * from [@B1_ABC_RDR1] Where U_OrdNo = '" + OrdNo + "' ", "getting detail row");
            foreach (System.Data.DataRow item in dtRows.Rows)
            {
                dtRDR1.Rows.Add(1);
                dtRDR1.SetValue("ItemCode", i, item["U_Code"].ToString());
                dtRDR1.SetValue("ItemName", i, item["U_Title"].ToString());

                string strExist = "Select frozenFor from oitm where itemcode = '" + item["U_Code"].ToString() + "'";

                System.Data.DataTable dtExist = Program.objHrmsUI.getDataTable(strExist, "GetItem");
                if (dtExist != null && dtExist.Rows.Count > 0)
                {
                    dtRDR1.SetValue("Exist", i, "Y");
                    string active = dtExist.Rows[0]["frozenFor"].ToString() == "Y" ? "N" : "Y";
                    dtRDR1.SetValue("Active", i, active);

                }
                else
                {
                    dtRDR1.SetValue("Active", i, "N");
                    dtRDR1.SetValue("Exist", i, "N");

                }


                dtRDR1.SetValue("Qty", i, item["U_Qty"].ToString());
                dtRDR1.SetValue("Price", i, item["U_Price"].ToString());
                dtRDR1.SetValue("DiscP", i, item["U_PriceDisc"].ToString());
                dtRDR1.SetValue("Address1", i, ShipAdd1);
                dtRDR1.SetValue("Address2", i, ShipAdd2);
                dtRDR1.SetValue("City", i, ShipAdd4);
                dtRDR1.SetValue("GiftCard", i, GiftMsg == null ? "" : GiftMsg.ToString());
                dtRDR1.SetValue("ItemId", i, item["U_ItemID"].ToString());


                i++;

            }
            
            mtOD.LoadFromDataSource();
            updateRowMatrix();
            
        }

        private void updateShipped()
        {
            string Login = Program.GetMagiConnectLogin();
            string Password = Program.GetMagiConnectPassword();
            Guid OrderStatus_Downloaded = new Guid("EAB0ED46-686F-4B57-8F16-124B03714F16");

            MagiConnect_Sales.sales SalesWebService = new MagiConnect_Sales.sales();
           
            Guid OrderStatus_Dispatched = new Guid("07266FFE-E61A-41A8-8422-FBD9608431AC");
            string strToShip = "Select t0.DocEntry,isnull(t0.U_B1_ABC_WEBID,'')  AS WebNum, t1.Code as WebId , t0.TrackNo from odln t0 inner join [@B1_ABC_ORDR] t1 on isnull(t0.U_B1_ABC_WEBID,'') = t1.U_OrdNo  where isnull(U_B1_ABC_WEBID,'') <> '' and isnull(U_B1_ABC_DELIVERED,'N') = 'N'";
            System.Data.DataTable dtShipment = Program.objHrmsUI.getDataTable(strToShip, "Shipment Candidates");
            if (dtShipment != null && dtShipment.Rows.Count > 0)
            {
                foreach (System.Data.DataRow dr in dtShipment.Rows)
                {
                    string DE = dr["DocEntry"].ToString();
                    string WebNum = dr["WebNum"].ToString();
                    string WebId = dr["WebId"].ToString();
                    string trackingNum = dr["TrackNo"].ToString();
                    string orderStatusMessage = "Order Dispatched";

                    string Results = SalesWebService.UpdateOrderStatus(Login, Password,
                     new Guid(WebId), OrderStatus_Downloaded, OrderStatus_Dispatched, trackingNum, orderStatusMessage);
                    if (Results == "OK")
                    {
                        string strUpdateDLN = "Update ODLN set U_B1_ABC_DELIVERED='Y' Where DocEntry = '" + DE + "'";
                        Program.objHrmsUI.ExecQuery(strUpdateDLN, "Delivered");
                    }

                }
                oApplication.SetStatusBarMessage("Updated all delivered orders on web!", BoMessageTime.bmt_Short, false);
            }
        }

        private void updateInvoiced()
        {
            string Login = Program.GetMagiConnectLogin();
            string Password = Program.GetMagiConnectPassword();
          
            MagiConnect_Sales.sales SalesWebService = new MagiConnect_Sales.sales();

            Guid OrderStatus_Dispatched = new Guid("07266FFE-E61A-41A8-8422-FBD9608431AC");
            Guid OrderStatus_Completed = new Guid("9aeb2f77-f1e2-4548-a7c8-a009e75a0f0f");


            string strToShip = "Select t0.DocEntry,isnull(t0.U_B1_ABC_WEBID,'')  AS WebNum, t1.Code as WebId from OINV t0 inner join [@B1_ABC_ORDR] t1 on isnull(t0.U_B1_ABC_WEBID,'') = t1.U_OrdNo  where isnull(U_B1_ABC_WEBID,'') <> '' and isnull(U_B1_ABC_COMPLETED,'N') = 'N'";
            System.Data.DataTable dtShipment = Program.objHrmsUI.getDataTable(strToShip, "Shipment Candidates");
            if (dtShipment != null && dtShipment.Rows.Count > 0)
            {
                foreach (System.Data.DataRow dr in dtShipment.Rows)
                {
                    string DE = dr["DocEntry"].ToString();
                    string WebNum = dr["WebNum"].ToString();
                    string WebId = dr["WebId"].ToString();
                    string trackingNum = "";
                    string orderStatusMessage = "Order Completed";

                    string Results = SalesWebService.UpdateOrderStatus(Login, Password,
                     new Guid(WebId), OrderStatus_Dispatched, OrderStatus_Completed, trackingNum, orderStatusMessage);
                    if (Results == "OK")
                    {
                        string strUpdateDLN = "Update OINV set U_B1_ABC_COMPLETED='Y' Where DocEntry = '" + DE + "'";
                        Program.objHrmsUI.ExecQuery(strUpdateDLN, "Completed");
                    }

                }
                oApplication.SetStatusBarMessage("Updated all completed orders on web!", BoMessageTime.bmt_Short, false);
            }
        }

        private string getDocNum(string orderId, string tblName)
        {
            string strResult = "";
            string strSql = "Select DocEntry from [" + tblName + "] where isnull(NumAtCard,'') = '" + orderId + "'";

            System.Data.DataTable dt = Program.objHrmsUI.getDataTable(strSql, "Get DE");
            if (dt != null && dt.Rows.Count > 0)
            {
                strResult = dt.Rows[0]["DocEntry"].ToString();
            }

            return strResult;

        }
        private string StateCode(string State)
        {
            string strResult = "";
            string strSql = "Select Top 1 Code from OCST where Name = '" + State + "' and country = 'US'";

            System.Data.DataTable dt = Program.objHrmsUI.getDataTable(strSql, "Get DE");
            if (dt != null && dt.Rows.Count > 0)
            {
                strResult = dt.Rows[0]["Code"].ToString();
            }

            return strResult;

        }
        private string CountryCode(string Country)
        {
            string strResult = "";
            string strSql = "Select Top 1 Code from OCRY where Name = '" + Country + "'";

            System.Data.DataTable dt = Program.objHrmsUI.getDataTable(strSql, "Get Country");
            if (dt != null && dt.Rows.Count > 0)
            {
                strResult = dt.Rows[0]["Code"].ToString();
            }

            return strResult;

        }
        private string getShipType(string Shiptype)
        {
            string strResult = "1";
            string strSql = "Select Top 1 [TrnspCode] from OSHP where [TrnspName] = '" + Shiptype + "' or [WebSite]   = '"+ Shiptype +"'";

            System.Data.DataTable dt = Program.objHrmsUI.getDataTable(strSql, "Get DE");
            if (dt != null && dt.Rows.Count > 0)
            {
                strResult = dt.Rows[0]["TrnspCode"].ToString();
            }

            return strResult;

        }

        private void InitiallizeForm()
        {

            Program.objHrmsUI.loadSettings();

           oForm.Freeze(true);

            initiallizing = true;


            mtWO = (SAPbouiCOM.Matrix)oForm.Items.Item("mtWO").Specific;
            mtOD = (SAPbouiCOM.Matrix)oForm.Items.Item("mtOD").Specific;

            lblStatus = (SAPbouiCOM.StaticText)oForm.Items.Item("lblStatus").Specific;
            dtRDR1 = oForm.DataSources.DataTables.Item("dtRDR1");
            dtORDR = oForm.DataSources.DataTables.Item("dtORDR");
            dtHead = oForm.DataSources.DataTables.Item("dtHead");
            dtCard = oForm.DataSources.DataTables.Item("dtCard");
            dtWebO = oForm.DataSources.DataTables.Item("dtWebO");

            cbOS = (SAPbouiCOM.ComboBox) oForm.Items.Item("cbOS").Specific;
            cbOS.ValidValues.Add("01", "All");
            cbOS.ValidValues.Add("02", "Not Posted");
            cbOS.ValidValues.Add("03", "Posted");
            cbOS.Item.DisplayDesc = true;
            cbOS.Select("01", BoSearchKey.psk_ByValue);
            txCEmail = (SAPbouiCOM.EditText)oForm.Items.Item("txCEmail").Specific;
            txCNCode = (SAPbouiCOM.EditText)oForm.Items.Item("txCNCode").Specific;
            txCNTel = (SAPbouiCOM.EditText)oForm.Items.Item("txCNTel").Specific;
            txCNEmail = (SAPbouiCOM.EditText)oForm.Items.Item("txCNEmail").Specific;
            txSName = (SAPbouiCOM.EditText)oForm.Items.Item("txSName").Specific;
            txSSPO = (SAPbouiCOM.EditText)oForm.Items.Item("txSSPO").Specific;
            txSCity = (SAPbouiCOM.EditText)oForm.Items.Item("txSCity").Specific;
            txSState = (SAPbouiCOM.EditText)oForm.Items.Item("txSState").Specific;
            txSZip = (SAPbouiCOM.EditText)oForm.Items.Item("txSZip").Specific;
            txSPhone = (SAPbouiCOM.EditText)oForm.Items.Item("txSPhone").Specific;
            txSEmail = (SAPbouiCOM.EditText)oForm.Items.Item("txSEmail").Specific;

            txCTel = (SAPbouiCOM.EditText)oForm.Items.Item("txCTel").Specific;
            txCCode = (SAPbouiCOM.EditText)oForm.Items.Item("txCCode").Specific;
            txOP = (SAPbouiCOM.EditText)oForm.Items.Item("txOP").Specific;
            txDPM = (SAPbouiCOM.EditText)oForm.Items.Item("txDPM").Specific;
            txMMI = (SAPbouiCOM.EditText)oForm.Items.Item("txMMI").Specific;
            txWDA = (SAPbouiCOM.EditText)oForm.Items.Item("txWDA").Specific;
            txWD = (SAPbouiCOM.EditText)oForm.Items.Item("txWD").Specific;
            txDC = (SAPbouiCOM.EditText)oForm.Items.Item("txDC").Specific;
            txCCBZ = (SAPbouiCOM.EditText)oForm.Items.Item("txCCBZ").Specific;
            txCCED = (SAPbouiCOM.EditText)oForm.Items.Item("txCCED").Specific;

            txWODT = (SAPbouiCOM.EditText)oForm.Items.Item("txWODT").Specific;
            txTotO = (SAPbouiCOM.EditText)oForm.Items.Item("txTotO").Specific;
            txTotSel = (SAPbouiCOM.EditText)oForm.Items.Item("txTotSel").Specific;
            txCR = (SAPbouiCOM.EditText)oForm.Items.Item("txCR").Specific;
            txOS = (SAPbouiCOM.EditText)oForm.Items.Item("txOS").Specific;
            txNC = (SAPbouiCOM.EditText)oForm.Items.Item("txNC").Specific;
            tx4D = (SAPbouiCOM.EditText)oForm.Items.Item("tx4D").Specific;
            txAutID = (SAPbouiCOM.EditText)oForm.Items.Item("txAutID").Specific;
            txCCAmt = (SAPbouiCOM.EditText)oForm.Items.Item("txCCAmt").Specific;
            txCCTD = (SAPbouiCOM.EditText)oForm.Items.Item("txCCTD").Specific;

            txFrom = (SAPbouiCOM.EditText)oForm.Items.Item("txFrom").Specific;
            txTo = (SAPbouiCOM.EditText)oForm.Items.Item("txTo").Specific;
            txWO = (SAPbouiCOM.EditText)oForm.Items.Item("txWO").Specific;

            cbOS = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbOS").Specific;
            btPost = (SAPbouiCOM.Button)oForm.Items.Item("btPost").Specific;
            btGet = (SAPbouiCOM.Button)oForm.Items.Item("btGet").Specific;

            ini_controls();

            oForm.PaneLevel = 1;
            oForm.Freeze(false);

            initiallizing = false;



        }

        private void ini_controls()
        {
            dtRDR1.Rows.Clear();

            dtORDR.Rows.Clear();
            dtHead.Rows.Clear();
            dtCard.Rows.Clear();
            dtWebO.Rows.Clear();

            dtHead.Rows.Add(1);


            dtHead.SetValue("From", 0, "");
            dtHead.SetValue("To", 0, "");
            dtHead.SetValue("TotOrdr", 0, "0");
            dtHead.SetValue("TotSel", 0, "0");

              mtOD.LoadFromDataSource();
              mtWO.LoadFromDataSource();
        }

        private void clearOrderInfo()
        {
            dtRDR1.Rows.Clear();

            dtORDR.Rows.Clear();
            dtCard.Rows.Clear();
          
            dtHead.Rows.Add(1);
            dtORDR.Rows.Add(1);
            dtCard.Rows.Add(1);

            mtOD.LoadFromDataSource();
          
        }
  
        private void fillCb()
        {


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
        private void ProcessPosting()
        {
            mtWO.FlushToDataSource();
            for (int i = 0; i < dtWebO.Rows.Count; i++)
            {
                string sel = dtWebO.GetValue("sel", i).ToString();
                if (sel == "Y")
                {
                    clearOrderInfo();
                    string webId = dtWebO.GetValue("WebNum", i).ToString(); ;
                    //try
                    //{
                        addWebOrder(webId);
                        postDocument();
                    //}
                    //catch (Exception ex)
                    //{
                    //    oApplication.StatusBar.SetText("Error adding order " + webId + " \n\r " + ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);

                    //}
                }

            }

            oApplication.StatusBar.SetText("Process completed", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
            getOrders();

        }
      
        public string addBp()
        {
            string result = "";
            SAPbobsCOM.BusinessPartners defbp = (SAPbobsCOM.BusinessPartners)Program.objHrmsUI.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);

            defbp.GetByKey("W-00006935");


            SAPbobsCOM.BusinessPartners bp = (SAPbobsCOM.BusinessPartners)Program.objHrmsUI.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);


            bp.CardCode = dtCard.GetValue("CardCode", 0).ToString(); ;
            bp.CardType = SAPbobsCOM.BoCardTypes.cCustomer;
            bp.CardName = dtCard.GetValue("BName", 0).ToString(); ;

            bp.Phone1 = dtCard.GetValue("CTel", 0).ToString(); ;
            bp.EmailAddress = dtCard.GetValue("CEmail", 0).ToString(); ;
            bp.FederalTaxID = "EX";




            bp.GroupCode = defbp.GroupCode;
            bp.PayTermsGrpCode = defbp.PayTermsGrpCode;
            bp.DownPaymentClearAct = defbp.DownPaymentClearAct;
            bp.DefaultAccount = defbp.DefaultAccount;
            bp.DebitorAccount = defbp.DebitorAccount;



            bp.ContactEmployees.Name = dtCard.GetValue("CNID", 0).ToString(); ;
            bp.ContactEmployees.Phone1 = dtCard.GetValue("CNTel", 0).ToString(); ;
            bp.ContactEmployees.E_Mail = dtCard.GetValue("CNEmail", 0).ToString(); ;

            bp.Addresses.AddressType = SAPbobsCOM.BoAddressType.bo_BillTo;
            bp.Addresses.AddressName = dtCard.GetValue("BName", 0).ToString(); ;
            bp.Addresses.Street = dtCard.GetValue("BStPO", 0).ToString(); ;
            bp.Addresses.City = dtCard.GetValue("BCity", 0).ToString(); ;
            string statecode = StateCode(dtCard.GetValue("BState", 0).ToString()) ;
            if (statecode != "")
            {
                bp.Addresses.State = statecode;
            }
            bp.Addresses.ZipCode = dtCard.GetValue("BZip", 0).ToString(); ;
            bp.Addresses.UserFields.Fields.Item("U_PhoneNum").Value = dtCard.GetValue("BTel", 0).ToString(); ;
            bp.Addresses.UserFields.Fields.Item("U_EMail").Value = dtCard.GetValue("BEmail", 0).ToString(); ;
            bp.Addresses.TaxCode = "EX";
            bp.Addresses.Add();

            bp.Addresses.AddressType = SAPbobsCOM.BoAddressType.bo_ShipTo;
            bp.Addresses.AddressName = "S-" + dtCard.GetValue("SName", 0).ToString(); ;
            bp.Addresses.Street = dtCard.GetValue("SStPO", 0).ToString(); ;
            bp.Addresses.City = dtCard.GetValue("SCity", 0).ToString(); ;
            statecode = StateCode(dtCard.GetValue("SState", 0).ToString()).ToString(); ;
            if (statecode != "")
            {
                bp.Addresses.State = statecode;
            }

            bp.Addresses.ZipCode = dtCard.GetValue("SZip", 0).ToString();
            bp.Addresses.UserFields.Fields.Item("U_PhoneNum").Value = dtCard.GetValue("STel", 0);
            bp.Addresses.UserFields.Fields.Item("U_EMail").Value = dtCard.GetValue("SEmail", 0);
            bp.Addresses.TaxCode = "EX";
            bp.Addresses.Add();


            int bpSuccess = bp.Add();
            if (bpSuccess != 0)
            {
                int erroCode = 0;
                string errDescr = "";
                Program.objHrmsUI.oCompany.GetLastError(out erroCode, out errDescr);
                oApplication.StatusBar.SetText("Failed to add BP : " + errDescr);
            }
            return result;
        }
        private void addShipTo(SAPbobsCOM.BusinessPartners bp)
        {
            int addressFound = 0;
            int addressCnt = 0;
            for (int i = 0; i < bp.Addresses.Count; i++)
            {
                bp.Addresses.SetCurrentLine(i);
                if (bp.Addresses.AddressName == "S-" + dtCard.GetValue("SName", 0) && bp.Addresses.AddressType == SAPbobsCOM.BoAddressType.bo_ShipTo)
                {

                    bp.Addresses.ZipCode = dtCard.GetValue("SZip", 0).ToString();
                    bp.Addresses.AddressType = SAPbobsCOM.BoAddressType.bo_ShipTo;
                    bp.Addresses.Street = dtCard.GetValue("SStPO", 0).ToString();
                    bp.Addresses.City = dtCard.GetValue("SCity", 0).ToString();
                    // bp.Addresses.TaxCode = "EX";
                    bp.Addresses.Country = CountryCode( dtCard.GetValue("SState", 0).ToString());
                    //string statecode = StateCode(dtCard.GetValue("SState", 0).ToString());
                    //if (statecode != "")
                    //{
                    //    bp.Addresses.State = statecode;
                    //}
                    addressFound = 1;
                }
                addressCnt++;

            }
            if (addressFound == 0)
            {
                bp.Addresses.Add();
                bp.Addresses.SetCurrentLine(addressCnt);

                bp.Addresses.AddressName = "S-" + dtCard.GetValue("SName", 0);
                bp.Addresses.ZipCode = dtCard.GetValue("SZip", 0).ToString();
                bp.Addresses.AddressType = SAPbobsCOM.BoAddressType.bo_ShipTo;
                bp.Addresses.Street = dtCard.GetValue("SStPO", 0).ToString();
                bp.Addresses.City = dtCard.GetValue("SCity", 0).ToString();
                bp.Addresses.Country = CountryCode(dtCard.GetValue("SState", 0).ToString());

                // bp.Addresses.TaxCode = "EX";

                //string statecode = StateCode(dtCard.GetValue("SState", 0).ToString());
                //if (statecode != "")
                //{
                //    bp.Addresses.State = statecode;
                //}
                bp.Addresses.UserFields.Fields.Item("U_PhoneNum").Value = dtCard.GetValue("STel", 0).ToString(); ;// sTel == null ? "" : sTel;
                bp.Addresses.UserFields.Fields.Item("U_EMail").Value = dtCard.GetValue("BEmail", 0).ToString(); // sEmail == null ? "" : sEmail;



            }

            int result =  bp.Update();

            if (result != 0)
            {
                int erroCode = 0;
                string errDescr = "";
                Program.objHrmsUI.oCompany.GetLastError(out erroCode, out errDescr);
                if (errDescr.Contains("This entry already exists in the following tables"))
                {
                }
                else
                {
                    oApplication.StatusBar.SetText("Failed to post Order : " + errDescr);
                }
            }
            else
            {
             

              //  oApplication.StatusBar.SetText("Sales Order Posted Successfully for " + dtORDR.GetValue("WBID", 0), BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);

            }

        }
        private void addBillTo(SAPbobsCOM.BusinessPartners bp)
        {
            int addressFound = 0;
            int addressCNt = 0;
            for (int i = 0; i < bp.Addresses.Count; i++)
            {
                bp.Addresses.SetCurrentLine(i);
                if (bp.Addresses.AddressName ==  dtCard.GetValue("BName", 0) && bp.Addresses.AddressType == SAPbobsCOM.BoAddressType.bo_BillTo)
                {


                    bp.Addresses.Street = dtCard.GetValue("BStPO", 0).ToString();
                    bp.Addresses.City = dtCard.GetValue("BCity", 0).ToString();
                    string statecode = StateCode(dtCard.GetValue("BState", 0).ToString());
                    if (statecode != "")
                    {
                        bp.Addresses.State = statecode;
                    }
                    bp.Addresses.ZipCode = dtCard.GetValue("BZip", 0).ToString();

                    addressFound = 1;
                }
                addressCNt++;
                
            }
            if (addressFound == 0)
            {
                bp.Addresses.Add();
                bp.Addresses.SetCurrentLine(addressCNt);

                bp.Addresses.AddressType = SAPbobsCOM.BoAddressType.bo_BillTo;
                bp.Addresses.AddressName = dtCard.GetValue("BName", 0).ToString();
                bp.Addresses.Street = dtCard.GetValue("BStPO", 0).ToString();
                bp.Addresses.City = dtCard.GetValue("BCity", 0).ToString();
                bp.Addresses.TaxCode = "EX";

                string statecode = StateCode(dtCard.GetValue("BState", 0).ToString());
                if (statecode != "")
                {
                    bp.Addresses.State = statecode;
                }
                bp.Addresses.ZipCode = dtCard.GetValue("BZip", 0).ToString();


            }

            bp.Update();
        }

        private void updateRetMatrix()
        {
            for (int i = 0; i < dtWebO.Rows.Count; i++)
            {
                string docEnt = Convert.ToString(dtWebO.GetValue("RdrNum", i));
                if (docEnt != "" && docEnt != "0")
                {
                    mtWO.CommonSetting.SetCellEditable(i + 1, 1, false);
                 


                }
                else
                {
                    mtWO.CommonSetting.SetCellEditable(i + 1, 1, true);

                }
            }

        }
        private void updateRowMatrix()
        {
            for (int i = 0; i < dtRDR1.Rows.Count; i++)
            {
                string Active = Convert.ToString(dtRDR1.GetValue("Active", i));
                string Exist = Convert.ToString(dtRDR1.GetValue("Exist", i));

                if (Exist=="Y")
                {
                    if (Active == "N")
                    {
                        mtOD.CommonSetting.SetCellBackColor(i + 1, 1, Common.SboConsts.COLOR_YELLOW);

                    }
                    else
                    {
                      mtOD.CommonSetting.SetCellBackColor(i + 1, 1, mtOD.CommonSetting.GetCellBackColor(i+1,2) );

                    }

                }
                else
                {
                    mtOD.CommonSetting.SetCellBackColor(i + 1, 1, Common.SboConsts.COLOR_RED);

                }
            }

        }
        public string postDocument()
        {

            Program.objHrmsUI.loadSettings();
            string outStr = "";
            try
            {

                if (dtORDR.Rows.Count == 0)
                    return "No Items";

                SAPbobsCOM.Documents Doc = (SAPbobsCOM.Documents)Program.objHrmsUI.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
                SAPbobsCOM.BusinessPartners bp = (SAPbobsCOM.BusinessPartners)Program.objHrmsUI.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
               
                bool bpexist = bp.GetByKey(dtCard.GetValue("CardCode", 0).ToString());
                if (!bpexist)
                {
                    addBp();

                }
                else
                {
                  //  addBillTo(bp);
                    addShipTo(bp);
                }
                Doc.DocCurrency = dtCard.GetValue("DocCur", 0).ToString();
                Doc.CardCode = dtCard.GetValue("CardCode", 0).ToString();
                Doc.DocDate =Convert.ToDateTime(dtORDR.GetValue("WBDT", 0));
                Doc.DocDueDate = Convert.ToDateTime(dtORDR.GetValue("WBDT", 0));
                Doc.SalesPersonCode = Convert.ToInt16( Program.objHrmsUI.settings["WebSlpCode"].ToString());
                Doc.PickRemark = dtORDR.GetValue("NumAtCard", 0).ToString();
                Doc.NumAtCard = dtORDR.GetValue("NumAtCard", 0).ToString();
                Doc.UserFields.Fields.Item("U_B1_ABC_PAYMETHOD").Value =dtCard.GetValue("pMethod", 0).ToString();
                Doc.UserFields.Fields.Item("U_B1_ABC_PAYREF").Value = dtCard.GetValue("payRef", 0).ToString();
                Doc.UserFields.Fields.Item("U_B1_ABC_WEBID").Value = dtORDR.GetValue("NumAtCard", 0).ToString();

                Doc.ShipToCode = "S-" + dtCard.GetValue("SName", 0).ToString();


                string Branch = Program.objHrmsUI.settings["WebBranch"].ToString();
                string WarehouseCode = Program.objHrmsUI.settings["Warehouse"].ToString();


                if (Branch != null && Branch != "")
                {
                    Doc.BPL_IDAssignedToInvoice = Convert.ToInt32( Branch);
                }
              //  Doc.PayToCode = dtCard.GetValue("BName", 0).ToString();
              



                for (int i = 0; i < dtRDR1.Rows.Count; i++)
                {

                    if (dtRDR1.GetValue("ItemCode", i).ToString() != "")
                    {
                        Doc.Lines.ItemCode = dtRDR1.GetValue("ItemCode", i).ToString();
                        Doc.Lines.ItemDescription = dtRDR1.GetValue("ItemName", i).ToString(); //rs.Fields.Item("ItemName").Value;
                        Doc.Lines.Quantity = Convert.ToDouble(dtRDR1.GetValue("Qty", i));
                        Doc.Lines.PriceAfterVAT = Convert.ToDouble(dtRDR1.GetValue("Price", i));
                        Doc.Lines.DiscountPercent = Convert.ToDouble(dtRDR1.GetValue("DiscP", i));
                        if (WarehouseCode != "") Doc.Lines.WarehouseCode = WarehouseCode;
                        //  Doc.Lines.ShipDate = Convert.ToDateTime(dtRDR1.GetValue("ShipDate", i));
                        string stCode = Convert.ToString(dtRDR1.GetValue("ST", i));
                        stCode = getShipType(stCode);
                        Doc.Lines.ShippingMethod = Convert.ToInt32(stCode);

                        Doc.Lines.Add();
                    }
                }

                try
                {

                    if (Doc.Add() != 0)
                    {
                        int erroCode = 0;
                        string errDescr = "";
                        Program.objHrmsUI.oCompany.GetLastError(out erroCode, out errDescr);
                        outStr = "Error:" + errDescr + outStr;
                        oApplication.StatusBar.SetText("Failed to post Order : " + errDescr);
                    }
                    else
                    {
                        outStr = Convert.ToString(Program.objHrmsUI.oCompany.GetNewObjectKey());

                      
                        oApplication.StatusBar.SetText("Sales Order Posted Successfully for " + dtORDR.GetValue("WBID", 0), BoMessageTime.bmt_Short,BoStatusBarMessageType.smt_Success);
                        oApplication.SetStatusBarMessage("Posting DPM", BoMessageTime.bmt_Short, false);
                        addDownPayment( Convert.ToInt32( outStr));

                    }
                }
                catch (Exception ex)
                {
                    oApplication.StatusBar.SetText("Failed in Exec Query on Posting Document.  : " + ex.Message);
                }
                finally
                {


                }
            }
            catch (Exception ex)
            {
                oApplication.SetStatusBarMessage("Error in posting document : " + ex.Message);
            }
            return outStr;

        }

       private void selectAllWO()
        {
            try
            {

                oForm.Freeze(true);
                SAPbouiCOM.Column col = mtWO.Columns.Item("V_3");

                if (col.TitleObject.Caption == "✓")
                {
                    for (int i = 0; i < dtWebO.Rows.Count; i++)
                    {

                        dtWebO.SetValue("sel", i, "N");
                        col.TitleObject.Caption = "";
                    }
                }
                else
                {
                    for (int i = 0; i < dtWebO.Rows.Count; i++)
                    {
                        string rdrNum = dtWebO.GetValue("RdrNum", i).ToString();
                        if (rdrNum != "") continue;
                        dtWebO.SetValue("sel", i, "Y");
                        col.TitleObject.Caption = "✓";
                    }
                }
                mtWO.LoadFromDataSource();
                oForm.Freeze(false);
            }
            catch (Exception ex)
            {
                oApplication.SetStatusBarMessage("Error occured " + ex.Message);
                // iniSalaryDetail();
            }
        }


        private void addDownPayment(int docEntry)
        {

            bool Success = false;
            SAPbobsCOM.Documents ObjDownPayment;
            SAPbobsCOM.Documents objSO;
            objSO = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
            objSO.GetByKey(docEntry);

            string docCur = objSO.DocCurrency;

            string PayMethod = objSO.UserFields.Fields.Item("U_B1_ABC_PAYMETHOD").Value.ToString();
            string PayRef = objSO.UserFields.Fields.Item("U_B1_ABC_PAYREF").Value.ToString();

            ObjDownPayment = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDownPayments);
            ObjDownPayment.DownPaymentType = SAPbobsCOM.DownPaymentTypeEnum.dptInvoice;
            ObjDownPayment.CardCode = objSO.CardCode;
            ObjDownPayment.NumAtCard = objSO.NumAtCard;
            ObjDownPayment.DocDate = objSO.DocDate;
            ObjDownPayment.DocDueDate = objSO.DocDate;
            ObjDownPayment.NumAtCard = objSO.NumAtCard;
            ObjDownPayment.TrackingNumber = PayRef;
            string Branch = Program.objHrmsUI.settings["WebBranch"].ToString();

            if (Branch != null && Branch != "")
            {
                ObjDownPayment.BPL_IDAssignedToInvoice = Convert.ToInt32(Branch);

            }

            ObjDownPayment.UserFields.Fields.Item("U_B1_ABC_PAYMETHOD").Value = objSO.UserFields.Fields.Item("U_B1_ABC_PAYMETHOD").Value.ToString();
            ObjDownPayment.UserFields.Fields.Item("U_B1_ABC_PAYREF").Value = objSO.UserFields.Fields.Item("U_B1_ABC_PAYREF").Value.ToString();

            for (int i = 0; i < objSO.Lines.Count; i++)
            {
                objSO.Lines.SetCurrentLine(i);

                ObjDownPayment.Lines.BaseType = 17;
                ObjDownPayment.Lines.BaseEntry = objSO.DocEntry;
                ObjDownPayment.Lines.BaseLine = objSO.Lines.VisualOrder;
                ObjDownPayment.Lines.ItemCode = objSO.Lines.ItemCode;
                ObjDownPayment.Lines.Quantity = objSO.Lines.Quantity;
                //  ObjDownPayment.Lines.UnitPrice = objSO.Lin
                ObjDownPayment.Lines.Add();

            }




            ObjDownPayment.DownPaymentPercentage = 100;


            // ObjDownPayment.DownPaymentAmount = amount

            Int32 lRetCode = ObjDownPayment.Add();
            if (lRetCode != 0)
            {
                Success = false;
                int erroCode = 0;
                string errDescr = "";
                oCompany.GetLastError(out erroCode, out errDescr);
                errDescr = errDescr.Replace("'", "");

                if (errDescr.Contains("Closing Period"))
                {

                }
                else
                {


                }
                oApplication.SetStatusBarMessage("Posting DPM Error " + errDescr, BoMessageTime.bmt_Short, true);


            }
            else
            {
                Success = true;
                int dpmKey = Convert.ToInt32(Program.objHrmsUI.oCompany.GetNewObjectKey());

                SAPbobsCOM.Payments ObjPayment = (SAPbobsCOM.Payments)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments);
                ObjPayment.CardCode = objSO.CardCode;
                ObjPayment.DocDate = objSO.DocDate;
                ObjPayment.DocType = SAPbobsCOM.BoRcptTypes.rCustomer;

                if (Branch != null && Branch != "")
                {
                    ObjPayment.BPLID = Convert.ToInt32(Branch);
                }

                ObjPayment.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_DownPayment;
                ObjPayment.Invoices.DocEntry = dpmKey;


                ObjPayment.Invoices.SumApplied = objSO.DocTotal;

                ObjPayment.Invoices.Add();

                ObjPayment.UserFields.Fields.Item("U_B1_ABC_PAYREF").Value = PayRef;
                ObjPayment.UserFields.Fields.Item("U_B1_ABC_PAYMETHOD").Value = PayMethod;
                ObjPayment.UserFields.Fields.Item("U_B1_ABC_WEBNUM").Value = objSO.NumAtCard;
                ObjPayment.DocCurrency = objSO.DocCurrency;
                ObjPayment.DocRate = objSO.DocRate;

                if (objSO.DocRate != 1)
                {
                    ObjPayment.CashSum = objSO.DocTotalFc;
                }
                else
                {
                    ObjPayment.CashSum = objSO.DocTotal;
                }
                ObjPayment.CashAccount = getCashAcct(PayMethod, docCur);


                if (ObjPayment.Add() == 0)
                {
                    Success = true;
                }
                else
                {
                    Success = false;
                    int erroCode = 0;
                    string errDescr = "";
                    oCompany.GetLastError(out erroCode, out errDescr);
                    errDescr = errDescr.Replace("'", "");

                    oApplication.SetStatusBarMessage("Posting DPM Error " + errDescr, BoMessageTime.bmt_Short, true);

                    Success = false;



                }

            }
        }

        private string getCashAcct(string payType, string DocCur)
       {
           string result = "";
           string strSql = "Select U_GL from [@B1_ABC_PAYMETHOD] where [Name] = '" + payType + "' and U_CUR='" + DocCur + "'";

           try
           {
               result = Convert.ToString( Program.objHrmsUI.getScallerValue(strSql));
           }
           catch { }

           return result;
       }

        private void updateStock()
        {
            string WarehouseCode = Program.objHrmsUI.settings["Warehouse"].ToString();
            string Login = Program.GetMagiConnectLogin();
            string Password = Program.GetMagiConnectPassword();

            MagiConnect_Stock.stock StockWebService = new MagiConnect_Stock.stock();

            System.Data.DataTable dtStock = Program.objHrmsUI.getDataTable("select OnHand,IsCommited, OnHand-IsCommited as Available,ItemCode from oitw where WhsCode = '" + WarehouseCode + "' ", "GetStock");
            List<string> itemCodes = new List<string>();
            List<decimal> itemQtys = new List<decimal>();
             

            foreach (System.Data.DataRow dr in dtStock.Rows)
            {
                itemCodes.Add(dr["ItemCode"].ToString());
                itemQtys.Add( Convert.ToDecimal( dr["Available"]));
               
            }

          string result=  StockWebService.UpdateStockActuals(Login, Password, itemCodes.ToArray(), itemQtys.ToArray());


        }
    }
}
