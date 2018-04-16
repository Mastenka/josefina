using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Josefina.Startup))]
namespace Josefina
{
  public partial class Startup
  {
    public void Configuration(IAppBuilder app)
    {
      ConfigureAuth(app);
      app.MapSignalR();
    }
  }
}
