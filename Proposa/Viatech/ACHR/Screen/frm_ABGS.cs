using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Mag =  ACHR.com.thefruitcompany.www;

using SAPbouiCOM;

namespace ACHR.Screen
{
    class frm_ABGS : HRMSBaseForm
    {


        public bool isForLoading = false;
        SAPbouiCOM.EditText txHost, txUsr, txPwd, txFP, txFldNO, txFldUF, txFldAF, txFldAck, txFldETA;
         SAPbouiCOM.CheckBox chDemo;
        SAPbouiCOM.DataTable dtHead;
        public string cardCode;
        bool initiallizing = false;
        public override void CreateForm(SAPbouiCOM.Application SboApp, string strXml, SAPbobsCOM.Company cmp, string frmId)
        {
            base.CreateForm(SboApp, strXml, cmp, frmId);

            oForm.EnableMenu("1282", false);  // Add New Record
            oForm.EnableMenu("1288", false);  // Next Record
            oForm.EnableMenu("1289", false);  // Pevious Record
            oForm.EnableMenu("1290", false);  // First Record
            oForm.EnableMenu("1291", false);  // Last record 
            oForm.EnableMenu("1281", false);  // Find record 
            oForm.Settings.Enabled = false;
            InitiallizeForm();




        }

        public override void etAfterClick(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == "1")
            {
                Program.objHrmsUI.SaveSetting("WebCardCode", Convert.ToString(dtHead.GetValue("CardCode", 0)));
                Program.objHrmsUI.SaveSetting("WebSlpCode", Convert.ToString(dtHead.GetValue("slp", 0)));
                Program.objHrmsUI.SaveSetting("WebBranch", Convert.ToString(dtHead.GetValue("branch", 0)));
                Program.objHrmsUI.SaveSetting("Warehouse", Convert.ToString(dtHead.GetValue("Whs", 0)));

            }
        }


        private void loadSetting()
        {
            Program.objHrmsUI.loadSettings();

            try
            {
                cardCode = Program.objHrmsUI.settings["WebCardCode"].ToString();
                dtHead.SetValue("CardCode", 0, Program.objHrmsUI.settings["WebCardCode"].ToString());
                dtHead.SetValue("slp", 0, Program.objHrmsUI.settings["WebSlpCode"].ToString());
                dtHead.SetValue("branch", 0, Program.objHrmsUI.settings["WebBranch"].ToString());
                dtHead.SetValue("Whs", 0, Program.objHrmsUI.settings["Warehouse"].ToString());


            }
            catch { }


        }
        private void InitiallizeForm()
        {
           

            oForm.Freeze(true);

            initiallizing = true;


            dtHead = oForm.DataSources.DataTables.Item("dtHead");
            dtHead.Rows.Add(1);


            loadSetting();

           oForm.Freeze(false);

            initiallizing = false;



        }

      
      
      
    }
}
