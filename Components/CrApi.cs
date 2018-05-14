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

        /// <summary>
        /// When you add a Lead to ClubReady, you will receive a response 
        /// </summary>
        /// <param name="json"></param>
        public CrAddLeadResponse(string json)
        {
            JObject jObject = JObject.Parse(json);

            UserId = (int)jObject["UserId"]; // The UserId of the Lead that was added, or if found already the ID of the match
            Success = (bool)jObject["Success"]; // A response for if the user was added  
            EmailSent = (bool)jObject["EmailSent"]; // True/False if an email was sent 
            PackageAdded = (bool)jObject["PackageAdded"]; // True/False if a package was added for the Lead
        }
    }

    public class CrApi
    {
        public CrApi()
        { }

        /// <summary>
        /// This method will "create" the Lead in ClubReady, when successful it will return the UserId of the newly added Lead.
        /// </summary>
        /// <param name="l">The Lead object</param>
        /// <param name="apiKey"></param>
        /// <param name="storeId">The ID of the Store/Club in ClubReady</param>
        /// <param name="prospectTypeId">The Lead Type to add the user as </param>
        /// <param name="referralTypeId">The Referral type to add the user as </param>
        /// <param name="sendEmail">Should the lead be Emailed a welcome email</param>
        /// <returns></returns>

        public static int CreateClubReadyLead(Lead l, string apiKey, int storeId, int prospectTypeId, int referralTypeId, bool sendEmail)
        {
            //set the URL for the end point
            Uri address = new Uri("https://www.clubready.com/api/current/users/prospect");

            // Create the web request  
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

            // Set type to POST  
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            //build out the parameters for the post
            StringBuilder data = new StringBuilder();
            data.Append("apikey=" + HttpUtility.UrlEncode(apiKey)); // this key is provided by ClubReady, you can request a key by emailing support@clubready.com
            data.Append("&storeid=" + HttpUtility.UrlEncode(storeId.ToString())); // You need to tell ClubReady which "store" you are adding the Lead to
            data.Append("&firstname=" + HttpUtility.UrlEncode(l.FirstName)); // First name of the Lead
            data.Append("&lastname=" + HttpUtility.UrlEncode(l.LastName)); // Last name of the Lead
            data.Append("&email=" + HttpUtility.UrlEncode(l.Email)); // Email address of the lead
            data.Append("&cellphone=" + HttpUtility.UrlEncode(l.CellPhone)); // Cell phone number of the Lead, this is extremely useful if you want to use SMS messages in CR
            data.Append("&sendemail=" + HttpUtility.UrlEncode(sendEmail.ToString())); // Should this lead be sent a welcome email, you can configure the email template to send as well with the EmailTemplateId parameter
            data.Append("&ProspectTypeId=" + HttpUtility.UrlEncode(prospectTypeId.ToString())); // The Lead type that this lead will be associated to, this is used for Automation rules and the sales flow
            data.Append("&ReferralTypeId=" + HttpUtility.UrlEncode(referralTypeId.ToString())); // Referral type, often times you will have a "web site" referral type, but this can also be used for Campaigns in ClubReady

            // Create a byte array of the data we want to send  
            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

            // Set the content length in the request headers  
            request.ContentLength = byteData.Length;

            // Write data via the POST
            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }

            //object for the response
            CrAddLeadResponse responseObject;

            // Get response from the request
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                // Get the response stream  
                StreamReader reader = new StreamReader(response.GetResponseStream());

                responseObject = new CrAddLeadResponse(reader.ReadToEnd());
            }

            if (responseObject != null)
            {
                //return the UserID of the Lead that was created
                return responseObject.UserId;
            }

            return -1;
        }
    }
}