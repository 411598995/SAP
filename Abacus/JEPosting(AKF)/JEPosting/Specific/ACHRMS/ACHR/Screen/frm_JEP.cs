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
    class frm_JEP : HRMSBaseForm
    {

       
        SAPbouiCOM.Item IcbOr, ItxDN, ItxDD, ItxRem, ItxRef1, ItxRef2, ItxDS, ItxCS, IbtPost, IchkPost;

        SAPbouiCOM.EditText txDN, txDD ,txRem ,txRef1 ,txRef2 , txDS ,txCS;
        SAPbouiCOM.Button btPost;
        SAPbouiCOM.LinkedButton lnkBG;
        SAPbouiCOM.ComboBox cbOr;
        SAPbouiCOM.Matrix mtJE, mtDet;
        SAPbouiCOM.DataTable dtDet, dtHead;
        SAPbouiCOM.CheckBox chkPost;

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
                    SAPbouiCOM.EditText InvNum = mtJE.Columns.Item("cDN").Cells.Item(pVal.Row).Specific;
                    fillJeDetailHead(InvNum.Value.ToString());
                               
                }
            }
            if (pVal.ItemUID == "lnkBG")
            {
                Hashtable hp = new Hashtable();
                hp.Add("~p1", txDN.Value.ToString());
                string strSelect = Program.objHrmsUI.getQryString("JE_GET_DE", hp);
                string strEntry = "0";
                System.Data.DataTable dtUsr = Program.objHrmsUI.getDataTable(strSelect, "Opening Linked Branch JE");
                if (dtUsr != null && dtUsr.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtUsr.Rows)
                    {
                        strEntry = dr["docentry"].ToString();

                    }
                }


                oApplication.OpenForm(BoFormObjectEnum.fo_UserDefinedObject, "ABGP", strEntry);
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

            dtPendingJEs.Columns.Add("DocDate");
            dtPendingJEs.Columns.Add("DocNum");
            dtPendingJEs.Columns.Add("DocTotal");
            dtPendingJEs.Columns.Add("Originator");

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

            oForm.DataSources.UserDataSources.Add("txDD", SAPbouiCOM.BoDataType.dt_DATE); // Days of Month
            txDD = oForm.Items.Item("txDD").Specific;
            ItxDD = oForm.Items.Item("txDD");
            txDD.DataBind.SetBound(true, "", "txDD");

            oForm.DataSources.UserDataSources.Add("txRem", SAPbouiCOM.BoDataType.dt_SHORT_TEXT,100); // Days of Month
            txRem = oForm.Items.Item("txRem").Specific;
            ItxRem = oForm.Items.Item("txRem");
            txRem.DataBind.SetBound(true, "", "txRem");

             oForm.DataSources.UserDataSources.Add("txRef1", SAPbouiCOM.BoDataType.dt_SHORT_TEXT,100); // Days of Month
            txRef1 = oForm.Items.Item("txRef1").Specific;
            ItxRef1 = oForm.Items.Item("txRef1");
            txRef1.DataBind.SetBound(true, "", "txRef1");

                oForm.DataSources.UserDataSources.Add("txRef2", SAPbouiCOM.BoDataType.dt_SHORT_TEXT,100); // Days of Month
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
             Hashtable hp=new Hashtable();
            hp.Add("~p1",oCompany.UserName);
            string strSelect = Program.objHrmsUI.getQryString("JE_getJEOriginators", hp);
            System.Data.DataTable dtUsr = Program.objHrmsUI.getDataTable(strSelect, "Filling Originator " + oCompany.DbServerType + strSelect);
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
           Hashtable hp = new Hashtable();
            hp.Add("~p1", oCompany.UserName);
            string strSelect = Program.objHrmsUI.getQryString("JE_getJEs", hp);
           
            dtPendingJEs.Rows.Clear();
            System.Data.DataTable dtUsr = Program.objHrmsUI.getDataTable(strSelect, "Filling All Jes");
            if (dtUsr != null && dtUsr.Rows.Count > 0)
            {
                foreach (DataRow dr in dtUsr.Rows)
                {
                    dtPendingJEs.Rows.Add(dr[0].ToString(), dr[1].ToString(),dr[2].ToString(),dr[3].ToString());
                }
            }
        }

        private void fillJeDetailHead(string docNum)
        {
            txDS.Value = "0.00";
            txCS.Value = "0.00";
            chkPost.Checked = false;
            Hashtable hp = new Hashtable();
            hp.Add("~p1", docNum);
            string strSelect = Program.objHrmsUI.getQryString("JE_getJEDetHead", hp);
            dtPendingJEs.Rows.Clear();
            System.Data.DataTable dtJeHead = Program.objHrmsUI.getDataTable(strSelect, "Filling All Jes");
            if (dtJeHead != null && dtJeHead.Rows.Count > 0)
            {
                foreach (DataRow dr in dtJeHead.Rows)
                {
                    oForm.DataSources.UserDataSources.Item("txDD").ValueEx = Convert.ToDateTime(dr["docDate"]).ToString("yyyyMMdd");
                    txDN.Value = dr["docNum"].ToString();
                    txRem.Value = dr["Remarks"].ToString();
                    txRef1.Value = dr["Ref1"].ToString();
                    txRef2.Value = dr["Ref2"].ToString();

                }
            }

            strSelect = @"SELECT     case when ( t0.U_DAccCode  in (select cardcode from ocrd)) then    'BP' else 'GL' end  lineType, 
                            case when (t0.U_DAccCode in (select cardcode from ocrd)) then U_DAccCode else 
                            (select acctcode from oact where replace( t0.U_DAccCode,'-','') = oact.FormatCode ) end acctCode

                            , t0.U_DAccName, t0.U_DebitAmt debit ,0 credit
                            ,  t0.U_Project project ,t0.U_ProfitCode ocr1 , t0.U_OcrCode2 ocr2 , t0.U_OcrCode3 ocr3 , t0.U_OcrCode4 ocr4 , t0.U_OcrCode5 ocr5
                            FROM            dbo.[@ABGP] t0 
						                             where t0.DocNum = '~p1'";

            strSelect = Program.objHrmsUI.getQryString("JE_getJEDetRows_D", hp);
            double debit = 0.00;
            double credit = 0.00;

            dtDet.Rows.Clear();
            System.Data.DataTable dtRows = Program.objHrmsUI.getDataTable(strSelect, "Filling Detail");
            if (dtRows != null && dtRows.Rows.Count > 0)
            {
                int i = 0;
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
                    dtDet.SetValue("cCode", i, Convert.ToString(dr["acctCode"].ToString()));
                    dtDet.SetValue("cName", i, Convert.ToString(dr["acctName"].ToString()));
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

            }


            strSelect = Program.objHrmsUI.getQryString("JE_getJEDetRows_C", hp);
         
             dtRows = Program.objHrmsUI.getDataTable(strSelect, "Filling Detail");
            if (dtRows != null && dtRows.Rows.Count > 0)
            {
                int i = 1;
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
                    dtDet.SetValue("cCode", i, Convert.ToString(dr["acctCode"].ToString()));
                    dtDet.SetValue("cName", i, Convert.ToString(dr["acctName"].ToString()));
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

            }


            mtDet.LoadFromDataSource();
            txDS.Value = debit.ToString();
            txCS.Value = credit.ToString();

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
                        dtHead.SetValue("cNum", i, i.ToString());
                        dtHead.SetValue("cDate", i, Convert.ToDateTime( dr["DocDate"].ToString()));
                        dtHead.SetValue("cDN", i, Convert.ToInt32(dr["DocNum"].ToString()));
                        dtHead.SetValue("cTotal", i, Convert.ToDouble( dr["DocTotal"].ToString()));
                        dtHead.SetValue("cOrigen", i, Convert.ToString( dr["Originator"].ToString()));
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

            if (!chkPost.Checked)
            {
                Program.objHrmsUI.oApplication.SetStatusBarMessage("You need to check the approved box before posting");
                return "Error";
            }
            if (txDN.Value == "")
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
             //   vJE.TransactionCode = "JP";
                vJE.Reference3 = "JP_" + txDN.Value.ToString()  ;

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
                    vJE.Lines.Credit = Convert.ToDouble(credit);
                    vJE.Lines.Debit = Convert.ToDouble(debit);
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
                string strObjJe = txDN.Value.ToString();

                string strUpdate = " UPDATE \"@ABGP\" SET \"Status\" ='C' ,  \"U_Ref3\"= '" + outStr + "' WHERE \"DocNum\"='" + strObjJe + "'";

                Program.objHrmsUI.ExecQuery(strUpdate , "Updating after posting");

                Program.objHrmsUI.oApplication.SetStatusBarMessage("Journal Entry Created. JE # " + outStr, BoMessageTime.bmt_Medium, false);
                iniUI();
            }
            return outStr;



        }
        

        #region //Common Methods


        #endregion

       

       
    }

}

