using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Josefina.Models
{
    public class ReactAppTicketViewModel
    {
        public int id { get; set; }
        public string qrCode { get; set; }
        public string code { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public bool chck {get; set;}
        public int CtgID { get; set; }
    }

    public class ReactAppTicketsViewModel
    {
        public ReactAppTicketsViewModel()
        {
            this.Tickets = new List<ReactAppTicketViewModel>();
            this.Headers = new List<ReactAppCategoryNameViewModel>();
            this.TicketExports = new List<ReactAppTicketViewModel>();
            this.ExportHeaders = new List<ReactAppCategoryNameViewModel>();
        }

        public List<ReactAppCategoryNameViewModel> ExportHeaders { get; private set; }
        public List<ReactAppCategoryNameViewModel> Headers { get; private set; }

        public List<ReactAppTicketViewModel> Tickets { get; private set; }
        public List<ReactAppTicketViewModel> TicketExports { get; private set; }
    }

    public class ReactAppCategoryNameViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class ReactAppTicketUploadViewModel
    {
        public int[] Tickets { get; set; }

        public int[] Exports { get; set; }

        public string GUID { get; set; }
    }
}