namespace BrewMasterWebApp.Models
{
    public class Beer
    {
        public int Id { get; set; }

        public string IdApi { get; set; }

        public string Name { get; set; }

        public string Abv { get; set; }
                
        public Style Style { get; set; }

        public string Label { get; set; }

        public string Icon { get; set; }

        public Brewery Brewery { get; set; }

        public string Description { get; set; }

        public string IsOrganic { get; set; }

        public string Ibu { get; set; }

        public Availability Availability { get; set; }

        public string GlassName { get; set; }
        
        public string UserName { get; set; }
    }
}
