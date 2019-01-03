using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using NLog;
using SAPbobsCOM;
using SAPbouiCOM;
using Field = SAPbobsCOM.Field;
using Items = SAPbouiCOM.Items;
using ValidValue = SAPbouiCOM.ValidValue;
using ValidValues = SAPbouiCOM.ValidValues;

namespace SapBusinessOneExtensions
{
    public static class SboDatasourceExtensions
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static bool Contains(this DataTables dataTables, string tableUid)
        {
            for (int i = 0; dataTables != null && i < dataTables.Count; i++)
            {
                if (dataTables.Item(i).UniqueID.Equals(tableUid))
                    return true;
            }

            return false;
        }

        public static void ExecuteQueryEx(this DataTable dataTable, string query, params object[] args)
        {
            var inQuery = String.Format(query, args);
            try
            {
                Logger.Trace("Executing query on datatable: {0}", inQuery);

                var sw = Stopwatch.StartNew();
                dataTable.ExecuteQuery(inQuery);
                sw.Stop();

                SboAddonTracker.TrackEvent("Query", metrics: new Dictionary<string, double> { ["RecordCount"] = dataTable.Rows.Count , ["Duration"] = sw.ElapsedMilliseconds } );
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error executing query: " + inQuery);
                throw;
            }
        }

        public static T GetValue<T>(this DataTable dataTable, object column, int rowIndex)
        {
            return (T) dataTable.GetValue(column, rowIndex);
        }

        public static XDocument AsXDocument(this DataTable dataTable)
        {
            return XDocument.Parse(dataTable.SerializeAsXML(BoDataTableXmlSelect.dxs_All));
        }

        public static List<T> GetColumnValues<T>(this DataTable dataTable, string column)
        {
            var xdoc = dataTable.AsXDocument();
            return (from r in xdoc.Root.Element("Rows").Elements("Row")
                let c = r.Descendants("ColumnUid").First(e => column.Equals(e.Value))
                where c?.Parent != null && c.Parent.HasElements
                select c.Parent.Element("Value").Value).Cast<T>().ToList();
        }

        public static List<T> GetColumnValues<T>(this DataTable dataTable, int column)
        {
            var xdoc = dataTable.AsXDocument();
            return (from r in xdoc.Root.Element("Rows").Elements("Row")
                let c = r.Descendants("ColumnUid").ElementAt(column)
                where c?.Parent != null && c.Parent.HasElements
                select c.Parent.Element("Value").Value).Cast<T>().ToList();
        }

        public static T DoQueryValue<T>(this Recordset recordset, string query, params object[] args)
        {
            var queryText = string.Format(query, args);
            try
            {
                var cacheKey = String.Format("SboExtensions.QueryCache({0})", queryText);
                if (SboTemporaryCache.Contains(cacheKey))
                    return SboTemporaryCache.Get<T>(cacheKey);

                recordset.DoQuery(queryText);
                return SboTemporaryCache.Set(cacheKey, recordset.RecordCount < 1 ? default(T) : (T) recordset.Fields.Item(0).Value);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error running query: {0}", queryText);
                throw;
            }
        }

        public static Dictionary<T1, T2> DoQueryDictionary<T1, T2>(this Recordset recordset, string query, params object[] args)
        {
            var strQuery = String.Format(query, args);
            try
            {
                recordset.DoQuery(strQuery);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error running query: " + strQuery);
                throw;
            }

            var dict = new Dictionary<T1, T2>();
            while (!recordset.EoF)
            {
                dict.Add((T1) recordset.Fields.Item(0).Value, (T2) recordset.Fields.Item(1).Value);
                recordset.MoveNext();
            }

            recordset.MoveFirst();
            return dict;
        }

        public static List<Dictionary<string, object>> DoQueryList(this Recordset recordset, string query, params object[] args)
        {
            var strQuery = args?.Length > 0 ? String.Format(query, args) : query;
            try
            {
                recordset.DoQuery(strQuery);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error running query: " + strQuery);
                throw;
            }

            var list = new List<Dictionary<string, object>>();
            while (!recordset.EoF)
            {
                var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                foreach (Field field in recordset.Fields)
                {
                    dict.Add(field.Name, recordset.Fields.Item(field.Name).Value);
                }
                list.Add(dict);
                recordset.MoveNext();
            }

            recordset.MoveFirst();
            return list;
        }

        public static T GetRecord<T>(this Recordset recordset, string code) where T : SboUserDefinedTableDefinition, new()
        {
            var record = new T();

            var tableName = record.GetType().GetCustomAttributes(typeof(SboUserDefinedTableAttribute), true).Select(a => ((SboUserDefinedTableAttribute)a).Name).FirstOrDefault();

            var columns = GetUserDefinedTableDefinitionSqlColumns(record);
            var selectColumns = SboAddon.Instance.Company.DbServerType.Equals(BoDataServerTypes.dst_HANADB)
                ? String.Join(", ",
                    columns.Select(c =>
                        c.Value == typeof(DateTime)
                            ? $@"""{c.Key}"", TO_NVARCHAR(""{c.Key}"", 'YYYY-MM-DD HH24:MI:SS.FF3') ""{c.Key}_RawDateTimeHack"""
                            : $@"""{c.Key}"""))
                : String.Join(", ",
                    columns.Select(c =>
                        c.Value == typeof(DateTime)
                            ? $@"""{c.Key}"", CONVERT(NVARCHAR, ""{c.Key}"", 121) ""{c.Key}_RawDateTimeHack"""
                            : $@"""{c.Key}"""));

            recordset.DoQuery(string.Format($@"SELECT {selectColumns} FROM ""@{tableName}"" WHERE ""Code"" = '{code}'"));
            if (recordset.RecordCount == 0)
                return null;

            var udfs = (from property in record.GetType().GetProperties()
                        let attribute = property.GetCustomAttributes(false).FirstOrDefault(a => a.GetType() == typeof(SboUserDefinedFieldAttribute))
                        select new { Property = property, Attribute = (SboUserDefinedFieldAttribute) attribute }).ToList();

            foreach (Field field in recordset.Fields)
            {
                if (field.IsNull().Equals(BoYesNoEnum.tYES))
                    continue;

                var fieldName = field.Name;
                if (fieldName.StartsWith("U_"))
                    fieldName = fieldName.Substring(2);

                var property =
                    udfs.Where(udf => (udf.Attribute != null && fieldName.Equals(udf.Attribute.Name)) || fieldName.Equals(udf.Property.Name))
                        .Select(udf => udf.Property)
                        .FirstOrDefault();

                if (property == null)
                {
                    if (!fieldName.Contains("_RawDateTimeHack"))
                        Logger.Warn($"Unknown record field name: {fieldName}");
                    continue;
                }

                var val = recordset.Fields.Item(field.Name).Value;

                var propertyType = Nullable.GetUnderlyingType(property.GetMethod.ReturnType) ?? property.GetMethod.ReturnType;
                object convertedVal = val;

                if (propertyType == typeof(bool)) convertedVal = "Y".Equals(val);
                else if (propertyType == typeof(short)) convertedVal = Convert.ToInt16(val);
                else if (propertyType.IsEnum) convertedVal = Enum.Parse(propertyType, Convert.ToString(val));
                else if (propertyType == typeof(DateTime))
                {
                    var hackVal = recordset.Fields.Item(field.Name + "_RawDateTimeHack").Value as string;
                    if (!string.IsNullOrWhiteSpace(hackVal) && hackVal.Length > 15)
                    {
                        var convertedValHack = DateTime.ParseExact(hackVal, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        if (convertedValHack.Date == (DateTime) convertedVal)
                            convertedVal = convertedValHack;
                    }
                }

                property.SetValue(record, convertedVal);
            }

            return record;
        } 

        public static void Replace(this Recordset recordset, SboUserDefinedTableDefinition record)
        {
            var tableName = record.GetType().GetCustomAttributes(typeof(SboUserDefinedTableAttribute), true).Select(a => ((SboUserDefinedTableAttribute)a).Name).FirstOrDefault();
            if (recordset.DoQueryValue<int>(@"SELECT COUNT(*) FROM ""@{0}"" WHERE ""Code"" = '{1}'", tableName, record.Code) > 0)
                Update(recordset, record);
            else
                Insert(recordset, record);
        }

        public static void Insert(this Recordset recordset, SboUserDefinedTableDefinition record)
        {
            var tableName = record.GetType().GetCustomAttributes(typeof(SboUserDefinedTableAttribute), true).Select(a => ((SboUserDefinedTableAttribute) a).Name).FirstOrDefault();
            if (String.IsNullOrEmpty(record.Code))
                record.Code = SboUserDefinedDataManager.GetNextTableCode(tableName);
            if (String.IsNullOrEmpty(record.Name))
                record.Name = record.Code;

            var fieldValues = GetUserDefinedTableDefinitionSqlValues(record);
            
            var query = String.Format(@"INSERT INTO ""@{0}"" ({1}) VALUES ({2})", tableName, String.Join(",", fieldValues.Keys.Select(k => $"\"{k}\"")), String.Join(",", fieldValues.Values));
            Logger.Trace("Inserting into UDT: {0}", query);

            recordset.DoQuery(query);
        }

        public static void Update(this Recordset recordset, SboUserDefinedTableDefinition record)
        {
            var tableName = record.GetType().GetCustomAttributes(typeof(SboUserDefinedTableAttribute), true).Select(a => ((SboUserDefinedTableAttribute)a).Name).FirstOrDefault();
            var fieldValues = GetUserDefinedTableDefinitionSqlValues(record);
            var updateValues = fieldValues.Where(fv => !fv.Key.Equals("Code")).Select(fv => $"\"{fv.Key}\" = {fv.Value}");

            var query = String.Format(@"UPDATE ""@{0}"" SET {1} WHERE ""Code"" = {2}", tableName, String.Join(",", updateValues), fieldValues["Code"]);
            Logger.Trace("Updating UDT: {0}", query);

            recordset.DoQuery(query);
        }

        private static Dictionary<string, Type> GetUserDefinedTableDefinitionSqlColumns(SboUserDefinedTableDefinition record)
        {
            var sqlColumns = new Dictionary<string, Type>();
            foreach (var property in record.GetType().GetProperties())
            {
                var udf =
                    (SboUserDefinedFieldAttribute)
                    property.GetCustomAttributes(false).FirstOrDefault(a => a.GetType() == typeof(SboUserDefinedFieldAttribute));
                if (udf == null && !(new[] {"Code", "Name"}).Contains(property.Name))
                    continue;

                var sqlFieldName = (new[] {"Code", "Name"}).Contains(property.Name)
                    ? property.Name
                    : "U_" + (udf != null ? udf.Name ?? property.Name : property.Name);

                var propertyType = Nullable.GetUnderlyingType(property.GetMethod.ReturnType) ?? property.GetMethod.ReturnType;

                sqlColumns.Add(sqlFieldName, propertyType);
            }

            return sqlColumns;
        }

        private static Dictionary<string, string> GetUserDefinedTableDefinitionSqlValues(SboUserDefinedTableDefinition record)
        {
            var sqlFieldValues = new Dictionary<string, string>();
            foreach (var property in record.GetType().GetProperties())
            {
                var udf = (SboUserDefinedFieldAttribute)property.GetCustomAttributes(false).FirstOrDefault(a => a.GetType() == typeof(SboUserDefinedFieldAttribute));
                if (udf == null && !(new [] {"Code", "Name"}).Contains(property.Name))
                    continue;

                var sqlFieldName = (new[] {"Code", "Name"}).Contains(property.Name)
                    ? property.Name
                    : "U_" + (udf != null ? udf.Name ?? property.Name : property.Name);

                var val = property.GetMethod.Invoke(record, null);
                if (val is string)
                    val = ((string) val).Replace("'", "''");

                string strVal = String.Empty;

                var propertyType = Nullable.GetUnderlyingType(property.GetMethod.ReturnType) ?? property.GetMethod.ReturnType;
                if (val == null) strVal = "NULL";
                else if (propertyType == typeof(bool))
                    strVal = String.Format(CultureInfo.InvariantCulture, "'{0}'", ((bool)val) ? "Y" : "N");
                else if (propertyType == typeof(string)) strVal = String.Format(CultureInfo.InvariantCulture, "'{0}'", val);
                else if (propertyType == typeof(short)) strVal = String.Format(CultureInfo.InvariantCulture, "{0}", val);
                else if (propertyType == typeof(int)) strVal = String.Format(CultureInfo.InvariantCulture, "{0}", val);
                else if (propertyType == typeof(long)) strVal = String.Format(CultureInfo.InvariantCulture, "{0}", val);
                else if (propertyType == typeof(float)) strVal = String.Format(CultureInfo.InvariantCulture, "{0:R}", val);
                else if (propertyType == typeof(double)) strVal = String.Format(CultureInfo.InvariantCulture, "{0:R}", val);
                else if (propertyType == typeof(DateTime) && BoFldSubTypes.st_Time.Equals(udf?.SubType)) strVal = String.Format(CultureInfo.InvariantCulture, "'{0:HHmm}'", val);
                else if (propertyType == typeof(DateTime)) strVal = String.Format(CultureInfo.InvariantCulture, "'{0:yyyy-MM-dd'T'HH':'mm':'ss.fff}'", val);
                else if (propertyType.IsEnum) strVal = String.Format(CultureInfo.InvariantCulture, "'{0}'", Enum.GetName(val.GetType(), val));
                else throw new Exception(string.Format("Unknown data type for property {0}", property.Name));

                sqlFieldValues.Add(sqlFieldName, strVal);
            }

            return sqlFieldValues;
        }

        public static Dictionary<string, string> ToDictionary(this ValidValuesMD validValues)
        {
            if (validValues == null || validValues.Count == 0)
                return null;

            var dictionary = new Dictionary<string, string>();
            for (int i = 0; i < validValues.Count; i++)
            {
                validValues.SetCurrentLine(i);
                dictionary.Add(validValues.Value, validValues.Description);
            }

            return dictionary;
        }
    }

    public static class SboItemExtensions
    {
        public static bool ContainsAll(this Items items, params string[] itemUids)
        {
            return itemUids.All(items.Contains);
        }

        public static bool Contains(this Items items, string itemUid)
        {
            try
            {
                return items.Item(itemUid) != null;
            }
            catch (Exception)
            {
                return false;
            }
/* Better but slower way....
            for (int i = 0; items != null && i < items.Count; i++)
            {
                if (items.Item(i).UniqueID.Equals(itemUid))
                    return true;
            }

            return false;
*/
        }

        public static T Item<T>(this Items items, object index)
        {
            return items.Item(index).Specific<T>();
        }

        public static T Item<T>(this GridColumns columns, object index)
        {
            return (T) columns.Item(index);
        }

        public static T Specific<T>(this Item item)
        {
            return (T) item.Specific;
        }

        public static void Add(this ValidValues validValues, Dictionary<string, string> values)
        {
            foreach (var val in values)
                if (validValues.Count == 0 || !validValues.Cast<ValidValue>().Any(vv => vv.Value.Equals(val.Key)))
                    validValues.Add(val.Key, val.Value);
        }

        public static void Set(this ValidValues validValues, Dictionary<string, string> values)
        {
            for (int i = validValues.Count - 1; i>=0; i--)
                validValues.Remove(i, BoSearchKey.psk_Index);

            validValues.Add(values);
        }

        public static int? IndexOf(this GridColumns columns, String colUid)
        {
            int index = 1;
            foreach (GridColumn col in columns)
                if (col.UniqueID.Equals(colUid))
                    return index;
                else
                    index++;

            return null;
        }

        public static void SetTextWithEvent(this EditTextColumn column, int row, string text)
        {
            column.Click(row);
            if (SboAddon.Instance.Application.Menus.Item("775").Enabled)
                SboAddon.Instance.Application.ActivateMenuItem("775");
            if (SboAddon.Instance.Application.Menus.Item("774").Enabled)
                SboAddon.Instance.Application.ActivateMenuItem("774");
            SboAddon.Instance.Application.SendKeys(text);
        }
    }

    public static class SboMenuExtensions
    {
        public static MenuItem AddOrUpdate(this Menus menus, string uniqueId, string parent, string title,
            BoMenuType menuType = BoMenuType.mt_STRING, bool enabled = true, int position = 1,
            MenuItemRelativePosition relativePosition = null)
        {
            MenuItem menuItem = menus.Item(parent);
            Menus subMenus = menuItem.SubMenus;
            if (!subMenus.Exists(uniqueId))
            {
                var creationPackage =
                    (MenuCreationParams)
                        SboAddon.Instance.Application.CreateObject(BoCreatableObjectType.cot_MenuCreationParams);
                creationPackage.Type = menuType;
                creationPackage.UniqueID = uniqueId;
                creationPackage.String = title;
                creationPackage.Enabled = enabled;
                if (relativePosition != null)
                    creationPackage.Position =
                        subMenus.PositionOf(relativePosition.RelativeTo).GetValueOrDefault(position) +
                        relativePosition.PlusPositions;
                else
                    creationPackage.Position = position;

                return subMenus.AddEx(creationPackage);
            }
            MenuItem existing = subMenus.Item(uniqueId);
            if (!title.Equals(existing.String))
                existing.String = title;
            if (!enabled.Equals(existing.Enabled))
                existing.Enabled = enabled;

            return existing;
        }

        public static void RemoveIfExist(this Menus menus, string menuUid)
        {
            try
            {
                menus.RemoveEx(menuUid);
            }
            catch (Exception) { }

            /* TODO: Better way - but doesn't work
            if (menus.Cast<MenuItem>().Any(item => item.UID.Equals(menuUid)))
            {
                menus.RemoveEx(menuUid);
            }*/
        }

        public static int? PositionOf(this Menus menus, string menuUid)
        {
            int index = 0;
            foreach (MenuItem item in menus)
            {
                index++;

                if (item.UID.Equals(menuUid))
                    return index;
            }

            return null;
        }

        public class MenuItemRelativePosition
        {
            public MenuItemRelativePosition(string relativeTo, short plusPositions)
            {
                RelativeTo = relativeTo;
                PlusPositions = plusPositions;
            }

            public string RelativeTo { get; set; }
            public short PlusPositions { get; set; }
        }
    }

    public static class EventExtensions
    {
        public static bool Matches(this ItemEvent itemEvent, bool? actionSuccess = null, bool? beforeAction = null,
            int? charPressed = null, string colUid = null, Enum eventType = null, int? formMode = null,
            int? formTypeCount = null, string formTypeEx = null, string formUid = null, bool? innerEvent = null,
            bool? itemChanged = null, string itemUid = null, Enum modifiers = null,
            int? popUpIndicator = null, int? row = null)
        {
            if (actionSuccess.HasValue && !itemEvent.ActionSuccess.Equals(actionSuccess.Value))
                return false;
            if (beforeAction.HasValue && !itemEvent.BeforeAction.Equals(beforeAction.Value))
                return false;
            if (charPressed.HasValue && !itemEvent.CharPressed.Equals(charPressed.Value))
                return false;
            if (colUid != null && !itemEvent.ColUID.Equals(colUid))
                return false;
            if (eventType != null && !itemEvent.EventType.ToString().Equals(eventType.ToString()))
                return false;
            if (formMode != null && !itemEvent.FormMode.Equals(formMode))
                return false;
            if (formTypeCount.HasValue && !itemEvent.FormTypeCount.Equals(formTypeCount.Value))
                return false;
            if (formTypeEx != null && !itemEvent.FormTypeEx.Equals(formTypeEx))
                return false;
            if (formUid != null && !itemEvent.FormUID.Equals(formUid))
                return false;
            if (innerEvent.HasValue && !itemEvent.InnerEvent.Equals(innerEvent.Value))
                return false;
            if (itemChanged.HasValue && !itemEvent.ItemChanged.Equals(itemChanged.Value))
                return false;
            if (itemUid != null && !itemEvent.ItemUID.Equals(itemUid))
                return false;
            if (modifiers != null && !itemEvent.Modifiers.ToString().Equals(modifiers.ToString()))
                return false;
            if (popUpIndicator.HasValue && !itemEvent.PopUpIndicator.Equals(popUpIndicator.Value))
                return false;
            if (row.HasValue && !itemEvent.Row.Equals(row.Value))
                return false;

            return true;
        }

        public static bool Matches(this BusinessObjectInfo dataEvent, bool? actionSuccess = null, bool? beforeAction = null,
            Enum eventType = null, List<Enum> eventTypes = null, string formTypeEx = null, string formUid = null, string type = null)
        {
            if (actionSuccess.HasValue && !dataEvent.ActionSuccess.Equals(actionSuccess.Value))
                return false;
            if (beforeAction.HasValue && !dataEvent.BeforeAction.Equals(beforeAction.Value))
                return false;
            if (eventType != null && !dataEvent.EventType.ToString().Equals(eventType.ToString()))
                return false;
            if (eventTypes != null && !eventTypes.Any(et => dataEvent.EventType.ToString().Equals(et.ToString())))
                return false;
            if (formTypeEx != null && !dataEvent.FormTypeEx.Equals(formTypeEx))
                return false;
            if (formUid != null && !dataEvent.FormUID.Equals(formUid))
                return false;
            if (type != null && !dataEvent.Type.Equals(type))
                return false;

            return true;
        }

        public static bool Matches(this ContextMenuInfo rcEvent, bool? actionSuccess = null, bool? beforeAction = null,
            Enum eventType = null, string formUid = null, string itemUid = null, string colUid = null, int? row = null)
        {
            if (actionSuccess.HasValue && !rcEvent.ActionSuccess.Equals(actionSuccess.Value))
                return false;
            if (beforeAction.HasValue && !rcEvent.BeforeAction.Equals(beforeAction.Value))
                return false;
            if (eventType != null && !rcEvent.EventType.ToString().Equals(eventType.ToString()))
                return false;
            if (formUid != null && !rcEvent.FormUID.Equals(formUid))
                return false;
            if (itemUid != null && !rcEvent.ItemUID.Equals(itemUid))
                return false;
            if (colUid != null && !rcEvent.ColUID.Equals(colUid))
                return false;
            if (row.HasValue && !row.Equals(rcEvent.Row))
                return false;

            return true;
        }
    }

    public static class OtherExtensions
    {
        public static int ToSapColor(this Color color)
        {
            return color.R | (color.G << 8) | (color.B << 16);
        }
    }
}