using Dji.Network.Packet.DjiPackets;
using Dji.Network.Packet.Extensions;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Dji.Dissector.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private string _raw;
        private string _output;
        private string _cmdSet;
        private string _cmdSetDesc;

        public string Raw
        {
            get => _raw;
            set => this.RaiseAndSetIfChanged(ref _raw, value);
        }

        public string Output
        {
            get => _output;
            set => this.RaiseAndSetIfChanged(ref _output, value);
        }

        public string CmdSet
        {
            get => _cmdSet;
            set => this.RaiseAndSetIfChanged(ref _cmdSet, value);
        }

        public string CmdSetDesc
        {
            get => _cmdSetDesc;
            set => this.RaiseAndSetIfChanged(ref _cmdSetDesc, value);
        }

        private void Parse()
        {
            if (string.IsNullOrEmpty(Raw)) return;

            byte[] data = Raw.FromHexString();

            for(int i = 0; i < data.Length; i++)
            {
                if(data[i] == 0x55)
                {
                    try
                    {
                        DjiCmdPacket cmd = new DjiCmdPacket();
                        if (cmd.Set(data, i))
                        {
                            Trace.TraceInformation($"Cmd @{i} E{i + cmd.DumlSize}: {data[i..(i + cmd.DumlSize)].ToHexString(false, false)}");
                            Trace.TraceInformation($"Wifi-Header: {data[0..i].ToHexString(false, false)}");
                        }
                    }
                    catch { }
                }
            }
        }

        private byte ToByte(string identifer)
        {
            if (!identifer.Contains('x'))
                return BitConverter.GetBytes(Convert.ToUInt16(identifer))[0];
            return HexToByte(identifer.Substring(2));
        }

        private byte HexToByte(string hex) => Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray()[0];

        private string ExtractEnumName(string name)
        {
            // 1. replace all chars != ASCII with a space
            for (int index = 0; index < name.Length; index++)
                if (!IsValidEnumChar(name[index]))
                    name = ReplaceAt(name, index, ' ');

            // 2. Enforce Camel Case
            for (int index = 0; index < name.Length; index++)
                if (name[index] != ' ' && index > 0 && name[index - 1] == ' ')
                    name = ReplaceAt(name, index, name[index].ToString().ToUpper()[0]);

            // 3. Get rid of the spaces
            return name.Replace(" ", "");
        }

        private bool IsValidEnumChar(char c) => (c >= 65 && c <= 90) || (c >= 97 && c <= 122);

        private string ReplaceAt(string value, int index, char newchar)
        {
            if (value.Length <= index)
                return value;
            else
                return string.Concat(value.Select((c, i) => i == index ? newchar : c));
        }

        private string ToHexString(byte data, bool useLeadingZero = true) => ToHexString(new byte[] { data }, useLeadingZero);

        private string ToHexString(byte[] data, bool useLeadingZero = true, bool useSpacing = true)
        {
            StringBuilder hex = new StringBuilder(data.Length * 2);

            for (int index = 0; index < data.Length; index++)
            {
                if (useLeadingZero) hex.Append("0x");
                hex.AppendFormat("{0:x2}", data[index]);
                if (useSpacing && index < data.Length - 1) hex.Append(" ");
            }

            return hex.ToString();
        }
    }
}
