using System;
using System.Xml;

using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using SAPDI;
using System.Xml.Serialization;
using System.IO;

namespace Task_CurrencyUpdator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        public static SAPDI.DIClass sboDI;
        public static SAPDI.DataServices ds;

        [STAThread]
        static void Main()
        {


            string db = "AYC_Db01N";
            string server = "AYYEKA-SAP\\SAP";
            string serverType = "2012";
            string dbuser = "sa";
            string dbPwd = "SAPB1Admin";
            string sboId = "manager";
            string sboPwd = "3331";



            //db = "SBODemoAU";
            //server = "ubaid-pc";
            //serverType = "2012";
            //dbuser = "sa";
            //dbPwd = "super";
            //sboId = "manager";
            //sboPwd = "super";




            sboDI = new DIClass(db, sboId, sboPwd, dbuser, dbPwd, serverType, server);

            string conStat = sboDI.connectCompany();
            if (conStat != "OK")
            {
               // MessageBox.Show("Unable to connect to sbo");
            }
            string sourceConstr = "Data Source=" + server + ";Initial Catalog=" + db + ";Integrated Security=False;Persist Security Info=False;User ID=" + dbuser+ ";Password=" + dbPwd;

            ds = new DataServices(sourceConstr);

          //  MessageBox.Show("Connection Status : " + conStat);
            try
            {
                UpdateForDate(DateTime.Now.Date);
            }
            catch (Exception ex)
            {
             //   MessageBox.Show(ex.Message);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Exit();
            //Application.Run(new Form1());
        }

        public static void UpdateForDate(DateTime pDATE)
        {
            SAPbobsCOM.SBObob bobs = sboDI.oDiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge);

            string _mainCurrency =  ((string)bobs.GetLocalCurrency().Fields.Item(0).Value).ToUpperInvariant();

            SAPbobsCOM.Recordset recordset = sboDI.oDiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            System.Data.DataTable currencies = ds.getDataTable (
                sboDI.oDiCompany.DbServerType.Equals(SAPbobsCOM. BoDataServerTypes.dst_HANADB)
                    ? @"SELECT OCRN.* FROM OCRN LEFT JOIN ORTT ON OCRN.""CurrCode"" = ORTT.""Currency"" AND ""RateDate"" = '" + pDATE.ToString("yyyyMMdd") + @"' WHERE COALESCE(""Rate"", 0) = 0"
                    : @"SELECT OCRN.* FROM OCRN LEFT JOIN ORTT ON OCRN.""CurrCode"" = ORTT.""Currency"" AND ""RateDate"" =  '" + pDATE.ToString("yyyyMMdd") + @"' WHERE COALESCE(""Rate"", 0) = 0");

            var currencyCodes = new Dictionary<string, string>();
            foreach (System.Data.DataRow currency in currencies.Rows)
            {
                string code = currency["CurrCode"] as string;
                string intcode = currency["DocCurrCod"] as string;
                string isocode = currency["ISOCurrCod"] as string;

                string currencyCode = string.IsNullOrWhiteSpace(isocode)
                    ? intcode?.ToUpperInvariant()
                    : isocode.ToUpperInvariant();

                if (code != null && !string.IsNullOrWhiteSpace(currencyCode) && currencyCode.Length == 3)
                    currencyCodes.Add(code, currencyCode);
            }
            string mainCurrency = currencyCodes[_mainCurrency];
            string strDate = pDATE.ToString("yyyy-MM-dd");
            currencyCodes.Remove(_mainCurrency);

            if (!currencyCodes.Any())
                return;

            JObject document;
            CURRENCIES result;

            using (WebClient wc = new WebClient())
            {
                //string callString = "https://openexchangerates.org/api/historical/" + strDate + ".json?app_id=64f0b566047049ec93b765fce90530e4&base=" + mainCurrency;
                ////  MessageBox.Show(_mainCurrency + strDate + callString);



                //document =
                //                        JObject.Parse(
                //                            wc.DownloadString(
                //                                $"https://openexchangerates.org/api/historical/" + strDate + ".json?app_id=64f0b566047049ec93b765fce90530e4&base=" + mainCurrency + ""));



                string strXml =wc.DownloadString($"http://www.boi.org.il/currency.xml");


                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strXml);
                XmlSerializer serializer = new XmlSerializer(typeof(CURRENCIES));
                using (TextReader reader = new StringReader(doc.InnerXml))
                {
                    result = (CURRENCIES)serializer.Deserialize(reader);

                }

            }

            //var test = from rate in document.SelectToken("rates").Children()
            //           where rate is JProperty && currencyCodes.ContainsValue(((JProperty)rate).Name)
            //           select
            //           new
            //           {
            //               CurrencyCode = ((JProperty)rate).Name,
            //               Date = pDATE.Date,
            //               Rate = Convert.ToDouble(1M / ((JProperty)rate).Value.Value<decimal>())
            //           };

            var test = from rate in result.CURRENCY
                       where currencyCodes.ContainsValue(rate.CURRENCYCODE)
                       select
                       new
                       {
                           CurrencyCode = rate.CURRENCYCODE,
                           Date = pDATE.Date,
                           Rate = Convert.ToDouble(rate.RATE)
                       };


            int codesUpdated = 0;
            foreach (var rate in test)
            {
               // MessageBox.Show("Updating " + rate.CurrencyCode);

                var code = currencyCodes.FirstOrDefault(kp => kp.Value.Equals(rate.CurrencyCode)).Key;
              

                var rateExists = false;
                try
                {
                    if (bobs.GetCurrencyRate(code, rate.Date).RecordCount > 0)
                        rateExists = true;
                }
                catch (Exception ex)
                {
                    string err = ex.Message;
                }
                finally
                {
                    if (!rateExists)
                    {

                        try
                        {
                            bobs.SetCurrencyRate(code, rate.Date, rate.Rate, false);
                        }
                        catch (Exception ex)
                        {
                          //  MessageBox.Show("Updating " + code);
                        }
                        codesUpdated++;
                    }
                }
            }

        }

    }
}
