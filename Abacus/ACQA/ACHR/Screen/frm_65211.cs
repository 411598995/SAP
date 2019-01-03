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
    class frm_65211 : SysBaseForm
    {

        SAPbouiCOM.Item oItem, oItem1, oItemRef;
        SAPbouiCOM.Button B1_INS;
        SAPbouiCOM.DBDataSource dbOWOR;
           

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
            if (pVal.ItemUID == B1_INS.Item.UniqueID)
            {

                if (oForm.Mode != BoFormMode.fm_OK_MODE)
                {
                    oApplication.MessageBox("Please save changes before sending it for inspaction");
                    return;
                }
                string DE = Convert.ToString(dbOWOR.GetValue("DocEntry", 0));
                string BS = Convert.ToString(dbOWOR.GetValue("U_B1_QA_INSBSN", 0));
                int StartFrom = Convert.ToInt32(dbOWOR.GetValue("U_B1_QA_INSBSNST", 0));

                if (BS == "")
                {
                    oApplication.MessageBox("Please update Batch / Serial # for item");
                    return;
                }
                string GR = Convert.ToString(dbOWOR.GetValue("U_B1_QA_INSGR", 0));
                if (GR != "")
                {
                    oApplication.MessageBox("Already Sent for Inspaction");
                    return;
                }
                string ItemCode = Convert.ToString(dbOWOR.GetValue("ItemCode", 0));
                double plannedQty = Convert.ToDouble(dbOWOR.GetValue("PlannedQty", 0));
                B1_INS.Item.Enabled = false;
                PostInspaction(DE, ItemCode, plannedQty, BS, StartFrom);
                B1_INS.Item.Enabled = true;
            }
        }
        private int PostInspaction( string WOREntry ,string itemCode,double Qty,  string BSN,int startFrom)
        {
            int result = 0;

            try

            {
                string strUsrInfo = "SELECT * FROM \"@B1_QA_OUSR\" WHERE  \"Code\" = '" + oCompany.UserName + "' ";
                System.Data.DataTable dtUsrInfo = Program.objHrmsUI.getDataTable(strUsrInfo, "User Info");
                string prdWhs = "";
                if (dtUsrInfo != null && dtUsrInfo.Rows.Count > 0)
                {
                    prdWhs = dtUsrInfo.Rows[0]["U_PWHS"].ToString().Trim();
                }
                else
                {
                    oApplication.MessageBox("Inspaciton posting is not allowed for current user");
                    return -1;
                }


                SAPbobsCOM.Items oitm = (SAPbobsCOM.Items)oCompany.GetBusinessObject(BoObjectTypes.oItems);
                oitm.GetByKey(itemCode);
                SAPbobsCOM.Documents InspactionGR = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(BoObjectTypes.oInventoryGenEntry);

                InspactionGR.DocDate = DateTime.Now.Date;
                InspactionGR.UserFields.Fields.Item("U_B1_QA_INSP_PN").Value = WOREntry;
                InspactionGR.Lines.ItemCode = itemCode;
                InspactionGR.Lines.WarehouseCode = prdWhs;
                InspactionGR.Lines.AccountCode = oitm.ExpanseAccount;
                InspactionGR.Lines.Quantity = Qty;
                InspactionGR.Lines.Price = 0.01;
                if(oitm.ManageBatchNumbers == BoYesNoEnum.tYES)
                {
                    InspactionGR.Lines.BatchNumbers.InternalSerialNumber = BSN;
                    InspactionGR.Lines.BatchNumbers.Quantity = Qty;
                    InspactionGR.Lines.BatchNumbers.Add();

                }

                if (oitm.ManageSerialNumbers == BoYesNoEnum.tYES)
                {
                    double serQty = Qty;
                    while (serQty > 0)
                    {
                        InspactionGR.Lines.SerialNumbers.InternalSerialNumber = BSN + startFrom.ToString();
                        InspactionGR.Lines.SerialNumbers.Quantity = 1;
                        InspactionGR.Lines.SerialNumbers.Add();
                        serQty--;
                        startFrom++;
                    }

                }
                InspactionGR.Comments = "Inspection GR";

                if (InspactionGR.Add() != 0)
                {
                    int erroCode = 0;
                    string errDescr = "";
                    Program.objHrmsUI.oCompany.GetLastError(out erroCode, out errDescr);
                    oApplication.StatusBar.SetText("Failed send for inspaction  : " + errDescr);
                }
                else
                {
                    string outStr = Convert.ToString(Program.objHrmsUI.oCompany.GetNewObjectKey());
                    string updateCall = "UPDATE OWOR set \"U_B1_QA_INSGR\"='" + outStr + "' WHERE \"DocEntry\" = '" + WOREntry.ToString() + "'";
                    result = Program.objHrmsUI.ExecQuery(updateCall, "Update Production Order");
                    oApplication.MessageBox("Inspaction Posted ");
                    oApplication.Menus.Item("1304").Activate();
                }
                
            }
            catch (Exception ex)
            {
                oApplication.MessageBox(ex.Message);
                result = -1;
            }

            return result;
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
            dbOWOR = oForm.DataSources.DBDataSources.Item("OWOR");



            oItemRef = oForm.Items.Item("2");
            SAPbouiCOM.StaticText lblCode, lblName, lblInt;
            SAPbouiCOM.CheckBox chkAll, chkMon, chkTue, chkWed, chkThs, chkFri, chkSat, chkSun;
            SAPbouiCOM.ComboBox cbInt;
            SAPbouiCOM.EditText txtCode, txtName;


            try
            {


                SAPbouiCOM.Item oItemRef1 = oForm.Items.Item("2");


                oItem = oForm.Items.Add("B1_INS", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                oItem.Top = oItemRef.Top;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width + 5;
                oItem.Width = oItemRef.Width + 60;
                oItem.Visible = true;
                B1_INS = (SAPbouiCOM.Button)oItem.Specific;

                B1_INS.Caption = "Offer for Inspection";







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

