using DWGAutoPublisherBackend.Model;
using DWGAutoPublisherBackend.Model.DrawingsFromFrontEnd;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DWGAutoPublisherBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LayoutStatusUpdater : ControllerBase
    {
        // PUT api/<ValuesController>/5
        [HttpPut]
        public void Put([FromBody] LayoutFromFrontEnd layout)
        {
            var foundLayout = DB.Layouts.FirstOrDefault(e =>
                e.Name == layout.Name &&
                e.Parent.FilePath == layout.ParentPath
            );

            if (foundLayout == null) return;

            foundLayout.Status = layout.Status;

            try
            {
                using (var connection = new SqlConnection(Config.SQLConnectionString))
                {
                    connection.Open();

                    Console.WriteLine("Successfully connected to the database. LayoutStatusUpdater");

                    string queryString = "UPDATE [master].[dbo].[Layouts] " +
                        "SET [Status] = @Status " +
                        "WHERE [Name] = @Name " +
                        "AND [DWGFileName] = @ParentFilePath";

                    using (var command = new SqlCommand(queryString, connection))
                    {
                        command.Parameters.AddWithValue("@Status", foundLayout.Status);
                        command.Parameters.AddWithValue("@Name", foundLayout.Name);
                        command.Parameters.AddWithValue("@ParentFilePath", foundLayout.ParentFilePath);

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