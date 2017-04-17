using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Josefina.Entities
{
    public class Project
    {
        public int ProjectID { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Resources.GeneralResources))]
        public string Name { get; set; }

        [ForeignKey("Owner")]
        public int OwnerID { get; set; }

        public virtual Org Owner { get; set; }

        [Display(Name = "DateStart", ResourceType = typeof(Resources.GeneralResources))]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:d.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Starts { get; set; }

        [Display(Name = "DateEnd", ResourceType = typeof(Resources.GeneralResources))]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:d.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Ends { get; set; }

        [ForeignKey("RootFolder")]
        public int RootFolderID { get; set; }

        public virtual Folder RootFolder { get; set; }

        public string OverView { get; set; }

        public virtual ICollection<TicketCategory> TicketCategories { get; set; }

        public virtual ICollection<TicketExport> TicketExports { get; set; }

        [ForeignKey("BankProxy")]
        public int? BankProxyID { get; set; }

        public virtual BankProxy BankProxy { get; set; }

        [ForeignKey("TicketSetting")]
        public int? TicketSettingID { get; set; }

        public virtual TicketSetting TicketSetting { get; set; }

        public string TicketsURL { get; set; }
    }
}