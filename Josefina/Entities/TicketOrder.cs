using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Josefina.Entities
{
    /// <summary>
    /// Represents actual Ticket for one person
    /// </summary>
    public class TicketOrder
    {
        public int TicketOrderID { get; set; }

        public virtual ICollection<TicketCategoryOrder> TicketCategoryOrders { get; set; }

        public long VariableSymbol { get; set; }

        public DateTime? Created { get; set; }

        public bool Paid { get; set; }

        public DateTime? PaidDate { get; set; }

        public DateTime? ReservedUntil { get; set; }

        public decimal TotalPaidPrice { get; set; }

        public string Email { get; set; }

        public bool Canceled { get; set; }

        public decimal TotalPrice { get; set; }

        public int OrgID { get; set; }

        public int ProjectID { get; set; }

        public bool IsEnglish { get; set; }

        public virtual ICollection<FioTransaction> FioTransactions { get; set; }

        public bool? TermsConditionsAccepted { get; set; }
    }
}