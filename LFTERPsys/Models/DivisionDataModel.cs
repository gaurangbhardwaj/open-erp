using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Web.Mvc;

namespace LFTERPsys.Models
{
    [Table("division_data", Schema = "public")]
    public class DivisionDataModel
    {
        [Key]
        [Required]
        [Display(Name = "Sno")]
        public int sno { get; set; }

        [Required]
        [Display(Name = "Division Name")]
        [Remote("IsDivNameExist", "DivisionData", ErrorMessage = "Division Name already exists")]
        public string divname { get; set; }
    }

    public class DivisionDataDbContext : DbContext
    {
        public DivisionDataDbContext() : base(nameOrConnectionString: "LFTERPsysDbConnection") { }
        public virtual DbSet<DivisionDataModel> DivisionDataDbSet { get; set; }
    }
}