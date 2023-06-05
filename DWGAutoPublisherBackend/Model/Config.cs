namespace DWGAutoPublisherBackend.Model
{
    public static class Config
    {
        // If you want to use this yourself youll also have to change the LayoutReaderScript
        private static string _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        public static string LayoutReaderScript = Path.Combine(_baseDirectory, "Model", "AutoCAD Handeler", "Scripts", "LayoutReaderScript.scr");
        public static string LayoutPublisherScript = Path.Combine(_baseDirectory, "Model", "AutoCAD Handeler", "Scripts", "GeneratedScript.scr");

        public static string RootFolder { get; } = @"C:\Test for Autocad greier";
        public static string ProjectIdentifier { get; } = "P-";
        public static int ProjectNumberLength { get; } = 5;
        public static string AutoCADCoreConsolePath { get; } = @"C:\Program Files\Autodesk\AutoCAD 2022\accoreconsole.exe";
    }
}
