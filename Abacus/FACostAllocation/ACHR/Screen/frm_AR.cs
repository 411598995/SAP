using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACHR.Screen
{
    class frm_AR : HRMSBaseForm
    {

        public bool isForLoading = false;
        SAPbouiCOM.Matrix mtR, mtRD;
        SAPbouiCOM.ComboBox cbCM,cbBU;
        SAPbouiCOM.EditText txnRN, txRN, txAP;
        SAPbouiCOM.Button btAddR, btAddD, btDelD, btDelR;
        System.Data.DataTable AllRules = new System.Data.DataTable();
        SAPbouiCOM.CheckBox chActive;
        int rowNum = 0;



        SAPbouiCOM.DataTable dtRM,dtRD,dtHeads;
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
            InitiallizeForm();

            oForm.Items.Item("btUpd").Visible = false;
              


        }
        public override void etAfterCmbSelect(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCmbSelect(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == cbBU.Item.UniqueID)
            {
                updateRDBaseUnitVal();
                oForm.Items.Item("btUpd").Visible = false ;
               // oForm.Items.Item("btUpd").Enabled =true;
            }
        }
        public override void etAfterKeyDown(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterKeyDown(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == "19")
            {
                ApplySearch();
            }
        }
        public override void etAfterClick(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == btAddR.Item.UniqueID)
            {
                string strRuleCode = Convert.ToString(dtHeads.GetValue("RNN", 0));

                if (strRuleCode.Trim() != "")
                {

                    Hashtable hsp = new Hashtable();
                    hsp.Add("~p1", strRuleCode);
                    string strCnt = Program.objHrmsUI.getQryString("AR_Click_001", hsp);
                    int oldCnt = Convert.ToInt32( Program.objHrmsUI.getScallerValue(strCnt));
                    if (oldCnt == 0)
                    {
                        string strInsert = Program.objHrmsUI.getQryString("AR_AddRule", hsp);
                   
                        Program.objHrmsUI.ExecQuery(strInsert, "Insert New Code");
                        getCodes();
                    }
                    else
                    {
                        oApplication.MessageBox("Please enter rule");
                    }
                }
                else
                {
                    oApplication.MessageBox("Please enter rule");
                }
            }

            if (pVal.ItemUID == btDelR.Item.UniqueID)
            {
                int selRow = mtSelRow(mtR);
                if (selRow > 0)
                {
                    string strRuleCode = Convert.ToString(dtRM.GetValue("RN", selRow -1));

                    if (strRuleCode.Trim() != "")
                    {
                        int confirm = oApplication.MessageBox("Are you sure you want to delete this rule", 2, "Yes", "No");
                        if (confirm == 1)
                        {
                            if (ARInIDRCnt(strRuleCode) == 0)
                            {
                                Hashtable hsp = new Hashtable();
                                hsp.Add("~p1", strRuleCode);
                                string strInsert = Program.objHrmsUI.getQryString("AR_DELCode", hsp);

                                Program.objHrmsUI.ExecQuery(strInsert, "Delete Rule Code");
                                getCodes();
                            }
                            else
                            {
                                oApplication.MessageBox("You can not delete this AR. It is already used in Indirect BOQ");
                            }
                        }

                    }
                }
                else
                {
                    oApplication.MessageBox("Please select rule to delete");
                }
            }
            

            if (pVal.ItemUID == btAddD.Item.UniqueID)
            {
                string strRuleCode = Convert.ToString(dtHeads.GetValue("RN", 0));
               
                double RP = Convert.ToDouble(dtHeads.GetValue("AP", 0));
                string CM = Convert.ToString(dtHeads.GetValue("CM", 0));

                if (CM == "")
                {
                    oApplication.MessageBox("Please select a Cost Master");
                }
                else
                {

                    Hashtable hsp = new Hashtable();
                    hsp.Add("~p2", strRuleCode);
                    hsp.Add("~p3", CM);
                    hsp.Add("~p4", RP);
                 
                    string strCnt = Program.objHrmsUI.getQryString("AR_D_CNT", hsp);
                    int oldCnt = Convert.ToInt32(Program.objHrmsUI.getScallerValue(strCnt));
                    if (oldCnt == 0)
                    {

                        long nextCode = Program.objHrmsUI.getMaxId("\"@B1_ARD\"", "\"Code\"");
                        string AddBy = nextCode.ToString();
                        hsp.Add("~p1", nextCode);
                        hsp.Add("~p5", AddBy);

                        string strInsert = Program.objHrmsUI.getQryString("AR_ADD_D", hsp);
                        strInsert = "INSERT INTO \"@B1_ARD\" (\"Code\", \"Name\", \"U_RuleCode\", U_CMC, U_AP,\"U_AddBy\") VALUES ('~p1', '~p1', '~p2', '~p3', '~p4','~p5');";
                        foreach (string pm in hsp.Keys)
                        {
                            strInsert = strInsert.Replace(pm, hsp[pm].ToString());
                        }
                        Program.objHrmsUI.ExecQuery(strInsert, "Adding Detail");
                        int addChilCosts = oApplication.MessageBox("Do you want to add all child items of that level ? ", 2, "Yes", "No");
                        if (addChilCosts == 1)
                        {
                            addChildern(nextCode.ToString(),CM,strRuleCode);
                        }
                        MarkUpdateDate(strRuleCode);

                        dtHeads.SetValue("AP", 0, "0");
                        getRuleDetail(strRuleCode);
                    }
                    else
                    {
                        int addChilCosts = oApplication.MessageBox("Already exists. Do you want to refresh child items of that level ? ", 2, "Yes", "No");
                        if (addChilCosts == 1)
                        {
                            string addBy = "_";
                            System.Data.DataTable dtAddBy = Program.objHrmsUI.getDataTable("SELECT * FROM \"@B1_ARD\" WHERE U_CMC = '" + CM + "'", "Add Child only of CM");
                            if (dtAddBy != null && dtAddBy.Rows.Count > 0)
                            {
                                addBy = dtAddBy.Rows[0]["Code"].ToString();
                            }
                            if (addBy != "_")
                            {
                                addChildern(addBy.ToString(), CM, strRuleCode);
                                MarkUpdateDate(strRuleCode);
                                dtHeads.SetValue("AP", 0, "0");
                                getRuleDetail(strRuleCode);
                            }
                          
                        }
                      
                    }

                }

            }
            if (pVal.ItemUID == btDelD.Item.UniqueID)
            {
                int selRow = mtSelRow(mtRD);
                if (selRow > 0)
                {
                    string strRuleCode = Convert.ToString(dtHeads.GetValue("RN", 0));
                    string CMCode = Convert.ToString(dtRD.GetValue("CM", selRow-1));
                    if (strRuleCode.Trim() != "")
                    {
                        Hashtable hsp = new Hashtable();
                        hsp.Add("~p2", strRuleCode);
                        hsp.Add("~p1", CMCode);
                        string strInsert = Program.objHrmsUI.getQryString("AR_D_DEL", hsp);

                        string addBy = "_";
                        System.Data.DataTable dtAddBy = Program.objHrmsUI.getDataTable( "SELECT * FROM \"@B1_ARD\" WHERE U_CMC = '" + CMCode + "'","Add Code of CMC");
                        if (dtAddBy != null && dtAddBy.Rows.Count > 0)
                        {
                            addBy = dtAddBy.Rows[0]["Code"].ToString();
                        }


                        strInsert = "DELETE FROM \"@B1_ARD\" WHERE \"U_RuleCode\" = '" + strRuleCode + "' AND (U_CMC = '" + CMCode + "' OR IFNULL(\"U_AddBy\" ,'') = '" + addBy + "')";
                        Program.objHrmsUI.ExecQuery(strInsert, "Delete Rule Detail Code");
                        MarkUpdateDate(strRuleCode);
             
                        getRuleDetail(strRuleCode);
                    }
                }
                else
                {
                    oApplication.MessageBox("Please select rule detail to delete");
                }
            }


            if (pVal.ItemUID == mtR.Item.UniqueID && pVal.Row>0 && pVal.Row<=mtR.RowCount)
            {
                filldetails(dtRM.GetValue("RN", pVal.Row - 1).ToString());

            }

            if (pVal.ItemUID == chActive.Item.UniqueID && txRN.Value != "")
            {
                string strCheck = "N";
            
                if (chActive.Checked)
                {
                   strCheck="Y";
              
                }
               
                Hashtable hsp = new Hashtable();
                hsp.Add("~p1", txRN.Value.ToString());
                hsp.Add("~p2", strCheck);
                string strUpdate = Program.objHrmsUI.getQryString("AR_ACTIVE", hsp);


                Program.objHrmsUI.ExecQuery(strUpdate, "Updating Status");
            }
            if (pVal.ItemUID == "btUpd")
            {

                updateAP();
               
            }
        }

        private void MarkUpdateDate(string AR_Code)
        {
            string strSQL = "UPDATE \"@B1_AR\" SET \"U_LstUpdate\" = NOW() WHERE \"Code\" = '" + AR_Code + "'";
            Program.objHrmsUI.ExecQuery(strSQL, "Updating Update Date");

        }
        private void updateAP()
        {
            string strUpdate = "";
            for (int i = 0; i < dtRD.Rows.Count; i++)
            {
                string strCode = Convert.ToString(dtRD.GetValue("CODE", i));
                string strAP = Convert.ToString(dtRD.GetValue("AP", i));

                Hashtable hsp = new Hashtable();
                hsp.Add("~p1", strCode);
                hsp.Add("~p2", strAP);
                strUpdate = Program.objHrmsUI.getQryString("AR_D_UP", hsp) + " ;";
                int result = Program.objHrmsUI.ExecQuery(strUpdate, "Update AP");
                if (result == 0)
                {
                    oForm.Items.Item("btUpd").Enabled = false;

                }
                else
                {
                    oApplication.MessageBox("Error in updating Percentages");
                }

            }
        }
        private void getCodes()
        {
            AllRules.Rows.Clear();
           // dtRM.Rows.Clear();
            Hashtable hsp = new Hashtable();
            string strSelect = Program.objHrmsUI.getQryString("AR_ALL", hsp);
            System.Data.DataTable dtCodes = Program.objHrmsUI.getDataTable(strSelect, "getting codes");
            int i=0;
            foreach (System.Data.DataRow dr in dtCodes.Rows)
            {
                //i++;
                //dtRM.Rows.Add(1);
                //dtRM.SetValue("Id", i-1, i.ToString());
                //dtRM.SetValue("RN", i - 1, dr["Code"].ToString());
                AllRules.Rows.Add(dr["Code"].ToString());

            }
            ApplySearch();
           // mtR.LoadFromDataSource();
           // ini_controls();

        }
        private void ApplySearch()
        {
            int i = 0;
            SAPbouiCOM.EditText txSearch = (SAPbouiCOM.EditText)oForm.Items.Item("19").Specific;

            string searchString = txSearch.Value.ToString(); // Convert.ToString(dtHeads.GetValue("Search", 0));
            System.Data.DataRow[] rows = AllRules.Select("RULE like '" + searchString + "*'");
            if (rows.Count() > 0)
            {
                dtRM.Rows.Clear();
                foreach (System.Data.DataRow dr in rows)
                {
                    i++;
                    dtRM.Rows.Add(1);
                    dtRM.SetValue("Id", i - 1, i.ToString());
                    dtRM.SetValue("RN", i - 1, dr["RULE"].ToString());
                
                }
                mtR.LoadFromDataSource();
              //  ini_controls();
            }
        }
        private void getRuleDetail(string RuleCode)
        {
            dtRD.Rows.Clear();

            Hashtable hsp = new Hashtable();
            hsp.Add("~p1", RuleCode);



            string strSelect = Program.objHrmsUI.getQryString("AR_D_ALL", hsp);
            System.Data.DataTable dtCodes = Program.objHrmsUI.getDataTable(strSelect, "getting codes details");
            double TotalBUV = 0.00;
            int i = 0;
            foreach (System.Data.DataRow dr in dtCodes.Rows)
            {
                i++;
                dtRD.Rows.Add(1);
                dtRD.SetValue("Id", i - 1, i.ToString());
                dtRD.SetValue("CM", i - 1, dr["U_CMC"].ToString());
                dtRD.SetValue("AP", i - 1, dr["U_AP"].ToString());
                dtRD.SetValue("BUV", i - 1, dr["BUV"].ToString());
                dtRD.SetValue("CODE", i - 1, dr["CODE"].ToString());
                string assetCode=""; 
                string GL = "";
                AMGL(dr["U_CMC"].ToString(), out GL, out assetCode);
                dtRD.SetValue("GL", i - 1, GL);
               

                TotalBUV += Convert.ToDouble(dr["BUV"]); ;
            }
            if (TotalBUV > 0)
            {
                i = 0;
                foreach (System.Data.DataRow dr in dtCodes.Rows)
                {
                    i++;
                    dtRD.SetValue("AP", i - 1, Convert.ToDouble(dr["BUV"]) / TotalBUV * 100.00);
                 
                }

            }
            mtRD.LoadFromDataSource();
            updateAP();
             

        }

        private void AMGL(string CM, out string acctCode, out string AssetCode)
        {
            acctCode = "";
            AssetCode = "";

            string strSelect = " SELECT T0.\"U_AlcBsUnt\", T0.\"U_AcctCode\" , T0.\"U_FaCode\" , T1.\"FormatCode\" , T1.\"AcctName\"  FROM \"@B1_AM\"  T0 Inner Join OACT T1 on T0.\"U_AcctCode\" = T1.\"AcctCode\" WHERE T0.\"Code\" ='" + CM + "'";
            System.Data.DataTable dtAM = Program.objHrmsUI.getDataTable(strSelect, "gettinggl");
            if (dtAM != null && dtAM.Rows.Count > 0)
            {
                acctCode = dtAM.Rows[0]["FormatCode"].ToString() + "-" + dtAM.Rows[0]["AcctName"].ToString();
                AssetCode = dtAM.Rows[0]["U_FaCode"].ToString();
            }

        }
        private void updateRDBaseUnitVal()
        {
            string strRuleCode = Convert.ToString( dtHeads.GetValue("RN", 0));
            if (strRuleCode != "")
            {
                Hashtable hsp = new Hashtable();
                hsp.Add("~p1", strRuleCode);
                hsp.Add("~p2", cbBU.Selected.Value.ToString());

                string strUpdate = Program.objHrmsUI.getQryString("AR_BU_UP", hsp);
                Program.objHrmsUI.ExecQuery(strUpdate, "Updating BU");
                MarkUpdateDate(strRuleCode);
                getRuleDetail(strRuleCode);
            }
        }

        private void addChildern(string addBy, string Father, string strRuleCode)
        {

            string strChildSel = "SELECT T0.\"Code\",IFNULL(\"U_FaCode\",'') as \"FA\"  FROM \"@B1_AM\"  T0 WHERE T0.\"U_Father\" ='" + Father + "'";
            System.Data.DataTable dtChildAM = Program.objHrmsUI.getDataTable(strChildSel, "Adding AM Childern");
            foreach (System.Data.DataRow dr in dtChildAM.Rows)
            {
                string FACode = dr["FA"].ToString();
                if (FACode != "")
                {
                    Hashtable hsp = new Hashtable();
                    hsp.Add("~p2", strRuleCode);
                    hsp.Add("~p3", dr["Code"].ToString());
                    hsp.Add("~p4", 0.00);

                    string strCnt = Program.objHrmsUI.getQryString("AR_D_CNT", hsp);
                    int oldCnt = Convert.ToInt32(Program.objHrmsUI.getScallerValue(strCnt));
                    if (oldCnt == 0)
                    {

                        long nextCode = Program.objHrmsUI.getMaxId("\"@B1_ARD\"", "\"Code\"");
                        hsp.Add("~p1", nextCode);
                        hsp.Add("~p5", addBy);

                        string strInsert = Program.objHrmsUI.getQryString("AR_ADD_D", hsp);
                        strInsert = "INSERT INTO \"@B1_ARD\" (\"Code\", \"Name\", \"U_RuleCode\", U_CMC, U_AP,\"U_AddBy\") VALUES ('~p1', '~p1', '~p2', '~p3', '~p4','~p5');";
                        foreach (string pm in hsp.Keys)
                        {
                            strInsert = strInsert.Replace(pm, hsp[pm].ToString());
                        }
                        Program.objHrmsUI.ExecQuery(strInsert, "Adding Detail");

                    }
                }
                else
                {
                    addChildern(addBy, dr["Code"].ToString(), strRuleCode);
                }
            }
        }
        private void filldetails(string RuleCode)
        {
            Hashtable hsp = new Hashtable();
            hsp.Add("~p1", RuleCode);

            string strSelect = Program.objHrmsUI.getQryString("AR_Fill_All", hsp);


            System.Data.DataTable dtRule = Program.objHrmsUI.getDataTable(strSelect, "getting rule detail");
            if (dtRule.Rows.Count > 0)
            {
                string Active = dtRule.Rows[0]["Active"].ToString();
                string baseUnit = dtRule.Rows[0]["BU"].ToString();
               if (baseUnit == "") baseUnit = "01";

                dtHeads.SetValue("Active", 0, Active);
                dtHeads.SetValue("RN", 0, RuleCode);
                dtHeads.SetValue("BU", 0, baseUnit);

                try
                {
                    string LastUpdated = dtRule.Rows[0]["LU"].ToString();
                    dtHeads.SetValue("LU", 0, LastUpdated);
                }
                catch { }

            }
            getRuleDetail(RuleCode);

        }
        private void InitiallizeForm()
        {


            oForm.Freeze(true);

            initiallizing = true;


            dtRM = oForm.DataSources.DataTables.Item("dtRM");
            dtRD = oForm.DataSources.DataTables.Item("dtRD");

            dtHeads = oForm.DataSources.DataTables.Item("dtHead");
            dtHeads.Rows.Add(1);
                      mtR = (SAPbouiCOM.Matrix)oForm.Items.Item("mtR").Specific;
            mtRD = (SAPbouiCOM.Matrix)oForm.Items.Item("mtRD").Specific;
            cbCM = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbCM").Specific;
            txAP = (SAPbouiCOM.EditText)oForm.Items.Item("txAP").Specific;
            txnRN = (SAPbouiCOM.EditText)oForm.Items.Item("txnRN").Specific;

            txRN = (SAPbouiCOM.EditText)oForm.Items.Item("txRN").Specific;

            btAddR = (SAPbouiCOM.Button)oForm.Items.Item("btAddR").Specific;
            btAddD = (SAPbouiCOM.Button)oForm.Items.Item("btAddD").Specific;
            btDelD = (SAPbouiCOM.Button)oForm.Items.Item("btDelD").Specific;
            btDelR = (SAPbouiCOM.Button)oForm.Items.Item("btDelR").Specific;
            cbBU =  (SAPbouiCOM.ComboBox)oForm.Items.Item("cbBU").Specific;

            cbBU.ValidValues.Add("01", "Size / Area");
            cbBU.ValidValues.Add("02", "Volume");
            cbBU.ValidValues.Add("03", "Floors");
            cbBU.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
            cbBU.Item.DisplayDesc = true;
            chActive = (SAPbouiCOM.CheckBox)oForm.Items.Item("chActive").Specific;
            oForm.Items.Item("btUpd").Enabled = false;
            oForm.Freeze(false);

            initiallizing = false;
            txnRN.Active = true;
            fillCb();
            getCodes();


        }

        private void ini_controls()
        {
            dtHeads.SetValue("RNN", 0, "");

            dtHeads.SetValue("RN", 0, "");
            dtHeads.SetValue("AP", 0, "0");
          
        }
       


    
       private void fillCb()
        {
            fillLavels();

        }
        private void fillLavels()
        {

            System.Data.DataTable dtRoot = Program.objHrmsUI.getDataTable("SELECT T0.\"Code\", T0.\"Name\" FROM \"@B1_AM_F\"  T0", "Fill Root");
         
            if (dtRoot.Rows.Count > 0)
            {
                int i = 0;
                foreach (System.Data.DataRow dr in dtRoot.Rows)
                {
                    cbCM.ValidValues.Add(dr["Code"].ToString(), dr["Name"].ToString());
                    i++;
                }
            }


           
        }

        private void fillChilds(string fatherCode,string Spacer)
        {
            Hashtable hp = new Hashtable();
            hp.Add("~p1", fatherCode);
            System.Data.DataTable dtRoot = Program.objHrmsUI.getDataTableQryCode("AM_FillChilds_001", hp, "Fill Root");


            foreach (System.Data.DataRow dr in dtRoot.Rows)
            {
                cbCM.ValidValues.Add(dr["Code"].ToString(), Spacer + dr["Name"].ToString());
                fillChilds(dr["Code"].ToString(), Spacer +  dr["Name"].ToString() + ">");
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

        private int ARInIDRCnt(string AR)
        {
            int result = 0;
            string strQuery = "SELECT COUNT(*) FROM \"@B1_IBOQ\" WHERE \"U_AR\" = '" + AR + "'";
            result = Convert.ToInt32(Program.objHrmsUI.getScallerValue(strQuery));

            return result;
        }

    
    

    }
}
