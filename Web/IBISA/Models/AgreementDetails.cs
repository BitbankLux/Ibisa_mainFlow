using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBISA.Models
{
    public class AgreementDetail
    {
        public string wallet_address { get; set; }
        public int agreementId { get; set; }
        //Referred as amt : CreateAgreement
        public decimal deposit_wallet { get; set; }
        public decimal withdrawal_wallet { get; set; }

        public int agreementPrimaryId { get; set; }
        public string agreementNumber { get; set; }
        public string zone { get; set; }
        public string crop { get; set; }
        public int maxPayout { get; set; }
        public int targetContrib { get; set; }
        public double date { get; set; }

        public string location { get; set; }
        public string plotId { get; set; }
        public string risk { get; set; }
        public string userDisplayName { get; set; }

        public string transactionHash { get; set; }
    }
}