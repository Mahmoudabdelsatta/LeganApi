//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CommitteeApi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MeetingHistory
    {
        public int Id { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public int MeetingId { get; set; }
        public string MeetingDate { get; set; }
        public string CreatedAt { get; set; }
    }
}
