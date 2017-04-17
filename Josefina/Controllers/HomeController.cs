using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Josefina.Models;
using Josefina.Models.SharedViewModel;

namespace Josefina.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      return View();
    }

    private void AutoLogin()
    {
      RedirectToAction("TestLogin", "Account");
    }

    public ActionResult About()
    {
      return View();
    }

    public ActionResult Contact()
    {
      return View();
    }  

    public ActionResult Message(MessageViewModel model)
    {
      return View(model);
    }
  }
}