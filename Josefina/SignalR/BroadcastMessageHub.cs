using Josefina.DAL;
using Josefina.Entities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Josefina
{
  [HubName("tasksHub")]
  public class TasksHub : Hub
  {
    public void JoinGroup(string groupName)
    {
      //System.Diagnostics.Debug.WriteLine("Join: " + groupName + " - " + this.Context.ConnectionId);
      this.AddNotificationWipe(groupName, this.Context.ConnectionId);
      this.Groups.Add(this.Context.ConnectionId, groupName);
    }

    public void LeaveGroup(string groupName)
    {
      //System.Diagnostics.Debug.WriteLine("Leave: " + this.Context.ConnectionId);
      this.Groups.Remove(this.Context.ConnectionId, groupName);
    }

    private string GetUserID()
    {
      var identity = this.Context.User.Identity as ClaimsIdentity;
      Claim identityClaim = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
      return identityClaim.Value;
    }

    public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
    {
      //System.Diagnostics.Debug.WriteLine("OnDiTA: " + this.Context.ConnectionId);
      ClearNotificationWipe(this.Context.ConnectionId);
      return base.OnDisconnected(stopCalled);
    }

    private void ClearNotificationWipe(string connectionID)
    {
      using (ApplicationDbContext context = new ApplicationDbContext())
      {
        string userId = GetUserID();
        var taskUpdate = context.TaskUpdates.FirstOrDefault(tu => tu.ConnectionIDSignalR == connectionID && tu.UserID == userId);
        if (taskUpdate != null)
        {
          taskUpdate.Count = 0;
          taskUpdate.ConnectionIDSignalR = null;
          context.SaveChanges();
        }
      }
    }

    private void AddNotificationWipe(string groupName, string connectionID)
    {
      if (groupName.Contains("TaskComments:"))
      {
        using (ApplicationDbContext context = new ApplicationDbContext())
        {
          int taskID = -1;
          if (int.TryParse(groupName.Replace("TaskComments:", ""), out taskID))
          {
            string userId = GetUserID();
            var taskUpdate = context.TaskUpdates.FirstOrDefault(tu => tu.TaskID == taskID && tu.UserID == userId);
            if (taskUpdate != null)
            {
              taskUpdate.ConnectionIDSignalR = connectionID;
              context.SaveChanges();
            }
          }
        }
      }
    }
  }
}