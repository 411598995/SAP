using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACHR.Screen
{
    class frm_ITB : HRMSBaseForm
    {

        public bool isForLoading = false;
        SAPbouiCOM.Matrix mtCA;
        SAPbouiCOM.ComboBox cbProj, cbBsUnt;
        SAPbouiCOM.EditText txFCode, txFName, txCode, txName, txActCod, txArea, txVolume, txGrpName;
        SAPbouiCOM.ButtonCombo btAdd;
        SAPbouiCOM.Button btAC, btAS, cmdOK;
        int rowNum = 0;
        public string rootGroup;
        public string rootGroupName;

        SAPbouiCOM.DataTable dtCA, dtHeads;
        string itemClicked = "";


        private bool initiallizing = false;
        string printMenuId = "";
        public override void CreateForm(SAPbouiCOM.Application SboApp, string strXml, SAPbobsCOM.Company cmp, string frmId)
        {
            base.CreateForm(SboApp, strXml, cmp, frmId);

            oForm.EnableMenu("1282", false);  // Add New Record
            oForm.EnableMenu("1288", false);  // Next Record
            oForm.EnableMenu("1289", false);  // Pevious Record
            oForm.EnableMenu("1290", false);  // First Record
            oForm.EnableMenu("1291", false);  // Last record 
            oForm.EnableMenu("1281", false);  // Find record 
            oForm.Settings.MatrixUID = "mtCA";
            oForm.Settings.Enabled = true;
            InitiallizeForm();




        }
        public override void oApplication_RightClickEvent(ref SAPbouiCOM.ContextMenuInfo eventInfo, out bool BubbleEvent)
        {
            base.oApplication_RightClickEvent(ref eventInfo, out BubbleEvent);
            if (eventInfo.BeforeAction)
            {
                SAPbouiCOM.MenuItem oMenuItem = null;
                SAPbouiCOM.Menus oMenus = null;


                try
                {
                    //  Create menu popup MyUserMenu01 and add it to Tools menu
                    SAPbouiCOM.MenuCreationParams oCreationPackage = null;
                    oCreationPackage = ((SAPbouiCOM.MenuCreationParams)(oApplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)));

                    oMenuItem = oApplication.Menus.Item("1280"); // Data'
                    oMenus = oMenuItem.SubMenus;

                    if (!oMenus.Exists("addGrp"))
                    {
                        oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                        oCreationPackage.UniqueID = "addGrp";
                        oCreationPackage.String = "Add new Sub Group";
                        oCreationPackage.Enabled = true;
                        oMenus.AddEx(oCreationPackage); 
            
                    }
                    if (!oMenus.Exists("remGrp"))
                    {
                        oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                        oCreationPackage.UniqueID = "remGrp";
                        oCreationPackage.String = "Remove Sub Group";
                        oCreationPackage.Enabled = true;
                        oMenus.AddEx(oCreationPackage);

                    }

                  

                }
                catch (Exception ex)
                {
                   oApplication.MessageBox(ex.Message);
                } 
            }
        }
        public override void oApplication_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
           base.oApplication_MenuEvent(ref pVal, out BubbleEvent);
            if (pVal.BeforeAction)
            {
                if (pVal.MenuUID == "addGrp")
                {
                    rowNum = mtSelRow(mtCA);
                    txGrpName.Item.Visible = true;
                    oForm.Items.Item("5").Visible = true;
                    txGrpName.Value = "";
                    txGrpName.Active = true;
                }

                if (pVal.MenuUID == "remGrp")
                {
                    rowNum = mtSelRow(mtCA);
                    removeCode();
                }
            }
        }
        public override void etAfterKeyDown(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterKeyDown(ref pVal, ref BubbleEvent);
            if (pVal.CharPressed ==13 && pVal.ItemUID == txGrpName.Item.UniqueID)
            {
                addCode();
                txGrpName.Value = "";

            }
        }
        private void InitiallizeForm()
        {


            oForm.Freeze(true);

            initiallizing = true;


            dtCA = oForm.DataSources.DataTables.Item("dtCA");
            mtCA = (SAPbouiCOM.Matrix)oForm.Items.Item("mtCA").Specific;
            txGrpName = (SAPbouiCOM.EditText)oForm.Items.Item("txGrpName").Specific;


            oForm.Freeze(false);

            initiallizing = false;
            updateCA();


        }

        private void ini_controls()
        {
            dtHeads.SetValue("Code", 0, "");

            dtHeads.SetValue("Name", 0, "");
            dtHeads.SetValue("Area", 0, "0");
            dtHeads.SetValue("Volume", 0, "0");
            dtHeads.SetValue("Floors", 0, "0");

            dtHeads.SetValue("AccCode", 0, "");
            dtHeads.SetValue("AcctName", 0, "");

            dtHeads.SetValue("FAC", 0, "");
            dtHeads.SetValue("FAN", 0, "");


            dtHeads.SetValue("BaseUnit", 0, "01");
            txCode.Active = true;

        }



        private void updateCA()
        {
         SAPbouiCOM.EditText tx = (SAPbouiCOM.EditText)   oForm.Items.Item("6").Specific;
         tx.Active = true;
            txGrpName.Item.Visible = false;
            oForm.Items.Item("5").Visible = false;
            dtCA.Rows.Clear();
           // rowNum = 0;
            fillLavels();
            mtCA.LoadFromDataSource();
            if (rowNum > 0)
            {
                mtCA.SelectRow(rowNum, true,false);
            }
        }

        private void fillCb()
        {


        }
        private void fillLavels()
        {

            dtCA.Rows.Add(1);
            dtCA.SetValue("Code", dtCA.Rows.Count - 1, rootGroup);

            dtCA.SetValue("Name", dtCA.Rows.Count - 1, rootGroupName);
            dtCA.SetValue("Level", dtCA.Rows.Count - 1, 1);

            fillChilds(rootGroup, ".      ",2);

            //System.Data.DataTable dtRoot = Program.objHrmsUI.getDataTable("Select * from [@B1_ITB] where U_Father='" + rootGroup + "'", "Fill Root");

            //if (dtRoot.Rows.Count > 0)
            //{
            //    foreach (System.Data.DataRow dr in dtRoot.Rows)
            //    {
            //        dtCA.Rows.Add(1);
            //        dtCA.SetValue("Code", dtCA.Rows.Count - 1, dr["Code"].ToString());

            //        fillChilds(dr["Code"].ToString(), ".    ");
            //    }
            //}

        }
        private void fillChilds(string fatherCode, string Spacer,int level)
        {
            System.Data.DataTable dtRoot = Program.objHrmsUI.getDataTable("Select * from [@B1_ITB] where U_Father='" + fatherCode + "' order by convert(int,code)", "Fill Root");


            foreach (System.Data.DataRow dr in dtRoot.Rows)
            {
                dtCA.Rows.Add(1);
                dtCA.SetValue("Code", dtCA.Rows.Count - 1, dr["Code"].ToString());

                dtCA.SetValue("Name", dtCA.Rows.Count - 1, Spacer + dr["U_SubGrp"].ToString());
                dtCA.SetValue("Level", dtCA.Rows.Count - 1, level.ToString());
              
                fillChilds(dr["Code"].ToString(), Spacer + "      " , level+1);
            }
        }
        private int mtSelRow(SAPbouiCOM.Matrix mt)
        {
            int selectedrow = 0;

            for (int i = 1; i <= mt.RowCount; i++)
            {
                if (mt.IsRowSelected(i))
                {
                    selectedrow = i;
                    return i;
                }
            }
            return selectedrow;

        }

        private void addCode()
        {
            if(txGrpName.Value.ToString()=="") return;
            int selRow = mtSelRow(mtCA);
            long code = Program.objHrmsUI.getMaxId("[@B1_ITB]", "CODE");

            string strSubGroup = txGrpName.Value.ToString();
            string father = Convert.ToString( dtCA.GetValue("Code", selRow-1));
            int level = Convert.ToInt16(dtCA.GetValue("Level", selRow - 1));

            string strInsert = " Insert Into [@B1_ITB]  (Code, Name, U_Father , U_SubGrp,U_Level ) ";
            strInsert += " Values ('" + code + "','" + code + "','" + father + "','" + strSubGroup + "','"  + level.ToString() +  "')";
          
            int result = Program.objHrmsUI.ExecQuery(strInsert, "Insert Code");

            if (result == 0)
            {
                updateCA();
            }
            else
            {
                oApplication.SetStatusBarMessage("Unable to add Code ");
            }




        }
        private void removeCode()
        {
            int selRow = mtSelRow(mtCA);
           
            string Code = Convert.ToString(dtCA.GetValue("Code", selRow - 1));
            int childExist = Convert.ToInt32( Program.objHrmsUI.getScallerValue( "Select count(*) from [@B1_ITB] where u_father = '" + Code + "'"));
            if (childExist == 0)
            {
                string strDelete = " Delete from [@B1_ITB] where code = '" + Code + "'";
                Program.objHrmsUI.ExecQuery(strDelete, "Deleting Code");
                if (rowNum > 1 && rowNum==mtCA.RowCount) rowNum--;
                updateCA();
            }
            else
            {
                oApplication.MessageBox("Remove linked sub groups first");
            }

        }




    }
}
