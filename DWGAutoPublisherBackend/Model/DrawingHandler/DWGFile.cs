using DWGAutoPublisherBackend.Model.DrawingsFromFrontEnd;

namespace DWGAutoPublisherBackend.Model.DrawingHandler
{
    public class DWGFile
    {
        public int ProjectNumber;
        public string FilePath;
        public string FileName;
        public string Status;
        public List<Layout> Layouts;
        private List<Layout> LayoutsToPrint; // Legg til ett delay på front enden



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
