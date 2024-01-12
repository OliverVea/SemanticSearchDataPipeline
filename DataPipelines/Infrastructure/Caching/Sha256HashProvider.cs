using System.Security.Cryptography;
using System.Text;

namespace DataPipelines.Infrastructure.Caching;

public class Sha256HashProvider : IHashProvider
{
    private readonly SHA256 _sha256 = SHA256.Create();

    public async Task<string> GetHashAsync<T>(T input, CancellationToken cancellationToken) where T : class
    {
        var inputString = input.ToString();
        if (string.IsNullOrWhiteSpace(inputString)) return string.Empty;
        
        var bytes = Encoding.UTF8.GetBytes(inputString);
        using var stream = new MemoryStream(bytes);
        var hash = await _sha256.ComputeHashAsync(stream, cancellationToken);
        return Convert.ToBase64String(hash);
    }
}