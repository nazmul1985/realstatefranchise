using System;
using Starcounter;

namespace RSF.Repository.Entities
{
    [Database]
    public class HomeSale
    {
        public FranchiseOffice FranchiseOffice { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public long ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public string Address => $"{this.Number}, {this.Street}, {this.City}, {this.Country}-{this.ZipCode}";

        public DateTime DateOfTransaction { get; set; }
        public decimal Price { get; set; }
        public decimal Commission { get; set; }
    }
}