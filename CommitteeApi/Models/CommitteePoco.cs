using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CommitteeApi.Models
{
    public class CommitteePoco
    {
        public int CommitteeId { get; set; }
        public string CommitteeName { get; set; }

        public Activity Activity { get; set; }
      
        public Importance Importance { get; set; }
       
        public Type Type { get; set; }
        public int MembersCount { get; set; }
        public long CreatedAt { get; set; }


    }
}