namespace DWGAutoPublisherBackend.Model.DrawingHandler
{
    public class Project
    {
        public string DisplayName { get; set; }
        public int ProjectNumber { get; set; }
        public string ProjectName { get; set; }
        public string ProjectPath { get; set; }
        public List<DWGFile> DWGs { get; set; }

        public Project(string path, int projectNumber, string projectName)
        {
            ProjectPath = path;
            ProjectNumber = projectNumber;
            ProjectName = projectName;
            DWGs = new List<DWGFile>();
            DisplayName = MakeDisplayName();

        }

        public string ToString()
        {
            string output = "Project: ";
            output += MakeDisplayName();

            output += ". ProjectPath: ";
            output += ProjectPath;

            foreach (DWGFile file in DWGs)
            {
                output += "\n    " + file.ToString();
            }

            return output;
        }



        public string MakeDisplayName()
        {
            return $"{Config.ProjectIdentifier}{ProjectNumber} {ProjectName}";
        }


    }
}