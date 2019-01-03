#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using SapBusinessOneExtensions;
using SAPbobsCOM;
#endregion

namespace WarehouseTransfer
{
    class clsStartup
    {
        #region Declarations
        protected static SAPbouiCOM.Application oApplication = null;
        public static SAPbobsCOM.Company oCompany = null;
        private static Hashtable oWhsCollection = null;
        public static Thread TmpThread = new Thread(CloseApp);
        public static bool blnFreight = false;
        public static bool blnBatchMessage = false;
        public static DocumentType DocumentReferenceType;
        public static string strType = string.Empty;
        public static int intFormCount = 0;
        public static Dictionary<string,string[]> dicINDE = null;
        public static Hashtable oCenWhsCollection = null;
        public static string DefaultGroup = string.Empty;

        public enum DocumentType
        {   
            ARInvoice,
            ARPaymentInvoice
        }

        #endregion

        #region Company Connection
        private void SetApplication()
        {
            SAPbouiCOM.SboGuiApi SboGuiApi = null;
            string sConnectionString = "0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056";
            try
            {
                sConnectionString = Environment.GetCommandLineArgs().GetValue(1).ToString();
            }
            catch { }

            SboGuiApi = new SAPbouiCOM.SboGuiApi();
         //   sConnectionString = System.Convert.ToString(Environment.GetCommandLineArgs().GetValue(1));

            SboGuiApi.Connect(sConnectionString);
            oApplication = SboGuiApi.GetApplication(-1);
           
        }

        private int SetConnectionContext()
        {
            String sCookie;
            String sConnectionContext;
            try
            {
                oCompany = new SAPbobsCOM.Company();
                sCookie = oCompany.GetContextCookie();
                sConnectionContext = oApplication.Company.GetConnectionContext(sCookie);
                if (oCompany.Connected)
                {
                    oCompany.Disconnect();
                }
                return oCompany.SetSboLoginContext(sConnectionContext);
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            return 1;
        }

        private int ConnectToCompany()
        {
            return oCompany.Connect();
        }

        private void Class_Init()
        {
            SetApplication();
            if (SetConnectionContext() == 0)
            {
                if (ConnectToCompany() != 0)
                {
                    oApplication.StatusBar.SetText("Failed Connecting to Company's Database", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    int i;
                    String errCode;
                    oCompany.GetLastError(out i, out errCode);

                    oApplication.MessageBox(i + " " + errCode, 1, "OK", "", "");
                }
                else
                {
                    SboAddon.Create("ANS", "B1",oApplication,oCompany);
                }
            }
            else
            {
                oApplication.StatusBar.SetText("Cannot make a connection to the DI API", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
        }

        static void CloseApp()
        {
            System.Windows.Forms.Application.Exit();
        }
        #endregion

        #region Create Tables
        bool CreateTables()
        {
            bool boolResult = false;

            //#region "Inventory Transer"

         

            //clsSBO.AddNumericField("WTR1", "BaseDE", "Transfer Base Entry", 8, oApplication, oCompany);
            //clsSBO.AddNumericField("WTR1", "BaseLn", "Transfer Base Line", 2, oApplication, oCompany);
            //clsSBO.AddFloatField("WTQ1", "RecQty", "Received Quantity", SAPbobsCOM.BoFldSubTypes.st_Quantity, oApplication, oCompany);

            //clsSBO.AddAlphaField("OWTR", "TrnsType", "Transit Type", 1, "S,R", "Shipment,Receipt", "", oApplication, oCompany);
            //clsSBO.AddAlphaField("OWTR", "TrnsDN", "Transit DocNum", 8, oApplication, oCompany);
            //clsSBO.AddAlphaField("OWTR", "TrnsTW", "To Warehouse", 8, oApplication, oCompany);
            //clsSBO.AddAlphaField("OWTR", "TrnsFW", "From Warehouse", 8, oApplication, oCompany);

            //clsSBO.AddNumericField("OWTQ", "SONum", "SO Number", 8, oApplication, oCompany);

            //clsSBO.AddAlphaField("OWTQ", "TrsfType", "Transfer Type", 8, oApplication, oCompany);

            //#endregion

         

            //clsSBO.AddFloatField("WTQ1", "TrsfQty", "Transfer Quantity", SAPbobsCOM.BoFldSubTypes.st_Quantity, oApplication, oCompany);
            //clsSBO.AddFloatField("WTQ1", "OTrsfQty", "Open Transfer Quantity", SAPbobsCOM.BoFldSubTypes.st_Quantity, oApplication, oCompany);

            //clsSBO.AddAlphaField("IGE1", "ITRDE", "ITR DocEntry", 8, oApplication, oCompany);
            //clsSBO.AddAlphaField("IGE1", "ITRLN", "ITR Line", 8, oApplication, oCompany);
            
        

            //#region "Stock Transit Shipment"

            //clsSBO.CreateTable("EJ_OSTS", "Stock Shipment Header", SAPbobsCOM.BoUTBTableType.bott_Document, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTS", "ReqNum", "Request Number", 8, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTS", "PickNum", "Picklist Number", 8, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTS", "CardCode", "Card Code", 8, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTS", "FromWhs", "From WareHouse", 8, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTS", "ToWhs", "To WareHouse", 8, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTS", "TrnWhs", "Transit WareHouse", 8, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTS", "ReqNum", "Request Number", 50, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTS", "TrnsNum", "Transfer Number", 50, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTS", "DocSta", "Document Status", 1, "O,P,C", "Open,Processed,Closed", "O", oApplication, oCompany);
            //clsSBO.AddDateField("@EJ_OSTS", "DocDate", "Document Date", SAPbobsCOM.BoFldSubTypes.st_None, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTS", "Comments", "Document Comments", 250, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTS", "SlpCode", "Salesperson", 20, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTS", "Driver", "Driver", 50, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTS", "Vehicle", "Vehicle", 50, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTS", "Brand", "Brand", 50, oApplication, oCompany);

            //clsSBO.CreateTable("EJ_STS1", "Stock Shipment Details", SAPbobsCOM.BoUTBTableType.bott_DocumentLines, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_STS1", "Barcode", "Barcode", 250, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_STS1", "ItemCode", "Item Code", 50, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_STS1", "ItemName", "Item Name", 200, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_STS1", "UOMCode", "UOM Code", 20, oApplication, oCompany);
            //clsSBO.AddFloatField("@EJ_STS1", "OnHand", "OnHand", SAPbobsCOM.BoFldSubTypes.st_Quantity, oApplication, oCompany);
            //clsSBO.AddFloatField("@EJ_STS1", "ReqQty", "Requested Quantity", SAPbobsCOM.BoFldSubTypes.st_Quantity, oApplication, oCompany);
            //clsSBO.AddFloatField("@EJ_STS1", "Qty", "Quantity", SAPbobsCOM.BoFldSubTypes.st_Quantity, oApplication, oCompany);
            //clsSBO.AddFloatField("@EJ_STS1", "RecQty", "Received Quantity", SAPbobsCOM.BoFldSubTypes.st_Quantity, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_STS1", "BaseDE", "Base Entry", 8, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_STS1", "BaseLn", "Base Line", 8, oApplication, oCompany);

            

            //#endregion

            //#region "Stock Transit Receipt"


            //clsSBO.CreateTable("EJ_OSTR", "Stock Receipt Header", SAPbobsCOM.BoUTBTableType.bott_Document, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTR", "ShpNum", "Shipment Number", 8, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTR", "CardCode", "Card Code", 8, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTR", "FromWhs", "From WareHouse", 8, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTR", "ToWhs", "To WareHouse", 8, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTR", "TrnWhs", "Transit WareHouse", 8, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTR", "TrnsNum", "Transfer Number", 50, oApplication, oCompany);
            //clsSBO.AddDateField("@EJ_OSTR", "DocDate", "Document Date", SAPbobsCOM.BoFldSubTypes.st_None, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTR", "Comments", "Document Comments", 250, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTR", "SlpCode", "Salesperson", 20, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTR", "Driver", "Driver", 50, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSTR", "Brand", "Brand", 50, oApplication, oCompany);
            //clsSBO.AddFieldLinkTable("@EJ_OSTR", "Atchmt", "Attachment", BoFieldTypes.db_Memo, 254, BoFldSubTypes.st_Link, "", oApplication, oCompany);

            //clsSBO.CreateTable("EJ_STR1", "Stock Receipt Details", SAPbobsCOM.BoUTBTableType.bott_DocumentLines, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_STR1", "Barcode", "Barcode", 250, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_STR1", "ItemCode", "Item Code", 50, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_STR1", "ItemName", "Item Name", 200, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_STR1", "UOMCode", "UOM Code", 20, oApplication, oCompany);
            //clsSBO.AddFloatField("@EJ_STR1", "ShpQty", "Shipment Quantity", SAPbobsCOM.BoFldSubTypes.st_Quantity, oApplication, oCompany);
            //clsSBO.AddFloatField("@EJ_STR1", "BalQty", "Shipment Quantity", SAPbobsCOM.BoFldSubTypes.st_Quantity, oApplication, oCompany);
            //clsSBO.AddFloatField("@EJ_STR1", "Qty", "Quantity", SAPbobsCOM.BoFldSubTypes.st_Quantity, oApplication, oCompany);
            //clsSBO.AddFloatField("@EJ_STR1", "DiffQty", "Difference Qty", SAPbobsCOM.BoFldSubTypes.st_Quantity, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_STR1", "BaseDE", "Base Entry", 8, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_STR1", "BaseLn", "Base Line", 8, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_STR1", "ITRDE", "ITR DocEntry", 8, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_STR1", "ITRLN", "ITR Line", 8, oApplication, oCompany);

            //clsSBO.CreateTable("EJ_OSSP", "Stock Shipment Park", SAPbobsCOM.BoUTBTableType.bott_NoObject, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSSP", "DocNum", "DocNum", 50, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSSP", "CardCode", "CardCode", 50, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSSP", "FromWhs", "FromWhs", 50, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSSP", "ToWhs", "ToWhs", 50, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSSP", "Remarks", "Remarks", 254, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSSP", "Sender", "Sender", 50, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OSSP", "ParkRemarks", "ParkRemarks", 254, oApplication, oCompany);

            //clsSBO.CreateTable("EJ_SSP1", "Stock Shipment Park Details", SAPbobsCOM.BoUTBTableType.bott_NoObjectAutoIncrement, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_SSP1", "ParkNo", "Park Number", 50, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_SSP1", "Sno", "Sno", 50, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_SSP1", "ItemCode", "ItemCode", 50, oApplication, oCompany);
            //clsSBO.AddFloatField("@EJ_SSP1", "Qty", "Quantity", SAPbobsCOM.BoFldSubTypes.st_Quantity, oApplication, oCompany);


            //clsSBO.CreateTable("EJ_ODRM", "Driver Master", SAPbobsCOM.BoUTBTableType.bott_NoObject, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_ODRM", "Phone", "Phone", 50, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_ODRM", "EMail", "EMail", 100, oApplication, oCompany);

            //clsSBO.CreateTable("EJ_OVHM", "Vehicle Master", SAPbobsCOM.BoUTBTableType.bott_NoObject, oApplication, oCompany);
            //clsSBO.AddAlphaField("@EJ_OVHM", "Capacity", "Capacity", 50, oApplication, oCompany);

            //clsSBO.CreateTable("EJ_OLSR", "Lost Sales Reason", SAPbobsCOM.BoUTBTableType.bott_NoObjectAutoIncrement, oApplication, oCompany);

            //clsSBO.CreateTable("EJ_OTRT", "Transfer Type", SAPbobsCOM.BoUTBTableType.bott_NoObjectAutoIncrement, oApplication, oCompany);


            //#endregion


             boolResult = true;
             return boolResult;
        }
        #endregion

        #region Create UDOs
        void CreateUDOs()
        {
           // clsSBO.CreateUDO("EJ_OSTS", "Stock Shipment Header", "EJ_OSTS", SAPbobsCOM.BoUDOObjType.boud_Document, "EJ_STS1", 1, "DocNum,U_FromWhs,U_ToWhs,U_Comments", oApplication, oCompany);
            //clsSBO.CreateUDO("EJ_OSTR", "Stock Receipt Header", "EJ_OSTR", SAPbobsCOM.BoUDOObjType.boud_Document, "EJ_STR1", 1, "DocNum,U_FromWhs,U_ToWhs,U_Comments", oApplication, oCompany);

        }
        #endregion

        #region Filters
        void SetFilter()
        {

            SAPbouiCOM.EventFilters oFilters = null;
            SAPbouiCOM.EventFilter oFilter = null;
            oFilters = new SAPbouiCOM.EventFilters();

         

         
            oFilter = oFilters.Add(SAPbouiCOM.BoEventTypes.et_ALL_EVENTS);
          
            oFilter.AddEx("EJ_OSTS");//Stock Transfer Shipment          
            oFilter.AddEx("EJ_OSTR");//Stock Transfer Receipt
            oFilter.AddEx("139");//Sales Order 
            oFilter.AddEx("179");//AR Credit Note 
            oFilter.AddEx("EJ_OCSH"); //Customer Sales History

            oFilter.AddEx("142");//P Order 
            oFilter.AddEx("181");//AP Credit Note 
            oFilter.AddEx("EJ_OSSH"); //Supplier History


           SetFilter(oFilters);            
        }

        void SetFilter(SAPbouiCOM.EventFilters oFilters)
        {
            oApplication.SetFilter(oFilters);
        }
        #endregion

        #region CheckExpiry

        private bool CheckExpiry()
        {
            bool retVal;
            retVal = true;
            try
            {
                string strExpDate;
                strExpDate = "";
                if (strExpDate != "")
                {
                    IFormatProvider ifp = new System.Globalization.CultureInfo("en-US", true);
                    DateTime dtparamDate = DateTime.ParseExact(strExpDate, "yyyyMMdd", ifp);
                    DateTime dtCurrentDate = DateTime.ParseExact(string.Format("{0:yyyyMMdd}", DateTime.Now), "yyyyMMdd", ifp);
                    if (dtparamDate < dtCurrentDate)
                    {
                        retVal = false;
                    }
                    else
                    {
                        retVal = true;
                    }   
                }
                return retVal;
            }
            catch (Exception ex)
            {
                return retVal;
                throw ex;
            }            
        }

        #endregion

        #region SET MENU

        public void SetMenuItems()
        {
            try
            {
            
              //  CreateMenuFolder("EJ_OWHT", "Warehouse Transfer", "3072", 4);
                //CreateMenuItem(SAPbouiCOM.BoMenuType.mt_STRING, "EJ_OSTS", "Stock Transfer Shipment", 2, "EJ_OWHT");
                //CreateMenuItem(SAPbouiCOM.BoMenuType.mt_STRING, "EJ_OSTR", "Stock Transfer Receipt", 2, "EJ_OWHT");

            }
            catch (Exception e)
            {
                oApplication.MessageBox(e.Message, 1, "Ok", "", "");
            }
        }

        #endregion SET MENU

        #region Menu creation

        private void CreateMenuItem(SAPbouiCOM.BoMenuType mType, string uniqueID, string desc, int position, string menuItemId)
        {
            SAPbouiCOM.Menus Menu = null;
            SAPbouiCOM.MenuItem MenuItem = null;
            Menu = oApplication.Menus;
            string rootPath = System.Windows.Forms.Application.StartupPath;

            SAPbouiCOM.MenuCreationParams CreationPara = null;
            CreationPara = (SAPbouiCOM.MenuCreationParams)(oApplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams));
            MenuItem = oApplication.Menus.Item(menuItemId);

            try
            {
                Menu = MenuItem.SubMenus;
                CreationPara.Type = mType;
                CreationPara.UniqueID = uniqueID;
                CreationPara.String = desc;
                CreationPara.Position = position;
                Menu.AddEx(CreationPara);
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
                //Global.SapApplication.MessageBox(ex.Message, 1, "Ok", "", "");
            }
        }
        #endregion

        #region Menu Folder creation

        public void CreateMenuFolder(string menuId, string MenuName, string ParentId, int Position)
        {
            SAPbouiCOM.Menus oMenus = null;
            SAPbouiCOM.MenuItem oMenuItem = null;
            try
            {
                SAPbouiCOM.MenuCreationParams oCreationPackage = null;
                oCreationPackage = ((SAPbouiCOM.MenuCreationParams)(oApplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)));
                oMenuItem = oApplication.Menus.Item(ParentId); // moudles'

                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_POPUP;
                oCreationPackage.UniqueID = menuId;
                oCreationPackage.String = MenuName;
                oCreationPackage.Enabled = true;
                oCreationPackage.Position = Position;

                oMenus = oMenuItem.SubMenus;
                oMenus.AddEx(oCreationPackage);
            }
            catch (Exception e)
            {
                if (!e.Message.Equals("Menu - Already exists  [66000-68]"))
                {
                    oApplication.MessageBox(e.Message, 1, "Ok", "", "");
                }
            }
        }
        #endregion

        #region Constructor
        public clsStartup()
        {
            if (CheckExpiry())
            {
                Class_Init();
                SAPbouiCOM.Form formCmdCenter;
                formCmdCenter = oApplication.Forms.GetFormByTypeAndCount(169, 1);
                formCmdCenter.Freeze(true);                
                SetMenuItems();
                formCmdCenter.Freeze(false);
                formCmdCenter.Update();

                if (CreateTables())
                {
                    CreateUDOs();
                }

                SetFilter();

                oApplication.StatusBar.SetText("eOrderEntry AddOn Connected Successfully", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);

                oApplication.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(oApplication_ItemEvent);
                oApplication.MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(oApplication_MenuEvent);
                oApplication.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(oApplication_AppEvent);
                oApplication.RightClickEvent += new SAPbouiCOM._IApplicationEvents_RightClickEventEventHandler(oApplication_RightClickEvent);
                oApplication.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(oApplication_FormDataEvent);
                oApplication.LayoutKeyEvent += new SAPbouiCOM._IApplicationEvents_LayoutKeyEventEventHandler(oApplication_LayoutKeyEvent);

              //  CreateAuthorizationTree();
            }
            else
            {
                MessageBox.Show("License Of EJADA SAP Business One additional component has been expired. Contact Administrator");
            }
        }
        #endregion

        #region Events

        public static void oApplication_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            SAPbouiCOM.Form oForm = null;
            BubbleEvent = true;

            try
            {                                        

               
                //Stock Shipment
                if ((pVal.FormTypeEx == "EJ_OSTS") && (pVal.EventType != SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD))
                {
                    oForm = oApplication.Forms.GetForm("EJ_OSTS", pVal.FormTypeCount);
                    WarehouseTransfer.Inventory.clsStockShipment.clsStockShipment_ItemEvent(ref oApplication, ref oCompany, oForm, ref pVal, ref BubbleEvent);
                }

                //Stock Receipt
                if ((pVal.FormTypeEx == "EJ_OSTR") && (pVal.EventType != SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD))
                {
                    oForm = oApplication.Forms.GetForm("EJ_OSTR", pVal.FormTypeCount);
                    WarehouseTransfer.Inventory.clsStockReceipt.clsStockReceipt_ItemEvent(ref oApplication, ref oCompany, oForm, ref pVal, ref BubbleEvent);
                }

                //Sales Order
                if ((pVal.FormTypeEx == "139") && (pVal.EventType != SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD))
                {
                    oForm = oApplication.Forms.GetForm("139", pVal.FormTypeCount);
                    WarehouseTransfer.Sales.clsSalesOrder.clsSalesOrder_ItemEvent(ref oApplication, ref oCompany, oForm, ref pVal, ref BubbleEvent);
                }

                //AR Credit Note
                if ((pVal.FormTypeEx == "179") && (pVal.EventType != SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD))
                {
                    oForm = oApplication.Forms.GetForm("179", pVal.FormTypeCount);
                    WarehouseTransfer.Sales.clsARCreditMemo.clsARCreditMemo_ItemEvent(ref oApplication, ref oCompany, oForm, ref pVal, ref BubbleEvent);
                }

                //Customer Sales History
                if ((pVal.FormTypeEx == "EJ_OCSH") && (pVal.EventType != SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD))
                {
                    oForm = oApplication.Forms.GetForm("EJ_OCSH", pVal.FormTypeCount);
                    WarehouseTransfer.Sales.clsCustomerSalesHistory.clsCustomerSalesHistory_ItemEvent(ref oApplication, ref oCompany, oForm, ref pVal, ref BubbleEvent);
                }

                //Purchase Order
                if ((pVal.FormTypeEx == "142") && (pVal.EventType != SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD))
                {
                    oForm = oApplication.Forms.GetForm("142", pVal.FormTypeCount);
                    WarehouseTransfer.Purchase.clsPO.clsPO_ItemEvent(ref oApplication, ref oCompany, oForm, ref pVal, ref BubbleEvent);
                }

                //AP Credit Note
                if ((pVal.FormTypeEx == "181") && (pVal.EventType != SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD))
                {
                    oForm = oApplication.Forms.GetForm("181", pVal.FormTypeCount);
                    WarehouseTransfer.Purchase.clsAPCreditMemo.clsAPCreditMemo_ItemEvent(ref oApplication, ref oCompany, oForm, ref pVal, ref BubbleEvent);
                }

                //Customer Sales History
                if ((pVal.FormTypeEx == "EJ_OSSH") && (pVal.EventType != SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD))
                {
                    oForm = oApplication.Forms.GetForm("EJ_OSSH", pVal.FormTypeCount);
                    WarehouseTransfer.Purchase.clsSupplierPurchaseHistory.clsSupplierPurchaseHistory_ItemEvent(ref oApplication, ref oCompany, oForm, ref pVal, ref BubbleEvent);
                }
                
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message + "\n" + oCompany.GetLastErrorDescription(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
              //  MessageBox.Show(ex.Source.ToString() + "\n" + oCompany.GetLastErrorDescription(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }

        private void oApplication_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            SAPbouiCOM.Form oForm = null;            

            try
            {
                SAPbouiCOM.MenuItem oMenuItems = (SAPbouiCOM.MenuItem)oApplication.Menus.Item("47616");                
                //SAPbouiCOM.MenuItem oMenuItems = (SAPbouiCOM.MenuItem)oApplication.Menus.Item("43520");                

                try
                {
                    oForm = oApplication.Forms.ActiveForm;
                }
                catch (Exception)
                {
                    
                }

                // Stock Shipment
                if (pVal.MenuUID == "EJ_OSTS" && pVal.BeforeAction == false)
                {
                    WarehouseTransfer.Inventory.clsStockShipment oStockShipment = new WarehouseTransfer.Inventory.clsStockShipment(ref oApplication, ref oCompany);
                }

                // Stock Receipt
                if (pVal.MenuUID == "EJ_OSTR" && pVal.BeforeAction == false)
                {
                    WarehouseTransfer.Inventory.clsStockReceipt oStockReceipt = new WarehouseTransfer.Inventory.clsStockReceipt(ref oApplication, ref oCompany);
                }

              

                if (oForm != null)
                {
                    // Stock Shipment
                    if (oForm.TypeEx == "EJ_OSTS")
                    {
                        WarehouseTransfer.Inventory.clsStockShipment.clsStockShipment_MenuEvent(ref oApplication, ref oCompany, oForm, ref pVal, ref BubbleEvent);
                    }                   

                    // Stock Receipt
                    if (oForm.TypeEx == "EJ_OSTR")
                    {
                        WarehouseTransfer.Inventory.clsStockReceipt.clsStockReceipt_MenuEvent(ref oApplication, ref oCompany, oForm, ref pVal, ref BubbleEvent);
                    } 
                }

            }
            catch (Exception ex)
            { 
                MessageBox.Show(ex.Message + "\n" + oCompany.GetLastErrorDescription(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }

        private void oApplication_RightClickEvent(ref SAPbouiCOM.ContextMenuInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            SAPbouiCOM.Form oForm = null;
            try
            {
                oForm = oApplication.Forms.ActiveForm;

             
                //Stock Transfer Shipment
                if (oForm.TypeEx == "EJ_OSTS")
                {
                    WarehouseTransfer.Inventory.clsStockShipment.clsStockShipment_RightClickEvent(ref oApplication, ref oCompany, oForm, ref eventInfo, ref BubbleEvent);
                }

                //Stock Transfer Receipt
                if (oForm.TypeEx == "EJ_OSTR")
                {
                    WarehouseTransfer.Inventory.clsStockReceipt.clsStockReceipt_RightClickEvent(ref oApplication, ref oCompany, oForm, ref eventInfo, ref BubbleEvent);
                }

                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void oApplication_FormDataEvent(ref SAPbouiCOM.BusinessObjectInfo oBusinessInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            SAPbouiCOM.Form oForm = null;
            try
            {
              //  oForm = oApplication.Forms.GetForm(oBusinessInfo.FormTypeEx, 0);
                oForm = oApplication.Forms.Item(oBusinessInfo.FormUID);

              

                // Stock Transfer Shipment
                if (oBusinessInfo.FormTypeEx == "EJ_OSTS")
                {
                    WarehouseTransfer.Inventory.clsStockShipment.clsStockShipment_FormDataEvent(ref oApplication, ref oCompany, oForm, ref oBusinessInfo, ref BubbleEvent);
                }

                // Stock Transfer Receipt
                if (oBusinessInfo.FormTypeEx == "EJ_OSTR")
                {
                    WarehouseTransfer.Inventory.clsStockReceipt.clsStockReceipt_FormDataEvent(ref oApplication, ref oCompany, oForm, ref oBusinessInfo, ref BubbleEvent);
                } 
                
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message + "\n" + oCompany.GetLastErrorDescription(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        public static void oApplication_LayoutKeyEvent(ref SAPbouiCOM.LayoutKeyInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = false;
            SAPbouiCOM.Form oForm = null;
            oForm = oApplication.Forms.ActiveForm;

            switch (oForm.TypeEx)
            {
                
                default:
                    break;
            }
        }

        private void oApplication_AppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            if (EventType == SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged || EventType == SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged || EventType == SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition || EventType == SAPbouiCOM.BoAppEventTypes.aet_ShutDown)
            {
                //string strFileName = (System.Windows.Forms.Application.StartupPath).ToString() + @"\" + "Remove Menus.xml";
                //clsSBO.LoadFromXML(ref strFileName, oApplication);
                oApplication.SetStatusBarMessage("eOrderEntry Addon Disconnected successfully - SAP", SAPbouiCOM.BoMessageTime.bmt_Short, false);
                GC.Collect();
                System.Windows.Forms.Application.Exit();
                TmpThread.Start();
            }
        }    

        private void CreateAuthorizationTree()
        {
            // Sample
            //AddAuthorization("BZ_OMRQ", "Material Requisition", "GMART", SAPbobsCOM.BoUPTOptions.bou_FullReadNone, "BZ_OMRQ");
           
        }

        private void AddAuthorization(string strPermissionID, string strName, string strParentID, SAPbobsCOM.BoUPTOptions Option, string strFormId)
        {
            SAPbobsCOM.UserPermissionTree oUserPerTree;
            oUserPerTree = (SAPbobsCOM.UserPermissionTree)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserPermissionTree);

            if (!oUserPerTree.GetByKey(strPermissionID))
            {
                oUserPerTree.Name = strName;
                oUserPerTree.PermissionID = strPermissionID;

                if (strParentID != "")
                {
                    oUserPerTree.ParentID = strParentID;
                    oUserPerTree.UserPermissionForms.FormType = strFormId;
                    oUserPerTree.IsItem = SAPbobsCOM.BoYesNoEnum.tYES;
                }

                oUserPerTree.Options = Option;
                int intStatus;
                intStatus = oUserPerTree.Add();

                string strErro = oCompany.GetLastErrorDescription();
            }          

        }

        public static string LinkReportToForm(SAPbouiCOM.Application SBO_Application, SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oUserForm, string ReportName, string LayoutReportName, string strFormName,string strReportFilePath)
        {
            string strRptType;
            strRptType = "";
            bool LinkExist = false;

            SAPbobsCOM.ReportTypesService rptTypeService = null;
            SAPbobsCOM.ReportType newType = null;
            SAPbobsCOM.ReportLayoutsService rptService = null;
            SAPbobsCOM.ReportLayout newReport = null;
            SAPbobsCOM.ReportTypeParams ExistingReportTypeParam = null;

            try
            {
                rptTypeService = (SAPbobsCOM.ReportTypesService)oCompany.GetCompanyService().GetBusinessService(SAPbobsCOM.ServiceTypes.ReportTypesService);


                SAPbobsCOM.ReportTypeParams ReportTypeParams = rptTypeService.GetReportTypeList() as SAPbobsCOM.ReportTypeParams;

                foreach (SAPbobsCOM.ReportTypeParams rtp in rptTypeService.GetReportTypeList())
                    if (rtp.AddonName == strFormName)
                    {
                        ExistingReportTypeParam = rtp;
                        LinkExist = true;
                        break;
                    }

                if (!LinkExist)
                {
                    newType = (SAPbobsCOM.ReportType)rptTypeService.GetDataInterface(SAPbobsCOM.ReportTypesServiceDataInterfaces.rtsReportType);
                    newType.TypeName = LayoutReportName;
                    newType.AddonName = strFormName;
                    newType.AddonFormType = strFormName;
                    newType.MenuID = ReportName;
                    SAPbobsCOM.ReportTypeParams newTypeParam = rptTypeService.AddReportType(newType);


                    rptService = (SAPbobsCOM.ReportLayoutsService)oCompany.GetCompanyService().GetBusinessService(SAPbobsCOM.ServiceTypes.ReportLayoutsService);
                    newReport = (SAPbobsCOM.ReportLayout)rptService.GetDataInterface(SAPbobsCOM.ReportLayoutsServiceDataInterfaces.rlsdiReportLayout);
                    newReport.Author = oCompany.UserName;
                    newReport.Category = SAPbobsCOM.ReportLayoutCategoryEnum.rlcCrystal;
                    newReport.Name = newType.TypeName;
                    newReport.TypeCode = newTypeParam.TypeCode;
                    SAPbobsCOM.ReportLayoutParams newReportParam = rptService.AddReportLayout(newReport);


                    newType = rptTypeService.GetReportType(newTypeParam);
                    newType.DefaultReportLayout = newReportParam.LayoutCode;
                    rptTypeService.UpdateReportType(newType);

                    SAPbobsCOM.BlobParams oBlobParams = (SAPbobsCOM.BlobParams)oCompany.GetCompanyService().GetDataInterface(SAPbobsCOM.CompanyServiceDataInterfaces.csdiBlobParams);
                    oBlobParams.Table = "RDOC";
                    oBlobParams.Field = "Template";
                    SAPbobsCOM.BlobTableKeySegment oKeySegment = oBlobParams.BlobTableKeySegments.Add();
                    oKeySegment.Name = "DocCode";
                    oKeySegment.Value = newReportParam.LayoutCode;

                    System.IO.FileStream oFile = new System.IO.FileStream(strReportFilePath, System.IO.FileMode.Open);
                    int fileSize = (int)oFile.Length;
                    byte[] buf = new byte[fileSize];
                    oFile.Read(buf, 0, fileSize);
                    oFile.Dispose();
                    SAPbobsCOM.Blob oBlob = (SAPbobsCOM.Blob)oCompany.GetCompanyService().GetDataInterface(SAPbobsCOM.CompanyServiceDataInterfaces.csdiBlob);
                    oBlob.Content = Convert.ToBase64String(buf, 0, fileSize);
                    oCompany.GetCompanyService().SetBlob(oBlobParams, oBlob);
                                          
                    strRptType = newType.TypeCode;
                }
                else
                {                     
                    strRptType = ExistingReportTypeParam.TypeCode;
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRptType;
        }

        
        #endregion

    }
}

