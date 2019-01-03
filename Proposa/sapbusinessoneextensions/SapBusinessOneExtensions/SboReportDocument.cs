using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using NLog;
using SAPbobsCOM;

namespace SapBusinessOneExtensions
{
    public sealed class SboReportDocument : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private ReportDocument ReportDocument { get; set; }

        private SboReportDocument() { }

        public static SboReportDocument FromFile(string fileName)
        {
            Logger.Trace("Getting Crystal Report document from file {0}", fileName);

            SboReportDocument instance = new SboReportDocument();

            instance.ReportDocument = new ReportDocument();
            instance.ReportDocument.Load(fileName);
            instance.ReportDocument.Refresh();
            instance.SetDefaultParameterValues();

            List<ReportParameter> prms = instance.GetParameters();
            return instance;
        }

        public static SboReportDocument FromLayoutCode(string layoutCode)
        {
            Logger.Trace("Getting Crystal Report document from layoutCode {0}", layoutCode);

            string tempFilename = null;
            var cacheKey = $"SboReportDocument.FromLayoutCode({layoutCode})";
            if (SboTemporaryCache.Contains(cacheKey))
                tempFilename = SboTemporaryCache.Get<string>(cacheKey);

            if (tempFilename == null || !File.Exists(tempFilename))
                using (var factory = new SboDisposableBusinessObjectFactory())
                {
                    var companyService = factory.GetCompanyService();
                    var blobParams = (BlobParams) companyService.GetDataInterface(CompanyServiceDataInterfaces.csdiBlobParams);
                    blobParams.Table = "RDOC";
                    blobParams.Field = "Template";

                    BlobTableKeySegment keySegment = blobParams.BlobTableKeySegments.Add();
                    keySegment.Name = "DocCode";
                    keySegment.Value = layoutCode;

                    var blob = (Blob) companyService.GetDataInterface(CompanyServiceDataInterfaces.csdiBlob);
                    blob = companyService.GetBlob(blobParams);

                    tempFilename = Path.GetTempFileName();
                    File.WriteAllBytes(tempFilename, Convert.FromBase64String(blob.Content));

                    SboTemporaryCache.Set(cacheKey, tempFilename);
                }

            return FromFile(tempFilename);
        }

        public static SboReportDocument FromDefaultReportFor(string reportCode, string cardCode, int? userSign = null)
        {
            Logger.Trace("Getting Crystal Report document from default for {0} - {1} - {2}", reportCode, cardCode, userSign);

            string layoutCode = null;

            var cacheKey = $"SboReportDocument.FromDefaultReportFor({reportCode},{cardCode},{userSign})";
            if (SboTemporaryCache.Contains(cacheKey))
                layoutCode = SboTemporaryCache.Get<string>(cacheKey);

            if (layoutCode == null)
                using (var factory = new SboDisposableBusinessObjectFactory())
                {
                    var companyService = factory.GetCompanyService();
                    var reportLayoutService = (ReportLayoutsService) companyService.GetBusinessService(ServiceTypes.ReportLayoutsService);

                    var reportParam =
                        (ReportParams) reportLayoutService.GetDataInterface(ReportLayoutsServiceDataInterfaces.rlsdiReportParams);
                    reportParam.UserID = userSign ?? SboAddon.Instance.Company.UserSignature;
                    reportParam.ReportCode = reportCode;
                    reportParam.CardCode = cardCode;

                    var reportLayout = reportLayoutService.GetDefaultReport(reportParam);
                    layoutCode = reportLayout.LayoutCode;

                    SboTemporaryCache.Set(cacheKey, layoutCode);

                   
                }

            return FromLayoutCode(layoutCode);
        }

        public static SboReportDocument FromDefaultReportFor(BoObjectTypes objectType, BoDocumentTypes documentType, string cardCode,
            int? userSign = null)
        {
            return
                FromDefaultReportFor(
                    SboDiUtils.ObjectTypeDefinitions.Single(def => def.ObjectType.Equals(objectType)).BaseReportCode +
                    (documentType.Equals(BoDocumentTypes.dDocument_Service) ? "1" : "2"), cardCode, userSign);
        }

        public List<ReportParameter> GetParameters()
        {
            var parameters = new List<ReportParameter>();
            foreach (ParameterFieldDefinition parameterField in ReportDocument.DataDefinition.ParameterFields)
            {
                parameters.Add(new ReportParameter()
                {
                    Name = parameterField.Name,
                    IsLinked = parameterField.IsLinked(),
                    IsOptional = parameterField.IsOptionalPrompt,
                    ParameterType = parameterField.ParameterType.ToString()
                });
            }

            return parameters;
        }

        public class ReportParameter
        {
            public string Name { get; set; }
            public bool IsLinked { get; set; }
            public bool IsOptional { get; set; }
            public string ParameterType { get; set; }
            public object Value { get; set; }
        }

        public byte[] AsPortableDocumentBytes()
        {
            var buffer = new MemoryStream();

            var sw = Stopwatch.StartNew();
         
                using (var stream = ReportDocument.ExportToStream(ExportFormatType.PortableDocFormat))
                    stream.CopyTo(buffer);
                sw.Stop();
          
            SboAddonTracker.TrackEvent("RenderReport", new Dictionary<string, string> {["ReportName"] = ReportDocument.Name}, new Dictionary<string, double> { ["Duration"] = sw.ElapsedMilliseconds });

            return buffer.ToArray();
        }

        public string AsPortableDocumentBase64()
        {
            return Convert.ToBase64String(AsPortableDocumentBytes());
        }

        public void Print()
        {
            ReportDocument.PrintToPrinter(new PrinterSettings(), new PageSettings(), false);
        }

        public void SetParameter(string name, object value)
        {
           SetParameters(new Dictionary<string, object>() { {name, value} });
        }

        public void SetParameter2(string name, object value)
        {
            foreach (ParameterFieldDefinition parameterField in ReportDocument.DataDefinition.ParameterFields)
            {
                if (parameterField.Name == name)
                {
                    if (!parameterField.IsLinked())
                    {
                        if (parameterField.ReportName != "")
                        {
                            ReportDocument.SetParameterValue(name, value, parameterField.ReportName);
                        }
                        else

                        {
                            ReportDocument.SetParameterValue(name, value);
                        }
                    }
                }
            }

        }
        public void SetParameters(Dictionary<string, object> parameters)
        {
            foreach (KeyValuePair<string, object> pair in parameters)
            {
                if (pair.Value == null)
                {
                    ReportDocument.SetParameterValue(pair.Key, pair.Value);
                    continue;
                }
                if (pair.Value.ToString() != string.Empty && ReportDocument.ParameterFields.Cast<ParameterField>().Any(f => pair.Key.Equals(f.ParameterFieldName)))
                {
                    ParameterField field = ReportDocument.ParameterFields[pair.Key];
                    switch (field.ParameterValueType)
                    {
                        case ParameterValueKind.DateParameter:
                        case ParameterValueKind.DateTimeParameter:
                            {
                                DateTime time;
                                ReportDocument.SetParameterValue(pair.Key, DateTime.TryParseExact(pair.Value.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out time) ? time : pair.Value);
                                continue;
                            }
                    }
                   ReportDocument.SetParameterValue(pair.Key, pair.Value);
                   // ReportDocument.ParameterFields[pair.Key].CurrentValues.AddValue(pair.Value);
                }
            }
        }

        private void SetDefaultParameterValues()
        {
            if (ReportDocument.ParameterFields.Cast<ParameterField>().Any(f => "ExtParam@".Equals(f.ParameterFieldName)))
                ReportDocument.SetParameterValue("ExtParam@", String.Empty);


            ParameterFieldDefinitions parmFields = ReportDocument.DataDefinition.ParameterFields;
            foreach (ParameterFieldDefinition def in parmFields)
            {
                if (!def.IsLinked())
                {
                    switch (def.ValueType)
                    {
                        case FieldValueType.StringField:
                            Logger.Trace("Setting default parameter value for parameter field {0} to {1}", def.Name, String.Empty);
                            ReportDocument.SetParameterValue(def.Name, String.Empty);

                            break;

                        case FieldValueType.NumberField:
                            ReportDocument.SetParameterValue(def.Name, 0);
                            break;

                        default:
                            ReportDocument.SetParameterValue(def.Name, null);
                            break;
                    }
                }
            }

        }

        private void SetDefaultParameterValues(ReportDocument rpt )
        {
           

            ParameterFieldDefinitions parmFields = rpt.DataDefinition.ParameterFields;
            foreach (ParameterFieldDefinition def in parmFields)
            {
                
                    switch (def.ValueType)
                    {
                        case FieldValueType.StringField:
                            Logger.Trace("Setting default parameter value for parameter field {0} to {1}", def.Name, String.Empty);
                            rpt.SetParameterValue(def.Name, String.Empty);

                            break;

                        case FieldValueType.NumberField:
                            rpt.SetParameterValue(def.Name, 0);
                            break;

                        default:
                            rpt.SetParameterValue(def.Name, null);
                            break;
                    }
                
            }

        }
        public void SetDatasources(string server, ServerType serverType, string database, bool useIntegratedSecurity, string username, string password)
        {

            if (ServerType.Hana.Equals(serverType))
            {
                if (!useIntegratedSecurity && !String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                {
                    ReportDocument.SetDatabaseLogon(username, password, server, database);
                }
                SetDatasource(ReportDocument, server, serverType, database, useIntegratedSecurity, username, password);

                foreach (ReportDocument document in ReportDocument.Subreports.Cast<ReportDocument>())
                {
                    SetDatasource(document, server, serverType, database, useIntegratedSecurity, username, password);
                }

            }
            else
            {
                ReportDocument.SetDatabaseLogon(username, password, server, database);
                for (int r= 0; r< ReportDocument.DataSourceConnections.Count;r++)
                {
                    ReportDocument.DataSourceConnections[r].SetConnection(server, database, username, password);
                }

                Tables CrTables;

               
           
                CrTables = ReportDocument.Database.Tables;
                foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
                {
                    TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                    TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                    ConnectionInfo crConnectionInfo = new ConnectionInfo();
                    crConnectionInfo.ServerName = server;
                    crConnectionInfo.DatabaseName = database;
                    crConnectionInfo.UserID = username;
                    crConnectionInfo.Password = password;
                    crConnectionInfo.Type = ConnectionInfoType.SQL;
                    crConnectionInfo.IntegratedSecurity = useIntegratedSecurity;

                    crtableLogoninfo = CrTable.LogOnInfo;
                    crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                    CrTable.ApplyLogOnInfo(crtableLogoninfo);


                }
               // ReportDocument.VerifyDatabase();

                foreach (ReportDocument rpt in ReportDocument.Subreports)
                {
                    rpt.SetDatabaseLogon(username, password, server, database);
                   
                   
                    Tables subCrTables = rpt.Database.Tables;
                    foreach (CrystalDecisions.CrystalReports.Engine.Table subCrTable in subCrTables)
                    {
                        TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                        TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                        ConnectionInfo crConnectionInfo = new ConnectionInfo();
                        crConnectionInfo.ServerName = server;
                        crConnectionInfo.DatabaseName = database;
                        crConnectionInfo.UserID = username;
                        crConnectionInfo.Password = password;
                        crConnectionInfo.Type = ConnectionInfoType.SQL;
                        crConnectionInfo.IntegratedSecurity = useIntegratedSecurity;

                        crtableLogoninfo = subCrTable.LogOnInfo;
                        crtableLogoninfo.ConnectionInfo =crConnectionInfo;

                        subCrTable.ApplyLogOnInfo(crtableLogoninfo);
                    }
                    for (int r = 0; r < rpt.DataSourceConnections.Count; r++)
                    {
                        rpt.DataSourceConnections[r].SetConnection(server, database, username, password);
                    }
                    rpt.VerifyDatabase();
                }

                
            }
            ReportDocument.VerifyDatabase();
        }

        private void SetDatasource(ReportDocument document, string server, ServerType serverType, string database, bool useIntegratedSecurity, string username, string password)
        {

          

          
          


            if (ServerType.Hana.Equals(serverType))
            {
                foreach (IConnectionInfo info in document.DataSourceConnections)
                {
                    info.Attributes.Collection = GetConnectionAttributes(info);
                }
                foreach (Table table in document.Database.Tables)
                {
                    TableLogOnInfo logOnInfo = table.LogOnInfo;
                    foreach (NameValuePair2 logonProperty in logOnInfo.ConnectionInfo.LogonProperties)
                    {
                        if (logonProperty.Name.ToString() == "Connection String")
                            logonProperty.Value =
                                $@"DRIVER={{B1CRHPROXY{(IntPtr.Size == 4 ? "32" : "")}}};UID={username};PWD={password};SERVERNODE={server};DATABASE=""{database}"";CS=""{database}""";
                    }
                    table.ApplyLogOnInfo(logOnInfo);
                }
            }
            else
            {
                 document.SetDatabaseLogon(username, password, server, database);
               

            }


        }

        private NameValuePairs2 GetConnectionAttributes(IConnectionInfo connection)
        {
            string str = string.Empty;
            string str2 = string.Empty;
            string str3 = string.Empty;
            string str4 = string.Empty;
            string str5 = string.Empty;
            string str6 = string.Empty;
            string str7 = string.Empty;
            foreach (NameValuePair2 pair in connection.Attributes.Collection)
            {
                if (pair.Name.ToString() == "QE_LogonProperties")
                {
                    foreach (NameValuePair2 pair2 in ((DbConnectionAttributes)pair.Value).Collection)
                    {
                        switch (pair2.Name.ToString())
                        {
                            case "Auto Translate":
                                str = pair2.Value.ToString();
                                break;

                            case "Connect Timeout":
                                str2 = pair2.Value.ToString();
                                break;

                            case "General Timeout":
                                str3 = pair2.Value.ToString();
                                break;

                            case "Locale Identifier":
                                str4 = pair2.Value.ToString();
                                break;

                            case "Tag with column collation when possible":
                                str5 = pair2.Value.ToString();
                                break;

                            case "Use DSN Default Properties":
                                str6 = pair2.Value.ToString();
                                break;

                            case "Use Encryption for Data":
                                str7 = pair2.Value.ToString();
                                break;
                        }
                    }
                }
            }
            NameValuePairs2 pairs = new NameValuePairs2();
            pairs.Add(new NameValuePair2("Database DLL", "crdb_ado.dll"));
            pairs.Add(new NameValuePair2("QE_DatabaseName", string.Empty));
            pairs.Add(new NameValuePair2("QE_DatabaseType", "OLE DB (ADO)"));
            DbConnectionAttributes oValue = new DbConnectionAttributes();
            NameValuePairs2 collection = oValue.Collection;
            collection.Add(!string.IsNullOrEmpty(str) ? new NameValuePair2("Auto Translate", str) : new NameValuePair2("Auto Translate", "-1"));
            collection.Add(!string.IsNullOrEmpty(str2) ? new NameValuePair2("Connect Timeout", str2) : new NameValuePair2("Connect Timeout", "15"));
            collection.Add(new NameValuePair2("Data Source", string.Empty));
            collection.Add(!string.IsNullOrEmpty(str3) ? new NameValuePair2("General Timeout", str3) : new NameValuePair2("General Timeout", "0"));
            collection.Add(new NameValuePair2("Initial Catalog", string.Empty));
            collection.Add(new NameValuePair2("Integrated Security", "FALSE"));
            collection.Add(!string.IsNullOrEmpty(str4) ? new NameValuePair2("Locale Identifier", str4) : new NameValuePair2("Locale Identifier", "1030"));
            collection.Add(new NameValuePair2("OLE DB Services", "-5"));
            collection.Add(new NameValuePair2("Provider", "SQLOLEDB"));
            collection.Add(!string.IsNullOrEmpty(str5) ? new NameValuePair2("Tag with column collation when possible", str5) : new NameValuePair2("Tag with column collation when possible", "0"));
            collection.Add(!string.IsNullOrEmpty(str6) ? new NameValuePair2("Use DSN Default Properties", str6) : new NameValuePair2("Use DSN Default Properties", "false"));
            collection.Add(new NameValuePair2("Use Encryption for Data", "0"));
            collection.Add(!string.IsNullOrEmpty(str7) ? new NameValuePair2("Use Encryption for Data", str7) : new NameValuePair2("Use Encryption for Data", "0"));
            pairs.Add(new NameValuePair2("QE_LogonProperties", oValue));
            pairs.Add(new NameValuePair2("QE_ServerDescription", "."));
            pairs.Add(new NameValuePair2("QE_SQLDB", "true"));
            pairs.Add(new NameValuePair2("SSO Enabled", "false"));
            return pairs;
        }


        public void Dispose()
        {
            if (ReportDocument != null)
            {
                ReportDocument.Close();
                ReportDocument.Dispose();
                ReportDocument = null;
            }
        }

        public enum ServerType
        {
            Mssql,
            Hana
        }
    }
}
