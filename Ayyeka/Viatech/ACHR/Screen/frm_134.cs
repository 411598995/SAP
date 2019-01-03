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
    class frm_134 : SysBaseForm
    {

       

        DataServices dsWEB;
        DataServices dsSAP;


        #region /////Events
        public override void etFormAfterLoad(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etFormAfterLoad(ref pVal, ref BubbleEvent);
          
            InitiallizeForm();
            
            
          //  oApplication.MessageBox("Project Form Loaded");
        }
        public override void etFormBeforeDataUpdate(ref BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent)
        {
            base.etFormBeforeDataUpdate(ref  BusinessObjectInfo, ref  BubbleEvent);
            BubbleEvent = false;
        }
        public override void etAfterClick(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == "B1_tbTS")
            {
                oForm.PaneLevel = 17;
                
            }
            if (pVal.ItemUID == "B1_tbSOT")
            {
                oForm.PaneLevel = 18;

            }
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

            

            
            SAPbouiCOM.Item oItem,oItem1;
            
            SAPbouiCOM.Folder oFolder,oFolder1;
            SAPbouiCOM.Item oItemRef = oForm.Items.Item("9");
            SAPbouiCOM.StaticText lblCode,lblName,lblInt;
            SAPbouiCOM.CheckBox chkAll,chkMon,chkTue,chkWed,chkThs,chkFri,chkSat,chkSun;
            SAPbouiCOM.ComboBox cbInt;
            SAPbouiCOM.EditText txtCode, txtName;

            /*

            try
            {
              
             SAPbouiCOM.Item oItemRef1 = oForm.Items.Item("9");
            SAPbouiCOM.Item oItemlbl = oForm.Items.Item("64");
           
               
                oItem = oForm.Items.Add("B1_tbTS", SAPbouiCOM.BoFormItemTypes.it_FOLDER);
                oItem.Top = oItemRef.Top;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width;
                oItem.Width = oItemRef.Width;
                oItem.Visible = true;
               

                oFolder = (SAPbouiCOM.Folder)oItem.Specific;
                oFolder.Caption = "TeleSales";
                oFolder.GroupWith(oItemRef.UniqueID);
                oFolder.Pane = 17;
                oFolder.AutoPaneSelection = true;


                oItem = oForm.Items.Add("lblCode", SAPbouiCOM.BoFormItemTypes.it_STATIC);


                oItem.Top = oItemlbl.Top;
                oItem.Height = oItemlbl.Height;
                oItem.Left = oItemlbl.Left + oItemRef.Width;
                oItem.Width = oItemlbl.Width;
                oItem.Visible = true;
                oItem.FromPane = 17;
                oItem.ToPane = 17;

                lblCode = (SAPbouiCOM.StaticText)oItem.Specific;
                lblCode.Caption = "Schedule Code";

                oItem = oForm.Items.Add("lblName", SAPbouiCOM.BoFormItemTypes.it_STATIC);


                oItem.Top = 245;
                oItem.Height = 14;
                oItem.Left = 160;
                oItem.Width = 100;
                oItem.Visible = true;
                oItem.FromPane = 17;
                oItem.ToPane = 17;

                lblName = (SAPbouiCOM.StaticText)oItem.Specific;
                lblName.Caption = "Schedule Name";


                //txtCode
                oItem = oForm.Items.Add("txtCode", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                oItem.Top = oItemlbl.Top;
                oItem.Height = oItemlbl.Height;
                oItem.Left = 240;
                oItem.Width = oItemlbl.Width;
                oItem.Visible = true;
                oItem.FromPane = 17;
                oItem.ToPane = 17;

                txtCode = (SAPbouiCOM.EditText)oItem.Specific;
                
                //txtName
                oItem = oForm.Items.Add("txtName", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                oItem.Top = 245;
                oItem.Height = oItemlbl.Height;
                oItem.Left = 240;
                oItem.Width = oItemlbl.Width;
                oItem.Visible = true;
                oItem.FromPane = 17;
                oItem.ToPane = 17;

                txtName = (SAPbouiCOM.EditText)oItem.Specific;


                //lblInterval
                oItem = oForm.Items.Add("lblInt", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = 265;
                oItem.Height = 14;
                oItem.Left = 160;
                oItem.Width = 120;
                oItem.Visible = true;
                oItem.FromPane = 17;
                oItem.ToPane = 17;

                lblInt = (SAPbouiCOM.StaticText)oItem.Specific;
              
                //combo Interval
                oItem = oForm.Items.Add("cbInt", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX);
                oItem.Top = 265;
                oItem.Height = oItemlbl.Height;
                oItem.Left = 240;
                oItem.Width = oItemlbl.Width;
                oItem.Visible = true;
                oItem.FromPane = 17;
                oItem.ToPane = 17;

                cbInt = (SAPbouiCOM.ComboBox)oItem.Specific;




                oItem1 = oForm.Items.Add("B1_tbSOT", SAPbouiCOM.BoFormItemTypes.it_FOLDER);
                oItem1.Top = oItemRef1.Top;
                oItem1.Height = oItemRef1.Height;
                oItem1.Left = oItemRef1.Left + oItemRef.Width;
                oItem1.Width = oItemRef1.Width;
                oItem1.Visible = true;


                oFolder1 = (SAPbouiCOM.Folder)oItem1.Specific;
                oFolder1.Caption = "Standing Order";
                oFolder1.GroupWith(oItemRef1.UniqueID);
                oFolder1.Pane = 18;
                oFolder1.AutoPaneSelection = true;
              
                 
               
                 

            }
            catch(Exception ex) {

                string message = ex.Message;
            
            }
             * */

            oForm.Freeze(false);
            dsSAP = new DataServices(Program.strConSAP);
           


        }


        #endregion

        #region //Common Methods

         

        #endregion

      

    }

}

