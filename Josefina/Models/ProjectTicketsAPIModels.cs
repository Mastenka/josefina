using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Josefina.Models;
using Josefina.Entities;

namespace Josefina.ApiModels.Tickets
{
    /// <summary>
    /// Defines basic atributes for Angular View model data classes
    /// </summary>
    public class AngularViewModel
    {
        public bool IsValid { get; set; }
        public bool IsAuthorized { get; set; }
        public string Title { get; set; }
    }

    public class CategoryGridRow
    {
        public string Name { get; set; }
        public int TicketCategoryID { get; set; }
        public int Paid { get; set; }
        public int Capacity { get; set; }
        public DateTime SoldFrom { get; set; }
        public DateTime SoldTo { get; set; }
        public decimal TicketPrice { get; set; }
        public decimal PaidTotal { get; set; }
        public int Unpaid { get; set; }
        public bool IsCategory { get; set; }
        public bool CodeRequired { get; set; }
        public string Code { get; set; }
    }

    public class TicketGridRow
    {
        public string Email { get; set; }
        public DateTime? Paid { get; set; }
        public DateTime Reserved { get; set; }
        public string Category { get; set; }
        public int Count { get; set; }
    }

    //
    public class CreateCategoryModel
    {
        public int ProjectID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public DateTime SoldFrom { get; set; }
        public DateTime SoldTo { get; set; }
        public bool IsNew { get; set; }
        public int CategoryID { get; set; }
        public bool CodeRequired { get; set; }
        public string Code { get; set; }
    }

    public class CreateExportModel
    {
        public int ProjectID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
    }

    public class DeleteCategoryModel
    {
        public int ProjectID { get; set; }
        public int CategoryID { get; set; }
    }

    public class GetTicketsModel
    {
        public int ProjectID
        {
            get;
            set;
        }
        public int CategoryID { get; set; }
        public bool ForCategory { get; set; }
    }

    public class TicketsViewModel : AngularViewModel
    {
        public List<TicketGridRow> Tickets { get; set; }
    }

    public class TicketsBankSettingsViewModel : AngularViewModel
    {
        public string Token { get; set; }

        public long? AccountNumber { get; set; }

        public string ProjectURL { get; set; }

        public string IBAN { get; set; }

        public string BIC { get; set; }
    }

    public class TicketsTicketSettingsViewModel : AngularViewModel
    {
        public int ProjectID { get; set; }

        public string ProjectNameCZ { get; set; }

        public string ProjectNameEN { get; set; }

        public string StartsCZ { get; set; }

        public string StartsEN { get; set; }

        public byte[] Logo { get; set; }

        public bool NamedTickets { get; set; }

        public int MaxTicketsPerMail { get; set; }

        public string LocationCZ { get; set; }

        public string LocationEN { get; set; }

        public string LogoURL { get; set; }

        public string NoteTicketCZ { get; set; }

        public string NoteTicketEN { get; set; }

        public string NoteOrderCZ { get; set; }

        public string NoteOrderEN { get; set; }
    }

    public class CreateUpdateBankProxyData
    {
        public int ProjectID { get; set; }
        public string Token { get; set; }
        public bool IsNew { get; set; }
        public int FioBankProxyID { get; set; }
    }

    public class FioBankProxyViewModel
    {
        public FioBankProxyViewModel(FioBankProxy fioBankProxy)
        {
            this.AccountNumber = fioBankProxy.AccountNumber;
            this.FioBankProxyID = fioBankProxy.FioBankProxyID;
            this.Token = fioBankProxy.Token;
            this.LastUpdate = fioBankProxy.LastUpdate;
            this.BIC = fioBankProxy.BIC;
            this.IBAN = fioBankProxy.IBAN;            
        }
        public string BIC { get; set; }
        public string IBAN { get; set; }
        public long? AccountNumber { get; set; }
        public string Token { get; set; }
        public DateTime? LastUpdate { get; set; }
        public int FioBankProxyID { get; set; }
    }

    public class BankProxyViewModel : AngularViewModel
    {
        public bool Error { get; set; }
        public FioBankProxyViewModel FioBankProxyViewModel { get; set; }
        public string TicketsURL { get; set; }
    }

    public class CategoriesViewModel : AngularViewModel
    {
        public List<CategoryGridRow> Categories { get; set; }
    }

    public class OrdersViewModel : AngularViewModel
    {
        public List<OrderGridViewModel> Orders { get; set; }
    }

    public class OrderGridViewModel
    {
        public int TicketOrderID { get; set; }

        public string Email { get; set; }

        public DateTime? PaidDate { get; set; }

        public DateTime? Ordered { get; set; }

        public string State { get; set; }
    }

    public class OrderViewModel : AngularViewModel
    {
        public int TicketOrderID { get; set; }

        public string Email { get; set; }

        public string PaidDate { get; set; }

        public string Created { get; set; }

        public bool Canceled { get; set; }

        public bool Paid { get; set; }

        public decimal TotalPaid { get; set; }

        public decimal TotalPrice { get; set; }

        public long VariableSymbol { get; set; }

        public List<TicketGridItem> TicketItems { get; set; }        
    }  

    public class TicketGridItem
    {
        public int TicketItemID { get; set; }

        public string Code { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }
    }

    public class ExportViewModel : AngularViewModel
    {
        public int TicketExportID { get; set; }

        public int Capacity { get; set; }

        public decimal Price { get; set; }

        public string Name { get; set; }

        public List<TicketExportGridItem> TicketExportItems { get; set; }
    }

    public class TicketExportGridItem
    {
        public int TicketExportItemID { get; set; }

        public string Code { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public bool Paid { get; set; }

        public bool SendTicketEmail { get; set; }
    }

    public class AngularErrorViewModel : AngularViewModel
    {
        public bool Error { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class TicketFinalGridRow
    {
        public string Email { get; set; }

        public string Category { get; set; }

        public string Code { get; set; }
    }

    public class FinalTicketsViewModel : AngularViewModel
    {
        public TicketFinalGridRow[] Tickets { get; set; }
    }

    public class LoadTransactionsViewModel
    {
        public DateTime FromDate { get; set; }

        public bool FromSelected { get; set; }

        public int ProjectID { get; set; }
    }
}