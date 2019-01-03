using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACHR.Screen
{
    class frm_IBOQ : HRMSBaseForm
    {

        public bool isForLoading = false;
        SAPbouiCOM.Matrix mtIC, mtDC;
        SAPbouiCOM.EditText txCode, txName, txDCB, txICB, txTCB, txDCA, txICA, txTCA, txRemarks, txACode, txAName;
        SAPbouiCOM.ChooseFromList cflOitm;
        SAPbouiCOM.Button cmdDR;
        SAPbouiCOM.ComboBox cbRule;
       
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
            cbRule = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbRule").Specific;
            txACode = (SAPbouiCOM.EditText)oForm.Items.Item("txACode").Specific;
            txAName = (SAPbouiCOM.EditText)oForm.Items.Item("txAName").Specific;

            txRemarks = (SAPbouiCOM.EditText)oForm.Items.Item("txRemarks").Specific;
            cflOitm = (SAPbouiCOM.ChooseFromList)oForm.ChooseFromLists.Item("cflOitm");
            cmdDR = (SAPbouiCOM.Button)oForm.Items.Item("cmdDR").Specific;
           
            SAPbouiCOM.Conditions oCons = cflOitm.GetConditions();
            SAPbouiCOM.Condition oCon = oCons.Add();
            oCon.Alias = "ItemType";
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
            oCon.CondVal = "F";
            cflOitm.SetConditions(oCons);
            fillCB();
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
                    double ActVal = Convert.ToDouble( ((SAPbouiCOM.EditText)mtDC.GetCellSpecific("V_2", selRow)).Value);
                    string costingCode = Convert.ToString(((SAPbouiCOM.EditText)mtDC.GetCellSpecific("cCostCode", selRow)).Value);
                   if (ActVal > 0)
                    {
                        oApplication.MessageBox("You can not delete already consumed costing code");
                        return;
                    }
                    else
                    {

                        deleteRow(costingCode );
                        fillDetails(BOQH.GetValue("ItemCode", 0).ToString());
                        addEmptyRow(mtDC, BOQD, "CostCode");
                    }
                }
            }
            if (pVal.ItemUID == "btAdd")
            {

                if (txACode.Value == "" || txAName.Value == "")
                {
                }
                else
                {

                    SAPbobsCOM.GeneralService oGeneralService;
                    SAPbobsCOM.GeneralData oGeneralData;

                    SAPbobsCOM.CompanyService cmpserv = oCompany.GetCompanyService();


                    oGeneralService = cmpserv.GetGeneralService("oIBOQ");

                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);
                    oGeneralData.SetProperty("Code", txACode.Value.ToString());
                    oGeneralData.SetProperty("Name", txAName.Value.ToString());

                    oGeneralService.Add(oGeneralData);



                    txACode.Value = "";
                    txAName.Value = "";
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
                if (dtSel.Rows.Count > 0)
                {
                    string strCode = dtSel.GetValue("Code", 0).ToString();
                    string strName = dtSel.GetValue("Name", 0).ToString();
                    string Remarks = dtSel.GetValue("U_Remarks", 0).ToString();
                    string AllocationRule = dtSel.GetValue("U_AR", 0).ToString();
                    BOQH.SetValue("ItemCode", 0, strCode);
                    BOQH.SetValue("ItemName", 0, strName);
                    BOQH.SetValue("Remarks", 0, Remarks);
                    BOQH.SetValue("AR", 0, AllocationRule);
                    fillDetails(strCode);
                    addEmptyRow(mtDC, BOQD, "CostCode");
                }
            }

            if (pVal.ItemUID == mtDC.Item.UniqueID && pVal.ColUID == "cCode")
            {
                if (dtSel.Rows.Count > 0)
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

        private void fillCB()
        {
            Hashtable hp = new Hashtable();

            string strRule = Program.objHrmsUI.getQryString("IBOQ_GET_001", hp);// "Select * from [@B1_AR] where isnull(U_Active,'N') = 'Y' ";
            System.Data.DataTable dtRule = Program.objHrmsUI.getDataTable(strRule, "Getting Rule");
            foreach (System.Data.DataRow dr in dtRule.Rows)
            {
                cbRule.ValidValues.Add(dr["Code"].ToString(), dr["Code"].ToString());
            }
        }
        private void fillDetails(string itemCode)
        {
            BOQD.Rows.Clear();
            double dbgtTotal = 0.00;
            double dactTotal = 0.00;

         //   addEmptyRow(mtDC, BOQD, "CostCode");
           // string strQD = "   Select * from [@B1_DBOQD] where  u_ItemCode='" + itemCode + "'";
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
            BOQH.SetValue("ICB", 0, dbgtTotal);
            BOQH.SetValue("ICA", 0, dactTotal);

            oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
        }

        private void update()
        {
            string ItemCode = Convert.ToString( BOQH.GetValue("ItemCode", 0));
            string remarks = Convert.ToString(BOQH.GetValue("Remarks", 0));
            if (ItemCode != "")
            {
               
                Hashtable hp = new Hashtable();
                hp.Add("~p1", remarks);
                hp.Add("~p2", cbRule.Selected.Value.ToString().Trim());
                hp.Add("~p3", ItemCode);

                string strUpdate = Program.objHrmsUI.getQryString("IBOQ_CRUD_001", hp);
               // string strUpdate = "Update \"@B1_IBOQ\" set \"U_Remarks\" = '" + remarks + "',U_AR = '" + cbRule.Selected.Value.ToString().Trim() + "' where Code = '" + ItemCode + "'";


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


                string strAct = Program.objHrmsUI.getQryString("IBOQ_GET_003", hp); ; //"select isnull( sum(linetotal),0) as ActVal from pch1 where  isnull(U_CostCode,'')  = '~p1' ";
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
                    hp.Add("~p01", WBSStd.ToString("yyyyMMdd"));
                    hp.Add("~p02", WBSED.ToString("yyyyMMdd"));
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
                    hp.Add("~p13", "I");


                    if (cnt > 0)
                    {


                        strExec = Program.objHrmsUI.getQryString("DBOQ_D_UP", hp);



                    }

                    else
                    {
                        strExec = Program.objHrmsUI.getQryString("DBOQ_D_IN", hp);



                    }

                    Program.objHrmsUI.ExecQuery(strExec, "Updating Detail");
          
                }
            }
            if (strExec != "")
            {
             //   Program.objHrmsUI.ExecQuery(strExec, "Updating Detail");
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
