using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LFTERPsys.Models;
using System.Web.Helpers;

namespace LFTERPsys.Controllers
{
    public class AnnouncementDataController : Controller
    {
        private AnnouncementDataDbContext announcementDataDbContext = new AnnouncementDataDbContext();

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


        public async Task<ActionResult> AnnouncementDataIndex(string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if (Authenticate("AnnouncementDataIndex"))
            {
                return View(await announcementDataDbContext.AnnouncementDataDbSet.ToListAsync());
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }


        public ActionResult AnnouncementDataCreate(string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if (Authenticate("AnnouncementDataCreate"))
            {
                return View();
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AnnouncementDataCreate(AnnouncementDataModel announcementDataModel, string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if (Authenticate("AnnouncementDataCreate"))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        announcementDataModel.date = DateTime.Now.Date;
                        announcementDataDbContext.AnnouncementDataDbSet.Add(announcementDataModel);
                        await announcementDataDbContext.SaveChangesAsync();
                        return RedirectToAction("AnnouncementDataIndex", new { sessionid = ViewBag.SessionID });
                    }
                }
                catch
                {
                    Response.Write("<script>alert('Server Error!!');</script>");
                    ViewBag.errormssg = "Server Error!!";
                }
                return View(announcementDataModel);
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }


        public async Task<ActionResult> AnnouncementDataEdit(string sessionid, int? id)
        {
            ViewBag.SessionID = sessionid;
            if (Authenticate("AnnouncementDataEdit"))
            { 
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                AnnouncementDataModel announcementDataModel = await announcementDataDbContext.AnnouncementDataDbSet.FindAsync(id);
                if (announcementDataModel == null)
                {
                    return HttpNotFound();
                }
                return View(announcementDataModel);
            }
           return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID})).ToString());   
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AnnouncementDataEdit(AnnouncementDataModel announcementDataModel, string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if (Authenticate("AnnouncementDataEdit"))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        announcementDataDbContext.Entry(announcementDataModel).State = EntityState.Modified;
                        await announcementDataDbContext.SaveChangesAsync();
                        return RedirectToAction("AnnouncementDataIndex", new { sessionid = ViewBag.SessionID });
                    }
                }
                catch
                {
                    Response.Write("<script>alert('Server Error!!');</script>");
                    ViewBag.errormssg = "Server Error!!";
                }
                return View(announcementDataModel);
            }

            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }


        [HttpPost, ValidateHeaderAntiForgeryToken]
        public async Task<JsonResult> AnnouncementDataDelete(string sessionid, int? id)
        {
            ViewBag.SessionID = sessionid;
            if (Authenticate("ProjectDataDelete"))
            {
                try
                {
                    if(id != null)
                    {
                        AnnouncementDataModel announcementDataModel = await announcementDataDbContext.AnnouncementDataDbSet.FindAsync(id);
                        announcementDataDbContext.AnnouncementDataDbSet.Remove(announcementDataModel);
                        await announcementDataDbContext.SaveChangesAsync();
                        return Json("success");
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
                announcementDataDbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
