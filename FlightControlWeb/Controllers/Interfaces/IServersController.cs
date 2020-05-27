namespace FlightControlWeb.Controllers {
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccessLibrary.Models;
    using Microsoft.AspNetCore.Mvc;

    public interface IServersController {
        Task<ActionResult<IEnumerable<Server>>> GetApiServer();

        Task<ActionResult<Server>> GetApiServer(int id);

        Task<ActionResult<Server>> PostApiServer(Server server);

        Task<ActionResult<Server>> DeleteApiServer(int id);
    }
}
