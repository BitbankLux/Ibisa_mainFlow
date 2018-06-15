using IBISA.Data;
using IBISA.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;

namespace IBISA.Controllers
{
    public class ChartApiController : ApiController
    {
        [HttpGet]
        [Route("chartapi/watcher")]
        public List<BubbleChartData> WatcherAssessmentDashboard()
        {
            List<BubbleChartData> listbubble = new List<BubbleChartData>();
            using (var ibisaRepository = new IBISARepository())
            {
                listbubble = ibisaRepository.GetBubblechartData(int.Parse(ConfigurationManager.AppSettings["BubbleChartRadiusMultiplicationFactor"]));
            }
            return listbubble;
        }
    }
}
