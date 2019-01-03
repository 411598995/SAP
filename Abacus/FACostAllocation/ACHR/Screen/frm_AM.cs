using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace ACHR.Screen
{
    class frm_AM : HRMSBaseForm
    {

        public bool isForLoading = false;
        SAPbouiCOM.Matrix mtCA;
        SAPbouiCOM.ComboBox cbProj, cbBsUnt;
        SAPbouiCOM.EditText txFCode, txFName, txCode, txName, txActCod, txArea, txVolume, txLU;
        SAPbouiCOM.ButtonCombo btAdd;
        SAPbouiCOM.Button btAC,btAS,cmdOK,btDel;
        int rowNum = 0;
        int curDtRow = 0;
        SAPbouiCOM.DataTable dtCA,dtHeads;
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

        public override void etAfterCfl(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCfl(ref pVal, ref BubbleEvent);

            if (pVal.ItemUID == txActCod.Item.UniqueID)
            {
                int rowind = pVal.Row;
                SAPbouiCOM.IChooseFromListEvent oCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pVal;
                SAPbouiCOM.DataTable dtSel = oCFLEvento.SelectedObjects;
                if (dtSel == null) return;
                if (dtSel.Rows.Count > 0)
                {

                    string strCode = dtSel.GetValue("AcctCode", 0).ToString();
                    string strName = dtSel.GetValue("AcctName", 0).ToString();
                    string strFC = dtSel.GetValue("FormatCode", 0).ToString();
                 
                    dtHeads.SetValue("AccCode", 0, strCode);
                    dtHeads.SetValue("AccFC", 0, strFC);
                    dtHeads.SetValue("AcctName", 0, strName);
                   
                    //bt.Item.Click();
                }
            }
            if (pVal.ItemUID == "txFAC")
            {
                int rowind = pVal.Row;
                SAPbouiCOM.IChooseFromListEvent oCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pVal;
                SAPbouiCOM.DataTable dtSel = oCFLEvento.SelectedObjects;
                if (dtSel == null) return;
                if (dtSel.Rows.Count > 0)
                {

                    string strCode = dtSel.GetValue("ItemCode", 0).ToString();
                    string strName = dtSel.GetValue("ItemName", 0).ToString();

                    dtHeads.SetValue("FAC", 0, strCode);
                    dtHeads.SetValue("FAN", 0, strName);

                    //bt.Item.Click();
                }
            }

        }
        public override void etAfterClick(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == mtCA.Item.UniqueID && pVal.Row > 0 && pVal.Row <= mtCA.RowCount)
            {
                mtCA.SelectRow(pVal.Row, true, false);
                string selCode = Convert.ToString(dtCA.GetValue("Code", pVal.Row - 1));
                fillDetail(selCode);
                cmdOK.Caption = "Update";
                txName.Active = true;
                txCode.Item.Enabled = false;
            }

            if (pVal.ItemUID == btDel.Item.UniqueID)
            {
                int selRow = mtSelRow(mtCA);
                if (selRow > 0)
                {
                    int cntdelAM = oApplication.MessageBox("Are you sure you want to delete (Y/N) ", 2, "Yes", "No");
                    if (cntdelAM == 1)
                    {
                        string selCode = Convert.ToString(dtCA.GetValue("Code", selRow - 1));

                        delCode(selCode);
                        ini_controls();
                    }
                }

            }

            if (pVal.ItemUID == cmdOK.Item.UniqueID && cmdOK.Caption == "Add")
            {
                int selRow = mtSelRow(mtCA);
                
                addUpdateCode();
                ini_controls();
                mtCA.SelectRow(selRow,true,false);
            }
            if (pVal.ItemUID == cmdOK.Item.UniqueID && cmdOK.Caption == "Update")
            {
                int selRow = mtSelRow(mtCA);

                addUpdateCode();
               
                mtCA.SelectRow(selRow, true, false);
            }

            if (pVal.ItemUID == "btAC")
            {
                int selRow = mtSelRow(mtCA);
                if (selRow > 0)
                {
                    txCode.Item.Enabled = true;
                   
                    ini_controls();
                    string strFatherCode = Convert.ToString(dtCA.GetValue("Name", selRow - 1));
                    string[] strFather = strFatherCode.Split('>');
                    string fatherCode = strFather[0].Replace(".","");
                    string fatherName = strFather[1];
                    dtHeads.SetValue("FatherC", 0, fatherCode);
                    dtHeads.SetValue("FatherN", 0, fatherName);
                    cmdOK.Caption = "Add";
                    txCode.Item.Enabled = true;

                }
            }
            if (pVal.ItemUID == "btAS")
            {
                int selRow = mtSelRow(mtCA);
                if (selRow > 0)
                {
                    txCode.Item.Enabled = true;
                   
                    ini_controls();
                    string strFatherCode = Convert.ToString(dtCA.GetValue("Name", selRow - 1));
                    cmdOK.Caption = "Add";
              

                }
            }
            
        }

        public override void etAfterCmbSelect(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCmbSelect(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == btAdd.Item.UniqueID)
            {
               // changeStatus();

               // fillReport();
                btAdd.Caption = "Add New Code";
                ini_controls();
                oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE;


            }
        }
        private void InitiallizeForm()
        {


            oForm.Freeze(true);

            initiallizing = true;
          

            dtCA = oForm.DataSources.DataTables.Item("dtCA");
            dtHeads = oForm.DataSources.DataTables.Item("dtHead");
            dtHeads.Rows.Add(1);
            dtHeads.SetValue("FatherC", 0, "0");
            dtHeads.SetValue("FatherN", 0, "Root");

            mtCA = (SAPbouiCOM.Matrix)oForm.Items.Item("mtCA").Specific;
            cbBsUnt = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbBsUnt").Specific;
            txFCode = (SAPbouiCOM.EditText)oForm.Items.Item("txFCode").Specific;
            txActCod = (SAPbouiCOM.EditText)oForm.Items.Item("txActCod").Specific;
            txLU = (SAPbouiCOM.EditText)oForm.Items.Item("txLU").Specific;
            txCode = (SAPbouiCOM.EditText)oForm.Items.Item("txCode").Specific;
            txName = (SAPbouiCOM.EditText)oForm.Items.Item("txName").Specific;

            btAdd = (SAPbouiCOM.ButtonCombo)oForm.Items.Item("btAdd").Specific;
           
            cmdOK = (SAPbouiCOM.Button)oForm.Items.Item("cmdOk").Specific;
            btDel = (SAPbouiCOM.Button)oForm.Items.Item("btDel").Specific;

            btAdd.ValidValues.Add("0","New code on same level");
            btAdd.ValidValues.Add("1", "New code on child level");

            cbBsUnt.Item.DisplayDesc = true;
           
            cbBsUnt.ValidValues.Add("01", "Size / Area");
            cbBsUnt.ValidValues.Add("02", "Volume");
            cbBsUnt.ValidValues.Add("03", "Floors");
            cbBsUnt.Item.Visible = false;

            cbBsUnt.Select(0,SAPbouiCOM.BoSearchKey.psk_Index);

            cmdOK.Caption = "Add";

            oForm.Freeze(false);
           
           initiallizing = false;
           updateCA();
           txCode.Active = true;
           cmdOK.Caption = "Add";
           txCode.Item.Enabled = true;


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
            dtHeads.SetValue("AccFC", 0, "");

            dtHeads.SetValue("FAC", 0, "");
            dtHeads.SetValue("FAN", 0, "");
           

            dtHeads.SetValue("BaseUnit", 0, "01");
            txCode.Active = true;

        }
       


        private void updateCA()
        {
            oForm.Freeze(true);
            dtCA.Rows.Clear();
            rowNum = 0;
            fillLevels();
            mtCA.LoadFromDataSource();
            oForm.Freeze(false);
        }

       private void fillCb()
        {
           

        }
        private void fillLevels()
        {

            Hashtable hp = new Hashtable();
          //  string strDelete = " CALL GET_AM_F();  ";
          //  Program.objHrmsUI.ExecQuery(strDelete, "Creating Level");
         //   System.Data.DataTable dtRoot = Program.objHrmsUI.getDataTableQryCode("AM_FillAM", hp, "Fill Root");
            System.Data.DataTable dtRoot = Program.objHrmsUI.getDataTable("CALL GET_AM_F();", "Fill Root");
            dtRoot = Program.objHrmsUI.getDataTable("SELECT * FROM \"@B1_AM_F\"", "Fill Root");

            dtCA.Rows.Clear();

            if (dtRoot.Rows.Count > 0)
            {
                dtCA.Rows.Add(dtRoot.Rows.Count);
                int i=0;
                foreach (System.Data.DataRow dr in dtRoot.Rows)
                {
                    dtCA.SetValue("Code", i, dr["Code"].ToString());
                    dtCA.SetValue("Name", i, dr["Name"].ToString());
                    i++;
                }
            }
            else
            {
                addRoot();
            }
            oApplication.StatusBar.SetText("Level updated successfully", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);

               
        }

        private void addRoot()
        {
            Hashtable hp = new Hashtable();

            int rootCnt = Convert.ToInt16(Program.objHrmsUI.getScallerValueQryCode("AM_RootExist",hp));

            if (rootCnt == 0)
            {
             
                 string strInsert = Program.objHrmsUI.getQryString("AM_AddRoot", hp); //,  "Insert into [@b1_AM](Code,Name,U_Father) Values ('0','Root','-1')";
                Program.objHrmsUI.ExecQuery(strInsert, "Adding Root");
            }
        }
        private void fillChilds(string fatherCode, string Spacer)
        {
            Hashtable hp = new Hashtable();
            hp.Add("~p1", fatherCode);
            System.Data.DataTable dtRoot = Program.objHrmsUI.getDataTableQryCode("AM_FillChilds_001", hp, "Fill Root");
          
            foreach (System.Data.DataRow dr in dtRoot.Rows)
            {

                   fillChilds(dr["Code"].ToString(), Spacer + "    ");
            }
        }
        private void fillDetail(string Code)
        {
            Hashtable hsp = new Hashtable();
            hsp.Add("~p1", Code);
            string strSql = Program.objHrmsUI.getQryString("AM_FillDetail", hsp);

            System.Data.DataTable dtCode = Program.objHrmsUI.getDataTable(strSql, "Fill Code");
            if (dtCode != null && dtCode.Rows.Count > 0)
            {
                dtHeads.SetValue("Code", 0, dtCode.Rows[0]["Code"].ToString());
                dtHeads.SetValue("Name", 0, dtCode.Rows[0]["Name"].ToString());
                dtHeads.SetValue("FatherC", 0, dtCode.Rows[0]["U_Father"].ToString());
                  
                dtHeads.SetValue("FatherN", 0, dtCode.Rows[0]["FatherName"].ToString());


                dtHeads.SetValue("Area", 0, dtCode.Rows[0]["U_Area"].ToString());
                dtHeads.SetValue("Volume", 0, dtCode.Rows[0]["U_Volume"].ToString());
                dtHeads.SetValue("Floors", 0, dtCode.Rows[0]["U_Floors"].ToString());
                dtHeads.SetValue("AccCode", 0, dtCode.Rows[0]["U_AcctCode"].ToString());

                dtHeads.SetValue("AcctName", 0, dtCode.Rows[0]["acctName"].ToString());
                dtHeads.SetValue("AccFC", 0, getAcctFmCode( dtCode.Rows[0]["U_AcctCode"].ToString()));
              
                dtHeads.SetValue("FAC", 0, dtCode.Rows[0]["U_FaCode"].ToString());
               
                dtHeads.SetValue("FAN", 0, dtCode.Rows[0]["ItemName"].ToString());

                dtHeads.SetValue("BaseUnit", 0, dtCode.Rows[0]["U_AlcBsUnt"].ToString());
                try
                {
                    dtHeads.SetValue("LU", 0, dtCode.Rows[0]["U_LstUpdate"].ToString());
                }
                catch { }

            }

        }
        private  string getAcctFmCode(string strCode)
        {

            string result = "";
            try
            {
                result = Program.objHrmsUI.getScallerValue("SELECT \"FormatCode\" FROM OACT Where \"AcctCode\" = '" + strCode + "'").ToString();
            }
            catch
            { }
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
        private void delCode(string strCode)
        {
            if (AMInRuleCnt(strCode) > 0)
            {
                oApplication.MessageBox("This AM is already used in AR. Are you sure you want update it", 2, "Yes", "No", "Cancel");
                return;


            }
            else
            {
                string strDelCode = "DELETE FROM \"@B1_AM\" WHERE \"Code\" = '" + strCode + "'";
                Program.objHrmsUI.ExecQuery(strDelCode, "Deleteing AM");

            }
        }

        private void addUpdateCode()
        {

           
            string strCode = Convert.ToString(dtHeads.GetValue("Code", 0));
            string strName = Convert.ToString(dtHeads.GetValue("Name", 0));
            string FatherC = Convert.ToString(dtHeads.GetValue("FatherC", 0));
            string FatherN = Convert.ToString(dtHeads.GetValue("FatherN", 0));
            string AccCode = Convert.ToString(dtHeads.GetValue("AccCode", 0));
            string FAC = Convert.ToString(dtHeads.GetValue("FAC", 0));

            string Area = Convert.ToString(dtHeads.GetValue("Area", 0));
            string Valume = Convert.ToString(dtHeads.GetValue("Volume", 0));
            string Floors = Convert.ToString(dtHeads.GetValue("Floors", 0));
           
            string BasUnit = Convert.ToString(dtHeads.GetValue("BaseUnit", 0));

            Hashtable hsp = new Hashtable();

            hsp.Add("~p1", strCode);
            hsp.Add("~p2", strName);
            hsp.Add("~p3", FatherC);
            hsp.Add("~p4", Area);
            hsp.Add("~p5", Valume);
            hsp.Add("~p6", BasUnit);
            hsp.Add("~p7", AccCode);
           
            hsp.Add("~p8", Floors);
            hsp.Add("~p9", FAC);


            string strInsert = Program.objHrmsUI.getQryString("AM_AddUpdateCode_001", hsp);
            if (cmdOK.Caption == "Update")
            {
                if (AMInRuleCnt(strCode ) > 0)
                {
                    int conformation = oApplication.MessageBox("This AM is already used in AR. Are you sure you want update it", 2, "Yes", "No", "Cancel");
                    if (conformation != 1) return;

                }
                strInsert = Program.objHrmsUI.getQryString("AM_AddUpdateCode_002", hsp);

            }
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

        private int AMInRuleCnt(string AM)
        {
            int result = 0;
            string strQuery = "SELECT COUNT(*) FROM \"@B1_ARD\" WHERE \"U_CMC\" = '" + AM + "'";
            result = Convert.ToInt32( Program.objHrmsUI.getScallerValue(strQuery));

            return result;
        }



    

    }
}
