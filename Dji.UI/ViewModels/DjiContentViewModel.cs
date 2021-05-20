using Dji.Network;
using Dji.Network.Packet;
using Dji.Network.Packet.DjiPackets.Base;
using Dji.UI.Extensions.Filesystem;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dji.UI.ViewModels
{
    public class DjiContentViewModel : ReactiveObject
    {
        private bool _isRecording;

        private static readonly Lazy<DjiContentViewModel> _singleton = new(() => new());

        private readonly DjiPacketResolver _operatorPacketResolver = new DjiOperatorPacketResolver();
        private readonly DjiPacketResolver _dronePacketResolver = new DjiDronePacketResolver();
        private readonly DjiPacketPCapWriter _packetWriter = new();
        private readonly DjiPacketSniffer _packetSniffer = new();

        private List<byte[]> data = new List<byte[]>();

        private DjiContentViewModel()
        {
            // connect the Dji network-related parts
            _packetSniffer.NetworkPacketReceived += (obj, data) => _packetWriter.Write(data);
            _packetSniffer.NetworkPacketReceived += (obj, data) => NetworkPacketReceived(data);
            _operatorPacketResolver.AddDjiPacketListener(packet => _packetWriter.Write(packet));
            _dronePacketResolver.AddDjiPacketListener(packet => _packetWriter.Write(packet));

            //_dronePacketResolver.AddDjiPacketListener<DjiNetworkPacket<DjiFrame>, DjiFrame>(frame =>  data.Add(frame.DjiPacket.FrameData));
        }

        public void Stuff()
        {
            File.WriteAllBytes(@"C:\Users\thoma\Downloads\test", data.SelectMany(s => s).ToArray());
        }

        private void NetworkPacketReceived(NetworkPacket networkPacket)
        {
            if (networkPacket.Participant == Participant.Drone)
                _dronePacketResolver.Feed(networkPacket);
            else if (networkPacket.Participant == Participant.Operator)
                _operatorPacketResolver.Feed(networkPacket);
            else throw new ArgumentException($"The provided {nameof(Participant)} " +
                $"'{networkPacket.Participant.ToString()}' isn't valid");
        }

        public static DjiContentViewModel Instance => _singleton.Value;

        public DjiPacketSniffer PacketSniffer => _packetSniffer;

        public DjiPacketPCapWriter PacketWriter => _packetWriter;

        public DjiPacketResolver OperatorPacketResolver => _operatorPacketResolver;

        public DjiPacketResolver DronePacketResolver => _dronePacketResolver;

        public bool IsRecording
        {
            get => _isRecording;
            private set => this.RaiseAndSetIfChanged(ref _isRecording, value);
        }

        public async Task OpenSimulation()
        {
            string sourceFile = await FileDialog.OpenDialogAsync(DjiPacketPCapWriter.FILE_EXTENSION);

            if (string.IsNullOrEmpty(sourceFile))
                return;

            DjiSimulationWindow djiSimulationWindow = new DjiSimulationWindow();
            djiSimulationWindow.DataContext = new DjiSimulationWindowViewModel(sourceFile, _packetSniffer);
            djiSimulationWindow.Show(DjiWindow.Instance);
        }

        public async Task StartRecording()
        {
            string targetFile = await FileDialog.SaveDialogAsync(
                DjiPacketPCapWriter.FILE_EXTENSION,
                DjiPacketPCapWriter.DEFAULT_FILE_NAME_FORMAT);

            if (string.IsNullOrEmpty(targetFile))
                return;

            PacketWriter.Enable(targetFile);
            IsRecording = true;
        }

        public void StopRecording()
        {
            PacketWriter.Disable();
            IsRecording = false;
        }
    }
}