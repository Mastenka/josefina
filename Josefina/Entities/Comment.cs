using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Josefina.Entities
{
  [DebuggerDisplay("{ParentUser.UserName} - {Created}")]
  public class Comment
  {
    public int CommentID { get; set; }

    public string Content { get; set; }

    public virtual ApplicationUser ParentUser { get; set; }

    [ForeignKey("ParentTask")]
    public int ParentTaskID { get; set; }

    public virtual Task ParentTask { get; set; }

    public DateTime? Created { get; set; }
  }
}