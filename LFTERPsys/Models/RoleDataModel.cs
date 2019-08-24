using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Web.Mvc;

namespace LFTERPsys.Models
{
    [Table("role_data", Schema = "public")]
    public class RoleDataModel
    {
        [Key]
        [Required]
        [Display(Name = "Employee Role")]
        [Remote("IsRoleNameExist", "RoleData", ErrorMessage = "Role Name already exists")]

        public string rolename { get; set; }

        [Required]
        [Display(Name = "View Access")]
        public string pageaccess { get; set; }


    }
    public class RoleDataDbContext : DbContext
    {
        public RoleDataDbContext() : base(nameOrConnectionString: "LFTERPsysDbConnection") { }
        public virtual DbSet<RoleDataModel> RoleDataDbSet { get; set; }
    }
}