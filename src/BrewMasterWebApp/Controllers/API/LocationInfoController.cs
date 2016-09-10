using AutoMapper;
using BrewMasterWebApp.Data;
using BrewMasterWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BrewMasterWebApp.Controllers.API
{
    [Authorize]
    [Route("api/location/{locationId}")]
    public class LocationInfoController : Controller
    {
        private ILogger _logger;
        private IBrewMasterRepository _repository;

        public LocationInfoController(IBrewMasterRepository repository,
        ILogger<LocationInfoController> logger)
        {
            _logger = logger;
            _repository = repository;
        }

        //GET: api/location/{locationId}
        //This method gets beer data from the db
        [HttpGet("")]
        public JsonResult Get(string locationId)
        {
            try
            {
                var id = Utilities.Extensions.SanitizeString(locationId);
                var location = _repository.GetLocationById(id, User.Identity.Name);

                var result = Mapper.Map<Location>(location);

                if (result == null)
                {
                    return Json(null);
                }

                return Json(result);
            }
            catch
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Error ocurred finiding the beer information");
            }
        }
    }
}
