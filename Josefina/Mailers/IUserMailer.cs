using Mvc.Mailer;
using Josefina.Models.TicketsViewModel;
using Josefina.Helpers;
using System.Collections.Generic;
using Josefina.Entities;

namespace Josefina.Mailers
{
    public interface IUserMailer
    {
        MvcMailMessage Welcome();
        MvcMailMessage EmailConfirmation(int confirmationCode, string email);
        MvcMailMessage SendInvitation(string emailTo, string senderName, string senderEmail, string orgName, int invitationCode);
        MvcMailMessage SendPasswordReset(string emailTo, string code, string userID);
        MvcMailMessage SendTicketOrder(TicketOrderViewModel model);
        MvcMailMessage SendTicketOrderConfirmation(TicketItemConfirmationViewModel model, List<TicketPDFGenerator.TicketToGenerateWrapper> ticketsToExport, TicketSetting settings);
        MvcMailMessage SendTicketOrderCanceled(TicketOrderCanceledViewModel model);
    }
}