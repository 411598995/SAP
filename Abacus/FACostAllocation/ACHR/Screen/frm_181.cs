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
    class frm_181 : SysBaseForm
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
                btALC.Item.Enabled = false;
                CancelAllocationJE(Convert.ToInt32(dbOPCH.GetValue("DocEntry", 0)));
                //oApplication.MessageBox("Posting Allocation Entry");
                btALC.Item.Enabled = true;
            }
        }
        
       #endregion

        #region ///Initiallization
        public override void AddNewRecord()
        {
            base.AddNewRecord();
           
        }
        private void CancelAllocationJE(int DocEntry)
        {
            SAPbobsCOM.Documents apCN = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(BoObjectTypes.oPurchaseCreditNotes);
            apCN.GetByKey(DocEntry);
            int totalCnt = apCN.Lines.Count;
            int currentRow = 0;
              
            for (int i = 0; i < apCN.Lines.Count; i++)
            {
                currentRow++;
                apCN.Lines.SetCurrentLine(i);
                string AllocJE = apCN.Lines.UserFields.Fields.Item("U_AlocJe").Value.ToString();
                if (AllocJE != "")
                {
                    oApplication.StatusBar.SetText("Processing Allocation JE (" + currentRow.ToString() + " of " + totalCnt.ToString(), BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                  
                    postAllocationJERev(Convert.ToInt32(AllocJE),i,DocEntry);
                }
            }

               
        }
        private void InitiallizeForm()
        {


            oForm.Freeze(true);
            dbOPCH = oForm.DataSources.DBDataSources.Item("ORPC");



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
                oItem.Width = oItemRef.Width + 40;
                oItem.Visible = true;
                btALC = (SAPbouiCOM.Button)oItem.Specific;

                btALC.Caption = "Cancel Allocation JEs";







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

        private void postAllocationJERev(int jeEntry,int lineNum,int CnEntry)
        {
            try
            {
                SAPbobsCOM.JournalEntries apJE = (SAPbobsCOM.JournalEntries)oCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);
                apJE.GetByKey(jeEntry);

                if (apJE.Cancel() != 0)
                {
                    int erroCode = 0;
                    string errDescr = "";
                    Program.objHrmsUI.oCompany.GetLastError(out erroCode, out errDescr);
                    oApplication.StatusBar.SetText("Failed to cancel JE  : " + errDescr);
                }
                else
                {
                    string outStr = Convert.ToString(Program.objHrmsUI.oCompany.GetNewObjectKey());


                    string updateCall = "UPDATE RPC1 set \"U_AlocJeRev\"='" + outStr + "' WHERE \"DocEntry\" = '" + CnEntry.ToString() + "' AND \"LineNum\"='" + lineNum.ToString() + "'";
                    int result = Program.objHrmsUI.ExecQuery(updateCall, "Update Line JE");

                    oApplication.Menus.Item("1304").Activate();
                }





            }
            catch (Exception ex)
            {
                oApplication.MessageBox(ex.Message);
            }

        }


        

        #endregion

      

    }

}

