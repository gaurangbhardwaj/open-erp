using LFTERPsys.Models;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Helpers;

namespace LFTERPsys.Controllers
{
    public class EosDataController : Controller
    {
        private EosDataDbContext eosDataDbContext = new EosDataDbContext();
        private readonly ProjectDataDbContext projectDataDbContext = new ProjectDataDbContext();
        private readonly EmployeeInfoDataDbContext employeeInfoDataDbContext = new EmployeeInfoDataDbContext();

        private DateTime currentdate = DateTime.Now;
        string[] months = { "april", "may", "june", "july", "august", "september", "october", "november", "december", "january", "february", "march" };
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


        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
        private sealed class ValidateHeaderAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationContext filterContext)
            {
                if (filterContext == null)
                {
                    throw new HttpAntiForgeryException("Anti Forgery Not Authenticated");
                }

                var httpContext = filterContext.HttpContext;
                var cookie = httpContext.Request.Cookies[AntiForgeryConfig.CookieName];
                try
                {
                    AntiForgery.Validate(cookie != null ? cookie.Value : null, httpContext.Request.Headers["__RequestVerificationToken"]);
                }
                catch
                {
                    filterContext.Result = new HttpStatusCodeResult(403);
                }
            }
        }

        
        [HttpGet]
        public async Task<ActionResult> MyEos(string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if(Authenticate("MyEos"))
            {
                currentFinancialYear = CurrentFinancialYearChecker();
                try
                {
                    ViewBag.AllProjectName = new SelectList(await projectDataDbContext.ProjectDataDbSet.Where(model => model.projstatus.Equals("Active")).OrderBy(model => model.projname).Select(model => model.projname).ToListAsync());
                    return View(await eosDataDbContext.EosDataDbSet.Where(model => model.empid.Equals(sessionid) && model.year.Equals(currentFinancialYear)).OrderBy(model => model.sno).ToListAsync());
                }
                catch
                {
                    Response.Write("<script>alert('Server error timeout!');</script>");
                }
                return HttpNotFound();
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }



        [HttpPost, ValidateHeaderAntiForgeryToken]
        public async Task<JsonResult> MyEos(List<EosDataModel> eosDataModels, string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if(Authenticate("MyEos"))
            {
                currentFinancialYear = CurrentFinancialYearChecker();

                try
                {
                    if (eosDataModels != null)
                    {
                        EosDataModel oldvalues = new EosDataModel();
                        foreach (EosDataModel eosDataModel in eosDataModels)
                        {
                            oldvalues = eosDataDbContext.EosDataDbSet.Where(model => model.empid.Equals(eosDataModel.empid) && model.projname.Equals(eosDataModel.projname)).FirstOrDefault();

                            if (oldvalues != null) // for updation of old values
                            {
                                if (currentdate.Month >= 1 && currentdate.Month <= 3) // for Jan, Feb and Mar Financial Month
                                {
                                    for (int i = (currentdate.Month) + 8; i < 12; i++) // Current Month till Mar
                                    {
                                        eosDataDbContext.Entry(oldvalues).Property(months[i]).CurrentValue = eosDataDbContext.Entry(eosDataModel).Property(months[i]).CurrentValue;
                                    }
                                }
                                else
                                {
                                    for (int i = (currentdate.Month) - 4; i < 12; i++)
                                    {
                                        eosDataDbContext.Entry(oldvalues).Property(months[i]).CurrentValue = eosDataDbContext.Entry(eosDataModel).Property(months[i]).CurrentValue;
                                    }
                                }
                            }
                            else // for adding new values
                            {
                                if (currentdate.Month >= 1 && currentdate.Month <= 3) // for Jan, Feb and Mar Financial Month
                                {
                                    for (int i = 0; i < (currentdate.Month) + 8; i++) // April Month till < Current Month
                                    {
                                        eosDataDbContext.Entry(eosDataModel).Property(months[i]).CurrentValue = 0;
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < (currentdate.Month) - 4; i++) // April Month till < Current Month
                                    {
                                        eosDataDbContext.Entry(eosDataModel).Property(months[i]).CurrentValue = 0;
                                    }
                                }
                                eosDataModel.year = currentFinancialYear;
                                eosDataDbContext.EosDataDbSet.Add(eosDataModel);
                            }
                        }

                        await eosDataDbContext.SaveChangesAsync();

                        return Json("Success");
                    }

                }
                catch
                {
                    return Json("Fail");
                }
            }
            return Json("Fail");
        }



        [HttpGet]
        public async Task<ActionResult> AllEmployeesEos(string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if(Authenticate("AllEmployeesEos"))
            {
                try
                {
                    ViewBag.AllProjectName = new SelectList(await projectDataDbContext.ProjectDataDbSet.Where(model => model.projstatus.Equals("Active")).OrderBy(model => model.projname).Select(model => model.projname).ToListAsync());
                    return View(await employeeInfoDataDbContext.EmployeeInfoDataDbSet.OrderBy(model => model.empid).ToListAsync());
                }
                catch
                {
                    Response.Write("<script>alert('Server error timeout!');</script>");
                }
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }


        [HttpGet]
        public async Task<ActionResult> EmployeeEos(string sessionid, string id)
        {
            ViewBag.SessionID = sessionid;
            ViewBag.EmpID = id;
            ViewBag.empname = await (employeeInfoDataDbContext.EmployeeInfoDataDbSet.Where(model => model.empid.Equals(id)).Select(model => model.empname).FirstOrDefaultAsync());
            if(Authenticate("AllEmployeesEos"))
            {
                currentFinancialYear = CurrentFinancialYearChecker();

                try
                {
                    if (id != null)
                    {
                        ViewBag.AllProjectName = new SelectList(await projectDataDbContext.ProjectDataDbSet.Where(model => model.projstatus.Equals("Active")).OrderBy(model => model.projname).Select(model => model.projname).ToListAsync());
                        return View(await eosDataDbContext.EosDataDbSet.Where(model => model.empid.Equals(id) && model.year.Equals(currentFinancialYear)).OrderBy(model => model.sno).ToListAsync());
                    }
                }
                catch
                {
                    Response.Write("<script>alert('Server error timeout!');</script>");
                }
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }


        [HttpPost, ValidateHeaderAntiForgeryToken]
        public async Task<JsonResult> EmployeeEos(List<EosDataModel> eosDataModels, string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if(Authenticate("AllEmployeesEos"))
            {
                currentFinancialYear = CurrentFinancialYearChecker();

                try
                {
                    if (eosDataModels != null)
                    {
                        EosDataModel oldvalues = new EosDataModel();
                        foreach (EosDataModel eosDataModel in eosDataModels)
                        {
                            oldvalues = eosDataDbContext.EosDataDbSet.Where(model => model.empid.Equals(eosDataModel.empid) && model.projname.Equals(eosDataModel.projname)).FirstOrDefault();
                            if (oldvalues != null)
                            {
                                for (int i = 0; i < 12; i++)
                                {
                                    eosDataDbContext.Entry(oldvalues).Property(months[i]).CurrentValue = eosDataDbContext.Entry(eosDataModel).Property(months[i]).CurrentValue;
                                }
                            }
                            else
                            {
                                eosDataModel.year = currentFinancialYear;
                                eosDataDbContext.EosDataDbSet.Add(eosDataModel);
                            }
                        }
                        await eosDataDbContext.SaveChangesAsync();

                        return Json("Success");
                    }
                }
                catch
                {
                    return Json("Fail");
                }
            }
            return Json("Fail");
        }


        [HttpPost, ValidateHeaderAntiForgeryToken]
        public async Task<JsonResult> EmployeeEosDelete(string empid, string projname, string sessionid)
        {
            System.Diagnostics.Debug.WriteLine(projname);
            ViewBag.SessionID = sessionid;
            if(Authenticate("AllEmployeesEos"))
            {
                try
                {
                    if (empid != null && projname != null)
                    {
                        EosDataModel eosDataModel = await eosDataDbContext.EosDataDbSet.Where(model => model.empid.Equals(empid) && model.projname.Equals(projname)).FirstOrDefaultAsync();
                        eosDataDbContext.EosDataDbSet.Remove(eosDataModel);
                        await eosDataDbContext.SaveChangesAsync();
                        return Json("Success");
                    }
                    else
                    {
                        return Json("Fail");
                    }
                }
                catch
                {
                    return Json("Fail");
                }
            }
            return Json("Fail");

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                eosDataDbContext.Dispose();
                projectDataDbContext.Dispose();
                employeeInfoDataDbContext.Dispose();

            }
            base.Dispose(disposing);
        }
    }
}