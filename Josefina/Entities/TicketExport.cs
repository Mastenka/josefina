using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Josefina.Entities
{
    public class TicketExport
    {
        public int TicketExportID { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Capacity { get; set; }

        public long VariableSymbol { get; set; }

        [ForeignKey("Project")]
        public int ProjectID { get; set; }

        public virtual Project Project { get; set; }

        public virtual ICollection<TicketExportItem> TicketExportItems { get; set; }
    }
}