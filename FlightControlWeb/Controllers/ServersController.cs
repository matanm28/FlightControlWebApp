using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServersController : ControllerBase
    {
        private readonly FlightControlContext _context;

        public ServersController(FlightControlContext context)
        {
            _context = context;
        }

        // GET: api/ApiServers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Server>>> GetApiServer()
        {
            return await _context.Servers.ToListAsync();
        }

        // GET: api/ApiServers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Server>> GetApiServer(int id)
        {
            var apiServer = await _context.Servers.FindAsync(id);

            if (apiServer == null)
            {
                return NotFound();
            }

            return apiServer;
        }

        // POST: api/ApiServers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Server>> PostApiServer(Server server)
        {
            _context.Servers.Add(server);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetApiServer", new { id = server.Id }, server);
        }

        // DELETE: api/ApiServers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Server>> DeleteApiServer(int id)
        {
            var apiServer = await _context.Servers.FindAsync(id);
            if (apiServer == null)
            {
                return NotFound();
            }

            _context.Servers.Remove(apiServer);
            await _context.SaveChangesAsync();

            return apiServer;
        }

        private bool ApiServerExists(int id)
        {
            return _context.Servers.Any(e => e.Id == id);
        }
    }
}
