using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using LFTERPsys.Models;
using System.Web.Helpers;

namespace LFTERPsys.Controllers
{
    public class DivisionDataController : Controller
    {
        private DivisionDataDbContext divisionDataDbContext = new DivisionDataDbContext();

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

        public JsonResult IsDivNameExist(string divname)
        {
            //check if any of the UserName matches the UserName specified in the Parameter using the ANY extension method.  
            bool check = !divisionDataDbContext.DivisionDataDbSet.Any(model => model.divname.ToUpper().Equals(divname.ToUpper()));
            return Json(check, JsonRequestBehavior.AllowGet);
        }

        // GET: DivisionData
        public async Task<ActionResult> DivisionDataIndex(String sessionid)
        {
            ViewBag.SessionID = sessionid;
            if(Authenticate("DivisionData"))
            {
                ViewBag.Divnames = await divisionDataDbContext.DivisionDataDbSet.OrderBy(model => model.divname).Select(model => model.divname).ToListAsync();
                return View();
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }

        // POST: DivisionData/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DivisionDataIndex(DivisionDataModel divisionDataModel, String sessionid)
        {
            ViewBag.SessionID = sessionid;
            if(Authenticate("DivisionData"))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        divisionDataDbContext.DivisionDataDbSet.Add(divisionDataModel);
                        await divisionDataDbContext.SaveChangesAsync();
                        return RedirectToAction("DivisionDataIndex", new { sessionid = ViewBag.SessionID });
                    }
                }
                catch
                {
                    Response.Write("<script > alert('Server Error!!');</ script >");
                    ViewBag.errormssg = "Server Error!!";
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }

        // POST: DivisionData/Delete/5
        [HttpPost, ValidateHeaderAntiForgeryToken]
        public async Task<JsonResult> DivisionDataDelete(string divname, string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if(Authenticate("DivisionData"))
            {
                try
                {
                    if (divname != null)
                    {
                        DivisionDataModel divisionDataModel = await divisionDataDbContext.DivisionDataDbSet.Where(model => model.divname.Equals(divname)).FirstOrDefaultAsync();
                        divisionDataDbContext.DivisionDataDbSet.Remove(divisionDataModel);
                        await divisionDataDbContext.SaveChangesAsync();
                        return Json("success");
                    }
                }
                catch
                {
                    return Json("fail");
                }
            }
            return Json("fail");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                divisionDataDbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
