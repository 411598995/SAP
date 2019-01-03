using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;
using SAPbobsCOM;



namespace ACHR.Screen
{
    class frm_JEM : HRMSBaseForm
    {

       
        SAPbouiCOM.Item IcbOr, itxCJ,itxPJ, ItxDN, ItxDD, ItxRem, ItxRef1, ItxRef2, ItxDS, ItxCS, IbtPost, IchkPost, ioptPP,ioptPC,ioptC;

        SAPbouiCOM.EditText txDN, txDD ,txRem ,txRef1 ,txRef2 , txDS ,txCS , txCJ,txPJ;
        SAPbouiCOM.Button btPost;
        SAPbouiCOM.LinkedButton lnkBG;
        SAPbouiCOM.ComboBox cbOr;
        SAPbouiCOM.Matrix mtJE, mtDet;
        SAPbouiCOM.DataTable dtDet, dtHead;
        SAPbouiCOM.CheckBox chkPost;
        SAPbouiCOM.OptionBtn optPP, optPC, optC;

        System.Data.DataTable dtPendingJEs = new System.Data.DataTable();
        #region /////Events

        public override void etAfterClick(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);

            if (pVal.ItemUID == "1")
            {

               
            }
            if (pVal.ItemUID == "btPost")
            {
                postJe();
            }
            if (pVal.ItemUID == "mtJE")
            {
                if (pVal.Row <= mtJE.RowCount && pVal.Row>0)
                {
                    SAPbouiCOM.EditText InvNum = mtJE.Columns.Item("#").Cells.Item(pVal.Row).Specific;
                    fillJeDetailHead(InvNum.Value.ToString(), pVal.Row);
                               
                }
            }
            if (pVal.ItemUID == "lnkBG")
            {
                oApplication.OpenForm(BoFormObjectEnum.fo_UserDefinedObject, "ABGP", txDN.Value.ToString());
            }
            if (pVal.ItemUID == "optPP" || pVal.ItemUID == "optPC" || pVal.ItemUID == "optC")
            {
                fillJeGrid();
                fillJeDetailHead("0" , 0);
            }
        }
        public override void etAfterCmbSelect(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCmbSelect(ref pVal, ref BubbleEvent);
            fillJeGrid();
        }
         public override void etBeforeMtLinkPressed(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etBeforeMtLinkPressed(ref pVal, ref BubbleEvent);
            oApplication.MessageBox("Linked Pressed");
        }
        
        
      
        #endregion

        #region ///Initiallization

        private void InitiallizeForm()
        {
            //  dtHead = oForm.DataSources.DataTables.Item("dtHead");
            // dtHead.Rows.Add(1);

            dtPendingJEs.Columns.Add("V#");
            dtPendingJEs.Columns.Add("Date");
            dtPendingJEs.Columns.Add("Remarks");
            dtPendingJEs.Columns.Add("Month");
            dtPendingJEs.Columns.Add("Year");
            dtPendingJEs.Columns.Add("Posted");
            dtPendingJEs.Columns.Add("PostedJE");
            dtPendingJEs.Columns.Add("Canceled");
            dtPendingJEs.Columns.Add("CanceledJE");

            oForm.Freeze(true);
            mtJE = oForm.Items.Item("mtJE").Specific;
            mtDet = oForm.Items.Item("mtDet").Specific;
            dtDet = oForm.DataSources.DataTables.Item("dtDet");
            dtHead = oForm.DataSources.DataTables.Item("dtHead");
            cbOr = oForm.Items.Item("cbOr").Specific;
            IcbOr = oForm.Items.Item("cbOr");
            cbOr.ValidValues.Add("All", "All");
            fillUserCb();

            oForm.DataSources.UserDataSources.Add("txDN", SAPbouiCOM.BoDataType.dt_LONG_NUMBER, 15); // Days of Month
            txDN = oForm.Items.Item("txDN").Specific;
            ItxDN = oForm.Items.Item("txDN");
            txDN.DataBind.SetBound(true, "", "txDN");


            oForm.DataSources.UserDataSources.Add("txCJ", SAPbouiCOM.BoDataType.dt_LONG_NUMBER, 15); // Days of Month
            txCJ = oForm.Items.Item("txCJ").Specific;
            itxCJ = oForm.Items.Item("txCJ");
            txCJ.DataBind.SetBound(true, "", "txCJ");


            oForm.DataSources.UserDataSources.Add("txPJ", SAPbouiCOM.BoDataType.dt_LONG_NUMBER, 15); // Days of Month
            txPJ = oForm.Items.Item("txPJ").Specific;
            itxPJ = oForm.Items.Item("txPJ");
            txPJ.DataBind.SetBound(true, "", "txPJ");



            oForm.DataSources.UserDataSources.Add("txDD", SAPbouiCOM.BoDataType.dt_DATE); // Days of Month
            txDD = oForm.Items.Item("txDD").Specific;
            ItxDD = oForm.Items.Item("txDD");
            txDD.DataBind.SetBound(true, "", "txDD");

            oForm.DataSources.UserDataSources.Add("txRem", SAPbouiCOM.BoDataType.dt_SHORT_TEXT,50); // Days of Month
            txRem = oForm.Items.Item("txRem").Specific;
            ItxRem = oForm.Items.Item("txRem");
            txRem.DataBind.SetBound(true, "", "txRem");

             oForm.DataSources.UserDataSources.Add("txRef1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT,30); // Days of Month
            txRef1 = oForm.Items.Item("txRef1").Specific;
            ItxRef1 = oForm.Items.Item("txRef1");
            txRef1.DataBind.SetBound(true, "", "txRef1");

                oForm.DataSources.UserDataSources.Add("txRef2", SAPbouiCOM.BoDataType.dt_SHORT_TEXT,30); // Days of Month
            txRef2 = oForm.Items.Item("txRef2").Specific;
            ItxRef2 = oForm.Items.Item("txRef2");
            txRef2.DataBind.SetBound(true, "", "txRef2");

                 oForm.DataSources.UserDataSources.Add("txDS", SAPbouiCOM.BoDataType.dt_SUM); // Days of Month
            txDS = oForm.Items.Item("txDS").Specific;
            ItxDS = oForm.Items.Item("txDS");
            txDS.DataBind.SetBound(true, "", "txDS");


                 oForm.DataSources.UserDataSources.Add("txCS", SAPbouiCOM.BoDataType.dt_SUM); // Days of Month
            txCS = oForm.Items.Item("txCS").Specific;
            ItxCS = oForm.Items.Item("txCS");
            txCS.DataBind.SetBound(true, "", "txCS");


            oForm.DataSources.UserDataSources.Add("chkPost", SAPbouiCOM.BoDataType.dt_SHORT_TEXT,1); // Days of Month
            chkPost = oForm.Items.Item("chkPost").Specific;
            IchkPost = oForm.Items.Item("chkPost");
            chkPost.DataBind.SetBound(true, "", "chkPost");

            oForm.DataSources.UserDataSources.Add("optPP", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1); // Days of Month
            optPP = oForm.Items.Item("optPP").Specific;
            ioptPP = oForm.Items.Item("optPP");
            optPP.DataBind.SetBound(true, "", "optPP");

            oForm.DataSources.UserDataSources.Add("optC", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1); // Days of Month
            optC = oForm.Items.Item("optC").Specific;
            ioptC = oForm.Items.Item("optC");
            optC.DataBind.SetBound(true, "", "optC");

            oForm.DataSources.UserDataSources.Add("optPC", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1); // Days of Month
            optPC = oForm.Items.Item("optPC").Specific;
            ioptPC = oForm.Items.Item("optPC");
            optPC.DataBind.SetBound(true, "", "optPC");

            optC.GroupWith("optPP");
            optPC.GroupWith("optPP");
            optPP.Selected = true;

            btPost = oForm.Items.Item("btPost").Specific;

            lnkBG = oForm.Items.Item("lnkBG").Specific;
            IchkPost = oForm.Items.Item("lnkBG");

           


        }
        public override void CreateForm(SAPbouiCOM.Application SboApp, string strXml, SAPbobsCOM.Company cmp, string frmId)
        {
            base.CreateForm(SboApp, strXml, cmp, frmId);
            InitiallizeForm();
            oForm.Visible = true;
          
        }
      
        #endregion

        private void fillUserCb()
        {
            string strSelect = " Select UserId,UserName from [dbo].[JE_getJEOriginators]('" + oCompany.UserName + "') ";

            System.Data.DataTable dtUsr = Program.objHrmsUI.getDataTable(strSelect, "Filling Originator");
            if (dtUsr != null && dtUsr.Rows.Count > 0)
            {
                foreach (DataRow dr in dtUsr.Rows)
                {
                    cbOr.ValidValues.Add(dr["UserId"].ToString(), dr["UserName"].ToString());

                }
            }
            cbOr.Select("All", BoSearchKey.psk_ByValue);
        }
        private void iniUI()
        {
            dtDet.Rows.Clear();
            dtHead.Rows.Clear();
            txCS.Value = "0.00";
            txDD.Value = "";
            txDN.Value = "";
            txDS.Value = "0.00";
            txRef1.Value = "";
            txRef2.Value = "";
            txRem.Value = "";
            chkPost.Checked = false;
            mtDet.LoadFromDataSource();
            fillJeGrid();


        }
        private void fillAllJes()
        {
            string jeType = "PP";
            if (optPP.Selected) jeType = "PP";
            if (optPC.Selected) jeType = "PC";
            if (optC.Selected) jeType = "C";
            DataServices ds = new DataServices(Program.strConMena);
               
            string strSelect = " Select Voucher_ID, Voucher_Date, Remarks, Salary_Month, Salary_Year, isnull(Is_Posted,0), isnull(JE_Number,0), isnull( Is_Cancelled,0), isnull( Cancelled_JE_Number,0) from [dbo].[JE_PRL_getJEs]('" + jeType + "') ";
            dtPendingJEs.Rows.Clear();
            System.Data.DataTable dtUsr =ds.getDataTable(strSelect, "Filling All Jes");
            if (dtUsr != null && dtUsr.Rows.Count > 0)
            {
                foreach (DataRow dr in dtUsr.Rows)
                {
                    dtPendingJEs.Rows.Add(dr[0].ToString(), dr[1].ToString(),dr[2].ToString(),dr[3].ToString(),dr[4].ToString(),dr[5],dr[6],dr[7],dr[8]);
                }
            }
        }

        private void fillJeDetailHead(string docNum , int RowNum)
        {
            txDS.Value = "0.00";
            txCS.Value = "0.00";
            txDD.Value = "";
            txDN.Value = "";
            txRem.Value = "";
            txRef1.Value = "";
            txRef2.Value = "";
            txPJ.Value = "";
            txCJ.Value = "";
            dtDet.Rows.Clear();
            string jeType = "PP";
            if (optPP.Selected) jeType = "PP";
            if (optPC.Selected) jeType = "PC";
            if (optC.Selected) jeType = "C";
            try
            {
                if (jeType == "C") 
                    btPost.Item.Enabled = false;
                    else btPost.Item.Enabled = true;
            }
            catch { }
            if (RowNum > 0)
            {
                txRem.Value = Convert.ToString(dtHead.GetValue("cRemarks", RowNum - 1));
                txRef1.Value = Convert.ToString(dtHead.GetValue("cNum", RowNum - 1));
                txPJ.Value = Convert.ToString(dtHead.GetValue("cPostedJE", RowNum - 1));
                txCJ.Value = Convert.ToString(dtHead.GetValue("cCanceledJE", RowNum - 1));
                oForm.DataSources.UserDataSources.Item("txDD").ValueEx = Convert.ToDateTime(dtHead.GetValue("cDate", RowNum - 1)).ToString("yyyyMMdd");
                    
            }
            chkPost.Checked = false;
            dtPendingJEs.Rows.Clear();
            DataServices ds = new DataServices(Program.strConMena);
               

            string strSelect = " Select MenaCode as lineType ,acctCode,acctName,debit ,credit , project ,ocr1 ,ocr2 ,ocr3 ,ocr4 ,ocr5 from [dbo].[JE_PRL_getJEDetRows]('" + docNum + "') ";
            dtDet.Rows.Clear();

            System.Data.DataTable dtRows = ds.getDataTable(strSelect, "Filling Detail");
            if (dtRows != null && dtRows.Rows.Count > 0)
            {
                int i = 0;
                double debit = 0.00;
                double credit = 0.00;
                foreach (DataRow dr in dtRows.Rows)
                {
                    double lineDebit = 0.00;
                    double lineCredit = 0.00;

                    lineDebit = Convert.ToDouble(dr["debit"].ToString());
                    lineCredit = Convert.ToDouble(dr["credit"].ToString());
                    debit += lineDebit;
                    credit += lineCredit;
                    dtDet.Rows.Add(1);
                    dtDet.SetValue("cNum", i, i.ToString());
                    dtDet.SetValue("cType", i, Convert.ToString(dr["lineType"].ToString()));
                    dtDet.SetValue("cCode", i, Convert.ToString(Program.objHrmsUI.getAcctSys(dr["lineType"].ToString().Replace("-", ""))));
                    dtDet.SetValue("cName", i, Convert.ToString(Program.objHrmsUI.getAcctName(dr["lineType"].ToString().Replace("-", ""))));
                    dtDet.SetValue("cDebit", i, Convert.ToString(dr["debit"].ToString()));
                    dtDet.SetValue("cCredit", i, Convert.ToString(dr["credit"].ToString()));
                    dtDet.SetValue("cProject", i, Convert.ToString(dr["project"].ToString()));
                    dtDet.SetValue("cOcr1", i, Convert.ToString(dr["ocr1"].ToString()));
                    dtDet.SetValue("cOcr2", i, Convert.ToString(dr["ocr2"].ToString()));
                    dtDet.SetValue("cOcr3", i, Convert.ToString(dr["ocr3"].ToString()));
                    dtDet.SetValue("cOcr4", i, Convert.ToString(dr["ocr4"].ToString()));
                    dtDet.SetValue("cOcr5", i, Convert.ToString(dr["ocr5"].ToString()));
                  
                    i++;




                }
                mtDet.LoadFromDataSource();
                txDS.Value = debit.ToString();
                txCS.Value = credit.ToString();
            }
            mtDet.LoadFromDataSource();
                

        }

        private void fillJeGrid()
        {
            fillAllJes();
            dtHead.Rows.Clear();
            int i=0;
            foreach (DataRow dr in dtPendingJEs.Rows)
            {
                if (Convert.ToString(cbOr.Value).Trim() == "All" || Convert.ToString(cbOr.Value).Trim() == dr["Originator"].ToString())
                {
                    try
                    {   

                        dtHead.Rows.Add(1);
                        dtHead.SetValue("cNum", i, dr["V#"].ToString());
                        dtHead.SetValue("cDate", i, Convert.ToDateTime(dr["Date"].ToString()));
                        dtHead.SetValue("cRemarks", i, dr["Remarks"].ToString());
                        dtHead.SetValue("cMonth", i, Convert.ToInt16( dr["Month"].ToString()));
                        dtHead.SetValue("cYear", i, Convert.ToInt16( dr["Year"].ToString()));
                        dtHead.SetValue("cPostedJE", i, Convert.ToInt16(dr["PostedJE"].ToString()));
                        dtHead.SetValue("cCanceledJE", i, Convert.ToInt16(dr["CanceledJE"].ToString()));
                    }
                    catch { }
                    i++;
                }
            }
            mtJE.LoadFromDataSource();

        }


        public string postJe()
        {
            long jdtNum = 0;
            int errnum = 0;
            string errDesc = "";
            string outStr = "";
            string jeType = "PP";
            if (optPP.Selected) jeType = "PP";
            if (optPC.Selected) jeType = "PC";
            if (optC.Selected) jeType = "C";

            if (!chkPost.Checked)
            {
                Program.objHrmsUI.oApplication.SetStatusBarMessage("You need to check the approved box before posting");
                return "Error";
            }
            if (txRef1.Value == "")
            {
                Program.objHrmsUI.oApplication.SetStatusBarMessage("Select a transaction to post!");
                return "Error";
            }
            SAPbobsCOM.JournalEntries vJE = (SAPbobsCOM.JournalEntries)Program.objHrmsUI.oCompany .GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);


            string Remarks = txRem.Value.ToString();
            DateTime postingDate = DateTime.ParseExact(txDD.Value, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);



            try
            {
                vJE.ReferenceDate = postingDate;
                vJE.TaxDate = postingDate;
                vJE.DueDate = postingDate;
                if (Remarks.Length > 20)
                {
                    vJE.Memo = Remarks.Substring(0, 20); // dgSales[3, i].Value.ToString().Substring(0, 20);
                }
                else
                {
                    vJE.Memo = Remarks;
                }
                vJE.Reference = txRef1.Value;
                vJE.Reference2 = txRef2.Value;
                vJE.Reference3 = "PRL";

                for (int i = 0; i < mtDet.RowCount; i++)
                {
                    string codetype = dtDet.GetValue("cType", i);
                    string acctcode = dtDet.GetValue("cCode", i);
                    string acctname = dtDet.GetValue("cName", i);
                    string debit = Convert.ToString(dtDet.GetValue("cDebit", i));
                    string credit = Convert.ToString(dtDet.GetValue("cCredit", i));
                    string project = dtDet.GetValue("cProject", i);
                    string ocr1 = dtDet.GetValue("cOcr1", i);
                    string ocr2 = dtDet.GetValue("cOcr2", i);
                    string ocr3 = dtDet.GetValue("cOcr3", i);
                    string ocr4 = dtDet.GetValue("cOcr4", i);
                    string ocr5 = dtDet.GetValue("cOcr5", i);


                    if (codetype == "GL")
                    {

                        vJE.Lines.AccountCode = acctcode;

                    }
                    else
                    {
                        vJE.Lines.ShortName = acctcode;

                    }
                    if (jeType == "PP")
                    {
                        vJE.Lines.Credit = Convert.ToDouble(credit);
                        vJE.Lines.Debit = Convert.ToDouble(debit);
                    }
                    else
                    {
                        vJE.Lines.Credit = Convert.ToDouble(debit);
                        vJE.Lines.Debit = Convert.ToDouble(credit);
                    }
                    vJE.Lines.DueDate = postingDate;
                    vJE.Lines.ReferenceDate1 = postingDate;
                    vJE.Lines.TaxDate = postingDate;
                    vJE.Lines.Reference1 = vJE.Reference;
                    vJE.Lines.Reference2 = vJE.Reference2;
                    vJE.Lines.ProjectCode = project;
                    vJE.Lines.CostingCode = ocr1;
                    vJE.Lines.CostingCode2 = ocr2;
                    vJE.Lines.CostingCode3 = ocr3;
                    vJE.Lines.CostingCode4 = ocr4;
                    vJE.Lines.CostingCode5 = ocr5;
                    vJE.Lines.Add();
                }


            }
            catch (Exception ex)
            {
                outStr = ex.Message;

            }
            if (vJE.Add() != 0)
            {
                int erroCode = 0;
                string errDescr = "";
                Program.objHrmsUI .oCompany.GetLastError(out erroCode, out errDescr);
                outStr = "Error:" + errDescr + outStr;
                Program.objHrmsUI.oApplication.SetStatusBarMessage(outStr);
            }
            else
            {
                outStr = Convert.ToString(Program.objHrmsUI .oCompany.GetNewObjectKey());
                DataServices ds = new DataServices(Program.strConMena);
                ds.ExecuteNonQuery("Exec [dbo].[JE_PRL_UpdatedPostedJE] '" + txRef1.Value + "','" + outStr + "','" + jeType + "'  ");

                Program.objHrmsUI.oApplication.SetStatusBarMessage("Journal Entry Created. JE # " + outStr, BoMessageTime.bmt_Medium, false);
                iniUI();
            }
            return outStr;



        }
        

        #region //Common Methods


        #endregion

       

       
    }

}

