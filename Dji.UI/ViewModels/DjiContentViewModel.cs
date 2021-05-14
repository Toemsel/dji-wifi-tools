using Dji.Network;
using Dji.UI.Extensions.Filesystem;
using ReactiveUI;
using System;
using System.Threading.Tasks;

namespace Dji.UI.ViewModels
{
    public class DjiContentViewModel : ReactiveObject
    {
        private bool _isRecording;

        private static readonly Lazy<DjiContentViewModel> _singleton = new(() => new());

        private readonly DjiPacketResolver _packetResolver = new();
        private readonly DjiPacketPCapWriter _packetWriter = new();
        private readonly DjiPacketSniffer _packetSniffer = new();

        private DjiContentViewModel()
        {
            // connect the Dji network-related parts
            _packetSniffer.NetworkPacketReceived += (obj, data) => _packetResolver.Feed(data);
            _packetResolver.AddDjiPacketListener(packet => _packetWriter.Write(packet));
        }

        public static DjiContentViewModel Instance => _singleton.Value;

        public DjiPacketSniffer PacketSniffer => _packetSniffer;

        public DjiPacketPCapWriter PacketWriter => _packetWriter;

        public DjiPacketResolver PacketResolver => _packetResolver;

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