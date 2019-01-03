using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;

namespace ACHR.Screen
{
    class frm_ADM : HRMSBaseForm
    {

        public bool isForLoading = false;
        SAPbouiCOM.Matrix mtList, mtA,mtS,mtAW,mtSW;

        SAPbouiCOM.ComboBox cbPWHS;
        SAPbouiCOM.EditText txCode, txName;
        SAPbouiCOM.Button btAdd, btAddA, btDelA, btAddW, btDelW;
        SAPbouiCOM.ChooseFromList cflCode;
        SAPbouiCOM.DataTable dtList, dtHead, dtA, dtS,dtAW,dtSW;
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

            if (pVal.ItemUID == cbPWHS.Item.UniqueID)
            {
                int selListRow = mtSelRow(mtList);
                if (selListRow > 0)
                {
                    string IGcode = Convert.ToString(dtList.GetValue("Code", selListRow - 1));
                    string strUpdate = "UPDATE   \"@B1_QA_OUSR\" SET  \"U_PWHS\" = '" + cbPWHS.Selected.Value.ToString().Trim() + "' WHERE \"Code\" = '" + IGcode + "'";
                    Program.objHrmsUI.ExecQuery(strUpdate, "Update Production WHS");
                }
                else
                {
                    oApplication.MessageBox("Please select User");
                }

            }

        }
       
        public override void etAfterClick(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);
          

            if(pVal.ItemUID == btAdd.Item.UniqueID)
            {
                string code = Convert.ToString(dtHead.GetValue("Code", 0));
                string name =  Convert.ToString(dtHead.GetValue("Name", 0));
                if (code != "")
                {
                    string strInsert = "INSERT INTO \"@B1_QA_OUSR\" (\"Code\" , \"Name\") VALUES ('" + code + "','" + name + "')";


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
                int selRow = mtSelRow(mtA);
                if (selRow > 0)
                {
                    string strCode = Convert.ToString(dtA.GetValue("Code", selRow - 1));

                    string strNCode = Program.objHrmsUI.getMaxId("@B1_QA_OUSR_OT", "Code").ToString();


                    string strInsert = "INSERT INTO  \"@B1_QA_OUSR_OT\" (\"Code\",\"Name\",\"U_ObjType\",\"U_UsrCode\") VALUES ('" + strNCode + "','" + strCode + "','" + strCode + "','" + IGcode + "')";


                    Program.objHrmsUI.ExecQuery(strInsert, "Insert New Code");
                    getCodesAttr(IGcode);
                }
                else
                {
                    oApplication.MessageBox("Please select Code");
                }
            }
            if (pVal.ItemUID == btAddW.Item.UniqueID)
            {
                int selListRow = mtSelRow(mtList);

                string IGcode = Convert.ToString(dtList.GetValue("Code", selListRow - 1));
                int selRow = mtSelRow(mtAW);
                if (selRow > 0)
                {
                    string strCode = Convert.ToString(dtAW.GetValue("Code", selRow - 1));
                    string strNCode = Program.objHrmsUI.getMaxId("@B1_QA_OUSR_WHS", "Code").ToString();

                    string strInsert = "INSERT INTO  \"@B1_QA_OUSR_WHS\" (\"Code\",\"Name\",\"U_WHS\",\"U_UsrCode\") VALUES ('" + strNCode + "','" + strCode + "','" + strCode + "','" + IGcode + "')";


                    Program.objHrmsUI.ExecQuery(strInsert, "Insert New WHS");
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
                int selRow = mtSelRow(mtS);
                if (selRow > 0)
                {
                    string strCode = Convert.ToString(dtS.GetValue("Code", selRow - 1));

                    string strInsert = "DELETE FROM  \"@B1_QA_OUSR_OT\" WHERE \"Name\" = '" + strCode + "' AND \"U_UsrCode\" = '" + IGcode + "'";


                    Program.objHrmsUI.ExecQuery(strInsert, "REMOVE  Code");
                    getCodesAttr(IGcode);
                }
                else
                {
                    oApplication.MessageBox("Please select Code");
                }
            }
            if (pVal.ItemUID == btDelW.Item.UniqueID)
            {
                int selListRow = mtSelRow(mtList);

                string IGcode = Convert.ToString(dtList.GetValue("Code", selListRow - 1));
                int selRow = mtSelRow(mtSW);
                if (selRow > 0)
                {
                    string strCode = Convert.ToString(dtSW.GetValue("Code", selRow - 1));

                    string strInsert = "DELETE FROM  \"@B1_QA_OUSR_WHS\" WHERE \"Name\" = '" + strCode + "' AND \"U_UsrCode\" = '" + IGcode + "' ";


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
                mtList.SelectRow(pVal.Row,true,false);
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
            if (pVal.ItemUID == mtS.Item.UniqueID && pVal.Row > 0 && pVal.Row <= mtS.RowCount)
            {
                mtS.SelectRow(pVal.Row, true, false);
            }
            if (pVal.ItemUID == mtA.Item.UniqueID && pVal.Row > 0 && pVal.Row <= mtA.RowCount)
            {
                mtA.SelectRow(pVal.Row, true, false);
            }
            if (pVal.ItemUID == mtSW.Item.UniqueID && pVal.Row > 0 && pVal.Row <= mtSW.RowCount)
            {
                mtSW.SelectRow(pVal.Row, true, false);
            }
            if (pVal.ItemUID == mtAW.Item.UniqueID && pVal.Row > 0 && pVal.Row <= mtAW.RowCount)
            {
                mtAW.SelectRow(pVal.Row, true, false);
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

                    strCode = dtSel.GetValue("USER_CODE", 0).ToString();
                    strName = dtSel.GetValue("U_NAME", 0).ToString();

                    dtHead.SetValue("Code", 0, strCode);
                    dtHead.SetValue("Name", 0, strName);


                }
            }
        }


        private void InitiallizeForm()
        {


            oForm.Freeze(true);

            initiallizing = true;

            dtHead = oForm.DataSources.DataTables.Item("dtHead");
            dtList = oForm.DataSources.DataTables.Item("dtList");

            dtA = oForm.DataSources.DataTables.Item("dtA");
            dtS = oForm.DataSources.DataTables.Item("dtS");
            dtAW = oForm.DataSources.DataTables.Item("dtAW");
            dtSW = oForm.DataSources.DataTables.Item("dtSW");



            mtA = (SAPbouiCOM.Matrix)oForm.Items.Item("mtA").Specific;
            mtS = (SAPbouiCOM.Matrix)oForm.Items.Item("mtS").Specific;
            mtAW = (SAPbouiCOM.Matrix)oForm.Items.Item("mtAW").Specific;
            mtSW = (SAPbouiCOM.Matrix)oForm.Items.Item("mtSW").Specific;


            mtList = (SAPbouiCOM.Matrix)oForm.Items.Item("mtList").Specific;

            dtHead.Rows.Add(1);
           

            txCode = (SAPbouiCOM.EditText)oForm.Items.Item("txCode").Specific;
            txName = (SAPbouiCOM.EditText)oForm.Items.Item("txName").Specific;
           
            btAdd = (SAPbouiCOM.Button)oForm.Items.Item("btAdd").Specific;
            btAddA = (SAPbouiCOM.Button)oForm.Items.Item("btAddA").Specific;
            btDelA = (SAPbouiCOM.Button)oForm.Items.Item("btDelA").Specific;
            btAddW = (SAPbouiCOM.Button)oForm.Items.Item("btAddW").Specific;
            btDelW = (SAPbouiCOM.Button)oForm.Items.Item("btDelW").Specific;

            cbPWHS = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbPWHS").Specific;


            cflCode = (SAPbouiCOM.ChooseFromList)oForm.ChooseFromLists.Item("cflCode");
         



            // oForm.Items.Item("btUpd").Enabled = false;
            oForm.Freeze(false);

            initiallizing = false;
            getCodes();


        }

        private void getCodes()
        {
            dtList.Rows.Clear();
            string codeType = "Group";
        
            string strSql = "SELECT \"Code\" , \"Name\" FROM \"@B1_QA_OUSR\"";

            System.Data.DataTable dtCodes = Program.objHrmsUI.getDataTable(strSql, "gettingCodes");
            int i = 0;

            foreach (System.Data.DataRow dr in dtCodes.Rows)
            {
                i++;
                dtList.Rows.Add(1);
                dtList.SetValue("Id", i - 1, i.ToString());
                dtList.SetValue("Code", i - 1, dr["Code"].ToString());
                dtList.SetValue("Name", i - 1, dr["Name"].ToString());

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

            string strUsrInfo = "SELECT * FROM \"@B1_QA_OUSR\" WHERE  \"Code\" = '" + Code + "' ";
            System.Data.DataTable dtUsrInfo = Program.objHrmsUI.getDataTable(strUsrInfo, "User Info");
            string prdWhs = "";
            if(dtUsrInfo!=null && dtUsrInfo.Rows.Count>0)
            {
                prdWhs = dtUsrInfo.Rows[0]["U_PWHS"].ToString().Trim();
            }

            string strAttrAll = "SELECT \"Code\" , \"Name\" FROM \"@B1_QA_OBJS\" WHERE  \"Code\" NOT IN (SELECT \"U_ObjType\" FROM \"@B1_QA_OUSR_OT\" WHERE \"U_UsrCode\" = '" + Code + "') ";
            System.Data.DataTable dtCodes = Program.objHrmsUI.getDataTable(strAttrAll, "gettingCodes");
            int i = 0;
            dtA.Rows.Clear();
            foreach (System.Data.DataRow dr in dtCodes.Rows)
            {
                i++;
                dtA.Rows.Add(1);
                dtA.SetValue("Id", i - 1, i.ToString());
                dtA.SetValue("Code", i - 1, dr["Code"].ToString());
                dtA.SetValue("Name", i - 1, dr["Name"].ToString());

            }
            mtA.LoadFromDataSource();



            strAttrAll = "SELECT \"Code\" , \"Name\" FROM \"@B1_QA_OBJS\" WHERE  \"Code\"  IN (SELECT \"U_ObjType\" FROM \"@B1_QA_OUSR_OT\" WHERE \"U_UsrCode\" = '" + Code + "') ";
            dtCodes = Program.objHrmsUI.getDataTable(strAttrAll, "gettingCodes");
            i = 0;
            dtS.Rows.Clear();
            foreach (System.Data.DataRow dr in dtCodes.Rows)
            {
                i++;
                dtS.Rows.Add(1);
                dtS.SetValue("Id", i - 1, i.ToString());
                dtS.SetValue("Code", i - 1, dr["Code"].ToString());
                dtS.SetValue("Name", i - 1, dr["Name"].ToString());

            }
            mtS.LoadFromDataSource();




            strAttrAll = "SELECT t0.\"WhsCode\" , t0.\"WhsName\" FROM \"OWHS\" t0 INNER JOIN  \"@B1_QA_ATTR_INSWHS\" t1 on t0.\"WhsCode\" = t1.\"U_OWHS\"  WHERE  t0.\"WhsCode\" NOT IN (SELECT \"U_WHS\" FROM \"@B1_QA_OUSR_WHS\" WHERE \"U_UsrCode\" = '" + Code + "') ";
           dtCodes = Program.objHrmsUI.getDataTable(strAttrAll, "gettingCodes");
             i = 0;
            dtAW.Rows.Clear();
            foreach (System.Data.DataRow dr in dtCodes.Rows)
            {
                i++;
                dtAW.Rows.Add(1);
                dtAW.SetValue("Id", i - 1, i.ToString());
                dtAW.SetValue("Code", i - 1, dr["WhsCode"].ToString());
                dtAW.SetValue("Name", i - 1, dr["WhsName"].ToString());

            }
            mtAW.LoadFromDataSource();
            int cnt = cbPWHS.ValidValues.Count;
            for (int k = 0; k < cnt; k++)
            {
                cbPWHS.ValidValues.Remove(0, BoSearchKey.psk_Index);
            }
            strAttrAll = "SELECT t0.\"WhsCode\" , t0.\"WhsName\" FROM \"OWHS\" t0 INNER JOIN  \"@B1_QA_ATTR_INSWHS\" t1 on t0.\"WhsCode\" = t1.\"U_OWHS\"  WHERE  t0.\"WhsCode\"  IN (SELECT \"U_WHS\" FROM \"@B1_QA_OUSR_WHS\" WHERE \"U_UsrCode\" = '" + Code + "') ";
            dtCodes = Program.objHrmsUI.getDataTable(strAttrAll, "gettingCodes");
            i = 0;
            dtSW.Rows.Clear();
            foreach (System.Data.DataRow dr in dtCodes.Rows)
            {
                i++;
                dtSW.Rows.Add(1);
                dtSW.SetValue("Id", i - 1, i.ToString());
                dtSW.SetValue("Code", i - 1, dr["WhsCode"].ToString());
                dtSW.SetValue("Name", i - 1, dr["WhsName"].ToString());
                cbPWHS.ValidValues.Add(dr["WhsCode"].ToString(), dr["WhsName"].ToString());
            }
            mtSW.LoadFromDataSource();

            if (prdWhs != "" && cbPWHS.ValidValues.Count > 0)
            {
                try
                {
                    cbPWHS.Select(prdWhs , BoSearchKey.psk_ByValue);
                }
                catch { }
            }

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
