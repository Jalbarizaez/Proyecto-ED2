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
        [HttpGet("{llave}", Name = "Get")]
        public ActionResult Get(string llave)
        {
            if (ModelState.IsValid)
            {
                var info = _Service.Get_(llave);
                if (info != null)
                {
                    return Ok(info);
                }
                else { return NotFound(); }
            }
            else { return BadRequest(); }
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
        [HttpPut("{llave}")]
        public ActionResult Put(string llave, [FromBody] Conversaciones value)
        {
            var mesage = _Service.Get(llave);

            if (ModelState.IsValid)
            {
                if (mesage != null)
                {
                    if (_Service.exist(value.llave) == true)
                    {
                        _Service.Update(llave, value);
                        return Ok(_Service.Get(llave));
                    }
                    else { return Conflict(); }

                }
                else
                { return NotFound(); }
            }
            else { return BadRequest(); }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{llave}")]
        public ActionResult Delete(string llave)
        {
            var mesage = _Service.Get(llave);
            if (mesage != null)
            {
                _Service.Remove(llave);
                return Ok();
            }
            else
            { return NotFound(); }

        }
    }
}
