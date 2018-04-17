using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Josefina.Entities
{
    public class FioTransaction
    {
        public long FioTransactionID { get; set; }

        public long TransactionID { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public string AccountNumber { get; set; }

        public int BankCode { get; set; }

        public long VariableSymbol { get; set; }

        public string Note { get; set; }

        [ForeignKey("TicketOrder")]
        public int TicketOrderID { get; set; }

        public virtual TicketOrder TicketOrder { get; set; }
    }
}