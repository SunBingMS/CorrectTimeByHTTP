using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.InteropServices;

namespace CorrectTimeByHTTP
{
    class Program
    {
        static void Main(string[] args)
        {
            // URLエンコーディング
            string url = "http://worldtimeapi.org/api/timezone/Asia/Tokyo";
            string q = HttpUtility.UrlEncode(string.Join(" ", args));
            string format = "yyyy-MM-ddTHH:mm:ss.fff";

            // HTTPアクセス
            var req = WebRequest.Create(url + q);
            req.Headers.Add("Accept-Language:ja,en-us;q=0.7,en;q=0.3");
            var res = req.GetResponse();
            // レスポンス(JSON)をオブジェクトに変換
            ServiceResult info;
            using (res)
            {
                using (var resStream = res.GetResponseStream())
                {
                    var serializer = new DataContractJsonSerializer(typeof(ServiceResult));
                    info = (ServiceResult)serializer.ReadObject(resStream);
                }
            }
            string strTime = info.datetime;

            if (strTime.Length > 30)
            {
                strTime = strTime.Substring(0, 23);
                DateTime dt = DateTime.ParseExact(strTime, format, null);
                SystemTime sysTime = new SystemTime();
                sysTime.wYear = (ushort)dt.Year;
                sysTime.wMonth = (ushort)dt.Month;
                sysTime.wDay = (ushort)dt.Day;
                sysTime.wHour = (ushort)dt.Hour;
                sysTime.wMinute = (ushort)dt.Minute;
                sysTime.wSecond = (ushort)dt.Second;
                sysTime.wMiliseconds = (ushort)dt.Millisecond;
                //システム日時を設定する
                SetLocalTime(ref sysTime);
            }
        }


        [DllImport("kernel32.dll")]
        public static extern bool SetLocalTime(ref SystemTime sysTime);
    }

    public class ServiceResult
    {
        public string abbreviation { get; set; }
        public string client_ip { get; set; }
        public string datetime { get; set; }
        public string day_of_week { get; set; }
        public string day_of_year { get; set; }
        public string dst { get; set; }
        public string dst_from { get; set; }
        public string dst_offset { get; set; }
        public string dst_until { get; set; }
        public string raw_offset { get; set; }
        public string timezone { get; set; }
        public string unixtime { get; set; }
        public string utc_datetime { get; set; }
        public string utc_offset { get; set; }
        public string week_number { get; set; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemTime
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMiliseconds;
    }
}