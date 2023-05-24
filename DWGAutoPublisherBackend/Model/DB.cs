using DWGAutoPublisherBackend.Model.DrawingHandler;
using DWGAutoPublisherBackend.Model.DrawingsFromFrontEnd;

namespace DWGAutoPublisherBackend.Model
{
    public class DB
    {
        private static DB _instance;

        private DB()
        {
            Projects = new List<Project>();
            DWGs = new List<DWGFile>();
            GetDirectory();
            ConnectProjectsToDWG();
            UpdateAllLastWriteTimeAndCheckForNewLayouts();

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

        public static List<Project> Projects { get; set; }
        public static List<DWGFile> DWGs { get; set; }

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
            int index = path.IndexOf(Config.ProjectIdentifier) + 2;
            return int.Parse(path.Substring(index, Config.ProjectNumberLength));
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
                        project.DWGs.Add(file);
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
    }
}