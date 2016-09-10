using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrewMasterWebApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BrewMasterWebApp.Data
{
    public class BrewMasterRepository : IBrewMasterRepository
    {
        private BrewMasterContext _context;
        private ILogger<BrewMasterRepository> _logger;

        public BrewMasterRepository(BrewMasterContext context, ILogger<BrewMasterRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public bool AddBeer(Beer newBeer, string userName)
        {
            var hasBeer = HasBeer(newBeer.IdApi, userName);

            if (!hasBeer)
            {
                _context.Add(newBeer);
            }
            return !hasBeer;

        }

        public bool DeleteBeer(string beerId, string userName)
        {
            var hasBeer = HasBeer(beerId, userName);

            if (hasBeer)
            {
                var beer = GetBeerById(beerId, userName);
                _context.Remove(beer.Availability);
                _context.Remove(beer.Brewery);
                _context.Remove(beer.Style);
                _context.Remove(beer);                
                return SaveAll();
            }

            return hasBeer;
        }

        public bool DeleteLocation(string locationId, string userName)
        {
            var hasLocation = HasLocation(locationId, userName);

            if (hasLocation)
            {
                var location = GetLocationById(locationId, userName);                
                _context.Remove(location.Brewery);
                _context.Remove(location);                
                return SaveAll();
            }

            return hasLocation;
        }

        public bool AddLocation(Location newLocation, string userName)
        {
            var hasLocation = HasLocation(newLocation.IdApi, userName);

            if (!hasLocation)
            {
                _context.Add(newLocation);
            }

            return !hasLocation;
        }

        public Beer GetBeerById(string beerId, string name)
        {
            try
            {
                return _context.Beers.Include(b => b.Brewery).Include(b => b.Style)
                .Include(b => b.Availability).Where(b => b.UserName == name && b.IdApi == beerId).First();
            }
            catch
            {
                return null;
            }
        }

        public Location GetLocationById(string locationId, string name)
        {
            try
            {
                return _context.Locations.Include(l => l.Brewery)
                    .Where(l => l.UserName == name && l.IdApi == locationId)
                    .First();
            }
            catch
            {
                return null;
            }
        }

        public Brewery GetBreweryById(string breweryId)
        {
            try
            {
                return _context.Breweries.Where(b => b.IdApi == breweryId).First();
            }
            catch
            {
                return null;
            }
        }
        
        public IEnumerable<Beer> GetUserBeers(string name)
        {
            try
            {
                return _context.Beers.Include(b => b.Brewery).Include(b => b.Style)
                .Include(b => b.Availability).Where(b => b.UserName == name).OrderBy(b => b.Name).ToList();

            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<Location> GetUserLocations(string name)
        {
            try
            {
                return _context.Locations.Include(l => l.Brewery).Where(l => l.UserName == name).OrderBy(l => l.Name).ToList();
            }
            catch
            {
                return null;
            }
        }
               
        public bool SaveAll()
        {
            return _context.SaveChanges() > 0;
        }

        private bool HasBeer(string id, string userName)
        {
            try
            {
                var beer = _context.Beers.Where(b => b.IdApi == id && b.UserName == userName).ToList();

                return beer.Count() > 0;
            }
            catch
            {
                return false;
            }
        }

        private bool HasLocation(string id, string userName)
        {
            try
            {
                var location = _context.Locations.Where(b => b.IdApi == id && b.UserName == userName).ToList();

                return location.Count() > 0;
            }
            catch
            {
                return false;
            }
        }


    }
}
