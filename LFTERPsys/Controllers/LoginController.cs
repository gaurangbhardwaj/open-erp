using System.Web.Mvc;
using LFTERPsys.Models;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Data.Entity;
using System.Net;
using System.Net.Mail;

namespace LFTERPsys.Controllers
{
    public class LoginController : Controller
    {

        //-----------------------------------------------Encryption&Decryption------------------------------------------------

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

        private string Decrypt(string str)
        {
            str = str.Replace(" ", "+");
            string DecryptKey = "0806;[pnuLIT)lOFRUtEChnologieS";
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byte[] inputByteArray = new byte[str.Length];

            byKey = System.Text.Encoding.UTF8.GetBytes(DecryptKey.Substring(0, 8));
            System.Security.Cryptography.DESCryptoServiceProvider des = new System.Security.Cryptography.DESCryptoServiceProvider();
            inputByteArray = System.Convert.FromBase64String(str.Replace(" ", "+"));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(ms, des.CreateDecryptor(byKey, IV), System.Security.Cryptography.CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }
        //-------------------------------------------------------------------------------------------------------------------


        private void Authenticate()
        {
            if (Session["LoggedEmpID" + ViewBag.SessionID] == null)
            {
                Response.Redirect(new Uri(Request.Url, Url.Action("Signin", "Login")).ToString());
            }
        }


        // GET: EmployeeLoginData
        public ActionResult Signin()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Signin(EmployeeLoginDataModel employeeLoginDataModel)
        {
            try
            {
                if (ModelState.IsValid) //validating the user inputs
                {
                    using (EmployeeLoginDataDbContext  employeeLoginDataDbContext = new EmployeeLoginDataDbContext())
                    {
                        EmployeeLoginDataModel verification = employeeLoginDataDbContext.EmployeeLoginDataDbSet.Where(loginvalues => loginvalues.empid.Equals(employeeLoginDataModel.empid)).FirstOrDefault();
                        if (verification != null)
                        {
                            if(Decrypt(verification.password) == employeeLoginDataModel.password)
                            {
                                Session["LoggedEmpID" + verification.empid.ToString()] = verification.empid.ToString().Trim();
                                Session["LoggedEmpRole" + verification.empid.ToString()] = verification.role.ToString().Trim();
                                return Redirect("/Home/Home/" + verification.empid.ToString());
                            }
                            else
                                ViewBag.invalidError = "Invalid password for "+employeeLoginDataModel.empid+" Employee ID!";
                        }
                        else
                        {
                            ViewBag.invalidError = "Can't find "+employeeLoginDataModel.empid+" Employee ID!";
                            return View();
                        }
                    }
                }
            }
            catch
            {
                Response.Write("<script>alert('Server error please try again after sometime');</script>");
                ViewBag.invalidError = "**Error: Server maintenance timeout!!";
            }
            return View(employeeLoginDataModel);
        }

        public ActionResult ChangePassword(string sessionid)
        {
            ViewBag.SessionID = sessionid;
            Authenticate();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(string sessionid, string newpassword)
        {
            ViewBag.SessionID = sessionid;
            Authenticate();
            try
                {
                if (newpassword != null)
                {
                    using (EmployeeLoginDataDbContext employeeLoginDataDbContext = new EmployeeLoginDataDbContext())
                    {
                        var oldvalues = await employeeLoginDataDbContext.EmployeeLoginDataDbSet.Where(model => model.empid.Equals(sessionid)).FirstOrDefaultAsync();
                        oldvalues.password = Encrypt(newpassword);
                        await employeeLoginDataDbContext.SaveChangesAsync();
                        MailMessage mailMessage = new MailMessage();
                        mailMessage.To.Add("noreply@logic-fruit.com");
                        mailMessage.From = new MailAddress("noreply@logic-fruit.com");
                        mailMessage.Subject = "Password Change/Modified";
                        mailMessage.Body = "Password of Employee ID : " + sessionid + " has been changed/modified recently <br> at <strong>"+DateTime.Now+ "<strong>.";
                        mailMessage.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("noreply@logic-fruit.com", "{LFTnoreply@ERPsys};"); //username password
                        smtp.EnableSsl = true;
                        smtp.Send(mailMessage);
                        return Redirect("/Home/Home/" + sessionid);
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            catch
            {
                Response.Write("<script>alert('Server error please try again after sometime');</script>");
            }
            return View();
        }

        [HttpPost]
        public JsonResult ForgotPassword(string empid)
        {
            var status = false;
            if (empid == "09001")
                status = true;

            return new JsonResult { Data = new { status = status } };
        }

    }
}
