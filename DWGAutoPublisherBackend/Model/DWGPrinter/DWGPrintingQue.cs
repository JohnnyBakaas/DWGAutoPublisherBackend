using DWGAutoPublisherBackend.Model.AutoCAD_Handeler;
using DWGAutoPublisherBackend.Model.DrawingHandler;
using Microsoft.Data.SqlClient;

namespace DWGAutoPublisherBackend.Model.DWGPrinter
{
    public static class DWGPrintingQue
    {
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

                UpdatePathInSQL(layouts[i]);

                paths[i] = "No layout shall ever be caled this";
            }

            _compleatedTickets.Add(firstValueInQ);

            _ticketQue.RemoveFirst();

            TicketHandeler();
        }

        private static void UpdatePathInSQL(Layout layout)
        {
            try
            {
                using (var conection = new SqlConnection(Config.SQLConnectionString))
                {
                    conection.Open();

                    Console.WriteLine("Successfully connected to the database. UpdatePathInSQL in DWGPrintingQue");

                    string dQLUpdater = "UPDATE [master].[dbo].[Layouts] " +
                        "SET [LastPrinted] = @LastPrinted, " +
                        "[FilePath] = @FilePath " +
                        "WHERE [Name] = @Name " +
                        "AND [DWGFileName] = @ParentFilePath";

                    using (var command = new SqlCommand(dQLUpdater, conection))
                    {

                        command.Parameters.AddWithValue("@LastPrinted", layout.LastPrinted);
                        command.Parameters.AddWithValue("@FilePath", layout.FilePath);
                        command.Parameters.AddWithValue("@Name", layout.Name);
                        command.Parameters.AddWithValue("@ParentFilePath", layout.ParentFilePath);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error: " + e.Message + " - UpdatePathInSQL in DWGPrintingQue");
            }
        }
    }
}

