using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using ClubReady.Modules.ClubReadyCalls.Models;
using DotNetNuke.UI.Modules.Html5;
using Newtonsoft.Json.Linq;

namespace ClubReady.Modules.ClubReadyCalls.Components
{
    //"{\"UserId\":9689773,\"Success\":true,\"EmailSent\":false,\"PackageAdded\":false}"
    public class CrAddLeadResponse
    {
        public int UserId { get; set; }
        public bool Success { get; set; }
        public bool EmailSent { get; set; }
        public bool PackageAdded { get; set; }

        public CrAddLeadResponse(string json)
        {
            JObject jObject = JObject.Parse(json);
            
            UserId = (int)jObject["UserId"];
            Success= (bool)jObject["Success"];
            EmailSent = (bool)jObject["EmailSent"];
            PackageAdded = (bool)jObject["PackageAdded"];
        }


    }

    public class CrApi
    {
        public CrApi()
        { }

        public static int CreateClubReadyLead(Lead l, string apiKey, int storeId, int prospectTypeId, int referralTypeId, bool sendEmail)
        {
            Uri address = new Uri("http://www.clubready.com/api/current/users/prospect");

            // Create the web request  
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

            // Set type to POST  
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            StringBuilder data = new StringBuilder();
            data.Append("apikey=" + HttpUtility.UrlEncode(apiKey));
            data.Append("&storeid=" + HttpUtility.UrlEncode(storeId.ToString()));
            data.Append("&firstname=" + HttpUtility.UrlEncode(l.FirstName));
            data.Append("&lastname=" + HttpUtility.UrlEncode(l.LastName));
            data.Append("&email=" + HttpUtility.UrlEncode(l.Email));
            data.Append("&cellphone=" + HttpUtility.UrlEncode(l.CellPhone));
            data.Append("&sendemail=" + HttpUtility.UrlEncode(sendEmail.ToString()));
            data.Append("&ProspectTypeId=" + HttpUtility.UrlEncode(prospectTypeId.ToString()));
            data.Append("&ReferralTypeId=" + HttpUtility.UrlEncode(referralTypeId.ToString()));

            // Create a byte array of the data we want to send  
            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

            // Set the content length in the request headers  
            request.ContentLength = byteData.Length;

            // Write data  
            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }

            CrAddLeadResponse responseObject;

            // Get response  
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                // Get the response stream  
                StreamReader reader = new StreamReader(response.GetResponseStream());

                // Console application output  
                //Console.WriteLine(reader.ReadToEnd());
                responseObject = new CrAddLeadResponse(reader.ReadToEnd());
            }

            if (responseObject != null)
            {
                return responseObject.UserId;
            }

            return -1;

        }

    }
}