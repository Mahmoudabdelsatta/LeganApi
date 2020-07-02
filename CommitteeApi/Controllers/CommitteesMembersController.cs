using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;
using CommitteeApi.Models;
using CommitteeApi.Repository;

namespace CommitteeApi.Controllers
{
    public class CommitteesMembersController : ApiController
    {
        private Committee_DBEntities1 db = new Committee_DBEntities1();

        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.HttpGet]
        [Route("api/CommitteesMembers/GetCommitteesMembers")]

        // GET: api/CommitteesMembers
        public IQueryable<CommitteesMember> GetCommitteesMembers()
        {
            return db.CommitteesMembers;
        }
        [Authorize(Roles = "Secretary, Admin, User")]
 [System.Web.Http.HttpGet]
        [Route("api/CommitteesMembers/GetCommitteesMembersNamesAndTitles")]

        // GET: api/CommitteesMembers
        public IQueryable<User> GetCommitteesMembersNamesAndTitles(int committeeId)
        {
            IQueryable<User> committeesMember = db.CommitteesMembers.Where(x=>x.CommitteeId==committeeId).Select(y=>y.User);
            if (committeesMember == null )
            {
                
            }

            return committeesMember;
        }

        //   [Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.HttpGet]
        [Route("api/CommitteesMembers/GetCommitteesMeetingsDetails")]

        // GET: api/CommitteesMembers
        public IHttpActionResult GetCommitteesMeetingsDetails(int committeeId)
        {
            IQueryable<Meeting> committeesMeetings = db.CommitteesMembers.Where(x => x.CommitteeId == committeeId).Select(y => y.Meeting);
            if (committeesMeetings == null)
            {
            }

            return Json(new { success = true, committeesMeetings = committeesMeetings });
           // return committeesMeetings;
        }
        [Route("api/CommitteesMembers/GetAlerts")]

        // GET: api/CommitteesMembers
        //public IQueryable<Alert> GetAlerts(int committeeId)
        //{
        //    IQueryable<Alert> committeesAlerts = db.CommitteesMembers.Where(x => x.CommitteeId == committeeId).Select(y => y.Alert);
        //    if (committeesAlerts == null)
        //    {
        //    }

        //    return committeesAlerts;
        //}
       // [Authorize(Roles = "SuperAdmin, Admin, User")]

        //[Route("api/CommitteesMembers/GetCommitteesMember")]
        //[HttpGet]
        //// GET: api/CommitteesMembers/5
        //[ResponseType(typeof(CommitteesMember))]
        //public IHttpActionResult GetCommitteesMember(int id)
        //{
        //    List<CommitteesMember> committeesMembers = db.CommitteesMembers.Where(x=>x.CommitteeId==id).ToList();
        //    if (committeesMembers == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(committeesMembers.Select(x=>x.Member));
        //}
        [Route("api/CommitteesMembers/GetCommitteesMember")]
        [HttpGet]
        // GET: api/CommitteesMembers/5
        [ResponseType(typeof(CommitteesMember))]
        public List<User> GetCommitteesMember(int id)
        {
            List<CommitteesMember> committeesMembers = db.CommitteesMembers.Where(x => x.CommitteeId == id).ToList();
            if (committeesMembers == null)
            {
               
            }

            return committeesMembers.Select(x => x.User).ToList();
        }
        [Route("api/CommitteesMembers/GetMemberDataFromCommitteeMember")]
        [HttpGet]
        // GET: api/CommitteesMembers/5
        [ResponseType(typeof(CommitteesMember))]
        public CommitteesMember GetMemberDataFromCommitteeMember(int committeeId,int userId)
        {
           CommitteesMember committeesMember = db.CommitteesMembers.FirstOrDefault(x => x.CommitteeId == committeeId &x.MemberId==userId);
           

            return committeesMember;
        }

        [Route("api/CommitteesMembers/GetMemberForGrid")]
        [HttpGet]
        // GET: api/CommitteesMembers/5
        [ResponseType(typeof(CommitteesMember))]
        public User GetMemberForGrid(int userId)
        {
            User Member = db.Users.FirstOrDefault(x => x.ID == userId);
           

            return Member;
        }
        [Authorize(Roles = "Secretary, Admin, User")]
        [AllowAnonymous]

        [Route("api/CommitteesMembers/AcceptMeetingMinute")]
        [HttpPut]
        // PUT: api/CommitteesMembers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult AcceptMeetingMinute(AcceptMeeting acceptMeeting)
        {
            object obj = null;
            int userId = 0;
            try
            {
                //  string _userName = System.Web.HttpContext.Current.User.Identity.Name;
                // int memberId = db.Users.FirstOrDefault(x => x.UserName == _userName).ID;
                try
                {
                    userId = Convert.ToInt32(((ClaimsIdentity)User?.Identity).Claims?.FirstOrDefault(x => x.Type == "ID").Value);
                }
                // string _userName = System.Web.HttpContext.Current.User.Identity.Name;

                catch (Exception ex)
                {


                    return Json(new { success = 0, error = "Invalid Token" });

                }
                string decryptedCommitteeId = HypridEncryption.decrypt(acceptMeeting.CommitteId, acceptMeeting.ps);
                string decryptedMeetingId = HypridEncryption.decrypt(acceptMeeting.MeetingId, acceptMeeting.ps);
                string decryptedSignature = HypridEncryption.decrypt(acceptMeeting.Signature, acceptMeeting.ps);

                int committeeId = Convert.ToInt32(decryptedCommitteeId);
                int meetingId = Convert.ToInt32(decryptedMeetingId);

                CommitteesMember committeeMember = db.CommitteesMembers.FirstOrDefault(x => x.CommitteeId == committeeId && x.MemberId == userId && x.MeetingId == meetingId);
                if (committeeMember != null)
                { 
                    Random generator = new Random();
                    int x = generator.Next(1, 10000000);
                    committeeMember.IsMemberAcceptedMiutesOfCommittee = true;
                    committeeMember.IsMinuteAccepted = true;
                    //   committeeMember.MemberSignature = "/"+x+".png";
                    committeeMember.MemberSignature = decryptedSignature;
                    db.Entry(committeeMember).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = 1, error=obj, data = obj });
                }
                else
                {
                    return Json(new { success = 0, error = "meeting not found", data = obj });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = 0, error = ex.Message });

            }
            
        }
        [Authorize(Roles = "Secretary, Admin, User")]
        [AllowAnonymous]

        [Route("api/CommitteesMembers/MeetingAttendence")]
        [HttpPut]
        // PUT: api/CommitteesMembers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult MeetingAttendence(AttendMeeting attendMeeting)
        {
            
            object obj = null;
            int userId = 0;
            try
            {
                //  string _userName = System.Web.HttpContext.Current.User.Identity.Name;
                try
                {
                    userId = Convert.ToInt32(((ClaimsIdentity)User?.Identity).Claims?.FirstOrDefault(x => x.Type == "ID").Value);
                }
                // string _userName = System.Web.HttpContext.Current.User.Identity.Name;

                catch (Exception ex)
                {


                    return Json(new { success = 0, error = "Invalid Token" });

                }
                string decryptedCommitteeId = HypridEncryption.decrypt(attendMeeting.CommitteeId, attendMeeting.ps);
                string decryptedMeetingId = HypridEncryption.decrypt(attendMeeting.MeetingId, attendMeeting.ps);

                int committeeId = Convert.ToInt32(decryptedCommitteeId);
                int meetingId = Convert.ToInt32(decryptedMeetingId);

                CommitteesMember committeeMember = db.CommitteesMembers.FirstOrDefault(x => x.CommitteeId == committeeId && x.MemberId == userId && x.MeetingId == meetingId);
                if (committeeMember != null)
                {


                    committeeMember.MemberWillAttend = attendMeeting.Attend;

                    db.Entry(committeeMember).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = 1, error = obj, data = obj });
                }
                else
                {
                    return Json(new { success = 0, error = "no data found for this request", data=obj });

                }

            }
            catch (Exception ex)
            {

                return Json(new { success = 0, error = ex.Message });

            }

        }
        [Authorize(Roles = "Secretary, Admin, User")]
        [Route("api/CommitteesMembers/ChangeCommitteeStatusToClosed")]
        [HttpPut]
        public IHttpActionResult ChangeCommitteeStatusToClosed(int committeId)
        {

            try
            {
                // CommitteesMember committeeMember = db.CommitteesMembers.FirstOrDefault(x => x.CommitteeId == committeId);
                var CheckAll = db.CommitteesMembers.Where(x => x.CommitteeId == committeId);
                if (CheckAll.All(x => x.IsMemberAcceptedMiutesOfCommittee == true))
                {
                    foreach (var item in CheckAll.ToList())
                    {


                        item.MeetingStatus = "Closed";
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true, committeeMember = CheckAll });

            }
            catch (Exception ex)
            {

                return Json(new { success = false, error = "error message." + ex.Message });

            }
        }
       [Authorize(Roles = "Secretary, Admin, User")]
        [AllowAnonymous]

        [Route("api/CommitteesMembers/RejectMeetingMinute")]
        [HttpPut]
        // PUT: api/CommitteesMembers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult RejectMeetingMinute(RejectMeeting rejectMeeting)
        {
            object obj = null;
            int userId = 0;
            try
            {
                // string _userName = System.Web.HttpContext.Current.User.Identity.Name;
                //  int memberId = db.Users.FirstOrDefault(x => x.UserName == _userName).ID;
                try
                {
                    userId = Convert.ToInt32(((ClaimsIdentity)User?.Identity).Claims?.FirstOrDefault(x => x.Type == "ID").Value);
                }
                // string _userName = System.Web.HttpContext.Current.User.Identity.Name;

                catch (Exception ex)
                {


                    return Json(new { success = 0, error = "Invalid Token" });

                }
                string decryptedCommitteeId = HypridEncryption.decrypt(rejectMeeting.CommitteId, rejectMeeting.ps);
                string decryptedMeetingId = HypridEncryption.decrypt(rejectMeeting.MeetingId, rejectMeeting.ps);
                string decryptedReasonOfRejection = HypridEncryption.decrypt(rejectMeeting.ReasonOfRejection, rejectMeeting.ps);

                int committeeId = Convert.ToInt32(decryptedCommitteeId);
                int meetingId = Convert.ToInt32(decryptedMeetingId);


                CommitteesMember committeeMember = db.CommitteesMembers.FirstOrDefault(x => x.CommitteeId == committeeId && x.MemberId == userId && x.MeetingId == meetingId);
               
                if (committeeMember!=null)
                {

                    if (committeeMember.IsMemberAcceptedMiutesOfCommittee == true)
                    {
                        return Json(new { success = 0, error = "you cant reject the meeting minute as you have accepted before.",data=obj });
                    }
                    committeeMember.IsMemberAcceptedMiutesOfCommittee = false;
                    committeeMember.IsMinuteAccepted = false;

                    committeeMember.ReasonOfRejection = decryptedReasonOfRejection;
                    db.Entry(committeeMember).State = EntityState.Modified;

                    db.SaveChanges();
                    return Json(new { success = 1,error= obj, data = obj });
                }
                else
                {
                    return Json(new { success = 0, error = "meeting not found", data = obj });
                }
            }
                catch (Exception ex)
                {

                return Json(new { success = 0, error =ex.Message });
                

            }
            
            
            

          
        }
        [Authorize(Roles = "Secretary, Admin, User")]
        [Route("api/CommitteesMembers/SuggestCommittee")]
        [HttpPut]
        // PUT: api/CommitteesMembers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult SuggestCommittee(SuggestCommittee suggestCommittee)
        {
            try
            {
                CommitteesMember committeeMember = db.CommitteesMembers.FirstOrDefault(x => x.CommitteeId == suggestCommittee.CommitteId && x.MemberId == suggestCommittee.MemberId && x.MeetingId == suggestCommittee.MeetingId);
                if (committeeMember != null)
                {


                    committeeMember.AgendamodifingSuggession = suggestCommittee.suggestCommittee;
                    db.Entry(committeeMember).State = EntityState.Modified;

                    db.SaveChanges();
                    return Json(new { success = true, error = "", committeeMember = committeeMember });
                }
                else
                {
                    return Json(new { success = true, error = "meeting not found", data = new { } });

                }

            }
            catch (Exception ex)
            {

                return Json(new { success = false, error = "error message." + ex.Message });


            }

        }

        [Route("api/CommitteesMembers/PostCommitteesMember")]
        [HttpPost]
        // POST: api/CommitteesMembers
        [ResponseType(typeof(CommitteesMember))]
        public void PostCommitteesMember(CommitteesMember committeesMember)
        {
           
                
           
            db.CommitteesMembers.Add(committeesMember);
            db.SaveChanges();

            
        }
        [Route("api/CommitteesMembers/PostListOfCommitteesMember")]

        // POST: api/CommitteesMembers
        [ResponseType(typeof(CommitteesMember))]
        public void PostListOfCommitteesMember(List<CommitteesMember> committeesMembers)
        {
            if (!ModelState.IsValid)
            {
               
            }
            for (int i = 0; i < committeesMembers.Count; i++)
            {
                //committeesMembers[0].CommitteeMemberId = db.CommitteesMembers.Max(x => x.CommitteeMemberId==null?0: x.CommitteeMemberId) + 1;

                db.CommitteesMembers.Add(committeesMembers[i]);
                db.SaveChanges();
            }
         
        }
        [Route("api/CommitteesMembers/AddAlert")]

        public void AddAlert(List <Member> committeesMember)
        {
            //send alert
        }
        [HttpPost]
        [Route("api/CommitteeMembers/DeleteCommitteeMembers")]

        // DELETE: api/CommitteesMembers/5
        [ResponseType(typeof(CommitteesMember))]
        public IHttpActionResult DeleteCommitteeMembers(int memberId,int committeeId)
        {
           List<CommitteesMember> committeeMembers = db.CommitteesMembers.Where(x=>x.MemberId==memberId&&x.CommitteeId==committeeId).ToList();
            if (committeeMembers.Count == 0)
            {
                return NotFound();
            }

            db.CommitteesMembers.RemoveRange(committeeMembers);
            db.SaveChanges();

            return Ok(committeeMembers);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CommitteesMemberExists(int id)
        {
            return db.CommitteesMembers.Count(e => e.CommitteeId == id) > 0;
        }
        [HttpGet]
        [Route("api/CommitteesMembers/GetMeetingMembers")]
        [ResponseType(typeof(CommitteesMember))]
        public List<CommitteesMember> GetMeetingMembers(int MeetingId)
        {
            List<CommitteesMember> committeesMembers = db.CommitteesMembers.Where(x => x.MeetingId == MeetingId).ToList();
            if (committeesMembers == null)
            {
               
            }

            return committeesMembers;
        }
        [HttpGet]
        [Route("api/CommitteesMembers/GetRoleName")]
        [ResponseType(typeof(CommitteesMember))]
        public string GetRoleName(int committeeRole)
        {
            string roleName  = db.SystemRoles.FirstOrDefault(x => x.Id == committeeRole).titleAr;
         
            return roleName;
        }
        [HttpGet]
        [Route("api/CommitteesMembers/GetCommitteesMembersMeeting")]
        [ResponseType(typeof(CommitteesMember))]
        public List<MeetingMemberSearch> GetCommitteesMembersMeeting(int MeetingId)
        {
            List<MeetingMemberSearch> meetingMemberSearches = new List<MeetingMemberSearch>();
            List<CommitteesMember> committeesMembers = db.CommitteesMembers.Where(x => x.MeetingId == MeetingId).ToList();
            foreach (var committeeMember in committeesMembers)
            {
                meetingMemberSearches.Add(new MeetingMemberSearch() { IsMemberAcceptedMiutesOfCommittee = committeeMember.IsMemberAcceptedMiutesOfCommittee,
                    MemberSignature = committeeMember.MemberSignature,
                    RejectionReason = committeeMember.ReasonOfRejection,
                MemberWillAttend=committeeMember.MemberWillAttend,User=committeeMember.User});
            }
            if (committeesMembers == null)
            {

            }

            return meetingMemberSearches;
        }
    }
}