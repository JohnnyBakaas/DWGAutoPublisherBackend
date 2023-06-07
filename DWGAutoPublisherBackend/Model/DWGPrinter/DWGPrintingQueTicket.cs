using DWGAutoPublisherBackend.Model.DrawingHandler;

namespace DWGAutoPublisherBackend.Model.DWGPrinter
{
    public class DWGPrintingQueTicket
    {
        public int TicketNumber { get; set; }
        private DWGFile DWGFile;

        private static int lastTicket = 0;

        public DWGPrintingQueTicket(DWGFile dWGFile)
        {
            TicketNumber = lastTicket++;

            DWGFile = dWGFile;
        }

        public DWGFile GetDWGFile()
        {
            return DWGFile;
        }
    }
}
