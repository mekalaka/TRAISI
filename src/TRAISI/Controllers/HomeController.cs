using System.Collections.Generic;
using System.Diagnostics;
using TRAISI.Data.Core.Interfaces;
using TRAISI.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TRAISI.Controllers
{
    public class HomeController : Controller
    {



        // GET api/values
        [Route("api/[controller]")]
        [Authorize]
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[] {"value1", "value2"};
        }
    }
}