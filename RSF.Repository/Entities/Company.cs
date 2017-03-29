using System.Collections.Generic;
using Starcounter;

namespace RSF.Repository.Entities
{
    [Database]
    public class Company
    {
        public string CompanyName { get; set; }

        public IEnumerable<FranchiseOffice> FranchiseOffices
        {
            get
            {
                var offices = Db.SQL<FranchiseOffice>($"select f from FranchiseOffice f where f.Company = ?", this);
                return offices;
            }
        }
    }
}
