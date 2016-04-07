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

using System.Collections.Generic;
using DotNetNuke.Data;
using DotNetNuke.Framework;
using ClubReady.Modules.ClubReadyCalls.Models;

namespace ClubReady.Modules.ClubReadyCalls.Components
{
    interface ILeadManager
    {
        void CreateLead(Lead t, string ApiKey, int StoreId, int ProspectTypeId, int ReferralTypeId, bool SendEmail);
        void DeleteLead(int LeadId, int moduleId);
        void DeleteLead(Lead t);
        IEnumerable<Lead> GetLeads(int moduleId);
        Lead GetLead(int LeadId, int moduleId);
        void UpdateLead(Lead t);
    }

    class LeadManager : ServiceLocator<ILeadManager, LeadManager>, ILeadManager
    {
        public void CreateLead(Lead t, string apiKey, int storeId, int ProspectTypeId, int ReferralTypeId, bool SendEmail)
        {
            //we're storing the Lead in the local DB first, so that we can keep track of any potential failures and be able to recover them at a future date
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Lead>();
                rep.Insert(t);
            }

            
            //call the CreateLead web service, this will return the "UserId" from ClubReady, we want to store that in the local DB as well
            t.ClubReadyUserId = CrApi.CreateClubReadyLead(t,apiKey,storeId, ProspectTypeId,ReferralTypeId,SendEmail);
            if (t.ClubReadyUserId > 0)
            {
                UpdateLead(t);
            }
        }

        public void DeleteLead(int LeadId, int moduleId)
        {
            var t = GetLead(LeadId, moduleId);
            DeleteLead(t);
        }

        public void DeleteLead(Lead t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Lead>();
                rep.Delete(t);
            }
        }

        public IEnumerable<Lead> GetLeads(int moduleId)
        {
            IEnumerable<Lead> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Lead>();
                t = rep.Get(moduleId);
            }
            return t;
        }

        public Lead GetLead(int LeadId, int moduleId)
        {
            Lead t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Lead>();
                t = rep.GetById(LeadId, moduleId);
            }
            return t;
        }

        public void UpdateLead(Lead t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Lead>();
                rep.Update(t);
            }
        }

        protected override System.Func<ILeadManager> GetFactory()
        {
            return () => new LeadManager();
        }
    }
}
