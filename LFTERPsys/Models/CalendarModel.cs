using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace LFTERPsys.Models
{
    [Table("calendar", Schema = "public")]
    public class CalendarModel
    {
        [Key]
        [Required]
        [Display(Name = "EventID")]
        public int eventid { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string subject { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] // use this format for edit controls
        [Display(Name = "Start Date")]
        public DateTime startdate { get; set; }

        [DataType(DataType.Date)]
       //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] // use this format for edit controls
        [Display(Name = "End Date")]
        public DateTime enddate { get; set; }

        [Display(Name = "Theme Color")]
        public string themecolor { get; set; }

       
    }

    public class CalendarDbContext : DbContext
    {
        public CalendarDbContext() : base(nameOrConnectionString: "LFTERPsysDbConnection") { }
        public virtual DbSet<CalendarModel> CalendarDbSet { get; set; }
    }

}