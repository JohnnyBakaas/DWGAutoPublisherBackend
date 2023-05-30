namespace DWGAutoPublisherBackend.Model.DWGPrinter
{
    public static class DWGPrintingQue // Dette er ett flott sted å bruke en Linked List istedefor det fucka arrayet
    {
        private static List<DWGPrintingQueTicket> _ticketQue = new List<DWGPrintingQueTicket>();
        private static List<DWGPrintingQueTicket> _compleatedTickets = new List<DWGPrintingQueTicket>();
        private static bool _imStillPrinting = false;

        public static void AddDWGFileToPublishList(DWGPrintingQueTicket dWGPrintingQueTicket)
        {
            _ticketQue.Add(dWGPrintingQueTicket);
            //if !Tom liste Start printeren
        }

        public static bool TicketReplyer(int ticketNumber)
        {
            DWGPrintingQueTicket foundTicket = _compleatedTickets.FirstOrDefault(e => e.TicketNumber == ticketNumber);
            if (foundTicket != null) { return false; }
            return true;
        }

        private static void TicketHandeler()
        {
            var firstInQ = _ticketQue.FirstOrDefault();
            if (firstInQ == null) return;
            //DWGPrinter.Print(firstInQ);
            TicketHandeler();
        }
    }
}

