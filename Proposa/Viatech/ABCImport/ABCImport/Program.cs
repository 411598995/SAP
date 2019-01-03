using System;
using System.Collections;
using System.Xml;

using System.Collections.Generic;

using System.Net;

using System.Windows.Forms;
using SAPDI;
using System.Xml.Serialization;
using System.IO;
using ABCImport.MagiConnect_Sales;
using ABCommerce.ABCModals;

namespace ABCImport
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        public static Hashtable settings = new Hashtable();


        public static SAPDI.DIClass sboDI;
        public static SAPDI.DataServices ds;
        public static string email;
        public static string emailpwd;
        public static string smtpserver;
        public static string port;


        public static string db;
        public static string server;
        public static string serverType;
        public static string dbuser;
        public static string dbPwd;
        public static string sboId;
        public static string sboPwd;
        public static Encryption encriptor = new Encryption();

        [STAThread]
        static void Main()
        {

            try
            {

                email = getSetting("email");
                emailpwd = getSetting("emailpwd");
                smtpserver = getSetting("smtpserver");
                port = getSetting("port");

                db = getSetting("db");
                server = getSetting("server");
                serverType = getSetting("serverType");
                dbuser = getSetting("dbuser");
                dbPwd = encriptor.Decrypt(getSetting("dbPwd"));
                sboId = getSetting("sboId");
                sboPwd = encriptor.Decrypt(getSetting("sboPwd"));




                sboDI = new DIClass(db, sboId, sboPwd, dbuser, dbPwd, serverType, server);

                string conStat = sboDI.connectCompany();
               // MessageBox.Show(conStat);
                string sourceConstr = "Data Source=" + server + ";Initial Catalog=" + db + ";Integrated Security=False;Persist Security Info=False;User ID=" + dbuser + ";Password=" + dbPwd;

                ds = new DataServices(sourceConstr);
                loadSettings();
                //  MessageBox.Show("Connection Status : " + conStat);
                try
                {
                   getOrders();
                  // changetonewOrders();

                }
                catch (Exception ex)
                {
                   // MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
              //  MessageBox.Show(ex.Message);
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Exit();
            //Application.Run(new Form1());
        }

    

        public static void getOrders()
        {

            DataExtract result;

            string Login = Program.GetMagiConnectLogin();
            string Password = Program.GetMagiConnectPassword();
            Guid OrderStatus_NewOrder = new Guid(System.Configuration.ConfigurationManager.AppSettings["MagiConnectOrderStatus_NewOrder"]);
    
            Guid OrderStatus_Downloaded = new Guid("EAB0ED46-686F-4B57-8F16-124B03714F16");

            MagiConnect_Sales.sales SalesWebService = new MagiConnect_Sales.sales();
           string OrdersXML = SalesWebService.ExportOrders(Login, Password, OrderStatus_NewOrder);

            XmlSerializer serializer = new XmlSerializer(typeof(DataExtract));
            using (TextReader reader = new StringReader(OrdersXML))
            {
                result = (DataExtract)serializer.Deserialize(reader);

            }



           // MessageBox.Show("found orders" + result.Order.Count.ToString());
            foreach (DataExtractOrder weborder in result.Order)
            {
                bool isDownloaded = saveOrder(weborder.OrdNo, result);

                if (isDownloaded)
                {

                    string OrderTrackingReference = "";
                    string OrderStatusMessage = "Order Downloaded into SAP";

                    string Results = SalesWebService.UpdateOrderStatus(Login, Password,
                       new Guid(weborder.OrderID), OrderStatus_NewOrder, OrderStatus_Downloaded, OrderTrackingReference, OrderStatusMessage);

                }
            }


            postDocument();


          //  FillOrderMatrix();

        }
        public static string GetMagiConnectLogin()
        {
            return System.Configuration.ConfigurationManager.AppSettings["MagiConnectLogin"].ToString();
        }

        public static string GetMagiConnectPassword()
        {
            return System.Configuration.ConfigurationManager.AppSettings["MagiConnectPassword"].ToString();
        }

        public static string getSetting(string strSetting)
        {
            return System.Configuration.ConfigurationManager.AppSettings[strSetting].ToString();
        }
        public static  bool saveOrder(string orderNum, DataExtract result)
        {
            bool outresult = false;

            DataExtractOrder weborder = new DataExtractOrder();
            foreach (DataExtractOrder ordr in result.Order)
            {
                if (ordr.OrdNo == orderNum)
                {
                    weborder = ordr;
                    break;
                }
            }


            if (weborder.OrdNo == null || weborder.OrdNo == "") return false;

            

            DateTime shipDate = DateTime.Now.AddDays(1);
            DateTime DeliveryDate = DateTime.Now.AddDays(1);

            string WebID = weborder.OrderID;
            DateTime utcDate = Convert.ToDateTime(weborder.Date);
            utcDate = DateTime.SpecifyKind(utcDate, DateTimeKind.Utc);
            DateTime localDT = utcDate.ToLocalTime();

            string OrderDate = localDT.ToString();




            string strInsert = @"INSERT INTO [@B1_ABC_ORDR]
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


            string orderREsult = ds.ExecuteNonQuery(strInsert, hsp);
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
                                INSERT INTO [@B1_ABC_RDR1]
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


                string detailResult = ds.ExecuteNonQuery(strRdr1Insert, hsp);
                if (detailResult != "OK") return false;

            }



            return true;
        }

        public static void loadSettings()
        {
            settings.Clear();
            string strSetting = "Select * from  \"@B1_SETTING\" ";
            System.Data.DataTable dtSettings = ds.getDataTable(strSetting);
            foreach ( System.Data. DataRow dr in dtSettings.Rows)
            {
                try
                {
                    settings.Add(dr["Name"].ToString(), dr["U_Value"].ToString());
                }
                catch { }
            }
        }


        public static string postDocument()
        {
            string outStr = "";
            try
            {

                string strSOCandidate = "Select * from [@B1_ABC_ORDR] where isnull(U_SBOPosted,'N') = 'N'";
                System.Data.DataTable dtOrders = ds.getDataTable(strSOCandidate);
                if (dtOrders != null && dtOrders.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow drOrder in dtOrders.Rows)
                    {
                        try
                        {

                           // MessageBox.Show("Posting SAP Order for web order " + drOrder["U_OrdNo"].ToString());
                            SAPbobsCOM.Documents Doc = (SAPbobsCOM.Documents)sboDI.oDiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
                            SAPbobsCOM.BusinessPartners bp = (SAPbobsCOM.BusinessPartners)sboDI.oDiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);

                            string cardCode = settings["WebCardCode"].ToString();
                            string slpCode = settings["WebSlpCode"].ToString();
                            bool bpexist = bp.GetByKey(settings["WebCardCode"].ToString());
                            string Branch = settings["WebBranch"].ToString();

                            if (Branch != null && Branch != "")
                            {
                                Doc.BPL_IDAssignedToInvoice = Convert.ToInt32(Branch);
                            }
                            //  addBillTo(bp);
                            addShipTo(bp, drOrder);


                            Doc.CardCode = cardCode;
                            Doc.DocDate = Convert.ToDateTime(drOrder["U_Date"]);
                            Doc.DocDueDate = Convert.ToDateTime(drOrder["U_Date"]);
                            Doc.SalesPersonCode = Convert.ToInt16(slpCode);
                            Doc.PickRemark = drOrder["U_OrdNo"].ToString();
                            Doc.NumAtCard = drOrder["U_OrdNo"].ToString();
                            Doc.ShipToCode = "S-" + drOrder["U_ShipFirstName"].ToString() + " " + drOrder["U_ShipLastName"].ToString();
                            //  Doc.PayToCode = dtCard.GetValue("BName", 0).ToString();

                            string strRdr1 = "Select * from [@B1_ABC_RDR1] where  U_OrderID = '" + drOrder["Code"].ToString() + "'";
                            System.Data.DataTable dtRdr1 = ds.getDataTable(strRdr1);
                            if (dtRdr1 != null && dtRdr1.Rows.Count > 0)
                            {


                                foreach (System.Data.DataRow drRdr1 in dtRdr1.Rows)
                                {

                                    if (drRdr1["U_Code"].ToString() != "")
                                    {
                                        Doc.Lines.ItemCode = drRdr1["U_Code"].ToString();
                                        Doc.Lines.ItemDescription = drRdr1["U_Title"].ToString(); //rs.Fields.Item("ItemName").Value;
                                        Doc.Lines.Quantity = Convert.ToDouble(drRdr1["U_Qty"]);
                                        Doc.Lines.PriceAfterVAT = Convert.ToDouble(drRdr1["U_OrigPrice"]);
                                        Doc.Lines.DiscountPercent = Convert.ToDouble(drRdr1["U_PriceDisc"]);
                                        Doc.Lines.Add();
                                    }
                                }

                            }

                            try
                            {

                                if (Doc.Add() != 0)
                                {
                                    int erroCode = 0;
                                    string errDescr = "";
                                    sboDI.oDiCompany.GetLastError(out erroCode, out errDescr);
                                    outStr = "Error:" + errDescr;
                                    string strUpdate = "Update [@B1_ABC_ORDR] set  U_SBOPosted = 'N', U_SBOError='" + errDescr.Replace("'", "''") + "'   where Code = '" + drOrder["Code"].ToString() + "' ";
                                    ds.ExecuteNonQuery(strUpdate);

                                }
                                else
                                {
                                    outStr = Convert.ToString(sboDI.oDiCompany.GetNewObjectKey());

                                    string strUpdate = "Update [@B1_ABC_ORDR] set  U_SBOPosted = 'Y', U_SBOError='', U_SBOPostDT = getdate()   where Code = '" + drOrder["Code"].ToString() + "' ";
                                    ds.ExecuteNonQuery(strUpdate);

                                    addDownPayment(Convert.ToInt32(outStr));

                                }
                            }
                            catch (Exception ex)
                            {
                            }
                            finally
                            {


                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message);
            }
            return outStr;

        }

        private static void addShipTo(SAPbobsCOM.BusinessPartners bp , System.Data.DataRow drOrder)
        {
            int addressFound = 0;
            int addressCnt = 0;
            for (int i = 0; i < bp.Addresses.Count; i++)
            {
                bp.Addresses.SetCurrentLine(i);
                if (bp.Addresses.AddressName == "S-" + drOrder["U_ShipFirstName"].ToString() + " " + drOrder["U_ShipLastName"].ToString()  && bp.Addresses.AddressType == SAPbobsCOM.BoAddressType.bo_ShipTo)
                {

                    bp.Addresses.ZipCode = drOrder["U_ShipPostCode"].ToString();
                    bp.Addresses.AddressType = SAPbobsCOM.BoAddressType.bo_ShipTo;
                    bp.Addresses.Street = drOrder["U_ShipAdd1"].ToString();
                    bp.Addresses.City = drOrder["U_ShipAdd3"].ToString();
                    //bp.Addresses.TaxCode = "EX";

                    string statecode = StateCode(drOrder["U_ShipAdd4"].ToString());
                    if (statecode != "")
                    {
                        bp.Addresses.State = statecode;
                    }
                    addressFound = 1;
                }
                addressCnt++;

            }
            if (addressFound == 0)
            {
                bp.Addresses.Add();
                bp.Addresses.SetCurrentLine(addressCnt);

                bp.Addresses.AddressName = "S-" + drOrder["U_ShipFirstName"].ToString() + " " + drOrder["U_ShipLastName"].ToString();
                bp.Addresses.ZipCode = drOrder["U_ShipPostCode"].ToString();
                bp.Addresses.AddressType = SAPbobsCOM.BoAddressType.bo_ShipTo;
                bp.Addresses.Street = drOrder["U_ShipAdd1"].ToString();
                bp.Addresses.City = drOrder["U_ShipAdd3"].ToString();
                // bp.Addresses.TaxCode = "EX";

                string statecode = StateCode(drOrder["U_ShipAdd4"].ToString());
                if (statecode != "")
                {
                    bp.Addresses.State = statecode;
                }
                bp.Addresses.UserFields.Fields.Item("U_PhoneNum").Value = drOrder["U_ShipPhoneDay"].ToString(); ;// sTel == null ? "" : sTel;
                bp.Addresses.UserFields.Fields.Item("U_EMail").Value = drOrder["U_ShipEmail"].ToString(); // sEmail == null ? "" : sEmail;



            }

            int result = bp.Update();

            if (result != 0)
            {
                int erroCode = 0;
                string errDescr = "";
               
            }
            else
            {


                //  oApplication.StatusBar.SetText("Sales Order Posted Successfully for " + dtORDR.GetValue("WBID", 0), BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);

            }


        }

        private static string StateCode(string State)
        {
            string strResult = "";
            string strSql = "Select Top 1 Code from OCST where Name = '" + State + "' and country = 'US'";

            System.Data.DataTable dt = ds.getDataTable(strSql);
            if (dt != null && dt.Rows.Count > 0)
            {
                strResult = dt.Rows[0]["Code"].ToString();
            }

            return strResult;

        }
        public static void changetonewOrders()
        {

            DataExtract result;

            string Login = Program.GetMagiConnectLogin();
            string Password = Program.GetMagiConnectPassword();
            Guid OrderStatus_NewOrder = new Guid(System.Configuration.ConfigurationManager.AppSettings["MagiConnectOrderStatus_NewOrder"]);

            Guid OrderStatus_Downloaded = new Guid("EAB0ED46-686F-4B57-8F16-124B03714F16");

            MagiConnect_Sales.sales SalesWebService = new sales();


            string OrdersXML = SalesWebService.ExportOrders(Login, Password, OrderStatus_Downloaded);

            XmlSerializer serializer = new XmlSerializer(typeof(DataExtract));
            using (TextReader reader = new StringReader(OrdersXML))
            {
                result = (DataExtract)serializer.Deserialize(reader);

            }




            foreach (DataExtractOrder weborder in result.Order)
            {
              
                    string OrderTrackingReference = "";
                    string OrderStatusMessage = "Order Downloaded into SAP";

                    string Results = SalesWebService.UpdateOrderStatus(Login, Password,
                       new Guid(weborder.OrderID), OrderStatus_Downloaded , OrderStatus_NewOrder, OrderTrackingReference, OrderStatusMessage);

               
            }


            postDocument();


            //  FillOrderMatrix();

        }

        private static void addDownPayment(int docEntry)
        {

            bool Success = false;
            SAPbobsCOM.Documents ObjDownPayment;
            SAPbobsCOM.Documents objSO;
            objSO = (SAPbobsCOM.Documents)sboDI.oDiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
            objSO.GetByKey(docEntry);



            string PayMethod = objSO.UserFields.Fields.Item("U_B1_ABC_PAYMETHOD").Value.ToString();
            string PayRef = objSO.UserFields.Fields.Item("U_B1_ABC_PAYREF").Value.ToString();

            ObjDownPayment = (SAPbobsCOM.Documents)sboDI.oDiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDownPayments);
            ObjDownPayment.DownPaymentType = SAPbobsCOM.DownPaymentTypeEnum.dptInvoice;
            ObjDownPayment.CardCode = objSO.CardCode;
            ObjDownPayment.NumAtCard = objSO.NumAtCard;
            ObjDownPayment.DocDate = objSO.DocDate;
            ObjDownPayment.DocDueDate = objSO.DocDate;
            ObjDownPayment.NumAtCard = objSO.NumAtCard;
            ObjDownPayment.TrackingNumber = PayRef;
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
              sboDI.oDiCompany.GetLastError(out erroCode, out errDescr);
                errDescr = errDescr.Replace("'", "");

                if (errDescr.Contains("Closing Period"))
                {

                }
                else
                {


                }

            }
            else
            {
                Success = true;
                int dpmKey = Convert.ToInt32(sboDI.oDiCompany.GetNewObjectKey());

                SAPbobsCOM.Payments ObjPayment = (SAPbobsCOM.Payments)sboDI.oDiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments);
                ObjPayment.CardCode = objSO.CardCode;
                ObjPayment.DocDate = objSO.DocDate;
                ObjPayment.DocType = SAPbobsCOM.BoRcptTypes.rCustomer;
                ObjPayment.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_DownPayment;
                ObjPayment.Invoices.DocEntry = dpmKey;

                ObjPayment.Invoices.SumApplied = objSO.DocTotal;

                ObjPayment.Invoices.Add();

                ObjPayment.UserFields.Fields.Item("U_B1_ABC_PAYREF").Value = PayRef;
                ObjPayment.UserFields.Fields.Item("U_B1_ABC_PAYMETHOD").Value = PayMethod;
                ObjPayment.UserFields.Fields.Item("U_B1_ABC_WEBNUM").Value = objSO.NumAtCard;



                ObjPayment.CashSum = objSO.DocTotal;
                ObjPayment.CashAccount = getCashAcct(PayMethod);


                if (ObjPayment.Add() == 0)
                {
                    Success = true;
                }
                else
                {
                    Success = false;
                    int erroCode = 0;


                }

            }
        }
        private static string getCashAcct(string payType)
        {
            string result = "";
            string strSql = "Select U_GL from [@B1_ABC_PAYMETHOD] where [Name] = '" + payType + "'";

            try
            {
                result = Convert.ToString(ds.getScallerValue(strSql));
            }
            catch { }

            return result;
        }



    }
}
