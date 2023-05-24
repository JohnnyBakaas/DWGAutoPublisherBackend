public class DirectoryReader
{
    private string _directoryPath;

    public DirectoryReader(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            throw new ArgumentException("Directory path cannot be null or empty.", nameof(directoryPath));
        }

        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"The directory '{directoryPath}' does not exist.");
        }

        _directoryPath = directoryPath;
    }

    public List<string> ReadAll()
    {
        var list = new List<string>();
        ReadAllInternal(_directoryPath, list);
        return list;
    }

    private void ReadAllInternal(string directoryPath, List<string> list)
    {
        try
        {
            foreach (var directory in Directory.GetDirectories(directoryPath))
            {
                list.Add(directory);
                ReadAllInternal(directory, list); // Recursion for subdirectories
            }

            foreach (var file in Directory.GetFiles(directoryPath))
            {
                list.Add(file);
            }
        }
        catch (UnauthorizedAccessException uaex)
        {
            Console.WriteLine($"Access denied to: {directoryPath}. {uaex.Message}");
        }
        catch (PathTooLongException ptlex)
        {
            Console.WriteLine($"Path too long: {directoryPath}. {ptlex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred accessing path: {directoryPath}. {ex.Message}");
        }
    }
}
