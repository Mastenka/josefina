using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Josefina.Entities
{
  /// <summary>
  /// Main organization entity
  /// </summary>
  public class Org
  {
    public int OrgID { get; set; }
    public string Name { get; set; }
    public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }

    [Range(1, 9999999999)]
    public long VariableSymbolCounter { get; set; }

    public DateTime? TransactionsLastUpdate { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }
  }
}