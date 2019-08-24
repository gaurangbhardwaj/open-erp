using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace LFTERPsys.Models
{
    [Table("announcement_data", Schema = "public")]
    public class AnnouncementDataModel
    {
        [Key]
        [Required]
        public int announcementid { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name ="Subject")]
        public string subject { get; set; }

        [Required]
        [StringLength(300)]
        [Display(Name = "Description")]
        public string description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime date { get; set; }
    }

    public class AnnouncementDataDbContext : DbContext
    {
        public AnnouncementDataDbContext() : base(nameOrConnectionString: "LFTERPsysDbConnection") { }
        public virtual DbSet<AnnouncementDataModel> AnnouncementDataDbSet { get; set; }
    }

}
