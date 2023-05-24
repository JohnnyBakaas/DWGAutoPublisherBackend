using DWGAutoPublisherBackend.Model;
using DWGAutoPublisherBackend.Model.DrawingHandler;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DWGAutoPublisherBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DWGsToFrontEndController : ControllerBase
    {
        // GET: api/<DWGsToFrontEndController>
        [HttpGet]
        public List<Project> Get()
        {
            DB.PrintEverything();
            return DB.Projects;
        }

    }
}
