namespace DWGAutoPublisherBackend.Model.DrawingHandler
{
    public class Layout
    {
        public DWGFile Parent;
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime LastPrinted { get; set; }
        public bool HasBeenPrinted => LastPrinted != DateTime.MinValue;
        public string? FilePath { get; set; }

        public Layout(string name, DWGFile parent)
        {
            Name = name;
            Parent = parent;
            Status = "Under prosjektering";
            LastPrinted = DateTime.MinValue;
            FilePath = null;
        }

        public string ToString()
        {
            return Name;
        }
    }
}
