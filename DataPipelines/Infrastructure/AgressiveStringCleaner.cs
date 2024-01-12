namespace DataPipelines.Infrastructure;

public class AggressiveStringCleaner : IStringCleaner
{
    private const string SingleSpace = " ";
    private const string DoubleSpace = "  ";
    private static readonly HashSet<char> AllowedChars = ['\n', ' ', '-', '.', ',', '/', '&', '#'];

    public Dictionary<string, T> Clean<T>(Dictionary<string, T> attributes)
    {
        return attributes.ToDictionary(
            x => Clean(x.Key),
            x => x.Value);
    }

    public Dictionary<string, string[]> Clean<T>(Dictionary<string, string[]> stringAttributes)
    {
        return stringAttributes.ToDictionary(
            x => Clean(x.Key),
            x => x.Value.Select(Clean).ToArray());
    }
    
    public string Clean(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        
        var charEnumerable = text
            .Where(IsValidChar)
            .Select(ReplaceWhitespaces);
        
        text = new string(charEnumerable.ToArray());
        while (text.Contains(DoubleSpace)) text = text.Replace(DoubleSpace, SingleSpace);
        
        return text.Trim().ToLowerInvariant();
    }
    
    private static bool IsValidChar(char c)
    {
        return char.IsLetterOrDigit(c) || AllowedChars.Contains(c);
    }
    
    private static char ReplaceWhitespaces(char c)
    {
        return char.IsWhiteSpace(c) ? ' ' : c;
    }
}