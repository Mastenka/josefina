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
using Josefina.Models.AccountViewModel;
using Josefina.Models.SharedViewModel;
using Josefina.Models;
using Resources;
using Josefina.DAL;
using Josefina.Entities;
using System.Transactions;
using Josefina.Mailers;

namespace Josefina.Controllers
{
  [Authorize]
  public class AccountController : Controller
  {
    private ApplicationUserManager _userManager;

    public AccountController()
    {
    }

    public AccountController(ApplicationUserManager userManager)
    {
      UserManager = userManager;
    }

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

    // GET: /Account/EmailConfirmation
    [AllowAnonymous]
    public ActionResult EmailConfirmation(int id)
    {
      var messageModel = new MessageViewModel();

      using (ApplicationDbContext context = new ApplicationDbContext())
      {
        var userToConfirm = from usr in context.Users
                            where usr.EmailConfirmationCode == id
                            select usr;
        if (userToConfirm.Count() == 1)
        {
          if (userToConfirm.FirstOrDefault().EmailConfirmed)
          {
            messageModel.Body = AccountResources.EmailAlreadyConfirmed;
          }
          else
          {
            userToConfirm.FirstOrDefault().EmailConfirmed = true;
            messageModel.Body = AccountResources.EmailConfirmed;
          }
        }
        else
        {
          return View("~/Views/Shared/Error.cshtml");
        }
        context.SaveChanges();
      }

      messageModel.Header = AccountResources.EmailConfirmation;
      messageModel.RedirectAction = "Login";
      messageModel.RedirectController = "Account";
      messageModel.Redirection = true;
      messageModel.RedirectionButtonText = AccountResources.LogMeIn;

      return RedirectToAction("Message", "Home", messageModel);
    }

    //
    // GET: /Account/Login
    [AllowAnonymous]
    public ActionResult Login(string returnUrl)
    {
      TempData["loginReturnUrl"] = returnUrl;
      return View();
    }

    //
    // POST: /Account/Login
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Login(LoginViewModel submitedModel)
    {
      if (ModelState.IsValid)
      {
        using (ApplicationDbContext context = new ApplicationDbContext())
        {
          ApplicationUser user = null;

          var userForEmail = await UserManager.FindByEmailAsync(submitedModel.Email.Trim().ToLower());
          if (userForEmail != null)
          {
            user = await UserManager.FindAsync(userForEmail.UserName, submitedModel.Password);
          }

          if (user != null)
          {
            if (user.EmailConfirmed)
            {
              await SignInAsync(user, submitedModel.RememberMe);

              var loginReturnUrl = TempData["loginReturnUrl"];

              if (loginReturnUrl != null)
              {
                return Redirect(loginReturnUrl.ToString());
              }

              //Success -> redirect to home of rog
              return RedirectToAction("Home", "Org");
            }
            else
            {
              //Email is not confirmed. Show dialog box
              var model = new MessageViewModel();
              model.Body = AccountResources.NotConfirmedEmailError;
              model.Header = GeneralResources.ErrorHasOccurred;
              model.RedirectAction = "Index";
              model.RedirectController = "Home";
              model.Redirection = true;
              return RedirectToAction("Message", "Home", model);
            }
          }
          else
          {
            ModelState.AddModelError("", AccountResources.InvalidUsernameOrPassword);
          }
        }
      }

      // If we got this far, something failed, redisplay form
      return View(submitedModel);
    }

    //
    // GET: /Account/Register
    [AllowAnonymous]
    public ActionResult Register()
    {
      var registrationCookie = Request.Cookies["RegisteringOrgID"];

      if (registrationCookie == null)
      {
        return RedirectToAction("RegistrationInfo", "Org");
      }

      return View();
    }

    //
    // POST: /Account/Register
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Register(RegisterUserViewModel submitedModel)
    {
      if (submitedModel.Username != null)
      {
        if (!submitedModel.Username.ToLower().Contains("sdbs"))
        {
          ModelState.AddModelError("", "Registrace je umožněna prozatím pouze členům SDBS");
          return View(submitedModel);
        }
      }

      ViewBag.Title = GeneralResources.Registration;
      bool success = false;
      bool unknownError = false;

      ApplicationUser user = null;

      if (ModelState.IsValid)
      {
        using (ApplicationDbContext context = new ApplicationDbContext())
        {

          Org registeringOrg = null;

          #region Get org (invitation/registration)
          if (submitedModel.IsInvited)
          {
            var invitation = from inv in context.Invitations
                             where inv.Code == submitedModel.InvitationCode
                             select inv;
            if (invitation.Count() != 1 || invitation.FirstOrDefault().Accepted)
            {
              unknownError = true;
            }
            else
            {
              submitedModel.Email = invitation.FirstOrDefault().Email;
              registeringOrg = invitation.FirstOrDefault().Org;
              invitation.FirstOrDefault().Accepted = true;
            }
          }
          else
          {
            unknownError = true;
            var cookie = Request.Cookies["RegisteringOrgID"];

            if (cookie != null)
            {
              int registeringOrgID;
              if (int.TryParse(cookie.Value, out registeringOrgID))
              {
                var registeringOrgQuerry = from org in context.Orgs
                                           where org.OrgID == registeringOrgID
                                           select org;
                if (registeringOrgQuerry.Count() == 1)
                {
                  registeringOrg = registeringOrgQuerry.FirstOrDefault();
                  unknownError = false;
                }
              }
            }
          }
          #endregion

          //Existing Org
          if (!unknownError)
          {
            #region User name and email unique validation
            {
              bool modelStateError = false;

              var existingUserEmail = from usr in context.Users
                                      where usr.Email == submitedModel.Email.Trim()
                                      select usr;

              if (existingUserEmail.Any())
              {
                modelStateError = true;
                ModelState.AddModelError("Email", AccountResources.AlreadyExistsEmailError);
              }

              var existingUserUsername = from usr in context.Users
                                         where usr.UserName == submitedModel.Username.Trim()
                                         select usr;

              if (existingUserUsername.Any())
              {
                modelStateError = true;
                ModelState.AddModelError("Username", AccountResources.AlreadyExistsUsernameError);
              }

              if (modelStateError)
              {
                return View(submitedModel);
              }
            }
            #endregion

            //var appUsername = JosefinaIdentityManager.GetMUUsername(registeringOrgQuerry.FirstOrDefault().Name, submitedModel.Username.Trim());
            user = new ApplicationUser();
            if (submitedModel.IsInvited)
            {
              user.EmailConfirmed = true;
            }
            else
            {
              Random random = new Random();
              int emailConfirmationCode;
              bool isUnique = false;
              do
              {
                emailConfirmationCode = random.Next(10000, 1000000000);

                var duplicitUserConfirmation = from usr in context.Users
                                               where usr.EmailConfirmationCode == emailConfirmationCode
                                               select usr;
                isUnique = !duplicitUserConfirmation.Any();
              }
              while (!isUnique);
              user.EmailConfirmationCode = emailConfirmationCode;
              user.EmailConfirmed = false;
            }
            user.Email = submitedModel.Email.Trim().ToLower();
            user.UserName = submitedModel.Username.Trim();
            user.OrgID = registeringOrg.OrgID;

            IdentityResult result = await UserManager.CreateAsync(user, submitedModel.Password);

            if (result.Succeeded && !string.IsNullOrEmpty(user.Id))
            {
              success = true;
              context.SaveChanges();
            }
            else
            {
              unknownError = true;
            }
          }
          else
          {
            unknownError = true;
          }
        }

        if (success)
        {
          IUserMailer userMailer = new UserMailer();
          var confirmationModel = new RegistrationConfirmationViewModel();

          if (!submitedModel.IsInvited)
          {
            var email = userMailer.EmailConfirmation(user.EmailConfirmationCode, user.Email);
            email.SendAsync();
            confirmationModel.ConfirmationOrg = AccountResources.DialogSuccesfullyRegisteredOrgBody;
            confirmationModel.ConfirmationMessage = AccountResources.DialogSuccesfullStartMessage;
          }
          else
          {
            confirmationModel.ConfirmationMessage = AccountResources.DialogSuccesfullStartMessageInvitation;
          }

          confirmationModel.ConfirmationUser = AccountResources.DialogSuccesfullyRegisteredUser;
          confirmationModel.Header = AccountResources.DialogSuccesfullRegistration;
          confirmationModel.IsOrgRegistered = !submitedModel.IsInvited;
          ViewBag.Body = AccountResources.DialogSuccesfullRegistration;
          return View("RegistrationConfirmation", confirmationModel);

          //return RedirectToAction("Home", "OrgController");
        }
        else if (unknownError)
        {
          return View("~/Views/Shared/Error.cshtml");
        }
        else
        {
          // If we got this far, something failed, redisplay form
          return View(submitedModel);
        }

      }
      return View(submitedModel);
    }

    ////
    //// GET: /Account/ConfirmEmail
    //[AllowAnonymous]
    //public async Task<ActionResult> ConfirmEmail(string userId, string code)
    //{
    //  if (userId == null || code == null)
    //  {
    //    return View("Error");
    //  }

    //  IdentityResult result = await UserManager.ConfirmEmailAsync(userId, code);
    //  if (result.Succeeded)
    //  {
    //    return View("ConfirmEmail");
    //  }
    //  else
    //  {
    //    AddErrors(result);
    //    return View();
    //  }
    //}

    //
    // GET: /Account/ForgotPassword
    [AllowAnonymous]
    public ActionResult ForgotPassword()
    {
      return View();
    }

    //
    // POST: /Account/ForgotPassword
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        var user = await UserManager.FindByEmailAsync(model.Email);
        if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
        {
          ModelState.AddModelError("", AccountResources.EmailDoesntExistOrNotConfirmed);
          return View();
        }

        string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

        IUserMailer userMailer = new UserMailer();
        var email = userMailer.SendPasswordReset(model.Email.Trim(), code, user.Id);
        email.SendAsync();

        var messageModel = new MessageViewModel()
        {
          Body = AccountResources.PasswordRecoveryEmailSent,
          Header = AccountResources.PasswordRecovery
        };

        return RedirectToAction("Message", "Home", messageModel);
      }

      // If we got this far, something failed, redisplay form
      return View(model);
    }

    //
    // GET: /Account/ResetPassword
    [AllowAnonymous]
    public ActionResult ResetPassword(string code)
    {
      if (code == null)
      {
        return View("~/Views/Shared/Error.cshtml");
      }
      return View();
    }

    //
    // POST: /Account/ResetPassword
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        var user = await UserManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
          ModelState.AddModelError("", AccountResources.EmailDoesntExist);
          return View();
        }
        IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        if (result.Succeeded)
        {
          var messageModel = new MessageViewModel()
          {
            Body = AccountResources.PasswordRecoverySuccesfull,
            Header = AccountResources.PasswordRecovery,
            Redirection = true,
            RedirectAction = "login",
            RedirectController = "account"
          };

          return RedirectToAction("Message", "Home", messageModel);
        }
        else
        {
          AddErrors(result);
          return View();
        }
      }
      // If we got this far, something failed, redisplay form
      return View(model);
    }

    //
    // POST: /Account/Disassociate
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
    {
      ManageMessageId? message = null;
      IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
      if (result.Succeeded)
      {
        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        await SignInAsync(user, isPersistent: false);
        message = ManageMessageId.RemoveLoginSuccess;
      }
      else
      {
        message = ManageMessageId.Error;
      }
      return RedirectToAction("Manage", new { Message = message });
    }

    //
    // GET: /Account/Manage
    public ActionResult Manage(ManageMessageId? message)
    {
      ViewBag.StatusMessage =
          message == ManageMessageId.ChangePasswordSuccess ? AccountResources.PasswordChanged
          : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
          : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
          : message == ManageMessageId.Error ? GeneralResources.ErrorHasOccurred
          : "";
      ViewBag.HasLocalPassword = HasPassword();
      ViewBag.ReturnUrl = Url.Action("Manage");
      return View();
    }

    //
    // POST: /Account/Manage
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Manage(ManageUserViewModel model)
    {
      bool hasPassword = HasPassword();
      ViewBag.HasLocalPassword = hasPassword;
      ViewBag.ReturnUrl = Url.Action("Manage");
      if (hasPassword)
      {
        if (ModelState.IsValid)
        {
          if (model.OldPassword == model.NewPassword)
          {
            ModelState.AddModelError("", AccountResources.NewPasswordMustBeNew);
            return View(model);
          }

          IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
          if (result.Succeeded)
          {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            await SignInAsync(user, isPersistent: false);
            return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
          }
          else
          {
            foreach (string error in result.Errors)
            {
              switch (error)
              {
                case ("Incorrect password."):
                  {
                    ModelState.AddModelError("", AccountResources.InvalidPassword);
                  }
                  break;
                default:
                  {
                    return View("~/Views/Shared/Error.cshtml");
                  }
              }
            }
            return View(model);
          }
        }
      }
      else
      {
        // User does not have a password so remove any validation errors caused by a missing OldPassword field
        ModelState state = ModelState["OldPassword"];
        if (state != null)
        {
          state.Errors.Clear();
        }

        if (ModelState.IsValid)
        {
          IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
          if (result.Succeeded)
          {
            return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
          }
          else
          {
            AddErrors(result);
          }
        }
      }

      // If we got this far, something failed, redisplay form
      return View(model);
    }

    //
    // POST: /Account/LogOff
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult LogOff()
    {
      AuthenticationManager.SignOut();
      return RedirectToAction("Index", "Home");
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && UserManager != null)
      {
        UserManager.Dispose();
        UserManager = null;
      }
      base.Dispose(disposing);
    }

    #region MS methods




    #region Not used MS Methods
    ////
    //// POST: /Account/ExternalLogin
    //[HttpPost]
    //[AllowAnonymous]
    //[ValidateAntiForgeryToken]
    //public ActionResult ExternalLogin(string provider, string returnUrl)
    //{
    //  // Request a redirect to the external login provider
    //  return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
    //}

    //
    //// GET: /Account/ExternalLoginCallback
    //[AllowAnonymous]
    //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
    //{
    //  var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
    //  if (loginInfo == null)
    //  {
    //    return RedirectToAction("Login");
    //  }

    //  // Sign in the user with this external login provider if the user already has a login
    //  var user = await UserManager.FindAsync(loginInfo.Login);
    //  if (user != null)
    //  {
    //    await SignInAsync(user, isPersistent: false);
    //    return RedirectToLocal(returnUrl);
    //  }
    //  else
    //  {
    //    // If the user does not have an account, then prompt the user to create an account
    //    ViewBag.ReturnUrl = returnUrl;
    //    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
    //    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
    //  }
    //}

    //
    //// POST: /Account/LinkLogin
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public ActionResult LinkLogin(string provider)
    //{
    //  // Request a redirect to the external login provider to link a login for the current user
    //  return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
    //}

    //
    // GET: /Account/LinkLoginCallback
    //public async Task<ActionResult> LinkLoginCallback()
    //{
    //  var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
    //  if (loginInfo == null)
    //  {
    //    return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
    //  }
    //  IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
    //  if (result.Succeeded)
    //  {
    //    return RedirectToAction("Manage");
    //  }
    //  return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
    //}

    //
    // POST: /Account/ExternalLoginConfirmation
    //[HttpPost]
    //[AllowAnonymous]
    //[ValidateAntiForgeryToken]
    //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
    //{
    //  if (User.Identity.IsAuthenticated)
    //  {
    //    return RedirectToAction("Manage");
    //  }

    //  if (ModelState.IsValid)
    //  {
    //    // Get the information about the user from the external login provider
    //    var info = await AuthenticationManager.GetExternalLoginInfoAsync();
    //    if (info == null)
    //    {
    //      return View("ExternalLoginFailure");
    //    }
    //    var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
    //    IdentityResult result = await UserManager.CreateAsync(user);
    //    if (result.Succeeded)
    //    {
    //      result = await UserManager.AddLoginAsync(user.Id, info.Login);
    //      if (result.Succeeded)
    //      {
    //        await SignInAsync(user, isPersistent: false);

    //        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
    //        // Send an email with this link
    //        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
    //        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
    //        // SendEmail(user.Email, callbackUrl, "Confirm your account", "Please confirm your account by clicking this link");

    //        return RedirectToLocal(returnUrl);
    //      }
    //    }
    //    AddErrors(result);
    //  }

    //  ViewBag.ReturnUrl = returnUrl;
    //  return View(model);
    //}



    ////
    //// GET: /Account/ExternalLoginFailure
    //[AllowAnonymous]
    //public ActionResult ExternalLoginFailure()
    //{
    //  return View();
    //}

    //[ChildActionOnly]
    //public ActionResult RemoveAccountList()
    //{
    //  var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
    //  ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
    //  return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
    //}


    ////
    //public string GetCurrentUsername()
    //{
    //  //UserManager.FindById(User.Identity.GetUserId());
    //  using (JosefinaContextDB context = new JosefinaContextDB())
    //  {
    //    var userQuerry = from usr in context.JosefinaUsers
    //                     where usr.MUUserID == User.Identity.GetUserId()
    //                     select usr;
    //    if (userQuerry.Count() == 1)
    //    {
    //      return userQuerry.FirstOrDefault().UserName;
    //    }
    //  }
    //  return "";
    //} 
    #endregion


    #region Helpers
    // Used for XSRF protection when adding external logins
    private const string XsrfKey = "XsrfId";

    private IAuthenticationManager AuthenticationManager
    {
      get
      {
        return HttpContext.GetOwinContext().Authentication;
      }
    }

    private async System.Threading.Tasks.Task SignInAsync(ApplicationUser user, bool isPersistent)
    {
      AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
      AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
    }

    private void AddErrors(IdentityResult result)
    {
      foreach (var error in result.Errors)
      {
        ModelState.AddModelError("", error);
      }
    }

    private bool HasPassword()
    {
      var user = UserManager.FindById(User.Identity.GetUserId());
      if (user != null)
      {
        return user.PasswordHash != null;
      }
      return false;
    }

    public enum ManageMessageId
    {
      ChangePasswordSuccess,
      SetPasswordSuccess,
      RemoveLoginSuccess,
      Error
    }

    private ActionResult RedirectToLocal(string returnUrl)
    {
      if (Url.IsLocalUrl(returnUrl))
      {
        return Redirect(returnUrl);
      }
      else
      {
        return RedirectToAction("Index", "Home");
      }
    }

    private class ChallengeResult : HttpUnauthorizedResult
    {
      public ChallengeResult(string provider, string redirectUri)
        : this(provider, redirectUri, null)
      {
      }

      public ChallengeResult(string provider, string redirectUri, string userId)
      {
        LoginProvider = provider;
        RedirectUri = redirectUri;
        UserId = userId;
      }

      public string LoginProvider { get; set; }
      public string RedirectUri { get; set; }
      public string UserId { get; set; }

      public override void ExecuteResult(ControllerContext context)
      {
        var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
        if (UserId != null)
        {
          properties.Dictionary[XsrfKey] = UserId;
        }
        context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
      }
    }
    #endregion

    #endregion
  }
}