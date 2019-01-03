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
    class frm_149 : SysBaseForm
    {

        SAPbouiCOM.Item oItem, oItem1, oItemRef;
        SAPbouiCOM.Matrix mtItems;
        SAPbouiCOM.ComboBox cbIG, cbPM1, cbPM2, cbPM7 ;
        SAPbouiCOM.CheckBox cbPM3, cbPM4, cbPM5, cbPM6, cbPM8;
        SAPbouiCOM.Button B1_IMP,btNI;
        SAPbouiCOM.DBDataSource dbOITB,dbQUT1;
        SAPbouiCOM.DataTable dtItem;
        SAPbouiCOM.EditText txItCode , txName,txFName,txAI,txRem,txL,txW,txH;
        SAPbouiCOM.PictureBox ItemImage;
        DataServices dsWEB;
        DataServices dsSAP;

        string[] checkboxes = { "cbPM3", "cbPM4", "cbPM5", "cbPM6", "cbPM8" };

                 

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
         
            if (pVal.ItemUID == "fldNI")
            {
                oForm.PaneLevel = 101;
            }
            if (pVal.ItemUID == "38" && pVal.ColUID == "0" && pVal.Row < mtItems.RowCount)
            {
               // mtItems.FlushToDataSource();
                string itemCode =( (SAPbouiCOM.EditText)mtItems.Columns.Item("1").Cells.Item(pVal.Row).Specific).Value.ToString(); //Convert.ToString(dbQUT1.GetValue("ItemCode", pVal.Row-1 ));
                if (itemCode != null && itemCode != "")
                {
                    string pictureCode = Program.objHrmsUI.getScallerValue("Select U_Picture from oitm where itemcode='" + itemCode + "'").ToString();
                    if (pictureCode != null)
                    {
                        ItemImage.Picture = oCompany.BitMapPath + "\\" + pictureCode;
                    }
                }
            }
            if (checkboxes.Contains(pVal.ItemUID))
            {
                updateItemDetail();
            }
            if (pVal.ItemUID == btNI.Item.UniqueID)
            {

                if (txItCode.Value != "" && txName.Value.ToString() != "" )
                {
                    SAPbouiCOM.EditText cardCode = (SAPbouiCOM.EditText)oForm.Items.Item("4").Specific;
                    if (cardCode.Value != "")
                    {
                        addItem();
                    }
                    else
                    {
                        oApplication.MessageBox("Please confirm if BP Selected ");
                    }
                }
                else
                {
                    oApplication.MessageBox("Please confirm if BP Selected and required info provided!");
                }
            }


          
        }
        private void addItem()
        {

            string itemCode = txItCode.Value.ToString();

            int cntExist = Convert.ToInt32(Program.objHrmsUI.getScallerValue("Select count(*) from oitm where ItemCode = '" + itemCode + "'"));
            if (cntExist == 0)
            {
                SAPbobsCOM.Items newRetItem;
                newRetItem = (SAPbobsCOM.Items)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems);
                newRetItem.ItemCode = itemCode;
                newRetItem.ItemName = txName.Value.ToString();
                newRetItem.ItemsGroupCode = Convert.ToInt16( cbIG.Selected.Value.ToString());
                newRetItem.SalesUnitLength = txL.Value != "" ? Convert.ToDouble(txL.Value) : 0.00 ;
                newRetItem.SalesUnitWidth = txW.Value != "" ? Convert.ToDouble(txW.Value) : 0.00;
                newRetItem.SalesUnitHeight = txH.Value != "" ? Convert.ToDouble(txH.Value) : 0.00;
              
                newRetItem.UserFields.Fields.Item("U_PARAM1").Value = cbPM1.Value.ToString();
                newRetItem.UserFields.Fields.Item("U_PARAM2").Value = cbPM2.Value.ToString();
                newRetItem.UserFields.Fields.Item("U_PARAM3").Value = cbPM3.Checked == true ? "Y" : "N";
                newRetItem.UserFields.Fields.Item("U_PARAM4").Value = cbPM4.Checked==true ? "Y":"N";
                newRetItem.UserFields.Fields.Item("U_PARAM5").Value = cbPM5.Checked==true?"Y":"N";
                newRetItem.UserFields.Fields.Item("U_PARAM6").Value = cbPM6.Checked == true ? "Y" : "N";
                newRetItem.UserFields.Fields.Item("U_PARAM7").Value = cbPM7.Value.ToString();
                newRetItem.UserFields.Fields.Item("U_PARAM8").Value = cbPM8.Checked == true ? "Y" : "N";
              
                int result = newRetItem.Add();
                if (result == 0)
                {
                    oApplication.SetStatusBarMessage("Item " + itemCode + " Added Successfully", BoMessageTime.bmt_Short, false);
                    txName.Value = "";
                    SAPbouiCOM.EditText newItemCode = (SAPbouiCOM.EditText) mtItems.Columns.Item("1").Cells.Item(mtItems.RowCount).Specific;
                    SAPbouiCOM.Folder general = (SAPbouiCOM.Folder) oForm.Items.Item("112").Specific;
                    general.Select();
                    oForm.PaneLevel = 1;
                    newItemCode.Value = itemCode;
                    





          

                   // txName.Active = true;
                }
                else
                {
                    int erroCode = 0;
                    string errDescr = "";
                    string Errmsg = "";
                    oCompany.GetLastError(out erroCode, out errDescr);
                    oApplication.SetStatusBarMessage("Item " + itemCode + " Failed to import: " + errDescr, BoMessageTime.bmt_Short, true);



                }
            }
        }
        public override void etAfterCmbSelect(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCmbSelect(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID != "" )
            {
                try
                {
                    updateItemDetail();
                }
                catch { }
            }
        }
        private void updateItemDetail()
        {
            try
            {
                string p3C = cbPM3.Checked ? "FR" : "";
                string p4C = cbPM4.Checked ? "PN" : "";
                string p5C = cbPM5.Checked ? "GL" : "";
                string p6C = cbPM6.Checked ? "FC" : "";
                string p8C = cbPM8.Checked ? "PV" : "";
                string p3N = cbPM3.Checked ? " Frame" : "";
                string p4N = cbPM4.Checked ? " Panel" : "";
                string p5N = cbPM5.Checked ? " Glass" : "";
                string p6N = cbPM6.Checked ? " FlyScreen" : "";
                string p8N = cbPM8.Checked ? " PV Box" : "";

                string itemCode = cbPM1.Selected.Value.ToString() + cbPM2.Selected.Value.ToString() + p3C + p4C + p5C + p6C + cbPM7.Selected.Value.ToString() + p8C;
                string ItemName = cbPM1.Selected.Description.ToString() + " " + cbPM2.Selected.Description.ToString() +  p3N +  p4N +  p5N +  p6N + cbPM7.Selected.Description.ToString() +  p8N + txL.Value.ToString() + "X" + txW.Value.ToString() + "X" + txH.Value.ToString();

                txItCode.Item.Enabled = true;
                txItCode.Value = itemCode;
                txName.Value = ItemName;
                txName.Active = true;
                txItCode.Item.Enabled = false;
            }
            catch { }
        }

       #endregion

        #region ///Initiallization
        public override void AddNewRecord()
        {
            base.AddNewRecord();
       //     ItemImage.Item.Visible = false;
           
        }
        public override void etFormAfterDataLoad(ref BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent)
        {
            base.etFormAfterDataLoad(ref BusinessObjectInfo, ref BubbleEvent);

            ItemImage.Item.Visible = true;
        }
        private void InitiallizeForm()
        {

            //dtItem = oForm.DataSources.DataTables.Add("dtItem");

            //dtItem.Columns.Add("ItemCode", BoFieldsType.ft_AlphaNumeric, 50);
            //dtItem.Columns.Add("ItemName", BoFieldsType.ft_AlphaNumeric, 100);
            //dtItem.Columns.Add("FrignName", BoFieldsType.ft_AlphaNumeric, 100);
            //dtItem.Columns.Add("ItemGroup", BoFieldsType.ft_AlphaNumeric, 50);
            //dtItem.Columns.Add("AI", BoFieldsType.ft_AlphaNumeric, 100);
            //dtItem.Columns.Add("Remarks", BoFieldsType.ft_AlphaNumeric, 100);

                
            oForm.Freeze(true);
            dbQUT1 = oForm.DataSources.DBDataSources.Item("QUT1");

            mtItems = (SAPbouiCOM.Matrix)oForm.Items.Item("38").Specific;

            oItemRef = oForm.Items.Item("20");


            try
            {
             
                oForm.DataSources.UserDataSources.Add("FolderDS", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1);

                oItem = oForm.Items.Add("ItemImage", SAPbouiCOM.BoFormItemTypes.it_PICTURE);
                oItem.Top = oItemRef.Top;
                oItem.Height = oItemRef.Height + 100;
                oItem.Left = oItemRef.Left + oItemRef.Width + 20;
                oItem.Width = oItemRef.Width + 200;
                oItem.Visible = true;
                ItemImage = (SAPbouiCOM.PictureBox)oItem.Specific;
                oItem.LinkTo = oItemRef.UniqueID;

                oItemRef = oForm.Items.Item("138");

                oItem = oForm.Items.Add("fldNI", SAPbouiCOM.BoFormItemTypes.it_FOLDER);
                oItem.Width = oItemRef.Width;
                oItem.Left = oItemRef.Left + 400;
                oItem.Top = oItemRef.Top;
                oItem.Height = oItemRef.Height;


                SAPbouiCOM.Folder fldNI = (SAPbouiCOM.Folder)oItem.Specific;
                fldNI.Pane = 101;
                fldNI.AutoPaneSelection = true;
                fldNI.Caption = "New Item";
                fldNI.DataBind.SetBound(true, "", "FolderDS");

                fldNI.GroupWith("1320002137");



              
              



                oItemRef = oForm.Items.Item("48");

                oItem = oForm.Items.Add("lblItCode", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top-60;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left;
                oItem.Width = oItemRef.Width;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "txItCode";
                SAPbouiCOM.StaticText lblItCode = (SAPbouiCOM.StaticText)oItem.Specific;
                lblItCode.Caption = "Item Code";

                oItem = oForm.Items.Add("txItCode", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                oItem.Top = oItemRef.Top-60;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width;
                oItem.Width = 150;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "lblItCode";
           txItCode = (SAPbouiCOM.EditText)oItem.Specific;
              //  txItCode.DataBind.SetBound(true, "dtItem", "ItemCode");
       

                oItemRef = oForm.Items.Item("lblItCode");
                oItem = oForm.Items.Add("lblName", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left;
                oItem.Width = oItemRef.Width;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "lblItCode";
                SAPbouiCOM.StaticText lblName = (SAPbouiCOM.StaticText)oItem.Specific;
                lblName.Caption = "Description";
             
                oItem = oForm.Items.Add("txName", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width;
                oItem.Width = 250;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "lblName";
        txName = (SAPbouiCOM.EditText)oItem.Specific;





        oItemRef = oForm.Items.Item("lblName");
                oItem = oForm.Items.Add("lblIG", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left;
                oItem.Width = oItemRef.Width;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "lblName";
                SAPbouiCOM.StaticText lblIG = (SAPbouiCOM.StaticText)oItem.Specific;
                lblIG.Caption = "Item Group";

                oItem = oForm.Items.Add("cbIG", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width;
                oItem.Width = 150;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "lblIG";
                cbIG = (SAPbouiCOM.ComboBox)oItem.Specific;
                oItem.DisplayDesc = true;
               
                oItemRef = oForm.Items.Item("lblIG");
                oItem = oForm.Items.Add("lblPM1", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left;
                oItem.Width = oItemRef.Width;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "lblIG";
                SAPbouiCOM.StaticText lblPM1 = (SAPbouiCOM.StaticText)oItem.Specific;
                lblPM1.Caption = "Type ";

                oItem = oForm.Items.Add("cbPM1", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width;
                oItem.Width = 150;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "cbIG";
                cbPM1 = (SAPbouiCOM.ComboBox)oItem.Specific;
                oItem.DisplayDesc = true;
               



                oItemRef = oForm.Items.Item("lblPM1");
                oItem = oForm.Items.Add("lblPM2", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left;
                oItem.Width = oItemRef.Width;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "lblPM1";
                SAPbouiCOM.StaticText lblPM2 = (SAPbouiCOM.StaticText)oItem.Specific;
                lblPM2.Caption = "Style";

                oItem = oForm.Items.Add("cbPM2", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width;
                oItem.Width = 150;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "cbPM1";
                cbPM2 = (SAPbouiCOM.ComboBox)oItem.Specific;
                oItem.DisplayDesc = true;
               
                oItemRef = oForm.Items.Item("lblPM2");
                oItem = oForm.Items.Add("lblPM3", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left;
                oItem.Width = oItemRef.Width;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "lblPM2";
                SAPbouiCOM.StaticText lblPM3 = (SAPbouiCOM.StaticText)oItem.Specific;
                lblPM3.Caption = "Frame";

                oItem = oForm.Items.Add("cbPM3", SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width;
                oItem.Width = 150;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "cbPM2";
               
                cbPM3 = (SAPbouiCOM.CheckBox)oItem.Specific;
                oForm.DataSources.UserDataSources.Add("cbPM3", SAPbouiCOM.BoDataType.dt_SHORT_TEXT); // Days of Month
                cbPM3.DataBind.SetBound(true, "", "cbPM3");

                oItem.DisplayDesc = true;
               
                oItemRef = oForm.Items.Item("lblPM3");
                oItem = oForm.Items.Add("lblPM4", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left;
                oItem.Width = oItemRef.Width;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "lblPM3";
                SAPbouiCOM.StaticText lblPM4 = (SAPbouiCOM.StaticText)oItem.Specific;
                lblPM4.Caption = "Panel";

                oItem = oForm.Items.Add("cbPM4", SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width;
                oItem.Width = 150;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "cbPM3";
                oItem.DisplayDesc = true;
                cbPM4 = (SAPbouiCOM.CheckBox)oItem.Specific;
                oForm.DataSources.UserDataSources.Add("cbPM4", SAPbouiCOM.BoDataType.dt_SHORT_TEXT); // Days of Month
                cbPM4.DataBind.SetBound(true, "", "cbPM4");


                oItemRef = oForm.Items.Item("lblPM4");
                oItem = oForm.Items.Add("lblPM5", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left;
                oItem.Width = oItemRef.Width;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "lblPM4";
                SAPbouiCOM.StaticText lblPM5 = (SAPbouiCOM.StaticText)oItem.Specific;
                lblPM5.Caption = "Glass";

                oItem = oForm.Items.Add("cbPM5", SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width;
                oItem.Width = 150;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "cbPM4";
                oItem.DisplayDesc = true;
                cbPM5 = (SAPbouiCOM.CheckBox)oItem.Specific;
                oForm.DataSources.UserDataSources.Add("cbPM5", SAPbouiCOM.BoDataType.dt_SHORT_TEXT); // Days of Month
                cbPM5.DataBind.SetBound(true, "", "cbPM5");


                oItemRef = oForm.Items.Item("lblPM5");
                oItem = oForm.Items.Add("lblPM6", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left;
                oItem.Width = oItemRef.Width;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "lblPM5";
                SAPbouiCOM.StaticText lblPM6 = (SAPbouiCOM.StaticText)oItem.Specific;
                lblPM6.Caption = "FlyScreen";

                oItem = oForm.Items.Add("cbPM6", SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width;
                oItem.Width = 150;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "cbPM5";
                oItem.DisplayDesc = true;
                cbPM6 = (SAPbouiCOM.CheckBox)oItem.Specific;
                oForm.DataSources.UserDataSources.Add("cbPM6", SAPbouiCOM.BoDataType.dt_SHORT_TEXT); // Days of Month
                cbPM6.DataBind.SetBound(true, "", "cbPM6");

                oItemRef = oForm.Items.Item("lblPM6");
                oItem = oForm.Items.Add("lblPM7", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left;
                oItem.Width = oItemRef.Width;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "lblPM6";
                SAPbouiCOM.StaticText lblPM7 = (SAPbouiCOM.StaticText)oItem.Specific;
                lblPM7.Caption = "Color";

                oItem = oForm.Items.Add("cbPM7", SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width;
                oItem.Width = 150;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "cbPM6";
                oItem.DisplayDesc = true;
                cbPM7 = (SAPbouiCOM.ComboBox)oItem.Specific;

                oItemRef = oForm.Items.Item("lblPM7");
                oItem = oForm.Items.Add("lblPM8", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left;
                oItem.Width = oItemRef.Width;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "lblPM7";
                SAPbouiCOM.StaticText lblPM8 = (SAPbouiCOM.StaticText)oItem.Specific;
                lblPM8.Caption = "PV Box";

                oItem = oForm.Items.Add("cbPM8", SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width;
                oItem.Width = 150;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "cbPM7";
                oItem.DisplayDesc = true;
                cbPM8 = (SAPbouiCOM.CheckBox)oItem.Specific;
                oForm.DataSources.UserDataSources.Add("cbPM8", SAPbouiCOM.BoDataType.dt_SHORT_TEXT); // Days of Month
                cbPM8.DataBind.SetBound(true, "", "cbPM8");


                oItemRef = oForm.Items.Item("cbPM1");
                oItem = oForm.Items.Add("lblLength", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top;
                oItem.Height = oItemRef.Height ;
                oItem.Left = oItemRef.Left + 300;
                oItem.Width = oItemRef.Width;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = oItemRef.UniqueID;
                SAPbouiCOM.StaticText lblLength = (SAPbouiCOM.StaticText)oItem.Specific;
                lblLength.Caption = "Length";

                oItem = oForm.Items.Add("txL", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                oItem.Top = oItemRef.Top ;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + 400;
                oItem.Width = 150;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "cbPM8";
                oItem.DisplayDesc = true;
                txL = (SAPbouiCOM.EditText)oItem.Specific;
                oForm.DataSources.UserDataSources.Add("txL", SAPbouiCOM.BoDataType.dt_SHORT_TEXT); // Days of Month
                txL.DataBind.SetBound(true, "", "txL");

                oItemRef = oForm.Items.Item("lblLength");
                oItem = oForm.Items.Add("lblWidth", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left;
                oItem.Width = oItemRef.Width;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "lblLength";
                SAPbouiCOM.StaticText lblW = (SAPbouiCOM.StaticText)oItem.Specific;
                lblW.Caption = "Width";

                oItemRef = oForm.Items.Item("txL");
               
                oItem = oForm.Items.Add("txW", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left ;
                oItem.Width = 150;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "txL";
                oItem.DisplayDesc = true;
                txW = (SAPbouiCOM.EditText)oItem.Specific;
                oForm.DataSources.UserDataSources.Add("txW", SAPbouiCOM.BoDataType.dt_SHORT_TEXT); // Days of Month
                txW.DataBind.SetBound(true, "", "txW");

                oItemRef = oForm.Items.Item("lblWidth");
                oItem = oForm.Items.Add("lblHeight", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left;
                oItem.Width = oItemRef.Width;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "lblWidth";
                SAPbouiCOM.StaticText lblH = (SAPbouiCOM.StaticText)oItem.Specific;
                lblH.Caption = "Height";

                oItemRef = oForm.Items.Item("txW");
              
                oItem = oForm.Items.Add("txH", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left ;
                oItem.Width = 150;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "txW";
                oItem.DisplayDesc = true;
                txH = (SAPbouiCOM.EditText)oItem.Specific;
                oForm.DataSources.UserDataSources.Add("txH", SAPbouiCOM.BoDataType.dt_SHORT_TEXT); // Days of Month
                txH.DataBind.SetBound(true, "", "txH");

                oItemRef = oForm.Items.Item("txH");
             
                oItem = oForm.Items.Add("btNI", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                oItem.Top = oItemRef.Top + oItemRef.Height + 2;
                oItem.Height = oItemRef.Height+5;
                oItem.Left = oItemRef.Left;
                oItem.Width = 150;
                oItem.FromPane = 101;
                oItem.ToPane = 101;
                oItem.LinkTo = "cbPM5";
                btNI = (SAPbouiCOM.Button)oItem.Specific;
              
                btNI.Caption = "Add New Item";

            
            }
            catch (Exception ex)
            {

                string message = ex.Message;

            }
            try
            {
                fillPrs();
            }
            catch { }
            oForm.Freeze(false);
            dsWEB = new DataServices(Program.strExtCon);



        }
        private void fillPrs()
        {
            string strVals = "Select Code,Name from [@B1_PARAM1]";
            System.Data.DataTable dtVal = Program.objHrmsUI.getDataTable(strVals, "FillVal");
            int i = 0;
            foreach (DataRow dr in dtVal.Rows)
            {
                cbPM1.ValidValues.Add(dr["Code"].ToString(), dr["Name"].ToString());
                i++;
            }
            if (i > 0) cbPM1.Select(0, BoSearchKey.psk_Index);



             strVals = "Select Code,Name from [@B1_PARAM2]";
            dtVal = Program.objHrmsUI.getDataTable(strVals, "FillVal");
             i = 0;
            foreach (DataRow dr in dtVal.Rows)
            {
                cbPM2.ValidValues.Add(dr["Code"].ToString(), dr["Name"].ToString());
                i++;
            }
            if (i > 0) cbPM2.Select(0, BoSearchKey.psk_Index);

            //strVals = "Select Code,Name from [@B1_PARAM3]";
            //dtVal = Program.objHrmsUI.getDataTable(strVals, "FillVal");
            //i = 0;
            //foreach (DataRow dr in dtVal.Rows)
            //{
            //    cbPM3.ValidValues.Add(dr["Code"].ToString(), dr["Name"].ToString());
            //    i++;
            //}
            //if (i > 0) cbPM3.Select(0, BoSearchKey.psk_Index);
            //strVals = "Select Code,Name from [@B1_PARAM4]";
            //dtVal = Program.objHrmsUI.getDataTable(strVals, "FillVal");
            //i = 0;
            //foreach (DataRow dr in dtVal.Rows)
            //{
            //    cbPM4.ValidValues.Add(dr["Code"].ToString(), dr["Name"].ToString());
            //    i++;
            //}
            //if (i > 0) cbPM4.Select(0, BoSearchKey.psk_Index);
            
            //strVals = "Select Code,Name from [@B1_PARAM5]";
            //dtVal = Program.objHrmsUI.getDataTable(strVals, "FillVal");
            //i = 0;
            //foreach (DataRow dr in dtVal.Rows)
            //{
            //    cbPM5.ValidValues.Add(dr["Code"].ToString(), dr["Name"].ToString());
            //    i++;
            //}
            //if (i > 0) cbPM5.Select(0, BoSearchKey.psk_Index);


            //strVals = "Select Code,Name from [@B1_PARAM6]";
            //dtVal = Program.objHrmsUI.getDataTable(strVals, "FillVal");
            //i = 0;
            //foreach (DataRow dr in dtVal.Rows)
            //{
            //    cbPM6.ValidValues.Add(dr["Code"].ToString(), dr["Name"].ToString());
            //    i++;
            //}
            //if (i > 0) cbPM6.Select(0, BoSearchKey.psk_Index);
            strVals = "Select Code,Name from [@B1_PARAM7]";
            dtVal = Program.objHrmsUI.getDataTable(strVals, "FillVal");
            i = 0;
            foreach (DataRow dr in dtVal.Rows)
            {
                cbPM7.ValidValues.Add(dr["Code"].ToString(), dr["Name"].ToString());
                i++;
            }
            if (i > 0) cbPM7.Select(0, BoSearchKey.psk_Index);
            //strVals = "Select Code,Name from [@B1_PARAM8]";
            //dtVal = Program.objHrmsUI.getDataTable(strVals, "FillVal");
            //i = 0;
            //foreach (DataRow dr in dtVal.Rows)
            //{
            //    cbPM8.ValidValues.Add(dr["Code"].ToString(), dr["Name"].ToString());
            //    i++;
            //}
            //if (i > 0) cbPM8.Select(0, BoSearchKey.psk_Index);

            strVals = "Select ItmsGrpCod,ItmsGrpNam from [OITB]";
            dtVal = Program.objHrmsUI.getDataTable(strVals, "FillVal");
            i = 0;
            foreach (DataRow dr in dtVal.Rows)
            {
                cbIG.ValidValues.Add(dr["ItmsGrpCod"].ToString(), dr["ItmsGrpNam"].ToString());
                i++;
            }
            if (i > 0) cbIG.Select(0, BoSearchKey.psk_Index);


        }


      
        #endregion

        #region //Common Methods


    
        #endregion

      

    }

}

