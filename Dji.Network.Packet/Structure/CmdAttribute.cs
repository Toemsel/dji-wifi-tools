using System;

namespace Dji.Network.Packet.Structure
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CmdAttribute : Attribute
    {
        public CmdAttribute(ushort data, byte cmdSet, string cmdSetDescription, byte cmd, string cmdDescription) =>
            (Data, CmdSet, CmdSetDescription, Cmd, CmdDescription) = (data, cmdSet, cmdSetDescription, cmd, cmdDescription);

        public ushort Data { get; init; }

        public byte Cmd { get; init; }

        public byte CmdSet { get; init; }

        public string CmdDescription { get; init; }

        public string CmdSetDescription { get; init; }
    }
}
