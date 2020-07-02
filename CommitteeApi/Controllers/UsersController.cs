using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CommitteeApi.Models;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Net.Http.Headers;
using System.Xml;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Security.Claims;
using CommitteeApi.Repository;
using System.Web;

namespace CommitteeApi.Controllers
{
    public class UsersController : ApiController
    {
        private Committee_DBEntities1 db = new Committee_DBEntities1();

        // GET: api/Users
        [Route("api/Users/GetUsers")]

        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        // GET: api/Users/5
        [Authorize(Roles = "Secretary, Admin, User")]
        [AllowAnonymous]

        [Route("api/Users/GetUserData")]
        [ResponseType(typeof(User))]
        [HttpGet]

        public IHttpActionResult GetUserData()
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

                //  string _userName = System.Web.HttpContext.Current.User.Identity.Name;
                //int userId = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type == "ID").Value);
                // int memberId = db.Users.FirstOrDefault(x => x.UserName == _userName).ID;
                User user = db.Users.Find(userId);

                if (user != null)
                {
                    //UserPoco userData = new UserPoco()
                    //{
                    //    ID = user.ID,
                    //    Phone = user.Phone,
                    //    UserEmailId = user.UserEmailId,
                    //    UserImage = user.UserImage,
                    //    UserName = user.UserName,
                    //    CommitteRole = user.SystemRoleMap
                    //};
                    AllUserDataPoco userData = new AllUserDataPoco()
                    {
                        ID = user.ID,
                        Address = user.Address,
                        Phone = Convert.ToInt64(user.Phone),
                        UserEmailId = user.UserEmailId,
                        UserName = user.UserName,
                        SystemRoleId = user.SystemRole,
                        SystemRole = user.SystemRoleMap,
                        WorkSide = user.WorkSide,
                        UserImage=user.UserImage,
                        Department = user.ManagerOfDepartment != null ? user.Department : null
                    };
                
            
            return Json(new { success = 1, error =obj, data = userData });
                }
                else
                {
                    return Json(new { success = 0, error = "user not found", data =user });
                }
            
            }
            catch (Exception ex)
            {
                
            return Json(new { success = 0, error = ex.Message, data = obj
});
               
            }



}

      
        [Authorize(Roles = "Secretary, Admin, User")]
        [AllowAnonymous]

        [Route("api/Users/CheckAccessToken")]
        [ResponseType(typeof(User))]
        [HttpGet]

        public bool CheckAccessToken()
        {
            object obj = null;
            int userId = 0;
          
                try
                {
                    userId = Convert.ToInt32(((ClaimsIdentity)User?.Identity).Claims?.FirstOrDefault(x => x.Type == "ID").Value);
                }
                // string _userName = System.Web.HttpContext.Current.User.Identity.Name;

                catch (Exception ex)
                {
                return false;

                }
                

                    return true;
                
               
        }
        [HttpGet]
        [ResponseType(typeof(Member))]
        [Route("api/Users/GetUserId")]
        public int GetUserId(string memberName)
        {
            int memberId = db.Users.FirstOrDefault(x => x.UserName.Trim().ToLower() == memberName.Trim().ToLower()).ID;
            if (memberId == 0)
            {
                return 0;
            }

            return memberId;
        }

        [ResponseType(typeof(User))]
        [Route("api/Users/GetUser")]
        [HttpGet]

        public User GetUser(string userName,string password)
        {
            User user = db.Users.FirstOrDefault(x => x.UserName.Trim().ToLower() == userName.Trim().ToLower()&&x.UserPassword.Trim().ToLower()==password.Trim().ToLower());
            return user;



        }

        [ResponseType(typeof(User))]
        [Route("api/Users/GetUserById")]
        [HttpGet]

        public User GetUserById(int id)
        {
            User user = db.Users.FirstOrDefault(x =>x.ID==id);
            return user;



        }

        [System.Web.Http.HttpGet]
        [Route("api/Users/GetUserByPhoneOrName")]
        public List<User> GetUserByPhoneOrName(string phone,string name,int deptId)
        {
           List<User> Users = db.Users.Where(x => x.Phone.ToString().Contains(phone.ToString().Trim())||x.Name.Trim().ToLower().Contains(name.Trim().ToLower())).ToList();
            if (Users.Count == 0)
            {
                Users = db.Users.ToList();
            }

            return Users.Where(x=>x.ManagerOfDepartment==deptId).ToList();
        }
        [System.Web.Http.HttpGet]
        [Route("api/Users/GetUserByPhoneOrNameForSystemAdmin")]
        public List<User> GetUserByPhoneOrNameForSystemAdmin(string phone, string name)
        {
            List<User> Users = db.Users.Where(x => x.Phone.ToString().Contains(phone.ToString().Trim()) || x.Name.Trim().ToLower().Contains(name.Trim().ToLower())).ToList();
            if (Users.Count == 0)
            {
                Users = db.Users.ToList();
            }

            return Users.Where(x=>x.ManagerOfDepartment!=null&&x.SystemRole!=6).ToList();
        }
        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        [Route("api/Users/UpdateUserData")]
        [Authorize(Roles = "Secretary, Admin, User")]
        [AllowAnonymous]

        [HttpPut]
        public IHttpActionResult UpdateUserData( UserUpdatePoco userData)
        {
            object obj = null;
            int userId = 0;
            try
            {
                try
                {
                    userId = Convert.ToInt32(((ClaimsIdentity)User?.Identity).Claims?.FirstOrDefault(y => y.Type == "ID").Value);
                }
                // string _userName = System.Web.HttpContext.Current.User.Identity.Name;

                catch (Exception ex)
                {


                    return Json(new { success = 0, error = "Invalid Token" });

                }
                string base64Encoded = userData.UserImage;
                string base64Decoded;
                byte[] data = System.Convert.FromBase64String(base64Encoded);
                base64Decoded = System.Text.ASCIIEncoding.ASCII.GetString(data);
                
                Random generator = new Random();
                int x = generator.Next(1, 10000000);
                File.WriteAllBytes(System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/" + x + ".png"), Convert.FromBase64String(userData.UserImage));


                //File.WriteAllBytes(System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/"+x+".jpg"), Convert.FromBase64String(userData.UserImage));
                //string _userName = System.Web.HttpContext.Current.User.Identity.;
                //int userId = db.Users.FirstOrDefault(x => x.UserName == _userName).ID;
                User user = db.Users.FirstOrDefault(y => y.ID == userId);
                user.ID = userId;
            //user.UserName = userData.UserName;
            //user.Phone = userData.Phone;
            //user.UserEmailId = userData.UserEmailId;
            user.UserImage = "/"+x+".png";
              
                //    user.Address = userData.Address;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = 1, error =obj, data = obj });
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, error = ex.Message});

            }

           
        }
        [ResponseType(typeof(void))]
        [Route("api/Notifications/updateFcmToken")]

        [HttpPut]
        public IHttpActionResult updateFcmToken(FCMObject fCMObject)
        {
            object obj = null;
            int userId = 0;
            try
            {
                string decryptedFcmToken = HypridEncryption.decrypt(fCMObject.FCMToken, fCMObject.ps);
             
                try
                {
                    userId = Convert.ToInt32(((ClaimsIdentity)User?.Identity).Claims?.FirstOrDefault(x => x.Type == "ID").Value);
                }
                // string _userName = System.Web.HttpContext.Current.User.Identity.Name;

                catch (Exception ex)
                {


                    return Json(new { success = 0, error = "Invalid Token" });

                }
                //string _userName = System.Web.HttpContext.Current.User.Identity.;
                // int userId = db.Users.FirstOrDefault(x => x.UserName == _userName).ID;
                User user = db.Users.FirstOrDefault(x => x.ID == userId);
                user.FCMToken = decryptedFcmToken;
                user.DeviceType = fCMObject.DeviceType;
                //    user.Address = userData.Address;
                db.Entry(user).State = EntityState.Modified;


                db.SaveChanges();
                return Json(new { success = 1, error = obj, data = obj });
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, error = ex.Message });

            }


        }
        //[Route("api/Users/SendVerification")]

        //[HttpPost]
        //public void SendVerification(string number)
        //{
        //    Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.;

        //    uint phoneAuthTimeoutMs = 10000;
        //    PhoneAuthProvider. provider = PhoneAuthProvider.GetInstance(auth);
        //    provider.VerifyPhoneNumber(number, phoneAuthTimeoutMs, null,
        //        verificationCompleted: (credential) => {

        //        },
        //        verificationFailed: (error) => {

        //        },
        //        codeSent: (id, token) => {
        //            MyText.text = "SMS Has been sent " + id;
        //        },
        //        codeAutoRetrievalTimeOut: (id) => {

        //        });
        //    MyText.text += "HMM";
        //}
        // POST: api/Users
        [Route("api/Users/PostUser")]
        [ResponseType(typeof(User))]
        [HttpPost]
        public void PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
            }
            
            db.Users.Add(user);
            db.SaveChanges();

            
        }

        [Route("api/Users/PostUserChatMessage")]
        [ResponseType(typeof(User))]
        [HttpPost]
        public ChatMessage PostUserChatMessage(ChatMessage chatMessage)
        {
            if (!ModelState.IsValid)
            {
            }

            db.ChatMessages.Add(chatMessage);
            db.SaveChanges();
           List<ChatMessage> chat= db.ChatMessages.Where(x=>x.CommitteeId==chatMessage.CommitteeId&&x.UserId==chatMessage.UserId&&x.Message.Trim().ToLower()==chatMessage.Message.Trim().ToLower()).OrderByDescending(x=>x.Id).ToList();
            int index = chat.Count;
            return chat[0];

        }
        [Route("api/Users/ValidateUserNameAndPassword")]

        public bool ValidateUserNameAndPassword(string userName, string password)
        {
            bool user = db.Users.Any(x => x.UserName.Trim().ToLower() == userName.Trim().ToLower() &&
            x.UserPassword.Trim().ToLower() == password.Trim().ToLower());
            if (user)
            {
                return user;

            }
            return false;
        }
        [Route("api/Users/ValidateUserPhone")]

        public bool ValidateUserMobile(Int64 Phone)
        {
            bool user = db.Users.Any(x => x.Phone == Phone.ToString());
            if (user)
            {
                return user;

            }
            return false;
        }
        [Route("api/Users/Login")]
        [ResponseType(typeof(User))]
        [HttpPost]

        public IHttpActionResult Login(Phone Phone)
        {

            object obj = null;
            String code = null;
            try
            {
                string decryptedPhone = HypridEncryption.decrypt(Phone.phone, Phone.ps);
                bool user = db.Users.Any(x => x.Phone == decryptedPhone);

                if (user)
                {
                    code = null;
                    code = "" + new Random().Next(100, 1000);

                    string phone = Phone.phone;
                    db.MobileVerifications.Add(new MobileVerification()
                    {
                        Code = Convert.ToInt32(code),
                        Phone = Convert.ToInt64(decryptedPhone),
                        DateTime = DateTime.Now
                    });

                    db.SaveChanges();





                    SMSlegan.SMSGateway.SendSms("+" + decryptedPhone, code);
                    return Json(new { success = 1, error = obj, data = code });
                }
                else
                {
                    return Json(new { success = 0, error = "Mobile not found", data = obj });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = 0, error =ex.StackTrace+ex.Message+ex.InnerException, data =obj});


            }

        }
        [Route("api/Users/VerifyMobile")]
        [ResponseType(typeof(User))]
        [HttpPost]
        public async Task<IHttpActionResult> VerifyMobile(Verify data)
        {
             object obj = null;
            try
            {

               // string phone = data.phone;
                string decryptedPhone = HypridEncryption.decrypt(data.phone, data.ps);
                string decryptedCode = HypridEncryption.decrypt(data.code, data.ps);
                int code = Convert.ToInt32(decryptedCode);

                //time now is 6 result 5:58.30
                Int64 phoneNum = Convert.ToInt64(decryptedPhone);
                DateTime dateNow = DateTime.Now;
                var res = db.MobileVerifications.ToList().LastOrDefault(x => x.Phone == phoneNum && code == x.Code);

                if (res!=null)
                {
                    DateTime dbdateTime = res.DateTime.Value.AddSeconds(90);
                    ///login 6
                    bool check = db.MobileVerifications.Any(x => x.Phone == phoneNum && dateNow <= dbdateTime && code == x.Code);
                    if (check)
                    {

                        User user = db.Users.FirstOrDefault(x => x.Phone.ToString().ToLower().Trim() == decryptedPhone.ToString().Trim().ToLower());

                        HttpClient client = CreateHttpClient();

                        client.BaseAddress = new Uri(Utilities.BASE_URL);

                        var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("phone", decryptedPhone.ToString()),
            new KeyValuePair<string, string>("code", decryptedCode.ToString())
        });
                        /// plesk - site - preview / leganapi.itmisr.net / token

                        var result = await client.PostAsync(Utilities.BASE_URL + "/token", content);

                        if (!result.IsSuccessStatusCode)
                        {

                            string resultContent = await result.Content.ReadAsStringAsync();
                            resultContent = resultContent.Replace(".issued", "issued").Replace(".expires", "expires");
                            TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(resultContent);

                            //return Ok(tokenResponse);
                            return Json(new { success = 0, error = "mobile not found", data = obj });
                        }
                        else
                        {
                            string resultContent = await result.Content.ReadAsStringAsync();
                            resultContent = resultContent.Replace(".issued", "issued").Replace(".expires", "expires");
                            TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(resultContent);

                            //return Ok(tokenResponse);

                            return Json(new
                            {
                                success = 1,
                                error = "",
                                data = new
                                {
                                    token = tokenResponse,
                                    user = new AllUserDataPoco()
                                    {
                                        ID = user.ID,
                                        Address = user.Address,
                                        Phone = Convert.ToInt64(user.Phone),
                                        UserEmailId = user.UserEmailId,
                                        UserName = user.UserName,
                                        SystemRoleId = user.SystemRole,
                                        SystemRole = user.SystemRoleMap,
                                        UserImage = user.UserImage,
                                        WorkSide = user.WorkSide,
                                        Department = user.ManagerOfDepartment != null ? user.Department : null
                                    }
                                }
                            });
                        }

                    }


                    else
                    {

                        return Json(new { success = 0, error = "verification code incorrect", data = obj });
                    }
                }


                else
                {
                    return Json(new { success = 0, error = "verification code incorrect", data = obj });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, error = ex.InnerException, data = obj });


            }

        }
        //[Route("api/Users/LogOut")]
        //[ResponseType(typeof(User))]
        //[HttpPost]
        //public IHttpActionResult LogOut()
        //{

        //    try
        //    {

        //        String authorization = request.getHeader("Authorization");
        //        if (authorization != null && authorization.contains("Bearer"))
        //        {
        //            String tokenId = authorization.substring("Bearer".length() + 1);
        //            tokenServices.revokeToken(tokenId);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = 0, error = ex.Message + ex.InnerException, data = new { } });


        //    }

        //}
        [ResponseType(typeof(User))]
        [Route("api/Users/GetUser")]
        [HttpGet]

        public User GetUser(int id)
        {
            User user = db.Users.FirstOrDefault(x => x.ID == id);
            //FCMObject so = new FCMObject()
            //{
            //    FCMToken = user.FCMToken,
            //    DeviceType = user.DeviceType

            //};
            return user;



        }
        private HttpClient CreateHttpClient()
        {
            HttpClientHandler handler = new HttpClientHandler() { UseDefaultCredentials = false };
            HttpClient client = new HttpClient(handler);
           
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            return client;
        }
        // DELETE: api/Users/5
       
        [Route("api/Users/DeleteUser")]
        [ResponseType(typeof(User))]
        [HttpGet]
        public int DeleteUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
             
              
            }

           List<Alert> cUsers = db.Alerts.Where(x => x.UserId == id).ToList();
            foreach (var cuser in cUsers)
            {
                db.Alerts.Remove(cuser);
                db.SaveChanges();
            }
            db.Users.Remove(user);

            db.SaveChanges();
            return user.ID;
            
        }

        [ResponseType(typeof(void))]
        [Route("api/Users/PutUser")]
        [HttpPost]


        public void PutUser(int id, User user)
        {




            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
            }

        }


        public Dictionary<String, String> UsersToVerify = new Dictionary<string, string>(); 


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.ID == id) > 0;
        }
    }
    public class TokenRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class TokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
     
    }


   
}