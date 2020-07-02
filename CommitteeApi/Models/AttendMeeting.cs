using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommitteeApi.Models
{
    public class AttendMeeting
    {
        public string CommitteeId { get; set; }
        
        public string MeetingId { get; set; }
        public bool Attend { get; set; }

        public string ps { get; set; }



    }
}