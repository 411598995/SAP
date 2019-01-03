using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACHR.Screen
{
    class frm_ProdList : HRMSBaseForm
    {

        public bool isForLoading = false;
        SAPbouiCOM.Matrix mtReport;
        SAPbouiCOM.ComboBox cbFilter,cbPL;
        SAPbouiCOM.EditText txProduct,txFrom,txTo;
        SAPbouiCOM.ButtonCombo btAct,btPL;
        SAPbouiCOM.OptionBtn opALL, opREL, opPL,opOd,opDd,opLast,opFirst,opX  ;
       

        SAPbouiCOM.DataTable dtRpt,dtHeads;
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
            oForm.Settings.MatrixUID = "mtReport";
            oForm.Settings.Enabled = true;

            string updateDispPosition = " Update OWOR set U_B1_DispPos =docentry where isnull(u_b1_dispPos,0) = 0 ";
            Program.objHrmsUI.ExecQuery(updateDispPosition,"Setting New Order positions");
            InitiallizeForm();

            SAPbouiCOM.Menus mnus = oApplication.Menus.Item("43557").SubMenus;
            foreach (SAPbouiCOM.MenuItem mnu in mnus)
            {
                string menuTitel = mnu.String;
                if (menuTitel == "Production Line Sequence Report")
                {
                     printMenuId =  mnu.UID.ToString();
                   
                }
            }

            
        }


        public override void etBeforeActClick(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etBeforeActClick(ref pVal, ref BubbleEvent);
            itemClicked = pVal.ItemUID;
        }

        public override void etAfterActClick(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterActClick(ref pVal, ref BubbleEvent);
            itemClicked = pVal.ItemUID;
            string rown = pVal.Row.ToString();
            
        }

        public override void etFormAfterResize(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etFormAfterResize(ref pVal, ref BubbleEvent);
            string rown = pVal.Row.ToString();
           

        }
        public override void etAfterClick(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {

            base.etAfterClick(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == "btPrint")
            {
                SAPbouiCOM.Menus mnus = oApplication.Menus;

                if (mnus.Exists(printMenuId))
                {
                    mnus.Item(printMenuId).Activate();
                }
                else
                {
                    oApplication.MessageBox("Please upload Production Sequence Report in production report menu");
                }
            }
            if (pVal.ItemUID == opALL.Item.UniqueID || pVal.ItemUID == opREL.Item.UniqueID || pVal.ItemUID == opPL.Item.UniqueID)
            {
                fillReport();
            }
            if (pVal.ItemUID == "btUp")
            {
                int selRow = mtSelRow(mtReport);
                if (selRow > 1)
                {
                    int DE1 = Convert.ToInt32(dtRpt.GetValue("DocEntry", selRow - 1));
                    int DE2 = Convert.ToInt32(dtRpt.GetValue("DocEntry", selRow - 2));

                    string strPos1 = Convert.ToString(dtRpt.GetValue("Seq", selRow - 1));
                    string strPos2 = Convert.ToString(dtRpt.GetValue("Seq", selRow - 2));

                    if (strPos1 != "" && strPos2 != "")
                    {

                        int Pos1 = Convert.ToInt32(dtRpt.GetValue("Seq", selRow - 1));
                        int Pos2 = Convert.ToInt32(dtRpt.GetValue("Seq", selRow - 2));

                        string pl1 = Convert.ToString(dtRpt.GetValue("ProdLine", selRow - 1));
                        string pl2 = Convert.ToString(dtRpt.GetValue("ProdLine", selRow - 2));


                        if (pl1 == pl2 && pl1 != "")
                        {
                            MoveUp(DE1, DE2, Pos1, Pos2);
                            mtReport.SelectRow(selRow - 1, true, false);
                        }
                    }
                    else
                    {
                        oApplication.MessageBox("Please check if production order has been assigned to production line and can be moved");
                    }
                }
            }
            if (pVal.ItemUID == "chMan")
            {
                chApply();
            }
            if (pVal.ItemUID == "btDown")
            {
                int selRow = mtSelRow(mtReport);
                if (selRow < mtReport.RowCount)
                {
                    int DE1 = Convert.ToInt32(dtRpt.GetValue("DocEntry", selRow - 1));
                    int DE2 = Convert.ToInt32(dtRpt.GetValue("DocEntry", selRow));
                    string strPos1 = Convert.ToString(dtRpt.GetValue("Seq", selRow - 1));
                    string strPos2 = Convert.ToString(dtRpt.GetValue("Seq", selRow));

                    if (strPos1 != "" && strPos2 != "")
                    {


                        int Pos1 = Convert.ToInt32(strPos1);
                        int Pos2 = Convert.ToInt32(strPos2);
                        string pl1 = Convert.ToString(dtRpt.GetValue("ProdLine", selRow - 1));
                        string pl2 = Convert.ToString(dtRpt.GetValue("ProdLine", selRow));


                        if (pl1 == pl2 && pl1 != "")
                        {
                            MoveDown(DE1, DE2, Pos1, Pos2);
                            mtReport.SelectRow(selRow + 1, true, false);
                        }

                    }
                    else
                    {
                        oApplication.MessageBox("Please check if production order has been assigned to production line and can be moved");
                    }
                }
            }

            if ((pVal.ItemUID == opOd.Item.UniqueID || pVal.ItemUID == opDd.Item.UniqueID || pVal.ItemUID=="txFrom" || pVal.ItemUID=="txTo") && !initiallizing)
            {
                if (txFrom.Value != "" || txTo.Value != "")
                {
                    fillReport();
                }
            }
            
           
          
        }
        public override void etBeforeClick(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etBeforeClick(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == mtReport.Item.UniqueID && pVal.Row > 0 && pVal.Row <= mtReport.RowCount)
            {
              
            }

        }
        public override void etAfterValidate(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterValidate(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == mtReport.Item.UniqueID)
            {
                if ( pVal.ColUID == "cFT")
                {
                    mtReport.FlushToDataSource();
                    int selRowIndex = pVal.Row - 1;
                    if (selRowIndex >= 0)
                    {
                        string DocEntry = Convert.ToString(dtRpt.GetValue("DocEntry", selRowIndex));
                        string Seq = Convert.ToString(dtRpt.GetValue("Seq", selRowIndex));
                        string FreeText = Convert.ToString(dtRpt.GetValue("FreeText", selRowIndex));
                        string prodLine = Convert.ToString(dtRpt.GetValue("ProdLine", selRowIndex));

                        if (pVal.ColUID == "cFT")
                        {
                            SAPbobsCOM.ProductionOrders prodOrder = (SAPbobsCOM.ProductionOrders)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oProductionOrders);
                            prodOrder.GetByKey(Convert.ToInt32(DocEntry));

                            prodOrder.UserFields.Fields.Item("U_B1_FrTxt").Value = FreeText;
                            int result = prodOrder.Update();
                            if (result != 0)
                            {
                                int errorCode = 0;
                                string errmsg = "";
                                oCompany.GetLastError(out errorCode, out errmsg);
                                oApplication.MessageBox(errmsg);


                            }
                            else
                            {

                            }

                        }
                        else
                        {
                          //  UpdateSeq(Convert.ToInt32(DocEntry), Convert.ToInt32(Seq), prodLine);
                        }

                    }

                }
            }
           
        }
        public override void etAfterKeyDown(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterKeyDown(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == "txProduct")
            {
                fillReport();
            }
            if (pVal.ItemUID == mtReport.Item.UniqueID && pVal.Row <= mtReport.RowCount)
            {

                if (pVal.CharPressed == (int)System.Windows.Forms.Keys.Enter || pVal.CharPressed == (int)System.Windows.Forms.Keys.Tab)
                    {
                        if (pVal.Row <= mtReport.RowCount)
                        {
                            int selRowIndex = pVal.Row - 1;

                            mtReport.FlushToDataSource();
                            string DocEntry = Convert.ToString(dtRpt.GetValue("DocEntry", selRowIndex));
                            string Seq = Convert.ToString(dtRpt.GetValue("Seq", selRowIndex));
                            string FreeText = Convert.ToString(dtRpt.GetValue("FreeText", selRowIndex));
                            string prodLine = Convert.ToString(dtRpt.GetValue("ProdLine", selRowIndex));
                            SAPbouiCOM.EditText newSeq = (SAPbouiCOM.EditText)mtReport.Columns.Item("cSeq").Cells.Item(pVal.Row ).Specific;
                            UpdateSeq(Convert.ToInt32(DocEntry), Convert.ToInt32(newSeq.Value), prodLine);
                            if (pVal.Row < mtReport.RowCount)
                            {
                                SAPbouiCOM.EditText nextSeq = (SAPbouiCOM.EditText)mtReport.Columns.Item("cSeq").Cells.Item(pVal.Row + 1).Specific;
                                nextSeq.Value = nextSeq.Value;
                                nextSeq.Active = true;
                            }
                        }
                    }
                
            }
        }

        public override void etAfterCmbSelect(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {

            base.etAfterCmbSelect(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == cbPL.Item.UniqueID && !initiallizing)
            {
                fillReport();
            }
            if (pVal.ItemUID == btAct.Item.UniqueID)
            {
                changeStatus();

                fillReport();
                btAct.Caption = "Change To";

            }
            if (pVal.ItemUID == btPL.Item.UniqueID)
            {
                assignPL();

                fillReport();
                btPL.Caption = "Assign to Production Line";

            }
            if (pVal.ItemUID == mtReport.Item.UniqueID)
            {
                if (pVal.ColUID == "cProdLine" || pVal.ColUID == "cLabel" || pVal.ColUID == "cFT")
                {
                    mtReport.FlushToDataSource();
                    int selRowIndex = pVal.Row-1;
                    if (selRowIndex >= 0)
                    {
                        string DocEntry = Convert.ToString(dtRpt.GetValue("DocEntry", selRowIndex));
                         string ProductionLine  = Convert.ToString( dtRpt.GetValue("ProdLine",selRowIndex));
                         string Label = Convert.ToString(dtRpt.GetValue("Label", selRowIndex));
                        SAPbobsCOM.ProductionOrders prodOrder = (SAPbobsCOM.ProductionOrders)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oProductionOrders);
                        prodOrder.GetByKey(Convert.ToInt32(DocEntry));

                        prodOrder.UserFields.Fields.Item("U_PMX_PLCD").Value = ProductionLine;
                        prodOrder.UserFields.Fields.Item("U_B1_Label").Value = Label;
                     
                       int result =  prodOrder.Update();
                       if (result != 0)
                       {
                           int errorCode = 0;
                           string errmsg = "";
                           oCompany.GetLastError(out errorCode, out errmsg);
                           oApplication.MessageBox(errmsg);
                          
                       }
                        
                       
                       
                    }

                }
            }
        }

        private void changeStatus()
        {
            string selButton = btAct.Selected.Value;
            string confirmMessage = "Are you sure you want to release selected production orders? ";
            if (selButton == "02") confirmMessage = "Are you sure you want to cancel selected production orders? ";
            int confirmresult = oApplication.MessageBox(confirmMessage, 1, "Yes", "No");
            if (confirmresult == 1)
            {
                SAPbobsCOM.ProductionOrders prodOrder = (SAPbobsCOM.ProductionOrders)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oProductionOrders);

                int selRowInd = 0;


                for (int i = 1; i <= mtReport.RowCount; i++)
                {
                    if (mtReport.IsRowSelected(i))
                    {
                        selRowInd = i - 1;

                        string DocEntry = Convert.ToString(dtRpt.GetValue("DocEntry", selRowInd));
                        prodOrder.GetByKey(Convert.ToInt32(DocEntry));
                        if (selButton == "01")
                        {
                            prodOrder.ProductionOrderStatus = SAPbobsCOM.BoProductionOrderStatusEnum.boposReleased;
                        }
                        else
                        {
                            prodOrder.ProductionOrderStatus = SAPbobsCOM.BoProductionOrderStatusEnum.boposCancelled;

                        }

                        int result = prodOrder.Update();
                        if (result != 0)
                        {
                            int errorCode = 0;
                            string errmsg = "";
                            oCompany.GetLastError(out errorCode, out errmsg);
                            oApplication.MessageBox(errmsg);


                        }
                        else
                        {

                        }
                    }
                }




            }
        }


        private void assignPL()
        {
            string selPL = btPL.Selected.Value;
            string selPLName = btPL.Selected.Description;
            string confirmMessage = "Are you sure you want selected production orders assign to  " + selPLName;
            int confirmresult = oApplication.MessageBox(confirmMessage, 1, "Yes", "No");
            if (confirmresult == 1)
            {
                SAPbobsCOM.ProductionOrders prodOrder = (SAPbobsCOM.ProductionOrders)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oProductionOrders);

                int selRowInd = 0;

                string posLast = "Y";
                if (opFirst.Selected) posLast = "N";

                string maxSeqNum = Program.objHrmsUI.getScallerValue("Select isnull( max( convert(int, U_B1_Seq)),0)  from owor where U_PMX_PLCD='" + selPL + "' and isnull(owor.Status,'')<>'C' ");
                if (maxSeqNum == "") maxSeqNum = "0";
                int maxSeqNumVal = Convert.ToInt32(maxSeqNum);


                for (int i = 1; i <= mtReport.RowCount; i++)
                {
                    if (mtReport.IsRowSelected(i))
                    {
                        maxSeqNumVal++;
                        selRowInd = i - 1;

                        string DocEntry = Convert.ToString(dtRpt.GetValue("DocEntry", selRowInd));
                        prodOrder.GetByKey(Convert.ToInt32(DocEntry));
                        prodOrder.UserFields.Fields.Item("U_PMX_PLCD").Value = selPL;

                        if (posLast == "Y")
                        {
                            prodOrder.UserFields.Fields.Item("U_B1_Seq").Value = maxSeqNumVal.ToString();

                        }
                        else
                        {
                            prodOrder.UserFields.Fields.Item("U_B1_Seq").Value = "1";

                        }


                        int result = prodOrder.Update();
                        if (result != 0)
                        {
                            maxSeqNumVal--;
                            int errorCode = 0;
                            string errmsg = "";
                            oCompany.GetLastError(out errorCode, out errmsg);
                            oApplication.MessageBox(errmsg);


                        }
                        else
                        {
                            if (posLast == "N")
                            {
                              string  strUpdate = "Update owor set U_B1_SEQ = isnull(U_B1_SEQ,999)  +1 where U_PMX_PLCD = '" + selPL + "'  and docentry <>  '" + DocEntry.ToString() + "' ; ";
                                Program.objHrmsUI.ExecQuery(strUpdate, "Increment old Seq");


                            }
                        }
                    }
                }




            }
        }
      
        
        private void InitiallizeForm()
        {


            oForm.Freeze(true);

            initiallizing = true;


            dtRpt = oForm.DataSources.DataTables.Item("dtRpt");
            dtHeads = oForm.DataSources.DataTables.Item("dtHeads");
            dtHeads.Rows.Add(1);
            mtReport = (SAPbouiCOM.Matrix)oForm.Items.Item("mtReport").Specific;
            btAct = (SAPbouiCOM.ButtonCombo)oForm.Items.Item("btAct").Specific;
            btPL = (SAPbouiCOM.ButtonCombo)oForm.Items.Item("btPL").Specific;

            btAct.ValidValues.Add("01", "Released");
            btAct.ValidValues.Add("02", "Canceled");


            opALL = (SAPbouiCOM.OptionBtn)oForm.Items.Item("opAO").Specific;
            opREL = (SAPbouiCOM.OptionBtn)oForm.Items.Item("opREL").Specific;
            opPL = (SAPbouiCOM.OptionBtn)oForm.Items.Item("opPL").Specific;

            opOd = (SAPbouiCOM.OptionBtn)oForm.Items.Item("opOd").Specific;
            opDd = (SAPbouiCOM.OptionBtn)oForm.Items.Item("opDd").Specific;

            opFirst = (SAPbouiCOM.OptionBtn)oForm.Items.Item("opFirst").Specific;
            opLast = (SAPbouiCOM.OptionBtn)oForm.Items.Item("opLast").Specific;
            opX = (SAPbouiCOM.OptionBtn)oForm.Items.Item("opX").Specific;


            opREL.GroupWith("opAO");
            opPL.GroupWith("opAO");
            opDd.GroupWith("opOd");

            opLast.GroupWith("opFirst");
            opX.GroupWith("opFirst");
            opLast.Selected = true;


            txProduct = (SAPbouiCOM.EditText)oForm.Items.Item("txProduct").Specific;
            txFrom = (SAPbouiCOM.EditText)oForm.Items.Item("txFrom").Specific;
            txTo = (SAPbouiCOM.EditText)oForm.Items.Item("txTo").Specific;

            cbFilter = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbFilter").Specific;
            cbPL = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbPL").Specific;
            cbPL.ValidValues.Add("0", "All");
            fillCbs();
          //  fillReport();
            oForm.Freeze(false);

            opALL.Selected = true;
            opOd.Selected = true;
            initiallizing = false;
          


        }

        private void fillCbs()
        {
            cbFilter.ValidValues.Add("Status", "Status");
            cbFilter.ValidValues.Add("Product", "Product");

            cbFilter.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);

            SAPbouiCOM.Column ProdLine = mtReport.Columns.Item("cProdLine");
            SAPbouiCOM.Column Label = mtReport.Columns.Item("cLabel");
            insertProdType();

            string cbDataSql = "Select * from [@PMX_OSPL]";
            System.Data.DataTable cbData = Program.objHrmsUI.getDataTable(cbDataSql, "Get CB Data");
            foreach (System.Data.DataRow dr in cbData.Rows)
            {
                ProdLine.ValidValues.Add(dr["Code"].ToString(), dr["Name"].ToString());
                cbPL.ValidValues.Add(dr["Code"].ToString(), dr["Name"].ToString());
                btPL.ValidValues.Add(dr["Code"].ToString(), dr["Name"].ToString());
            }


            cbPL.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
            int cnt = Convert.ToInt32( Program.objHrmsUI.getScallerValue("Select count(*) from [@B1_Label]"));
            if (cnt == 0)
            {
                insertLabel();

            }
            cbDataSql = "Select * from [@B1_Label]";
            cbData = Program.objHrmsUI.getDataTable(cbDataSql, "Get CB Data");
            foreach (System.Data.DataRow dr in cbData.Rows)
            {
                Label.ValidValues.Add(dr["Code"].ToString(), dr["Name"].ToString());
            }

            
        }
        private void insertLabel()
        {
            string strInsert = "Insert into [@B1_Label] select 'Red','Red'";
            Program.objHrmsUI.ExecQuery(strInsert, "inserting default lable");
            strInsert = "Insert into [@B1_Label] select 'Green','Green'";
            Program.objHrmsUI.ExecQuery(strInsert, "inserting default lable");
            strInsert = "Insert into [@B1_Label] select 'Yellow','Yellow'";
            Program.objHrmsUI.ExecQuery(strInsert, "inserting default lable");

        }
        private void insertProdType()
        {
            string strInert = @"
                        insert into [@PMX_OSPL] (code,name)

                        SELECT        t1.Code, t1.Name 
                        FROM            [@PMX_OSPL] AS t0 RIGHT OUTER JOIN
                                                 PMX_OSPL AS t1 ON t0.Code = t1.Code
                        where t0.Code is null";
            Program.objHrmsUI.ExecQuery(strInert, "inserting default lable");
        }

        private void fillReport()
        {
            SAPbouiCOM.StaticText lblLoading = (SAPbouiCOM.StaticText) oForm.Items.Item("lblLoad").Specific;
            int GoldenColor = 0xFFC908;// System.Drawing.Color.DarkGoldenrod.ToArgb() - 255;
            int redColor = System.Drawing.Color.Red.R | (System.Drawing.Color.Red.G << 8) | (System.Drawing.Color.Red.B << 16);
            int greenColor = System.Drawing.Color.Green.R | (System.Drawing.Color.Green.G << 8) | (System.Drawing.Color.Green.B << 16);
         

           
            lblLoading.Item.Visible = true;
            lblLoading.Item.FontSize = 14;
            lblLoading.Item.BackColor = GoldenColor;
            lblLoading.Item.ForeColor = greenColor;

            oForm.Freeze(true);
            dtRpt.Rows.Clear();
            string strCritaria = " Where (owor.status  <> 'L' and owor.status  <> 'C') ";
          //  strCritaria = " Where 'Y' = 'Y'  ";
            string dtField = "owor.postDate";
            if (opDd.Selected) dtField = "owor.DueDate";


            if (txFrom.Value != "" || txTo.Value == "")
            {
                if (txFrom.Value != "")
                {
                    strCritaria += " And " + dtField + " >= '" +  txFrom.Value + "'";
        
                }
                if (txTo.Value != "")
                {
                    strCritaria += " And " + dtField + " <= '" + txTo.Value + "'";

                }
            }
            if (cbPL.Value.ToString().Trim() != "0")
            {
                strCritaria += " And isnull(U_PMX_PLCD,'') = '" + cbPL.Value.ToString() +"'";
            }
            if (txProduct.Value != "")
            {
                strCritaria += " And isnull(owor.itemcode,'') Like  '" + txProduct.Value.ToString() + "%'";
            }
            else
            {
            }
            if (opREL.Selected)
            {
                strCritaria += " and owor.status = 'R' ";

            }


            if (opPL.Selected)
            {
                strCritaria += " and owor.status = 'P' ";

            }

            string strSelect = "Select owor.DocNum as DocNum, case  owor.type when 'S' then 'Standard' when 'P' then 'Special' when 'D' then 'Disassembly' else 'UD' end as Type , case owor.status when 'P' then 'Planned' when 'R' then 'Released' else 'Closed' end as Status,owor.itemcode,oitm.itemname ";
            strSelect += " , owor.postdate, owor.duedate , owor.plannedQty , U_PMX_PLCD , U_B1_Label,U_B1_SEQ, U_B1_FrTxt,isnull(U_B1_DispPos,owor.docentry) as Pos, owor.DocEntry ";
            strSelect += " from owor inner join oitm on oitm.itemcode = owor.itemcode ";

            string strOrderBy = "  order by  case isnull(U_PMX_PLCD,'') when '' then '999' else U_PMX_PLCD end , convert(int, case  isnull(U_B1_SEQ,'') when '' then '99' else  U_B1_SEQ end)";
            
            System.Data.DataTable sboDtRpt = Program.objHrmsUI.getDataTable(strSelect + strCritaria + strOrderBy, "Open POS");

            int i=0;
            int rowCnt = sboDtRpt.Rows.Count;
            if (rowCnt > 0)
            {
                dtRpt.Rows.Add(rowCnt);
            }
            foreach (System.Data.DataRow dr in sboDtRpt.Rows)
            {
                i++;
                string seq = "";
                if (dr["U_B1_SEQ"].ToString() != "") seq = dr["U_B1_SEQ"].ToString();
                dtRpt.SetValue("Id", i - 1, i.ToString());
                dtRpt.SetValue("Type", i - 1, dr["type"].ToString());
                dtRpt.SetValue("DocNum", i - 1, dr["DocNum"].ToString());
                dtRpt.SetValue("OD", i - 1, Convert.ToDateTime(dr["postdate"]));
                dtRpt.SetValue("DueDate", i - 1, Convert.ToDateTime(dr["DueDate"]));
                dtRpt.SetValue("PlannedQty", i - 1, Convert.ToString(dr["PlannedQty"]));

                dtRpt.SetValue("Status", i - 1, dr["Status"].ToString());

                dtRpt.SetValue("ItemCode", i - 1, dr["ItemCode"].ToString());
                dtRpt.SetValue("ItemName", i - 1, dr["ItemName"].ToString());
                dtRpt.SetValue("ProdLine", i - 1, dr["U_PMX_PLCD"].ToString());
                dtRpt.SetValue("Label", i - 1, dr["U_B1_Label"].ToString());

                dtRpt.SetValue("Seq", i - 1, seq);
                dtRpt.SetValue("FreeText", i - 1, dr["U_B1_FrTxt"].ToString());
                dtRpt.SetValue("Pos", i - 1, dr["Pos"].ToString());
                dtRpt.SetValue("DocEntry", i - 1, dr["DocEntry"].ToString());


            }
            mtReport.LoadFromDataSource();

            chApply();

            oForm.Freeze(false);
            lblLoading.Item.Visible = false;
          
            if (mtReport.RowCount > 0)
            {
                mtReport.SelectRow(1, true, false);
            }

        }

        private void chApply()
        {
            string chMan = Convert.ToString(dtHeads.GetValue("chMan", 0));
            for (int K = 0; K < dtRpt.Rows.Count; K++)
            {
                int colnum = 10;

                string rowSeqAssigned = Convert.ToString(dtRpt.GetValue("Seq", K));
                if (rowSeqAssigned == "")
                {
                    if (chMan != "Y")
                    {
                        mtReport.CommonSetting.SetCellEditable(K + 1, colnum, false);
                    }
                    else
                    {
                        mtReport.CommonSetting.SetCellEditable(K + 1, colnum, true);

                    }
                }
                else
                {
                    mtReport.CommonSetting.SetCellEditable(K + 1, colnum, true);

                }


            }
        }

        private void MoveUp(int DE1, int DE2, int Pos1, int Pos2)
        {
            string strUpdate = "Update owor set U_B1_SEQ = '" + Pos2 + "' where docentry = '" + DE1 + "';";
            strUpdate += "Update owor set U_B1_SEQ = '" + Pos1 + "' where docentry = '" + DE2 + "';";
            Program.objHrmsUI.ExecQuery(strUpdate,"Moving Up");
            fillReport();
        }
        private void MoveDown(int DE1, int DE2, int Pos1, int Pos2)
        {
            string strUpdate = "Update owor set U_B1_SEQ = '" + Pos2 + "' where docentry = '" + DE1 + "';";
            strUpdate += "Update owor set U_B1_SEQ = '" + Pos1 + "' where docentry = '" + DE2 + "';";
            Program.objHrmsUI.ExecQuery(strUpdate, "Moving Down");
            fillReport();
        }

        private void UpdateSeq(int DE, int newSeq,string ProdLine)
        {
            int strOldSeq = Convert.ToInt32(Program.objHrmsUI.getScallerValue("select  case isnull(U_B1_SEQ,'') when '' then '99' else U_B1_SEQ end  from owor where docentry = '" + DE.ToString() + "'"));

            if (strOldSeq != newSeq)
            {
                string strUpdate = " Update owor set U_B1_SEQ = '" + newSeq + "' where docentry = '" + DE.ToString() + "'";

                string chMan = Convert.ToString(dtHeads.GetValue("chMan", 0));
                if (chMan != "Y")
                {

                    if (Convert.ToInt32(strOldSeq) < newSeq)
                    {
                        strUpdate += " ; Update owor set U_B1_SEQ = convert(int,U_B1_SEQ) -1 where U_PMX_PLCD = '" + ProdLine + "' and docentry <> '" + DE.ToString() + "' and convert(int,U_B1_SEQ) between '" + strOldSeq + "' and '" + newSeq + "' ; ";


                    }
                    if (Convert.ToInt32(strOldSeq) > newSeq)
                    {
                        strUpdate += " ; Update owor set U_B1_SEQ =convert(int, U_B1_SEQ) +1 where U_PMX_PLCD = '" + ProdLine + "' and docentry <> '" + DE.ToString() + "' and convert(int,U_B1_SEQ) between '" + newSeq + "' and '" + strOldSeq + "' ; ";


                    }
                }
                Program.objHrmsUI.ExecQuery(strUpdate, "Update to down");
            }

            fillReport();
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



    

    }
}
