using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace LFTERPsys.Models
{
    [Table("employee_login_data",Schema ="public")]
    public class EmployeeLoginDataModel
    {
        [Key]
        [Display(Name = "Employee ID")]
        public string empid { get; set; }


        [Display(Name = "Password")]
        [DataType (DataType.Password)]
        public string password { get; set; }


        [Display(Name ="Employee Role")]
        public string role { get; set; }

    }

    public class EmployeeLoginDataDbContext : DbContext
    {
        public EmployeeLoginDataDbContext() : base(nameOrConnectionString: "LFTERPsysDbConnection") { }
        public virtual DbSet<EmployeeLoginDataModel> EmployeeLoginDataDbSet { get; set; }
    }
}