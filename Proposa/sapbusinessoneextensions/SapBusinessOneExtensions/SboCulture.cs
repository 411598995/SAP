using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbobsCOM;
using SAPbouiCOM;

namespace SapBusinessOneExtensions
{
    public static class SboCulture
    {
        public static int SumDecimals => SboDiUtils.QueryValue<int>(@"SELECT ""SumDec"" FROM OADM ORDER BY ""UpdateDate"" DESC");
        public static int PriceDecimals => SboDiUtils.QueryValue<int>(@"SELECT ""PriceDec"" FROM OADM ORDER BY ""UpdateDate"" DESC");
        public static int RateDecimals => SboDiUtils.QueryValue<int>(@"SELECT ""RateDec"" FROM OADM ORDER BY ""UpdateDate"" DESC");
        public static int QuantityDecimals => SboDiUtils.QueryValue<int>(@"SELECT ""QtyDec"" FROM OADM ORDER BY ""UpdateDate"" DESC");
        public static int PercentDecimals => SboDiUtils.QueryValue<int>(@"SELECT ""PercentDec"" FROM OADM ORDER BY ""UpdateDate"" DESC");
        public static string DecimalSeparator => SboDiUtils.QueryValue<string>(@"SELECT ""DecSep"" FROM OADM ORDER BY ""UpdateDate"" DESC");

        public static double AsSum(double value, MidpointRounding midpointRounding = MidpointRounding.AwayFromZero)
        {
            return Math.Round(value, SumDecimals, midpointRounding);
        }

        public static double AsPrice(double value, MidpointRounding midpointRounding = MidpointRounding.AwayFromZero)
        {
            return Math.Round(value, PriceDecimals, midpointRounding);
        }

        public static double AsRate(double value, MidpointRounding midpointRounding = MidpointRounding.AwayFromZero)
        {
            return Math.Round(value, RateDecimals, midpointRounding);
        }

        public static double AsQuantity(double value, MidpointRounding midpointRounding = MidpointRounding.AwayFromZero)
        {
            return Math.Round(value, QuantityDecimals, midpointRounding);
        }

        public static string AsUiSum(double value)
        {
            var val = AsSum(value);
            var format = (CultureInfo) CultureInfo.InvariantCulture.Clone();
            format.NumberFormat.NumberDecimalSeparator = DecimalSeparator;

            return value.ToString(format);
        }
    }
}
