using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;

namespace ACHR.Screen
{
    class frm_ModSetup : HRMSBaseForm
    {

        public bool isForLoading = false;
        SAPbouiCOM.Matrix mtAddon;

        SAPbouiCOM.DataTable dtHead, dtSetting;

        SAPbouiCOM.EditText txDfltExp, txUID, txPWD, txINCCode, txLTDCode, txINCDB, txLTDDB,txTax;

        SAPbouiCOM.ChooseFromList cflDE;

       
        public override void CreateForm(SAPbouiCOM.Application SboApp, string strXml, SAPbobsCOM.Company cmp, string frmId)
        {
            base.CreateForm(SboApp, strXml, cmp, frmId);
    InitiallizeForm();
    oForm.PaneLevel = 1;
            
        }

        public override void etAfterClick(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == "1")
            {
                updateModuleSetup();
            }
        }
        public override void etAfterCfl(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterCfl(ref pVal, ref BubbleEvent);
            SAPbouiCOM.IChooseFromListEvent oCFLEvento = (SAPbouiCOM.IChooseFromListEvent)pVal;
            SAPbouiCOM.DataTable dtSel = oCFLEvento.SelectedObjects;
            if (pVal.ItemUID == txDfltExp.Item.UniqueID)
            {
                if (dtSel != null && dtSel.Rows.Count > 0)
                {
                    string strCode = dtSel.GetValue("AcctCode", 0).ToString();
                    string strName = dtSel.GetValue("AcctName", 0).ToString();
                    dtSetting.SetValue("dfltExp", 0, strCode);

                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE;
                }
            }

           

        }
        public override void etAfterKeyDown(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterKeyDown(ref pVal, ref BubbleEvent);
           
        }
        public override void etAfterValidate(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterValidate(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == "txPwd")
            {
                dtSetting.SetValue("encpwd", 0, Program.encriptor.Encrypt(dtSetting.GetValue("pwd", 0).ToString()));
            }
        }



        private void InitiallizeForm()
        {

            Program.objHrmsUI.loadSettings();
            oForm.Freeze(true);


           

            dtHead = oForm.DataSources.DataTables.Item("dtHead");
            dtSetting = oForm.DataSources.DataTables.Item("dtSetting");
            dtSetting.Rows.Add(1);
            mtAddon = (SAPbouiCOM.Matrix)oForm.Items.Item("mtAddon").Specific;

            txDfltExp = (SAPbouiCOM.EditText)oForm.Items.Item("txDfltExp").Specific;

            txUID = (SAPbouiCOM.EditText)oForm.Items.Item("txUID").Specific;
            txPWD = (SAPbouiCOM.EditText)oForm.Items.Item("txPWD").Specific;
            txINCCode = (SAPbouiCOM.EditText)oForm.Items.Item("txINCCode").Specific;
            txLTDCode = (SAPbouiCOM.EditText)oForm.Items.Item("txLTDCode").Specific;
            txINCDB = (SAPbouiCOM.EditText)oForm.Items.Item("txINCDB").Specific;
            txLTDDB = (SAPbouiCOM.EditText)oForm.Items.Item("txLTDDB").Specific;
            txTax = (SAPbouiCOM.EditText)oForm.Items.Item("txTax").Specific;

            cflDE = oForm.ChooseFromLists.Item("cflDE");
            fillAddons();
            fillSettings();
            filtertocfl();

            oForm.Freeze(false);

            
        }

        private void fillSettings()
        {
            txDfltExp.Value = Program.objHrmsUI.getSetting("DfltExp");

           txINCCode.Value = Program.objHrmsUI.getSetting("INCCODE");
            txLTDCode.Value = Program.objHrmsUI.getSetting("LTDCODE");
            txUID.Value = Program.objHrmsUI.getSetting("UID");
            txPWD.Value = Program.objHrmsUI.getSetting("PWD");
            txINCDB.Value = Program.objHrmsUI.getSetting("INCDB");
            txLTDDB.Value = Program.objHrmsUI.getSetting("LTDDB");
            txTax.Value = Program.objHrmsUI.getSetting("TAX");




        }


        private void saveSettings()
        {
            Program.objHrmsUI.SaveSetting("DfltExp",txDfltExp.Value.ToString());

            Program.objHrmsUI.SaveSetting("UID", txUID.Value.ToString());
            Program.objHrmsUI.SaveSetting("PWD", txPWD.Value.ToString());
            Program.objHrmsUI.SaveSetting("INCCODE", txINCCode.Value.ToString());
            Program.objHrmsUI.SaveSetting("LTDCODE", txLTDCode.Value.ToString());
            Program.objHrmsUI.SaveSetting("INCDB", txINCDB.Value.ToString());
            Program.objHrmsUI.SaveSetting("LTDDB", txLTDDB.Value.ToString());

            Program.objHrmsUI.SaveSetting("TAX", txTax.Value.ToString());



        }

        private void filtertocfl()
        {
            SAPbouiCOM.Conditions oCons;
            SAPbouiCOM.Condition oCon;

            oCons = cflDE.GetConditions();
            oCon = oCons.Add();
            oCon.Alias = "Postable";
            oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
            oCon.CondVal = "Y";
            cflDE.SetConditions(oCons);

        }

        private void fillAddons()
        {
            dtHead.Rows.Clear();

            string strSql = "Select * from [@B1_MODULES]";
            System.Data.DataTable dtModules = Program.objHrmsUI.getDataTable(strSql, "Filling Module");
            int i=0;
            foreach (System.Data.DataRow dr in dtModules.Rows)
            {
                dtHead.Rows.Add(1);
                dtHead.SetValue("Id", i, (i + 1).ToString());
                dtHead.SetValue("AC", i, dr["CODE"].ToString());
                dtHead.SetValue("AN", i, dr["NAME"].ToString());
                dtHead.SetValue("LK", i, dr["U_LicenseKey"].ToString());
                dtHead.SetValue("Active", i, dr["U_Enabled"].ToString());
                i++;
            }
            mtAddon.LoadFromDataSource();
        }

        private void updateModuleSetup()
        {

            mtAddon.FlushToDataSource();
            string updateAddon = "";
            for (int i = 0; i < dtHead.Rows.Count; i++)
            {
                updateAddon += "Update [@B1_MODULES] set U_LicenseKey='" + dtHead.GetValue("LK", i).ToString() + "',U_Enabled='" + dtHead.GetValue("Active", i).ToString() + "' where Code='" + dtHead.GetValue("AC", i).ToString() + "' ;";

            }




            int result = Program.objHrmsUI.ExecQuery(updateAddon, "Updating Module Setup");

            saveSettings();
            Program.objHrmsUI.createConfigurationTables();
            Program.objHrmsUI.loadSettings();
            Program.objHrmsUI.loadAddons();

        }
    
    }
}
