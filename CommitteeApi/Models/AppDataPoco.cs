using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommitteeApi.Models
{
    public class AppDataPoco
    {
        public List<Activity> Activity { get; set; }
        public List<Importance> Importance { get; set; }
        public List<Type> Type { get; set; }



    }
}