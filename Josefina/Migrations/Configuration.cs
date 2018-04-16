using Josefina.DAL;
using Josefina.Entities;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Josefina.Migrations
{


  internal sealed class Configuration : DbMigrationsConfiguration<Josefina.DAL.ApplicationDbContext>
  {
    public Configuration()
    {
      AutomaticMigrationsEnabled = true;
      AutomaticMigrationDataLossAllowed = true;
      ContextKey = "IdentityModel";
    }

    protected override void Seed(ApplicationDbContext context)
    {
      //  This method will be called after migrating to the latest version.

      SeedHelper seedHelper = new SeedHelper();
      seedHelper.Seed(context);
    }
  }
}
