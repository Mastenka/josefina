using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Josefina.Entities
{
  /// <summary>
  /// Stores data about sended invitatons
  /// </summary>
  public class Invitation
  {
    public int InvitationID { get; set; }

    [ForeignKey("Org")]
    public int OrgID { get; set; }
    public virtual Org Org { get; set; }

    public int Code { get; set; }

    public string Email { get; set; }

    public bool Accepted { get; set; }
  }
}