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

namespace CommitteeApi.Controllers
{
    public class MembersController : ApiController
    {
        private Committee_DBEntities1 db = new Committee_DBEntities1();

        [Route("api/Member/GetMembers")]

        // GET: api/Committees/5
        [ResponseType(typeof(Committee))]
        // GET: api/Members
        public IQueryable<Member> GetMembers()
        {
            return db.Members;
        }

        // GET: api/Members/5
        [ResponseType(typeof(Member))]
        [Route("api/Member/GetMember")]
        public IHttpActionResult GetMember(string phone)
        {
            Member member = db.Members.FirstOrDefault(x=>x.MemberPhone==phone.ToString());
            if (member == null)
            {
                return NotFound();
            }

            return Ok(member);
        }
        [HttpGet]
        [ResponseType(typeof(Member))]
        [Route("api/Member/GetMemberId")]
        public int GetMemberId(string memberName)
        {
            int memberId = db.Members.FirstOrDefault(x => x.MemberName.Trim().ToLower() == memberName.Trim().ToLower()).MemberId;
            if (memberId == 0)
            {
                return 0;
            }

            return memberId;
        }

        // PUT: api/Members/5
        [ResponseType(typeof(void))]
        [Route("api/Member/PutMember")]
        [HttpPost]

        public void PutMember(int id, Member member)
        {
          

           

            db.Entry(member).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
            }

        }

        // POST: api/Members
        [ResponseType(typeof(Member))]
        [Route("api/Member/PostMember")]
        public void PostMember(Member member)
        {

            db.Members.Add(member);
            db.SaveChanges();

          
        }

        // DELETE: api/Members/5
        [Route("api/Member/DeleteMember")]
        [ResponseType(typeof(Member))]
        public IHttpActionResult DeleteMember(int id)
        {
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return NotFound();
            }

            db.Members.Remove(member);
            db.SaveChanges();

            return Ok(member);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MemberExists(int id)
        {
            return db.Members.Count(e => e.MemberId == id) > 0;
        }
    }
}