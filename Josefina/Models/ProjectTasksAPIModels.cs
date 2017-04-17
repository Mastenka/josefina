using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Josefina.Models;
using Josefina.Entities;

namespace Josefina.ApiModels.Tasks
{
  /// <summary>
  /// Defines basic atributes for Angular View model data classes
  /// </summary>
  public class AngularViewModel
  {
    public bool IsValid { get; set; }
    public bool IsAuthorized { get; set; }
    public string Title { get; set; }
  }

  /// <summary>
  /// View model for state: "tasks"
  /// </summary>
  public class TasksViewModel : AngularViewModel
  {
    /// <summary>
    /// TreeNodes to be shown
    /// </summary>
    public List<TreeViewNode> TreeViewNodes { get; set; }

    /// <summary>
    /// Help array with cloned TreeNodes
    /// </summary>
    public List<TreeViewNode> TreeViewClonedNodes { get; set; }
  }

  /// <summary>
  /// Send to client to inform about operation/action result
  /// </summary>
  public class ActionResult : AngularViewModel
  {
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
  }

  /// <summary>
  /// Main ViewModel containing partial ViewModels
  /// </summary>
  public class TaskViewModel : AngularViewModel
  {
    public TaskViewModel()
    {
      this.TaskLowerViewModel = new TaskLowerViewModel();
      this.TaskUpperViewModel = new TaskUpperViewModel();
      this.TaskPropertiesViewModel = new TaskPropertiesViewModel();
    }

    /// <summary>
    /// View model for ContentView
    /// </summary>
    public TaskUpperViewModel TaskUpperViewModel { get; set; }

    /// <summary>
    /// View model for CommentsView
    /// </summary>
    public TaskLowerViewModel TaskLowerViewModel { get; set; }

    /// <summary>
    /// View model for PropertiesView
    /// </summary>
    public TaskPropertiesViewModel TaskPropertiesViewModel { get; set; }
  }

  /// <summary>
  /// Comments loaded when scrolled to another page in Task CommentsView
  /// </summary>
  public class CommentsPageViewModel : AngularViewModel
  {
    /// <summary>
    /// Comments to be added to client-side comments buffer
    /// </summary>
    public CommentViewModel[] Comments { get; set; }
  }

  /// <summary>
  /// Contains content to be shown in upper viw
  /// </summary>
  public class TaskContentViewModel : AngularViewModel
  {
    public string Content { get; set; }
  }

  /// <summary>
  /// Comments to be appended/updated on clienside
  /// </summary>
  public class UpdatedCommentsViewModel : CommentsPageViewModel
  {
    /// <summary>
    /// New time of load from which queue will be ordered
    /// </summary>
    public long NewTimeOfLoad { get; set; }
  }

  /// <summary>
  /// Send to client for Task content editation
  /// </summary>
  public class TaskEditViewModel : AngularViewModel
  {
    /// <summary>
    /// Determines if task content is beeing edited by another user
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// User name of user holding lock
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Actual content to be shown for editation
    /// </summary>
    public string Content { get; set; }
  }

  public class TreeViewNode
  {
    public string id { get; set; }
    public string text { get; set; }
    public string parent { get; set; }
    public string icon { get; set; }
    public State state { get; set; }
    public string deadline { get; set; }
  }

  public class State
  {
    public bool opened { get; set; }
  }

  /// <summary>
  /// Model sent from client to create new folder
  /// </summary>
  public class NewFolder
  {
    public string NodeId { get; set; }
    public string Name { get; set; }

    public string ProjectId { get; set; }
  }

  /// <summary>
  /// Model sent from client to create new Task
  /// </summary>
  public class NewTask : NewFolder
  {
    public bool WithDeadline { get; set; }
    public DateTime DeadLine { get; set; }
  }

  public class TaskUpperViewModel
  {
    public bool IsValid { get; set; }
    public string Content { get; set; }
  }

  public class TaskLowerViewModel
  {
    public long TimeOfLoad { get; set; }
    public CommentViewModel[] Comments { get; set; }
  }

  /// <summary>
  /// ViewModel for properties view of task
  /// </summary>
  public class TaskPropertiesViewModel
  {
    public string Deadline { get; set; }

    /// <summary>
    /// Defines in which state is right now
    /// </summary>
    public ETaskState TaskState { get; set; }

    /// <summary>
    /// Determines if user is subscribed for updates for this task
    /// </summary>
    public bool Subscribed { get; set; }

    /// <summary>
    /// Is task completed
    /// </summary>
    public bool Completed { get; set; }
  }

  public abstract class PostTaskData
  {
    public int TaskID {get; set;}
  }

  /// <summary>
  /// Send to server from client when post new comment
  /// </summary>
  public class NewCommentData : PostTaskData
  {
    public string Content { get; set; }
    public long TimeOfLoad { get; set; }
  }



  /// <summary>
  /// Sent from client to server when saves update content
  /// </summary>
  public class UpdateTaskContentData : PostTaskData
  {
    public string Content { get; set; }
  }

  public class UpdateTaskStateData : PostTaskData
  {
    public string NewDeadline { get; set; }

    public bool? Completed { get; set; }

    public bool? Completable {get; set;}

    public string Name { get; set; }
  }

  public class ChangeNameData
  {
    public string TreeNodeID { get; set; }

    public string Name { get; set; }
  }

  public class CommentViewModel
  {
    public string Content { get; set; }

    public string Date { get; set; }

    public string UserName { get; set; }
  }

  public abstract class SignalRViewModel
  {
    public string GroupID { get; set; }
  }

  public class CommentSignalRViewModel : SignalRViewModel
  {
    public CommentViewModel Comment { get; set; }
  }

  public class ContentSignalRViewModel : SignalRViewModel
  {
    public string Content { get; set; }
  }
}