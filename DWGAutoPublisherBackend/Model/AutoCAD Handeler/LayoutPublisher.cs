using DWGAutoPublisherBackend.Model.DrawingHandler;
using System.Diagnostics;

namespace DWGAutoPublisherBackend.Model.AutoCAD_Handeler
{
    public static class LayoutPublisher
    {
        public static void Publish(string filePath, List<Layout> layoutList)
        {
            List<string> layoutNames = new List<string>();
            foreach (Layout layout in layoutList)
            {
                layoutNames.Add(layout.Name);
            }

            if (layoutNames.Count == 0) { return; }

            string layouts = string.Join(" ", layoutNames.Select(name => $"\"{name}\""));

            Console.WriteLine(Config.LayoutPublisherBatch + " Config.LayoutPublisherBatch");
            Console.WriteLine(layouts + " layouts");
            Console.WriteLine(filePath + " filePath");

            ProcessStartInfo psi = new ProcessStartInfo()
            {
                FileName = Config.LayoutPublisherBatch,
                Arguments = $"\"{filePath}\" \"{layouts}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();

                // Uncomment the following lines if you want to get the console output
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                // Uncomment the following lines if you want to get the console output
                Console.WriteLine(output);
                Console.WriteLine(error);
            }
        }
    }
}