using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Drawing;
using System.Web;
using System.Web.Mvc;

namespace LFTERPsys.Models
{
    [Table("employee_info_data", Schema = "public")]
    public class EmployeeInfoDataModel
    {
        [Key]
        [Required]
        [Display(Name = "ID")]
        [Remote("IsEmpIdExist", "EmployeeInfoData", ErrorMessage = "Employee ID already exists")]
        public string empid { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string empname { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string empstatus { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] // use this format for edit controls
        [Display(Name = "Date of Hire")]
        public DateTime empjoindate { get; set; }

        [Required]
        [Display(Name = "Experience(Yrs)")]
        public decimal empexperience { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string empcategory { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] // use this format for edit controls
        [Display(Name = "Date of Exit")]
        public DateTime? empexitdate { get; set; }

        [Required]
        [Display(Name = "Division")]
        public string divname { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string depname { get; set; }

        [Required]
        [Display(Name = "Work Location")]
        public string empworklocation { get; set; }

        [Required]
        [Display(Name = "Designation")]
        public string desgname { get; set; }

        [Required]
        [Display(Name = "Reporting To")]
        public string empreporting { get; set; }


        [Display(Name = "Image")]
        public byte[]? empphoto { get; set; }

    }

    public class EmployeeInfoDataDbContext : DbContext
    {
        public EmployeeInfoDataDbContext() : base(nameOrConnectionString: "LFTERPsysDbConnection") { }
        public virtual DbSet<EmployeeInfoDataModel> EmployeeInfoDataDbSet { get; set; }
    }

}
