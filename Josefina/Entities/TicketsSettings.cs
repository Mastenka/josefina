using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Josefina.Entities
{
    public class TicketSetting
    {
        public int TicketSettingID { get; set; }

        public string ProjectViewNameCZ { get; set; }

        public string ProjectViewNameEN { get; set; }

        public string StartsCZ { get; set; }

        public string StartsEN { get; set; }

        public string LocationCZ { get; set; }

        public string LocationEN { get; set; }

        public byte[] Logo { get; set; }

        public string LogoURL { get; set; }

        public string NoteCZ { get; set; }

        public string NoteEN { get; set; }

        public bool NamedTickets { get; set; }

        public int MaxTicketsPerEmail { get; set; }

        public string NoteOrderCZ { get; set; }

        public string NoteOrderEN { get; set; }

        public string TermsConditionsCZ { get; set; }

        public string TermsConditionsEN { get; set; }

        public bool AllowTermsConditions { get; set; }
    }
}