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
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        public UsersController(UserService userService)
        {
            _userService = userService;
        }
        
		// GET: api/Users
        [HttpGet]
        public ActionResult Get()
        {
            List<lista> names = new List<lista>();
            var users = _userService.Get();
            foreach(var item in users)
            {
                names.Add(new lista { user = item.usuario});
            }
            return Ok(names);
        }

        // GET: api/Users/srgio
        [HttpGet("{user}")]
        public ActionResult Get(string user)
        {
            if (ModelState.IsValid)
            {
                var info = _userService.Get_(user);
                if (user != null)
                {
                    return Ok(info);
                }
                else { return NotFound(); }
            }
            else { return BadRequest(); }
            
        }

        // POST: api/Users
        [HttpPost]
        public ActionResult Post([FromBody] User value)
        {
            if (ModelState.IsValid)
            {
                if (_userService.exist(value.usuario) == true)
                {
                    _userService.Create(value);

                    return Ok(_userService.Get_(value.usuario));
                }
                else { return Conflict(); }
            }
            else { return BadRequest(); }
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] User value)
        {
            var user = _userService.Get(id);

            if (ModelState.IsValid)
            {
                if (user != null)
                {    if (_userService.exist(value.usuario) == true)
                    {
                        _userService.Update(id, value);
                        return Ok(_userService.Get(id));
                    }
                    else { return Conflict(); }
                   
                }
                else
                { return NotFound(); }
            }
            else { return BadRequest(); }
        }

		// DELETE
		[HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var user = _userService.Get(id);
            if (user != null)
            {
                _userService.Remove(id);
                return Ok();
            }
            else
            { return NotFound(); }
        }
    }
}

