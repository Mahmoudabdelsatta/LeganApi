using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommitteeApi.Models
{
    public class UploadMeetingImage
    {
        public int MeetingId { get; set; }
        public List<string> Images { get; set; }



    }
}