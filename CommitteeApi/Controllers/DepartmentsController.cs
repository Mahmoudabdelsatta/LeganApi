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
    public class DepartmentsController : ApiController
    {
        private Committee_DBEntities1 db = new Committee_DBEntities1();

        // GET: api/Departments
        [Route("api/Departments/GetDepartments")]
        [HttpGet]
        [ResponseType(typeof(Department))]
        public IHttpActionResult GetDepartments(string name)
        {
            List<Department> departments = db.Departments.Where(x => x.DeptName.Contains(name)||x.DeptAddress.Contains(name)).ToList();
            if (departments == null || departments.Count == 0)
            {
                return Ok(db.Departments);
            }

            return Ok(departments);
        }
        // GET: api/Departments/5
        [Route("api/Departments/GetDepartmentById")]
        [HttpGet]
        [ResponseType(typeof(Department))]
        public IHttpActionResult GetDepartmentById(int id)
        {
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return Ok(department);
            }

            return Ok(department);
        }
        [System.Web.Http.HttpGet]
        [Route("api/Departments/GetDepartmentCommittees")]

        // GET: api/CommitteesMembers
        public IHttpActionResult GetDepartmentCommittees(int deptId)
        {
            List<Committee> deptsCommittee = db.Committees.Where(x => x.DeptId == deptId).ToList();
            if (deptsCommittee == null)
            {
                return NotFound();
            }

            return Json(deptsCommittee);
        }

        // PUT: api/Departments/5
        [Route("api/Departments/PutDepartment")]
        [HttpPost]
        [ResponseType(typeof(Department))]
        public IHttpActionResult PutDepartment(int id, Department department)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != department.DeptId)
            {
                return BadRequest();
            }

            db.Entry(department).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
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

        // POST: api/Departments
        [ResponseType(typeof(Department))]
        [Route("api/Departments/PostDepartment")]
        [HttpPost]
        public IHttpActionResult PostDepartment(Department department)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Departments.Add(department);
            db.SaveChanges();

            return Ok (department);
        }

        // DELETE: api/Departments/5
        [Route("api/Departments/DeleteDepartment")]
        [HttpGet]
        [ResponseType(typeof(Department))]
      
        public int DeleteDepartment(int id)
        {
            Department department = db.Departments.Find(id);
           
            db.Departments.Remove(department);
            db.SaveChanges();

            return department.DeptId;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DepartmentExists(int id)
        {
            return db.Departments.Count(e => e.DeptId == id) > 0;
        }
    }
}