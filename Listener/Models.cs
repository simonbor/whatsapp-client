
namespace Listener.Models
{
    public class Address
    {
        public string StreetLocalName { get; set; }
        public int CityId { get; set; }
        public int CountryId { get; set; }
        public int Building { get; set; }
    }

    public class Driver
    {
        public string MobileNum { get; set; }
    }

    public class Chance
    {
        public string DateStart { get; set; }
    }

    public class ChanceReq
    {
        public Address Address { get; set; }
        public Driver Driver { get; set; }
        public Chance Chance { get; set; }
    }
}

