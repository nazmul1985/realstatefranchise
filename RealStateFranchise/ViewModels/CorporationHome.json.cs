using System.Collections.Generic;
using System.Linq;
using RSF.Repository.Entities;
using Starcounter;

namespace RealStateFranchise.ViewModels
{
    partial class CorporationHome : Json, IBound<Company>
    {
        protected override void OnData()
        {
            base.OnData();
            var data = this.Data;
            this.CompanyName = data.CompanyName;
            var offices = this.Data.FranchiseOffices;
            this.ShowOfficcesIntoView(offices);
        }

        private void ShowOfficcesIntoView(IEnumerable<FranchiseOffice> offices)
        {
            foreach (var franchiseOffice in offices)
            {
                var offObj = franchiseOffice;
                var office = this.Offices.Add();
                var objectId = franchiseOffice.GetObjectID();
                office.Link = "/RealStateFranchise/Franchise/" + objectId;
                office.Key = objectId;
                office.Name = offObj.Name;
                office.HomesSold = offObj.NumberOfHomeSold;
                office.TotalCommision = offObj.TotalCommission;
                office.AverageCommision = offObj.AverageCommission;
                office.Trend = offObj.Trend;
            }
        }

        private void Handle(Input.SortHomeSaleTrigger action)
        {
            var orderedByNumberOfHomeSold = this.Offices.OrderByDescending(e => e.HomesSold).ToList();
            this.Offices.Clear();
            foreach (var office in orderedByNumberOfHomeSold)
            {
                this.AddOfficeIntoView(office);
            }
        }

        private void Handle(Input.SortTotalCommission action)
        {
            var orderedByTotalCommission = this.Offices.OrderByDescending(e => e.TotalCommision).ToList();
            this.Offices.Clear();
            foreach (var o in orderedByTotalCommission)
            {
                this.AddOfficeIntoView(o);
            }
        }

        private void Handle(Input.SortTrend action)
        {
            var orderedByTrend = this.Offices.OrderByDescending(e => e.Trend).ToList();
            this.Offices.Clear();
            foreach (var o in orderedByTrend)
            {
                this.AddOfficeIntoView(o);
            }
        }

        private void AddOfficeIntoView(OfficesElementJson o)
        {
            var office = this.Offices.Add();
            office.HomesSold = o.HomesSold;
            office.AverageCommision = o.AverageCommision;
            office.Key = o.Key;
            office.Link = o.Link;
            office.Name = o.Name;
            office.TotalCommision = o.TotalCommision;
            office.Trend = o.Trend;
        }

        private void Handle(Input.SortAverageCommission action)
        {
            var orderedByAverageCommission = this.Offices.OrderByDescending(e => e.AverageCommision).ToList();
            this.Offices.Clear();
            foreach (var o in orderedByAverageCommission)
            {
                this.AddOfficeIntoView(o);
            }
        }

        private void Handle(Input.CreateOffice action)
        {
            action.Cancel();
            this.OpenModal = true;
        }

        private void Handle(Input.CancelTrigger action)
        {
            this.OpenModal = false;
        }

        private void Handle(Input.CreateTrigger action)
        {
            Db.Scope(() =>
            {
                var newOfficeData = new FranchiseOffice
                {
                    Company = this.Data,
                    Name = this.NewOffice.Name,
                    Address = this.NewOffice.Address,
                    City = this.NewOffice.City,
                    Contact = this.NewOffice.Contact,
                    Country = this.NewOffice.Country,
                    Street = this.NewOffice.Street,
                    ZipCode = (int) this.NewOffice.ZipCode
                };
                var newOffice = this.Offices.Add();
                newOffice.Name = this.NewOffice.Name;
                newOffice.Link = "/RealStateFranchise/Franchise/" + newOfficeData.GetObjectID();
                newOffice.Key = newOfficeData.GetObjectID();
            });

            this.Transaction.Commit();
            this.OpenModal = false;
        }

        [CorporationHome_json.NewOffice]
        partial class NewOfficeExt : Json
        {
            public string OfficeAddress => $"{this.Street}, {this.City}, {this.Country}-{this.ZipCode}";
        }
    }
}