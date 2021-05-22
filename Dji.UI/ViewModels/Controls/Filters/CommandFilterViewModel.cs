using Dji.Network.Packet;
using Dji.Network.Packet.DjiPackets;
using Dji.Network.Packet.DjiPackets.Base;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dji.UI.ViewModels.Controls.Filters
{
    public class CommandFilterViewModel : FilterControlViewModel
    {
        private const string DESELECTION_DESCRIPTION = "Reset Filter";

        private int? _srcModuleSelectionIndex = null;
        private int? _destModuleSelectionIndex = null;
        private int? _cmdSetSelectionIndex = null;
        private int? _cmdSelectionIndex = null;
        private int? _commsSelectionIndex = null;
        private int? _ackSelectionIndex = null;

        private List<string> _transceivers;
        private List<string> _cmdSet;
        private List<string> _comms;
        private List<string> _acks;
        private List<string> _cmd;

        public CommandFilterViewModel()
        {
            void PopulateCollection<T>(ref List<string> collection, Func<string, string> selector = null) where T : Enum
            {
                collection = new List<string>();
                // default selection = no filter
                collection.Add(DESELECTION_DESCRIPTION);

                foreach (string enumVal in Enum.GetNames(typeof(T)))
                {
                    string selectorVal = selector?.Invoke(enumVal) ?? enumVal;
                    if (collection.Contains(selectorVal)) continue;
                    else collection.Add(selectorVal);
                }
            }

            int? SubscriptionBuilder(int? value, ref List<string> collection) => !value.HasValue ? value : value.Value == 0 || value.Value == -1 ? null : value;

            PopulateCollection<Transceiver>(ref _transceivers);
            PopulateCollection<Cmd>(ref _cmdSet, cmd => CmdAttribute.TryGetAttribute(Enum.Parse<Cmd>(cmd)).CmdSetDescription);
            PopulateCollection<Cmd>(ref _cmd, cmd => CmdAttribute.TryGetAttribute(Enum.Parse<Cmd>(cmd)).CmdDescription);
            PopulateCollection<Comms>(ref _comms);
            PopulateCollection<Ack>(ref _acks);

            this.WhenAnyValue(instance => instance.SrcModuleSelectionIndex).Subscribe(idx => SrcModuleSelectionIndex = SubscriptionBuilder(idx, ref _transceivers));
            this.WhenAnyValue(instance => instance.DestModuleSelectionIndex).Subscribe(idx => DestModuleSelectionIndex = SubscriptionBuilder(idx, ref _transceivers));
            this.WhenAnyValue(instance => instance.CmdSetSelectionIndex).Subscribe(idx => CmdSetSelectionIndex = SubscriptionBuilder(idx, ref _cmdSet));
            this.WhenAnyValue(instance => instance.CmdSelectionIndex).Subscribe(idx => CmdSelectionIndex = SubscriptionBuilder(idx, ref _cmd));
            this.WhenAnyValue(instance => instance.CommsSelectionIndex).Subscribe(idx => CommsSelectionIndex = SubscriptionBuilder(idx, ref _comms));
            this.WhenAnyValue(instance => instance.AcksSelectionIndex).Subscribe(idx => AcksSelectionIndex = SubscriptionBuilder(idx, ref _acks));

            this.WhenAnyValue(instance => instance.SrcModuleSelectionIndex).Subscribe(idx => DjiNetworkPacketPool?.EvaluateFilterOnPackets());
            this.WhenAnyValue(instance => instance.DestModuleSelectionIndex).Subscribe(idx => DjiNetworkPacketPool?.EvaluateFilterOnPackets());
            this.WhenAnyValue(instance => instance.CmdSetSelectionIndex).Subscribe(idx => DjiNetworkPacketPool?.EvaluateFilterOnPackets());
            this.WhenAnyValue(instance => instance.CmdSelectionIndex).Subscribe(idx => DjiNetworkPacketPool?.EvaluateFilterOnPackets());
            this.WhenAnyValue(instance => instance.CommsSelectionIndex).Subscribe(idx => DjiNetworkPacketPool?.EvaluateFilterOnPackets());
            this.WhenAnyValue(instance => instance.AcksSelectionIndex).Subscribe(idx => DjiNetworkPacketPool?.EvaluateFilterOnPackets());
        }

        protected override Expression<Func<NetworkPacket, bool>> FilterExpression => (networkPacket) =>
            (!_srcModuleSelectionIndex.HasValue || ((DjiNetworkPacket<DjiCmdPacket>)networkPacket).DjiPacket.Sender.ToString() == _transceivers[_srcModuleSelectionIndex.Value]) &&
            (!_destModuleSelectionIndex.HasValue || ((DjiNetworkPacket<DjiCmdPacket>)networkPacket).DjiPacket.Receiver.ToString() == _transceivers[_destModuleSelectionIndex.Value]) &&
            (!_cmdSetSelectionIndex.HasValue || ((DjiNetworkPacket<DjiCmdPacket>)networkPacket).DjiPacket.CommandDetails.CmdSetDescription == _cmdSet[_cmdSetSelectionIndex.Value]) &&
            (!_cmdSelectionIndex.HasValue || ((DjiNetworkPacket<DjiCmdPacket>)networkPacket).DjiPacket.CommandDetails.CmdDescription == _cmd[_cmdSelectionIndex.Value]) &&
            (!_commsSelectionIndex.HasValue || ((DjiNetworkPacket<DjiCmdPacket>)networkPacket).DjiPacket.Comms.ToString() == _comms[_commsSelectionIndex.Value]) &&
            (!_ackSelectionIndex.HasValue || ((DjiNetworkPacket<DjiCmdPacket>)networkPacket).DjiPacket.Ack.ToString() == _acks[_ackSelectionIndex.Value]);

        public List<string> Transceivers => _transceivers;

        public List<string> CmdSet => _cmdSet;

        public List<string> Cmd => _cmd;

        public List<string> Comms => _comms;

        public List<string> Acks => _acks;

        public int? SrcModuleSelectionIndex
        {
            get => _srcModuleSelectionIndex;
            set => this.RaiseAndSetIfChanged(ref _srcModuleSelectionIndex, value);
        }

        public int? DestModuleSelectionIndex
        {
            get => _destModuleSelectionIndex;
            set => this.RaiseAndSetIfChanged(ref _destModuleSelectionIndex, value);
        }

        public int? CmdSetSelectionIndex
        {
            get => _cmdSetSelectionIndex;
            set => this.RaiseAndSetIfChanged(ref _cmdSetSelectionIndex, value);
        }

        public int? CmdSelectionIndex
        {
            get => _cmdSelectionIndex;
            set => this.RaiseAndSetIfChanged(ref _cmdSelectionIndex, value);
        }

        public int? CommsSelectionIndex
        {
            get => _commsSelectionIndex;
            set => this.RaiseAndSetIfChanged(ref _commsSelectionIndex, value);
        }

        public int? AcksSelectionIndex
        {
            get => _ackSelectionIndex;
            set => this.RaiseAndSetIfChanged(ref _ackSelectionIndex, value);
        }
    }
}
