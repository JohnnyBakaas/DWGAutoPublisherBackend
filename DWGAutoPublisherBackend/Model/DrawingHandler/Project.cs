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
            output += ". ProjectPath: ";
            output += ProjectPath;

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