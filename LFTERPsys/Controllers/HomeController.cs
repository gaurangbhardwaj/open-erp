using LFTERPsys.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LFTERPsys.Controllers
{
    public class HomeController : Controller
    {
        private readonly CalendarDbContext CalendarDbContext = new CalendarDbContext();
        private readonly EmployeeInfoDataDbContext employeeInfoDataDbContext = new EmployeeInfoDataDbContext();
        private readonly AnnouncementDataDbContext announcementDataDbContext = new AnnouncementDataDbContext();
        private readonly ProjectDataDbContext projectDataDbContext = new ProjectDataDbContext();

        private void Authenticate()
        {
            if (Session["LoggedEmpID" + ViewBag.SessionID] == null)
            {
                Response.Redirect(new Uri(Request.Url, Url.Action("Signin", "Login")).ToString());
            }
        }

        public ActionResult Home(string sessionid)
        {
            ViewBag.SessionID = sessionid;
            Authenticate();
            ViewBag.projno = projectDataDbContext.ProjectDataDbSet.Select(model => model.projname).Count();
            ViewBag.AllAnnouncements = announcementDataDbContext.AnnouncementDataDbSet.Where(model => model.date.Month.Equals(DateTime.Now.Month)).ToList();
            return View();
        }

        public JsonResult GetEvents()
        {
            var events = CalendarDbContext.CalendarDbSet.ToList();
            return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public async Task<ActionResult> MyProfile(string sessionid)
        {
            ViewBag.SessionID = sessionid;
            Authenticate();
            try
            {
                using (EmployeeInfoDataDbContext employeeInfoDataDbContext = new EmployeeInfoDataDbContext())
                {
                    EmployeeInfoDataModel employeeInfoDataModel = await employeeInfoDataDbContext.EmployeeInfoDataDbSet.Where(model => model.empid.Equals(sessionid)).FirstOrDefaultAsync();
                    if (employeeInfoDataModel == null)
                    {
                        return HttpNotFound();
                    }
                    return View(employeeInfoDataModel);
                }
            }
            catch
            {
                return HttpNotFound();
            }
        }

        public ActionResult Logout(string sessionid)
        {
            ViewBag.SessionID = sessionid;
            Session.Remove("LoggedEmpID" + ViewBag.SessionID);
            return RedirectToAction("Signin", "Login");
        }

    }
}