using CommitteeApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CommitteeApi.Controllers
{
    public class ChartController : ApiController
    {

        private Committee_DBEntities1 db = new Committee_DBEntities1();
        //[Authorize(Roles = "SuperAdmin, Admin, User")]
        [System.Web.Http.HttpGet]
        [Route("api/Chart/GetActiveAndNonActiveCommittee")]
        public List<int> GetActiveAndNonActive()
        {
            List<int> GetActiveAndNonActiveCommittee = new List<int>();
            int activeCommittee = db.Committees.Where(x => x.Activity.Id == 1).Count();
            int InactiveCommittee = db.Committees.Where(x => x.Activity.Id == 2).Count();
            GetActiveAndNonActiveCommittee.Add(activeCommittee);
            GetActiveAndNonActiveCommittee.Add(InactiveCommittee);
            return GetActiveAndNonActiveCommittee;


        }
        [System.Web.Http.HttpGet]
        [Route("api/Chart/GetTypeCommittee")]
        public List<int> GetTypeCommittee()
        {
            List<int> GetTypeCommittee = new List<int>();
            int militaryCommittee = db.Committees.Where(x => x.Type.Id == 1).Count();
            int civilCommittee = db.Committees.Where(x => x.Type.Id == 2).Count();
            GetTypeCommittee.Add(militaryCommittee);
            GetTypeCommittee.Add(civilCommittee);
            return GetTypeCommittee;


        }
        [System.Web.Http.HttpGet]
        [Route("api/Chart/AcceptedAndRejectedCount")]
        public List<int> AcceptedAndRejectedCount()
        {
            List<int> AcceptedAndRejectedCount = new List<int>();
            int AcceptedMinute = db.CommitteesMembers.Where(x => x.IsMemberAcceptedMiutesOfCommittee == true).Count();
            int RejectedMinute = db.CommitteesMembers.Where(x => x.IsMemberAcceptedMiutesOfCommittee == false).Count();
            AcceptedAndRejectedCount.Add(AcceptedMinute);
            AcceptedAndRejectedCount.Add(RejectedMinute);
            return AcceptedAndRejectedCount;


        }
        [System.Web.Http.HttpGet]
        [Route("api/Chart/GetMemeberCount")]
        public int GetMemeberCount()
        {
            int membersCount = db.Users.Where(x => x.ID != 0).Count();

            return membersCount;


        }
        [System.Web.Http.HttpGet]
        [Route("api/Chart/GetCommitteeCount")]
        public int GetCommitteeCount()
        {
            int committeesCount = db.Committees.Where(x => x.CommitteeId != 0).Count();

            return committeesCount;


        }
        [Route("api/Chart/GetDepartmentsOfEachCommittee")]
        [HttpGet]

        public List<DepartmentCommittees> GetDepartmentsOfEachCommittee()
        {
            List<DepartmentCommittees> departmentCommittees = new List<DepartmentCommittees>();
            var results = from p in db.Committees where p.Department.DeptName != null || p.Department.DeptId != 0
                          group p.CommitteeName by p.Department.DeptName.Trim().ToLower() into g

                          select new { Department = g.Key, Committees = g.Count() };

            var newResults = results.Where(x => x.Department != null);
            if (results != newResults)
            {
                foreach (var res in results)
                {
                    departmentCommittees.Add(new DepartmentCommittees() { DeptName = res.Department==null?"لجان بدون ادارة": res.Department, CommitteesCount = res.Committees });

                }
            }
            return departmentCommittees.Where(x => x.DeptName != null || x.DeptName!="").ToList();
        }
        [System.Web.Http.HttpGet]
        [Route("api/Chart/GetDepartmentCount")]
        public int GetDepartmentCount()
        {
            int departmentCount = db.Departments.Where(x => x.DeptId != 0).Count();

            return departmentCount;


        }
    }
}
