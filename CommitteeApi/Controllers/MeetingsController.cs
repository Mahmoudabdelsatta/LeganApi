using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
    public class MeetingsController : ApiController
    {
        private Committee_DBEntities1 db = new Committee_DBEntities1();
        //[Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.HttpGet]
        [Route("api/Meetings/GetMeetings")]
        // GET: api/Meetings
        public IHttpActionResult GetMeetings()
        {
            return Ok(db.Meetings);
        }
        [Route("api/Meetings/GetMinutesOfMeeting")]

        // GET: api/Committees/5
        [ResponseType(typeof(Committee))]
        public IHttpActionResult GetMinutesOfMeeting(int meetingId)
        {
            string minutesOfMeeting = db.Meetings.FirstOrDefault(x => x.MeetingId == meetingId).MinutesOfMeeting;
            if (minutesOfMeeting == null )
            {
                return NotFound();
            }

            return Ok(minutesOfMeeting);
        }
        [Authorize(Roles = "Secretary, Admin, User")]
        [System.Web.Http.HttpGet]
        [Route("api/Meeting/GetCommitteeMinutesOfMeeting")]

        // GET: api/CommitteesMembers
        public IHttpActionResult GetCommitteeMinutesOfMeeting(int meetingId)
        {
            try
            {
                string minutesOfMeeting = db.Meetings.FirstOrDefault(x => x.MeetingId == meetingId).MinutesOfMeeting.ToString();

                return Json(new { success = true, minutesOfMeeting = minutesOfMeeting });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, error = "error message." + ex.InnerException });
            }

        }
        //   [Authorize(Roles = "SuperAdmin, Admin, User")]

        // GET: api/Meetings/5
       [Authorize(Roles = "Secretary, Admin, User")]
        [AllowAnonymous]

        [Route("api/Meetings/GetMeetingById")]
        [ResponseType(typeof(Meeting))]
        public IHttpActionResult GetMeetingById(string meetingid, string committeeid, string ps)
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
                string decryptedCommitteeId = HypridEncryption.decrypt(committeeid, ps);
                string decryptedMeetingId = HypridEncryption.decrypt(meetingid,ps);
                int decCommitteeId = Convert.ToInt32(decryptedCommitteeId);
                int decMeetingId = Convert.ToInt32(decryptedMeetingId);

                Meeting meeting = db.Meetings.Find(decMeetingId);

                CommitteesMember committeesMembers = db.CommitteesMembers.FirstOrDefault(x => x.MeetingId == decMeetingId && x.MemberId== userId && x.CommitteeId== decCommitteeId);
                if (meeting!=null || committeesMembers!=null)
                {
                    DateTime createdAt = DateTime.Parse(meeting.CreatedAt);
                    DateTime meetingDate = DateTime.Parse(meeting.MeetingDate);
                    List<AgendaPoco> Agendas = new List<AgendaPoco>();
                    foreach (var agenda in meeting.Agenda)
                    {
                      //  DateTime AgendaTime = DateTime.Parse(agenda.AgendaTime);
                        DateTime CreatedAt = DateTime.Parse(agenda.CreatedAt);
                        Agendas.Add(new AgendaPoco() { id = agenda.id, MeetingId = agenda.MeetingId, AgendaDesc = agenda.AgendaDesc, CreatedAt = UnixStamp.DateTimeToUnixTimestamp(CreatedAt) });

                    }
                    List<MeetingDetailsPoco> MeetingDetailsPoco = new List<MeetingDetailsPoco>();
                  
                    List<MeetingHistoryPoco> MeetingHistories = new List<MeetingHistoryPoco>();
                    foreach (var MeetingHistory in meeting.MeetingHistories)
                    {
                        DateTime meetingHistoryDate = DateTime.Parse(MeetingHistory.MeetingDate);
                        DateTime meetingHistoryCreatedAt = DateTime.Parse(MeetingHistory.CreatedAt);
                        MeetingHistories.Add(new MeetingHistoryPoco() { Id = MeetingHistory.Id, MeetingId = MeetingHistory.MeetingId, MeetingDate = UnixStamp.DateTimeToUnixTimestamp(meetingHistoryDate),TitleAr = MeetingHistory.TitleAr,TitleEn=MeetingHistory.TitleEn, CreatedAt = UnixStamp.DateTimeToUnixTimestamp(meetingHistoryCreatedAt) });

                    }
                    MeetingDetailsPoco.Add(new Models.MeetingDetailsPoco()
                    {
                       Agenda= Agendas.Count==0?null:Agendas,
                        MeetingHistory= MeetingHistories.Count==0?new List<MeetingHistoryPoco>():MeetingHistories,
                        MeetingId=meeting.MeetingId,
                        MeetingTitle=meeting.MeetingTitle,
                       CommitteeId=meeting.CommitteeId,
                       MeetingDate=UnixStamp.DateTimeToUnixTimestamp(meetingDate),
                        Latitude=meeting.Latitude,
                        longitude=meeting.longitude,
                        MeetingDesc=meeting.MeetingDesc,
                        Status=MeetingHistories.LastOrDefault(x=>x.MeetingId==meeting.MeetingId)==null?null: MeetingHistories.Last(x => x.MeetingId == meeting.MeetingId),
                        MeetingAddress=meeting.MeetingAddress,
                        CreatedAt=UnixStamp.DateTimeToUnixTimestamp(createdAt),
                        IsAttended= committeesMembers?.MemberWillAttend,
                        IsMinuteAccepted = committeesMembers?.IsMinuteAccepted,
                        MinutePDF=meeting?.MinutesOfMeeting

                  });

                
                return Json(new { success = 1,error= obj, data = MeetingDetailsPoco[0] });
                }
                else
                {
                    return Json(new { success = 0, error = "Meeting not found", data = obj });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = 0, error =ex.InnerException });


            }
        }
        
        [ResponseType(typeof(Meeting))]
        [Route("api/Meetings/GetMeeting")]
        [HttpGet]
        public List<Meeting> GetMeeting(string meetingName,int deptId)
        {
            List<Committee> committeesOfDepts = db.Committees.Where(x => x.DeptId == deptId).ToList();
            List<Meeting> meetings = new List<Meeting>();
            List<Meeting> meetingsOfCommittees = new List<Meeting>();
            List<Meeting> meetingsFinal = new List<Meeting>();
          //  List<Meeting> meetings2 = db.Meetings.Where(x => x.MeetingTitle.Trim().ToLower().Contains(meetingName.Trim().ToLower())).ToList();
            foreach (var committeeOfDept in committeesOfDepts)
            {
                meetingsOfCommittees = db.Meetings.Where(x => x.CommitteeId == committeeOfDept.CommitteeId).ToList();
                meetings.AddRange(meetingsOfCommittees);
            }
           
            if (meetings.Count == 0)
            {
                return db.Meetings.ToList();
            }
            if (meetingName==null)
            {
                meetingsFinal = meetings;
            }
            else
            {
                meetingsFinal = meetings.Where(x => x.MeetingTitle.Trim().ToLower().Contains(meetingName.Trim().ToLower())).ToList();
            }
          
            return meetingsFinal;
        }
        [Route("api/Meetings/GetMeetingForWeb")]
        [HttpGet]
        public Meeting GetMeetingForWeb(int meetingId)
        {
          Meeting meeting = db.Meetings.Find(meetingId);
            
            return meeting;
        }

        [Route("api/Meetings/GetMeetingUsers")]
        [HttpGet]
        public IHttpActionResult GetMeetingUsers(int meetingId)
        {
           List <User> Users = db.CommitteesMembers.Where(x=>x.MeetingId==meetingId && x.IsMemberAcceptedMiutesOfCommittee==null && x.Meeting.MinutesOfMeeting!=null).Select(x=>x.User).ToList();

            return Ok(Users);
        }

        [Route("api/Meetings/GetMeetingsUsersForAttendenceMeeting")]
        [HttpGet]
        public IHttpActionResult GetMeetingsUsersForAttendenceMeeting(int meetingId)
        {
            List<User> Users = db.CommitteesMembers.Where(x => x.MeetingId==meetingId).Select(x => x.User).ToList();

            return Ok(Users);
        }

        // PUT: api/Meetings/5
        [HttpPost]
      
        [Route("api/Meetings/putMeeting")]
        [ResponseType(typeof(Meeting))]
        public void PutMeeting(int id, Meeting meeting)
        {
           

            db.Entry(meeting).State = EntityState.Modified;

            
                db.SaveChanges();
            
        }
        [HttpPost]

        [Route("api/Meetings/putMeetingAgenda")]
        [ResponseType(typeof(Agendum))]
        public void putMeetingAgenda(int id, Agendum agenda)
        {


            db.Entry(agenda).State = EntityState.Modified;


            db.SaveChanges();

        }
        [HttpPost]

        [Route("api/Meetings/putMeetingHistory")]
        [ResponseType(typeof(Agendum))]
        public void putMeetingHistory(int id, MeetingHistory status)
        {


            db.Entry(status).State = EntityState.Modified;


            db.SaveChanges();

        }

        // POST: api/Meetings
        [Route("api/Meetings/PostMeeting")]
        [ResponseType(typeof(Meeting))]
        public IHttpActionResult PostMeeting(Meeting meeting)
      {
           

            db.Meetings.Add(meeting);
            db.SaveChanges();
          
             int mId=meeting.MeetingId;
            List<CommitteesMember> comms = db.CommitteesMembers.Where(x => x.CommitteeId == meeting.CommitteeId).ToList();
            if (comms.Count != 0)
            {
                foreach (var com in comms)
                {
                    if (com.CommitteeId == meeting.CommitteeId && com.MeetingId == null)
                    {
                        com.MeetingId = meeting.MeetingId;
                        db.Entry(com).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else if (com.MeetingId != meeting.MeetingId)
                    {
                        int? cmRole = com.CommitteeRole;
                        if (!db.CommitteesMembers.Any(x => x.CommitteeId == com.CommitteeId && x.MeetingId == meeting.MeetingId && x.MemberId == com.MemberId))
                        {


                            db.CommitteesMembers.Add(new CommitteesMember()
                            {
                                CommitteeId = meeting.CommitteeId,
                                MemberId = com.MemberId,
                                MeetingId = meeting.MeetingId,
                                CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                CommitteeRole = cmRole



                            });
                            db.SaveChanges();
                        }



                    }







                }
            }
           
            return Ok(meeting.MeetingId);
           




        }
        [Route("api/Meetings/PostAgendaOfMeeting")]
        [ResponseType(typeof(Agendum))]
        [HttpPost]
        public void PostAgendaOfMeeting(Agendum agenda)
        {


            db.Agenda.Add(agenda);
            db.SaveChanges();
           


        }
        [Route("api/Meetings/PostStatusOfMeeting")]
        [ResponseType(typeof(MeetingHistory))]
        [HttpPost]
        public void PostStatusOfMeeting(MeetingHistory status)
        {


            db.MeetingHistories.Add(status);
            db.SaveChanges();



        }
        // DELETE: api/Meetings/5
       // [ResponseType(typeof(Meeting))]
        [Route("api/Meetings/DeleteMeeting")]
        [HttpGet]
        public string DeleteMeeting(int id)
        {
            Meeting meeting = db.Meetings.Find(id);
            List<Agendum> ids = db.Agenda.Where(x => x.MeetingId == id).ToList();
   db.Agenda.RemoveRange(ids);
            db.Meetings.Remove(meeting);
         
            db.SaveChanges();

            return id.ToString() ;
        }
        [Route("api/Meetings/DeleteAgenda")]
        [HttpGet]
        public string DeleteAgenda(int id)
        {
            Agendum agenda = db.Agenda.Find(id);
           
            db.Agenda.Remove(agenda);

            db.SaveChanges();

            return id.ToString();
        }
        [Route("api/Meetings/DeleteStatus")]
        [HttpGet]
        public string DeleteStatus(int id)
        {
            MeetingHistory status = db.MeetingHistories.Find(id);

            db.MeetingHistories.Remove(status);

            db.SaveChanges();

            return id.ToString();
        }
        [System.Web.Http.HttpGet]
        [Route("api/Meetings/GetMeetingAgendas")]

        // GET: api/CommitteesMembers
        public IHttpActionResult GetMeetingAgendas(int meetingId)
        {
            return Json(db.Agenda.Where(x=>x.MeetingId== meetingId).ToList());
        }

        [System.Web.Http.HttpGet]
        [Route("api/Meetings/GetMeetingHistory")]

        // GET: api/CommitteesMembers
        public IHttpActionResult GetMeetingHistory(int meetingId)
        {
            return Json(db.MeetingHistories.Where(x => x.MeetingId == meetingId).ToList());
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MeetingExists(int id)
        {
            return db.Meetings.Count(e => e.MeetingId == id) > 0;
        }


        [Authorize(Roles = "Secretary, Admin, User")]
        [AllowAnonymous]

        [Route("api/Meetings/UploadMeetingImage")]
        [HttpPut]
        // PUT: api/CommitteesMembers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult UploadMeetingImage(UploadMeetingImage meetingUploadImage)
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

                Meeting meeting = db.Meetings.FirstOrDefault(x => x.MeetingId == meetingUploadImage.MeetingId);
                if (meeting != null)
                {
                    MeetingImage meetingImage = new MeetingImage();
                    List<string> Images = new List<string>();
                    Random generator = new Random();
                    int x = generator.Next(1, 10000000);
                    if (meetingUploadImage.Images.Count > 0)
                    {
                        for (int i = 0; i < meetingUploadImage.Images.Count; i++)
                        {
                            // Images.Add(meetingUploadImage.Images[i]);
                            db.MeetingImages.Add(new MeetingImage() { MeetingId = meetingUploadImage.MeetingId, MeetingImage1 = meetingUploadImage.Images[i] });
                        }
                        db.SaveChanges();

                    }




                    db.SaveChanges();
                    return Json(new { success = 1, error = obj, data = obj });
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


        [System.Web.Http.HttpGet]
        [Route("api/Meetings/GetMeetingImage")]

        // GET: api/CommitteesMembers
        public IHttpActionResult GetMeetingImage(string meetingId, string ps)
        {
            object obj = null;

            string decryptedMeetingId = HypridEncryption.decrypt(meetingId, ps);
            int decMeetingId = Convert.ToInt32(decryptedMeetingId);
            var Images=  db.MeetingImages.Where(x => x.MeetingId == decMeetingId).ToList();
            if (Images!=null || Images.Count!=0)
            {
                return Json(new { success = 1, error = obj, data = Images });

            }
            else
            {
                return Json(new { success = 0, error = obj, data = obj });
            }

        }

        [System.Web.Http.HttpGet]
        [Route("api/Meetings/GetMeetingImageForWeb")]

        // GET: api/CommitteesMembers
        public IHttpActionResult GetMeetingImageForWeb(int meetingId)
        {
            return Json(db.MeetingImages.Where(x => x.MeetingId == meetingId).ToList());
        }
    }
}