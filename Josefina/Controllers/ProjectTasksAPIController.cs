using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Josefina.DAL;
using Josefina.Entities;
using Josefina.Models;
using Josefina.ApiModels.Tasks;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Microsoft.AspNet.SignalR;

namespace Josefina.Controllers
{
  [System.Web.Http.Authorize]
  [RoutePrefix("api/project/tasks")]
  public class ProjectTasksAPIController : ApiController
  {
    [HttpPost]
    [Route("changetaskstate/")]
    [ResponseType(typeof(IHttpActionResult))]
    public IHttpActionResult ChangeTaskState(UpdateTaskStateData data)
    {
      try
      {
        Task task = null;
        using (ApplicationDbContext context = new ApplicationDbContext())
        {
          task = context.Tasks.SingleOrDefault(t => t.TaskID == data.TaskID);
          if (task != null)
          {
            if (IsAuthorized(task, context))
            {
              if (data.Completed.HasValue)
              {
                task.Completed = data.Completed.Value;
              }

              if (data.Completable.HasValue)
              {
                if (data.Completable.Value)
                {
                  DateTime newDatetime;

                  if(DateTime.TryParse(data.NewDeadline, out newDatetime))
                  {
                    task.Deadline = newDatetime;
                  }
                  else
                  {
                    return NotFound();
                  }
                }
                else
                {
                  task.Deadline = null;
                }
              }

              if (data.Name != null && data.Name != "")
              {
                task.Name = data.Name; 
              }

              context.SaveChanges();
            }
            else
            {
              return Unauthorized();
            }
          }
          else
          {
            return NotFound();
          }
        }

        if (task != null)
        {
          NotifyTreeNodeUpdate(task, false);
          return Ok();
        }

        return NotFound();
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        throw;
      }
    }

    [HttpPost]
    [Route("changename/")]
    [ResponseType(typeof(IHttpActionResult))]
    public IHttpActionResult ChangeName(ChangeNameData data)
    {
      try
      {
        bool isTask;

        if (data.TreeNodeID[0] == 'T')
        {
          isTask = true;
        }
        else if (data.TreeNodeID[0] == 'F')
        {
          isTask = false;
        }
        else
        {
          return NotFound();
        }

        int entityID;
        if (!int.TryParse(data.TreeNodeID.Remove(0, 1), out entityID))
        {
          return NotFound();
        }

        Task task = null;
        Folder folder = null;

        using (ApplicationDbContext context = new ApplicationDbContext())
        {
          if (isTask)
          {
            task = context.Tasks.FirstOrDefault(t => t.TaskID == entityID);
            if (task != null)
            {
              task.Name = data.Name.Trim();
              context.SaveChanges();
            }
          }
          else
          {
            folder = context.Folders.FirstOrDefault(t => t.FolderID == entityID);
            if (folder != null)
            {
              folder.Name = data.Name.Trim();
              context.SaveChanges();
            }
          }
        }

        if (task != null)
        {
          NotifyTreeNodeUpdate(task, false);
        }
        else if (folder != null)
        {
          NotifyTreeNodeUpdate(folder, false);
        }

        return Ok();
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        throw;
      }
    }

    [HttpGet]
    [Route("deletenode/{nodeId}")]
    [ResponseType(typeof(AngularViewModel))]
    public ActionResult DeleteNode(string nodeId)
    {
      try
      {
        ActionResult result = new ActionResult();

        bool isTask;

        if (nodeId[0] == 'T')
        {
          isTask = true;
        }
        else if (nodeId[0] == 'F')
        {
          isTask = false;
        }
        else
        {
          result.IsValid = false;
          return result;
        }

        int entityID;
        if (!int.TryParse(nodeId.Remove(0, 1), out entityID))
        {
          return result;
        }

        using (ApplicationDbContext context = new ApplicationDbContext())
        {
          if (isTask)
          {
            var task = context.Tasks.SingleOrDefault(t => t.TaskID == entityID);
            if (task != null)
            {
              result.IsValid = false;
              if (IsAuthorized(task, context))
              {
                //If user is last editing user
                if (task.EditingUserID != null && task.EditHeartBeat.HasValue && task.EditHeartBeat.Value >= DateTime.Now.AddMinutes(-2)) //Client heartBeat: every 1min
                {
                  var editingUser = context.Users.Single(u => u.Id == task.EditingUserID);

                  result.ErrorMessage = "Obsah úkolu je momentáně upravován uživatelem: " + editingUser.UserName;
                  result.Success = false;
                }
                else
                {
                  result.IsAuthorized = true;
                  var taskUpdates = context.TaskUpdates.Where(tu => tu.TaskID == task.TaskID);
                  if (taskUpdates.Any())
                  {
                    context.TaskUpdates.RemoveRange(taskUpdates);
                  }
                  context.Tasks.Remove(task);
                  context.SaveChanges();
                  result.Success = true;
                }
              }
              else
              {
                result.IsAuthorized = false;
                result.Success = false;
              }
            }
          }
          else
          {
            var folder = context.Folders.FirstOrDefault(t => t.FolderID == entityID);
            if (folder != null)
            {
              result.IsValid = false;
              if (true) 
              {
                result.IsAuthorized = true;
                if ((folder.ChildFolders != null && folder.ChildFolders.Any()) || (folder.ChildTasks != null && folder.ChildTasks.Any()))
                {
                  result.ErrorMessage = "Nelze smazat složku obsahující podsložky, případně podúkoly.";
                  result.Success = false;
                }
                else
                {
                  context.Folders.Remove(folder);
                  context.SaveChanges();
                  result.Success = true;
                }
              }
              //else
              //{
              //  result.IsAuthorized = false;
              //  result.Success = false;
              //}
            }
          }
          return result;
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        throw;
      }
    }

    [HttpPost]
    [Route("postcontentedit/")]
    [ResponseType(typeof(IHttpActionResult))]
    public IHttpActionResult SaveEditTaskContent(UpdateTaskContentData data)
    {
      try
      {
        using (ApplicationDbContext context = new ApplicationDbContext())
        {
          var task = context.Tasks.FirstOrDefault(t => t.TaskID == data.TaskID);
          if (task != null)
          {
            if (task.EditingUserID == GetUser(context).Id)
            {
              task.Content = data.Content;
              task.EditHeartBeat = null;
              task.EditingUser = null;
              context.SaveChanges();

              var groupName = "TaskContent:" + task.TaskID;
              ContentSignalRViewModel signlaRContent = new ContentSignalRViewModel() { GroupID = groupName, Content = task.Content };
              var hubContext = GlobalHost.ConnectionManager.GetHubContext<TasksHub>();
              hubContext.Clients.Group(groupName).updateContent(signlaRContent);

              return Ok();
            }
            else
            {
              return Unauthorized();
            }
          }
          else
          {
            return NotFound();
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        throw;
      }
    }

    [Route("GetTaskEditHeartbeatResponse/{taskId:int}")]
    [ResponseType(typeof(AngularViewModel))]
    public AngularViewModel GetTaskEditHeartbeatResponse(int taskId)
    {
      try
      {
        using (ApplicationDbContext context = new ApplicationDbContext())
        {
          AngularViewModel viewModel = new AngularViewModel();
          var task = context.Tasks.FirstOrDefault(t => t.TaskID == taskId);
          if (task != null)
          {
            viewModel.IsValid = true;
            if (task.EditingUserID == GetUser(context).Id)
            {
              viewModel.IsAuthorized = true;

              task.EditHeartBeat = DateTime.Now;
              context.SaveChanges();

              return viewModel;
            }
            else
            {
              viewModel.IsAuthorized = false;
            }
          }
          else
          {
            viewModel.IsValid = false;
          }

          return viewModel;
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        throw;
      }
    }

    [HttpGet]
    [Route("cancelcontentedit/{taskId:int}")]
    [ResponseType(typeof(IHttpActionResult))]
    public IHttpActionResult CancelContentEdit(int taskId)
    {
      try
      {
        using (ApplicationDbContext context = new ApplicationDbContext())
        {
          var task = context.Tasks.FirstOrDefault(t => t.TaskID == taskId);
          if (task != null)
          {
            if (task.EditingUserID == GetUser(context).Id)
            {
              task.EditingUserID = null;
              task.EditHeartBeat = null;
              context.SaveChanges();
            }
            return Ok();
          }
          return NotFound();
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        throw;
      }
    }

    [HttpGet]
    [Route("subscribetoupdates/{taskId:int}")]
    [ResponseType(typeof(IHttpActionResult))]
    public IHttpActionResult SubscribeToUpdates(int taskId)
    {
      try
      {
        using (ApplicationDbContext context = new ApplicationDbContext())
        {
          var task = context.Tasks.FirstOrDefault(t => t.TaskID == taskId);
          if (task != null)
          {
            if (IsAuthorized(task, context))
            {
              ApplicationUser user = GetUser(context);

              TaskUpdate taskUpdate = new TaskUpdate();
              taskUpdate.UserID = user.Id;
              taskUpdate.Task = task;
              context.TaskUpdates.Add(taskUpdate);
              context.SaveChanges();
            }
            return Ok();
          }
          return NotFound();
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        throw;
      }
    }

    [HttpGet]
    [Route("unsubscribetoupdates/{taskId:int}")]
    [ResponseType(typeof(IHttpActionResult))]
    public IHttpActionResult UnsubscribeToUpdates(int taskId)
    {
      try
      {
        using (ApplicationDbContext context = new ApplicationDbContext())
        {
          var task = context.Tasks.FirstOrDefault(t => t.TaskID == taskId);
          if (task != null)
          {
            if (IsAuthorized(task, context))
            {
              ApplicationUser user = GetUser(context);
              TaskUpdate taskUpdate = context.TaskUpdates.First(tu => tu.UserID == user.Id && tu.TaskID == task.TaskID);
              context.TaskUpdates.Remove(taskUpdate);
              context.SaveChanges();
            }
            return Ok();
          }
          return NotFound();
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        throw;
      }
    }

    [Route("getedittaskcontent/{taskId:int}")]
    [ResponseType(typeof(TaskEditViewModel))]
    public TaskEditViewModel GetTaskEditViewModel(int taskId)
    {
      try
      {
        using (ApplicationDbContext context = new ApplicationDbContext())
        {
          TaskEditViewModel viewModel = new TaskEditViewModel();
          var task = context.Tasks.FirstOrDefault(t => t.TaskID == taskId);
          if (task != null)
          {
            viewModel.IsValid = true;
            if (IsAuthorized(task, context))
            {
              viewModel.IsAuthorized = true;

              string currentUserID = GetUser(context).Id;

              //If user is last editing user
              if (task.EditingUserID != null && task.EditingUserID != currentUserID && task.EditHeartBeat.HasValue)
              {
                if (task.EditHeartBeat.Value >= DateTime.Now.AddMinutes(-2)) //Client heartBeat: every 1min
                {
                  //HeartBeat not expired 
                  viewModel.IsLocked = true;
                  viewModel.UserName = context.Users.First(u => u.Id == task.EditingUserID).UserName;
                  return viewModel;
                }
              }

              task.EditHeartBeat = DateTime.Now;
              task.EditingUserID = currentUserID;

              viewModel.Title = task.Name + "*";
              viewModel.Content = task.Content;
              context.SaveChanges();
              return viewModel;
            }
            else
            {
              viewModel.IsAuthorized = false;
            }
          }
          else
          {
            viewModel.IsValid = false;
          }
          return viewModel;
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        throw;
      }
    }

    [HttpPost]
    [Route("postcomment/")]
    [ResponseType(typeof(UpdatedCommentsViewModel))]
    public UpdatedCommentsViewModel CreateNewComment(NewCommentData data)
    {
      try
      {
        using (ApplicationDbContext context = new ApplicationDbContext())
        {
          UpdatedCommentsViewModel viewModel = new UpdatedCommentsViewModel();
          var task = context.Tasks.FirstOrDefault(t => t.TaskID == data.TaskID);
          if (task != null)
          {
            viewModel.IsValid = true;

            viewModel.IsValid = true;
            if (IsAuthorized(task, context))
            {
              viewModel.IsAuthorized = true;
              ApplicationUser logedUser = GetUser(context);

              DateTime currentDateTime = DateTime.Now;

              Comment newComment = new Comment() { Content = data.Content, Created = currentDateTime.AddMilliseconds(-1), ParentUser = logedUser, ParentTask = task };

              context.Comments.Add(newComment);

              var notificationToAdd = context.TaskUpdates.Where(tu => tu.TaskID == task.TaskID && tu.UserID != logedUser.Id);
              foreach (var taskUpdate in notificationToAdd)
              {
                taskUpdate.Count++;
              }

              context.SaveChanges();
              //DateTime lastTimeOfLoad = new DateTime(data.TimeOfLoad);

              //CommentViewModel[] commentsToBeUpdated = GetCommentsAfterDate(task, lastTimeOfLoad, context);

              //viewModel.Comments = commentsToBeUpdated;
              //viewModel.NewTimeOfLoad = currentDateTime.Ticks;

              var groupName = "TaskComments:" + task.TaskID;
              CommentViewModel commentViewModel = new CommentViewModel() { Content = newComment.Content, UserName = logedUser.UserName, Date = String.Format("{0:H:mm, d.M.}", newComment.Created.Value) };
              CommentSignalRViewModel signalRComment = new CommentSignalRViewModel() { GroupID = groupName, Comment = commentViewModel };

              var hubContext = GlobalHost.ConnectionManager.GetHubContext<TasksHub>();
              hubContext.Clients.Group(groupName).newComment(signalRComment);

              return viewModel;
            }
            else
            {
              viewModel.IsAuthorized = false;
            }

          }
          else
          {
            viewModel.IsValid = false;
          }
          return viewModel;
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        throw;
      }
    }

    [Route("gettaskcommentspage/{taskId:int}/{pageNumber:int}/{timeOfLoadTicks:long}")]
    [ResponseType(typeof(CommentsPageViewModel))]
    public CommentsPageViewModel GetTaskCommentsPage(int taskId, int pageNumber, long timeOfLoadTicks)
    {
      try
      {
        DateTime timeOfLoad = new DateTime(timeOfLoadTicks);
        using (ApplicationDbContext context = new ApplicationDbContext())
        {
          CommentsPageViewModel viewModel = new CommentsPageViewModel();
          var task = context.Tasks.FirstOrDefault(t => t.TaskID == taskId);
          if (task != null)
          {
            viewModel.IsValid = true;

            if (IsAuthorized(task, context))
            {
              viewModel.IsAuthorized = true;
              viewModel.Comments = GetComments(task, context, pageNumber, timeOfLoad);

              return viewModel;
            }
            else
            {
              viewModel.IsAuthorized = false;
            }
          }
          else
          {
            viewModel.IsValid = false;
          }
          return viewModel;
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        throw;
      }
    }

    [Route("gettaskviewmodel/{taskId:int}")]
    [ResponseType(typeof(TaskViewModel))]
    public TaskViewModel GetTaskViewModel(int taskId)
    {
      try
      {
        using (ApplicationDbContext context = new ApplicationDbContext())
        {
          TaskViewModel viewModel = new TaskViewModel();
          var task = context.Tasks.FirstOrDefault(t => t.TaskID == taskId);
          if (task != null)
          {
            viewModel.IsValid = true;
            if (IsAuthorized(task, context))
            {
              viewModel.IsAuthorized = true;
              viewModel.Title = task.Name;
              viewModel.TaskUpperViewModel.Content = task.Content;

              viewModel.TaskPropertiesViewModel.Deadline = task.Deadline.HasValue ? task.Deadline.Value.ToShortDateString() : "";
              viewModel.TaskPropertiesViewModel.Completed = task.Completed;

              var user = GetUser(context);
              viewModel.TaskPropertiesViewModel.Subscribed = context.TaskUpdates.Any(tu => tu.TaskID == task.TaskID && tu.UserID == user.Id);

              DateTime timeOfLoad = DateTime.Now;

              viewModel.TaskLowerViewModel.TimeOfLoad = timeOfLoad.Ticks;
              viewModel.TaskLowerViewModel.Comments = GetComments(task, context, 1, timeOfLoad);

              return viewModel;
            }
            else
            {
              viewModel.IsAuthorized = false;
            }
          }
          else
          {
            viewModel.IsValid = false;
          }
          return viewModel;
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        throw;
      }
    }

    [Route("gettaskcontent/{taskId:int}")]
    [ResponseType(typeof(TaskViewModel))]
    public TaskContentViewModel GetTaskContent(int taskId)
    {
      try
      {
        using (ApplicationDbContext context = new ApplicationDbContext())
        {
          TaskContentViewModel viewModel = new TaskContentViewModel();
          var task = context.Tasks.FirstOrDefault(t => t.TaskID == taskId);
          if (task != null)
          {
            viewModel.IsValid = true;
            if (IsAuthorized(task, context))
            {
              viewModel.IsAuthorized = true;
              viewModel.Title = task.Name;
              viewModel.Content = task.Content;
              return viewModel;
            }
            else
            {
              viewModel.IsAuthorized = false;
            }
          }
          else
          {
            viewModel.IsValid = false;
          }
          return viewModel;
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        throw;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <returns></returns>
    [Route("gettasksviewmodel/{projectId:int}")]
    [ResponseType(typeof(TasksViewModel))]
    public TasksViewModel GetTasksViewModel(int projectId)
    {
      try
      {
        using (ApplicationDbContext context = new ApplicationDbContext())
        {
          var project = context.Projects.FirstOrDefault(p => p.ProjectID == projectId);
          TasksViewModel viewModel = new TasksViewModel();
          if (project != null)
          {
            viewModel.IsValid = true;

            if (IsAuthorized(project, context))
            {
              viewModel.IsAuthorized = true;

              var rootFolder = project.RootFolder;

              viewModel.Title = project.Name;

              string userId = GetUserID();

              List<TreeViewNode> treeViewNodes = new List<TreeViewNode>();
              treeViewNodes.Add(new TreeViewNode()
              {
                id = "P" + rootFolder.FolderID.ToString(),
                parent = "#",
                text = rootFolder.Name,
                icon = "/content/Icons/TreeView/Project.png",
                state = new State() { opened = true }
              });
              treeViewNodes.AddRange(GetFolderChilds(rootFolder, true, userId, context));
              viewModel.TreeViewNodes = treeViewNodes;
              viewModel.TreeViewClonedNodes = treeViewNodes;
              return viewModel;
            }
            else
            {
              viewModel.IsAuthorized = false;
              return viewModel;
            }
          }
          else
          {
            viewModel.IsValid = false;
            return viewModel;
          }
        }
      }
      catch (Exception e)
      {
        throw;
      }
    }

    [HttpPost]
    [Route("createtask/")]
    [ResponseType(typeof(TreeViewNode))]
    public IHttpActionResult CreateTask(NewTask newTask)
    {
      int id = 0;

      if ((newTask.NodeId[0] == 'F' || newTask.NodeId[0] == 'P') && int.TryParse(newTask.NodeId.Remove(0, 1), out id))
      {
        Task task = null;
        using (ApplicationDbContext context = new ApplicationDbContext())
        {
          ApplicationUser user = GetUser(context);
          task = new Task()
          {
            OrgID = user.OrgID,
            Name = newTask.Name.Trim(),
            Deadline = null,
            ParentID = id,
            Content = "<h1>" + newTask.Name.Trim() + "</h1>"
          };

          if (newTask.WithDeadline)
          {
            task.Deadline = newTask.DeadLine.AddHours(1).AddMinutes(-1); // Konverze z špatného časového pásma
          }

          context.Tasks.Add(task);
          context.SaveChanges();
        }
        NotifyTreeNodeUpdate(task, true);
        return Ok();
      }
      return NotFound();
    }

    [HttpPost]
    [Route("createfolder/")]
    [ResponseType(typeof(NewFolder))]
    public IHttpActionResult CreateFolder(NewFolder newFolder)
    {
      int id = 0;
      string strID = newFolder.NodeId.Remove(0, 1);

      if ((newFolder.NodeId[0] == 'F' || newFolder.NodeId[0] == 'P') && int.TryParse(strID, out id))
      {
        Folder folder;
        using (ApplicationDbContext context = new ApplicationDbContext())
        {
          folder = new Folder()
          {
            Name = newFolder.Name.Trim(),
            ParentID = id,
          };

          context.Folders.Add(folder);
          context.SaveChanges();
        }
        NotifyTreeNodeUpdate(folder, true);
        return Ok();
      }
      return NotFound();
    }


    #region HelperMethods

    /// <summary>
    /// Defines how count of comments loaded on one page
    /// </summary>
    private const int COMMENTS_PAGE_COUNT = 100;

    private ApplicationUser GetUser(ApplicationDbContext context)
    {
      var identity = User.Identity as ClaimsIdentity;
      Claim identityClaim = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

      return context.Users.FirstOrDefault(u => u.Id == identityClaim.Value);
    }

    private string GetUserID()
    {
      var identity = User.Identity as ClaimsIdentity;
      Claim identityClaim = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
      return identityClaim.Value;
    }

    private bool IsAuthorized(Project project, ApplicationDbContext context)
    {
      ApplicationUser user = GetUser(context);

      return user.OrgID == project.OwnerID;
    }

    private bool IsAuthorized(Task task, ApplicationDbContext context)
    {
      ApplicationUser user = GetUser(context);

      return user.OrgID == task.OrgID;
    }

    private void NotifyTreeNodeUpdate(Task task, bool isNew)
    {
      using (ApplicationDbContext context = new ApplicationDbContext())
      {
        TreeViewNode node = GetTreeViewNode(task, context);
        Project project = GetProject(task, context);
        NotifyTreeNodeUpdate(node, project.ProjectID, isNew);
      }
    }

    private void NotifyTreeNodeUpdate(Folder folder, bool isNew)
    {
      try
      {
        using (ApplicationDbContext context = new ApplicationDbContext())
        {
          TreeViewNode node = GetTreeViewNode(folder, context);
          Project project = GetProject(folder.FolderID, context);
          NotifyTreeNodeUpdate(node, project.ProjectID, isNew);
        }
      }
      catch (Exception e)
      {
        throw;
      }
    }

    private void NotifyTreeNodeUpdate(TreeViewNode node, int projectId, bool isNew)
    {
      var hubContext = GlobalHost.ConnectionManager.GetHubContext<TasksHub>();
      var groupName = "TasksTreeNodes:" + projectId;

      hubContext.Clients.Group(groupName).updateTreeView(groupName, node, isNew);
    }

    private TreeViewNode GetTreeViewNode(Task task, ApplicationDbContext context)
    {
      Task realoadedTask = context.Tasks.Single(t => t.TaskID == task.TaskID);

      TreeViewNode newNode = new TreeViewNode();
      newNode.icon = GetTaskIcon(realoadedTask);
      newNode.text = realoadedTask.Name;
      newNode.id = "T" + realoadedTask.TaskID.ToString();
      newNode.deadline = realoadedTask.Deadline.HasValue ? String.Format("{0:dd.MM.yyyy}", realoadedTask.Deadline.Value) : "";

      if (!realoadedTask.Parent.ParentID.HasValue)
      {
        newNode.parent = "P" + realoadedTask.ParentID.ToString();
      }
      else
      {
        newNode.parent = "F" + realoadedTask.ParentID.ToString();
      }
      newNode.state = new State() { opened = false };
      return newNode;
    }

    private string GetTaskIcon(Task childTask)
    {
      var taskState = TaskHelper.GetTaskState(childTask);

      switch (taskState)
      {
        case ETaskState.Completed:
          return "/content/Icons/TreeView/Tasks/greenCheck.png";
        case ETaskState.InProgres:
          return "/content/Icons/TreeView/Tasks/greyCheck.png";
        case ETaskState.OverDue:
          return "/content/Icons/TreeView/Tasks/redCheck.png";
        case ETaskState.Uncompletable:
          return "/content/Icons/TreeView/Tasks/greyCheck.png";
        default:
          return "/content/Icons/TreeView/Tasks/greyCheck.png";
      }
    }

    private TreeViewNode GetTreeViewNode(Folder folder, ApplicationDbContext context)
    {
      TreeViewNode newNode = new TreeViewNode();
      newNode.icon = "";
      newNode.text = folder.Name;
      newNode.id = "F" + folder.FolderID.ToString();

      Folder reloadedFolder = context.Folders.Single(f => f.FolderID == folder.FolderID);

      if (!reloadedFolder.Parent.ParentID.HasValue)
      {
        newNode.parent = "P" + reloadedFolder.ParentID.ToString();
      }
      else
      {
        newNode.parent = "F" + reloadedFolder.ParentID.ToString();
      }
      newNode.state = new State() { opened = false };
      return newNode;
    }

    private Project GetProject(Task task, ApplicationDbContext context)
    {
      return GetProject(task.ParentID, context);
    }

    private Project GetProject(int? folderId, ApplicationDbContext context)
    {
      Folder folder = context.Folders.Single(f => f.FolderID == folderId);

      if (folder.ParentID.HasValue)
      {
        return GetProject(folder.ParentID, context);
      }
      else
      {
        return context.Projects.Single(p => p.RootFolderID == folder.FolderID);
      }
    }

    /// <summary>
    /// Gets comments before and in given date. 
    /// </summary>
    /// <param name="task"></param>
    /// <param name="timeOfLoad"></param>
    /// <returns></returns>
    private CommentViewModel[] GetCommentsAfterDate(Task task, DateTime timeOfLoad, ApplicationDbContext context)
    {
      var comments = context.Comments.Where(c => c.ParentTaskID == task.TaskID && c.Created.Value >= timeOfLoad).OrderByDescending(c => c.Created).ToArray();

      CommentViewModel[] commentViewModels = new CommentViewModel[comments.Length];

      for (int i = 0; i < comments.Length; i++)
      {
        commentViewModels[i] = new CommentViewModel() { Content = comments[i].Content, Date = String.Format("{0:H:mm, d.M.}", comments[i].Created.Value), UserName = comments[i].ParentUser.UserName };
      }

      return commentViewModels;
    }

    /// <summary>
    /// Return array of comment ordered 
    /// </summary>
    /// <param name="task"></param>
    /// <param name="context"></param>
    /// <param name="page"></param>
    /// <param name="dtStart"></param>
    /// <returns></returns>
    private CommentViewModel[] GetComments(Task task, ApplicationDbContext context, int page, DateTime dtStart)
    {
      var comments = context.Comments.Where(c => c.ParentTaskID == task.TaskID && c.Created.Value <= dtStart).OrderByDescending(c => c.Created).ToArray();

      int start = (page - 1) * COMMENTS_PAGE_COUNT; // 0 based
      int end = page * COMMENTS_PAGE_COUNT; // 1 based

      if (comments.Length - 1 < start)
      {
        return new CommentViewModel[0];
      }
      else if (comments.Length < end)
      {
        end = comments.Length;
      }

      if (page == 1)
      {
        comments = comments.Take(end - start).ToArray();
      }
      else
      {
        comments = comments.Skip(start).Take(end - start).ToArray();
      }

      CommentViewModel[] commentViewModels = new CommentViewModel[comments.Length];

      for (int i = 0; i < comments.Length; i++)
      {
        commentViewModels[i] = new CommentViewModel() { Content = comments[i].Content, Date = String.Format("{0:H:mm, d.M.}", comments[i].Created.Value), UserName = comments[i].ParentUser.UserName };
      }

      return commentViewModels;
    }

    private List<TreeViewNode> GetFolderChilds(Folder folder, bool root, string userId, ApplicationDbContext context)
    {
      List<TreeViewNode> childs = new List<TreeViewNode>();

      foreach (var childFolder in folder.ChildFolders)
      {
        childs.Add(new TreeViewNode()
        {
          id = "F" + childFolder.FolderID.ToString(),
          parent = (root ? "P" + folder.FolderID : "F" + folder.FolderID),
          text = childFolder.Name,
          icon = "",
          state = new State() { opened = false }
        });
        childs.AddRange(GetFolderChilds(childFolder, false, userId, context));
      }

      foreach (var childTask in folder.ChildTasks)
      {
        TaskUpdate taskUpdate = context.TaskUpdates.FirstOrDefault(tu => tu.TaskID == childTask.TaskID && tu.UserID == userId);
        string taskNameWithNotify = "";
        if (taskUpdate != null)
        {
          if (taskUpdate.Count > 0)
          {
            taskNameWithNotify = childTask.Name + "<span style=\"margin-top:-2px;margin-left:10px\" class=\"badge\">" + taskUpdate.Count + "</span>";
          }
          else
          {
            taskNameWithNotify = childTask.Name;
          }
        }
        else
        {
          taskNameWithNotify = childTask.Name;
        }

        string deadlineTask = childTask.Deadline.HasValue ? String.Format("{0:dd.MM.yyyy}", childTask.Deadline.Value) : "";

        childs.Add(new TreeViewNode()
        {
          id = "T" + childTask.TaskID.ToString(),
          parent = (root ? "P" + folder.FolderID : "F" + folder.FolderID),
          text = taskNameWithNotify,
          icon = GetTaskIcon(childTask),
          state = new State() { opened = false },
          deadline = deadlineTask
        });
      }

      return childs;
    }

    #endregion
  }
}