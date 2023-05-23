namespace DWGAutoPublisherBackend.Model.DrawingHandler
{
    public static class DWGPrintingQue
    {
        private static List<DWGFile> files = new List<DWGFile>();

        public static void AddDWGFileToPublishList(DWGFile dWGFile)
        {
            files.Add(dWGFile);
            //if !Tom liste Start printeren
        }
    }
}

