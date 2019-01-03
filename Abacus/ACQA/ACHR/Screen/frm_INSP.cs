using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;

namespace ACHR.Screen
{
    class frm_INSP : HRMSBaseForm
    {

        public bool isForLoading = false;
        SAPbouiCOM.Matrix mtDocs, mtDocRows, mtRpt,mtSB;
        SAPbouiCOM.Button btAppr, btRej, btAtt;
        SAPbouiCOM.DataTable dtDoc, dtRow, dtRpt,dtHead,dtSB;
        SAPbouiCOM.EditText txAtt;
        int rowNum = 0;
        List<string> userWhs = new List<string>();
        List<string> userOT = new List<string>();

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

            SAPbouiCOM.Menus mnus = oApplication.Menus.Item("43557").SubMenus;
            foreach (SAPbouiCOM.MenuItem mnu in mnus)
            {
                string menuTitel = mnu.String;
                if (menuTitel == "Inspection Report")
                {
                    printMenuId = mnu.UID.ToString();

                }
            }

            // oForm.Items.Item("btUpd").Visible = false;



        }
        public override void etAfterCmbSelect(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCmbSelect(ref pVal, ref BubbleEvent);
           

        }
        public override void etAfterValidate(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterValidate(ref pVal, ref BubbleEvent);
            if(pVal.ItemUID == "12")
            {

                int selRow = mtSelRow(mtSB);
                if (selRow > 0)
                {
                    double Qty = Convert.ToDouble(dtSB.GetValue("Qty", selRow-1));

                    double ApprQty = Convert.ToDouble(dtHead.GetValue("UApprQty", 0));
                    if (ApprQty > Qty)
                    {
                        oApplication.MessageBox("Wrong Qty. Max qty is " + Qty);
                        ApprQty = Qty;
                        dtHead.SetValue("UApprQty", 0, ApprQty.ToString());

                    }
                    double RejQty = Qty - ApprQty;
                    dtHead.SetValue("URejQty", 0, RejQty.ToString());


                }

            }
        }

        public override void etAfterActClick(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterActClick(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == "btFinal")
            {
                int confirm = oApplication.MessageBox("Are you sure you want to post stock?", 2, "Yes", "No");
                if (confirm == 1)
                {

                    int postingStatus = -1;
                    int trsnsferResult = -1;
                    int selRow = mtSelRow(mtDocs);
                    if (selRow > 0)
                    {
                        string strCode = Convert.ToString(dtDoc.GetValue("OT", selRow - 1));
                        string dn = Convert.ToString(dtDoc.GetValue("DE", selRow - 1));
                        string docType = Convert.ToString(dtDoc.GetValue("DT", selRow - 1));
                        if (docType == "Offer for Inspection")
                        {
                            string strProdNum = "SELECT \"U_B1_QA_INSP_PN\" FROM \"OIGN\" WHERE \"DocEntry\" ='" + dn + "'";
                            System.Data.DataTable dtProdNum = Program.objHrmsUI.getDataTable(strProdNum, "ProdNum");
                            if (dtProdNum != null && dtProdNum.Rows.Count > 0)
                            {
                                if (dtProdNum.Rows[0]["U_B1_QA_INSP_PN"] != DBNull.Value && Convert.ToInt32(dtProdNum.Rows[0]["U_B1_QA_INSP_PN"]) > 0)
                                {
                                    postingStatus =  ReleaseInspaction(dn);

                                }
                            }
                        }
                        else
                        {
                            postingStatus = PostTransfer(strCode, dn);
                        }
                        string strDocCode = strCode + "-" + dn;
                        if (strCode.Trim() != "" && postingStatus != -1)
                        {

                            string strInsert = "UPDATE   \"@B1_QA_DOC\"  SET \"U_Date\" = '" + DateTime.Now.ToString("yyyyMMdd") + "' ,  \"U_Final\"='Y' ,  \"U_WTRentry\" = '" + postingStatus + "'   WHERE \"Code\" = '" + strDocCode + "'";
                            Program.objHrmsUI.ExecQuery(strInsert, "Finalllizing Inspaction");

                            


                        }
                    }


                   //
                    _iniForm();

                    getDocs();

                }
            }
            if (pVal.ItemUID == "btPrint")
            {
                SAPbouiCOM.Menus mnus = oApplication.Menus;

                if (mnus.Exists(printMenuId) && printMenuId!="")
                {
                    int selRow = mtSelRow(mtDocs);

                    string strCode = Convert.ToString(dtDoc.GetValue("OT", selRow - 1));
                    string dn = Convert.ToString(dtDoc.GetValue("DE", selRow - 1));


                    mnus.Item(printMenuId).Activate();

                    SAPbouiCOM.Form prmForm = oApplication.Forms.ActiveForm;
                    EditText objType = (SAPbouiCOM.EditText)prmForm.Items.Item("1000003").Specific;
                    EditText DocKey = (SAPbouiCOM.EditText)prmForm.Items.Item("1000009").Specific;
                    SAPbouiCOM.Button ok = (SAPbouiCOM.Button)prmForm.Items.Item("1").Specific;
                 //   SAPbouiCOM.Button cancel = (SAPbouiCOM.Button)prmForm.Items.Item("2").Specific;

                    objType.Value = strCode;
                    DocKey.Value = dn;
                    prmForm.ActiveItem = ok.Item.UniqueID;

                    objType.Item.Enabled = false;
                  //  DocKey.Item.Enabled = false;

                     ok.Item.Click();
                  //  prmForm.Select();
                 //   cancel.Item.Click();




                }
                else
                {
                    oApplication.MessageBox("Please upload \"Inspection Report\" in production report menu");
                }

            }

            if (pVal.ItemUID == "txAtt")
            {
                try
                {
                    //  txAtt.ClickPicker();
                  System.Diagnostics.  Process.Start(dtHead.GetValue("Att",0).ToString());
                }
                catch(Exception ex)
                {
                    string errmsg = ex.Message;
                }
            }
            if (pVal.ItemUID == btAtt.Item.UniqueID)
            {
                string strFileName = Program.objHrmsUI.addAttacment("");
                if (strFileName != "")
                {
                    dtHead.SetValue("Att", 0, strFileName);

                    int selRow = mtSelRow(mtDocs);
                    string strCode = Convert.ToString(dtDoc.GetValue("OT", selRow - 1));
                    string dn = Convert.ToString(dtDoc.GetValue("DE", selRow - 1));
                    string strDocCode = strCode + "-" + dn;


                    string strInsert = "UPDATE   \"@B1_QA_DOC\"  SET \"U_Att\" = '" + strFileName + "' WHERE \"Code\" = '" + strDocCode + "'";
                    Program.objHrmsUI.ExecQuery(strInsert, "Finalllizing Inspaction");


                }
            }
            if (pVal.ItemUID == "1000001")
            {
                string RC = Convert.ToString(dtHead.GetValue("RC", 0));
                string SBC = Convert.ToString(dtHead.GetValue("SBC", 0));

                string strCode = RC + "-" + SBC;

                saveReport(strCode);


            }
            if (pVal.ItemUID == mtDocs.Item.UniqueID && pVal.Row>0 && pVal.Row <= mtDocs.RowCount )
            {
                mtDocs.SelectRow(pVal.Row,true,false);
                 int selRow = pVal.Row;
                if (selRow > 0)
                {
                    string strCode = Convert.ToString(dtDoc.GetValue("OT", selRow - 1));
                    string dn = Convert.ToString(dtDoc.GetValue("DE", selRow - 1));
                    string tbl = Convert.ToString(dtDoc.GetValue("Tbl", selRow - 1));

                    if (strCode.Trim() != "")
                    {
                        string strDocCode = strCode + "-" + dn;

                        string strExist = "SELECT * from  \"@B1_QA_DOC\" where \"Code\" = '" + strDocCode + "'";
                        System.Data.DataTable dtExist = Program.objHrmsUI.getDataTable(strExist, "DocDetExist");
                        if (dtExist.Rows.Count == 0)
                        {

                            string strInsert = "INSERT INTO  \"@B1_QA_DOC\"  (\"Code\",\"Name\",\"U_ObjType\",\"U_DocEntry\",\"U_Date\")";
                            strInsert += " VALUES ('" + strDocCode + "','" + strDocCode + "','" + strCode + "','" + dn + "','" + DateTime.Now.ToString("yyyyMMdd") + "')";
                            Program.objHrmsUI.ExecQuery(strInsert, "Rpt Doc Entry into UDT");

                        }
                        else
                        {
                            dtHead.SetValue("Att", 0, dtExist.Rows[0]["U_Att"].ToString());
                        }
                        getRows(tbl, dn);

                      
                    }
                }
                else
                {
                    oApplication.MessageBox("Please select a document before inspaction");
                }
            }

            if (pVal.ItemUID == mtDocRows.Item.UniqueID && pVal.Row > 0 && pVal.Row <= mtDocRows.RowCount)
            {
                mtDocRows.SelectRow(pVal.Row, true, false);
                int selRow = pVal.Row;// mtSelRow(mtDocRows);
                if (selRow > 0)
                {
                    string strCode = Convert.ToString(dtRow.GetValue("ItemCode", selRow - 1));
                    string MM = Convert.ToString(dtRow.GetValue("MM", selRow - 1));
                    string OBT = Convert.ToString(dtRow.GetValue("OBT", selRow - 1));
                    string LN = Convert.ToString(dtRow.GetValue("LN", selRow - 1));
                    string ItemCode = Convert.ToString(dtRow.GetValue("ItemCode", selRow - 1));
                    string Qty = Convert.ToString(dtRow.GetValue("Quantity", selRow - 1));
                    int DocselRow = mtSelRow(mtDocs);



                    string DE = Convert.ToString(dtDoc.GetValue("DE", DocselRow - 1));
                    dtHead.SetValue("RC", 0, OBT + "-" + DE + "-" + LN);
                    dtHead.SetValue("MM", 0,MM);
                    dtHead.SetValue("OBT", 0, OBT);
                    dtHead.SetValue("LN", 0, LN);
                    dtHead.SetValue("DE", 0,DE);
                    dtHead.SetValue("Qty", 0, Qty);

                    dtHead.SetValue("ItemCode", 0, ItemCode);


                    if (strCode.Trim() != "")
                    {


                        getAttributes(strCode);
                        fillSerilaOrBatch(Qty, DE, LN, OBT, MM);

                       
                    }
                }
                else
                {
                    oApplication.MessageBox("Please select an item before inspaction");
                }
              
            }

            if (pVal.ItemUID == mtSB.Item.UniqueID && pVal.Row > 0 && pVal.Row <= mtSB.RowCount )
            {
                mtSB.SelectRow(pVal.Row, true, false);
                int selRow = pVal.Row;// mtSelRow(mtSB);
                if (selRow > 0)
                {
                    string RC = Convert.ToString(dtHead.GetValue("RC", 0));
                    string MM = Convert.ToString(dtHead.GetValue("MM", 0));
                    string OBT = Convert.ToString(dtHead.GetValue("OBT", 0));
                    string LN = Convert.ToString(dtHead.GetValue("LN", 0));
                    string DE = Convert.ToString(dtHead.GetValue("DE", 0));
                    string ItemCode = Convert.ToString(dtHead.GetValue("ItemCode", 0));
                    string Qty = Convert.ToString(dtHead.GetValue("Qty", 0));



                    string strCode = Convert.ToString(dtSB.GetValue("Code", selRow - 1));
                    string strQty = Convert.ToString(dtSB.GetValue("Qty", selRow - 1));

                    dtHead.SetValue("SBC", 0, strCode);
                    getReport(RC + "-" + strCode, OBT, DE, LN, MM, strQty, ItemCode, strCode);
                    

                }

            }




        }


       
        private void InitiallizeForm()
        {


            oForm.Freeze(true);

            initiallizing = true;

            dtDoc = oForm.DataSources.DataTables.Item("dtDoc");
            dtRow = oForm.DataSources.DataTables.Item("dtRow");
            dtRpt = oForm.DataSources.DataTables.Item("dtRpt");
            dtHead = oForm.DataSources.DataTables.Item("dtHead");
            dtSB = oForm.DataSources.DataTables.Item("dtSB");

            dtHead.Rows.Add(1);
            mtDocs = (SAPbouiCOM.Matrix)oForm.Items.Item("mtDocs").Specific;
            mtDocRows = (SAPbouiCOM.Matrix)oForm.Items.Item("mtDocRows").Specific;
            mtRpt = (SAPbouiCOM.Matrix)oForm.Items.Item("mtRpt").Specific;
            mtSB = (SAPbouiCOM.Matrix)oForm.Items.Item("mtSB").Specific;
            btAtt = (SAPbouiCOM.Button) oForm.Items.Item("btAtt").Specific;

            txAtt = (SAPbouiCOM.EditText)oForm.Items.Item("txAtt").Specific;
            txAtt.Item.Enabled = false;
            getUserWHSAndOT();

            // oForm.Items.Item("btUpd").Enabled = false;
            oForm.Freeze(false);

            getDocs();
            initiallizing = false;
         
           

        }
        private void _iniForm()
        {
            dtHead.Rows.Clear();
            dtHead.Rows.Add(1);
            dtRow.Rows.Clear();
            dtSB.Rows.Clear();
            dtRpt.Rows.Clear();
            mtDocRows.LoadFromDataSource();
            mtSB.LoadFromDataSource();
            mtRpt.LoadFromDataSource();
        }

      
        private void ini_controls()
        {


        }
        private void getUserWHSAndOT()
        {
            string userName = oCompany.UserName;
            string strSqlOT = "Select \"U_ObjType\" FROM \"@B1_QA_OUSR_OT\" WHERE \"U_UsrCode\" = '" + userName + "'";
            System.Data.DataTable dtUOT = Program.objHrmsUI.getDataTable(strSqlOT, "User OT");
            foreach (System.Data.DataRow dr in dtUOT.Rows)
            {
                userOT.Add(dr["U_ObjType"].ToString());
            }

             strSqlOT = "Select \"U_WHS\" FROM \"@B1_QA_OUSR_WHS\" WHERE \"U_UsrCode\" = '" + userName + "'";
            dtUOT = Program.objHrmsUI.getDataTable(strSqlOT, "User OT");
            foreach (System.Data.DataRow dr in dtUOT.Rows)
            {
                userWhs.Add(dr["U_WHS"].ToString());
            }


        }
        private void getReport(string strCode, string OBT, string DE, string LN, string MM, string Qty, string ItemCode, string selCode)
        {

            string strExist = "SELECT * FROM \"@B1_QA_RPT\" WHERE \"Code\" = '" + strCode + "'";
            System.Data.DataTable dtExist = Program.objHrmsUI.getDataTable(strExist, "codeExist");
            if (dtExist != null && dtExist.Rows.Count > 0)
            {
                double apprQty = Convert.ToDouble(dtExist.Rows[0]["U_ApprQty"]);
                double rejQty = Convert.ToDouble(dtExist.Rows[0]["U_RejQty"]);

                dtHead.SetValue("UApprQty", 0, apprQty.ToString());
                dtHead.SetValue("URejQty", 0, rejQty.ToString());
            }
            else
            {
                dtHead.SetValue("UApprQty", 0, Qty.ToString());
                dtHead.SetValue("URejQty", 0, "0");
                addDefaultReportHeader(strCode, OBT, DE, LN, MM, Qty, ItemCode, selCode);
            }

            for(int i=0;i<dtRpt.Columns.Count;i++)
            {
                string colUid = dtRpt.Columns.Item(i).Name.ToString();
                if (colUid != "col_00")
                {
                    string attrCode = colUid.Split('_')[1].ToString();

                    string strValExist = "SELECT * FROM \"@B1_QA_RPT_DET\" WHERE \"Code\" = '" + strCode  + "-" + attrCode + "'";
                    System.Data.DataTable dtValExist = Program.objHrmsUI.getDataTable(strValExist, "codeExist");
                    if (dtValExist != null && dtValExist.Rows.Count > 0)
                    {
                        try
                        {
                            dtRpt.SetValue(colUid, 1, dtValExist.Rows[0]["U_SelVal"].ToString());
                        }
                        catch { }
                    }
                    else
                    {
                        addDefaultAttrVal(strCode,attrCode);
                    }
                }
            }
            mtRpt.LoadFromDataSource();


        }

        private void addDefaultReportHeader(string strCode, string OBT, string DE, string LN, string MM, string Qty, string ItemCode, string selCode)
        {

            string Batch = "";
            string Serial = "";
            if (MM == "S") Serial = selCode;
            if (MM == "B") Batch = selCode;

            string strInsert = "INSERT INTO  \"@B1_QA_RPT\" (\"Code\", \"Name\", \"U_DocType\", \"U_DocEntry\", \"U_LineNum\",\"U_Batch\", \"U_UserId\", \"U_Date\", \"U_Serial\", \"U_Qty\", \"U_ApprQty\", \"U_RejQty\", \"U_ItemCode\")";
            strInsert += "Values ('" + strCode + "','" + strCode + "','" + OBT + "','" + DE + "','" + LN + "','" + Batch + "','" + oCompany.UserName + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Serial + "','" + Qty + "','" + Qty + "','0','" + ItemCode + "')";

            Program.objHrmsUI.ExecQuery(strInsert, "add new report");



        }

        private void addDefaultAttrVal(string strCode, string attrCode)
        {


            string detCode = strCode + "-" + attrCode;
            string strInsert = "INSERT INTO  \"@B1_QA_RPT_DET\" (\"Code\", \"Name\", \"U_HeadCode\", \"U_AttrCode\", \"U_SelVal\")";
            strInsert += "Values ('" + detCode + "','" + detCode + "','" + strCode + "','" + attrCode + "','')";
            Program.objHrmsUI.ExecQuery(strInsert, "add new report");

        }

        private void getDocDetail(string docCode)
        {
            string strSelect = "SELECT * FROM \"@B1_QA_DOC\" WHERE \"Code\" = '" + docCode + "'";
            System.Data.DataTable dtDocDetail = Program.objHrmsUI.getDataTable(strSelect, "DocHeaderDetail");
            foreach (System.Data.DataRow dr in dtDocDetail.Rows)
            {
                dtHead.SetValue("Att", 0, dr["U_Att"].ToString());
            }
            

        }
        private void getRows(string strTbl, string strDE)
        {
            dtRow.Rows.Clear();

            string strRows = "  SELECT t2.\"ManBtchNum\" AS  \"BATCH\" , t2.\"ManSerNum\" AS  \"SERIAL\"  , t1.\"ObjType\", t0.\"ItemCode\",t0.\"LineNum\",t0.\"Dscription\",t0.\"FreeTxt\" , t0.\"Quantity\" , t0.\"WhsCode\" from \"" + strTbl + "1\" t0 inner join \"O" + strTbl + "\" t1 on t0.\"DocEntry\" = t1.\"DocEntry\" ";
            strRows += " INNER JOIN \"OITM\" t2 on t2.\"ItemCode\" = t0.\"ItemCode\"";
            strRows += " where t0.\"DocEntry\" = '" + strDE  + "' ";

            System.Data.DataTable dtRows = Program.objHrmsUI.getDataTable(strRows, "getting docs");
            int i = 0;
            foreach (System.Data.DataRow dr in dtRows.Rows)
            {
                i++;
                string MM = "R";
                if (dr["BATCH"].ToString() == "Y") MM = "B";
                if (dr["SERIAL"].ToString() == "Y") MM = "S";
                dtRow.Rows.Add(1);
                dtRow.SetValue("Id", i - 1, i.ToString());
                dtRow.SetValue("ItemCode", i - 1, dr["ItemCode"].ToString());
                dtRow.SetValue("ItemName", i - 1, dr["Dscription"].ToString());
                dtRow.SetValue("Quantity", i - 1, dr["Quantity"].ToString());
                dtRow.SetValue("Remarks", i - 1, dr["FreeTxt"].ToString());
                dtRow.SetValue("MM", i - 1, MM);
                dtRow.SetValue("OBT", i - 1, dr["ObjType"].ToString());
                dtRow.SetValue("LN", i - 1, dr["LineNum"].ToString());
                dtRow.SetValue("WHS", i - 1, dr["WhsCode"].ToString());


                double ApprQty = 0;
                double RejQty = 0;
                string RC = dr["ObjType"].ToString() + "-" + strDE + "-" + dr["LineNum"].ToString();
                getRptQtyForRows(RC, out ApprQty, out RejQty);

                dtRow.SetValue("ApprQty", i - 1, ApprQty.ToString());
                dtRow.SetValue("RejQty", i - 1, RejQty.ToString());

                
            }
            mtDocRows.LoadFromDataSource();
        }

        private void saveReport(string strCode)
        {


            string ApprQty = dtHead.GetValue("UApprQty", 0).ToString();
            string RejQty = dtHead.GetValue("URejQty", 0).ToString();
            string strUpdateHeader = "UPDATE \"@B1_QA_RPT\" SET  \"U_ApprQty\" = '" + ApprQty +   "' ,  \"U_RejQty\" = '" + RejQty + "' WHERE  \"Code\" = '" + strCode + "'";
            Program.objHrmsUI.ExecQuery(strUpdateHeader, "Updating Report");
            mtRpt.FlushToDataSource();

            string colCnt = dtRpt.Columns.Count.ToString();
            string rowCnt = dtRpt.Rows.Count.ToString();
            for (int i = 0; i < dtRpt.Columns.Count; i++)
            {
                string colUid = dtRpt.Columns.Item(i).Name.ToString();
                if (colUid != "col_00")
                {
                    string attrCode = colUid.Split('_')[1].ToString();
                    string strValue = dtRpt.GetValue(colUid, 1).ToString();

                    string strValExist = "UPDATE  \"@B1_QA_RPT_DET\" SET \"U_SelVal\" = '" + strValue + "'   WHERE \"Code\" = '" + strCode + "-" + attrCode + "'";
                    Program.objHrmsUI.ExecQuery(strValExist,"Updating Report");
                   
                }
            }
            refRowQtys();
        }

        private void fillSerilaOrBatch(string qty, string DE, string LN, string OT,string MM)
        {
            dtSB.Rows.Clear();

            if (MM == "R")
            {
                dtSB.Rows.Add(1);
                dtSB.SetValue("Id", 0, "1");
                dtSB.SetValue("Code", 0, "Row");
                dtSB.SetValue("Qty", 0, qty);

            }

            if (MM == "B")
            {
                string strSQL = "SELECT \"BatchNum\",\"Quantity\" from \"IBT1\"  WHERE \"Direction\" = '0' AND  \"BaseType\" = '" + OT + "' and \"BaseEntry\" = '" + DE + "' and \"BaseLinNum\" = '" + LN + "'";

                System.Data.DataTable dtRows = Program.objHrmsUI.getDataTable(strSQL, "getting Batches");
                int i = 0;
                foreach (System.Data.DataRow dr in dtRows.Rows)
                {


                    dtSB.Rows.Add(1);
                    dtSB.SetValue("Id", i, (i + 1).ToString());
                    dtSB.SetValue("Code", i, dr["BatchNum"].ToString());
                    dtSB.SetValue("Qty", i, dr["Quantity"].ToString());

                    i++;
                }

            }

            if (MM == "S")
            {
                string strSQL = "select t1.\"IntrSerial\" ,\"Quantity\" from \"SRI1\" t0 inner join \"OSRI\" t1 on t0.\"SysSerial\" = t1.\"SysSerial\" And t0.\"ItemCode\" = t1.\"ItemCode\"  WHERE t0.\"BaseType\" = '" + OT + "' and t0.\"BaseEntry\" = '" + DE + "' and t0.\"BaseLinNum\" = '" + LN + "'";

                System.Data.DataTable dtRows = Program.objHrmsUI.getDataTable(strSQL, "getting Batches");
                int i = 0;
                foreach (System.Data.DataRow dr in dtRows.Rows)
                {
                   

                    dtSB.Rows.Add(1);
                    dtSB.SetValue("Id", i, (i+1).ToString());
                    dtSB.SetValue("Code", i, dr["IntrSerial"].ToString());
                    dtSB.SetValue("Qty", i, 1);

                    i++;
                }

            }
            mtSB.LoadFromDataSource();


        }
        private void getAttributes(string itemCode)
        {
            oForm.Freeze(true);
            try
            {
                dtRpt.Rows.Clear();
                mtRpt.LoadFromDataSource();

                int colcnt = dtRpt.Columns.Count;
                for (int i = 1; i < colcnt; i++)
                {
                    string colid = dtRpt.Columns.Item(1).Name;
                    dtRpt.Columns.Remove(colid);
                    mtRpt.Columns.Remove("m" + colid);
                }

                mtRpt.LoadFromDataSource();

               


                int k = 1;

                string strRows = "   SELECT t0.\"Code\", t0.\"Name\", t0.\"U_IGCode\", t1.\"Code\" AS \"AttrCode\", t1.\"Name\" AS \"AttrName\", t1.\"U_Descr\" AS \"AttrDescr\", t1.\"U_Type\", t1.\"U_DefVal\", t1.\"U_LOVT\", t1.\"U_LOVSQL\", t1.\"U_DefValRem\", \"U_VisOrder\", \"U_Active\" FROM \"@B1_QA_CODES_ATTR\" t0 INNER JOIN \"@B1_QA_ATTR\" t1 ON t0.\"Name\" = t1.\"Code\" WHERE \"U_IGCode\" = '" + itemCode + "'  ";
                System.Data.DataTable dtRows = Program.objHrmsUI.getDataTable(strRows, "getting docs");
                foreach (System.Data.DataRow dr in dtRows.Rows)
                {


                    SAPbouiCOM.DataColumn dccol1;

                    string type = dr["U_Type"].ToString();
                    switch (type)
                    {
                        case "01":
                           
                            dccol1 = dtRpt.Columns.Add("col_" + dr["AttrCode"].ToString() , BoFieldsType.ft_AlphaNumeric, 250);

                            break;
                        case "02":
                            dccol1 = dtRpt.Columns.Add("col_" + dr["AttrCode"].ToString(), BoFieldsType.ft_Float, 250);
                            break;
                        case "03":
                            dccol1 = dtRpt.Columns.Add("col_" + dr["AttrCode"].ToString(), BoFieldsType.ft_Date);
                            break;

                    }


                    string Lovtype = dr["U_LOVT"].ToString();

                    SAPbouiCOM.Column col1;
                    switch (Lovtype)
                    {

                        case "01":
                            col1 = mtRpt.Columns.Add("mcol_" + dr["AttrCode"].ToString(), BoFormItemTypes.it_EDIT);
                            col1.TitleObject.Caption = dr["AttrDescr"].ToString();
                            col1.Editable = true;
                            col1.Width = 20;
                            col1.DataBind.Bind("dtRpt", "col_" + dr["AttrCode"].ToString());

                            break;

                        case "02":
                            col1 = mtRpt.Columns.Add("mcol_" + dr["AttrCode"].ToString(), BoFormItemTypes.it_COMBO_BOX);
                            col1.TitleObject.Caption = dr["AttrDescr"].ToString();
                            col1.Editable = true;
                            col1.Width = 20;
                            col1.DataBind.Bind("dtRpt", "col_" + dr["AttrCode"].ToString());
                            col1.DisplayDesc = true;

                            fillLOVinCol(col1, dr["AttrCode"].ToString(),"");
                            break;
                        case "03":
                            col1 = mtRpt.Columns.Add("mcol_" + dr["AttrCode"].ToString(), BoFormItemTypes.it_COMBO_BOX);
                            col1.TitleObject.Caption = dr["AttrDescr"].ToString();
                            col1.Editable = true;
                            col1.Width = 20;
                            col1.DisplayDesc = true;

                            col1.DataBind.Bind("dtRpt", "col_" + dr["AttrCode"].ToString());
                            fillLOVinCol(col1, dr["AttrCode"].ToString(), dr["U_LOVSQL"].ToString());
                           

                            break;

                    }



                    k++;

                }

                for(int i=1;i<dtRpt.Columns.Count;i++ )
                {
                    SAPbouiCOM.Column col = mtRpt.Columns.Item(i);
                  
                    string colUid = dtRpt.Columns.Item(i).Name.ToString();
                    if (colUid != "col_00")
                    {
                        string attrCode = colUid.Split('_')[1].ToString();
                        col.DataBind.Bind("dtRpt", "col_" + attrCode);

                    }
                    else
                    {
                        col.DataBind.Bind("dtRpt", "col_00");

                    }


                }

                dtRpt.Rows.Add(2);

                mtRpt.LoadFromDataSource();
                mtRpt.CommonSetting.SetRowEditable(1, false);
            }
            finally
            {
                oForm.Freeze(false);
            }
            //mtRpt.Item.Width = 500;

        }

        private void fillLOVinCol(SAPbouiCOM.Column col, string attrId, string sql)
        {

            string strSelect = "SELECT \"U_Val\" , \"U_Descr\" FROM \"@B1_QA_ATTR_LOV\" WHERE \"U_AttrCode\" = '" + attrId + "' ORDER BY \"U_VisOrder\"";
            if (sql != "") strSelect = sql;
            System.Data.DataTable dtCodes = Program.objHrmsUI.getDataTable(strSelect, "getting codes LOV");
            int i = 0;
            foreach (System.Data.DataRow dr in dtCodes.Rows)
            {
                col.ValidValues.Add(dr[0].ToString(), dr[1].ToString());

            }

        }
        private void getDocs()
        {
            string strWhs = "";

            foreach (string whs in userWhs)
            {
                if (strWhs == "")
                {
                    strWhs = "'" + whs + "'";
                }
                else
                {
                    strWhs += " ,'" + whs + "'";
                }
            }

            string strOT = "";
            foreach (string ot in userOT)
            {
                if (strOT == "")
                {
                    strOT = "'" + ot + "'";
                }
                else
                {
                    strOT += " ,'" + ot + "'";
                }
            }

            if (strOT == "" || strWhs == "") return;
            string strDocs = "  SELECT t0.\"Comments\" , t0.\"TransType\", t0.BASE_REF,t0.\"CreatedBy\", t1.\"Name\" , t1.\"U_TblScm\" FROM OINM t0 INNER JOIN \"@B1_QA_OBJS\" t1 ON t0.\"TransType\" = CAST(t1.\"Code\" AS integer)  ";
            strDocs += " WHERE t0.\"InQty\" > 0 AND t0.\"Warehouse\" IN (" + strWhs + ") AND  t0.\"TransType\" IN (" + strOT + ") " +
                " AND  NOT EXISTS (SELECT tObj.\"U_ObjType\" FROM \"@B1_QA_DOC\" tObj WHERE tObj.\"U_Final\" = 'Y' AND  tObj.\"U_ObjType\" = t0.\"TransType\" AND tObj.\"U_DocEntry\" =  t0.\"CreatedBy\" ) " +
                "GROUP BY  t0.\"TransType\", t0.BASE_REF,t0.\"CreatedBy\", t1.\"Name\", t1.\"U_TblScm\" ,t0.\"Comments\" ";

            System.Data.DataTable dtdocs = Program.objHrmsUI.getDataTable(strDocs, "getting docs");
            int i = 0;
            dtDoc.Rows.Clear();
            foreach (System.Data.DataRow dr in dtdocs.Rows)
            {
                string DocType = dr["Name"].ToString();
                string transType = dr["TransType"].ToString();
                if (transType == "59")
                {
                    string DE = dr["CreatedBy"].ToString();
                    string strProdNum = "SELECT \"U_B1_QA_INSP_PN\" FROM \"OIGN\" WHERE \"DocEntry\" ='" + DE + "'";
                    System.Data.DataTable dtProdNum = Program.objHrmsUI.getDataTable(strProdNum, "ProdNum");
                    if (dtProdNum != null && dtProdNum.Rows.Count > 0)
                    {
                        if(dtProdNum.Rows[0]["U_B1_QA_INSP_PN"] !=DBNull.Value &&  Convert.ToInt32(dtProdNum.Rows[0]["U_B1_QA_INSP_PN"])>0)
                        {

                            DocType = "Offer for Inspection";
                        }
                    }
                }
                i++;
                dtDoc.Rows.Add(1);
                dtDoc.SetValue("Id", i - 1, i.ToString());
                dtDoc.SetValue("DE", i - 1, dr["CreatedBy"].ToString());
                dtDoc.SetValue("DN", i - 1, dr["BASE_REF"].ToString());
                dtDoc.SetValue("DT", i - 1,DocType );
                dtDoc.SetValue("Tbl", i - 1, dr["U_TblScm"].ToString());
                dtDoc.SetValue("OT", i - 1, dr["TransType"].ToString());

            }
            mtDocs.LoadFromDataSource();


        }

        private int ReleaseInspaction(string GREntry )
        {
            int result = -1;

            int rowCnt = dtRow.Rows.Count;
            for (int i = 0; i < rowCnt; i++)
            {
                double ApprQty = Convert.ToDouble(dtRow.GetValue("ApprQty", i));
                double RejQty = Convert.ToDouble(dtRow.GetValue("RejQty", i));
                double Quantity = Convert.ToDouble(dtRow.GetValue("Quantity", i));
                if (Quantity != ApprQty + RejQty)
                {
                    oApplication.MessageBox("Inspaction not completed. Please save Accepted/Rejected Qty for all rows");
                    return -1;
                }
            }

            try

            {
               

                SAPbobsCOM.Documents InspactionGR = (SAPbobsCOM.Documents)oCompany.GetBusinessObject( SAPbobsCOM. BoObjectTypes.oInventoryGenEntry);
                InspactionGR.GetByKey( Convert.ToInt32( GREntry) );
                SAPbobsCOM.Documents InspactionGI = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM. BoObjectTypes.oInventoryGenExit);
                string worNum = InspactionGR.UserFields.Fields.Item("U_B1_QA_INSP_PN").Value.ToString();
                InspactionGI.DocDate = DateTime.Now.Date;
                InspactionGI.UserFields.Fields.Item("U_B1_QA_INSP_PN").Value = InspactionGR.UserFields.Fields.Item("U_B1_QA_INSP_PN").Value;

                for (int i = 0; i < InspactionGR.Lines.Count; i++)
                {

                    InspactionGR.Lines.SetCurrentLine(i);
                    if (InspactionGR.Lines.ItemCode != "")
                    {
                        InspactionGI.Lines.ItemCode = InspactionGR.Lines.ItemCode;
                        InspactionGI.Lines.WarehouseCode = InspactionGR.Lines.WarehouseCode;
                        InspactionGI.Lines.AccountCode = InspactionGR.Lines.AccountCode;
                        InspactionGI.Lines.Quantity = InspactionGR.Lines.Quantity;
                        for (int j = 0; j < InspactionGR.Lines.BatchNumbers.Count; j++)
                        {
                            InspactionGR.Lines.BatchNumbers.SetCurrentLine(j);
                            string batchNum = InspactionGR.Lines.BatchNumbers.BatchNumber;
                            if (batchNum != "")
                            {
                                InspactionGI.Lines.BatchNumbers.BatchNumber = batchNum;
                                InspactionGI.Lines.BatchNumbers.Quantity = InspactionGR.Lines.BatchNumbers.Quantity;
                                InspactionGI.Lines.BatchNumbers.Add();

                            }
                        }
                        for (int j = 0; j < InspactionGR.Lines.SerialNumbers.Count; j++)
                        {
                            InspactionGR.Lines.SerialNumbers.SetCurrentLine(j);
                            string serialNum = InspactionGR.Lines.SerialNumbers.InternalSerialNumber;
                            if (serialNum != "")
                            {
                                InspactionGI.Lines.SerialNumbers.InternalSerialNumber = serialNum;
                                InspactionGI.Lines.SerialNumbers.Quantity = InspactionGR.Lines.SerialNumbers.Quantity;
                                InspactionGI.Lines.SerialNumbers.Add();

                            }
                        }

                        InspactionGI.Lines.Add();
                    }
                }

                if (InspactionGI.Add() != 0)
                {
                    int erroCode = 0;
                    string errDescr = "";
                    Program.objHrmsUI.oCompany.GetLastError(out erroCode, out errDescr);
                    oApplication.StatusBar.SetText("Failed to add good issue to revert good receipt  : " + errDescr);
                    result = -1;
                }
                else
                {
                    string outStr = Convert.ToString(Program.objHrmsUI.oCompany.GetNewObjectKey());
                    string updateCall = "UPDATE OWOR set \"U_B1_QA_INSGI\"='" + outStr + "' WHERE \"DocEntry\" = '" + worNum.ToString() + "'";
                    result = Program.objHrmsUI.ExecQuery(updateCall, "Update Production Order");

                    if (result != -1) result = Convert.ToInt32(outStr); 
                 
                }

            }
            catch (Exception ex)
            {
                oApplication.MessageBox(ex.Message);
                result = -1;
            }

            return result;
        }


        private int PostTransfer(string ObjType, string docKey)
        {
            int result = -1;

            int rowCnt = dtRow.Rows.Count;

            for (int i = 0; i < rowCnt; i++)
            {
                double ApprQty = Convert.ToDouble(dtRow.GetValue("ApprQty", i));
                double RejQty = Convert.ToDouble(dtRow.GetValue("RejQty", i));
                double Quantity = Convert.ToDouble(dtRow.GetValue("Quantity", i));
                if(Quantity!=ApprQty+RejQty)
                {
                    oApplication.MessageBox("Inspaction not completed. Please save Accepted/Rejected Qty for all rows");
                    return -1;
                }
            }
            try

            {

               

                SAPbobsCOM.StockTransfer Inspactiontransfer = (SAPbobsCOM.StockTransfer)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer);
                Inspactiontransfer.DocDate = DateTime.Now.Date;
                Inspactiontransfer.UserFields.Fields.Item("U_B1_QA_INSP_OT").Value = ObjType;
                Inspactiontransfer.UserFields.Fields.Item("U_B1_QA_INSP_DE").Value = docKey;




                for (int i = 0; i < rowCnt; i++)
                {
                    string FromWhs = Convert.ToString(dtRow.GetValue("WHS", i));
                    string strSel = "SELECT * from \"@B1_QA_ATTR_INSWHS\" WHERE \"U_OWHS\" = '" + FromWhs + "'";

                    System.Data.DataTable dtWhs = Program.objHrmsUI.getDataTable(strSel, "getting whs");
                    if (dtWhs == null || dtWhs.Rows.Count == 0) return -1;

                    mtDocRows.SelectRow(i + 1, true, false);
                    string strCode = Convert.ToString(dtRow.GetValue("ItemCode", i));
                    string MM = Convert.ToString(dtRow.GetValue("MM", i));
                    string OBT = Convert.ToString(dtRow.GetValue("OBT", i));
                    string LN = Convert.ToString(dtRow.GetValue("LN", i));
                    string Qty = Convert.ToString(dtRow.GetValue("Quantity", i));


                    fillSerilaOrBatch(Qty,docKey,LN,OBT,MM);

                    string toWHSAppr = dtWhs.Rows[0]["U_OWHS_APPR"].ToString();
                    string toWHSRej = dtWhs.Rows[0]["U_OWHS_REJ"].ToString();

                    double ApprQty = Convert.ToDouble(dtRow.GetValue("ApprQty", i));
                    double RejQty = Convert.ToDouble(dtRow.GetValue("RejQty", i));
                    string itemCode = Convert.ToString(dtRow.GetValue("ItemCode", i));
                    if (ApprQty>0 && itemCode!="") 
                    {
                        Inspactiontransfer.Lines.ItemCode = itemCode;
                        Inspactiontransfer.Lines.FromWarehouseCode = FromWhs;
                        Inspactiontransfer.Lines.WarehouseCode = toWHSAppr;
                        Inspactiontransfer.Lines.Quantity = ApprQty;
                        if (MM == "B")
                        {
                            string strBatchApproved = " SELECT * from \"@B1_QA_RPT\" WHERE \"U_DocType\" = '" + OBT + "' AND \"U_DocEntry\"='" + docKey + "' AND \"U_LineNum\" = '" + LN + "' AND \"U_ApprQty\">0 ";
                            System.Data.DataTable dtLineBatch = Program.objHrmsUI.getDataTable(strBatchApproved, "Assigning Batch");
                            foreach (System.Data.DataRow dr in dtLineBatch.Rows)
                            {
                                Inspactiontransfer.Lines.BatchNumbers.BatchNumber = dr["U_Batch"].ToString();
                                Inspactiontransfer.Lines.BatchNumbers.Quantity = Convert.ToDouble(dr["U_ApprQty"]);
                                Inspactiontransfer.Lines.BatchNumbers.Add();
                            }

                        }
                        if (MM == "S")
                        {
                            string strBatchApproved = " SELECT * from \"@B1_QA_RPT\" WHERE \"U_DocType\" = '" + OBT + "' AND \"U_DocEntry\"='" + docKey + "' AND \"U_LineNum\" = '" + LN + "' AND \"U_ApprQty\">0 ";
                            System.Data.DataTable dtLineSerial = Program.objHrmsUI.getDataTable(strBatchApproved, "Assigning Batch");
                            foreach (System.Data.DataRow dr in dtLineSerial.Rows)
                            {
                                Inspactiontransfer.Lines.SerialNumbers.InternalSerialNumber = dr["U_Serial"].ToString();
                                Inspactiontransfer.Lines.SerialNumbers.Quantity = Convert.ToDouble(dr["U_ApprQty"]);
                                Inspactiontransfer.Lines.SerialNumbers.Add();
                            }
                        }

                        Inspactiontransfer.Lines.Add();
                    }
                    if (RejQty > 0 && itemCode != "")
                    {
                        Inspactiontransfer.Lines.ItemCode = itemCode;
                        Inspactiontransfer.Lines.FromWarehouseCode = FromWhs;
                        Inspactiontransfer.Lines.WarehouseCode = toWHSRej;
                        Inspactiontransfer.Lines.Quantity = RejQty;

                        if (MM == "B")
                        {
                            string strBatchApproved = " SELECT * from \"@B1_QA_RPT\" WHERE \"U_DocType\" = '" + OBT + "' AND \"U_DocEntry\"='" + docKey + "' AND \"U_LineNum\" = '" + LN + "' AND \"U_RejQty\">0 ";
                            System.Data.DataTable dtLineBatch = Program.objHrmsUI.getDataTable(strBatchApproved, "Assigning Batch");
                            foreach (System.Data.DataRow dr in dtLineBatch.Rows)
                            {
                                Inspactiontransfer.Lines.BatchNumbers.BatchNumber = dr["U_Batch"].ToString();
                                Inspactiontransfer.Lines.BatchNumbers.Quantity = Convert.ToDouble(dr["U_RejQty"]);
                                Inspactiontransfer.Lines.BatchNumbers.Add();
                            }

                        }
                        if (MM == "S")
                        {
                            string strBatchApproved = " SELECT * from \"@B1_QA_RPT\" WHERE \"U_DocType\" = '" + OBT + "' AND \"U_DocEntry\"='" + docKey + "' AND \"U_LineNum\" = '" + LN + "' AND \"U_RejQty\">0 ";
                            System.Data.DataTable dtLineSerial = Program.objHrmsUI.getDataTable(strBatchApproved, "Assigning Batch");
                            foreach (System.Data.DataRow dr in dtLineSerial.Rows)
                            {
                                Inspactiontransfer.Lines.SerialNumbers.InternalSerialNumber = dr["U_Serial"].ToString();
                                Inspactiontransfer.Lines.SerialNumbers.Quantity = Convert.ToDouble(dr["U_RejQty"]);
                                Inspactiontransfer.Lines.SerialNumbers.Add();
                            }
                        }

                        Inspactiontransfer.Lines.Add();
                    }
                }

                if (Inspactiontransfer.Add() != 0)
                {
                    int erroCode = 0;
                    string errDescr = "";
                    Program.objHrmsUI.oCompany.GetLastError(out erroCode, out errDescr);
                    oApplication.StatusBar.SetText("Failed to add transfer request   : " + errDescr);
                    result = -1;
                }
                else
                {
                    string outStr = Convert.ToString(Program.objHrmsUI.oCompany.GetNewObjectKey());
                    result = Convert.ToInt32(outStr);
                }

            }
            catch (Exception ex)
            {
                oApplication.MessageBox(ex.Message);
                result = -1;
            }

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

        private void refRowQtys()
        {
            int selRow = mtSelRow(mtDocRows);
            if (selRow > 0)
            {
                string RC = Convert.ToString(dtHead.GetValue("RC", 0));

                double ApprQty = 0;
                double RejQty = 0;
                getRptQtyForRows(RC, out ApprQty, out RejQty);
                dtRow.SetValue("ApprQty", selRow - 1, ApprQty.ToString());
                dtRow.SetValue("RejQty", selRow - 1, RejQty.ToString());

            }
            mtDocRows.SetLineData(selRow);


        }
        private void getRptQtyForRows(string RC,out double ApprQty, out double RejQty)
        {
            ApprQty = 0;
            RejQty = 0;
            string strQtys = "SELECT SUM(\"U_ApprQty\") AS \"ApprQty\" , SUM(\"U_RejQty\") AS \"RejQty\" FROM \"@B1_QA_RPT\" WHERE \"Code\" LIKE '" + RC + "%'";
            System.Data.DataTable dtQty = Program.objHrmsUI.getDataTable(strQtys, "RowQty");
            if (dtQty != null && dtQty.Rows.Count > 0)
            {
                ApprQty = Convert.ToDouble(dtQty.Rows[0]["ApprQty"]);
                RejQty = Convert.ToDouble(dtQty.Rows[0]["RejQty"]);
            }

        }


    }
}
