using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LFTERPsys.Models;
using System.IO;
using System.Collections.Generic;

namespace LFTERPsys.Controllers
{
    public class EmployeeInfoDataController : Controller
    {
        private EmployeeInfoDataDbContext employeeInfoDataDbContext = new EmployeeInfoDataDbContext();
        private EmployeeLoginDataDbContext employeeLoginDataDbContext = new EmployeeLoginDataDbContext();
        private readonly DepartmentDataDbContext departmentDataDbContext = new DepartmentDataDbContext();
        private readonly DivisionDataDbContext divisionDataDbContext = new DivisionDataDbContext();
        private readonly DesignationDataDbContext designationDataDbContext = new DesignationDataDbContext();

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

        private byte[] ConvertToByte(HttpPostedFileBase file)
        {
            byte[] imageByte = null;
            BinaryReader rdr = new BinaryReader(file.InputStream);
            imageByte = rdr.ReadBytes((int)file.ContentLength);
            return imageByte;
        }


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

        public JsonResult IsEmpIdExist(string empid)
        {
            //check if any of the UserName matches the UserName specified in the Parameter using the ANY extension method.  
            return Json(!employeeInfoDataDbContext.EmployeeInfoDataDbSet.Any(model => model.empid.ToUpper().Equals(empid.ToUpper())), JsonRequestBehavior.AllowGet);
        }


        // GET: EmployeeInfoData
        public async Task<ActionResult> EmployeeInfoDataIndex(string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if (Authenticate("EmployeeInfoDataIndex"))
            {
                return View(await employeeInfoDataDbContext.EmployeeInfoDataDbSet.OrderBy(model => model.empid).ToListAsync());

            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }


        // GET: EmployeeInfoData/Details/sessionid/id
        public async Task<ActionResult> EmployeeInfoDataDetails(string sessionid, string id)
        {
            ViewBag.SessionID = sessionid;
            if (Authenticate("EmployeeInfoDataDetails"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                EmployeeInfoDataModel employeeInfoDataModel = await employeeInfoDataDbContext.EmployeeInfoDataDbSet.Where(model => model.empid.Equals(id)).FirstOrDefaultAsync();
                if (employeeInfoDataModel == null)
                {
                    return HttpNotFound();
                }
                return View(employeeInfoDataModel);
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }


        // GET: EmployeeInfoData/Create
        public async Task<ActionResult> EmployeeInfoDataCreate(string sessionid)
        {
            ViewBag.SessionID = sessionid;
            if (Authenticate("EmployeeInfoDataCreate"))
            {
                ViewBag.AllDepartmentName = new SelectList(await departmentDataDbContext.DepartmentDataDbSet.OrderBy(model => model.depname).Select(model => model.depname).ToListAsync());
                ViewBag.AllDesignationName = new SelectList(await designationDataDbContext.DesignationDataDbSet.OrderBy(model => model.desgname).Select(model => model.desgname).ToListAsync());
                ViewBag.AllDivisionName = new SelectList(await divisionDataDbContext.DivisionDataDbSet.OrderBy(model => model.divname).Select(model => model.divname).ToListAsync());
                ViewBag.AllReportingTo = new SelectList(await employeeInfoDataDbContext.EmployeeInfoDataDbSet.Where(model => model.empexitdate.Equals(null)).OrderBy(model => model.empname).Select(model => model.empname).ToListAsync());
                return View();
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }


        // POST: EmployeeInfoData/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeInfoDataCreate(EmployeeInfoDataModel employeeInfoDataModel, HttpPostedFileBase uploadedphoto, string sessionid)
        {
            ViewBag.SessionID = sessionid;
            ViewBag.AllDepartmentName = new SelectList(await departmentDataDbContext.DepartmentDataDbSet.OrderBy(model => model.depname).Select(model => model.depname).ToListAsync());
            ViewBag.AllDesignationName = new SelectList(await designationDataDbContext.DesignationDataDbSet.OrderBy(model => model.desgname).Select(model => model.desgname).ToListAsync());
            ViewBag.AllDivisionName = new SelectList(await divisionDataDbContext.DivisionDataDbSet.OrderBy(model => model.divname).Select(model => model.divname).ToListAsync());
            ViewBag.AllReportingTo = new SelectList(await employeeInfoDataDbContext.EmployeeInfoDataDbSet.Where(model => model.empexitdate.Equals(null)).OrderBy(model => model.empname).Select(model => model.empname).ToListAsync());
            if (Authenticate("EmployeeInfoDataCreate"))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (uploadedphoto != null)
                        {
                            employeeInfoDataModel.empphoto = ConvertToByte(uploadedphoto);
                        }
                        employeeInfoDataDbContext.EmployeeInfoDataDbSet.Add(employeeInfoDataModel);

                        // login id/password generation
                        EmployeeLoginDataModel employeeLoginDataModel = new EmployeeLoginDataModel();
                        employeeLoginDataModel.empid = employeeInfoDataModel.empid;
                        employeeLoginDataModel.password = Encrypt("welcome@1234");
                        //System.Diagnostics.Debug.WriteLine(encrypass);
                        employeeLoginDataModel.role = "user";
                        employeeLoginDataDbContext.EmployeeLoginDataDbSet.Add(employeeLoginDataModel);

                        await employeeLoginDataDbContext.SaveChangesAsync();
                        await employeeInfoDataDbContext.SaveChangesAsync();

                        return RedirectToAction("EmployeeInfoDataIndex", new { sessionid = ViewBag.SessionID });
                    }
                }
                catch
                {
                    Response.Write("<script>alert('Server Error!!');</script>");
                    ViewBag.errormssg = "Server Error!!";
                }
                return View(employeeInfoDataModel);
            }
            
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }
        

        public async Task<ActionResult> EmployeeInfoDataEdit(string sessionid, string id)
        {
            ViewBag.SessionID = sessionid;
            if (Authenticate("EmployeeInfoDataEdit"))
            {
                ViewBag.AllDepartmentName = new SelectList(await departmentDataDbContext.DepartmentDataDbSet.OrderBy(model => model.depname).Select(model => model.depname).ToListAsync());
                ViewBag.AllDesignationName = new SelectList(await designationDataDbContext.DesignationDataDbSet.OrderBy(model => model.desgname).Select(model => model.desgname).ToListAsync());
                ViewBag.AllDivisionName = new SelectList(await divisionDataDbContext.DivisionDataDbSet.OrderBy(model => model.divname).Select(model => model.divname).ToListAsync());
                ViewBag.AllReportingTo = new SelectList(await employeeInfoDataDbContext.EmployeeInfoDataDbSet.Where(model => model.empexitdate.Equals(null)).OrderBy(model => model.empname).Select(model => model.empname).ToListAsync());
                ViewBag.AllEmployeeStatus = new SelectList(new List<string>() { "Confirm", "Consultant", "Intern", "Probation" });
                ViewBag.AllEmployeeLocation = new SelectList(new List<string>() { "Banglore", "Gurugram" });
                ViewBag.AllEmployeeCategory = new SelectList(new List<string>() { "Non-Technical", "Technical" });

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                EmployeeInfoDataModel employeeInfoDataModel = await employeeInfoDataDbContext.EmployeeInfoDataDbSet.Where(model => model.empid.Equals(id)).FirstOrDefaultAsync();
                if (employeeInfoDataModel == null)
                {
                    return HttpNotFound();
                }

                return View(employeeInfoDataModel);
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeInfoDataEdit(EmployeeInfoDataModel employeeInfoDataModel, HttpPostedFileBase uploadedphoto, string sessionid)
        {
            ViewBag.SessionID = sessionid;
            ViewBag.AllDepartmentName = new SelectList(await departmentDataDbContext.DepartmentDataDbSet.OrderBy(model => model.depname).Select(model => model.depname).ToListAsync());
            ViewBag.AllDesignationName = new SelectList(await designationDataDbContext.DesignationDataDbSet.OrderBy(model => model.desgname).Select(model => model.desgname).ToListAsync());
            ViewBag.AllDivisionName = new SelectList(await divisionDataDbContext.DivisionDataDbSet.OrderBy(model => model.divname).Select(model => model.divname).ToListAsync());
            ViewBag.AllReportingTo = new SelectList(await employeeInfoDataDbContext.EmployeeInfoDataDbSet.Where(model => model.empexitdate.Equals(null)).OrderBy(model => model.empname).Select(model => model.empname).ToListAsync());
            ViewBag.AllEmployeeStatus = new SelectList(new List<string>() { "Confirm", "Consultant", "Intern", "Probation" });
            ViewBag.AllEmployeeLocation = new SelectList(new List<string>() { "Banglore", "Gurugram" });
            ViewBag.AllEmployeeCategory = new SelectList(new List<string>() { "Non-Technical", "Technical" });
            if(Authenticate("EmployeeInfoDataEdit"))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        employeeInfoDataDbContext.Entry(employeeInfoDataModel).State = EntityState.Modified;
                        if (uploadedphoto != null)
                        {
                            employeeInfoDataModel.empphoto = ConvertToByte(uploadedphoto);
                        }
                        await employeeInfoDataDbContext.SaveChangesAsync();
                        return RedirectToAction("EmployeeInfoDataIndex", new { sessionid = ViewBag.SessionID });
                    }
                }
                catch
                {
                    Response.Write("<script>alert('Server Error!!');</script>");
                    ViewBag.errormssg = "Server Error!!";
                }

                return View(employeeInfoDataModel);
            }

            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }



        public async Task<ActionResult> EmployeeInfoDataDelete(string sessionid, string id)
        {
            ViewBag.SessionID = sessionid;
            if (Authenticate("EmployeeInfoDataDelete"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                EmployeeInfoDataModel employeeInfoDataModel = await employeeInfoDataDbContext.EmployeeInfoDataDbSet.Where(model => model.empid.Equals(id)).FirstOrDefaultAsync();
                if (employeeInfoDataModel == null)
                {
                    return HttpNotFound();
                }
                return View(employeeInfoDataModel);
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }


        [HttpPost, ActionName("EmployeeInfoDataDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string sessionid, string id)
        {
            ViewBag.SessionID = sessionid;
            if(Authenticate("EmployeeInfoDataDelete"))
            {
                EmployeeInfoDataModel employeeInfoDataModel = await employeeInfoDataDbContext.EmployeeInfoDataDbSet.Where(model => model.empid.Equals(id)).FirstOrDefaultAsync();
                employeeInfoDataDbContext.EmployeeInfoDataDbSet.Remove(employeeInfoDataModel);

                EmployeeLoginDataModel employeeLoginDataModel = await employeeLoginDataDbContext.EmployeeLoginDataDbSet.Where(model => model.empid.Equals(id)).FirstOrDefaultAsync();
                employeeLoginDataDbContext.EmployeeLoginDataDbSet.Remove(employeeLoginDataModel);

                await employeeLoginDataDbContext.SaveChangesAsync();
                await employeeInfoDataDbContext.SaveChangesAsync();
                return RedirectToAction("EmployeeInfoDataIndex", new { sessionid = ViewBag.SessionID });
            }
            return Redirect(new Uri(Request.Url, Url.Action("AuthenticationError", "Error", new { sessionid = ViewBag.SessionID })).ToString());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                employeeInfoDataDbContext.Dispose();
                employeeLoginDataDbContext.Dispose();
                departmentDataDbContext.Dispose();
                divisionDataDbContext.Dispose();
                designationDataDbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
