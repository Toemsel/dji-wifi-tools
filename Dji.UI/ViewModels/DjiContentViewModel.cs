using Dji.Camera;
using Dji.Network;
using Dji.Network.Packet;
using Dji.Network.Packet.DjiPackets.Base;
using Dji.UI.Extensions.Filesystem;
using ReactiveUI;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Dji.UI.ViewModels
{
    public class DjiContentViewModel : ReactiveObject
    {
        private bool _isRecording = false;
        private bool _isCameraReady = false;
        private bool _isExportingForHotReplay = false;

        private static readonly Lazy<DjiContentViewModel> _singleton = new(() => new());

        private readonly DjiPacketResolver _operatorPacketResolver = new DjiOperatorPacketResolver();
        private readonly DjiPacketResolver _dronePacketResolver = new DjiDronePacketResolver();
        private readonly DjiPacketPCapWriter _packetWriter = new();
        private readonly DjiPacketSniffer _packetSniffer = new();
        private readonly DjiCamera _camera;

        private DjiContentViewModel()
        {
            // connect the Dji network-related parts
            _packetSniffer.NetworkPacketReceived += (obj, data) => _packetWriter.Write(data);
            _packetSniffer.NetworkPacketReceived += (obj, data) => NetworkPacketReceived(data);

            _operatorPacketResolver.AddDjiPacketListener(packet => _packetWriter.Write(packet));
            _dronePacketResolver.AddDjiPacketListener(packet => _packetWriter.Write(packet));

            // initialize a new DjiCamera which will listen for frame-updates
            _camera = new DjiCamera(_dronePacketResolver as DjiDronePacketResolver);
            _camera.CameraStateChanged += (cam, state) => IsCameraReady = state == CameraState.VideoAvailable;
        }

        private void NetworkPacketReceived(NetworkPacket networkPacket)
        {
            if (networkPacket.Participant == Participant.Drone)
                _dronePacketResolver.Feed(networkPacket);
            else if (networkPacket.Participant == Participant.Operator)
                _operatorPacketResolver.Feed(networkPacket);
            else throw new ArgumentException($"The provided {nameof(Participant)} " +
                $"'{networkPacket.Participant}' isn't valid");
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

        public bool IsCameraReady
        {
            get => _isCameraReady;
            set => this.RaiseAndSetIfChanged(ref _isCameraReady, value);
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

        public async Task ExportCameraFeed()
        {
            string targetFile = await FileDialog.SaveDialogAsync(DjiCamera.DEFAULT_VIDEO_FORMAT, 
                DjiPacketPCapWriter.DEFAULT_FILE_NAME_FORMAT);

            if (string.IsNullOrEmpty(targetFile))
                return;

            await _camera.ExportVideo(targetFile, DjiCamera.DEFAULT_VIDEO_FORMAT);
        }

        public void PlayCameraFeed(object sender, object param) => _ = PlayCameraFeed();

        public async Task PlayCameraFeed()
        {
            // avoid parsing the camera-feed several times concurrently
            if (_isExportingForHotReplay) return;

            _isExportingForHotReplay = true;

            string targetFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                $"djiCameraPlaybackTemp.{DjiCamera.DEFAULT_VIDEO_FORMAT}");

            if (await _camera.ExportVideo(targetFile, DjiCamera.DEFAULT_VIDEO_FORMAT))
                VideoPlayer.PlayVideo(targetFile);

            // upon an exception the feed won't work anymore. However,
            // a exception indicates that libvlc isn't available or has issues.
            // thus, any future conversion will fail as well. Hence, it is
            // perfectly fine that we don't re-enable the play-camera-feed
            _isExportingForHotReplay = false;
        }
    }
}