using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommitteeApi.Models
{
    public class MeetingMemberSearch
    {
        public Nullable<bool> MemberWillAttend { get; set; }

        public Nullable<bool> IsMemberAcceptedMiutesOfCommittee { get; set; }
        public string MemberSignature { get; set; }
        public string RejectionReason { get; set; }

        public virtual User User { get; set; }
    }
}