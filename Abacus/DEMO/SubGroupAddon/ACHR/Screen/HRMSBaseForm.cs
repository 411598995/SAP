using System;
using System.Threading;
using System.Data;
using System.Data.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbobsCOM;
using SAPbouiCOM.Framework;
using SAPbouiCOM;


namespace ACHR.Screen
{
    public class HRMSBaseForm :IDisposable
    {
        public Hashtable CodeIndex = new Hashtable();
        public int currentRecord = -1;
        public int totalRecord = 0;
        private bool isActiveForm = false;
       
        public SAPbouiCOM.Form oForm;
        public SAPbobsCOM.Company oCompany;
        public SAPbouiCOM.Application oApplication;
        private bool _disposed;
        public string currentObjId = "";
        public Hashtable SearchKeyVal = new Hashtable();
        public Hashtable objDataSources = new Hashtable();
        public string formId;
        public string frmXml;
        public string cflId = "";
        public string strCfl = System.Windows.Forms.Application.StartupPath + "\\CFL.bmp";
        public string activeMatrixId = "";
        public HRMSBaseForm()
        {

        }
        public void statusMsg(string strMessage, BoStatusBarMessageType msgType = BoStatusBarMessageType.smt_None  )
        {
            oApplication.StatusBar.SetText(Text:strMessage , Type: msgType);

        }
        public void Dispose()
        {
            Dispose(true);
            GC.Collect();
            GC.SuppressFinalize(this);
            GC.Collect();

        }

        private void IniTxtObject(string sboId, SAPbouiCOM.EditText txtBox)
        {


        }
        protected virtual void Dispose(bool disposing)
        {
            // If you need thread safety, use a lock around these  
            // operations, as well as in your methods that use the resource. 
            if (!_disposed)
            {
                if (disposing)
                {
                    oApplication.ItemEvent -= new _IApplicationEvents_ItemEventEventHandler(oApplication_ItemEvent);
                    oApplication.MenuEvent -= new _IApplicationEvents_MenuEventEventHandler(oApplication_MenuEvent);
                    oApplication.RightClickEvent -= new _IApplicationEvents_RightClickEventEventHandler(oApplication_RightClickEvent);
                    oApplication.AppEvent -= new _IApplicationEvents_AppEventEventHandler(oApplication_AppEvent);
           

                }

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
       public virtual void oApplication_MenuEvent(ref MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            //try
            //{
            //    if (oForm == null || oApplication.Forms.ActiveForm.UniqueID != oForm.UniqueID)
            //    {
            //        isActiveForm = false;
            //    }
            //    else
            //    {
            //        isActiveForm = true;
            //    }
            //    BubbleEvent = true;
            //    if (pVal.BeforeAction)
            //    {
            //        if (pVal.MenuUID == "5890")
            //        {
            //            if (activeMatrixId != "")
            //            {
            //                updateFormSetting(activeMatrixId);
            //            }
            //        }
            //    }
            //    else
            //    {
                    
            //        if (pVal.MenuUID == "1282" && isActiveForm) AddNewRecord();
            //        if (pVal.MenuUID == "1288" && isActiveForm) getNextRecord();
            //        if (pVal.MenuUID == "1289" && isActiveForm) getPreviouRecord();
            //        if (pVal.MenuUID == "1290" && isActiveForm) getFirstRecord();
            //        if (pVal.MenuUID == "1291" && isActiveForm) getLastRecord();
            //        if (pVal.MenuUID == "1281" && isActiveForm) FindRecordMode();
                   
            //    }
            //}
            //catch (Exception ex)
            //{
            //}
           
        }
        
       public virtual void oApplication_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
       {

           BubbleEvent = true;
           if (FormUID != formId) return;

           try
           {


               if (pVal.Before_Action)
               {
                   //Item Events
                   if (pVal.EventType == BoEventTypes.et_ITEM_PRESSED) etBeforeClick(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_CLICK) this.etBeforeActClick(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_LOST_FOCUS) etBeforeLostFocus(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_GOT_FOCUS) etBeforeGetFocus(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST) etBeforeCfl(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_COMBO_SELECT) etBeforeCmbSelect(ref pVal, ref BubbleEvent);

                   if (pVal.EventType == BoEventTypes.et_MATRIX_LINK_PRESSED) etBeforeMtLinkPressed(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_MATRIX_LOAD) etBeforeMtLinkLoad(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_MENU_CLICK) etBeforeMnuClick(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_RIGHT_CLICK) etBeforeRightClick(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_KEY_DOWN) etBeforeKeyDown(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_DOUBLE_CLICK) etBeforeDoubleClick(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_Drag) etBeforeDrag(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_VALIDATE) etBeforeValidate(ref pVal, ref BubbleEvent);

                   if (pVal.EventType == BoEventTypes.et_FORM_CLOSE) etFormBeforClose(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_FORM_DATA_ADD) etFormBeforeDataAdd(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_FORM_DATA_DELETE) etFormBeforeDataDelete(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_FORM_DATA_LOAD) etFormBeforeDataLoad(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_FORM_DATA_UPDATE) etFormBeforeDataUpdate(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_FORM_DEACTIVATE) etFormBeforeDeactivate(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_FORM_KEY_DOWN) etFormBeforeKeyDown(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_FORM_LOAD) etFormBeforeLoad(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_FORM_RESIZE) etFormBeforeResize(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_FORM_UNLOAD) etFormBeforeUnload(ref pVal, ref BubbleEvent);

                   if (pVal.EventType == BoEventTypes.et_FORMAT_SEARCH_COMPLETED)
                   {
                       etAfterCfl(ref pVal, ref BubbleEvent);

                   }

                
                   //form events

               }
               else
               {
                   if (pVal.EventType == BoEventTypes.et_ITEM_PRESSED) etAfterClick(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_CLICK) this.etAfterActClick(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_LOST_FOCUS) etAfterLostFocus(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_GOT_FOCUS) etAfterGetFocus(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST) etAfterCfl(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_COMBO_SELECT) etAfterCmbSelect(ref pVal, ref BubbleEvent);

                   if (pVal.EventType == BoEventTypes.et_MATRIX_LINK_PRESSED) etAfterMtLinkPressed(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_MATRIX_LOAD) etAfterMtLinkLoad(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_MENU_CLICK) etAfterMnuClick(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_RIGHT_CLICK) etAfterRightClick(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_KEY_DOWN) etAfterKeyDown(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_DOUBLE_CLICK) etAfterDoubleClick(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_Drag) etAfterDrag(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_VALIDATE) etAfterValidate(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_FORM_CLOSE) etFormAfterClose(ref pVal, ref BubbleEvent);

                   if (pVal.EventType == BoEventTypes.et_FORM_DATA_ADD) etFormAfterDataAdd(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_FORM_DATA_DELETE) etFormAfterDataDelete(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_FORM_DATA_LOAD) etFormAfterDataLoad(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_FORM_DATA_UPDATE) etFormAfterDataUpdate(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_FORM_DEACTIVATE) etFormAfterDeactivate(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_FORM_KEY_DOWN) etFormAfterKeyDown(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_FORM_LOAD) etFormAfterLoad(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_FORM_RESIZE) etFormAfterResize(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_FORM_UNLOAD) etFormAfterUnload(ref pVal, ref BubbleEvent);
                   if (pVal.EventType == BoEventTypes.et_FORM_ACTIVATE) etFormAfterActivate(ref pVal, ref BubbleEvent);




               }
           }
           catch (Exception ex)
           {
               oApplication.SetStatusBarMessage(ex.Message);
           }
       }
       

        public void setFilter()
        {
            
            

        }



        public virtual void oApplication_RightClickEvent(ref ContextMenuInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            //try
            //{
            //    if (eventInfo.BeforeAction)
            //    {
            //        SAPbouiCOM.Item clickedItem = oForm.Items.Item(eventInfo.ItemUID);
            //        if (clickedItem.Type == BoFormItemTypes.it_MATRIX)
            //        {
            //            activeMatrixId = clickedItem.UniqueID;
            //        }
            //        else
            //        {
            //            activeMatrixId = "";
            //        }

            //    }
              
            //}
            //catch (Exception ex)
            //{
            //    oApplication.SetStatusBarMessage("Error: " + ex.Message);
            //}
        }

       
        public void oApplication_AppEvent(BoAppEventTypes EventType)
        {
            
        }
        public virtual void CreateForm( SAPbouiCOM. Application SboApp, string strXml, SAPbobsCOM.Company cmp, string frmId)
        {

            formId = frmId;
            frmXml = strXml;
            oCompany = cmp;
            oApplication = SboApp;
            
            oApplication.ItemEvent += new _IApplicationEvents_ItemEventEventHandler(oApplication_ItemEvent);
            oApplication.RightClickEvent += new _IApplicationEvents_RightClickEventEventHandler(oApplication_RightClickEvent);
            oApplication.AppEvent += new _IApplicationEvents_AppEventEventHandler(oApplication_AppEvent);
            oApplication.MenuEvent += new _IApplicationEvents_MenuEventEventHandler(oApplication_MenuEvent);

            UDClass clsudo = Program.objHrmsUI;
            setFilter();
            try
            {
                clsudo.AddXML(strXml);
                oForm = oApplication.Forms.Item(frmId);
            }
            catch (Exception ex)
            {
                try
                {
                    oForm = oApplication.Forms.Item(frmId);
                    oForm.Select();

                }
                catch (Exception ex1)
                {
                    oApplication.StatusBar.SetText(ex1.Message);
                }
            }
            oForm.EnableMenu("1282", true);  // Add New Record
            oForm.EnableMenu("1288", true);  // Next Record
            oForm.EnableMenu("1289", true);  // Pevious Record
            oForm.EnableMenu("1290", true);  // First Record
            oForm.EnableMenu("1291", true);  // Last record 
            oForm.EnableMenu("1281", true);  // Find record 
            
            string mnuId = frmId.Replace("frm","mnu");

            //isFormReadOnly(mnuId);
           

        }
       
        public DateTime textToDate(string strDt)
        {
            return DateTime.ParseExact(strDt, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
        }
        //General Methods
        public virtual void getNextRecord()
        {
            if (currentRecord + 1 == totalRecord)
            {
                currentRecord = 0;
                oApplication.SetStatusBarMessage(Program.objHrmsUI.getStrMsg("Nev_Rec_Last"), SAPbouiCOM.BoMessageTime.bmt_Short, false);

            }
            else
            {
                currentRecord = currentRecord + 1;
            }
            fillFields();
        }
        public virtual void getPreviouRecord()
        {
            if (currentRecord <= 0)
            {
                currentRecord = totalRecord - 1;
            }
            else
            {
                currentRecord = currentRecord - 1;
            }
            fillFields();
            
        }
        public virtual void AddNewRecord() { if (oForm.Selected == false) return; }
        public virtual void getLastRecord()
        {
            currentRecord = totalRecord - 1;
            fillFields();
        }
        public virtual void getRecord(string id)
        {
            string codeId = Convert.ToString( CodeIndex [id].ToString());
            currentRecord = Convert.ToInt32(CodeIndex[id].ToString());
            fillFields();
        }
        public virtual void getFirstRecord()
        {
            currentRecord = 0;
            fillFields();

        }
        public virtual void FindRecordMode() { if (oForm.Selected == false) return; }

        public virtual void updateFormSetting(string matrixUid){
        
            oForm.Settings.Enabled=true;
            oForm.Settings.MatrixUID = matrixUid;
        }
        public virtual void fillFields() { }
        public  virtual void PrepareSearchKeyHash() { }

        // Before events
        //Items
        public virtual void etBeforeClick(ref ItemEvent pVal, ref bool BubbleEvent) { }

        public virtual void etBeforeActClick(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etAfterActClick(ref ItemEvent pVal, ref bool BubbleEvent) { }


        public virtual void etBeforeCfl(ref ItemEvent pVal, ref bool BubbleEvent)
        {
           
        }
        //public string cflThread()
        //{

        //    System.Threading.Thread ShowFolderBrowserThread = null;
        //    try
        //    {
        //        ShowFolderBrowserThread = new System.Threading.Thread(SaveFileBrowser);

        //        if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Unstarted)
        //        {
        //            ShowFolderBrowserThread.SetApartmentState(System.Threading.ApartmentState.STA);
        //            ShowFolderBrowserThread.Start();
        //        }
        //        else if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Stopped)
        //        {
        //            ShowFolderBrowserThread.Start();
        //            ShowFolderBrowserThread.Join();

        //        }
        //        Thread.Sleep(5000);

        //        while (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Running)
        //        {
        //            System.Windows.Forms.Application.DoEvents();
        //        }
        //        if (!string.IsNullOrEmpty(FileName))
        //        {
        //            return FileName;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        oApplication.MessageBox("FileFile" + ex.Message);
        //    }

        //    return "";

        //}

        public virtual void handleCfl()
        {
        }
        public virtual void etBeforeCmbSelect(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etBeforeLostFocus(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etBeforeGetFocus(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etBeforeMtLinkPressed(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etBeforeMtLinkLoad(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etBeforeMnuClick(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etBeforeRightClick(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etBeforeKeyDown(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etBeforeDoubleClick(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etBeforeDrag(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etBeforeValidate(ref ItemEvent pVal, ref bool BubbleEvent) { }
        
        //Form
        public virtual void etFormBeforClose(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etFormBeforActivate(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etFormBeforeDataAdd(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etFormBeforeDataDelete(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etFormBeforeDataLoad(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etFormBeforeDataUpdate(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etFormBeforeDeactivate(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etFormBeforeKeyDown(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etFormBeforeLoad(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etFormBeforeResize(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etFormBeforeUnload(ref ItemEvent pVal, ref bool BubbleEvent) { }



        // After events
        //Items
        public virtual void etAfterClick(ref ItemEvent pVal, ref bool BubbleEvent) {

            try
            {
                SAPbouiCOM.Item clickIt = oForm.Items.Item(pVal.ItemUID);

                string itType = clickIt.Type.ToString();
                if (itType == "it_FOLDER")
                {
                    if (pVal.ItemUID.Contains("_"))
                    {
                        string strTabPan = pVal.ItemUID.Split('_')[1].ToString();
                        oForm.PaneLevel = Convert.ToInt16(strTabPan);
                        BubbleEvent = false;
                    }
                }
            }
            catch (Exception ex)
            {
                oApplication.SetStatusBarMessage(ex.Message);
            }
            
        }
        public virtual void etAfterCfl(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etAfterCmbSelect(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etAfterLostFocus(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etAfterGetFocus(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etAfterMtLinkPressed(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etAfterMtLinkLoad(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etAfterMnuClick(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etAfterRightClick(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etAfterKeyDown(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etAfterDoubleClick(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etAfterDrag(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etAfterValidate(ref ItemEvent pVal, ref bool BubbleEvent) { }

        public virtual void etFormAfterClose(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            this.Dispose();

        }

        //Form

        public virtual void etFormAfterActivate(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etFormAfterDataAdd(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etFormAfterDataDelete(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etFormAfterDataLoad(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etFormAfterDataUpdate(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etFormAfterDeactivate(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etFormAfterKeyDown(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etFormAfterLoad(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etFormAfterResize(ref ItemEvent pVal, ref bool BubbleEvent) { }
        public virtual void etFormAfterUnload(ref ItemEvent pVal, ref bool BubbleEvent) { }

        //Form ActionSuccess 
       
        //public virtual void etFormAfterClose(ref BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent) { }
       // public virtual void etFormAfterActivate(ref BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent) { }
        public virtual void etFormSuccessDataAdd(ref BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent) { }
        public virtual void etFormSuccessDataDelete(ref BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent) { }
        public virtual void etFormSuccessDataLoad(ref BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent) { }
        public virtual void etFormSuccessDataUpdate(ref BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent) { }
       // public virtual void etFormAfterDeactivate(ref BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent) { }
       // public virtual void etFormAfterKeyDown(ref BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent) { }
        public virtual void etFormSuccessLoad(ref BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent) { }
       // public virtual void etFormAfterResize(ref BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent) { }
        public virtual void etFormSuccessUnload(ref BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent) { }


       
    }

}
