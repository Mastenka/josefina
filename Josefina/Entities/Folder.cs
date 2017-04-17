using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Josefina.Entities
{
  public class Folder
  {
    public int FolderID { get; set; }

    public string Name { get; set; }

    [ForeignKey("Parent")]
    public int? ParentID { get; set; }

    public virtual Folder Parent { get; set; }

    public virtual ICollection<Folder> ChildFolders { get; set; }

    public virtual ICollection<Task> ChildTasks { get; set; }
  }
}