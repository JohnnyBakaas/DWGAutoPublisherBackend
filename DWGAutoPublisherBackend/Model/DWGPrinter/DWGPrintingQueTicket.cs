using DWGAutoPublisherBackend.Model.DrawingHandler;

namespace DWGAutoPublisherBackend.Model.DWGPrinter
{
    public class DWGPrintingQueTicket
    {
        public int TicketNumber { get; set; }
        public bool Done { get; set; }

        private DWGFile DWGFile;
        private static int lastTicket = 0;

        public DWGPrintingQueTicket(DWGFile dWGFile)
        {
            TicketNumber = lastTicket++;
            Done = false;
            DWGFile = dWGFile;
        }
    }
}
