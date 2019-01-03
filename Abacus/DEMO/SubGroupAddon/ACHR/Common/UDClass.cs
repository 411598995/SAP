using System;

	
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
	
using System.Diagnostics;
using System.Threading;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Management;
using Microsoft.CSharp;
using System.Security.Cryptography;
using System.Text;

using System.Reflection;
using System.IO;
using SAPbobsCOM;
using SAPbouiCOM;
using System.Resources;

using System.Net;


namespace ACHR
{


    public class UDClass
    {
        long v_RetVal;
        int v_ErrCode;
        string v_ErrMsg = "";
        public SAPbobsCOM.Company oCompany;
        public SAPbobsCOM.Company oDiCompany;
        
        string FileName;
        string defFileName;
        public Hashtable StringMessages = new Hashtable();
        public System.Data.DataTable LOVs = new System.Data.DataTable();
        public System.Data.DataTable AllLovs = new System.Data.DataTable();
        public SAPbouiCOM.Application oApplication;
        public string HRMSDbName = "";
        public string HRMSDbServer = "";
        public string HRMSDBuid = "";
        public string HRMSDbPwd = "";
        public string HRMSLicHash = "";
        public string hrConstr = "";
        public string HRMServerType = "";
        public bool CalculateTax = false;
        public bool isDIConnected = false;
        public string showReportCode = "";
        public bool isSystemReport = false;
        public string rptCritaria = "";
        private static System.Timers.Timer aTimer;
        public static bool busy = false;

        List<string> frmList = new List<string>();
      
        public System.Data.DataTable dtRetScaned;
        public UDClass(string connectString)
        {
            string errmsg = "";
            try
            {
                SboGuiApi sboApi = new SboGuiApi();
                if (connectString == "")
                {
                    MessageBox.Show("Add-on must be run from SAP Business One.");
                }
                

                
                sboApi.Connect(connectString);
                 oApplication = sboApi.GetApplication();
                oCompany = new SAPbobsCOM.Company();
                string sCookies = oCompany.GetContextCookie();
                string conStr = oApplication.Company.GetConnectionContext(sCookies);

              
                int ret = oCompany.SetSboLoginContext(conStr);
                int ret2 = 0;//; oCompany.Connect();
                
              oCompany = (SAPbobsCOM.Company) oApplication.Company.GetDICompany();
                if (ret2 == 0)
                {

                    frmList = new List<string> { "mnu_ProdList"};
                    
                    frmList.Add("134");
                    frmList.Add("63");
                    frmList.Add("150");
                    oApplication.StatusBar.SetText("Addon Connected Successfully.!", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    oApplication.MenuEvent += new _IApplicationEvents_MenuEventEventHandler(oApplication_MenuEvent);
                    oApplication.AppEvent += new _IApplicationEvents_AppEventEventHandler(oApplication_appEvent);
                    oApplication.ItemEvent += new _IApplicationEvents_ItemEventEventHandler(oApplication_ItemEvent);
                    oApplication.FormDataEvent += new _IApplicationEvents_FormDataEventEventHandler(oApplication_FormDataEvent);

                    string lang = oApplication.Language.ToString();
                    Program.sboLanguage = lang;
                    try
                    {
                        int langnum = Convert.ToInt16(oApplication.Language.ToString());
                        lang = "_" + lang;
                    }
                    catch
                    {

                    }
                  
                    if(lang.Contains("English"))
                    {
                        lang = "ln_English";
                    }
                   
                      
                    Program.sboLanguage = lang;
                    loadMenu(lang);
                    Program.sboLanguage = lang;

                    createConfigurationTables();

                  //  execTimer();
                }

                else
                {
                     MessageBox.Show(oCompany.GetLastErrorDescription());
                     MessageBox.Show("UI Not connected");
                     Environment.Exit(0);
                        
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to connect company!" + ex.Message);

                Environment.Exit(0);
            }
            
        }


        public void execTimer()
        {
            aTimer = new System.Timers.Timer(1);
            aTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 1 * 60 * 1000;
            aTimer.Enabled = true;
        }

        public void createConfigurationTables()
        {

            string result = "Creating Configuration Table";


      
            AddTable("B1_ITB", "Sub Groups", BoUTBTableType.bott_NoObject);
            AddColumns("@B1_ITB", "SubGrp", "Sub Group", SAPbobsCOM.BoFieldTypes.db_Alpha, 100, SAPbobsCOM.BoFldSubTypes.st_None);
            AddColumns("@B1_ITB", "Father", "Father Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 8, SAPbobsCOM.BoFldSubTypes.st_None);

            AddColumns("OITM", "SubGrp", "Sub Group", SAPbobsCOM.BoFieldTypes.db_Alpha, 8, SAPbobsCOM.BoFldSubTypes.st_None, "B1_ITB");



        }
        private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            if (!busy)
            {
                busy = true;
                busy = false;
            }
            //sendMessage();
        }

        private void loadMenu(string lang)
        {
            //SAPbouiCOM.Menus mnus = oApplication.Menus;

            //if (mnus.Exists("mnu_ProdList"))
            //{
            //    mnus.RemoveEx("mnu_ProdList");
            //}
            //mnus.Item("4352").SubMenus.Add("mnu_ProdList", "Production Sequencer", BoMenuType.mt_STRING,2);
        

         
         


        }




     
        public virtual void oApplication_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {


            BubbleEvent = true;


            try
            {
                if (pVal.EventType == BoEventTypes.et_FORM_LOAD && pVal.Before_Action == false)
                {
                    if (frmList.Contains(pVal.FormTypeEx) )
                    {
                        try
                        {
                            Type oFormType = Type.GetType("ACHR.Screen." + "frm_" + pVal.FormTypeEx);
                            Screen.SysBaseForm objScr = ((Screen.SysBaseForm)oFormType.GetConstructor(System.Type.EmptyTypes).Invoke(null));
                            objScr.CreateForm(oApplication, oCompany, pVal.FormUID);
                            objScr.etFormAfterLoad(ref pVal, ref BubbleEvent);
                        }
                        catch { }
                    }

                }

            }
            catch (Exception ex)
            {
                oApplication.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Medium, true);
            }
        }
        private void fillDtFromTemplate(System.Data.DataTable dt , string pfileName )
        {
            string fileName = pfileName;
            using (StreamReader file = new StreamReader(fileName))
            {
                string line = "";
                string[] pastrts;
              
               line= file.ReadLine();
                pastrts = line.Split('\t');
                foreach (string colName in pastrts)
                {
                    dt.Columns.Add(colName);
                }
                while ("a" == "a")
                {
                    line = file.ReadLine();
                    if (line == null) break;
                    pastrts = line.Split('\t');
                    dt.Rows.Add(pastrts);
                    // dt.Rows.Add(pastrts(0), pastrts(1), pastrts(2), pastrts(3), pastrts(4), pastrts(5), pastrts(6), pastrts(7), pastrts(8), pastrts(9), pastrts(10), pastrts(11))
                }
            }
        }
       
    
        private void oApplication_appEvent(BoAppEventTypes EventType)
        {

            try
            {
                SAPbouiCOM.Menus mnus = oApplication.Menus;

            
                switch (EventType)
                {
                               case BoAppEventTypes.aet_CompanyChanged:
                       

                        if (mnus.Exists("B1_STK"))
                        {
                            mnus.RemoveEx("B1_STK");
                        }
                        System.Windows.Forms.Application.Exit();
                        break;
                    case BoAppEventTypes.aet_ShutDown:
                       
                        if (mnus.Exists("B1_STK"))
                        {
                            mnus.RemoveEx("B1_STK");
                        }
                        System.Windows.Forms.Application.Exit();
                        break;
                    case BoAppEventTypes.aet_LanguageChanged:
                        string lang = oApplication.Language.ToString();
                        Program.sboLanguage = lang;
                        try
                        {
                            int langnum = Convert.ToInt16(oApplication.Language.ToString());
                            lang = "_" + lang;
                        }
                        catch
                        {

                        }
                        loadMenu(lang);
                        break;


                }
            }
            catch (Exception ex)
            {
                oApplication.SetStatusBarMessage(ex.Message);
            }
        }
        public string getAcctName(string strAcctCode)
        {
            string strOut = "";

            SAPbobsCOM.Recordset oRecSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

            oRecSet.DoQuery("select acctname from oact where oact.acctcode='" + strAcctCode + "'");
            if (oRecSet.EoF)
            {
                strOut = "Not Found";
                return strOut;
            }
            strOut = oRecSet.Fields.Item("AcctName").Value.ToString();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecSet);
            oRecSet = null;
           

            return strOut;
        }
        public string getCardCode(string strCardCode)
        {
            string strOut = "";

            SAPbobsCOM.Recordset oRecSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

            oRecSet.DoQuery("select cardcode from ocrd where ocrd.cardcode='" + strCardCode + "'");
            if (oRecSet.EoF)
            {
                strOut = "Not Found";
                return strOut;
            }
            strOut = oRecSet.Fields.Item("CardCode").Value.ToString();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecSet);
            oRecSet = null;


            return strOut;
        }
        public string doSAPNonQuery(string strSql)
        {
            string result = "Ok";



            SAPbobsCOM.Recordset oRecSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

            try
            {
                oRecSet.DoQuery(strSql);
              
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            finally
            {

                System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecSet);
                oRecSet = null;
            }
            return result;
        }
        
        
      
        
        public string getStrMsg(string strKey)
        {
            string outStr = "Un-Known Message";

            try
            {
                outStr = StringMessages[strKey].ToString();
            }
            catch { }

            return outStr;
        }
        
        public void oApplication_FormDataEvent(ref BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
        
           
       

        }
        private void createCouponReport(SAPbouiCOM.Form couponForm )
        {
            
           


        }
       
        public void oApplication_MenuEvent(ref MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
           
            string mnuFrm = pVal.MenuUID;
       
            if (!frmList.Contains(pVal.MenuUID))
            {
                return;
            }


            try
            {
                if (!pVal.BeforeAction)
                {
                    if (pVal.MenuUID.Contains("mnu_"))
                    {
                        string strLang = oApplication.Language.ToString();

                        strLang = "ln_English";

                        string comName = pVal.MenuUID.Replace("mnu_", "");
                        try
                        {
                            oApplication.Forms.Item("frm_" + comName).Select();
                            oApplication.Forms.Item("frm_" + comName).Visible = true;
                        }
                        catch
                        {
                            Type oFormType = Type.GetType("ACHR.Screen." + "frm_" + comName);
                            Screen.HRMSBaseForm objScr = ((Screen.HRMSBaseForm)oFormType.GetConstructor(System.Type.EmptyTypes).Invoke(null));

                            List<string> checkValues = new List<string> { "EmpVL", "Conf" };

                            if (checkValues.Contains(comName))
                            {
                                objScr.CreateForm(oApplication, "ACHR.XMLScreen." + strLang + ".xml_" + comName + ".srf", oCompany, "frm_" + comName);
                            }

                            else
                            {
                                objScr.CreateForm(oApplication, "ACHR.XMLScreen." + strLang + ".xml_" + comName + ".xml", oCompany, "frm_" + comName);
                            }
                        }
                    }
                   

                }
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Unhandeled Exception Caught at General Class:" + ex.Message);
            }
        }

        public UDClass(ref SAPbobsCOM.Company comp, ref SAPbouiCOM.Application app)
        {
            oCompany = comp;
            oApplication = app;

        }

        public bool ColumnExists(string TableName, string FieldID)
        {
            bool oFlag = true;
            try
            {
                SAPbobsCOM.Recordset rsetField = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                string s = "Select 1 from [CUFD] Where TableID='" + TableName.Trim() + "' and AliasID='" + FieldID.Trim() + "'";
                rsetField.DoQuery("Select 1 from [CUFD] Where TableID='" + TableName.Trim() + "' and AliasID='" + FieldID.Trim() + "'");
                if (rsetField.EoF)
                    oFlag = false;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rsetField);
                rsetField = null;
                GC.Collect();
                return oFlag;
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed to Column Exists : " + ex.Message);
            }
            finally
            {
            }
            return oFlag;
        }
        
        public int  ExecQuery(string sql, string CallerRef)
        {

            
            int result = 0;
            SAPbobsCOM.Recordset rs = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {
                rs.DoQuery(sql);
               

            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed in Exec Query on " + CallerRef + " : " + ex.Message);
                result = -1;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
                rs = null;

            }
            return result;
        }

        public System.Data.DataTable getDataTable(string sql, string CallerRef)
        {
            System.Data.DataTable dtOut = new System.Data.DataTable();
            SAPbobsCOM.Recordset rs = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {
                rs.DoQuery(sql);
               
                    for (int i=0;i<rs.Fields.Count;i++)
                    {
                        dtOut.Columns.Add(rs.Fields.Item(i).Description);
                    }
               

                while (!rs.EoF)
                {
                    DataRow nr = dtOut.NewRow();
                    for (int i = 0; i < rs.Fields.Count; i++)
                    {
                        nr[i] = rs.Fields.Item(i).Value;
                    }
                    dtOut.Rows.Add(nr);
                    rs.MoveNext();
                }

            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed in Exec Query on " + CallerRef + " : " + ex.Message);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
                rs = null;

            }
            return dtOut;
        }

        public string getScallerValue(string strSql)
        {
            string strResult = "";
            System.Data.DataTable dtScaller = getDataTable(strSql,"GetScaller of UDClass");

            if (dtScaller.Rows.Count > 0)
            {
                strResult = dtScaller.Rows[0][0].ToString();
            }

            return strResult;
        }
        public string getSetting(string settingcode)
        {
            string settingVal = "";
            string strSql = " Select U_Value   as setting from [@adr_setting] where code='" + settingcode  + "' ";
            try
            {
                settingVal  = Convert.ToString (getScallerValue(strSql)) ;
            }
            catch { }


            return settingVal;
        }
        public long getMaxId(string tblName, string idCol)
        {
            long nextId = 1;
            string strSql = " Select isnull(max(convert(int," + idCol + ")),0)  as nextId from " + tblName;
            try
            {
                nextId = Convert.ToInt32(getScallerValue(strSql)) +1;
            }
            catch { }
            return nextId;
        }
        public void ExecFileQuery(string filePath, string callerRef)
        {

            try
            {

                System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath);
                StreamReader reader = new StreamReader(stream);

                string strSql = reader.ReadToEnd();

                ExecQuery(strSql, callerRef);


            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed to execute pat" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            finally
            {
            }
        }

        public bool AddColumns(string TableName, string Name, string Description, SAPbobsCOM.BoFieldTypes Type, int Size = 0, SAPbobsCOM.BoFldSubTypes SubType = SAPbobsCOM.BoFldSubTypes.st_None, string LinkedTable = "", string[,] LOV = null, string DefV = "")
        {
            bool outResult = false;
            try
            {
                SAPbobsCOM.UserFieldsMD v_UserField = default(SAPbobsCOM.UserFieldsMD);

                if (TableName.StartsWith("@") == true)
                {
                    if (!ColumnExists(TableName, Name))
                    {
                        v_UserField = (SAPbobsCOM.UserFieldsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
                        v_UserField.TableName = TableName;
                        v_UserField.Name = Name;
                        if (!string.IsNullOrEmpty(DefV))
                        {
                            v_UserField.DefaultValue = DefV;
                        }

                        if (LOV == null)
                        {
                        }
                        else
                        {
                            for (int k = 0; k <= LOV.Length - 1; k++)
                            {
                                v_UserField.ValidValues.Value = LOV[k, 0];
                                v_UserField.ValidValues.Value = LOV[k, 1];
                                v_UserField.ValidValues.Add();
                            }

                        }

                        v_UserField.Description = Description;
                        v_UserField.Type = Type;
                        if (Type != SAPbobsCOM.BoFieldTypes.db_Date)
                        {
                            if (Size != 0)
                            {
                                v_UserField.Size = Convert.ToInt16(Size);
                                v_UserField.EditSize = Convert.ToInt16(Size);
                            }
                        }
                        if (SubType != SAPbobsCOM.BoFldSubTypes.st_None)
                        {
                            v_UserField.SubType = SubType;
                        }
                        if (!string.IsNullOrEmpty(LinkedTable))
                            v_UserField.LinkedTable = LinkedTable;
                        v_RetVal = v_UserField.Add();
                        if (v_RetVal != 0)
                        {
                            oCompany.GetLastError(out v_ErrCode, out v_ErrMsg);
                            oApplication.StatusBar.SetText("Failed to add UserField " + Description + " - " + v_ErrCode + " " + v_ErrMsg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            return false;
                        }
                        else
                        {
                            oApplication.StatusBar.SetText("[@" + TableName + "] - " + Description + " added successfully!!!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                            outResult = true;
                            return true;
                        }
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UserField);
                        v_UserField = null;
                    }
                    else
                    {
                        return false;
                    }
                }


                if (TableName.StartsWith("@") == false)
                {
                    if (!UDFExists(TableName, Name))
                    {
                        v_UserField = (SAPbobsCOM.UserFieldsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
                        v_UserField.TableName = TableName;
                        v_UserField.Name = Name;
                        if (!string.IsNullOrEmpty(DefV))
                        {
                            v_UserField.DefaultValue = DefV;
                        }

                        if (LOV == null)
                        {
                        }
                        else
                        {
                            for (int k = 0; k <= LOV.Length / 2 - 1; k++)
                            {
                                v_UserField.ValidValues.Value = LOV[k, 0];
                                v_UserField.ValidValues.Description = LOV[k, 1];
                                v_UserField.ValidValues.Add();
                            }

                        }
                        v_UserField.Description = Description;
                        v_UserField.Type = Type;
                        if (Type != SAPbobsCOM.BoFieldTypes.db_Date)
                        {
                            if (Size != 0)
                            {
                                v_UserField.Size = Size;
                                v_UserField.EditSize = Size;
                            }
                        }
                        if (SubType != SAPbobsCOM.BoFldSubTypes.st_None)
                        {
                            v_UserField.SubType = SubType;
                        }
                        if (!string.IsNullOrEmpty(LinkedTable))
                            v_UserField.LinkedTable = LinkedTable;
                        v_RetVal = v_UserField.Add();
                        if (v_RetVal != 0)
                        {
                            oCompany.GetLastError(out v_ErrCode, out v_ErrMsg);
                            oApplication.StatusBar.SetText("Failed to add UserField " + Description + " - " + v_ErrCode + " " + v_ErrMsg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            return false;
                        }
                        else
                        {
                            oApplication.StatusBar.SetText("[@" + TableName + "] - " + Description + " added successfully!!!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                            outResult = true;
                            return true;
                        }
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UserField);
                        v_UserField = null;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed to Add Columns : " + ex.Message);
            }
            finally
            {
            }
            return outResult;
        }
        
        public void AddXML(string pathstr)
        {
            try
            {
                System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
                System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(pathstr);
                System.IO.StreamReader streamreader = new System.IO.StreamReader(stream, true);
                xmldoc.LoadXml(streamreader.ReadToEnd());
                streamreader.Close();
                oApplication.LoadBatchActions(xmldoc.InnerXml);
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed to Load XML,AddXMl Method Failed" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            finally
            {
            }
        }
        
        public void CopyStream(ref Stream input, ref Stream output)
        {
            // Insert null checking here for production
            byte[] buffer = new byte[8193];
            int bytesRead = 1;
            while ((bytesRead > 0))
            {
                bytesRead = input.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    output.Write(buffer, 0, bytesRead);
                }

            }
        }
        
        public void DownloadEmbFile(string pathstr)
        {
            try
            {
                string strFileName = SaveFile(pathstr);


                if (!string.IsNullOrEmpty(strFileName))
                {

                    System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(pathstr);

                    byte[] buf = new byte[stream.Length + 1];
                    stream.Read(buf, 0, buf.Length);
                    File.WriteAllBytes(strFileName, buf);
                    oApplication.MessageBox("File saved successfully !");
                    //streamwriter.WriteLine(streamreader.ReadLine())
                }

            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed to Load XML,AddXMl Method Failed" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            finally
            {
            }
        }
        
        public bool UDOExists(string code)
        {
            bool outResult = false;
            try
            {
                SAPbobsCOM.UserObjectsMD v_UDOMD = default(SAPbobsCOM.UserObjectsMD);
                bool v_ReturnCode = false;

                GC.Collect();
                v_UDOMD = (SAPbobsCOM.UserObjectsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);
                v_ReturnCode = v_UDOMD.GetByKey(code);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UDOMD);
                v_UDOMD = null;
                outResult = v_ReturnCode;
                return v_ReturnCode;
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed to UDO Exists : " + ex.Message);
            }
            finally
            {
            }
            return outResult;
        }
        
        public bool registerUDO(string UDOCode, string UDOName, SAPbobsCOM.BoUDOObjType UDOType, string[,] findAliasNDescription, string parentTableName, string childTable1 = "", string childTable2 = "", string childTable3 = "", string childTable4 = "", SAPbobsCOM.BoYesNoEnum LogOption = SAPbobsCOM.BoYesNoEnum.tNO, string MenuId = "", int parrentId = 0)
        {
            bool functionReturnValue = false;

            try
            {
                bool actionSuccess = false;
                SAPbobsCOM.UserObjectsMD v_udoMD = default(SAPbobsCOM.UserObjectsMD);

                functionReturnValue = false;
                v_udoMD = (SAPbobsCOM.UserObjectsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);
                v_udoMD.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES;
                v_udoMD.CanClose = SAPbobsCOM.BoYesNoEnum.tYES;
                v_udoMD.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO;
                v_udoMD.CanDelete = SAPbobsCOM.BoYesNoEnum.tYES;
                v_udoMD.CanFind = SAPbobsCOM.BoYesNoEnum.tYES;
                v_udoMD.CanLog = SAPbobsCOM.BoYesNoEnum.tNO;
                v_udoMD.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tYES;
                v_udoMD.ManageSeries = SAPbobsCOM.BoYesNoEnum.tYES;
                if (!string.IsNullOrEmpty(MenuId))
                {
                    v_udoMD.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tYES;
                    //v_udoMD.RebuildEnhancedForm = SAPbobsCOM.BoYesNoEnum.tYES
                    v_udoMD.MenuItem = SAPbobsCOM.BoYesNoEnum.tYES;

                    v_udoMD.MenuUID = MenuId;
                    v_udoMD.MenuCaption = UDOName;
                    // v_udoMD.EnableEnhancedForm = SAPbobsCOM.BoYesNoEnum.tYES
                    v_udoMD.FatherMenuID = parrentId;
                    v_udoMD.Position = 2;
                }

                v_udoMD.Code = UDOCode;
                v_udoMD.Name = UDOName;
                v_udoMD.TableName = parentTableName;
                if (LogOption == SAPbobsCOM.BoYesNoEnum.tYES)
                {
                    v_udoMD.CanLog = SAPbobsCOM.BoYesNoEnum.tYES;
                    v_udoMD.LogTableName = "A" + parentTableName;
                }
                v_udoMD.ObjectType = UDOType;
                for (Int16 i = 0; i <= findAliasNDescription.GetLength(0) - 1; i++)
                {
                    if (i > 0)
                        v_udoMD.FindColumns.Add();
                    v_udoMD.FindColumns.ColumnAlias = findAliasNDescription[i, 0];
                    v_udoMD.FindColumns.ColumnDescription = findAliasNDescription[i, 1];
                }
                if (!string.IsNullOrEmpty(childTable1))
                {
                    v_udoMD.ChildTables.TableName = childTable1;
                    v_udoMD.ChildTables.Add();
                }
                if (!string.IsNullOrEmpty(childTable2))
                {
                    v_udoMD.ChildTables.TableName = childTable2;
                    v_udoMD.ChildTables.Add();
                }
                if (!string.IsNullOrEmpty(childTable3))
                {
                    v_udoMD.ChildTables.TableName = childTable3;
                    v_udoMD.ChildTables.Add();
                }
                if (!string.IsNullOrEmpty(childTable4))
                {
                    v_udoMD.ChildTables.TableName = childTable4;
                    v_udoMD.ChildTables.Add();
                }
                if (v_udoMD.Add() == 0)
                {
                    functionReturnValue = true;
                    oApplication.StatusBar.SetText("Successfully Registered UDO >" + UDOCode.ToString() + ">" + UDOName.ToString() + " >" + oCompany.GetLastErrorDescription(), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }
                else
                {
                    oApplication.StatusBar.SetText("Failed to Register UDO >" + UDOCode + ">" + UDOName + " >" + oCompany.GetLastErrorDescription(), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    functionReturnValue = false;
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(v_udoMD);
                v_udoMD = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed to UDO Register : " + ex.Message);
            }
            finally
            {
            }
            return functionReturnValue;
        }
        
        public bool TableExists(string TableName)
        {
            bool outResult = false;
            try
            {
                SAPbobsCOM.UserTablesMD oTables = default(SAPbobsCOM.UserTablesMD);
                bool oFlag = false;

                oTables = (SAPbobsCOM.UserTablesMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);
                oFlag = oTables.GetByKey(TableName);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oTables);
                outResult = oFlag;
                return oFlag;
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed to Table Exists : " + ex.Message);
            }
            finally
            {
            }
            return outResult;
        }
        
        public void addQryCategor(string catName)
        {
            try
            {
                SAPbobsCOM.QueryCategories qCat = default(SAPbobsCOM.QueryCategories);
                qCat = (SAPbobsCOM.QueryCategories)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oQueryCategories);
                qCat.Name = catName;
                qCat.Add();

            }
            catch { }


        }
        
        public int addQuery(string strQuery, string queryName)
        {
            int queryId = 0;
            int catId = 0;

            SAPbobsCOM.Recordset oRecSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            oRecSet.DoQuery("select CategoryId from OQCN where CatName='HRMS Payroll'");
            if (!oRecSet.EoF)
            {
                catId = Convert.ToInt32( oRecSet.Fields.Item("CategoryId").Value);


            }
            else
            {
                addQryCategor("HRMS Payroll");
                oRecSet.DoQuery("select CategoryId from OQCN where CatName='HRMS Payroll'");
                if (!oRecSet.EoF)
                {
                    catId = Convert.ToInt32( oRecSet.Fields.Item("CategoryId").Value);


                }
            }

            oRecSet.DoQuery("select intrnalKey as qId from ouqr where QName ='" + queryName + "'");
            if (!oRecSet.EoF)
            {
                queryId = Convert.ToInt32(oRecSet.Fields.Item("qId").Value);
            }
            else
            {
                oRecSet.DoQuery("select isnull(max(intrnalKey),0) +1 as newId from ouqr");
                queryId = Convert.ToInt32(oRecSet.Fields.Item("newId").Value);
                string sQuery = " insert into ouqr ([IntrnalKey] ,[QCategory] ,[QName] ,[QString] ,[QType] ) ";
                sQuery += " values ('" + queryId.ToString() + "','" + catId.ToString() + "','" + queryName + "','" + strQuery + "','W')";
                oRecSet.DoQuery(sQuery);
            }
            oRecSet = null;

            return queryId;
        }
        
        public void addFms(string frmId, string itmId, string colID, string query)
        {

            int queryId = 0;
            int fmsId = 0;

            SAPbobsCOM.Recordset oRecSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                oRecSet.DoQuery("select QueryId,IndexID from CSHS where formId='" + frmId + "' and ItemId='" + itmId + "' and colID='" + colID + "'");
                if (!oRecSet.EoF)
                {
                    queryId = Convert.ToInt32(oRecSet.Fields.Item("QueryId").Value);
                    fmsId = Convert.ToInt32(oRecSet.Fields.Item("IndexID").Value);
                    oRecSet.DoQuery("update OUQR set qString='" + query + "' where intrnalKey='" + queryId.ToString() + "'");

                }
                else
                {
                    oRecSet.DoQuery("select isnull(max(IndexID),0) +1 as fmsId from CSHS");
                    fmsId = Convert.ToInt32(oRecSet.Fields.Item("fmsId").Value);
                    queryId = addQuery(query, "Fms_" + frmId + "_" + itmId + "_" + colID);

                    string strS = "INSERT into [CSHS] ([FormID] ,[ItemID] ,[ColID] ,[ActionT] ,[QueryId] ,[IndexID] ,[Refresh]  ,[FrceRfrsh] ,[ByField]) ";
                    strS += " Values ('" + frmId + "','" + itmId + "','" + colID + "','2','" + queryId.ToString() + "','" + fmsId.ToString() + "','N','N','N')";
                    oRecSet.DoQuery(strS);
                }

                oRecSet = null;
            }
            catch (Exception ex)
            {
                oApplication.SetStatusBarMessage("Error in creating formatted search" + "Fms_" + frmId + "_" + itmId + "_" + colID + ex.Message);
            }

        }
        
        public bool AddTable(string TableName, string TableDescription, SAPbobsCOM.BoUTBTableType TableType)
        {
            bool outResult = false;
            try
            {

                SAPbobsCOM.UserTablesMD v_UserTableMD = default(SAPbobsCOM.UserTablesMD);

                GC.Collect();
                if (!TableExists(TableName))
                {
                    oApplication.StatusBar.SetText("Creating Table " + TableName + " ...................", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                    v_UserTableMD = (SAPbobsCOM.UserTablesMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);
                    v_UserTableMD.TableName = TableName;
                    v_UserTableMD.TableDescription = TableDescription;
                    v_UserTableMD.TableType = TableType;
                    v_RetVal = v_UserTableMD.Add();
                    if (v_RetVal != 0)
                    {
                        oCompany.GetLastError(out v_ErrCode, out  v_ErrMsg);
                        oApplication.StatusBar.SetText("Failed to Create Table " + TableName + v_ErrCode + " " + v_ErrMsg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UserTableMD);
                        v_UserTableMD = null;
                        GC.Collect();
                        return false;
                    }
                    else
                    {
                        oApplication.StatusBar.SetText("[@" + TableName + "] - " + TableDescription + " created successfully!!!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UserTableMD);
                        v_UserTableMD = null;
                        outResult = true;
                        GC.Collect();
                        return true;
                    }
                }
                else
                {
                    GC.Collect();
                    return false;
                }
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed to Add Table : " + ex.Message);
            }
            finally
            {
            }
            return outResult;
        }
        
        public bool UDFExists(string TableName, string FieldID)
        {
            bool outResult = false;
            try
            {
                SAPbobsCOM.Recordset rsetUDF = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                bool oFlag = true;

                rsetUDF.DoQuery("Select 1 from [CUFD] Where TableID='" + TableName.Trim() + "' and AliasID='" + FieldID.Trim() + "'");
                if (rsetUDF.EoF)
                    oFlag = false;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rsetUDF);
                rsetUDF = null;
                outResult = oFlag;
                GC.Collect();
                return oFlag;
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Failed to UDF Exisits : " + ex.Message);
            }
            finally
            {
            }
            return outResult;
        }
        
        public class WindowWrapper : System.Windows.Forms.IWin32Window
        {

            private IntPtr _hwnd;
            public WindowWrapper(IntPtr handle)
            {
                _hwnd = handle;
            }

            public System.IntPtr Handle
            {
                get { return _hwnd; }
            }

        }
        
        
        
        public string FindFile()
        {

            System.Threading.Thread ShowFolderBrowserThread = null;
            try
            {
                ShowFolderBrowserThread = new System.Threading.Thread(ShowFolderBrowser);
                if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Unstarted)
                {
                    ShowFolderBrowserThread.SetApartmentState(System.Threading.ApartmentState.STA);
                    ShowFolderBrowserThread.Start();
                    ShowFolderBrowserThread.Join();

                }
                else if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    ShowFolderBrowserThread.Start();
                    ShowFolderBrowserThread.Join();

                }
                Thread.Sleep(5000);
                while (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Running)
                {
                    System.Windows.Forms.Application.DoEvents();
                }


                if (!string.IsNullOrEmpty(FileName))
                {
                    return FileName;
                }
            }
            catch (Exception ex)
            {
                oApplication.MessageBox("FileFile" + ex.Message);
            }

            return "";

        }
        
        public string SaveFile(string defName)
        {

            defFileName = defName;
            System.Threading.Thread ShowFolderBrowserThread = null;
            try
            {
                ShowFolderBrowserThread = new System.Threading.Thread (SaveFileBrowser);

                if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Unstarted)
                {
                    ShowFolderBrowserThread.SetApartmentState(System.Threading.ApartmentState.STA);
                    ShowFolderBrowserThread.Start();
                }
                else if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    ShowFolderBrowserThread.Start();
                    ShowFolderBrowserThread.Join();

                }
                Thread.Sleep(5000);

                while (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Running)
                {
                    System.Windows.Forms.Application.DoEvents();
                }
                if (!string.IsNullOrEmpty(FileName))
                {
                    return FileName;
                }
            }
            catch (Exception ex)
            {
                oApplication.MessageBox("FileFile" + ex.Message);
            }

            return "";

        }
        
        public void ShowFolderBrowser()
        {
            System.Diagnostics.Process[] MyProcs = null;
            dynamic UserName = Environment.UserName;

            FileName = "";
            OpenFileDialog OpenFile = new OpenFileDialog();

            try
            {
                OpenFile.Multiselect = false;
                OpenFile.Filter = "All files(*.)|*.*";
                int filterindex = 0;
                try
                {
                    filterindex = 0;
                }
                catch (Exception ex)
                {
                }

                OpenFile.FilterIndex = filterindex;

                OpenFile.RestoreDirectory = true;
                MyProcs = System.Diagnostics.Process.GetProcessesByName("SAP Business One");

                for (int i = 0; i <= MyProcs.GetLength(0); i++)
                {

                    if (GetProcessUserName(MyProcs[i]) == UserName)
                    {
                        goto NEXT_STEP;
                    }
                }
                oApplication.MessageBox("Unable to determine Running processes by UserName!");
                OpenFile.Dispose();
                GC.Collect();
                return;
            NEXT_STEP:
                if (MyProcs.Length == 1)
                {

                    for (int i = 0; i <= MyProcs.Length - 1; i++)
                    {
                        WindowWrapper MyWindow = new WindowWrapper(MyProcs[i].MainWindowHandle);
                        DialogResult ret = OpenFile.ShowDialog(MyWindow);

                        if (ret == DialogResult.OK)
                        {
                            FileName = OpenFile.FileName;
                            OpenFile.Dispose();
                        }
                        else
                        {
                            System.Windows.Forms.Application.ExitThread();
                        }
                    }
                }
                else
                {
                    oApplication.MessageBox("More than 1 SAP B1 is started!");
                }
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText(ex.Message);
                FileName = "";
            }
            finally
            {
                OpenFile.Dispose();
                GC.Collect();
            }

        }
        
        public void SaveFileBrowser()
        {
            System.Diagnostics.Process[] MyProcs = null;
            dynamic UserName = Environment.UserName;

            FileName = "";
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.FileName = defFileName;
            try
            {
                MyProcs = System.Diagnostics.Process.GetProcessesByName("SAP Business One");

                for (int i = 0; i <= MyProcs.GetLength(1); i++)
                {
                    if (GetProcessUserName(MyProcs[i]) == UserName)
                    {
                        goto NEXT_STEP;
                    }
                }
                oApplication.MessageBox("Unable to determine Running processes by UserName!");
                saveFile.Dispose();
                GC.Collect();
                return;
            NEXT_STEP:
                if (MyProcs.Length == 1)
                {

                    for (int i = 0; i <= MyProcs.Length - 1; i++)
                    {
                        WindowWrapper MyWindow = new WindowWrapper(MyProcs[i].MainWindowHandle);
                        DialogResult ret = saveFile.ShowDialog(MyWindow);

                        if (ret == DialogResult.OK)
                        {
                            FileName = saveFile.FileName;
                            saveFile.Dispose();
                        }
                        else
                        {
                            System.Windows.Forms.Application.ExitThread();
                        }
                    }
                }
                else
                {
                    oApplication.MessageBox("More than 1 SAP B1 is started!");
                }
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText(ex.Message);
                FileName = "";
            }
            finally
            {
                saveFile.Dispose();
                GC.Collect();
            }

        }
        
        private string GetProcessUserName(System.Diagnostics.Process Process)
        {
            string strResult = "";
            ObjectQuery sq = new ObjectQuery("Select * from Win32_Process Where ProcessID = '" + Process.Id + "'");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(sq);


            if (searcher.Get().Count == 0)
                return null;

            foreach (ManagementObject oReturn in searcher.Get())
            {
                string[] o = new string[2];

                //Invoke the method and populate the o var with the user name and domain                         
                oReturn.InvokeMethod("GetOwner", (object[])o);
                strResult = o[0];
                return o[0];
            }
            return strResult;


        }
        
        private void LoadMenuFromXML(string FileName, string iconPath)
        {
            try
            {
                string sPath = null;
                System.Reflection.Assembly thisExe = null;
                thisExe = System.Reflection.Assembly.GetExecutingAssembly();
                System.IO.Stream file = thisExe.GetManifestResourceStream(FileName);
                string xml = null;

                // Using 
                System.IO.StreamReader sr = new System.IO.StreamReader(file);

                try
                {
                    xml = sr.ReadToEnd();

                }
                catch (Exception EX)
                {
                }
                finally
                {
                    ((IDisposable)sr).Dispose();
                }

                sPath = System.Windows.Forms.Application.StartupPath + "\\";
                xml = xml.Replace("Payroll.bmp", sPath + "Payroll.bmp");
                //'// load the form to the SBO application in one batch
                oApplication.LoadBatchActions(xml);
                sPath = oApplication.GetLastBatchResults();


            }
            catch (Exception ex)
            {
                oApplication.MessageBox(ex.Message + " " +  FileName);
            }
        }

        public  void ftpFile(string URL, string uid, string pwd, string Folder, string fileName)
        {
            // Get the object used to communicate with the server.



            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(URL + "/" + fileName);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(uid, pwd);

            // Copy the contents of the file to the request stream.
          //  StreamReader sourceStream = new StreamReader(Folder + "\\" + fileName);
            byte[] fileContents = System.IO.File.ReadAllBytes(Folder + "\\" + fileName);

           // byte[] fileContents = Encoding.ASCII.GetBytes(sourceStream.ReadToEnd());
           // sourceStream.Close();
            request.ContentLength = fileContents.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
          
            response.Close();
        }
        public string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        } 
        public string getFileName(string filter)
        {
            string fileName = "";
            using (ACHR.Common.GetFileNameClass oGetFileName = new ACHR.Common.GetFileNameClass())
            {
                if (filter != "")
                {
                    oGetFileName.Filter = filter;
                }
                oGetFileName.InitialDirectory = "c:";

                Thread threadGetFile = new Thread(new ThreadStart(oGetFileName.GetFileName));
                try
                {
                    threadGetFile.ApartmentState = ApartmentState.STA;

                    threadGetFile.Start();

                    Thread.Sleep(1000);                    // Wait a sec more
                    while (threadGetFile.IsAlive) ;
                    threadGetFile.Join();

                    // Use file name as you will here
                    if (oGetFileName.FileName != string.Empty)
                    {
                        fileName = oGetFileName.FileName;
                    }
                }
                catch (Exception ex)
                {
                    oApplication.MessageBox(ex.Message, 1, "OK", "", "");
                }
            }
            return fileName;

        }

        public string getStrTime(int intTime)
        {
            string outtime = "12:00 AM";
            if (intTime == 0) return outtime;
            try
            {
                string strAP = " AM";
                string hr = intTime.ToString().PadLeft(4,'0').Substring(0, 2);

                string mn = intTime.ToString().PadLeft(4, '0').Substring(2, 2);

                if (Convert.ToInt16(hr) > 12)
                {
                    hr = (Convert.ToInt16(hr) - 12).ToString();
                    strAP = " PM";
                }

                outtime = hr.PadLeft(2,'0') + ":" + mn.PadLeft(2,'0') + strAP;




            }
            catch { }

            return outtime;
        }


        public int getIntTime(string strTime)
        {
            int outtime = 0;

            try
            {
                string strAP = strTime.Substring(strTime.Length - 2);
                string strTP = strTime.Replace(strAP, "").Replace(":", "").Replace(" ", "").PadLeft(4, '0');
                int hr = Convert.ToInt16(strTP.Substring(0, 2));
                int mn = Convert.ToInt16(strTP.Substring(2, 2));

                if (strAP.ToUpper() == "PM") hr += 12;

                outtime = hr * 100 + mn;





            }
            catch { }

            return outtime;
        }
        public string getFileName2(string FileType)
        {
            string fileName = "";

            using (ACHR.Common.GetFileNameClass oGetFileName = new ACHR.Common.GetFileNameClass())
            {
                oGetFileName.Filter = FileType + " files (."+ FileType + ")|*." + FileType + "|All files (*.*)|*.*";
                oGetFileName.InitialDirectory = "c:";

                Thread threadGetFile = new Thread(new ThreadStart(oGetFileName.GetFileName));
                try
                {
                    threadGetFile.ApartmentState = ApartmentState.STA;

                    threadGetFile.Start();

                    Thread.Sleep(1000);                    // Wait a sec more
                    while (threadGetFile.IsAlive) ;
                    threadGetFile.Join();

                    // Use file name as you will here
                    if (oGetFileName.FileName != string.Empty)
                    {

                        fileName = oGetFileName.FileName;
                    }
                }
                catch (Exception ex)
                {
                    oApplication.MessageBox(ex.Message, 1, "OK", "", "");
                }
            }
            return fileName;

        }



         
      
    }
}
