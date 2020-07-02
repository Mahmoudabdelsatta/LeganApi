using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommitteeApi.Models
{
    public class SuggestCommittee
    {
        public int CommitteId { get; set; }
        public int MemberId { get; set; }
        public int MeetingId { get; set; }
     
        public string suggestCommittee { get; set; }



    }
}