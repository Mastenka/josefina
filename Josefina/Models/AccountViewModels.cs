using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;

namespace Josefina.Models.AccountViewModel
{
  public class ExternalLoginConfirmationViewModel
  {
    [Required(ErrorMessageResourceType = typeof(Resources.GeneralResources), ErrorMessageResourceName = "ErrorRequiredField")]
    [EmailAddress(ErrorMessageResourceType = typeof(Resources.AccountResources), ErrorMessageResourceName = "InvalidEmailError", ErrorMessage = null)]
    [Display(Name = "Email")]
    public string Email { get; set; }
  }

  public class ExternalLoginListViewModel
  {
    public string Action { get; set; }
    public string ReturnUrl { get; set; }
  }

  public class ManageUserViewModel
  {
    [Required(ErrorMessageResourceType = typeof(Resources.GeneralResources), ErrorMessageResourceName = "ErrorRequiredField")]
    [DataType(DataType.Password)]
    [Display(Name = "CurrentPassword", ResourceType = typeof(Resources.AccountResources))]
    public string OldPassword { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources.GeneralResources), ErrorMessageResourceName = "ErrorRequiredField")]
    [StringLength(100, ErrorMessageResourceName = "ErrorMinimumLengthPlural", ErrorMessageResourceType = typeof(Resources.GeneralResources), MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "NewPassword", ResourceType = typeof(Resources.AccountResources))]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "NewPasswordConfirmation", ResourceType = typeof(Resources.AccountResources))]
    [Compare("NewPassword", ErrorMessageResourceName = "ErrorPasswordsNotMatch", ErrorMessageResourceType = typeof(Resources.GeneralResources))]
    public string ConfirmPassword { get; set; }
  }

  public class LoginViewModel
  {
    [Required(ErrorMessageResourceType = typeof(Resources.GeneralResources), ErrorMessageResourceName = "ErrorRequiredField")]
    [EmailAddress(ErrorMessageResourceType = typeof(Resources.AccountResources), ErrorMessageResourceName = "InvalidEmailError", ErrorMessage = null)]
    [Display(Name = "Email", ResourceType = typeof(Resources.AccountResources))]
    public string Email { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources.GeneralResources), ErrorMessageResourceName = "ErrorRequiredField")]
    [DataType(DataType.Password)]
    [Display(Name = "Password", ResourceType = typeof(Resources.AccountResources))]
    public string Password { get; set; }

    [Display(Name = "RememberLogin", ResourceType = typeof(Resources.AccountResources))]
    public bool RememberMe { get; set; }
  }

  public class RegisterUserViewModel
  {
    public RegisterUserViewModel()
    {
      IsInvited = false;
      InvitationCode = -1;
    }

    public bool IsInvited { get; set; }

    public int InvitationCode { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources.GeneralResources), ErrorMessageResourceName = "ErrorRequiredField")]
    [Display(Name = "Username", ResourceType = typeof(Resources.AccountResources))]
    public string Username { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources.GeneralResources), ErrorMessageResourceName = "ErrorRequiredField")]
    [EmailAddress(ErrorMessageResourceType = typeof(Resources.AccountResources), ErrorMessageResourceName = "InvalidEmailError", ErrorMessage = null)]
    [Display(Name = "Email", ResourceType = typeof(Resources.AccountResources))]
    public string Email { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources.GeneralResources), ErrorMessageResourceName = "ErrorRequiredField")]
    [StringLength(100, ErrorMessageResourceName = "ErrorMinimumLengthPlural", ErrorMessageResourceType = typeof(Resources.GeneralResources), MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password", ResourceType = typeof(Resources.AccountResources))]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "NewPasswordConfirmation", ResourceType = typeof(Resources.AccountResources))]
    [Compare("Password", ErrorMessageResourceName = "ErrorPasswordsNotMatch", ErrorMessageResourceType = typeof(Resources.GeneralResources))]
    public string ConfirmPassword { get; set; }
  }

  public class ResetPasswordViewModel
  {
    [Required(ErrorMessageResourceType = typeof(Resources.GeneralResources), ErrorMessageResourceName = "ErrorRequiredField")]
    [EmailAddress(ErrorMessageResourceType = typeof(Resources.AccountResources), ErrorMessageResourceName = "InvalidEmailError", ErrorMessage = null)]
    [Display(Name = "Email", ResourceType = typeof(Resources.AccountResources))]
    public string Email { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resources.GeneralResources), ErrorMessageResourceName = "ErrorRequiredField")]
    [StringLength(100, ErrorMessageResourceName = "ErrorMinimumLengthPlural", ErrorMessageResourceType = typeof(Resources.GeneralResources), MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password", ResourceType = typeof(Resources.AccountResources))]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "NewPasswordConfirmation", ResourceType = typeof(Resources.AccountResources))]
    [Compare("Password", ErrorMessageResourceName = "ErrorPasswordsNotMatch", ErrorMessageResourceType = typeof(Resources.GeneralResources))]
    public string ConfirmPassword { get; set; }

    public string Code { get; set; }
  }

  public class ForgotPasswordViewModel
  {
    [Required(ErrorMessageResourceType = typeof(Resources.GeneralResources), ErrorMessageResourceName = "ErrorRequiredField")]
    [EmailAddress(ErrorMessageResourceType = typeof(Resources.AccountResources), ErrorMessageResourceName = "InvalidEmailError", ErrorMessage = null)]
    [Display(Name = "Email", ResourceType = typeof(Resources.AccountResources))]
    public string Email { get; set; }
  }

  /// <summary>
  /// Cotains information which should be shown after succesfull information
  /// </summary>
  public class RegistrationConfirmationViewModel
  {
    public string Header { get; set; }
    public string ConfirmationOrg { get; set; }
    public string ConfirmationUser { get; set; }
    public string ConfirmationMessage { get; set; }
    public bool IsOrgRegistered { get; set; }
  }

}
