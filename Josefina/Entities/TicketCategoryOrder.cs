using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Josefina.Entities
{
  /// <summary>
  /// Represents amount of ticket from category for order
  /// </summary>
  public class TicketCategoryOrder
  {
    public int TicketCategoryOrderID { get; set; }

    [ForeignKey("TicketCategory")]
    public int TicketCategoryID { get; set; }

    public virtual TicketCategory TicketCategory { get; set; }

    [ForeignKey("TicketOrder")]
    public int TicketOrderID { get; set; }

    public virtual TicketOrder TicketOrder { get; set; }

    public virtual ICollection<TicketItem> TicketItems { get; set; }

    public int Count { get; set; }

    public bool Paid { get; set; }

    public bool Canceled { get; set; }
  }
}