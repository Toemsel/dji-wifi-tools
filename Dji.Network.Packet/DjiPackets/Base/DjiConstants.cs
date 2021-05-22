using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Dji.Network.Packet.DjiPackets.Base
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CmdAttribute : Attribute
    {

        private static readonly Dictionary<Cmd, CmdAttribute> _commandAttributes = new();

        public CmdAttribute(ushort data, byte cmdSet, string cmdSetDescription, byte cmd, string cmdDescription) =>
            (Data, CmdSet, CmdSetDescription, Cmd, CmdDescription) = (data, cmdSet, cmdSetDescription, cmd, cmdDescription);

        public ushort Data { get; init; }

        public byte Cmd { get; init; }

        public byte CmdSet { get; init; }

        public string CmdDescription { get; init; }

        public string CmdSetDescription { get; init; }

        static CmdAttribute()
        {
            foreach (var enumVal in Enum.GetValues(typeof(Cmd)))
            {
                var memberInfo = typeof(Cmd).GetMember(enumVal.ToString());
                var enumValMemberInfo = memberInfo.FirstOrDefault(m => m.DeclaringType == typeof(Cmd));
                var enumAttribute = enumValMemberInfo.GetCustomAttributes(typeof(CmdAttribute), false);

                if (enumAttribute == null || enumAttribute.Length <= 0)
                    Trace.TraceWarning($"Enum {typeof(Cmd).Name} misses {nameof(CmdAttribute)}");

                var attribute = enumAttribute?.FirstOrDefault() as CmdAttribute;

                if (attribute != null)
                    _commandAttributes.Add((Cmd)enumVal, attribute);
            }
        }

        public static CmdAttribute TryGetAttribute(Cmd cmd) => _commandAttributes.ContainsKey(cmd) ? _commandAttributes[cmd] : null;
    }

    public enum WhType
    {
        OperatorEmpty = 0x00,
        DroneCmd1 = 0x01,
        DroneImgFrame = 0x02,
        DroneCmd2 = 0x03,
        OperatorCmd1 = 0x04,
        OperatorCmd2 = 0x05,
        OperatorCmd3 = 0x06        
    }

    // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L59
    public enum Comms
    {
        Request = 0x00,
        Response = 0x01
    }

    // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L42
    public enum Ack
    {
        NoAck = 0x00,
        BeforeExe = 0x01,
        AfterExe = 0x02
    }

    // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L48
    public enum Encryption
    {
        None = 0x00,
        AES128 = 0x01,
        SelfDef = 0x02,
        XOR = 0x03,
        DES56 = 0x04,
        DES112 = 0x05,
        AES192 = 0x06,
        AES256 = 0x07
    }

    // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L8
    public enum Transceiver
    {
        Invalid = 0x00,
        Camera = 0x01,
        App = 0x02,
        FC = 0x03,
        Gimbal = 0x04,
        CenterBoard = 0x05,
        RemoteControl = 0x06,
        DroneWifi = 0x07,
        DroneDM36X = 0x08,
        DroneHDMCU = 0x09,
        PC = 0x0A,
        Battery = 0x0B,
        ESC = 0x0C,
        OperatorDM36X = 0x0D,
        OperatorHDMCU = 0x0E,
        DroneSerial = 0x0F,
        OperatorSerial = 0x10,
        Monocular = 0x11,
        Binocular = 0x12,
        DroneFPGA = 0x13,
        OperatorFPGA = 0x14,
        Simulator = 0x15,
        BaseStation = 0x16,
        DroneComputingPlatform = 0x17,
        RCBattery = 0x18,
        IMU = 0x19,
        GPS = 0x1A,
        OperatorWifi = 0x1B,
        SigCVT = 0x1C,
        PMU = 0x1D,
        Unknown = 0x1E,
        Last = 0x1F
    }

    // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-flyc.lua#L210
    public enum FlightStatusArea
    {
        None = 0x00,
        NearLimit = 0x01,
        InHalfLimit = 0x02,
        InSlowDownArea = 0x03,
        InnerLimit = 0x04,
        InnerUnLimit = 0x05,
        Other = 0x06
    }

    // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-flyc.lua#L220
    public enum FlightStatus
    {
        None = 0x00,
        ExitLanding = 0x01,
        Collision = 0x02,
        StartLanding = 0x03,
        StopMotor = 0x04,
        Other = 0x64
    }

    // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L64
    public enum Cmd
    {
        // [0] https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L494
        // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-general.lua#L9
        [Cmd(0, 0x00, "General", 0x00, "Ping")]
        General_Ping = 0x0000,
        [Cmd(1, 0x00, "General", 0x01, "Version Inquiry")]
        General_VersionInquiry = 0x0001,
        [Cmd(2, 0x00, "General", 0x02, "Push Param Set")]
        General_PushParamSet = 0x0002,
        [Cmd(3, 0x00, "General", 0x03, "Push Param Get")]
        General_PushParamGet = 0x0003,
        [Cmd(4, 0x00, "General", 0x04, "Push Param Start")]
        General_PushParamStart = 0x0004,
        [Cmd(5, 0x00, "General", 0x05, "Multi Param Set")]
        General_MultiParamSet = 0x0005,
        [Cmd(6, 0x00, "General", 0x06, "Multi Param Get")]
        General_MultiParamGet = 0x0006,
        [Cmd(7, 0x00, "General", 0x07, "Enter Loader")]
        General_EnterLoader = 0x0007,
        [Cmd(8, 0x00, "General", 0x08, "Update Confirm")]
        General_UpdateConfirm = 0x0008,
        [Cmd(9, 0x00, "General", 0x09, "Update Transmit")]
        General_UpdateTransmit = 0x0009,
        [Cmd(10, 0x00, "General", 0x0a, "Update Finish")]
        General_UpdateFinish = 0x000a,
        [Cmd(11, 0x00, "General", 0x0b, "Reboot Chip")]
        General_RebootChip = 0x000b,
        [Cmd(12, 0x00, "General", 0x0c, "Get Device State")]
        General_GetDeviceState = 0x000c,
        [Cmd(13, 0x00, "General", 0x0d, "Set Device Version")]
        General_SetDeviceVersion = 0x000d,
        [Cmd(14, 0x00, "General", 0x0e, "Heartbeat/Log Message")]
        General_HeartbeatLogMessage = 0x000e,
        [Cmd(15, 0x00, "General", 0x0f, "Upgrade Self Request")]
        General_UpgradeSelfRequest = 0x000f,
        [Cmd(16, 0x00, "General", 0x10, "Set SDK Std Msgs Frequency")]
        General_SetSDKStdMsgsFrequency = 0x0010,
        [Cmd(32, 0x00, "General", 0x20, "File List")]
        General_FileList = 0x0020,
        [Cmd(33, 0x00, "General", 0x21, "File Info")]
        General_FileInfo = 0x0021,
        [Cmd(34, 0x00, "General", 0x22, "File Send")]
        General_FileSend = 0x0022,
        [Cmd(35, 0x00, "General", 0x23, "File Receive")]
        General_FileReceive = 0x0023,
        [Cmd(36, 0x00, "General", 0x24, "File Sending")]
        General_FileSending = 0x0024,
        [Cmd(37, 0x00, "General", 0x25, "File Segment Err")]
        General_FileSegmentErr = 0x0025,
        [Cmd(38, 0x00, "General", 0x26, "FileTrans App 2 Camera")]
        General_FileTransAppCamera = 0x0026,
        [Cmd(39, 0x00, "General", 0x27, "FileTrans Camera 2 App")]
        General_FileTransCameraApp = 0x0027,
        [Cmd(40, 0x00, "General", 0x28, "FileTrans Delete")]
        General_FileTransDelete = 0x0028,
        [Cmd(42, 0x00, "General", 0x2a, "FileTrans General Trans")]
        General_FileTransGeneralTrans = 0x002a,
        [Cmd(48, 0x00, "General", 0x30, "Encrypt Config")]
        General_EncryptConfig = 0x0030,
        [Cmd(50, 0x00, "General", 0x32, "Activate Config")]
        General_ActivateConfig = 0x0032,
        [Cmd(51, 0x00, "General", 0x33, "MFi Cert")]
        General_MFiCert = 0x0033,
        [Cmd(52, 0x00, "General", 0x34, "Safe Communication")]
        General_SafeCommunication = 0x0034,
        [Cmd(64, 0x00, "General", 0x40, "Fw Update Desc Push")]
        General_FwUpdateDescPush = 0x0040,
        [Cmd(65, 0x00, "General", 0x41, "Fw Update Push Control")]
        General_FwUpdatePushControl = 0x0041,
        [Cmd(66, 0x00, "General", 0x42, "Fw Upgrade Push Status")]
        General_FwUpgradePushStatus = 0x0042,
        [Cmd(67, 0x00, "General", 0x43, "Fw Upgrade Finish")]
        General_FwUpgradeFinish = 0x0043,
        [Cmd(69, 0x00, "General", 0x45, "Sleep Control")]
        General_SleepControl = 0x0045,
        [Cmd(70, 0x00, "General", 0x46, "Shutdown Notification")]
        General_ShutdownNotification = 0x0046,
        [Cmd(71, 0x00, "General", 0x47, "Power State")]
        General_PowerState = 0x0047,
        [Cmd(72, 0x00, "General", 0x48, "LED Control")]
        General_LEDControl = 0x0048,
        [Cmd(74, 0x00, "General", 0x4a, "Set Date/Time")]
        General_SetDateTime = 0x004a,
        [Cmd(75, 0x00, "General", 0x4b, "Get Date/Time")]
        General_GetDateTime = 0x004b,
        [Cmd(76, 0x00, "General", 0x4c, "Get Module Sys Status")]
        General_GetModuleSysStatus = 0x004c,
        [Cmd(77, 0x00, "General", 0x4d, "Set RT")]
        General_SetRT = 0x004d,
        [Cmd(78, 0x00, "General", 0x4e, "Get RT")]
        General_GetRT = 0x004e,
        [Cmd(79, 0x00, "General", 0x4f, "Get Cfg File")]
        General_GetCfgFile = 0x004f,
        [Cmd(80, 0x00, "General", 0x50, "Set Serial Number")]
        General_SetSerialNumber = 0x0050,
        [Cmd(81, 0x00, "General", 0x51, "Get Serial Number")]
        General_GetSerialNumber = 0x0051,
        [Cmd(82, 0x00, "General", 0x52, "Set Gps Push Config")]
        General_SetGpsPushConfig = 0x0052,
        [Cmd(83, 0x00, "General", 0x53, "Push Gps Info")]
        General_PushGpsInfo = 0x0053,
        [Cmd(84, 0x00, "General", 0x54, "Get Temperature Info")]
        General_GetTemperatureInfo = 0x0054,
        [Cmd(85, 0x00, "General", 0x55, "Get Alive Time")]
        General_GetAliveTime = 0x0055,
        [Cmd(86, 0x00, "General", 0x56, "Over Temperature")]
        General_OverTemperature = 0x0056,
        [Cmd(87, 0x00, "General", 0x57, "Send Network Info")]
        General_SendNetworkInfo = 0x0057,
        [Cmd(88, 0x00, "General", 0x58, "Time Sync")]
        General_TimeSync = 0x0058,
        [Cmd(89, 0x00, "General", 0x59, "Test Mode")]
        General_TestMode = 0x0059,
        [Cmd(90, 0x00, "General", 0x5a, "Play Sound")]
        General_PlaySound = 0x005a,
        [Cmd(92, 0x00, "General", 0x5c, "UAV Fly Info")]
        General_UAVFlyInfo = 0x005c,
        [Cmd(96, 0x00, "General", 0x60, "Auto Test Info")]
        General_AutoTestInfo = 0x0060,
        [Cmd(97, 0x00, "General", 0x61, "Set Product Newest Ver")]
        General_SetProductNewestVer = 0x0061,
        [Cmd(98, 0x00, "General", 0x62, "Get Product Newest Ver")]
        General_GetProductNewestVer = 0x0062,
        [Cmd(239, 0x00, "General", 0xef, "Send Reserved Key")]
        General_SendReservedKey = 0x00ef,
        [Cmd(240, 0x00, "General", 0xf0, "Log Push")]
        General_LogPush = 0x00f0,
        [Cmd(241, 0x00, "General", 0xf1, "Component Self Test State")]
        General_ComponentSelfTestState = 0x00f1,
        [Cmd(242, 0x00, "General", 0xf2, "Log Control Global")]
        General_LogControlGlobal = 0x00f2,
        [Cmd(243, 0x00, "General", 0xf3, "Log Control Module")]
        General_LogControlModule = 0x00f3,
        [Cmd(244, 0x00, "General", 0xf4, "Test Start")]
        General_TestStart = 0x00f4,
        [Cmd(245, 0x00, "General", 0xf5, "Test Stop")]
        General_TestStop = 0x00f5,
        [Cmd(246, 0x00, "General", 0xf6, "Test Query Result")]
        General_TestQueryResult = 0x00f6,
        [Cmd(247, 0x00, "General", 0xf7, "Push Test Result")]
        General_PushTestResult = 0x00f7,
        [Cmd(248, 0x00, "General", 0xf8, "Get Metadata")]
        General_GetMetadata = 0x00f8,
        [Cmd(250, 0x00, "General", 0xfa, "Log Control")]
        General_LogControl = 0x00fa,
        [Cmd(251, 0x00, "General", 0xfb, "Selftest State")]
        General_SelftestState = 0x00fb,
        [Cmd(252, 0x00, "General", 0xfc, "Selftest State Count")]
        General_SelftestStateCount = 0x00fc,
        [Cmd(253, 0x00, "General", 0xfd, "Dump Frame Buffer")]
        General_DumpFrameBuffer = 0x00fd,
        [Cmd(254, 0x00, "General", 0xfe, "Self Define")]
        General_SelfDefine = 0x00fe,
        [Cmd(255, 0x00, "General", 0xff, "Query Device Info")]
        General_QueryDeviceInfo = 0x00ff,

        // [1] https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L494
        // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L95
        [Cmd(256, 0x01, "Special", 0x00, "Sdk Ctrl Mode Open/Close Nav")]
        Special_SdkCtrlModeOpenCloseNav = 0x0100,
        [Cmd(257, 0x01, "Special", 0x01, "Old Special App Control")]
        Special_OldSpecialAppControl = 0x0101,
        [Cmd(258, 0x01, "Special", 0x02, "Old Special Remote Control")]
        Special_OldSpecialRemoteControl = 0x0102,
        [Cmd(259, 0x01, "Special", 0x03, "New Special App Control")]
        Special_NewSpecialAppControl = 0x0103,
        [Cmd(260, 0x01, "Special", 0x04, "New Special Remote Control")]
        Special_NewSpecialRemoteControl = 0x0104,
        [Cmd(261, 0x01, "Special", 0x05, "SDK Ctrl Mode Arm/Disarm")]
        Special_SDKCtrlModeArmDisarm = 0x0105,
        [Cmd(282, 0x01, "Special", 0x1a, "SDK Ctrl Gimbal Speed Ctrl")]
        Special_SDKCtrlGimbalSpeedCtrl = 0x011a,
        [Cmd(283, 0x01, "Special", 0x1b, "SDK Ctrl Gimbal Angle Ctrl")]
        Special_SDKCtrlGimbalAngleCtrl = 0x011b,
        [Cmd(288, 0x01, "Special", 0x20, "SDK Ctrl Camera Shot Ctrl")]
        Special_SDKCtrlCameraShotCtrl = 0x0120,
        [Cmd(289, 0x01, "Special", 0x21, "SDK Ctrl Camera Start Video Ctrl")]
        Special_SDKCtrlCameraStartVideoCtrl = 0x0121,
        [Cmd(290, 0x01, "Special", 0x22, "SDK Ctrl Camera Stop Video Ctrl")]
        Special_SDKCtrlCameraStopVideoCtrl = 0x0122,
        [Cmd(511, 0x01, "Special", 0xff, "UAV Loopback")]
        Special_UAVLoopback = 0x01ff,

        // [2] https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L494
        // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-camera.lua#L9
        [Cmd(513, 0x02, "Camera", 0x01, "Do Capture Photo")]
        Camera_DoCapturePhoto = 0x0201,
        [Cmd(514, 0x02, "Camera", 0x02, "Do Record")]
        Camera_DoRecord = 0x0202,
        [Cmd(515, 0x02, "Camera", 0x03, "HeartBeat")]
        Camera_HeartBeat = 0x0203,
        [Cmd(516, 0x02, "Camera", 0x04, "Set Usb Switch")]
        Camera_SetUsbSwitch = 0x0204,
        [Cmd(517, 0x02, "Camera", 0x05, "Virtual Key Send")]
        Camera_VirtualKeySend = 0x0205,
        [Cmd(518, 0x02, "Camera", 0x06, "Get Usb Switch")]
        Camera_GetUsbSwitch = 0x0206,
        [Cmd(528, 0x02, "Camera", 0x10, "Camera Work Mode Set")]
        Camera_CameraWorkModeSet = 0x0210,
        [Cmd(529, 0x02, "Camera", 0x11, "Camera Work Mode Get")]
        Camera_CameraWorkModeGet = 0x0211,
        [Cmd(530, 0x02, "Camera", 0x12, "Photo Format Set")]
        Camera_PhotoFormatSet = 0x0212,
        [Cmd(531, 0x02, "Camera", 0x13, "Photo Format Get")]
        Camera_PhotoFormatGet = 0x0213,
        [Cmd(532, 0x02, "Camera", 0x14, "Photo Quality Set")]
        Camera_PhotoQualitySet = 0x0214,
        [Cmd(533, 0x02, "Camera", 0x15, "Photo Quality Get")]
        Camera_PhotoQualityGet = 0x0215,
        [Cmd(534, 0x02, "Camera", 0x16, "Photo Storage Fmt Set")]
        Camera_PhotoStorageFmtSet = 0x0216,
        [Cmd(535, 0x02, "Camera", 0x17, "Photo Storage Fmt Get")]
        Camera_PhotoStorageFmtGet = 0x0217,
        [Cmd(536, 0x02, "Camera", 0x18, "Video Format Set")]
        Camera_VideoFormatSet = 0x0218,
        [Cmd(537, 0x02, "Camera", 0x19, "Video Format Get")]
        Camera_VideoFormatGet = 0x0219,
        [Cmd(538, 0x02, "Camera", 0x1a, "Video Quality Set")]
        Camera_VideoQualitySet = 0x021a,
        [Cmd(539, 0x02, "Camera", 0x1b, "Video Quality Get")]
        Camera_VideoQualityGet = 0x021b,
        [Cmd(540, 0x02, "Camera", 0x1c, "Video Storage Fmt Set")]
        Camera_VideoStorageFmtSet = 0x021c,
        [Cmd(541, 0x02, "Camera", 0x1d, "Video Storage Fmt Get")]
        Camera_VideoStorageFmtGet = 0x021d,
        [Cmd(542, 0x02, "Camera", 0x1e, "Exposure Mode Set")]
        Camera_ExposureModeSet = 0x021e,
        [Cmd(543, 0x02, "Camera", 0x1f, "Exposure Mode Get")]
        Camera_ExposureModeGet = 0x021f,
        [Cmd(544, 0x02, "Camera", 0x20, "Scene Mode Set")]
        Camera_SceneModeSet = 0x0220,
        [Cmd(545, 0x02, "Camera", 0x21, "Scene Mode Get")]
        Camera_SceneModeGet = 0x0221,
        [Cmd(546, 0x02, "Camera", 0x22, "AE Meter Set")]
        Camera_AEMeterSet = 0x0222,
        [Cmd(547, 0x02, "Camera", 0x23, "AE Meter Get")]
        Camera_AEMeterGet = 0x0223,
        [Cmd(548, 0x02, "Camera", 0x24, "Focus Mode Set")]
        Camera_FocusModeSet = 0x0224,
        [Cmd(549, 0x02, "Camera", 0x25, "Focus Mode Get")]
        Camera_FocusModeGet = 0x0225,
        [Cmd(550, 0x02, "Camera", 0x26, "Aperture Size Set")]
        Camera_ApertureSizeSet = 0x0226,
        [Cmd(551, 0x02, "Camera", 0x27, "Aperture Size Get")]
        Camera_ApertureSizeGet = 0x0227,
        [Cmd(552, 0x02, "Camera", 0x28, "Shutter Speed Set")]
        Camera_ShutterSpeedSet = 0x0228,
        [Cmd(553, 0x02, "Camera", 0x29, "Shutter Speed Get")]
        Camera_ShutterSpeedGet = 0x0229,
        [Cmd(554, 0x02, "Camera", 0x2a, "ISO Set")]
        Camera_ISOSet = 0x022a,
        [Cmd(555, 0x02, "Camera", 0x2b, "ISO Get")]
        Camera_ISOGet = 0x022b,
        [Cmd(556, 0x02, "Camera", 0x2c, "White Balance Env Set")]
        Camera_WhiteBalanceEnvSet = 0x022c,
        [Cmd(557, 0x02, "Camera", 0x2d, "White Balance Env Get")]
        Camera_WhiteBalanceEnvGet = 0x022d,
        [Cmd(558, 0x02, "Camera", 0x2e, "Exposition Bias Set")]
        Camera_ExpositionBiasSet = 0x022e,
        [Cmd(559, 0x02, "Camera", 0x2f, "Exposition Bias Get")]
        Camera_ExpositionBiasGet = 0x022f,
        [Cmd(560, 0x02, "Camera", 0x30, "Focus Region Set")]
        Camera_FocusRegionSet = 0x0230,
        [Cmd(561, 0x02, "Camera", 0x31, "Focus Region Get")]
        Camera_FocusRegionGet = 0x0231,
        [Cmd(562, 0x02, "Camera", 0x32, "AE Meter Region Set")]
        Camera_AEMeterRegionSet = 0x0232,
        [Cmd(563, 0x02, "Camera", 0x33, "AE Meter Region Get")]
        Camera_AEMeterRegionGet = 0x0233,
        [Cmd(564, 0x02, "Camera", 0x34, "Zoom Param Set")]
        Camera_ZoomParamSet = 0x0234,
        [Cmd(565, 0x02, "Camera", 0x35, "Zoom Param Get")]
        Camera_ZoomParamGet = 0x0235,
        [Cmd(566, 0x02, "Camera", 0x36, "Flash Mode Set")]
        Camera_FlashModeSet = 0x0236,
        [Cmd(567, 0x02, "Camera", 0x37, "Flash Mode Get")]
        Camera_FlashModeGet = 0x0237,
        [Cmd(568, 0x02, "Camera", 0x38, "Sharpeness Set")]
        Camera_SharpenessSet = 0x0238,
        [Cmd(569, 0x02, "Camera", 0x39, "Sharpeness Get")]
        Camera_SharpenessGet = 0x0239,
        [Cmd(570, 0x02, "Camera", 0x3a, "Contrast Set")]
        Camera_ContrastSet = 0x023a,
        [Cmd(571, 0x02, "Camera", 0x3b, "Contrast Get")]
        Camera_ContrastGet = 0x023b,
        [Cmd(572, 0x02, "Camera", 0x3c, "Saturation Set")]
        Camera_SaturationSet = 0x023c,
        [Cmd(573, 0x02, "Camera", 0x3d, "Saturation Get")]
        Camera_SaturationGet = 0x023d,
        [Cmd(574, 0x02, "Camera", 0x3e, "Hue Set")]
        Camera_HueSet = 0x023e,
        [Cmd(575, 0x02, "Camera", 0x3f, "Hue Get")]
        Camera_HueGet = 0x023f,
        [Cmd(576, 0x02, "Camera", 0x40, "Face Detect Set")]
        Camera_FaceDetectSet = 0x0240,
        [Cmd(577, 0x02, "Camera", 0x41, "Face Detect Get")]
        Camera_FaceDetectGet = 0x0241,
        [Cmd(578, 0x02, "Camera", 0x42, "Digital Effect Set")]
        Camera_DigitalEffectSet = 0x0242,
        [Cmd(579, 0x02, "Camera", 0x43, "Digital Effect Get")]
        Camera_DigitalEffectGet = 0x0243,
        [Cmd(580, 0x02, "Camera", 0x44, "Digital Denoise Set")]
        Camera_DigitalDenoiseSet = 0x0244,
        [Cmd(581, 0x02, "Camera", 0x45, "Digital Denoise Get")]
        Camera_DigitalDenoiseGet = 0x0245,
        [Cmd(582, 0x02, "Camera", 0x46, "Anti Flicker Set")]
        Camera_AntiFlickerSet = 0x0246,
        [Cmd(583, 0x02, "Camera", 0x47, "Anti Flicker Get")]
        Camera_AntiFlickerGet = 0x0247,
        [Cmd(584, 0x02, "Camera", 0x48, "Multi Cap Param Set")]
        Camera_MultiCapParamSet = 0x0248,
        [Cmd(585, 0x02, "Camera", 0x49, "Multi Cap Param Get")]
        Camera_MultiCapParamGet = 0x0249,
        [Cmd(586, 0x02, "Camera", 0x4a, "Conti Cap Param Set")]
        Camera_ContiCapParamSet = 0x024a,
        [Cmd(587, 0x02, "Camera", 0x4b, "Conti Cap Param Get")]
        Camera_ContiCapParamGet = 0x024b,
        [Cmd(588, 0x02, "Camera", 0x4c, "Hdmi Output Param Set")]
        Camera_HdmiOutputParamSet = 0x024c,
        [Cmd(589, 0x02, "Camera", 0x4d, "Hdmi Output Param Get")]
        Camera_HdmiOutputParamGet = 0x024d,
        [Cmd(590, 0x02, "Camera", 0x4e, "Quickview Param Set")]
        Camera_QuickviewParamSet = 0x024e,
        [Cmd(591, 0x02, "Camera", 0x4f, "Quickview Param Get")]
        Camera_QuickviewParamGet = 0x024f,
        [Cmd(592, 0x02, "Camera", 0x50, "OSD Param Set")]
        Camera_OSDParamSet = 0x0250,
        [Cmd(593, 0x02, "Camera", 0x51, "OSD Param Get")]
        Camera_OSDParamGet = 0x0251,
        [Cmd(594, 0x02, "Camera", 0x52, "Preview OSD Param Set")]
        Camera_PreviewOSDParamSet = 0x0252,
        [Cmd(595, 0x02, "Camera", 0x53, "Preview OSD Param Get")]
        Camera_PreviewOSDParamGet = 0x0253,
        [Cmd(596, 0x02, "Camera", 0x54, "Camera Date/Time Set")]
        Camera_CameraDateTimeSet = 0x0254,
        [Cmd(597, 0x02, "Camera", 0x55, "Camera Date/Time Get")]
        Camera_CameraDateTimeGet = 0x0255,
        [Cmd(598, 0x02, "Camera", 0x56, "Language Param Set")]
        Camera_LanguageParamSet = 0x0256,
        [Cmd(599, 0x02, "Camera", 0x57, "Language Param Get")]
        Camera_LanguageParamGet = 0x0257,
        [Cmd(600, 0x02, "Camera", 0x58, "Camera GPS Set")]
        Camera_CameraGPSSet = 0x0258,
        [Cmd(601, 0x02, "Camera", 0x59, "Camera GPS Get")]
        Camera_CameraGPSGet = 0x0259,
        [Cmd(602, 0x02, "Camera", 0x5a, "Discon State Set")]
        Camera_DisconStateSet = 0x025a,
        [Cmd(603, 0x02, "Camera", 0x5b, "Discon State Get")]
        Camera_DisconStateGet = 0x025b,
        [Cmd(604, 0x02, "Camera", 0x5c, "File Index Mode Set")]
        Camera_FileIndexModeSet = 0x025c,
        [Cmd(605, 0x02, "Camera", 0x5d, "File Index Mode Get")]
        Camera_FileIndexModeGet = 0x025d,
        [Cmd(606, 0x02, "Camera", 0x5e, "AE bCap Param Set")]
        Camera_AEBCapParamSet = 0x025e,
        [Cmd(607, 0x02, "Camera", 0x5f, "AE bCap Param Get")]
        Camera_AEBCapParamGet = 0x025f,
        [Cmd(608, 0x02, "Camera", 0x60, "Histogram Set")]
        Camera_HistogramSet = 0x0260,
        [Cmd(609, 0x02, "Camera", 0x61, "Histogram Get")]
        Camera_HistogramGet = 0x0261,
        [Cmd(610, 0x02, "Camera", 0x62, "Video Subtitles Set")]
        Camera_VideoSubtitlesSet = 0x0262,
        [Cmd(611, 0x02, "Camera", 0x63, "Video Subtitles Get")]
        Camera_VideoSubtitlesGet = 0x0263,
        [Cmd(612, 0x02, "Camera", 0x64, "Video Subtitles Log Set")]
        Camera_VideoSubtitlesLogSet = 0x0264,
        [Cmd(613, 0x02, "Camera", 0x65, "Mgear Shutter Speed Set")]
        Camera_MgearShutterSpeedSet = 0x0265,
        [Cmd(614, 0x02, "Camera", 0x66, "Video Standard Set")]
        Camera_VideoStandardSet = 0x0266,
        [Cmd(615, 0x02, "Camera", 0x67, "Video Standard Get")]
        Camera_VideoStandardGet = 0x0267,
        [Cmd(616, 0x02, "Camera", 0x68, "AE Lock Status Set")]
        Camera_AELockStatusSet = 0x0268,
        [Cmd(617, 0x02, "Camera", 0x69, "AE Lock Status Get")]
        Camera_AELockStatusGet = 0x0269,
        [Cmd(618, 0x02, "Camera", 0x6a, "Photo Capture Type Set")]
        Camera_PhotoCaptureTypeSet = 0x026a,
        [Cmd(619, 0x02, "Camera", 0x6b, "Photo Capture Type Get")]
        Camera_PhotoCaptureTypeGet = 0x026b,
        [Cmd(620, 0x02, "Camera", 0x6c, "Video Record Mode Set")]
        Camera_VideoRecordModeSet = 0x026c,
        [Cmd(621, 0x02, "Camera", 0x6d, "Video Record Mode Get")]
        Camera_VideoRecordModeGet = 0x026d,
        [Cmd(622, 0x02, "Camera", 0x6e, "Panorama Mode Set")]
        Camera_PanoramaModeSet = 0x026e,
        [Cmd(623, 0x02, "Camera", 0x6f, "Panorama Mode Get")]
        Camera_PanoramaModeGet = 0x026f,
        [Cmd(624, 0x02, "Camera", 0x70, "System State Get")]
        Camera_SystemStateGet = 0x0270,
        [Cmd(625, 0x02, "Camera", 0x71, "SDcard Info Get")]
        Camera_SDcardInfoGet = 0x0271,
        [Cmd(626, 0x02, "Camera", 0x72, "SDcard Do Format")]
        Camera_SDcardDoFormat = 0x0272,
        [Cmd(627, 0x02, "Camera", 0x73, "SDcard Format Progress Get")]
        Camera_SDcardFormatProgressGet = 0x0273,
        [Cmd(628, 0x02, "Camera", 0x74, "Fw Upgrade Progress Get")]
        Camera_FwUpgradeProgressGet = 0x0274,
        [Cmd(629, 0x02, "Camera", 0x75, "Photo Sync Progress Get")]
        Camera_PhotoSyncProgressGet = 0x0275,
        [Cmd(630, 0x02, "Camera", 0x76, "Camera Power Info Get")]
        Camera_CameraPowerInfoGet = 0x0276,
        [Cmd(631, 0x02, "Camera", 0x77, "Settings Save")]
        Camera_SettingsSave = 0x0277,
        [Cmd(632, 0x02, "Camera", 0x78, "Settings Load")]
        Camera_SettingsLoad = 0x0278,
        [Cmd(633, 0x02, "Camera", 0x79, "File Delete")]
        Camera_FileDelete = 0x0279,
        [Cmd(634, 0x02, "Camera", 0x7a, "Video Play Control")]
        Camera_VideoPlayControl = 0x027a,
        [Cmd(635, 0x02, "Camera", 0x7b, "Thumbnail 2 Single Ctrl")]
        Camera_ThumbnailSingleCtrl = 0x027b,
        [Cmd(636, 0x02, "Camera", 0x7c, "Camera Shutter Cmd")]
        Camera_CameraShutterCmd = 0x027c,
        [Cmd(637, 0x02, "Camera", 0x7d, "PB Zoom Ctrl")]
        Camera_PBZoomCtrl = 0x027d,
        [Cmd(638, 0x02, "Camera", 0x7e, "PB Pic Drag Ctrl")]
        Camera_PBPicDragCtrl = 0x027e,
        [Cmd(640, 0x02, "Camera", 0x80, "Camera State Info")]
        Camera_CameraStateInfo = 0x0280,
        [Cmd(641, 0x02, "Camera", 0x81, "Camera Shot Params")]
        Camera_CameraShotParams = 0x0281,
        [Cmd(642, 0x02, "Camera", 0x82, "Camera PlayBack Params")]
        Camera_CameraPlayBackParams = 0x0282,
        [Cmd(643, 0x02, "Camera", 0x83, "Camera Chart Info")]
        Camera_CameraChartInfo = 0x0283,
        [Cmd(644, 0x02, "Camera", 0x84, "Camera Recording Name")]
        Camera_CameraRecordingName = 0x0284,
        [Cmd(645, 0x02, "Camera", 0x85, "Camera Raw Params")]
        Camera_CameraRawParams = 0x0285,
        [Cmd(646, 0x02, "Camera", 0x86, "Camera Cur Pano Status")]
        Camera_CameraCurPanoStatus = 0x0286,
        [Cmd(647, 0x02, "Camera", 0x87, "Camera Shot Info")]
        Camera_CameraShotInfo = 0x0287,
        [Cmd(648, 0x02, "Camera", 0x88, "Camera Timelapse Parms")]
        Camera_CameraTimelapseParms = 0x0288,
        [Cmd(649, 0x02, "Camera", 0x89, "Camera Tracking Status")]
        Camera_CameraTrackingStatus = 0x0289,
        [Cmd(650, 0x02, "Camera", 0x8a, "Camera FOV Param")]
        Camera_CameraFOVParam = 0x028a,
        [Cmd(651, 0x02, "Camera", 0x8b, "Racing Liveview Format Set")]
        Camera_RacingLiveviewFormatSet = 0x028b,
        [Cmd(652, 0x02, "Camera", 0x8c, "Racing Liveview Format Get")]
        Camera_RacingLiveviewFormatGet = 0x028c,
        [Cmd(656, 0x02, "Camera", 0x90, "Sensor Calibrate Test")]
        Camera_SensorCalibrateTest = 0x0290,
        [Cmd(657, 0x02, "Camera", 0x91, "Sensor Calibrate Complete")]
        Camera_SensorCalibrateComplete = 0x0291,
        [Cmd(658, 0x02, "Camera", 0x92, "Video Clip Info Get")]
        Camera_VideoClipInfoGet = 0x0292,
        [Cmd(659, 0x02, "Camera", 0x93, "TransCode Control")]
        Camera_TransCodeControl = 0x0293,
        [Cmd(660, 0x02, "Camera", 0x94, "Focus Range Get")]
        Camera_FocusRangeGet = 0x0294,
        [Cmd(661, 0x02, "Camera", 0x95, "Focus Stroke Set")]
        Camera_FocusStrokeSet = 0x0295,
        [Cmd(662, 0x02, "Camera", 0x96, "Focus Stroke Get")]
        Camera_FocusStrokeGet = 0x0296,
        [Cmd(664, 0x02, "Camera", 0x98, "FileSystem Info Get")]
        Camera_FileSystemInfoGet = 0x0298,
        [Cmd(665, 0x02, "Camera", 0x99, "Shot Info Get")]
        Camera_ShotInfoGet = 0x0299,
        [Cmd(666, 0x02, "Camera", 0x9a, "Focus Aid Set")]
        Camera_FocusAidSet = 0x029a,
        [Cmd(667, 0x02, "Camera", 0x9b, "Video Adaptive Gamma Set")]
        Camera_VideoAdaptiveGammaSet = 0x029b,
        [Cmd(668, 0x02, "Camera", 0x9c, "Video Adaptive Gamma Get")]
        Camera_VideoAdaptiveGammaGet = 0x029c,
        [Cmd(669, 0x02, "Camera", 0x9d, "Awb Meter Region Set")]
        Camera_AwbMeterRegionSet = 0x029d,
        [Cmd(670, 0x02, "Camera", 0x9e, "Awb Meter Region Get")]
        Camera_AwbMeterRegionGet = 0x029e,
        [Cmd(671, 0x02, "Camera", 0x9f, "Audio Param Set")]
        Camera_AudioParamSet = 0x029f,
        [Cmd(672, 0x02, "Camera", 0xa0, "Audio Param Get")]
        Camera_AudioParamGet = 0x02a0,
        [Cmd(673, 0x02, "Camera", 0xa1, "Format Raw SSD")]
        Camera_FormatRawSSD = 0x02a1,
        [Cmd(674, 0x02, "Camera", 0xa2, "Focus Distance Set")]
        Camera_FocusDistanceSet = 0x02a2,
        [Cmd(675, 0x02, "Camera", 0xa3, "Calibration Control Set")]
        Camera_CalibrationControlSet = 0x02a3,
        [Cmd(676, 0x02, "Camera", 0xa4, "Focus Window Set")]
        Camera_FocusWindowSet = 0x02a4,
        [Cmd(677, 0x02, "Camera", 0xa5, "Tracking Region Get")]
        Camera_TrackingRegionGet = 0x02a5,
        [Cmd(678, 0x02, "Camera", 0xa6, "Tracking Region Set")]
        Camera_TrackingRegionSet = 0x02a6,
        [Cmd(679, 0x02, "Camera", 0xa7, "Iris Set")]
        Camera_IrisSet = 0x02a7,
        [Cmd(680, 0x02, "Camera", 0xa8, "AE Unlock Mode Set")]
        Camera_AEUnlockModeSet = 0x02a8,
        [Cmd(681, 0x02, "Camera", 0xa9, "AE Unlock Mode Get")]
        Camera_AEUnlockModeGet = 0x02a9,
        [Cmd(682, 0x02, "Camera", 0xaa, "Pano File Params Get")]
        Camera_PanoFileParamsGet = 0x02aa,
        [Cmd(683, 0x02, "Camera", 0xab, "Video Encode Set")]
        Camera_VideoEncodeSet = 0x02ab,
        [Cmd(684, 0x02, "Camera", 0xac, "Video Encode Get")]
        Camera_VideoEncodeGet = 0x02ac,
        [Cmd(685, 0x02, "Camera", 0xad, "MCTF Set")]
        Camera_MCTFSet = 0x02ad,
        [Cmd(686, 0x02, "Camera", 0xae, "MCTF Get")]
        Camera_MCTFGet = 0x02ae,
        [Cmd(687, 0x02, "Camera", 0xaf, "SSD Video Format Set")]
        Camera_SSDVideoFormatSet = 0x02af,
        [Cmd(688, 0x02, "Camera", 0xb0, "SSD Video Format Get")]
        Camera_SSDVideoFormatGet = 0x02b0,
        [Cmd(689, 0x02, "Camera", 0xb1, "Record Fan Set")]
        Camera_RecordFanSet = 0x02b1,
        [Cmd(690, 0x02, "Camera", 0xb2, "Record Fan Get")]
        Camera_RecordFanGet = 0x02b2,
        [Cmd(691, 0x02, "Camera", 0xb3, "Request IFrame")]
        Camera_RequestIFrame = 0x02b3,
        [Cmd(692, 0x02, "Camera", 0xb4, "Camera Prepare Open Fan")]
        Camera_CameraPrepareOpenFan = 0x02b4,
        [Cmd(693, 0x02, "Camera", 0xb5, "Camera Sensor Id Get")]
        Camera_CameraSensorIdGet = 0x02b5,
        [Cmd(694, 0x02, "Camera", 0xb6, "Forearm Lamp Config Set")]
        Camera_ForearmLampConfigSet = 0x02b6,
        [Cmd(695, 0x02, "Camera", 0xb7, "Forearm Lamp Config Get")]
        Camera_ForearmLampConfigGet = 0x02b7,
        [Cmd(696, 0x02, "Camera", 0xb8, "Camera Optics Zoom Mode")]
        Camera_CameraOpticsZoomMode = 0x02b8,
        [Cmd(697, 0x02, "Camera", 0xb9, "Image Rotation Set")]
        Camera_ImageRotationSet = 0x02b9,
        [Cmd(698, 0x02, "Camera", 0xba, "Image Rotation Get")]
        Camera_ImageRotationGet = 0x02ba,
        [Cmd(699, 0x02, "Camera", 0xbb, "Gimbal Lock Config Set")]
        Camera_GimbalLockConfigSet = 0x02bb,
        [Cmd(700, 0x02, "Camera", 0xbc, "Gimbal Lock Config Get")]
        Camera_GimbalLockConfigGet = 0x02bc,
        [Cmd(701, 0x02, "Camera", 0xbd, "Old Cam LCD Format Set")]
        Camera_OldCamLCDFormatSet = 0x02bd,
        [Cmd(702, 0x02, "Camera", 0xbe, "Old Cam LCD Format Get")]
        Camera_OldCamLCDFormatGet = 0x02be,
        [Cmd(703, 0x02, "Camera", 0xbf, "File Star Flag Set")]
        Camera_FileStarFlagSet = 0x02bf,
        [Cmd(704, 0x02, "Camera", 0xc0, "MFDemarcate")]
        Camera_MFDemarcate = 0x02c0,
        [Cmd(705, 0x02, "Camera", 0xc1, "Log Mode Set")]
        Camera_LogModeSet = 0x02c1,
        [Cmd(706, 0x02, "Camera", 0xc2, "Param Name Set")]
        Camera_ParamNameSet = 0x02c2,
        [Cmd(707, 0x02, "Camera", 0xc3, "Param Name Get")]
        Camera_ParamNameGet = 0x02c3,
        [Cmd(708, 0x02, "Camera", 0xc4, "Camera Tap Zoom Set")]
        Camera_CameraTapZoomSet = 0x02c4,
        [Cmd(709, 0x02, "Camera", 0xc5, "Camera Tap Zoom Get")]
        Camera_CameraTapZoomGet = 0x02c5,
        [Cmd(710, 0x02, "Camera", 0xc6, "Camera Tap Zoom Target Set")]
        Camera_CameraTapZoomTargetSet = 0x02c6,
        [Cmd(711, 0x02, "Camera", 0xc7, "Camera Tap Zoom State Info")]
        Camera_CameraTapZoomStateInfo = 0x02c7,
        [Cmd(712, 0x02, "Camera", 0xc8, "Defog Enabled Set")]
        Camera_DefogEnabledSet = 0x02c8,
        [Cmd(713, 0x02, "Camera", 0xc9, "Defog Enabled Get")]
        Camera_DefogEnabledGet = 0x02c9,
        [Cmd(714, 0x02, "Camera", 0xca, "Raw Equip Info Set")]
        Camera_RawEquipInfoSet = 0x02ca,
        [Cmd(716, 0x02, "Camera", 0xcc, "SSD Raw Video Digital Filter Set")]
        Camera_SSDRawVideoDigitalFilterSet = 0x02cc,
        [Cmd(718, 0x02, "Camera", 0xce, "Calibration Control Get")]
        Camera_CalibrationControlGet = 0x02ce,
        [Cmd(719, 0x02, "Camera", 0xcf, "Mechanical Shutter Set")]
        Camera_MechanicalShutterSet = 0x02cf,
        [Cmd(720, 0x02, "Camera", 0xd0, "Mechanical Shutter Get")]
        Camera_MechanicalShutterGet = 0x02d0,
        [Cmd(721, 0x02, "Camera", 0xd1, "Cam DCF Abstract Push")]
        Camera_CamDCFAbstractPush = 0x02d1,
        [Cmd(722, 0x02, "Camera", 0xd2, "Dust Reduction State Set")]
        Camera_DustReductionStateSet = 0x02d2,
        [Cmd(723, 0x02, "Camera", 0xd3, "Camera UnknownD3")]
        Camera_CameraUnknownD = 0x02d3,
        [Cmd(733, 0x02, "Camera", 0xdd, "ND Filter Set")]
        Camera_NDFilterSet = 0x02dd,
        [Cmd(734, 0x02, "Camera", 0xde, "Raw New Param Set")]
        Camera_RawNewParamSet = 0x02de,
        [Cmd(735, 0x02, "Camera", 0xdf, "Raw New Param Get")]
        Camera_RawNewParamGet = 0x02df,
        [Cmd(736, 0x02, "Camera", 0xe0, "Capture Sound")]
        Camera_CaptureSound = 0x02e0,
        [Cmd(737, 0x02, "Camera", 0xe1, "Capture Config Set")]
        Camera_CaptureConfigSet = 0x02e1,
        [Cmd(738, 0x02, "Camera", 0xe2, "Capture Config Get")]
        Camera_CaptureConfigGet = 0x02e2,
        [Cmd(752, 0x02, "Camera", 0xf0, "Camera TBD F0")]
        Camera_CameraTBDF = 0x02f0,
        [Cmd(753, 0x02, "Camera", 0xf1, "Camera Tau Param")]
        Camera_CameraTauParam = 0x02f1,
        [Cmd(754, 0x02, "Camera", 0xf2, "Camera Tau Param Get")]
        Camera_CameraTauParamGet = 0x02f2,
        [Cmd(761, 0x02, "Camera", 0xf9, "Focus Infinite Get")]
        Camera_FocusInfiniteGet = 0x02f9,
        [Cmd(762, 0x02, "Camera", 0xfa, "Focus Infinite Set")]
        Camera_FocusInfiniteSet = 0x02fa,

        // [3] https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L494
        // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-flyc.lua#L9
        // ToDo: Verify whether we do have to account the 'new platform' values
        [Cmd(768, 0x03, "FlightController", 0x00, "FlyC Scan/Test")]
        FlightController_FlyCScanTest = 0x0300,
        [Cmd(769, 0x03, "FlightController", 0x01, "FlyC Status Get")]
        FlightController_FlyCStatusGet = 0x0301,
        [Cmd(770, 0x03, "FlightController", 0x02, "FlyC Params Get")]
        FlightController_FlyCParamsGet = 0x0302,
        [Cmd(771, 0x03, "FlightController", 0x03, "Origin GPS Set")]
        FlightController_OriginGPSSet = 0x0303,
        [Cmd(772, 0x03, "FlightController", 0x04, "Origin GPS Get")]
        FlightController_OriginGPSGet = 0x0304,
        [Cmd(773, 0x03, "FlightController", 0x05, "GPS Coordinate Get")]
        FlightController_GPSCoordinateGet = 0x0305,
        [Cmd(774, 0x03, "FlightController", 0x06, "Fly Limit Param Set")]
        FlightController_FlyLimitParamSet = 0x0306,
        [Cmd(775, 0x03, "FlightController", 0x07, "Fly Limit Param Get")]
        FlightController_FlyLimitParamGet = 0x0307,
        [Cmd(776, 0x03, "FlightController", 0x08, "Nofly Zone Set")]
        FlightController_NoflyZoneSet = 0x0308,
        [Cmd(777, 0x03, "FlightController", 0x09, "Nofly Status Get")]
        FlightController_NoflyStatusGet = 0x0309,
        [Cmd(778, 0x03, "FlightController", 0x0a, "Battery Status Get")]
        FlightController_BatteryStatusGet = 0x030a,
        [Cmd(779, 0x03, "FlightController", 0x0b, "Motor Work Status Set")]
        FlightController_MotorWorkStatusSet = 0x030b,
        [Cmd(780, 0x03, "FlightController", 0x0c, "Motor Work Status Get")]
        FlightController_MotorWorkStatusGet = 0x030c,
        [Cmd(781, 0x03, "FlightController", 0x0d, "Statistical Info Save")]
        FlightController_StatisticalInfoSave = 0x030d,
        [Cmd(782, 0x03, "FlightController", 0x0e, "Emergency Stop")]
        FlightController_EmergencyStop = 0x030e,
        [Cmd(784, 0x03, "FlightController", 0x10, "A2 Push Commom")]
        FlightController_APushCommom = 0x0310,
        [Cmd(785, 0x03, "FlightController", 0x11, "Sim Rc")]
        FlightController_SimRc = 0x0311,
        [Cmd(790, 0x03, "FlightController", 0x16, "Sim Status")]
        FlightController_SimStatus = 0x0316,
        [Cmd(796, 0x03, "FlightController", 0x1c, "Date and Time Set")]
        FlightController_DateAndTimeSet = 0x031c,
        [Cmd(797, 0x03, "FlightController", 0x1d, "Initialize Onboard FChannel")]
        FlightController_InitializeOnboardFChannel = 0x031d,
        [Cmd(798, 0x03, "FlightController", 0x1e, "Get Onboard FChannel Output")]
        FlightController_GetOnboardFChannelOutput = 0x031e,
        [Cmd(799, 0x03, "FlightController", 0x1f, "Set Onboard FChannel Output")]
        FlightController_SetOnboardFChannelOutput = 0x031f,
        [Cmd(800, 0x03, "FlightController", 0x20, "Send GPS To Flyc")]
        FlightController_SendGPSToFlyc = 0x0320,
        [Cmd(801, 0x03, "FlightController", 0x21, "UAV Status Get")]
        FlightController_UAVStatusGet = 0x0321,
        [Cmd(802, 0x03, "FlightController", 0x22, "Upload Air Route")]
        FlightController_UploadAirRoute = 0x0322,
        [Cmd(803, 0x03, "FlightController", 0x23, "Download Air Route")]
        FlightController_DownloadAirRoute = 0x0323,
        [Cmd(804, 0x03, "FlightController", 0x24, "Upload Waypoint")]
        FlightController_UploadWaypoint = 0x0324,
        [Cmd(805, 0x03, "FlightController", 0x25, "Download Waypoint")]
        FlightController_DownloadWaypoint = 0x0325,
        [Cmd(806, 0x03, "FlightController", 0x26, "Enable Waypoint")]
        FlightController_EnableWaypoint = 0x0326,
        [Cmd(807, 0x03, "FlightController", 0x27, "Exec Fly")]
        FlightController_ExecFly = 0x0327,
        [Cmd(808, 0x03, "FlightController", 0x28, "One Key Back")]
        FlightController_OneKeyBack = 0x0328,
        [Cmd(809, 0x03, "FlightController", 0x29, "Joystick")]
        FlightController_Joystick = 0x0329,
        [Cmd(810, 0x03, "FlightController", 0x2a, "Function Control")]
        FlightController_FunctionControl = 0x032a,
        [Cmd(811, 0x03, "FlightController", 0x2b, "IOC Mode Type Set")]
        FlightController_IOCModeTypeSet = 0x032b,
        [Cmd(812, 0x03, "FlightController", 0x2c, "IOC Mode Type Get")]
        FlightController_IOCModeTypeGet = 0x032c,
        [Cmd(813, 0x03, "FlightController", 0x2d, "Limit Params Set")]
        FlightController_LimitParamsSet = 0x032d,
        [Cmd(814, 0x03, "FlightController", 0x2e, "Limit Params Get")]
        FlightController_LimitParamsGet = 0x032e,
        [Cmd(815, 0x03, "FlightController", 0x2f, "Battery Voltage Alarm Set")]
        FlightController_BatteryVoltageAlarmSet = 0x032f,
        [Cmd(816, 0x03, "FlightController", 0x30, "Battery Voltage Alarm Get")]
        FlightController_BatteryVoltageAlarmGet = 0x0330,
        [Cmd(817, 0x03, "FlightController", 0x31, "UAV Home Point Set")]
        FlightController_UAVHomePointSet = 0x0331,
        [Cmd(818, 0x03, "FlightController", 0x32, "FlyC Deform Status Get")]
        FlightController_FlyCDeformStatusGet = 0x0332,
        [Cmd(819, 0x03, "FlightController", 0x33, "UAV User String Set")]
        FlightController_UAVUserStringSet = 0x0333,
        [Cmd(820, 0x03, "FlightController", 0x34, "UAV User String Get")]
        FlightController_UAVUserStringGet = 0x0334,
        [Cmd(821, 0x03, "FlightController", 0x35, "Change Param Ping")]
        FlightController_ChangeParamPing = 0x0335,
        [Cmd(822, 0x03, "FlightController", 0x36, "Request SN")]
        FlightController_RequestSN = 0x0336,
        [Cmd(823, 0x03, "FlightController", 0x37, "Device Info Get")]
        FlightController_DeviceInfoGet = 0x0337,
        [Cmd(824, 0x03, "FlightController", 0x38, "Device Info Set")]
        FlightController_DeviceInfoSet = 0x0338,
        [Cmd(825, 0x03, "FlightController", 0x39, "Enter Flight Data Mode")]
        FlightController_EnterFlightDataMode = 0x0339,
        [Cmd(826, 0x03, "FlightController", 0x3a, "Ctrl Fly Data Recorder")]
        FlightController_CtrlFlyDataRecorder = 0x033a,
        [Cmd(827, 0x03, "FlightController", 0x3b, "RC Lost Action Set")]
        FlightController_RCLostActionSet = 0x033b,
        [Cmd(828, 0x03, "FlightController", 0x3c, "RC Lost Action Get")]
        FlightController_RCLostActionGet = 0x033c,
        [Cmd(829, 0x03, "FlightController", 0x3d, "Time Zone Set")]
        FlightController_TimeZoneSet = 0x033d,
        [Cmd(830, 0x03, "FlightController", 0x3e, "FlyC Request Limit Update")]
        FlightController_FlyCRequestLimitUpdate = 0x033e,
        [Cmd(831, 0x03, "FlightController", 0x3f, "Set NoFly Zone Data")]
        FlightController_SetNoFlyZoneData = 0x033f,
        [Cmd(833, 0x03, "FlightController", 0x41, "Upload Unlimit Areas")]
        FlightController_UploadUnlimitAreas = 0x0341,
        [Cmd(834, 0x03, "FlightController", 0x42, "FlyC Unlimit State / UAV Posture")]
        FlightController_FlyCUnlimitStateUAVPosture = 0x0342,
        [Cmd(835, 0x03, "FlightController", 0x43, "OSD General Data Get")]
        FlightController_OSDGeneralDataGet = 0x0343,
        [Cmd(836, 0x03, "FlightController", 0x44, "OSD Home Point Get")]
        FlightController_OSDHomePointGet = 0x0344,
        [Cmd(837, 0x03, "FlightController", 0x45, "FlyC GPS SNR Get")]
        FlightController_FlyCGPSSNRGet = 0x0345,
        [Cmd(838, 0x03, "FlightController", 0x46, "FlyC GPS SNR Set")]
        FlightController_FlyCGPSSNRSet = 0x0346,
        [Cmd(839, 0x03, "FlightController", 0x47, "Enable Unlimit Areas")]
        FlightController_EnableUnlimitAreas = 0x0347,
        [Cmd(841, 0x03, "FlightController", 0x49, "Push Encrypted Package")]
        FlightController_PushEncryptedPackage = 0x0349,
        [Cmd(842, 0x03, "FlightController", 0x4a, "Push Att IMU Info")]
        FlightController_PushAttIMUInfo = 0x034a,
        [Cmd(843, 0x03, "FlightController", 0x4b, "Push RC Stick Value")]
        FlightController_PushRCStickValue = 0x034b,
        [Cmd(844, 0x03, "FlightController", 0x4c, "Push Fussed Pos Speed Data")]
        FlightController_PushFussedPosSpeedData = 0x034c,
        [Cmd(848, 0x03, "FlightController", 0x50, "Imu Data Status")]
        FlightController_ImuDataStatus = 0x0350,
        [Cmd(849, 0x03, "FlightController", 0x51, "FlyC Battery Status Get")]
        FlightController_FlyCBatteryStatusGet = 0x0351,
        [Cmd(850, 0x03, "FlightController", 0x52, "Smart Low Battery Actn Set")]
        FlightController_SmartLowBatteryActnSet = 0x0352,
        [Cmd(851, 0x03, "FlightController", 0x53, "FlyC Vis Avoidance Param Get")]
        FlightController_FlyCVisAvoidanceParamGet = 0x0353,
        [Cmd(853, 0x03, "FlightController", 0x55, "FlyC Limit State Get")]
        FlightController_FlyCLimitStateGet = 0x0355,
        [Cmd(854, 0x03, "FlightController", 0x56, "FlyC LED Status Get")]
        FlightController_FlyCLEDStatusGet = 0x0356,
        [Cmd(855, 0x03, "FlightController", 0x57, "GPS GLNS Info")]
        FlightController_GPSGLNSInfo = 0x0357,
        [Cmd(856, 0x03, "FlightController", 0x58, "Push Att Stick Speed Pos Data")]
        FlightController_PushAttStickSpeedPosData = 0x0358,
        [Cmd(857, 0x03, "FlightController", 0x59, "Push Sdk Data")]
        FlightController_PushSdkData = 0x0359,
        [Cmd(858, 0x03, "FlightController", 0x5a, "Push FC Data")]
        FlightController_PushFCData = 0x035a,
        [Cmd(864, 0x03, "FlightController", 0x60, "SVO API Transfer")]
        FlightController_SVOAPITransfer = 0x0360,
        [Cmd(865, 0x03, "FlightController", 0x61, "FlyC Activation Info")]
        FlightController_FlyCActivationInfo = 0x0361,
        [Cmd(866, 0x03, "FlightController", 0x62, "FlyC Activation Exec")]
        FlightController_FlyCActivationExec = 0x0362,
        [Cmd(867, 0x03, "FlightController", 0x63, "FlyC On Board Recv")]
        FlightController_FlyCOnBoardRecv = 0x0363,
        [Cmd(868, 0x03, "FlightController", 0x64, "Send On Board Set")]
        FlightController_SendOnBoardSet = 0x0364,
        [Cmd(871, 0x03, "FlightController", 0x67, "FlyC Power Param Get")]
        FlightController_FlyCPowerParamGet = 0x0367,
        [Cmd(873, 0x03, "FlightController", 0x69, "RTK Switch")]
        FlightController_RTKSwitch = 0x0369,
        [Cmd(874, 0x03, "FlightController", 0x6a, "FlyC Avoid")]
        FlightController_FlyCAvoid = 0x036a,
        [Cmd(875, 0x03, "FlightController", 0x6b, "Recorder Data Cfg")]
        FlightController_RecorderDataCfg = 0x036b,
        [Cmd(876, 0x03, "FlightController", 0x6c, "FlyC RTK Location Data Get")]
        FlightController_FlyCRTKLocationDataGet = 0x036c,
        [Cmd(877, 0x03, "FlightController", 0x6d, "Upload Hotpoint")]
        FlightController_UploadHotpoint = 0x036d,
        [Cmd(878, 0x03, "FlightController", 0x6e, "Download Hotpoint")]
        FlightController_DownloadHotpoint = 0x036e,
        [Cmd(880, 0x03, "FlightController", 0x70, "Set Product SN")]
        FlightController_SetProductSN = 0x0370,
        [Cmd(881, 0x03, "FlightController", 0x71, "Get Product SN")]
        FlightController_GetProductSN = 0x0371,
        [Cmd(882, 0x03, "FlightController", 0x72, "Reset Product SN")]
        FlightController_ResetProductSN = 0x0372,
        [Cmd(883, 0x03, "FlightController", 0x73, "Set Product Id")]
        FlightController_SetProductId = 0x0373,
        [Cmd(884, 0x03, "FlightController", 0x74, "Get Product Id")]
        FlightController_GetProductId = 0x0374,
        [Cmd(885, 0x03, "FlightController", 0x75, "Write EEPROM FC0")]
        FlightController_WriteEEPROMFC = 0x0375,
        [Cmd(886, 0x03, "FlightController", 0x76, "Read EEPROM FC0")]
        FlightController_ReadEEPROMFC = 0x0376,
        [Cmd(896, 0x03, "FlightController", 0x80, "Navigation Mode Set")]
        FlightController_NavigationModeSet = 0x0380,
        [Cmd(897, 0x03, "FlightController", 0x81, "Mission IOC: Set Lock Yaw")]
        FlightController_MissionIOCSetLockYaw = 0x0381,
        [Cmd(898, 0x03, "FlightController", 0x82, "Miss. WP: Upload Mission Info")]
        FlightController_MissWPUploadMissionInfo = 0x0382,
        [Cmd(899, 0x03, "FlightController", 0x83, "Miss. WP: Download Mission Info")]
        FlightController_MissWPDownloadMissionInfo = 0x0383,
        [Cmd(900, 0x03, "FlightController", 0x84, "Upload Waypoint Info By Idx")]
        FlightController_UploadWaypointInfoByIdx = 0x0384,
        [Cmd(901, 0x03, "FlightController", 0x85, "Download Waypoint Info By Idx")]
        FlightController_DownloadWaypointInfoByIdx = 0x0385,
        [Cmd(902, 0x03, "FlightController", 0x86, "Mission WP: Go/Stop")]
        FlightController_MissionWPGoStop = 0x0386,
        [Cmd(903, 0x03, "FlightController", 0x87, "Mission WP: Pasue/Resume")]
        FlightController_MissionWPPasueResume = 0x0387,
        [Cmd(904, 0x03, "FlightController", 0x88, "Push Navigation Status Info")]
        FlightController_PushNavigationStatusInfo = 0x0388,
        [Cmd(905, 0x03, "FlightController", 0x89, "Push Navigation Event Info")]
        FlightController_PushNavigationEventInfo = 0x0389,
        [Cmd(906, 0x03, "FlightController", 0x8a, "Miss. HotPoint: Start With Info")]
        FlightController_MissHotPointStartWithInfo = 0x038a,
        [Cmd(907, 0x03, "FlightController", 0x8b, "Miss. HotPoint: Cancel")]
        FlightController_MissHotPointCancel = 0x038b,
        [Cmd(908, 0x03, "FlightController", 0x8c, "Miss. HotPoint: Pasue/Resume")]
        FlightController_MissHotPointPasueResume = 0x038c,
        [Cmd(909, 0x03, "FlightController", 0x8d, "App Set API Sub Mode")]
        FlightController_AppSetAPISubMode = 0x038d,
        [Cmd(910, 0x03, "FlightController", 0x8e, "App Joystick Data")]
        FlightController_AppJoystickData = 0x038e,
        [Cmd(911, 0x03, "FlightController", 0x8f, "Noe Mission pasue/resume")]
        FlightController_NoeMissionPasueResume = 0x038f,
        [Cmd(912, 0x03, "FlightController", 0x90, "Miss. Follow: Start With Info")]
        FlightController_MissFollowStartWithInfo = 0x0390,
        [Cmd(913, 0x03, "FlightController", 0x91, "Miss. Follow: Cancel")]
        FlightController_MissFollowCancel = 0x0391,
        [Cmd(914, 0x03, "FlightController", 0x92, "Miss. Follow: Pasue/Resume")]
        FlightController_MissFollowPasueResume = 0x0392,
        [Cmd(915, 0x03, "FlightController", 0x93, "Miss. Follow: Get Target Info")]
        FlightController_MissFollowGetTargetInfo = 0x0393,
        [Cmd(916, 0x03, "FlightController", 0x94, "Mission Noe: Start")]
        FlightController_MissionNoeStart = 0x0394,
        [Cmd(917, 0x03, "FlightController", 0x95, "Mission Noe: Stop")]
        FlightController_MissionNoeStop = 0x0395,
        [Cmd(918, 0x03, "FlightController", 0x96, "Mission HotPoint: Download")]
        FlightController_MissionHotPointDownload = 0x0396,
        [Cmd(919, 0x03, "FlightController", 0x97, "Mission IOC: Start")]
        FlightController_MissionIOCStart = 0x0397,
        [Cmd(920, 0x03, "FlightController", 0x98, "Mission IOC: Stop")]
        FlightController_MissionIOCStop = 0x0398,
        [Cmd(921, 0x03, "FlightController", 0x99, "Miss. HotPoint: Set Params")]
        FlightController_MissHotPointSetParams = 0x0399,
        [Cmd(922, 0x03, "FlightController", 0x9a, "Miss. HotPoint: Set Radius")]
        FlightController_MissHotPointSetRadius = 0x039a,
        [Cmd(923, 0x03, "FlightController", 0x9b, "Miss. HotPoint: Set Head")]
        FlightController_MissHotPointSetHead = 0x039b,
        [Cmd(924, 0x03, "FlightController", 0x9c, "Miss. WP: Set Idle Veloc")]
        FlightController_MissWPSetIdleVeloc = 0x039c,
        [Cmd(925, 0x03, "FlightController", 0x9d, "Miss. WP: Get Idle Veloc")]
        FlightController_MissWPGetIdleVeloc = 0x039d,
        [Cmd(926, 0x03, "FlightController", 0x9e, "App Ctrl Mission Yaw Rate")]
        FlightController_AppCtrlMissionYawRate = 0x039e,
        [Cmd(927, 0x03, "FlightController", 0x9f, "Miss. HotPoint: Auto Radius Ctrl")]
        FlightController_MissHotPointAutoRadiusCtrl = 0x039f,
        [Cmd(928, 0x03, "FlightController", 0xa0, "Send AGPS Data")]
        FlightController_SendAGPSData = 0x03a0,
        [Cmd(929, 0x03, "FlightController", 0xa1, "FlyC AGPS Status Get")]
        FlightController_FlyCAGPSStatusGet = 0x03a1,
        [Cmd(930, 0x03, "FlightController", 0xa2, "Race Drone OSD Push")]
        FlightController_RaceDroneOSDPush = 0x03a2,
        [Cmd(931, 0x03, "FlightController", 0xa3, "Miss. WP: Get BreakPoint Info")]
        FlightController_MissWPGetBreakPointInfo = 0x03a3,
        [Cmd(932, 0x03, "FlightController", 0xa4, "Miss. WP: Return To Cur Line")]
        FlightController_MissWPReturnToCurLine = 0x03a4,
        [Cmd(933, 0x03, "FlightController", 0xa5, "App Ctrl Fly Sweep Ctrl")]
        FlightController_AppCtrlFlySweepCtrl = 0x03a5,
        [Cmd(934, 0x03, "FlightController", 0xa6, "Set RKT Homepoint")]
        FlightController_SetRKTHomepoint = 0x03a6,
        [Cmd(938, 0x03, "FlightController", 0xaa, "Sbus Packet")]
        FlightController_SbusPacket = 0x03aa,
        [Cmd(939, 0x03, "FlightController", 0xab, "Ctrl Attitude Data Send")]
        FlightController_CtrlAttitudeDataSend = 0x03ab,
        [Cmd(940, 0x03, "FlightController", 0xac, "Ctrl Taillock Data Send")]
        FlightController_CtrlTaillockDataSend = 0x03ac,
        [Cmd(941, 0x03, "FlightController", 0xad, "FlyC Install Error Get")]
        FlightController_FlyCInstallErrorGet = 0x03ad,
        [Cmd(942, 0x03, "FlightController", 0xae, "Cmd Handler RC App Chl Handler")]
        FlightController_CmdHandlerRCAppChlHandler = 0x03ae,
        [Cmd(943, 0x03, "FlightController", 0xaf, "Product Config")]
        FlightController_ProductConfig = 0x03af,
        [Cmd(944, 0x03, "FlightController", 0xb0, "Get Battery Groups Single Info")]
        FlightController_GetBatteryGroupsSingleInfo = 0x03b0,
        [Cmd(949, 0x03, "FlightController", 0xb5, "FlyC Fault Inject Set")]
        FlightController_FlyCFaultInjectSet = 0x03b5,
        [Cmd(950, 0x03, "FlightController", 0xb6, "FlyC Fault Inject Get")]
        FlightController_FlyCFaultInjectGet = 0x03b6,
        [Cmd(951, 0x03, "FlightController", 0xb7, "Redundancy IMU Index Set and Get")]
        FlightController_RedundancyIMUIndexSetAndGet = 0x03b7,
        [Cmd(952, 0x03, "FlightController", 0xb8, "Redundancy Status")]
        FlightController_RedundancyStatus = 0x03b8,
        [Cmd(953, 0x03, "FlightController", 0xb9, "Push Redundancy Status")]
        FlightController_PushRedundancyStatus = 0x03b9,
        [Cmd(954, 0x03, "FlightController", 0xba, "Forearm LED Status Set")]
        FlightController_ForearmLEDStatusSet = 0x03ba,
        [Cmd(955, 0x03, "FlightController", 0xbb, "Open LED Info Get")]
        FlightController_OpenLEDInfoGet = 0x03bb,
        [Cmd(956, 0x03, "FlightController", 0xbc, "Open Led Action Register")]
        FlightController_OpenLedActionRegister = 0x03bc,
        [Cmd(957, 0x03, "FlightController", 0xbd, "Open Led Action Logout")]
        FlightController_OpenLedActionLogout = 0x03bd,
        [Cmd(958, 0x03, "FlightController", 0xbe, "Open Led Action Status Set")]
        FlightController_OpenLedActionStatusSet = 0x03be,
        [Cmd(959, 0x03, "FlightController", 0xbf, "Flight Push")]
        FlightController_FlightPush = 0x03bf,
        [Cmd(966, 0x03, "FlightController", 0xc6, "Shell Test")]
        FlightController_ShellTest = 0x03c6,
        [Cmd(973, 0x03, "FlightController", 0xcd, "Update Nofly Area")]
        FlightController_UpdateNoflyArea = 0x03cd,
        [Cmd(974, 0x03, "FlightController", 0xce, "Push Forbid Data Infos")]
        FlightController_PushForbidDataInfos = 0x03ce,
        [Cmd(975, 0x03, "FlightController", 0xcf, "New Nofly Area Get")]
        FlightController_NewNoflyAreaGet = 0x03cf,
        [Cmd(980, 0x03, "FlightController", 0xd4, "Additional Info Get")]
        FlightController_AdditionalInfoGet = 0x03d4,
        [Cmd(983, 0x03, "FlightController", 0xd7, "FlyC Flight Record Get")]
        FlightController_FlyCFlightRecordGet = 0x03d7,
        [Cmd(985, 0x03, "FlightController", 0xd9, "Process Sensor Api Data")]
        FlightController_ProcessSensorApiData = 0x03d9,
        [Cmd(986, 0x03, "FlightController", 0xda, "FlyC Detection")]
        FlightController_FlyCDetection = 0x03da,
        [Cmd(991, 0x03, "FlightController", 0xdf, "Assistant Unlock Handler")]
        FlightController_AssistantUnlockHandler = 0x03df,
        [Cmd(992, 0x03, "FlightController", 0xe0, "Config Table: Get Tbl Attribute")]
        FlightController_ConfigTableGetTblAttribute = 0x03e0,
        [Cmd(993, 0x03, "FlightController", 0xe1, "Config Table: Get Item Attribute")]
        FlightController_ConfigTableGetItemAttribute = 0x03e1,
        [Cmd(994, 0x03, "FlightController", 0xe2, "Config Table: Get Item Value")]
        FlightController_ConfigTableGetItemValue = 0x03e2,
        [Cmd(995, 0x03, "FlightController", 0xe3, "Config Table: Set Item Value")]
        FlightController_ConfigTableSetItemValue = 0x03e3,
        [Cmd(996, 0x03, "FlightController", 0xe4, "Config Table: Reset Def. Item Value")]
        FlightController_ConfigTableResetDefItemValue = 0x03e4,
        [Cmd(997, 0x03, "FlightController", 0xe5, "Push Config Table: Get Tbl Attribute")]
        FlightController_PushConfigTableGetTblAttribute = 0x03e5,
        [Cmd(998, 0x03, "FlightController", 0xe6, "Push Config Table: Get Item Attribute")]
        FlightController_PushConfigTableGetItemAttribute = 0x03e6,
        [Cmd(999, 0x03, "FlightController", 0xe7, "Push Config Table: Set Item Param")]
        FlightController_PushConfigTableSetItemParam = 0x03e7,
        [Cmd(1000, 0x03, "FlightController", 0xe8, "Push Config Table: Clear")]
        FlightController_PushConfigTableClear = 0x03e8,
        [Cmd(1001, 0x03, "FlightController", 0xe9, "Config Command Table: Get or Exec")]
        FlightController_ConfigCommandTableGetOrExec = 0x03e9,
        [Cmd(1002, 0x03, "FlightController", 0xea, "Register Open Motor Error Action")]
        FlightController_RegisterOpenMotorErrorAction = 0x03ea,
        [Cmd(1003, 0x03, "FlightController", 0xeb, "Logout Open Motor Error Action")]
        FlightController_LogoutOpenMotorErrorAction = 0x03eb,
        [Cmd(1004, 0x03, "FlightController", 0xec, "Set Open Motor Error Action Status")]
        FlightController_SetOpenMotorErrorActionStatus = 0x03ec,
        [Cmd(1005, 0x03, "FlightController", 0xed, "ESC Echo Set")]
        FlightController_ESCEchoSet = 0x03ed,
        [Cmd(1006, 0x03, "FlightController", 0xee, "GoHome CountDown Get")]
        FlightController_GoHomeCountDownGet = 0x03ee,
        [Cmd(1008, 0x03, "FlightController", 0xf0, "Config Table: Get Param Info by Index")]
        FlightController_ConfigTableGetParamInfoByIndex = 0x03f0,
        [Cmd(1009, 0x03, "FlightController", 0xf1, "Config Table: Read Params By Index")]
        FlightController_ConfigTableReadParamsByIndex = 0x03f1,
        [Cmd(1010, 0x03, "FlightController", 0xf2, "Config Table: Write Params By Index")]
        FlightController_ConfigTableWriteParamsByIndex = 0x03f2,
        [Cmd(1011, 0x03, "FlightController", 0xf3, "Config Table: Reset Default Param Val")]
        FlightController_ConfigTableResetDefaultParamVal = 0x03f3,
        [Cmd(1012, 0x03, "FlightController", 0xf4, "Config Table: Set Item By Index")]
        FlightController_ConfigTableSetItemByIndex = 0x03f4,
        [Cmd(1013, 0x03, "FlightController", 0xf5, "Ver Phone Set")]
        FlightController_VerPhoneSet = 0x03f5,
        [Cmd(1014, 0x03, "FlightController", 0xf6, "Push Param PC Log")]
        FlightController_PushParamPCLog = 0x03f6,
        [Cmd(1015, 0x03, "FlightController", 0xf7, "Config Table: Get Param Info By Hash")]
        FlightController_ConfigTableGetParamInfoByHash = 0x03f7,
        [Cmd(1016, 0x03, "FlightController", 0xf8, "Config Table: Read Param By Hash")]
        FlightController_ConfigTableReadParamByHash = 0x03f8,
        [Cmd(1017, 0x03, "FlightController", 0xf9, "Config Table: Write Param By Hash")]
        FlightController_ConfigTableWriteParamByHash = 0x03f9,
        [Cmd(1018, 0x03, "FlightController", 0xfa, "Config Table: Reset Params By Hash")]
        FlightController_ConfigTableResetParamsByHash = 0x03fa,
        [Cmd(1019, 0x03, "FlightController", 0xfb, "Config Table: Read Params By Hash")]
        FlightController_ConfigTableReadParamsByHash = 0x03fb,
        [Cmd(1020, 0x03, "FlightController", 0xfc, "Config Table: Write Params By Hash")]
        FlightController_ConfigTableWriteParamsByHash = 0x03fc,
        [Cmd(1021, 0x03, "FlightController", 0xfd, "Product Type Get")]
        FlightController_ProductTypeGet = 0x03fd,
        [Cmd(1022, 0x03, "FlightController", 0xfe, "Motor Force Disable Set")]
        FlightController_MotorForceDisableSet = 0x03fe,
        [Cmd(1023, 0x03, "FlightController", 0xff, "Motor Force Disable Get")]
        FlightController_MotorForceDisableGet = 0x03ff,

        // [4] https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L494
        // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-gimbal.lua#L9
        [Cmd(1024, 0x04, "Gimbal", 0x00, "Gimbal Reserved")]
        Gimbal_GimbalReserved = 0x0400,
        [Cmd(1025, 0x04, "Gimbal", 0x01, "Gimbal Control")]
        Gimbal_GimbalControl = 0x0401,
        [Cmd(1026, 0x04, "Gimbal", 0x02, "Gimbal Get Position")]
        Gimbal_GimbalGetPosition = 0x0402,
        [Cmd(1027, 0x04, "Gimbal", 0x03, "Gimbal Set Param")]
        Gimbal_GimbalSetParam = 0x0403,
        [Cmd(1028, 0x04, "Gimbal", 0x04, "Gimbal Get Param")]
        Gimbal_GimbalGetParam = 0x0404,
        [Cmd(1029, 0x04, "Gimbal", 0x05, "Gimbal Params Get")]
        Gimbal_GimbalParamsGet = 0x0405,
        [Cmd(1030, 0x04, "Gimbal", 0x06, "Gimbal Push AETR")]
        Gimbal_GimbalPushAETR = 0x0406,
        [Cmd(1031, 0x04, "Gimbal", 0x07, "Gimbal Adjust Roll")]
        Gimbal_GimbalAdjustRoll = 0x0407,
        [Cmd(1032, 0x04, "Gimbal", 0x08, "Gimbal Calibration")]
        Gimbal_GimbalCalibration = 0x0408,
        [Cmd(1033, 0x04, "Gimbal", 0x09, "Gimbal Reserved2")]
        Gimbal_GimbalReserved2 = 0x0409,
        [Cmd(1034, 0x04, "Gimbal", 0x0a, "Gimbal Ext Ctrl Degree")]
        Gimbal_GimbalExtCtrlDegree = 0x040a,
        [Cmd(1035, 0x04, "Gimbal", 0x0b, "Gimbal Get Ext Ctrl Status")]
        Gimbal_GimbalGetExtCtrlStatus = 0x040b,
        [Cmd(1036, 0x04, "Gimbal", 0x0c, "Gimbal Ext Ctrl Accel")]
        Gimbal_GimbalExtCtrlAccel = 0x040c,
        [Cmd(1037, 0x04, "Gimbal", 0x0d, "Gimbal Suspend/Resume")]
        Gimbal_GimbalSuspendResume = 0x040d,
        [Cmd(1038, 0x04, "Gimbal", 0x0e, "Gimbal Thirdp Magn")]
        Gimbal_GimbalThirdpMagn = 0x040e,
        [Cmd(1039, 0x04, "Gimbal", 0x0f, "Gimbal User Params Set")]
        Gimbal_GimbalUserParamsSet = 0x040f,
        [Cmd(1040, 0x04, "Gimbal", 0x10, "Gimbal User Params Get")]
        Gimbal_GimbalUserParamsGet1 = 0x0410,
        [Cmd(1041, 0x04, "Gimbal", 0x11, "Gimbal User Params Save")]
        Gimbal_GimbalUserParamsSave = 0x0411,
        [Cmd(1043, 0x04, "Gimbal", 0x13, "Gimbal User Params Reset Default")]
        Gimbal_GimbalUserParamsResetDefault = 0x0413,
        [Cmd(1044, 0x04, "Gimbal", 0x14, "Gimbal Abs Angle Control")]
        Gimbal_GimbalAbsAngleControl = 0x0414,
        [Cmd(1045, 0x04, "Gimbal", 0x15, "Gimbal Movement")]
        Gimbal_GimbalMovement = 0x0415,
        [Cmd(1052, 0x04, "Gimbal", 0x1c, "Gimbal Type Get")]
        Gimbal_GimbalTypeGet = 0x041c,
        [Cmd(1054, 0x04, "Gimbal", 0x1e, "Gimbal Degree Info Subscription")]
        Gimbal_GimbalDegreeInfoSubscription = 0x041e,
        [Cmd(1056, 0x04, "Gimbal", 0x20, "Gimbal TBD 20")]
        Gimbal_GimbalTBD20 = 0x0420,
        [Cmd(1057, 0x04, "Gimbal", 0x21, "Gimbal TBD 21")]
        Gimbal_GimbalTBD21 = 0x0421,
        [Cmd(1060, 0x04, "Gimbal", 0x24, "Gimbal User Params Get")]
        Gimbal_GimbalUserParamsGet2 = 0x0424,
        [Cmd(1063, 0x04, "Gimbal", 0x27, "Gimbal Abnormal Status Get")]
        Gimbal_GimbalAbnormalStatusGet = 0x0427,
        [Cmd(1067, 0x04, "Gimbal", 0x2b, "Gimbal Tutorial Status Get")]
        Gimbal_GimbalTutorialStatusGet = 0x042b,
        [Cmd(1068, 0x04, "Gimbal", 0x2c, "Gimbal Tutorial Step Set")]
        Gimbal_GimbalTutorialStepSet = 0x042c,
        [Cmd(1072, 0x04, "Gimbal", 0x30, "Gimbal Auto Calibration Status")]
        Gimbal_GimbalAutoCalibrationStatus = 0x0430,
        [Cmd(1073, 0x04, "Gimbal", 0x31, "Robin Params Set")]
        Gimbal_RobinParamsSet = 0x0431,
        [Cmd(1074, 0x04, "Gimbal", 0x32, "Robin Params Get")]
        Gimbal_RobinParamsGet = 0x0432,
        [Cmd(1075, 0x04, "Gimbal", 0x33, "Robin Battery Info Push")]
        Gimbal_RobinBatteryInfoPush = 0x0433,
        [Cmd(1076, 0x04, "Gimbal", 0x34, "Gimbal Handle Params Set")]
        Gimbal_GimbalHandleParamsSet = 0x0434,
        [Cmd(1078, 0x04, "Gimbal", 0x36, "Gimbal Handle Params Get")]
        Gimbal_GimbalHandleParamsGet = 0x0436,
        [Cmd(1079, 0x04, "Gimbal", 0x37, "Gimbal Timelapse Params Set")]
        Gimbal_GimbalTimelapseParamsSet = 0x0437,
        [Cmd(1080, 0x04, "Gimbal", 0x38, "Gimbal Timelapse Status")]
        Gimbal_GimbalTimelapseStatus = 0x0438,
        [Cmd(1081, 0x04, "Gimbal", 0x39, "Gimbal Lock")]
        Gimbal_GimbalLock = 0x0439,
        [Cmd(1082, 0x04, "Gimbal", 0x3a, "Gimbal Rotate Camera X Axis")]
        Gimbal_GimbalRotateCameraXAxis = 0x043a,
        [Cmd(1093, 0x04, "Gimbal", 0x45, "Gimbal Get Temp")]
        Gimbal_GimbalGetTemp = 0x0445,
        [Cmd(1095, 0x04, "Gimbal", 0x47, "Gimbal TBD 47")]
        Gimbal_GimbalTBD47 = 0x0447,
        [Cmd(1100, 0x04, "Gimbal", 0x4c, "Gimbal Reset And Set Mode")]
        Gimbal_GimbalResetAndSetMode = 0x044c,
        [Cmd(1110, 0x04, "Gimbal", 0x56, "Gimbal NotiFy Camera Id")]
        Gimbal_GimbalNotiFyCameraId = 0x0456,
        [Cmd(1111, 0x04, "Gimbal", 0x57, "Handheld Stick State Get/Push")]
        Gimbal_HandheldStickStateGetPush = 0x0457,
        [Cmd(1112, 0x04, "Gimbal", 0x58, "Handheld Stick Control Set")]
        Gimbal_HandheldStickControlSet = 0x0458,

        // [5] https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L494
        // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L110
        // ToDo: Verify whether we do have to account the 'new platform' values
        [Cmd(1280, 0x05, "CenterBoard", 0x00, "Center Open/Close Virtual RC")]
        CenterBoard_CenterOpenCloseVirtualRC = 0x0500,
        [Cmd(1281, 0x05, "CenterBoard", 0x01, "Center Req Batt Info Confirm")]
        CenterBoard_CenterReqBattInfoConfirm = 0x0501,
        [Cmd(1282, 0x05, "CenterBoard", 0x02, "Center Push Batt Dynamic Info")]
        CenterBoard_CenterPushBattDynamicInfo = 0x0502,
        [Cmd(1283, 0x05, "CenterBoard", 0x03, "Center Control Uav Status Led")]
        CenterBoard_CenterControlUavStatusLed = 0x0503,
        [Cmd(1284, 0x05, "CenterBoard", 0x04, "Center Transform Control")]
        CenterBoard_CenterTransformControl = 0x0504,
        [Cmd(1285, 0x05, "CenterBoard", 0x05, "Center Req Push Bat Normal Data")]
        CenterBoard_CenterReqPushBatNormalData = 0x0505,
        [Cmd(1286, 0x05, "CenterBoard", 0x06, "Center Battery Common")]
        CenterBoard_CenterBatteryCommon = 0x0506,
        [Cmd(1287, 0x05, "CenterBoard", 0x07, "Center Query Bat Status")]
        CenterBoard_CenterQueryBatStatus = 0x0507,
        [Cmd(1288, 0x05, "CenterBoard", 0x08, "Center Query Bat Hisoty Status")]
        CenterBoard_CenterQueryBatHisotyStatus = 0x0508,
        [Cmd(1289, 0x05, "CenterBoard", 0x09, "Center Bat SelfDischarge Days")]
        CenterBoard_CenterBatSelfDischargeDays = 0x0509,
        [Cmd(1290, 0x05, "CenterBoard", 0x0a, "Center Bat Storage Info")]
        CenterBoard_CenterBatStorageInfo = 0x050a,
        [Cmd(1313, 0x05, "CenterBoard", 0x21, "Center Req Bat Static Data")]
        CenterBoard_CenterReqBatStaticData = 0x0521,
        [Cmd(1314, 0x05, "CenterBoard", 0x22, "Center Req Bat Dynamic Data")]
        CenterBoard_CenterReqBatDynamicData = 0x0522,
        [Cmd(1315, 0x05, "CenterBoard", 0x23, "Center Req Bat Auth Data")]
        CenterBoard_CenterReqBatAuthData = 0x0523,
        [Cmd(1316, 0x05, "CenterBoard", 0x24, "Center Req Bat Auth Result")]
        CenterBoard_CenterReqBatAuthResult = 0x0524,
        [Cmd(1329, 0x05, "CenterBoard", 0x31, "Center Req Bat SelfDischarge Time")]
        CenterBoard_CenterReqBatSelfDischargeTime = 0x0531,
        [Cmd(1330, 0x05, "CenterBoard", 0x32, "Center Set Bat SelfDischarge Time")]
        CenterBoard_CenterSetBatSelfDischargeTime = 0x0532,
        [Cmd(1331, 0x05, "CenterBoard", 0x33, "Center Req Bat Barcode")]
        CenterBoard_CenterReqBatBarcode = 0x0533,

        // [6] https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L494
        // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L132
        [Cmd(1537, 0x06, "RemoteControl", 0x01, "RC Channel Params Get")]
        RemoteControl_RCChannelParamsGet = 0x0601,
        [Cmd(1538, 0x06, "RemoteControl", 0x02, "RC Channel Params Set")]
        RemoteControl_RCChannelParamsSet = 0x0602,
        [Cmd(1539, 0x06, "RemoteControl", 0x03, "RC Calibiration Set")]
        RemoteControl_RCCalibirationSet = 0x0603,
        [Cmd(1540, 0x06, "RemoteControl", 0x04, "RC Physical Channel Parameter Get")]
        RemoteControl_RCPhysicalChannelParameterGet = 0x0604,
        [Cmd(1541, 0x06, "RemoteControl", 0x05, "RC Parameter Get/Push")]
        RemoteControl_RCParameterGetPush = 0x0605,
        [Cmd(1542, 0x06, "RemoteControl", 0x06, "RC Master/Slave Mode Set")]
        RemoteControl_RCMasterSlaveModeSet = 0x0606,
        [Cmd(1543, 0x06, "RemoteControl", 0x07, "RC Master/Slave Mode Get")]
        RemoteControl_RCMasterSlaveModeGet = 0x0607,
        [Cmd(1544, 0x06, "RemoteControl", 0x08, "RC Name Set")]
        RemoteControl_RCNameSet = 0x0608,
        [Cmd(1545, 0x06, "RemoteControl", 0x09, "RC Name Get")]
        RemoteControl_RCNameGet = 0x0609,
        [Cmd(1546, 0x06, "RemoteControl", 0x0a, "RC Password Set")]
        RemoteControl_RCPasswordSet = 0x060a,
        [Cmd(1547, 0x06, "RemoteControl", 0x0b, "RC Password Get")]
        RemoteControl_RCPasswordGet = 0x060b,
        [Cmd(1548, 0x06, "RemoteControl", 0x0c, "RC Connected Master Id Set")]
        RemoteControl_RCConnectedMasterIdSet = 0x060c,
        [Cmd(1549, 0x06, "RemoteControl", 0x0d, "RC Connected Master Id Get")]
        RemoteControl_RCConnectedMasterIdGet = 0x060d,
        [Cmd(1550, 0x06, "RemoteControl", 0x0e, "RC Available Master Id Get")]
        RemoteControl_RCAvailableMasterIdGet = 0x060e,
        [Cmd(1551, 0x06, "RemoteControl", 0x0f, "RC Search Mode Set")]
        RemoteControl_RCSearchModeSet = 0x060f,
        [Cmd(1552, 0x06, "RemoteControl", 0x10, "RC Search Mode Get")]
        RemoteControl_RCSearchModeGet = 0x0610,
        [Cmd(1553, 0x06, "RemoteControl", 0x11, "RC Master/Slave Switch Set")]
        RemoteControl_RCMasterSlaveSwitchSet = 0x0611,
        [Cmd(1554, 0x06, "RemoteControl", 0x12, "RC Master/Slave Switch Conf Get")]
        RemoteControl_RCMasterSlaveSwitchConfGet = 0x0612,
        [Cmd(1555, 0x06, "RemoteControl", 0x13, "RC Request Join By Slave")]
        RemoteControl_RCRequestJoinBySlave = 0x0613,
        [Cmd(1556, 0x06, "RemoteControl", 0x14, "RC List Request Join Slave")]
        RemoteControl_RCListRequestJoinSlave = 0x0614,
        [Cmd(1557, 0x06, "RemoteControl", 0x15, "RC Delete Slave")]
        RemoteControl_RCDeleteSlave = 0x0615,
        [Cmd(1558, 0x06, "RemoteControl", 0x16, "RC Delete Master")]
        RemoteControl_RCDeleteMaster = 0x0616,
        [Cmd(1559, 0x06, "RemoteControl", 0x17, "RC Slave Control Right Set")]
        RemoteControl_RCSlaveControlRightSet = 0x0617,
        [Cmd(1560, 0x06, "RemoteControl", 0x18, "RC Slave Control Right Get")]
        RemoteControl_RCSlaveControlRightGet = 0x0618,
        [Cmd(1561, 0x06, "RemoteControl", 0x19, "RC Control Mode Set")]
        RemoteControl_RCControlModeSet = 0x0619,
        [Cmd(1562, 0x06, "RemoteControl", 0x1a, "RC Control Mode Get")]
        RemoteControl_RCControlModeGet = 0x061a,
        [Cmd(1563, 0x06, "RemoteControl", 0x1b, "RC GPS Info Get/Push")]
        RemoteControl_RCGPSInfoGetPush = 0x061b,
        [Cmd(1564, 0x06, "RemoteControl", 0x1c, "RC RTC Info Get/Push")]
        RemoteControl_RCRTCInfoGetPush = 0x061c,
        [Cmd(1565, 0x06, "RemoteControl", 0x1d, "RC Temperature Info Get/Push")]
        RemoteControl_RCTemperatureInfoGetPush = 0x061d,
        [Cmd(1566, 0x06, "RemoteControl", 0x1e, "RC Battery Info Get/Push")]
        RemoteControl_RCBatteryInfoGetPush = 0x061e,
        [Cmd(1567, 0x06, "RemoteControl", 0x1f, "RC Master/Slave Conn Info Get/Push")]
        RemoteControl_RCMasterSlaveConnInfoGetPush = 0x061f,
        [Cmd(1568, 0x06, "RemoteControl", 0x20, "RC Power Mode CE/FCC Set")]
        RemoteControl_RCPowerModeCEFCCSet = 0x0620,
        [Cmd(1569, 0x06, "RemoteControl", 0x21, "RC Power Mode CE/FCC Get")]
        RemoteControl_RCPowerModeCEFCCGet = 0x0621,
        [Cmd(1570, 0x06, "RemoteControl", 0x22, "RC Gimbal Ctr Permission Request")]
        RemoteControl_RCGimbalCtrPermissionRequest = 0x0622,
        [Cmd(1571, 0x06, "RemoteControl", 0x23, "RC Gimbal Ctr Permission Ack")]
        RemoteControl_RCGimbalCtrPermissionAck = 0x0623,
        [Cmd(1572, 0x06, "RemoteControl", 0x24, "RC Simulate Flight Mode Set")]
        RemoteControl_RCSimulateFlightModeSet = 0x0624,
        [Cmd(1573, 0x06, "RemoteControl", 0x25, "RC Simulate Flight Mode Get")]
        RemoteControl_RCSimulateFlightModeGet = 0x0625,
        [Cmd(1574, 0x06, "RemoteControl", 0x26, "RC AETR Value Push")]
        RemoteControl_RCAETRValuePush = 0x0626,
        [Cmd(1575, 0x06, "RemoteControl", 0x27, "RC Detection Info Get")]
        RemoteControl_RCDetectionInfoGet = 0x0627,
        [Cmd(1576, 0x06, "RemoteControl", 0x28, "RC Gimbal Control Access Right Get")]
        RemoteControl_RCGimbalControlAccessRightGet = 0x0628,
        [Cmd(1577, 0x06, "RemoteControl", 0x29, "RC Slave Control Mode Set")]
        RemoteControl_RCSlaveControlModeSet = 0x0629,
        [Cmd(1578, 0x06, "RemoteControl", 0x2a, "RC Slave Control Mode Get")]
        RemoteControl_RCSlaveControlModeGet = 0x062a,
        [Cmd(1579, 0x06, "RemoteControl", 0x2b, "RC Gimbal Control Speed Set")]
        RemoteControl_RCGimbalControlSpeedSet = 0x062b,
        [Cmd(1580, 0x06, "RemoteControl", 0x2c, "RC Gimbal Control Speed Get")]
        RemoteControl_RCGimbalControlSpeedGet = 0x062c,
        [Cmd(1581, 0x06, "RemoteControl", 0x2d, "RC Self Defined Key Func Set")]
        RemoteControl_RCSelfDefinedKeyFuncSet = 0x062d,
        [Cmd(1582, 0x06, "RemoteControl", 0x2e, "RC Self Defined Key Func Get")]
        RemoteControl_RCSelfDefinedKeyFuncGet = 0x062e,
        [Cmd(1583, 0x06, "RemoteControl", 0x2f, "RC Pairing")]
        RemoteControl_RCPairing = 0x062f,
        [Cmd(1584, 0x06, "RemoteControl", 0x30, "RC Test GPS")]
        RemoteControl_RCTestGPS = 0x0630,
        [Cmd(1585, 0x06, "RemoteControl", 0x31, "RC RTC Clock Set")]
        RemoteControl_RCRTCClockSet = 0x0631,
        [Cmd(1586, 0x06, "RemoteControl", 0x32, "RC RTC Clock Get")]
        RemoteControl_RCRTCClockGet = 0x0632,
        [Cmd(1587, 0x06, "RemoteControl", 0x33, "RC Gimbal Control Sensitivity Set")]
        RemoteControl_RCGimbalControlSensitivitySet = 0x0633,
        [Cmd(1588, 0x06, "RemoteControl", 0x34, "RC Gimbal Control Sensitivity Get")]
        RemoteControl_RCGimbalControlSensitivityGet = 0x0634,
        [Cmd(1589, 0x06, "RemoteControl", 0x35, "RC Gimbal Control Mode Set")]
        RemoteControl_RCGimbalControlModeSet = 0x0635,
        [Cmd(1590, 0x06, "RemoteControl", 0x36, "RC Gimbal Control Mode Get")]
        RemoteControl_RCGimbalControlModeGet = 0x0636,
        [Cmd(1591, 0x06, "RemoteControl", 0x37, "RC Enter App Mode Request")]
        RemoteControl_RCEnterAppModeRequest = 0x0637,
        [Cmd(1592, 0x06, "RemoteControl", 0x38, "RC Calibration Value Get")]
        RemoteControl_RCCalibrationValueGet = 0x0638,
        [Cmd(1593, 0x06, "RemoteControl", 0x39, "RC Master Slave Connect Status Push")]
        RemoteControl_RCMasterSlaveConnectStatusPush = 0x0639,
        [Cmd(1594, 0x06, "RemoteControl", 0x3a, "RC 2014 Usb Mode Set")]
        RemoteControl_RCUsbModeSet = 0x063a,
        [Cmd(1595, 0x06, "RemoteControl", 0x3b, "RC Id Set")]
        RemoteControl_RCIdSet = 0x063b,
        [Cmd(1596, 0x06, "RemoteControl", 0x3c, "RC Coach Mode")]
        RemoteControl_RCCoachMode = 0x063c,
        [Cmd(1599, 0x06, "RemoteControl", 0x3f, "RC Mater/Slave Id")]
        RemoteControl_RCMaterSlaveId = 0x063f,
        [Cmd(1602, 0x06, "RemoteControl", 0x42, "RC Follow Focus Get/Push")]
        RemoteControl_RCFollowFocusGetPush = 0x0642,
        [Cmd(1607, 0x06, "RemoteControl", 0x47, "RC App Special Control")]
        RemoteControl_RCAppSpecialControl = 0x0647,
        [Cmd(1608, 0x06, "RemoteControl", 0x48, "RC Freq Mode Info Get")]
        RemoteControl_RCFreqModeInfoGet = 0x0648,
        [Cmd(1612, 0x06, "RemoteControl", 0x4c, "RC Pro Custom Buttons Status Get/Push")]
        RemoteControl_RCProCustomButtonsStatusGetPush = 0x064c,
        [Cmd(1616, 0x06, "RemoteControl", 0x50, "RC Push Rmc Key Info")]
        RemoteControl_RCPushRmcKeyInfo = 0x0650,
        [Cmd(1617, 0x06, "RemoteControl", 0x51, "RC Push To Glass")]
        RemoteControl_RCPushToGlass = 0x0651,
        [Cmd(1618, 0x06, "RemoteControl", 0x52, "RC Push LCD To MCU")]
        RemoteControl_RCPushLCDToMCU = 0x0652,
        [Cmd(1619, 0x06, "RemoteControl", 0x53, "RC Unit Language Get")]
        RemoteControl_RCUnitLanguageGet = 0x0653,
        [Cmd(1620, 0x06, "RemoteControl", 0x54, "RC Unit Language Set")]
        RemoteControl_RCUnitLanguageSet = 0x0654,
        [Cmd(1621, 0x06, "RemoteControl", 0x55, "RC Test Mode Set")]
        RemoteControl_RCTestModeSet = 0x0655,
        [Cmd(1622, 0x06, "RemoteControl", 0x56, "RC Quiry Role")]
        RemoteControl_RCQuiryRole = 0x0656,
        [Cmd(1623, 0x06, "RemoteControl", 0x57, "RC Quiry Ms Link Status")]
        RemoteControl_RCQuiryMsLinkStatus = 0x0657,
        [Cmd(1624, 0x06, "RemoteControl", 0x58, "RC Work Function Set")]
        RemoteControl_RCWorkFunctionSet = 0x0658,
        [Cmd(1625, 0x06, "RemoteControl", 0x59, "RC Work Function Get")]
        RemoteControl_RCWorkFunctionGet = 0x0659,
        [Cmd(1688, 0x06, "RemoteControl", 0x98, "Follow Focus2 Get/Push")]
        RemoteControl_FollowFocusGetPush = 0x0698,
        [Cmd(1689, 0x06, "RemoteControl", 0x99, "Follow Focus Info Set")]
        RemoteControl_FollowFocusInfoSet = 0x0699,
        [Cmd(1776, 0x06, "RemoteControl", 0xf0, "RC RF Cert Config Set")]
        RemoteControl_RCRFCertConfigSet = 0x06f0,
        [Cmd(1781, 0x06, "RemoteControl", 0xf5, "RC Test Stick Value")]
        RemoteControl_RCTestStickValue = 0x06f5,
        [Cmd(1782, 0x06, "RemoteControl", 0xf6, "RC Factory Get Board Id")]
        RemoteControl_RCFactoryGetBoardId = 0x06f6,
        [Cmd(1783, 0x06, "RemoteControl", 0xf7, "RC Push Buzzer To MCU")]
        RemoteControl_RCPushBuzzerToMCU = 0x06f7,
        [Cmd(1784, 0x06, "RemoteControl", 0xf8, "RC Stick Verification Data Get")]
        RemoteControl_RCStickVerificationDataGet = 0x06f8,
        [Cmd(1785, 0x06, "RemoteControl", 0xf9, "RC Post Calibiration Set")]
        RemoteControl_RCPostCalibirationSet = 0x06f9,
        [Cmd(1786, 0x06, "RemoteControl", 0xfa, "RC Stick Middle Value Get")]
        RemoteControl_RCStickMiddleValueGet = 0x06fa,

        // [7] https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L494
        // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L219
        [Cmd(1792, 0x07, "Wifi", 0x00, "WiFi Reserved")]
        Wifi_WiFiReserved = 0x0700,
        [Cmd(1793, 0x07, "Wifi", 0x01, "WiFi Ap Scan Results Push")]
        Wifi_WiFiApScanResultsPush = 0x0701,
        [Cmd(1794, 0x07, "Wifi", 0x02, "WiFi Ap Channel SNR Get")]
        Wifi_WiFiApChannelSNRGet = 0x0702,
        [Cmd(1795, 0x07, "Wifi", 0x03, "WiFi Ap Channel Set")]
        Wifi_WiFiApChannelSet = 0x0703,
        [Cmd(1796, 0x07, "Wifi", 0x04, "WiFi Ap Channel Get")]
        Wifi_WiFiApChannelGet = 0x0704,
        [Cmd(1797, 0x07, "Wifi", 0x05, "WiFi Ap Tx Pwr Set")]
        Wifi_WiFiApTxPwrSet = 0x0705,
        [Cmd(1798, 0x07, "Wifi", 0x06, "WiFi Ap Tx Pwr Get")]
        Wifi_WiFiApTxPwrGet = 0x0706,
        [Cmd(1799, 0x07, "Wifi", 0x07, "WiFi Ap SSID Get")]
        Wifi_WiFiApSSIDGet = 0x0707,
        [Cmd(1800, 0x07, "Wifi", 0x08, "WiFi Ap SSID Set")]
        Wifi_WiFiApSSIDSet = 0x0708,
        [Cmd(1801, 0x07, "Wifi", 0x09, "WiFi Ap RSSI Push")]
        Wifi_WiFiApRSSIPush = 0x0709,
        [Cmd(1802, 0x07, "Wifi", 0x0a, "WiFi Ap Ant RSSI Get")]
        Wifi_WiFiApAntRSSIGet = 0x070a,
        [Cmd(1803, 0x07, "Wifi", 0x0b, "WiFi Ap Mac Addr Set")]
        Wifi_WiFiApMacAddrSet = 0x070b,
        [Cmd(1804, 0x07, "Wifi", 0x0c, "WiFi Ap Mac Addr Get")]
        Wifi_WiFiApMacAddrGet = 0x070c,
        [Cmd(1805, 0x07, "Wifi", 0x0d, "WiFi Ap Passphrase Set")]
        Wifi_WiFiApPassphraseSet = 0x070d,
        [Cmd(1806, 0x07, "Wifi", 0x0e, "WiFi Ap Passphrase Get")]
        Wifi_WiFiApPassphraseGet = 0x070e,
        [Cmd(1807, 0x07, "Wifi", 0x0f, "WiFi Ap Factory Reset")]
        Wifi_WiFiApFactoryReset = 0x070f,
        [Cmd(1808, 0x07, "Wifi", 0x10, "WiFi Ap Band Set")]
        Wifi_WiFiApBandSet = 0x0710,
        [Cmd(1809, 0x07, "Wifi", 0x11, "WiFi Ap Sta MAC Push")]
        Wifi_WiFiApStaMACPush = 0x0711,
        [Cmd(1810, 0x07, "Wifi", 0x12, "WiFi Ap Phy Param Get")]
        Wifi_WiFiApPhyParamGet = 0x0712,
        [Cmd(1811, 0x07, "Wifi", 0x13, "WiFi Ap Power Mode Set")]
        Wifi_WiFiApPowerModeSet = 0x0713,
        [Cmd(1812, 0x07, "Wifi", 0x14, "WiFi Ap Calibrate")]
        Wifi_WiFiApCalibrate = 0x0714,
        [Cmd(1813, 0x07, "Wifi", 0x15, "WiFi Ap Wifi Restart")]
        Wifi_WiFiApWifiRestart = 0x0715,
        [Cmd(1814, 0x07, "Wifi", 0x16, "WiFi Ap Selection Mode Set")]
        Wifi_WiFiApSelectionModeSet = 0x0716,
        [Cmd(1815, 0x07, "Wifi", 0x17, "WiFi Ap Selection Mode Get")]
        Wifi_WiFiApSelectionModeGet = 0x0717,
        [Cmd(1816, 0x07, "Wifi", 0x18, "WiFi Ap 18")]
        Wifi_WiFiAp = 0x0718,
        [Cmd(1817, 0x07, "Wifi", 0x19, "WiFi Ap 19")]
        Wifi_WiFiAp19 = 0x0719,
        [Cmd(1818, 0x07, "Wifi", 0x1a, "WiFi Ap 1A")]
        Wifi_WiFiApA = 0x071a,
        [Cmd(1819, 0x07, "Wifi", 0x1b, "WiFi Ap 1B")]
        Wifi_WiFiApB = 0x071b,
        [Cmd(1820, 0x07, "Wifi", 0x1c, "WiFi Ap 1C")]
        Wifi_WiFiApC = 0x071c,
        [Cmd(1821, 0x07, "Wifi", 0x1d, "WiFi Ap 1D")]
        Wifi_WiFiApD = 0x071d,
        [Cmd(1822, 0x07, "Wifi", 0x1e, "WiFi SSID Get")]
        Wifi_WiFiSSIDGet = 0x071e,
        [Cmd(1823, 0x07, "Wifi", 0x1f, "WiFi Ap 1F")]
        Wifi_WiFiApF = 0x071f,
        [Cmd(1824, 0x07, "Wifi", 0x20, "WiFi Ap Wifi Frequency Get")]
        Wifi_WiFiApWifiFrequencyGet = 0x0720,
        [Cmd(1825, 0x07, "Wifi", 0x21, "WiFi Ap Set Bw")]
        Wifi_WiFiApSetBw = 0x0721,
        [Cmd(1826, 0x07, "Wifi", 0x22, "WiFi Ap 22")]
        Wifi_WiFiAp22 = 0x0722,
        [Cmd(1827, 0x07, "Wifi", 0x23, "WiFi Ap 23")]
        Wifi_WiFiAp23 = 0x0723,
        [Cmd(1828, 0x07, "Wifi", 0x24, "WiFi Ap 24")]
        Wifi_WiFiAp24 = 0x0724,
        [Cmd(1829, 0x07, "Wifi", 0x25, "WiFi Ap 25")]
        Wifi_WiFiAp25 = 0x0725,
        [Cmd(1830, 0x07, "Wifi", 0x26, "WiFi Ap Realtime Acs")]
        Wifi_WiFiApRealtimeAcs = 0x0726,
        [Cmd(1831, 0x07, "Wifi", 0x27, "WiFi Ap Manual Switch SDR")]
        Wifi_WiFiApManualSwitchSDR = 0x0727,
        [Cmd(1832, 0x07, "Wifi", 0x28, "WiFi Ap Channel List Get/Push")]
        Wifi_WiFiApChannelListGetPush = 0x0728,
        [Cmd(1833, 0x07, "Wifi", 0x29, "WiFi Ap Channel Noise/SNR Req")]
        Wifi_WiFiApChannelNoiseSNRReq = 0x0729,
        [Cmd(1834, 0x07, "Wifi", 0x2a, "WiFi Ap Channel Noise/SNR Push")]
        Wifi_WiFiApChannelNoiseSNRPush = 0x072a,
        [Cmd(1835, 0x07, "Wifi", 0x2b, "WiFi Ap Set Hw Mode")]
        Wifi_WiFiApSetHwMode = 0x072b,
        [Cmd(1836, 0x07, "Wifi", 0x2c, "Wifi Ap Code Rate Set")]
        Wifi_WifiApCodeRateSet = 0x072c,
        [Cmd(1837, 0x07, "Wifi", 0x2d, "Wifi Ap Cur Code Rate Get")]
        Wifi_WifiApCurCodeRateGet = 0x072d,
        [Cmd(1838, 0x07, "Wifi", 0x2e, "WiFi Ap Set Usr Pref")]
        Wifi_WiFiApSetUsrPref = 0x072e,
        [Cmd(1839, 0x07, "Wifi", 0x2f, "WiFi Ap Get Usr Pref")]
        Wifi_WiFiApGetUsrPref = 0x072f,
        [Cmd(1840, 0x07, "Wifi", 0x30, "WiFi Ap Set Country Code")]
        Wifi_WiFiApSetCountryCode = 0x0730,
        [Cmd(1841, 0x07, "Wifi", 0x31, "WiFi Ap Reset Freq")]
        Wifi_WiFiApResetFreq = 0x0731,
        [Cmd(1842, 0x07, "Wifi", 0x32, "WiFi Ap Del Country Code")]
        Wifi_WiFiApDelCountryCode = 0x0732,
        [Cmd(1843, 0x07, "Wifi", 0x33, "WiFi Ap Verify Cc")]
        Wifi_WiFiApVerifyCc = 0x0733,
        [Cmd(1849, 0x07, "Wifi", 0x39, "WiFi Get Work Mode")]
        Wifi_WiFiGetWorkMode = 0x0739,
        [Cmd(1850, 0x07, "Wifi", 0x3a, "WiFi Set Work Mode")]
        Wifi_WiFiSetWorkMode = 0x073a,
        [Cmd(1851, 0x07, "Wifi", 0x3b, "WiFi Config By Qrcode")]
        Wifi_WiFiConfigByQrcode = 0x073b,
        [Cmd(1920, 0x07, "Wifi", 0x80, "WiFi Push Mac Stat")]
        Wifi_WiFiPushMacStat = 0x0780,
        [Cmd(1922, 0x07, "Wifi", 0x82, "WiFi Master/Slave Status Get/Push")]
        Wifi_WiFiMasterSlaveStatusGetPush = 0x0782,
        [Cmd(1923, 0x07, "Wifi", 0x83, "WiFi Master/Slave AuthCode Set")]
        Wifi_WiFiMasterSlaveAuthCodeSet = 0x0783,
        [Cmd(1924, 0x07, "Wifi", 0x84, "WiFi Scan Master List")]
        Wifi_WiFiScanMasterList = 0x0784,
        [Cmd(1925, 0x07, "Wifi", 0x85, "WiFi Connect Master With Id AuthCode")]
        Wifi_WiFiConnectMasterWithIdAuthCode = 0x0785,
        [Cmd(1929, 0x07, "Wifi", 0x89, "WiFi AuthCode Get")]
        Wifi_WiFiAuthCodeGet = 0x0789,
        [Cmd(1931, 0x07, "Wifi", 0x8b, "WiFi MS Error Info Get/Push")]
        Wifi_WiFiMSErrorInfoGetPush = 0x078b,
        [Cmd(1937, 0x07, "Wifi", 0x91, "WiFi Rc Info Set")]
        Wifi_WiFiRcInfoSet = 0x0791,
        [Cmd(1938, 0x07, "Wifi", 0x92, "WiFi Update Sw State")]
        Wifi_WiFiUpdateSwState = 0x0792,

        // [8] https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L494
        // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L286
        [Cmd(2048, 0x08, "DM36XTransmitter", 0x00, "DM36x Reserved")]
        DMX_DMXReserved = 0x0800,
        [Cmd(2049, 0x08, "DM36XTransmitter", 0x01, "DM36x Gnd Ctrl Info Send")]
        DMX_DMXGndCtrlInfoSend = 0x0801,
        [Cmd(2050, 0x08, "DM36XTransmitter", 0x02, "DM36x Gnd Ctrl Info Recv")]
        DMX_DMXGndCtrlInfoRecv = 0x0802,
        [Cmd(2051, 0x08, "DM36XTransmitter", 0x03, "DM36x UAV Ctrl Info Send")]
        DMX_DMXUAVCtrlInfoSend = 0x0803,
        [Cmd(2052, 0x08, "DM36XTransmitter", 0x04, "DM36x UAV Ctrl Info Recv")]
        DMX_DMXUAVCtrlInfoRecv = 0x0804,
        [Cmd(2053, 0x08, "DM36XTransmitter", 0x05, "DM36x Gnd Stat Info Send")]
        DMX_DMXGndStatInfoSend = 0x0805,
        [Cmd(2054, 0x08, "DM36XTransmitter", 0x06, "DM36x UAV Stat Info Send")]
        DMX_DMXUAVStatInfoSend = 0x0806,
        [Cmd(2053, 0x08, "DM36XTransmitter", 0x05, "DM36x Gnd Stat Info Recv")]
        DMX_DMXGndStatInfoRecv = 0x0807, // corrected from 0805 to 0807
        [Cmd(2062, 0x08, "DM36XTransmitter", 0x0e, "DM36x App Connect Stat Get")]
        DMX_DMXAppConnectStatGet = 0x080e,
        [Cmd(2063, 0x08, "DM36XTransmitter", 0x0f, "DM36x Recycle Vision Frame Info")]
        DMX_DMXRecycleVisionFrameInfo = 0x080f,
        [Cmd(2080, 0x08, "DM36XTransmitter", 0x20, "DM36x Bitrate Set")]
        DMX_DMXBitrateSet = 0x0820,
        [Cmd(2081, 0x08, "DM36XTransmitter", 0x21, "DM36x Bitrate Get")]
        DMX_DMXBitrateGet = 0x0821,
        [Cmd(2096, 0x08, "DM36XTransmitter", 0x30, "DM36x Foresight Showed Set")]
        DMX_DMXForesightShowedSet = 0x0830,
        [Cmd(2097, 0x08, "DM36XTransmitter", 0x31, "DM36x Foresight Showed Get")]
        DMX_DMXForesightShowedGet = 0x0831,
        [Cmd(2144, 0x08, "DM36XTransmitter", 0x60, "Active Track Camera Set")]
        DMX_ActiveTrackCameraSet = 0x0860,

        // [9] https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L494
        // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L304
        // ToDo: Verify whether we do have to account the 'new platform' values
        [Cmd(2305, 0x09, "HDLink", 0x01, "HDLnk OSD General Data Get/Push")]
        HDLink_HDLnkOSDGeneralDataGetPush = 0x0901,
        [Cmd(2306, 0x09, "HDLink", 0x02, "HDLnk OSD Home Point Get/Push")]
        HDLink_HDLnkOSDHomePointGetPush = 0x0902,
        [Cmd(2307, 0x09, "HDLink", 0x03, "HDLnk Baseband State Get/Push")]
        HDLink_HDLnkBasebandStateGetPush = 0x0903,
        [Cmd(2308, 0x09, "HDLink", 0x04, "HDLnk FPGA Write")]
        HDLink_HDLnkFPGAWrite = 0x0904,
        [Cmd(2309, 0x09, "HDLink", 0x05, "HDLnk FPGA Read")]
        HDLink_HDLnkFPGARead = 0x0905,
        [Cmd(2310, 0x09, "HDLink", 0x06, "HDLnk TCX Hardware Reg Write")]
        HDLink_HDLnkTCXHardwareRegWrite = 0x0906,
        [Cmd(2311, 0x09, "HDLink", 0x07, "HDLnk TCX Hardware Reg Read")]
        HDLink_HDLnkTCXHardwareRegRead = 0x0907,
        [Cmd(2312, 0x09, "HDLink", 0x08, "HDLnk VT Signal Quality Push")]
        HDLink_HDLnkVTSignalQualityPush = 0x0908,
        [Cmd(2313, 0x09, "HDLink", 0x09, "HDLnk Sweep Frequency Set")]
        HDLink_HDLnkSweepFrequencySet = 0x0909,
        [Cmd(2314, 0x09, "HDLink", 0x0a, "HDLnk Sweep Frequency Get/Push")]
        HDLink_HDLnkSweepFrequencyGetPush = 0x090a,
        [Cmd(2315, 0x09, "HDLink", 0x0b, "HDLnk Device Status Get/Push")]
        HDLink_HDLnkDeviceStatusGetPush = 0x090b,
        [Cmd(2316, 0x09, "HDLink", 0x0c, "HDLnk VT Config Info Get/Push")]
        HDLink_HDLnkVTConfigInfoGetPush = 0x090c,
        [Cmd(2317, 0x09, "HDLink", 0x0d, "HDLnk VT Config Info Set")]
        HDLink_HDLnkVTConfigInfoSet = 0x090d,
        [Cmd(2318, 0x09, "HDLink", 0x0e, "HDLnk USB Iface Change")]
        HDLink_HDLnkUSBIfaceChange = 0x090e,
        [Cmd(2319, 0x09, "HDLink", 0x0f, "HDLnk Reset Cy68013")]
        HDLink_HDLnkResetCy = 0x090f,
        [Cmd(2320, 0x09, "HDLink", 0x10, "HDLnk Upgrade Tip Set")]
        HDLink_HDLnkUpgradeTipSet = 0x0910,
        [Cmd(2321, 0x09, "HDLink", 0x11, "HDLnk Wl Env Quality Get/Push")]
        HDLink_HDLnkWlEnvQualityGetPush = 0x0911,
        [Cmd(2322, 0x09, "HDLink", 0x12, "HDLnk Factory Test Set")]
        HDLink_HDLnkFactoryTestSet = 0x0912,
        [Cmd(2323, 0x09, "HDLink", 0x13, "HDLnk Factory Test Get")]
        HDLink_HDLnkFactoryTestGet = 0x0913,
        [Cmd(2324, 0x09, "HDLink", 0x14, "HDLnk Max Video Bandwidth Set")]
        HDLink_HDLnkMaxVideoBandwidthSet = 0x0914,
        [Cmd(2325, 0x09, "HDLink", 0x15, "HDLnk Max Video Bandwidth Get/Push")]
        HDLink_HDLnkMaxVideoBandwidthGetPush = 0x0915,
        [Cmd(2326, 0x09, "HDLink", 0x16, "HDLnk Debug Info Push")]
        HDLink_HDLnkDebugInfoPush = 0x0916,
        [Cmd(2336, 0x09, "HDLink", 0x20, "HDLnk SDR Downward Sweep Frequency")]
        HDLink_HDLnkSDRDownwardSweepFrequency = 0x0920,
        [Cmd(2337, 0x09, "HDLink", 0x21, "HDLnk SDR Vt Config Info Get")]
        HDLink_HDLnkSDRVtConfigInfoGet = 0x0921,
        [Cmd(2338, 0x09, "HDLink", 0x22, "HDLnk SDR Dl Auto Vt Info Get/Push")]
        HDLink_HDLnkSDRDlAutoVtInfoGetPush = 0x0922,
        [Cmd(2339, 0x09, "HDLink", 0x23, "HDLnk SDR Rt Status Set")]
        HDLink_HDLnkSDRRtStatusSet = 0x0923,
        [Cmd(2340, 0x09, "HDLink", 0x24, "HDLnk SDR UAV Rt Status Get/Push")]
        HDLink_HDLnkSDRUAVRtStatusGetPush = 0x0924,
        [Cmd(2341, 0x09, "HDLink", 0x25, "HDLnk SDR Gnd Rt Status Get/Push")]
        HDLink_HDLnkSDRGndRtStatusGetPush = 0x0925,
        [Cmd(2342, 0x09, "HDLink", 0x26, "HDLnk SDR Debug/Assitant Read")]
        HDLink_HDLnkSDRDebugAssitantRead = 0x0926,
        [Cmd(2343, 0x09, "HDLink", 0x27, "HDLnk SDR Debug/Assitant Write")]
        HDLink_HDLnkSDRDebugAssitantWrite = 0x0927,
        [Cmd(2344, 0x09, "HDLink", 0x28, "HDLnk SDR Start Log Set")]
        HDLink_HDLnkSDRStartLogSet = 0x0928,
        [Cmd(2345, 0x09, "HDLink", 0x29, "HDLnk SDR Upward Sweep Frequency")]
        HDLink_HDLnkSDRUpwardSweepFrequency = 0x0929,
        [Cmd(2346, 0x09, "HDLink", 0x2a, "HDLnk SDR Upward Select Channel")]
        HDLink_HDLnkSDRUpwardSelectChannel = 0x092a,
        [Cmd(2347, 0x09, "HDLink", 0x2b, "HDLnk SDR Revert Role")]
        HDLink_HDLnkSDRRevertRole = 0x092b,
        [Cmd(2348, 0x09, "HDLink", 0x2c, "HDLnk SDR Amt Process")]
        HDLink_HDLnkSDRAmtProcess = 0x092c,
        [Cmd(2349, 0x09, "HDLink", 0x2d, "HDLnk SDR LBT Status Get")]
        HDLink_HDLnkSDRLBTStatusGet = 0x092d,
        [Cmd(2350, 0x09, "HDLink", 0x2e, "HDLnk SDR LBT Status Set")]
        HDLink_HDLnkSDRLBTStatusSet = 0x092e,
        [Cmd(2351, 0x09, "HDLink", 0x2f, "HDLnk SDR Link Test")]
        HDLink_HDLnkSDRLinkTest = 0x092f,
        [Cmd(2352, 0x09, "HDLink", 0x30, "HDLnk SDR Wireless Env State")]
        HDLink_HDLnkSDRWirelessEnvState = 0x0930,
        [Cmd(2353, 0x09, "HDLink", 0x31, "HDLnk SDR Scan Freq Cfg")]
        HDLink_HDLnkSDRScanFreqCfg = 0x0931,
        [Cmd(2354, 0x09, "HDLink", 0x32, "HDLnk SDR Factory Mode Set")]
        HDLink_HDLnkSDRFactoryModeSet = 0x0932,
        [Cmd(2355, 0x09, "HDLink", 0x33, "HDLnk Tracking State Ind")]
        HDLink_HDLnkTrackingStateInd = 0x0933,
        [Cmd(2356, 0x09, "HDLink", 0x34, "HDLnk SDR Liveview Mode Set")]
        HDLink_HDLnkSDRLiveviewModeSet = 0x0934,
        [Cmd(2357, 0x09, "HDLink", 0x35, "HDLnk SDR Liveview Mode Get")]
        HDLink_HDLnkSDRLiveviewModeGet = 0x0935,
        [Cmd(2358, 0x09, "HDLink", 0x36, "HDLnk SDR Liveview Rate Ind")]
        HDLink_HDLnkSDRLiveviewRateInd = 0x0936,
        [Cmd(2359, 0x09, "HDLink", 0x37, "HDLnk Abnormal Event Ind")]
        HDLink_HDLnkAbnormalEventInd = 0x0937,
        [Cmd(2360, 0x09, "HDLink", 0x38, "HDLnk SDR Set Rate")]
        HDLink_HDLnkSDRSetRate = 0x0938,
        [Cmd(2361, 0x09, "HDLink", 0x39, "HDLnk Liveview Config Set")]
        HDLink_HDLnkLiveviewConfigSet = 0x0939,
        [Cmd(2362, 0x09, "HDLink", 0x3a, "HDLnk Dl Freq Energy Push")]
        HDLink_HDLnkDlFreqEnergyPush = 0x093a,
        [Cmd(2363, 0x09, "HDLink", 0x3b, "HDLnk SDR Tip Interference")]
        HDLink_HDLnkSDRTipInterference = 0x093b,
        [Cmd(2364, 0x09, "HDLink", 0x3c, "HDLnk SDR Upgrade Rf Power")]
        HDLink_HDLnkSDRUpgradeRfPower = 0x093c,
        [Cmd(2366, 0x09, "HDLink", 0x3e, "HDLnk Slave RT Status Push")]
        HDLink_HDLnkSlaveRTStatusPush = 0x093e,
        [Cmd(2367, 0x09, "HDLink", 0x3f, "HDLnk RC Conn Status Push")]
        HDLink_HDLnkRCConnStatusPush = 0x093f,
        [Cmd(2369, 0x09, "HDLink", 0x41, "HDLnk Racing Set Modem Info")]
        HDLink_HDLnkRacingSetModemInfo = 0x0941,
        [Cmd(2370, 0x09, "HDLink", 0x42, "HDLnk Racing Get Modem Info")]
        HDLink_HDLnkRacingGetModemInfo = 0x0942,
        [Cmd(2384, 0x09, "HDLink", 0x50, "HDLnk LED Set")]
        HDLink_HDLnkLEDSet = 0x0950,
        [Cmd(2385, 0x09, "HDLink", 0x51, "HDLnk Power Set")]
        HDLink_HDLnkPowerSet = 0x0951,
        [Cmd(2386, 0x09, "HDLink", 0x52, "HDLnk Power Status Get/Push")]
        HDLink_HDLnkPowerStatusGetPush = 0x0952,
        [Cmd(2387, 0x09, "HDLink", 0x53, "HDLnk SDR Cp Status Get")]
        HDLink_HDLnkSDRCpStatusGet = 0x0953,
        [Cmd(2388, 0x09, "HDLink", 0x54, "Osmo Calibration Push")]
        HDLink_OsmoCalibrationPush = 0x0954,
        [Cmd(2391, 0x09, "HDLink", 0x57, "HDLnk Mic Gain Set")]
        HDLink_HDLnkMicGainSet = 0x0957,
        [Cmd(2392, 0x09, "HDLink", 0x58, "HDLnk Mic Gain Get")]
        HDLink_HDLnkMicGainGet = 0x0958,
        [Cmd(2393, 0x09, "HDLink", 0x59, "HDLnk Mic Info Get/Push")]
        HDLink_HDLnkMicInfoGetPush = 0x0959,
        [Cmd(2402, 0x09, "HDLink", 0x62, "HDLnk Mic Enable Get")]
        HDLink_HDLnkMicEnableGet = 0x0962,
        [Cmd(2403, 0x09, "HDLink", 0x63, "HDLnk Mic Enable Set")]
        HDLink_HDLnkMicEnableSet = 0x0963,
        [Cmd(2417, 0x09, "HDLink", 0x71, "HDLnk Main Camera Bandwidth Percent Set")]
        HDLink_HDLnkMainCameraBandwidthPercentSet = 0x0971,
        [Cmd(2418, 0x09, "HDLink", 0x72, "HDLnk Main Camera Bandwidth Percent Get")]
        HDLink_HDLnkMainCameraBandwidthPercentGet = 0x0972,

        // [A] https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L494
        // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L377
        [Cmd(2561, 0x0a, "MBINO", 0x01, "Eye Bino Info")]
        MBINO_EyeBinoInfo = 0x0a01,
        [Cmd(2562, 0x0a, "MBINO", 0x02, "Eye Mono Info")]
        MBINO_EyeMonoInfo = 0x0a02,
        [Cmd(2563, 0x0a, "MBINO", 0x03, "Eye Ultrasonic Info")]
        MBINO_EyeUltrasonicInfo = 0x0a03,
        [Cmd(2564, 0x0a, "MBINO", 0x04, "Eye Oa Info")]
        MBINO_EyeOaInfo = 0x0a04,
        [Cmd(2565, 0x0a, "MBINO", 0x05, "Eye Relitive Pos")]
        MBINO_EyeRelitivePos = 0x0a05,
        [Cmd(2566, 0x0a, "MBINO", 0x06, "Eye Avoidance Param")]
        MBINO_EyeAvoidanceParam = 0x0a06,
        [Cmd(2567, 0x0a, "MBINO", 0x07, "Eye Obstacle Info")]
        MBINO_EyeObstacleInfo = 0x0a07,
        [Cmd(2568, 0x0a, "MBINO", 0x08, "Eye TapGo Obst Avo Info")]
        MBINO_EyeTapGoObstAvoInfo = 0x0a08,
        [Cmd(2570, 0x0a, "MBINO", 0x0a, "Eye Push Vision Debug Info")]
        MBINO_EyePushVisionDebugInfo = 0x0a0a,
        [Cmd(2571, 0x0a, "MBINO", 0x0b, "Eye Push Control Debug Info")]
        MBINO_EyePushControlDebugInfo = 0x0a0b,
        [Cmd(2573, 0x0a, "MBINO", 0x0d, "Eye Track Log")]
        MBINO_EyeTrackLog = 0x0a0d,
        [Cmd(2574, 0x0a, "MBINO", 0x0e, "Eye Point Log")]
        MBINO_EyePointLog = 0x0a0e,
        [Cmd(2575, 0x0a, "MBINO", 0x0f, "Eye Push SDK Control Cmd")]
        MBINO_EyePushSDKControlCmd = 0x0a0f,
        [Cmd(2576, 0x0a, "MBINO", 0x10, "Eye Enable Tracking Taptogo")]
        MBINO_EyeEnableTrackingTaptogo = 0x0a10,
        [Cmd(2577, 0x0a, "MBINO", 0x11, "Eye Push Target Speed Pos Info")]
        MBINO_EyePushTargetSpeedPosInfo = 0x0a11,
        [Cmd(2578, 0x0a, "MBINO", 0x12, "Eye Push Target Pos Info")]
        MBINO_EyePushTargetPosInfo = 0x0a12,
        [Cmd(2579, 0x0a, "MBINO", 0x13, "Eye Push Trajectory")]
        MBINO_EyePushTrajectory = 0x0a13,
        [Cmd(2580, 0x0a, "MBINO", 0x14, "Eye Push Expected Speed Angle")]
        MBINO_EyePushExpectedSpeedAngle = 0x0a14,
        [Cmd(2581, 0x0a, "MBINO", 0x15, "Eye Receive Frame Info")]
        MBINO_EyeReceiveFrameInfo = 0x0a15,
        [Cmd(2585, 0x0a, "MBINO", 0x19, "Eye Flat Check")]
        MBINO_EyeFlatCheck = 0x0a19,
        [Cmd(2589, 0x0a, "MBINO", 0x1d, "Eye Fixed Wing Ctrl")]
        MBINO_EyeFixedWingCtrl = 0x0a1d,
        [Cmd(2590, 0x0a, "MBINO", 0x1e, "Eye Fixed Wing Status Push")]
        MBINO_EyeFixedWingStatusPush = 0x0a1e,
        [Cmd(2592, 0x0a, "MBINO", 0x20, "Eye Marquee Push")]
        MBINO_EyeMarqueePush = 0x0a20,
        [Cmd(2593, 0x0a, "MBINO", 0x21, "Eye Tracking Cnf Cancel")]
        MBINO_EyeTrackingCnfCancel = 0x0a21,
        [Cmd(2594, 0x0a, "MBINO", 0x22, "Eye Move Marquee Push")]
        MBINO_EyeMoveMarqueePush = 0x0a22,
        [Cmd(2595, 0x0a, "MBINO", 0x23, "Eye Tracking Status Push")]
        MBINO_EyeTrackingStatusPush = 0x0a23,
        [Cmd(2596, 0x0a, "MBINO", 0x24, "Eye Position Push")]
        MBINO_EyePositionPush = 0x0a24,
        [Cmd(2597, 0x0a, "MBINO", 0x25, "Eye Fly Ctl Push")]
        MBINO_EyeFlyCtlPush = 0x0a25,
        [Cmd(2598, 0x0a, "MBINO", 0x26, "Eye TapGo Status Push")]
        MBINO_EyeTapGoStatusPush = 0x0a26,
        [Cmd(2599, 0x0a, "MBINO", 0x27, "Eye Common Ctl Cmd")]
        MBINO_EyeCommonCtlCmd = 0x0a27,
        [Cmd(2600, 0x0a, "MBINO", 0x28, "Eye Get Para Cmd")]
        MBINO_EyeGetParaCmd = 0x0a28,
        [Cmd(2601, 0x0a, "MBINO", 0x29, "Eye Set Para Cmd")]
        MBINO_EyeSetParaCmd = 0x0a29,
        [Cmd(2602, 0x0a, "MBINO", 0x2a, "Eye Com Status Update")]
        MBINO_EyeComStatusUpdate = 0x0a2a,
        [Cmd(2604, 0x0a, "MBINO", 0x2c, "Eye Ta Lock Update")]
        MBINO_EyeTaLockUpdate = 0x0a2c,
        [Cmd(2605, 0x0a, "MBINO", 0x2d, "Eye Smart Landing")]
        MBINO_EyeSmartLanding = 0x0a2d,
        [Cmd(2606, 0x0a, "MBINO", 0x2e, "Eye Function List Push")]
        MBINO_EyeFunctionListPush = 0x0a2e,
        [Cmd(2607, 0x0a, "MBINO", 0x2f, "Eye Sensor Status Push")]
        MBINO_EyeSensorStatusPush = 0x0a2f,
        [Cmd(2608, 0x0a, "MBINO", 0x30, "Eye Self Calibration")]
        MBINO_EyeSelfCalibration = 0x0a30,
        [Cmd(2610, 0x0a, "MBINO", 0x32, "Eye Easy Self Calib State")]
        MBINO_EyeEasySelfCalibState = 0x0a32,
        [Cmd(2615, 0x0a, "MBINO", 0x37, "Eye QRCode Mode")]
        MBINO_EyeQRCodeMode = 0x0a37,
        [Cmd(2617, 0x0a, "MBINO", 0x39, "Eye Vision Tip")]
        MBINO_EyeVisionTip = 0x0a39,
        [Cmd(2618, 0x0a, "MBINO", 0x3a, "Eye Precise Landing Energy")]
        MBINO_EyePreciseLandingEnergy = 0x0a3a,
        [Cmd(2630, 0x0a, "MBINO", 0x46, "Eye RC Packet")]
        MBINO_EyeRCPacket = 0x0a46,
        [Cmd(2631, 0x0a, "MBINO", 0x47, "Eye Set Buffer Config")]
        MBINO_EyeSetBufferConfig = 0x0a47,
        [Cmd(2632, 0x0a, "MBINO", 0x48, "Eye Get Buffer Config")]
        MBINO_EyeGetBufferConfig = 0x0a48,
        [Cmd(2723, 0x0a, "MBINO", 0xa3, "Eye Enable SDK Func")]
        MBINO_EyeEnableSDKFunc = 0x0aa3,
        [Cmd(2724, 0x0a, "MBINO", 0xa4, "Eye Detection Msg Push")]
        MBINO_EyeDetectionMsgPush = 0x0aa4,
        [Cmd(2725, 0x0a, "MBINO", 0xa5, "Eye Get SDK Func")]
        MBINO_EyeGetSDKFunc = 0x0aa5,

        // [B] https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L494
        // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L428
        [Cmd(2817, 0x0b, "Simulation", 0x01, "Simu Connect Heart Packet")]
        Simulation_SimuConnectHeartPacket = 0x0b01,
        [Cmd(2818, 0x0b, "Simulation", 0x02, "Simu IMU Status Push")]
        Simulation_SimuIMUStatusPush = 0x0b02,
        [Cmd(2819, 0x0b, "Simulation", 0x03, "Simu SDR Status Push")]
        Simulation_SimuSDRStatusPush = 0x0b03,
        [Cmd(2820, 0x0b, "Simulation", 0x04, "Simu Get Headbelt SN")]
        Simulation_SimuGetHeadbeltSN = 0x0b04,
        [Cmd(2822, 0x0b, "Simulation", 0x06, "Simu Flight Status Params")]
        Simulation_SimuFlightStatusParams = 0x0b06,
        [Cmd(2823, 0x0b, "Simulation", 0x07, "Simu GetWind Set")]
        Simulation_SimuGetWindSet = 0x0b07,
        [Cmd(2824, 0x0b, "Simulation", 0x08, "Simu GetArea Set")]
        Simulation_SimuGetAreaSet = 0x0b08,
        [Cmd(2825, 0x0b, "Simulation", 0x09, "Simu GetAirParams Set")]
        Simulation_SimuGetAirParamsSet = 0x0b09,
        [Cmd(2826, 0x0b, "Simulation", 0x0a, "Simu Force Moment")]
        Simulation_SimuForceMoment = 0x0b0a,
        [Cmd(2827, 0x0b, "Simulation", 0x0b, "Simu GetTemperature Set")]
        Simulation_SimuGetTemperatureSet = 0x0b0b,
        [Cmd(2828, 0x0b, "Simulation", 0x0c, "Simu GetGravity Set")]
        Simulation_SimuGetGravitySet = 0x0b0c,
        [Cmd(2829, 0x0b, "Simulation", 0x0d, "Simu Crash ShutDown")]
        Simulation_SimuCrashShutDown = 0x0b0d,
        [Cmd(2830, 0x0b, "Simulation", 0x0e, "Simu Ctrl Motor")]
        Simulation_SimuCtrlMotor = 0x0b0e,
        [Cmd(2831, 0x0b, "Simulation", 0x0f, "Simu Momentum")]
        Simulation_SimuMomentum = 0x0b0f,
        [Cmd(2832, 0x0b, "Simulation", 0x10, "Simu GetArmLength Set")]
        Simulation_SimuGetArmLengthSet = 0x0b10,
        [Cmd(2833, 0x0b, "Simulation", 0x11, "Simu GetMassInertia Set")]
        Simulation_SimuGetMassInertiaSet = 0x0b11,
        [Cmd(2834, 0x0b, "Simulation", 0x12, "Simu GetMotorSetting Set")]
        Simulation_SimuGetMotorSettingSet = 0x0b12,
        [Cmd(2835, 0x0b, "Simulation", 0x13, "Simu GetBatterySetting Set")]
        Simulation_SimuGetBatterySettingSet = 0x0b13,
        [Cmd(2836, 0x0b, "Simulation", 0x14, "Simu Frequency Get")]
        Simulation_SimuFrequencyGet = 0x0b14,
        [Cmd(2842, 0x0b, "Simulation", 0x1a, "Simu Set Sim Vision Mode")]
        Simulation_SimuSetSimVisionMode = 0x0b1a,
        [Cmd(2843, 0x0b, "Simulation", 0x1b, "Simu Get Sim Vision Mode")]
        Simulation_SimuGetSimVisionMode = 0x0b1b,

        // [C] https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L494
        // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L452

        // [D] https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L494
        // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L455
        [Cmd(3329, 0x0d, "Battery", 0x01, "Battery Static Data Get")]
        Battery_BatteryStaticDataGet = 0x0d01,
        [Cmd(3330, 0x0d, "Battery", 0x02, "Battery Dynamic Data Get/Push")]
        Battery_BatteryDynamicDataGetPush = 0x0d02,
        [Cmd(3331, 0x0d, "Battery", 0x03, "Battery Cell Voltage Get/Push")]
        Battery_BatteryCellVoltageGetPush = 0x0d03,
        [Cmd(3332, 0x0d, "Battery", 0x04, "Battery BarCode Data Get")]
        Battery_BatteryBarCodeDataGet = 0x0d04,
        [Cmd(3333, 0x0d, "Battery", 0x05, "Battery History Get")]
        Battery_BatteryHistoryGet = 0x0d05,
        [Cmd(3334, 0x0d, "Battery", 0x06, "Battery Push Common Info")]
        Battery_BatteryPushCommonInfo = 0x0d06,
        [Cmd(3345, 0x0d, "Battery", 0x11, "Battery SetSelfDischargeDays Get")]
        Battery_BatterySetSelfDischargeDaysGet = 0x0d11,
        [Cmd(3346, 0x0d, "Battery", 0x12, "Battery ShutDown")]
        Battery_BatteryShutDown = 0x0d12,
        [Cmd(3347, 0x0d, "Battery", 0x13, "Battery Force ShutDown")]
        Battery_BatteryForceShutDown = 0x0d13,
        [Cmd(3348, 0x0d, "Battery", 0x14, "Battery StartUp")]
        Battery_BatteryStartUp = 0x0d14,
        [Cmd(3349, 0x0d, "Battery", 0x15, "Battery Pair Get")]
        Battery_BatteryPairGet = 0x0d15,
        [Cmd(3350, 0x0d, "Battery", 0x16, "Battery Pair Set")]
        Battery_BatteryPairSet = 0x0d16,
        [Cmd(3362, 0x0d, "Battery", 0x22, "Battery Data Record Control")]
        Battery_BatteryDataRecordControl = 0x0d22,
        [Cmd(3363, 0x0d, "Battery", 0x23, "Battery Authentication")]
        Battery_BatteryAuthentication = 0x0d23,
        [Cmd(3377, 0x0d, "Battery", 0x31, "Battery Re-Arrangement Get/Push")]
        Battery_BatteryReArrangementGetPush = 0x0d31,
        [Cmd(3378, 0x0d, "Battery", 0x32, "Battery Mult Battery Info Get")]
        Battery_BatteryMultBatteryInfoGet = 0x0d32,

        // [E] https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L494
        // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L474
        [Cmd(3618, 0x0e, "DataLog", 0x22, "DLog Battery Data")]
        DataLog_DLogBatteryData = 0x0e22,
        [Cmd(3619, 0x0e, "DataLog", 0x23, "DLog Battery Message")]
        DataLog_DLogBatteryMessage = 0x0e23,

        // [F] https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L494
        // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L479
        [Cmd(3849, 0x0f, "RTK", 0x09, "Rtk Status")]
        RTK_RtkStatus = 0x0f09,

        // [10] https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L494
        // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L479

        // [11] https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L494
        // https://github.com/o-gs/dji-firmware-tools/blob/05e24cb12803943f63ac5ae1574e517e59a2dd0a/comm_dissector/wireshark/dji-dumlv1-proto.lua#L486

        // error-correction
        // https://github.com/district-michael/fpv_live/blob/4c7bb40e5cc5daec67b39cc093235afb959a4bfe/src/main/java/dji/midware/data/config/P3/CmdIdADS_B.java#L17
        [Cmd(4354, 0x11, "ADSB", 0x02, "GetPushData")]
        ADSB_GetPushData = 0x1102,
        [Cmd(4360, 0x11, "ADSB", 0x08, "GetPushWarning")]
        ADSB_GetPushWarning = 0x1108,
        [Cmd(4361, 0x11, "ADSB", 0x09, "GetPushOriginal")]
        ADSB_GetPushOriginal = 0x1109,
        [Cmd(4368, 0x11, "ADSB", 0x10, "SendWhiteList")]
        ADSB_SendWhiteList = 0x1110,
        [Cmd(4369, 0x11, "ADSB", 0x11, "RequestLicense")]
        ADSB_RequestLicense = 0x1111,
        [Cmd(4370, 0x11, "ADSB", 0x12, "SetLicenseEnabled")]
        ADSB_SetLicenseEnabled = 0x1112,
        [Cmd(4371, 0x11, "ADSB", 0x13, "GetLicenseId")]
        ADSB_GetLicenseId = 0x1113,
        [Cmd(4372, 0x11, "ADSB", 0x14, "GetPushUnlockInfo")]
        ADSB_GetPushUnlockInfo = 0x1114,
        [Cmd(4373, 0x11, "ADSB", 0x15, "SetUserId")]
        ADSB_SetUserId = 0x1115,
        [Cmd(4374, 0x11, "ADSB", 0x16, "GetKeyVersion")]
        ADSB_GetKeyVersion = 0x1116,
        [Cmd(4375, 0x11, "ADSB", 0x17, "GetPushAvoidanceAction")]
        ADSB_GetPushAvoidanceAction = 0x1117,
    }

    public enum Participant
    {
        Drone,
        Operator
    }
}