using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_PROYECTO.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_PROYECTO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly MessageService _Service;

        public MessagesController(MessageService Service)
        {
            _Service = Service;
        }

        // GET: api/Messages
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(_Service.Get());
        }

        // GET: api/Messages/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Messages
        [HttpPost]
        public ActionResult Post([FromBody] Conversaciones value)
        {
            if (ModelState.IsValid)
            {
                _Service.Create(value);

                return Ok();
            }
            else { return BadRequest(); }
        }

        // PUT: api/Messages/5
        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] Conversaciones value)
        {
            var mesage = _Service.Get(id);

            if (ModelState.IsValid)
            {
                if (mesage != null)
                {
                    if (_Service.exist(value.llave) == true)
                    {
                        _Service.Update(id, value);
                        return Ok(_Service.Get(id));
                    }
                    else { return Conflict(); }

                }
                else
                { return NotFound(); }
            }
            else { return BadRequest(); }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var mesage = _Service.Get(id);
            if (mesage != null)
            {
                _Service.Remove(id);
                return Ok();
            }
            else
            { return NotFound(); }

        }
    }
}
