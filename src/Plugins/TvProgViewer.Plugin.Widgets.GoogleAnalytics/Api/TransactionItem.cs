//Contributor: https://www.codeproject.com/Articles/493455/Server-side-Google-Analytics-Transactions

using System;
using System.Globalization;

namespace TvProgViewer.Plugin.Widgets.GoogleAnalytics.Api
{
    public class TransactionItem
    {
        private readonly string _utmt = "item";

        private string _utmtid;     //OrderId
        private string _utmipc;     //TvChannel code
        private string _utmipn;     //TvChannel name
        private string _utmipr;     //TvChannel price (unit price)
        private string _utmiqt;     //Quantity
        private string _utmiva;     //TvChannel category

        /// <summary>
        /// Create a new TransactionItem
        /// </summary>
        /// <param name="orderId">Order number</param>
        /// <param name="tvChannelName">TvChannel name</param>
        /// <param name="tvChannelPrice">Unit price</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="category">The tvChannel category</param>
        public TransactionItem(string orderId, string tvChannelCode, string tvChannelName, decimal tvChannelPrice, int quantity, string category)
        {
            var usCulture = new CultureInfo("en-US");

            _utmtid = Uri.EscapeDataString(orderId);
            _utmipc = Uri.EscapeDataString(tvChannelCode);
            _utmipn = Uri.EscapeDataString(tvChannelName);
            _utmipr = tvChannelPrice.ToString("0.00", usCulture);
            _utmiqt = quantity.ToString();
            _utmiva = Uri.EscapeDataString(category);
        }

        public string CreateParameterString()
        {
            return string.Format("utmt={0}&utmtid={1}&utmipc={2}&utmipn={3}&utmipr={4}&utmiqt={5}&utmiva={6}",
                                 _utmt,
                                 _utmtid,
                                 _utmipc,
                                 _utmipn,
                                 _utmipr,
                                 _utmiqt,
                                 _utmiva);
        }
    }
}
