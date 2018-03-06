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
    [AllowAnonymous]
    [RoutePrefix("api/project/react")]
    public class ReactAppAPIController : ApiController
    {
        [HttpGet]
        [Route("GetTickets/{pass}")]
        [ResponseType(typeof(ReactAppTicketsViewModel))]
        public ReactAppTicketsViewModel GetTickets(string pass)
        {
            if(pass != "3deca158-9668-42d2-8619-1135e487685a")
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }            

            return GetTickets();
        }

        [HttpPost]
        [Route("sync/")]
        [ResponseType(typeof(AngularErrorViewModel))]
        public ReactAppTicketsViewModel SyncTickets(ReactAppTicketUploadViewModel model)
        {
            if (model.GUID != "3deca158-9668-42d2-8619-1135e487685a")
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            if (model.Tickets.Any() || model.Exports.Any())
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    foreach (var ticketId in model.Tickets)
                    {
                        var ticketItem = context.TicketItems.First(ti => ti.TicketItemID == ticketId);
                        ticketItem.Checked = true;
                    }

                    foreach (var ticketExportId in model.Exports)
                    {
                        var ticketExportItem = context.TicketExportItems.First(tei => tei.TicketExportItemID == ticketExportId);
                        ticketExportItem.Checked = true;
                    }

                    context.SaveChanges();
                } 
            }

            return GetTickets();
        }


        private ReactAppTicketsViewModel GetTickets()
        {
            ReactAppTicketsViewModel viewModel = new Models.ReactAppTicketsViewModel();

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                TicketOrder neco = new TicketOrder();
                //neco.TicketCategoryOrders.First()..TicketCategory
                var ticketOrders = context.TicketOrders.Where(tor => tor.Paid && (tor.ProjectID == 14))
                    .Include("TicketCategoryOrders.TicketItems")
                    .Include("TicketCategoryOrders.TicketCategory").ToList();

                foreach (var ticketOrder in ticketOrders)
                {
                    foreach (var ticketCategoryOrder in ticketOrder.TicketCategoryOrders)
                    {
                        foreach (var ticketItem in ticketCategoryOrder.TicketItems)
                        {
                            var ticket = new ReactAppTicketViewModel();
                            ticket.chck = ticketItem.Checked;
                            ticket.code = ticketItem.Code;
                            ticket.email = ticketOrder.Email;
                            ticket.id = ticketItem.TicketItemID;
                            ticket.name = ticketItem.Name;
                            ticket.qrCode = ticketItem.QRCode;
                            ticket.CtgID = ticketCategoryOrder.TicketCategoryID;

                            viewModel.Tickets.Add(ticket);
                        }
                    }
                }

                var ticketCategories = context.TicketCategories.Where(tc => tc.ProjectID == 14).ToList();

                foreach (var ticketCategory in ticketCategories)
                {
                    viewModel.Headers.Add(new ReactAppCategoryNameViewModel() { id = ticketCategory.TicketCategoryID, name = ticketCategory.HeaderCZ });
                }

                var ticketExports = context.TicketExports.Where(te => te.ProjectID == 14).Include("TicketExportItems").ToList();

                foreach (var ticketExport in ticketExports)
                {
                    foreach (var ticketExportItem in ticketExport.TicketExportItems)
                    {
                        if (ticketExportItem.Paid)
                        {
                            var ticket = new ReactAppTicketViewModel();
                            ticket.chck = ticketExportItem.Checked;
                            ticket.code = ticketExportItem.Code;
                            ticket.email = ticketExportItem.Email;
                            ticket.id = ticketExportItem.TicketExportItemID;
                            ticket.name = ticketExportItem.Name;
                            ticket.qrCode = ticketExportItem.QRCode;
                            ticket.CtgID = ticketExport.TicketExportID;

                            viewModel.TicketExports.Add(ticket);
                        }
                    }

                    viewModel.ExportHeaders.Add(new ReactAppCategoryNameViewModel() { id = ticketExport.TicketExportID, name = ticketExport.Name });
                }

            }
            return viewModel;
        }
    }
}