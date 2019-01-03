using System;
using System.Data;

using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;
using SAPbobsCOM;
using System.Diagnostics;
using System.Threading;

using System.Reflection;
using System.IO;
using System.Text;




namespace ACHR.Screen
{
    class frm_65211 : SysBaseForm
    {

        SAPbouiCOM.Button cmdUpdate;
        string UICreated = "N";
        SAPbouiCOM.DataTable dtRout;
        SAPbouiCOM.Matrix oMatrix;
        SAPbouiCOM.EditText txSupplier,txPostingDate,txEA,txPE,txRef;
        SAPbouiCOM.Button btPost;
       SAPbouiCOM.StaticText  lblCost, lblCard, lblPD, lblEA, lblPE,lblRef;
        SAPbouiCOM.ChooseFromList ocflCard;
        double currentProdCost = 0.00;
        DataServices dsWEB;
        DataServices dsSAP;

         SAPbouiCOM.DBDataSource OWOR ; 
              string oworEntry = ""; 
             

        #region /////Events
        public override void etFormAfterDataLoad(ref BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent)
        {
            base.etFormAfterDataLoad(ref BusinessObjectInfo, ref BubbleEvent);


            if (UICreated == "N")
            {
                createUI();
                UICreated = "Y";
            }

            SAPbouiCOM.Item oItemRef1 = oForm.Items.Item("58");
            SAPbouiCOM.EditText ItemCost = (SAPbouiCOM.EditText)oForm.Items.Item("41").Specific;
            SAPbouiCOM.EditText OWORDocNum = (SAPbouiCOM.EditText)oForm.Items.Item("18").Specific;
            string oworNum = OWORDocNum.Value.ToString();
            oworEntry = Convert.ToString(Program.objHrmsUI.getScallerValue("Select DocEntry from owor where docnum='" + oworNum + "'"));

            // ItemCost.Item.Enabled = true;

            string strCost = ItemCost.Value.ToString();
            if (strCost != "")
            {
                strCost = System.Text.RegularExpressions.Regex.Match(strCost, @"(\d*|\d{1,3}(,\d{3})*)(\.\d+)?\b$").Value; ;
                currentProdCost = Convert.ToDouble(strCost);
            

            }
            else
            {
                currentProdCost = 0;
                disableItems();
            }


           
            oForm.DataSources.UserDataSources.Item("txSupplier").ValueEx = "";
            oForm.DataSources.UserDataSources.Item("txRef").ValueEx = "";
            oForm.DataSources.UserDataSources.Item("txPD").ValueEx = "";
            oForm.DataSources.UserDataSources.Item("txPE").ValueEx = "";
            
            if (currentProdCost > 0)
            {
                string strSel = "Select TOP 1 U_CostAP  as U_CostAP  from OWOR where DocEntry = '" + oworEntry + "'";
                System.Data.DataTable CostAP = Program.objHrmsUI.getDataTable(strSel, "Getting CostAP");
                if (CostAP.Rows.Count > 0)
                {
                    string costAPEntry = CostAP.Rows[0]["U_CostAP"].ToString();
                    if (costAPEntry != "0" && costAPEntry != "")
                    {
                        oForm.DataSources.UserDataSources.Item("txPE").ValueEx = costAPEntry;
                        System.Data.DataTable dtCostAP = Program.objHrmsUI.getDataTable("Select CardCode,NumAtCard,DocDate from opch where docentry = '" + costAPEntry + "'", "AP");
                        foreach (DataRow dr in dtCostAP.Rows)
                        {
                            oForm.DataSources.UserDataSources.Item("txSupplier").ValueEx = dr["CardCode"].ToString();
                            oForm.DataSources.UserDataSources.Item("txRef").ValueEx = dr["NumAtCard"].ToString();
                            oForm.DataSources.UserDataSources.Item("txPD").ValueEx = Convert.ToDateTime(dr["DocDate"]).ToString("yyyyMMdd");

                        }

                        disableItems();
                    }
                    else
                    {
                        EnableItems();

                    }
                }
                else
                {
                    EnableItems();
                }
            }

            txPE.Item.Enabled = false;
        }

        private void disableItems()
        {
            ((SAPbouiCOM.EditText)oForm.Items.Item("3").Specific).Active = true;
            txEA.Item.Enabled = false;
            txSupplier.Item.Enabled = false;
            txPostingDate.Item.Enabled = false;
            txRef.Item.Enabled = false;
            btPost.Item.Enabled = false;
        }
        private void EnableItems()
        {
            txEA.Item.Enabled = true;
            txSupplier.Item.Enabled = true;
            txPostingDate.Item.Enabled = true;
            txRef.Item.Enabled = true;
            btPost.Item.Enabled = true;
        }
     
        public override void etFormAfterDataAdd(ref BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent)
        {
            base.etFormAfterDataAdd(ref BusinessObjectInfo, ref BubbleEvent);

            if (BusinessObjectInfo.ActionSuccess)
            {
                string newDocEntry = BusinessObjectInfo.ObjectKey;
                //<?xml version="1.0" encoding="UTF-16" ?><ProductionOrderParams><AbsoluteEntry>4413</AbsoluteEntry></ProductionOrderParams>
                //<?xml version="1.0" encoding="UTF-16" ?><DocumentParams><DocEntry>135</DocEntry></DocumentParams>
                try
                {
                    newDocEntry = newDocEntry.Substring(newDocEntry.IndexOf("<DocEntry>", 0) + "<DocEntry>".Length, newDocEntry.IndexOf("</DocEntry>", 0) - (newDocEntry.IndexOf("<DocEntry>", 0) + "<DocEntry>".Length));
                }
                catch { }

                try
                {
                    newDocEntry = newDocEntry.Substring(newDocEntry.IndexOf("<AbsoluteEntry>", 0) + "<AbsoluteEntry>".Length, newDocEntry.IndexOf("</AbsoluteEntry>", 0) - (newDocEntry.IndexOf("<AbsoluteEntry>", 0) + "<AbsoluteEntry>".Length));
                }
                catch { }




            }

        }
        public override void etFormAfterLoad(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etFormAfterLoad(ref pVal, ref BubbleEvent);
          
               oForm.DataSources.DBDataSources.Item("OWOR");


            InitiallizeForm();
         //   Program.objHrmsUI.loadSettings();
            
          //  oApplication.MessageBox("Project Form Loaded");
        }
        public override void etAfterClick(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == "btPost")
            {
                oForm.Mode = BoFormMode.fm_OK_MODE;
                postDocument(currentProdCost);
                oApplication.MessageBox("Document Posted");
            }
        }

        public override void etAfterCfl(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCfl(ref pVal, ref BubbleEvent);
            SAPbouiCOM.IChooseFromListEvent oCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pVal;
            SAPbouiCOM.DataTable dtSel = oCFLEvento.SelectedObjects;
            try
            {
                if (pVal.ItemUID == txSupplier.Item.UniqueID)
                {
                    if (dtSel != null && dtSel.Rows.Count > 0)
                    {
                        string strCode = dtSel.GetValue("CardCode", 0).ToString();
                        string strName = dtSel.GetValue("CardName", 0).ToString();
                        oForm.DataSources.UserDataSources.Item("txSupplier").ValueEx = strCode;
                        oForm.DataSources.UserDataSources.Item("txPD").ValueEx = DateTime.Now.Date.ToString("yyyyMMdd");

                        // oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE;
                    }
                }

            }
            catch { }

        }
       #endregion

        #region ///Initiallization
        public override void AddNewRecord()
        {
            base.AddNewRecord();
           
        }

       

        private void createUI()
        {
            int oldpanLevel = oForm.PaneLevel;
            if (oldpanLevel == 2) oForm.PaneLevel = 1;
            oForm.Freeze(true);



            SAPbouiCOM.Item oItem;
            SAPbouiCOM.Item oItem1;
            SAPbouiCOM.Folder oFolder;
            SAPbouiCOM.Item oItemRef = oForm.Items.Item("126");
          

            try
            {
                cflcardcode(ocflCard, "ocflCard");


                // add Routing Matrix

                oItem = oForm.Items.Add("lblCost", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top +30;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left;
                oItem.Width = oItemRef.Width;
                oItem.FromPane = 2;
                oItem.ToPane = 2;
                oItem.TextStyle = 4;

                Program.objHrmsUI.loadSettings();
                oItem.LinkTo = oItemRef.UniqueID;
                lblCost = (SAPbouiCOM.StaticText)oItem.Specific;
                lblCost.Caption = "Cost Posting";


                oItem = oForm.Items.Add("lblCard", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top + 50;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left;
                oItem.Width = oItemRef.Width;
                oItem.FromPane = 2;
                oItem.ToPane = 2;
                oItem.Visible = false;
                oItem.LinkTo = oItemRef.UniqueID;
                lblCard = (SAPbouiCOM.StaticText)oItem.Specific;
                lblCard.Caption = "Supplier";


                oItem = oForm.Items.Add("txSupplier", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                oItem.Top = oItemRef.Top + 50;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width;
                oItem.Width = 150;
                oItem.FromPane = 2;
                oItem.ToPane = 2;
                oItem.LinkTo = "lblCard";
                txSupplier = (SAPbouiCOM.EditText)oItem.Specific;

                oForm.DataSources.UserDataSources.Add("txSupplier", SAPbouiCOM.BoDataType.dt_SHORT_TEXT);
                txSupplier.DataBind.SetBound(true, "", "txSupplier");


                txSupplier.ChooseFromListUID = ocflCard.UniqueID;
                txSupplier.ChooseFromListAlias = "CardCode";
                oItem.Visible = false;

                txSupplier.ChooseFromListUID = ocflCard.UniqueID;
                txSupplier.ChooseFromListAlias = "CardCode";
            


                oItem = oForm.Items.Add("lnkCard", SAPbouiCOM.BoFormItemTypes.it_LINKED_BUTTON);
                oItem.Top = oItemRef.Top + 50;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width-20;
                oItem.FromPane = 2;
                oItem.ToPane = 2;
                oItem.LinkTo = "txSupplier";
                SAPbouiCOM.LinkedButton lnkCard = (SAPbouiCOM.LinkedButton)oItem.Specific;
                lnkCard.LinkedObject = BoLinkedObject.lf_BusinessPartner;
                lnkCard.LinkedObjectType = "2";

               



                oItemRef = lblCard.Item;

                oItem = oForm.Items.Add("lblRef", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top + 18;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left;
                oItem.Width = oItemRef.Width;
                oItem.FromPane = 2;
                oItem.ToPane = 2;
                oItem.Visible = false;
                oItem.LinkTo = oItemRef.UniqueID;
                lblRef = (SAPbouiCOM.StaticText)oItem.Specific;
                lblRef.Caption = "Ref #";


                oItem = oForm.Items.Add("txRef", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                oItem.Top = oItemRef.Top + 18;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width;
                oItem.Width = 150;
                oItem.FromPane = 2;
                oItem.ToPane = 2;
                oItem.Visible = false;
                oItem.LinkTo = oItemRef.UniqueID;
                txRef = (SAPbouiCOM.EditText)oItem.Specific;
                oForm.DataSources.UserDataSources.Add("txRef", SAPbouiCOM.BoDataType.dt_SHORT_TEXT);
                txRef.DataBind.SetBound(true, "", "txRef");


                oItemRef = lblRef.Item;

                oItem = oForm.Items.Add("lblPD", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top + 18;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left;
                oItem.Width = oItemRef.Width;
                oItem.FromPane = 2;
                oItem.ToPane = 2;
                oItem.Visible = false;
                oItem.LinkTo = oItemRef.UniqueID;
                lblPD = (SAPbouiCOM.StaticText)oItem.Specific;
                lblPD.Caption = "Posting Date";


                oItem = oForm.Items.Add("txPD", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                oItem.Top = oItemRef.Top + 18;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width;
                oItem.Width = 150;
                oItem.FromPane = 2;
                oItem.ToPane = 2;
                oItem.Visible = false;
                oItem.LinkTo = oItemRef.UniqueID;
                txPostingDate = (SAPbouiCOM.EditText)oItem.Specific;

                oForm.DataSources.UserDataSources.Add("txPD", SAPbouiCOM.BoDataType.dt_DATE);
                txPostingDate.DataBind.SetBound(true, "", "txPD");

                oItemRef = lblPD.Item;

                oItem = oForm.Items.Add("lblEA", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top + 18;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left;
                oItem.Width = oItemRef.Width;
                oItem.FromPane = 2;
                oItem.ToPane = 2;
                oItem.Visible = false;
                oItem.LinkTo = oItemRef.UniqueID;
                lblEA = (SAPbouiCOM.StaticText)oItem.Specific;
                lblEA.Caption = "Expense Account";


                oItem = oForm.Items.Add("txEA", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                oItem.Top = oItemRef.Top + 18;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width;
                oItem.Width = 150;
                oItem.FromPane = 2;
                oItem.ToPane = 2;
                oItem.Visible = false;
                oItem.LinkTo = oItemRef.UniqueID;
                txEA = (SAPbouiCOM.EditText)oItem.Specific;

                oForm.DataSources.UserDataSources.Add("txEA", SAPbouiCOM.BoDataType.dt_SHORT_TEXT);
                txEA.DataBind.SetBound(true, "", "txEA");
                txEA.Value = Program.objHrmsUI.getSetting("DfltExp");
              

                oItemRef = lblEA.Item;

                oItem = oForm.Items.Add("lblPE", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top + 18;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left;
                oItem.Width = oItemRef.Width;
                oItem.FromPane = 2;
                oItem.ToPane = 2;
                oItem.Visible = false;
                oItem.LinkTo = oItemRef.UniqueID;
                lblPE = (SAPbouiCOM.StaticText)oItem.Specific;
                lblPE.Caption = "Posted AP";
                lblPE.Item.Visible = false;

                oItem = oForm.Items.Add("txPE", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                oItem.Top = oItemRef.Top + 18;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width;
                oItem.Enabled = false;
                oItem.Width = 150;
                oItem.FromPane = 2;
                oItem.ToPane = 2;
                oItem.Visible = false;
                oItem.LinkTo = oItemRef.UniqueID;
                txPE = (SAPbouiCOM.EditText)oItem.Specific;
                oForm.DataSources.UserDataSources.Add("txPE", SAPbouiCOM.BoDataType.dt_SHORT_TEXT);
                txPE.DataBind.SetBound(true, "", "txPE");



                oItem = oForm.Items.Add("lnkPE", SAPbouiCOM.BoFormItemTypes.it_LINKED_BUTTON);
                oItem.Top = oItemRef.Top + 18;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width - 20;
                oItem.FromPane = 2;
                oItem.ToPane = 2;
                oItem.LinkTo = "txPE";
                SAPbouiCOM.LinkedButton lnkPE = (SAPbouiCOM.LinkedButton)oItem.Specific;
                lnkPE.LinkedObject = BoLinkedObject.lf_PurchaseInvoice;
                lnkPE.LinkedObjectType = "18";

               

                oItemRef = txPE.Item;


                oItem = oForm.Items.Add("btPost", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                oItem.Top = oItemRef.Top + 18;
                oItem.Height = oItemRef.Height + 5;
                oItem.Left = oItemRef.Left;
                oItem.Enabled = true;
                oItem.Width = 150;
                oItem.FromPane = 2;
                oItem.ToPane = 2;
                oItem.LinkTo = oItemRef.UniqueID; 
                btPost = (SAPbouiCOM.Button)oItem.Specific;
                btPost.Caption = "Post AP";
                btPost.Item.Visible = false;

                txPE.Item.Enabled = false;


            }
            catch (Exception ex)
            {

                string message = ex.Message;

            }

            oForm.Freeze(false);
            txPE.Item.Enabled = false;
            oForm.PaneLevel = oldpanLevel;
           
        }
        private void InitiallizeForm()
        {


            oForm.Freeze(true);
            oForm.Freeze(false);
           // hideAll();
           

            OWOR = oForm.DataSources.DBDataSources.Item("WOR1");

            dsSAP = new DataServices(Program.strConSAP);
            dsWEB = new DataServices(Program.strConWeb);



        }

        private void hideAll()
        {
            lblCost.Item.Visible = false;
            lblCard.Item.Visible = false;
            lblRef.Item.Visible = false;

            lblEA.Item.Visible = false;
            lblPD.Item.Visible = false;
            lblPE.Item.Visible = false;

            txSupplier.Item.Visible = false;
            txRef.Item.Visible = false;
            txPostingDate.Item.Visible = false;
            txPE.Item.Visible = false;
            txPE.Item.Enabled = false;
            txEA.Item.Visible = false;
        }

        #endregion

        #region //Common Methods
        
      


        private void cflcardcode(SAPbouiCOM.ChooseFromList oCFL, string uID)
        {

            try
            {

                SAPbouiCOM.ChooseFromListCollection oCFLs;
                SAPbouiCOM.Conditions oCons;
                SAPbouiCOM.Condition oCon;
                oCFLs = oForm.ChooseFromLists;

                SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams;

                oCFLCreationParams = (SAPbouiCOM.ChooseFromListCreationParams)oApplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams);


                oCFLCreationParams.MultiSelection = false;

                oCFLCreationParams.ObjectType = "2";

                oCFLCreationParams.UniqueID = uID;

                ocflCard = oCFLs.Add(oCFLCreationParams);


                oCons = ocflCard.GetConditions();

                oCon = oCons.Add();

                oCon.Alias = "CardType";

                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;

                oCon.CondVal = "S";

                ocflCard.SetConditions(oCons);



            }
            catch (Exception ex)
            {

                // MsgBox(Err.Description)

            }

        }

        #endregion


        public string postDocument(double amount)
        {

                       
            string outStr = "";
            try
            {

               
                SAPbobsCOM.Documents Doc = (SAPbobsCOM.Documents)Program.objHrmsUI.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices);
                SAPbobsCOM.BusinessPartners bp = (SAPbobsCOM.BusinessPartners)Program.objHrmsUI.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);

                SAPbouiCOM.EditText OWORDocNum = (SAPbouiCOM.EditText)oForm.Items.Item("18").Specific;
                string oworNum = OWORDocNum.Value.ToString();
                oworEntry = Convert.ToString(Program.objHrmsUI.getScallerValue("Select DocEntry from owor where docnum='" + oworNum + "'"));

                Doc.CardCode = txSupplier.Value.ToString();
               Doc.DocDate = DateTime.ParseExact(txPostingDate.Value.ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
              //  Doc.DocDueDate = Convert.ToDateTime(dtORDR.GetValue("CCDate", 0));
                Doc.NumAtCard = txRef.Value.ToString();
                Doc.DocType = BoDocumentTypes.dDocument_Service;

                Doc.Lines.AccountCode = txEA.Value.ToString();
                Doc.Lines.LineTotal = amount;
                Doc.Lines.Add();
                    

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
                       string strUpdate = "Update OWOR set U_CostAP = '" + outStr + "' WHERE DocEntry = '" + oworEntry + "'";
                        Program.objHrmsUI.ExecQuery(strUpdate,"Marking AP");

                        oApplication.StatusBar.SetText("Sales AP Posted Successfully for " , BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                        oApplication.Menus.Item("1304").Activate();
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

    }

}

