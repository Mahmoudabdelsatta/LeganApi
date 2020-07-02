using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommitteeApi.Models
{
    public class CommitteeMeetingPoco
    {
        public int CommitteeId { get; set; }
       // public UserCommitteePoco CommitteeManager { get; set; }
        public string CommitteeName { get; set; }
        public string CommitteeTopic { get; set; }
        public string CommitteeDependOn { get; set; }
        public string CommitteeInbox { get; set; }
        public string Department { get; set; }

        public string CommitteeEnrollmentNum { get; set; }
        public string CommitteeEnrollmentDate { get; set; }

    //    public UserCommitteePoco CommitteeSecretary { get; set; }

        public Activity  Activity { get; set; }
        public Importance  Importance { get; set; }
        public Type Type { get; set; }

        //  List<Member> Members { get;}
        //  public string MemberName { get; set; }
        // public string MemberTitle { get; set; }
        // List<Meeting> Meetings { get;set }
        public List<MeetingPoco> Meetings { get; set; }
        public List<UserCommitteePoco> Members { get; set; }

      //  public int MeetingId { get; set; }

        //public string MeetingDate { get; set; }
        //public string MeetingAddress { get; set; }

    }
}