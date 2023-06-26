using DWGAutoPublisherBackend.Model.AutoCAD_Handeler;
using DWGAutoPublisherBackend.Model.DrawingsFromFrontEnd;
using Microsoft.Data.SqlClient;

namespace DWGAutoPublisherBackend.Model.DrawingHandler
{
    public class DWGFile
    {
        public string FilePath { get; set; }
        public int ProjectNumber { get; set; }
        public string ProjectName { get; set; }
        public string FileName { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<Layout> Layouts { get; set; }

        private List<Layout> LayoutsToPrint; // Legg til ett delay på front enden
        public Project Parent;

        public DWGFile(string path, int projectNumber, string projectName)
        {
            FilePath = path;
            ProjectNumber = projectNumber;
            ProjectName = projectName;

            FileName = GetNameFromFilePath();

            Status = "Ikke påbegynt";

            Layouts = new List<Layout>();
            LayoutsToPrint = new List<Layout>();

            UpdateLayoutList();
        }

        public string ToString()
        {
            string output = "ProjectNumber: ";
            output += ProjectNumber;
            if (ProjectName != string.Empty)
            {
                output += ". ProjectName: ";
                output += ProjectName;
            }

            output += ". FileName: ";
            output += FileName;

            output += ". Status: ";
            output += Status;

            output += ". LastUpdated: ";
            output += LastUpdated;

            output += ". FilePath: ";
            output += FilePath;

            foreach (var layout in Layouts)
            {
                output += layout.ToString();
            }

            return output;
        }

        private string GetNameFromFilePath()
        {
            string reversedPath = ReverseString(FilePath);

            string reversedFileName = reversedPath.Substring(0, reversedPath.IndexOf("\\"));
            return ReverseString(reversedFileName);
        }

        private string ReverseString(string input)
        {
            string outPut = string.Empty;

            for (int i = input.Length - 1; i >= 0; i--)
            {
                outPut += input[i];
            }

            return outPut;
        }

        public void UpdateLayoutList()
        {
            List<string> layoutNames = LayoutReader.GetLayoutNames(FilePath);
            foreach (string layoutName in layoutNames)
            {
                //Console.WriteLine(layoutName);
                if (!Layouts.Exists(e => e.Name == layoutName))
                {
                    var layout = new Layout(layoutName, this);
                    Layouts.Add(layout);
                    DB.Layouts.Add(layout);
                }
            }
        }

        public void UpdateStatus(DWGFromFrontEnd file)
        {
            if (!Status.Equals(file.Status)) return;
            Status = file.Status;
            UpdateLayoutsStatus();
        }

        public void AddToLayoutsToPrint(DWGFromFrontEnd file)
        {
            foreach (LayoutFromFrontEnd layout in file.LayoutsFromFrontEnd)
            {
                if (layout.ToBePrinted)
                {
                    var foundLayout = Layouts.FirstOrDefault(e => e.Name == layout.Name);
                    if (foundLayout != null) LayoutsToPrint.Add(foundLayout);
                }
            }
        }

        private void UpdateLayoutsStatus()
        {
            foreach (Layout layout in Layouts)
            {
                layout.Status = Status;
            }
        }

        public void UpdateLastWriteTime()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(FilePath);
            if (!LastUpdated.Equals(lastWriteTime))
            {
                LastUpdated = lastWriteTime;

                try
                {
                    using (var connection = new SqlConnection(Config.SQLConnectionString))
                    {
                        connection.Open();
                        Console.WriteLine("Successfully connected to the database. UpdateLastWriteTime");

                        string queryString = "UPDATE [master].[dbo].[DWGs] SET LastUpdated = @LastUpdated WHERE FileName = @FileName";
                        using (var command = new SqlCommand(queryString, connection))
                        {
                            command.Parameters.AddWithValue("@LastUpdated", LastUpdated);
                            command.Parameters.AddWithValue("@FileName", FileName);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                UpdateLayoutList();
                RemoveLinkesToLayoutPathInSQL();
            }
        }

        private void RemoveLinkesToLayoutPathInSQL()
        {
            foreach (Layout layout in Layouts)
            {
                if (!layout.PastNoReturn())
                {
                    try
                    {
                        using (var connection = new SqlConnection(Config.SQLConnectionString))
                        {
                            connection.Open();
                            Console.WriteLine("Successfully connected to the database. UpdateLastWriteTime");

                            string queryString =
                                "UPDATE [master].[dbo].[Layouts] " +
                                "SET LastPrinted = @LastPrinted " +
                                "WHERE DWGFileName = @DWGFileName AND Name = @Name";
                            using (var command = new SqlCommand(queryString, connection))
                            {
                                command.Parameters.AddWithValue("@LastPrinted", null);
                                command.Parameters.AddWithValue("@DWGFileName", FilePath);
                                command.Parameters.AddWithValue("@Name", layout.Name);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        public List<Layout> GetLayoutsToPrint()
        {
            var layouts = new List<Layout>(LayoutsToPrint.Count);
            foreach (var layout in LayoutsToPrint)
            {
                layouts.Add(layout);
            }
            LayoutsToPrint = new List<Layout>();
            return layouts;
        }
    }
}