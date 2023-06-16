using DWGAutoPublisherBackend.Helpers;
using DWGAutoPublisherBackend.Model;
using DWGAutoPublisherBackend.Model.DrawingHandler;
using DWGAutoPublisherBackend.Model.DrawingsFromFrontEnd;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DWGAutoPublisherBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DWGStatusUpdateController : ControllerBase
    {
        private readonly IValidator _validator;
        public DWGStatusUpdateController(IValidator validator)
        {
            _validator = validator;
        }

        // PUT api/<StatusUpdateController>/5
        [HttpPut]
        public void Put([FromBody] DWGUpdateFromFrontEnd dwg)
        {
            if (!_validator.IsValidStatus(dwg.Status))
            {
                throw new ArgumentNullException("No object was found " + nameof(dwg.Status));
            }

            DWGFile foundFile = DB.DWGs.FirstOrDefault(e => e.FilePath == dwg.FilePath);
            if (foundFile == null) return;

            foundFile.Status = dwg.Status;

            try
            {
                using (var connection = new SqlConnection(Config.SQLConnectionString))
                {
                    connection.Open();

                    Console.WriteLine("Successfully connected to the database. LayoutStatusUpdater");

                    string queryString = "UPDATE [master].[dbo].[DWGs] " +
                        "SET [Status] = @Status " +
                        "WHERE [Prjoect] = @Prjoect " +
                        "AND [FileName] = @FileName";

                    using (var command = new SqlCommand(queryString, connection))
                    {
                        command.Parameters.AddWithValue("@Status", foundFile.Status);
                        command.Parameters.AddWithValue("@Prjoect", foundFile.Parent.DisplayName);
                        command.Parameters.AddWithValue("@FileName", foundFile.FileName);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error: " + e.Message);
            }
        }
    }
}
