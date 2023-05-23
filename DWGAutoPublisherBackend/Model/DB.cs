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
        }

        public static DWGFile MatchDWGFromFrontEndToDBDWGs(DWGFromFrontEnd dWGFromFrontEnd)
        {
            return DWGs.Find(e => e.FilePath == dWGFromFrontEnd.FilePath);
        }
    }
}
