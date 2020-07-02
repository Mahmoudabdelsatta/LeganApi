using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommitteeApi.Models
{
    public class MeetingPoco
    {
        public int MeetingId { get; set; }

        public long MeetingDate { get; set; }
        public string MeetingAddress { get; set; }
        public string MeetingTitle { get; set; }
        public long CreatedAt { get; set; }
        public bool? IsAttended { get; set; }

        

    }
}