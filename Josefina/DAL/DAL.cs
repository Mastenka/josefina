using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Josefina.Models;
using Josefina.Entities;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Josefina.DAL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
          : base("DefaultConnection", throwIfV1Schema: false)
        {
        } 

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Org> Orgs { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<TicketSetting> TicketsSettings { get; set; }

        public DbSet<Task> Tasks { get; set; }

        public DbSet<Folder> Folders { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Invitation> Invitations { get; set; }

        public DbSet<TaskUpdate> TaskUpdates { get; set; }

        public DbSet<TicketCategoryOrder> TicketCategoryOrders { get; set; }

        public DbSet<TicketCategory> TicketCategories { get; set; }

        public DbSet<TicketItem> TicketItems { get; set; }

        public DbSet<TicketOrder> TicketOrders { get; set; }

        public DbSet<BankProxy> BankProxies { get; set; }

        public DbSet<FioBankProxy> FioBankProxies { get; set; }

        public DbSet<TicketExport> TicketExports { get; set; }

        public DbSet<TicketExportItem> TicketExportItems { get; set; }
        
        public DbSet<FioTransaction> FioTransactions { get; set; }

    }
}