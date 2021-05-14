using System.Threading.Tasks;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Dji.UI.Extensions.Filesystem
{
    public static class FileDialog
    {
        public static async Task<string> SaveDialogAsync(string fileExtension, string defaultName = "")
        {
            var dialog = new SaveFileDialog();
            dialog.InitialFileName = DateTime.Now.ToString(defaultName);
            dialog.DefaultExtension = fileExtension;
            dialog.Title = $"{nameof(SaveDialogAsync)}";
            return await dialog.ShowAsync(DjiWindow.Instance);
        }

        public static async Task<string> OpenDialogAsync(string fileExtension)
        {
            // Filters not supported yet
            // https://github.com/AvaloniaUI/Avalonia/issues/2949
            // https://github.com/AvaloniaUI/Avalonia/issues/4001
            Trace.TraceInformation($"{nameof(OpenDialogAsync)} file filters are not yet available. '{fileExtension}' value ignored");

            var dialog = new OpenFileDialog();
            dialog.AllowMultiple = false;
            dialog.Title = $"{nameof(OpenDialogAsync)}";
            return (await dialog.ShowAsync(DjiWindow.Instance)).FirstOrDefault();
        }
    }
}
