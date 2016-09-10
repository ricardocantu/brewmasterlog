using BrewMasterWebApp.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BrewMasterWebApp.Services
{
    public class BeerApiService
    {
        private readonly string NO_IMG = "/img/nobrewerypic.jpg";
        private readonly string NOT_AVAILABLE = "N/A";
        private string _key;
        private string _url;

        private ILogger _logger;
        
        public BeerApiService(ILoggerFactory loggerFactory)
        {
            _url = Startup.Configuration["BeerApiURL"];
            _key = Startup.Configuration["BeerApiKey"];
            _logger = loggerFactory.CreateLogger<BeerApiService>();
        }

        private async Task<JObject> GetJsonFromApi(string searchQuery)
        {
            var client = new HttpClient();

            var json = await client.GetStringAsync(SearchUrl(searchQuery));

            _logger.LogInformation(20,SearchUrl(searchQuery));
            return JObject.Parse(json);
        }
        
        private string SearchUrl(string searchQuery)
        {         
            return _url + searchQuery + _key;
        }
               
        private string StringFromJObjectInJToken(JToken jToken, string objKey, string valueKey, bool isImage)
        {
            var returnValue = string.Empty;
            try
            {                
                returnValue = jToken.Value<JToken>(objKey).Value<string>(valueKey);
            }
            catch
            {                
                returnValue = isImage ? NO_IMG : NOT_AVAILABLE;
            }
                        
            return Utilities.Extensions.SanitizeString(returnValue);
        }
       
        private string StringFromJToken(JToken jToken, string valueKey)
        {
            
            var returnValue = jToken.Value<string>(valueKey) ?? NOT_AVAILABLE;
            
            return Utilities.Extensions.SanitizeString(returnValue);
        }
        
        public async Task<BeerApiServiceResult> GetBeerInformation(string beerId)
        {          
            var result = new BeerApiServiceResult()
            {
                Success = false,
                Message = "Undetermined failure while looking up for beer information!"
            };
            
            var results = await GetJsonFromApi($"beer/{beerId}?withBreweries=Y&key=");
            
            if (results.Value<string>("status") != "success")
            {
                return result;
            }

            JToken beerData;

            try
            {              
                beerData = results.Value<JToken>("data");
            }
            catch
            {
                return result;
            }

            JToken breweryData;
            try
            {   
                breweryData = beerData.Value<JArray>("breweries")[0];
            }
            catch 
            {
                breweryData = null;           
            }            
              
            result.Beer = new Beer()
            {
                Name = StringFromJToken(beerData,"name"),
                IdApi = StringFromJToken(beerData,"id"),
                Label = StringFromJObjectInJToken(beerData, "labels", "large", true),
                Icon = StringFromJObjectInJToken(beerData, "labels", "icon", true),
                Abv = StringFromJToken(beerData,"abv"),
                Ibu = StringFromJToken(beerData,"ibu"),
                GlassName = StringFromJObjectInJToken(beerData,"glass", "name", false),
                IsOrganic = beerData.Value<string>("isOrganic") == "Y" ? "Yes" : "No" ?? "N/A",
                Description = StringFromJToken(beerData,"description"),
                Brewery = new Brewery
                {
                    Name = StringFromJToken(breweryData,"nameShortDisplay"),
                    Banner = StringFromJObjectInJToken(breweryData,"images","large",true),
                    Website = StringFromJToken(breweryData,"website"),
                    Established = StringFromJToken(breweryData,"established"),
                    IdApi = StringFromJToken(breweryData,"id"),
                    Description = StringFromJToken(breweryData,"description"),
                    Image = StringFromJObjectInJToken(breweryData,"images","squareMedium",true)
                },
                Availability = new Availability
                {
                    Name = StringFromJObjectInJToken(beerData, "available", "name", false),
                    Description = StringFromJObjectInJToken(beerData, "available", "description", false),
                    IdApi = StringFromJObjectInJToken(beerData, "available", "id", false)
                },
                Style = new Style
                {
                    IdApi = StringFromJObjectInJToken(beerData, "style", "id", false),
                    Name = StringFromJObjectInJToken(beerData, "style", "name", false),
                    ShortName = StringFromJObjectInJToken(beerData, "style", "shortName", false),
                    Description = StringFromJObjectInJToken(beerData, "style", "description", false),
                    IbuMin = StringFromJObjectInJToken(beerData, "style", "ibuMin", false),
                    IbuMax = StringFromJObjectInJToken(beerData, "style", "ibuMax", false),
                    AbvMin = StringFromJObjectInJToken(beerData,"style","abvMin",false),
                    AbvMax = StringFromJObjectInJToken(beerData,"style","abvMax",false),
                    SrmMin = StringFromJObjectInJToken(beerData,"style","srmMin",false),
                    SrmMax = StringFromJObjectInJToken(beerData,"style","srmMax",false),
                    OgMin = StringFromJObjectInJToken(beerData,"style","ogMin",false),
                    OgMax = StringFromJObjectInJToken(beerData,"style","ogMax",false),
                    FgMin = StringFromJObjectInJToken(beerData,"style","fgMin",false),
                    FgMax = StringFromJObjectInJToken(beerData,"style","fgMax",false)
                }                
            };
            
            result.Success = true;
            result.Message = $"Succesfully found {result.Beer.Name} information!";

            return result;
        }
        
        public async Task<BeerApiServiceResult> GetBeersFromBrewery(string breweryId)
        {
            var result = new BeerApiServiceResult()
            {
                Success = false,
                Message = "Undetermined failure while looking up for brewery beers!"
            };
            
            var results = await GetJsonFromApi($"brewery/{breweryId}?key=");
            
            if(results.Value<string>("status") != "success")
            {
                return result;
            }
            
            JToken breweryData;
            try
            {
                breweryData = results.Value<JToken>("data");
            }
            catch
            {
                //No data available so return the result obejct with unsuccesfull data
                return result;
            }
            
            //The Brewery object from the BeerApiServiceResult object is set with data from the
            //JToken breweryData
            result.Brewery = new Brewery
            {
                //By using the StringFromJToken and StringFromJObjectInJToken methods data is retrieved
                //passing the breweryData as a parameter and the keys to retrieve the data from the JToken
                //also data is return sanitize from XSS
                Name = StringFromJToken(breweryData,"name"),
                Description = StringFromJToken(breweryData, "description"),
                Website = StringFromJToken(breweryData, "website"),
                Established = StringFromJToken(breweryData, "established"),
                Banner = StringFromJObjectInJToken(breweryData,"images","large",true)
            };
            
            //Then search for beers

            //the results object data is now replace by a new JObject data
            //passing the searchQuery as a parameter and breweryId being part of the string        
            results = await GetJsonFromApi($"brewery/{breweryId}/beers?key=");

            //Check if their is data if not return the result object with default unsuccesfull data
            if (results.Value<string>("status") != "success")
            {
                return result;
            }

            //A neew list of Beers is set for the result object Beers list prop
            result.Beers = new List<Beer>();

            var data = new JEnumerable<JToken>();

            try
            {
                //retrieve the beers data from the results JObject
               data  = results["data"].Children();
            }
            catch
            {
                //if no data then return result with unsuccesfull data
                return result;
            }            
            
            var totalResults = 0;

            //Retrieve all the Beers data from the data JEnumerable<JToken> 
            foreach(var item in data)
            {
                var beer = new Beer
                {
                    //The data from the beer item is passed to
                    //a new Beer obejct
                    Name = StringFromJToken(item,"name"),
                    IdApi = StringFromJToken(item, "id"),
                    Icon = StringFromJObjectInJToken(item,"labels","icon",true),
                    Abv = StringFromJToken(item, "abv"),
                    Style = new Style
                    {
                        ShortName = StringFromJObjectInJToken(item, "style", "shortName", false)
                    }
                };

                //Add the new Beer object to the Beers list from the result object
                result.Beers.Add(beer);
                totalResults++;
            }

            //Change the message passed the totalResults variable data as part of the Message string
            result.Message = $"Found {totalResults} results";
            result.Success = true;

            //After successfully retrieved data return the result object to the controller
            return result;
        }

        //this method takes a string and an int parameter to retrieve data from an api using those two values
        public async Task<BeerApiServiceResult> GetBeers(string beerName, int pageNum)
        {
            //A BeerApiServiceResult object is created with the unsuccesfull data as default
            var result = new BeerApiServiceResult()
            {
                Success = false,
                Message = "Undetermined failure while looking up for beers!"
            };
            //A list of beers needs to be instantitiated
            result.Beers = new List<Beer>();

            //the parameters are used in the GetJsonFromApi method to form
            //the searchQuery if successfull a JObject with data is retrieved
            var results = await GetJsonFromApi($"beers/?name=*{beerName}*&p={pageNum}&withBreweries=Y&key=");

            //This two are to flag if data was found or not
            bool foundData = true;
            int maxPageNum;

            try
            {
                maxPageNum = (int)results["numberOfPages"];
            }
            catch
            {
                foundData = false;
                maxPageNum = -1;
            }
            
            var validMaxPageNum = (pageNum > 0 && pageNum <= maxPageNum);

            if (foundData && validMaxPageNum)
            {
                //a JEnumerable<JToken> collection of data is retrieve from the results and store in data
                var data = results["data"].Children();

                //the total number of results found from the passed beer name search string is
                //retrieved from the json result
                var totalResults = StringFromJToken(results,"totalResults");

                foreach (var item in data)
                {          
                   string breweryName;
                    try
                    {
                        //Brewery name data is retrieve from the jtoken if available if not
                        //an exception will be thrown that data is sanitize by the sanitize string method
                        //the method returns the sanitize string and it is stored in the breweryName variable
                        var brewery = item.Value<JArray>("breweries");
                        breweryName = brewery[0].Value<string>("nameShortDisplay");                        
                    }
                    catch
                    {
                        //If no data available 'N/A' string is stored in breweryName
                        breweryName = NOT_AVAILABLE;
                    }                 

                    //A new beer object is created with data retrieved from the json result
                    var beer = new Beer()
                    {
                        //Almost all props use a method to set the data this method retrieves
                        //the data and sanitizes it if no data available a default is set for the prop
                        Name = StringFromJToken(item, "nameDisplay"),
                        IdApi = StringFromJToken(item, "id"),
                        Abv = StringFromJToken(item, "abv"),
                        Icon = StringFromJObjectInJToken(item,"labels","icon",true),
                        Style = new Style
                        {
                            ShortName = StringFromJObjectInJToken(item,"style","shortName",false)
                        },
                        Brewery = new Brewery
                        {
                            Name = breweryName
                        }
                    };
                    //the beer object is added to the Beers list of the BeerApiServiceResult result object
                    result.Beers.Add(beer);
                }

                //Message prop is set for the result obejct with the beerName string as
                //part of the message
                result.Message = $"Found {totalResults} results for \"{beerName}\"";
                result.Success = true;
                result.PageNumber = pageNum;
                result.MaxPageNumber = maxPageNum;
            }
            else
            {
                result.PageNumber = 1;
                if (foundData)
                {
                    result.Message = "Invalid page number!";
                }
                else
                {
                    //Error message prop is set for the result obejct with the beerName string as
                    //part of the message
                    result.Message = $"Could not find \"{beerName}\" please try another name";
                }                
            }
            
            //the BeerApiServiceResult object is return with data retrieved from the json request
            //or with error messages set and success set as false
            //this object can now be used in the controller to form the view model 
            return result;
        }

        //This method takes to parameters to form a url request call to an api and retrieves data
        public async Task<BeerApiServiceResult> GetBreweries(string breweryName, int pageNum)
        {
            //create a BeerApiServiceResult object with defaults props set to unsuccessfull
            var result = new BeerApiServiceResult()
            {
                Success = false,
                Message = "Undetermined failure while looking up for breweries!"
            };

            result.Breweries = new List<Brewery>();
            
            var results = await GetJsonFromApi($"breweries/?name=*{breweryName}*&p={pageNum}&key=");
            
            bool foundData = true;
            int maxPageNum;
            try
            {
                maxPageNum = (int)results["numberOfPages"];
            }
            catch 
            {
                foundData = false;
                maxPageNum = -1;
            }

            var validMaxPageNum = (pageNum > 0 && pageNum <= maxPageNum);

            if (foundData && validMaxPageNum)
            {
                var data = results["data"].Children();
                
                var totalResults = StringFromJToken(results,"totalResults");

                foreach (var item in data)
                {   
                    var brewery = new Brewery()
                    {
                        IdApi = StringFromJToken(item,"id"),
                        Name = StringFromJToken(item, "name"),
                        Image = StringFromJObjectInJToken(item, "images", "squareMedium", true)
                    };
                    
                    result.Breweries.Add(brewery);
                }
                
                result.Message = $"Found {totalResults} results for \"{breweryName}\"";
                result.Success = true;
                result.PageNumber = pageNum;
                result.MaxPageNumber = maxPageNum;
            }
            else
            {
                result.PageNumber = 1;
                if (foundData)
                {
                    result.Message = "Invalid page number!";
                }
                else
                {   
                    result.Message = $"Could not find \"{breweryName}\" please try another name";
                }
            }
            return result;
        }

        public async Task<BeerApiServiceResult> GetLocation(string locationId)
        {
            var result = new BeerApiServiceResult()
            {
                Success = false,
                Message = "Undetermined failure while looking for locations!"
            };

            var results = await GetJsonFromApi($"location/{locationId}?key=");

            if (results.Value<string>("status") != "success")
            {
                return result;
            }

            JToken locationData;
            try
            {
                locationData = results.Value<JToken>("data");
            }
            catch
            {
                return result;
            }

            JToken breweryData;
            try
            {
                breweryData = locationData.Value<JToken>("brewery");
            }
            catch
            {
                breweryData = null;
            }

            result.Location = new Location()
            {
                StreetAddress = StringFromJToken(locationData, "streetAddress"),
                IdApi = StringFromJToken(locationData, "id"),
                City = StringFromJToken(locationData, "locality"),
                State = StringFromJToken(locationData, "region"),
                PostalCode = StringFromJToken(locationData, "postalCode"),
                PhoneNumber = StringFromJToken(locationData, "phone"),
                HoursOfOperation = StringFromJToken(locationData, "hoursOfOperation"),
                Country = StringFromJObjectInJToken(locationData, "country", "displayName", false),
                Website = StringFromJToken(locationData, "website"),
                LocationType = StringFromJToken(locationData, "locationTypeDisplay"),
                Latitude = StringFromJToken(locationData, "latitude"),
                Longitude = StringFromJToken(locationData, "longitude"),
                Name = StringFromJToken(breweryData, "name"),
                Brewery = new Brewery
                {
                    Name = StringFromJToken(breweryData, "nameShortDisplay"),
                    Banner = StringFromJObjectInJToken(breweryData, "images", "large", true),
                    Website = StringFromJToken(breweryData, "website"),
                    Established = StringFromJToken(breweryData, "established"),
                    IdApi = StringFromJToken(breweryData, "id"),
                    Description = StringFromJToken(breweryData, "description"),
                    Image = StringFromJObjectInJToken(breweryData, "images", "squareMedium", true)
                },

            };

            result.Success = true;

            result.Message = $"Succesfully found {result.Location.Brewery.Name} information!";

            return result;
        }

        public async Task<BeerApiServiceResult> GetLocations(string region, string locality, int pageNum)
        {
            var result = new BeerApiServiceResult()
            {
                Success = false,
                Message = "Undetermined failure while looking for locations!"
            };
            var searchQuery = $"region={region}";
            if (!string.IsNullOrEmpty(locality))
            {
                searchQuery += $"&locality={locality}";
            }

            result.Locations = new List<Location>();
            var results = await GetJsonFromApi($"locations?isPrimary=Y&{searchQuery}&p={pageNum}&key=");
            
            bool foundData = true;
            int maxPageNum;
            try
            {
                maxPageNum = (int)results["numberOfPages"];
            }
            catch
            {
                foundData = false;
                maxPageNum = -1;
            }

            var validMaxPageNum = (pageNum > 0 && pageNum <= maxPageNum);

            if (foundData && validMaxPageNum)
            {
                var data = results["data"].Children();

                var totalResults = StringFromJToken(results, "totalResults");

                foreach (var item in data)
                {
                    var location = new Location()
                    {
                        IdApi = StringFromJToken(item, "id"),
                        StreetAddress = StringFromJToken(item, "streetAddress"),
                        City = StringFromJToken(item, "locality"),
                        State = StringFromJToken(item, "region"),
                        Country = StringFromJObjectInJToken(item, "country", "displayName", false),
                        Brewery = new Brewery(),
                        LocationType = StringFromJToken(item, "locationTypeDisplay")

                    };
                    try
                    {
                        var breweryData = item.Value<JToken>("brewery");
                        location.Brewery.Name = StringFromJToken(breweryData, "nameShortDisplay");
                        location.Brewery.Image = StringFromJObjectInJToken(breweryData, "images", "squareMedium", true);
                    }
                    catch
                    {
                        location.Brewery.Name = NOT_AVAILABLE;
                        location.Brewery.Image = NO_IMG;
                    }

                    result.Locations.Add(location);
                }
                var city = string.IsNullOrEmpty(locality) ? "" : locality + ", ";
                result.Message = $"Found {totalResults} results for \"{city}{region}\"";
                result.Success = true;
                result.PageNumber = pageNum;
                result.MaxPageNumber = maxPageNum;
            }
            else
            {
                result.PageNumber = 1;
                if (foundData)
                {
                    result.Message = "Invalid page number!";
                }
                else
                {
                    var city = string.IsNullOrEmpty(locality) ? "" : locality + ", ";
                    result.Message = $"Could not find any results for \"{city}{region}\" please try another search";
                }
            }

            return result;
        }

    }
}
