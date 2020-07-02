using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommitteeApi.Models
{
    public class AllUserDataPoco
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public Nullable<Int64> Phone { get; set; }
        public string Address { get; set; }

        public string UserEmailId { get; set; }
        public string UserImage { get; set; }
        public int? SystemRoleId { get; set; }
        public SystemRole SystemRole { get; set; }
        public string WorkSide { get; set; }
        public Department Department { get; set; }



    }
}