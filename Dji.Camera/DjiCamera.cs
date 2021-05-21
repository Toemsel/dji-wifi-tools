using Dji.Network;
using Dji.Network.Packet;
using Dji.Network.Packet.DjiPackets.Drone;
using Dji.Network.Packet.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WeakEvent;

namespace Dji.Camera
{
    public class DjiCamera : IDisposable
    {
        private static string BUFFER_PREFIX = $"_{nameof(DjiCamera)}";
        public const string DEFAULT_VIDEO_FORMAT = "avi";

        private readonly WeakEventSource<CameraState> _cameraStateChanged = new WeakEventSource<CameraState>();
        private readonly DjiDronePacketResolver _dronePackets;

        private string _frameBuffer = string.Empty;
        private byte[] _cameraSession = null;

        public DjiCamera(DjiDronePacketResolver dronePackets)
        {
            // remember the frame-input instance
            _dronePackets = dronePackets;

            // initialize required file-buffers
            InitializeBuffers();

            // listen to frame-updates
            _dronePackets.AddDjiPacketListener<DjiNetworkPacket<DjiFramePacket>, DjiFramePacket>(FrameUpdate);
        }

        private string FilePath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) 
            ?? throw new NotSupportedException($"App location not known on OS {RuntimeInformation.OSDescription}");

        public event EventHandler<CameraState> CameraStateChanged
        {
            add { _cameraStateChanged.Subscribe(value); }
            remove { _cameraStateChanged.Unsubscribe(value); }
        }

        private void FrameUpdate(DjiNetworkPacket<DjiFramePacket> framePacket)
        {
            // a framePacket may origin from different sources. Hence, we do require
            // to check whether this frame will break our buffers
            if (_cameraSession == null || _cameraSession.Length != 2 ||
                (_cameraSession[0] != framePacket.DjiPacket.Session[0] ||
                _cameraSession[1] != framePacket.DjiPacket.Session[1]))
                OnSessionUpdate(new byte[] { framePacket.DjiPacket.Session[0], framePacket.DjiPacket.Session[1] });

            using (Stream fileStream = new FileStream(_frameBuffer, FileMode.Append))
                fileStream.Write(framePacket.DjiPacket.FrameData);

            _cameraStateChanged?.Raise(this, CameraState.VideoAvailable);
        }

        private void OnSessionUpdate(byte[] session)
        {
            Trace.TraceInformation($"{nameof(DjiCamera)}: new camera-session detected. [{session[0].ToHexString()} {session[1].ToHexString()}]");

            _cameraSession = new byte[2];
            _cameraSession[0] = session[0];
            _cameraSession[1] = session[1];

            InitializeBuffers();

            _cameraStateChanged?.Raise(this, CameraState.VideoSourceReset);
        }

        private void InitializeBuffers()
        {
            // before we are going to init any buffer, we
            // first cleanup our local filesystem from
            // 'old' buffer values which still may be arround
            string[] bufferFiles = Directory.GetFiles(FilePath).Where(f => f.Contains(BUFFER_PREFIX)).ToArray();

            try
            {
                foreach (string file in bufferFiles)
                    if (File.Exists(file))
                        File.Delete(file);
            }
            catch(Exception exception)
            {
                Trace.TraceWarning($"{nameof(DjiCamera)}: Cleanup failure. Temp files may be remain on your drive", exception);
            }

            _frameBuffer = Path.Combine(FilePath, $"{BUFFER_PREFIX}{Guid.NewGuid():N}");
        }

        public async Task<byte[]> GetVideo()
        {
            if (string.IsNullOrEmpty(_frameBuffer)) return null;
            else if (!File.Exists(_frameBuffer)) return null;

            string targetFile = $"{_frameBuffer}{Guid.NewGuid():N}.{DEFAULT_VIDEO_FORMAT}";

            if (!await ExportVideo(targetFile))
                Trace.TraceWarning($"Failed to convert camera-feed to video");

            if (!File.Exists(targetFile)) return null;

            // obtain the actual file-data
            byte[] data = File.ReadAllBytes(targetFile);
            // get rid of the temp file
            File.Delete(targetFile);
            // either return a valid video, or, nothing
            return data != null && data.Length > 0 ? data : null;
        }

        public async Task<bool> ExportVideo(string targetFile, string format = DEFAULT_VIDEO_FORMAT)
        {
            if (string.IsNullOrEmpty(targetFile) || string.IsNullOrEmpty(format))
                throw new ArgumentException($"The {nameof(targetFile)} nor the {nameof(format)} may be null or empty");

            // ensure that the format has a valid format
            format = format.Replace(".", "");
            // ensure that the targetFile has a valid extension
            targetFile = targetFile.RemoveFileExtension();
            targetFile = $"{targetFile}.{format}";
            // actually export the video to the provided target-file
            return await ExportVideo(targetFile);
        }

        private async Task<bool> ExportVideo(string targetFile) => await FFMpegCore.FFMpegArguments.FromFileInput(_frameBuffer).OutputToFile(targetFile).ProcessAsynchronously(false);

        public void Dispose() => InitializeBuffers();
    }
}