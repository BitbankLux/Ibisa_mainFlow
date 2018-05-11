using System;
using System.Web.Mvc;

namespace IBISA.Controllers
{
    // [Authorize]
    public class ErrorsController : Controller
    {

        public ActionResult General(Exception exception)
        {
            GenerateErrorMessage("Something went wrong. Please try again!", exception);
            return View("Index");
        }

        public ActionResult Http404(Exception exception)
        {
            GenerateErrorMessage("404! Page not found!", exception);
            return View("Index");
        }

        public ActionResult Http403()
        {
            GenerateErrorMessage("Access forbidden!", new Exception("You are not authorized to view this page. Please contact system administrator!"));
            return View("Index");
        }
        public ActionResult Http500(Exception exception)
        {
            GenerateErrorMessage("Something went wrong. Please try again!", exception);
            return View("Index");
        }

        private void GenerateErrorMessage(string errorHead , Exception exception)
        {
            ViewBag.ErrorHead = errorHead;

            if (exception != null)
                ViewBag.ErrorMessage = exception;
        }
    }
}
