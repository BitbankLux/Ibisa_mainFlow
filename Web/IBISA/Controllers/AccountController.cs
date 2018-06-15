using IBISA.Data;
using IBISA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace IBISA.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel loginModel)
        {
            LoginModel logindetails = new LoginModel();
            using (var ibisaRepository = new IBISARepository())
            {
                logindetails = ibisaRepository.CheckLogin(loginModel);
            }
            if (logindetails != null)
            {
                FormsAuthentication.SetAuthCookie(logindetails.userName, false);
                Session["userID"] = logindetails.userId;
                Session["userName"] = logindetails.userName;
                ViewBag.userId = logindetails.userId;
                return RedirectToAction("Workplace", "IBISAWatchers");
            }
            else
            {
                ViewBag.FailedLogin = "Wrong Username or Password";
                return View();
            }
        }
    }
}