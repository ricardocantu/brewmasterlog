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
    [Route("api/beers")]
    public class  BeersController : Controller
    {
        private ILogger _logger;
        private IBrewMasterRepository _repository;

        public BeersController(IBrewMasterRepository repository, 
        ILogger<BeersController> logger)
        {
            _logger = logger;
            _repository = repository;
        }

        
        //GET: api/beers
        //This method gets beer data from the db
        [HttpGet("")]
        public JsonResult Get()
        {
            var beers = _repository.GetUserBeers(User.Identity.Name);  

            var results = Mapper.Map<IEnumerable<Beer>>(beers);

            return Json(results);
        }

    }
}