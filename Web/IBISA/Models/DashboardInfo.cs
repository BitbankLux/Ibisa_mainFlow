using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBISA.Models
{
    public class DashboardInfo
    {
        public int totalUsers { get; set; }
        public List<AgreementDetail> AgreementDetails { get; set; }
    }
}