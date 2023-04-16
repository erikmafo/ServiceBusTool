namespace ServiceBusTool.Bootstrapping;

public static class ApplicationData
{
    public static string JsonConfigPath => string.Format(
        "{0}{1}ServiceBusTool{1}ServiceBusTool.json",
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        Path.PathSeparator);
}