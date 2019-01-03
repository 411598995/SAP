using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            if (pVal.ItemUID == "btSync")
            {
                SyncCompanyItem();
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

          

            
        }


        private bool saveOrder(string orderNum)
        {
          



            return true;
        }
        private void SyncCompanyItem()
        {
            string strSelect = "  SELECT T0.itemcode,T0.itemname,T0.FrgnName , T0.[PrchseItem], T0.[SellItem], T0.[InvntItem] , T0.[SuppCatNum] , T0.[SWW] ,t0.itmsgrpcod , t0.codebars ";
            strSelect += " , T0.[ManBtchNum], T0.[ManOutOnly], T0.[ManSerNum] ";
            strSelect += " FROM " + Program.LTDDB + ".dbo.OITM T0 INNER JOIN " + Program.LTDDB + ".dbo.OITB T1 ON T0.ItmsGrpCod = T1.ItmsGrpCod";
            strSelect += " WHERE ItemCode not in (SELECT ItemCode FROM OITM) ";
           // oApplication.SetStatusBarMessage(strSelect);
           System.Data. DataTable dtNewItem = Program.dsINC.getDataTable(strSelect);
            foreach ( System.Data.DataRow  dr in dtNewItem.Rows)
            {
                oApplication.SetStatusBarMessage("Transfereing " + dr["ItemCode"].ToString(), BoMessageTime.bmt_Short, false);
                SAPbobsCOM.Items newRetItem;
                newRetItem = (SAPbobsCOM.Items)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems);
                newRetItem.ItemCode = dr["itemcode"].ToString();
                newRetItem.ItemName = dr["itemname"].ToString();
                newRetItem.ForeignName = dr["FrgnName"].ToString();

                // newRetItem.ItemsGroupCode = Convert.ToInt32(dr["itmsgrpcod"]);
                newRetItem.ItemsGroupCode = 100;
                newRetItem.PurchaseItem = dr["PrchseItem"].ToString() =="Y"? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
                newRetItem.SalesItem = dr["SellItem"].ToString() == "Y" ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
                newRetItem.InventoryItem = dr["InvntItem"].ToString() == "Y" ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
                newRetItem.ManageBatchNumbers = dr["ManBtchNum"].ToString() == "Y" ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
                newRetItem.ManageSerialNumbers = dr["ManSerNum"].ToString() == "Y" ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
                newRetItem.ManageSerialNumbersOnReleaseOnly = dr["ManOutOnly"].ToString() == "Y" ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;

                newRetItem.SupplierCatalogNo = dr["SuppCatNum"].ToString();
                newRetItem.SWW =  dr["SWW"].ToString();

                newRetItem.BarCode = dr["codebars"].ToString();
                if (newRetItem.Add() != 0)
                {
                    int erroCode = 0;
                    string errDescr = "";
                    string Errmsg = "";
                    oCompany.GetLastError(out erroCode, out errDescr);
                    oApplication.SetStatusBarMessage(errDescr);
                }

            }
            oApplication.SetStatusBarMessage("Transfer Completed ", BoMessageTime.bmt_Short, false);



        }


        private void getOrders()
        {
          


            FillOrderMatrix();

        }

        private void FillOrderMatrix()
        {
           

            oForm.Freeze(true);
            dtWebO.Rows.Clear();
            int i = 0;
            DateTime OrdersSinceCreateDate = Convert.ToDateTime(dtHead.GetValue("From", 0));
            DateTime OrdersToCreateDate = Convert.ToDateTime(dtHead.GetValue("To", 0));
            string LTDINCCard = Program.objHrmsUI.getSetting("INCCODE").ToString();

            string strSql = "Select DocEntry, Convert(varchar(30),DocDate,101) as DocDate,OINV.DocNum from OINV where OINV.CardCode = '" + LTDINCCard  + "' And  DocDate between '" + OrdersSinceCreateDate.ToString("yyyyMMdd") + "' and '" + OrdersToCreateDate.ToString("yyyyMMdd") + "' order by DocNum";

            System.Data.DataTable dtOrders = Program.dsLTD. getDataTable(strSql, "gettingOrders");

            string selStatus = cbOS.Selected.Value.ToString();
            foreach ( System.Data.DataRow  dr in dtOrders.Rows)
            {
                string orderStatus = "O";
                string strId = dr["DocEntry"].ToString();

                string pchEntry = getDocNum("LTD Inv # " + dr["DocNum"].ToString(), "OPOR");

                if (pchEntry != "" && pchEntry != "0") orderStatus = "C";

                if (selStatus == "01" || (selStatus == "02" && orderStatus == "O") || (selStatus == "03" && orderStatus == "C"))
                {
                    dtWebO.Rows.Add(1);

                    dtWebO.SetValue("Id", i, (i + 1).ToString());
                    dtWebO.SetValue("WebNum", i, dr["DocNum"].ToString());
                    dtWebO.SetValue("RdrNum", i, pchEntry);
                  
                    dtWebO.SetValue("RdrDT", i, dr["DocDate"].ToString());

               
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

        private void addWebOrder(string InvNum)
        {

            string strDetail = "exec TmSp_DocLineTypeLayout '851',17";



            string strDlnSelect = "Select top 1  T0.DocEntry,t1.Comments,T1.DocTotalFC, T1.DocNum, T1.DocDate , T1.VatSumFC,T1.DiscPrcnt DocDisc,T0.UomCode, T0.ItemCode, OITM.ItemName, T0.Quantity, T0.Price,t0.VatGroup, T0.DiscPrcnt, T0.TotalFrgn as LineTotal from INV1 t0 inner join OINV  T1 on t0.docentry = t1.docentry inner join oitm on t0.itemcode = oitm.itemcode where t1.DocNum = '" + InvNum + "'";
            System.Data.DataTable dtDln = Program.dsLTD .getDataTable(strDlnSelect, "Shipment");

            int i = 0;
            double grossTotal = 0.00;
            double discPrcnt = 0.00;
            dtRDR1.Rows.Clear();
            foreach (System.Data.DataRow dr in dtDln.Rows)
            {
                if (i == 0)
                {
                    dtHead.SetValue("InvDate", 0, Convert.ToDateTime( dr["DocDate"]));
                    dtHead.SetValue("InvNum", 0, dr["DocNum"].ToString());
                    dtHead.SetValue("TV", 0, dr["VatSumFC"].ToString());
                    dtHead.SetValue("DP", 0, dr["DocDisc"].ToString());
                    discPrcnt = Convert.ToDouble(dr["DocDisc"]);
                    dtHead.SetValue("REM", 0,  dr["Comments"].ToString());
                    dtHead.SetValue("Total", 0, dr["DocTotalFC"].ToString());
                    strDetail = "exec TmSp_DocLineTypeLayout '" + dr["DocEntry"].ToString() + "',13";
                    //  dtHead.SetValue("DA", 0, dr["VatSum"].ToString());
                }
               

            }

            System.Data.DataTable dtRows = Program.dsLTD .getDataTable(strDetail, "getting Detail");

            foreach (System.Data.DataRow dr in dtRows.Rows)
            {
                // string ItemCode = dr["ItemCode"].ToString();

                dtRDR1.Rows.Add(1);
                dtRDR1.SetValue("Id", i, (i + 1).ToString());
                string lineType = dr["LineType"].ToString();
                string visOrder = dr["LineNum"].ToString();
                dtRDR1.SetValue("LT", i, lineType);

                if (lineType == "T")
                {
                    dtRDR1.SetValue("ItemName", i, dr["LineText"].ToString());

                }
                else
                {
                    string strInvRegRow = "Select top 1  T0.DocEntry,t1.Comments,T1.DocTotal, T1.DocNum, T1.DocDate , T1.VatSum,T1.DiscPrcnt DocDisc,T0.UomCode, T0.ItemCode, OITM.ItemName, T0.Quantity, T0.PriceBefDi,t0.VatGroup, T0.DiscPrcnt, T0.TotalFrgn from INV1 t0 inner join OINV  T1 on t0.docentry = t1.docentry inner join oitm on t0.itemcode = oitm.itemcode where t1.DocNum = '" + InvNum + "' and t0.VisOrder = '" + visOrder + "'";
                    System.Data.DataTable dtInvRegRow = Program.dsLTD.getDataTable(strInvRegRow, "Shipment");

                    foreach (System.Data.DataRow drReg in dtInvRegRow.Rows)
                    {
                        dtRDR1.SetValue("ItemCode", i, drReg["ItemCode"].ToString());
                        dtRDR1.SetValue("ItemName", i, drReg["ItemName"].ToString());
                        dtRDR1.SetValue("Qty", i, drReg["Quantity"].ToString());
                        dtRDR1.SetValue("Price", i, drReg["PriceBefDi"].ToString());
                        dtRDR1.SetValue("DiscP", i, drReg["DiscPrcnt"].ToString());
                        dtRDR1.SetValue("LineTotal", i, drReg["TotalFrgn"].ToString());
                        dtRDR1.SetValue("VAT", i, drReg["VatGroup"].ToString());
                        dtRDR1.SetValue("UOM", i, drReg["UomCode"].ToString());
                        grossTotal += Convert.ToDouble(drReg["TotalFrgn"]);
                    }
                }

                i++;

            }
            dtHead.SetValue("TBD", 0, grossTotal.ToString());
          
            dtHead.SetValue("DA", 0, Convert.ToString( grossTotal * discPrcnt /100) );
        
            mtOD.LoadFromDataSource();

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

            txFrom = (SAPbouiCOM.EditText)oForm.Items.Item("txFrom").Specific;
            txTo = (SAPbouiCOM.EditText)oForm.Items.Item("txTo").Specific;



            txCTel = (SAPbouiCOM.EditText)oForm.Items.Item("txCTel").Specific;
            txCCode = (SAPbouiCOM.EditText)oForm.Items.Item("txCCode").Specific;
           
            cbOS = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbOS").Specific;
            btPost = (SAPbouiCOM.Button)oForm.Items.Item("btPost").Specific;
            btGet =  (SAPbouiCOM.Button)oForm.Items.Item("btGet").Specific;
            //   ini_controls();

            dtHead.Rows.Add(1);

            string INCCard = Program.objHrmsUI.getSetting("LTDCODE").ToString();

            dtHead.SetValue("CardCode", 0, INCCard);
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

                    string statecode = StateCode(dtCard.GetValue("SState", 0).ToString());
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

                bp.Addresses.AddressName = "S-" + dtCard.GetValue("SName", 0);
                bp.Addresses.ZipCode = dtCard.GetValue("SZip", 0).ToString();
                bp.Addresses.AddressType = SAPbobsCOM.BoAddressType.bo_ShipTo;
                bp.Addresses.Street = dtCard.GetValue("SStPO", 0).ToString();
                bp.Addresses.City = dtCard.GetValue("SCity", 0).ToString();
               // bp.Addresses.TaxCode = "EX";

                string statecode = StateCode(dtCard.GetValue("SState", 0).ToString());
                if (statecode != "")
                {
                    bp.Addresses.State = statecode;
                }
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
            string TaxCode = Program.objHrmsUI.getSetting("TAX").ToString();
            try
            {

                if (dtORDR.Rows.Count == 0)
                    return "No Items";

                SAPbobsCOM.Documents Doc = (SAPbobsCOM.Documents)Program.objHrmsUI.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders);
                SAPbobsCOM.BusinessPartners bp = (SAPbobsCOM.BusinessPartners)Program.objHrmsUI.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
               
                bool bpexist = bp.GetByKey(dtHead.GetValue("CardCode", 0).ToString());
               
               
                Doc.CardCode = dtHead.GetValue("CardCode", 0).ToString();
                Doc.DocDate =Convert.ToDateTime(dtHead.GetValue("InvDate", 0));
                Doc.DocDueDate = Convert.ToDateTime(dtHead.GetValue("InvDate", 0));
               // Doc.SalesPersonCode = Convert.ToInt16( Program.objHrmsUI.settings["WebSlpCode"].ToString());
                Doc.PickRemark = "LTD A/R Invoice " + dtHead.GetValue("InvNum", 0).ToString(); // dtORDR.GetValue("NumAtCard", 0).ToString();
                Doc.NumAtCard = "LTD Inv # " +  dtHead.GetValue("InvNum", 0).ToString();
                Doc.Comments = dtHead.GetValue("REM", 0).ToString();
                Doc.DiscountPercent = Convert.ToDouble(dtHead.GetValue("DP", 0));
                int ItemLineNum = -1;
                for (int i = 0; i < dtRDR1.Rows.Count; i++)
                {

                    if (dtRDR1.GetValue("ItemCode", i).ToString() != "")
                    {
                        Doc.Lines.ItemCode = dtRDR1.GetValue("ItemCode", i).ToString();
                        Doc.Lines.ItemDescription = dtRDR1.GetValue("ItemName", i).ToString(); //rs.Fields.Item("ItemName").Value;
                        Doc.Lines.Quantity = Convert.ToDouble(dtRDR1.GetValue("Qty", i));
                        Doc.Lines.UnitPrice = Convert.ToDouble(dtRDR1.GetValue("Price", i));
                        Doc.Lines.DiscountPercent = Convert.ToDouble(dtRDR1.GetValue("DiscP", i));
                        Doc.Lines.UoMEntry = getUomEntry(dtRDR1.GetValue("UOM", i).ToString());
                        // Doc.Lines.VatGroup = TaxCode.ToString();
                        Doc.Lines.TaxCode = TaxCode;
                        Doc.Lines.Add();
                        ItemLineNum++;
                    }
                    else
                    {
                        Doc.SpecialLines.LineType = SAPbobsCOM.BoDocSpecialLineType.dslt_Text;
                        Doc.SpecialLines.LineText = dtRDR1.GetValue("ItemName", i).ToString();
                        Doc.SpecialLines.AfterLineNumber = ItemLineNum;
                        Doc.SpecialLines.Add();
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

        private int getUomEntry(string uomCode)
        {
            int uomEntr = -1;
            System.Data.DataTable dt =  Program.dsINC.getDataTable ("  select UOMCode, UomEntry from ouom where UOMCode = '" + uomCode + "' ", "GetUOM");
            foreach (System.Data.DataRow dr in dt.Rows)
            {

                try
                {
                    uomEntr = Convert.ToInt32(dr["UomEntry"].ToString());
                }
                catch { }
            }

            return uomEntr;
        }
     
        private string getCashAcct(string payType)
       {
           string result = "";
           string strSql = "Select U_GL from [@B1_ABC_PAYMETHOD] where [Name] = '" + payType + "'";

           try
           {
               result = Convert.ToString( Program.objHrmsUI.getScallerValue(strSql));
           }
           catch { }

           return result;
       }


    }
}
