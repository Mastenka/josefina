using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Josefina.Entities
{
  public class Task
  {
    public int TaskID { get; set; }

    public string Name { get; set; }

    [ForeignKey("Parent")]
    public int ParentID { get; set; }

    public virtual Folder Parent { get; set; }

    public DateTime? Deadline { get; set; }

    public bool Completed { get; set; }

    public string Content { get; set; }

    public virtual ICollection<Comment> Comments { get; set; }

    [ForeignKey("EditingUser")]
    public string EditingUserID { get; set; }

    public ApplicationUser EditingUser { get; set; }

    [ForeignKey("Creator")]
    public string CreatorID { get; set; }

    public ApplicationUser Creator { get; set; }

    public DateTime? EditHeartBeat { get; set; }

    [ForeignKey("Org")]
    public int OrgID { get; set; }

    public Org Org { get; set; }

    //public virtual Project Project { get; set; } //TODO
  }

  public class TaskHelper
  {
    public static ETaskState GetTaskState(Task task)
    {
      if (task.Completed)
      {
        return ETaskState.Completed;
      }
      else if (task.Deadline.HasValue)
      {
        if (task.Deadline.Value.Date >= DateTime.Now.Date)
        {
          return ETaskState.InProgres;
        }
        else
        {
          return ETaskState.OverDue;
        }
      }
      else
      {
        return ETaskState.InProgres;
      }
    }
  }


  /// <summary>
  /// Determines state of task
  /// </summary>
  public enum ETaskState
  {
    /// <summary>
    /// Task have been marked as completed
    /// </summary>
    Completed,
    /// <summary>
    /// Task is completable, but heavent pass Deadline or havent been marked as completed
    /// </summary>
    InProgres,
    /// <summary>
    /// Task has passed deadline
    /// </summary>
    OverDue,
    /// <summary>
    /// Can't be completed
    /// </summary>
    Uncompletable
  }
}