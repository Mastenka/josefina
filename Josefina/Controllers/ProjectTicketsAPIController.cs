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
using Josefina.Models.TicketsViewModel;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Microsoft.AspNet.SignalR;
using Josefina.ApiModels.Tickets;
using Josefina.Helpers;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
using Josefina.Mailers;

namespace Josefina.Controllers
{
    [System.Web.Http.Authorize]
    [RoutePrefix("api/project/tickets")]
    public class ProjectTicketsAPIController : ApiController
    {
        [HttpGet]
        [Route("GetCategoriesViewModel/{projectId:int}")]
        [ResponseType(typeof(CategoriesViewModel))]
        public CategoriesViewModel GetCategoriesViewModel(int projectId)
        {
            CategoriesViewModel viewModel = new CategoriesViewModel();
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    Project project = context.Projects.SingleOrDefault(p => p.ProjectID == projectId);

                    if (project != null)
                    {
                        viewModel.IsValid = true;
                        if (IsAuthorized(project, context))
                        {
                            viewModel.IsAuthorized = true;
                            viewModel.Title = "Vstupenky | " + project.Name;

                            List<CategoryGridRow> ticketGridRows = GetProjectCategoriesGridRows(project, context);
                            viewModel.Categories = ticketGridRows;
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
                }

                return viewModel;
            }
            catch (Exception e)
            {
                viewModel.IsValid = false;
                return viewModel;
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        [HttpGet]
        [Route("GetOrdersViewModel/{projectId:int}")]
        [ResponseType(typeof(OrdersViewModel))]
        public OrdersViewModel GetOrdersViewModel(int projectId)
        {
            OrdersViewModel viewModel = new OrdersViewModel();
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    Project project = context.Projects.SingleOrDefault(p => p.ProjectID == projectId);

                    if (project != null)
                    {
                        viewModel.IsValid = true;
                        if (IsAuthorized(project, context))
                        {
                            viewModel.IsAuthorized = true;
                            viewModel.Title = "Objednávky | " + project.Name;


                            viewModel.Orders = new List<OrderGridViewModel>();
                            var ticketOrders = context.TicketOrders.Where(to => to.ProjectID == project.ProjectID);

                            foreach (TicketOrder ticketOrder in ticketOrders)
                            {
                                var orderViewModel = new OrderGridViewModel();
                                orderViewModel.Email = ticketOrder.Email;
                                orderViewModel.Ordered = ticketOrder.Created;
                                orderViewModel.PaidDate = ticketOrder.PaidDate;
                                orderViewModel.TicketOrderID = ticketOrder.TicketOrderID;
                                if (ticketOrder.Paid)
                                {
                                    orderViewModel.State = "Zaplacena";
                                }
                                else if (ticketOrder.Canceled)
                                {
                                    orderViewModel.State = "Nezaplacena/Storno";
                                }
                                else
                                {
                                    orderViewModel.State = "Rezervace";
                                }
                                viewModel.Orders.Add(orderViewModel);
                            }

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
                }

                return viewModel;
            }
            catch (Exception e)
            {
                viewModel.IsValid = false;
                return viewModel;
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        [HttpGet]
        [Route("GetOrderViewModel/{orderID:int}")]
        [ResponseType(typeof(OrderViewModel))]
        public OrderViewModel GetOrderViewModel(int orderID)
        {
            OrderViewModel viewModel = new OrderViewModel();
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    TicketOrder ticketOrder = context.TicketOrders.FirstOrDefault(to => to.TicketOrderID == orderID);

                    if (ticketOrder == null)
                    {
                        viewModel.IsValid = false;
                        return viewModel;
                    }

                    Project project = context.Projects.SingleOrDefault(p => p.ProjectID == ticketOrder.ProjectID);
                    if (project != null)
                    {
                        viewModel.IsValid = true;
                        if (IsAuthorized(project, context))
                        {
                            viewModel.IsAuthorized = true;
                            viewModel.Title = "Objednávka | " + project.Name;
                            viewModel.Paid = ticketOrder.Paid;
                            viewModel.TotalPaid = ticketOrder.TotalPaidPrice;
                            viewModel.TotalPrice = ticketOrder.TotalPrice;
                            viewModel.Canceled = ticketOrder.Canceled;
                            viewModel.PaidDate = ticketOrder.PaidDate.HasValue ? ticketOrder.PaidDate.Value.ToShortDateString() : "";
                            viewModel.Created = ticketOrder.Created.Value.ToShortDateString();
                            viewModel.TicketOrderID = ticketOrder.TicketOrderID;
                            viewModel.Email = ticketOrder.Email;
                            viewModel.VariableSymbol = ticketOrder.VariableSymbol;
                            viewModel.TicketItems = new List<TicketGridItem>();

                            context.Entry(ticketOrder).Collection(to => to.TicketCategoryOrders).Load();

                            foreach (TicketCategoryOrder ticketCategoryOrder in ticketOrder.TicketCategoryOrders)
                            {
                                context.Entry(ticketCategoryOrder).Collection(tco => tco.TicketItems).Load();
                                context.Entry(ticketCategoryOrder).Reference(tco => tco.TicketCategory).Load();

                                foreach (TicketItem ticketItem in ticketCategoryOrder.TicketItems)
                                {
                                    TicketGridItem ticketGridItem = new TicketGridItem();
                                    ticketGridItem.Category = ticketCategoryOrder.TicketCategory.HeaderCZ;
                                    ticketGridItem.Code = ticketItem.Code;
                                    ticketGridItem.Email = ticketItem.Email;
                                    ticketGridItem.Name = ticketItem.Name;
                                    ticketGridItem.TicketItemID = ticketItem.TicketItemID;

                                    viewModel.TicketItems.Add(ticketGridItem);
                                }
                            }
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
                }

                return viewModel;
            }
            catch (Exception e)
            {
                viewModel.IsValid = false;
                return viewModel;
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("UpdateTicketOrder/")]
        [ResponseType(typeof(AngularViewModel))]
        public AngularViewModel UpdateTicketOrder(OrderViewModel model)
        {
            AngularViewModel viewModel = new AngularViewModel();
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    TicketOrder ticketOrder = context.TicketOrders.FirstOrDefault(to => to.TicketOrderID == model.TicketOrderID);

                    if (ticketOrder == null)
                    {
                        viewModel.IsValid = false;
                        return viewModel;
                    }

                    Project project = context.Projects.SingleOrDefault(p => p.ProjectID == ticketOrder.ProjectID);

                    if (project != null)
                    {
                        viewModel.IsValid = true;
                        if (IsAuthorized(project, context))
                        {
                            viewModel.IsAuthorized = true;
                            viewModel.Title = "Objednávka | " + project.Name;

                            ticketOrder.Email = model.Email;
                            ticketOrder.Paid = model.Paid;

                            context.Entry(ticketOrder).Collection(to => to.TicketCategoryOrders).Load();

                            foreach (TicketCategoryOrder ticketCategoryOrder in ticketOrder.TicketCategoryOrders)
                            {
                                context.Entry(ticketCategoryOrder).Collection(tco => tco.TicketItems).Load();
                                context.Entry(ticketCategoryOrder).Reference(tco => tco.TicketCategory).Load();

                                foreach (TicketItem ticketItem in ticketCategoryOrder.TicketItems)
                                {
                                    TicketGridItem ticketGridItem = model.TicketItems.SingleOrDefault(ti => ti.TicketItemID == ticketItem.TicketItemID);

                                    if (ticketGridItem == null)
                                    {
                                        viewModel.IsValid = false;
                                        return viewModel;
                                    }

                                    ticketItem.Name = ticketGridItem.Name;
                                    ticketItem.Email = ticketGridItem.Email;
                                }
                            }

                            context.SaveChanges();
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
                }

                return viewModel;
            }
            catch (Exception e)
            {
                viewModel.IsValid = false;
                return viewModel;
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        [HttpGet]
        [Route("ResendTickets/{orderID:int}")]
        [ResponseType(typeof(OrderViewModel))]
        public AngularErrorViewModel ResendTickets(int orderID)
        {
            AngularErrorViewModel viewModel = new AngularErrorViewModel();
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    TicketOrder ticketOrder = context.TicketOrders.FirstOrDefault(to => to.TicketOrderID == orderID);

                    if (ticketOrder == null)
                    {
                        viewModel.IsValid = false;
                        return viewModel;
                    }

                    Project project = context.Projects.SingleOrDefault(p => p.ProjectID == ticketOrder.ProjectID);
                    if (project != null)
                    {
                        viewModel.IsValid = true;
                        if (IsAuthorized(project, context))
                        {
                            viewModel.IsAuthorized = true;
                            viewModel.Title = "Objednávka | " + project.Name;

                            context.Entry(ticketOrder).Collection(to => to.TicketCategoryOrders).Load();

                            TicketItemsHelper.SendConfirmationEmail(ticketOrder, project, context);
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
                }

                return viewModel;
            }
            catch (Exception e)
            {
                viewModel.IsValid = false;
                return viewModel;
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        [HttpGet]
        [Route("GetExportViewModel/{exportID:int}")]
        [ResponseType(typeof(ExportViewModel))]
        public ExportViewModel GetExportViewModel(int exportID)
        {
            ExportViewModel viewModel = new ExportViewModel();
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    TicketExport ticketExport = context.TicketExports.FirstOrDefault(te => te.TicketExportID == exportID);

                    if (ticketExport == null)
                    {
                        viewModel.IsValid = false;
                        return viewModel;
                    }

                    Project project = context.Projects.SingleOrDefault(p => p.ProjectID == ticketExport.ProjectID);
                    if (project != null)
                    {
                        viewModel.IsValid = true;
                        if (IsAuthorized(project, context))
                        {
                            viewModel.IsAuthorized = true;
                            viewModel.Title = "Export | " + project.Name;

                            viewModel.Name = ticketExport.Name;
                            viewModel.Price = ticketExport.Price;
                            viewModel.Capacity = ticketExport.Capacity;
                            viewModel.TicketExportID = ticketExport.TicketExportID;

                            viewModel.TicketExportItems = new List<TicketExportGridItem>();

                            context.Entry(ticketExport).Collection(te => te.TicketExportItems).Load();

                            foreach (TicketExportItem ticketExportItem in ticketExport.TicketExportItems)
                            {

                                TicketExportGridItem ticketExportGridItem = new TicketExportGridItem()
                                {
                                    TicketExportItemID = ticketExportItem.TicketExportItemID,
                                    Code = ticketExportItem.Code,
                                    Name = ticketExportItem.Name,
                                    Email = ticketExportItem.Email,
                                    Paid = ticketExportItem.Paid,
                                    SendTicketEmail = false
                                };
                                viewModel.TicketExportItems.Add(ticketExportGridItem);
                            }
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
                }

                return viewModel;
            }
            catch (Exception e)
            {
                viewModel.IsValid = false;
                return viewModel;
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("UpdateTicketExport/")]
        [ResponseType(typeof(AngularErrorViewModel))]
        public AngularErrorViewModel UpdateTicketExport(ExportViewModel model)
        {
            AngularErrorViewModel viewModel = new AngularErrorViewModel();
            viewModel.Error = true;
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    TicketExport ticketExport = context.TicketExports.FirstOrDefault(te => te.TicketExportID == model.TicketExportID);

                    if (ticketExport == null)
                    {
                        viewModel.IsValid = false;
                        return viewModel;
                    }

                    Project project = context.Projects.SingleOrDefault(p => p.ProjectID == ticketExport.ProjectID);
                    if (project != null)
                    {
                        viewModel.IsValid = true;
                        if (IsAuthorized(project, context))
                        {
                            viewModel.IsAuthorized = true;
                            viewModel.Title = "Export | " + project.Name;

                            context.Entry(ticketExport).Collection(te => te.TicketExportItems).Load();

                            foreach (TicketExportItem ticketExportItem in ticketExport.TicketExportItems)
                            {

                                TicketExportGridItem ticketExportGridItem = model.TicketExportItems.Single(tei => tei.TicketExportItemID == ticketExportItem.TicketExportItemID);


                                ticketExportItem.Name = ticketExportGridItem.Name;
                                ticketExportItem.Email = ticketExportGridItem.Email;
                                ticketExportItem.Paid = ticketExportGridItem.Paid;

                                if (ticketExportGridItem.SendTicketEmail)
                                {
                                    TicketItemsHelper.SendConfirmationEmail(ticketExportItem, project);
                                }

                            }
                            context.SaveChanges();
                            viewModel.Error = false;
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
                }

                return viewModel;
            }
            catch (Exception e)
            {
                viewModel.Error = true;
                viewModel.ErrorMessage = "Změny se nepodařilo uložit. Zkontrolujte že každá vstupenka které chcete odeslat mají korektně vyplněný email.";

                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        [HttpGet]
        [Route("GetBankSettingsViewModel/{projectId:int}")]
        [ResponseType(typeof(TicketsBankSettingsViewModel))]
        public TicketsBankSettingsViewModel GetBankSettingsViewModel(int projectId)
        {
            TicketsBankSettingsViewModel viewModel = new TicketsBankSettingsViewModel();
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    Project project = context.Projects.SingleOrDefault(p => p.ProjectID == projectId);
                    if (project != null)
                    {
                        viewModel.IsValid = true;
                        if (IsAuthorized(project, context))
                        {
                            viewModel.IsAuthorized = true;
                            viewModel.Title = "Nastavení vstupenek | " + project.Name;

                            context.Entry(project).Reference(p => p.BankProxy).Load();
                            if (project.BankProxy != null)
                            {
                                context.Entry(project.BankProxy).Reference(p => p.FioBankProxy).Load();

                                if (project.BankProxy != null && project.BankProxy.FioBankProxy != null)
                                {
                                    viewModel.AccountNumber = project.BankProxy.FioBankProxy.AccountNumber;
                                    viewModel.Token = project.BankProxy.FioBankProxy.Token;
                                    viewModel.ProjectURL = project.TicketsURL;
                                    viewModel.BIC = project.BankProxy.FioBankProxy.BIC;
                                    viewModel.IBAN = project.BankProxy.FioBankProxy.IBAN;
                                }
                            }
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
                }

                return viewModel;
            }
            catch (Exception e)
            {
                viewModel.IsValid = false;
                return viewModel;
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        [HttpGet]
        [Route("GetTicketSettingsViewModel/{projectId:int}")]
        [ResponseType(typeof(TicketsTicketSettingsViewModel))]
        public TicketsTicketSettingsViewModel GetTicketSettingsViewModel(int projectId)
        {
            TicketsTicketSettingsViewModel viewModel = new TicketsTicketSettingsViewModel();
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    Project project = context.Projects.SingleOrDefault(p => p.ProjectID == projectId);
                    if (project != null)
                    {
                        viewModel.IsValid = true;
                        if (IsAuthorized(project, context))
                        {
                            viewModel.IsAuthorized = true;
                            viewModel.Title = "Nastavení vstupenek | " + project.Name;
                            viewModel.Logo = project.TicketSetting.Logo;
                            viewModel.MaxTicketsPerMail = project.TicketSetting.MaxTicketsPerEmail;
                            viewModel.NamedTickets = project.TicketSetting.NamedTickets;
                            viewModel.ProjectNameCZ = project.TicketSetting.ProjectViewNameCZ;
                            viewModel.ProjectNameEN = project.TicketSetting.ProjectViewNameEN;
                            viewModel.StartsCZ = project.TicketSetting.StartsCZ;
                            viewModel.StartsEN = project.TicketSetting.StartsEN;
                            viewModel.LocationCZ = project.TicketSetting.LocationCZ;
                            viewModel.LocationEN = project.TicketSetting.LocationEN;
                            viewModel.LogoURL = project.TicketSetting.LogoURL;
                            viewModel.NoteTicketCZ = project.TicketSetting.NoteCZ;
                            viewModel.NoteTicketEN = project.TicketSetting.NoteEN;
                            viewModel.NoteOrderCZ = project.TicketSetting.NoteOrderCZ;
                            viewModel.NoteOrderEN = project.TicketSetting.NoteOrderEN;
                            viewModel.ProjectID = projectId;
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
                }

                return viewModel;
            }
            catch (Exception e)
            {
                viewModel.IsValid = false;
                return viewModel;
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("SaveTicketsSettings/")]
        [ResponseType(typeof(AngularErrorViewModel))]
        public AngularErrorViewModel SaveTicketsSettings(TicketsTicketSettingsViewModel model)
        {
            AngularErrorViewModel viewModel = new AngularErrorViewModel();
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    Project project = context.Projects.SingleOrDefault(p => p.ProjectID == model.ProjectID);
                    if (project != null)
                    {
                        viewModel.IsValid = true;
                        if (IsAuthorized(project, context))
                        {
                            viewModel.IsAuthorized = true;
                            viewModel.Title = "Nastavení | " + project.Name;

                            project.TicketSetting.Logo = model.Logo;
                            project.TicketSetting.MaxTicketsPerEmail = model.MaxTicketsPerMail;
                            project.TicketSetting.NamedTickets = model.NamedTickets;
                            project.TicketSetting.ProjectViewNameCZ = model.ProjectNameCZ;
                            project.TicketSetting.ProjectViewNameEN = model.ProjectNameEN;
                            project.TicketSetting.StartsCZ = model.StartsCZ;
                            project.TicketSetting.StartsEN = model.StartsEN;
                            project.TicketSetting.LocationCZ = model.LocationCZ;
                            project.TicketSetting.LocationEN = model.LocationEN;
                            project.TicketSetting.NoteCZ = model.NoteTicketCZ;
                            project.TicketSetting.NoteEN = model.NoteTicketEN;
                            project.TicketSetting.NoteOrderCZ = model.NoteOrderCZ;
                            project.TicketSetting.NoteOrderEN = model.NoteOrderEN;

                            if (model.LogoURL != project.TicketSetting.LogoURL)
                            {
                                if (!string.IsNullOrEmpty(model.LogoURL))
                                {
                                    bool validLogo = false;
                                    try
                                    {
                                        using (WebClient webClient = new WebClient())
                                        {
                                            byte[] logoData = webClient.DownloadData(model.LogoURL);

                                            using (MemoryStream mem = new MemoryStream(logoData))
                                            {
                                                using (var logoImage = Image.FromStream(mem))
                                                {
                                                    validLogo = true;
                                                }
                                            }

                                            if (validLogo)
                                            {
                                                project.TicketSetting.Logo = logoData;
                                                project.TicketSetting.LogoURL = model.LogoURL;
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        viewModel.Error = true;
                                        viewModel.ErrorMessage = "Nepodařilo se načíst obrázek ze zadané URL adreasy, prosím zkontrolujte, že adresa je v platném tvaru. Například: \"http://i.imgur.com/fvfEkN8.jpg\"";
                                        return viewModel;
                                    }
                                }
                                else
                                {
                                    project.TicketSetting.Logo = null;
                                }
                            }
                            context.SaveChanges();

                            viewModel.Error = false;
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
                }

                return viewModel;
            }
            catch (Exception e)
            {
                viewModel.IsValid = false;
                return viewModel;
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("UpdateTransactions/")]
        [ResponseType(typeof(AngularErrorViewModel))]
        public AngularErrorViewModel UpdateTransactions(LoadTransactionsViewModel data)
        {
            AngularErrorViewModel viewModel = new AngularErrorViewModel();
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    Project project = context.Projects.SingleOrDefault(p => p.ProjectID == data.ProjectID);

                    if (project != null)
                    {
                        viewModel.IsValid = true;
                        if (IsAuthorized(project, context))
                        {
                            viewModel.IsAuthorized = true;
                            viewModel.Title = "Vstupenky | " + project.Name;

                            context.Entry(project).Reference(p => p.Owner).Load();

                            if (project.Owner.TransactionsLastUpdate.HasValue)
                            {
                                TimeSpan quarterHour = new TimeSpan(0, 5, 0);

                                if (DateTime.Now - project.Owner.TransactionsLastUpdate.Value < quarterHour)
                                {
                                    viewModel.Error = true;
                                    viewModel.ErrorMessage = "Získat data z bankovního účtu je možné pouze jednou za 5min. Nejdříve v " + (project.Owner.TransactionsLastUpdate.Value.Add(quarterHour)).ToShortTimeString();
                                    return viewModel;
                                }
                            }

                            FIOTransactionResult result;

                            if (data.FromSelected)
                            {
                                data.FromDate = data.FromDate.AddHours(1);
                                result = FIOTransactionLoader.GetFIOTransactions(project, context, data.FromDate);
                            }
                            else
                            {
                                result = FIOTransactionLoader.GetFIOTransactions(project, context);
                            }


                            viewModel.Error = result.Error;

                            if (!result.Error)
                            {
                                TicketItemsHelper.ProcessTransactions(result.Transactions, project, context);
                                context.SaveChanges();
                            }
                            else
                            {
                                viewModel.ErrorMessage = "Nepodařilo se načíst platby. Zkontroluj prosím platnost FIO API Tokenu. Případně zkuste opakovat akci později.";
                            }
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
                }

                return viewModel;
            }
            catch (Exception e)
            {
                viewModel.IsValid = false;
                return viewModel;
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("CreateUpdateCategory/")]
        [ResponseType(typeof(CategoriesViewModel))]
        public CategoriesViewModel CreateUpdateCategory(CreateCategoryModel data)
        {
            CategoriesViewModel viewModel = new CategoriesViewModel();
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    Project project = context.Projects.SingleOrDefault(p => p.ProjectID == data.ProjectID);

                    if (project != null)
                    {
                        viewModel.IsValid = true;
                        if (IsAuthorized(project, context))
                        {
                            viewModel.IsAuthorized = true;
                            viewModel.Title = "Vstupenky | " + project.Name;

                            if (data.IsNew)
                            {
                                TicketCategory newCategory = new TicketCategory()
                                {
                                    HeaderCZ = data.Name,
                                    Capacity = data.Capacity,
                                    Price = data.Price,
                                    SoldFrom = data.SoldFrom.AddHours(2).Date,
                                    SoldTo = data.SoldTo.AddHours(2).Date,
                                    Project = project,
                                    Deleted = false,
                                    CodeRequired = data.CodeRequired
                                };

                                if (newCategory.CodeRequired)
                                {
                                    newCategory.Code = data.Code;
                                }

                                context.TicketCategories.Add(newCategory);
                                context.SaveChanges();
                            }
                            else
                            {
                                TicketCategory ticketCategory = context.TicketCategories.SingleOrDefault(tc => tc.TicketCategoryID == data.CategoryID);

                                if (ticketCategory == null)
                                {
                                    viewModel.IsValid = false;
                                    return viewModel;
                                }

                                ticketCategory.HeaderCZ = data.Name;
                                ticketCategory.Price = data.Price;
                                ticketCategory.Capacity = data.Capacity;
                                ticketCategory.SoldFrom = data.SoldFrom.AddHours(2).Date;
                                ticketCategory.SoldTo = data.SoldTo.AddHours(2).Date;
                                ticketCategory.CodeRequired = data.CodeRequired;
                                if (ticketCategory.CodeRequired)
                                {
                                    ticketCategory.Code = data.Code.Trim();
                                }
                                context.SaveChanges();
                            }

                            List<CategoryGridRow> ticketGridRows = GetProjectCategoriesGridRows(project, context);
                            viewModel.Categories = ticketGridRows;

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
                }

                return viewModel;
            }
            catch (Exception e)
            {
                viewModel.IsValid = false;
                return viewModel;
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("CreateExport/")]
        [ResponseType(typeof(CategoriesViewModel))]
        public CategoriesViewModel CreateExport(CreateExportModel data)
        {
            CategoriesViewModel viewModel = new CategoriesViewModel();
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {

                    using (DbContextTransaction transaction = context.Database.BeginTransaction())
                    {

                        Project project = context.Projects.SingleOrDefault(p => p.ProjectID == data.ProjectID);

                        if (project != null)
                        {
                            viewModel.IsValid = true;
                            if (IsAuthorized(project, context))
                            {
                                viewModel.IsAuthorized = true;
                                viewModel.Title = "Vstupenky | " + project.Name;

                                TicketExport ticketExport = null;
                                for (int i = 0; i < 20; i++)
                                {
                                    Org org = context.Orgs.Single(o => o.OrgID == project.OwnerID);

                                    ticketExport = new TicketExport()
                                    {
                                        Name = data.Name,
                                        Price = data.Price,
                                        Capacity = data.Capacity,
                                        Project = project,
                                        VariableSymbol = project.Owner.VariableSymbolCounter++
                                    };

                                    context.TicketExports.Add(ticketExport);

                                    try
                                    {
                                        context.SaveChanges();
                                        break;
                                    }
                                    catch (DbUpdateConcurrencyException ex)
                                    {
                                        ex.Entries.Single().Reload();
                                        org = (Org)ex.Entries.Single().Entity;
                                        continue;
                                    }
                                }

                                for (int i = 0; i < ticketExport.Capacity; i++)
                                {
                                    var quid = Guid.NewGuid();

                                    TicketExportItem ticketExportItem = new TicketExportItem()
                                    {
                                        Code = ticketExport.VariableSymbol.ToString() + "-" + (i + 1),
                                        QRCode = quid.ToString(),
                                        Paid = false,
                                        TicketExport = ticketExport
                                    };

                                    context.TicketExportItems.Add(ticketExportItem);
                                }

                                context.SaveChanges();

                                List<CategoryGridRow> ticketGridRows = GetProjectCategoriesGridRows(project, context);
                                viewModel.Categories = ticketGridRows;

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

                        transaction.Commit();
                        return viewModel;
                    }
                }
            }
            catch (Exception e)
            {
                viewModel.IsValid = false;
                return viewModel;
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("DeleteCategory/")]
        [ResponseType(typeof(CategoriesViewModel))]
        public CategoriesViewModel DeleteCategory(DeleteCategoryModel data)
        {
            CategoriesViewModel viewModel = new CategoriesViewModel();
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    Project project = context.Projects.SingleOrDefault(p => p.ProjectID == data.ProjectID);

                    if (project != null)
                    {
                        viewModel.IsValid = true;
                        if (IsAuthorized(project, context))
                        {
                            viewModel.IsAuthorized = true;
                            viewModel.Title = "Vstupenky | " + project.Name;

                            TicketCategory ticketCategory = context.TicketCategories.SingleOrDefault(tc => tc.TicketCategoryID == data.CategoryID);


                            if (ticketCategory == null)
                            {
                                viewModel.IsValid = false;
                                return viewModel;
                            }

                            ticketCategory.Deleted = true;
                            context.SaveChanges();

                            List<CategoryGridRow> ticketGridRows = GetProjectCategoriesGridRows(project, context);
                            viewModel.Categories = ticketGridRows;

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
                }

                return viewModel;
            }
            catch (Exception e)
            {
                viewModel.IsValid = false;
                return viewModel;
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("createUpdateBankProxy/")]
        [ResponseType(typeof(BankProxyViewModel))]
        public BankProxyViewModel CreateUpdateBankProxy(CreateUpdateBankProxyData data)
        {
            BankProxyViewModel viewModel = new BankProxyViewModel();
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    Project project = context.Projects.SingleOrDefault(p => p.ProjectID == data.ProjectID);
                    if (project != null)
                    {
                        viewModel.IsValid = true;
                        if (IsAuthorized(project, context))
                        {
                            viewModel.IsAuthorized = true;
                            viewModel.Title = "Vstupenky | " + project.Name;

                            var result = FIOTransactionLoader.SetProjectBankInfoFromToken(project, context, data.Token);

                            if (result.Error)
                            {
                                viewModel.Error = true;
                                return viewModel;
                            }

                            if (data.IsNew)
                            {
                                FioBankProxy fioBankProxy = new FioBankProxy()
                                {
                                    Token = data.Token,
                                    AccountNumber = result.AccountNumber,
                                    BIC = result.BIC,
                                    IBAN = result.IBAN,
                                    LastUpdate = DateTime.Now
                                };

                                context.FioBankProxies.Add(fioBankProxy);

                                BankProxy bankProxy = new BankProxy()
                                {
                                    BankProxyType = EBankProxyType.FIO,
                                    FioBankProxy = fioBankProxy
                                };

                                context.BankProxies.Add(bankProxy);

                                project.BankProxy = bankProxy;
                                project.TicketsURL = Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, String.Empty) + "/vstupenky/" + project.ProjectID + "/" + project.Name.Replace(' ', '-').Replace('/', '-');

                                context.SaveChanges();
                            }
                            else
                            {
                                context.Entry(project.BankProxy).Reference(bp => bp.FioBankProxy).Load();

                                project.BankProxy.FioBankProxy.AccountNumber = result.AccountNumber;
                                project.BankProxy.FioBankProxy.IBAN = result.IBAN;
                                project.BankProxy.FioBankProxy.BIC = result.BIC;
                                project.BankProxy.FioBankProxy.LastUpdate = DateTime.Now;
                                project.BankProxy.FioBankProxy.Token = data.Token;

                                context.SaveChanges();
                            }

                            if (!viewModel.Error)
                            {
                                viewModel.FioBankProxyViewModel = new FioBankProxyViewModel(project.BankProxy.FioBankProxy);
                                viewModel.TicketsURL = project.TicketsURL;
                            }

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
                }

                return viewModel;
            }
            catch (Exception e)
            {
                viewModel.IsValid = false;
                return viewModel;
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("GetTickets/")]
        [ResponseType(typeof(TicketsViewModel))]
        public TicketsViewModel GetTickets(GetTicketsModel data)
        {
            TicketsViewModel viewModel = new TicketsViewModel();
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    Project project = context.Projects.SingleOrDefault(p => p.ProjectID == data.ProjectID);

                    if (project != null)
                    {
                        viewModel.IsValid = true;
                        if (IsAuthorized(project, context))
                        {
                            viewModel.IsAuthorized = true;
                            viewModel.Title = "Vstupenky | " + project.Name;

                            List<TicketGridRow> ticketGridRows = new List<TicketGridRow>();

                            if (data.ForCategory)
                            {
                                TicketCategory ticketCategory = context.TicketCategories.SingleOrDefault(tc => tc.TicketCategoryID == data.CategoryID);

                                if (ticketCategory == null)
                                {
                                    viewModel.IsValid = false;
                                    return viewModel;
                                }

                                ticketGridRows.AddRange(GetTicketRowsForCategory(ticketCategory, context));
                            }
                            else
                            {
                                foreach (TicketCategory ticketCategory in project.TicketCategories)
                                {
                                    ticketGridRows.AddRange(GetTicketRowsForCategory(ticketCategory, context));
                                }
                            }

                            viewModel.Tickets = ticketGridRows;
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
                }

                return viewModel;
            }
            catch (Exception e)
            {
                viewModel.IsValid = false;
                return viewModel;
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        [HttpGet]
        [Route("GetFinalTickets/{projectId:int}")]
        [ResponseType(typeof(FinalTicketsViewModel))]
        public FinalTicketsViewModel GetFinalTickets(int projectId)
        {
            FinalTicketsViewModel viewModel = new FinalTicketsViewModel();
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    Project project = context.Projects.SingleOrDefault(p => p.ProjectID == projectId);

                    if (project != null)
                    {
                        viewModel.IsValid = true;
                        if (IsAuthorized(project, context))
                        {
                            viewModel.IsAuthorized = true;
                            viewModel.Title = "Vstupenky | " + project.Name;

                            List<TicketFinalGridRow> ticketGridRows = new List<TicketFinalGridRow>();

                            context.Entry(project).Collection(p => p.TicketCategories).Load();

                            foreach (TicketCategory ticketCategory in project.TicketCategories)
                            {
                                context.Entry(ticketCategory).Collection(tc => tc.TicketCategoryOrders).Load();
                                foreach (TicketCategoryOrder ticketCategoryOrder in ticketCategory.TicketCategoryOrders.Where(tco => tco.Paid))
                                {
                                    context.Entry(ticketCategoryOrder).Reference(tco => tco.TicketOrder).Load();
                                    context.Entry(ticketCategoryOrder).Collection(tco => tco.TicketItems).Load();
                                    foreach (TicketItem ticketItem in ticketCategoryOrder.TicketItems)
                                    {
                                        TicketFinalGridRow ticketFinalGridRow = new TicketFinalGridRow()
                                        {
                                            Email = ticketCategoryOrder.TicketOrder.Email,
                                            Code = ticketItem.Code,
                                            Category = ticketCategory.HeaderCZ
                                        };

                                        ticketGridRows.Add(ticketFinalGridRow);
                                    }
                                }
                            }

                            viewModel.Tickets = ticketGridRows.OrderBy(tgr => tgr.Code).ToArray();
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
                }

                return viewModel;
            }
            catch (Exception e)
            {
                viewModel.IsValid = false;
                return viewModel;
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("CancelOverDueTickets/")]
        [ResponseType(typeof(string))]
        public string CancelOverDueTickets()
        {
            string errorTracer = "Start " + DateTime.Now.ToLongTimeString() + " | ";
            try
            {

                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    var ticketOrdersToCancel = context.TicketOrders.Where(to => !to.Paid && !to.Canceled && to.ReservedUntil < DateTime.Now);

                    var projectIDs = ticketOrdersToCancel.Select(tco => tco.ProjectID).ToList();
                    projectIDs = projectIDs.Distinct().ToList();

                    errorTracer += "ProjectIds:  | " + string.Join(", ", projectIDs);

                    List<string> updatedTokens = new List<string>();

                    foreach (var projectId in projectIDs)
                    {
                        Project project = context.Projects.SingleOrDefault(p => p.ProjectID == projectId);

                        if (project == null)
                        {
                            context.TicketOrders.RemoveRange(ticketOrdersToCancel.Where(to => to.ProjectID == projectId));
                            continue;
                        }

                        context.Entry(project).Reference(p => p.BankProxy).Load();
                        context.Entry(project.BankProxy).Reference(bp => bp.FioBankProxy).Load();

                        if (!updatedTokens.Contains(project.BankProxy.FioBankProxy.Token))
                        {
                            errorTracer += " Token:  | " + project.BankProxy.FioBankProxy.Token;

                            updatedTokens.Add(project.BankProxy.FioBankProxy.Token);

                            var result = FIOTransactionLoader.GetFIOTransactions(project, context, DateTime.Now.AddDays(-1));
                            project.BankProxy.FioBankProxy.LastUpdate = DateTime.Now;

                            if (result.Error)
                            {
                                errorTracer += " Error occured during loading of transactions. ProjectId: " + projectId;
                            }
                            else if (result.Transactions == null || !result.Transactions.Any())
                            {
                                errorTracer += " No transactions was loaded. ProjectId: " + projectId;
                            }
                            else
                            {
                                errorTracer += " Result | " + result.Error.ToString() + " | " + string.Join(", ", result.Transactions.Select(t => (t.TransactionID + "-" + t.VariableSymbol.ToString())));
                                TicketItemsHelper.ProcessTransactions(result.Transactions, project, context);
                            }
                        }
                    }
                    errorTracer += " SaveChanges | ";
                    context.SaveChanges();

                    List<TicketOrder> ticketOrdersToCancel1 = context.TicketOrders.Where(to => !to.Paid && !to.Canceled && to.ReservedUntil < DateTime.Now).ToList();

                    errorTracer += "CancelIds:  | " + string.Join(", ", ticketOrdersToCancel1.Select(to => (to.VariableSymbol.ToString() + " - " + to.TicketOrderID)));

                    foreach (TicketOrder ticketOrderToCancel in ticketOrdersToCancel1)
                    {
                        TicketItemsHelper.CancelOrder(ticketOrderToCancel, context);
                    }
                    errorTracer += " End " + DateTime.Now.ToLongTimeString();
                }

                UserMailer userMailer = new UserMailer();
                var email = userMailer.GetSimpleEmail("bittmann.stefan@gmail.com", errorTracer, "CancelOrders");
                email.Send();

                return ""; //Dummy, because of HTTP Response
            }
            catch (Exception e)
            {

                UserMailer userMailer = new UserMailer();
                errorTracer += " |||||| Error: " + e.Message + e.StackTrace + e.InnerException != null ? e.InnerException.Message : " without inner ";

                var email = userMailer.GetSimpleEmail("bittmann.stefan@gmail.com", errorTracer, "CancelOrders");
                email.Send();

                System.Diagnostics.Debug.WriteLine(e.Message);
                throw;
            }
        }

        #region Helpers

        private List<TicketGridRow> GetTicketRowsForCategory(TicketCategory ticketCategory, ApplicationDbContext context)
        {
            List<TicketGridRow> ticketGridRows = new List<TicketGridRow>();
            context.Entry(ticketCategory).Collection(tc => tc.TicketCategoryOrders).Load();
            foreach (TicketCategoryOrder ticketCategoryOrder in ticketCategory.TicketCategoryOrders)
            {
                //context.Entry(ticketCategoryOrder).Collection(tco => tco.TicketItems).Load();
                context.Entry(ticketCategoryOrder).Reference(tco => tco.TicketOrder).Load();

                TicketGridRow ticketGridRow = new TicketGridRow();
                ticketGridRow.Email = ticketCategoryOrder.TicketOrder.Email;
                ticketGridRow.Reserved = ticketCategoryOrder.TicketOrder.Created.Value;

                if (ticketCategoryOrder.Paid)
                {
                    ticketGridRow.Paid = ticketCategoryOrder.TicketOrder.PaidDate;
                }
                ticketGridRow.Category = !ticketCategory.Deleted ? ticketCategory.HeaderCZ : ticketCategory.HeaderCZ + " (Zrušeno)";
                ticketGridRow.Count = ticketCategoryOrder.Count;
                ticketGridRows.Add(ticketGridRow);
            }

            return ticketGridRows;
        }

        private List<CategoryGridRow> GetProjectCategoriesGridRows(Project project, ApplicationDbContext context)
        {
            context.Entry(project).Collection(p => p.TicketCategories).Load();
            context.Entry(project).Collection(p => p.TicketExports).Load();

            List<CategoryGridRow> ticketGridRows = new List<CategoryGridRow>();

            foreach (TicketExport ticketExport in project.TicketExports)
            {
                CategoryGridRow ticketGridRow = new CategoryGridRow()
                {
                    TicketCategoryID = ticketExport.TicketExportID,
                    Capacity = ticketExport.Capacity,
                    TicketPrice = ticketExport.Price,
                    Name = ticketExport.Name,
                    IsCategory = false
                };
                context.Entry(ticketExport).Collection(te => te.TicketExportItems).Load();

                ticketGridRow.Paid = ticketExport.TicketExportItems.Where(tco => tco.Paid).Count();
                ticketGridRow.Unpaid = ticketExport.TicketExportItems.Where(tco => !tco.Paid).Count();

                ticketGridRow.PaidTotal = ticketGridRow.Paid * ticketGridRow.TicketPrice;
                ticketGridRows.Add(ticketGridRow);
            }


            foreach (TicketCategory ticketCategory in project.TicketCategories.Where(tc => !tc.Deleted))
            {
                CategoryGridRow ticketGridRow = new CategoryGridRow()
                {
                    TicketCategoryID = ticketCategory.TicketCategoryID,
                    Capacity = ticketCategory.Capacity,
                    SoldFrom = ticketCategory.SoldFrom.Value,
                    SoldTo = ticketCategory.SoldTo.Value,
                    TicketPrice = ticketCategory.Price,
                    Name = ticketCategory.HeaderCZ,
                    CodeRequired = ticketCategory.CodeRequired,
                    Code = ticketCategory.Code,
                    IsCategory = true
                };

                context.Entry(ticketCategory).Collection(tc => tc.TicketCategoryOrders).Load();

                ticketGridRow.Paid = ticketCategory.TicketCategoryOrders.Where(tco => tco.Paid && !tco.Canceled).Sum(tco => tco.Count);
                ticketGridRow.Unpaid = ticketCategory.TicketCategoryOrders.Where(tco => !tco.Paid && !tco.Canceled).Sum(tco => tco.Count);

                ticketGridRow.PaidTotal = ticketGridRow.Paid * ticketGridRow.TicketPrice;
                ticketGridRows.Add(ticketGridRow);
            }

            return ticketGridRows;
        }

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

        private bool IsAuthorized(Josefina.Entities.Task task, ApplicationDbContext context)
        {
            ApplicationUser user = GetUser(context);

            return user.OrgID == task.OrgID;
        }
        #endregion
    }
}