namespace DWGAutoPublisherBackend.Model.DrawingsFromFrontEnd
{
    public class DWGFromFrontEnd
    {
        public string FilePath { get; set; }
        public string Status { get; set; }
        public LayoutFromFrontEnd[] LayoutsFromFrontEnd { get; set; }
    }
}
