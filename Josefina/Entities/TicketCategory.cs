using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Josefina.Entities
{
    /// <summary>
    /// Category of tickets with limited amnout and period when it will be sold
    /// </summary>
    public class TicketCategory
    {
        public int TicketCategoryID { get; set; }

        [ForeignKey("Project")]
        public int ProjectID { get; set; }

        public virtual Project Project { get; set; }

        public int Capacity { get; set; }

        public int Ordered { get; set; }

        public decimal Price { get; set; }

        public DateTime? SoldFrom { get; set; }

        public DateTime? SoldTo { get; set; }

        public string HeaderEN { get; set; }

        public string HeaderCZ { get; set; }

        public virtual ICollection<TicketCategoryOrder> TicketCategoryOrders { get; set; }

        public bool Deleted { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public bool CodeRequired { get; set; }

        public string Code { get; set; }
    }
}