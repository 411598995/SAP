using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Mag =  ACHR.com.thefruitcompany.www;

using SAPbouiCOM;

namespace ACHR.Screen
{
    class frm_WOC : HRMSBaseForm
    {

        List<Mag.salesOrderListEntity> salesOrderEntityList;
       



        public bool isForLoading = false;
        SAPbouiCOM.EditText txWON, txCD, txSD, txDD, txShipD, txShipA, txCustFN, txCustLN, txCustE, txCustG, txCustCG, txStore, BAdd1, BAdd2, BCompany, SAdd1, SAdd2, SCompany;
        SAPbouiCOM.EditText  txSName, txSSPO, txSCity, txSState, txSZip, txSPhone, txSEmail;
        SAPbouiCOM.Button  btGet;
        SAPbouiCOM.StaticText lblStatus;
       
        SAPbouiCOM.DataTable  dtCard, dtHead, dtORDR;
        bool initiallizing = false;
        public override void CreateForm(SAPbouiCOM.Application SboApp, string strXml, SAPbobsCOM.Company cmp, string frmId)
        {
            base.CreateForm(SboApp, strXml, cmp, frmId);

            oForm.EnableMenu("1282", false);  // Add New Record
            oForm.EnableMenu("1288", false);  // Next Record
            oForm.EnableMenu("1289", false);  // Pevious Record
            oForm.EnableMenu("1290", false);  // First Record
            oForm.EnableMenu("1291", false);  // Last record 
            oForm.EnableMenu("1281", false);  // Find record 
            oForm.Settings.Enabled = false;
            InitiallizeForm();




        }
        public override void etAfterKeyDown(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterKeyDown(ref pVal, ref BubbleEvent);


        }
        public override void etAfterCmbSelect(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCmbSelect(ref pVal, ref BubbleEvent);
          
        }
        public override void etAfterClick(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);



            if (pVal.ItemUID == btGet.Item.UniqueID)
            {
                if (txWON.Value.ToString() == "")
                    return;

                getOrders();
            }

           


         
        }




        private void getOrders()
        {
            lblStatus.Caption = "Please wait while loading!!!";
           
            string mLogin;
            Mag.MagentoService mService;
            Program.LoginToMagento(out mLogin, out mService);



            var cpf = new Mag.complexFilter[1];

            cpf[0] = new Mag.complexFilter
            {
                key = "increment_id",
                value = new Mag.associativeEntity
                {
                    key = "eq",
                    value = Convert.ToString(dtHead.GetValue("WON", 0))
                }
            };

           
            var filters = new Mag.filters();
            filters.complex_filter = cpf;




         
            if (salesOrderEntityList != null) salesOrderEntityList.Clear();

            salesOrderEntityList = mService.salesOrderList(mLogin, filters).ToList();
            if (salesOrderEntityList.Count > 0)
            {
                addWebOrder(Convert.ToString(dtHead.GetValue("WON", 0)), mService);
                lblStatus.Caption = "Web Order Loaded.";


            }
            else
            {
                lblStatus.Caption = "Web Order Not Found";


            }

        }



        private void addWebOrder(string incrementId,Mag.MagentoService svc)
        {

            string mLogin = "";
            svc.PreAuthenticate = true;
            Program.LoginToMagento(out mLogin, out svc);
            Mag.salesOrderPaymentEntity pmt = new Mag.salesOrderPaymentEntity();
            Mag.shoppingCartPaymentEntity scpmt = new Mag.shoppingCartPaymentEntity();
            Mag.salesOrderEntity soe = new Mag.salesOrderEntity();

            Mag.shoppingCartInfoEntity cart = new Mag.shoppingCartInfoEntity();
            Mag.shoppingCartPaymentMethodEntity pmtmethod = new Mag.shoppingCartPaymentMethodEntity();
            soe = svc.salesOrderInfo(mLogin, incrementId);
            cart = svc.shoppingCartInfo(mLogin, Convert.ToInt32(soe.quote_id), soe.store_id);




            pmt = soe.payment;



            string WebID = soe.increment_id;
            DateTime utcDate = Convert.ToDateTime(soe.created_at);
            utcDate = DateTime.SpecifyKind(utcDate, DateTimeKind.Utc);
            DateTime localDT = utcDate.ToLocalTime();

            string OrderDate = localDT.ToString();

            string CCode = "";
            if (cart.customer_id != null)
            {
                CCode = cart.customer_id;
            }
            else
            {
                CCode = soe.customer_id;
            }

            string CTel1 = soe.shipping_address.telephone;
            string CMail = soe.customer_email;

            string Contact = soe.customer_firstname + " " + soe.customer_lastname;
            string CNTel = soe.billing_address.telephone;
            string CNEmail = soe.customer_email;

            string bName = soe.billing_address.firstname + " " + soe.billing_address.lastname;
            string bStreet = soe.billing_address.street;
            string bCity = soe.billing_address.city;
            string bState = soe.billing_address.region;
            string bZipCode = soe.billing_address.postcode;
            string bPhone = soe.billing_address.telephone == null ? "" : soe.billing_address.telephone;
            string bEmail = soe.customer_email;

            string sName = soe.shipping_address.firstname + " " + soe.shipping_address.lastname;
            string sStreet = soe.shipping_address.street;
            string sCity = soe.shipping_address.city;
            string sState = soe.shipping_address.region;
            string sPhone = soe.shipping_address.telephone == null ? "" : soe.shipping_address.telephone;
            string sZipCode = soe.shipping_address.postcode;
            string sEmail = soe.customer_email;
            string shipcharge = "Ship Charge";

            string sCompany = soe.shipping_address.company == null ? "" : soe.shipping_address.company;

            dtCard.Rows.Clear();
            dtCard.Rows.Add(1);
            if (CCode == null) CCode = "99999999";
            CCode = "W-" + CCode.PadLeft(8, '0');
          

            dtCard.SetValue("BName", 0, bName);
            dtCard.SetValue("BStPO", 0, bStreet);

            string[] streetArr = bStreet.Split('\n');
            string addr1B = "";
            string addr2B = "";
            if (streetArr.Length > 0) addr1B = streetArr[0].ToString();
            if (streetArr.Length > 1) addr2B = streetArr[1].ToString();


            dtCard.SetValue("BAdd1", 0, addr1B);
            dtCard.SetValue("BAdd2", 0,addr2B );
 //dtCard.SetValue("BCompany", 0, soe.com);

            dtCard.SetValue("BCity", 0, bCity);
            dtCard.SetValue("BState", 0, bState);
            dtCard.SetValue("BZip", 0, bZipCode);
            dtCard.SetValue("BTel", 0, bPhone);
            dtCard.SetValue("BEmail", 0, bEmail);

            dtCard.SetValue("SName", 0, sName);
            dtCard.SetValue("SStPO", 0, sStreet);

            streetArr = sStreet.Split('\n');
            string addr1S = "";
            string addr2S = "";
            if (streetArr.Length > 0) addr1S = streetArr[0].ToString();
            if (streetArr.Length > 1) addr2S = streetArr[1].ToString();
            dtCard.SetValue("SAdd1", 0, addr1S);
            dtCard.SetValue("SAdd2", 0, addr2S);

            dtCard.SetValue("SCity", 0, sCity);
            dtCard.SetValue("SZip", 0, sZipCode);
            dtCard.SetValue("STel", 0, sPhone);
            dtCard.SetValue("SEmail", 0, sEmail);
            dtCard.SetValue("SState", 0, sState);
            dtCard.SetValue("SComp", 0, sCompany);

            Mag.shoppingCartPaymentEntity cpmt = new Mag.shoppingCartPaymentEntity();

            cpmt = cart.payment;


            string numAtCard = soe.increment_id;
            string DiscCode = cart.coupon_code == null ? "" : cart.coupon_code;
            string BillingZip = soe.billing_address.postcode;
            string ordrsource = "W";
            string CCName = cart.payment.cc_owner == null ? "" : cart.payment.cc_owner; ; // soe.payment.cc_owner;
            string l4Digit = cart.payment.cc_last4 == null ? "" : cart.payment.cc_last4;
            string AuthNet = cart.payment.cc_ss_issue == null ? "" : cart.payment.cc_ss_issue;
            string ChargeAmount = soe.payment.amount_ordered;
            string MagentoId = soe.master_order_id;
            string TransDate = localDT.ToString();
            string WebOrderAt = localDT.ToString();
            string WebDiscP = cart.items[0].discount_percent.ToString();
            string WebDiscAmonut = soe.discount_amount;
            string CardExp = cart.payment.cc_exp_month + "/1/" + cart.payment.cc_exp_year;
            string CardType = cart.payment.cc_type;

            DateTime expDate = Convert.ToDateTime(CardExp);
            expDate = expDate.AddMonths(1).AddDays(-1);

            string slpCode = "Websales";
            dtORDR.Rows.Clear();
            dtORDR.Rows.Add(1);
            dtORDR.SetValue("CreateDate", 0, Convert.ToDateTime(TransDate).ToString("yyyyMMdd"));
            try
            {
                dtORDR.SetValue("ShipDate", 0, Convert.ToDateTime(soe.ship_date));
            }
            catch { dtORDR.SetValue("ShipDate", 0, Convert.ToDateTime(DateTime.Now.AddDays(1))); }
            try
            {
                dtORDR.SetValue("DelDate", 0, Convert.ToDateTime(soe.delivery_date));
            }
            catch { dtORDR.SetValue("DelDate", 0, Convert.ToDateTime(DateTime.Now.Date.AddDays(1))); }
            dtORDR.SetValue("ShipDescr", 0, soe.shipping_description);
            dtORDR.SetValue("ShipAmount", 0, soe.shipping_amount);
            dtORDR.SetValue("CustFName", 0, soe.customer_firstname);
            dtORDR.SetValue("CustLName", 0,soe.customer_lastname);
            dtORDR.SetValue("CustEmail", 0, soe.customer_email);
            dtORDR.SetValue("isGuest", 0, soe.customer_is_guest);
            dtORDR.SetValue("CustGroup", 0, soe.customer_group_id);
            dtORDR.SetValue("Store", 0, soe.store_id);


            dtHead.SetValue("ProdType", 0, soe.items[0].product_type);
            dtHead.SetValue("ProdOpt", 0, soe.items[0].product_options);
            DateTime shipDate = DateTime.Now.AddDays(1);
            DateTime DeliveryDate = DateTime.Now.AddDays(1);

            try { shipDate = Convert.ToDateTime(soe.ship_date); } catch { }

            try { DeliveryDate = Convert.ToDateTime(soe.delivery_date); } catch { }


            string statuscomment = "";
            string authTransId = "";
            if (cart.payment.method == "authorizenet")
            {
                foreach (Mag.salesOrderStatusHistoryEntity status in soe.status_history)
                {
                    if (status.comment == null) continue;
                    if (status.comment.Contains("Authorize.Net Transaction ID"))
                    {
                        statuscomment = status.comment;
                    }
                }

                int start = statuscomment.IndexOf('"');
                statuscomment = statuscomment.Substring(start + 1);
                int len = statuscomment.IndexOf('"');

                authTransId = statuscomment.Substring(0, len);

            }
          

            CCName = cart.payment.cc_ss_owner; // soe.payment.cc_owner;
            l4Digit = cart.payment.cc_last4;


            string strInsert = @"INSERT INTO [@B1_MORDR]
           ([Code]
           ,[Name]
           ,[U_CardCode]
           ,[U_Tel1]
           ,[U_Email]
           ,[U_Contact]
           ,[U_CNPhone]
           ,[U_CNEmail]
           ,[U_BName]
           ,[U_BStreet]
           ,[U_BCity]
           ,[U_BState]
           ,[U_BZip]
           ,[U_BPhone]
           ,[U_BEmail]
           ,[U_SName]
           ,[U_SStreet]
           ,[U_SCity]
           ,[U_SState]
           ,[U_SZip]
           ,[U_SPhone]
           ,[U_SEmail]
           ,[U_WEBID]
           ,[U_NumAtCard]
           ,[U_DiscCode]
           ,[U_CCName]
           ,[U_L4Dig]
           ,[U_AuthNetId]
           ,[U_CCAmnt]
           ,[U_MagentoID]
           ,[U_CCTDate]
           ,[U_CCExpDate]
           ,[U_CCType]
           ,[U_CCZip]
           ,[U_WODate]
           ,[U_WODiscP]
           ,[U_WODiscA]
           ,[U_DPMCreated]
           ,[U_WOSource]
           ,[U_Printed]
           ,[U_SlpCode]
           ,[U_SComp])
     VALUES
           (@Code
           ,@Name
           ,@U_CardCode
           ,@U_Tel1
           ,@U_Email
           ,@U_Contact
           ,@U_CNPhone
           ,@U_CNEmail
           ,@U_BName
           ,@U_BStreet
           ,@U_BCity
           ,@U_BState
           ,@U_BZip
           ,@U_BPhone
           ,@U_BEmail
           ,@U_SName
           ,@U_SStreet
           ,@U_SCity
           ,@U_SState
           ,@U_SZip
           ,@U_SPhone
           ,@U_SEmail
           ,@U_WEBID
           ,@U_NumAtCard
           ,@U_DiscCode
           ,@U_CCName
           ,@U_L4Dig
           ,@U_AuthNetId
           ,@U_CCAmnt
           ,@U_MagentoID
           ,@U_CCTDate
           ,@U_CCExpDate
           ,@U_CCType
           ,@U_CCZip
           ,@U_WODate
           ,@U_WODiscP
           ,@U_WODiscA
           ,@U_DPMCreated
           ,@U_WOSource
           ,@U_Printed
           ,@U_SlpCode
           ,@U_SComp )";

            Hashtable hsp = new Hashtable();
            hsp.Add("@Code", incrementId);
            hsp.Add("@Name", incrementId);
            hsp.Add("@U_CardCode", CCode);
            hsp.Add("@U_Tel1", CTel1 == null ? "" : CTel1);
            hsp.Add("@U_Email", CMail == null ? "" : CMail);
            hsp.Add("@U_Contact", Contact);
            hsp.Add("@U_CNPhone", CNTel == null ? "" : CNTel);
            hsp.Add("@U_CNEmail", CNEmail == null ? "" : CNEmail);
            hsp.Add("@U_BName", bName == null ? "" : bName);
            hsp.Add("@U_BStreet", bStreet == null ? "" : bStreet);
            hsp.Add("@U_BCity", bCity == null ? "" : bCity);
            hsp.Add("@U_BState", bState == null ? "" : bState);
            hsp.Add("@U_BZip", bZipCode == null ? "" : bZipCode);
            hsp.Add("@U_BPhone", bPhone == null ? "" : bPhone);
            hsp.Add("@U_BEmail", bEmail == null ? "" : bEmail);
            hsp.Add("@U_SName", sName == null ? "" : sName);
            hsp.Add("@U_SStreet", sStreet == null ? "" : sStreet);
            hsp.Add("@U_SCity", sCity == null ? "" : sCity);
            hsp.Add("@U_SState", sState == null ? "" : sState);
            hsp.Add("@U_SZip", sZipCode == null ? "" : sZipCode);
            hsp.Add("@U_SPhone", sPhone == null ? "" : sPhone);
            hsp.Add("@U_SEmail", sEmail == null ? "" : sEmail);
            hsp.Add("@U_WEBID", incrementId);
            hsp.Add("@U_NumAtCard", incrementId);
            hsp.Add("@U_DiscCode", DiscCode == null ? "" : DiscCode);
            hsp.Add("@U_CCName", bName == null ? "" : bName);
            hsp.Add("@U_L4Dig", l4Digit == null ? "" : l4Digit);
            hsp.Add("@U_AuthNetId", authTransId == null ? "" : authTransId);
            hsp.Add("@U_CCAmnt", ChargeAmount);
            hsp.Add("@U_MagentoID", soe.master_order_id == null ? "" : soe.master_order_id);
            hsp.Add("@U_CCTDate", Convert.ToDateTime(TransDate).ToShortDateString());
            hsp.Add("@U_CCExpDate", expDate.Date.ToShortDateString());
            hsp.Add("@U_CCType", CardType == null ? "" : CardType);
            hsp.Add("@U_CCZip", BillingZip == null ? "" : BillingZip);
            hsp.Add("@U_WODate", Convert.ToDateTime(WebOrderAt).ToShortDateString());
            hsp.Add("@U_WODiscP", WebDiscP);
            hsp.Add("@U_WODiscA", WebDiscAmonut);
            hsp.Add("@U_DPMCreated", "N");
            hsp.Add("@U_WOSource", "W");
            hsp.Add("@U_Printed", "Y");
            hsp.Add("@U_SlpCode", slpCode);
            hsp.Add("@U_SComp", sCompany);

            string orderREsult = Program.objHrmsUI.ExecQuery(strInsert, hsp, "addORDR");

            int i = 0;

            bool shippingApplied = false;
           
            foreach (Mag.salesOrderItemEntity item in soe.items)
            {
              
               


               
                string stpo = soe.shipping_address.street;
                string[] street = soe.shipping_address.street.Split('\n');
                string addr1 = "";
                string addr2 = "";
                if (street.Length > 0) addr1 = street[0].ToString();
                if (street.Length > 1) addr2 = street[1].ToString();

                string stpo2 = soe.shipping_address.street;

                string strStateCode = StateCode(soe.shipping_address.region);
               

                hsp.Clear();
                hsp.Add("@Code", incrementId + "_" + i.ToString());
                hsp.Add("@Name", incrementId + "_" + i.ToString());
                hsp.Add("@U_ItemCode", item.sku);
                hsp.Add("@U_ItemName", item.name);
                hsp.Add("@U_Quantity", item.qty_ordered);
                hsp.Add("@U_Price", item.price);
                hsp.Add("@U_DiscP", item.discount_percent);
                hsp.Add("@U_ShipType", soe.shipping_description);
                hsp.Add("@U_ShipToName", sName);
                hsp.Add("@U_ShiptoImp", sName);
                hsp.Add("@U_CmpImp", sCompany);
                hsp.Add("@U_Addr1", addr1);
                hsp.Add("@U_Addr2", addr2);
                hsp.Add("@U_City", soe.shipping_address.city);
                hsp.Add("@U_State", strStateCode);
                hsp.Add("@U_Zipcode", sZipCode);
                hsp.Add("@U_Phone", sPhone);
                hsp.Add("@U_Email", sEmail);
                hsp.Add("@U_ArrDate", Convert.ToDateTime(DeliveryDate));
                hsp.Add("@U_ShipDate", Convert.ToDateTime(shipDate));

                hsp.Add("@U_GiftCardMsg", item.gift_message == null ? "" : item.gift_message.ToString());
                hsp.Add("@U_ShiptoID", sName);
                string Freight1 = "";
                string freightA = "";
                if (!shippingApplied && Convert.ToDouble(soe.shipping_amount) > 0)
                {
                    Freight1 = shipcharge;
                    freightA = soe.shipping_amount;
                    shippingApplied = true;
                }
                hsp.Add("@U_Freight", Freight1);
                hsp.Add("@U_FrAmt", freightA);
                hsp.Add("@U_ItemId", item.item_id);
                hsp.Add("@U_WEBID", soe.increment_id);
                hsp.Add("@U_SComp", sCompany);

                i++;

                string strRdr1Insert = @" INSERT INTO [@B1_MRDR1]
           ([Code]
           ,[Name]
           ,[U_ItemCode]
           ,[U_ItemName]
           ,[U_Quantity]
           ,[U_Price]
           ,[U_DiscP]
           ,[U_ShipType]
           ,[U_ShipToName]
           ,[U_ShiptoImp]
           ,[U_CmpImp]
           ,[U_Addr1]
           ,[U_Addr2]
           ,[U_City]
           ,[U_State]
           ,[U_Zipcode]
           ,[U_Phone]
           ,[U_Email]
           ,[U_ArrDate]
           ,[U_GiftCardMsg]
           ,[U_ShiptoID]
           ,[U_Freight]
           ,[U_FreightAmt]
           ,[U_SComp]
           ,[U_ItemId]
           ,[U_WEBID]
            ,[U_ShipDate])
     VALUES
           (@Code
           ,@Name
           ,@U_ItemCode
           ,@U_ItemName
           ,@U_Quantity
           ,@U_Price
           ,@U_DiscP
           ,@U_ShipType
           ,@U_ShipToName
           ,@U_ShiptoImp
           ,@U_CmpImp
           ,@U_Addr1
           ,@U_Addr2
           ,@U_City
           ,@U_State
           ,@U_Zipcode
           ,@U_Phone
           ,@U_Email
           ,@U_ArrDate
           ,@U_GiftCardMsg
           ,@U_ShiptoID
           ,@U_Freight
           ,@U_FrAmt
           ,@U_SComp
           ,@U_ItemId
           ,@U_WEBID
            ,@U_ShipDate)";


                string detailResult = Program.objHrmsUI.ExecQuery(strRdr1Insert, hsp, "Add Row");

            }




        }


        private string getDocNum(string orderId, string tblName)
        {
            string strResult = "";
            string strSql = "Select DocEntry from [" + tblName + "] where U_PRX_SID = '" + orderId + "'";

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
            Program.isDemo = Program.objHrmsUI.settings["IsDemo"].ToString() == "Y" ? true : false;


            oForm.Freeze(true);

            initiallizing = true;


          
            lblStatus = (SAPbouiCOM.StaticText)oForm.Items.Item("lblStatus").Specific;
            dtORDR = oForm.DataSources.DataTables.Item("dtORDR");
            dtHead = oForm.DataSources.DataTables.Item("dtHead");
            dtCard = oForm.DataSources.DataTables.Item("dtCard");
            txWON = (SAPbouiCOM.EditText)oForm.Items.Item("txWON").Specific;


            txSName = (SAPbouiCOM.EditText)oForm.Items.Item("txSName").Specific;
            txSSPO = (SAPbouiCOM.EditText)oForm.Items.Item("txSSPO").Specific;
            txSCity = (SAPbouiCOM.EditText)oForm.Items.Item("txSCity").Specific;
            txSState = (SAPbouiCOM.EditText)oForm.Items.Item("txSState").Specific;
            txSZip = (SAPbouiCOM.EditText)oForm.Items.Item("txSZip").Specific;
            txSPhone = (SAPbouiCOM.EditText)oForm.Items.Item("txSPhone").Specific;
            txSEmail = (SAPbouiCOM.EditText)oForm.Items.Item("txSEmail").Specific;

           SCompany = (SAPbouiCOM.EditText)oForm.Items.Item("txSComp").Specific;
            SAdd2 = (SAPbouiCOM.EditText)oForm.Items.Item("txSAdd2").Specific;
            SAdd1 = (SAPbouiCOM.EditText)oForm.Items.Item("txSAdd1").Specific;
            BCompany = (SAPbouiCOM.EditText)oForm.Items.Item("txBComp").Specific;
            BAdd2 = (SAPbouiCOM.EditText)oForm.Items.Item("txBAdd2").Specific;
            BAdd1 = (SAPbouiCOM.EditText)oForm.Items.Item("txBAdd1").Specific;

            txDD = (SAPbouiCOM.EditText)oForm.Items.Item("txDD").Specific;
            txShipD = (SAPbouiCOM.EditText)oForm.Items.Item("txShipD").Specific;
            txShipA = (SAPbouiCOM.EditText)oForm.Items.Item("txShipA").Specific;
            txCustFN = (SAPbouiCOM.EditText)oForm.Items.Item("txCustFN").Specific;
            txCustLN = (SAPbouiCOM.EditText)oForm.Items.Item("txCustLN").Specific;
            txCustE = (SAPbouiCOM.EditText)oForm.Items.Item("txCustE").Specific;
            txCustG = (SAPbouiCOM.EditText)oForm.Items.Item("txCustG").Specific;
            txCustCG = (SAPbouiCOM.EditText)oForm.Items.Item("txCustCG").Specific;
            txStore = (SAPbouiCOM.EditText)oForm.Items.Item("txStore").Specific;
            txCD = (SAPbouiCOM.EditText)oForm.Items.Item("txCD").Specific;
            txSD = (SAPbouiCOM.EditText)oForm.Items.Item("txSD").Specific;

           btGet = (SAPbouiCOM.Button)oForm.Items.Item("btGet").Specific;

            ini_controls();

           oForm.Freeze(false);

            initiallizing = false;



        }

        private void ini_controls()
        {
           
            dtORDR.Rows.Clear();
            dtHead.Rows.Clear();
            dtCard.Rows.Clear();
           
            dtHead.Rows.Add(1);


        }

        private void clearOrderInfo()
        {
           
            dtORDR.Rows.Clear();
            dtCard.Rows.Clear();
          
            dtHead.Rows.Add(1);
            dtORDR.Rows.Add(1);
            dtCard.Rows.Add(1);

         
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
        private void GenerateAck()
        {
            ACHR.EDI.Classes.EDIPOAck POAck = new EDI.Classes.EDIPOAck();

           
        }
      
      
    }
}
