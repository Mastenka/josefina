using Josefina.DAL;
using Josefina.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Josefina.Models.TicketsViewModel;
using System.Data.Entity.Infrastructure;
using Josefina.Mailers;
using System.Data.Entity;
using System.Web.Routing;

namespace Josefina.Controllers
{
    [AllowAnonymous]
    public class TicketsController : Controller
    {
        private const string LocalizationCookie = "LocalizationCookie";

        //[Route("ChangeLanguage/{projectId:int}")]
        public ActionResult ChangeLanguage(int id)
        {
            var cookie = Request.Cookies[LocalizationCookie];
            var newCookie = new HttpCookie(LocalizationCookie);
            if (cookie != null)
            {
                if (cookie.Value == "en")
                {
                    newCookie.Value = "cz";
                }
                else
                {
                    newCookie.Value = "en";
                }
            }
            newCookie.Expires = DateTime.Now.AddYears(2);
            Response.SetCookie(newCookie);

            return Redirect(string.Format("http://{0}/vstupenky/{1}", Request.Url.Authority, id));
        }

        public ActionResult CreateReservation(int projectId, string projectString)
        {
            var viewModel = CreateReservationInit(projectId, false);
            return View("CreateReservationLocalized", viewModel);
        }

        public ActionResult CreateCodeReservation(int projectId)
        {
            var viewModel = CreateReservationInit(projectId, true);
            return View("CreateCodeReservationLocalized", viewModel);
        }

        private ShowTicketCategoriesViewModel CreateReservationInit(int projectId, bool isCode)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                Project project = context.Projects.SingleOrDefault(p => p.ProjectID == projectId);

                if (project == null)
                {
                    throw new Exception("Project not found.");
                }

                ViewBag.Title = project.Name + " | Vstupenky";
                ShowTicketCategoriesViewModel viewModel;

                if (isCode)
                {
                    viewModel = new ShowTicketCategoriesCodeViewModel();
                }
                else
                {
                    viewModel = new ShowTicketCategoriesViewModel();
                }

                viewModel.ProjectID = project.ProjectID;
                viewModel.ProjectName = project.Name;
                viewModel.MaxTicketsPerMail = project.TicketSetting.MaxTicketsPerEmail;

                viewModel.TicketCategories = new List<TicketCategoryViewModel>();

                if (project.TicketsURL != null && project.TicketsURL != "")
                {
                    viewModel.TicketsAvailable = true;
                }
                else
                {
                    viewModel.TicketsAvailable = false;
                    return viewModel;
                }

                foreach (TicketCategory ticketCategory in project.TicketCategories.Where(tc => !tc.Deleted && tc.CodeRequired == isCode))
                {

                    if (ticketCategory.SoldFrom.Value.ToLocalTime() <= DateTime.Now && ticketCategory.SoldTo.Value.ToLocalTime() >= DateTime.Now)
                    {
                        TicketCategoryViewModel ticketCategoryViewModel = new TicketCategoryViewModel()
                        {
                            AvailableCapacity = ticketCategory.Capacity - ticketCategory.Ordered,
                            Capacity = ticketCategory.Capacity,
                            Header = ticketCategory.HeaderCZ,
                            Price = ticketCategory.Price + "Kč",
                            TicketCategoryID = ticketCategory.TicketCategoryID,
                            SoldFrom = String.Format("{0:d.M.yyyy HH:mm:ss}", ticketCategory.SoldFrom.Value.ToLocalTime()),
                            SoldTo = String.Format("{0:d.M.yyyy HH:mm:ss}", ticketCategory.SoldTo.Value.ToLocalTime()),
                            RowVersion = ticketCategory.RowVersion
                        };

                        viewModel.TicketCategories.Add(ticketCategoryViewModel);
                    }
                }

                viewModel.TicketsAvailable = viewModel.TicketCategories.Any();

                SetLocalization(viewModel, projectId, isCode);

                return viewModel;
            }
        }

        [HttpPost]
        public ActionResult CreateReservation(ShowTicketCategoriesViewModel submitedModel)
        {
            string viewName = "~/Views/Tickets/CreateReservationLocalized.cshtml";
            SetLocalization(submitedModel, submitedModel.ProjectID, false);

            if (!IsModelValid(submitedModel))
            {
                return View(viewName, submitedModel);
            }

            using (ApplicationDbContext context = new ApplicationDbContext())
            {

                var result = CreateReservationFromAction(submitedModel, context, viewName);
                return result;
            }
        }

        [HttpPost]
        public ActionResult CreateCodeReservation(ShowTicketCategoriesCodeViewModel submitedModel)
        {
            string viewName = "~/Views/Tickets/CreateCodeReservationLocalized.cshtml";
            SetLocalization(submitedModel, submitedModel.ProjectID, true);

            if (!IsModelValid(submitedModel))
            {
                return View(viewName, submitedModel);
            }

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                if (!ValidateTicketCode(submitedModel, context))
                {
                    return View(viewName, submitedModel);
                }

                var result = CreateReservationFromAction(submitedModel, context, viewName);
                return result;
            }
        }


        private bool ValidateTicketCode(ShowTicketCategoriesCodeViewModel submitedModel, ApplicationDbContext context)
        {
            var categoryToOrder = submitedModel.TicketCategories.Single(tc => tc.Ordered > 0);

            if (submitedModel.Code != context.TicketCategories.Single(tc => tc.TicketCategoryID == categoryToOrder.TicketCategoryID).Code)
            {
                ModelState.AddModelError("ErrorSum", "Zadaný kod není platný.");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsModelValid(ShowTicketCategoriesViewModel submitedModel)
        {
            if (submitedModel.TicketCategories.Any(tc => tc.AvailableCapacity - tc.Ordered < 0))
            {
                ModelState.AddModelError("ErrorSum", "Dané množství vstupenek není možné objednat");
                return false;
            }

            if (!submitedModel.TicketCategories.Any(tc => tc.Ordered > 0))
            {
                ModelState.AddModelError("ErrorSum", "Není vybrána žádná vstupenka");
                return false;
            }

            if (submitedModel is ShowTicketCategoriesCodeViewModel)
            {
                if (submitedModel.TicketCategories.Count(tc => tc.Ordered > 0) > 1)
                {
                    ModelState.AddModelError("ErrorSum", "Je možné objednat vstupenky pouze z jedné kategorie");
                    return false;
                }
            }

            return true;
        }

        private ActionResult CreateReservationFromAction(ShowTicketCategoriesViewModel submitedModel, ApplicationDbContext context, string viewName)
        {
            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                Project project = context.Projects.SingleOrDefault(p => p.ProjectID == submitedModel.ProjectID);

                if (project == null)
                {
                    return View("~/Views/Shared/Error.cshtml");
                }

                if (ExceededMaxTickets(project, submitedModel, context))
                {
                    ModelState.AddModelError("ErrorSum", "Překročen maximální počet vstupenek na email: " + project.TicketSetting.MaxTicketsPerEmail);
                    return View(viewName, submitedModel);
                }

                if (project.TicketSetting.NamedTickets)
                {
                    if (!submitedModel.AfterNameSetting)
                    {
                        foreach (var ticketCategory in submitedModel.TicketCategories)
                        {
                            if (ticketCategory.Ordered > 0)
                            {
                                ticketCategory.Names = new TicketName[ticketCategory.Ordered];
                                ticketCategory.Emails = new TicketEmail[ticketCategory.Ordered];
                                ticketCategory.Emails[0] = new TicketEmail();
                                ticketCategory.Emails[0].Email = submitedModel.Email;
                            }
                        }
                        

                        submitedModel.AfterNameSetting = true;
                        return View("~/Views/Tickets/NamedTicketsLocalized.cshtml", submitedModel);
                    }
                }

                if (project.TicketSetting.AllowTermsConditions)
                {
                    if (project.TicketSetting.NamedTickets && submitedModel.AfterNameSetting && !submitedModel.AfterTermsConditionsSetting)
                    {
                        submitedModel.AfterTermsConditionsSetting = true;
                        bool isEnglish = false;
                        var cookie = Request.Cookies[LocalizationCookie];

                        if (cookie != null)
                        {
                            if (cookie.Value == "en")
                            {
                                isEnglish = true;
                            }
                        }
                        submitedModel.TermsConditions = isEnglish ? project.TicketSetting.TermsConditionsEN : project.TicketSetting.TermsConditionsCZ;

                        return View("~/Views/Tickets/TermsConditionsLocalized.cshtml", submitedModel);
                    }
                }


                bool overBought = false;

                TicketOrder ticketOrder = new TicketOrder();
                ticketOrder.Created = DateTime.Now;
                ticketOrder.Email = submitedModel.Email;
                ticketOrder.OrgID = project.OwnerID;
                ticketOrder.ProjectID = project.ProjectID;

                ticketOrder.ReservedUntil = DateTime.Today.AddDays(7).Date;

                if (project.TicketSetting.AllowTermsConditions) {
                    ticketOrder.TermsConditionsAccepted = submitedModel.TicketCategories[0].TermsConditionsAccepted;
                }

                context.TicketOrders.Add(ticketOrder);
                context.SaveChanges();

                List<TicketCategoryOrderViewModel> ticketCategoryOrderViewModels = new List<TicketCategoryOrderViewModel>();

                foreach (var ticketCategoryViewModel in submitedModel.TicketCategories)
                {
                    TicketCategory ticketCategory = context.TicketCategories.SingleOrDefault(tc => tc.TicketCategoryID == ticketCategoryViewModel.TicketCategoryID);

                    //Objednání dané kapacity, budu při konkurenci zopakováno 10x
                    for (int i = 0; i < 10; i++)
                    {
                        if (ticketCategory == null || !project.TicketCategories.Contains(ticketCategory) || ticketCategory.Deleted)
                        {
                            transaction.Rollback();
                            return View("~/Views/Shared/Error.cshtml");
                        }

                        //Capacity changed
                        if (ticketCategoryViewModel.Capacity != ticketCategory.Capacity)
                        {
                            ticketCategoryViewModel.Capacity = ticketCategory.Capacity;
                        }

                        //Available capacity changed
                        if (ticketCategoryViewModel.AvailableCapacity != ticketCategory.Capacity - ticketCategory.Ordered)
                        {
                            ticketCategoryViewModel.AvailableCapacity = ticketCategory.Capacity - ticketCategory.Ordered;
                        }

                        if (ticketCategoryViewModel.Ordered > 0 && !overBought)
                        {
                            if (ticketCategory.Capacity < ticketCategory.Ordered + ticketCategoryViewModel.Ordered)
                            {
                                overBought = true;
                                break;
                            }
                            else
                            {
                                ticketCategory.Ordered += ticketCategoryViewModel.Ordered;

                                TicketCategoryOrder ticketCategoryOrder = new TicketCategoryOrder()
                                {
                                    Count = ticketCategoryViewModel.Ordered,
                                    TicketCategory = ticketCategory,
                                    TicketOrder = ticketOrder
                                };

                                context.TicketCategoryOrders.Add(ticketCategoryOrder);

                                try
                                {
                                    context.SaveChanges();

                                    decimal categoryPrice = ticketCategory.Price * ticketCategoryViewModel.Ordered;

                                    TicketCategoryOrderViewModel ticketCategoryOrderViewModel = new TicketCategoryOrderViewModel()
                                    {
                                        Header = ticketCategory.HeaderCZ,
                                        Ordered = ticketCategoryViewModel.Ordered,
                                        TotalPrice = categoryPrice
                                    };

                                    ticketCategoryOrderViewModels.Add(ticketCategoryOrderViewModel);
                                    ticketOrder.TotalPrice += categoryPrice;
                                    //Úspěšně byla navíšena nakoupená kapacita na dané kategorii a zároveň vytvořena položka objednávky pro danou kategorii
                                    break;
                                }
                                catch (DbUpdateConcurrencyException ex)
                                {
                                    //Je odstraněna nevalidní objednávka
                                    context.TicketCategoryOrders.Remove(ticketCategoryOrder);
                                    //Je znovu načtena daná kategorie s korektní sumou
                                    ex.Entries.Single().Reload();
                                    ticketCategory = (TicketCategory)ex.Entries.Single().Entity;
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            //Již překoupeno, případně pro danou kategorii nejsou objednány žádné vstupenky
                            break;
                        }

                    }
                }

                if (!overBought)
                {
                    TicketOrderViewModel ticketOrderViewModel = CreateReservration(submitedModel, context, transaction, project, ticketOrder, ticketCategoryOrderViewModels);

                    return RedirectToActionPermanent("OrderConfirmation", new RouteValueDictionary(ticketOrderViewModel));
                }
                else
                {
                    transaction.Rollback();
                    ModelState.AddModelError("ErrorSum", "Dané množství vstupenek není možné objednat");
                    return View(viewName, submitedModel);
                }
            }
        }

        private TicketOrderViewModel CreateReservration(ShowTicketCategoriesViewModel submitedModel, ApplicationDbContext context, DbContextTransaction transaction, Project project, TicketOrder ticketOrder, List<TicketCategoryOrderViewModel> ticketCategoryOrderViewModels)
        {
            Org org = context.Orgs.First(o => o.OrgID == project.OwnerID);

            //Vytvořit VS a uložit objednávku
            for (int i = 0; i < 20; i++)
            {
                ticketOrder.VariableSymbol = org.VariableSymbolCounter++;

                foreach (var ticketCategoryOrder in ticketOrder.TicketCategoryOrders)
                {
                    var ticketCategoryViewModel = submitedModel.TicketCategories.Single(tc => tc.TicketCategoryID == ticketCategoryOrder.TicketCategoryID);

                    for (int j = 0; j < ticketCategoryOrder.Count; j++)
                    {

                        TicketItem ticketItem = new TicketItem()
                        {
                            Code = ticketOrder.VariableSymbol.ToString() + "-" + (j + 1),
                            QRCode = Guid.NewGuid().ToString(),
                            TicketCategoryOrder = ticketCategoryOrder
                        };

                        if (project.TicketSetting.NamedTickets)
                        {
                            ticketItem.Name = ticketCategoryViewModel.Names[j].Name;
                            ticketItem.Email = ticketCategoryViewModel.Emails[j].Email;
                        }

                        context.TicketItems.Add(ticketItem);
                    }
                }

                try
                {
                    context.SaveChanges();
                    transaction.Commit();
                    //Úspěšně vytvořen VS pro danou objednávku a navýšeno počítadlo pro danou organizaci
                    break;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ex.Entries.Single().Reload();
                    org = (Org)ex.Entries.Single().Entity;
                    continue;
                }
            }

            context.Entry(project.BankProxy).Reference(bp => bp.FioBankProxy).Load();

            string accountNumber = project.BankProxy.FioBankProxy.AccountNumber + "/" + FioBankHelper.BANK_CODE;

            TicketOrderViewModel ticketOrderViewModel = new TicketOrderViewModel()
            {
                ProjectID = submitedModel.ProjectID,
                ProjectName = project.Name,
                Note = project.TicketSetting.NoteOrderCZ,
                AccountNumber = accountNumber,
                Email = ticketOrder.Email,
                ReservedUntil = ticketOrder.ReservedUntil.Value.ToShortDateString(),
                TotalPrice = ticketOrder.TotalPrice + " Kč",
                VariableSymbol = ticketOrder.VariableSymbol,
                CategoryOrders = ticketCategoryOrderViewModels
            };

            //SendEmail(ticketOrderViewModel);
            return ticketOrderViewModel;
        }

        public ActionResult OrderConfirmation(TicketOrderViewModel ticketOrderViewModel)
        {
            ticketOrderViewModel.CategoryOrders = new List<TicketCategoryOrderViewModel>();
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var ticketOrder = context.TicketOrders.Single(to => to.VariableSymbol == ticketOrderViewModel.VariableSymbol && to.ProjectID == ticketOrderViewModel.ProjectID);

                var project = context.Projects.Include("BankProxy.FioBankProxy").Single(p => p.ProjectID == ticketOrderViewModel.ProjectID);

                ticketOrderViewModel.IBAN = project.BankProxy.FioBankProxy.IBAN;
                ticketOrderViewModel.SWIFT = project.BankProxy.FioBankProxy.BIC;
                ticketOrderViewModel.MessageForPayee = "/VS/" + ticketOrderViewModel.VariableSymbol;

                var results = context.TicketCategoryOrders.Where(to => to.TicketOrderID == ticketOrder.TicketOrderID).Include(tco => tco.TicketCategory);
                foreach (var result in results)
                {
                    ticketOrderViewModel.CategoryOrders.Add(new TicketCategoryOrderViewModel() { Header = result.TicketCategory.HeaderCZ, Ordered = result.Count, TotalPrice = result.Count * result.TicketCategory.Price });
                }
            }

            SetLocalization(ticketOrderViewModel);
            SendEmail(ticketOrderViewModel);
            return View("~/Views/Tickets/TicketOrderLocalized.cshtml", ticketOrderViewModel);
        }

        private bool ExceededMaxTickets(Project project, ShowTicketCategoriesViewModel submitedModel, ApplicationDbContext context)
        {
            var orders = context.TicketOrders.Where(to => to.Email == submitedModel.Email);

            var ticketOrders = orders.Where(to => to.TicketCategoryOrders.Any(tco => tco.TicketCategory.ProjectID == project.ProjectID)).Select(to => to.TicketOrderID);

            int count = 0;

            foreach (int ticketOrderId in ticketOrders)
            {
                TicketOrder ticketOrder1 = context.TicketOrders.SingleOrDefault(to => to.TicketOrderID == ticketOrderId);
                count += ticketOrder1.TicketCategoryOrders.Sum(tco => tco.Count);
            }

            count += submitedModel.TicketCategories.Sum(tc => tc.Ordered);

            return count > project.TicketSetting.MaxTicketsPerEmail;
        }

        private void SendEmail(TicketOrderViewModel ticketOrderViewModel)
        {
            IUserMailer userMailer = new UserMailer();
            var email = userMailer.SendTicketOrder(ticketOrderViewModel);
            email.Send();
        }

        private void SetLocalization(ShowTicketCategoriesViewModel viewModel, int projectId, bool isCode)
        {
            //TODO: Set cookie, create button to set cookie/localization response bla bla           
            bool isEnglish = false;
            bool isJunkTown = projectId == 3;
            var cookie = Request.Cookies[LocalizationCookie];

            if (cookie != null)
            {
                if (cookie.Value == "en")
                {
                    isEnglish = true;
                }
            }

            TicketOrderLocalization ticketOrderLocalization = new TicketOrderLocalization();
            if (isEnglish)
            {
                if (isJunkTown)
                {
                    ticketOrderLocalization.FreeTicketsHdr = "Available capacity";
                    ticketOrderLocalization.RegistrationStartHdr = "Start of registration";
                    ticketOrderLocalization.EdnRegistrationHdr = "End of registration";
                    ticketOrderLocalization.TicketPriceHdr = "Registration fee";
                    ticketOrderLocalization.TicketCountHdr = "Count";
                    ticketOrderLocalization.RegisterBtn = "Register";
                    ticketOrderLocalization.NoFreeTicketsMsg = "There is no available capacity at the moment";
                    ticketOrderLocalization.TicketTypeBtn = isCode ? "Public registration" : "Private registration";
                }
                else
                {
                    ticketOrderLocalization.FreeTicketsHdr = "Available tickets";
                    ticketOrderLocalization.RegistrationStartHdr = "Start of presale";
                    ticketOrderLocalization.EdnRegistrationHdr = "End of presale";
                    ticketOrderLocalization.TicketPriceHdr = "Price";
                    ticketOrderLocalization.TicketCountHdr = "Count";
                    ticketOrderLocalization.RegisterBtn = "Order";
                    ticketOrderLocalization.NoFreeTicketsMsg = "There is no available tickets at the moment";
                    ticketOrderLocalization.TicketTypeBtn = isCode ? "Public tickets" : "Private tickets";
                }

                ticketOrderLocalization.ParticipantNameHdr = "Civic name";
                ticketOrderLocalization.ParticipantEmailHdr = "Email";
                ticketOrderLocalization.CategoryHdr = "Categories";
                ticketOrderLocalization.LanguageBtn = "Czech";
                ticketOrderLocalization.NameViewHdr1 = "For the successful completion of the registration organizer demands that you must fill in the civic names and emails of individual visitors.";
                ticketOrderLocalization.NameViewHdr2 = "These civic names will be checked on entry.";
                ticketOrderLocalization.TermsConditionHdr = "Please read terms and conditions and agree to them.";
                ticketOrderLocalization.TermsConditionAgree = "I agree with terms, conditions and processing of personal informations.";
                ticketOrderLocalization.ReservationCode = "Code";
                ticketOrderLocalization.TermsConditionWarning = "You have to agree with terms and conditions!";

            }
            else
            {
                if (isJunkTown)
                {
                    ticketOrderLocalization.FreeTicketsHdr = "Dostupná místa";
                    ticketOrderLocalization.RegistrationStartHdr = "Začátek registrace";
                    ticketOrderLocalization.EdnRegistrationHdr = "Konec registrace";
                    ticketOrderLocalization.TicketPriceHdr = "Celková výše stanového členského příspěvku";
                    ticketOrderLocalization.TicketCountHdr = "Počet";
                    ticketOrderLocalization.RegisterBtn = "Registrovat";
                    ticketOrderLocalization.TicketTypeBtn = isCode ? "Veřejná registrace" : "Soukromá registrace";
                }
                else
                {
                    ticketOrderLocalization.FreeTicketsHdr = "Dostupných vstupenek";
                    ticketOrderLocalization.RegistrationStartHdr = "Začátek prodeje";
                    ticketOrderLocalization.EdnRegistrationHdr = "Konec prodeje";
                    ticketOrderLocalization.TicketPriceHdr = "Cena";
                    ticketOrderLocalization.TicketCountHdr = "Počet vstupenek";
                    ticketOrderLocalization.RegisterBtn = "Objednat";
                    ticketOrderLocalization.TicketTypeBtn = isCode ? "Veřejné vstupenky" : "Soukromé vstupenky";
                }

                ticketOrderLocalization.ParticipantNameHdr = "Občanské jméno";
                ticketOrderLocalization.ParticipantEmailHdr = "Email";
                ticketOrderLocalization.CategoryHdr = "Kategorie";
                ticketOrderLocalization.NoFreeTicketsMsg = "Momentálně nejsou dostupná žádná volná místa.";
                ticketOrderLocalization.LanguageBtn = "English";
                ticketOrderLocalization.NameViewHdr1 = "Pro úspěšné dokončení registrace vyžaduje pořadatel vyplnění jednotlivých občanských jmen a emailů návštěvníků.";
                ticketOrderLocalization.NameViewHdr2 = "Uvedená občanská jména budou kontrolována na vstupu.";
                ticketOrderLocalization.TermsConditionHdr = "Seznamte se s obchodními podmínkami a odsouhlaste je.";
                ticketOrderLocalization.TermsConditionAgree = "Souhlasím s obchodními podmínkami a zpracováním osobních údajů.";
                ticketOrderLocalization.ReservationCode = "Kód";
                ticketOrderLocalization.TermsConditionWarning = "Musíte souhlasit s obchodními podmínkami!";
            }

            ticketOrderLocalization.ChangeLangLink = string.Format("http://{0}/tickets/ChangeLanguage/{1}", Request.Url.Authority, projectId);
            viewModel.Localization = ticketOrderLocalization;
        }

        private void SetLocalization(TicketOrderViewModel viewModel)
        {
            //TODO: Set cookie, create button to set cookie/localization response bla bla           
            bool isEnglish = false;
            bool isJunkTown = viewModel.ProjectID == 3;
            var cookie = Request.Cookies[LocalizationCookie];

            if (cookie != null)
            {
                if (cookie.Value == "en")
                {
                    isEnglish = true;
                }
            }

            TicketFinalOrderLocalization ticketFinalOrderLocalization = new TicketFinalOrderLocalization();
            if (isEnglish)
            {
                if (isJunkTown)
                {
                    ticketFinalOrderLocalization.FinalizedHdr = "Reservation completed";
                    ticketFinalOrderLocalization.OrderedTicketsHdr = "Reserved places on Junktown 2017 festival";
                    ticketFinalOrderLocalization.TicketCountHdr = "Reserved places";
                    ticketFinalOrderLocalization.TicketTotalPriceHdr = "Total membership fee";
                    ticketFinalOrderLocalization.ToYourEmail2 = " we've sent you a copy of reservation.";
                    ticketFinalOrderLocalization.PaymentInfo1 = "";
                    ticketFinalOrderLocalization.PaymentInfo1 = "";
                    ticketFinalOrderLocalization.PaymentInfo1 = "";
                    ticketFinalOrderLocalization.PaymentInfo1 = "";
                }
                else
                {
                    ticketFinalOrderLocalization.OrgNoteHdr = "Message from organizer";
                    ticketFinalOrderLocalization.MessageForRecipient = "Message for recipient";
                    ticketFinalOrderLocalization.InternationPaymentHdr = "Internation payment";
                    ticketFinalOrderLocalization.PaymentInformation = "Payment info";
                    ticketFinalOrderLocalization.FinalizedHdr = "Order completed";
                    ticketFinalOrderLocalization.OrderedTicketsHdr = "Ordered tickets";
                    ticketFinalOrderLocalization.TicketCountHdr = "Ordered tickets";
                    ticketFinalOrderLocalization.TicketTotalPriceHdr = "Total price";
                    ticketFinalOrderLocalization.ToYourEmail2 = " we've sent you a copy of order.";
                    ticketFinalOrderLocalization.PaymentInfo1 = "Payment for ordered tickets please make by bank transfer.";
                    ticketFinalOrderLocalization.PaymentInfo2 = "After we register your complete payment we'll send your ticket on your email address.";
                    ticketFinalOrderLocalization.PaymentInfo3 = "Due date is latest day of registering your payment on our account.";
                    ticketFinalOrderLocalization.PaymentInfo4 = "After due date this order will not be valid. Create new order for buing a ticket.";

                }
                ticketFinalOrderLocalization.ToYourEmail1 = "To your email:";
                ticketFinalOrderLocalization.CategoryHdr = "Category";
                ticketFinalOrderLocalization.AccountNumberHdr = "Bank account number: ";
                ticketFinalOrderLocalization.VSHeader = "Variable symbol:";
                ticketFinalOrderLocalization.KSHeader = "Constant symbol:";
                ticketFinalOrderLocalization.DueDateHdr = "Due date:";
            }
            else
            {
                if (isJunkTown)
                {
                    ticketFinalOrderLocalization.FinalizedHdr = "Rezervace dokončena";
                    ticketFinalOrderLocalization.OrderedTicketsHdr = "Rezervovaná místa na spolkovém festivalu Junktown 2017";
                    ticketFinalOrderLocalization.TicketCountHdr = "Rezervovaných míst";
                    ticketFinalOrderLocalization.TicketTotalPriceHdr = "Celková výše stanoveného členského příspěvku";
                    ticketFinalOrderLocalization.ToYourEmail2 = " Vám byla odeslána kopie vytvořené rezervace.";
                    ticketFinalOrderLocalization.PaymentInfo1 = "Po připsání celkové částky / příspěvku na účet pořadatele / spolku Vám bude obratem na tento Email odesláno potvrzení o úspěšné úhradě členského příspěvku, které vás na základě interních předpisů spolku opravňuje ke vstupu na příslušný ročník spolkového festivalu Junktown 2017.";
                    ticketFinalOrderLocalization.PaymentInfo2 = "Datum splatnosti je nejpozdější datum, kdy musí být připsán vámi uhrazený členský příspěvek na příslušný účet spolku, přičemž nestane-li se tak, propadne Vám tím zcela Vaše rezervace příslušných míst na spojkovém festivalu Junktown 2017.";
                    ticketFinalOrderLocalization.PaymentInfo3 = "ZÁKONNÉ POUČENÍ / PLATEBNÍ PODMÍNKY:";
                    ticketFinalOrderLocalization.PaymentInfo4 = "Projekt Junktown (dále  jen „Projekt“) a akce Junktown 2017 (dále jen „Akce“) je realizována pod záštitou Občanského sdružení Alternativa II, z.s. (dále jen „Spolku“) na spolkové neziskové a nespotřebitelské členské bázi dle stanov Spolku (dále jen „Stanov“), za účelem rekultivace objektu bývalé raketové základny Bratronice a naplňování vybraných oblastí spolkového poslání, kterým je podpora aktivního bytí, seberealizace, vědy, kultury, sportu, tělovýchovy, vzdělávání, občanské angažovanosti, volnočasového vyžití, jakož i všech ostatních aktivit směřujících k  všestrannému rozvoji jedince i společnosti. Veškeré platby ve prospěch projektu Junktown jsou přijímány Spolkem výhradně ve formě členských příspěvků (neboli zkráceně jen „příspěvků“) stanovovaných Spolkem na základě § 233 zákona č. 89/2012 Sb. (dále jen „NOZ“), přičemž upozorňujeme, že dobrovolná úhrada příspěvku stanoveného příslušným orgánem Spolku je dle spolkových Stanov považována současně i za projev vůle ve smyslu § 233 (2) NOZ, na základě čehož plátci stanoveného příspěvku vzniká nejpozději okamžikem přijetí příspěvku ze strany Spolku v souladu se Stanovami a interními předpisy bezzávazkový členský status „podporovatel“ Projektu a řadové neregistrované členství ve Spolku, které příslušného člena opravňuje ve Spolkem stanoveném rozsahu navštěvovat prostory Projektu, účastnit se Akcí Projektu a čerpat příslušné členské výhody a služby Projektu poskytované „podporovatelům“ Projektu  výhradně a  pouze  na  spolkové neziskové a nespotřebitelské bázi v rámci hlavní činnosti Spolku za účelem realizace spolkového poslání a efektivního  interního fundraisingu / crowdfundingu. Nesouhlasíte-li s uvedenými platebními podmínkami, neprovádějte ve prospěch Spolku jakoukoliv platbu, anebo požádejte neprodleně o vrácení platby, pokud jste tuto platbu provedl(a) v domnění, že se jedná o jiný právní akt za jiných platebních podmínek. Změnu členství  z neregistrovaného na  registrované lze provést na adrese www.osa2.cz/registrace. Stanovy, jakož i ostatní informace o Spolku jsou k dispozici ve spolkovém rejstříku u Městského soudu v Praze pod spisovou značkou L 16540.";
                }
                else
                {
                    ticketFinalOrderLocalization.OrgNoteHdr = "Poznámka pořadatele";
                    ticketFinalOrderLocalization.InternationPaymentHdr = "Mezinárodní platba";
                    ticketFinalOrderLocalization.MessageForRecipient = "Zpráva pro příjemce";
                    ticketFinalOrderLocalization.PaymentInformation = "Platební informace";
                    ticketFinalOrderLocalization.FinalizedHdr = "Objednávka dokončena";
                    ticketFinalOrderLocalization.OrderedTicketsHdr = "Objednané vstupenky";
                    ticketFinalOrderLocalization.TicketCountHdr = "Objednaných vstupenek";
                    ticketFinalOrderLocalization.TicketTotalPriceHdr = "Celková cena";
                    ticketFinalOrderLocalization.ToYourEmail2 = " Vám byla odeslána kopie vytvořené objednávky.";
                    ticketFinalOrderLocalization.PaymentInfo1 = "Platbu za objednané vstupenky prosím proveďte pomocí bankovního převodu.";
                    ticketFinalOrderLocalization.PaymentInfo2 = "Po připsání celkové částky na účet pořadatele Vám budou obratem na Váš Email odeslány vstupenky.";
                    ticketFinalOrderLocalization.PaymentInfo3 = "Datum splatnosti je nejpozdější datum připsání částky na účet.";
                    ticketFinalOrderLocalization.PaymentInfo4 = "Po datu splatnosti tato objednávka již nebude platná. Pro zakoupení vstupenek vytvořte novou objednávku.";
                }
                ticketFinalOrderLocalization.ToYourEmail1 = "Na Váš email:";
                ticketFinalOrderLocalization.CategoryHdr = "Kategorie";
                ticketFinalOrderLocalization.AccountNumberHdr = "Číslo účtu:";
                ticketFinalOrderLocalization.VSHeader = "Variabilní symbol:";
                ticketFinalOrderLocalization.KSHeader = "Konstantní symbol:";
                ticketFinalOrderLocalization.DueDateHdr = "Datum splatnosti:";
            }

            viewModel.Localization = ticketFinalOrderLocalization;
        }
    }
}