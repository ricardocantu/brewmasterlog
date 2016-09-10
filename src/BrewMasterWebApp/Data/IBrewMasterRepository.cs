using BrewMasterWebApp.Models;
using System.Collections.Generic;

namespace BrewMasterWebApp.Data
{
    public interface IBrewMasterRepository
    {
        bool AddBeer(Beer newBeer, string userName);

        bool SaveAll();

        Beer GetBeerById(string beerId, string name);

        IEnumerable<Beer> GetUserBeers(string name);

        Brewery GetBreweryById(string brewryId);

        Location GetLocationById(string locationId, string name);

        bool DeleteBeer(string beerId, string userName);

        bool DeleteLocation(string locationId, string userName);

        bool AddLocation(Location newLocation, string userName);

        IEnumerable<Location> GetUserLocations(string name);
    }
}
