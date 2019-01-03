using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACHR.Screen
{
    class frm_DBOQ : HRMSBaseForm
    {

        public bool isForLoading = false;
        SAPbouiCOM.Matrix mtIC, mtDC;
        SAPbouiCOM.EditText txCode, txName, txDCB, txICB, txTCB, txDCA, txICA, txTCA, txRemarks;
        SAPbouiCOM.ChooseFromList cflOitm;
        SAPbouiCOM.Button  cmdDR;
       
         int rowNum = 0;

        SAPbouiCOM.DataTable BOQD,BOQH,BOQID;
        string itemClicked = "";


        private bool initiallizing = false;
        string printMenuId = "";
        private void InitiallizeForm()
        {


            oForm.Freeze(true);

            initiallizing = true;


            BOQD = oForm.DataSources.DataTables.Item("BOQD");
            BOQH = oForm.DataSources.DataTables.Item("BOQH");
            BOQID = oForm.DataSources.DataTables.Item("BOQID");

            BOQH.Rows.Add(1);

            mtIC = (SAPbouiCOM.Matrix)oForm.Items.Item("mtIC").Specific;
            mtDC = (SAPbouiCOM.Matrix)oForm.Items.Item("mtDC").Specific;

            cmdDR = (SAPbouiCOM.Button)oForm.Items.Item("cmdDR").Specific;
            SAPbouiCOM.Column col = mtDC.Columns.Item("cType");
            col.ValidValues.Add("S", "Service");
            col.ValidValues.Add("I", "Item");
            col.DisplayDesc = true;
            txCode = (SAPbouiCOM.EditText)oForm.Items.Item("txCode").Specific;
            txName = (SAPbouiCOM.EditText)oForm.Items.Item("txName").Specific;
            txDCB = (SAPbouiCOM.EditText)oForm.Items.Item("txDCB").Specific;
            txICB = (SAPbouiCOM.EditText)oForm.Items.Item("txICB").Specific;
            txTCB = (SAPbouiCOM.EditText)oForm.Items.Item("txTCB").Specific;
            txDCA = (SAPbouiCOM.EditText)oForm.Items.Item("txDCA").Specific;
            txICA = (SAPbouiCOM.EditText)oForm.Items.Item("txICA").Specific;
            txTCA = (SAPbouiCOM.EditText)oForm.Items.Item("txTCA").Specific;
            txRemarks = (SAPbouiCOM.EditText)oForm.Items.Item("txRemarks").Specific;
            cflOitm = (SAPbouiCOM.ChooseFromList)oForm.ChooseFromLists.Item("cflOitm");
            SAPbouiCOM.Conditions oCons = cflOitm.GetConditions();
            SAPbouiCOM.Condition oCon = oCons.Add();
            oCon.Alias = "ItemType";
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
            oCon.CondVal = "F";
            cflOitm.SetConditions(oCons);

            oForm.Freeze(false);

            initiallizing = false;


        }


        public override void CreateForm(SAPbouiCOM.Application SboApp, string strXml, SAPbobsCOM.Company cmp, string frmId)
        {
            base.CreateForm(SboApp, strXml, cmp, frmId);

            InitiallizeForm();

            oForm.EnableMenu("1282", false);  // Add New Record
            oForm.EnableMenu("1288", false);  // Next Record
            oForm.EnableMenu("1289", false);  // Pevious Record
            oForm.EnableMenu("1290", false);  // First Record
            oForm.EnableMenu("1291", false);  // Last record 
            oForm.EnableMenu("1281", false);  // Find record 
          //  getData();
          
        }
        public override void AddNewRecord()
        {
            base.AddNewRecord();

            iniControlls();
            txCode.Active = true;
        }
     
      
       
        public override void etBeforeCfl(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etBeforeCfl(ref pVal, ref BubbleEvent);
            SAPbouiCOM.IChooseFromListEvent oCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pVal;
            mtDC.FlushToDataSource();
            if (pVal.ItemUID == mtDC.Item.UniqueID && pVal.ColUID == "cCode")
            {
                SAPbouiCOM.ComboBox cbColType = (SAPbouiCOM.ComboBox)mtDC.Columns.Item("cType").Cells.Item(pVal.Row).Specific;
                SAPbouiCOM.Column CodCol = mtDC.Columns.Item("cCode");
                if (cbColType.Selected.Value == "I")
                {
                    CodCol.ChooseFromListUID = "cflrOITM";
                    CodCol.ChooseFromListAlias = "ItemCode";

                }
                if (cbColType.Selected.Value == "S")
                {
                    CodCol.ChooseFromListUID = "cflrGL";

                    CodCol.ChooseFromListAlias = "AcctCode";

                }
            }
        }
        public override void etAfterValidate(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterValidate(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == mtDC.Item.UniqueID && pVal.Row > 0 && pVal.ColUID == "cCostCode")
            {
                mtDC.FlushToDataSource();
                addEmptyRow(mtDC,BOQD,"CostCode");
            }
        }
        public override void etAfterClick(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == "1")
            {
                update();
            }
            if (pVal.ItemUID == cmdDR.Item.UniqueID)
            {
                int selRow = mtSelRow(mtDC);
                if (selRow > 0)
                {
                    double ActVal = Convert.ToDouble(((SAPbouiCOM.EditText)mtDC.GetCellSpecific("V_2", selRow)).Value);
                    string costingCode = Convert.ToString(((SAPbouiCOM.EditText)mtDC.GetCellSpecific("cCostCode", selRow)).Value);
                    if (ActVal > 0)
                    {
                        oApplication.MessageBox("You can not delete already consumed costing code");
                        return;
                    }
                    else
                    {

                        deleteRow(costingCode);
                        fillDetails(BOQH.GetValue("ItemCode", 0).ToString());
                        addEmptyRow(mtDC, BOQD, "CostCode");
                    }
                }
            }
        }
        public override void etAfterCfl(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCfl(ref pVal, ref BubbleEvent);
            SAPbouiCOM.IChooseFromListEvent oCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pVal;
            SAPbouiCOM.DataTable dtSel = oCFLEvento.SelectedObjects;
            if (pVal.ItemUID == txCode.Item.UniqueID)
            {
                if (dtSel!=null &&  dtSel.Rows.Count > 0)
                {
                    string strCode = dtSel.GetValue("ItemCode", 0).ToString();
                    string strName = dtSel.GetValue("ItemName", 0).ToString();
                    string Remarks = dtSel.GetValue("U_B1_BOQREM", 0).ToString();
                    string Qty = dtSel.GetValue("U_B1_BOQQty", 0).ToString();
                    double APC = getAPCCost(strCode);
                    BOQH.SetValue("ItemCode", 0, strCode);
                    BOQH.SetValue("ItemName", 0, strName);
                    BOQH.SetValue("Remarks", 0, Remarks);
                    BOQH.SetValue("Qty", 0, Qty);
                    BOQH.SetValue("APC", 0, APC);

                    fillDetails(strCode);
                    getIndirectCost(strCode);
                
                    addEmptyRow(mtDC,BOQD,"CostCode");
                }
            }

            if (pVal.ItemUID == mtDC.Item.UniqueID && pVal.ColUID == "cCode")
            {
                if (dtSel != null &&  dtSel.Rows.Count > 0)
                {
                    SAPbouiCOM.ComboBox cbColType = (SAPbouiCOM.ComboBox)mtDC.Columns.Item("cType").Cells.Item(pVal.Row).Specific;
                    if (cbColType.Selected.Value == "I")
                    {
                        string strCode = dtSel.GetValue("ItemCode", 0).ToString();
                        string strName = dtSel.GetValue("ItemName", 0).ToString();
                        BOQD.SetValue("Code", pVal.Row - 1, strCode);
                        BOQD.SetValue("Name", pVal.Row - 1, strName);

                    }
                    if (cbColType.Selected.Value == "S")
                    {
                        string strCode = dtSel.GetValue("AcctCode", 0).ToString();
                        string strName = dtSel.GetValue("AcctName", 0).ToString();
                        BOQD.SetValue("Code", pVal.Row - 1, strCode);
                        BOQD.SetValue("Name", pVal.Row - 1, strName);

                    }
                    mtDC.LoadFromDataSource();
                }
            }

             
        }

        private double getAPCCost(string FACode)
        {
            double result = 0.00;

            try
            {
                string strSelect = " SELECT TOP 1 \"APC\" FROM ITM8 T0 WHERE T0.\"ItemCode\" = '" + FACode + "'";
                System.Data.DataTable dt = Program.objHrmsUI.getDataTable(strSelect, "Getting APC");
                if (dt != null && dt.Rows.Count > 0)
                {
                    result = Convert.ToDouble(dt.Rows[0]["APC"]);
                }
            }
            catch { }

            try
            {
                string strSelect = "SELECT SUM(T0.\"LineTotal\") AS \"APC\" FROM \"ACQ1\" T0 INNER JOIN \"OACQ\" T1 ON T0.\"DocEntry\" = T1.\"DocEntry\"   WHERE T1.\"DocStatus\" = 'P' AND  T0.\"ItemCode\" = '" + FACode + "'";
                System.Data.DataTable dt = Program.objHrmsUI.getDataTable(strSelect, "Getting APC");
                if (dt != null && dt.Rows.Count > 0)
                {
                    result = result +  Convert.ToDouble(dt.Rows[0]["APC"]);
                }
            }
            catch { }

            try
            {
                string strSelect = "SELECT SUM(T0.\"LineTotal\") AS \"APC\" FROM \"ACD1\" T0 INNER JOIN \"OACD\" T1 ON T0.\"DocEntry\" = T1.\"DocEntry\"   WHERE T1.\"DocStatus\" = 'P' AND  T0.\"ItemCode\" = '" + FACode + "'";
                System.Data.DataTable dt = Program.objHrmsUI.getDataTable(strSelect, "Getting APC");
                if (dt != null && dt.Rows.Count > 0)
                {
                    result = result -  Convert.ToDouble(dt.Rows[0]["APC"]);
                }
            }
            catch { }

            return result;
        }

        private void getIndirectCost(string FACode)
        {
            BOQID.Rows.Clear();
            System.Data.DataTable dtIndirectCostCode = new System.Data.DataTable();
            dtIndirectCostCode.Columns.Add("CostCode");
            dtIndirectCostCode.Columns.Add("AR");
            dtIndirectCostCode.Columns.Add("Budget");

            System.Data.DataTable AMS = new System.Data.DataTable();
            AMS.Columns.Add("AM");
            AMS.Columns.Add("Father");


            Hashtable hp = new Hashtable();
            hp.Add("~p1", FACode);
            System.Data.DataTable dtFA = Program.objHrmsUI.getDataTableQryCode("DBOQ_IC_001", hp, "Fill Root");

            if (dtFA.Rows.Count > 0)
            {
                foreach (System.Data.DataRow drFA in dtFA.Rows)
                {
                    string AM = drFA["Code"].ToString();
                    string Father = drFA["U_Father"].ToString();
                    while (AM != "0")
                    {
                        AMS.Rows.Add(AM, Father);
                        hp.Clear();
                        hp.Add("~p1", Father);
                        string strFather = Program.objHrmsUI.getQryString("DBOQ_IC_002", hp);

                        System.Data.DataTable dtFather = Program.objHrmsUI.getDataTable(strFather, "Father");
                        if (dtFather.Rows.Count > 0)
                        {
                            AM = dtFather.Rows[0]["Code"].ToString();
                            Father = dtFather.Rows[0]["U_Father"].ToString();

                        }
                        else
                        {
                            Father = "0";
                            AM = "Root";
                        }

                    }
                }
            }
            double indirectCostBudget = 0.00;
            double indirectCostActual = 0.00;
            foreach (System.Data.DataRow drAM in AMS.Rows)
            {
                hp.Clear();
                hp.Add("~p1", drAM["AM"].ToString());
                string strSqlAR = Program.objHrmsUI.getQryString("DBOQ_IC_003", hp);
                System.Data.DataTable dtRule = Program.objHrmsUI.getDataTable(strSqlAR, "Allocation Rule");
                if (dtRule.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow drAR in dtRule.Rows)
                    {
                        hp.Clear();
                        hp.Add("~p1", drAR["AR"].ToString());
                        string strCosts = Program.objHrmsUI.getQryString("DBOQ_IC_004", hp);
                        System.Data.DataTable dtCostCodes = Program.objHrmsUI.getDataTable(strCosts, "Indirect Cost Codes");
                        foreach (System.Data.DataRow drCostCode in dtCostCodes.Rows)
                        {
                            BOQID.Rows.Add(1);
                            BOQID.SetValue("CostCode", BOQID.Rows.Count - 1, drCostCode["code"].ToString());
                            BOQID.SetValue("STD", BOQID.Rows.Count - 1, Convert.ToDateTime(drCostCode["U_WBSStd"]));
                            BOQID.SetValue("ETD", BOQID.Rows.Count - 1, Convert.ToDateTime(drCostCode["U_WBSED"]));
                            BOQID.SetValue("WBL", BOQID.Rows.Count - 1, drCostCode["U_WBSLevel"].ToString());
                            BOQID.SetValue("WBD", BOQID.Rows.Count - 1, drCostCode["U_WBSDscr"].ToString());
                            double indirectCost = getIndirectACt(drCostCode["code"].ToString(), FACode);
                            double indirectBgtCost = Convert.ToDouble(drCostCode["U_BgtCost"]) * (Convert.ToDouble(drAR["AP"]) / 100.00);

                            BOQID.SetValue("BGTC", BOQID.Rows.Count - 1, indirectBgtCost);
                            BOQID.SetValue("ACTC", BOQID.Rows.Count - 1, indirectCost);
                            BOQID.SetValue("Remarks", BOQID.Rows.Count - 1, drCostCode["U_Remarks"].ToString());

                            indirectCostActual += indirectCost;
                            indirectCostBudget += indirectBgtCost;
                        }
                    }
                }

            }
            BOQH.SetValue("ICB", 0, indirectCostBudget);
            BOQH.SetValue("ICA", 0, indirectCostActual);

            BOQH.SetValue("TCB", 0, indirectCostBudget + Convert.ToDouble(BOQH.GetValue("DCB", 0)));
            BOQH.SetValue("TCA", 0, indirectCostActual + Convert.ToDouble(BOQH.GetValue("DCA", 0)));

            mtIC.LoadFromDataSource();

        }


        private double getIndirectACt(string costCode, string FACode)
        {
            double result = 0.00;
            Hashtable hp = new Hashtable();
            hp.Add("~p1", costCode);
            hp.Add("~p2", FACode);

            string strIndirectActual = Program.objHrmsUI.getQryString("DBOQ_IC_005", hp);
               
            result = Convert.ToDouble( Program.objHrmsUI.getScallerValue(strIndirectActual));

            return result;

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

        private void iniControlls()
        {
            BOQH.Rows.Clear();
            BOQH.Rows.Add(1);
            BOQD.Rows.Clear();

            BOQID.Rows.Clear();

            BOQD.Rows.Add(1);
            mtDC.LoadFromDataSource();
            mtIC.LoadFromDataSource();
           
        }


        private void fillDetails(string itemCode)
        {
            BOQD.Rows.Clear();
            double dbgtTotal = 0.00;
            double dactTotal = 0.00;

         //   addEmptyRow(mtDC, BOQD, "CostCode");
            Hashtable hp = new Hashtable();
            hp.Add("~p1", itemCode);
            string strQD = Program.objHrmsUI.getQryString("DBOQ_FD_005", hp);
             
            System.Data.DataTable dtQD = Program.objHrmsUI.getDataTable(strQD, "getting details");
            if (dtQD.Rows.Count > 0)
            {
                BOQD.Rows.Add(dtQD.Rows.Count);
                int i = 0;
                foreach (System.Data.DataRow dr in dtQD.Rows)
                {
                    BOQD.SetValue("Id", i, (i + 1).ToString());
                    BOQD.SetValue("CostCode", i, dr["Code"].ToString());
                    BOQD.SetValue("STD", i, Convert.ToDateTime(dr["U_WBSStD"]).ToString("yyyyMMdd"));
                    BOQD.SetValue("ETD", i, Convert.ToDateTime(dr["U_WBSED"]).ToString("yyyyMMdd"));
                    BOQD.SetValue("WBL", i, Convert.ToString(dr["U_WBSLevel"]).ToString());
                    BOQD.SetValue("WBD", i, Convert.ToString(dr["U_WBSDscr"]).ToString());
                    BOQD.SetValue("Type", i, Convert.ToString(dr["U_Type"]).ToString());
                    BOQD.SetValue("Code", i, Convert.ToString(dr["U_rCode"]).ToString());
                    BOQD.SetValue("Name", i, Convert.ToString(dr["U_rName"]).ToString());
                    BOQD.SetValue("Remarks", i, Convert.ToString(dr["U_Remarks"]).ToString());
                    BOQD.SetValue("Qty", i, Convert.ToString(dr["U_Qty"]).ToString());
                    BOQD.SetValue("UP", i, Convert.ToString(dr["U_Price"]).ToString());
                    BOQD.SetValue("BGTC", i, Convert.ToString(dr["U_BgtCost"]).ToString());
                    double ActValue = getActCost(dr["Code"].ToString());
                    BOQD.SetValue("ACTC", i, ActValue);

                    dbgtTotal += Convert.ToDouble(dr["U_BgtCost"]);
                    dactTotal += ActValue;

                    i++;


                }
            }
            mtDC.LoadFromDataSource();
            BOQH.SetValue("DCB", 0, dbgtTotal);
            BOQH.SetValue("DCA", 0, dactTotal);

            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
        }

        private void update()
        {
            string ItemCode = Convert.ToString( BOQH.GetValue("ItemCode", 0));
            string remarks = Convert.ToString(BOQH.GetValue("Remarks", 0));
            string Qty = Convert.ToString(BOQH.GetValue("Qty", 0));

          

            if (ItemCode != "")
            {
                Hashtable hp = new Hashtable();
                hp.Add("~p1", remarks);
                hp.Add("~p2", Qty);
                hp.Add("~p3", ItemCode);

                string strUpdate = Program.objHrmsUI.getQryString("DBOQ_UP_001", hp);
            
                Program.objHrmsUI.ExecQuery(strUpdate, "Updating description");
                updateDCodes(ItemCode);


            }
        }
        private double getActCost(string CostCode)
        {
            double result = 0.00;
            if (CostCode != "")
            {

                Hashtable hp = new Hashtable();
                hp.Add("~p1", CostCode);

                string strAct = Program.objHrmsUI.getQryString("DBOQ_GET_001", hp);

                result = Convert.ToDouble(Program.objHrmsUI.getScallerValue(strAct));

            }


            return result;
        }
           

        private void updateDCodes(string itemCode)
        {
            mtDC.FlushToDataSource();
            string strExec = "";
            for (int i = 0; i < BOQD.Rows.Count; i++)
            {
                string costCode = Convert.ToString(BOQD.GetValue("CostCode", i));
                if (costCode != "")
                {

                    DateTime WBSStd = Convert.ToDateTime(BOQD.GetValue("STD", i));
                    DateTime WBSED = Convert.ToDateTime(BOQD.GetValue("ETD", i));
                    string WBSLevel = Convert.ToString(BOQD.GetValue("WBL", i));
                    string WBSDscr = Convert.ToString(BOQD.GetValue("WBD", i));
                    string rType = Convert.ToString(BOQD.GetValue("Type", i));
                    string rCode = Convert.ToString(BOQD.GetValue("Code", i));
                    string rName = Convert.ToString(BOQD.GetValue("Name", i));
                    string Remarks = Convert.ToString(BOQD.GetValue("Remarks", i));

                    double qty = Convert.ToDouble(BOQD.GetValue("Qty", i));
                    double Price = Convert.ToDouble(BOQD.GetValue("UP", i));
                    double BDGTC = Convert.ToDouble(BOQD.GetValue("BGTC", i));

                    Hashtable hp = new Hashtable();
                    hp.Add("~p1", costCode);
                    hp.Add("~p2", itemCode);
                    string strCnt = Program.objHrmsUI.getQryString("DBOQ_GET_002", hp);

                    int cnt = Convert.ToInt32(Program.objHrmsUI.getScallerValue(strCnt));

                       hp.Clear();
                       hp.Add("~p00", costCode);
                        hp.Add("~p01", WBSStd.ToString("yyyyMMdd") );
                        hp.Add("~p02",  WBSED.ToString("yyyyMMdd"));
                        hp.Add("~p03", WBSLevel);
                        hp.Add("~p04", WBSDscr);
                        hp.Add("~p05", rType);
                        hp.Add("~p06", rCode);
                        hp.Add("~p07", rName);
                        hp.Add("~p08", qty.ToString());
                        hp.Add("~p09", Price.ToString());
                        hp.Add("~p10", BDGTC.ToString());
                        hp.Add("~p11", Remarks);
                        hp.Add("~p12", itemCode);
                        hp.Add("~p13", "D");


                    if (cnt > 0)
                    {
                       

                        strExec = Program.objHrmsUI.getQryString("DBOQ_D_UP", hp);
                        Program.objHrmsUI.ExecQuery(strExec, "Updating Detail");


                    }

                    else
                    {
                        strExec = Program.objHrmsUI.getQryString("DBOQ_D_IN", hp);
                      //  strExec = "INSERT INTO \"@B1_DBOQD\" (\"Code\", \"Name\", \"U_WBSStD\", \"U_WBSED\", \"U_WBSLevel\", \"U_WBSDscr\", \"U_Type\", \"U_rCode\", \"U_rName\", \"U_Qty\", \"U_Price\", \"U_BgtCost\", \"U_Remarks\", \"U_ItemCode\", \"U_BOQType\") VALUES ('" + hp["~p00"].ToString() + "', '" + hp["~p00"].ToString() + "', '" + hp["~p01"].ToString() + "', '" + hp["~p02"].ToString() + "', '" + hp["~p03"].ToString() + "', '" + hp["~p04"].ToString() + "', '" + hp["~p05"].ToString() + "', '" + hp["~p06"].ToString() + "', '" + hp["~p07"].ToString() + "', '" + hp["~p08"].ToString() + "', '" + hp["~p09"].ToString() + "', '" + hp["~p10"].ToString() + "', '" + hp["~p11"].ToString() + "', '" + hp["~p12"].ToString() + "', '" + hp["~p13"].ToString() + "')";

                        Program.objHrmsUI.ExecQuery(strExec, "Updating Detail");

                    }


                }
            }
           
        }


        private void addEmptyRow(SAPbouiCOM.Matrix mt, SAPbouiCOM.DataTable dt, string firstCol)
        {


            if (dt.Rows.Count == 0)
            {
                dt.Rows.Add(2);
                dt.Rows.Remove(1);
                dt.SetValue("Id", 0, "1");
                mt.AddRow(1, mt.RowCount + 1);
            }
            else
            {
                if (Convert.ToString( dt.GetValue(firstCol, dt.Rows.Count - 1)) == "")
                {
                    string strCostCode = Convert.ToString(dt.GetValue(firstCol, dt.Rows.Count - 1));


                }
                else
                {
                    mt.AddRow(1, mt.RowCount + 1);
                    mt.SetLineData(mt.RowCount);
                    mt.FlushToDataSource();

                  
                    dt.SetValue("Id", dt.Rows.Count - 1, dt.Rows.Count.ToString());
                    dt.SetValue(firstCol, dt.Rows.Count - 1, "");



                    BOQD.SetValue("STD", dt.Rows.Count - 1,"");
                    BOQD.SetValue("ETD", dt.Rows.Count - 1,"");
                    BOQD.SetValue("WBL", dt.Rows.Count - 1, "");
                    BOQD.SetValue("WBD", dt.Rows.Count - 1, "");
                    BOQD.SetValue("Type", dt.Rows.Count - 1, "");
                    BOQD.SetValue("Code", dt.Rows.Count - 1, "");
                    BOQD.SetValue("Name", dt.Rows.Count - 1, "");
                    BOQD.SetValue("Remarks", dt.Rows.Count - 1, "");
                    BOQD.SetValue("Qty", dt.Rows.Count - 1, "0");
                    BOQD.SetValue("UP", dt.Rows.Count - 1,"0");
                    BOQD.SetValue("BGTC", dt.Rows.Count - 1,"0");



                    mt.SetLineData(mt.RowCount);

                 
                }

            }
        //    mt.LoadFromDataSource();

        }
        private void deleteRow(string code)
        {

            string strSql = "DELETE from \"@B1_DBOQD\" WHERE \"Code\" = '" + code + "'";

            Program.objHrmsUI.ExecQuery(strSql, "Deleting Code");

        }

    }
}
