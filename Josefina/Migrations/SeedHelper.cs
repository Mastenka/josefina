using Josefina.DAL;
using Josefina.Entities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace Josefina.Migrations
{
    public class SeedHelper
    {
        private ApplicationDbContext context;

        private string LoremIpsum = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Nulla accumsan, elit sit amet varius semper, nulla mauris mollis quam, tempor suscipit diam nulla vel leo. Praesent dapibus. Integer imperdiet lectus quis justo. Nunc auctor. Duis viverra diam non justo. Mauris suscipit, ligula sit amet pharetra semper, nibh ante cursus purus, vel sagittis velit mauris vel metus. Fusce suscipit libero eget elit. Maecenas aliquet accumsan leo. Maecenas lorem. Aliquam ante. Etiam bibendum elit eget erat. Proin pede metus, vulputate nec, fermentum fringilla, vehicula vitae, justo. Aliquam erat volutpat. Fusce dui leo, imperdiet in, aliquam sit amet, feugiat eu, orci. Maecenas lorem. In rutrum. Fusce suscipit libero eget elit. Curabitur ligula sapien, pulvinar a vestibulum quis, facilisis vel sapien. Sed ac dolor sit amet purus malesuada congue. Praesent in mauris eu tortor porttitor accumsan. Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat. Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. Etiam neque. Sed elit dui, pellentesque a, faucibus vel, interdum nec, diam. Nulla est. Quisque porta. Nunc dapibus tortor vel mi dapibus sollicitudin. Praesent dapibus. Etiam posuere lacus quis dolor. Maecenas aliquet accumsan leo. Nullam lectus justo, vulputate eget mollis sed, tempor sed magna. Maecenas fermentum, sem in pharetra pellentesque, velit turpis volutpat ante, in pharetra metus odio a lectus. Integer imperdiet lectus quis justo.";

        public void Seed(ApplicationDbContext _context)
        {
            this.context = _context;

            try
            {
                var org1 = new Org() { Name = "SDBS", VariableSymbolCounter = 1 };

                context.Orgs.AddOrUpdate(org1);

                var passwordHash = new PasswordHasher();
                string password = passwordHash.HashPassword("testtest");

                var user1 = new ApplicationUser { Email = "bittmann.stefan@gmail.com", OrgID = org1.OrgID, EmailConfirmed = true, UserName = "Batman", PasswordHash = password, SecurityStamp = Guid.NewGuid().ToString() };

                context.Users.AddOrUpdate(user1);

                string password2 = passwordHash.HashPassword("testtest");

                var user2 = new ApplicationUser { Email = "robin@localhost:44301", OrgID = org1.OrgID, EmailConfirmed = true, UserName = "Robin", PasswordHash = password2, SecurityStamp = Guid.NewGuid().ToString() };

                context.Users.AddOrUpdate(user2);

                string password3 = passwordHash.HashPassword("testtest");

                var user3 = new ApplicationUser { Email = "freeze@localhost:44301", OrgID = org1.OrgID, EmailConfirmed = true, UserName = "Freeze", PasswordHash = password3, SecurityStamp = Guid.NewGuid().ToString() };

                context.Users.AddOrUpdate(user3);

                var rootFolder1 = new Folder() { Name = "Psy High15" };

                context.Folders.AddOrUpdate(rootFolder1);

                var ticketsSettings = new TicketSetting() { LocationCZ = "Praha 5", LocationEN = "Prague 5", MaxTicketsPerEmail = 8, NamedTickets = false, ProjectViewNameCZ = "Fesťák", ProjectViewNameEN = "The Festival", StartsCZ = "20.listopad", StartsEN = "5th July" };

                context.TicketsSettings.AddOrUpdate(ticketsSettings);

                var project = new Project() { RootFolder = rootFolder1, Name = "Psy High15", Starts = DateTime.Now.AddDays(-90), Ends = DateTime.Now.AddDays(20), Owner = org1, TicketSetting = ticketsSettings };

                context.Projects.AddOrUpdate(project);


                List<Task> tasks = new List<Task>();

                var task1 = new Task() { Creator = user1, Org = org1, Deadline = DateTime.Now.AddDays(5), Name = "Prodlužky", Parent = rootFolder1, Content = "<h1>Úkol pro prodlužovací kabely</h1> </br> <p> Toto je hlavní úkol pro prodlužky </p>" };
                var task2 = new Task() { Creator = user1, Org = org1, Deadline = DateTime.Now.AddDays(6), Name = "Nářadí", Parent = rootFolder1, Content = "<h1>Úkol pro stavební vybavení</h1> </br> <p> Toto je hlavní úkol pro nářadí </p>" };
                var task3 = new Task() { Creator = user1, Org = org1, Deadline = DateTime.Now.AddDays(7), Name = "Světla", Parent = rootFolder1, Content = "<h1>Úkol pro světelnou techniku</h1> </br> <p> Toto je hlavní úkol pro světla </p>" };

                context.Tasks.AddOrUpdate(task1);
                context.Tasks.AddOrUpdate(task2);
                context.Tasks.AddOrUpdate(task3);

                tasks.Add(task1);
                tasks.Add(task2);
                tasks.Add(task3);

                //Random rnd = new Random();

                //DateTime startDateTime = DateTime.Now.AddDays(-80);

                //for (int i = 0; i < 100; i++)
                //{
                //  startDateTime = startDateTime.AddHours(1);
                //  CreateTaskComments(user1, tasks[i % 3], rnd.Next(2, 10), startDateTime);
                //  startDateTime = startDateTime.AddHours(1);
                //  CreateTaskComments(user2, tasks[i % 3], rnd.Next(2, 10), startDateTime);
                //  startDateTime = startDateTime.AddHours(1);
                //  CreateTaskComments(user3, tasks[i % 3], rnd.Next(2, 10), startDateTime);
                //}

                var folder2 = new Folder() { Name = "Stavby", Parent = rootFolder1 };
                context.Folders.AddOrUpdate(folder2);

                var task4 = new Task() { Creator = user2, Org = org1, Deadline = DateTime.Now.AddDays(5), Name = "Bar", Parent = folder2 };
                var task5 = new Task() { Creator = user3, Org = org1, Deadline = DateTime.Now.AddDays(6), Name = "Stage", Parent = folder2 };
                var task6 = new Task() { Creator = user3, Org = org1, Deadline = DateTime.Now.AddDays(7), Name = "Přednášky", Parent = folder2 };

                context.Tasks.AddOrUpdate(task4);
                context.Tasks.AddOrUpdate(task5);
                context.Tasks.AddOrUpdate(task6);

                TicketCategory ticketCategory1 = new TicketCategory() { Capacity = 100, Price = 110, Project = project, SoldFrom = new DateTime(2015, 10, 1), SoldTo = new DateTime(2015, 12, 31), HeaderCZ = "Stání", HeaderEN = "Sitting" };
                TicketCategory ticketCategory2 = new TicketCategory() { Capacity = 200, Price = 220, Project = project, SoldFrom = new DateTime(2015, 10, 1), SoldTo = new DateTime(2015, 12, 31), HeaderCZ = "VIP sezení", HeaderEN = "VIP sitting" };
                context.TicketCategories.AddOrUpdate(ticketCategory1);
                context.TicketCategories.AddOrUpdate(ticketCategory2);

                ////Order:1
                //TicketOrder ticketOrder1 = new TicketOrder() { Created = new DateTime(2015, 10, 1), Email = "customer1@localhost:44301", Paid = new DateTime(2015, 10, 2), TotalPrice = 66.60, VariableSymbol = 666777, OrgID = org1.OrgID };
                //context.TicketOrders.AddOrUpdate(ticketOrder1);
                ////Order:1, Category:1
                //TicketCategoryOrder ticketCategoryOrder1 = new TicketCategoryOrder() { Count = 2, Paid = true, TicketCategory = ticketCategory1, TicketOrder = ticketOrder1 };
                //context.TicketCategoryOrders.AddOrUpdate(ticketCategoryOrder1);
                //////Order:1, Category:1, TicketItems
                ////TicketItem ticketItem111 = new TicketItem() { TicketCategoryOrder = ticketCategoryOrder1, Code = "ticketItem111" };
                ////TicketItem ticketItem112 = new TicketItem() { TicketCategoryOrder = ticketCategoryOrder1, Code = "ticketItem112" };
                ////context.TicketItems.AddOrUpdate(ticketItem111);
                ////context.TicketItems.AddOrUpdate(ticketItem112);
                ////Order:1, Category:2
                //TicketCategoryOrder ticketCategoryOrder2 = new TicketCategoryOrder() { Count = 1, Paid = true, TicketCategory = ticketCategory2, TicketOrder = ticketOrder1 };
                //context.TicketCategoryOrders.AddOrUpdate(ticketCategoryOrder2);
                //////Order:1, Category:1, TicketItems
                ////TicketItem ticketItem121 = new TicketItem() { TicketCategoryOrder = ticketCategoryOrder2, Code = "ticketItem121" };
                ////context.TicketItems.AddOrUpdate(ticketItem121);

                ////Order:2
                //TicketOrder ticketOrder2 = new TicketOrder() { Created = new DateTime(2015, 10, 5), Email = "customer2@localhost:44301", Paid = new DateTime(2015, 10, 6), TotalPrice = 42.00, VariableSymbol = 420420, OrgID = org1.OrgID };
                //context.TicketOrders.AddOrUpdate(ticketOrder2);
                ////Order:2, Category:1
                //TicketCategoryOrder ticketCategoryOrder3 = new TicketCategoryOrder() { Count = 2, Paid = true, TicketCategory = ticketCategory1, TicketOrder = ticketOrder2 };
                //context.TicketCategoryOrders.AddOrUpdate(ticketCategoryOrder3);
                //////Order:2, Category:1, TicketItems
                ////TicketItem ticketItem211 = new TicketItem() { TicketCategoryOrder = ticketCategoryOrder3, Code = "ticketItem211" };
                ////TicketItem ticketItem212 = new TicketItem() { TicketCategoryOrder = ticketCategoryOrder3, Code = "ticketItem212" };
                ////context.TicketItems.AddOrUpdate(ticketItem211);
                ////context.TicketItems.AddOrUpdate(ticketItem212);

                ////Order:2, Category:2
                //TicketCategoryOrder ticketCategoryOrder4 = new TicketCategoryOrder() { Count = 1, Paid = true, TicketCategory = ticketCategory2, TicketOrder = ticketOrder2 };
                //context.TicketCategoryOrders.AddOrUpdate(ticketCategoryOrder4);
                //////Order:2, Category:1, TicketItems
                ////TicketItem ticketItem221 = new TicketItem() { TicketCategoryOrder = ticketCategoryOrder4, Code = "ticketItem221" };
                ////context.TicketItems.AddOrUpdate(ticketItem221);

                FioBankProxy fioBankProxy = new FioBankProxy()
                {
                    AccountNumber = 2200908004,              
                    Token = "kuNbfeSqViKWPhAn4wW1WzXvnOpKhJYWoxDxtrLYwA97baSuQlJygxBIQg0bjk59",
                    LastUpdate = DateTime.Now
                };

                context.FioBankProxies.AddOrUpdate(fioBankProxy);

                BankProxy bankProxy = new BankProxy()
                {
                    BankProxyType = EBankProxyType.FIO,
                    FioBankProxy = fioBankProxy
                };

                context.BankProxies.AddOrUpdate(bankProxy);

                project.BankProxy = bankProxy;

                project.TicketsURL = "http://localhost:44301/vstupenky/1/Psy-High15";

                ticketCategory1.Ordered = 4;
                ticketCategory2.Ordered = 2;
                org1.VariableSymbolCounter = 2;
                context.SaveChanges();

                var org2 = new Org() { Name = "SDBS2", VariableSymbolCounter = 1 };

                context.Orgs.AddOrUpdate(org2);

                var passwordHash4 = new PasswordHasher();
                string password4 = passwordHash4.HashPassword("testtest");

                var user4 = new ApplicationUser { Email = "robina@gmail.com", OrgID = org2.OrgID, EmailConfirmed = true, UserName = "Robina", PasswordHash = password4, SecurityStamp = Guid.NewGuid().ToString() };

                context.Users.AddOrUpdate(user4);

                var rootFolder2 = new Folder() { Name = "Psy High2" };

                context.Folders.AddOrUpdate(rootFolder2);

                var project2 = new Project() { RootFolder = rootFolder2, Name = "Psy High2", Starts = DateTime.Now.AddDays(-90), Ends = DateTime.Now.AddDays(20), Owner = org2 };

                context.Projects.AddOrUpdate(project2);

                TicketCategory ticketCategory3 = new TicketCategory() { Capacity = 100, Price = 110, Project = project2, SoldFrom = new DateTime(2015, 10, 1), SoldTo = new DateTime(2015, 12, 31), HeaderCZ = "Stání", HeaderEN = "Sitting" };
                TicketCategory ticketCategory4 = new TicketCategory() { Capacity = 200, Price = 220, Project = project2, SoldFrom = new DateTime(2015, 10, 1), SoldTo = new DateTime(2015, 12, 31), HeaderCZ = "VIP sezení", HeaderEN = "VIP sitting" };
                context.TicketCategories.AddOrUpdate(ticketCategory3);
                context.TicketCategories.AddOrUpdate(ticketCategory4);

                ////Order:2
                //TicketOrder ticketOrder3 = new TicketOrder() { Created = new DateTime(2015, 10, 5), Email = "customer2@localhost:44301", Paid = new DateTime(2015, 10, 6), TotalPrice = 42.00, VariableSymbol = 420420, OrgID = org2.OrgID };
                //context.TicketOrders.AddOrUpdate(ticketOrder3);
                ////Order:2, Category:1
                //TicketCategoryOrder ticketCategoryOrder5 = new TicketCategoryOrder() { Count = 2, Paid = true, TicketCategory = ticketCategory3, TicketOrder = ticketOrder3 };
                //context.TicketCategoryOrders.AddOrUpdate(ticketCategoryOrder5);

                FioBankProxy fioBankProxy2 = new FioBankProxy()
                {
                    AccountNumber = 2200908004,
                    Token = "kuNbfeSqViKWPhAn4wW1WzXvnOpKhJYWoxDxtrLYwA97baSuQlJygxBIQg0bjk59",
                    LastUpdate = DateTime.Now
                };

                context.FioBankProxies.AddOrUpdate(fioBankProxy2);

                BankProxy bankProxy2 = new BankProxy()
                {
                    BankProxyType = EBankProxyType.FIO,
                    FioBankProxy = fioBankProxy2
                };

                context.BankProxies.AddOrUpdate(bankProxy2);

                project2.BankProxy = bankProxy2;

                project2.TicketsURL = "http://localhost:44301/vstupenky/2/Psy-High2";

                //ticketCategory1.Ordered = 4;
                //ticketCategory2.Ordered = 2;
                //org1.VariableSymbolCounter = 2;
                context.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("_____Pruser");
                Console.WriteLine("_____Pruser");
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private void CreateTaskComments(ApplicationUser user, Task task, int count, DateTime startDate)
        {
            Random rnd = new Random();

            for (int i = 0; i < count; i++)
            {
                int startIndes = rnd.Next(0, LoremIpsum.Length - 200);
                int length = rnd.Next(5, 150);
                string content = LoremIpsum.Substring(startIndes, length);

                Comment comment = new Comment() { Content = content, Created = startDate.AddMinutes(rnd.Next(1, 59)), ParentUser = user, ParentTask = task };

                context.Comments.AddOrUpdate(comment);
            }
        }
    }
}