namespace FlightControlWeb.Controllers.Implementations
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccessLibrary.DataAccess.Interfaces;
    using DataAccessLibrary.Models;
    using FlightControlWeb.Controllers.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class ServersController : ControllerBase, IServersController {
        private readonly IServerService serverService;

        /// <inheritdoc />
        public ServersController(IServerService serverService) {
            this.serverService = serverService;
        }
        
        // GET: api/ApiServers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Server>>> GetApiServer()
        {
            return await this.serverService.GetAllAsync();
        }

        // GET: api/ApiServers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Server>> GetApiServer(int id)
        {
            var apiServer = await this.serverService.FindAsync(id);
            if (apiServer == null)
            {
                return this.NotFound();
            }

            return this.Ok(apiServer);
        }

        // POST: api/ApiServers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Server>> PostApiServer(Server server) {
            var tempServer = await this.serverService.FindAsync(server);
            if (tempServer!=null) {
                return Conflict(
                        $"A server with same exact data already exists on this server.\nTry using 'api/Servers/{{id}}' with '{tempServer.Id}' as 'id'");
            }
            await this.serverService.AddAsync(server);
            await this.serverService.SaveChangesAsync();

            return this.CreatedAtAction("GetApiServer", new { id = server.Id }, server);
        }

        // DELETE: api/ApiServers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Server>> DeleteApiServer(int id) {
            
            var apiServer = await this.serverService.RemoveAsync(id);
            if (apiServer == null)
            {
                return this.NotFound();
            }

            await this.serverService.SaveChangesAsync();
            return apiServer;
        }

    }
}
