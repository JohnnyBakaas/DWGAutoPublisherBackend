using DWGAutoPublisherBackend.Model;
using DWGAutoPublisherBackend.Model.DrawingHandler;
using DWGAutoPublisherBackend.Model.DrawingsFromFrontEnd;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DWGAutoPublisherBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DWGStatusUpdateController : ControllerBase
    {

        // PUT api/<StatusUpdateController>/5
        [HttpPut]
        public void Put([FromBody] DWGUpdateFromFrontEnd dwg)
        {
            DWGFile foundFile = DB.DWGs.FirstOrDefault(e => e.FilePath == dwg.FilePath);
            if (foundFile == null) return;

            foundFile.Status = dwg.Status;
        }
    }
}
