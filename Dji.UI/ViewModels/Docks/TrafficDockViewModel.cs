using Avalonia.Threading;
using Dji.Network.Packet;
using Dji.UI.Extensions;
using Dji.UI.Pooling;
using Dji.UI.View.DataGrid;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Dji.UI.ViewModels.Docks
{
    public abstract class TrafficDockViewModel : DjiNetworkPacketPool
    {
        private static readonly Dictionary<DjiHexComparisonWindowViewModel, DjiHexComparisonWindow> _djiHexWindowMap = new();

        private DjiNetworkInformation _djiNetworkInformation;

        public TrafficDockViewModel()
        {
            DjiContentViewModel.Instance.PacketSniffer.NetworkStatusChanged += (obj, data) => DjiNetworkInformation = data;
            DjiHexComparisonWindowViewModel.HexComparisonAdded += (obj, data) => HexComparisonAdded(data);
            DjiHexComparisonWindowViewModel.HexComparisonRemoved += (obj, data) => HexComparisonRemoved(data);

            // define what happens if a context-menu item has been selected
            ContextMenuInteractionCommand = ReactiveCommand.Create<Tuple<object, object>>(tuple =>
                ContextMenuSelected(tuple), outputScheduler: AvaloniaScheduler.Instance);

            // build the default context menu
            BuildContextMenu();
        }

        protected DjiNetworkInformation DjiNetworkInformation
        {
            get => _djiNetworkInformation;
            set => this.RaiseAndSetIfChanged(ref _djiNetworkInformation, value);
        }

        public IEnumerable<ContextMenuEntry> ContextMenuEntries { get; private set; }

        public ICommand ContextMenuInteractionCommand { get; init; }

        private void HexComparisonAdded(DjiHexComparisonWindowViewModel viewModel) => BuildContextMenu();

        private void HexComparisonRemoved(DjiHexComparisonWindowViewModel viewModel)
        {
            // TrafficDockViewModel is responsible for the DjiHexComparisonWindow,
            // as it has been created and shown within this context.
            // thus, as soon as no packet is within the inspector present,
            // we require todo propper cleanup. Hence, close the window.
            if (_djiHexWindowMap.ContainsKey(viewModel) && 
                viewModel.HexControlViewModels.Count == 0)
            {
                _djiHexWindowMap[viewModel].Close();
                _djiHexWindowMap.Remove(viewModel);
            }

            // as the window is gone, re-build the context menu
            BuildContextMenu();
        }

        private void ContextMenuSelected(Tuple<object, object> param)
        {
            NetworkPacket selectedNetworkPacket = param.Item1 as NetworkPacket;
            ContextMenuEntry selectedContextMenu = param.Item2 as ContextMenuEntry;
            if (selectedNetworkPacket == null || selectedContextMenu == null) return;
            selectedContextMenu.OnClick?.Invoke(selectedNetworkPacket);
        }

        protected virtual IEnumerable<ContextMenuEntry> CreateDockSpecificContextMenu() => Enumerable.Empty<ContextMenuEntry>();

        private void BuildContextMenu()
        {
            // rebuild a entirely new context menu
            var contextMenu = new List<ContextMenuEntry>();

            // add a default 'new' hexWindow
            contextMenu.Add(new ContextMenuEntry("Open in inspector", (networkPacket) =>
            {
                var hexComparisonWindow = new DjiHexComparisonWindow();
                var hexComparsionViewModel = hexComparisonWindow.DataContext as DjiHexComparisonWindowViewModel;

                // archive the new comparison window for any other Traffic-Dock instance
                _djiHexWindowMap.Add(hexComparsionViewModel, hexComparisonWindow);

                // add the network-packet to the inspector
                hexComparsionViewModel.Add(networkPacket);

                // open up the window
                hexComparisonWindow.Show(DjiWindow.Instance);
            }));

            // add all available hexWindows
            foreach (var hexWindow in _djiHexWindowMap.Keys)
                contextMenu.Add(new ContextMenuEntry($"Add to inspector {hexWindow.Title}", (networkPacket) => hexWindow.Add(networkPacket)));

            // add copy commands
            contextMenu.Add(new ContextMenuEntry("Copy data hex", async (networkPacket) => await networkPacket?.UdpPacket?.Data?.CopyToClipboard(true)));
            contextMenu.Add(new ContextMenuEntry("Copy data stream", async (networkPacket) => await networkPacket?.UdpPacket?.Data?.CopyToClipboard(false)));
            contextMenu.Add(new ContextMenuEntry("Copy payload hex", async (networkPacket) => await networkPacket?.UdpPacket?.Payload?.CopyToClipboard(true)));
            contextMenu.Add(new ContextMenuEntry("Copy payload stream", async (networkPacket) => await networkPacket?.UdpPacket?.Payload?.CopyToClipboard(false)));

            // add context-specific entries
            contextMenu.AddRange(CreateDockSpecificContextMenu());

            ContextMenuEntries = contextMenu;
            // inform the UI that a new context menu is available
            this.RaisePropertyChanged(nameof(ContextMenuEntries));
        }
    }
}