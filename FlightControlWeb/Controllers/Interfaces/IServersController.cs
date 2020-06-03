namespace FlightControlWeb.Controllers.Interfaces {

    using DataAccessLibrary.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IServersController {

        Task<ActionResult<IEnumerable<Server>>> GetApiServer();

        Task<ActionResult<Server>> GetApiServer(string id);

        Task<ActionResult<Server>> PostApiServer(Server server);

        Task<ActionResult<Server>> DeleteApiServer(string id);
    }
}