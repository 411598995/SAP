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
    class frm_672 : SysBaseForm
    {

        SAPbouiCOM.Item oItem, oItem1, oItemRef;
        SAPbouiCOM.Button B1_IMP;
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
            if (pVal.ItemUID == B1_IMP.Item.UniqueID)
            {
                oApplication.MessageBox("Importing BOM");

                System.Data.DataTable dtItems = dsWEB.getDataTable("Select * from BOM_LINE ");
                foreach (DataRow dr in dtItems.Rows)
                {
                    addItem(dr["Code"].ToString(), dr["ChildName"].ToString());
                }

                System.Data.DataTable dtBOM = dsWEB.getDataTable("Select * from BOM ");
                foreach (DataRow dr in dtBOM.Rows)
                {
                    addItem(dr["Code"].ToString(), dr["ItemName"].ToString());
                    importBOM(dr["Code"].ToString());
                }

                oApplication.MessageBox("Imported!");

            }
        }

       #endregion

        #region ///Initiallization
        public override void AddNewRecord()
        {
            base.AddNewRecord();
            B1_IMP.Item.Visible = false;
           
        }
        public override void etFormAfterDataLoad(ref BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent)
        {
            base.etFormAfterDataLoad(ref BusinessObjectInfo, ref BubbleEvent);

            B1_IMP.Item.Visible = true;
        }
        private void InitiallizeForm()
        {


            oForm.Freeze(true);
         

            oItemRef = oForm.Items.Item("2");
          

            try
            {


                SAPbouiCOM.Item oItemRef1 = oForm.Items.Item("2");


                oItem = oForm.Items.Add("B1_IMP", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                oItem.Top = oItemRef.Top;
                oItem.Height = oItemRef.Height;
                oItem.Left = oItemRef.Left + oItemRef.Width + 5;
                oItem.Width = oItemRef.Width + 80;
                oItem.Visible = true;
                B1_IMP = (SAPbouiCOM.Button)oItem.Specific;

                B1_IMP.Caption = "Import BOM from FPRO";







            }
            catch (Exception ex)
            {

                string message = ex.Message;

            }

            oForm.Freeze(false);
            dsWEB = new DataServices(Program.strExtCon);



        }

        private void addItem(string itemCode, string itemName)
        {
            int cntExist = Convert.ToInt32(Program.objHrmsUI.getScallerValue("Select count(*) from oitm where ItemCode = '" + itemCode + "'"));
            if (cntExist == 0)
            {
                SAPbobsCOM.Items newRetItem;
                newRetItem = (SAPbobsCOM.Items)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems);
                newRetItem.ItemCode = itemCode;
                newRetItem.ItemName = itemName;
               int result= newRetItem.Add();
               if (result == 0)
               {
                   oApplication.SetStatusBarMessage("Item " + itemCode + " Imported Successfully", BoMessageTime.bmt_Short, false);
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

        private void importBOM(string bom)
        {
             int cntExist = Convert.ToInt32(Program.objHrmsUI.getScallerValue("Select count(*) from OITT where Code = '" + bom + "'"));
             if (cntExist == 0)
             {
                 SAPbobsCOM.ProductTrees BOM;
                 BOM = (SAPbobsCOM.ProductTrees)oCompany.GetBusinessObject(BoObjectTypes.oProductTrees);

                 BOM.TreeCode = bom;

                 string strChilds = "Select * from BOM_LINE where FATHER = '" + bom + "'";
                 System.Data.DataTable dtChild = dsWEB.getDataTable(strChilds, "getting childeren");
                 foreach (DataRow dr in dtChild.Rows)
                 {
                     BOM.Items.ItemCode = dr["Code"].ToString();
                     BOM.Items.Quantity = Convert.ToDouble(dr["Quantity"]);
                     BOM.Items.Add();
                 }
                 int result = BOM.Add();

                 if (result == 0)
                 {
                     oApplication.SetStatusBarMessage("BOM " + bom + " Imported Successfully", BoMessageTime.bmt_Short, false);
                 }
                 else
                 {
                     int erroCode = 0;
                     string errDescr = "";
                     string Errmsg = "";
                     oCompany.GetLastError(out erroCode, out errDescr);
                     oApplication.SetStatusBarMessage("BOM " + bom + " Failed to import: " + errDescr, BoMessageTime.bmt_Short, true);



                 }
             }

        }

        #endregion

        #region //Common Methods


    
        #endregion

      

    }

}

