using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommitteeApi.Models
{
    public class UserPoco
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string UserEmailId { get; set; }
        public string UserImage { get; set; }
        
       public SystemRole CommitteRole { get; set; }

    }
}