using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using LFTERPsys.Models;
using System;
using System.Linq;
using System.Web.Helpers;

namespace LFTERPsys.Controllers
{
    public class DepartmentDataController : Controller
    {
        private DepartmentDataDbContext departmentDataDbContext = new DepartmentDataDbContext();

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

        public JsonResult IsDepNameExist(string depname)
        {
            //check if any of the UserName matches the UserName specified in the Parameter using the ANY extension method.

            bool check = !departmentDataDbContext.DepartmentDataDbSet.Any(model => model.depname.ToUpper().Equals(depname.ToUpper()));
            return Json(check, JsonRequestBehavior.AllowGet);
        }

        // GET: DepartmentData

        public async Task<ActionResult> DepartmentDataIndex(String sessionid)
        {
            ViewBag.SessionID = sessionid;
            if(Authenticate("DepartmentData"))
            {
                ViewBag.Depnames = await departmentDataDbContext.DepartmentDataDbSet.OrderBy(model => model.depname).Select(model => model.depname).ToListAsync();
                return View();
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DepartmentDataIndex(DepartmentDataModel departmentDataModel, String sessionid)
        {
            ViewBag.SessionID = sessionid;
            if(Authenticate("DepartmentData"))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        departmentDataDbContext.DepartmentDataDbSet.Add(departmentDataModel);
                        await departmentDataDbContext.SaveChangesAsync();
                        return RedirectToAction("DepartmentDataIndex", new { sessionid = ViewBag.SessionID });
                    }
                }
                catch
                {
                    Response.Write("<script>alert('Server Error!!');</script>");
                    ViewBag.errormssg = "Server Error!!";
                }

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }


        [HttpPost, ValidateHeaderAntiForgeryToken]
        public async Task<JsonResult> DepartmentDataDelete(string depname, string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if(Authenticate("DepartmentData"))
            {
                try
                {
                    if (depname != null)
                    {
                        DepartmentDataModel departmentDataModel = await departmentDataDbContext.DepartmentDataDbSet.Where(model => model.depname.Equals(depname)).FirstOrDefaultAsync();
                        departmentDataDbContext.DepartmentDataDbSet.Remove(departmentDataModel);
                        await departmentDataDbContext.SaveChangesAsync();
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
                departmentDataDbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
