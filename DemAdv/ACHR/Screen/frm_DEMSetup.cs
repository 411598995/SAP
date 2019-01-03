using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACHR.Screen
{
    class frm_DEMSetup : HRMSBaseForm
    {

        public bool isForLoading = false;
        SAPbouiCOM.Matrix mtAddon;

        SAPbouiCOM.DataTable dtHead, dtSetting;

        SAPbouiCOM.EditText txDfltExp;

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

       
        
        private void InitiallizeForm()
        {

            Program.objHrmsUI.loadSettings();
            oForm.Freeze(true);


           

            dtHead = oForm.DataSources.DataTables.Item("dtHead");
            dtSetting = oForm.DataSources.DataTables.Item("dtSetting");
            dtSetting.Rows.Add(1);
            mtAddon = (SAPbouiCOM.Matrix)oForm.Items.Item("mtAddon").Specific;

            txDfltExp = (SAPbouiCOM.EditText)oForm.Items.Item("txDfltExp").Specific;

            cflDE = oForm.ChooseFromLists.Item("cflDE");
            fillAddons();
            fillSettings();
            filtertocfl();

            oForm.Freeze(false);

            
        }

        private void fillSettings()
        {
            txDfltExp.Value = Program.objHrmsUI.getSetting("DfltExp");

        }


        private void saveSettings()
        {
            Program.objHrmsUI.SaveSetting("DfltExp",txDfltExp.Value.ToString());
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

            string strSql = "Select * from [@DEM_MODULES]";
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
                updateAddon += "Update [@DEM_MODULES] set U_LicenseKey='" + dtHead.GetValue("LK", i).ToString() + "',U_Enabled='" + dtHead.GetValue("Active", i).ToString() + "' where Code='" + dtHead.GetValue("AC", i).ToString() + "' ;";

            }




            int result = Program.objHrmsUI.ExecQuery(updateAddon, "Updating Module Setup");

            saveSettings();
            Program.objHrmsUI.createConfigurationTables();
            Program.objHrmsUI.loadSettings();
            Program.objHrmsUI.loadAddons();

        }
    
    }
}
