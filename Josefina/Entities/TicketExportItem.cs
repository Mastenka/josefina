using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Josefina.Entities
{
    public class TicketExportItem
    {
        public int TicketExportItemID { get; set; }

        public string Code { get; set; }

        [ForeignKey("TicketExport")]
        public int TicketExportID { get; set; }

        public virtual TicketExport TicketExport { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public bool Paid { get; set; }

        public string QRCode { get; set; }

        public bool Checked { get; set; }

    }
}