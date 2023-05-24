using DWGAutoPublisherBackend.Model.DrawingHandler;
using DWGAutoPublisherBackend.Model.DrawingsFromFrontEnd;

namespace DWGAutoPublisherBackend.Model
{
    public static class DB
    {
        public static List<Project> Projects { get; set; }
        public static List<DWGFile> DWGs { get; set; }

        public static void StartUp()
        {
            Projects = new List<Project>();
            DWGs = new List<DWGFile>();
            GetDirectory();
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

            Projects.ForEach(e => { Console.WriteLine(e.ToString()); });
            DWGs.ForEach(e => { Console.WriteLine(e.ToString()); });
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
    }
}