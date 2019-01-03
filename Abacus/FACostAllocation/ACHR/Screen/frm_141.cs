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




namespace ACHR.Screen
{
    class frm_141 : SysBaseForm
    {

        SAPbouiCOM.Item oItem, oItem1, oItemRef;
        SAPbouiCOM.Button btALC;
        SAPbouiCOM.DBDataSource dbOPCH;
           

        DataServices dsWEB;
        DataServices dsSAP;


        #region /////Events
        public override void etFormAfterLoad(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etFormAfterLoad(ref pVal, ref BubbleEvent);
          
            InitiallizeForm();
            
            
          //  oApplication.MessageBox("Project Form Loaded");
        }


        public override void etAfterClick(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);
            if(pVal.ItemUID==btALC.Item.UniqueID)
            {
                postAllocationJE( Convert.ToInt32( dbOPCH.GetValue("DocEntry",0)));
                //oApplication.MessageBox("Posting Allocation Entry");
            }
        }
        public override void etFormBeforeDataUpdate(ref BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent)
        {
           
        }
        
       #endregion

        #region ///Initiallization
        public override void AddNewRecord()
        {
            base.AddNewRecord();
           
        }

        private void InitiallizeForm()
        {


            oForm.Freeze(true);
            dbOPCH = oForm.DataSources.DBDataSources.Item("OPCH");



            oItemRef = oForm.Items.Item("2");
            SAPbouiCOM.StaticText lblCode, lblName, lblInt;
            SAPbouiCOM.CheckBox chkAll, chkMon, chkTue, chkWed, chkThs, chkFri, chkSat, chkSun;
            SAPbouiCOM.ComboBox cbInt;
            SAPbouiCOM.EditText txtCode, txtName;


            try
            {


                SAPbouiCOM.Item oItemRef1 = oForm.Items.Item("9");


                oItem = oForm.Items.Add("B1_ALC", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                oItem.Top = oItemRef.Top;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width + 5;
                oItem.Width = oItemRef.Width + 30;
                oItem.Visible = true;
                btALC = (SAPbouiCOM.Button)oItem.Specific;

                btALC.Caption = "Post Allocation";







            }
            catch (Exception ex)
            {

                string message = ex.Message;

            }

            oForm.Freeze(false);
            dsSAP = new DataServices(Program.strConSAP);



        }



        #endregion

        #region //Common Methods

        private void postAllocationJE(int invEntry)
        {
            try
            {
                SAPbobsCOM.Documents apInv = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(BoObjectTypes.oPurchaseInvoices);
                SAPbobsCOM.JournalEntries apJE = (SAPbobsCOM.JournalEntries)oCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);
                SAPbobsCOM.JournalEntries AllocationJE;
                apInv.GetByKey(invEntry);
                int ApJeNum = apInv.TransNum;
                apJE.GetByKey(ApJeNum);

                System.Data.DataTable dtCandidateRows = new System.Data.DataTable();

                dtCandidateRows.Columns.Add("LineNum");
                dtCandidateRows.Columns.Add("AlocCode");
                dtCandidateRows.Columns.Add("GLCode");
                dtCandidateRows.Columns.Add("Amount");
                dtCandidateRows.Columns.Add("DR");
               

                for (int i = 0; i < apInv.Lines.Count; i++)
                {
                    apInv.Lines.SetCurrentLine(i);
                    if (apInv.Lines.UserFields.Fields.Item("U_CostCode").Value.ToString().Trim() != "" && (apInv.Lines.UserFields.Fields.Item("U_AlocJe").Value.ToString().Trim()=="0" ||  apInv.Lines.UserFields.Fields.Item("U_AlocJe").Value.ToString().Trim()==""))
                    {
                        dtCandidateRows.Rows.Add(i, apInv.Lines.UserFields.Fields.Item("U_CostCode").Value.ToString().Trim(), apInv.Lines.AccountCode, (apInv.Lines.LineTotal),apInv.Lines.CostingCode);
                    }
                }

                int totalCnt = dtCandidateRows.Rows.Count;
                int currentRow = 0;
                foreach (DataRow dr in dtCandidateRows.Rows)
                {

                    currentRow++;
                    oApplication.StatusBar.SetText("Processing Allocation JE (" + currentRow.ToString() + " of " + totalCnt.ToString(), BoMessageTime.bmt_Short,BoStatusBarMessageType.smt_Warning);
                    string costCode = dr["AlocCode"].ToString();

                    Hashtable hp = new Hashtable();
                    hp.Add("~p1", costCode);




                    string strRulCode = Program.objHrmsUI.getQryString("141_GET_001", hp); //"select t1.U_AR from [@B1_DBOQD] t0 inner join [@B1_IBOQ] t1 on t0.U_ItemCode = t1.Code where t0.Code = '~p1' ";


                    System.Data.DataTable dtRuleCode = Program.objHrmsUI.getDataTable(strRulCode, "Getting RuleCode");
                    if (dtRuleCode == null || dtRuleCode.Rows.Count == 0)
                    {
                        oApplication.MessageBox("Cost Code not associated with Indirect BOQ to post allocaiton JE");
                        continue;
                    }

                    string allocationCode = dtRuleCode.Rows[0]["U_AR"].ToString();


                    string CreditGLCode = dr["GLCode"].ToString();
                    double CreditAmount = Convert.ToDouble(dr["Amount"]);
                    int lineNum = Convert.ToInt32(dr["LineNum"]);
                    string DR = dr["DR"].ToString();
                    hp.Clear();
                    hp.Add("~p1", allocationCode);


                    string strDetails = Program.objHrmsUI.getQryString("141_GET_002", hp);// @"Select t0.U_AP as AP , t1.U_AcctCode,t1.Code, t3.[U_BU] as BaseOn ,isnull(t1.U_FACode,'') as FA from [@B1_ARD] t0 inner join [@B1_AM] t1 on t1.Code = t0.U_CMC  inner join [@B1_AR] t3 on t3.[Code]=t0.[U_RuleCode] where t0.U_RuleCode='~p1'";
                    System.Data.DataTable dtrule = Program.objHrmsUI.getDataTable(strDetails, "Getting Rule Detail");
                    if (dtrule.Rows.Count > 0)
                    {



                        AllocationJE = (SAPbobsCOM.JournalEntries)oCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);
                        AllocationJE.Reference = allocationCode;

                        double debitTotal = 0.00;
                        foreach (DataRow drDetails in dtrule.Rows)
                        {
                            string debitAccount = drDetails["U_AcctCode"].ToString();
                            double AP = Convert.ToDouble(drDetails["AP"]);
                            double debitAmount = CreditAmount * AP / 100.00000;
                            string baseOn = Convert.ToString(drDetails["BaseOn"]);
                            string FA = Convert.ToString(drDetails["FA"]);
                            addChildJeLines(AllocationJE, drDetails["Code"].ToString(), debitAmount, debitAccount, baseOn, FA, costCode,AP.ToString());

                        }

                        for (int k = 0; k < AllocationJE.Lines.Count; k++)
                        {
                            AllocationJE.Lines.SetCurrentLine(k);
                            debitTotal += AllocationJE.Lines.Debit;
                        }

                        AllocationJE.Lines.AccountCode = CreditGLCode;
                      //  AllocationJE.Lines.Credit = debitTotal;
                        AllocationJE.Lines.Credit = CreditAmount;
                        AllocationJE.Lines.Reference1 = allocationCode;
                        AllocationJE.Lines.CostingCode = DR;
                        AllocationJE.Lines.Add();



                        double diffAmount = CreditAmount - debitTotal;

                        if (CreditAmount != debitTotal)
                        {
                            string roundAcct = getRoundingAcct();
                            AllocationJE.Lines.AccountCode = roundAcct;

                            AllocationJE.Lines.Debit = diffAmount;
                            AllocationJE.Lines.Reference1 = allocationCode;
                            //  AllocationJE.Lines.CostingCode = DR;
                            AllocationJE.Lines.Add();
                        }




                        if (AllocationJE.Add() != 0)
                        {
                            int erroCode = 0;
                            string errDescr = "";
                            Program.objHrmsUI.oCompany.GetLastError(out erroCode, out errDescr);
                            oApplication.StatusBar.SetText("Failed to add Order  : " + errDescr);
                        }
                        else
                        {
                            string outStr = Convert.ToString(Program.objHrmsUI.oCompany.GetNewObjectKey());

                            hp.Clear();
                            hp.Add("~p1", outStr);
                            hp.Add("~p2", invEntry);
                            hp.Add("~p3", lineNum);

                            string updateCall = Program.objHrmsUI.getQryString("141_CRUD_001", hp);// "Update pch1 set U_AlocJE='~p1' where docentry = '~p2' and linenum='~p3'";
                            int result = Program.objHrmsUI.ExecQuery(updateCall, "Update Line JE");

                            oApplication.Menus.Item("1304").Activate();
                        }




                    }

                }

            }
            catch (Exception ex)
            {
                oApplication.MessageBox(ex.Message);
            }

        }
        private string getRoundingAcct()
        {
            string roundAccout = "";
            string strSql = "SELECT T0.\"LinkAct_24\", T0.\"AbsEntry\" FROM OACP T0 ORDER BY \"AbsEntry\" DESC";
            System.Data.DataTable dtRound = Program.objHrmsUI.getDataTable(strSql, "RoundingAccount");
            if (dtRound != null && dtRound.Rows.Count > 0)
            {
                roundAccout = dtRound.Rows[0]["LinkAct_24"].ToString();
            }
            return roundAccout;
        }


        private void addChildJeLines(SAPbobsCOM.JournalEntries AllocationJE, string AMCode, double devideAmount, string debitAccount,string AllocBs,string FA,string costCode , string AllocationRate)
        {

//            string strAMChildern = @"
//                                        select case convert(varchar, '~p1') when '01' then u_area when '02' then U_Volume when '03' then U_Floors else U_Area end as AllocValue
//                                        , code,name,U_Father,U_AcctCode , isnull(u_facode,'') as FA
//                                         from [@B1_AM] t0 where U_Father = '~p2'";
            Hashtable hp = new Hashtable();
            hp.Clear();
            hp.Add("~p1", AllocBs);
            hp.Add("~p2", AMCode);

            string strAMChildern = Program.objHrmsUI.getQryString("141_GET_003", hp);// "Update pch1 set U_AlocJE='~p1' where docentry = '~p2' and linenum='~p3'";
                        
            System.Data.DataTable dtChildrens = Program.objHrmsUI.getDataTable(strAMChildern, "Getting Childeren");
            if (dtChildrens.Rows.Count == 0)
            {
                AllocationJE.Lines.AccountCode = debitAccount;
                AllocationJE.Lines.Debit = Convert.ToDouble( Convert.ToInt32( devideAmount * 100)) /100 ;
                AllocationJE.Lines.Reference1 = costCode;
                AllocationJE.Lines.Reference2 = FA;
                AllocationJE.Lines.UserFields.Fields.Item("U_B1_APP").Value = AllocationRate;
                AllocationJE.Lines.Add();
            }
            else
            {
                double unitTotal = 0;
                foreach (DataRow drChild in dtChildrens.Rows)
                {
                    unitTotal += Convert.ToDouble(drChild["AllocValue"]);
                }
                foreach (DataRow drChild in dtChildrens.Rows)
                {
                    string childCode = drChild["code"].ToString();
                    string ChildAccount = drChild["U_AcctCode"].ToString();
                    double ChildVal = Convert.ToDouble(drChild["AllocValue"]);
                    string cFA = drChild["FA"].ToString();
                    double debitAmount = devideAmount * ChildVal / unitTotal;
                    addChildJeLines(AllocationJE, childCode, debitAmount, ChildAccount, AllocBs, cFA,costCode,AllocationRate);

                }
            }

        }

         

        #endregion

      

    }

}

