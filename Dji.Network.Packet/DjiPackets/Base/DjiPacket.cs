using Dji.Network.Packet.Extensions;
using System;

namespace Dji.Network.Packet.DjiPackets.Base
{
    public abstract class DjiPacket
    {
        #region Payload
        private byte[] _data;
        #endregion

        #region Wifi
        private ushort _size;
        private byte[] _session;

        public ushort Size
        {
            get => _size;
            set => SetValue(ref _size, value);
        }

        public byte[] Session
        {
            get => _session;
            set => SetValue(ref _session, value);
        }
        #endregion
        protected void SetValue<T>(ref T property, T value)
        {
            // no need to drop any cache if the value remains the same.
            if (property == null && value == null) return;
            else if (property != null && value != null && property.Equals(value)) return;

            // as the child did set a new property, our data isn't valid
            // anymore. Thus, we do require to clear our _data cache.
            _data = null;

            // finally assign the new value.
            property = value;
        }

        protected abstract byte[] Build();

        protected abstract bool Build(byte[] data);

        public bool Set(byte[] data, int? delimiter = null)
        {
            // not enough data available for the wifi-header
            if (!delimiter.HasValue && data.Length < 0x0F)
                return false;

            Size = GetPacketSize(data[0..2]);
            Session = data[2..4];

            int dataPacketStart = delimiter ?? GetWifiSize(data);

            // the wifi-header is present at this point.
            // however, the payload might not exist.
            if (data.Length - dataPacketStart > 0)
                return Build(data[dataPacketStart..]);

            return true;
        }

        public byte[] Get(bool includeWifi = false)
        {
            byte[] GetBytes() => _data[(includeWifi ? 0 : 4)..];

            // always try to use the cached value
            if (_data != null) return GetBytes();

            byte[] payload = Build();
            byte[] wifi = new byte[4];

            wifi[0] = (byte)(Size << 8);
            wifi[1] = (byte)(0x80 + (byte)Size);
            wifi[2] = Session[0];
            wifi[3] = Session[1];

            _data = new byte[wifi.Length + payload.Length];
            Array.Copy(wifi, 0, _data, 0, wifi.Length);
            Array.Copy(payload, 0, _data, wifi.Length, payload.Length);

            return GetBytes();
        }

        public static ushort GetPacketSize(byte[] data)
        {
            if (data == null || data.Length != 2)
                throw new ArgumentException($"Parameter {nameof(data)} may not be null or .length != 2");

            byte low = data[0];
            byte high = data[1];

            return (ushort)(((high & 0x0F) << 8) + low);
        }

        public static byte GetWifiSize(byte[] data)
        {
            // not enough data available for the wifi-header
            if (data.Length < 0x0F)
                throw new ArgumentException($"Parameter {nameof(data)} does not provide enough data for a valid wifi-header");

            byte leftParam = (byte)(data[0x0C] << 1);
            byte rightParam = (byte)(data[0x0E] + data[0x0F]);
            
            return rightParam == 0x00 ? (byte)0x14 : (byte)(0x1E + leftParam);
        }
    }
}