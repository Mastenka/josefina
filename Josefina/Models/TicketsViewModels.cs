using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Josefina.Models.TicketsViewModel
{
    public class ShowTicketCategoriesCodeViewModel : ShowTicketCategoriesViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.GeneralResources), ErrorMessageResourceName = "ErrorRequiredField")]
        [Display(Name = "Code")]
        public string Code { get; set; }
    }

    public class ShowTicketCategoriesViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.GeneralResources), ErrorMessageResourceName = "ErrorRequiredField")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.AccountResources), ErrorMessageResourceName = "InvalidEmailError", ErrorMessage = null)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public int ProjectID { get; set; }

        public bool TicketsAvailable { get; set; }

        public string ProjectName { get; set; }

        public bool AfterNameSetting { get; set; }

        public bool AfterTermsConditionsSetting { get; set; }

        public TicketOrderLocalization Localization { get; set; }

        public List<TicketCategoryViewModel> TicketCategories { get; set; }

        public string TermsConditions { get; set; }

        public int MaxTicketsPerMail { get; set; }
    }

    public class TicketOrderLocalization
    {
        public string TicketTypeBtn { get; set; }

        public string CategoryHdr { get; set; }

        public string FreeTicketsHdr { get; set; }

        public string RegistrationStartHdr { get; set; }

        public string EdnRegistrationHdr { get; set; }

        public string TicketPriceHdr { get; set; }

        public string TicketCountHdr { get; set; }

        public string RegisterBtn { get; set; }

        public string NoFreeTicketsMsg { get; set; }

        public string Language { get; set; }

        public string LanguageBtn { get; set; }

        public string ChangeLangLink { get; set; }

        public string ParticipantNameHdr { get; set; }

        public string ParticipantEmailHdr { get; set; }

        public string NameViewHdr1 { get; set; }

        public string NameViewHdr2 { get; set; }
        public string OrgNoteHdr { get; internal set; }

        public string TermsConditionHdr { get; set; }

        public string TermsConditionAgree { get; set; }

        public string ReservationCode { get; set; }

        public string TermsConditionWarning { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class MustBeTrueAttribute : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            return value != null && value is bool && (bool)value;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessage,
                ValidationType = "mustbetrue"
            };
        }
    }

    public class TicketCategoryViewModel
    {
        public int TicketCategoryID { get; set; }

        public string Header { get; set; }

        public string Price { get; set; }

        public int Capacity { get; set; }

        public int AvailableCapacity { get; set; }

        public string SoldFrom { get; set; }

        public string SoldTo { get; set; }

        public byte[] RowVersion { get; set; }

        [RegularExpression("([0-9]+)", ErrorMessage = "Zadejte prosím kladné číslo")]
        [Display(Name = "Počet vstupenek")]
        [Required(ErrorMessageResourceType = typeof(Resources.GeneralResources), ErrorMessageResourceName = "ErrorRequiredField")]
        [Range(0, int.MaxValue, ErrorMessage = "Zadejte prosím kladné číslo")]
        public int Ordered { get; set; }

        public TicketName[] Names { get; set; }

        public TicketEmail[] Emails { get; set; }

        [MustBeTrue(ErrorMessage = "Musíte souhlasit s obchodními podmínkami!")]
        public bool TermsConditionsAccepted { get; set; }
    }

    public class TicketName
    {
        [Required(ErrorMessageResourceType = typeof(Resources.GeneralResources), ErrorMessageResourceName = "ErrorRequiredField")]
        public string Name { get; set; }
    }

    public class TicketEmail
    {
        [Required(ErrorMessageResourceType = typeof(Resources.GeneralResources), ErrorMessageResourceName = "ErrorRequiredField")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.AccountResources), ErrorMessageResourceName = "InvalidEmailError", ErrorMessage = null)]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class TicketCategoryOrderViewModel
    {
        public string Header { get; set; }

        public int Ordered { get; set; }

        public decimal TotalPrice { get; set; }
    }

    public class TicketOrderViewModel
    {
        public int ProjectID { get; set; } //HACK: Junktown

        public string ProjectName { get; set; }

        public string AccountNumber { get; set; }

        public string Email { get; set; }

        public string ReservedUntil { get; set; }

        public string TotalPrice { get; set; }

        public long VariableSymbol { get; set; }

        public List<TicketCategoryOrderViewModel> CategoryOrders { get; set; }

        public string Note { get; set; }

        public string IBAN { get; set; }

        public string SWIFT { get; set; }

        public string MessageForPayee { get; set; }

        public TicketFinalOrderLocalization Localization { get; set; }
    }

    public class TicketFinalOrderLocalization
    {
        public string FinalizedHdr { get; set; }

        public string OrderedTicketsHdr { get; set; }

        public string CategoryHdr { get; set; }

        public string TicketCountHdr { get; set; }

        public string TicketTotalPriceHdr { get; set; }

        public string AccountNumberHdr { get; set; }

        public string VSHeader { get; set; }

        public string KSHeader { get; set; }

        public string DueDateHdr { get; set; }

        public string ToYourEmail1 { get; set; }

        public string ToYourEmail2 { get; set; }
        public string PaymentInformation { get; internal set; }
        public string InternationPaymentHdr { get; internal set; }
        public string MessageForRecipient { get; internal set; }
        public string OrgNoteHdr { get; internal set; }

        public string PaymentInfo1 { get; set; }

        public string PaymentInfo2 { get; set; }

        public string PaymentInfo3 { get; set; }

        public string PaymentInfo4 { get; set; }
    }

    public class TicketItemConfirmationViewModel
    {
        public string ProjectName { get; set; }

        public string DateStart { get; set; }

        public string Email { get; set; }

        public string Location { get; set; }

        public long VariableSymbol { get; set; }

        public bool IsExport { get; set; }

        public string Note { get; set; }
    }

    public class TicketOrderCanceledViewModel
    {
        public string ProjectName { get; set; }

        public string ReserverdUntil { get; set; }

        public string Email { get; set; }

        public long VariableSymbol { get; set; }
    }
}
