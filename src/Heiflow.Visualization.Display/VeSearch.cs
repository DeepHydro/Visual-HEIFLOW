using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Windows.Forms;

namespace HUST.WREIS.Dot3D
{
    /// <summary>
    /// from Jason Fuller's VE Mobile, via Reflector,only slightly modified to use my basic PushPin structure
    /// </summary>
    public class Search
    {
        private Search()
        {
        }

        private static string DoSearchRequest(string searchParams)
        {
            string text1 = string.Empty;
            HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create("http://local.live.com/search.ashx");
            request1.Method = "POST";
            request1.ContentType = "application/x-www-form-urlencoded";
            UTF8Encoding encoding1 = new UTF8Encoding();
            byte[] buffer1 = encoding1.GetBytes(searchParams);
            request1.ContentLength = buffer1.Length;
            try
            {
                Stream stream1 = request1.GetRequestStream();
                stream1.Write(buffer1, 0, buffer1.Length);
                stream1.Close();
                stream1 = null;
                text1 = Search.GetSearchResults(request1);
            }
            catch (WebException)
            {
            }
            return text1;
        }

        internal static string GetSearchResults(HttpWebRequest searchRequest)
        {
            string text1 = string.Empty;
            HttpWebResponse response1 = (HttpWebResponse)searchRequest.GetResponse();
            Cursor.Current = Cursors.WaitCursor;
            Stream stream1 = response1.GetResponseStream();
            Cursor.Current = Cursors.Default;
            Encoding encoding1 = Encoding.GetEncoding("utf-8");
            StreamReader reader1 = new StreamReader(stream1, encoding1);
            char[] chArray1 = new char[0x100];
            for (int num1 = reader1.Read(chArray1, 0, 0x100); num1 > 0; num1 = reader1.Read(chArray1, 0, 0x100))
            {
                string text2 = new string(chArray1, 0, num1);
                text1 = text1 + text2;
            }
            reader1.Close();
            reader1 = null;
            response1.Close();
            response1 = null;
            return text1;
        }

        public static bool SearchForAddress(string address, out double lat1, out double long1, out double lat2, out double long2)
        {
            double num1;
            long2 = num1 = 0;
            long1 = num1;
            lat2 = num1;
            lat1 = num1;
            string text1 = "a=&b=" + address + "&c=0.0&d=0.0&e=0.0&f=0.0&g=&i=&r=0";
            string text2 = Search.DoSearchRequest(text1);
            if ((text2 == null) || (text2 == string.Empty))
            {
                return false;
            }
            Regex regex1 = new Regex(@"SetViewport\((?<lat1>\S+),(?<long1>\S+),(?<lat2>\S+),(?<long2>\S+)\)");
            Match match1 = regex1.Match(text2);
            if (!match1.Success)
            {
                return false;
            }
            lat1 = double.Parse(match1.Groups["lat1"].Value);
            long1 = double.Parse(match1.Groups["long1"].Value);
            lat2 = double.Parse(match1.Groups["lat2"].Value);
            long2 = double.Parse(match1.Groups["long2"].Value);
            return true;
        }

        public static ArrayList SearchForBusiness(string business, double lat1, double lon1, double lat2, double lon2)
        {
            int num1 = 0;
            ArrayList list1 = new ArrayList();
            while (true)
            {
                bool flag1 = false;
                string text2 = string.Concat(new object[] { "a=", business.Trim(), "&b=&c=", lat1, "&d=", lon1, "&e=", lat2, "&f=", lon2, "&g=", num1, "&i=0&r=false" });
                string text1 = Search.DoSearchRequest(text2);
                if ((text1 != null) && (text1 != string.Empty))
                {
                    Regex regex1 = new Regex(@"VE_SearchResult\((?<id>[0-9]*),'(?<name>[^']*)','(?<address>[^']*)','(?<phone>[^']*)',(?<rating>[^,]*),'(?<type>[^']*)',(?<latitude>[^,]*),(?<longitude>[^,)]*)\)");
                    MatchCollection collection1 = regex1.Matches(text1);
                    foreach (Match match1 in collection1)
                    {
                        PushPin pushPin = new PushPin();
                        pushPin.Name = match1.Groups["name"].Value;
                        pushPin.Address = match1.Groups["address"].Value;
                        pushPin.Phone = match1.Groups["phone"].Value;
                        pushPin.Latitude = double.Parse(match1.Groups["latitude"].Value);
                        pushPin.Longitude = double.Parse(match1.Groups["longitude"].Value);
                        list1.Add(pushPin);
                    }
                    num1 += 10;
                    regex1 = new Regex(@"true,''\);$");
                    if (regex1.IsMatch(text1))
                    {
                        flag1 = true;
                    }
                }
                if (!flag1 || (list1.Count >= 50))
                {
                    return list1;
                }
            }
        }

    }

    //TODO ultimately the PushPin class should be replaced with the use of the WW Icon class
    public class PushPin
    {
        public string Name;
        public string Address;
        public string Phone;
        public double Latitude;
        public double Longitude;
    }
}
