using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Josefina.Entities
{
    public class TicketItem
    {
        public int TicketItemID { get; set; }

        public string Code { get; set; }

        [ForeignKey("TicketCategoryOrder")]
        public int TicketCategoryOrderID { get; set; }

        public virtual TicketCategoryOrder TicketCategoryOrder { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string QRCode { get; set; }

        public bool Checked { get; set; }
    }
}