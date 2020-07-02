using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommitteeApi.Models
{
    public class AgendaPoco
    {
        public int id { get; set; }
        public long AgendaTime { get; set; }
        public string AgendaDesc { get; set; }
        public int MeetingId { get; set; }
        public long CreatedAt { get; set; }
    }
}