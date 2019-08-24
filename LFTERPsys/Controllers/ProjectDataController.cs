using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using LFTERPsys.Models;
using System.Web.Helpers;
using System.Collections.Generic;

namespace LFTERPsys.Controllers
{
    public class ProjectDataController : Controller
    {
        private ProjectDataDbContext projectDataDbContext = new ProjectDataDbContext();
        private readonly DepartmentDataDbContext departmentDataDbContext = new DepartmentDataDbContext();

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

        public JsonResult IsProjIdExist(string projid)
        {
            //check if any of the EmployeeID matches the EmployeeID specified in the Parameter using the ANY extension method.  
            return Json(!projectDataDbContext.ProjectDataDbSet.Any(model => model.projid.ToUpper().Equals(projid.ToUpper())), JsonRequestBehavior.AllowGet);
        }


        // GET: ProjectData
        public async Task<ActionResult> ProjectDataIndex(string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if (Authenticate("ProjectDataIndex"))
            {
                return View(await projectDataDbContext.ProjectDataDbSet.OrderBy(model => model.projname).ToListAsync());
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }

        
        public async Task<ActionResult> ProjectDataCreate(string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if (Authenticate("ProjectDataCreate"))
            {
                ViewBag.AllDepartmentName = new SelectList(await departmentDataDbContext.DepartmentDataDbSet.OrderBy(model => model.depname).Select(model => model.depname).ToListAsync());
                
                return View();
            }

            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProjectDataCreate(string sessionid, ProjectDataModel projectDataModel)

        {
            ViewBag.SessionID = sessionid;
            ViewBag.AllDepartmentName = new SelectList(await departmentDataDbContext.DepartmentDataDbSet.OrderBy(model => model.depname).Select(model => model.depname).ToListAsync());

            if (Authenticate("ProjectDataCreate"))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        projectDataDbContext.ProjectDataDbSet.Add(projectDataModel);
                        await projectDataDbContext.SaveChangesAsync();
                        return RedirectToAction("ProjectDataIndex", new { sessionid = ViewBag.SessionID });
                    }
                }
                catch
                {
                    Response.Write("<script>alert('Server Error!!');</script>");
                    ViewBag.errormssg = "Server Error!!";
                }
                return View(projectDataModel);
            }
            
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }

        
        public async Task<ActionResult> ProjectDataEdit(string id, string sessionid)
        {
            ViewBag.SessionID = sessionid;
            ViewBag.AllDepartmentName = new SelectList(await departmentDataDbContext.DepartmentDataDbSet.OrderBy(model => model.depname).Select(model => model.depname).ToListAsync());
            ViewBag.AllProjectStatus = new SelectList(new List<string>() { "Active", "InActive" });

            if(Authenticate("ProjectDataEdit"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ProjectDataModel projectDataModel = await projectDataDbContext.ProjectDataDbSet.Where(model => model.projid.Equals(id)).FirstOrDefaultAsync();
                if (projectDataModel == null)
                {
                    return HttpNotFound();
                }
                return View(projectDataModel);
            }
            
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }

  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProjectDataEdit(string sessionid, ProjectDataModel projectDataModel)
        {
            ViewBag.SessionID = sessionid;
            ViewBag.AllDepartmentName = new SelectList(await departmentDataDbContext.DepartmentDataDbSet.OrderBy(model => model.depname).Select(model => model.depname).ToListAsync());
            ViewBag.AllProjectStatus = new SelectList(new List<string>() { "Active", "InActive" });


            if(Authenticate("ProjectDataEdit"))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        projectDataDbContext.Entry(projectDataModel).State = EntityState.Modified;
                        await projectDataDbContext.SaveChangesAsync();
                        return RedirectToAction("ProjectDataIndex", new { sessionid = ViewBag.SessionID });
                    }
                }
                catch
                {
                    Response.Write("<script>alert('Server Error!!');</script>");
                    ViewBag.errormssg = "Server Error!!";
                }

                return View(projectDataModel);
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }


        [HttpPost, ValidateHeaderAntiForgeryToken]
        public async Task<JsonResult> ProjectDataDelete(string projid, string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if (Authenticate("ProjectDataDelete"))
            {
                try
                {
                    if (projid != null)
                    {
                        ProjectDataModel projectDataModel = await projectDataDbContext.ProjectDataDbSet.Where(model => model.projid.Equals(projid)).FirstOrDefaultAsync();
                        projectDataDbContext.ProjectDataDbSet.Remove(projectDataModel);
                        await projectDataDbContext.SaveChangesAsync();
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
                projectDataDbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
