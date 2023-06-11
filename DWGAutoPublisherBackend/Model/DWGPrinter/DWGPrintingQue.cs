using DWGAutoPublisherBackend.Model.AutoCAD_Handeler;
using DWGAutoPublisherBackend.Model.DrawingHandler;

namespace DWGAutoPublisherBackend.Model.DWGPrinter
{
    public static class DWGPrintingQue
    {
        // I should really be using a propper Que with a linked list, but its like 10 elements max... so this is fine.
        // Also the time complexety is O(n^AutoCAD) so thats the actual limiting factor. Multy threading would be a good idea to.
        // Update! Now using a LL, Dose not matter alt all, but this is cooler and better for preformance... tecnicaly
        // This is stil O(n^AutoCAD), so you know
        private static LinkedList<DWGPrintingQueTicket> _ticketQue = new LinkedList<DWGPrintingQueTicket>();
        private static List<DWGPrintingQueTicket> _compleatedTickets = new List<DWGPrintingQueTicket>();
        private static bool _imStillPrinting = false;

        public static void AddDWGFileToPublishList(DWGPrintingQueTicket dWGPrintingQueTicket)
        {
            _ticketQue.AddLast(dWGPrintingQueTicket);
            if (!_imStillPrinting)
            {
                _imStillPrinting = true;
                TicketHandeler();
            }
        }

        public static DWGFile? TicketReplyer(int ticketNumber)
        {
            DWGPrintingQueTicket? foundTicket = _compleatedTickets.FirstOrDefault(e => e.TicketNumber == ticketNumber);

            return foundTicket?.GetDWGFile() ?? null;
        }

        private static void TicketHandeler()
        {
            if (_ticketQue.Count == 0)
            {
                _imStillPrinting = false;
                return;

            }
            DWGPrintingQueTicket firstValueInQ = _ticketQue.First.Value;
            DWGFile theFile = firstValueInQ.GetDWGFile();
            var layouts = theFile.GetLayoutsToPrint();

            Console.WriteLine("DWGPRINTINGQUE Her er jeg XDDDDDDDDDXDDXDXDXDXDXDXDXXD");
            layouts.ForEach(e => { Console.WriteLine(e.ToString()); });

            List<string> paths = LayoutPublisher.Publish(theFile.FilePath, layouts);

            for (int i = 0; i < layouts.Count; i++)
            {
                string foundPath = paths.FirstOrDefault(e => e.Contains(layouts[i].Name));
                layouts[i].FilePath = paths[i];
                layouts[i].LastPrinted = DateTime.Now;

                paths[i] = "No layout shall ever be caled this";
                // Im qurious what would be better, this or shifting the array, im guessing both are shit
            }

            _compleatedTickets.Add(firstValueInQ);

            _ticketQue.RemoveFirst();

            TicketHandeler();
        }
    }
}

