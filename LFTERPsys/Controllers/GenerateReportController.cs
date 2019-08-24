using LFTERPsys.Models;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace LFTERPsys.Controllers
{
    public class GenerateReportController : Controller
    {
        private readonly EmployeeInfoDataDbContext employeeInfoDataDbContext = new EmployeeInfoDataDbContext();
        private readonly ProjectDataDbContext projectDataDbContext = new ProjectDataDbContext();
        private readonly EosDataDbContext eosDataDbContext = new EosDataDbContext();
        private readonly DateTime currentdate = DateTime.Now;

        private readonly string[] months = { "april", "may", "june", "july", "august", "september", "october", "november", "december", "january", "february", "march" };
        private string currentFinancialYear;


        private bool Authenticate(string pageaccess)
        {
            if (Session["LoggedEmpID" + ViewBag.SessionID] == null || Session["LoggedEmpRole" + ViewBag.SessionID] == null)
            {
                Response.Redirect(new Uri(Request.Url, Url.Action("Signin", "Login")).ToString());
            }
            else
            {
                string emprole = Session["LoggedEmpRole" + ViewBag.SessionID];
                using (RoleDataDbContext roleDataDbContext = new RoleDataDbContext())
                {
                    RoleDataModel roleDataModel = roleDataDbContext.RoleDataDbSet.Where(model => model.rolename.Equals(emprole)).FirstOrDefault();
                    string[] pages = (roleDataModel.pageaccess).Split(',');
                    if (pages.Contains(pageaccess))
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }


        private string CurrentFinancialYearChecker()
        {
            if (currentdate.Month >= 1 && currentdate.Month <= 3)
                currentFinancialYear = (Convert.ToInt32(currentdate.ToString("yyyy")) - 1) + "-" + currentdate.ToString("yyyy");
            else
                currentFinancialYear = currentdate.ToString("yyyy") + "-" + (Convert.ToInt32(currentdate.ToString("yyyy")) + 1);

            return (currentFinancialYear);
        }


        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export(string GridHtml)
        {
            return File(Encoding.ASCII.GetBytes(GridHtml), "application/vnd.ms-excel", "EOS Report" + ".xls");
        }


        [HttpGet]
        public ActionResult CurrentMonthReport(string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if (Authenticate("CurrentMonthReport"))
            {
                currentFinancialYear = CurrentFinancialYearChecker();

                string[] projnames = projectDataDbContext.ProjectDataDbSet.Where(model => model.projstatus.Equals("Active")).OrderBy(model => model.projname).Select(model => model.projname).ToArray();

                string[] empnames = employeeInfoDataDbContext.EmployeeInfoDataDbSet.OrderBy(model => model.empid).Select(model => model.empname).ToArray();
                string[] empids = employeeInfoDataDbContext.EmployeeInfoDataDbSet.OrderBy(model => model.empid).Select(model => model.empid).ToArray();

                string[,] empinfos = new string[(projnames.Length) + 2, (empnames.Length) + 1];

                for (int i = 0; i < (projnames.Length) + 2; i++)
                {
                    for (int j = 0; j < (empnames.Length) + 1; j++)
                    {
                        if (i < 2)
                        {
                            if (j == 0)
                                empinfos[i, j] = " ";
                            else if (i == 0)
                                empinfos[i, j] = empnames[j - 1];
                            else
                                empinfos[i, j] = empids[j - 1];
                        }
                        else
                        {
                            if (j == 0)
                                empinfos[i, j] = projnames[i - 2];
                            else
                            {
                                string tempempid = empids[j - 1];
                                string tempprojname = projnames[i - 2];
                                var WorkpercentValues = eosDataDbContext.EosDataDbSet.Where(model => model.empid.Equals(tempempid) && model.projname.Equals(tempprojname) && model.year.Equals(currentFinancialYear)).FirstOrDefault();

                                if (WorkpercentValues != null)
                                    empinfos[i, j] = eosDataDbContext.Entry(WorkpercentValues).Property(currentdate.ToString("MMMM").ToLower()).CurrentValue.ToString();
                                else
                                    empinfos[i, j] = "-";
                            }
                        }

                    }
                }
                ViewBag.CurrentMonthReport = empinfos;
                return View();
            }

            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }


        [HttpGet]
        public ActionResult CustomReport(string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if (Authenticate("CustomReport"))
            {
                ViewBag.AllFinancialYears = eosDataDbContext.EosDataDbSet.OrderBy(model => model.year).Select(model => model.year).Distinct().ToList();
                ViewBag.AllFinancialMonths = months;
                return View();
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CustomReport(string sessionid, string fyear, string fmonth)
        {
            ViewBag.AllFinancialYears = eosDataDbContext.EosDataDbSet.OrderBy(model => model.year).Select(model => model.year).Distinct().ToList();
            ViewBag.AllFinancialMonths = months;
            ViewBag.SessionID = sessionid;
            if (Authenticate("CustomReport"))
            {
                if (fyear != null && fmonth != null)
                {

                    string[] projnames = projectDataDbContext.ProjectDataDbSet.Where(model => model.projstatus.Equals("Active")).OrderBy(model => model.projname).Select(model => model.projname).ToArray();

                    string[] empnames = employeeInfoDataDbContext.EmployeeInfoDataDbSet.OrderBy(model => model.empid).Select(model => model.empname).ToArray();
                    string[] empids = employeeInfoDataDbContext.EmployeeInfoDataDbSet.OrderBy(model => model.empid).Select(model => model.empid).ToArray();

                    string[,] empinfos = new string[(projnames.Length) + 2, (empnames.Length) + 1];

                    for (int i = 0; i < (projnames.Length) + 2; i++)
                    {
                        for (int j = 0; j < (empnames.Length) + 1; j++)
                        {
                            if (i < 2)
                            {
                                if (j == 0)
                                    empinfos[i, j] = " ";
                                else if (i == 0)
                                    empinfos[i, j] = empnames[j - 1];
                                else
                                    empinfos[i, j] = empids[j - 1];
                            }
                            else
                            {
                                if (j == 0)
                                    empinfos[i, j] = projnames[i - 2];
                                else
                                {
                                    string tempempid = empids[j - 1];
                                    string tempprojname = projnames[i - 2];
                                    var WorkpercentValues = eosDataDbContext.EosDataDbSet.Where(model => model.empid.Equals(tempempid) && model.projname.Equals(tempprojname) && model.year.Equals(fyear)).FirstOrDefault();

                                    if (WorkpercentValues != null)
                                        empinfos[i, j] = eosDataDbContext.Entry(WorkpercentValues).Property(fmonth).CurrentValue.ToString();
                                    else
                                        empinfos[i, j] = "-";
                                }
                            }

                        }
                    }
                    ViewBag.CustomYear = fyear;
                    ViewBag.CustomMonth = fmonth;
                    ViewBag.CustomReport = empinfos;
                    return View();
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }


    }
}