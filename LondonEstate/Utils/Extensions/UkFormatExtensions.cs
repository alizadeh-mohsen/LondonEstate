using System.Globalization;

namespace LondonEstate.Utils.Extensions
{
    public static class UkFormatExtensions
    {
        public static string ToUkDateString(this DateTime date)
        {
            return date.ToString("dd'/'MM'/'yyyy");
        }
        public static string ToUkCurrencyString(this decimal amount)
        {
            return amount.ToString("C2", CultureInfo.GetCultureInfo("en-GB"));
        }
    }
}
