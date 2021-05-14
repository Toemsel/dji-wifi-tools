using Dji.Network.Packet;
using ReactiveUI;
using System;

namespace Dji.UI.View.DataGrid
{
    public class ContextMenuEntry : ReactiveObject
    {
        private readonly string _title;
        private readonly Action<NetworkPacket> _onClick;

        public ContextMenuEntry(string title, Action<NetworkPacket> onClick) => (_title, _onClick) = (title, onClick);

        public string Title => _title;

        public Action<NetworkPacket> OnClick => _onClick;
    }
}