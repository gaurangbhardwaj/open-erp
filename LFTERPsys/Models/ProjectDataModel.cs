using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Web.Mvc;

namespace LFTERPsys.Models
{
    [Table("project_data", Schema = "public")]
    public class ProjectDataModel
    {
        [Key]
        [Required]
        [Display(Name = "Project ID")]
        [Remote("IsProjIdExist", "ProjectData", ErrorMessage = "Project ID already exists")]

        public string projid { get; set; }

        [Required]
        [Display(Name = "Project Name")]
        public string projname { get; set; }

        [Required]
        [Display(Name = " ERP Project Name")]
        public string erpprojname { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string depname { get; set; }

        [Required]
        [Display(Name = "Project Status")]
        public string projstatus { get; set; }
    }

    public class ProjectDataDbContext : DbContext
    {
        public ProjectDataDbContext() : base(nameOrConnectionString: "LFTERPsysDbConnection") { }
        public virtual DbSet<ProjectDataModel> ProjectDataDbSet { get; set; }
    }
}