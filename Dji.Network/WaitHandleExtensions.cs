using System.Threading;

namespace Dji.Network
{
    public static class WaitHandleExtensions
    {
        public static bool WaitOne(this WaitHandle handle, CancellationToken cancellationToken)
        {
            int n = WaitHandle.WaitAny(new[] { handle, cancellationToken.WaitHandle });
            switch (n)
            {
                case 0:
                    return true;
                default:
                    return false;
            }
        }
    }
}
