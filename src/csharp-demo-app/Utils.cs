using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace csharp_demo_app
{
    public static class Utils
    {
        public static async Task<byte[]> GetBytesFromStreamAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            await using var memStream = new MemoryStream();
            await stream.CopyToAsync(memStream, cancellationToken).ConfigureAwait(false);
            return memStream.ToArray();
        }
    }
}