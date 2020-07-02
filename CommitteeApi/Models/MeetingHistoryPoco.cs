using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommitteeApi.Models
{
    public class MeetingHistoryPoco
    {
        public int Id { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public int MeetingId { get; set; }
        public long MeetingDate { get; set; }
        public long CreatedAt { get; set; }
    }
}