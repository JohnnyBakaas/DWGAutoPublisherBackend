using DWGAutoPublisherBackend.Model.DrawingHandler;
using System.Diagnostics;

namespace DWGAutoPublisherBackend.Model.AutoCAD_Handeler
{
    public static class LayoutPublisher
    {
        public static void Publish(string dwgFilePath, List<Layout> layoutList)
        {
            string scriptContent = MakeScriptString(layoutList);
            WriteScriptfile(scriptContent);

            Process autocadProcess = new Process();
            autocadProcess.StartInfo.FileName = Config.AutoCADCoreConsolePath;
            autocadProcess.StartInfo.Arguments = $@"/i ""{dwgFilePath}"" /s ""{Config.LayoutPublisherScript}""";
            autocadProcess.StartInfo.UseShellExecute = false;
            autocadProcess.StartInfo.RedirectStandardOutput = true;
            autocadProcess.StartInfo.RedirectStandardInput = true;
            autocadProcess.Start();
            // Read the output from AutoCAD Core Console
            string internalOutput = autocadProcess.StandardOutput.ReadToEnd();
            Console.WriteLine("AutoCAD output: " + internalOutput.Trim());

            // Send "exit" command to AutoCAD Core Console
            autocadProcess.StandardInput.WriteLine("exit");

            // Wait for AutoCAD Core Console to complete
            autocadProcess.WaitForExit();
            string output = internalOutput.Trim();

            Console.WriteLine(output);

            File.Delete(Config.LayoutPublisherScript);
        }

        private static string MakeScriptString(List<Layout> layoutList)
        {
            string script = @"(command ""_netload"" ""C:\\Program Files\\Autodesk\\AutoCAD 2022\\AutoCAD Plugins and Addons.dll"")";
            script += "\n";

            foreach (Layout layout in layoutList)
            {
                script += @$"(command ""-layout"" ""s"" ""{layout.Name}"")";
                script += "\n";
                //script += @$"(command ""PublishLayoutToPDFAOne"" ""{"isoA1"/*layout.Name*/}"")";
                script += "-plot";
                script += "\n";
                script += "";
                script += "\n";
                script += "";
                script += "\n";
                script += "";
                script += "\n";
                script += "";
                script += "\n";
                script += "";
                script += "\n";
                script += "y";
                script += "\n";
                script += "y";
                script += "\n";
                script += "y";
                script += "\n";
                script += "y";
                script += "\n";
            }

            script += "(close)";

            Console.WriteLine(script);

            return script;
        }


        private static void WriteScriptfile(string scriptString)
        {
            string pathName = Config.LayoutPublisherScript;
            using (StreamWriter writer = new StreamWriter(pathName, false))  // opens the file if it exists or creates a new one
            {
                writer.Write(scriptString);
            }
        }

    }
}

/*
                
 */