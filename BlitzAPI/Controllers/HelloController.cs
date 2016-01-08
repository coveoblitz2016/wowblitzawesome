using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace BlitzAPI.Controllers
{
    [Route("api/[controller]")]
    public class HelloController : Controller
    {
        // GET: api/hellow
        [HttpGet]
        public string Hello()
        {
            return "Hello World!";
        }
    }
}
