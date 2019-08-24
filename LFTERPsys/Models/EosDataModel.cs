using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;


namespace LFTERPsys.Models
{
    [Table("eos_data", Schema = "public")]
    public class EosDataModel
    {
        [Key]
        [Required]
        [Display(Name = "Sno")]
        public int sno { get; set; }

        [Required]
        [Display(Name = "Employee ID")]
        public string empid { get; set; }

        [Required]
        [Display(Name = "Project Name")]
        public string projname { get; set; }

        [Required]
        [Display(Name = "EOS Year")]
        public string year { get; set; }

        [Required]
        [Display(Name = "Apr")]
        public int april { get; set; }

        [Required]
        [Display(Name = "May")]
        public int may { get; set; }

        [Required]
        [Display(Name = "Jun")]
        public int june { get; set; }

        [Required]
        [Display(Name = "Jul")]
        public int july { get; set; }

        [Required]
        [Display(Name = "Aug")]
        public int august { get; set; }

        [Required]
        [Display(Name = "Sep")]
        public int september { get; set; }

        [Required]
        [Display(Name = "Oct")]
        public int october { get; set; }

        [Required]
        [Display(Name = "Nov")]
        public int november { get; set; }

        [Required]
        [Display(Name = "Dec")]
        public int december { get; set; }

        [Required]
        [Display(Name = "Jan")]
        public int january { get; set; }

        [Required]
        [Display(Name = "Feb")]
        public int february { get; set; }

        [Required]
        [Display(Name = "Mar")]
        public int march { get; set; }

    }

    public class EosDataDbContext : DbContext
    {
        public EosDataDbContext() : base(nameOrConnectionString: "LFTERPsysDbConnection") { }
        public virtual DbSet<EosDataModel> EosDataDbSet { get; set; }
    }
}