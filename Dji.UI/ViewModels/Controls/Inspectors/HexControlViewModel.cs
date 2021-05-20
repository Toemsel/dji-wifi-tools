using System.Collections.Generic;
using System.Diagnostics;
using Dji.Network.Packet;
using ReactiveUI;
using System;

namespace Dji.UI.ViewModels.Controls.Inspectors
{
    public class HexControlViewModel : ReactiveObject, IBinaryComparable<HexControlViewModel>
    {
        private readonly NetworkPacket _networkPacket;
        private readonly string _title;

        public HexControlViewModel(NetworkPacket networkPacket)
        {
            _networkPacket = networkPacket;
            _title = $"#{_networkPacket.Id}";

            foreach (var currentByte in _networkPacket.Payload)
                HexValueViewModels.Add(new HexValueControlViewModel(currentByte));
        }

        public byte[] Data => _networkPacket.Payload;

        public string Title => _title;

        public NetworkPacket NetworkPacket => _networkPacket;

        public List<HexValueControlViewModel> HexValueViewModels { get; init; } = new List<HexValueControlViewModel>();

        public void ResetUniqueness() => HexValueViewModels.ForEach(viewModel => viewModel.ResetUniqueness());

        public void DetermineUniqueness(HexControlViewModel other)
        {
            int maxIndex = Math.Min(HexValueViewModels.Count, other.HexValueViewModels.Count);

            if (maxIndex != HexValueViewModels.Count ||
                maxIndex != other.HexValueViewModels.Count)
                Trace.TraceWarning($"{nameof(DetermineUniqueness)} invalid comparison base");

            for (int index = 0; index < maxIndex; index++)
                HexValueViewModels[index].DetermineUniqueness(other.HexValueViewModels[index]);
        }
    }
}