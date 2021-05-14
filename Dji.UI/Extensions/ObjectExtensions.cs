using Avalonia.Threading;
using System;
using System.Threading.Tasks;

namespace Dji.UI.Extensions
{
    public static class ObjectExtensions
    {
        public static void OnUIThread(this object obj, Action action, DispatcherPriority dispatcherPriority = DispatcherPriority.DataBind)
        {
            if (!Dispatcher.UIThread.CheckAccess())
                Dispatcher.UIThread.Post(action, dispatcherPriority);
            else action();
        }

        public static async Task OnUIThreadAsync(this object obj, Action action, DispatcherPriority dispatcherPriority = DispatcherPriority.DataBind)
        {
            if (!Dispatcher.UIThread.CheckAccess())
                await Dispatcher.UIThread.InvokeAsync(action, dispatcherPriority);
            else action();
        }
    }
}
