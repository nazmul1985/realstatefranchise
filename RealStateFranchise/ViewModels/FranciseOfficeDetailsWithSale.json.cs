using System;
using System.Linq;
using RSF.Repository.Entities;
using Starcounter;

namespace RealStateFranchise.ViewModels
{
    partial class FranciseOfficeDetailsWithSale : Json
    {
        public string AddressReact
        {
            get
            {
                if (this.Data == null)
                {
                    return string.Empty;
                }
                var office = this.Data as FranchiseOffice;
                var address = $"{office.Street}, {office.City}, {office.Country}-{office.ZipCode}";
                return address;
            }
        }

        protected override void OnData()
        {
            base.OnData();
            var office = this.Data as FranchiseOffice;
            this.BackUrl = "/RealStateFranchise/corporation/" + office.Company.GetObjectID();

            this.LoadOfficeDataIntoView(office);

            this.LoadTransactionsIntoView(office);
        }

        private void LoadTransactionsIntoView(FranchiseOffice office)
        {
            foreach (var tansaction in office.Transactions)
            {
                var transaction = this.Transactions.Add();
                transaction.Address = tansaction.Address;
                transaction.Commission = tansaction.Commission;
                transaction.Price = tansaction.Price;
                transaction.SaleDate = tansaction.DateOfTransaction.ToString();
            }
        }

        private void LoadOfficeDataIntoView(FranchiseOffice office)
        {
            this.Office = new FranchiseOfficeExt();
            this.Office.City = office.City;
            this.Office.Name = office.Name;
            this.Office.Street = office.Street;
            this.Office.ZipCode = office.ZipCode;
            this.Office.Country = office.Country;
            this.Office.Contact = office.Contact;
        }

        private void Handle(Input.UpdateOfficeTrigger acton)
        {
            Db.Scope(() =>
            {
                var office = this.Data as FranchiseOffice;
                office.Name = this.Office.Name;
                office.Contact = this.Office.Contact;
                office.Country = this.Office.Country;
                office.City = this.Office.City;
                office.ZipCode = this.Office.ZipCode;
                office.Street = this.Office.Street;
            });
            this.Transaction.Commit();
        }

        private void Handle(Input.AddHomeForSale action)
        {
            this.OpenModal = true;
        }

        private void Handle(Input.CancelTrigger action)
        {
            this.OpenModal = false;
        }

        private void Handle(Input.AddHomeTrigger action)
        {
            Db.Scope(() =>
            {
                var homeSale = new HomeSale();
                var franchiseOffice = this.Data as FranchiseOffice;
                homeSale.FranchiseOffice = franchiseOffice;
                homeSale.ZipCode = this.HomeSale.ZipCode;
                homeSale.Street = this.HomeSale.Street;
                homeSale.Number = this.HomeSale.Number;
                homeSale.City = this.HomeSale.City;
                homeSale.Commission = this.HomeSale.Commission;
                homeSale.Country = this.HomeSale.Country;
                homeSale.DateOfTransaction = DateTime.Parse(this.HomeSale.SaleDate);
                homeSale.Price = this.HomeSale.Price;

                var t = this.Transactions.Add();
                t.Address = this.HomeSale.Address;
                t.Price = this.HomeSale.Price;
                t.SaleDate = this.HomeSale.SaleDate;
                t.Commission = this.HomeSale.Commission;
            });
            this.Transaction.Commit();
            Db.Scope(() =>
            {
                var trend = this.CalculateTrend();
                if (trend == 0) return;
                var franchiseOffice = this.Data as FranchiseOffice;
                franchiseOffice.Trend = trend;
            });
            this.Transaction.Commit();
            this.OpenModal = false;
        }

        private int CalculateTrend()
        {
            var office = this.Data as FranchiseOffice;
            if (office == null)
            {
                return 0;
            }
            var transactions = office.Transactions.ToList();
            var today = DateTime.Today;
            var start = new DateTime(today.Year, 1, 1);
            var end = new DateTime(today.Year, today.Month, 1);
            var totalPerviousTransactionOfThisYear =
                transactions.Where(e => e.DateOfTransaction >= start && e.DateOfTransaction < end).Sum(e => e.Price);
            if (totalPerviousTransactionOfThisYear == 0)
            {
                return 0;
            }
            var monthlyAverage = totalPerviousTransactionOfThisYear/today.Month - 1;

            var startCurrent = new DateTime(today.Year, today.Month, 1);
            var transactionThisMonth =
                transactions.Where(e => e.DateOfTransaction > startCurrent && e.DateOfTransaction <= DateTime.Now)
                    .Sum(e => e.Price);

            var trend = (int) ((transactionThisMonth - monthlyAverage)*100/monthlyAverage);
            return trend;
        }

        [FranciseOfficeDetailsWithSale_json.Office]
        partial class FranchiseOfficeExt : Json
        {
            public string AddressReact => $"{this.Name}, {this.Street}, {this.City}, {this.Country}-{this.ZipCode}";
        }

        [FranciseOfficeDetailsWithSale_json.HomeSale]
        partial class HomeForSale : Json
        {
            public string HomeAddress => $"{this.Number}, {this.Street}, {this.City}, {this.Country}-{this.ZipCode}";
        }
    }
}