using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommitteeApi.Repository
{
    public class UnixStamp
    {
        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (long)(TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }
    }
}