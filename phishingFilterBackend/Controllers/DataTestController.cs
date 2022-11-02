using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace phishingFilterBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataTestController : ControllerBase
    {
        // GET: api/<DataTestController>
        [HttpGet]
        public ObjectResult Get()
        {
            return Ok(new { dato = "Se pudo" });
        }

        // GET api/<DataTestController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DataTestController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<DataTestController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DataTestController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
