using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Josefina.Entities
{
    public class FioBankProxy
    {
        public int FioBankProxyID { get; set; }

        public long? AccountNumber { get; set; }

        public string IBAN { get; set; }

        public string BIC { get; set; }

        public string Token { get; set; }

        public DateTime? LastUpdate { get; set; }

        public DateTime? LastTransactionLoad { get; set; }
    }

    public static class FioBankHelper
    {
        public const int BANK_CODE = 2010;

        public const string TRANSACTIONS_RESET_PREFIX = "https://www.fio.cz/ib_api/rest/set-last-date/";

        public const string TRANSACTIONS_LAST_PREFIX = "https://www.fio.cz/ib_api/rest/last/";

        public const string TRANSACTIONS_LAST_POSTFIX = "/transactions.xml";

        public const string TRANSACTIONS_DATE_PREFIX = "https://www.fio.cz/ib_api/rest/periods/";

        public const string TRANSACTIONS_DATE_POSTFIX = "transactions.xml";

        public static string GetDatePostfix(DateTime from, DateTime to)
        {
            string fromStr = String.Format("{0:yyyy-MM-dd}", from);
            string toStr = String.Format("{0:yyyy-MM-dd}", to);

            // /2016-03-01/2016-03-26/transactions.xml
            return string.Format("/{0}/{1}/transactions.xml", fromStr, toStr);
        }
    }
}