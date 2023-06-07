using DWGAutoPublisherBackend.Model.DrawingHandler;
using System.Diagnostics;

namespace DWGAutoPublisherBackend.Model.AutoCAD_Handeler
{
    public static class LayoutPublisher
    {
        public static List<string> Publish(string dwgFilePath, List<Layout> layoutList)
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

            string[] lines = output.Split('\n');

            List<string> paths = new List<string>();

            foreach (string line in lines)
            {
                int indexOfLastDont = line.LastIndexOf('.');

                if (indexOfLastDont != -1)
                {
                    if (
                        indexOfLastDont < line.IndexOf('p', indexOfLastDont) &&
                        line.IndexOf('p', indexOfLastDont) < line.IndexOf('d', indexOfLastDont) &&
                        line.IndexOf('d', indexOfLastDont) < line.IndexOf('f', indexOfLastDont)
                        )
                    {
                        string cleanLine = "";
                        for (int i = 0; i < line.Length; i++)
                        {
                            // This is becouase ACAD is sutch a "Nice" program and nothing is bad about it.
                            // We love ACAD with all of ouer heart :)))))
                            if (i % 2 == 1) cleanLine += line[i];
                        }

                        Console.WriteLine(cleanLine);
                        if (cleanLine.Contains(".pdf")) paths.Add(cleanLine);
                    }
                }
            }

            File.Delete(Config.LayoutPublisherScript);

            return paths;
        }

        private static string MakeScriptString(List<Layout> layoutList)
        {
            string script = "";

            //TODO Gjør det til en fancy comand istedefor sånn det er nå :)

            foreach (Layout layout in layoutList)
            {
                script += @$"(command ""-layout"" ""s"" ""{layout.Name}"")";
                script += "\n";
                script += "-plot";
                script += "\n";
                script += "n";
                script += "\n";
                script += $"{layout.Name}";
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
            }

            script += "(close)";

            Console.WriteLine(script);

            return script;
        }

        private static void WriteScriptfile(string scriptString)
        {
            string pathName = Config.LayoutPublisherScript;
            using (StreamWriter writer = new StreamWriter(pathName, false))
            {
                writer.Write(scriptString);
            }
        }
    }
}

/*

 */