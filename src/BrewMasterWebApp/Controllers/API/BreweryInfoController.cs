using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BrewMasterWebApp.Data;
using BrewMasterWebApp.Models;

namespace BrewMasterWebApp.Controllers.API
{
    [Authorize]
    [Route("api/brewery/{breweryId}")]
    public class  BreweryInfoController : Controller
    {
        private ILogger _logger;
        private IBrewMasterRepository _repository;

        public BreweryInfoController(IBrewMasterRepository repository, 
        ILogger<BreweryInfoController> logger)
        {
            _logger = logger;
            _repository = repository;
        }

        //GET: api/brewery/{breweryId}
        //This method gets beer data from the db
        [HttpGet("")]
        public JsonResult Get(string breweryId)
        {
            try
            {
                var id = BrewMasterWebApp.Utilities.Extensions.SanitizeString(breweryId);
                var brewery = _repository.GetBreweryById(id);  
                
                var result = Mapper.Map<Brewery>(brewery);

                if(result == null)
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