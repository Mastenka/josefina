using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Josefina.Entities;
using Josefina.DAL;
using System.Net;
using System.Xml;
using Josefina.Models.TicketsViewModel;
using Josefina.Mailers;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Josefina.Helpers;
using System.Globalization;

namespace Josefina.Helpers
{
    public class FIOTransactionLoader
    {
        public static FIOTransactionResult GetFIOTransactions(Project project, ApplicationDbContext context)
        {
            context.Entry(project).Reference(p => p.BankProxy).Load();
            context.Entry(project.BankProxy).Reference(bp => bp.FioBankProxy).Load();
            return GetFIOTransactions(project, project.BankProxy.FioBankProxy.Token, context, false, project.BankProxy.FioBankProxy.LastTransactionLoad.HasValue ? project.BankProxy.FioBankProxy.LastTransactionLoad.Value : DateTime.Now);
        }


        public static FIOTransactionResult GetFIOTransactions(Project project, ApplicationDbContext context, DateTime from)
        {
            context.Entry(project).Reference(p => p.BankProxy).Load();
            context.Entry(project.BankProxy).Reference(bp => bp.FioBankProxy).Load();
            project.BankProxy.FioBankProxy.LastUpdate = DateTime.Now;
            return GetFIOTransactions(project, project.BankProxy.FioBankProxy.Token, context, false, from);            
        }

        //cancel overdue
        public static FIOTransactionResult SetProjectBankInfoFromToken(Project project, ApplicationDbContext context, string token)
        {
            return GetFIOTransactions(project, token, context, true, DateTime.Now);
        }

        public static FIOTransactionResult GetFIOTransactions(Project project, string token, ApplicationDbContext context, bool loadOnlyAccountInfo, DateTime from)
        {
            FIOTransactionResult result = new FIOTransactionResult();
            project.Owner.TransactionsLastUpdate = DateTime.Now;

            try
            {
                string url = FioBankHelper.TRANSACTIONS_DATE_PREFIX + token + FioBankHelper.GetDatePostfix(from, DateTime.Now);

                WebClient client = new WebClient();
                string data = "";

                try
                {
                    data = client.DownloadString(url);
                }
                catch (Exception e)
                {
                    result.Error = true;
                    return result;
                }

                if (string.IsNullOrEmpty(data))
                {
                    result.Error = true;
                    return result;
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(data);

                if (loadOnlyAccountInfo)
                {
                    SetAccountInformation(doc, result);
                    result.Error = false;
                    return result;
                }

                XmlNode transactionList = doc.DocumentElement.SelectSingleNode("/AccountStatement/TransactionList");

                result.Transactions = new List<FioTransaction>();

                if (!transactionList.HasChildNodes)
                {
                    result.Error = false;
                    return result;
                }

                XmlNodeList transactions = doc.DocumentElement.SelectNodes("/AccountStatement/TransactionList/Transaction");

                foreach (XmlNode transactionNode in transactions)
                {
                    result.Log += "TransactionStart | ";

                    string transactionIDString = transactionNode.SelectSingleNode("column_22").InnerText;

                    result.Log += "ID1: *" + transactionIDString + "* | ";

                    long transactionID;
                    if (!long.TryParse(transactionIDString, NumberStyles.Any, CultureInfo.InvariantCulture, out transactionID))
                    {
                        result.Log += "ID2|";
                        result.Error = true;
                        return result;
                    }

                    result.Log += "ID3: *" + transactionID + "* | ";

                    //Ammount
                    decimal ammount;
                    XmlNode ammountNode = transactionNode.SelectSingleNode("column_1");

                    if (ammountNode != null && !string.IsNullOrEmpty(ammountNode.InnerText) && !string.IsNullOrWhiteSpace(ammountNode.InnerText))
                    {

                        if(ammountNode.InnerText.Contains('-'))
                        {
                            continue;
                        }

                        result.Log += "Ammount1: *" + ammountNode.InnerText + "* | ";

                        if (!decimal.TryParse(ammountNode.InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out ammount))
                        {
                            double ammountDouble;

                            if (double.TryParse(ammountNode.InnerText.Replace('.', ','), out ammountDouble))
                            {
                                result.Log += "Ammount2: *" + ammountDouble + "* | ";

                                ammount = Convert.ToDecimal(ammountDouble);
                            }
                            else
                            {
                                result.Log += "Ammount3| ";
                                result.Error = true;
                                return result;
                            }
                        }
                    }
                    else
                    {
                        ammount = 0;
                    }

                    result.Log += "Ammount4: *" + ammount + "* | ";

                    //Variable symbol
                    long variableSymbol;
                    XmlNode variableSymbolNode = transactionNode.SelectSingleNode("column_5");

                    if (variableSymbolNode != null && !string.IsNullOrEmpty(variableSymbolNode.InnerText) && !string.IsNullOrWhiteSpace(variableSymbolNode.InnerText))
                    {
                        result.Log += "VS1: *" + variableSymbolNode.InnerText + "* | ";

                        if (!long.TryParse(variableSymbolNode.InnerText, out variableSymbol))
                        {
                            result.Log += "VS2|";
                            result.Error = true;
                            return result;
                        }
                    }
                    else
                    {
                        variableSymbol = 0;
                    }

                    result.Log += "VS1: *" + variableSymbol + "* | ";

                    //Date
                    DateTime date;
                    XmlNode dateNode = transactionNode.SelectSingleNode("column_0");

                    if (dateNode != null && !string.IsNullOrEmpty(dateNode.InnerText) && !string.IsNullOrWhiteSpace(dateNode.InnerText))
                    {
                        if (!DateTime.TryParse(dateNode.InnerText, out date))
                        {
                            result.Error = true;
                            return result;
                        }
                    }
                    else
                    {
                        date = DateTime.Now;
                    }

                    //Bank number
                    int bankNumber;
                    XmlNode bankNumberNode = transactionNode.SelectSingleNode("column_3");

                    if (bankNumberNode != null && !string.IsNullOrEmpty(bankNumberNode.InnerText) && !string.IsNullOrWhiteSpace(bankNumberNode.InnerText))
                    {
                        if (!int.TryParse(bankNumberNode.InnerText, out bankNumber))
                        {
                            result.Error = true;
                            return result;
                        }
                    }
                    else
                    {
                        bankNumber = 0;
                    }

                    string note = transactionNode.SelectSingleNode("column_25") != null ? transactionNode.SelectSingleNode("column_25").InnerText : "";
                    string accountNumber = transactionNode.SelectSingleNode("column_2") != null ? transactionNode.SelectSingleNode("column_2").InnerText : "";

                    FioTransaction fioTransaction = new FioTransaction()
                    {
                        AccountNumber = accountNumber,
                        BankCode = bankNumber,
                        Amount = ammount,
                        Date = date,
                        TransactionID = transactionID,
                        Note = note,
                        VariableSymbol = variableSymbol
                    };

                    result.Log += fioTransaction.ToString() + " ||| ";

                    result.Transactions.Add(fioTransaction);
                }
            }
            catch (Exception e)
            {
                result.Log += e.Message + " ||||    " + e.StackTrace;
                result.Error = true;
                return result;
            }
            result.Error = false;
            return result;
        }

        private static void SetAccountInformation(XmlDocument doc, FIOTransactionResult result)
        {
            XmlNode infoNode = doc.DocumentElement.SelectSingleNode("/AccountStatement/Info");

            if (infoNode.HasChildNodes)
            {
                string accountNumberString = doc.DocumentElement.SelectSingleNode("/AccountStatement/Info/accountId").InnerText;
                long accountNumber;
                if (long.TryParse(accountNumberString, out accountNumber))
                {
                    result.AccountNumber = accountNumber;
                }

                result.IBAN = doc.DocumentElement.SelectSingleNode("/AccountStatement/Info/iban") != null ? doc.DocumentElement.SelectSingleNode("/AccountStatement/Info/iban").InnerText : "";
                result.BIC = doc.DocumentElement.SelectSingleNode("/AccountStatement/Info/bic") != null ? doc.DocumentElement.SelectSingleNode("/AccountStatement/Info/bic").InnerText : "";
            }
        }
    }

    public class FIOTransactionResult
    {
        public bool Error { get; set; }

        public List<FioTransaction> Transactions { get; set; }

        public long AccountNumber { get; set; }

        public string BIC { get; set; }

        public string IBAN { get; set; }

        public string Log { get; set; }
    }

    public class TicketItemsHelper
    {
        public static void ProcessTransactions(List<FioTransaction> fioTransactions, Project project, ApplicationDbContext context)
        {
            string logMsg = "";
            foreach (FioTransaction fioTransaction in fioTransactions)
            {
                logMsg += string.Format("{0}/{1}, \t{2},\t{3},\t{4},\t{5}\n", fioTransaction.AccountNumber, fioTransaction.BankCode, fioTransaction.VariableSymbol, fioTransaction.Amount, fioTransaction.Date, fioTransaction.Note);
                //Already processed transaction
                if (context.FioTransactions.Any(ft => ft.TransactionID == fioTransaction.TransactionID))
                {
                    logMsg += "Processed\n\n";
                    continue;
                }

                //VS is unique for organisation
                TicketOrder ticketOrder = context.TicketOrders.SingleOrDefault(to => to.VariableSymbol == fioTransaction.VariableSymbol && to.OrgID == project.OwnerID);

                //No order for this Org with this VS
                if (ticketOrder == null)
                {
                    logMsg += "Not found\n\n";
                    continue;
                }

                ticketOrder.TotalPaidPrice += fioTransaction.Amount;

                if (!ticketOrder.Canceled && !ticketOrder.Paid && ticketOrder.TotalPaidPrice >= ticketOrder.TotalPrice)
                {
                    ticketOrder.Paid = true;
                    ticketOrder.PaidDate = DateTime.Now;

                    foreach (TicketCategoryOrder ticketCategoryOrder in ticketOrder.TicketCategoryOrders)
                    {
                        ticketCategoryOrder.Paid = true;
                    }

                    SendConfirmationEmail(ticketOrder, project, context);
                }

                fioTransaction.TicketOrder = ticketOrder;

                context.FioTransactions.Add(fioTransaction);
            }
            Console.WriteLine(logMsg);
        }

        /// <summary>
        /// Cancels given TicketOrder in separate transaction
        /// </summary>
        /// <param name="ticketOrder"></param>
        /// <param name="context"></param>
        public static void CancelOrder(TicketOrder ticketOrder, ApplicationDbContext context)
        {
            string projectName = "";
            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                context.Entry(ticketOrder).Collection(to => to.TicketCategoryOrders).Load();

                foreach (TicketCategoryOrder ticketCategoryOrder in ticketOrder.TicketCategoryOrders)
                {
                    TicketCategory ticketCategory = context.TicketCategories.Single(tc => tc.TicketCategoryID == ticketCategoryOrder.TicketCategoryID);


                    if (projectName != "")
                    {
                        context.Entry(ticketCategory).Reference(tc => tc.Project).Load();
                        projectName = ticketCategory.Project.Name;
                    }

                    bool success = false;
                    for (int i = 0; i < 15; i++)
                    {
                        ticketCategory.Ordered -= ticketCategoryOrder.Count;
                        try
                        {
                            context.SaveChanges();
                            success = true;
                            break;
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            ex.Entries.Single().Reload();
                            ticketCategory = (TicketCategory)ex.Entries.Single().Entity;
                            continue;
                        }
                    }

                    if (success)
                    {
                        ticketCategoryOrder.Canceled = true;
                    }
                    else
                    {
                        transaction.Rollback();
                        return;
                    }
                }

                ticketOrder.Canceled = true;
                context.SaveChanges();
                transaction.Commit();
            }

            SendCanceledEmail(ticketOrder, projectName);

            return;
        }

        public static void SendConfirmationEmail(TicketExportItem ticketExportItem, Project project)
        {
            TicketItemConfirmationViewModel ticketExportConfirmationViewModel = new TicketItemConfirmationViewModel()
            {
                ProjectName = project.TicketSetting.ProjectViewNameCZ,
                Email = ticketExportItem.Email,
                IsExport = true,
                DateStart = project.TicketSetting.StartsCZ,
                Location = project.TicketSetting.LocationCZ,
                Note = project.TicketSetting.NoteCZ
            };
         
            List<TicketPDFGenerator.TicketToGenerateWrapper> ticketsToGenerate = new List<TicketPDFGenerator.TicketToGenerateWrapper>();
            TicketPDFGenerator.TicketToGenerateWrapper ticketWrapper = new TicketPDFGenerator.TicketToGenerateWrapper();

            ticketWrapper.CategoryName = ticketExportItem.TicketExport.Name;
            ticketWrapper.Location = project.TicketSetting.LocationEN;
            ticketWrapper.StartDate = project.TicketSetting.StartsEN;
            ticketWrapper.ProjectName = project.TicketSetting.ProjectViewNameEN;
            ticketWrapper.Note = project.TicketSetting.NoteCZ;

            ticketWrapper.VisitorEmail = ticketExportItem.Email;
            ticketWrapper.VisitorName = ticketExportItem.Name;
            ticketWrapper.Code = ticketExportItem.Code;
            ticketWrapper.QRCode = ticketExportItem.QRCode;

            ticketsToGenerate.Add(ticketWrapper);


            IUserMailer userMailer = new UserMailer();
            var email = userMailer.SendTicketOrderConfirmation(ticketExportConfirmationViewModel, ticketsToGenerate, project.TicketSetting);
            email.Send();
        }

        public static void SendConfirmationEmail(TicketOrder ticketOrder, Project project, ApplicationDbContext context)
        {
            TicketItemConfirmationViewModel ticketOrderFinalViewModel = new TicketItemConfirmationViewModel()
            {
                ProjectName = ticketOrder.IsEnglish ? project.TicketSetting.ProjectViewNameEN : project.TicketSetting.ProjectViewNameCZ,
                Email = ticketOrder.Email,
                VariableSymbol = ticketOrder.VariableSymbol,
                IsExport = false
            };

            if (ticketOrder.IsEnglish)
            {
                ticketOrderFinalViewModel.DateStart = project.TicketSetting.StartsEN;
                ticketOrderFinalViewModel.Location = project.TicketSetting.LocationEN;
                ticketOrderFinalViewModel.Note = project.TicketSetting.NoteEN;
            }
            else
            {
                ticketOrderFinalViewModel.DateStart = project.TicketSetting.StartsCZ;
                ticketOrderFinalViewModel.Location = project.TicketSetting.LocationCZ;
                ticketOrderFinalViewModel.Note = project.TicketSetting.NoteCZ;
            }

            List<TicketPDFGenerator.TicketToGenerateWrapper> ticketsToGenerate = new List<TicketPDFGenerator.TicketToGenerateWrapper>();

            foreach (TicketCategoryOrder ticketCategoryOrder in ticketOrder.TicketCategoryOrders)
            {
                context.Entry(ticketCategoryOrder).Collection(tco => tco.TicketItems).Load();
                foreach (TicketItem ticketItem in ticketCategoryOrder.TicketItems)
                {
                    TicketPDFGenerator.TicketToGenerateWrapper ticketWrapper = new TicketPDFGenerator.TicketToGenerateWrapper();

                    if (ticketOrder.IsEnglish)
                    {
                        ticketWrapper.CategoryName = ticketCategoryOrder.TicketCategory.HeaderEN;
                        ticketWrapper.Location = project.TicketSetting.LocationEN;
                        ticketWrapper.StartDate = project.TicketSetting.StartsEN;
                        ticketWrapper.ProjectName = project.TicketSetting.ProjectViewNameEN;
                        ticketWrapper.Note = project.TicketSetting.NoteEN;
                    }
                    else
                    {
                        ticketWrapper.CategoryName = ticketCategoryOrder.TicketCategory.HeaderCZ;
                        ticketWrapper.Location = project.TicketSetting.LocationCZ;
                        ticketWrapper.StartDate = project.TicketSetting.StartsCZ;
                        ticketWrapper.ProjectName = project.TicketSetting.ProjectViewNameCZ;
                        ticketWrapper.Note = project.TicketSetting.NoteCZ;
                    }

                    ticketWrapper.VisitorEmail = string.IsNullOrEmpty(ticketItem.Email) ? ticketOrder.Email : ticketItem.Email;
                    ticketWrapper.VisitorName = ticketItem.Name;
                    ticketWrapper.Code = ticketItem.Code;
                    ticketWrapper.QRCode = ticketItem.QRCode;           

                    ticketsToGenerate.Add(ticketWrapper);
                }
            }

            IUserMailer userMailer = new UserMailer();
            var email = userMailer.SendTicketOrderConfirmation(ticketOrderFinalViewModel, ticketsToGenerate, project.TicketSetting);
            email.Send();
        }

        private static void SendCanceledEmail(TicketOrder ticketOrder, string projectName)
        {
            TicketOrderCanceledViewModel ticketOrderCanceledViewModel = new TicketOrderCanceledViewModel()
            {
                Email = ticketOrder.Email,
                ProjectName = projectName,
                VariableSymbol = ticketOrder.VariableSymbol,
                ReserverdUntil = ticketOrder.ReservedUntil.Value.ToShortDateString()
            };

            IUserMailer userMailer = new UserMailer();
            var email = userMailer.SendTicketOrderCanceled(ticketOrderCanceledViewModel);
            email.Send();
        }
    }
}