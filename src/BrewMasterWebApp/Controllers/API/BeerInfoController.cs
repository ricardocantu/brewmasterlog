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
    [Route("api/beer/{beerId}")]
    public class  BeerInfoController : Controller
    {
        private ILogger _logger;
        private IBrewMasterRepository _repository;

        public BeerInfoController(IBrewMasterRepository repository, 
        ILogger<BeerInfoController> logger)
        {
            _logger = logger;
            _repository = repository;
        }

        //GET: api/beer/{beerId}
        //This method gets beer data from the db
        [HttpGet("")]
        public JsonResult Get(string beerId)
        {
            try
            {
                var id = Utilities.Extensions.SanitizeString(beerId);
                var beer = _repository.GetBeerById(id,User.Identity.Name);  
                
                var result = Mapper.Map<Beer>(beer);

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