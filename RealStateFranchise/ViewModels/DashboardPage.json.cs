using System.Collections.Generic;
using RSF.Repository.Entities;
using Starcounter;

namespace RealStateFranchise.ViewModels
{
    partial class DashboardPage : Json
    {
        protected override void OnData()
        {
            base.OnData();
            var corporations = this.Data as IEnumerable<Company>;
            this.Corporations.Clear();
            foreach (var corporation in corporations)
            {
                var corp = this.Corporations.Add();
                corp.Name = corporation.CompanyName;
                corp.Link = "/RealStateFranchise/corporation/" + corporation.GetObjectID();
            }
        }

        private void Handle(Input.CreateCorporationTrigger action)
        {
            Db.Scope(() =>
            {
                var company = new Company
                {
                    CompanyName = this.CorporationName
                };
                var companyJson = this.Corporations.Add();
                companyJson.Name = this.CorporationName;
                companyJson.Link = "/RealStateFranchise/corporation/" + company.GetObjectID();
            });
            this.Transaction.Commit();
            this.CorporationName = "";
        }
    }
}
