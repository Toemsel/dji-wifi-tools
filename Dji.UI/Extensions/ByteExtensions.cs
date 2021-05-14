using Dji.Network.Packet.Extensions;
using System.Threading.Tasks;

namespace Dji.UI.Extensions
{
    public static class ByteExtensions
    {
        public static async Task CopyToClipboard(this byte[] data, bool readable = false) => await TextCopy.ClipboardService.SetTextAsync(data.ToHexString(readable, readable));
    }
}