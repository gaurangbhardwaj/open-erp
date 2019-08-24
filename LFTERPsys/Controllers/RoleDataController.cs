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
    public class RoleDataController : Controller
    {
        private RoleDataDbContext roleDataDbContext = new RoleDataDbContext();

        private void Authenticate()
        {
            if (Session["LoggedEmpID" + ViewBag.SessionID] == null)
            {

                Response.Redirect(new Uri(Request.Url, Url.Action("Signin", "Login")).ToString());
            }
            else if(Session["LoggedEmpRole" + ViewBag.SessionID] != "superuser")
            {
                Response.Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
            }
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
        public JsonResult IsRoleNameExist(string rolename)
        {
            //check if any of the EmployeeID matches the EmployeeID specified in the Parameter using the ANY extension method.  
            return Json(!roleDataDbContext.RoleDataDbSet.Any(model => model.rolename.ToUpper().Equals(rolename.ToUpper())), JsonRequestBehavior.AllowGet);
        }

        // GET: RoleData
        public async Task<ActionResult> RoleDataIndex(string sessionid)
        {
            ViewBag.SessionID = sessionid;
            Authenticate();
            return View(await roleDataDbContext.RoleDataDbSet.OrderBy(model=>model.rolename).ToListAsync());
        }


        // GET: RoleData/Create
        public ActionResult RoleDataCreate(string sessionid)
        {
            ViewBag.SessionID = sessionid;
            Authenticate();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RoleDataCreate(RoleDataModel roleDataModel, string sessionid)
        {
            ViewBag.SessionID = sessionid;
            Authenticate();
            try
            {
                if (ModelState.IsValid)
            {
                roleDataDbContext.RoleDataDbSet.Add(roleDataModel);
                await roleDataDbContext.SaveChangesAsync();
                return RedirectToAction("RoleDataIndex", new { sessionid = ViewBag.SessionID });
            }
            }
            catch
            {
                Response.Write("<script>alert('Server Error!!');</script>");
                ViewBag.errormssg = "Server Error!!";
            }

            return View(roleDataModel);
        }

       
        public async Task<ActionResult> RoleDataEdit(string id, string sessionid)
        {
            ViewBag.SessionID = sessionid;
            Authenticate();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleDataModel roleDataModel = await roleDataDbContext.RoleDataDbSet.Where(model => model.rolename.Equals(id)).FirstOrDefaultAsync();
            if (roleDataModel == null)
            {
                return HttpNotFound();
            }
            return View(roleDataModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RoleDataEdit(RoleDataModel roleDataModel, string sessionid)
        {
            ViewBag.SessionID = sessionid;
            Authenticate();
            try
            {
                if (ModelState.IsValid)
                {
                roleDataDbContext.Entry(roleDataModel).State = EntityState.Modified;
                await roleDataDbContext.SaveChangesAsync();
                return RedirectToAction("RoleDataIndex",new { sessionid = ViewBag.SessionID });
                }
            }
            catch
            {
                Response.Write("<script>alert('Server Error!!');</script>");
                ViewBag.errormssg = "Server Error!!";
            }
            return View(roleDataModel);
        }

        // POST: RoleData/Delete/5

        [HttpPost, ValidateHeaderAntiForgeryToken]
        public async Task<JsonResult> RoleDataDelete(string rolename, string sessionid)
        {
            ViewBag.SessionID = sessionid;
            Authenticate();
            try
            {
                if (rolename != null)
                {
                    RoleDataModel roleDataModel = await roleDataDbContext.RoleDataDbSet.Where(model => model.rolename.Equals(rolename)).FirstOrDefaultAsync();
                    roleDataDbContext.RoleDataDbSet.Remove(roleDataModel);
                    await roleDataDbContext.SaveChangesAsync();
                    return Json("success");
                }
            }
            catch
            {
                return Json("fail");
            }
            return Json("fail");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                roleDataDbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
