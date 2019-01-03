using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACHR.Screen
{
    class frm_Attr : HRMSBaseForm
    {

        public bool isForLoading = false;
        SAPbouiCOM.Matrix mtAttr, mtLOV;
        SAPbouiCOM.ComboBox cbType, cbLOV;
        SAPbouiCOM.EditText txSAtt, txNCode, txNName, txCode, txName, txDef, txNV, txNDescr, txSQL, txVD;
        SAPbouiCOM.Button btAddAttr, btDelAttr, btUPAtt, btDownAtt, btAddLOV, btDelLOV, btUpLOV, btDownLOV, btUpdate;
        SAPbouiCOM.DataTable dtAtrr, dtHead, dtLOV;
        System.Data.DataTable AllRules = new System.Data.DataTable();
        SAPbouiCOM.CheckBox chActive;
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
            AllRules.Columns.Add("RULE");
            AllRules.Columns.Add("Descr");
            AllRules.Columns.Add("VisOrder");
            InitiallizeForm();

            // oForm.Items.Item("btUpd").Visible = false;



        }
        public override void etAfterCmbSelect(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCmbSelect(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == cbLOV.Item.UniqueID)
            {
                hideValues();
            }

        }
        public override void etAfterKeyDown(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterKeyDown(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == txSAtt.Item.UniqueID)
            {
                ApplySearch();
            }
        }
        public override void etAfterClick(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == btAddAttr.Item.UniqueID)
            {
                string strNewCode = Convert.ToString(dtHead.GetValue("nCode", 0));

                if (strNewCode.Trim() != "")
                {
                    string strNewName = Convert.ToString(dtHead.GetValue("nName", 0));
                    string strCnt = "SELECT COUNT(*) FROM \"@B1_QA_ATTR\" WHERE \"Code\"='" + strNewCode + "'";
                   
                    int oldCnt = Convert.ToInt32(Program.objHrmsUI.getScallerValue(strCnt));
                    long maxVis = Convert.ToInt32( Program.objHrmsUI.getMaxId("@B1_QA_ATTR", "U_VisOrder"));

                    if (oldCnt == 0)
                    {
                        string strInsert = "INSERT INTO \"@B1_QA_ATTR\" (\"Code\",\"Name\",\"U_Descr\",\"U_VisOrder\",\"U_Active\",\"U_Type\",\"U_LOVT\") VALUES ('" + strNewCode + "','" + strNewCode + "','" + strNewName + "','" + maxVis.ToString() + "','Y','01','01')";


                        Program.objHrmsUI.ExecQuery(strInsert, "Insert New Code");
                        getCodes();
                    }
                    else
                    {
                        oApplication.MessageBox("Please enter Code");
                    }
                }
                else
                {
                    oApplication.MessageBox("Please enter Code");
                }
            }

            if (pVal.ItemUID == btDelAttr.Item.UniqueID )
            {
                int selRow = mtSelRow(mtAttr);
                if (selRow > 0)
                {
                    string strCode = Convert.ToString(dtAtrr.GetValue("Code", selRow - 1));

                    if (strCode.Trim() != "")
                    {
                        int confirm = oApplication.MessageBox("Are you sure you want to delete this attribute", 2, "Yes", "No");
                        if (confirm == 1)
                        {


                            string strInsert = "DELETE FROM \"@B1_QA_ATTR\" WHERE \"Code\" = '" + strCode + "'";
                            Program.objHrmsUI.ExecQuery(strInsert, "Delete Rule Code");
                            getCodes();

                        }

                    }
                }
                else
                {
                    oApplication.MessageBox("Please select rule to delete");
                }
            }


            if (pVal.ItemUID == btAddLOV.Item.UniqueID)
            {
                string strAttrCode = Convert.ToString(dtHead.GetValue("Code", 0));
                string strNewCode = Convert.ToString(dtHead.GetValue("LOVV", 0));


                if (strNewCode.Trim() != "")
                {
                    string strLovCode = strAttrCode + "_" + strNewCode;
                    string strNewName = Convert.ToString(dtHead.GetValue("LOVD", 0));

                    string strCnt = "SELECT COUNT(*) FROM \"@B1_QA_ATTR_LOV\" WHERE \"Code\"='" + strLovCode + "'";
                    long maxV = 1;
                    int oldCnt = Convert.ToInt32(Program.objHrmsUI.getScallerValue(strCnt));
                    string maxVSel = " SELECT isnull(max(convert(int,U_VisOrder)),0)  AS \"nextId\" FROM \"@B1_QA_ATTR_LOV\" WHERE \"U_AttrCode\" = '" + strAttrCode + "' ";

                    if (oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                    {
                        maxVSel = " SELECT IFNULL(MAX(CAST(\"U_VisOrder\" AS integer)), 0) AS \"nextId\" FROM \"@B1_QA_ATTR_LOV\" WHERE \"U_AttrCode\" = '" + strAttrCode + "' ;";

                    }
                    try
                    {
                        maxV = Convert.ToInt32(Program.objHrmsUI.getScallerValue(maxVSel)) + 1;
                    }
                    catch { }


                    if (oldCnt == 0)
                    {
                        string strInsert = "INSERT INTO \"@B1_QA_ATTR_LOV\" (\"Code\",\"Name\",\"U_Val\",\"U_Descr\",\"U_VisOrder\",\"U_AttrCode\") VALUES ('" + strLovCode + "','" + strLovCode + "','" + strNewCode + "','" + strNewName + "','" + maxV.ToString() + "','" + strAttrCode + "')";


                        Program.objHrmsUI.ExecQuery(strInsert, "Insert New Code");
                        getLOVList(strAttrCode);
                    }
                    else
                    {
                        oApplication.MessageBox("Please select different Code");
                    }
                }
                else
                {
                    oApplication.MessageBox("Please enter Code");
                }



            }
            if (pVal.ItemUID == btDelLOV.Item.UniqueID)
            {
                int selRow = mtSelRow(mtLOV);
                if (selRow > 0)
                {
                    string AttrCode = Convert.ToString(dtHead.GetValue("Code", 0));
                    string ValCode = Convert.ToString(dtLOV.GetValue("ValueSTR", selRow - 1));
                    string lovCode = AttrCode + "_" + ValCode;
                    if (lovCode.Trim() != "")
                    {


                        string strDelete = "DELETE FROM \"@B1_QA_ATTR_LOV\" WHERE \"Code\" = '" + lovCode + "'";
                        Program.objHrmsUI.ExecQuery(strDelete, "Delete Rule Detail Code");

                        getLOVList(AttrCode);
                    }
                }
                else
                {
                    oApplication.MessageBox("Please select rule detail to delete");
                }
            }


            if (pVal.ItemUID == mtAttr.Item.UniqueID && pVal.Row > 0 && pVal.Row <= mtAttr.RowCount)
            {
                mtAttr.SelectRow(pVal.Row, true, false);
                filldetails(dtAtrr.GetValue("Code", pVal.Row - 1).ToString());

            }

            if (pVal.ItemUID == chActive.Item.UniqueID && txNV.Value != "")
            {
                string strCheck = "N";

                if (chActive.Checked)
                {
                    strCheck = "Y";

                }

                Hashtable hsp = new Hashtable();
                hsp.Add("~p1", txNV.Value.ToString());
                hsp.Add("~p2", strCheck);
                string strUpdate = Program.objHrmsUI.getQryString("AR_ACTIVE", hsp);


                Program.objHrmsUI.ExecQuery(strUpdate, "Updating Status");
            }

            if (pVal.ItemUID == btUpdate.Item.UniqueID)
            {
                string strAttrCode = Convert.ToString(dtHead.GetValue("Code", 0));
                string Descr = Convert.ToString(dtHead.GetValue("Name", 0));
                string Active = Convert.ToString(dtHead.GetValue("Active", 0));
                string Type = Convert.ToString(dtHead.GetValue("Type", 0));
                string LOVT = Convert.ToString(dtHead.GetValue("LOV", 0));
                string LOVSQL = Convert.ToString(dtHead.GetValue("SQL", 0));
                string U_DefVal = Convert.ToString(dtHead.GetValue("DefVal", 0));
                string U_DefValRem = Convert.ToString(dtHead.GetValue("defValid", 0));


                string strUpdate = "  UPDATE  \"@B1_QA_ATTR\" SET \"U_Descr\" = '" + Descr.Replace("'", "''") + "' ,\"U_Active\" = '" + Active.Replace("'", "''") + "',\"U_Type\" = '" + Type.Replace("'", "''") + "',\"U_LOVT\"='" + LOVT.Replace("'", "''") + "' ,\"U_LOVSQL\"='" + LOVSQL.Replace("'", "''") + "' , \"U_DefVal\"='" + U_DefVal.Replace("'", "''") + "',\"U_DefValRem\"='" + U_DefValRem.Replace("'", "''") + "' WHERE \"Code\"='" + strAttrCode + "' ";
                string result = Program.objHrmsUI.ExecQuery(strUpdate, "Updating Attribute", 0);
                if (result == "OK")
                {
                    oApplication.SetStatusBarMessage("Attributed Updated!", SAPbouiCOM.BoMessageTime.bmt_Short, false);
                }
            }
            if (pVal.ItemUID == btUPAtt.Item.UniqueID)
            {
                int selId = mtSelRow(mtAttr);
                if (selId ==1) return;
                string strAttrCode = Convert.ToString(dtHead.GetValue("Code", 0));
                moveRow(0, strAttrCode, selId , selId-1);
                getCodes();
                mtAttr.SelectRow(selId - 1, true, false);
            }
            if (pVal.ItemUID == btDownAtt.Item.UniqueID)
            {
                int selId = mtSelRow(mtAttr);
                if (selId == mtAttr.RowCount) return;
                string strAttrCode = Convert.ToString(dtHead.GetValue("Code", 0));
                moveRow(0, strAttrCode, selId, selId + 1);
                getCodes();
                mtAttr.SelectRow(selId + 1, true, false);

            }

            if (pVal.ItemUID == btUpLOV.Item.UniqueID)
            {
                int selId = mtSelRow(mtLOV);
                if (selId == 1) return;
                string strAttrCode = Convert.ToString(dtHead.GetValue("Code", 0));
                moveRow(1, strAttrCode, selId, selId - 1);
                getLOVList(strAttrCode);
                mtLOV.SelectRow(selId - 1, true, false);
            }
            if (pVal.ItemUID == btDownLOV.Item.UniqueID)
            {
                int selId = mtSelRow(mtLOV);
                if (selId == mtLOV.RowCount) return;
                string strAttrCode = Convert.ToString(dtHead.GetValue("Code", 0));
                moveRow(1, strAttrCode, selId, selId + 1);
                getLOVList(strAttrCode);
                mtLOV.SelectRow(selId + 1, true, false);

            }


        }

        private void MarkUpdateDate(string AR_Code)
        {
            string strSQL = "UPDATE \"@B1_AR\" SET \"U_LstUpdate\" = NOW() WHERE \"Code\" = '" + AR_Code + "'";
            Program.objHrmsUI.ExecQuery(strSQL, "Updating Update Date");

        }

        private void getCodes()
        {
            AllRules.Rows.Clear();
            // dtRM.Rows.Clear();
            string strSQL = "SELECT * FROM \"@B1_QA_ATTR\" ORDER BY \"U_VisOrder\" ";
            System.Data.DataTable dtCodes = Program.objHrmsUI.getDataTable(strSQL, "getting codes");
            int i = 0;
            foreach (System.Data.DataRow dr in dtCodes.Rows)
            {
                //i++;
                //dtRM.Rows.Add(1);
                //dtRM.SetValue("Id", i-1, i.ToString());
                //dtRM.SetValue("RN", i - 1, dr["Code"].ToString());
                AllRules.Rows.Add(dr["Code"].ToString(), dr["U_Descr"].ToString(), Convert.ToInt32(dr["U_VisOrder"]));

            }
            ApplySearch();
            // mtR.LoadFromDataSource();
            // ini_controls();

        }
        private void ApplySearch()
        {
            int i = 0;

            string searchString = txSAtt.Value.ToString(); // Convert.ToString(dtHeads.GetValue("Search", 0));
            System.Data.DataRow[] rows = AllRules.Select("RULE like '" + searchString + "*'", "VisOrder Asc");
            if (rows.Count() > 0)
            {
                dtAtrr.Rows.Clear();
                foreach (System.Data.DataRow dr in rows)
                {
                    i++;
                    dtAtrr.Rows.Add(1);
                    dtAtrr.SetValue("Id", i - 1, i.ToString());
                    dtAtrr.SetValue("Code", i - 1, dr["RULE"].ToString());
                    dtAtrr.SetValue("Name", i - 1, dr["Descr"].ToString());

                }
                mtAttr.LoadFromDataSource();
                //  ini_controls();
            }
        }
        private void getLOVList(string code)
        {
            dtLOV.Rows.Clear();


            string strSelect = "SELECT * FROM \"@B1_QA_ATTR_LOV\" WHERE \"U_AttrCode\" = '" + code + "' ORDER BY \"U_VisOrder\"";
            System.Data.DataTable dtCodes = Program.objHrmsUI.getDataTable(strSelect, "getting codes LOV");
            int i = 0;
            foreach (System.Data.DataRow dr in dtCodes.Rows)
            {
                i++;
                dtLOV.Rows.Add(1);
                dtLOV.SetValue("Id", i - 1, i.ToString());
                dtLOV.SetValue("ValueSTR", i - 1, dr["U_Val"].ToString());
                dtLOV.SetValue("Descr", i - 1, dr["U_Descr"].ToString());

            }

            mtLOV.LoadFromDataSource();


        }



        private void hideValues()
        {
            txSQL.Item.Visible = false;
            mtLOV.Item.Visible = false;
            btAddLOV.Item.Visible = false;
            btDelLOV.Item.Visible = false;
            btUpLOV.Item.Visible = false;
            btDownLOV.Item.Visible = false;
            txNV.Item.Visible = false;
            txNDescr.Item.Visible = false;
            if (cbLOV.Value == "03") txSQL.Item.Visible = true;
            if (cbLOV.Value == "02")
            {
                mtLOV.Item.Visible = true;
                btAddLOV.Item.Visible = true;
                btDelLOV.Item.Visible = true;
                btUpLOV.Item.Visible = true;
                btDownLOV.Item.Visible = true;
                txNV.Item.Visible = true;
                txNDescr.Item.Visible = true;

            }

        }

        private void filldetails(string Code)
        {

            string strSelect = "SELECT * FROM \"@B1_QA_ATTR\" WHERE \"Code\" ='" + Code + "'";


            System.Data.DataTable dtRule = Program.objHrmsUI.getDataTable(strSelect, "getting rule detail");
            if (dtRule.Rows.Count > 0)
            {
                string Active = dtRule.Rows[0]["U_Active"].ToString();
                // string baseUnit = dtRule.Rows[0]["BU"].ToString();
                //if (baseUnit == "") baseUnit = "01";

                dtHead.SetValue("Active", 0, Active);
                dtHead.SetValue("Code", 0, Code);
                dtHead.SetValue("Name", 0, dtRule.Rows[0]["U_Descr"].ToString());
                dtHead.SetValue("DefVal", 0, dtRule.Rows[0]["U_DefVal"].ToString());
                dtHead.SetValue("SQL", 0, dtRule.Rows[0]["U_LOVSQL"].ToString());
                dtHead.SetValue("defValid", 0, dtRule.Rows[0]["U_DefValRem"].ToString());

                cbType.Select(dtRule.Rows[0]["U_Type"].ToString());
                cbLOV.Select(dtRule.Rows[0]["U_LOVT"].ToString());
                getLOVList(Code);




            }


        }
        private void InitiallizeForm()
        {


            oForm.Freeze(true);

            initiallizing = true;

            dtAtrr = oForm.DataSources.DataTables.Item("dtAtrr");
            dtHead = oForm.DataSources.DataTables.Item("dtHead");
            dtLOV = oForm.DataSources.DataTables.Item("dtLOV");

            dtHead.Rows.Add(1);
            mtAttr = (SAPbouiCOM.Matrix)oForm.Items.Item("mtAttr").Specific;
            mtLOV = (SAPbouiCOM.Matrix)oForm.Items.Item("mtLOV").Specific;

            cbType = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbType").Specific;
            cbLOV = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbLOV").Specific;
            cbType.Item.DisplayDesc = true;
            cbLOV.Item.DisplayDesc = true;

            txSAtt = (SAPbouiCOM.EditText)oForm.Items.Item("txSAtt").Specific;
            txNCode = (SAPbouiCOM.EditText)oForm.Items.Item("txNCode").Specific;
            txNName = (SAPbouiCOM.EditText)oForm.Items.Item("txNName").Specific;
            txCode = (SAPbouiCOM.EditText)oForm.Items.Item("txCode").Specific;
            txName = (SAPbouiCOM.EditText)oForm.Items.Item("txName").Specific;
            txDef = (SAPbouiCOM.EditText)oForm.Items.Item("txDef").Specific;
            txNV = (SAPbouiCOM.EditText)oForm.Items.Item("txNV").Specific;
            txNDescr = (SAPbouiCOM.EditText)oForm.Items.Item("txNDescr").Specific;
            txSQL = (SAPbouiCOM.EditText)oForm.Items.Item("txSQL").Specific;
            txVD = (SAPbouiCOM.EditText)oForm.Items.Item("txVD").Specific;

            btAddAttr = (SAPbouiCOM.Button)oForm.Items.Item("btAddAttr").Specific;
            btDelAttr = (SAPbouiCOM.Button)oForm.Items.Item("btDelAttr").Specific;
            btUPAtt = (SAPbouiCOM.Button)oForm.Items.Item("btUPAtt").Specific;
            btDownAtt = (SAPbouiCOM.Button)oForm.Items.Item("btDownAtt").Specific;
            btAddLOV = (SAPbouiCOM.Button)oForm.Items.Item("btAddLOV").Specific;
            btDelLOV = (SAPbouiCOM.Button)oForm.Items.Item("btDelLOV").Specific;
            btUpLOV = (SAPbouiCOM.Button)oForm.Items.Item("btUpLOV").Specific;
            btDownLOV = (SAPbouiCOM.Button)oForm.Items.Item("btDownLOV").Specific;
            btUpdate = (SAPbouiCOM.Button)oForm.Items.Item("btUpdate").Specific;


            chActive = (SAPbouiCOM.CheckBox)oForm.Items.Item("chActive").Specific;

            // oForm.Items.Item("btUpd").Enabled = false;
            oForm.Freeze(false);

            initiallizing = false;
            txSAtt.Active = true;
            fillCb();
            getCodes();


        }

        private void ini_controls()
        {


        }




        private void fillCb()
        {
            cbType.ValidValues.Add("01", "Alphanumeric");
            cbType.ValidValues.Add("02", "Numeric");
            cbType.ValidValues.Add("03", "Date");

            cbType.Select("01");
            cbLOV.ValidValues.Add("01", "None");
            cbLOV.ValidValues.Add("02", "Fixed List");
            cbLOV.ValidValues.Add("03", "SQL");
            cbLOV.Select("01");

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
