using System;

	
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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


using System.Resources;



namespace SAPDI
{


    public class DIClass
    {
        long v_RetVal;
        int v_ErrCode;
        string v_ErrMsg = "";
         public SAPbobsCOM.Company oDiCompany;
         public bool isDIConnected = false;
        string FileName;
        string defFileName;
        public Hashtable StringMessages = new Hashtable();
        public System.Data.DataTable LOVs = new System.Data.DataTable();
        public System.Data.DataTable AllLovs = new System.Data.DataTable();

        public string companyDb, SboUID, SboPwd, DbUserName, DbPassword, ServerType, SboServer;


        public DIClass(string pcompanyDb, string pSboUID, string pSboPwd, string pDbUserName, string pDbPassword, string pServerType, string pSboServer)
        {

            companyDb = pcompanyDb;
            SboUID = pSboUID;
            SboPwd = pSboPwd;
            DbUserName = pDbUserName;
            DbPassword = pDbPassword;
            ServerType = pServerType;
            SboServer = pSboServer;
        }

        public string connectCompay(SAPbobsCOM.Company cmp)
        {
            string outResult = "OK";
            oDiCompany = cmp;
            return outResult;
        }
        public string connectCompany()
        {
            string result = "OK";
            oDiCompany = new SAPbobsCOM.Company();

            oDiCompany.CompanyDB = companyDb;
            oDiCompany.UserName = SboUID;
            oDiCompany.Password = SboPwd;
            oDiCompany.DbUserName = DbUserName;
            oDiCompany.DbPassword = DbPassword;
            //Program.oDiCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2005;
            if (ServerType.Trim() == "2005")
            {
                oDiCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2005;
            }
            else if (ServerType.Trim() == "2008")
            {
                oDiCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2008;
            }
            else if (ServerType.Trim() == "2012")
            {
                oDiCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012;
            }
            else if (ServerType.Trim() == "2014")
            {
                oDiCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2014;
            }
            else
            {
                oDiCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2008;
            }
            oDiCompany.Server = SboServer;
            //Try to connect
            int lRetCode = oDiCompany.Connect();

            int errCode = 0;
            string errMsg = "";
            if (lRetCode != 0) // if the connection failed
            {
                oDiCompany.GetLastError(out errCode, out errMsg);
                isDIConnected = false;
                result = errCode + ":" + errMsg;
             

            }
            else
            {
                result = "OK";
                isDIConnected = true;
            }
            return result;
        }
        
        
       
       
        
        

        public bool ColumnExists(string TableName, string FieldID)
        {
            bool oFlag = true;
            try
            {
                SAPbobsCOM.Recordset rsetField = (SAPbobsCOM.Recordset)oDiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

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
                throw new Exception("hello");
                throw new Exception("Failed to Column Exists : " + ex.Message);
            }
            finally
            {
            }
            return oFlag;
        }
        
        public void ExecQuery(string sql, string CallerRef)
        {
            SAPbobsCOM.Recordset rs = (SAPbobsCOM.Recordset)oDiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {
                rs.DoQuery(sql);

            }
            catch (Exception ex)
            {
                throw new Exception("Failed in Exec Query on " + CallerRef + " : " + ex.Message);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
                rs = null;

            }
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
                throw new Exception("Failed to execute pat" + ex.Message);
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
        
        
        public bool UDOExists(string code)
        {
            bool outResult = false;
            try
            {
                SAPbobsCOM.UserObjectsMD v_UDOMD = default(SAPbobsCOM.UserObjectsMD);
                bool v_ReturnCode = false;

                GC.Collect();
                v_UDOMD = (SAPbobsCOM.UserObjectsMD)oDiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);
                v_ReturnCode = v_UDOMD.GetByKey(code);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UDOMD);
                v_UDOMD = null;
                outResult = v_ReturnCode;
                return v_ReturnCode;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to UDO Exists : " + ex.Message);
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
                v_udoMD = (SAPbobsCOM.UserObjectsMD)oDiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);
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
                    }
                else
                {
                    throw new Exception("Failed to Register UDO >" + UDOCode + ">" + UDOName + " >" + oDiCompany.GetLastErrorDescription());
                    functionReturnValue = false;
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(v_udoMD);
                v_udoMD = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to UDO Register : " + ex.Message);
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

                oTables = (SAPbobsCOM.UserTablesMD)oDiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);
                oFlag = oTables.GetByKey(TableName);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oTables);
                outResult = oFlag;
                return oFlag;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to Table Exists : " + ex.Message);
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
        
       
        
        
         
        
        
        

    }
}
