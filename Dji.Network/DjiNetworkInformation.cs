namespace Dji.Network
{
    public class DjiNetworkInformation
    {
        public DjiNetworkInformation(string operatorIpAddress, string droneIpAddress, NetworkStatus status) => (OperatorIpAddress, DroneIpAddress, Status) = (operatorIpAddress, droneIpAddress, status);

        public string OperatorIpAddress { get; init; }

        public string DroneIpAddress { get; init; }

        public NetworkStatus Status { get; init; }
    }

    public enum NetworkStatus
    {
        Connected,
        Disconnected
    }
}
