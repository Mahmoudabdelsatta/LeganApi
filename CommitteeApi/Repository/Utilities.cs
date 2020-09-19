using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace CommitteeApi.Repository
{
    public static class Utilities
    {


        public static string BASE_URL
        {
            get { return Utilities.GetAppSettings("BASE_URL"); }
        }
        public static string LogError_Path
        {
            get { return Utilities.GetAppSettings("LogError_Path"); }
        }
        public static string GetAppSettings(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (Exception ex)
            {
                return string.Empty;

            }
        }
        //"/*http://162.253.126.178/plesk-site-preview/leganapi.itmisr.net*/";
        public static string ConvertDateCalendar(DateTime DateConv, string Calendar, string DateLangCulture)
        {
            System.Globalization.DateTimeFormatInfo DTFormat;
            DateLangCulture = DateLangCulture.ToLower();
            /// We can't have the hijri date writen in English. We will get a runtime error - LAITH - 11/13/2005 1:01:45 PM -

            if (Calendar == "Hijri" && DateLangCulture.StartsWith("en-"))
            {
                DateLangCulture = "ar-sa";
            }

            /// Set the date time format to the given culture - LAITH - 11/13/2005 1:04:22 PM -
            DTFormat = new System.Globalization.CultureInfo(DateLangCulture, false).DateTimeFormat;

            /// Set the calendar property of the date time format to the given calendar - LAITH - 11/13/2005 1:04:52 PM -
            switch (Calendar)
            {
                case "Hijri":
                    DTFormat.Calendar = new System.Globalization.HijriCalendar();
                    break;

                case "Gregorian":
                    DTFormat.Calendar = new System.Globalization.GregorianCalendar();
                    break;

                default:
                    return "";
            }

            /// We format the date structure to whatever we want - LAITH - 11/13/2005 1:05:39 PM -
            DTFormat.ShortDatePattern = "d";
            return (DateConv.Date.ToString("f", DTFormat));
        }
        public static void LogError(Exception ex)
        {
            string filePath = @Utilities.LogError_Path + "Error.txt";


            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();

                while (ex != null)
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Message : " + ex.Message);
                    writer.WriteLine("StackTrace : " + ex.StackTrace);

                    ex = ex.InnerException;
                }
            }

        }
    }
}