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
    class frm_63 : SysBaseForm
    {

        SAPbouiCOM.Item oItem, oItem1, oItemRef;
        SAPbouiCOM.Button B1_ITB;
        SAPbouiCOM.DBDataSource dbOITB;
           

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
            if (pVal.ItemUID == B1_ITB.Item.UniqueID)
            {
                openITB();
            }
        }

       #endregion

        #region ///Initiallization
        public override void AddNewRecord()
        {
            base.AddNewRecord();
            B1_ITB.Item.Visible = false;
           
        }
        public override void etFormAfterDataLoad(ref BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent)
        {
            base.etFormAfterDataLoad(ref BusinessObjectInfo, ref BubbleEvent);

            B1_ITB.Item.Visible = true;
        }
        private void InitiallizeForm()
        {


            oForm.Freeze(true);
            dbOITB = oForm.DataSources.DBDataSources.Item("OITB");



            oItemRef = oForm.Items.Item("2");
            SAPbouiCOM.StaticText lblCode, lblName, lblInt;
            SAPbouiCOM.CheckBox chkAll, chkMon, chkTue, chkWed, chkThs, chkFri, chkSat, chkSun;
            SAPbouiCOM.ComboBox cbInt;
            SAPbouiCOM.EditText txtCode, txtName;


            try
            {


                SAPbouiCOM.Item oItemRef1 = oForm.Items.Item("9");


                oItem = oForm.Items.Add("B1_ITB", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                oItem.Top = oItemRef.Top;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width + 5;
                oItem.Width = oItemRef.Width + 40;
                oItem.Visible = true;
                B1_ITB = (SAPbouiCOM.Button)oItem.Specific;

                B1_ITB.Caption = "Item Sub Groups";







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



        private void openITB()
        {
         
            frm_ITB objScr = new frm_ITB();
            objScr.rootGroup = dbOITB.GetValue("ItmsGrpCod", 0);
            objScr.rootGroupName = dbOITB.GetValue("ItmsGrpNam", 0);
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

