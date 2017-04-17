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
using Mvc.Mailer;
using Josefina.Mailers;
using Josefina.Models.SharedViewModel;
using Josefina.Models.AccountViewModel;
using Josefina.Helpers;

namespace Josefina.Controllers
{
    [Authorize]
    public class OrgController : Controller
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

        private IUserMailer _userMailer = new UserMailer();
        public IUserMailer UserMailer
        {
            get { return _userMailer; }
            set { _userMailer = value; }
        }

        [AllowAnonymous]
        public ActionResult RegistrationInfo()
        {
            ViewBag.Title = GeneralResources.Registration;
            return View();
        }

        /// <summary>
        /// First aproach
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.Title = GeneralResources.Registration;
            return View();
        }

        /// <summary>
        /// Send - validate Org name
        /// </summary>
        /// <param name="submitedModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(RegisterOrgViewModel submitedModel)
        { 
            ViewBag.Title = GeneralResources.Registration;
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var query = from org in context.Orgs
                            where org.Name == submitedModel.Name
                            select org;
                if (query.Any())
                {
                    ModelState.AddModelError("Name", OrgResources.AlreadyExistsError);
                }
                else
                {
                    var newOrg = new Entities.Org() { Name = submitedModel.Name.Trim(), VariableSymbolCounter = 1 };
                    context.Orgs.Add(newOrg);
                    context.SaveChanges();

                    var cookie = new HttpCookie("RegisteringOrgID", newOrg.OrgID.ToString());
                    cookie.Expires = DateTime.Now.AddHours(12);
                    Response.AppendCookie(cookie);


                    return RedirectToAction("Register", "Account");
                }
            }
            return View();
        }

        public ActionResult Home()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

            List<Project> projects;

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                projects = context.Projects.Where(p => p.OwnerID == user.OrgID).ToList();
            }
            return View(projects);
        }

        [AllowAnonymous]
        public ActionResult SendInvitation()
        {
            //Models.TicketsViewModel.TicketOrderConfirmationViewModel model = new Models.TicketsViewModel.TicketOrderConfirmationViewModel()
            //{
            //  DateEnd = "End",
            //  DateStart = "Start",
            //  Email = "bittmann.stefan@gmail.com",
            //  ProjectName = "Project",
            //  VariableSymbol = 666
            //};

            //List<TicketPDFGenerator.TicketToGenerateWrapper> ticketsToExport = new List<TicketPDFGenerator.TicketToGenerateWrapper>();

            //for (int i = 1; i < 201; i++)
            //{
            //  ticketsToExport.Add(new TicketPDFGenerator.TicketToGenerateWrapper()
            //  {
            //    Code = "10-" + i,
            //    ProjectName = "JunkTown",
            //    VisitorName = "",
            //    VisitorEmail = "",
            //    CategoryName = "Metroplex předprodej",
            //    Location = "50°3'31.522\"N, 14°0'39.485\"E",
            //    StartDate = "23.6 (18:00) - 26. 6"
            //  });
            //}     


            //byte[] zipTickets = TicketPDFGenerator.GetZippedPdfTickets(ticketsToExport);

            //string fileName = "tickets.zip";
            //return File(zipTickets, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            //TicketPDFGenerator.GetPDFTicket(ticketsToExport);

            //string fileName = "tickets.pdf";
            //return File(ticketsToExport.First().PDFTicket, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);


            //var email = _userMailer.SendTicketOrderConfirmation(model, ticketsToExport);
            //email.SendAsync();



            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendInvitation(SendInvitationViewModel submitedModel)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());

                string orgName = null;
                int invitationCode = -1;

                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    var existingEmail = await UserManager.FindByEmailAsync(submitedModel.Email);

                    if (existingEmail == null)
                    {
                        var userOrg = from org in context.Orgs
                                      where org.OrgID == user.OrgID
                                      select org;
                        if (userOrg.Count() == 1)
                        {
                            Random random = new Random();
                            int emailConfirmationCode;
                            bool isUnique = false;

                            do
                            {
                                emailConfirmationCode = random.Next(10000, 1000000000);

                                var duplicitInvitationCode = from inv in context.Invitations
                                                             where inv.Code == emailConfirmationCode
                                                             select inv;
                                isUnique = !duplicitInvitationCode.Any();
                            }
                            while (!isUnique);

                            var newInvitation = new Invitation()
                            {
                                Accepted = false,
                                Code = emailConfirmationCode,
                                Email = submitedModel.Email.Trim().ToLower(),
                                Org = userOrg.FirstOrDefault()
                            };

                            context.Invitations.Add(newInvitation);
                            context.SaveChanges();


                            invitationCode = newInvitation.Code;
                            orgName = newInvitation.Org.Name;
                        }
                        else
                        {
                            return View("~/Views/Shared/Error.cshtml");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", OrgResources.InvitationCantBeSentAlreadyExistEmail);
                        return View(submitedModel);
                    }
                }

                var email = _userMailer.SendInvitation(submitedModel.Email.Trim(), user.UserName, user.Email, orgName, invitationCode);
                email.SendAsync();

                ViewBag.StatusMessage = OrgResources.InvitationSend;
                return View(new SendInvitationViewModel());
            }

            return View(submitedModel);
        }

        [AllowAnonymous]
        public ActionResult InvitationRegistration(int id)
        {
            int invCode;
            string email;
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var invitationToConfirm = from inv in context.Invitations
                                          where inv.Code == id
                                          select inv;

                if (invitationToConfirm.Count() == 1)
                {
                    if (invitationToConfirm.FirstOrDefault().Accepted)
                    {
                        var messageModel = new MessageViewModel()
                        {
                            Redirection = false,
                            Body = OrgResources.InvitationAlreadyAccepted,
                            Header = GeneralResources.ErrorHasOccurred,
                        };

                        return RedirectToAction("Message", "Home", messageModel);
                    }
                    else
                    {
                        invCode = invitationToConfirm.FirstOrDefault().Code;
                        email = invitationToConfirm.FirstOrDefault().Email;
                    }
                }
                else
                {
                    return View("~/Views/Shared/Error.cshtml");
                }
            }

            var registerModel = new RegisterUserViewModel()
            {
                IsInvited = true,
                InvitationCode = invCode,
                Email = email
            };

            return View("~/Views/Account/Register.cshtml", registerModel);
            //return RedirectToAction("Register", "Account");
        }

        public ActionResult Projects()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                Org org = context.Orgs.Where(o => o.OrgID == user.OrgID).FirstOrDefault();

                var projects = from prj in context.Projects
                               where prj.OwnerID == org.OrgID
                               select prj;

                ProjectsListViewModel viewModel = new ProjectsListViewModel();
                foreach (var project in projects)
                {
                    viewModel.Projects.Add(project);
                }

                return View(viewModel);
            }
        }
    }
}