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
    public class DesignationDataController : Controller
    {
        private DesignationDataDbContext designationDataDbContext = new DesignationDataDbContext();

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


        public JsonResult IsDesgNameExist(string desgname)
        {
            //check if any of the UserName matches the UserName specified in the Parameter using the ANY extension method.  
            bool check = !designationDataDbContext.DesignationDataDbSet.Any(model => model.desgname.ToUpper().Equals(desgname.ToUpper()));
            return Json(check, JsonRequestBehavior.AllowGet);
        }

       
        public async Task<ActionResult> DesignationDataIndex(String sessionid)
        {
            ViewBag.SessionID = sessionid;
            if(Authenticate("DesignationData"))
            {
                ViewBag.Desgnames = await designationDataDbContext.DesignationDataDbSet.OrderBy(model => model.desgname).Select(model => model.desgname).ToListAsync();
                return View();
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }

 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DesignationDataIndex(DesignationDataModel designationDataModel, String sessionid)
        {
            ViewBag.SessionID = sessionid;
            if(Authenticate("DesignationData"))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        designationDataDbContext.DesignationDataDbSet.Add(designationDataModel);
                        await designationDataDbContext.SaveChangesAsync();
                        return RedirectToAction("DesignationDataIndex", new { sessionid = ViewBag.SessionID });
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

        

        [HttpPost, ValidateHeaderAntiForgeryToken]
        public async Task<JsonResult> DesignationDataDelete(string desgname, string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if(Authenticate("DesignationData"))
            {
                try
                {
                    if (desgname != null)
                    {
                        DesignationDataModel designationDataModel = await designationDataDbContext.DesignationDataDbSet.Where(model => model.desgname.Equals(desgname)).FirstOrDefaultAsync();
                        designationDataDbContext.DesignationDataDbSet.Remove(designationDataModel);
                        await designationDataDbContext.SaveChangesAsync();
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
                designationDataDbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
