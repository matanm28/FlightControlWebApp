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
    public class ApiServersController : ControllerBase
    {
        private readonly FlightControlContext _context;

        public ApiServersController(FlightControlContext context)
        {
            _context = context;
        }

        // GET: api/ApiServers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiServer>>> GetApiServer()
        {
            return await _context.ApiServer.ToListAsync();
        }

        // GET: api/ApiServers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiServer>> GetApiServer(int id)
        {
            var apiServer = await _context.ApiServer.FindAsync(id);

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
        public async Task<ActionResult<ApiServer>> PostApiServer(ApiServer apiServer)
        {
            _context.ApiServer.Add(apiServer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetApiServer", new { id = apiServer.Id }, apiServer);
        }

        // DELETE: api/ApiServers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiServer>> DeleteApiServer(int id)
        {
            var apiServer = await _context.ApiServer.FindAsync(id);
            if (apiServer == null)
            {
                return NotFound();
            }

            _context.ApiServer.Remove(apiServer);
            await _context.SaveChangesAsync();

            return apiServer;
        }

        private bool ApiServerExists(int id)
        {
            return _context.ApiServer.Any(e => e.Id == id);
        }
    }
}
