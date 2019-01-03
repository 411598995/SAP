using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace ACHR.Screen
{
    class frm_NC : HRMSBaseForm
    {

        public bool isForLoading = false;
        SAPbouiCOM.Matrix mtNote, mtDetail;
        SAPbouiCOM.ComboBox cbNote, cbValOf,cbCS,cbPRJ;
        SAPbouiCOM.EditText txCell, txAcct, txFV, txTxt,txHCD;
      
        SAPbouiCOM.Button btAddAcct, btAddFV, btAddTXT, btRemove;
        SAPbouiCOM.ChooseFromList cflAcct;
        int oldSelrow = 1;
        int oldSelCol = 1;
        int rowNum = 0;

        SAPbouiCOM.DataTable dtNote, dtDetail, dtHead;
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
            oForm.Settings.MatrixUID = "mtNote";
            oForm.Settings.Enabled = true;
            InitiallizeForm();


            oForm.PaneLevel = 1;
            
        }

        public override void etAfterCfl(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCfl(ref pVal, ref BubbleEvent);


            if (pVal.ItemUID == "txAcct")
            {
                int rowind = pVal.Row;
                SAPbouiCOM.IChooseFromListEvent oCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pVal;
                SAPbouiCOM.DataTable dtSel = oCFLEvento.SelectedObjects;
                if (dtSel == null) return;
                if (dtSel.Rows.Count > 0)
                {
                    for (int k = 0; k < dtSel.Rows.Count; k++)
                    {
                        string strCode = dtSel.GetValue("AcctCode", k).ToString();
                        string strName = dtSel.GetValue("FormatCode", k).ToString();

                        dtHead.SetValue("AcctCode", 0, strName);

                        addGLRecord("GL");
                        ini_controls();
                        fillDetail();

                    }
                    //bt.Item.Click();
                }

            }

        }
        public override void etAfterClick(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == mtNote.Item.UniqueID && pVal.Row > 0 && pVal.Row <= mtNote.RowCount)
            {
              
               // mtNote.SelectRow(pVal.Row, true, false);
                string selCode = Convert.ToString(dtNote.GetValue( pVal.ColUID, pVal.Row - 1));
                string valOf = "01";
                dtHead.SetValue("SEL", 0, selCode.ToString());
                string[] rowcode = selCode.Split(',');
               

                if (rowcode.Length > 0)
                {
                    string[] colCodeN = rowcode[1].Split('_');

                    string colNum = colCodeN[1].ToString();
                    try
                    {
                        mtNote.CommonSetting.SetCellBackColor(oldSelrow, oldSelCol, mtNote.CommonSetting.GetCellBackColor(1, 1));
                 
                    }
                    catch { }
                    mtNote.CommonSetting.SetCellBackColor(pVal.Row, Convert.ToInt16(colNum) + 1, 65535);
                
                    oldSelrow = pVal.Row;
                    oldSelCol = Convert.ToInt16(colNum) + 1;
                    string strRow = " SELECT * FROM \"@AC_RPT_NR\" WHERE \"Code\" = '" + rowcode[0].ToString() + "'";
                    System.Data.DataTable dtRowSource = Program.objHrmsUI.getDataTable(strRow, "Cols");

                    if (dtRowSource != null && dtRowSource.Rows.Count > 0)
                    {
                        try
                        {
                            valOf = dtRowSource.Rows[0]["U_ValOf"].ToString();
                        }
                        catch { }
                        if (valOf == "") valOf = "01";
                     
                        cbValOf.Select(valOf, SAPbouiCOM.BoSearchKey.psk_ByValue);
                    
                    }
                }
                   



                fillDetail();
              //  fillDetail(selCode);
              
            }
            if (pVal.ItemUID == btAddAcct.Item.UniqueID)
            {
                addGLRecord("GL");
                ini_controls();
                fillDetail();
            }

            if (pVal.ItemUID == btAddFV.Item.UniqueID)
            {
                addGLRecord("HC");
                ini_controls();
                fillDetail();
            }

            if (pVal.ItemUID == btAddTXT.Item.UniqueID)
            {
                addGLRecord("TX");
                ini_controls();
                fillDetail();
            }


            if (pVal.ItemUID == btRemove.Item.UniqueID)
            {
                int SelRow = mtSelRow(mtDetail);
                if (SelRow > 0)
                {
                    string strCode = dtDetail.GetValue("Code", SelRow - 1).ToString();
                    removeGLRecord(strCode);
                }
                fillDetail();
            }

        
           
            
        }

        public override void etAfterCmbSelect(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCmbSelect(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == cbNote.Item.UniqueID)
            {
                string strCol = " SELECT * FROM \"@AC_RPT_NC\" WHERE \"U_Note\" = '" + cbNote.Selected.Value.ToString().Trim() + "'";
                System.Data.DataTable dtColSource = Program.objHrmsUI.getDataTable(strCol, "Cols");

                string strRow = " SELECT * FROM \"@AC_RPT_NR\" WHERE \"U_Note\" = '" + cbNote.Selected.Value.ToString().Trim() + "'";
                System.Data.DataTable dtRowSource = Program.objHrmsUI.getDataTable(strRow, "Cols");
               

                System.Data.DataTable dtSourc = new System.Data.DataTable();
                dtSourc.Columns.Add("Descr");

                foreach (System.Data.DataRow dr in dtColSource.Rows)
                {
                   System.Data.DataColumn ndc =   dtSourc.Columns.Add(dr["U_Title"].ToString());
                    ndc.ExtendedProperties.Add("Code",dr["Code"].ToString());
                
                }

                foreach (System.Data.DataRow dr in dtRowSource.Rows)
                {
                    System.Data.DataRow ndr = dtSourc.Rows.Add();
                    ndr[0] = dr["U_Title"];
                    int p = 0;
                    foreach (System.Data.DataColumn cdc in dtSourc.Columns)
                    {
                        if (p > 0)
                        {
                            ndr[p] = dr["Code"].ToString()  + "," + cdc.ExtendedProperties["Code"].ToString();
                        }
                        p++;
                    }

                }
              


                dtToMtProcess(dtSourc);

                for (int row = 0; row < mtNote.RowCount; row++)
                {
                    for (int col = 0; col < dtDetail.Rows.Count; col++)
                    {
                        mtNote.CommonSetting.SetCellBackColor(row, col, mtNote.CommonSetting.GetCellBackColor(1, 1));
                    }
                }
                oldSelCol = 1;
                oldSelCol = 1;
            }
            if (pVal.ItemUID == cbValOf.Item.UniqueID)
            {
                string selCode = Convert.ToString(dtHead.GetValue("SEL", 0));
                string[] rowcode = selCode.Split(',');

                string strUpdate = " UPDATE \"@AC_RPT_NR\" SET \"U_ValOf\"='" + cbValOf.Selected.Value.ToString() + "' WHERE \"Code\" = '" + rowcode[0].ToString() + "'";
                Program.objHrmsUI.ExecQuery(strUpdate, "Upsating");
            }
        }
        private void InitiallizeForm()
        {


            oForm.Freeze(true);

            initiallizing = true;
            dtNote = oForm.DataSources.DataTables.Item("dtNote");

            dtDetail = oForm.DataSources.DataTables.Item("dtDetail");

            dtHead = oForm.DataSources.DataTables.Item("dtHead");
            dtHead.Rows.Add(1);

            mtNote = (SAPbouiCOM.Matrix)oForm.Items.Item("mtNote").Specific;
            mtDetail = (SAPbouiCOM.Matrix)oForm.Items.Item("mtDetail").Specific;

            cbNote = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbNote").Specific;
            cbValOf = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbValOf").Specific;
            cbCS = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbCS").Specific;
            cbPRJ = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbPRJ").Specific;

            txCell = (SAPbouiCOM.EditText)oForm.Items.Item("txCell").Specific;
            txTxt = (SAPbouiCOM.EditText)oForm.Items.Item("txTxt").Specific;
            txHCD = (SAPbouiCOM.EditText)oForm.Items.Item("26").Specific;
            btAddAcct = (SAPbouiCOM.Button)oForm.Items.Item("btAddAcct").Specific;
            btAddFV = (SAPbouiCOM.Button)oForm.Items.Item("btAddFV").Specific;
            btAddTXT = (SAPbouiCOM.Button)oForm.Items.Item("btAddTXT").Specific;
            btRemove = (SAPbouiCOM.Button)oForm.Items.Item("btRemove").Specific;

            txFV = (SAPbouiCOM.EditText)oForm.Items.Item("txFV").Specific;

            fillCb();
            cbValOf.ValidValues.Add("01", "Closing");
            cbValOf.ValidValues.Add("02", "Change");
            cbValOf.ValidValues.Add("03", "Opening");

            cbNote.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);

            cflAcct = (SAPbouiCOM.ChooseFromList)oForm.ChooseFromLists.Item("cflAcct");

            SAPbouiCOM.Conditions oCons = cflAcct.GetConditions();
            SAPbouiCOM.Condition oCon = oCons.Add();
            oCon.Alias = "Postable";
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_START;
            oCon.CondVal = "Y";
            
            cflAcct.SetConditions(oCons);

            oForm.Freeze(false);

            initiallizing = false;


        }

        private void ini_controls()
        {
            dtHead.SetValue("AcctCode", 0, "");

            dtHead.SetValue("FV", 0, "0.00");
            dtHead.SetValue("TXT", 0, "");
        

        }





        private void fillCb()
        {

            System.Data.DataTable dtNotes = Program.objHrmsUI.getDataTable("Select * from \"@AC_RPT_NOTE\"", "Fill CB");
            foreach (System.Data.DataRow dr in dtNotes.Rows)
            {
                cbNote.ValidValues.Add(dr["Code"].ToString(), dr["Code"].ToString() + "-" + dr["U_Title"].ToString());
                    
            }

            cbPRJ.ValidValues.Add("-1" , "ALL");
            dtNotes = Program.objHrmsUI.getDataTable("Select * from \"OPRJ\"", "Fill CB");
            foreach (System.Data.DataRow dr in dtNotes.Rows)
            {
                cbPRJ.ValidValues.Add(dr["PrjCode"].ToString(), dr["PrjName"].ToString() );

            }
            cbPRJ.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
            cbCS.ValidValues.Add("-1", "ALL");
            dtNotes = Program.objHrmsUI.getDataTable("Select * from \"OPRC\" Where \"DimCode\" = '1' ", "Fill CB");
            foreach (System.Data.DataRow dr in dtNotes.Rows)
            {
                cbCS.ValidValues.Add(dr["PrcCode"].ToString(), dr["PrcName"].ToString());

            }

            cbCS.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
         
        }

        private void fillDetail()
        {
            dtDetail.Rows.Clear();
            string selCell = dtHead.GetValue("SEL", 0).ToString();
            if (selCell != "")
            {
                string[] CellAddr = selCell.Split(',');
                if (CellAddr.Length == 2)
                {

                    string rowCode = CellAddr[0].ToString();
                    string colCode = CellAddr[1].ToString();

                    string strSql = "Select * FROM  \"@AC_RPT_NGL\" WHERE \"U_Row\" ='" + rowCode + "' AND \"U_Col\"='" + colCode + "' ";
                    System.Data.DataTable dtSysDetail = Program.objHrmsUI.getDataTable(strSql, "Fill Detail");
                    int i = 0;
                    foreach (System.Data.DataRow dr in dtSysDetail.Rows)
                    {
                        dtDetail.Rows.Add(1);
                        dtDetail.SetValue("Id", i, i.ToString());
                        dtDetail.SetValue("SV", i, dr["U_GL"].ToString());
                        dtDetail.SetValue("ST", i, dr["U_ValType"].ToString());
                        dtDetail.SetValue("Code", i, dr["Code"].ToString());
                     if(dr["U_ValType"].ToString()=="HC")   dtDetail.SetValue("HCD", i, dr["U_HCDate"].ToString());
                     if (dr["U_ValType"].ToString() == "GL") dtDetail.SetValue("CS", i, dr["U_PC"].ToString());
                     if (dr["U_ValType"].ToString() == "GL") dtDetail.SetValue("PRJ", i, dr["U_PRJ"].ToString());
                       

                        i++;

                    }
                }
            }
            mtDetail.LoadFromDataSource();

        }

        private void removeGLRecord(string Code)
        {
            string strDelete = "DELETE FROM \"@AC_RPT_NGL\" WHERE \"Code\" = '" + Code + "';";
            int result = Program.objHrmsUI.ExecQuery(strDelete, "NewGL");
                   
        }
        private void addGLRecord(string ValueType)
        {
             string selCell = dtHead.GetValue("SEL", 0).ToString();
             if (selCell != "")
             {
                 string[] CellAddr = selCell.Split(',');
                 if (CellAddr.Length == 2)
                 {
                     string strCode = "000000001";
                     string strMaxCode = "SELECT MAX(CAST(\"Code\" AS integer)) AS \"nextId\" FROM \"@AC_RPT_NGL\";";
             
                     if (oCompany.DbServerType != SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                     {
                            strMaxCode = "SELECT MAX(convert(int , \"Code\" )) AS \"nextId\" FROM \"@AC_RPT_NGL\"";
                 
                     }
                     
                     System.Data.DataTable dtMax = Program.objHrmsUI.getDataTable(strMaxCode, "MaxCode");
                     if (dtMax != null && dtMax.Rows.Count > 0)
                     {
                         int MaxId = Convert.ToInt32(dtMax.Rows[0]["nextId"]) + 1;
                         strCode = MaxId.ToString().PadLeft(9, '0');

                     }
                     string rowCode = CellAddr[0].ToString();
                     string colCode = CellAddr[1].ToString();
                     string pvalue = dtHead.GetValue("AcctCode", 0).ToString();
                     string CS = cbCS.Selected.Value.ToString().Trim();
                     string PRJ = cbPRJ.Selected.Value.ToString().Trim();
                     if (CS == "-1") CS = "";
                     if (PRJ == "-1") PRJ = "";
                     if (ValueType == "HC")
                     {
                         pvalue = dtHead.GetValue("FV", 0).ToString();
                     }

                     if (ValueType == "TX")
                     {
                         pvalue = dtHead.GetValue("TXT", 0).ToString();
                     }


                     if (ValueType == "GL" && pvalue == "")
                     {

                         oApplication.SetStatusBarMessage("Select a GL");
                         return;
                     }
                     if (ValueType == "GL" && pvalue == "")
                     {

                         oApplication.SetStatusBarMessage("Select a GL");
                         return;
                     }
                     if (ValueType == "HC" &&  (Convert.ToDouble( pvalue) ==0 || txHCD.Value==""))
                     {

                         oApplication.SetStatusBarMessage("Enter fix amount and amount date");
                         return;
                     }
                     if (ValueType == "TX" && pvalue=="")
                     {

                         oApplication.SetStatusBarMessage("Text Required");
                         return;
                     }


                     string strSql = "INSERT INTO  \"@AC_RPT_NGL\" (  \"Code\", \"Name\", \"U_Row\", \"U_Col\", \"U_GL\", \"U_ValType\",\"U_PC\",\"U_PRJ\" ) VALUES('" + strCode + "','" + strCode + "','" + rowCode + "','" + colCode + "','" + pvalue + "','" + ValueType + "','" + CS + "','" + PRJ + "') ";

                     if (ValueType == "HC")
                     {
                         DateTime dt = Convert.ToDateTime(dtHead.GetValue("HCD", 0));
                         strSql = "INSERT INTO  \"@AC_RPT_NGL\" (  \"Code\", \"Name\", \"U_Row\", \"U_Col\", \"U_GL\", \"U_ValType\",\"U_HCDate\" ) VALUES('" + strCode + "','" + strCode + "','" + rowCode + "','" + colCode + "','" + pvalue + "','" + ValueType + "','" + dt.ToString("yyyyMMdd") + "') ";

                     }
                     if (ValueType == "TX")
                     {
                         strSql = "INSERT INTO  \"@AC_RPT_NGL\" (  \"Code\", \"Name\", \"U_Row\", \"U_Col\", \"U_GL\", \"U_ValType\" ) VALUES('" + strCode + "','" + strCode + "','" + rowCode + "','" + colCode + "','" + pvalue + "','" + ValueType + "') ";

                     }
                     int result = Program.objHrmsUI.ExecQuery(strSql, "NewGL");
                    

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

        private void dtToMtProcess(System.Data.DataTable dtReportTwo)
        {


            SAPbouiCOM.Matrix mt = mtNote;
            System.Data.DataTable dt = dtReportTwo;

            int OldColCnt = mt.Columns.Count;
            dtNote.Rows.Clear();
            mt.LoadFromDataSource();
            oForm.Freeze(true);
            for (int i = 0; i < OldColCnt; i++)
            {
                mt.Columns.Remove(0);
                dtNote.Columns.Remove(0);
            }

            // add new colum with all dt Cols
            string colId = "col0";
            SAPbouiCOM.DataColumn sboDc = dtNote.Columns.Add(colId,SAPbouiCOM.BoFieldsType .ft_Integer, 100);
            SAPbouiCOM.Column mCol = mt.Columns.Add(colId,SAPbouiCOM.BoFormItemTypes.it_EDIT);
            mCol.TitleObject.Caption = "#";
            mCol.DataBind.Bind(dtNote.UniqueID, colId);
            mCol.Width = 20;
            mCol.Editable = false;
            mCol.RightJustified = true;


            //  mCol.ColumnSetting.SumValue = "T";


            int j = 1;
            foreach (System.Data.DataColumn dc in dt.Columns)
            {
                colId = "col" + j.ToString();

                sboDc = dtNote.Columns.Add(colId, SAPbouiCOM.BoFieldsType.ft_AlphaNumeric, 100);



                mCol = mt.Columns.Add(colId, SAPbouiCOM.BoFormItemTypes.it_EDIT);
                mCol.TitleObject.Caption = dc.ColumnName;
                if (j == 1)
                {
                    mCol.Width = 150;
                }
                else
                {
                    mCol.Width = 80;
                }
                mCol.TitleObject.Sortable = true;
                mCol.Editable = false;
                mCol.DataBind.Bind(dtNote.UniqueID, colId);


                j++;




            }
             int row = 0;
            int col = 0;
            foreach (System.Data. DataRow dr in dt.Rows)
            {
                dtNote.Rows.Add(1);
                dtNote.SetValue("col0", row, (row + 1).ToString());
              
                col = 1;
                foreach (System.Data.DataColumn dc in dt.Columns)
                {
                    

                        dtNote.SetValue("col" + col.ToString(), row, dr[dc.ColumnName]);
                        col++;
                    
                  
                }
                row++;
            }

           

            int m = 1;
            //  mt.LoadFromDataSource();
            oForm.Freeze(false);
            //  mt.LoadFromDataSource();
           
            mt.LoadFromDataSource();

          


            

           


        }
      

      


    

    }
}
