namespace DWGAutoPublisherBackend.Model.DrawingHandler
{
    public class Layout
    {
        public DWGFile Parent;
        public string Name;
        public string Status;
        public DateTime LastPrinted;
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
    }
}
