namespace DWGAutoPublisherBackend.Model
{
    public static class Config
    {
        public static string RootFolder { get; } = @"C:\Test for Autocad greier";
        public static string ProjectIdentifier { get; } = "P-";
        public static int ProjectNumberLength { get; } = 5;
        public static string AutoCADCoreConsolePath { get; } = @"C:\Program Files\Autodesk\AutoCAD 2022\accoreconsole.exe";
    }
}
