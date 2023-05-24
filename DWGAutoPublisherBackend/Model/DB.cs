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
                string sufix = GetSufix(file);
                Console.WriteLine(file);
                Console.WriteLine("Dette er en " + sufix);
            }
        }

        private static string GetSufix(string file)
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
                file.IndexOf("P-") != -1 &&
                file.IndexOf('\\', file.IndexOf("P-")) == -1
                ) return "porject";

            if (indexOfLastDot == 0) return "folder";


            string sufix = file.Substring(indexOfLastDot);

            if (acseptibleSufixes.Contains(sufix)) return sufix;
            return string.Empty;
        }

        private static void MakeProjectOrDWG(string file)
        {
            string sufix = GetSufix(file);
            if (sufix == null) return;
            if (sufix == "folder")
            {

            }
        }
    }
}