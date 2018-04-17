using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;
using Resources;

namespace Josefina.Models.SharedViewModel
{

  /// <summary>
  /// ViewModel for Shared/MessageBox
  /// </summary>
  public class MessageViewModel
  {
    public MessageViewModel()
    {
      RedirectionButtonText = GeneralResources.Ok;
    }

    public string Header { get; set; }
    public string Body { get; set; }
    public string RedirectAction { get; set; }
    public string RedirectController { get; set; }
    public string RedirectionButtonText { get; set; }
    public bool Redirection { get; set; }
  }
}