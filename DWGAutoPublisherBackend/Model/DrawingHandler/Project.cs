namespace DWGAutoPublisherBackend.Model.DrawingHandler
{
    public class Project
    {
        public int ProjectNumber;
        public string ProjectName;
        public string ProjectPath;
        public List<DWGFile> DWGs;

        public Project(string path, int projectNumber, string projectName)
        {
            ProjectPath = path;
            ProjectNumber = projectNumber;
            ProjectName = projectName;
            DWGs = new List<DWGFile>();
        }

        public string ToString()
        {
            string output = "Project: ";
            output += DisplayName();

            output += ". ProjectPath: ";
            output += ProjectPath;

            foreach (DWGFile file in DWGs)
            {
                output += "\n    " + file.ToString();
            }

            return output;
        }



        public string DisplayName()
        {
            return $"{Config.ProjectIdentifier}{ProjectNumber} {ProjectName}";
        }

        public void UpddateDWGs()
        {
            // Noe som leser alle under mapper
            if (DWGs == null)
            {
                DWGs = new List<DWGFile>();
            }
        }
    }
}