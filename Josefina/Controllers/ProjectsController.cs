using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

using Josefina.DAL;
using Josefina.Entities;
using Resources;
using Josefina.Models.SharedViewModel;

namespace Josefina.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Project project)
        {
            if (ModelState.IsValid)
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    var user = UserManager.FindById(User.Identity.GetUserId());

                    if (context.Projects.Where(p => p.OwnerID == user.OrgID && p.Name == project.Name.Trim() && p.Starts == project.Starts && p.Ends == project.Ends).Any())
                    {
                        ModelState.AddModelError("", ProjectResources.AlreadyExistingProjectError);
                    }
                    else
                    {
                        project.OwnerID = user.OrgID;
                        var rootFolder = new Folder();
                        rootFolder.Name = project.Name.Trim();
                        context.Folders.Add(rootFolder);
                        TicketSetting ticketSetting = new TicketSetting();
                        ticketSetting.MaxTicketsPerEmail = 8;
                        project.TicketSetting = ticketSetting;

                        context.SaveChanges();

                        project.RootFolder = rootFolder;
                        context.Projects.Add(project);
                        context.SaveChanges();

                        var messageModel = new MessageViewModel();
                        messageModel.RedirectAction = "Home";
                        messageModel.Redirection = true;
                        messageModel.RedirectController = "Org";
                        messageModel.RedirectionButtonText = GeneralResources.Ok;
                        messageModel.Header = ProjectResources.ProjectCreated;
                        messageModel.Body = ProjectResources.ProjectCreatedBody;
                        return RedirectToAction("Message", "Home", messageModel);
                    }
                }
            }

            return View(project);
        }

        // GET: Projects/Edit/Id
        public ActionResult Edit(int? id)
        {
            Project project;
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var user = UserManager.FindById(User.Identity.GetUserId());

                if (id == null || !context.Projects.Any(p => p.ProjectID == id))
                {
                    return View("~/Views/Shared/Error.cshtml");
                }
                else
                {
                    project = context.Projects.First(p => p.ProjectID == id);
                    if (project.OwnerID != user.OrgID)
                    {
                        return View("~/Views/Shared/Error.cshtml");
                    }
                }
                return View(project);
            }
        }

        // POST: Projects/Edit/Id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Project project)
        {
            if (ModelState.IsValid)
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    var user = UserManager.FindById(User.Identity.GetUserId());

                    if (context.Projects.Any(p => p.ProjectID != project.ProjectID && p.OwnerID == user.OrgID
                      && p.Name == project.Name.Trim() && p.Starts == project.Starts && p.Ends == project.Ends))
                    {
                        ModelState.AddModelError("", ProjectResources.AlreadyExistingProjectError);
                        return View(project);
                    }
                    else
                    {
                        var projectToUpdate = context.Projects.SingleOrDefault(p => p.ProjectID == project.ProjectID);

                        projectToUpdate.Name = project.Name;
                        projectToUpdate.Ends = project.Ends;
                        projectToUpdate.Starts = project.Starts;
                        context.SaveChanges();
                    }
                }

                var messageModel = new MessageViewModel();
                messageModel.RedirectAction = "Home";
                messageModel.Redirection = true;
                messageModel.RedirectController = "Org";
                messageModel.RedirectionButtonText = GeneralResources.Ok;
                messageModel.Header = ProjectResources.ProjectUpdated;
                messageModel.Body = ProjectResources.ProjectUpdatedBody;
                return RedirectToAction("Message", "Home", messageModel);
            }
            return View(project);
        }

        // GET: Projects/Delete/Id
        public ActionResult Delete(int? id)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                if (id == null || !context.Projects.Any(p => p.ProjectID == id))
                {
                    return View("~/Views/Shared/Error.cshtml");
                }

                context.Projects.Remove(context.Projects.First(p => p.ProjectID == id));
                context.SaveChanges();
            }

            var messageModel = new MessageViewModel();
            messageModel.RedirectAction = "Home";
            messageModel.Redirection = true;
            messageModel.RedirectController = "Org";
            messageModel.RedirectionButtonText = GeneralResources.Ok;
            messageModel.Header = ProjectResources.ProjectDeleted;
            messageModel.Body = ProjectResources.ProjectDeletedBody;
            return RedirectToAction("Message", "Home", messageModel);
        }
    }
}
