using Dji.Network.Packet;
using Dji.Network.Packet.DjiPackets;
using PacketDotNet;
using SharpPcap;
using System;
using System.IO;

namespace Dji.Network
{
    public class DjiPacketPCapWriter : IDisposable
    {
        public const string FILE_EXTENSION = "pcap";
        public const string DEFAULT_FILE_NAME_FORMAT = "HH-mm-ss-ffff";

        private BinaryWriter _udpBinaryWriter;
        private BinaryWriter _dumlBinaryWriter;

        public void Enable(string filename)
        {
            string udpFile = ObtainFilename(filename, "_udp", FILE_EXTENSION);
            string dumlFile = ObtainFilename(filename, "_duml", FILE_EXTENSION);

            // ensure that we start from scratch
            if (_udpBinaryWriter != null || _dumlBinaryWriter != null) Dispose();

            _udpBinaryWriter = new BinaryWriter(File.OpenWrite(udpFile));
            _udpBinaryWriter.Write(0xa1b2c3d4);
            _udpBinaryWriter.Write((ushort)2);
            _udpBinaryWriter.Write((ushort)4);
            _udpBinaryWriter.Write(0);
            _udpBinaryWriter.Write((uint)0);
            _udpBinaryWriter.Write((uint)65535);
            _udpBinaryWriter.Write((uint)LinkLayers.Ethernet);
            _udpBinaryWriter.Flush();

            _dumlBinaryWriter = new BinaryWriter(File.OpenWrite(dumlFile));
            _dumlBinaryWriter.Write(0xa1b2c3d4);
            _dumlBinaryWriter.Write((ushort)2);
            _dumlBinaryWriter.Write((ushort)4);
            _dumlBinaryWriter.Write(0);
            _dumlBinaryWriter.Write((uint)0);
            _dumlBinaryWriter.Write((uint)65535);
            _dumlBinaryWriter.Write((uint)150);
            _dumlBinaryWriter.Flush();
        }

        private string ObtainFilename(string startValue, string endValue, string extension)
        {
            if (string.IsNullOrEmpty(endValue) || string.IsNullOrEmpty(extension))
                throw new ArgumentException($"Parameter {nameof(endValue)} and {nameof(extension)} may not be null or empty.");

            // Make sure that we do have a valid file-name
            string filename = string.IsNullOrEmpty(startValue) ? DateTime.Now.ToString(DEFAULT_FILE_NAME_FORMAT) : startValue;

            // if the extension is already present, get rid of it
            var fileInfo = new FileInfo(filename);
            if (!string.IsNullOrEmpty(fileInfo.Extension))
                filename = Path.Combine(fileInfo.DirectoryName, Path.GetFileNameWithoutExtension(fileInfo.Name));

            // concat the end value to the filename
            filename = $"{filename}{endValue}";

            // add the final file-extension
            filename = $"{filename}.{extension}";

            // add the relative path
            filename = Path.Combine(Directory.GetCurrentDirectory(), filename);

            // if the file already exists, repeat
            if (File.Exists(filename))
                return ObtainFilename(string.Empty, endValue, extension);

            return filename;
        }

        public void Disable()
        {
            _udpBinaryWriter?.Flush();
            _udpBinaryWriter?.Dispose();
            _udpBinaryWriter = null;
            _dumlBinaryWriter?.Flush();
            _dumlBinaryWriter?.Dispose();
            _dumlBinaryWriter = null;
        }

        public void Write(NetworkPacket networkPacket)
        {
            if (_udpBinaryWriter != null)
                Write(_udpBinaryWriter, networkPacket.RawCapture.Timeval, networkPacket.RawCapture.Data);

            _udpBinaryWriter?.Flush();
        }

        public void Write(DjiNetworkPacket djiNetworkPacket)
        {
            if (_dumlBinaryWriter != null && djiNetworkPacket.DjiPacket is DjiDUMLPacket && djiNetworkPacket.DjiPacket.Get(false).Length > 0)
                Write(_dumlBinaryWriter, djiNetworkPacket.RawCapture.Timeval, djiNetworkPacket.DjiPacket.Get(false));

            _dumlBinaryWriter?.Flush();
        }

        private void Write(BinaryWriter binaryWriter, PosixTimeval posix, byte[] data)
        {
            binaryWriter.Write(Convert.ToUInt32(posix.Seconds));
            binaryWriter.Write(Convert.ToUInt32(posix.MicroSeconds));
            binaryWriter.Write(Convert.ToUInt32(data.Length));
            binaryWriter.Write(Convert.ToUInt32(data.Length));
            binaryWriter.Write(data);
        }

        public void Dispose() => Disable();
    }
}