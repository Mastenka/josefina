using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Josefina.Entities
{
  public class TaskUpdate
  {
    public int TaskUpdateID { get; set; }

    public string UserID { get; set; }

    [ForeignKey("Task")]
    public int TaskID { get; set; }

    public Task Task { get; set; }

    public int Count { get; set; }

    public string ConnectionIDSignalR { get; set; } 
  }
}