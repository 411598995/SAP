using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace ACHR.Screen
{
    class frm_Conf : HRMSBaseForm
    {
        SAPbouiCOM.EditText txNPServer, txNPDB, txNPUID, txNPPwd, txSServer, txSDB, txSUID, txSPwd, txSDBUid, txSDBPwd, txSSType, txFI, txSoSer, txSalesEmp, txOwn, txWhs, txStdPL, txPrFldr, txCatFldr;
        SAPbouiCOM.Button cmdUdfs, cmdTestNP, cmdClear, cmdRole, cmdCus, cmdIG, cmdItems, cmdPrice, cmdGPrice;
        SAPbouiCOM.Item ItxNPServer, ItxNPDB, ItxNPUID, ItxNPPwd, ItxSServer, ItxSDB, ItxSUID, ItxSPwd, ItxSDBUid, ItxSDBPwd, ItxSSType, ItxFI, ItxSoSer, ItxSalesEmp, ItxOwn, ItxWhs, ItxStdPL, ItxPrFldr, ItxCatFldr;
        SAPbouiCOM.Item IcmdStart, IcmdTestNP, IcmdClear, IcmdRole, IcmdCus, IcmdIG, IcmdItems, IcmdPrice, IcmdGPrice;
        
        SAPbouiCOM.DataTable dtPeriods, dtEmpsPr, dtEmpsPost, dtPrEle, dtPrOth, dtJE, dtJeDet, dtHead;
        public string selJe = "";
        public DateTime PeriodStartDate, PeriodEndDate;
        private bool periodLocked = false;

        #region /////Events

        public override void etFormAfterLoad(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etFormAfterLoad(ref pVal, ref BubbleEvent);

         

        }
        public override void etAfterClick(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            try
            {
                base.etAfterClick(ref pVal, ref BubbleEvent);




                if (pVal.ItemUID == "cmdClear")
                {

                    NopServices ns = new NopServices(Program. strConNOP, Program. strConSAP, Program.SboAPI);
                string result=    ns.cleanData();
                    oApplication.SetStatusBarMessage("UDFs Created Successfully." + result , SAPbouiCOM.BoMessageTime.bmt_Medium, false);
                }
                if (pVal.ItemUID == "cmdUdfs")
                {
                    try
                    {
                        oApplication.SetStatusBarMessage("Creating Udfs.", SAPbouiCOM.BoMessageTime.bmt_Medium, false);
                        NopServices ns = new NopServices(Program.strConNOP, Program.strConSAP, Program.SboAPI);
                        ns.addSboFields();
                        oApplication.SetStatusBarMessage("UDFs Created Successfully.", SAPbouiCOM.BoMessageTime.bmt_Medium,false);
                    }
                    catch (Exception ex)
                    {
                    }
                }
               
                if (pVal.ItemUID ==  "cmdRole")
                {
                    try
                    {
                        NopServices ns = new NopServices(Program.strConNOP, Program.strConSAP, Program.SboAPI);
                        ns.SyncRole();
                        oApplication.SetStatusBarMessage("Role Created Successfully.", SAPbouiCOM.BoMessageTime.bmt_Medium, false);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                if (pVal.ItemUID == "cmdCus")
                {
                    try
                    {
                        NopServices ns = new NopServices(Program.strConNOP, Program.strConSAP, Program.SboAPI);
                        ns.SyncCustomer();
                        oApplication.SetStatusBarMessage("Customer Transfered Successfully.", SAPbouiCOM.BoMessageTime.bmt_Medium, false);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                if (pVal.ItemUID == "cmdIG")
                {
                    try
                    {
                        NopServices ns = new NopServices(Program.strConNOP, Program.strConSAP, Program.SboAPI);
                        ns.SyncItemGroup();
                        ns.syncCatImages(Program.categorImageFolder);
                       
                        oApplication.SetStatusBarMessage("Item Group Transfered Successfully.", SAPbouiCOM.BoMessageTime.bmt_Medium, false);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                if (pVal.ItemUID == "cmdItems")
                {
                    try
                    {
                        NopServices ns = new NopServices(Program.strConNOP, Program.strConSAP, Program.SboAPI);
                        ns.SyncItems();
                      //  ns.syncProductImages(Program.productImageFolder);
                        oApplication.SetStatusBarMessage("Items Transfered Successfully.", SAPbouiCOM.BoMessageTime.bmt_Medium, false);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                if (pVal.ItemUID == "cmdPrice")
                {
                    try
                    {
                        NopServices ns = new NopServices(Program.strConNOP, Program.strConSAP, Program.SboAPI);
                        ns.UpdateStandardPrice(Program.standardPricelist);
                        oApplication.SetStatusBarMessage("Items Price Updated Successfully.", SAPbouiCOM.BoMessageTime.bmt_Medium, false);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                if (pVal.ItemUID == "cmdGPrice")
                {
                    try
                    {
                        NopServices ns = new NopServices(Program.strConNOP, Program.strConSAP, Program.SboAPI);
                        ns.UpdateStandardPrice(Program.standardPricelist);
                        oApplication.SetStatusBarMessage("Group Price Updated Successfully.", SAPbouiCOM.BoMessageTime.bmt_Medium, false);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                if (pVal.ItemUID == "1")
                {

                    
                    NopServices ns = new NopServices(Program.strConNOP, Program.strConSAP, Program.SboAPI);

                   string consaveResult =  ns.updateConConfiguration(txSDB.Value, txSUID.Value, txSPwd.Value, txSDBUid.Value, txSDBPwd.Value, txSSType.Value, txSServer.Value, txNPServer.Value, txNPDB.Value, txNPUID.Value, txNPPwd.Value);

                   if (consaveResult == "Ok")
                   {
                       Program.companyDb = txSDB.Value;
                       Program.SboUID = txSUID.Value;
                       Program.SboPwd = txSPwd.Value;
                       Program.DbUserName = txSPwd.Value;
                       Program.DbPassword = txSDBPwd.Value;
                       Program.ServerType = txSSType.Value;
                       Program.SboServer = txSServer.Value;

                       Program.standardPricelist = txStdPL.Value; ;
                       Program.whsCode = txWhs.Value; ;
                      Program.  productImageFolder = txPrFldr.Value;
                      Program.categorImageFolder = txPrFldr.Value;

                      Program.NopDbUserName = txNPUID.Value;
                      Program.NopDbPassword =txNPPwd.Value;
                      Program.NopDbName = txNPDB.Value;
                      Program.NopSServer = txNPServer.Value;
                   }
                    ns.updateConfiguration(txSDB.Value, txSUID.Value, txSPwd.Value, txSDBUid.Value, txSDBPwd.Value, txSSType.Value, txSServer.Value, txNPServer.Value, txNPDB.Value, txNPUID.Value, txNPPwd.Value, txStdPL.Value, txWhs.Value, txPrFldr.Value, txCatFldr.Value,txFI.Value,txSoSer.Value,txSalesEmp.Value,txOwn.Value);
                   
                    Program.updateSetting();
                }
            }
            catch (Exception ex)
            {
                oApplication.SetStatusBarMessage("Error occured " + ex.Message);
            }
        }

      
        #endregion

        #region ///Initiallization

        private void InitiallizeForm()
        {
            //  dtHead = oForm.DataSources.DataTables.Item("dtHead");
            // dtHead.Rows.Add(1);

            oForm.Freeze(true);


            oForm.DataSources.UserDataSources.Add("txNPServer", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 50); // Days of Month
            txNPServer = oForm.Items.Item("txNPServer").Specific;
            ItxNPServer = oForm.Items.Item("txNPServer");
            txNPServer.DataBind.SetBound(true, "", "txNPServer");

            oForm.DataSources.UserDataSources.Add("txNPDB", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 50); // Days of Month
            txNPDB = oForm.Items.Item("txNPDB").Specific;
            ItxNPDB = oForm.Items.Item("txNPDB");
            txNPDB.DataBind.SetBound(true, "", "txNPDB");

            oForm.DataSources.UserDataSources.Add("txNPUID", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 50); // Days of Month
            txNPUID = oForm.Items.Item("txNPUID").Specific;
            ItxNPUID = oForm.Items.Item("txNPUID");
            txNPUID.DataBind.SetBound(true, "", "txNPUID");

            oForm.DataSources.UserDataSources.Add("txNPPwd", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 50); // Days of Month
            txNPPwd = oForm.Items.Item("txNPPwd").Specific;
            ItxNPPwd = oForm.Items.Item("txNPPwd");
            txNPPwd.DataBind.SetBound(true, "", "txNPPwd");


            oForm.DataSources.UserDataSources.Add("txSServer", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 50); // Days of Month
            txSServer = oForm.Items.Item("txSServer").Specific;
            ItxSServer = oForm.Items.Item("txSServer");
            txSServer.DataBind.SetBound(true, "", "txSServer");

            oForm.DataSources.UserDataSources.Add("txSDB", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 50); // Days of Month
            txSDB = oForm.Items.Item("txSDB").Specific;
            ItxSDB = oForm.Items.Item("txSDB");
            txSDB.DataBind.SetBound(true, "", "txSDB");



            oForm.DataSources.UserDataSources.Add("txSUID", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 50); // Days of Month
            txSUID = oForm.Items.Item("txSUID").Specific;
            ItxSUID = oForm.Items.Item("txSUID");
            txSUID.DataBind.SetBound(true, "", "txSUID");

            oForm.DataSources.UserDataSources.Add("txSPwd", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 50); // Days of Month
            txSPwd = oForm.Items.Item("txSPwd").Specific;
            ItxSPwd = oForm.Items.Item("txSPwd");
            txSPwd.DataBind.SetBound(true, "", "txSPwd");


            oForm.DataSources.UserDataSources.Add("txSDBUid", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 50); // Days of Month
            txSDBUid = oForm.Items.Item("txSDBUid").Specific;
            ItxSDBUid = oForm.Items.Item("txSDBUid");
            txSDBUid.DataBind.SetBound(true, "", "txSDBUid");

            oForm.DataSources.UserDataSources.Add("txSDBPwd", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 50); // Days of Month
            txSDBPwd = oForm.Items.Item("txSDBPwd").Specific;
            ItxSDBPwd = oForm.Items.Item("txSDBPwd");
            txSDBPwd.DataBind.SetBound(true, "", "txSDBPwd");

            oForm.DataSources.UserDataSources.Add("txSSType", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 4); // Days of Month
            txSSType = oForm.Items.Item("txSSType").Specific;
            ItxSSType = oForm.Items.Item("txSSType");
            txSSType.DataBind.SetBound(true, "", "txSSType");

            oForm.DataSources.UserDataSources.Add("txFI", SAPbouiCOM.BoDataType.dt_SHORT_TEXT); // Days of Month
            txFI = oForm.Items.Item("txFI").Specific;
            ItxFI = oForm.Items.Item("txFI");
            txFI.DataBind.SetBound(true, "", "txFI");

            oForm.DataSources.UserDataSources.Add("txSoSer", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 30); // Days of Month
            txSoSer = oForm.Items.Item("txSoSer").Specific;
            ItxSoSer = oForm.Items.Item("txSoSer");
            txSoSer.DataBind.SetBound(true, "", "txSoSer");


            oForm.DataSources.UserDataSources.Add("txSalesEmp", SAPbouiCOM.BoDataType.dt_LONG_NUMBER, 30); // Days of Month
            txSalesEmp = oForm.Items.Item("txSalesEmp").Specific;
            ItxSalesEmp = oForm.Items.Item("txSalesEmp");
            txSalesEmp.DataBind.SetBound(true, "", "txSalesEmp");

            oForm.DataSources.UserDataSources.Add("txOwn", SAPbouiCOM.BoDataType.dt_LONG_NUMBER, 30); // Days of Month
            txOwn = oForm.Items.Item("txOwn").Specific;
            ItxOwn = oForm.Items.Item("txOwn");
            txOwn.DataBind.SetBound(true, "", "txOwn");



            oForm.DataSources.UserDataSources.Add("txWhs", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 30); // Days of Month
            txWhs = oForm.Items.Item("txWhs").Specific;
            ItxWhs = oForm.Items.Item("txWhs");
            txWhs.DataBind.SetBound(true, "", "txWhs");
            oForm.DataSources.UserDataSources.Add("txStdPL", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 30); // Days of Month
            txStdPL = oForm.Items.Item("txStdPL").Specific;
            ItxStdPL = oForm.Items.Item("txStdPL");
            txStdPL.DataBind.SetBound(true, "", "txStdPL");
            oForm.DataSources.UserDataSources.Add("txPrFldr", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 100); // Days of Month
            txPrFldr = oForm.Items.Item("txPrFldr").Specific;
            ItxPrFldr = oForm.Items.Item("txPrFldr");
            txPrFldr.DataBind.SetBound(true, "", "txPrFldr");
            oForm.DataSources.UserDataSources.Add("txCatFldr", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 100); // Days of Month
            txCatFldr = oForm.Items.Item("txCatFldr").Specific;
            ItxCatFldr = oForm.Items.Item("txCatFldr");
            txCatFldr.DataBind.SetBound(true, "", "txCatFldr");



            cmdUdfs = oForm.Items.Item("cmdUdfs").Specific;
            cmdTestNP = oForm.Items.Item("cmdTestNP").Specific;
            cmdClear = oForm.Items.Item("cmdClear").Specific;
            cmdRole = oForm.Items.Item("cmdRole").Specific;
            cmdCus = oForm.Items.Item("cmdCus").Specific;
            cmdIG = oForm.Items.Item("cmdIG").Specific;
            cmdItems = oForm.Items.Item("cmdItems").Specific;
            cmdPrice = oForm.Items.Item("cmdPrice").Specific;
            cmdGPrice = oForm.Items.Item("cmdGPrice").Specific;

            oForm.PaneLevel = 1;
            oForm.Freeze(false);


        }
        public override void CreateForm(SAPbouiCOM.Application SboApp, string strXml, SAPbobsCOM.Company cmp, string frmId)
        {
            base.CreateForm(SboApp, strXml, cmp, frmId);
            InitiallizeForm();
            NopServices ns = new NopServices(Program.strConNOP, Program.strConSAP, Program.SboAPI);
            DataTable dtConf = ns.getConfiguration();

            if (dtConf.Rows.Count > 0)
            {

                txSDB.Value = dtConf.Rows[0][2].ToString();
                txSUID.Value = dtConf.Rows[0][3].ToString();
                txSPwd.Value = dtConf.Rows[0][4].ToString();
                txSDBUid.Value = dtConf.Rows[0][5].ToString();
                txSDBPwd.Value = dtConf.Rows[0][6].ToString();
                txSSType.Value = dtConf.Rows[0][7].ToString();
                txSServer.Value = dtConf.Rows[0][8].ToString();
                txNPServer.Value = dtConf.Rows[0][9].ToString();
                txNPDB.Value = dtConf.Rows[0][10].ToString();
                txNPUID.Value = dtConf.Rows[0][11].ToString();
                txNPPwd.Value = dtConf.Rows[0][12].ToString();
                txStdPL.Value = dtConf.Rows[0][13].ToString();
                txWhs.Value = dtConf.Rows[0][14].ToString();
                txPrFldr.Value = dtConf.Rows[0][15].ToString();
                txCatFldr.Value = dtConf.Rows[0][16].ToString();

                txFI.Value = dtConf.Rows[0][17].ToString();
                txSoSer.Value = dtConf.Rows[0][18].ToString();
                txSalesEmp.Value = dtConf.Rows[0][19].ToString();
                txOwn.Value = dtConf.Rows[0][20].ToString();






            }   
        }
      
        #endregion

        #region //Common Methods




        #endregion

       

       
    }

}

