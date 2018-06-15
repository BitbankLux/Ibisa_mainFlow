using IBISA.Data;
using System.Configuration;
using IBISA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace IBISA.Controllers
{
    public class IBISAWatchersController : Controller
    {
        // GET: IBISAWatchers
        [Authorize]
        public ActionResult WatcherDashboard()
        {
            int userId = 0;
            string username = "";
            if (Session["UserID"] != null)
            {
                userId = Convert.ToInt32(Session["UserID"]);
                username = Session["userName"].ToString();
            }
            List<WatcherWorkplaceDetails> details = new List<WatcherWorkplaceDetails>();
            List<WatherResponse> ss = new List<WatherResponse>();
            using (var ibisaRepository = new IBISARepository())
            {
                details = ibisaRepository.GetWorkplaceDetail();
                ss = ibisaRepository.getTaskCompletDetail(userId);
            }
            ViewBag.taskNumber = details.Count();
            ViewBag.countpercent = (ss.Count()) * 100 / details.Count();
            return View();
        }
        [Authorize]
        public ActionResult WatcherAssessmentDashboard()
        {
            if (true)
            {

            }
            else
            {

            }
            List<BubbleChartData> listbubble = new List<BubbleChartData>();
            using (var ibisaRepository = new IBISARepository())
            {
                listbubble = ibisaRepository.GetBubblechartData(int.Parse(ConfigurationManager.AppSettings["BubbleChartRadiusMultiplicationFactor"]));
            }
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(listbubble);
            ViewBag.datajson = json;
            return View();
        }
        public JsonResult chart()
        {
            List<BubbleChartData> listbubble = new List<BubbleChartData>();
            using (var ibisaRepository = new IBISARepository())
            {
                listbubble = ibisaRepository.GetBubblechartData(int.Parse(ConfigurationManager.AppSettings["BubbleChartRadiusMultiplicationFactor"]));
            }
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(listbubble);
            
            return Json(listbubble,JsonRequestBehavior.AllowGet);
        }
        public ActionResult MachineassessmentChart()
        {

            return View();
        }
        [Authorize]
        public ActionResult Workplace()
        {
            int userId = 0;
            string username = "";
            if (Session["UserID"] != null)
            {
                userId = Convert.ToInt32(Session["UserID"]);
                username = Session["userName"].ToString();
            }
            List<WatcherWorkplaceDetails> details = new List<WatcherWorkplaceDetails>();
            List<WatherResponse> ss = new List<WatherResponse>();

            using (var ibisaRepository = new IBISARepository())
            {
                details = ibisaRepository.GetWorkplaceDetail();
                ss = ibisaRepository.getTaskCompletDetail(userId);
            }
           
            foreach (var item in details)
            {
                foreach (var item1 in ss)
                {
                    //var isans = ss.FirstOrDefault(i => i.QuestionId == item.quationsId);
                    if (item1.QuestionId != 0 && item1.QuestionId == item.quationsId)
                    {
                        item.isAnswerd = true;
                        break;
                    }

                }
            }
            ViewBag.userName = username;
            return View(details);
        }
        [Authorize]
        public ActionResult WorkplaceDetails(int qid, int quationid)
        {

            List<options> Options = new List<options>();
            int selectedOpt = 0;
            int wid = 0;
            string username = "";
            string quation = "";
            if (Session["UserID"] != null)
            {
                wid = Convert.ToInt32(Session["UserID"]);
                username = Session["userName"].ToString();
            }
            using (var ibisaRepository = new IBISARepository())
            {
                Options = ibisaRepository.getOptions(qid);
                selectedOpt = ibisaRepository.GetSelectedOption(wid, quationid);
                quation = ibisaRepository.getquation(qid);
            }

            WatcherQA wQa = new WatcherQA();
            wQa.QuationId = quationid;
            wQa.options = Options;
            wQa.watcherId = wid;
            wQa.selectedOption = selectedOpt;
            wQa.QName = quation;
            wQa.iddirId = qid;
            ViewBag.userName = username;
            return View(wQa);
        }

        public ActionResult SubmitAns(WatcherQA wqa)
        {
            var watcherResponceDetail = new WatcherRespons
            {
                WatcherId = wqa.watcherId,
                QuestionId = wqa.QuationId,
                OptionId = wqa.selectedOption,
                CreatedDate = DateTime.Now
            };
            using (var ibisaRepository = new IBISARepository())
            {
                ibisaRepository.saveWatcherresponse(watcherResponceDetail);
            }
            return RedirectToAction("Workplace");
        }
     
    }
}