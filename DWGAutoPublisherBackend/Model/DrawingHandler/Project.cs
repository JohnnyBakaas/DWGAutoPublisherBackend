namespace DWGAutoPublisherBackend.Model.DrawingHandler
{
    public class Project
    {
        public int ProjectNumber;
        public string ProjectName;
        public List<DWGFile> DWGs;

        public string DisplayName()
        {
            string displayName = $"P-{ProjectNumber} {ProjectName}";
            Console.WriteLine(displayName);
            return displayName;
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
