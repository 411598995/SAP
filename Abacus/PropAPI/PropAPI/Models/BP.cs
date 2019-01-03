using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PropAPI.Models
{
    public class BP
    {

        public string BPCode { get; set; }
        public string BPName { get; set; }
        public string BPGroup { get; set; }
        public string BillingAddress { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PostedSAP { get; set; }
        public DateTime PostedDt { get; set; }
        public string SAPCode { get; set; }

    }
}