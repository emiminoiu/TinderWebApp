using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
       private readonly DataContext _context;
       public ValuesController(DataContext _context)
       {
           this._context = _context;
       }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetValues()
        {
            var values = _context.Values.ToList();
            return Ok(values);
        }

         [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetValue(int id)
        {
            var val = _context.Values.FirstOrDefault(v => v.Id == id);
            return Ok(val);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
