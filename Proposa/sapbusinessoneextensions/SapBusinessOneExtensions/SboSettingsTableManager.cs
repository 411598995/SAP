using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Caching;
using NLog;
using SAPbobsCOM;

namespace SapBusinessOneExtensions
{
    public interface ISboSettingsTableManager
    {
        T GetValue<T>(string key);
        T GetValueOrDefault<T>(string key, T defaultValue);
        T GetValueOrDefault<T>(string key) where T : new();
        void SetValue<T>(string key, T val);
        void SetValueIfNotSet<T>(string key, T val);
    }

    public class SboSettingsTableManager : ISboSettingsTableManager
    {
        protected static Logger Log = LogManager.GetCurrentClassLogger();

        private readonly ObjectCache _cache;

        private readonly string _addonName;
        private readonly string _tableName;
        private readonly string _userName;

        public SboSettingsTableManager(string tableName, string addonName, string userName = null, ObjectCache cache = null)
        {
            _tableName = tableName;
            _addonName = addonName;
            _userName = userName;

            _cache = cache ?? new MemoryCache(_tableName + _addonName + _userName + "_settings");

            Initialize();
        }

        public T GetValue<T>(string key)
        {
            var val = GetSettingValue(key);
            if (val == null)
                return (T) (object) null;

            return ConvertValue<T>(val);
        }

        public T GetValueOrDefault<T>(string key, T defaultValue)
        {
            try
            {
                var val = GetSettingValue(key);
                if (val == null)
                    return defaultValue;

                return ConvertValue<T>(val);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public T GetValueOrDefault<T>(string key) where T : new()
        {
            return GetValueOrDefault(key, new T());
        }

        public T ConvertValue<T>(string val)
        {
            var converter = TypeDescriptor.GetConverter(typeof (T));
            return (T) converter.ConvertFromInvariantString(val);
        }

        public void SetValue<T>(string key, T val)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            var setVal = Equals(val, null) ? String.Empty : converter.ConvertToInvariantString(val);

            SetSettingValue(key, setVal);
        }

        public void SetValueIfNotSet<T>(string key, T val)
        {
            InitSetting(key, Convert.ToString(val, CultureInfo.InvariantCulture));
        }

        private bool HasSetting(string key)
        {
            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                var recordSet = factory.Create<Recordset>(BoObjectTypes.BoRecordset);

                recordSet.DoQuery(_userName == null
                                      ? string.Format(
                                          @"SELECT ""Code"" FROM ""@{0}"" WHERE ""U_Addon"" = '{1}' AND RTRIM(COALESCE(""U_User"", '')) = '' AND ""U_BigKey"" = '{2}'", _tableName,
                                          _addonName, key)
                                      : string.Format(
                                          @"SELECT ""Code"" FROM ""@{0}"" WHERE ""U_Addon"" = '{1}' AND ""U_User"" = '{2}' AND ""U_BigKey"" = '{3}'",
                                          _tableName, _addonName, _userName, key));

                if (recordSet.RecordCount > 0)
                    return true;

                return false;
            }
        }

        private string GetSettingValue(string key)
        {
            var cacheKey = String.Format("{0}||||{1}||||{2}||||{3}", _tableName, _addonName, _userName, key);
            if (_cache.Contains(cacheKey))
                return (string) _cache.Get(cacheKey);

            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                var recordSet = factory.Create<Recordset>(BoObjectTypes.BoRecordset);
                recordSet.DoQuery(_userName == null
                                      ? string.Format(
                                          @"SELECT ""U_BigValue"" FROM ""@{0}"" WHERE ""U_Addon"" = '{1}' AND RTRIM(COALESCE(""U_User"", '')) = '' AND ""U_BigKey"" = '{2}'", _tableName,
                                          _addonName, key)
                                      : string.Format(
                                          @"SELECT ""U_BigValue"" FROM ""@{0}"" WHERE ""U_Addon"" = '{1}' AND ""U_User"" = '{2}' AND ""U_BigKey"" = '{3}'",
                                          _tableName, _addonName, _userName, key));
                if (recordSet.RecordCount == 0)
                    return null;

                recordSet.MoveFirst();

                var parameterValue = (string) recordSet.Fields.Item("U_BigValue").Value;

                _cache.Set(cacheKey, parameterValue, new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromMinutes(5) });

                return parameterValue;
            }
        }

        private void SetSettingValue(string key, string val)
        {
            var cacheKey = String.Format("{0}||||{1}||||{2}||||{3}", _tableName, _addonName, _userName, key);
            _cache.Remove(cacheKey);

            var nextTableCode = SboUserDefinedDataManager.GetNextTableCode(_tableName);

            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                var recordSet = factory.Create<Recordset>(BoObjectTypes.BoRecordset);

                try
                {
                    if (HasSetting(key))
                    {
                        Log.Debug("Updating setting key " + key);
                        recordSet.DoQuery(_userName == null
                                              ? string.Format(
                                                  @"UPDATE ""@{0}"" SET ""U_BigValue"" = '{3}' WHERE ""U_Addon"" = '{1}' AND RTRIM(COALESCE(""U_User"", '')) = '' AND ""U_BigKey"" = '{2}'",
                                                  _tableName, _addonName, key,
                                                  val
                                                    )
                                              : string.Format(
                                                  @"UPDATE ""@{0}"" SET ""U_BigValue"" = '{4}' WHERE ""U_Addon"" = '{1}' AND ""U_User"" = '{2}' AND ""U_BigKey"" = '{3}'",
                                                  _tableName, _addonName, _userName, key,
                                                  val
                                                    )
                            );
                    }
                    else
                    {
                        Log.Debug("Inserting setting key " + key);
                        recordSet.DoQuery(_userName == null
                                              ? string.Format(
                                                  @"INSERT INTO ""@{0}"" (""Code"", ""Name"", ""U_Addon"", ""U_BigKey"", ""U_BigValue"") VALUES ('{1}', '{1}', '{2}', '{3}', '{4}')",
                                                  _tableName,
                                                  nextTableCode,
                                                  _addonName, key, val
                                                    )
                                              : string.Format(
                                                  @"INSERT INTO ""@{0}"" (""Code"", ""Name"", ""U_Addon"", ""U_User"", ""U_BigKey"", ""U_BigValue"") VALUES ('{1}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                                                  _tableName,
                                                  nextTableCode,
                                                  _addonName, _userName, key, val
                                                    )
                            );
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "Recordset error");
                }
            }
        }


        private void InitSetting(string key, string initialValue)
        {
            if (!HasSetting(key))
                SetSettingValue(key, initialValue);
        }

        private void Initialize()
        {
            SboUserDefinedDataManager.AddUserTableIfNotExist(_tableName, "Global Settings");
            SboUserDefinedDataManager.AddUserFieldIfNotExist("@" + _tableName, "Addon", "Addon", BoFieldTypes.db_Alpha,
                BoFldSubTypes.st_None, 100);
            SboUserDefinedDataManager.AddUserFieldIfNotExist("@" + _tableName, "User", "User", BoFieldTypes.db_Alpha,
                BoFldSubTypes.st_None, 50);
            SboUserDefinedDataManager.AddUserFieldIfNotExist("@" + _tableName, "BigKey", "Setting key",
                BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, 254);
            SboUserDefinedDataManager.AddUserFieldIfNotExist("@" + _tableName, "BigValue", "Setting value",
                BoFieldTypes.db_Alpha, BoFldSubTypes.st_None, 254);
        }
    }
}