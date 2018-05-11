using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IBISA.Models;
namespace IBISA.Controllers
{
    public class IBISADashboardController : Controller
    {

        public ActionResult Dashboard()
        {
            return View();
        }
    }
}