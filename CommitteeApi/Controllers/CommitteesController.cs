using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;
using CommitteeApi.Models;
using CommitteeApi.Repository;
using Newtonsoft.Json;

namespace CommitteeApi.Controllers
{
    public class CommitteesController : ApiController
    {
        private Committee_DBEntities1 db = new Committee_DBEntities1();
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [AllowAnonymous]

        [System.Web.Http.HttpGet]
        [Route("api/Committees/GetCommittees")]

        // GET: api/Committees
        public IHttpActionResult GetCommittees()
        {
            List<CommitteePoco> committeesPoco = new List<CommitteePoco>();
            object obj = null;
            int userId = 0;
            List<Committee> Committees = new List<Committee>();
            List<CommitteesMember> committeesMember = new List<CommitteesMember>();
            try
            {
                try
                {
                    userId = Convert.ToInt32(((ClaimsIdentity)User?.Identity).Claims?.FirstOrDefault(x => x.Type == "ID").Value);
                }
                // string _userName = System.Web.HttpContext.Current.User.Identity.Name;

                catch (Exception ex)
                {


                    return Json(new { success = 0, error = "Invalid Token" });

                }
                User user = db.Users.FirstOrDefault(x => x.ID == userId);
                if (user.SystemRoleMap.titleEn == "Admin" || user.SystemRoleMap.titleEn == "Secrtary")
                {
                    Committees = db.Committees.Select(x => x).ToList();
                }
                if (user.SystemRoleMap.titleEn == "Department Manger")
                {
                    Committees = db.Committees.Where(x => x.DeptId == user.ManagerOfDepartment).ToList();
                }
                if (user.SystemRoleMap.titleEn == "Member")
                {
                    committeesMember = db.CommitteesMembers.Where(x => x.MemberId == userId).ToList();

                    if (committeesMember.Count != 0)
                    {
                        foreach (var Committee in committeesMember)
                        {
                            DateTime t = DateTime.Parse(Committee.CreatedAt);


                            committeesPoco.Add(new CommitteePoco()
                            {

                                CommitteeId = Committee.Committee.CommitteeId,
                                CommitteeName = Committee.Committee.CommitteeName,
                                Activity = Committee.Committee.Activity,

                                Importance = Committee.Committee.Importance,
                                Type = Committee.Committee.Type,
                                MembersCount = db.CommitteesMembers.Where(x => x.CommitteeId == Committee.CommitteeId).Count(),

                                CreatedAt = UnixStamp.DateTimeToUnixTimestamp(t)
                            ,
                            });
                        };
                        return Json(new
                        {
                            success = 1,
                            error = obj,
                            data = committeesPoco.GroupBy(a => a.CommitteeId)
                 .Select(g => g.First()).Distinct().ToList()
                        });

                    }


                }
                    
                    // Committees = 

                    if (Committees.Count != 0)
                    {
                        foreach (var Committee in Committees)
                        {
                            DateTime t = DateTime.Parse(Committee.CreatedAt);


                            committeesPoco.Add(new CommitteePoco()
                            {

                                CommitteeId = Committee.CommitteeId,
                                CommitteeName = Committee.CommitteeName,
                                Activity = Committee.Activity,

                                Importance = Committee.Importance,
                                Type = Committee.Type,
                                MembersCount = db.CommitteesMembers.Where(x => x.CommitteeId == Committee.CommitteeId).Count(),

                                CreatedAt = UnixStamp.DateTimeToUnixTimestamp(t)
                            ,
                            });
                        };
                        return Json(new
                        {
                            success = 1,
                            error = obj,
                            data = committeesPoco.GroupBy(a => a.CommitteeId)
                      .Select(g => g.First()).Distinct().ToList()
                        });
                    }
                    else
                    {
                        return Json(new { success = 0, error = "no data found", data = obj });

                    }


                
            }
            catch (Exception ex)
            {

                return Json(new { success = 0, error = ex.InnerException });


            }

        }

        [Route("api/Committees/GetCommitteesForWeb")]
        // GET: api/Committees
        public IQueryable<Committee> GetCommitteesForWeb(int deptId)
        {
            return db.Committees.Where(x=>x.DeptId==deptId);
        }
        [Authorize(Roles = "SuperAdmin, Admin, User")]

        [Route("api/Committees/GetCommittee")]

        // GET: api/Committees/5
        [ResponseType(typeof(Committee))]
        public IHttpActionResult GetCommittee(int committeeId)
        {
            List<CommitteesMember> committeedetails = db.CommitteesMembers.Where(x => x.CommitteeId == committeeId).ToList();
            if (committeedetails == null || committeedetails.Count == 0)
            {
                return NotFound();
            }

            return Ok(committeedetails);
        }
        [Route("api/Committees/GetCommitteeId")]

        // GET: api/Committees/5
        [ResponseType(typeof(Committee))]
        public int GetCommitteeId(string committeeName)
        {
            int committeeId = db.Committees.FirstOrDefault(x => x.CommitteeName == committeeName).CommitteeId;
            if (committeeId == null)
            {

            }

            return committeeId;
        }
        [Route("api/Committees/GetMinutesOfCommitteePDF")]

        // GET: api/Committees/5
        [ResponseType(typeof(Committee))]
        public IHttpActionResult GetMinutesOfCommitteePDF(int committeeId)
        {
            string MinutesOfCommitteePDF = db.Committees.FirstOrDefault(x => x.CommitteeId == committeeId).CommitteePDFView;
            if (MinutesOfCommitteePDF == null || MinutesOfCommitteePDF == "")
            {
                return NotFound();
            }

            return Ok(MinutesOfCommitteePDF);
        }
        //  [Authorize(Roles = "SuperAdmin, Admin, User")]

        [Authorize(Roles = "Secretary, Admin, User")]
        [Route("api/Committees/FilterCommittee")]


        [HttpGet]

        [ResponseType(typeof(Committee))]
        public IHttpActionResult FilterCommittee(bool isActive, bool isImportant, bool isMilitarized)
        {
            try
            {
                List<Committee> committees = db.Committees.Where(x => x.IsActive == isActive && x.IsImportant == isImportant && x.IsMilitarized == isMilitarized).ToList();

                return Json(new { success = 1, committees = committees });

            }
            catch (Exception ex)
            {

                return Json(new { success = 0, error = "error message." + ex.InnerException });

            }
            //return NotFound();

            //return Ok();


        }
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [AllowAnonymous]

        // [Authorize(Roles = "Secretary, Admin, User")]
        [Route("api/Committees/GetCommitteeById")]

        [HttpGet]

        [ResponseType(typeof(Committee))]
        public IHttpActionResult GetCommitteeById(string committeeid, string ps)
        {
            object obj = null;
            int userId = 0;
            try
            {
                string decryptedCommitteeId = HypridEncryption.decrypt(committeeid, ps);
                int decryptedcommitteeId = Convert.ToInt32(decryptedCommitteeId);
                List<UserCommitteePoco> users = new List<UserCommitteePoco>();
                List<MeetingPoco> meetings = new List<MeetingPoco>();
                CommitteesMember cm = db.CommitteesMembers.FirstOrDefault(x => x.CommitteeId == decryptedcommitteeId);
                if (cm != null)
                {

                    //   string _userName = System.Web.HttpContext.Current.User.Identity.Name;
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

                    string commmitteManger = db.Committees.FirstOrDefault(x => x.CommitteeId == decryptedcommitteeId).CommitteeManager;
                    string commmitteSecratry = db.Committees.FirstOrDefault(x => x.CommitteeId == decryptedcommitteeId).CommitteeSecretary;
                    int mangId = Convert.ToInt32(commmitteManger);
                    User userManager = db.Users.FirstOrDefault(x => x.ID == mangId);
                    int secId = Convert.ToInt32(commmitteSecratry);
                    User userSecratry = db.Users.FirstOrDefault(x => x.ID == secId);
                    List<CommitteesMember> committeesMeeting = db.CommitteesMembers.Where(x => x.CommitteeId == decryptedcommitteeId).ToList();
                    foreach (var committeeMeeting in committeesMeeting)
                    {
                        DateTime createdAt = DateTime.Parse(committeeMeeting?.CreatedAt);
                        string t = Utilities.ConvertDateCalendar( Convert.ToDateTime(committeeMeeting?.Meeting?.MeetingDate), "Gregorian", "en-US");
                        if (committeeMeeting.Meeting == null)
                        {
                           
                        }
                        else
                        {


                            DateTime meetingDate = DateTime.Parse(t);

                            if (!meetings.Any(x => x.MeetingId == committeeMeeting?.Meeting?.MeetingId))
                            {
                                meetings.Add(new MeetingPoco() { MeetingId = committeeMeeting.Meeting.MeetingId, MeetingAddress = committeeMeeting.Meeting.MeetingAddress, MeetingTitle = committeeMeeting.Meeting.MeetingTitle, MeetingDate = UnixStamp.DateTimeToUnixTimestamp(meetingDate), CreatedAt = UnixStamp.DateTimeToUnixTimestamp(createdAt), IsAttended = committeeMeeting.MemberWillAttend });

                            }
                        }
                    }
                    List<CommitteesMember> members = db.CommitteesMembers.Where(x => x.CommitteeId == decryptedcommitteeId).ToList();
                    users.Add(new UserCommitteePoco()
                    {
                        ID = userManager?.ID,
                        Phone =Convert.ToInt64(userManager?.Phone),
                        UserEmailId = userManager?.UserEmailId,
                        UserImage = userManager?.UserImage,
                        UserName = userManager?.UserName,
                        WorkSide=userManager?.WorkSide,
                        CommitteRole = db.Roles.FirstOrDefault(x => x.ID == 5)
                    });
                    users.Add(new UserCommitteePoco()
                    {
                        ID = userSecratry?.ID,
                        Phone = Convert.ToInt64(userSecratry?.Phone),
                        UserEmailId = userSecratry?.UserEmailId,
                        UserImage = userSecratry?.UserImage,
                        UserName = userSecratry?.UserName,
                        WorkSide = userSecratry?.WorkSide,
                        CommitteRole = db.Roles.FirstOrDefault(x => x.ID == 4)
                    });
                    if (members.Count!=0)
                    {


                        foreach (var member in members.GroupBy(a => a.MemberId)
                     .Select(g => g.First()).Distinct().ToList())
                        {
                            if (!users.Any(x => x.ID == member.MemberId))
                            {


                                users.Add(new UserCommitteePoco()
                                {
                                    ID = member?.User?.ID,
                                    UserName = member?.User?.UserName,
                                    Phone = Convert.ToInt64(member?.User?.Phone),
                                    UserEmailId = member?.User?.UserEmailId,
                                    UserImage = member?.User?.UserImage,
                                    WorkSide = userManager?.WorkSide,
                                    CommitteRole = db.Roles.FirstOrDefault(x => x.ID == 6)


                                });

                            }
                        }
                    }
                    else
                    {
                        members = null;
                    }
                  //  UserCommitteePoco committeeManger = new UserCommitteePoco() { ID = userManager.ID, Phone = userManager.Phone, UserEmailId = userManager.UserEmailId, UserImage = userManager.UserImage, UserName = userManager.UserName, CommitteRole = db.Roles.FirstOrDefault(x => x.ID == 5) };
                   // UserCommitteePoco committeeSecrtary = new UserCommitteePoco() { ID = userSecratry.ID, UserName = userSecratry.UserName, Phone = userSecratry.Phone, UserEmailId = userSecratry.UserEmailId, UserImage = userSecratry.UserImage, CommitteRole = db.Roles.FirstOrDefault(x => x.ID == 4) };

                    CommitteeMeetingPoco committeDetails = new CommitteeMeetingPoco()
                    {
                        CommitteeId = decryptedcommitteeId,
                        
                        CommitteeName = cm.Committee.CommitteeName,
                        CommitteeTopic=cm.Committee.CommitteeTopic,
                        CommitteeDependOn=cm.Committee.CommitteeBasedOn,
                        CommitteeInbox=cm.Committee.CommitteeInbox1,
                        CommitteeEnrollmentNum=cm.Committee.CommitteeEnrollmentNumber,
                        CommitteeEnrollmentDate=cm.Committee.CommitteeEnrollmentDate,
                        Department=cm.Committee?.Department?.DeptName,
                     //   CommitteeManager = committeeManger,
                       // CommitteeSecretary = committeeSecrtary,
                        Activity = cm.Committee.Activity,
                        Importance = cm.Committee.Importance,
                        Type = cm.Committee.Type,
                        Members = users.Where(x=>x.ID!=null ).ToList(),

                        Meetings = meetings,


                    };
                    
                    //MemberName=x.Member.MemberName,
                    //MemberTitle=x.Member.MemberTitle,
                    //MeetingId=x.Meeting.MeetingId,
                    //MeetingDate=x.Meeting.MeetingDate,
                    //MeetingAddress=x.Meeting.MeetingAddress

                    //List<MemberPoco> members = new List<MemberPoco>();
                    //List<MeetingPoco> meetings = new List<MeetingPoco>();
                    //List<object> container = new List<object>();
                    //string Milarty = "";
                    //string Important = "";
                    //foreach (var item in committeDetails)
                    //{
                    //    Milarty = item.Miltraized;
                    //    Important = item.Important;
                    //    members.Add(item.Member);
                    //    meetings.Add(item.Meeting);
                    //};
                    //container.Add(new { Miltraized = Milarty, Important = Important });
                    //// ss.Add(new { Important = Important });
                    //container.Add(new { Members = members });
                    //container.Add(new { Meetings = meetings });

                    return Json(new { success = 1, error = obj, data = committeDetails });

                }

                else
                {
                    Committee committee = db.Committees.FirstOrDefault(x => x.CommitteeId == decryptedcommitteeId);
                    if (committee != null)
                    {


                        try
                        {
                            userId = Convert.ToInt32(((ClaimsIdentity)User?.Identity).Claims?.FirstOrDefault(x => x.Type == "ID").Value);
                        }
                        // string _userName = System.Web.HttpContext.Current.User.Identity.Name;

                        catch (Exception ex)
                        {


                            return Json(new { success = 0, error = "Invalid Token" });

                        }

                        string commmitteManger = db.Committees.FirstOrDefault(x => x.CommitteeId == decryptedcommitteeId).CommitteeManager;
                        string commmitteSecratry = db.Committees.FirstOrDefault(x => x.CommitteeId == decryptedcommitteeId).CommitteeSecretary;
                        int mangId = Convert.ToInt32(commmitteManger);
                        User userManager = db.Users.FirstOrDefault(x => x.ID == mangId);
                        int secId = Convert.ToInt32(commmitteSecratry);
                        User userSecratry = db.Users.FirstOrDefault(x => x.ID == secId);

                        CommitteeMeetingPoco committeDetails = new CommitteeMeetingPoco()
                        {
                            CommitteeId = decryptedcommitteeId,

                            CommitteeName = committee.CommitteeName,
                            CommitteeTopic = committee.CommitteeTopic,
                            CommitteeDependOn = committee.CommitteeBasedOn,
                            CommitteeInbox = committee.CommitteeInbox1,
                            CommitteeEnrollmentNum = committee.CommitteeEnrollmentNumber,
                            CommitteeEnrollmentDate = committee.CommitteeEnrollmentDate,
                            Department = committee?.Department?.DeptName,
                            //   CommitteeManager = committeeManger,
                            // CommitteeSecretary = committeeSecrtary,
                            Activity = committee.Activity,
                            Importance = committee.Importance,
                            Type = committee.Type,
                            Members = new List<UserCommitteePoco>(),

                            Meetings = new List<MeetingPoco>(),


                        };
                        return Json(new { success = 1, error = obj, data = committeDetails });
                    }
                    else
                    {
                        return Json(new { success = 0, error = "Committee not found", data = obj });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, error = ex.Message + ex.InnerException });
            }

        }

        [Authorize(Roles = "Secretary, Admin, User")]
        [AllowAnonymous]

        [Route("api/Committees/GetCommitteeChat")]
        [ResponseType(typeof(User))]
        [HttpGet]

        public IHttpActionResult GetCommitteeChat(string chatMessageId, string committeeId, string ps)
        {
            object obj = null;
            int userId = 0;
            List<ChatMessage> topMessages = new List<ChatMessage>();
            try
            {
                string decryptedCommitteeId = HypridEncryption.decrypt(committeeId, ps);
                string decryptedchatMessageId = HypridEncryption.decrypt(chatMessageId, ps);
                int decCommitteeId = Convert.ToInt32(decryptedCommitteeId);
                int decChatMessageId = Convert.ToInt32(decryptedchatMessageId);
                try
                {
                    userId = Convert.ToInt32(((ClaimsIdentity)User?.Identity).Claims?.FirstOrDefault(x => x.Type == "ID").Value);
                }
                // string _userName = System.Web.HttpContext.Current.User.Identity.Name;

                catch (Exception ex)
                {


                    return Json(new { success = 0, error = "Invalid Token" });

                }

                //  string _userName = System.Web.HttpContext.Current.User.Identity.Name;
                //int userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type == "ID").Value);
                // int memberId = db.Users.FirstOrDefault(x => x.UserName == _userName).ID;
                if (decChatMessageId == 0)
                {
                    topMessages = (from c in db.ChatMessages
                                                     where  c.CommitteeId == decCommitteeId
                                   orderby c.Id descending
                                                     select c).Take(50).ToList();
                }
                else
                {

                   topMessages = (from c in db.ChatMessages
                                                     where c.Id < decChatMessageId & c.CommitteeId == decCommitteeId
                                  orderby c.Id descending
                                                     select c).Take(50).ToList();
                }


                if (topMessages.Count != 0)

                { 
                    return Json(new { success = 1, error = obj, data = topMessages });
                }
                else
                {
                    return Json(new { success = 0, error = "no data found", data = obj });
                }

            }
            catch (Exception ex)
            {

                return Json(new
                {
                    success = 0,
                    error = ex.Message,
                    data = obj
                });

            }



        }
        [Route("api/Committees/GetCommitteeByIdForWeb")]

        // GET: api/Committees/5
        [ResponseType(typeof(Committee))]
        public Committee GetCommitteeByIdForWeb(int committeeId)
        {
            Committee committeedetails = db.Committees.FirstOrDefault(x => x.CommitteeId == committeeId);
            if (committeedetails == null)
            {

            }

            return committeedetails;
        }
        [Route("api/Committees/IsCommitteeExist")]

        [HttpGet]

        [ResponseType(typeof(Committee))]
        public bool IsCommitteeExist(string committeeName)
        {
            bool isCommitteeExist = db.Committees.Any(x => x.CommitteeName.Trim().ToLower().Contains(committeeName.Trim().ToLower()));
            if (isCommitteeExist)
            {
                return isCommitteeExist;
            }
            return false;
        }
        [Route("api/Committees/GetCommitteeDetails")]

        [HttpGet]

        [ResponseType(typeof(Committee))]
        public IHttpActionResult GetCommitteeDetails(string committeeName, string commiteeDate,int deptId)
        {
            List<Committee> committeeDeatils = db.Committees.Where(x => x.CommitteeName.Trim().ToLower().Contains(committeeName.Trim().ToLower())||x.CommitteeDate.ToLower().Trim().Contains(commiteeDate.ToLower().Trim())).ToList();
            //committeeDeatils = committeeDeatils.Where(x => x.DeptId == deptId).ToList();
            if (committeeDeatils.Count==0)
            {
                committeeDeatils = db.Committees.ToList();
            }
            return Json(committeeDeatils.Where(x => x.DeptId == deptId).ToList());
        }
      

        [System.Web.Http.HttpGet]
        [Route("api/Committees/GetAppData")]

        // GET: api/Committees
        public IHttpActionResult GetAppData()
        {
            object obj = null;
            try
            {
                List<Activity> activity = db.Activities.Select(x => x).ToList();
                List<Importance> importance = db.Importances.Select(x => x).ToList(); ;
                List<Models.Type> type = db.Types.Select(x => x).ToList();
                AppDataPoco appdata = new AppDataPoco()
                {
                    Activity = activity,
                    Importance = importance,
                    Type = type
                };
                return Json(new { success = 1, error = obj, data = appdata });



            }
            catch (Exception ex)
            {
                return Json(new { success = 0, error = ex.Message });


            }

        }

        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [AllowAnonymous]
        [System.Web.Http.HttpGet]
        [Route("api/Committees/getNotifications")]

        // GET: api/Committees
        public IHttpActionResult getNotifications()
        {
            object obj = null;
            int userId = 0;
            try
            {
                try
                {
                    userId = Convert.ToInt32(((ClaimsIdentity)User?.Identity).Claims?.FirstOrDefault(x => x.Type == "ID").Value);
                }
                // string _userName = System.Web.HttpContext.Current.User.Identity.Name;

                catch (Exception ex)
                {


                    return Json(new { success = 0, error = "Invalid Token" });

                }
                List<AlertPoco> alertPoco = new List<AlertPoco>();
                List<Alert> alerts = db.Alerts.Where(x => x.UserId == userId).ToList();
                foreach (var alert in alerts)
                {
                    DateTime t = DateTime.Parse(alert.CreatedAt);

                    alertPoco.Add(new AlertPoco()
                    {

                        Id = alert.Id,
                        Title = alert?.Title,
                        UserId = userId,
                        Message = alert.Message,
                        CreatedAt = UnixStamp.DateTimeToUnixTimestamp(t),
                        Action_Id1=alert?.Action_Id1,
                        Action_Id2=alert?.Action_Id2,
                        Action_Type=alert?.ClickAction?.Click_Action
                        
                    });
                };

                if (alertPoco.Count!=0)
                {
            return Json(new { success = 1, error = obj, data = alertPoco.OrderByDescending(x=>x.Id) });
                }
                else
                {
                    return Json(new { success = 0, error = "No data", data = obj });

                }





            }
            catch (Exception ex)
            {
                return Json(new { success = 0, error = ex.Message });


            }

        }
        // PUT: api/Committees/5
        [ResponseType(typeof(void))]
        [Route("api/Committees/PutCommittee")]
        [HttpPost]
        public void PutCommittee(int id, Committee committee)
        {
            

            db.Entry(committee).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommitteeExists(id))
                {
                   
                }
                else
                {
                    throw;
                }
            }

            
        }
        [HttpGet]
        [Route("api/Committees/GetMeetingId")]
        public int? GetMeetingId(int committeeId, string createdAt)
        {
            int? meetingId = db.CommitteesMembers.FirstOrDefault(x => x.CommitteeId == committeeId && x.CreatedAt.Trim().ToLower()==createdAt.Trim().ToLower()).MeetingId;
            if (meetingId == 0)
            {
                return 0;
            }

            return meetingId;
        }

        // POST: api/Committees
        [Route("api/Committees/PostCommittee")]

        [ResponseType(typeof(Committee))]
        public IHttpActionResult PostCommittee(Committee committee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Committees.Add(committee);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new {id = committee.CommitteeId }, committee);
        }
        [Route("api/Committees/PostAlert")]
        [HttpPost]
     [ResponseType(typeof(Alert))]
        public void PostAlert(Alert alert)
        {

            db.Alerts.Add(alert);
            db.SaveChanges();

            
        }
        [Route("api/Committees/PostCommitteeForWeb")]
        [HttpPost]
        [ResponseType(typeof(Committee))]
        public void PostCommitteeForWeb(Committee committee)
        {
            if (committee!=null)
            {
 db.Committees.Add(committee); 
            db.SaveChanges();
            }
           
            
        }
        [Route("api/Deapartments/GetDepartments")]
        [HttpGet]
        [ResponseType(typeof(Department))]
        public IHttpActionResult GetDepartments()
        {
            List<Department> departments = db.Departments.Select(x=>x).ToList();
            if (departments == null || departments.Count == 0)
            {
                return NotFound();
            }

            return Ok(departments);
        }
        [System.Web.Http.HttpGet]
        [Route("api/Committees/GetCommitteeMeetings")]
        [ResponseType(typeof(Meeting))]
        // GET: api/CommitteesMembers
        public List<Meeting> GetCommitteeMeetings(int committeeId)
        {
            List<Meeting> meetings = db.Meetings.Where(x => x.CommitteeId == committeeId).ToList();
            if (meetings == null)
            {
                
            }

            return meetings;
        }


        [Route("api/Committees/DeleteCommittee")]
        [HttpGet]
        // DELETE: api/Committees/5
        [ResponseType(typeof(Committee))]
        public string DeleteCommittee(string id)
        {
            if (id!=null)
            {
 Committee committee = db.Committees.Find(Convert.ToInt32(id));
           

            db.Committees.Remove(committee);
            db.SaveChanges();

            }
            return id;
          
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CommitteeExists(int id)
        {
            return db.Committees.Count(e => e.CommitteeId == id) > 0;
        }
    }
}