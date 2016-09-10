namespace BrewMasterWebApp.Models
{
    public class Location
    {
        public int Id { get; set; }

        public string IdApi { get; set; }

        public string Name { get; set; }

        public Brewery Brewery { get; set; }

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string PhoneNumber { get; set; }

        public string HoursOfOperation { get; set; }

        public string Website { get; set; }

        public string LocationType { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string UserName { get; set; }

    }
}