using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Web.Mvc;

namespace LFTERPsys.Models
{
    [Table("department_data", Schema = "public")]
    public class DepartmentDataModel
    {
        [Key]
        [Required]
        [Display(Name = "Sno")]
        public int sno { get; set; }

        [Required]
        [Display(Name = "Department Name")]
        [Remote("IsDepNameExist", "DepartmentData", ErrorMessage = "Department Name already exists")]

        public string depname { get; set; }
    }

    public class DepartmentDataDbContext : DbContext
    {
        public DepartmentDataDbContext() : base(nameOrConnectionString: "LFTERPsysDbConnection") { }
        public virtual DbSet<DepartmentDataModel> DepartmentDataDbSet { get; set; }
    }
}