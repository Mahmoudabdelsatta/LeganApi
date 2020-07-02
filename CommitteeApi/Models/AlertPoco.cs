using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommitteeApi.Models
{
    public class AlertPoco
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public Nullable<int> UserId { get; set; }
        public long CreatedAt { get; set; }
        public Nullable<int> Action_Id1 { get; set; }
        public Nullable<int> Action_Id2 { get; set; }
        public string Action_Type { get; set; }

    }
}