using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NLog;
using SAPbobsCOM;

namespace SapBusinessOneExtensions
{
    public static class SboUserDefinedDataManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static bool UserFieldExists(string tableName, string name)
        {
            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                var recordSet = factory.Create<Recordset>(BoObjectTypes.BoRecordset);
                recordSet.DoQuery(string.Format(@"SELECT * FROM CUFD WHERE (""TableID"" = '{0}' OR ""TableID"" = '@{0}') AND (""AliasID"" = '{1}' OR ""AliasID"" = 'U_{1}')", tableName, name));
                return recordSet.RecordCount != 0;
            }
        }

        public static void AddUserField(string tableName, string name, string description, BoFieldTypes type = BoFieldTypes.db_Alpha, BoFldSubTypes subType = BoFldSubTypes.st_None, int? editSize = null,
                                                Dictionary<string, string> validValues = null, string defaultValue = null, string linkedUdo = null, bool? mandatory = false)
        {
            if (UserFieldExists(tableName, name))
                throw new Exception(string.Format("Userfield {0} already exists in table {1}", name, tableName));

            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                var userFieldsMd = factory.Create<UserFieldsMD>(BoObjectTypes.oUserFields);

                userFieldsMd.TableName = tableName;
                userFieldsMd.Name = name;
                userFieldsMd.Description = description;
                userFieldsMd.Type = type;
                userFieldsMd.SubType = subType;
                if (!editSize.HasValue)
                {
                    switch (type)
                    {
                        case BoFieldTypes.db_Alpha:
                            editSize = 254;
                            break;
                        case BoFieldTypes.db_Memo:
                            break;
                        case BoFieldTypes.db_Numeric:
                            break;
                        case BoFieldTypes.db_Date:
                            break;
                        case BoFieldTypes.db_Float:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }
                }

                if (!type.Equals(BoFieldTypes.db_Date) && editSize.HasValue)
                    userFieldsMd.EditSize = editSize.Value;
                if (validValues != null)
                {
                    int insertedValues = 0;
                    foreach (string key in validValues.Keys)
                    {
                        userFieldsMd.ValidValues.Value = key;
                        userFieldsMd.ValidValues.Description = validValues[key];

                        if (insertedValues < validValues.Count - 1)
                            userFieldsMd.ValidValues.Add();

                        insertedValues++;
                    }
                }
                if (defaultValue != null)
                    userFieldsMd.DefaultValue = defaultValue;

                if (linkedUdo != null)
                    userFieldsMd.LinkedUDO = linkedUdo;

                userFieldsMd.Mandatory = mandatory.GetValueOrDefault(false) ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;

                Logger.Debug("Adding field {0} to table {1}", name, tableName);
                if (userFieldsMd.Add() != 0)
                     throw new Exception(SboAddon.Instance.Company.GetLastErrorDescription());
                
                Logger.Info("Added field {0} to table {1}", name, tableName);
            }
        }

        public static void UpdateUserField(string tableName, string name, string description = null, int? editSize = null,
                                        Dictionary<string, string> validValues = null, string defaultValue = null, string linkedUdo = null, bool? mandatory = false)
        {
            if (!UserFieldExists(tableName, name))
                throw new Exception(string.Format("Userfield {0} doesn't exists in table {1}", name, tableName));

            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                var fieldId = SboDiUtils.QueryValue<int>(string.Format(@"SELECT ""FieldID"" FROM CUFD WHERE (""TableID"" = '{0}' OR ""TableID"" = '@{0}') AND (""AliasID"" = '{1}' OR ""AliasID"" = 'U_{1}')", tableName, name));

                var userFieldsMd = factory.Create<UserFieldsMD>(BoObjectTypes.oUserFields);
                if (!userFieldsMd.GetByKey(tableName, fieldId))
                    throw new Exception(string.Format("Could not find userfield {0} in table {1}", name, tableName));

                if (!String.IsNullOrWhiteSpace(description))
                    userFieldsMd.Description = description;
                if (editSize.HasValue)
                    userFieldsMd.EditSize = editSize.Value;
                /* TODO: Update validvalues
                if (validValues != null)
                {
                    int insertedValues = 0;
                    foreach (string key in validValues.Keys)
                    {
                        userFieldsMd.ValidValues.Value = key;
                        userFieldsMd.ValidValues.Description = validValues[key];

                        if (insertedValues < validValues.Count - 1)
                            userFieldsMd.ValidValues.Add();

                        insertedValues++;
                    }
                }*/
                if (!String.IsNullOrWhiteSpace(defaultValue))
                    userFieldsMd.DefaultValue = defaultValue;
                if (!String.IsNullOrWhiteSpace(linkedUdo))
                    userFieldsMd.LinkedUDO = linkedUdo;

                if (mandatory.HasValue)
                    userFieldsMd.Mandatory = mandatory.GetValueOrDefault(false) ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;

                Logger.Debug("Updating field {0} in table {1}", name, tableName);
                if (userFieldsMd.Update() != 0)
                    throw new Exception(SboAddon.Instance.Company.GetLastErrorDescription());

                Logger.Info("Updated field {0} in table {1}", name, tableName);
            }
        }

        public static void AddUserFieldIfNotExist(string tableName, string name, string description,
                                                          BoFieldTypes type = BoFieldTypes.db_Alpha, BoFldSubTypes subType = BoFldSubTypes.st_None, int? editSize = null,
                                                          Dictionary<string, string> validValues = null, string defaultValue = null, string linkedUdo = null, bool? mandatory = false)
        {
            if (UserFieldExists(tableName, name))
                Logger.Info("UserField {1} found in table {0}, skipping creation.", tableName, name);
            else
                AddUserField(tableName, name, description, type, subType, editSize, validValues, defaultValue, linkedUdo, mandatory);
        }

        public static bool UserTableExists(string name)
        {
            if (name.StartsWith("@"))
                name = name.Substring(1);

            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                var userTablesMd = factory.Create<UserTablesMD>(BoObjectTypes.oUserTables);
                return userTablesMd.GetByKey(name);
            }
        }

        public static void AddUserTable(String name, String description, BoUTBTableType type)
        {
            if (UserTableExists(name))
                throw new Exception(string.Format("Table '{0}' already exists", name));

            if (name.StartsWith("@"))
                name = name.Substring(1);

            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                var userTablesMd = factory.Create<UserTablesMD>(BoObjectTypes.oUserTables);

                userTablesMd.TableName = name;
                userTablesMd.TableDescription = description;
                userTablesMd.TableType = type;

                if (userTablesMd.Add() != 0)
                    throw new Exception(SboAddon.Instance.Company.GetLastErrorDescription());
                
                Logger.Info("Table '{0}' was added successfully", userTablesMd.TableName);
            }
        }

        public static void AddUserTable(Type definition)
        {
            if (!(typeof(SboUserDefinedTableDefinition).IsAssignableFrom(definition)))
                throw new ArgumentException("Table definition must be of type SboUserDefinedTableDefinition");
        
            var tableName = definition.GetCustomAttributes(typeof(SboUserDefinedTableAttribute), true).Select(a => ((SboUserDefinedTableAttribute)a).Name).FirstOrDefault();
            var tableDescription = definition.GetCustomAttributes(typeof(SboUserDefinedTableAttribute), true).Select(a => ((SboUserDefinedTableAttribute)a).Description).FirstOrDefault();
            var tableType = definition.GetCustomAttributes(typeof(SboUserDefinedTableAttribute), true).Select(a => ((SboUserDefinedTableAttribute)a).Type).FirstOrDefault();
            var isObject = definition.GetCustomAttributes(typeof (SboUserDefinedObjectAttribute), true).Any();

            AddUserTableIfNotExist(tableName, tableDescription, tableType);

            var properties = from property in definition.GetProperties()
                let udfAttribute =
                    property.GetCustomAttributes(typeof (SboUserDefinedFieldAttribute), false).SingleOrDefault() as
                        SboUserDefinedFieldAttribute
                orderby udfAttribute != null ? udfAttribute.Order : 0  
                select property;
            foreach (var property in properties)
            {
                var udf = (SboUserDefinedFieldAttribute)property.GetCustomAttributes(false).FirstOrDefault(a => a.GetType() == typeof(SboUserDefinedFieldAttribute));
                if (udf == null)
                    continue;

                var fieldName = (udf.Name ?? property.Name);
                var fieldDescription = String.IsNullOrWhiteSpace(udf.Description) ? fieldName : udf.Description;
                var fieldType = udf.TypeNullable;
                var fieldSubType = udf.SubTypeNullable;
                var fieldMandatory = udf.Mandatory;
                if (!fieldType.HasValue)
                {
                    var propertyType = Nullable.GetUnderlyingType(property.GetMethod.ReturnType) ?? property.GetMethod.ReturnType;
                    if (propertyType == typeof(bool)) { fieldType = BoFieldTypes.db_Alpha; udf.Length = 1; }
                    else if (propertyType == typeof(short)) { fieldType = BoFieldTypes.db_Numeric; udf.Length = udf.LengthNullable ?? 6; }
                    else if (propertyType == typeof(int)) {fieldType = BoFieldTypes.db_Numeric; udf.Length = udf.LengthNullable ?? 11; }
                    else if (propertyType == typeof(long)) { throw new Exception("Long data type cannot be used in SAP Business One SDK UDF"); }
                    else if (propertyType == typeof(float)) { fieldType = BoFieldTypes.db_Float; }
                    else if (propertyType == typeof(double)) { fieldType = BoFieldTypes.db_Float; }
                    else if (propertyType == typeof(decimal)) { fieldType = BoFieldTypes.db_Float; }
                    else if (propertyType == typeof(DateTime)) { fieldType = BoFieldTypes.db_Date; }
                    else if (propertyType == typeof(char))
                    {
                        throw new Exception("Char datatype is not supported");
                    }
                    else if (propertyType == typeof (string))
                    {
                        fieldType = !udf.LengthNullable.HasValue || udf.LengthNullable < 255 ? BoFieldTypes.db_Alpha : BoFieldTypes.db_Memo;
                        udf.Length = udf.LengthNullable ?? 254;
                    }
                    else if (propertyType.IsEnum)
                    {
                        fieldType = BoFieldTypes.db_Alpha;
                        udf.Length = udf.LengthNullable ?? 254;
                    }
                    else
                    {
                        throw new Exception(string.Format("Unsupported data type for UDF: {0}", propertyType));
                    }
                }

                if (!UserFieldExists(tableName, fieldName))
                    AddUserField(tableName, fieldName, fieldDescription, fieldType ?? BoFieldTypes.db_Alpha, fieldSubType ?? BoFldSubTypes.st_None, udf.LengthNullable, null, null, null, fieldMandatory);
            }

            if (isObject)
            {
                AddUserObjectIfNotExist(definition);
            }
        }

        public static void AddUserTableIfNotExist(String name, String description, BoUTBTableType type = BoUTBTableType.bott_NoObject)
        {
            if (UserTableExists(name))
                Logger.Info("UserTable {0} found, skipping creation.", name);
            else
                AddUserTable(name, description, type);
        }

        public static void RenameUserTable(String oldName, String newName, String description = null)
        {
            if (oldName.StartsWith("@"))
                oldName = oldName.Substring(1);
            if (newName.StartsWith("@"))
                newName = newName.Substring(1);
            if (!UserTableExists(oldName))
                throw new Exception(String.Format("Cannot rename table {0} to {1}, table {0} doesn't exist", oldName, newName));
            if (UserTableExists(newName))
                throw new Exception(String.Format("Cannot rename table {0} to {1}, table {1} already exists", oldName, newName));

            var newFields = new List<dynamic>();
            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                var userTableOld = factory.Create<UserTablesMD>(BoObjectTypes.oUserTables);
                userTableOld.GetByKey(oldName);

                AddUserTable(newName, description ?? userTableOld.TableDescription, userTableOld.TableType);

                var userField = factory.Create<UserFieldsMD>(BoObjectTypes.oUserFields);
                var recordSet = factory.Create<Recordset>(BoObjectTypes.BoRecordset);
                recordSet.DoQuery(string.Format(@"SELECT * FROM CUFD WHERE (""TableID"" = '{0}' OR ""TableID"" = '@{0}')",
                    oldName));

                while (!recordSet.EoF)
                {
                    userField.GetByKey((string) recordSet.Fields.Item("TableID").Value, (int) recordSet.Fields.Item("FieldID").Value);
                    newFields.Add(
                        new
                        {
                            TableName = newName,
                            Name = userField.Name,
                            Description = userField.Description,
                            Type = userField.Type,
                            SubType = userField.SubType,
                            EditSize = userField.EditSize,
                            ValidValues = userField.ValidValues.ToDictionary(),
                            DefaultValue = userField.DefaultValue
                        });

                    recordSet.MoveNext();
                }
            }

            foreach (var f in newFields)
            {
                AddUserField(f.TableName, f.Name, f.Description, f.Type, f.SubType, f.EditSize, f.ValidValues, f.DefaultValue);
            }

            SboDiUtils.QueryValue<int>(string.Format(@"INSERT INTO ""@{0}"" SELECT * FROM ""@{1}""", newName, oldName));

            RemoveUserTable(oldName);

            Logger.Info("Renamed user table {0} to {1}", oldName, newName);
        }

        public static void RemoveUserTable(string name)
        {
            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                var userTable = factory.Create<UserTablesMD>(BoObjectTypes.oUserTables);
                userTable.GetByKey(name);
                if (userTable.Remove() != 0)
                    throw new Exception(string.Format("Error removing table {0}: {1}",
                        name,
                        SboAddon.Instance.Company.GetLastErrorDescription()));
            }
        }

        public static bool UserObjectExists(string name)
        {
            if (name.StartsWith("@"))
                name = name.Substring(1);

            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                var userObjectsMd = factory.Create<UserObjectsMD>(BoObjectTypes.oUserObjectsMD);
                return userObjectsMd.GetByKey(name);
            }
        }

        public static UserObjectsMD AddUserObject(String name, String description, BoUDOObjType type)
        {
            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                if (name.StartsWith("@"))
                    name = name.Substring(1);

                var userObjectMd = factory.Create<UserObjectsMD>(BoObjectTypes.oUserObjectsMD);
                userObjectMd.Code = name;
                userObjectMd.Name = description;
                userObjectMd.ObjectType = type;
                userObjectMd.TableName = name;

                if (userObjectMd.Add() != 0)
                    throw new Exception(SboAddon.Instance.Company.GetLastErrorDescription());

                Logger.Info("Object '{0}' was added successfully", userObjectMd.Name);
                return userObjectMd;
            }
        }

        public static void AddUserObjectIfNotExist(String name, String description, BoUDOObjType type)
        {
            if (UserObjectExists(name))
            {
                Logger.Info("UserObject {0} found, skipping creation.", name);
                return;
            }

            AddUserObject(name, description, type);
        }

        private static void AddUserObjectIfNotExist(Type definition)
        {
            var tableName = definition.GetCustomAttributes(typeof(SboUserDefinedTableAttribute), true).Select(a => ((SboUserDefinedTableAttribute)a).Name).FirstOrDefault();

            var objectAttribute = definition.GetCustomAttributes(typeof (SboUserDefinedObjectAttribute), true);
            var objectName = objectAttribute.Select(a => ((SboUserDefinedObjectAttribute)a).Name).FirstOrDefault();
            var objectDescription = objectAttribute.Select(a => ((SboUserDefinedObjectAttribute)a).Description).FirstOrDefault();
            var objectType = objectAttribute.Select(a => ((SboUserDefinedObjectAttribute)a).Type).FirstOrDefault();
            var objectFormType = objectAttribute.Select(a => ((SboUserDefinedObjectAttribute)a).FormType).FirstOrDefault();
            var menuFather = objectAttribute.Select(a => ((SboUserDefinedObjectAttribute)a).FatherMenu).FirstOrDefault();
            var menuUid = objectAttribute.Select(a => ((SboUserDefinedObjectAttribute)a).MenuUid).FirstOrDefault();
            var menuCaption = objectAttribute.Select(a => ((SboUserDefinedObjectAttribute)a).MenuCaption).FirstOrDefault();
            var formXmlResourcePath = objectAttribute.Select(a => ((SboUserDefinedObjectAttribute)a).FormXmlResourcePath).FirstOrDefault();

            if (UserObjectExists(objectName))
                return;

            var properties = (from property in definition.GetProperties()
                let udfAttribute =
                    property.GetCustomAttributes(typeof (SboUserDefinedFieldAttribute), false).SingleOrDefault() as
                        SboUserDefinedFieldAttribute
                where udfAttribute != null
                orderby udfAttribute.Order
                select new {Property = property, PropertyAttribute = udfAttribute}).ToList();

            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                var userObjectMd = factory.Create<UserObjectsMD>(BoObjectTypes.oUserObjectsMD);

                userObjectMd.Code = objectName;
                userObjectMd.Name = objectDescription;
                userObjectMd.ObjectType = objectType;
                userObjectMd.TableName = tableName;
                if (objectFormType.Equals(SboUserDefinedObjectFormType.Matrix))
                {
                    userObjectMd.CanCreateDefaultForm = BoYesNoEnum.tYES;
                    userObjectMd.EnableEnhancedForm = BoYesNoEnum.tNO;
                    userObjectMd.UseUniqueFormType = BoYesNoEnum.tYES;
                    userObjectMd.FormColumns.FormColumnAlias = "Code";
                    userObjectMd.FormColumns.FormColumnDescription = "Code";
                    userObjectMd.FormColumns.Add();
                    foreach (var p in properties.Where(p => p.PropertyAttribute.FormField))
                    {
                        userObjectMd.FormColumns.FormColumnAlias = "U_" + (String.IsNullOrEmpty(p.PropertyAttribute.Name) ? p.Property.Name : p.PropertyAttribute.Name);
                        userObjectMd.FormColumns.FormColumnDescription = p.PropertyAttribute.Description;
                        userObjectMd.FormColumns.Editable = BoYesNoEnum.tYES;
                        userObjectMd.FormColumns.Add();
                    }
                }
                else if (objectFormType.Equals(SboUserDefinedObjectFormType.Enhanced))
                {
                    userObjectMd.CanCreateDefaultForm = BoYesNoEnum.tYES;
                    userObjectMd.EnableEnhancedForm = BoYesNoEnum.tYES;
                    userObjectMd.UseUniqueFormType = BoYesNoEnum.tYES;
                    userObjectMd.FormColumns.FormColumnAlias = "Code";
                    userObjectMd.FormColumns.FormColumnDescription = "Code";
                    userObjectMd.FormColumns.Add();
                    foreach (var p in properties.Where(p => p.PropertyAttribute.FormField))
                    {
                        userObjectMd.FormColumns.FormColumnAlias = "U_" + (String.IsNullOrEmpty(p.PropertyAttribute.Name) ? p.Property.Name : p.PropertyAttribute.Name);
                        userObjectMd.FormColumns.FormColumnDescription = p.PropertyAttribute.Description;
                        userObjectMd.FormColumns.Editable = BoYesNoEnum.tYES;
                        userObjectMd.FormColumns.Add();
                    }
                }
                if (properties.Any(p => p.PropertyAttribute.SearchField))
                {
                    userObjectMd.CanFind = BoYesNoEnum.tYES;
                    foreach (var p in properties.Where(p => p.PropertyAttribute.SearchField))
                    {
                        userObjectMd.FindColumns.ColumnAlias = "U_" + (String.IsNullOrEmpty(p.PropertyAttribute.Name) ? p.Property.Name : p.PropertyAttribute.Name);
                        userObjectMd.FindColumns.ColumnDescription = p.PropertyAttribute.Description;
                        userObjectMd.FindColumns.Add();
                    }
                }

                if (menuFather > 0)
                {
                    userObjectMd.MenuItem = BoYesNoEnum.tYES;
                    userObjectMd.FatherMenuID = menuFather;
                    userObjectMd.MenuUID = menuUid ?? objectName;
                    userObjectMd.MenuCaption = menuCaption ?? objectDescription;
                }

                if (userObjectMd.Add() != 0)
                    throw new Exception(SboAddon.Instance.Company.GetLastErrorDescription());

                Logger.Info("Object '{0}' was added successfully", userObjectMd.Name);
            }
        }

        public static string GetNextTableCode(string tableName, int codeLength = 10)
        {
            if (!tableName.StartsWith("@")) tableName = "@" + tableName;

            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                var recordSet = factory.Create<Recordset>(BoObjectTypes.BoRecordset);
                recordSet.DoQuery(string.Format(SboAddon.Instance.Company.DbServerType.Equals(BoDataServerTypes.dst_HANADB) ?
                    @"SELECT CAST(MAX(CAST(""Code"" AS BIGINT)) AS NVARCHAR(30)) AS Code FROM ""{0}""" :
                    @"SELECT CONVERT(nvarchar(30), MAX(CONVERT(bigint, Code))) [Code] FROM [{0}] WHERE ISNUMERIC(Code) = 1", tableName));

                long? code = (recordSet.RecordCount == 0 ||
                              String.IsNullOrWhiteSpace((string) recordSet.Fields.Item("Code").Value))
                    ? 0
                    : Convert.ToInt64(recordSet.Fields.Item("Code").Value, CultureInfo.InvariantCulture);

                if (!code.HasValue)
                    code = 0; 

                return (code.Value + 1).ToString(CultureInfo.InvariantCulture).PadLeft(codeLength, '0');
            }
        }
    }

    public class SboUserDefinedTableDefinition
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class SboUserDefinedTableAttribute : Attribute
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public BoUTBTableType? TypeNullable;
        public BoUTBTableType Type { get { return TypeNullable ?? BoUTBTableType.bott_NoObject; } set { TypeNullable = value; } }
    }

    public class SboUserDefinedObjectAttribute : Attribute
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public BoUDOObjType? TypeNullable;
        public BoUDOObjType Type { get { return TypeNullable ?? BoUDOObjType.boud_MasterData; } set { TypeNullable = value; } }

        public SboUserDefinedObjectFormType? FormTypeNullable { get; set; }
        public SboUserDefinedObjectFormType FormType { get { return FormTypeNullable ?? SboUserDefinedObjectFormType.Matrix; } set { FormTypeNullable = value; } }

        public int FatherMenu { get; set; }
        public string MenuUid { get; set; }
        public string MenuCaption { get; set; }

        public string FormXmlResourcePath { get; set; }
    }

    public enum SboUserDefinedObjectFormType
    {
        Matrix,
        Enhanced,
        Custom
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class SboUserDefinedFieldAttribute : Attribute
    {
        private readonly int _order;
        public SboUserDefinedFieldAttribute([CallerLineNumber] int order = 0)
        {
            _order = order;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? LengthNullable;
        public int Length { get { return LengthNullable.GetValueOrDefault(); } set { LengthNullable = value; } }

        public BoFieldTypes? TypeNullable;
        public BoFieldTypes Type { get { return TypeNullable.GetValueOrDefault(); } set { TypeNullable = value; } }

        public BoFldSubTypes? SubTypeNullable;
        public BoFldSubTypes SubType { get { return SubTypeNullable.GetValueOrDefault(); } set { SubTypeNullable = value; } }

        public bool? MandatoryNullable;
        public bool Mandatory { get { return MandatoryNullable.GetValueOrDefault(); } set { MandatoryNullable = value; } }

        public bool? SearchFieldNullable;
        public bool SearchField { get { return SearchFieldNullable.GetValueOrDefault(); } set { SearchFieldNullable = value; } }

        public bool? FormFieldNullable;
        public bool FormField { get { return FormFieldNullable.GetValueOrDefault(); } set { FormFieldNullable = value; } }

        public int Order { get { return _order; } }
    }
}
