using System;
using System.Data;
using System.Globalization;
using System.Linq;
using SAPbobsCOM;
using System.Collections.Generic;
using System.IO;

namespace SapBusinessOneExtensions
{
    public class SboDiUtils
    {
        public class ObjectTypeDefinition
        {
            public BoObjectTypes ObjectType { get; set; }
            public string FriendlyName { get; set; }
            public string FormType { get; set; }
            public string DataBaseTable { get; set; }
            public string DocEntryColumn { get; set; }
            public string DocNumColumn { get; set; }
            public string SeriesColumn { get; set; }
            public string BaseReportCode { get; set; }

            public ObjectTypeDefinition(BoObjectTypes type, string friendlyName, string formType, string table, string docEntryCol = "DocEntry", string docNumCol = "DocNum", string seriesCol = "Series", string baseReportCode = null)
            {
                ObjectType = type;
                FriendlyName = friendlyName;
                FormType = formType;
                DataBaseTable = table;
                DocEntryColumn = docEntryCol;
                DocNumColumn = docNumCol;
                SeriesColumn = seriesCol;
                BaseReportCode = baseReportCode;
            }
        }
        public static List<ObjectTypeDefinition> ObjectTypeDefinitions = new List<ObjectTypeDefinition>()
        {
            new ObjectTypeDefinition(BoObjectTypes.oPurchaseQuotations, "Purchase Quotation", "540000988", "OPQT", baseReportCode: "PQT"),
            new ObjectTypeDefinition(BoObjectTypes.oPurchaseOrders, "Purchase Order", "142", "OPOR", baseReportCode: "POR"),
            new ObjectTypeDefinition(BoObjectTypes.oPurchaseDeliveryNotes, "Goods Receipt PO", "143", "OPDN", baseReportCode: "PDN"),
            new ObjectTypeDefinition(BoObjectTypes.oPurchaseReturns, "Goods Return", "182", "ORPD", baseReportCode: "RPD"),
            new ObjectTypeDefinition(BoObjectTypes.oPurchaseInvoices, "A/P Invoice", "141", "OPCH", baseReportCode: "PCH"),
            new ObjectTypeDefinition(BoObjectTypes.oPurchaseCreditNotes, "A/P Credit Memo", "181", "ORPC", baseReportCode: "RPC"),
            new ObjectTypeDefinition(BoObjectTypes.oJournalEntries, "Journal Entry", "392", "OJDT", "TransId", "Number", baseReportCode: "JDT"),
            new ObjectTypeDefinition(BoObjectTypes.oCreditNotes, "A/R Credit Memo", "179", "ORIN", baseReportCode: "RIN"),
            new ObjectTypeDefinition(BoObjectTypes.oInvoices, "A/R Invoice", "133", "OINV", baseReportCode: "INV"),
            new ObjectTypeDefinition(BoObjectTypes.oDownPayments, "A/R Down Payment Invoice", "65300", "ODPI", baseReportCode: "DPI"),
            new ObjectTypeDefinition(BoObjectTypes.oReturns, "Return", "180", "ORDN", baseReportCode: "RDN"),
            new ObjectTypeDefinition(BoObjectTypes.oDeliveryNotes, "Delivery", "140", "ODLN", baseReportCode: "DLN"),
            new ObjectTypeDefinition(BoObjectTypes.oOrders, "Sales Order", "139", "ORDR", baseReportCode: "RDR"),
            new ObjectTypeDefinition(BoObjectTypes.oQuotations, "Sales Quotation", "149", "OQUT", baseReportCode: "QUT"),
            new ObjectTypeDefinition(BoObjectTypes.oInventoryGenExit, "Goods Issue", "149", "OIGE", baseReportCode: "IGE"),
            new ObjectTypeDefinition(BoObjectTypes.oDrafts, "Draft", "112", "ODRF")
        };

        public static Dictionary<string, string> ObjectTypeDescriptions =
            ObjectTypeDefinitions.ToDictionary(d => ((int) d.ObjectType).ToString(CultureInfo.InvariantCulture), d => d.FriendlyName);

        public static int? GetDocEntryFromDocNum(BoObjectTypes type, int docNum, int? seriesCode = null)
        {
            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                var recordSet = factory.Create<Recordset>(BoObjectTypes.BoRecordset);
                if (!seriesCode.HasValue)
                    seriesCode = GetDefaultSeries(type).Series;

                var objectTypeDefinition = ObjectTypeDefinitions.FirstOrDefault((def) => { return def.ObjectType.Equals(type); });
                if (objectTypeDefinition != null)
                    return recordSet.DoQueryValue<int?>(string.Format("SELECT {0} FROM {1} WHERE {2} = '{3}' AND Series = '{4}'",
                                                                    objectTypeDefinition.DocEntryColumn,
                                                                    objectTypeDefinition.DataBaseTable,
                                                                    objectTypeDefinition.DocNumColumn,
                                                                    docNum,
                                                                    seriesCode));
                else
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        public static System.Data.DataTable getDataTable(string sql, string CallerRef)
        {
            System.Data.DataTable dtOut = new System.Data.DataTable();

            using (var factory = new SboDisposableBusinessObjectFactory())
            {
               SAPbobsCOM.Recordset rs = (SAPbobsCOM.Recordset)factory.Create<Recordset>(BoObjectTypes.BoRecordset);

                try
                {
                    rs.DoQuery(sql);
                    if (!rs.EoF)
                    {
                        for (int i = 0; i < rs.Fields.Count; i++)
                        {
                            dtOut.Columns.Add(rs.Fields.Item(i).Description);
                        }
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
                    dtOut = new DataTable();
                    dtOut.Columns.Add("Error");
                    dtOut.Rows.Add(ex.Message);
                 }
                finally
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
                    rs = null;

                }
            }
            return dtOut;
        }


        public static int? GetDocNumFromDocEntry(BoObjectTypes type, int docEntry)
        {
            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                var recordSet = factory.Create<Recordset>(BoObjectTypes.BoRecordset);

                var objectTypeDefinition = ObjectTypeDefinitions.FirstOrDefault(def => def.ObjectType.Equals(type));
                if (objectTypeDefinition != null)
                    return recordSet.DoQueryValue<int?>(
                        $@"SELECT ""{objectTypeDefinition.DocNumColumn}"" FROM {objectTypeDefinition.DataBaseTable} WHERE ""{objectTypeDefinition.DocEntryColumn}"" = '{docEntry}'");
                else
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        public static Series GetDefaultSeries(BoObjectTypes type)
        {
            var companyService = SboAddon.Instance.Company.GetCompanyService();
            var seriesService = (SeriesService)companyService.GetBusinessService(ServiceTypes.SeriesService);

            var documentTypeParams = (DocumentTypeParams)seriesService.GetDataInterface(SeriesServiceDataInterfaces.ssdiDocumentTypeParams);
            documentTypeParams.Document = Convert.ToInt32(type).ToString(CultureInfo.InvariantCulture);

            var series = (Series)seriesService.GetDataInterface(SeriesServiceDataInterfaces.ssdiSeries);
            series = seriesService.GetDefaultSeries(documentTypeParams);

            return series;
        }

        public static T QueryValue<T>(string query, params object[] args)
        {
            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                var ast = System.Threading.Thread.CurrentThread.GetApartmentState();
                var recordset = factory.Create<Recordset>(BoObjectTypes.BoRecordset);
                return recordset.DoQueryValue<T>(query, args);
            }
        }

        public static Dictionary<T1, T2> QueryDictionary<T1, T2>(string query, params object[] args)
        {
            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                return factory.Create<Recordset>(BoObjectTypes.BoRecordset).DoQueryDictionary<T1, T2>(query, args);
            }
        }

        public static List<Dictionary<string, object>> QueryList(string query, params object[] args)
        {
            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                return factory.Create<Recordset>(BoObjectTypes.BoRecordset).DoQueryList(query, args);
            }
        }

        public static T GetRecord<T>(string code) where T : SboUserDefinedTableDefinition, new()
        {
            using (var f = new SboDisposableBusinessObjectFactory())
                return f.Create<Recordset>(BoObjectTypes.BoRecordset).GetRecord<T>(code);
        }

        public static T GetRecordByQuery<T>(string query, params object[] parameters) where T : SboUserDefinedTableDefinition, new()
        {
            using (var f = new SboDisposableBusinessObjectFactory())
            {
                var rs = f.Create<Recordset>(BoObjectTypes.BoRecordset);
                rs.DoQuery(String.Format(query, parameters));
                return rs.RecordCount > 0 ? rs.GetRecord<T>((string) rs.Fields.Item("Code").Value) : null;
            }
        }

        public static List<T> GetRecordsByQuery<T>(string query, params object[] parameters) where T : SboUserDefinedTableDefinition, new()
        {
            using (var f = new SboDisposableBusinessObjectFactory())
            {
                var rs = f.Create<Recordset>(BoObjectTypes.BoRecordset);
                rs.DoQuery(String.Format(query, parameters));
                var list = new List<T>();

                if (rs.RecordCount > 0)
                {
                    rs.MoveFirst();
                    while (!rs.EoF)
                    {
                        list.Add(GetRecord<T>((string) rs.Fields.Item("Code").Value));
                        rs.MoveNext();
                    }
                }

                return list;
            }
        }

        public static void InsertRecord(SboUserDefinedTableDefinition record)
        {
            using (var f = new SboDisposableBusinessObjectFactory())
                f.Create<Recordset>(BoObjectTypes.BoRecordset).Insert(record);
        }

        public static void UpdateRecord(SboUserDefinedTableDefinition record)
        {
            using (var f = new SboDisposableBusinessObjectFactory())
                f.Create<Recordset>(BoObjectTypes.BoRecordset).Update(record);
        }

        public static void ReplaceRecord(SboUserDefinedTableDefinition record)
        {
            using (var f = new SboDisposableBusinessObjectFactory())
                f.Create<Recordset>(BoObjectTypes.BoRecordset).Replace(record);
        }
        private static string _getLocalCurrencyCache;
        public static string GetLocalCurrency()
        {
            if (_getLocalCurrencyCache != null)
                return _getLocalCurrencyCache;

            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                CompanyService companyService = factory.GetCompanyService();
                AdminInfo adminInfo = companyService.GetAdminInfo();

                _getLocalCurrencyCache = adminInfo.LocalCurrency.ToUpperInvariant();
                return _getLocalCurrencyCache;
            }
        }

        public static string GetLastErrorDescription()
        {
            return String.Format("{0} - {1}",
                SboAddon.Instance.Company.GetLastErrorCode(),
                SboAddon.Instance.Company.GetLastErrorDescription());
        }

        public static void AddDocumentAttachment(BoObjectTypes type, int docEntry, params string[] filenames)
        {
            if (docEntry <= 0) return;

            using (var factory = new SboDisposableBusinessObjectFactory())
            {
                var sapDocument = factory.Create<Documents>(type);
                sapDocument.GetByKey(docEntry);

                var attachment = factory.Create<Attachments2>(BoObjectTypes.oAttachments2);
                if (sapDocument.AttachmentEntry > 0)
                {
                    attachment.GetByKey(sapDocument.AttachmentEntry);
                    attachment.Lines.Add();
                }
                foreach (var file in filenames)
                {
                    attachment.Lines.SourcePath = Path.GetDirectoryName(file);
                    attachment.Lines.FileName = Path.GetFileNameWithoutExtension(file);
                    attachment.Lines.FileExtension = Path.GetExtension(file).TrimStart('.');
                    attachment.Lines.Add();
                }
                if (sapDocument.AttachmentEntry > 0)
                {
                    if (attachment.Update() != 0)
                        throw new Exception(SboAddon.Instance.Company.GetLastErrorDescription());
                }
                else
                {
                    if (attachment.Add() != 0)
                        throw new Exception(SboAddon.Instance.Company.GetLastErrorDescription());

                    var attachmentId = SboAddon.Instance.Company.GetNewObjectKey();

                    sapDocument.AttachmentEntry = Convert.ToInt32(attachmentId);
                    if (sapDocument.Update() != 0)
                        throw new Exception(SboAddon.Instance.Company.GetLastErrorDescription());
                }
            }
        }
    }
}