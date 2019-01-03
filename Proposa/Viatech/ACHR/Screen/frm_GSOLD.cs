using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;
using SAPbobsCOM;

namespace ACHR.Screen
{
    class frm_GS : HRMSBaseForm
    {

        public bool isForLoading = false;
        SAPbouiCOM.Folder tbTS, tbSO;
        SAPbouiCOM.OptionBtn opOD, opOW, opOM,OMDT,OMDY;
        SAPbouiCOM.Matrix mtItem, mtTOR, mtSTO, mtORI, mtORAT, mtStock, mtDate, mtDay,mtOSCN,mtOT;
        SAPbouiCOM.ComboBox cbSP, cbDays, cbWeeks,cbHH,cbMM,cbAP,cbEWN,cbOTHH,cbOTMM,cbOTAP;
        SAPbouiCOM.EditText txCode, txName, txCT, txICode, txOMDT,txOICODE,tsEWD,txOTDT,txSL;
        SAPbouiCOM.ChooseFromList cardCFL;
        SAPbouiCOM.PictureBox imgImage;


        SAPbouiCOM.DataTable dtHead, dtItem, dtDate, dtDays,dtOSCN,dtOT;

        string currentTab = "TS";

        public override void CreateForm(SAPbouiCOM.Application SboApp, string strXml, SAPbobsCOM.Company cmp, string frmId)
        {
            base.CreateForm(SboApp, strXml, cmp, frmId);
            InitiallizeForm();
        }
        public override void etAfterCfl(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCfl(ref pVal, ref BubbleEvent);
            SAPbouiCOM.IChooseFromListEvent oCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pVal;
            SAPbouiCOM.DataTable dtSel = oCFLEvento.SelectedObjects;
                
            if (pVal.ItemUID == "txCode")
            {
               string CardCode = "";
                string CardName = "";
                if (dtSel != null && dtSel.Rows.Count > 0)
                {
                    for (int i = 0; i < dtSel.Rows.Count; i++)
                    {

                        CardCode = dtSel.GetValue("CardCode", i).ToString();
                        CardName =  dtSel.GetValue("CardName", i).ToString();
                    }
                    // mtCus.SetLineData(pVal.Row); 
                    dtHead.Rows.Remove(0);
                    dtHead.Rows.Add(1);

                    dtHead.SetValue("CardCode", 0, CardCode);
                    dtHead.SetValue("CardName", 0, CardName);

                    AddNewCardSche(CardCode);
                    getSchedule();
                }
              
            }
            if (pVal.ItemUID == "txICode")
            {
               
                if (dtSel != null && dtSel.Rows.Count > 0)
                {
                    string ItemCode="", ItemName = "";
                    for (int i = 0; i < dtSel.Rows.Count; i++)
                    {

                        ItemCode = dtSel.GetValue("ItemCode", i).ToString();
                        ItemName = dtSel.GetValue("ItemName", i).ToString();
                    }
                    // mtCus.SetLineData(pVal.Row); 
                    dtHead.SetValue("ItemCode", 0, ItemCode);
                    dtHead.SetValue("ItemName", 0, ItemName);

                   
                }
            }

            if (pVal.ItemUID == "txOICODE")
            {

                if (dtSel != null && dtSel.Rows.Count > 0)
                {
                    string ItemCode = "", ItemName = "";
                    for (int i = 0; i < dtSel.Rows.Count; i++)
                    {

                        ItemCode = dtSel.GetValue("ItemCode", i).ToString();
                        ItemName = dtSel.GetValue("ItemName", i).ToString();
                    }
                    // mtCus.SetLineData(pVal.Row); 
                    dtHead.SetValue("OSCNCode", 0, ItemCode);
                    dtHead.SetValue("OSCNName", 0, ItemName);


                }
            }

        }
        public override void etAfterClick(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);

            if (pVal.ItemUID == mtOSCN.Item.UniqueID)
            {


                if (pVal.ColUID == "Id" && pVal.Row >0)
                {
                    getOSCNDetail();
                }
            }

            if (pVal.ItemUID == mtOSCN.Item.UniqueID)
            {


                if (pVal.ColUID == "Id" && pVal.Row > 0)
                {
                    getOSCNDetail();
                }
            }

            if (pVal.ItemUID == "btUC")
            {
                updateOSCN();
            }

            if (pVal.ItemUID == "btOAdd")
            {
                addOSCN();
            }
            if (pVal.ItemUID == "btImg")
            {
                addAttacment();
            }
            if (pVal.ItemUID == "tbTS")
            {
                currentTab = "TS";
                getSchedule();
            }
            if (pVal.ItemUID == "tbSO")
            {
                currentTab = "SO";
                getSchedule();
            }
          
            if (pVal.ItemUID == "1")
            {
                updateSchedule();
            }
            if (pVal.ItemUID == "btAddDt")
            {
                string cardCode = Convert.ToString(dtHead.GetValue("CardCode", 0));

                addMODT(cardCode);
            }
            if (pVal.ItemUID == "btRemDt")
            {
                string cardCode = Convert.ToString(dtHead.GetValue("CardCode", 0));

                remMODT(cardCode);
            }
            if (pVal.ItemUID == "btAddWD")
            {
                string cardCode = Convert.ToString(dtHead.GetValue("CardCode", 0));

                addMODay(cardCode);
            }

            if (pVal.ItemUID == "btRemWD")
            {
                string cardCode = Convert.ToString(dtHead.GetValue("CardCode", 0));

                remMODay(cardCode);
            }


            if (pVal.ItemUID == "btAdd")
            {
                addSOItem();
              

            }
            if (pVal.ItemUID == "btOTAdd")
            {
              
                string cardCode = Convert.ToString(dtHead.GetValue("CardCode", 0));
                string otDT = Convert.ToString(dtHead.GetValue("otDT", 0));
                if (cardCode == "") { oApplication.MessageBox("Select Customer"); return; }
                if (otDT == "") { oApplication.MessageBox("Select Date"); return; }

                addOT(cardCode);


            }

            if (pVal.ItemUID == "btRem")
            {
                int selRow = mtSelRow(mtItem);
                string rowId = Convert.ToString( dtItem.GetValue("Id", selRow-1));
                dtItem.Rows.Remove(selRow-1);
                mtItem.LoadFromDataSource();


                string strInsert = " Delete From  [@B1_SO] where  Code='" + rowId + "'";
                Program.objHrmsUI.ExecQuery(strInsert, "Delete SO Item");

            }
            if (pVal.ItemUID == "btOTRem")
            {
                int selRow = mtSelRow(mtOT);
                string rowId = Convert.ToString(dtOT.GetValue("Id", selRow - 1));
                dtOT.Rows.Remove(selRow - 1);
                mtOT.LoadFromDataSource();


                string strInsert = " Delete From [@B1_SCHOT] where  Code='" + rowId + "'";
                Program.objHrmsUI.ExecQuery(strInsert, "Delete OT Date");

            }
        }
        public override void etAfterCmbSelect(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCmbSelect(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == cbHH.Item.UniqueID || pVal.ItemUID == cbMM.Item.UniqueID || pVal.ItemUID == cbAP.Item.UniqueID)
            {
                setCallTime();
            }
        }

       
        
        private void addSOItem()
        {
            long maxId = Program.objHrmsUI.getMaxId("[@B1_SO]", "CODE");

            string itemCode = "", itemName = "", Qty = "";
            itemCode = Convert.ToString(dtHead.GetValue("ItemCode", 0));
            itemName = Convert.ToString(dtHead.GetValue("ItemName", 0));
            Qty = Convert.ToString(dtHead.GetValue("Qty", 0));
            dtItem.Rows.Add(1);
            dtItem.SetValue("Id", dtItem.Rows.Count - 1, maxId.ToString());
            dtItem.SetValue("ItemCode", dtItem.Rows.Count - 1, dtHead.GetValue("ItemCode", 0));
            dtItem.SetValue("ItemName", dtItem.Rows.Count - 1, dtHead.GetValue("ItemName", 0));
            dtItem.SetValue("Qty", dtItem.Rows.Count - 1, dtHead.GetValue("Qty", 0));
            mtItem.LoadFromDataSource();
            string strInsert = " insert into  [@B1_SO]  (Code, Name, U_SCCode, U_ItemCode, U_Qty) ";
            strInsert += " Values ('" + maxId + "' ,'" + maxId + "','SO_" + txCode.Value.ToString() + "','" + itemCode + "','" + Qty + "')";
            Program.objHrmsUI.ExecQuery(strInsert, "Adding SO Item");


            dtHead.SetValue("ItemCode", 0, "");
            dtHead.SetValue("ItemName", 0, "");
            dtHead.SetValue("Qty", 0, "0.00");

            txICode.Active = true;


        }

        private void addOT(string cardCode)
        {
            string schCode = "";
            if (currentTab == "TS")
            {
                schCode = "TS_" + cardCode;

            }

            else
            {
                schCode = "SO_" + cardCode;

            }



            long maxId = Program.objHrmsUI.getMaxId("[@B1_SCHOT]", "CODE");

            string strTime = "";
            DateTime strDate = Convert.ToDateTime(dtHead.GetValue("otDT", 0));
            strTime = cbOTHH.Selected.Value.Trim() + ":" + cbOTMM.Value.ToString().Trim() + " " + cbOTAP.Value.ToString().Trim();

            for (int i = 0; i < dtOT.Rows.Count; i++)
            {
                DateTime rowDate = Convert.ToDateTime(dtOT.GetValue("Date", i));
                string rowTime = Convert.ToString(dtOT.GetValue("Time", i));
                if (strDate == rowDate && strTime == rowTime)
                {
                    oApplication.MessageBox("Already Exist");
                    return;
                }
            }
            dtOT.Rows.Add(1);
            dtOT.SetValue("Id", dtOT.Rows.Count - 1, maxId.ToString());
            dtOT.SetValue("Date", dtOT.Rows.Count - 1, strDate);
            dtOT.SetValue("Time", dtOT.Rows.Count - 1, strTime);
            mtOT.LoadFromDataSource();
            string strInsert = " insert into  [@B1_SCHOT]  (Code, Name, U_SCCode, U_Date, U_Time) ";
            strInsert += " Values ('" + maxId + "' ,'" + maxId + "','" + schCode + "','" + strDate.ToString("yyyyMMdd") + "','" + Program.objHrmsUI.getIntTime(strTime) + "')";
            Program.objHrmsUI.ExecQuery(strInsert, "Adding OT Entry");


            dtHead.SetValue("otDT", 0, "");



        }


        private void addMODT(string cardCode)
        {
            string schCode = "";
            if (currentTab == "TS")
            {
                schCode = "TS_" + cardCode;

            }

            else
            {
                schCode = "SO_" + cardCode;

            }
            long maxId = Program.objHrmsUI.getMaxId("[@B1_SCHMDT]", "CODE");

            string strDate = "";
            strDate = Convert.ToString(dtHead.GetValue("txOMDT", 0));
            if (strDate == "0" || strDate == "") return;
            for (int i = 0; i < dtDate.Rows.Count; i++)
            {
                string rowDate = Convert.ToString(dtDate.GetValue("Date", i));
                if (strDate == rowDate )
                {
                    oApplication.MessageBox("Already Exist");
                    return;
                }
            }
            dtDate.Rows.Add(1);
            dtDate.SetValue("Id", dtDate.Rows.Count - 1, maxId.ToString());
            dtDate.SetValue("Date", dtDate.Rows.Count - 1, dtHead.GetValue("txOMDT", 0));
            mtDate.LoadFromDataSource();
            string strInsert = " insert into  [@B1_SCHMDT]  (Code, Name, U_SCCode, U_Mdates) ";
            strInsert += " Values ('" + maxId + "' ,'" + maxId + "','" + schCode + "','" + strDate + "')";
            Program.objHrmsUI.ExecQuery(strInsert, "Adding MO Date");


            dtHead.SetValue("txOMDT", 0, "0");

            txOMDT.Active = true;


        }


        private void remMODT(string cardCode)
        {
            string schCode = "";
            if (currentTab == "TS")
            {
                schCode = "TS_" + cardCode;

            }

            else
            {
                schCode = "SO_" + cardCode;

            }
          
            string strDate = "";
            int selRowInd = mtSelRow(mtDate);
            if (selRowInd == 0 || selRowInd > mtDate.RowCount) return;
            strDate = Convert.ToString(dtDate.GetValue("Date", selRowInd-1));
           string strInsert = " delete from   [@B1_SCHMDT] where  U_SCCode = '" + schCode + "' and U_Mdates = '" + strDate + "'";
           Program.objHrmsUI.ExecQuery(strInsert, "Removing MO Date");


           dtDate.Rows.Remove(selRowInd-1);

           mtDate.LoadFromDataSource();


        }
        private void addOSCN()
        {
            string strCardCode = dtHead.GetValue("CardCode", 0).ToString();
            if (strCardCode == "")
            {
                oApplication.MessageBox("Customer Not Selected");
                return;
            }
            else
            {
                string itemCode = dtHead.GetValue("OSCNCode", 0).ToString();
                string CAT = dtHead.GetValue("OSCNCat", 0).ToString();

                SAPbobsCOM.AlternateCatNum catNum = (SAPbobsCOM.AlternateCatNum)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oAlternateCatNum);

                catNum.ItemCode = itemCode;
                catNum.CardCode = strCardCode;
                catNum.Substitute = CAT;
                if (catNum.Add() != 0)
                {
                    int erroCode = 0;
                    string errDescr = "";
                    Program.objHrmsUI.oCompany.GetLastError(out erroCode, out errDescr);
                    oApplication.StatusBar.SetText("Failed to add BP Catalog  : " + errDescr);
                }
                else
                {

                    dtHead.SetValue("OSCNCode", 0, "");
                    dtHead.SetValue("OSCNCat", 0, "");
                    txOICODE.Active = true;
                    oApplication.SetStatusBarMessage("Catalog Added Successfully", SAPbouiCOM.BoMessageTime.bmt_Short, false);
                    getOSCN();
                    
                }


            }


        }

        
        private void addMODay(string cardCode)
        {
            string schCode = "";
            if (currentTab == "TS")
            {
                schCode = "TS_" + cardCode;

            }

            else
            {
                schCode = "SO_" + cardCode;

            }
            long maxId = Program.objHrmsUI.getMaxId("[@B1_SCHMDY]", "CODE");

            string strDay,StrWeek = "";
            strDay = cbDays.Value.ToString();
            StrWeek = cbWeeks.Value.ToString();
            if (strDay == "" || StrWeek == "")
            {
                return;
            }
            for (int i = 0; i < dtDays.Rows.Count; i++)
            {
                string rowDay = Convert.ToString(dtDays.GetValue("Day", i));
                string rowWeek = Convert.ToString(dtDays.GetValue("Week", i));
                if (strDay == rowDay && StrWeek == rowWeek)
                {
                    oApplication.MessageBox("Already Exist");
                    return;
                }
            }
            dtDays.Rows.Add(1);
            dtDays.SetValue("Id", dtDays.Rows.Count - 1, maxId.ToString());
            dtDays.SetValue("Day", dtDays.Rows.Count - 1, strDay);
            dtDays.SetValue("Week", dtDays.Rows.Count - 1, StrWeek);

            mtDay.LoadFromDataSource();
            string strInsert = " insert into  [@B1_SCHMDY]	  (Code, Name, U_SCCode, U_Day,U_WeekNum) ";
            strInsert += " Values ('" + maxId + "' ,'" + maxId + "','" + schCode + "','" + strDay + "','" + StrWeek + "')";
            Program.objHrmsUI.ExecQuery(strInsert, "Adding MO Days");

     

        }


        private void remMODay(string cardCode)
        {
            string schCode = "";
            if (currentTab == "TS")
            {
                schCode = "TS_" + cardCode;

            }

            else
            {
                schCode = "SO_" + cardCode;

            }
            long maxId = Program.objHrmsUI.getMaxId("[@B1_SCHMDY]", "CODE");

            string strDay, StrWeek = "";
            int selRowId = mtSelRow(mtDay);
            strDay = Convert.ToString(dtDays.GetValue("Day", selRowId - 1));
            StrWeek = Convert.ToString(dtDays.GetValue("Week", selRowId - 1));

            string strInsert = " delete  from  [@B1_SCHMDY] where  U_SCCode='" + schCode + "' and  U_Day='" + strDay + "' and U_WeekNum='" + StrWeek + "'  ";
            Program.objHrmsUI.ExecQuery(strInsert, "removing MO Days");
            dtDays.Rows.Remove(selRowId - 1);
            mtDay.LoadFromDataSource();


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

        public override void etAfterLostFocus(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterLostFocus(ref pVal, ref BubbleEvent);
           
        }

        private void InitiallizeForm()
        {


            oForm.Freeze(true);

           // Program.objHrmsUI.ExecQuery("Update B1_SCHMDT set u_upd='Y' where isnull(u_upd,'N') = 'N'", "Finalizing temp");
            Program.objHrmsUI.ExecQuery("delete from [@B1_SCHMDT]  where isnull(u_upd,'N') = 'N'", "Finalizing temp");
            Program.objHrmsUI.ExecQuery("delete from [@B1_SCHMDY]  where isnull(u_upd,'N') = 'N'", "Finalizing temp");
            Program.objHrmsUI.ExecQuery("delete from [@B1_SO]  where isnull(u_upd,'N') = 'N'", "Finalizing temp");
            Program.objHrmsUI.ExecQuery("delete from [@B1_SCHOT]  where isnull(u_upd,'N') = 'N'", "Finalizing temp");


            cardCFL = (SAPbouiCOM.ChooseFromList)oForm.ChooseFromLists.Item("cflCRD");
            SAPbouiCOM.Conditions oCons = cardCFL.GetConditions();
            SAPbouiCOM.Condition oCon = oCons.Add();
            oCon.Alias = "CardType";
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
            oCon.CondVal = "C";
            cardCFL.SetConditions(oCons);

            dtHead = oForm.DataSources.DataTables.Item("dtHead");
            dtItem = oForm.DataSources.DataTables.Item("dtItem");
            dtDate = oForm.DataSources.DataTables.Item("dtDate");
            dtDays = oForm.DataSources.DataTables.Item("dtDays");
            dtOSCN = oForm.DataSources.DataTables.Item("dtOSCN");
            dtOT = oForm.DataSources.DataTables.Item("dtOT");

            mtItem = (SAPbouiCOM.Matrix)oForm.Items.Item("mtItem").Specific;

            mtDate = (SAPbouiCOM.Matrix)oForm.Items.Item("mtDate").Specific;
            mtDay = (SAPbouiCOM.Matrix)oForm.Items.Item("mtDays").Specific;
            mtOSCN = (SAPbouiCOM.Matrix)oForm.Items.Item("mtOSCN").Specific;
            mtOT = (SAPbouiCOM.Matrix)oForm.Items.Item("mtOT").Specific;
          

            dtHead.Rows.Add(1);
            dtHead.SetValue("tsActive",0, "Y");

          
             txCode = (SAPbouiCOM.EditText)oForm.Items.Item("txCode").Specific;
             txName = (SAPbouiCOM.EditText)oForm.Items.Item("txName").Specific;
             txCT = (SAPbouiCOM.EditText)oForm.Items.Item("txCT").Specific;
             txOTDT = (SAPbouiCOM.EditText)oForm.Items.Item("txOTDT").Specific;

             txICode = (SAPbouiCOM.EditText)oForm.Items.Item("txICode").Specific;
             txOMDT = (SAPbouiCOM.EditText)oForm.Items.Item("txOMDT").Specific;
             txOICODE = (SAPbouiCOM.EditText)oForm.Items.Item("txOICODE").Specific;
             imgImage = (SAPbouiCOM.PictureBox)oForm.Items.Item("imgImage").Specific;
             txSL = (SAPbouiCOM.EditText)oForm.Items.Item("txSL").Specific;


           
            
        

            cbSP = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbSP").Specific;
            cbDays = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbDays").Specific;
            cbWeeks = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbWeeks").Specific;

            cbHH = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbHH").Specific;
            cbMM = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbMM").Specific;
            cbAP = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbAP").Specific;
            cbEWN = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbEWN").Specific;

            cbOTHH = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbOTHH").Specific;
            cbOTMM = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbOTMM").Specific;
            cbOTAP = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbOTAP").Specific;
           

         


            opOD = (SAPbouiCOM.OptionBtn)oForm.Items.Item("opOD").Specific;
            opOW = (SAPbouiCOM.OptionBtn)oForm.Items.Item("opOW").Specific;
            opOM = (SAPbouiCOM.OptionBtn)oForm.Items.Item("opOM").Specific;
            OMDY = (SAPbouiCOM.OptionBtn)oForm.Items.Item("OMDY").Specific;
            OMDT = (SAPbouiCOM.OptionBtn)oForm.Items.Item("OMDT").Specific;

            opOW.GroupWith("opOD");
            opOM.GroupWith("opOD");

            OMDY.GroupWith("OMDT");
            opOD.Selected = true;
            OMDT.Selected = true;
            tbSO = (SAPbouiCOM.Folder) oForm.Items.Item("tbSO").Specific;
            tbTS = (SAPbouiCOM.Folder)oForm.Items.Item("tbTS").Specific;

            tbTS.Select();
            fillCBs();
            oForm.Freeze(false);

            txCode.Active = true;

           




        }

       
        private void fillCBs()
        {

            for (int i = 1; i <= 12; i++)
            {
                cbHH.ValidValues.Add(i.ToString().PadLeft(2, '0'), i.ToString().PadLeft(2, '0'));
                cbOTHH.ValidValues.Add(i.ToString().PadLeft(2, '0'), i.ToString().PadLeft(2, '0'));

            }

            for (int i = 0; i <= 59; i+=5)
            {
                cbMM.ValidValues.Add(i.ToString().PadLeft(2,'0'), i.ToString().PadLeft(2,'0'));
                cbOTMM.ValidValues.Add(i.ToString().PadLeft(2, '0'), i.ToString().PadLeft(2, '0'));

            }
            cbAP.ValidValues.Add("AM","AM");
            cbAP.ValidValues.Add("PM", "PM");
            cbOTAP.ValidValues.Add("AM", "AM");
            cbOTAP.ValidValues.Add("PM", "PM");


            cbEWN.ValidValues.Add("1", "1");
            cbEWN.ValidValues.Add("2", "2");
            cbEWN.ValidValues.Add("3", "3");
            cbEWN.ValidValues.Add("4", "4");

            cbHH.Select("12", BoSearchKey.psk_ByValue);
            cbMM.Select(0, BoSearchKey.psk_Index);
            cbAP.Select(0, BoSearchKey.psk_Index);

            cbOTHH.Select("12", BoSearchKey.psk_ByValue);
            cbOTMM.Select(0, BoSearchKey.psk_Index);
            cbOTAP.Select(0, BoSearchKey.psk_Index);
            

            cbEWN.Select(0, BoSearchKey.psk_Index);


            System.Data.DataTable dtSP = Program.objHrmsUI.getDataTable("Select slpcode,slpname from oslp", "FillSP");

            foreach (System.Data.DataRow dr in dtSP.Rows)
            {
                cbSP.ValidValues.Add(dr["slpcode"].ToString(), dr["slpname"].ToString());
            }

            cbSP.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);


            cbDays.ValidValues.Add("Mon", "Monday");
            cbDays.ValidValues.Add("Tue", "Tuesday");
            cbDays.ValidValues.Add("Wed", "Wednesday");
            cbDays.ValidValues.Add("Thu", "Thursday");
            cbDays.ValidValues.Add("Fri", "Friday");
            cbDays.ValidValues.Add("Sat", "Saturday");
            cbDays.ValidValues.Add("Sun", "Sunday");



            cbWeeks.ValidValues.Add("1", "Week 1");
            cbWeeks.ValidValues.Add("2", "Week 2");
            cbWeeks.ValidValues.Add("3", "Week 3");
            cbWeeks.ValidValues.Add("4", "Week 4");
            cbWeeks.ValidValues.Add("5", "Week 5");



           SAPbouiCOM.Column mtColDay = mtDay.Columns.Item("colDay");
           mtColDay.ValidValues.Add("Mon", "Monday");
           mtColDay.ValidValues.Add("Tue", "Tuesday");
           mtColDay.ValidValues.Add("Wed", "Wednesday");
           mtColDay.ValidValues.Add("Thu", "Thursday");
           mtColDay.ValidValues.Add("Fri", "Friday");
           mtColDay.ValidValues.Add("Sat", "Saturday");
           mtColDay.ValidValues.Add("Sun", "Sunday");

           SAPbouiCOM.Column mtColWeek = mtDay.Columns.Item("colWeek");
           mtColWeek.ValidValues.Add("1", "Week 1");
           mtColWeek.ValidValues.Add("2", "Week 2");
           mtColWeek.ValidValues.Add("3", "Week 3");
           mtColWeek.ValidValues.Add("4", "Week 4");
           mtColWeek.ValidValues.Add("5", "Week 5");







        }

        private void AddNewCardSche(string cardCode)
        {
            string strExisting = @"SELECT        TOP (200) Code, Name, U_SchType, U_Active, U_Intrvl, U_W1, U_W2, U_W3, U_W4, U_W5, U_W6, U_W7, U_CallTime
                                        FROM            [@B1_CRDSCH] ";
            strExisting += "Where Code='TS_" + cardCode + "'";

            System.Data.DataTable dtSchedule = Program.objHrmsUI.getDataTable(strExisting, "getting schedule");
            if (dtSchedule.Rows.Count == 0)
            {
                string strInsert = " insert into  [@B1_CRDSCH] (Code, Name, U_SchType, U_Active, U_Intrvl, U_W1, U_W2, U_W3, U_W4, U_W5, U_W6, U_W7, U_CallTime) ";
                strInsert += " Values ('TS_" + cardCode + "','TS_" + cardCode + "','T','N','D','N','N','N','N','N','N','N','0')";
                Program.objHrmsUI.ExecQuery(strInsert, "Adding TS Schedule");

            }

            strExisting = @"SELECT        TOP (200) Code, Name, U_SchType, U_Active, U_Intrvl, U_W1, U_W2, U_W3, U_W4, U_W5, U_W6, U_W7, U_CallTime
                                        FROM            [@B1_CRDSCH] ";
            strExisting += "Where Code='SO_" + cardCode + "'";

            dtSchedule = Program.objHrmsUI.getDataTable(strExisting, "getting schedule");
            if (dtSchedule.Rows.Count == 0)
            {
                string strInsert = " insert into  [@B1_CRDSCH] (Code, Name, U_SchType, U_Active, U_Intrvl, U_W1, U_W2, U_W3, U_W4, U_W5, U_W6, U_W7, U_CallTime) ";
                strInsert += " Values ('SO_" + cardCode + "','SO_" + cardCode + "','O','N','D','N','N','N','N','N','N','N','0')";
                Program.objHrmsUI.ExecQuery(strInsert, "Adding TS Schedule");

            }


        }

        private void getOSCN()
        {
            string cardCode = Convert.ToString(dtHead.GetValue("CardCode", 0));
            if (cardCode == "") return;
            dtOSCN.Rows.Clear();
            string strGet = "select  oitm.ItemCode,oscn.CardCode,Substitute, oitm.ItemName from OSCN inner join oitm on oitm.ItemCode = oscn.ItemCode where OSCN.CardCode = '" + cardCode + "'";
            System.Data.DataTable dtDates = Program.objHrmsUI.getDataTable(strGet, "Loading OSCN");

            int i = 0;
            foreach (System.Data.DataRow dr in dtDates.Rows)
            {
                dtOSCN.Rows.Add(1);

                dtOSCN.SetValue("Id", i, (i + 1).ToString());
                dtOSCN.SetValue("ItemCode", i, dr["ItemCode"].ToString());
                dtOSCN.SetValue("ItemName", i, dr["ItemName"].ToString());
                dtOSCN.SetValue("CAT", i, dr["Substitute"].ToString());
               
                i++;
            }
            mtOSCN.LoadFromDataSource();


        }
 
        private void updateSchedule()
        {

            string cardCode = Convert.ToString(dtHead.GetValue("CardCode", 0));

            if (cardCode == "")
            {
                oApplication.MessageBox("Please select a customer to update a schedule");
                return;
            }

            string Code, schName, chActive, SchInterval, WD1, WD2, WD3, WD4, WD5, WD6, WD7, CallTime, MOType = "", EWN;
           DateTime     EWD;

            string slpCode = "";
          

            if (currentTab == "TS")
            {
                Code = "TS_" + cardCode;
                slpCode = cbSP.Value.ToString();
           
            }

            else
            {
                Code = Code = "SO_" + cardCode;
                
            }

            schName = Code;
            chActive = Convert.ToString(dtHead.GetValue("tsActive", 0));
            SchInterval = "D";
            if (opOD.Selected) SchInterval = "D";
            if (opOW.Selected) SchInterval = "W";
            if (opOM.Selected) SchInterval = "M";
           

            MOType = "DT";

            if (OMDY.Selected) MOType = "DY";


            WD1 = Convert.ToString(dtHead.GetValue("chMon", 0));
            WD2 = Convert.ToString(dtHead.GetValue("chTue", 0));
            WD3 = Convert.ToString(dtHead.GetValue("chWed", 0));
            WD4 = Convert.ToString(dtHead.GetValue("chThu", 0));
            WD5 = Convert.ToString(dtHead.GetValue("chFri", 0));
            WD6 = Convert.ToString(dtHead.GetValue("chSat", 0));
            WD7 = Convert.ToString(dtHead.GetValue("chSun", 0));
            CallTime = Program.objHrmsUI.getIntTime( Convert.ToString(  dtHead.GetValue("tsCT", 0))).ToString();
            EWN = Convert.ToString(dtHead.GetValue("tsEWN", 0));
            EWD = Convert.ToDateTime(dtHead.GetValue("tsEWD", 0));


            string updateSch = "Update [@B1_CRDSCH] set U_Active='" + chActive + "',U_Intrvl='" + SchInterval + "',U_W1='" + WD1 + "',U_W2 ='" + WD2
                + "',U_W3='" + WD3 + "',U_W4='" + WD4 + "',U_W5='" + WD5 + "',U_W6='" + WD6 + "',U_W7='" + WD7 + "',U_CallTime='" + CallTime + "' , U_slpCode = '" + slpCode
                + "' , U_MOType='" + MOType + "' , u_EWN = '" + EWN + "', u_EWD='" + EWD.ToString("yyyyMMdd") + "'  where Code='" + Code + "'";

            int result = Program.objHrmsUI.ExecQuery(updateSch, "Updating Schedule");

            string deleteCalls = " Delete from   [@B1_SCHCALL] where  U_SCCode = '" + Code + "' and U_Status='Open' and U_DATE >='" + EWD.ToString("yyyyMMdd") + "'";
            result = Program.objHrmsUI.ExecQuery(deleteCalls, "Delete Calls");



            Program.objHrmsUI.ExecQuery("update [@B1_SCHMDT] set u_upd='Y' where isnull(u_upd,'N') = 'N'", "Finalizing temp");
            Program.objHrmsUI.ExecQuery("update [@B1_SCHMDY]  set u_upd='Y' where isnull(u_upd,'N') = 'N'", "Finalizing temp");
            Program.objHrmsUI.ExecQuery("update [@B1_SO]  set u_upd='Y' where isnull(u_upd,'N') = 'N'", "Finalizing temp");
            Program.objHrmsUI.ExecQuery("update  [@B1_SCHOT]  set u_upd='Y' where isnull(u_upd,'N') = 'N'", "Finalizing temp");


        }
        private void getSchedule()
        {
            string cardCode = Convert.ToString(dtHead.GetValue("CardCode", 0));
            string Code = "";
            getMoDates();
            getMoDays();
            getOSCN();
            getOTDates();
            if (currentTab == "TS")
            {
                Code = "TS_" + cardCode;
               
            }

            else
            {
                Code = Code = "SO_" + cardCode;
                getSOItems();


            }

            string strGet = "Select Code, Name, U_SchType, U_Active, U_slpCode, U_Intrvl, U_W1, U_W2, U_W3, U_W4, U_W5, U_W6, U_W7, U_CallTime, U_MOType, isnull(U_EWN,1) U_EWN , isnull(U_EWD,getdate()) as U_EWD  from [@B1_CRDSCH] where [Code]='" + Code + "'";
            System.Data.DataTable dtSch = Program.objHrmsUI.getDataTable(strGet, "Loading Data");

            foreach (System.Data.DataRow dr in dtSch.Rows)
            {
               string slpCode =  dr["U_slpCode"].ToString();
                dtHead.SetValue("tsActive", 0, dr["U_Active"].ToString());
                string strTime = Program.objHrmsUI.getStrTime(Convert.ToInt32(dr["U_CallTime"]));

                string strHH = strTime.Substring(0, 2);
                string strMM = strTime.Substring(3, 2);
                string strAP = strTime.Substring(6, 2);
                
                if (strHH == "00") cbHH.Select("12", BoSearchKey.psk_ByValue); else cbHH.Select(strHH, BoSearchKey.psk_ByValue);
                cbMM.Select(strMM, BoSearchKey.psk_ByValue);
                cbAP.Select(strAP, BoSearchKey.psk_ByValue);
                
                dtHead.SetValue("tsCT", 0, strTime);

                string SchInterval = dr["U_Intrvl"].ToString();
                string MoType = dr["U_MOType"].ToString();
                try
                {
                    if (SchInterval == "D") dtHead.SetValue("tsOD", 0, "1");
                    if (SchInterval == "W") dtHead.SetValue("tsOD", 0, "2");
                    if (SchInterval == "M") dtHead.SetValue("tsOD", 0, "3");
                    if (MoType == "DT") dtHead.SetValue("OMDT", 0, "1");
                    if (MoType == "DY") dtHead.SetValue("OMDT", 0, "2");


                    if (slpCode != "")
                    {
                        cbSP.Select(slpCode, SAPbouiCOM.BoSearchKey.psk_ByValue);
                    }
                }
                catch(Exception ex)
                {
                    //oApplication.SetStatusBarMessage(ex.Message);
                }


                dtHead.SetValue("chMon", 0, dr["U_W1"].ToString());
                dtHead.SetValue("chTue", 0, dr["U_W2"].ToString());
                dtHead.SetValue("chWed", 0, dr["U_W3"].ToString());
                dtHead.SetValue("chThu", 0, dr["U_W4"].ToString());
                dtHead.SetValue("chFri", 0, dr["U_W5"].ToString());
                dtHead.SetValue("chSat", 0, dr["U_W6"].ToString());
                dtHead.SetValue("chSun", 0, dr["U_W7"].ToString());

                DateTime EWD = Convert.ToDateTime( dr["U_EWD"]);
                dtHead.SetValue("tsEWN", 0, dr["U_EWN"].ToString());
                dtHead.SetValue("tsEWD", 0,EWD.ToString("yyyyMMdd"));

            }
               

        }


        private void getSOItems()
        {
            dtItem.Rows.Clear();
            string cardCode = Convert.ToString(dtHead.GetValue("CardCode", 0));
            string Code = "";

            Code = Code = "SO_" + cardCode;




            string strGet = "SELECT        Code, Name, U_SCCode, U_ItemCode, U_Qty , oitm.itemname FROM   [@B1_SO] inner join oitm on oitm.itemcode = u_itemcode where   isnull(u_upd,'N') = 'Y'  and [U_SCCode]='" + Code + "'";
            System.Data.DataTable dtSch = Program.objHrmsUI.getDataTable(strGet, "Loading Data");

            int i = 0;
            foreach (System.Data.DataRow dr in dtSch.Rows)
            {
                dtItem.Rows.Add(1);

                dtItem.SetValue("Id", i, dr["Code"].ToString());
                dtItem.SetValue("ItemCode", i, dr["U_ItemCode"].ToString());
                dtItem.SetValue("ItemName", i, dr["ItemName"].ToString());

                dtItem.SetValue("Qty", i, dr["U_Qty"].ToString());

                i++;
            }
            mtItem.LoadFromDataSource();


        }


        private void getMoDates()
        {
            dtDate.Rows.Clear();
            string cardCode = Convert.ToString(dtHead.GetValue("CardCode", 0));
            string Code = "";

            if (currentTab == "TS")
            {
                Code = "TS_" + cardCode;

            }

            else
            {
                Code = "SO_" + cardCode;

            }




            string strGet = "SELECT        Code, Name, U_SCCode, U_Mdates FROM  [@B1_SCHMDT]  where  isnull(u_upd,'N') = 'Y'  and  [U_SCCode]='" + Code + "'";
            System.Data.DataTable dtDates = Program.objHrmsUI.getDataTable(strGet, "Loading Data");

            int i = 0;
            foreach (System.Data.DataRow dr in dtDates.Rows)
            {
                dtDate.Rows.Add(1);

                dtDate.SetValue("Id", i, dr["Code"].ToString());
                dtDate.SetValue("Date", i, dr["U_Mdates"].ToString());
                  i++;
            }
            mtDate.LoadFromDataSource();

        }



        private void getOTDates()
        {
            dtOT.Rows.Clear();
            string cardCode = Convert.ToString(dtHead.GetValue("CardCode", 0));
            string Code = "";

            if (currentTab == "TS")
            {
                Code = "TS_" + cardCode;

            }

            else
            {
                Code = "SO_" + cardCode;

            }




            string strGet = "SELECT       Code, Name, U_SCCode, U_Date, U_Time FROM   [@B1_SCHOT]  where  isnull(u_upd,'N') = 'Y'  and   [U_SCCode]='" + Code + "'";
            System.Data.DataTable dtDates = Program.objHrmsUI.getDataTable(strGet, "Loading Data");

            int i = 0;
            foreach (System.Data.DataRow dr in dtDates.Rows)
            {
                dtOT.Rows.Add(1);

                dtOT.SetValue("Id", i,  dr["Code"].ToString());
                dtOT.SetValue("Date", i, Convert.ToDateTime( dr["U_Date"]));
                dtOT.SetValue("Time", i, Program.objHrmsUI.getStrTime( Convert.ToInt32( dr["U_Time"])));
                i++;
            }
            mtOT.LoadFromDataSource();

        }


        private void getMoDays()
        {
            dtDays.Rows.Clear();
            string cardCode = Convert.ToString(dtHead.GetValue("CardCode", 0));
            string Code = "";

            if (currentTab == "TS")
            {
                Code = "TS_" + cardCode;

            }

            else
            {
                Code = "SO_" + cardCode;

            }




            string strGet = "SELECT        Code, Name, U_SCCode, U_Day, U_WeekNum FROM   [@B1_SCHMDY]	  where  isnull(u_upd,'N') = 'Y'  and  [U_SCCode]='" + Code + "'";
            System.Data.DataTable dtDates = Program.objHrmsUI.getDataTable(strGet, "Loading Data");

            int i = 0;
            foreach (System.Data.DataRow dr in dtDates.Rows)
            {
                dtDays.Rows.Add(1);

                dtDays.SetValue("Id", i, dr["Code"].ToString());
                dtDays.SetValue("Day", i, dr["U_Day"].ToString());
                dtDays.SetValue("Week", i, dr["U_WeekNum"].ToString());
                i++;
            }
            mtDay.LoadFromDataSource();

        }

        private void addAttacment()
        {

            string strFileName = Program.objHrmsUI.getFileName2("jpg");
            if (strFileName == "") return;
            SAPbobsCOM.Attachments2 oAtt;

            oAtt =( SAPbobsCOM.Attachments2) oCompany.GetBusinessObject( BoObjectTypes.oAttachments2);
            string sSourcePath = "";
            string sFile = "";
            string sExt = "";
            splitPath(strFileName, out sFile, out sExt, out sSourcePath);
            oAtt.Lines.SourcePath = sSourcePath;
            oAtt.Lines.FileName = sFile;
            oAtt.Lines.FileExtension = sExt;
            oAtt.Lines.Override = BoYesNoEnum.tYES;
            int iRet = oAtt.Add();//if iRet = 0 SAP sucessfully added the attachment data to the DB
            if (iRet != 0)
            {
                int erroCode = 0;
                string errDescr = "";
                Program.objHrmsUI.oCompany.GetLastError(out erroCode, out errDescr);
                oApplication.StatusBar.SetText("Failed to attac file  : " + errDescr);

            }
            else
            {

                string strPath = oCompany.AttachMentPath.ToString() + "\\" + sFile + "." + sExt;
                dtHead.SetValue("Img", 0, strPath);

             //   imgImage.Picture = strPath;

            }
        }

        private void splitPath(string path, out string file, out string ext, out string folder)
        {
            file = "";
            ext = "";
            folder = "";
            string revPath = Reverse(path);

            string[] part = revPath.Split('\\');
            file = part[0].ToString();
            if (file != "")
            {
                folder = revPath.Replace(file, "");

                file = Reverse(file);
                folder = Reverse(folder);

                if (file.Contains("."))
                {
                    string[] filePart = file.Split('.');
                    file = filePart[0];
                    ext = filePart[1];
                }

            }

        }
        public string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private void updateOSCN()
        {

            int selRow = mtSelRow(mtOSCN);

            if (selRow > 0)
            {
                string cardCode = dtHead.GetValue("CardCode", 0).ToString();
                string ItemCode = dtOSCN.GetValue("ItemCode", selRow - 1).ToString() ;
                string Substitute = dtOSCN.GetValue("CAT", selRow - 1).ToString();
                string cutSpec = dtHead.GetValue("OSCNCS", 0).ToString();
                string PackSpec = dtHead.GetValue("OSCNPS", 0).ToString();
                string Chepec = dtHead.GetValue("OSCNCHS", 0).ToString();
                string img = dtHead.GetValue("Img", 0).ToString();
                string SL = dtHead.GetValue("SL", 0).ToString();


                string sSourcePath = "";
                string sFile = "";
                string sExt = "";
              

                SAPbobsCOM.AlternateCatNum catNum = (SAPbobsCOM.AlternateCatNum)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oAlternateCatNum);
                catNum.GetByKey(ItemCode, cardCode, Substitute);

                catNum.UserFields.Fields.Item("U_PecSpec").Value = PackSpec;
                catNum.UserFields.Fields.Item("U_CutSpec").Value = cutSpec;
                catNum.UserFields.Fields.Item("U_CheSpec").Value = Chepec;
                catNum.UserFields.Fields.Item("U_SL").Value = Convert.ToInt32( SL);


                if (img != "")
                {
                    splitPath(img, out sFile, out sExt, out sSourcePath);
                    catNum.UserFields.Fields.Item("U_Img").Value = sFile + "." + sExt;
                }
                catNum.Update();
                oApplication.SetStatusBarMessage("Catalog Updated Successfully", SAPbouiCOM.BoMessageTime.bmt_Short, false);
                 
            }
            else
            {
                oApplication.MessageBox("Please select an item");
            }
        }

        private void getOSCNDetail()
        {
            int selRow = mtSelRow(mtOSCN);

            if (selRow > 0)
            {
                string cardCode = dtHead.GetValue("CardCode", 0).ToString();
                string ItemCode = dtOSCN.GetValue("ItemCode", selRow - 1).ToString();
                string Substitute = dtOSCN.GetValue("CAT", selRow - 1).ToString();

               
                SAPbobsCOM.AlternateCatNum catNum = (SAPbobsCOM.AlternateCatNum)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oAlternateCatNum);
                catNum.GetByKey(ItemCode, cardCode, Substitute);


                dtHead.SetValue("OSCNCS", 0, catNum.UserFields.Fields.Item("U_CutSpec").Value);
                dtHead.SetValue("OSCNPS", 0, catNum.UserFields.Fields.Item("U_PecSpec").Value);
                dtHead.SetValue("OSCNCHS", 0, catNum.UserFields.Fields.Item("U_CheSpec").Value);
                dtHead.SetValue("SL", 0, catNum.UserFields.Fields.Item("U_SL").Value);
              
                dtHead.SetValue("Img", 0, oCompany.AttachMentPath.ToString() + "\\" + catNum.UserFields.Fields.Item("U_Img").Value);



            }
            else
            {
                oApplication.MessageBox("Please select an item");
            }
        }
        private void setCallTime()
        {
            dtHead.SetValue("tsCT", 0, cbHH.Value + ":" + cbMM.Value  + " " + cbAP.Value );
          
        }
       
    }
}
