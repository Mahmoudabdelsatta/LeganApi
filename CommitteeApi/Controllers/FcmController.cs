using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Script.Serialization;
using CommitteeApi.Models;
using CommitteeApi.Repository;

namespace CommitteeApi.Controllers
{
    public class FcmController : ApiController
    {
        private Committee_DBEntities1 db = new Committee_DBEntities1();

        [HttpPost]
        [Route("api/Fcm/SendMessage")]
        public IHttpActionResult SendMessage(string _to, FireBaseData fireBaseData)
        {
            
            try
            {
               
                if (_to != null)
                {
                    var data = new
                    {
                        to = _to,
                        //to = "fQcj264J6Iw:APA91bGFmVPo9rg4LRZC-zSNeDqn6dzsTLkUVuWEqFRr0cgZ6W2HHwYUWXWfzv1HuzK2XfJe0K9DusPw8aZaqZ1IiNexAcExVYLTomgzAeJozKXA6HVI6VFRn77UiQOUe96B7cm9n0VJ",
                        data = new FireBaseData()
                        {
                            Title = fireBaseData.Title,
                            Body = fireBaseData.Body,
                            Action_Id1 = fireBaseData.Action_Id1,
                            Action_Type = fireBaseData.Action_Type,
                            Action_Id2 = fireBaseData.Action_Id2,
                            CreatedAt = fireBaseData.CreatedAt
                        }
                    };
                    SendNotification(data);
                    return Ok(data);
                }
                return Ok();

            }
            catch (Exception ex)
            {
                string filePath = @Utilities.LogError_Path + "Error.txt";


                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("-----------------------------------------------------------------------------");
                    writer.WriteLine("Date : " + DateTime.Now.ToString());
                    writer.WriteLine();

                    while (ex != null)
                    {
                        writer.WriteLine(ex.GetType().FullName);
                        writer.WriteLine("Message : " + ex.Message+ _to);
                        writer.WriteLine("StackTrace : " + ex.StackTrace);

                        ex = ex.InnerException;
                    }
                }
                return NotFound();
            }
            
        }

        private void SendNotification(object data)
        {
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            byte[] byteArray = Encoding.UTF8.GetBytes(json);
            SendNotification(byteArray);
        }
        private void SendNotification(byte[] byteArray)
        {
            try
            {
                string server_api_key = ConfigurationManager.AppSettings["SERVER_API_KEY"];
                string sender_id = ConfigurationManager.AppSettings["SENDER_ID"];
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = " application/json";
                tRequest.Headers.Add($"Authorization: Key={ server_api_key}");
                tRequest.Headers.Add($"Sender: id={ sender_id}");
                tRequest.ContentLength = byteArray.Length;
                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse tResponse = tRequest.GetResponse();
                dataStream = tResponse.GetResponseStream();
                StreamReader tReader = new StreamReader(dataStream);
                string sResponseFromServer = tReader.ReadToEnd();
                tReader.Close();
                dataStream.Close();




            }
            catch (Exception)
            {

                throw;
            }
          
        }
        // GET: api/Fcm
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        // GET: api/Fcm/5
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Fcm/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.ID)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Fcm
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Users.Add(user);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = user.ID }, user);
        }

        // DELETE: api/Fcm/5
        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }

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
}