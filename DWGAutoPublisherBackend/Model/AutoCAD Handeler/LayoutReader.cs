using System.Diagnostics;

namespace DWGAutoPublisherBackend.Model.AutoCAD_Handeler
{
    public static class LayoutReader
    {
        public static List<string> Read(string dwgFilePath)
        {
            string autoCADPath = Config.AutoCADCoreConsolePath;
            //@"C:\Test for Autocad greier\P-10001\DWG\P-10001 2.etg.dwg"
            string scriptFilePath = Config.LayoutReaderScript;

            string output = string.Empty;

            try
            {
                Process autocadProcess = new Process();
                autocadProcess.StartInfo.FileName = autoCADPath;
                autocadProcess.StartInfo.Arguments = $@"/i ""{dwgFilePath}"" /s ""{scriptFilePath}""";
                autocadProcess.StartInfo.UseShellExecute = false;
                autocadProcess.StartInfo.RedirectStandardOutput = true;
                autocadProcess.StartInfo.RedirectStandardInput = true;
                autocadProcess.Start();
                // Read the output from AutoCAD Core Console
                string internalOutput = autocadProcess.StandardOutput.ReadToEnd();
                //Console.WriteLine("AutoCAD output: " + internalOutput.Trim());

                // Send "exit" command to AutoCAD Core Console
                autocadProcess.StandardInput.WriteLine("exit");

                // Wait for AutoCAD Core Console to complete
                autocadProcess.WaitForExit();
                output = internalOutput.Trim();


                List<string> layouts = new List<string>();
                int index = output.IndexOf('|');

                while (index != -1)
                {
                    int nextIndex = output.IndexOf('|', index + 1);

                    if (nextIndex - index != 2)
                    {
                        string layoutString = string.Empty;
                        for (int i = index + 1; i < nextIndex; i++)
                        {
                            if (i % 2 != 1) layoutString += output[i];
                        }
                        if (layoutString != "  " && layoutString != "" && layoutString != "Model")
                            layouts.Add(layoutString);
                    }

                    // Hold på bunn
                    index = nextIndex;
                }
                /*
                foreach (string layout in layouts)
                {
                    Console.WriteLine(layout);
                }

                Console.WriteLine();
                Console.WriteLine("HEi");
                */

                return layouts;
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return new List<string>();
            }
        }
    }
}