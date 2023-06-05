namespace DWGAutoPublisherBackend.Model.DrawingHandler
{
    public class Layout
    {
        public DWGFile Parent;
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime LastPrinted { get; set; }
        public bool HasBeenPrinted => LastPrinted != DateTime.MinValue;

        public Layout(string name, DWGFile parent)
        {
            Name = name;
            Parent = parent;
            Status = "Under prosjektering";
            LastPrinted = DateTime.MinValue;
        }

        public void AddToPrintOrder()
        {
            //Parent.AddLayoutsToPrint(this);
        }

        public string ToString()
        {
            return Name;
        }
    }
}
