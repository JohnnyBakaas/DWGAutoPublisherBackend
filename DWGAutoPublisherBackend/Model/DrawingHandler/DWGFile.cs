using DWGAutoPublisherBackend.Model.AutoCAD_Handeler;
using DWGAutoPublisherBackend.Model.DrawingsFromFrontEnd;

namespace DWGAutoPublisherBackend.Model.DrawingHandler
{
    public class DWGFile
    {
        public string FilePath { get; set; }
        public int ProjectNumber { get; set; }
        public string ProjectName { get; set; }
        public string FileName { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<Layout> Layouts { get; set; }

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

            UpdateLayoutList();
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

            output += ". LastUpdated: ";
            output += LastUpdated;

            output += ". FilePath: ";
            output += FilePath;

            foreach (var layout in Layouts)
            {
                output += layout.ToString();
            }

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

        public void UpdateLayoutList()
        {
            List<string> layoutNames = LayoutReader.Read(FilePath);
            foreach (string layoutName in layoutNames)
            {
                //Console.WriteLine(layoutName);
                if (!Layouts.Exists(e => e.Name == layoutName))
                {
                    Layouts.Add(new Layout(layoutName, this));
                }
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

        public void UpdateLastWriteTime()
        {
            DateTime lastWriteTime = File.GetLastWriteTime(FilePath);
            if (LastUpdated != lastWriteTime)
            {
                LastUpdated = lastWriteTime;
                UpdateLayoutList();
            }
        }
    }
}