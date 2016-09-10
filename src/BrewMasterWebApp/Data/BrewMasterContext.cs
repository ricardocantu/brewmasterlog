using BrewMasterWebApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BrewMasterWebApp.Data
{
    public class BrewMasterContext : IdentityDbContext<BrewMasterUser>
    {

        public DbSet<Beer> Beers { get; set; }
        public DbSet<Brewery> Breweries { get; set; }
        public DbSet<Style> Styles { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Location> Locations { get; set; }

        public BrewMasterContext(DbContextOptions<BrewMasterContext> options)
        : base(options) 
        {
            try
            {
                Database.Migrate();
            }
            catch
            {
                Database.EnsureCreated();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
