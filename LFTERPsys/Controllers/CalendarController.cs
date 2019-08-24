using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using LFTERPsys.Models;

namespace LFTERPsys.Controllers
{
    public class CalendarController : Controller
    {
        private CalendarDbContext CalendarDbContext = new CalendarDbContext();

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

        // GET: LofruCalendar
        public ActionResult CalendarIndex(string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if(Authenticate("CalendarData"))
            {
                return View();
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }

        public JsonResult GetEvents()
        {
            var events = CalendarDbContext.CalendarDbSet.ToList();
            return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet }; 
        }

        [HttpPost]
        public JsonResult SaveEvent(CalendarModel e)
        {
            var status = false;
            //System.Diagnostics.Debug.WriteLine("Save Function called!!" +e.eventid);
            if (e.eventid > 0)
            {
                //Update the event
                var v = CalendarDbContext.CalendarDbSet.Where(a => a.eventid == e.eventid).FirstOrDefault();
                if (v != null)
                {
                    v.subject = e.subject;
                    v.startdate = e.startdate;
                    v.enddate = e.enddate;
                    v.description = e.description;
                    v.themecolor = e.themecolor;
                }
            }
            else
            {
                CalendarDbContext.CalendarDbSet.Add(e);
            }
                CalendarDbContext.SaveChangesAsync();
                status = true;
            
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            var status = false;
            {
                var v = CalendarDbContext.CalendarDbSet.Where(a => a.eventid == eventID).FirstOrDefault();
                if (v != null)
                {
                    CalendarDbContext.CalendarDbSet.Remove(v);
                    CalendarDbContext.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CalendarDbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
