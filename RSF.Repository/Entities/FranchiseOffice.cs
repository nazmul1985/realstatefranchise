using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter;

namespace RSF.Repository.Entities
{
    [Database]
    public class FranchiseOffice
    {
        public Company Company { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string Contact { get; set; }
        public long ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }

        public IEnumerable<HomeSale> Transactions
        {
            get { return Db.SQL<HomeSale>("select e from HomeSale e where e.FranchiseOffice = ?", this); }
        }

        public int Trend { get; set; }

        public long NumberOfHomeSold
        {
            get
            {
                return Db.SQL<long>($"select COUNT(e) from HomeSale e where e.FranchiseOffice = ?", this).First;
            }
        }

        public decimal AverageCommission
        {
            get
            {
                try
                {
                    //var averageCommission = Db.SQL<decimal>("select avg(e.Commission) from HomeSale e where e.FranchiseOffice = ?", this)
                    //    .First;
                    var averageCommission = this.Transactions.Average(e => e.Commission);
                    return averageCommission;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return 0;
                }
            }
        }

        public decimal TotalCommission
        {
            get
            {
                return Db.SQL<decimal>($"select sum(e.Commission) from HomeSale e where e.FranchiseOffice = ?", this)
                      .First;
            }
        }
    }
}
