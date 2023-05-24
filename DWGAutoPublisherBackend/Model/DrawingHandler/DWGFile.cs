using DWGAutoPublisherBackend.Model.DrawingsFromFrontEnd;

namespace DWGAutoPublisherBackend.Model.DrawingHandler
{
    public class DWGFile
    {
        public string FilePath;
        public int ProjectNumber;
        public string ProjectName;
        public string FileName;
        public string Status;
        public List<Layout> Layouts;
        private List<Layout> LayoutsToPrint; // Legg til ett delay på front enden

        public DWGFile(string path, int projectNumber, string projectName)
        {
            FilePath = path;
            ProjectNumber = projectNumber;
            ProjectName = projectName;

            FileName = GetNameFromFilePath();

            Status = "Ikke påbegynt";

            Layouts = new List<Layout>();
            LayoutsToPrint = new List<Layout>();
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

            output += ". FilePath: ";
            output += FilePath;

            return output;
        }

        private string GetNameFromFilePath()
        {
            string reversedPath = ReverseThisShit(FilePath);

            string reversedFileName = reversedPath.Substring(0, reversedPath.IndexOf("\\"));
            return ReverseThisShit(reversedFileName);
        }

        private string ReverseThisShit(string shit)
        {
            string shitOut = string.Empty;

            for (int i = shit.Length - 1; i >= 0; i--)
            {
                shitOut += shit[i];
            }

            return shitOut;
        }

        private List<string> RetriveLayoutNames()
        {
            List<string> layoutNames = new List<string>();
            // Gjør det så den henter navnene på alle Layouts
            return layoutNames;
        } //TODO Legg til funksjonalitet

        public void UpdateLayoutList()
        {
            List<string> layoutNames = RetriveLayoutNames();
            foreach (string layoutName in layoutNames)
            {
                if (Layouts.Exists(e => e.Name != layoutName)) Layouts.Add(new Layout(layoutName, this));
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
    }
}