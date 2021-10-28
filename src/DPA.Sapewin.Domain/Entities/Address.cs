using DPA.Sapewin.Repository;

namespace DPA.Sapewin.Domain.Entities
{
    public class Address : Entity
    {               
        public string Street { get; set; }
        public int Number { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string State { get; set; }
    }
}