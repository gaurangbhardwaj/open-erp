using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using LFTERPsys.Models;
using System.Web.Helpers;
using System.IO;

namespace LFTERPsys.Controllers
{
   public class EmployeeLoginDataController : Controller
    {
        private EmployeeLoginDataDbContext employeeLoginDataDbContext = new EmployeeLoginDataDbContext();
        private readonly EmployeeInfoDataDbContext employeeInfoDataDbContext = new EmployeeInfoDataDbContext();
        private readonly RoleDataDbContext roleDataDbContext = new RoleDataDbContext();

        //------------------------------------Encryption&Decryption---------------------------------------------
        private string Encrypt(string str)
        {
            string EncrptKey = "0806;[pnuLIT)lOFRUtEChnologieS";
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byKey = System.Text.Encoding.UTF8.GetBytes(EncrptKey.Substring(0, 8));
            System.Security.Cryptography.DESCryptoServiceProvider des = new System.Security.Cryptography.DESCryptoServiceProvider();
            byte[] inputByteArray = System.Text.Encoding.UTF8.GetBytes(str);
            MemoryStream ms = new System.IO.MemoryStream();
            System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(ms, des.CreateEncryptor(byKey, IV), System.Security.Cryptography.CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }
        //------------------------------------------------------------------------------------------------------

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


        public async Task<ActionResult> EmployeeLoginDataIndex(string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if (Authenticate("EmployeeLoginData"))
            {
                return View(await employeeLoginDataDbContext.EmployeeLoginDataDbSet.OrderBy(model=>model.empid).ToListAsync());
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }



        public async Task<ActionResult> EmployeeLoginDataEdit(string id, string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if (Authenticate("EmployeeLoginData"))
            {
                if (id == null)
                {
                  return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                 EmployeeLoginDataModel employeeLoginDataModel = await employeeLoginDataDbContext.EmployeeLoginDataDbSet.FindAsync(id);
                 if (employeeLoginDataModel == null)
                 {
                  return HttpNotFound();
                 }
                 ViewBag.AllRoles = new SelectList(await roleDataDbContext.RoleDataDbSet.OrderBy(model=>model.rolename).Where(model=>model.rolename != "superuser").Select(model => model.rolename).ToListAsync());
                 return View(employeeLoginDataModel);
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeLoginDataEdit(EmployeeLoginDataModel employeeLoginDataModel, string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if (Authenticate("EmployeeLoginData") && employeeLoginDataModel.role != "superuser")
            {
                try
                {
                    if (ModelState.IsValid)
                    {

                        var oldvalues = await employeeLoginDataDbContext.EmployeeLoginDataDbSet.Where(model => model.empid.Equals(employeeLoginDataModel.empid)).FirstOrDefaultAsync();
                        oldvalues.role = employeeLoginDataModel.role;

                        if(oldvalues != null)
                        {
                            if (oldvalues.role != "superuser")
                            {
                                await employeeLoginDataDbContext.SaveChangesAsync();
                                return RedirectToAction("EmployeeLoginDataIndex", new { sessionid = ViewBag.SessionID });
                            }
                        }
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                }

                catch
                {
                    Response.Write("<script>alert('Server Error!!');</script>");
                    ViewBag.errormssg = "Server Error!!";
                }
                ViewBag.AllRoles = new SelectList(await roleDataDbContext.RoleDataDbSet.OrderBy(model => model.rolename).Where(model=>model.rolename != "superuser").Select(model => model.rolename).ToListAsync());
                return View(employeeLoginDataModel);
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }


        [HttpPost, ValidateHeaderAntiForgeryToken]
        public async Task<JsonResult> EmployeeLoginDataPasswordReset(string empid, string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if (Authenticate("EmployeeLoginData"))
            {
                try
                {
                    if (empid != null)
                    {
                        var oldvalues = await employeeLoginDataDbContext.EmployeeLoginDataDbSet.Where(model => model.empid.Equals(empid)).FirstOrDefaultAsync();
                        oldvalues.password = Encrypt("welcome@1234");
                        await employeeLoginDataDbContext.SaveChangesAsync();
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
                employeeLoginDataDbContext.Dispose();
                employeeInfoDataDbContext.Dispose();
                roleDataDbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
