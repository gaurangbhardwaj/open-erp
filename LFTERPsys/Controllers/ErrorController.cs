using System;
using System.Web.Mvc;

namespace LFTERPsys.Controllers
{
    public class ErrorController : Controller
    {
        private void Authenticate()
        {
            if (Session["LoggedEmpID" + ViewBag.SessionID] == null)
            {
                Response.Redirect(new Uri(Request.Url, Url.Action("Signin", "Login")).ToString());
            }
        }
        // GET: Error
        public ActionResult AuthenticationError(string sessionid)
        {
            ViewBag.SessionID = sessionid;
            Authenticate();
            return View();
        }
    }
}