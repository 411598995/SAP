using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACHR.Screen
{
    class frm_MRPD : HRMSBaseForm
    {

        public bool isForLoading = false;
        SAPbouiCOM.Folder tbTS, tbSO;
        private int newPOEntry = -1;

        SAPbouiCOM.Matrix mtAnimals, mtRpt, mtProd, mtReqT, mtTypePRO;
        SAPbouiCOM.ComboBox cbSP, cbDays, cbWeeks;
        SAPbouiCOM.EditText   txQtyT,txSupplier;
        SAPbouiCOM.ComboBox cbAT,cbAC;
        SAPbouiCOM.Folder tab1, tab2, tab3,tab4,tab5;
        SAPbouiCOM.CheckBox chSO, chPRO;
        SAPbouiCOM.ChooseFromList ocflCard;
        Hashtable ItemType = new Hashtable();
        SAPbouiCOM.DataTable dtHead, Animals, DTRpt, dtPro, dtReqT, dtTypeS;


        public override void CreateForm(SAPbouiCOM.Application SboApp, string strXml, SAPbobsCOM.Company cmp, string frmId)
        {
            base.CreateForm(SboApp, strXml, cmp, frmId);

            
            InitiallizeForm();

        }

        public override void etAfterClick(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID.Contains("mt"))
            {
                try
                {
                    if (chSO.Checked)
                    {
                        oForm.Settings.MatrixUID = pVal.ItemUID;
                        oForm.Settings.Enabled = true;
                    }
                }
                catch { }
            }
            if (pVal.ItemUID == "1")
            {

            }
            if (pVal.ItemUID == "43")
            {
                RefreshForm();
            }
            if (pVal.ItemUID == "btCreate")
            {
                oApplication.MessageBox("Creating Sp ");

               
                createProductionOrder();
              
                RefreshForm();
               
               
            }
            if (pVal.ItemUID == "34")
            {


                int poResult = createTypePurchaseOrder();
                if (poResult != 0)
                {
                    int createProduction = oApplication.MessageBox("PO not created for a supplier. Do you still want to add production order?(Y/N)", 2, "Yes", "No");
                    if (createProduction == 2) return;

                }
                createTypeProductionOrder();
                // createSPProductionOrder();
                //  createProductionOrder();
                dtHead.SetValue("TodayQty", 0, "0");
                fillAnimals();
                getReport();
                fillPO();
                oApplication.StatusBar.SetText("Operation Completed Successfully!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);

            }
            if (pVal.ItemUID == "btCalcT")
            {

                CalcToday();
            }
         
        }
        public override void etAfterCfl(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCfl(ref pVal, ref BubbleEvent);
            SAPbouiCOM.IChooseFromListEvent oCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pVal;
            SAPbouiCOM.DataTable dtSel = oCFLEvento.SelectedObjects;
            if (pVal.ItemUID == txSupplier.Item.UniqueID)
            {
                if (dtSel != null && dtSel.Rows.Count > 0)
                {
                    string strCode = dtSel.GetValue("CardCode", 0).ToString();
                    string strName = dtSel.GetValue("CardName", 0).ToString();
                    dtHead.SetValue("CardCode", 0, strCode);
                    dtHead.SetValue("CardName", 0, strName);
                   

                }
            }

        }
        public override void etAfterCmbSelect(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCmbSelect(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == cbAT.Item.UniqueID)
            {
               // RefreshForm();
            

            }
            if (pVal.ItemUID == cbAC.Item.UniqueID)
            {
                fillAT(cbAC.Selected.Value.ToString());


            }
            if (cbAT.Value == "")
            {
                dtHead.SetValue("TodayQty", 0, 0);
                   
                txQtyT.Item.Enabled = false;
            }
            else
            {
                txQtyT.Item.Enabled = true;
            }
        }
        public override void etAfterValidate(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterValidate(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID  == "29")
            {
               // RefreshForm();
                fillAnimals();
                CalcToday();
            }

            if (pVal.ItemUID == "txDD")
            {
                dtHead.SetValue("PoDate", 0, Convert.ToDateTime(dtHead.GetValue("DocDate", 0)));

                RefreshForm();
            }
        }

        private void RefreshForm()
        {
            if (!isForLoading)
            {
                fillPO();
              
                fillAnimals();
                fillRequiredT();
           
               CalcToday();
               
            }
         
        }

    

        private void CalcToday()
        {
            mtAnimals.FlushToDataSource();
            double TodayTotal = Convert.ToDouble(dtHead.GetValue("TodayQty", 0));
           


            mtAnimals.LoadFromDataSource();
            getReport();
        }
      
        
        
        private void InitiallizeForm()
        {


            isForLoading = true;
            oForm.Freeze(true);

            tab1 = (SAPbouiCOM.Folder)oForm.Items.Item("21").Specific;
            tab3 = (SAPbouiCOM.Folder)oForm.Items.Item("19").Specific;
            tab4 = (SAPbouiCOM.Folder)oForm.Items.Item("35").Specific;
           
            dtHead = oForm.DataSources.DataTables.Item("dtHead");
            Animals = oForm.DataSources.DataTables.Item("Animals");
            DTRpt = oForm.DataSources.DataTables.Item("DTRpt");
            dtPro = oForm.DataSources.DataTables.Item("dtPro");
            dtReqT = oForm.DataSources.DataTables.Item("dtReqT");
            dtTypeS = oForm.DataSources.DataTables.Item("dtTypeS");
        

            ocflCard = (SAPbouiCOM.ChooseFromList)oForm.ChooseFromLists.Item("cflCard");
            cflcardcode(ocflCard, "cflCard");

                 
            mtAnimals = (SAPbouiCOM.Matrix)oForm.Items.Item("mtAnimals").Specific;
            mtReqT = (SAPbouiCOM.Matrix)oForm.Items.Item("mtReqT").Specific;
          
            mtRpt = (SAPbouiCOM.Matrix)oForm.Items.Item("mtRpt").Specific;
            mtProd = (SAPbouiCOM.Matrix)oForm.Items.Item("mtProd").Specific;
            mtTypePRO = (SAPbouiCOM.Matrix)oForm.Items.Item("mtTypePRO").Specific;


            cbAT = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbAT").Specific;
            cbAC = (SAPbouiCOM.ComboBox)oForm.Items.Item("cbAC").Specific;


            txQtyT = (SAPbouiCOM.EditText)oForm.Items.Item("29").Specific;
                  txSupplier = (SAPbouiCOM.EditText)oForm.Items.Item("47").Specific;

            chSO = (SAPbouiCOM.CheckBox)oForm.Items.Item("chSO").Specific;
            chPRO = (SAPbouiCOM.CheckBox)oForm.Items.Item("crRPRO").Specific;

            dtHead.Rows.Add(1);
            dtHead.SetValue("DocDate", 0, DateTime.Now);
            dtHead.SetValue("PoDate", 0, DateTime.Now);

            dtHead.SetValue("stDate", 0, DateTime.Now.AddDays(1));
            dtHead.SetValue("eDate", 0, DateTime.Now.AddDays(4));
            oForm.Freeze(false);
           fillCB();

           txQtyT.Item.Enabled = false;
            oForm.PaneLevel =4;
            tab4.Select();
            isForLoading = false;
            fillPO();
            mtProd.Item.Visible = false;
            fillAThash();
            RefreshForm();
        }



        private void fillCB()
        {
            cbAT.ValidValues.Add(" ", "[Select One]");
            cbAC.ValidValues.Add(" ", "[Select One]");
    
            string animalType = "Select * from [@DEM_AnimalType] where isnull(u_isCat,'N') = 'Y'";
            System.Data.DataTable dtTypes = Program.objHrmsUI.getDataTable(animalType, "Animal Types");
            int i = 0;
            foreach (System.Data.DataRow dr in dtTypes.Rows)
            {
                cbAC.ValidValues.Add(dr["Code"].ToString(), dr["Name"].ToString());
                i++;
            }
            try
            {
                if (i > 0) cbAC.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
            }
            catch { }
           
        }

        private void fillAT(string Cat)
        {
            //ItemType.Clear();
            string animalType = "Select OITM.ItemCode,OITM.ItemName from OITM Inner Join oitt on oitt.Code = oitm.ItemCode where isnull(OITM.U_AnimalType,'N') = '" + Cat + "'";
            System.Data.DataTable dtTypes = Program.objHrmsUI.getDataTable(animalType, "Animal Types");
            int i = 0;
            int cbCnt = cbAT.ValidValues.Count;
            if (cbCnt > 1)
            {
                for (int k = 1; k < cbCnt; k++)
                {
                    cbAT.ValidValues.Remove(1, SAPbouiCOM.BoSearchKey.psk_Index);
                }
            }
            foreach (System.Data.DataRow dr in dtTypes.Rows)
            {
               // ItemType.Add(dr["ItemCode"].ToString(), dr["ItemName"].ToString());
                cbAT.ValidValues.Add(dr["ItemCode"].ToString(), dr["ItemName"].ToString());
                i++;
            }
            try
            {
                if (i > 0) cbAT.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
            }
            catch { }
        }

        private void fillAThash()
        {
            ItemType.Clear();
            string animalType = "Select OITM.ItemCode,OITM.ItemName from OITM Inner Join oitt on oitt.Code = oitm.ItemCode where isnull(OITM.U_AnimalType,'N') <> ''";
            System.Data.DataTable dtTypes = Program.objHrmsUI.getDataTable(animalType, "Animal Types");
            foreach (System.Data.DataRow dr in dtTypes.Rows)
            {
                ItemType.Add(dr["ItemCode"].ToString(), dr["ItemName"].ToString());

            }

        }
        private void updateMrpPer(string itemCode, double per)
        {
            string strUPdate = "Update oitm set U_MRPPer='" + per.ToString() + "' where itemcode = '" + itemCode + "'";
            Program.objHrmsUI.ExecQuery(strUPdate, "Set Percentage");
        }


       private void fillAnimals()
        {
            Animals.Rows.Clear();

            //DateTime dtToday = Convert.ToDateTime(dtHead.GetValue("DocDate", 0));

            string GradeCodes = "";
            //string strScheduled = "Select distinct itemcode from owor where DueDate  between '" + dtToday.ToString("yyyyMMdd") + "' and '" + dtToday.ToString("yyyyMMdd") + "' and OWOR.Status in ('R','P') ";
          
            //System.Data.DataTable dtOwor = Program.objHrmsUI.getDataTable(strScheduled,"POs");
           
           for (int k = 0; k < dtTypeS.Rows.Count; k++)
            {
                GradeCodes += GradeCodes == "" ?  "'" + Convert.ToString(dtTypeS.GetValue("ItemCode", k)) +"'"  : ", '" + Convert.ToString(dtTypeS.GetValue("ItemCode", k)) + "'";
           }

          
           string selItemCode = cbAT.Value.ToString();
           double qtyToAdd = Convert.ToDouble(dtHead.GetValue("TodayQty", 0));
              
           if (GradeCodes != "")
           {



               string animals = @"Select distinct ItemCode,ItemName , oitm.onHand 
                            from OITM inner join ITT1 on ITT1.code = oitm.itemcode inner join oitt on itt1.father = oitt.Code inner join oitt bom2 on bom2.Code = itt1.Code  where ITT1.FATHER in ( " + GradeCodes + @")";

               System.Data.DataTable dtAnimals = Program.objHrmsUI.getDataTable(animals, "Animal Types");
               int i = 0;
               DateTime dtToday = Convert.ToDateTime(dtHead.GetValue("DocDate", 0));

               dtToday = Convert.ToDateTime(dtHead.GetValue("DocDate", 0));
               DateTime dtFrom = Convert.ToDateTime(dtHead.GetValue("DocDate", 0));
               DateTime dtTo = Convert.ToDateTime(dtHead.GetValue("DocDate", 0));


               foreach (System.Data.DataRow dr in dtAnimals.Rows)
               {
                   Animals.Rows.Add(1);
                   Animals.SetValue("Id", i, (i + 1).ToString());
                   Animals.SetValue("ItemCode", i, dr["ItemCode"].ToString());
                   Animals.SetValue("ItemName", i, dr["ItemName"].ToString());
                   double toProduce = scheduledQty(dtToday, dtToday, dr["ItemCode"].ToString());

                  
                   Animals.SetValue("QtySche", i, toProduce);
               
                   double onHand = 0.00; ;
                   double IsCommited = 0.00;
                   double foreCasted = 0.00;
                   for (int n = 0; n < dtReqT.Rows.Count; n++)
                   {
                       string ReqItemCode = dtReqT.GetValue("ItemCode", n).ToString();
                       if (dr["ItemCode"].ToString() == ReqItemCode)
                       {
                           onHand = Convert.ToDouble(dtReqT.GetValue("onHand", n));
                           IsCommited = Convert.ToDouble(dtReqT.GetValue("Orderd", n));
                           foreCasted = Convert.ToDouble(dtReqT.GetValue("Forecast", n));
                       }
                   }

                   Animals.SetValue("Stock", i, onHand.ToString());
                   Animals.SetValue("Ordered", i, IsCommited.ToString());
                   Animals.SetValue("Forecast", i, foreCasted.ToString());
                   Animals.SetValue("ATP", i, (Math.Round(toProduce + onHand - IsCommited - foreCasted, 3)).ToString());




                   i++;
               }
           }

           if (selItemCode != "" && qtyToAdd>0)
           {
               string animalsToAdd = @"Select distinct ItemCode,ItemName ,itt1.Quantity , oitt.Qauntity as FQty 
                            from OITM inner join ITT1 on ITT1.code = oitm.itemcode inner join oitt on itt1.father = oitt.Code inner join oitt bom2 on bom2.Code = itt1.Code  where ITT1.FATHER = '" + selItemCode + @"'";

               System.Data.DataTable dtAnimalstoAdd = Program.objHrmsUI.getDataTable(animalsToAdd, "Animal Types");
               foreach (System.Data.DataRow dr in dtAnimalstoAdd.Rows)
               {
                   bool itemFound = false;
              
                   string strItemCode = dr["ItemCode"].ToString();
                   double childqty = Convert.ToDouble(dr["Quantity"]);
                   double fatherqty = Convert.ToDouble(dr["FQty"]);
                   double toChildAddQty = fatherqty > 0 ? childqty / fatherqty * qtyToAdd : 0;
                   for (int p = 0; p < Animals.Rows.Count; p++)
                   {
                       string grItem = Convert.ToString( Animals.GetValue("ItemCode", p));
                       if (grItem == strItemCode)
                       {
                           itemFound = true;
                           double oldToProduc = Convert.ToDouble(Animals.GetValue("QtySche", p));
                           Animals.SetValue("QtySche", p, oldToProduc + toChildAddQty);
                           Animals.SetValue("Ordered", p, "0");
                           Animals.SetValue("Stock", p, "0");
                           Animals.SetValue("ATP", p, "0");
                       }
                           
                   }
                   if (!itemFound)
                   {
                      
                       Animals.Rows.Add(1);
                       int rown = Animals.Rows.Count-1;

                       Animals.SetValue("Id", rown, (rown + 1).ToString());
                       Animals.SetValue("ItemCode", rown, dr["ItemCode"].ToString());
                       Animals.SetValue("ItemName", rown, dr["ItemName"].ToString());
                      

                       Animals.SetValue("QtySche", rown, toChildAddQty);
                       Animals.SetValue("Ordered",rown, "0");
                       Animals.SetValue("Stock",rown, "0");
                       Animals.SetValue("ATP", rown, "0");

                   }


               }

           }

           for (int k = 0; k < Animals.Rows.Count; k++)
           {
               string WIPitemCode = Convert.ToString(Animals.GetValue("ItemCode", k));
               double QtyScheduled = Convert.ToDouble(Animals.GetValue("QtySche", k));
                    

               for (int ki = 0; ki < dtReqT.Rows.Count; ki++)
               {
                   string reqItemCode = Convert.ToString(dtReqT.GetValue("ItemCode", ki));
                   if (WIPitemCode == reqItemCode)
                   {
                       double qtyOrdered = Convert.ToDouble(dtReqT.GetValue("Orderd", ki));
                       double qtyStock = Convert.ToDouble(dtReqT.GetValue("onHand", ki));
                       double qtyForecast = Convert.ToDouble(dtReqT.GetValue("Forecast", ki));

                       Animals.SetValue("Ordered", k, qtyOrdered.ToString());
                       Animals.SetValue("Stock", k, qtyStock.ToString());
                       Animals.SetValue("ATP", k, QtyScheduled + qtyStock - qtyOrdered - qtyForecast);
                   }
               }
           }
            mtAnimals.LoadFromDataSource();
         
           //// WIP Matrix 
        }
   
       
        private void fillRequiredT()
        {
            dtReqT.Rows.Clear();
            DateTime dt = Convert.ToDateTime(dtHead.GetValue("DocDate", 0));
            string animals = "Select rdr1.ItemCode,rdr1.Dscription as ItemName ,SUM(rdr1.OpenQty * NumPerMsr)  as OpenQty , Min(oitm.onhand) as onhand   from ORDR inner join rdr1 on rdr1.docEntry = ORDR.DocEntry inner join oitm on oitm.itemcode = rdr1.itemcode   where rdr1.openQty>0 and  ordr.docduedate = '" + dt.Date.ToString("yyyyMMdd") + "'";
            animals += " Group By rdr1.ItemCode,rdr1.Dscription ";
            System.Data.DataTable dtRequired = Program.objHrmsUI.getDataTable(animals, "Required Today");
            int i = 0;
            DateTime dtToday = dt.Date;
            DateTime dtFrom = dt.Date;
            DateTime dtTo = dt.Date;

            foreach (System.Data.DataRow dr in dtRequired.Rows)
            {
                dtReqT.Rows.Add(1);
                dtReqT.SetValue("Id", i, (i + 1).ToString());
                dtReqT.SetValue("ItemCode", i, dr["ItemCode"].ToString());
                dtReqT.SetValue("ItemName", i, dr["ItemName"].ToString());
                dtReqT.SetValue("onHand", i, dr["onhand"].ToString());
                double openQty = Convert.ToDouble(dr["OpenQty"].ToString());
                double onhand = Convert.ToDouble(dr["onhand"]);
                double reqQty = openQty - onhand;
                if (openQty <= 0) reqQty = 0;
                dtReqT.SetValue("Orderd", i, dr["OpenQty"].ToString());
                dtReqT.SetValue("Forecast", i, "0.00");


                dtReqT.SetValue("reqQty", i, reqQty);

                i++;
            }

            System.Data.DataTable dtFCT = Program.objHrmsUI.getDataTable("SELECT T0.[ItemCode],  T0.[Quantity] , oitm.onhand , oitm.itemname FROM FCT1 T0 inner join oitm on oitm.itemcode = t0.itemcode WHERE T0.[Date] ='" + dt.Date.ToString("yyyyMMdd") + "'", "forcast");

            foreach (System.Data.DataRow dr in dtFCT.Rows)
            {
                bool alreadFound = false;
                double fctQty = Convert.ToDouble(dr["Quantity"]);
                      
                for (int k = 0; k < dtReqT.Rows.Count; k++)
                {
                    string itemCode = Convert.ToString(dtReqT.GetValue("ItemCode", k));
                    if (itemCode == dr["ItemCode"].ToString()) 
                    {
                        alreadFound = true;
                        double reqQty = Convert.ToDouble(dtReqT.GetValue("reqQty", k)) + fctQty;
                        dtReqT.SetValue("Forecast", k, fctQty.ToString());
                       // dtReqT.SetValue("reqQty", k,   reqQty.ToString());

                    }
                }
                if (!alreadFound)
                {
                    dtReqT.Rows.Add(1);
                    dtReqT.SetValue("Id", dtReqT.Rows.Count - 1, dtReqT.Rows.Count.ToString());
                    dtReqT.SetValue("ItemCode", dtReqT.Rows.Count - 1, dr["ItemCode"].ToString());
                    dtReqT.SetValue("ItemName", dtReqT.Rows.Count - 1, dr["ItemName"].ToString());
                    dtReqT.SetValue("onHand", dtReqT.Rows.Count - 1, dr["onhand"].ToString());
                    dtReqT.SetValue("Orderd", dtReqT.Rows.Count - 1, "0.00");
                    dtReqT.SetValue("Forecast", dtReqT.Rows.Count - 1, fctQty.ToString());
                    dtReqT.SetValue("reqQty", dtReqT.Rows.Count - 1,"0.00");
                }
            }



            mtReqT.LoadFromDataSource();

        }

   
        private double scheduledQty(DateTime dtfrom, DateTime dtTo, string ItemCode)
        {
            double i = 0.00;

            string strScheduled = "Select sum(WOR1.plannedQty) from WOR1 inner join owor on owor.docentry = wor1.docentry where wor1.ItemCode = '" + ItemCode + "' and  owor.DueDate  between '" + dtfrom.ToString("yyyyMMdd") + "' and '" + dtTo.ToString("yyyyMMdd") +"' and OWOR.Status in ('R','P') ";
            try
            {
                i = Math.Round(Convert.ToDouble(Program.objHrmsUI.getScallerValue(strScheduled)), 3) ;
            }
            catch { }
            return i;
        }
        private void getReport()
        {
            DTRpt.Rows.Clear();
            DateTime dt = Convert.ToDateTime(dtHead.GetValue("DocDate", 0));
           
          
           // DateTime dt = DateTime.Now.Date;
            if (dtHead.GetValue("DocDate", 0) != "")
            {
                dt = Convert.ToDateTime(dtHead.GetValue("DocDate", 0));

                mtAnimals.FlushToDataSource();
                DTRpt.Rows.Clear();
                System.Data.DataTable dtBomChildItems = new System.Data.DataTable();
                dtBomChildItems.Columns.Add("ItemCode");
                dtBomChildItems.Columns.Add("Qty");
                string strItemCode = "";

                for (int i = 0; i < Animals.Rows.Count; i++)
                {
                    if (Convert.ToInt32(Animals.GetValue("Ordered", i)) + Convert.ToInt32(Animals.GetValue("QtySche", i)) > 0)
                    {
                        double fatherQty = Convert.ToDouble(Animals.GetValue("Ordered", i)) + Convert.ToDouble(Animals.GetValue("QtySche", i));
                        string strSel = "select itt1.code, Quantity * " + fatherQty.ToString() + " / oitt.Qauntity  as [Quantity]  from oitt inner join  ITT1 on oitt.code=itt1.father  inner join oitm on itt1.code = oitm.ItemCode where Father = '" + Animals.GetValue("ItemCode", i).ToString() + "'";
                        System.Data.DataTable dtCh = Program.objHrmsUI.getDataTable(strSel, "getting bom child");
                        foreach (System.Data.DataRow innerdr in dtCh.Rows)
                        {
                            dtBomChildItems.Rows.Add(innerdr[0], innerdr[1]);
                        }
                        strItemCode += strItemCode == "" ? "'" + Animals.GetValue("ItemCode", i).ToString() + "'" : " , '" + Animals.GetValue("ItemCode", i).ToString() + "'";
                    }
                }
                if (strItemCode == "")
                {
                    mtRpt.LoadFromDataSource();
                    return;
                }
                string OnOrdered = "Select sum(openqty * NumPerMsr) from rdr1 inner join ordr on ordr.docentry = rdr1.docentry where rdr1.itemcode = oitm.itemcode and  ordr.docdate between '" + dt.Date.ToString("yyyyMMdd") + "' and '" + dt.Date.ToString("yyyyMMdd") + "'";

                string strChildItems = "Select ItemCode, ItemName,OnHand,IsCommited , isnull(( " + OnOrdered + "),0.000) as Ordered  from OITM where itemcode in ( Select Code from itt1 where father in (" + strItemCode + "))";

                System.Data.DataTable dtChilderen = Program.objHrmsUI.getDataTable(strChildItems, "Get Child");
                int K = 0;



                foreach (System.Data.DataRow dr in dtChilderen.Rows)
                {
                    double onProd = 0.00;


                    DTRpt.Rows.Add(1);
                    DTRpt.SetValue("Id", K, (K + 1).ToString());
                    DTRpt.SetValue("ItemCode", K, dr["ItemCode"].ToString());
                    DTRpt.SetValue("ItemName", K, dr["ItemName"].ToString());
                    System.Data.DataRow[] childitemqtys = dtBomChildItems.Select("ItemCode = '" + dr["ItemCode"].ToString() + "'");
                    foreach (System.Data.DataRow childqtyrow in childitemqtys)
                    {
                        onProd += Convert.ToDouble(childqtyrow["Qty"]);
                    }
                    DTRpt.SetValue("OnProd", K, onProd.ToString());



                    double onHand = 0.00; ;
                    double IsCommited = 0.00;
                    double foreCasted = 0.00;
                    for (int n = 0; n < dtReqT.Rows.Count; n++)
                    {
                        string ReqItemCode = dtReqT.GetValue("ItemCode", n).ToString();
                        if (dr["ItemCode"].ToString() == ReqItemCode)
                        {
                            onHand = Convert.ToDouble(dtReqT.GetValue("onHand", n));
                            IsCommited = Convert.ToDouble(dtReqT.GetValue("Orderd", n));
                            foreCasted = Convert.ToDouble(dtReqT.GetValue("Forecast", n));
                        }
                    }

                    DTRpt.SetValue("OnHand", K, onHand.ToString());
                    DTRpt.SetValue("OnOrder", K, IsCommited.ToString());
                    DTRpt.SetValue("Forecast", K, foreCasted.ToString());
                    DTRpt.SetValue("ATP", K, (Math.Round(onProd + onHand - IsCommited - foreCasted, 3)).ToString());



                    K++;
                }
            }

            for (int k = 0; k < DTRpt.Rows.Count; k++)
            {
                string FGitemCode = Convert.ToString(DTRpt.GetValue("ItemCode", k));
                double QtyScheduled = Convert.ToDouble(DTRpt.GetValue("OnProd", k));


                for (int ki = 0; ki < dtReqT.Rows.Count; ki++)
                {
                    string reqItemCode = Convert.ToString(dtReqT.GetValue("ItemCode", ki));
                    if (FGitemCode == reqItemCode)
                    {
                        double qtyOrdered = Convert.ToDouble(dtReqT.GetValue("Orderd", ki));
                        double qtyStock = Convert.ToDouble(dtReqT.GetValue("onHand", ki));
                        double qtyForecast = Convert.ToDouble(dtReqT.GetValue("Forecast", ki));

                        DTRpt.SetValue("OnOrder", k, qtyOrdered.ToString());
                        DTRpt.SetValue("OnHand", k, qtyStock.ToString());
                        DTRpt.SetValue("ATP", k, QtyScheduled + qtyStock - qtyOrdered - qtyForecast);
                    }
                }
            }

            mtRpt.LoadFromDataSource();

            double TodayTotal = Convert.ToDouble(dtHead.GetValue("TodayQty", 0));
           // Finished Good Fill
           

           
        }



   
        private void createProductionOrder()
        {
           DateTime dt = Convert.ToDateTime(dtHead.GetValue("DocDate", 0));
            oApplication.SetStatusBarMessage("Creating Additional Production order of qty " , SAPbouiCOM.BoMessageTime.bmt_Short, false);
            newPOEntry = -1;
          
           for (int i = 0; i < Animals.Rows.Count; i++)
            {
               double AddQty = 0.00;
               AddQty = Math.Round(Convert.ToDouble(Animals.GetValue("Ordered", i)), 3);
               oApplication.SetStatusBarMessage("Creating Additional Production order of qty " + AddQty.ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, false); 
               if (AddQty  > 0)
                {
                    SAPbobsCOM.ProductionOrders prodOrder = (SAPbobsCOM.ProductionOrders)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oProductionOrders);


                    prodOrder.ItemNo = Animals.GetValue("ItemCode", i).ToString();
                    double Yield = Convert.ToDouble(Program.objHrmsUI.getScallerValue("Select isnull(U_Yield,0) from oitt where code = '" + prodOrder.ItemNo + "'"));
                  

                    if (Yield == 0) Yield = 100;

                    string strSqlDfltPL = "Select isnull(U_B1_dfltPL,'') PL from OITT where Code='" + prodOrder.ItemNo + "'";
                    string strDfltPL = Convert.ToString(Program.objHrmsUI.getScallerValue(strSqlDfltPL));


                   
                    

                    prodOrder.ProductionOrderType = SAPbobsCOM.BoProductionOrderTypeEnum.bopotDisassembly;
                    prodOrder.PlannedQuantity = AddQty;
                    prodOrder.DueDate = dt.Date;
                    prodOrder.PostingDate = dt.Date;
                    prodOrder.UserFields.Fields.Item("U_B1_Seq").Value = "1";
                
                    if (strDfltPL != "")
                    {
                        prodOrder.UserFields.Fields.Item("U_PMX_PLCD").Value = strDfltPL;
                        string nextSeq = " Select  max( isnull(convert(int,U_B1_SEQ),0))  + 1 from owor where Status<>'L' and ISNUMERIC (U_B1_SEQ) =1 and U_PMX_PLCD = '" + strDfltPL + "'  ";
                        string seq = Convert.ToString(Program.objHrmsUI.getScallerValue(nextSeq));
                        prodOrder.UserFields.Fields.Item("U_B1_Seq").Value = seq;
               
                     
                  
                    }

                    int result = prodOrder.Add();
                    if (result != 0)
                    {
                        int errorCode = 0;
                        string errmsg = "";
                        oCompany.GetLastError(out errorCode, out errmsg);
                        oApplication.MessageBox(errmsg);


                    }
                    else
                    {
                        int newWorEntry = Convert.ToInt32( oCompany.GetNewObjectKey());

                       // prodOrder.GetByKey(newWorEntry);
                       // for (int p = 0; p < prodOrder.Lines.Count; p++)
                       // {
                       //     prodOrder.Lines.SetCurrentLine(p);
                       //     prodOrder.Lines.PlannedQuantity = prodOrder.Lines.PlannedQuantity * Yield / 100;
                       // }
                       //// prodOrder.ProductionOrderStatus = SAPbobsCOM.BoProductionOrderStatusEnum.boposReleased;
                       // prodOrder.Update();


                    }

                    dtHead.SetValue("TodayQty", 0, 0);

                          
                }
              
            }



        }
        private void createSPProductionOrder()
        {
            DateTime dt = Convert.ToDateTime(dtHead.GetValue("DocDate", 0));
            for (int i = 0; i < Animals.Rows.Count; i++)
            {
               double AddQty = Math.Round(Convert.ToDouble(Animals.GetValue("Forecast", i)) - Convert.ToDouble(Animals.GetValue("QtySche", i))  , 3);
              
                if (AddQty > 0)
                {
                    SAPbobsCOM.ProductionOrders prodOrder = (SAPbobsCOM.ProductionOrders)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oProductionOrders);

                    prodOrder.ItemNo = Animals.GetValue("ItemCode", i).ToString();
                    double Yield = Convert.ToDouble(Program.objHrmsUI.getScallerValue("Select isnull(U_Yield,0) from oitt where code = '" + prodOrder.ItemNo + "'"));
                    if (Yield == 0) Yield = 100;

                    string strSqlDfltPL = "Select isnull(U_B1_dfltPL,'') PL from OITT where Code='" + prodOrder.ItemNo + "'";
                    string strDfltPL = Convert.ToString(Program.objHrmsUI.getScallerValue(strSqlDfltPL));
                   
                    prodOrder.ProductionOrderType = SAPbobsCOM.BoProductionOrderTypeEnum.bopotDisassembly;
                    prodOrder.PlannedQuantity = AddQty;
                    prodOrder.DueDate = dt.Date;
                    prodOrder.PostingDate = dt.Date;
                    prodOrder.UserFields.Fields.Item("U_B1_Seq").Value = "1";
                    if (strDfltPL != "")
                    {
                        prodOrder.UserFields.Fields.Item("U_PMX_PLCD").Value = strDfltPL;
                    }


                    int result = prodOrder.Add();
                    if (result != 0)
                    {
                        int errorCode = 0;
                        string errmsg = "";
                        oCompany.GetLastError(out errorCode, out errmsg);
                        oApplication.MessageBox(errmsg);


                    }
                    else
                    {
                        int newWorEntry = Convert.ToInt32(oCompany.GetNewObjectKey());

                        prodOrder.GetByKey(newWorEntry);
                        for (int p = 0; p < prodOrder.Lines.Count; p++)
                        {
                            prodOrder.Lines.SetCurrentLine(p);
                            prodOrder.Lines.PlannedQuantity = prodOrder.Lines.PlannedQuantity * Yield / 100;
                        }
                     //   prodOrder.ProductionOrderStatus = SAPbobsCOM.BoProductionOrderStatusEnum.boposReleased;
                        prodOrder.Update();

                    }




                }

            }



        }

        private void createTypeProductionOrder()
        {
            oApplication.SetStatusBarMessage("Creating Main Type Production order of qty ", SAPbouiCOM.BoMessageTime.bmt_Short, false);
            DateTime dt = Convert.ToDateTime(dtHead.GetValue("DocDate", 0));
          
                double AddQty = 0.00;
                AddQty = Math.Round(Convert.ToDouble(dtHead.GetValue("TodayQty", 0)));
                oApplication.SetStatusBarMessage("Creating Main Item Production order of qty " + AddQty.ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, false);
                if (AddQty > 0)
                {
                    SAPbobsCOM.ProductionOrders prodOrder = (SAPbobsCOM.ProductionOrders)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oProductionOrders);

                    prodOrder.ItemNo = cbAT.Selected.Value.ToString().Trim();
                    double Yield = Convert.ToDouble(Program.objHrmsUI.getScallerValue("Select isnull(U_Yield,0) from oitt where code = '" + prodOrder.ItemNo + "'"));


                    if (Yield == 0) Yield = 100;

                    string strSqlDfltPL = "Select isnull(U_B1_dfltPL,'') PL from OITT where Code='" + prodOrder.ItemNo + "'";
                    string strDfltPL = Convert.ToString(Program.objHrmsUI.getScallerValue(strSqlDfltPL));





                    prodOrder.ProductionOrderType = SAPbobsCOM.BoProductionOrderTypeEnum.bopotDisassembly;
                    prodOrder.PlannedQuantity = AddQty;
                    prodOrder.DueDate = dt.Date;
                    prodOrder.PostingDate = dt.Date;
                    prodOrder.UserFields.Fields.Item("U_B1_Seq").Value = "1";
                    prodOrder.UserFields.Fields.Item("U_PONum").Value = newPOEntry.ToString();


                    if (strDfltPL != "")
                    {
                        prodOrder.UserFields.Fields.Item("U_PMX_PLCD").Value = strDfltPL;
                        string nextSeq = " Select  max( isnull(convert(int,U_B1_SEQ),0))  + 1 from owor where Status<>'L' and ISNUMERIC (U_B1_SEQ) =1 and U_PMX_PLCD = '" + strDfltPL + "'  ";
                        string seq = Convert.ToString(Program.objHrmsUI.getScallerValue(nextSeq));
                        prodOrder.UserFields.Fields.Item("U_B1_Seq").Value = seq;



                    }

                    int result = prodOrder.Add();
                    if (result != 0)
                    {
                        int errorCode = 0;
                        string errmsg = "";
                        oCompany.GetLastError(out errorCode, out errmsg);
                        oApplication.MessageBox(errmsg);


                    }
                    else
                    {
                        int newWorEntry = Convert.ToInt32(oCompany.GetNewObjectKey());

                        //prodOrder.GetByKey(newWorEntry);
                        //for (int p = 0; p < prodOrder.Lines.Count; p++)
                        //{
                        //    prodOrder.Lines.SetCurrentLine(p);
                        //    prodOrder.Lines.PlannedQuantity = prodOrder.Lines.PlannedQuantity * Yield / 100;
                        //}
                        //// prodOrder.ProductionOrderStatus = SAPbobsCOM.BoProductionOrderStatusEnum.boposReleased;
                        //prodOrder.Update();


                    }




                

            }



        }



        private int createTypePurchaseOrder()
        {

            int result = -1;
            double AddQty = 0.00;
            AddQty = Math.Round(Convert.ToDouble(dtHead.GetValue("TodayQty", 0)));
            string cardCode = Convert.ToString( dtHead.GetValue("CardCode", 0));
            string numAtCard = Convert.ToString(dtHead.GetValue("Ref", 0));
            DateTime dtDocDate = Convert.ToDateTime(dtHead.GetValue("PoDate", 0));

            oApplication.SetStatusBarMessage("Creating Purcahse order of qty " + AddQty.ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, false);
            if (AddQty > 0)
            {
                SAPbobsCOM.Documents PurchaseOrder = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders);
                PurchaseOrder.DocDate = dtDocDate;

                PurchaseOrder.CardCode = cardCode;
                PurchaseOrder.NumAtCard = numAtCard;

                PurchaseOrder.Lines.ItemCode = cbAT.Selected.Value.ToString().Trim();
                PurchaseOrder.Lines.Quantity = AddQty;
               // PurchaseOrder.Lines.GrossBuyPrice
                PurchaseOrder.Lines.Add();

                 result = PurchaseOrder.Add();
                if (result != 0)
                {
                    int errorCode = 0;
                    string errmsg = "";
                    oCompany.GetLastError(out errorCode, out errmsg);
                    oApplication.MessageBox(errmsg);


                }
                else
                {
                    int newWorEntry = Convert.ToInt32(oCompany.GetNewObjectKey());
                    newPOEntry = newWorEntry;
                    //prodOrder.GetByKey(newWorEntry);
                    //for (int p = 0; p < prodOrder.Lines.Count; p++)
                    //{
                    //    prodOrder.Lines.SetCurrentLine(p);
                    //    prodOrder.Lines.PlannedQuantity = prodOrder.Lines.PlannedQuantity * Yield / 100;
                    //}
                    //// prodOrder.ProductionOrderStatus = SAPbobsCOM.BoProductionOrderStatusEnum.boposReleased;
                    //prodOrder.Update();


                }

               



            }

            return result;


        }
       

        private void fillPO()
        {
            DateTime dtFrom = Convert.ToDateTime(dtHead.GetValue("DocDate", 0)).Date;
            DateTime dtTo = Convert.ToDateTime(dtHead.GetValue("DocDate", 0)).Date;

            Animals.Rows.Clear();
            dtPro.Rows.Clear();
            dtTypeS.Rows.Clear();
            string strPOs = "SELECT        dbo.OWOR.DocEntry, dbo.OWOR.ItemCode, dbo.OITM.ItemName, dbo.OWOR.PlannedQty, dbo.OWOR.DueDate,Case owor.status When 'P' then 'Planned' ELSE 'Released' end as status , isnull(OWOR.U_PONum,'') as PoNum ";
            strPOs += " , isnull(OPOR.CardName,'') as Supplier , isnull(U_B1_Seq,-1) as Seq ";
            strPOs += " FROM            dbo.OWOR INNER JOIN  dbo.OITM ON dbo.OWOR.ItemCode = dbo.OITM.ItemCode Left join OPOR on opor.docentry = isnull(OWOR.U_PONum,-1) ";
            strPOs += " where OWOR.Status in ('R','P') and  dbo.OWOR.DueDate between '" + dtFrom.ToString("yyyyMMdd") + "' and '" + dtTo.ToString("yyyyMMdd") + "' ";

            System.Data.DataTable dtWOR = Program.objHrmsUI.getDataTable(strPOs, "Get PROS");
            int K = 0;
            int j = 0;
            foreach (System.Data.DataRow dr in dtWOR.Rows)
            {
                string itemCode = dr["ItemCode"].ToString();
                if (ItemType.Contains(itemCode))
                {
                    dtTypeS.Rows.Add(1);
                    dtTypeS.SetValue("Id", j, (j + 1).ToString());
                    dtTypeS.SetValue("ItemCode", j, dr["ItemCode"].ToString());
                    dtTypeS.SetValue("ItemName", j, dr["ItemName"].ToString());
                    dtTypeS.SetValue("ProNum", j, dr["DocEntry"].ToString());
                    dtTypeS.SetValue("Supplier", j, dr["Supplier"].ToString());
                    dtTypeS.SetValue("PONum", j, dr["PoNum"].ToString());
                   
                    dtTypeS.SetValue("Quantity", j, dr["PlannedQty"].ToString());
                    dtTypeS.SetValue("Status", j, dr["Status"].ToString());
                    dtTypeS.SetValue("SeqNum", j, dr["Seq"].ToString());

                   



                    j++;
                }
                else
                {

                    dtPro.Rows.Add(1);
                    dtPro.SetValue("Id", K, (K + 1).ToString());
                    dtPro.SetValue("ItemCode", K, dr["ItemCode"].ToString());
                    dtPro.SetValue("ItemName", K, dr["ItemName"].ToString());
                    dtPro.SetValue("PO", K, dr["DocEntry"].ToString());
                    dtPro.SetValue("DueDate", K, Convert.ToDateTime(dr["DueDate"].ToString()));
                    dtPro.SetValue("Quantity", K, dr["PlannedQty"].ToString());
                    dtPro.SetValue("Status", K, dr["Status"].ToString());
                    K++;
                }


             
            }

            mtProd.LoadFromDataSource();
            mtTypePRO.LoadFromDataSource();
            mtProd.Item.Visible = false;
        }
        private void cflcardcode(SAPbouiCOM.ChooseFromList oCFL, string uID)
        {

            try
            {

                SAPbouiCOM.Conditions oCons;
                SAPbouiCOM.Condition oCon;
                oCons = oCFL.GetConditions();
                oCon = oCons.Add();
                oCon.Alias = "CardType";
                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                oCon.CondVal = "S";
                oCFL.SetConditions(oCons);



            }
            catch (Exception ex)
            {

                // MsgBox(Err.Description)

            }

        }


    }
}
