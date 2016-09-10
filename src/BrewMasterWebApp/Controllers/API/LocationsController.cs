using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BrewMasterWebApp.Data;
using BrewMasterWebApp.Models;

namespace BrewMasterWebApp.Controllers.API
{
    [Authorize]
    [Route("api/locations")]
    public class LocationsController : Controller
    {

        private ILogger _logger;
        private IBrewMasterRepository _repository;

        public LocationsController(ILogger<LocationsController> logger,
            IBrewMasterRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        //GET: api/locations
        [HttpGet("")]
        public JsonResult Get()
        {
            var locations = _repository.GetUserLocations(User.Identity.Name);

            var results = Mapper.Map<IEnumerable<Location>>(locations);

            return Json(results);
        }

    }
}
