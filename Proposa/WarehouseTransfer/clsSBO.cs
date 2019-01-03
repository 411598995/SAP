#region Using Directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using System.Xml;
using System.Reflection;
using SAPbobsCOM;
#endregion

namespace WarehouseTransfer
{
    class clsSBO
    {
        #region Declarations
        static SAPbobsCOM.Recordset oRecordSet;
       public static List<SAPbobsCOM.ItemPriceReturnParams> itemPrices = new List<ItemPriceReturnParams>();
        //static StringBuilder oBuilder;
        public static int Year = 2005;
        public static Hashtable UOMEntries = new Hashtable();
        #endregion

        #region Create Table
        public static bool CreateTable(string TableName, string TableDescription, SAPbobsCOM.BoUTBTableType TableType, SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {
            SAPbobsCOM.UserTablesMD oUserTableMD;
            int intRecCode;
            bool boolResult = false;

            oUserTableMD = (SAPbobsCOM.UserTablesMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);

            try
            {
                if (!oUserTableMD.GetByKey(TableName))
                {
                    oUserTableMD.TableName = TableName;
                    oUserTableMD.TableDescription = TableDescription;
                    oUserTableMD.TableType = TableType;                    
                    intRecCode = oUserTableMD.Add();
                    if (intRecCode == 0)
                        boolResult = true;
                }
            }
            catch (Exception e)
            { oApplication.MessageBox(e.Message, 1, "Ok", "", ""); }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserTableMD);
                GC.Collect();
            }
            return boolResult;
        }
        #endregion

        #region Add Fields
        private static void AddField(string TableName, string ColumnName, string ColDescription, SAPbobsCOM.BoFieldTypes FieldType, int Size, SAPbobsCOM.BoFldSubTypes SubType, string ValidValues, string ValidDescription, string SetValidValues, SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany,string strLinkedTable)
        {
            int intLoop;
            string[] strValue, strDesc;
            SAPbobsCOM.UserFieldsMD oUserFieldsMD;
            SAPbobsCOM.Recordset oRecordSet;

            oUserFieldsMD = (SAPbobsCOM.UserFieldsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
            oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {
                oRecordSet.DoQuery("SELECT COUNT(*) FROM \"CUFD\" WHERE \"TableID\" = '" + TableName + "' And \"AliasID\" = '" + ColumnName + "'");
                
                if (Convert.ToInt16(oRecordSet.Fields.Item(0).Value) == 0)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordSet);
                    oRecordSet = null;
                    strValue = ValidValues.Split(Convert.ToChar(","));
                    strDesc = ValidDescription.Split(Convert.ToChar(","));
                    if (strValue.GetLength(0) != strDesc.GetLength(0))
                    {
                        throw new Exception("Invalid Values");
                    }

                    oUserFieldsMD.TableName = TableName;
                    oUserFieldsMD.Name = ColumnName;
                    oUserFieldsMD.Description = ColDescription;
                    oUserFieldsMD.Type = FieldType;
                    if (FieldType != SAPbobsCOM.BoFieldTypes.db_Numeric)
                        oUserFieldsMD.Size = Size;
                    else
                        oUserFieldsMD.EditSize = Size;
                    oUserFieldsMD.SubType = SubType;
                    if (strLinkedTable != "")
                    {
                        oUserFieldsMD.LinkedTable = strLinkedTable;
                    }
                   
                    oUserFieldsMD.DefaultValue = SetValidValues;

                    for (intLoop = 0; intLoop <= strValue.GetLength(0) - 1; intLoop++)
                    {
                        oUserFieldsMD.ValidValues.Value = strValue[intLoop];
                        oUserFieldsMD.ValidValues.Description = strDesc[intLoop];
                        oUserFieldsMD.ValidValues.Add();
                    }                    

                    if (oUserFieldsMD.Add() != 0)
                        UpdateLastErrorDetails(-104, oCompany);
                }
            }
            catch (Exception e)
            { oApplication.MessageBox(e.Message, 1, "Ok", "", ""); }
            finally
            {
                if (oRecordSet != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordSet);
                    oRecordSet = null;
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserFieldsMD);
                GC.Collect();
            }
        }


        private static void AddField(string TableName, string ColumnName, string ColDescription, SAPbobsCOM.BoFieldTypes FieldType, int Size, SAPbobsCOM.BoFldSubTypes SubType, string ValidValues, string ValidDescription, string SetValidValues, SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {
            int intLoop;
            string[] strValue, strDesc;
            SAPbobsCOM.UserFieldsMD oUserFieldsMD;
            SAPbobsCOM.Recordset oRecordSet;

            oUserFieldsMD = (SAPbobsCOM.UserFieldsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
            oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {                
                oRecordSet.DoQuery("SELECT COUNT(*) FROM \"CUFD\" WHERE \"TableID\" = '" + TableName + "' AND \"AliasID\" = '" + ColumnName + "'");
                if (Convert.ToInt16(oRecordSet.Fields.Item(0).Value) == 0)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordSet);
                    oRecordSet = null;
                    strValue = ValidValues.Split(Convert.ToChar(","));
                    strDesc = ValidDescription.Split(Convert.ToChar(","));
                    if (strValue.GetLength(0) != strDesc.GetLength(0))
                    {
                        throw new Exception("Invalid Values");
                    }

                    oUserFieldsMD.TableName = TableName;
                    oUserFieldsMD.Name = ColumnName;
                    oUserFieldsMD.Description = ColDescription;
                    oUserFieldsMD.Type = FieldType;
                    if (FieldType != SAPbobsCOM.BoFieldTypes.db_Numeric)
                        oUserFieldsMD.Size = Size;
                    else
                        oUserFieldsMD.EditSize = Size;
                    oUserFieldsMD.SubType = SubType;

                    oUserFieldsMD.DefaultValue = SetValidValues;

                    for (intLoop = 0; intLoop <= strValue.GetLength(0) - 1; intLoop++)
                    {
                        oUserFieldsMD.ValidValues.Value = strValue[intLoop];
                        oUserFieldsMD.ValidValues.Description = strDesc[intLoop];
                        oUserFieldsMD.ValidValues.Add();
                    }

                    if (oUserFieldsMD.Add() != 0)
                        UpdateLastErrorDetails(-104, oCompany);
                }
            }
            catch (Exception e)
            { oApplication.MessageBox(e.Message, 1, "Ok", "", ""); }
            finally
            {
                if (oRecordSet != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordSet);
                    oRecordSet = null;
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserFieldsMD);
                GC.Collect();
            }
        }

        private static void AddField(string TableName, string ColumnName, string ColDescription, SAPbobsCOM.BoFieldTypes FieldType, int Size, SAPbobsCOM.BoFldSubTypes SubType, string SetDefaultValue, SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {            
            SAPbobsCOM.UserFieldsMD oUserFieldsMD;
            SAPbobsCOM.Recordset oRecordSet;

            oUserFieldsMD = (SAPbobsCOM.UserFieldsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
            oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {
                oRecordSet.DoQuery("SELECT COUNT(*) FROM \"CUFD\" WHERE \"TableID\" = '" + TableName + "' AND \"AliasID\" = '" + ColumnName + "'");
                if (Convert.ToInt16(oRecordSet.Fields.Item(0).Value) == 0)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordSet);
                    oRecordSet = null;
                    
                    oUserFieldsMD.TableName = TableName;
                    oUserFieldsMD.Name = ColumnName;
                    oUserFieldsMD.Description = ColDescription;
                    oUserFieldsMD.Type = FieldType;
                    if (FieldType != SAPbobsCOM.BoFieldTypes.db_Numeric)
                        oUserFieldsMD.Size = Size;
                    else
                        oUserFieldsMD.EditSize = Size;

                    oUserFieldsMD.SubType = SubType;

                    oUserFieldsMD.DefaultValue = SetDefaultValue;
                    
                      if (oUserFieldsMD.Add() != 0)
                        UpdateLastErrorDetails(-104, oCompany);
                }
            }
            catch (Exception e)
            { oApplication.MessageBox(e.Message, 1, "Ok", "", ""); }
            finally
            {
                if (oRecordSet != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordSet);
                    oRecordSet = null;
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserFieldsMD);
                GC.Collect();
            }
        }

        public static void AddFieldLinkTable(string TableName, string ColumnName, string ColDescription, SAPbobsCOM.BoFieldTypes FieldType, int Size, SAPbobsCOM.BoFldSubTypes SubType, string strLinkTable, SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {
            SAPbobsCOM.UserFieldsMD oUserFieldsMD;
            SAPbobsCOM.Recordset oRecordSet;

            oUserFieldsMD = (SAPbobsCOM.UserFieldsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
            oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {
                oRecordSet.DoQuery("SELECT COUNT(*) FROM \"CUFD\" WHERE \"TableID\" = '" + TableName + "' AND \"AliasID\" = '" + ColumnName + "'");
                if (Convert.ToInt16(oRecordSet.Fields.Item(0).Value) == 0)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordSet);
                    oRecordSet = null;

                    oUserFieldsMD.TableName = TableName;
                    oUserFieldsMD.Name = ColumnName;
                    oUserFieldsMD.Description = ColDescription;
                    oUserFieldsMD.Type = FieldType;
                    if (FieldType != SAPbobsCOM.BoFieldTypes.db_Numeric)
                        oUserFieldsMD.Size = Size;
                    else
                        oUserFieldsMD.EditSize = Size;

                    oUserFieldsMD.SubType = SubType;

                    //oUserFieldsMD.LinkedTable = strLinkTable;

                    if (oUserFieldsMD.Add() != 0)
                        UpdateLastErrorDetails(-104, oCompany);
                }
            }
            catch (Exception e)
            { oApplication.MessageBox(e.Message, 1, "Ok", "", ""); }
            finally
            {
                if (oRecordSet != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordSet);
                    oRecordSet = null;
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserFieldsMD);
                GC.Collect();
            }
        }

        public static void AddFloatField(string TableName, string ColumnName, string ColDescription, SAPbobsCOM.BoFldSubTypes SubType, SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {
            AddField(TableName, ColumnName, ColDescription, SAPbobsCOM.BoFieldTypes.db_Float , 0, SubType, "", "", "", oApplication, oCompany);
        }

        public static void AddDateField(string TableName, string ColumnName, string ColDescription, SAPbobsCOM.BoFldSubTypes SubType, SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {
            AddField(TableName, ColumnName, ColDescription, SAPbobsCOM.BoFieldTypes.db_Date, 0, SubType, "", "", "", oApplication, oCompany);
        }

        public static void AddImageField(string TableName, string ColumnName, string ColDescription,  SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {
            AddField(TableName, ColumnName, ColDescription, SAPbobsCOM.BoFieldTypes.db_Alpha, 0, SAPbobsCOM.BoFldSubTypes.st_Image, "", "", "", oApplication, oCompany);
        }


        public static void AddAlphaField(string TableName, string ColumnName, string ColDescription, int Size, SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {
            AddField(TableName, ColumnName, ColDescription, SAPbobsCOM.BoFieldTypes.db_Alpha, Size, SAPbobsCOM.BoFldSubTypes.st_None, "", "", "", oApplication, oCompany);
        }

        public static void AddAlphaField(string TableName, string ColumnName, string ColDescription, int Size, SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany, string strLinkedTable)
        {
            AddField(TableName, ColumnName, ColDescription, SAPbobsCOM.BoFieldTypes.db_Alpha, Size, SAPbobsCOM.BoFldSubTypes.st_None, "", "", "", oApplication, oCompany,strLinkedTable);
        }

        public static void AddAlphaField(string TableName, string ColumnName, string ColDescription, SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {
            AddField(TableName, ColumnName, ColDescription, SAPbobsCOM.BoFieldTypes.db_Memo, 0, SAPbobsCOM.BoFldSubTypes.st_None, "", "", "", oApplication, oCompany);
        }

        public static void AddAlphaField(string TableName, string ColumnName, string ColDescription, int Size, string ValidValues, string ValidDescriptions, string SetValidValues, SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {
            AddField(TableName, ColumnName, ColDescription, SAPbobsCOM.BoFieldTypes.db_Alpha, Size, SAPbobsCOM.BoFldSubTypes.st_None, ValidValues, ValidDescriptions, SetValidValues, oApplication, oCompany);
        }

        public static void AddAlphaField(string TableName, string ColumnName, string ColDescription, int Size,SAPbobsCOM.BoFldSubTypes SubType,SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {
            AddField(TableName, ColumnName, ColDescription, SAPbobsCOM.BoFieldTypes.db_Alpha, Size, SubType, "", "", "", oApplication, oCompany);
        }
       
        public static void AddAlphaField(string TableName, string ColumnName, string ColDescription, int Size, string SetDefaultValues, SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {
            AddField(TableName, ColumnName, ColDescription, SAPbobsCOM.BoFieldTypes.db_Alpha, Size, SAPbobsCOM.BoFldSubTypes.st_None,  SetDefaultValues, oApplication, oCompany);
        }

      

        
        public static void AddAlphaFieldLinkedTable(string TableName, string ColumnName, string ColDescription, int Size, string strLinkTable, SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {
            AddFieldLinkTable(TableName, ColumnName, ColDescription, SAPbobsCOM.BoFieldTypes.db_Alpha, Size, SAPbobsCOM.BoFldSubTypes.st_None, strLinkTable, oApplication, oCompany);
        }

        public static void AddAlphaMemoField(string TableName, string ColumnName, string ColDescription, int Size, SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {
            AddField(TableName, ColumnName, ColDescription, SAPbobsCOM.BoFieldTypes.db_Memo, Size, SAPbobsCOM.BoFldSubTypes.st_None, "", "", "", oApplication, oCompany);
        }

        public static void AddNumericField(string TableName, string ColumnName, string ColDescription, int Size, SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {
            AddField(TableName, ColumnName, ColDescription, SAPbobsCOM.BoFieldTypes.db_Numeric, Size, SAPbobsCOM.BoFldSubTypes.st_None, "", "", "", oApplication, oCompany);
        }

        public static void AddNumericField(string TableName, string ColumnName, string ColDescription, int Size, string ValidValues, string ValidDescription, string DefaultValues, SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {
            AddField(TableName, ColumnName, ColDescription, SAPbobsCOM.BoFieldTypes.db_Numeric, Size, SAPbobsCOM.BoFldSubTypes.st_None, ValidValues, ValidDescription, DefaultValues, oApplication, oCompany);
        }

        private static void UpdateLastErrorDetails(int ErrorCode, SAPbobsCOM.Company oCompany)
        {
            int LastErrorCode;
            LastErrorCode = ErrorCode;
            string LastErrorDescription = oCompany.GetLastErrorCode() + ":" + oCompany.GetLastErrorDescription();
            System.Windows.Forms.MessageBox.Show(LastErrorDescription);
        }
        #endregion

        #region Create UDOs
        public static bool CreateUDO(string UDOName, string UDODescription, string TableName, SAPbobsCOM.BoUDOObjType TableType, string ChildTableName, int Find, string strColumns, SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {
            SAPbobsCOM.UserObjectsMD oUserObjectsMD;
            SAPbobsCOM.UserObjectMD_ChildTables oChildTables;

            try
            {
                oUserObjectsMD = (SAPbobsCOM.UserObjectsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);

                if (!oUserObjectsMD.GetByKey(UDOName))
                {
                    oUserObjectsMD.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES;
                    oUserObjectsMD.CanClose = SAPbobsCOM.BoYesNoEnum.tYES;
                    oUserObjectsMD.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tYES;
                    oUserObjectsMD.CanDelete = SAPbobsCOM.BoYesNoEnum.tYES;
                    oUserObjectsMD.CanFind = SAPbobsCOM.BoYesNoEnum.tYES;
                    oUserObjectsMD.CanLog = SAPbobsCOM.BoYesNoEnum.tNO;
                    oUserObjectsMD.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO;
                    oUserObjectsMD.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO;
                    

                    oUserObjectsMD.LogTableName = "";
                    oUserObjectsMD.ExtensionName = "";
                    oUserObjectsMD.Code = UDOName;
                    oUserObjectsMD.Name = UDODescription;
                    oUserObjectsMD.ObjectType = (SAPbobsCOM.BoUDOObjType)TableType;
                    oUserObjectsMD.TableName = TableName;

                    if (ChildTableName.Trim() != "")
                    {
                        string[] ChildTables = ChildTableName.Split(',');
                        oChildTables = oUserObjectsMD.ChildTables;
                        for (int Index = 0; Index < ChildTables.Length; Index++)
                        {
                            oChildTables.TableName = ChildTables[Index];
                            oChildTables.Add();
                            oChildTables.SetCurrentLine(1);
                        }
                    }

                    if (Find == 1)
                    {
                        string[] Columns = strColumns.Split(',');
                        for (int Index = 0; Index < Columns.Length; Index++)
                        {
                            oUserObjectsMD.FindColumns.ColumnAlias = Columns[Index];
                            oUserObjectsMD.FindColumns.ColumnDescription = Columns[Index];
                            oUserObjectsMD.FindColumns.Add();
                            oUserObjectsMD.FindColumns.SetCurrentLine(1);
                        }
                    }

                    if (oUserObjectsMD.Add() != 0)
                    {
                        MessageBox.Show(oCompany.GetLastErrorDescription().ToString(), "Error");
                        oApplication.MessageBox("Error adding UDO master data", 1, "", "", "");
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjectsMD);
                        oUserObjectsMD = null;
                        return false;
                    }
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjectsMD);
                }
            }
            catch (Exception e)
            { oApplication.MessageBox(e.Message, 1, "", "", ""); }
            finally
            {
                oUserObjectsMD = null;
                GC.Collect();
            }
            return true;
        }

        public static bool CreateUDODF(string UDOName, string UDODescription, string TableName, SAPbobsCOM.BoUDOObjType TableType, string ChildTableName, int Find, string strColumns, SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {
            SAPbobsCOM.UserObjectsMD oUserObjectsMD;
            SAPbobsCOM.UserObjectMD_ChildTables oChildTables;

            try
            {
                oUserObjectsMD = (SAPbobsCOM.UserObjectsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);

                if (!oUserObjectsMD.GetByKey(UDOName))
                {
                    oUserObjectsMD.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES;
                    oUserObjectsMD.CanClose = SAPbobsCOM.BoYesNoEnum.tYES;
                    oUserObjectsMD.CanDelete = SAPbobsCOM.BoYesNoEnum.tYES;
                    oUserObjectsMD.CanFind = SAPbobsCOM.BoYesNoEnum.tYES;
                    oUserObjectsMD.CanLog = SAPbobsCOM.BoYesNoEnum.tNO;
                    oUserObjectsMD.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO;
                    oUserObjectsMD.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO;
                    oUserObjectsMD.CanCreateDefaultForm = BoYesNoEnum.tYES;
                    oUserObjectsMD.EnableEnhancedForm = BoYesNoEnum.tYES;
                    oUserObjectsMD.FormColumns.FormColumnAlias = "Code";
                    oUserObjectsMD.FormColumns.FormColumnDescription = "Code";
                    oUserObjectsMD.FormColumns.Add();
                    oUserObjectsMD.FormColumns.FormColumnAlias = "Name";
                    oUserObjectsMD.FormColumns.FormColumnDescription = "Name";
                    oUserObjectsMD.FormColumns.Add();
                    oUserObjectsMD.LogTableName = "";
                    oUserObjectsMD.ExtensionName = "";
                    oUserObjectsMD.Code = UDOName;
                    oUserObjectsMD.Name = UDODescription;
                    oUserObjectsMD.ObjectType = (SAPbobsCOM.BoUDOObjType)TableType;
                    oUserObjectsMD.TableName = TableName;

                    if (ChildTableName.Trim() != "")
                    {
                        string[] ChildTables = ChildTableName.Split(',');
                        oChildTables = oUserObjectsMD.ChildTables;
                        for (int Index = 0; Index < ChildTables.Length; Index++)
                        {
                            oChildTables.TableName = ChildTables[Index];
                            oChildTables.Add();
                            oChildTables.SetCurrentLine(1);
                        }
                    }

                    if (Find == 1)
                    {
                        string[] Columns = strColumns.Split(',');
                        for (int Index = 0; Index < Columns.Length; Index++)
                        {
                            oUserObjectsMD.FindColumns.ColumnAlias = Columns[Index];
                            oUserObjectsMD.FindColumns.ColumnDescription = Columns[Index];
                            oUserObjectsMD.FindColumns.Add();
                            oUserObjectsMD.FindColumns.SetCurrentLine(1);
                        }
                    }                   
                    

                    if (oUserObjectsMD.Add() != 0)
                    {
                        MessageBox.Show(oCompany.GetLastErrorDescription().ToString(), "Error");
                        oApplication.MessageBox("Error adding UDO master data", 1, "", "", "");
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjectsMD);
                        oUserObjectsMD = null;
                        return false;
                    }
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjectsMD);
                }
            }
            catch (Exception e)
            { oApplication.MessageBox(e.Message, 1, "", "", ""); }
            finally
            {
                oUserObjectsMD = null;
                GC.Collect();
            }
            return true;
        }

        public static bool CreateUDO_Series(string UDOName, string UDODescription, string TableName, SAPbobsCOM.BoUDOObjType TableType, string ChildTableName, int Find, string strColumns, SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany)
        {
            SAPbobsCOM.UserObjectsMD oUserObjectsMD;
            SAPbobsCOM.UserObjectMD_ChildTables oChildTables;

            try
            {
                oUserObjectsMD = (SAPbobsCOM.UserObjectsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);

                if (!oUserObjectsMD.GetByKey(UDOName))
                {
                    oUserObjectsMD.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES;
                    oUserObjectsMD.CanClose = SAPbobsCOM.BoYesNoEnum.tYES;
                    oUserObjectsMD.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO;
                    oUserObjectsMD.CanDelete = SAPbobsCOM.BoYesNoEnum.tYES;
                    oUserObjectsMD.CanFind = SAPbobsCOM.BoYesNoEnum.tYES;
                    oUserObjectsMD.CanLog = SAPbobsCOM.BoYesNoEnum.tNO;
                    oUserObjectsMD.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO;
                    oUserObjectsMD.ManageSeries = SAPbobsCOM.BoYesNoEnum.tYES;

                    oUserObjectsMD.LogTableName = "";
                    oUserObjectsMD.ExtensionName = "";
                    oUserObjectsMD.Code = UDOName;
                    oUserObjectsMD.Name = UDODescription;
                    oUserObjectsMD.ObjectType = (SAPbobsCOM.BoUDOObjType)TableType;
                    oUserObjectsMD.TableName = TableName;

                    if (ChildTableName.Trim() != "")
                    {
                        string[] ChildTables = ChildTableName.Split(',');
                        oChildTables = oUserObjectsMD.ChildTables;
                        for (int Index = 0; Index < ChildTables.Length; Index++)
                        {
                            oChildTables.TableName = ChildTables[Index];
                            oChildTables.Add();
                            oChildTables.SetCurrentLine(1);
                        }
                    }

                    if (Find == 1)
                    {
                        string[] Columns = strColumns.Split(',');
                        for (int Index = 0; Index < Columns.Length; Index++)
                        {
                            oUserObjectsMD.FindColumns.ColumnAlias = Columns[Index];
                            oUserObjectsMD.FindColumns.ColumnDescription = Columns[Index];
                            oUserObjectsMD.FindColumns.Add();
                            oUserObjectsMD.FindColumns.SetCurrentLine(1);
                        }
                    }

                    if (oUserObjectsMD.Add() != 0)
                    {
                        MessageBox.Show(oCompany.GetLastErrorDescription().ToString(), "Error");
                        oApplication.MessageBox("Error adding UDO master data", 1, "", "", "");
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjectsMD);
                        oUserObjectsMD = null;
                        return false;
                    }
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjectsMD);
                }
            }
            catch (Exception e)
            { oApplication.MessageBox(e.Message, 1, "", "", ""); }
            finally
            {
                oUserObjectsMD = null;
                GC.Collect();
            }
            return true;
        }
        #endregion

        #region Menu Objects
        public static void LoadMenu(string XMLFile, SAPbouiCOM.Application oApplication)
        {
            XmlDocument oXml;
            string strXML;

            try
            {
                oXml = new XmlDocument();
                oXml.Load(XMLFile);
                strXML = oXml.InnerXml;
                oApplication.LoadBatchActions(ref strXML);  
            }
            catch (Exception e)
            { MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        #endregion

        #region Form Objects

        /*
        public static SAPbouiCOM.Form LoadForm(string FileName, string FormType,ref SAPbouiCOM.Application oApplication)
        {
            XmlDocument oXmlDoc = new XmlDocument();
            SAPbouiCOM.FormCreationParams oFormCreationParams = null;
            try
            {
                oXmlDoc.Load(FileName);
                oFormCreationParams = (SAPbouiCOM.FormCreationParams)oApplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);
                oFormCreationParams.XmlData = oXmlDoc.InnerXml;
                oFormCreationParams.FormType = FormType;
                oFormCreationParams.UniqueID = oApplication.Forms.Count.ToString();
            }
            catch (Exception e)
            { oApplication.MessageBox(e.Message, 1, "Ok", "", ""); }
            return oApplication.Forms.AddEx(oFormCreationParams);
        }*/

        public static SAPbouiCOM.Form LoadForm(string XMLFile, string FormType, ref SAPbouiCOM.Application oApplication)
        {
            System.Xml.XmlDocument oXML = null;
            System.IO.Stream oXmlStream = null;
            SAPbouiCOM.FormCreationParams objFormCreationParams = null;
            Assembly oAssembly = Assembly.GetExecutingAssembly();
            try
            {
                oXML = new System.Xml.XmlDocument();
                string strFile = oAssembly.GetName().Name + ".bin.Debug." + XMLFile;
                oXmlStream = oAssembly.GetManifestResourceStream(strFile);
                oXML.Load(oXmlStream);

                objFormCreationParams = (SAPbouiCOM.FormCreationParams)oApplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);
                objFormCreationParams.XmlData = oXML.InnerXml;
                objFormCreationParams.FormType = FormType;
                objFormCreationParams.UniqueID = oApplication.Forms.Count.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return oApplication.Forms.AddEx(objFormCreationParams);
        }

        private static bool AppFormExist(string strFrmID,SAPbouiCOM.Application oApplication)
        {
            bool boolFormExist = false;
            try
            {               
                foreach (SAPbouiCOM.Form oFrm in oApplication.Forms)
                {
                    if (oFrm.UniqueID == strFrmID)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {                
                throw ex;
            }
            return boolFormExist;
        }

        public static SAPbouiCOM.Form LoadForm(string FileName, string FormType, SAPbouiCOM.Application oApplication)
        {
            XmlDocument oXmlDoc = new XmlDocument();
            SAPbouiCOM.FormCreationParams oFormCreationParams = null;

            try
            {
                string pathstr = "eOrderEntry.Layout." + FileName;

                System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
                System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(pathstr);
                System.IO.StreamReader streamreader = new System.IO.StreamReader(stream, true);
                oXmlDoc.LoadXml(streamreader.ReadToEnd());
                streamreader.Close();


               // oXmlDoc.Load(FileName);
                oFormCreationParams = (SAPbouiCOM.FormCreationParams)oApplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);
                oFormCreationParams.XmlData = oXmlDoc.InnerXml;
                oFormCreationParams.FormType = FormType;
                string FrmUniqueID = oApplication.Forms.Count.ToString();
                for (int i = oApplication.Forms.Count; i <= 1000; i++)
                {
                    if (AppFormExist(i.ToString(), oApplication))
                    {

                    }
                    else
                    {
                        FrmUniqueID = i.ToString();
                        break;
                    }
                }
                oFormCreationParams.UniqueID = FrmUniqueID.ToString();               
            }
            catch (Exception e)
            { oApplication.MessageBox(e.Message, 1, "Ok", "", ""); }
            return oApplication.Forms.AddEx(oFormCreationParams);
        }

        public static SAPbouiCOM.Form LoadNewForm(string FileName, string FormType, SAPbouiCOM.Application oApplication)
        {
            XmlDocument oXmlDoc = new XmlDocument();
            SAPbouiCOM.FormCreationParams oFormCreationParams = null;

            try
            {
                oXmlDoc.Load(FileName);
                oFormCreationParams = (SAPbouiCOM.FormCreationParams)oApplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);
                oFormCreationParams.XmlData = oXmlDoc.InnerXml;
                oFormCreationParams.FormType = FormType;
                oFormCreationParams.UniqueID = Convert.ToString(oApplication.Forms.Count + 1);
            }
            catch (Exception e)
            { oApplication.MessageBox(e.Message, 1, "Ok", "", ""); }
            return oApplication.Forms.AddEx(oFormCreationParams);
        }

        //public static SAPbouiCOM.Form LoadAuthorizationForm(string FileName, string FormType, ref SAPbouiCOM.Application oApplication)
        //{
        //    XmlDocument oXmlDoc = new XmlDocument();
        //    SAPbouiCOM.FormCreationParams oFormCreationParams = null;

        //    try
        //    {
        //        oXmlDoc.Load(FileName);
        //        oFormCreationParams = (SAPbouiCOM.FormCreationParams)oApplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);
        //        oFormCreationParams.XmlData = oXmlDoc.InnerXml;
        //        oFormCreationParams.FormType = FormType;
        //        oFormCreationParams.UniqueID = oApplication.Forms.Count.ToString();
        //    }
        //    catch (Exception e)
        //    { oApplication.MessageBox(e.Message, 1, "Ok", "", ""); }

        //    try
        //    { return oApplication.Forms.AddEx(oFormCreationParams); }
        //    catch (Exception)
        //    { return null; }

        //}

        #endregion

        #region XML Objects
        public static void LoadFromXML(ref string FileName, SAPbouiCOM.Application oApplication)
        {
            XmlDocument oXMLDoc = new XmlDocument();
            try
            {
                oXMLDoc.Load(FileName);
                string str = oXMLDoc.InnerXml;
                oApplication.LoadBatchActions(ref str);
            }
            catch (Exception e)
            { MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        #endregion

        #region ExecuteQuery
        public static SAPbobsCOM.Recordset ExecuteQuery(SAPbobsCOM.Company oCompany, string Query)
        {
            oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            oRecordSet.DoQuery(Query);
            return oRecordSet;
        }
        #endregion

        #region Matrix Add, Delete Line
        public static void AddLine(SAPbouiCOM.Application oApplication, string FormName, string TableName, string MatrixName, string ColumnName)
        {
            SAPbouiCOM.Form oMatrixForm;
            oMatrixForm = oApplication.Forms.GetForm(FormName, 0);
            SAPbouiCOM.Matrix oMatrix;
            oMatrix = (SAPbouiCOM.Matrix)oMatrixForm.Items.Item(MatrixName).Specific;

            if (oMatrix.RowCount > 0)
            {
                if (((SAPbouiCOM.EditText)oMatrix.Columns.Item(ColumnName).Cells.Item(oMatrix.RowCount).Specific).Value.ToString() == "")
                { goto End; }
            }

            oMatrixForm.DataSources.DBDataSources.Item(TableName).Clear();
            oMatrix.AddRow(1, -1);
        End:
            int i = 0;
        }

        public static void DeleteLine(SAPbouiCOM.Application oApplication, string FormName, string MatrixName)
        {
            SAPbouiCOM.Form oForm;
            oForm = oApplication.Forms.GetForm(FormName, 0);

            int RowCountInt;
            SAPbouiCOM.Matrix mMatrixRowCount;
            mMatrixRowCount = ((SAPbouiCOM.Matrix)(oForm.Items.Item(MatrixName).Specific));
            RowCountInt = mMatrixRowCount.RowCount;
            for (int n = 1; n <= mMatrixRowCount.RowCount; n++)
            {
                if (mMatrixRowCount.IsRowSelected(n))
                {
                    mMatrixRowCount.DeleteRow(n);
                }
            }
            if (oForm.Mode != SAPbouiCOM.BoFormMode.fm_ADD_MODE)
            {
                oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE;
            }
        }
        #endregion

        #region CFL DataTable
        public static SAPbouiCOM.DataTable GetDataTable(SAPbouiCOM.Form oForm, ref SAPbouiCOM.ItemEvent pVal)
        {
            SAPbouiCOM.IChooseFromListEvent oCFLEvent = null;
            SAPbouiCOM.ChooseFromList oCFL = null;
            SAPbouiCOM.DataTable oDataTable = null;
            string strCFLID = null;

            oCFLEvent = ((SAPbouiCOM.IChooseFromListEvent)(pVal));
            strCFLID = oCFLEvent.ChooseFromListUID;
            oCFL = oForm.ChooseFromLists.Item(strCFLID);
            oDataTable = oCFLEvent.SelectedObjects;

            return oDataTable;
        }
        #endregion

        #region Control Creation
        public static void CreateStaticText(SAPbouiCOM.Form oForm, string Id, string Caption, int Top, int Left, int Height, int Width, int FromPane, int ToPane)
        {
            oForm.Items.Add(Id, SAPbouiCOM.BoFormItemTypes.it_STATIC);
            oForm.Items.Item(Id).Top = Top;
            oForm.Items.Item(Id).Height = Height;
            oForm.Items.Item(Id).Left = Left;
            oForm.Items.Item(Id).Width = Width;
            oForm.Items.Item(Id).FromPane = FromPane;
            oForm.Items.Item(Id).ToPane = ToPane;
            ((SAPbouiCOM.StaticText)oForm.Items.Item(Id).Specific).Caption = Caption;


        }

        public static void CreateComboBox(SAPbouiCOM.Form oForm, string Id, int Top, int Left, int Height, int Width, int FromPane, int ToPane)
        {
            oForm.Items.Add(Id, SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX);
            oForm.Items.Item(Id).Top = Top;
            oForm.Items.Item(Id).Height = Height;
            oForm.Items.Item(Id).Left = Left;
            oForm.Items.Item(Id).Width = Width;
            oForm.Items.Item(Id).FromPane = FromPane;
            oForm.Items.Item(Id).ToPane = ToPane;
            oForm.Items.Item(Id).DisplayDesc = true;
        }

        public static void CreateComboBox(SAPbouiCOM.Form oForm, string Id, int Top, int Left, int Height, int Width, string TableName, string ColumnName, int FromPane, int ToPane)
        {
            oForm.Items.Add(Id, SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX);
            oForm.Items.Item(Id).Top = Top;
            oForm.Items.Item(Id).Height = Height;
            oForm.Items.Item(Id).Left = Left;
            oForm.Items.Item(Id).Width = Width;
            oForm.Items.Item(Id).FromPane = FromPane;
            oForm.Items.Item(Id).ToPane = ToPane;
            oForm.Items.Item(Id).DisplayDesc = true;
            ((SAPbouiCOM.ComboBox)oForm.Items.Item(Id).Specific).DataBind.SetBound(true, TableName, ColumnName);
        }

        public static void CreateEditBox(SAPbouiCOM.Form oForm, string Id, int Top, int Left, int Height, int Width, string TableName, string ColumnName, int FromPane, int ToPane)
        {
            oForm.Items.Add(Id, SAPbouiCOM.BoFormItemTypes.it_EDIT);
            oForm.Items.Item(Id).Top = Top;
            oForm.Items.Item(Id).Height = Height;
            oForm.Items.Item(Id).Left = Left;
            oForm.Items.Item(Id).Width = Width;
            oForm.Items.Item(Id).FromPane = FromPane;
            oForm.Items.Item(Id).ToPane = ToPane;
            oForm.Items.Item(Id).DisplayDesc = true;
            ((SAPbouiCOM.EditText)oForm.Items.Item(Id).Specific).DataBind.SetBound(true, TableName, ColumnName);
        }

        public static void LinkTo(SAPbouiCOM.Form oForm, string StaticText, string Field)
        {
            oForm.Items.Item(StaticText).LinkTo = Field;
        }
        #endregion

        #region Get DocEntry
        public static int GetDocEntry(string TableName, SAPbobsCOM.Company oCompany)
        {
            SAPbobsCOM.Recordset oRecordSet;
            int DocEntry = 0;
            oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            string strQuery = "SELECT Max(TO_BIGINT(\"Code\")) FROM " + TableName;
            oRecordSet.DoQuery(strQuery);

            if (oRecordSet.RecordCount == 0)
                DocEntry = 1;
            else
                DocEntry = Convert.ToInt16(oRecordSet .Fields .Item (0).Value .ToString ())  + 1;

            return DocEntry;
        }
        #endregion

        public static int GetCode(string TableName, SAPbobsCOM.Company oCompany)
        {
            SAPbobsCOM.Recordset oRecordSet;
            int Code = 0;
            oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            string strQuery = "SELECT IFNULL(Max(TO_BIGINT(\"Code\")),0) FROM " + TableName;
            oRecordSet.DoQuery(strQuery);

            if (oRecordSet.RecordCount == 0)
                Code = 1;
            else
                Code = Convert.ToInt32(oRecordSet.Fields.Item(0).Value) + 1;

            return Code;
        }


        public static int GetLineId(string TableName, string DocEntry, SAPbobsCOM.Company oCompany)
        {
            SAPbobsCOM.Recordset oRecordSet;
            int LineId = 0;
            oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            string strQuery = "SELECT IFNULL(Max(\"LineId\"),0) + 1 FROM " + TableName + "  WHERE \"DocEntry\" = " + DocEntry;
            oRecordSet.DoQuery(strQuery);

            if (oRecordSet.RecordCount == 0)
                LineId = 1;
            else
                LineId = Convert.ToInt16(oRecordSet.Fields.Item(0).Value.ToString() );

            return LineId;
        }

        public static bool FormExist(ref SAPbouiCOM.Application oApplication,string FormUID)
        {
            int intLoop = 0;

            for (intLoop = oApplication.Forms.Count - 1; intLoop >= 0; intLoop += -1)
            {
                if (FormUID.ToString() == oApplication.Forms.Item(intLoop).TypeEx.ToString())
                {
                    return true;
                }
            }
            return false;
        }

        public static int FormCount(ref SAPbouiCOM.Application oApplication, string FormUID)
        {
            int intLoop = 0, intCount = 0;

            for (intLoop = oApplication.Forms.Count - 1; intLoop >= 0; intLoop += -1)
            {
                if (FormUID.ToString() == oApplication.Forms.Item(intLoop).TypeEx.ToString())
                {
                    intCount++;
                }
            }
            return intCount;
        }

        //Check the Authorization...
        public static bool CheckAuthorization(ref SAPbobsCOM.Company oCompany, string strMenu, int intUser)
        {                       
            SAPbobsCOM.Recordset oRecordSet;
            oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            string strQry = "SELECT \"SuperUser\" FROM \"OUSR\" Where \"UserId\" ='" + oCompany.UserSignature.ToString() + "'";
                oRecordSet.DoQuery(strQry);
                if (!oRecordSet.EoF)
                {
                    if (oRecordSet.Fields.Item(0).Value.ToString() == "Y")
                    {
                        return true;
                    }
                else
                {
                    strQry = "SELECT \"Permission\" FROM \"USR3\" WHERE \"UserLink\" ='" + intUser + "' AND \"PermId\" ='" + strMenu + "'";
                    oRecordSet.DoQuery(strQry);
                    if (oRecordSet.EoF)
                    {
                        return false;
                    }
                    else
                    {
                        if (oRecordSet.Fields.Item(0).Value.ToString() == "N")
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            return true;
        }

        //Auto Creation of the Service Call in the A/R Invoice...
        public static void CreateServiceCall(SAPbobsCOM.Company oCompany, string strDocEntry)
        {
            SAPbobsCOM.ServiceCalls oServiceCall;
            oServiceCall = (SAPbobsCOM.ServiceCalls)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oServiceCalls);
            oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            string strQry = "";
            strQry = " SELECT T1.CardCode,T1.CardName,T0.ItemCode,T1.DocNum FROM INV1 T0 JOIN OINV T1 ON T0.DOCENTRY = T1.DocEntry   ";
            strQry += " JOIN OITM T2 ON T0.Itemcode = T2.ItemCode JOIN OITB T3 ON T2.ItmsGrpCod = T3.ItmsGrpCod AND T3.U_SerCall = 'Y' ";
            strQry += " WHERE T1.DocEntry ='" + strDocEntry + "'";

            oRecordSet.DoQuery(strQry);
            if (!oRecordSet.EoF)
            {
                while (!oRecordSet.EoF)
                {
                    oServiceCall.CustomerCode = oRecordSet.Fields.Item(0).Value.ToString();
                    oServiceCall.CustomerName = oRecordSet.Fields.Item(1).Value.ToString();
                    oServiceCall.Description = "Service Call Test";
                    oServiceCall.ItemCode = oRecordSet.Fields.Item(2).Value.ToString();
                    oServiceCall.Subject = "Fix Demo Call Ref DocumentNo: " + oRecordSet.Fields.Item(3).Value.ToString() + "";
                    oServiceCall.Priority = SAPbobsCOM.BoSvcCallPriorities.scp_High;                    
                    oServiceCall.Add();
                    oRecordSet.MoveNext();
                }
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(oServiceCall);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oRecordSet);
        }

        public static string GetDefaultGroup(ref Company oCompany)
        {
            string str = string.Empty;
            SAPbobsCOM.Recordset businessObject = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            string queryStr = "Select \"DfltsGroup\" FROM \"OUSR\" Where \"USERID\" = '" + oCompany.UserSignature + "'";
            businessObject.DoQuery(queryStr);
            if (!businessObject.EoF)
            {
                str = businessObject.Fields.Item(0).Value.ToString();
            }
            return str;
        }

        public static string getColVisibility(string formId, string colId, string userid, Company oCompany)
        {
            string result = "Y";

            try
            {
                string strSel = " select top 1 VisInForm from cprf inner join OUSR on ousr.USERID = cprf.UserSign where FormID = '" + formId + "' and colID = '" + colId + "' and ousr.USER_CODE = '" + userid + "'";
                System.Data.DataTable dtSetting = getDataTable(strSel, "Setting", oCompany);
                if (dtSetting.Rows.Count > 0)
                {
                    result = dtSetting.Rows[0]["VisInForm"].ToString();
                }
            }
            catch(Exception ex)
            {

            }

            return result;


        }

        public static System.Data.DataTable getDataTable(string sql, string CallerRef, SAPbobsCOM.Company oCompany)
        {
            System.Data.DataTable dtOut = new System.Data.DataTable();
            SAPbobsCOM.Recordset rs = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {
                rs.DoQuery(sql);

                for (int i = 0; i < rs.Fields.Count; i++)
                {
                    dtOut.Columns.Add(rs.Fields.Item(i).Description);
                }


                while (!rs.EoF)
                {
                    System.Data. DataRow nr = dtOut.NewRow();
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
               // oApplication.StatusBar.SetText("Failed in Exec Query on " + CallerRef + " : " + ex.Message);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
                rs = null;

            }
            return dtOut;
        }

        public static SAPbobsCOM.ItemPriceReturnParams getUnitPriceSys(SAPbobsCOM.Company oCompany, string itemCode, string CardCode, double quantity, DateTime pdate, string uomCode)
        {
            

            
            SAPbobsCOM.ItemPriceParams priceParam;


            SAPbobsCOM.ItemPriceReturnParams ItemPrice;
            SAPbobsCOM.CompanyService cmpSvc = oCompany.GetCompanyService();
            priceParam = (SAPbobsCOM.ItemPriceParams)cmpSvc.GetDataInterface(SAPbobsCOM.CompanyServiceDataInterfaces.csdiItemPriceParams);
            priceParam.CardCode = CardCode;
            priceParam.ItemCode = itemCode;
            priceParam.UoMQuantity = quantity;
            priceParam.UoMEntry = getUomEntry(oCompany, uomCode);
            //  priceParam.InventoryQuantity = quantity;
            priceParam.Date = pdate;

            ItemPrice = (SAPbobsCOM.ItemPriceReturnParams)cmpSvc.GetDataInterface(SAPbobsCOM.CompanyServiceDataInterfaces.csdiItemPriceReturnParams);
            ItemPrice = cmpSvc.GetItemPrice(priceParam);
          






            return ItemPrice;

        }
        public static int getUomEntry(SAPbobsCOM.Company oCompany, string uomCode)
        {
            int result = -1;

            if (UOMEntries.Contains(uomCode))
            {
                return Convert.ToInt32(UOMEntries[uomCode]);
            }
            else
            {
                fillUOMEntries(oCompany);
                try
                {
                    result = Convert.ToInt32(UOMEntries[uomCode]);
                }
                catch { }
            }
           
            return result;

        }

        public static void fillUOMEntries(SAPbobsCOM.Company oCompany)
        {
            System.Data.DataTable dt = clsSBO.getDataTable("  select UOMCode, UomEntry from ouom ", "GetUOM", oCompany);
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                try
                {
                    UOMEntries.Add(dr["UOMCode"].ToString(), dr["UomEntry"].ToString());
                }
                catch { }
            }
        }





    }
}