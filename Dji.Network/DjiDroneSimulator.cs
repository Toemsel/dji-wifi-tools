using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using PacketDotNet;
using System.IO;
using SharpPcap;
using PcapNet;
using System;
using WeakEvent;

namespace Dji.Network
{
    public enum SimulationState
    {
        Loaded,
        Simulate,
        Pause,
        Complete,
        Cancelled
    }

    public class Simulation
    {
        internal Simulation(SimulationState simulationState, int count, int index) => (State, NetworkPacketCount, LastPacketIndex) = (simulationState, count, index);

        public SimulationState State { get; init; }

        public int NetworkPacketCount { get; init; }

        public int LastPacketIndex { get; init; }
    }

    public class DjiDroneSimulator : IDisposable
    {
        private readonly WeakEventSource<SimulationState> _simulationStateReceivedSource = new WeakEventSource<SimulationState>();
        private readonly Queue<RawCapture> _rawCaptures = new Queue<RawCapture>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly DjiPacketSniffer _djiPacketSniffer;

        private CancellationTokenSource _cancellationTokenSource;
        private bool _multiStepSimulation = false;
        private bool _singleStepSimulation = false;
        private ManualResetEvent _autoResetEvent;
        private Thread _simulationThread;
        private bool _disposed = false;

        public DjiDroneSimulator(DjiPacketSniffer djiPacketSniffer) => _djiPacketSniffer = djiPacketSniffer ??
            throw new ArgumentException($"The {nameof(djiPacketSniffer)} may not be null");

        public event EventHandler<SimulationState> SimulationStateChanged
        {
            add { _simulationStateReceivedSource.Subscribe(value); }
            remove { _simulationStateReceivedSource.Unsubscribe(value); }
        }

        public async Task<bool> LoadSimulation(string file)
        {
            if (string.IsNullOrEmpty(file))
                throw new ArgumentException($"The {nameof(file)} must not be null or empty");
            else if(!File.Exists(file))
                throw new ArgumentException($"'{file}' does not exist");

            await _semaphore.WaitAsync();

            try
            {
                // check whether this instance already has been populated
                if (_rawCaptures.Count > 0)
                    throw new InvalidOperationException($"A simulation already has been loaded. Can't load more than once simulation per instance");

                var serializer = new PcapSerializer();

                using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    var pcap = await serializer.DeserializeAsync(stream);

                    foreach (var currentPacket in pcap.Packets)
                        _rawCaptures.Enqueue(new RawCapture(LinkLayers.Raw, new PosixTimeval(currentPacket.Timestamp), currentPacket.Data));
                }

                // the dispose might have been called BEFORE the simulation
                // has been loaded, leaving a zombie thread behind. Avoid this
                if (_disposed) return false;

                Trace.TraceInformation($"Simulation {file} has been loaded successfully");

                _cancellationTokenSource = new CancellationTokenSource();
                _autoResetEvent = new ManualResetEvent(false);
                _simulationThread = new Thread(Simulation);
                _simulationThread.IsBackground = true;
                _simulationThread.Start();

                // notify any listeners that the simulation has been loaded
                _simulationStateReceivedSource?.Raise(this, SimulationState.Loaded);

                return true;
            }
            catch(Exception exception) when (!(exception is InvalidOperationException))
            {
                Trace.TraceError($"Couldn't load simulation {file}", exception);

                return false;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void ValidateSimulationState(Action action, SimulationState? state = null)
        {
            try
            {
                if (_simulationThread == null)
                    throw new InvalidOperationException($"The simulation hasn't been loaded yet. Call {nameof(LoadSimulation)} first");
                if (_cancellationTokenSource.IsCancellationRequested)
                    throw new OperationCanceledException($"The simulation has been cancelled");
                else if (!_simulationThread.IsAlive)
                    throw new OperationCanceledException($"The simulation has been finished");

                action?.Invoke();

                if(state.HasValue)
                    _simulationStateReceivedSource?.Raise(this, state.Value);
            }
            catch(OperationCanceledException) { }
        }

        public void ContinueSimulation() => ValidateSimulationState(() => _autoResetEvent.Set(), SimulationState.Simulate);

        public void PauseSimulation() => ValidateSimulationState(() => _autoResetEvent.Reset(), SimulationState.Pause);

        public void SingleStepSimulation() => ValidateSimulationState(() => { _singleStepSimulation = true; ContinueSimulation(); PauseSimulation(); });

        public void MultiStepSimulation() => ValidateSimulationState(() => { _multiStepSimulation = true; ContinueSimulation(); });

        private void Simulation()
        {
            RawCapture NextPacket()
            {
                var nextPacket = _rawCaptures.Dequeue();
                _djiPacketSniffer.Device_OnPacketArrival(this, new CaptureEventArgs(nextPacket, null));
                return nextPacket;
            }

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    if(_singleStepSimulation)
                    {
                        _singleStepSimulation = false;
                        _autoResetEvent.Reset();
                    }

                    if(!_multiStepSimulation)
                        _autoResetEvent.WaitOne(_cancellationTokenSource.Token);
                }
                catch(ObjectDisposedException) { }
                catch (InvalidOperationException) { }
                catch(AbandonedMutexException) { }
                catch(OperationCanceledException) { }

                // check whether this instance has been shutdown
                if (_cancellationTokenSource.IsCancellationRequested) break;

                // check if there is a packet remaining within the queue
                if (_rawCaptures.Count <= 0) break;
                var currentPacket = NextPacket();

                // check if there is yet anotherone available
                if (_rawCaptures.Count <= 0) break;
                var nextPacket = _rawCaptures.Peek();

                // simulate the 'real-time' packet delay
                if(!_multiStepSimulation)
                    Thread.Sleep(nextPacket.Timeval.Date - currentPacket.Timeval.Date);
            }

            _simulationStateReceivedSource?.Raise(this, !_cancellationTokenSource.IsCancellationRequested 
                ? SimulationState.Complete : SimulationState.Cancelled);
        }

        public void Dispose()
        {
            // shutdown any possible thread
            _cancellationTokenSource?.Cancel();
            // ensure that any async operation can determin whether
            // this instance already has been disposed or not.
            _disposed = true;
        }
    }
}