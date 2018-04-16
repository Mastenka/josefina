using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;

using Josefina.Entities;

namespace Josefina.Models.OrgViewModel
{
    /// <summary>
    /// Org/Register
    /// </summary>
    public class RegisterOrgViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.GeneralResources), ErrorMessageResourceName = "ErrorRequiredField")]
        [Display(Name = "OrgName", ResourceType = typeof(Resources.OrgResources))]
        public string Name { get; set; }
    }

    public class ProjectsListViewModel
    {
        public ProjectsListViewModel()
      {
        Projects = new List<Project>();
      }

        public List<Project> Projects { get; set; }
    }

    /// <summary>
    /// Org/SendInvitation
    /// </summary>
    public class SendInvitationViewModel
    {
      [Required(ErrorMessageResourceType = typeof(Resources.GeneralResources), ErrorMessageResourceName = "ErrorRequiredField")]
      [EmailAddress(ErrorMessageResourceType = typeof(Resources.AccountResources), ErrorMessageResourceName = "InvalidEmailError", ErrorMessage = null)]
      [Display(Name = "Email", ResourceType = typeof(Resources.AccountResources))]
      public string Email { get; set; }
    }
}