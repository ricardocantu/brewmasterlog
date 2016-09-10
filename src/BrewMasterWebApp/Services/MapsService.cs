using Microsoft.Extensions.Logging;

namespace BrewMasterWebApp.Services
{
    public class MapsService
    {
        private string _key;
        private string _url;
        private ILogger _logger;

        public MapsService (ILoggerFactory loggerFactory)
        {
            _url = Startup.Configuration["GoogleMapsURL"];
            _key = Startup.Configuration["GoogleMapsKey"];
            _logger = loggerFactory.CreateLogger<BeerApiService>();
        }

        private string ReplaceSpaces(string value)
        {
            return value.Replace(' ','+');
        }

        public string GetMapSrc(string name, string address, string city, string state)
        {
            var src = _url + "key=" + _key + "&q=";
            
            if(!string.IsNullOrEmpty(name))
            {
                src += ReplaceSpaces(name) + ",";
            }                       

            if(!string.IsNullOrEmpty(address))
            {
                src += ReplaceSpaces(address) + ",";
            }

            if(!string.IsNullOrEmpty(city))
            {
                src += ReplaceSpaces(city) + "+";
            }

            if(!string.IsNullOrEmpty(state))
            {
                src += ReplaceSpaces(state);
            }

            return src;
        }
    }
}