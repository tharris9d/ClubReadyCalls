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

using DotNetNuke.Web.Mvc.Framework.Controllers;
using DotNetNuke.Collections;
using System.Web.Mvc;
using DotNetNuke.Security;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;

namespace ClubReady.Modules.ClubReadyCalls.Controllers
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [DnnHandleError]
    public class SettingsController : DnnController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Settings()
        {
            var settings = new Models.Settings();
            settings.ProspectTypeId = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("ProspectTypeId", -1);
            settings.ReferralTypeId = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("ReferralTypeId", -1);
            settings.SendEmail = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("SendEmail", false);
            settings.StoreId = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("StoreId", -1);
            settings.ApiKey = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("ApiKey", string.Empty);

            return View(settings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supportsTokens"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Settings(Models.Settings settings)
        {
            ModuleContext.Configuration.ModuleSettings["ProspectTypeId"] = settings.ProspectTypeId.ToString();
            ModuleContext.Configuration.ModuleSettings["ReferralTypeId"] = settings.ReferralTypeId.ToString();
            ModuleContext.Configuration.ModuleSettings["SendEmail"] = settings.SendEmail.ToString();
            ModuleContext.Configuration.ModuleSettings["StoreId"] = settings.StoreId.ToString();
            ModuleContext.Configuration.ModuleSettings["ApiKey"] = settings.ApiKey;
            return RedirectToDefaultRoute();
        }
    }
}