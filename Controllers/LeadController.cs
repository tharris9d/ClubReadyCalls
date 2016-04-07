/*
' Copyright (c) 2016 ClubReady.com
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Linq;
using System.Web.Mvc;
using ClubReady.Modules.ClubReadyCalls.Components;
using ClubReady.Modules.ClubReadyCalls.Models;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Security;

namespace ClubReady.Modules.ClubReadyCalls.Controllers
{
    [DnnHandleError]
    public class LeadController : DnnController
    {

        public ActionResult Delete(int leadId)
        {
            LeadManager.Instance.DeleteLead(leadId, ModuleContext.ModuleId);
            return RedirectToDefaultRoute();
        }

        public ActionResult Edit(int leadId = -1)
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            
            var lead = (leadId == -1)
                 ? new Lead { ModuleId = ModuleContext.ModuleId }
                 : LeadManager.Instance.GetLead(leadId, ModuleContext.ModuleId);
            
            return View(lead);
        }

        [HttpPost]
        //[DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
        public ActionResult Index(Lead lead)
        {

            if (lead.LeadId == -1)
            {
                lead.CreatedByUserId = User.UserID;
                lead.CreatedOnDate = DateTime.UtcNow;
                lead.LastModifiedByUserId = User.UserID;
                lead.LastModifiedOnDate = DateTime.UtcNow;

                var prospectTypeId = -1;
                var referralTypeId = -1;
                var sendEmail = false;
                var apiKey = string.Empty;
                var storeId = -1;

                if (ModuleContext.Settings.Contains("ApiKey"))
                    apiKey = ModuleContext.Settings["ApiKey"].ToString();

                if (ModuleContext.Settings.Contains("StoreId"))
                    storeId = Convert.ToInt32(ModuleContext.Settings["StoreId"]);

                if (ModuleContext.Settings.Contains("ProspectTypeId"))
                    prospectTypeId = Convert.ToInt32(ModuleContext.Settings["ProspectTypeId"]);

                if (ModuleContext.Settings.Contains("ReferralTypeId"))
                    referralTypeId = Convert.ToInt32(ModuleContext.Settings["ReferralTypeId"]);

                if (ModuleContext.Settings.Contains("SendEmail"))
                    sendEmail = Convert.ToBoolean(ModuleContext.Settings["SendEmail"]);

                LeadManager.Instance.CreateLead(lead, apiKey, storeId, prospectTypeId, referralTypeId, sendEmail);
            }
            else
            {
                var existingItem = LeadManager.Instance.GetLead(lead.LeadId, lead.ModuleId);
                existingItem.LastModifiedByUserId = User.UserID;
                existingItem.LastModifiedOnDate = DateTime.UtcNow;
                existingItem.FirstName = lead.FirstName;
                existingItem.LastName = lead.LastName;
                existingItem.Email = lead.Email;

                LeadManager.Instance.UpdateLead(existingItem);
            }

            return RedirectToDefaultRoute();
        }

        [HttpGet]
        [ModuleAction(ControlKey = "Edit", TitleKey = "Edit")]
        public ActionResult Index(int leadId = -1)
        {

            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);

            var lead = (leadId == -1)
                 ? new Lead { ModuleId = ModuleContext.ModuleId }
                 : LeadManager.Instance.GetLead(leadId, ModuleContext.ModuleId);

            return View(lead);

            //var leads = LeadManager.Instance.GetLeads(ModuleContext.ModuleId);
            //return View(leads);
        }
    }
}
