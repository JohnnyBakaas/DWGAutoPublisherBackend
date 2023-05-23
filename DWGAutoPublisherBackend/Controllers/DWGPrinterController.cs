using DWGAutoPublisherBackend.Model;
using DWGAutoPublisherBackend.Model.DrawingHandler;
using DWGAutoPublisherBackend.Model.DrawingsFromFrontEnd;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DWGAutoPublisherBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DWGPrinterController : ControllerBase
    {

        // PUT api/<DWGPrinterController>/5
        [HttpPut("{name}")]
        public int Put(string name, [FromBody] DWGFromFrontEnd dwgFromFrontEnd)
        {
            DWGFile foundFile = DB.MatchDWGFromFrontEndToDBDWGs(dwgFromFrontEnd);
            if (foundFile == null) return 0;
            foundFile.UpdateStatus(dwgFromFrontEnd);
            DWGPrintingQue.AddDWGFileToPublishList(foundFile);
            return 1;
        }
    }
}
