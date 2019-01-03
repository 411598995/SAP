using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;

namespace ACHR.Screen
{
    class frm_AS : HRMSBaseForm
    {

        public bool isForLoading = false;
        SAPbouiCOM.Matrix mtList, mtA,mtS;
        SAPbouiCOM.EditText txCode, txName;
        SAPbouiCOM.Button btAdd, btAddA, btDelA;
        SAPbouiCOM.OptionBtn opI, opG;
        SAPbouiCOM.ChooseFromList cflCode,cflGroup;
        SAPbouiCOM.DataTable dtList, dtHead, dtA, dtS;
        int rowNum = 0;


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
          
            InitiallizeForm();

            // oForm.Items.Item("btUpd").Visible = false;



        }
        public override void etAfterCmbSelect(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCmbSelect(ref pVal, ref BubbleEvent);
           

        }
       
        public override void etAfterClick(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);
            if(pVal.ItemUID == opG.Item.UniqueID || pVal.ItemUID == opI.Item.UniqueID)
            {
                if (opI.Selected)
                {
                    txCode.ChooseFromListUID = "cflCode";
                    txCode.ChooseFromListAlias = "CardCode";

                }
                else
                {
                    txCode.ChooseFromListUID = "cflGroup";
                    txCode.ChooseFromListAlias = "ItmsGrpCod";

                }
                getCodes();
            }

            if(pVal.ItemUID == btAdd.Item.UniqueID)
            {
                string code = Convert.ToString(dtHead.GetValue("Code", 0));
                string name =  Convert.ToString(dtHead.GetValue("Name", 0));
                string codeType = opG.Selected ? "Group" : "Item";
                string strCode = Program.objHrmsUI.getMaxId("@B1_QA_ATTR_CODES", "Code").ToString();
                if (code != "")
                {
                    string strInsert = "INSERT INTO \"@B1_QA_ATTR_CODES\" (\"Code\",\"Name\",\"U_Descr\",\"U_CodeType\") VALUES ('" + strCode + "','" + code + "','" + name + "','" + codeType + "')";


                    Program.objHrmsUI.ExecQuery(strInsert, "Insert New Code");
                    dtHead.SetValue("Code", 0, "");
                    dtHead.SetValue("Name", 0, "");
                    getCodes();
                }
                else
                {
                    oApplication.MessageBox("Please select Code");
                }
            }


            if (pVal.ItemUID == btAddA.Item.UniqueID)
            {
                int selListRow = mtSelRow(mtList);

                string IGcode = Convert.ToString(dtList.GetValue("Code", selListRow-1));
                string codeType = opG.Selected ? "Group" : "Item";
                int selRow = mtSelRow(mtA);
                if (selRow > 0)
                {
                    string strCode = Convert.ToString(dtA.GetValue("Code", selRow - 1));
                    string strNCode = Program.objHrmsUI.getMaxId("@B1_QA_CODES_ATTR", "Code").ToString();

                    string strInsert = "INSERT INTO  \"@B1_QA_CODES_ATTR\" (\"Code\",\"Name\",\"U_IGCode\") VALUES ('" + strNCode + "','" + strCode + "','" + IGcode + "')";


                    Program.objHrmsUI.ExecQuery(strInsert, "Insert New Code");
                    getCodesAttr(IGcode);
                }
                else
                {
                    oApplication.MessageBox("Please select Code");
                }
            }

            if (pVal.ItemUID == btDelA.Item.UniqueID)
            {
                int selListRow = mtSelRow(mtList);

                string IGcode = Convert.ToString(dtList.GetValue("Code", selListRow - 1));
                string codeType = opG.Selected ? "Group" : "Item";
                int selRow = mtSelRow(mtS);
                if (selRow > 0)
                {
                    string strCode = Convert.ToString(dtS.GetValue("Code", selRow - 1));

                    string strInsert = "DELETE FROM  \"@B1_QA_CODES_ATTR\" WHERE \"Name\" = '" + strCode + "' AND \"U_IGCode\" = '" + IGcode + "'";


                    Program.objHrmsUI.ExecQuery(strInsert, "REMOVE  Code");
                    getCodesAttr(IGcode);
                }
                else
                {
                    oApplication.MessageBox("Please select Code");
                }
            }

            if (pVal.ItemUID == mtList.Item.UniqueID && pVal.Row > 0 && pVal.Row <= mtList.RowCount)
            {
                mtList.SelectRow(pVal.Row, true, false);
                int selRow = pVal.Row;// mtSelRow(mtList);
                if (selRow > 0)
                {
                    string strCode = Convert.ToString(dtList.GetValue("Code", selRow - 1));

                    if (strCode.Trim() != "")
                    {
                        
                            getCodesAttr(strCode);

                     

                    }
                }
                else
                {
                    oApplication.MessageBox("Please select rule to delete");
                }
            }


            if ((pVal.ItemUID == mtA.Item.UniqueID)  &&  pVal.Row > 0 && pVal.Row <= mtA.RowCount)
            {
                mtA.SelectRow(pVal.Row, true, false);
              
            }

            if ((pVal.ItemUID == mtS.Item.UniqueID) && pVal.Row > 0 && pVal.Row <= mtS.RowCount)
            {
                mtS.SelectRow(pVal.Row, true, false);

            }

        }
        public override void etAfterCfl(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCfl(ref pVal, ref BubbleEvent);
            SAPbouiCOM.IChooseFromListEvent oCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pVal;
            SAPbouiCOM.DataTable dtSel = oCFLEvento.SelectedObjects;
            if (pVal.ItemUID == txCode.Item.UniqueID)
            {
                if (dtSel.Rows.Count > 0)
                {
                    string strCode = "";
                    string strName = "";
                    if (oCFLEvento.ChooseFromListUID == "cflCode")
                    {
                        strCode = dtSel.GetValue("ItemCode", 0).ToString();
                        strName = dtSel.GetValue("ItemName", 0).ToString();
                    }
                    if (oCFLEvento.ChooseFromListUID == "cflGroup")
                    {
                        strCode = dtSel.GetValue("ItmsGrpCod", 0).ToString();
                        strName = dtSel.GetValue("ItmsGrpNam", 0).ToString();
                    }
                     dtHead.SetValue("Code", 0, strCode);
                    dtHead.SetValue("Name", 0, strName);
                  

                }
            }
        }


        private void InitiallizeForm()
        {


            oForm.Freeze(true);

            initiallizing = true;

            dtA = oForm.DataSources.DataTables.Item("dtA");
            dtHead = oForm.DataSources.DataTables.Item("dtHead");
            dtS = oForm.DataSources.DataTables.Item("dtS");
            dtList = oForm.DataSources.DataTables.Item("dtList");

            mtA = (SAPbouiCOM.Matrix)oForm.Items.Item("mtA").Specific;
            mtS = (SAPbouiCOM.Matrix)oForm.Items.Item("mtS").Specific;
            mtList = (SAPbouiCOM.Matrix)oForm.Items.Item("mtList").Specific;

            dtHead.Rows.Add(1);
           

            txCode = (SAPbouiCOM.EditText)oForm.Items.Item("txCode").Specific;
            txName = (SAPbouiCOM.EditText)oForm.Items.Item("txName").Specific;
           
            btAdd = (SAPbouiCOM.Button)oForm.Items.Item("btAdd").Specific;
            btAddA = (SAPbouiCOM.Button)oForm.Items.Item("btAddA").Specific;
            btDelA = (SAPbouiCOM.Button)oForm.Items.Item("btDelA").Specific;
           

            opI = (SAPbouiCOM.OptionBtn)oForm.Items.Item("opI").Specific;
            opG = (SAPbouiCOM.OptionBtn)oForm.Items.Item("opG").Specific;


            cflCode = (SAPbouiCOM.ChooseFromList)oForm.ChooseFromLists.Item("cflCode");
            cflGroup = (SAPbouiCOM.ChooseFromList)oForm.ChooseFromLists.Item("cflGroup");




            opG.GroupWith("opI");
            opG.Selected = true;


            // oForm.Items.Item("btUpd").Enabled = false;
            oForm.Freeze(false);

            initiallizing = false;
            getCodes();


        }

        private void getCodes()
        {
            dtList.Rows.Clear();
            string codeType = "Group";
            if (opI.Selected) codeType = "Item";

            string strSql = "SELECT \"Name\",\"U_Descr\" FROM \"@B1_QA_ATTR_CODES\" WHERE \"U_CodeType\" = '"+ codeType + "'";

            System.Data.DataTable dtCodes = Program.objHrmsUI.getDataTable(strSql, "gettingCodes");
            int i = 0;

            foreach (System.Data.DataRow dr in dtCodes.Rows)
            {
                i++;
                dtList.Rows.Add(1);
                dtList.SetValue("Id", i - 1, i.ToString());
                dtList.SetValue("Code", i - 1, dr["Name"].ToString());
                dtList.SetValue("Name", i - 1, dr["U_Descr"].ToString());

            }
            mtList.LoadFromDataSource();

        }

        private void ini_controls()
        {


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

        private int ARInIDRCnt(string AR)
        {
            int result = 0;
            string strQuery = "SELECT COUNT(*) FROM \"@B1_IBOQ\" WHERE \"U_AR\" = '" + AR + "'";
            result = Convert.ToInt32(Program.objHrmsUI.getScallerValue(strQuery));

            return result;
        }

        private void getCodesAttr(string Code)
        {
            string codeType = opG.Selected ? "Group" : "Item";

            string strAttrAll = "SELECT \"Code\" , \"U_Descr\" FROM \"@B1_QA_ATTR\" WHERE  \"Code\" NOT IN (SELECT \"Name\" FROM \"@B1_QA_CODES_ATTR\" WHERE \"U_IGCode\" = '" + Code + "') ";
            System.Data.DataTable dtCodes = Program.objHrmsUI.getDataTable(strAttrAll, "gettingCodes");
            int i = 0;
            dtA.Rows.Clear();
            foreach (System.Data.DataRow dr in dtCodes.Rows)
            {
                i++;
                dtA.Rows.Add(1);
                dtA.SetValue("Id", i - 1, i.ToString());
                dtA.SetValue("Code", i - 1, dr["Code"].ToString());
                dtA.SetValue("Name", i - 1, dr["U_Descr"].ToString());

            }
            mtA.LoadFromDataSource();



            strAttrAll = "SELECT \"Code\" , \"U_Descr\" FROM \"@B1_QA_ATTR\" WHERE  \"Code\"  IN (SELECT \"Name\" FROM \"@B1_QA_CODES_ATTR\" WHERE \"U_IGCode\" = '" + Code + "') ";
            dtCodes = Program.objHrmsUI.getDataTable(strAttrAll, "gettingCodes");
            i = 0;
            dtS.Rows.Clear();
            foreach (System.Data.DataRow dr in dtCodes.Rows)
            {
                i++;
                dtS.Rows.Add(1);
                dtS.SetValue("Id", i - 1, i.ToString());
                dtS.SetValue("Code", i - 1, dr["Code"].ToString());
                dtS.SetValue("Name", i - 1, dr["U_Descr"].ToString());

            }
            mtS.LoadFromDataSource();


        }

        private void moveRow(int tbl,string AttrCode,int currentPosition , int newPosition)
        {
            if (tbl == 0)
            {

                string tableName = "\"@B1_QA_ATTR\"";
                string swappingAttribute = Convert.ToString(Program.objHrmsUI.getScallerValue("SELECT \"Code\" FROM \"@B1_QA_ATTR\" WHERE \"U_VisOrder\" = '" + newPosition + "'"));
                string strCurPosSql = "UPDATE " + tableName + " SET \"U_VisOrder\" =  " + newPosition + " WHERE \"Code\" = '" + AttrCode + "'";
                string strNewPosSql = "UPDATE " + tableName + " SET \"U_VisOrder\" =  " + currentPosition + " WHERE \"Code\" = '" + swappingAttribute + "'";

                Program.objHrmsUI.ExecQuery(strCurPosSql, "Swapping");
                Program.objHrmsUI.ExecQuery(strNewPosSql, "Swapping");



            }

            if (tbl ==1)
            {

                string tableName = "\"@B1_QA_ATTR_LOV\"";
                string swappingAttribute = Convert.ToString(Program.objHrmsUI.getScallerValue("SELECT \"Code\" FROM \"@B1_QA_ATTR_LOV\" WHERE \"U_AttrCode\" = '" + AttrCode + "' AND  \"U_VisOrder\" = '" + newPosition + "'"));
                string strCurPosSql = "UPDATE " + tableName + " SET \"U_VisOrder\" =  " + newPosition + " WHERE \"U_AttrCode\" = '" + AttrCode + "' AND \"U_VisOrder\" = '" + currentPosition + "'";
                string strNewPosSql = "UPDATE " + tableName + " SET \"U_VisOrder\" =  " + currentPosition + " WHERE \"Code\" = '" + swappingAttribute + "'";

                Program.objHrmsUI.ExecQuery(strCurPosSql, "Swapping");
                Program.objHrmsUI.ExecQuery(strNewPosSql, "Swapping");



            }

        }

    }
}
