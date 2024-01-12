namespace DataPipelines.Infrastructure;

public interface IStringCleaner
{
    string Clean(string input);
    Dictionary<string, T> Clean<T>(Dictionary<string, T> attributes);
    Dictionary<string, string[]> Clean<T>(Dictionary<string, string[]> stringAttributes);
}