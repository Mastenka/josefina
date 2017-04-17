using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Josefina
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("AngularViews/LayoutViews/{filename}.html");
            routes.IgnoreRoute("AngularViews/LowerViews/{filename}.html");
            routes.IgnoreRoute("AngularViews/Modals/{filename}.html");
            routes.IgnoreRoute("AngularViews/PropertiesViews/{filename}.html");
            routes.IgnoreRoute("AngularViews/PropertiesViews/Tasks/{filename}.html");
            routes.IgnoreRoute("AngularViews/RightViews/{filename}.html");
            routes.IgnoreRoute("AngularViews/UpperViews/{filename}.html");
            routes.IgnoreRoute("AngularViews/UpperViews/Tasks/{filename}.html");
            routes.IgnoreRoute("AngularViews/LowerViews/Tasks/{filename}.html");
            routes.IgnoreRoute("AngularViews/TicketsViews/{filename}.html");
            routes.IgnoreRoute("AngularViews/{filename}.html");

            routes.MapRoute(
                 name: "CreateCodeReservation",
                 url: "vstupenkyCode/{projectId}/{projectString}",
                 defaults: new { controller = "Tickets", action = "CreateCodeReservation", projectId = UrlParameter.Optional, projectString = UrlParameter.Optional }
             );

            routes.MapRoute(
                 name: "CreateReservation",
                 url: "vstupenky/{projectId}/{projectString}",
                 defaults: new { controller = "Tickets", action = "CreateReservation", projectId = UrlParameter.Optional, projectString = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                  name: "EmailConfirmation",
                  url: "{controller}/{action}/{id}",
                  defaults: new { controller = "Account", action = "EmailConfirmation", id = UrlParameter.Optional }
              );
            routes.MapRoute(
                    name: "InvitationRegistration",
                    url: "{controller}/{action}/{id}",
                    defaults: new { controller = "Org", action = "InvitationRegistration", id = UrlParameter.Optional }
                );
        }
    }
}
