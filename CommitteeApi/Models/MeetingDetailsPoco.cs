using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommitteeApi.Models
{
    public class MeetingDetailsPoco
    {
        public List<AgendaPoco> Agenda { get; set; }
        public List<MeetingHistoryPoco> MeetingHistory { get; set; }
        public int MeetingId { get; set; }
       public string MeetingTitle { get; set; }
        public int? CommitteeId { get; set; }
        public long MeetingDate { get; set; }
        public double? Latitude { get; set; }
        public double? longitude { get; set; }
        public string MeetingDesc { get; set; }
        public MeetingHistoryPoco Status { get; set; }
        public string MeetingAddress { get; set; }
        public long CreatedAt { get; set; }
        public bool? IsAttended { get; set; }
        public bool? IsMinuteAccepted { get; set; }
        public string MinutePDF { get; set; }








    }
}