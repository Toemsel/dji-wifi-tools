using Dji.UI.ViewModels.Controls.Filters;
using Avalonia.Controls;
using Dji.UI.Pooling;
using Avalonia.Data;
using Avalonia;
using System;

namespace Dji.UI.View.Controls.Filters
{
    public class FilterControl : UserControl, IDisposable
    {
        private DjiNetworkPacketPool _djiNetworkPacketPool;

        public static readonly DirectProperty<FilterControl, DjiNetworkPacketPool> DjiNetworkPacketPoolProperty = AvaloniaProperty.RegisterDirect<FilterControl, DjiNetworkPacketPool>(
            nameof(DjiNetworkPacketPool),
            u => u.DjiNetworkPacketPool,
            (u, i) => u.SetDjiNetworkPacketPool(i),
            defaultBindingMode: BindingMode.OneTime);

        public DjiNetworkPacketPool DjiNetworkPacketPool
        {
            get => _djiNetworkPacketPool;
            set => SetAndRaise(DjiNetworkPacketPoolProperty, ref _djiNetworkPacketPool, value);
        }

        public FilterControl() => DataContextChanged += FilterControl_DataContextChanged;

        private void SetDjiNetworkPacketPool(DjiNetworkPacketPool djiNetworkPacketPool)
        {
            _djiNetworkPacketPool = djiNetworkPacketPool;

            if (DataContext is FilterControlViewModel viewModel && viewModel.DjiNetworkPacketPool == null)
                FilterControl_DataContextChanged(null, null);
        }

        private void FilterControl_DataContextChanged(object sender, EventArgs e)
        {
            if (DataContext is FilterControlViewModel viewModel)
                viewModel.DjiNetworkPacketPool = _djiNetworkPacketPool;
        }

        public void Dispose() => DataContextChanged -= FilterControl_DataContextChanged;
    }
}