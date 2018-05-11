using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IBISA.Controllers;
using IBISA.Data;

namespace IBISA.Models
{
    public class TransactionInfo
    {
        public int TransactionId { get; set; }
        public int AgreementPrimaryId { get; set; }
        public string AgreementNumber { get; set; }
        public string TransactionHash { get; set; }
        public DateTime DateOfAgreement { get; set; }
        public decimal Amount { get; set; }
        public int TransactionTypeId { get; set; }
    }
}