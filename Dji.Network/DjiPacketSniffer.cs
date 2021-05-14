using Dji.Network.Packet;
using SharpPcap;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using WeakEvent;

namespace Dji.Network
{
    public class DjiPacketSniffer : IDisposable
    {
        private const string DRONE_IP_ADDRESS = "192.168.2.1";

        private readonly WeakEventSource<NetworkPacket> _networkPacketReceivedSource = new WeakEventSource<NetworkPacket>();
        private readonly WeakEventSource<DjiNetworkInformation> _networkInformationReceivedSource = new WeakEventSource<DjiNetworkInformation>();
        private readonly ConcurrentQueue<NetworkPacket> _networkPackets = new ConcurrentQueue<NetworkPacket>();
        private readonly ManualResetEvent _networkFrameAvailable = new ManualResetEvent(false);

        private List<ICaptureDevice> _captureDevices = new List<ICaptureDevice>();
        private SnifferState _snifferState = SnifferState.WaitingForDrone;
        private string _operatorIpAddress = string.Empty;

        private CancellationTokenSource _cancellationTokenSource;
        private Thread _networkFrameDeliveryThread;

        public DjiPacketSniffer()
        {
            Trace.TraceInformation($"{nameof(DjiPacketSniffer)} status {_snifferState} - Drone {DRONE_IP_ADDRESS}");

            // as we have no clue what the IP of the drone is,
            // we require to listen on all interfaces for incomming packets.
            _ = ListenOnAllInterfaces();
        }

        public event EventHandler<NetworkPacket> NetworkPacketReceived
        {
            add { _networkPacketReceivedSource.Subscribe(value); }
            remove { _networkPacketReceivedSource.Unsubscribe(value); }
        }

        public event EventHandler<DjiNetworkInformation> NetworkStatusChanged
        {
            add { _networkInformationReceivedSource.Subscribe(value); }
            remove { _networkInformationReceivedSource.Unsubscribe(value); }
        }

        private async Task ListenOnAllInterfaces()
        {
            // wait for the UI to catch up
            await Task.Delay(5000);

            // start the packet-delivery thread, which is responsible
            // for the actual drone/operator packet delivery
            _cancellationTokenSource = new CancellationTokenSource();
            _networkFrameDeliveryThread = new Thread(DeliverFrames);
            _networkFrameDeliveryThread.IsBackground = true;
            _networkFrameDeliveryThread.Start();

            foreach (var device in CaptureDeviceList.Instance)
            {
                try
                {
                    device.OnPacketArrival += Device_OnPacketArrival;

                    device.Open();
                    device.StartCapture();
                    _captureDevices.Add(device);

                    Trace.TraceError($"Listening for operator packets on interface {device.Name}");
                }
                catch(Exception exception)
                {
                    Trace.TraceError($"Can't listen for operator packets on interface {device.Name}", exception);
                }
            }
        }

        public void Device_OnPacketArrival(object sender, CaptureEventArgs capture)
        {
            // skip none UDP packets
            if (capture.Packet.Data.Length < 42) return;

            UdpPacket udpPacket = new UdpPacket(capture.Packet.Data);

            switch (_snifferState)
            {
                case SnifferState.WaitingForDrone:

                    // an other interface could already have found the ip-address
                    // but this interface hasn't been shutdown yet
                    if (!string.IsNullOrEmpty(_operatorIpAddress)) return;

                    // as we have no clue what the operator ip is, we
                    // need to wrap the bytes into a UDP-Packet and
                    // extract the source and destination ip-address

                    // if we the packet isn't for the drone, skip
                    if (udpPacket.DestinationIpAddress != DRONE_IP_ADDRESS) return;

                    IPAddress sourceIpAddress = IPAddress.Parse(udpPacket.SourceIpAddress);

                    // ignore network and broadcast packets + ignore non-local packets
                    if (sourceIpAddress.GetAddressBytes()[3] == 0) return;
                    else if (sourceIpAddress.GetAddressBytes()[3] == Convert.ToInt16(255)) return;
                    else if (!udpPacket.SourceIpAddress.StartsWith("192.168")) return;

                    // ignore our own ip address
                    var localAddresses = Dns.GetHostEntry(Dns.GetHostName())
                        .AddressList.Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                        .Select(s => s.ToString()).ToList();

                    if (localAddresses.Contains(udpPacket.SourceIpAddress)) return;

                    // drone has been found. We require to do following actions:
                    // 1. remember the operator's ip-address
                    // 2. switch the local state to 'PacketSniffing'
                    // 3. notify any possible subscribers about our accomplishment
                    // 4. reroute this packet to the subscribers, as this is our first valid packet
                    // 5. stop listening on all other interfaces
                    var networkInformation = new DjiNetworkInformation(udpPacket.SourceIpAddress, DRONE_IP_ADDRESS, NetworkStatus.Connected);

                    Trace.TraceInformation($"Interface {capture.Device?.Name ?? "Simulation"} detected possible drone {udpPacket.SourceIpAddress}");

                    _operatorIpAddress = networkInformation.OperatorIpAddress;
                    _snifferState = SnifferState.PacketSniffing;
                    _networkInformationReceivedSource?.Raise(capture, networkInformation);
                    Device_OnPacketArrival(sender, capture);

                    Task.Run(() =>
                    {
                        var irrelevantDevices = _captureDevices.Where(cp => cp != capture.Device).ToList();

                        for (int index = 0; index < irrelevantDevices.Count; index++)
                            ShutdownInterface(irrelevantDevices[index]);
                    });

                    break;

                case SnifferState.PacketSniffing:

                    // ignore all none-dji related network packets
                    if((udpPacket.DestinationIpAddress != DRONE_IP_ADDRESS &&
                        udpPacket.DestinationIpAddress != _operatorIpAddress) ||
                        (udpPacket.SourceIpAddress != DRONE_IP_ADDRESS &&
                        udpPacket.SourceIpAddress != _operatorIpAddress))
                        return;

                    var networkPacket = new NetworkPacket(udpPacket, capture.Packet);
                    _networkPackets.Enqueue(networkPacket);
                    _networkFrameAvailable.Set();

                    break;

                default:
                    throw new ApplicationException($"{nameof(SnifferState)} hasn't been implemented yet");
            }
        }

        private void DeliverFrames()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                while (_networkPackets.Count > 0)
                {
                    if (_networkPackets.TryDequeue(out NetworkPacket networkPacket))
                        _networkPacketReceivedSource?.Raise(this, networkPacket);
                    else Thread.Sleep(1);

                    if (_networkPackets.Count > 0 && _networkPackets.Count % 1000 == 0)
                        Trace.TraceWarning($"{nameof(_networkFrameDeliveryThread)} is throtteling. {_networkPackets.Count} in queue");
                }

                // wait till we receive another packet
                _networkFrameAvailable.WaitOne(_cancellationTokenSource.Token);
            }
        }

        private void ShutdownInterface(ICaptureDevice device)
        {
            if (device == null) throw new ArgumentException($"{nameof(device)} must not be null or empty");
            else if (!device.Started) return;

            try
            {
                device.OnPacketArrival -= Device_OnPacketArrival;

                device.StopCapture();
                device.Close();

                if (_captureDevices.Contains(device))
                    _captureDevices.Remove(device);

                Trace.TraceInformation($"Stop listening on interface {device.Name}");
            }
            catch (Exception exception)
            {
                Trace.TraceError($"Can't gracefully stop listening on interface {device}", exception);
            }
        }

        public void Forceshutdown()
        {
            for (int index = _captureDevices.Count - 1; index >= 0; index--)
                ShutdownInterface(_captureDevices[index]);

            _cancellationTokenSource.Cancel();
        }

        public void Dispose() => Forceshutdown();
    }

    internal enum SnifferState
    {
        WaitingForDrone,
        PacketSniffing
    }
}