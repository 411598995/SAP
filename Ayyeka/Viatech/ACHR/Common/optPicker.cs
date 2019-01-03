using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using SAPbobsCOM;
using SAPbouiCOM;
namespace ACHR
{
    public class optPicker
    {
        private SAPbouiCOM.Application oApplication;
        private string sInput = "";
        private string sTitle = "";
        public bool flgMultiple = true;
        private bool bLoadInputEvents;
        private System.Data.DataTable dtTable;
        private System.Data.DataTable dtOut = new System.Data.DataTable();
        private SAPbouiCOM.DataTable dtSearch;
        SAPbouiCOM.Matrix mtSearch;
        SAPbouiCOM.Form oform;
        SAPbouiCOM.EditText SearchField;
        SAPbouiCOM.Item IbtChoos, ISearchField;
        SAPbouiCOM.Button btChoos;

        public optPicker(SAPbouiCOM.Application app, System.Data.DataTable dt)
        {
            oApplication = app;
            oApplication.ItemEvent += new _IApplicationEvents_ItemEventEventHandler(ItemEvents);
            dtTable = dt;
        }

        public System.Data.DataTable ShowInput(string Title, string Message)
        {

            try
            {
                bLoadInputEvents = true;
                sTitle = Title;
                oApplication.MessageBox(Message);

            }
            catch
            {
                bLoadInputEvents = false;
            }
            return dtOut;
        }

        // handles Form event trafic for the form associated to this class instance
        public virtual void ItemEvents(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {


                switch (pVal.EventType)
                {

                    case BoEventTypes.et_ITEM_PRESSED:

                        e_ItemPressed(ref pVal, ref BubbleEvent);

                        break;
                    case BoEventTypes.et_FORM_LOAD:

                        e_FormLoad(ref pVal, ref BubbleEvent);

                        break;
                    case BoEventTypes.et_FORM_CLOSE:
                        bLoadInputEvents = false;
                        break;

                }




            }
            catch (Exception ex)
            {
                //oApplication.MessageBox(ex.Message);
                bLoadInputEvents = false;
            }



        }

        protected virtual void e_FormLoad(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {

            try
            {
                if (pVal.BeforeAction == false && bLoadInputEvents)
                {
                    foreach (System.Data.DataColumn cl in dtTable.Columns)
                    {
                        dtOut.Columns.Add(cl.ColumnName);
                       
                      

                    }

                    SAPbouiCOM.Button btOption;
                    oform = oApplication.Forms.Item(pVal.FormUID);
                    oform.ClientWidth = 500;
                    oform.ClientHeight = 400;
                    IbtChoos = oform.Items.Item("1");
                   
                    int offsetY = 40;
                    int buttonId = 0;
                    foreach (System.Data.DataRow dr in dtTable.Rows)
                    {
                        SAPbouiCOM.Item btnItem = oform.Items.Add(dr[0].ToString()  , BoFormItemTypes.it_BUTTON);

                        btnItem.Top = offsetY;
                        btnItem.Left = 75;
                        btnItem.Width = 350;
                        btnItem.Height = 40;
                        btOption = (SAPbouiCOM.Button) btnItem.Specific;
                        btOption.Caption = dr[1].ToString();

                        buttonId++;
                        offsetY += 50;
                    }


                 oform.Items.Item("1").Visible=false;
                    
                    //SAPbouiCOM.StaticText lblCri = oItem.Specific;





                    oform = null;

                }
            }
            catch (Exception ex)
            {
                bLoadInputEvents = false;
            }


        }

        protected virtual void e_ItemPressed(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {

            if (pVal.ItemUID == "1" && pVal.BeforeAction)
            {
                /*

                sInput = "Success";
                if (string.IsNullOrEmpty(sInput))
                {
                    BubbleEvent = false;
                }
                else
                {
                    bLoadInputEvents = false;
                }
                 * */
            }

            if (pVal.ItemUID.Contains( "opt")  && pVal.BeforeAction)
            {
                System.Data.DataRow dr = dtOut.NewRow();

                dr[0] = pVal.ItemUID;
                dtOut.Rows.Add(dr);
                IbtChoos.Visible = true;
                IbtChoos.Click();
            }

        }
       

    }
}