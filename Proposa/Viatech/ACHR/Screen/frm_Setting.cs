using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACHR.Screen
{
    class frm_Setting : HRMSBaseForm
    {

        public bool isForLoading = false;
        SAPbouiCOM.Folder tbTS, tbSO;
        SAPbouiCOM.OptionBtn opOD, opOW, opOM,OMDT,OMDY;
        SAPbouiCOM.Matrix mtSOP, mtTOR, mtSTO, mtORI, mtORAT, mtStock;
        SAPbouiCOM.ComboBox cbSP, cbDays, cbWeeks;
        SAPbouiCOM.EditText txSD, txLO, txMS, txHistory, txNRP;

        SAPbouiCOM.DataTable dtHead;

       
        public override void CreateForm(SAPbouiCOM.Application SboApp, string strXml, SAPbobsCOM.Company cmp, string frmId)
        {
            base.CreateForm(SboApp, strXml, cmp, frmId);

            AddNewSetting();
          
            InitiallizeForm();
            
        }

        public override void etAfterClick(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == "1")
            {
                updateSetting();
            }
        }
       
        
        private void InitiallizeForm()
        {


            oForm.Freeze(true);


           

            dtHead = oForm.DataSources.DataTables.Item("dtHeads");
            dtHead.Rows.Add(1);
         

            txSD = (SAPbouiCOM.EditText)oForm.Items.Item("txSD").Specific;
            txLO = (SAPbouiCOM.EditText)oForm.Items.Item("txLO").Specific;
            txMS = (SAPbouiCOM.EditText)oForm.Items.Item("txMS").Specific;
            txHistory = (SAPbouiCOM.EditText)oForm.Items.Item("txHistory").Specific;
            txNRP = (SAPbouiCOM.EditText)oForm.Items.Item("txNRP").Specific;


            getSchedule();
            oForm.Freeze(false);


        }



        private void AddNewSetting()
        {
            string strExisting = @"SELECT         Code, Name, U_SchDays, U_NLastOrdr, U_NMSI, U_NDTH  
                                        FROM            [@B1_CONFIG] ";
            strExisting += "Where Code='0001'";

            System.Data.DataTable dtSchedule = Program.objHrmsUI.getDataTable(strExisting, "getting schedule");
            if (dtSchedule.Rows.Count == 0)
            {
                string strInsert = " insert into  [@B1_CONFIG]  (Code, Name, U_SchDays, U_NLastOrdr, U_NMSI, U_NDTH,U_NRP,U_AlwPriceCh) ";
                strInsert += " Values ('0001','0001','30','10','10','7','10','N')";
                Program.objHrmsUI.ExecQuery(strInsert, "Adding Setting");

            }



        }

        private void updateSetting()
        {




            string updateSch = "Update [@B1_CONFIG] set U_SchDays='" + txSD.Value.ToString() + "',U_NLastOrdr='" + txLO.Value.ToString() + "',U_NMSI='" 
                + txMS.Value.ToString() + "',U_NDTH ='" + txHistory.Value.ToString() + "',U_NRP = '" + txNRP.Value.ToString() + "',U_SED = '" + dtHead.GetValue("SDD",0).ToString()
                + "',U_DBPATH ='" + dtHead.GetValue("dbPath", 0).ToString()
                + "' , U_AlwPriceCh = '" + dtHead.GetValue("CP", 0).ToString() + "'  where Code='0001' ";

            int result = Program.objHrmsUI.ExecQuery(updateSch, "Updating Configuration");


        }
        private void getSchedule()
        {


            string strGet = "Select * from [@B1_CONFIG] where [Code]='0001'";
            System.Data.DataTable dtSch = Program.objHrmsUI.getDataTable(strGet, "Loading Data");

            foreach (System.Data.DataRow dr in dtSch.Rows)
            {



                dtHead.SetValue("SchDays", 0, dr["U_SchDays"].ToString());
                dtHead.SetValue("LNOrdr", 0, dr["U_NLastOrdr"].ToString());
                dtHead.SetValue("MSOrdr", 0, dr["U_NMSI"].ToString());
                dtHead.SetValue("DCHistory", 0, dr["U_NDTH"].ToString());
                dtHead.SetValue("NRP", 0, dr["U_NRP"].ToString());
                dtHead.SetValue("SDD", 0, dr["U_SED"].ToString());
                dtHead.SetValue("dbPath", 0, dr["U_DBPATH"].ToString());
                dtHead.SetValue("CP", 0, dr["U_AlwPriceCh"].ToString());
            }
               

        }

    }
}
