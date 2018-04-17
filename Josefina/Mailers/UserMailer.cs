using Mvc.Mailer;
using PreMailer.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Resources;
using System.Net.Mail;
using Josefina.Helpers;
using Josefina.Entities;

namespace Josefina.Mailers
{
    public class UserMailer : MailerBase, IUserMailer
    {
        public UserMailer()
        {
            MasterName = "~/Views/UserMailer/_Layout.cshtml";
        }

        public virtual MvcMailMessage Welcome()
        {
            var email = Populate(x =>
            {
                x.Subject = AccountResources.EmailConfirmation;
                x.ViewName = "Welcome";
                x.To.Add("bittmann.stefan@centrum.cz");
                x.To.Add("bittmann.stefan@gmail.com");
            });

            email.Body = PreMailer.Net.PreMailer.MoveCssInline(email.Body).Html;

            email.IsBodyHtml = true;
            email.BodyEncoding = System.Text.Encoding.UTF8;
            return email;
        }

        public MvcMailMessage EmailConfirmation(int confirmationCode, string email)
        {
            ViewBag.ConfirmationID = confirmationCode;
            var emailMsg = Populate(x =>
            {
                x.Subject = AccountResources.EmailConfirmation;
                x.ViewName = "EmailConfirmation";
                x.To.Add(email);
            });
            emailMsg.IsBodyHtml = true;
            emailMsg.BodyEncoding = System.Text.Encoding.UTF8;
            return emailMsg;
        }

        public MvcMailMessage GetSimpleEmail(string email, string simpleMessage, string subject)
        {
            ViewBag.SimpleMessage = simpleMessage;
            var emailMsg = Populate(x =>
            {
                x.Subject = subject;
                x.ViewName = "EmailSimpleMessage";
                x.To.Add(email);
            });
            emailMsg.IsBodyHtml = true;
            emailMsg.BodyEncoding = System.Text.Encoding.UTF8;
            return emailMsg;
        }

        public MvcMailMessage SendInvitation(string emailTo, string senderName, string senderEmail, string orgName, int invitationCode)
        {
            ViewBag.SenderName = senderName;
            ViewBag.SenderEmail = senderEmail;
            ViewBag.OrgName = orgName;
            ViewBag.InvitationCode = invitationCode;

            var emailMsg = Populate(x =>
            {
                x.Subject = OrgResources.InvitationTitle;
                x.ViewName = "SendInvitation";
                x.To.Add(emailTo);
            });

            return emailMsg;
        }

        public MvcMailMessage SendPasswordReset(string emailTo, string code, string userID)
        {
            ViewBag.UserID = userID;
            ViewBag.Code = code;

            var emailMsg = Populate(x =>
            {
                x.Subject = AccountResources.PasswordRecovery;
                x.ViewName = "ForgotPassword";
                x.To.Add(emailTo);
            });

            return emailMsg;
        }

        public MvcMailMessage SendTicketOrder(Models.TicketsViewModel.TicketOrderViewModel model)
        {
            ViewData = new ViewDataDictionary(model);

            var emailMsg = Populate(x =>
            {
                x.Subject = "Objednávka vstupenek č." + model.VariableSymbol;
                x.ViewName = "TicketOrder";
                x.To.Add(model.Email.Trim());
            });

            return emailMsg;
        }

        public MvcMailMessage SendTicketOrderConfirmation(Models.TicketsViewModel.TicketItemConfirmationViewModel model, List<TicketPDFGenerator.TicketToGenerateWrapper> ticketsToExport, TicketSetting settings)
        {
            ViewData = new ViewDataDictionary(model);
            TicketPDFGenerator.GetPDFTicket(ticketsToExport, settings);

            string subject = "Vstupenky: " + model.ProjectName;

            var emailMsg = Populate(x =>
            {
                x.Subject = subject;
                x.ViewName = "TicketOrderConfirmation";
                x.To.Add(model.Email);
                foreach (var ticketToExport in ticketsToExport)
                {
                    MemoryStream memoryStream = new MemoryStream(ticketToExport.PDFTicket);
                    Attachment attachement = new Attachment(memoryStream, System.Net.Mime.MediaTypeNames.Application.Pdf);
                    attachement.ContentDisposition.FileName = string.Format("{0} | Vstupenka: {1}.pdf", ticketToExport.ProjectName, ticketToExport.Code);
                    x.Attachments.Add(attachement);
                }
            });

            return emailMsg;
        }

        public MvcMailMessage SendTicketOrderCanceled(Models.TicketsViewModel.TicketOrderCanceledViewModel model)
        {
            ViewData = new ViewDataDictionary(model);
            var emailMsg = Populate(x =>
            {
                x.Subject = "Zrušení objednávky vstupenek č." + model.VariableSymbol;
                x.ViewName = "TicketOrderCanceled";
                x.To.Add(model.Email);
            });

            return emailMsg;
        }
    }
}