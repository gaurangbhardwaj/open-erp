using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Web.Mvc;

namespace LFTERPsys.Models
{
    [Table("designation_data", Schema = "public")]
    public class DesignationDataModel
    {
        [Key]
        [Required]
        [Display(Name = "Sno")]
        public int sno { get; set; }

        [Required]
        [Display(Name = "Designation Name")]
        [Remote("IsDesgNameExist", "DesignationData", ErrorMessage = "Designation Name already exists")]

        public string desgname { get; set; }
    }

    public class DesignationDataDbContext : DbContext
    {
        public DesignationDataDbContext() : base(nameOrConnectionString: "LFTERPsysDbConnection") { }
        public virtual DbSet<DesignationDataModel> DesignationDataDbSet { get; set; }
    }
}