using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using System.Xml.Linq;

using Josefina.Models.OrgViewModel;
using Resources;
using Josefina.DAL;
using Josefina.Entities;
using Josefina.Models.SharedViewModel;
using Josefina.Models.AccountViewModel;


namespace Josefina.Controllers
{
  [Authorize]
  public class ProjectController : Controller
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
    
    public ActionResult Show(int? id)
    {
      if(id == null)
      {
        return View("~/Views/Shared/Error.cshtml");
      }

      using (ApplicationDbContext context = new ApplicationDbContext())
      {
        var user = UserManager.FindById(User.Identity.GetUserId());

        Project project = context.Projects.FirstOrDefault(p => p.ProjectID == id);

        if(project == null || project.OwnerID != user.OrgID)
        {
          return View("~/Views/Shared/Error.cshtml");
        }

        return View("Tasks", project);
      }
    }
  }
}