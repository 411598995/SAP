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
    class frm_150: SysBaseForm
    {

        SAPbouiCOM.Item oItem, oItem1, oItemRef;
        SAPbouiCOM.Button B1_ITB;
        SAPbouiCOM.StaticText B1_lbSub;
        SAPbouiCOM.DBDataSource dbOITM;
        SAPbouiCOM.ComboBox cbSubGroup;
           

        DataServices dsWEB;
        DataServices dsSAP;
        SqlStr sqlProvider = new SqlStr();

        #region /////Events
        public override void etFormAfterLoad(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etFormAfterLoad(ref pVal, ref BubbleEvent);
            if (pVal.BeforeAction == false)
            {
                InitiallizeForm();
            }
            
          //  oApplication.MessageBox("Project Form Loaded");
        }

        public override void etAfterCmbSelect(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCmbSelect(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == "39")
            {
                fillCmb();
            }
        }
        

        public override void etFormAfterDataLoad(ref BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent)
        {
            base.etFormAfterDataLoad(ref BusinessObjectInfo, ref BubbleEvent);

            fillCmb();
            oForm.Mode = BoFormMode.fm_OK_MODE;
        }

        public override void etAfterClick(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);
            
          
        }

       #endregion

        #region ///Initiallization
      
        private void InitiallizeForm()
        {


            oForm.Freeze(true);
            dbOITM = oForm.DataSources.DBDataSources.Item("OITM");



            oItemRef = oForm.Items.Item("39");
            SAPbouiCOM.StaticText lblCode, lblName, lblInt;
            SAPbouiCOM.CheckBox chkAll, chkMon, chkTue, chkWed, chkThs, chkFri, chkSat, chkSun;
            SAPbouiCOM.ComboBox cbInt;
            SAPbouiCOM.EditText txtCode, txtName;


            try
            {


                SAPbouiCOM.Item oItemRef1 = oForm.Items.Item("106");


                oItem = oForm.Items.Add("B1_lbSub", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef1.Left;
                oItem.Width = oItemRef.Width + 40;
                oItem.Visible = true;
                B1_lbSub = (SAPbouiCOM.StaticText)oItem.Specific;

                B1_lbSub.Caption = "Sub Group";

                oItemRef1 = oForm.Items.Item("107");

                oItem = oForm.Items.Add("cbSubGroup", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX);
                oItem.Top = oItemRef.Top;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef1.Left;
                oItem.Width = oItemRef.Width + 90;
                oItem.Visible = true;
                oItem.DisplayDesc = true;
                oItem.LinkTo = "B1_lbSub";
                cbSubGroup =(SAPbouiCOM.ComboBox) oItem.Specific;

                cbSubGroup.DataBind.SetBound(true, "OITM", "U_SubGrp");
            

              //   <databind>
              //    <DataTable UniqueId="dtHead" ColumnUID="RNN"/>
              //  </databind>
              //</specific>



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

        private void fillCmb()
        {
            string subGrpCode = Convert.ToString(dbOITM.GetValue("U_SubGrp", 0));
               
            while (cbSubGroup.ValidValues.Count > 1)
            {
                try
                {
                    cbSubGroup.ValidValues.Remove(1, BoSearchKey.psk_Index);
                }
                catch { }

            }
            if (cbSubGroup.ValidValues.Count == 0)
            {
                cbSubGroup.ValidValues.Add("-1", "[No Sub Group]");
          
            }
            cbSubGroup.Select(0, BoSearchKey.psk_Index);
            

            fillChilds(dbOITM.GetValue("ItmsGrpCod",0),"");

            try
            {
                if (subGrpCode != "-1" && subGrpCode != "")
                {
                    cbSubGroup.Select(subGrpCode.Trim(), BoSearchKey.psk_ByValue);
                }
            }
            catch(Exception ex)
            { 
            
            }


        }


        private void fillChilds(string fatherCode, string Spacer)
        {
            System.Data.DataTable dtRoot = Program.objHrmsUI.getDataTable ( sqlProvider.frm_150_getChild1(fatherCode) , "Fill Root");


            foreach (System.Data.DataRow dr in dtRoot.Rows)
            {
                cbSubGroup.ValidValues.Add(dr["Code"].ToString().Trim(), Spacer + dr["U_SubGrp"].ToString());
                fillChilds(dr["Code"].ToString(), Spacer + dr["U_SubGrp"].ToString() + ">");
            }
        }


        private void openITB()
        {
         
            frm_ITB objScr = new frm_ITB();
            objScr.rootGroup = dbOITM.GetValue("ItmsGrpCod", 0);
            objScr.rootGroupName = dbOITM.GetValue("ItmsGrpNam", 0);
            try
            {
                objScr.CreateForm(oApplication, "ACHR.XMLScreen.ln_English.xml_ITB.xml", oCompany, "frm_ITB");
                oApplication.Forms.Item("frm_ITB").Select();
            }
            catch (Exception ex)
            {
                oApplication.MessageBox(ex.Message);
            }

        }

    
        #endregion

      

    }

}

