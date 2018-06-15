using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IBISA.Models
{
    public class WatcherWorkplace
    {
        public List<WatcherWorkplaceDetails> watchdetails { get; set; }
        public List<WatherResponse> watherResponse { get; set; }
    }
    public class WatcherWorkplaceDetails
    {
        public int iddirId { get; set; }
        public string iddirName { get; set; }
        public int quationsId { get; set; }
        public string quations { get; set; }
        public DateTime dueDate { get; set; }
        public bool isAnswerd { get; set; }
        public string remainingTime{ get; set; }
    }
    public class WatherResponse
    {
        public int WatcherResponseId { get; set; }
        public int WatcherId { get; set; }
        public int QuestionId { get; set; }
        public int OptionId { get; set; }
    }
    public class WatcherQA
    {
        public int QuationId { get; set; }
        public string QName { get; set; }
        public List<options> options { get; set; }
        public int selectedOption { get; set; }
        public int watcherId { get; set; }
        public int iddirId { get; set; }
    }
    public class options
    {
        public int optionId { get; set; }
        public string optionDesc { get; set; }
        public int optionValue { get; set; }
        public string quation { get; set; }
        public byte? displayorder { get; set; }

    }

    public class LoginModel
    {
        public int userId { get; set; }
        [Display(Name = "User Name")]
        public string userName { get; set; }
        [Display(Name = "Password")]
        public string password { get; set; }
    }
}