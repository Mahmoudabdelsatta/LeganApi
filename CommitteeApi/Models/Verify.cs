using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommitteeApi.Models
{
    public class Verify
    {
        public string phone { get; set; }
        public string code { get; set; }
        public string ps { get; set; }
        //public string grant_type { get; set; }

    }
}