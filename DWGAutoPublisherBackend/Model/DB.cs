using DWGAutoPublisherBackend.Model.DrawingHandler;
using DWGAutoPublisherBackend.Model.DrawingsFromFrontEnd;
using Microsoft.Data.SqlClient;
using Layout = DWGAutoPublisherBackend.Model.DrawingHandler.Layout;

namespace DWGAutoPublisherBackend.Model
{
    public class DB
    {
        private static DB _instance;

        public static List<Project> Projects { get; set; }
        public static List<DWGFile> DWGs { get; set; }
        public static List<Layout> Layouts { get; set; }

        private DB()
        {
            Projects = new List<Project>();
            DWGs = new List<DWGFile>();
            Layouts = new List<Layout>();
            GetDirectory();
            ConnectProjectsToDWG();

            UpdateAllLastWriteTimeAndCheckForNewLayouts();

            UpdateSQLWithProjectsOnStartup();
            UpdateSQLWithDWGsOnStartup();
            UpdateSQLWithLayoutsOnStartup();

            GetStatusForDWGsFromSQL();
            GetStatusForLayoutsromSQL();

            Console.WriteLine("DB Startup lets GO!!! \n");
            Projects.ForEach(e => { Console.WriteLine(e.ToString()); });
            Console.WriteLine("\nDB Startup ferdig \n");
        }

        public static DB Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DB();
                }

                return _instance;
            }
        }

        public static void PrintEverything()
        {
            Console.WriteLine("DB Startup lets GO!!! \n");
            Projects.ForEach(e => { Console.WriteLine(e.ToString()); });
            Console.WriteLine("\nDB Startup ferdig \n");
        }

        public static DWGFile MatchDWGFromFrontEndToDBDWGs(DWGFromFrontEnd dWGFromFrontEnd)
        {
            return DWGs.FirstOrDefault(e => e.FilePath == dWGFromFrontEnd.FilePath);
        }

        private static void GetDirectory()
        {
            var directory = new DirectoryReader(Config.RootFolder);
            List<string> list = directory.ReadAll();
            foreach (string file in list)
            {
                string child = GetTypeOfChild(file);
                int pNumber = PathToProjectNumber(file);
                string pName = PathToProjectName(file);
                if (child == "porject") Projects.Add(new Project(file, pNumber, pName));
                else if (child == ".dwg") DWGs.Add(new DWGFile(file, pNumber, pName));
            }
        }

        private static int PathToProjectNumber(string path)
        {
            try
            {
                int index = path.IndexOf(Config.ProjectIdentifier) + 2;
                Console.WriteLine(path);
                return int.Parse(path.Substring(index, Config.ProjectNumberLength));
            }
            catch
            {
                return -1;
            }
        }

        private static string PathToProjectName(string path)
        {
            int index = path.IndexOf(Config.ProjectIdentifier) + 2 + Config.ProjectNumberLength + 1;
            if (index - 1 == path.Length) return string.Empty;
            return path.Substring(index);
        }

        private static string GetTypeOfChild(string file)
        {
            string[] acseptibleSufixes = { ".dwg", ".pdf" };
            bool foundLast = false;
            int indexOfLastDot = 0;
            int searchIndex = 0;
            while (!foundLast)
            {
                searchIndex = file.IndexOf('.', searchIndex + 1);

                if (searchIndex == -1) { foundLast = true; }
                else { indexOfLastDot = searchIndex; }
            }

            if (
                indexOfLastDot == 0 &&
                file.IndexOf(Config.ProjectIdentifier) != -1 &&
                file.IndexOf('\\', file.IndexOf(Config.ProjectIdentifier)) == -1
                ) return "porject";

            if (indexOfLastDot == 0) return "folder";

            string sufix = file.Substring(indexOfLastDot);

            if (acseptibleSufixes.Contains(sufix)) return sufix;
            return string.Empty;
        }

        private static void ConnectProjectsToDWG()
        {
            foreach (Project project in Projects)
            {
                foreach (DWGFile file in DWGs)
                {
                    if (file.FilePath.Contains(project.ProjectPath))
                    {
                        if (file.FileName.Contains(project.ProjectNumber.ToString()))
                        {
                            file.Parent = project;
                            project.DWGs.Add(file);
                        }
                    }
                }
            }
        }

        public static void UpdateAllLastWriteTimeAndCheckForNewLayouts()
        {
            foreach (DWGFile file in DWGs)
            {
                file.UpdateLastWriteTime();
            }
        }

        private static void UpdateSQLWithProjectsOnStartup()
        {
            try
            {
                var namesFromDB = new List<string>();
                var missingProjectNamesFromDB = new List<string>();

                using (var conection = new SqlConnection(Config.SQLConnectionString))
                {
                    conection.Open();

                    Console.WriteLine("Successfully connected to the database. UpdateSQLWithProjects");

                    string queryString = "SELECT DisplayName FROM [master].[dbo].[Projects]";
                    using (var command = new SqlCommand(queryString, conection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine(String.Format("{0}", reader[0]));
                                namesFromDB.Add(String.Format("{0}", reader[0]));
                            }
                        }
                        foreach (var projec in Projects)
                        {
                            Console.WriteLine(projec.DisplayName);
                            if (!namesFromDB.Contains(projec.DisplayName))
                            {
                                missingProjectNamesFromDB.Add(projec.DisplayName);
                            }
                        }
                    }
                    if (missingProjectNamesFromDB.Count != 0)
                    {
                        string projectAdder = "INSERT INTO [master].[dbo].[Projects] (DisplayName) VALUES";
                        for (int i = 0; i < missingProjectNamesFromDB.Count; i++)
                        {
                            if (i == 0) projectAdder += $" (@Name{i})";
                            else projectAdder += $", (@Name{i})";
                        }
                        using (var command = new SqlCommand(projectAdder, conection))
                        {
                            for (int i = 0; i < missingProjectNamesFromDB.Count; i++)
                            {
                                command.Parameters.AddWithValue($"@Name{i}", missingProjectNamesFromDB[i]);
                            }
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error: " + e.Message);
            }
        }

        private static void UpdateSQLWithDWGsOnStartup()
        {
            try
            {
                var missingDWGsFromSQL = new List<DWGFile>();
                var allFileNamesFromSQL = new List<string>();

                using (var conection = new SqlConnection(Config.SQLConnectionString))
                {
                    conection.Open();

                    Console.WriteLine("Successfully connected to the database. UpdateSQLWithProjects");

                    string queryString = "SELECT FileName FROM [master].[dbo].[DWGs]";
                    using (var command = new SqlCommand(queryString, conection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine(String.Format("{0}", reader[0]));
                                allFileNamesFromSQL.Add(String.Format("{0}", reader[0]));
                            }
                        }
                        foreach (var dWG in DWGs)
                        {
                            Console.WriteLine(dWG.FileName);
                            if (dWG.Parent != null)
                            {
                                if (!allFileNamesFromSQL.Contains(dWG.FileName))
                                {
                                    missingDWGsFromSQL.Add(dWG);
                                }
                            }
                        }
                    }
                    if (missingDWGsFromSQL.Count != 0)
                    {
                        string dWGAdder = "INSERT INTO [master].[dbo].[DWGs] (Prjoect, FileName, Status, LastUpdated) VALUES";
                        for (int i = 0; i < missingDWGsFromSQL.Count; i++)
                        {
                            if (i == 0) dWGAdder += $" (@Prjoect{i}, @FileName{i}, @Status{i} ,@LastUpdated{i})";
                            else dWGAdder += $", (@Prjoect{i}, @FileName{i}, @Status{i} ,@LastUpdated{i})";
                        }
                        using (var command = new SqlCommand(dWGAdder, conection))
                        {
                            for (int i = 0; i < missingDWGsFromSQL.Count; i++)
                            {
                                command.Parameters.AddWithValue($"@Prjoect{i}", missingDWGsFromSQL[i].Parent.DisplayName);
                                command.Parameters.AddWithValue($"@FileName{i}", missingDWGsFromSQL[i].FileName);
                                command.Parameters.AddWithValue($"@Status{i}", missingDWGsFromSQL[i].Status);
                                command.Parameters.AddWithValue($"@LastUpdated{i}", missingDWGsFromSQL[i].LastUpdated);
                            }
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error: " + e.Message);
            }
        }

        private static void UpdateSQLWithLayoutsOnStartup()
        {
            try
            {
                var missingLayoutsFromSQL = new List<Layout>();
                var allParentFileNamesFromSQL = new List<string>();
                var allFileNamesFromSQL = new List<string>();

                using (var conection = new SqlConnection(Config.SQLConnectionString))
                {
                    conection.Open();

                    Console.WriteLine("Successfully connected to the database. UpdateSQLWithLayoutsOnStartup");

                    string queryString = "SELECT DWGFileName, Name  FROM [master].[dbo].[Layouts]";
                    using (var command = new SqlCommand(queryString, conection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                allParentFileNamesFromSQL.Add(String.Format("{0}", reader[0]));
                                allFileNamesFromSQL.Add(String.Format("{0}", reader[1]));
                            }
                        }



                        foreach (var layout in Layouts)
                        {
                            bool add = true;
                            for (int i = 0; i < allFileNamesFromSQL.Count; i++)
                            {

                                if (!(
                                    layout.Name == allFileNamesFromSQL[i] &&
                                    allParentFileNamesFromSQL[i].Contains(layout.Parent.FileName)
                                    ))
                                {
                                    add = false; break;
                                }
                            }
                            if (add) missingLayoutsFromSQL.Add(layout);
                        }
                    }

                    if (missingLayoutsFromSQL.Count != 0)
                    {
                        string layoutAdder = "INSERT INTO [master].[dbo].[Layouts] (DWGFileName, Name, Status) VALUES";
                        for (int i = 0; i < missingLayoutsFromSQL.Count; i++)
                        {
                            if (i == 0) layoutAdder += $" (@DWGFileName{i}, @Name{i}, @Status{i})";
                            else layoutAdder += $", (@DWGFileName{i}, @Name{i}, @Status{i})";
                        }
                        using (var command = new SqlCommand(layoutAdder, conection))
                        {
                            for (int i = 0; i < missingLayoutsFromSQL.Count; i++)
                            {
                                command.Parameters.AddWithValue($"@DWGFileName{i}", missingLayoutsFromSQL[i].Parent.FilePath);
                                command.Parameters.AddWithValue($"@Name{i}", missingLayoutsFromSQL[i].Name);
                                command.Parameters.AddWithValue($"@Status{i}", missingLayoutsFromSQL[i].Status);
                            }
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error: " + e.Message + " - UpdateSQLWithLayoutsOnStartup");
            }
        }

        private static void GetStatusForDWGsFromSQL()
        {
            try
            {
                using (var conection = new SqlConnection(Config.SQLConnectionString))
                {
                    conection.Open();

                    Console.WriteLine("Successfully connected to the database. GetStatusForDWGFromSQL");

                    string queryString = "SELECT FileName, Status FROM [master].[dbo].[DWGs]";
                    using (var command = new SqlCommand(queryString, conection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine(String.Format("{0}", reader[0]));
                                Console.WriteLine(String.Format("{0}", reader[1]));

                                var fount = DWGs.FirstOrDefault(e => e.FileName == String.Format("{0}", reader[0]));
                                if (fount != null) fount.Status = String.Format("{0}", reader[1]);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error: " + e.Message);
            }
        }

        private static void GetStatusForLayoutsromSQL()
        {
            try
            {
                using (var conection = new SqlConnection(Config.SQLConnectionString))
                {
                    conection.Open();

                    Console.WriteLine("Successfully connected to the database. GetStatusForLayoutsromSQL");

                    string queryString = "SELECT DWGFileName, Name, Status, LastPrinted, FilePath FROM [master].[dbo].[Layouts]";
                    using (var command = new SqlCommand(queryString, conection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var dWGFileName = String.Format("{0}", reader[0]);
                                var name = String.Format("{0}", reader[1]);
                                var status = String.Format("{0}", reader[2]);
                                var lastPrinted = String.Format("{0}", reader[3]);
                                var filePath = String.Format("{0}", reader[4]);

                                Console.WriteLine(dWGFileName);
                                Console.WriteLine(name);
                                Console.WriteLine(status);
                                Console.WriteLine(lastPrinted);
                                Console.WriteLine(filePath);

                                var fount = Layouts.FirstOrDefault(e => e.ParentFilePath == dWGFileName && e.Name == name);

                                if (fount != null)
                                {
                                    fount.Status = status;
                                    if (lastPrinted != string.Empty) fount.LastPrinted = DateTime.Parse(lastPrinted);
                                    if (filePath != string.Empty) fount.FilePath = filePath;
                                }
                            }
                        }
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