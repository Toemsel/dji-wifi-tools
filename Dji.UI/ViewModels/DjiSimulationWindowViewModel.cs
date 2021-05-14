using Dji.Network;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.IO;

namespace Dji.UI.ViewModels
{
    public class DjiSimulationWindowViewModel : ReactiveObject, IDisposable
    {
        private readonly string _simulationFile;
        private readonly DjiPacketSniffer _packetSniffer;
        private readonly DjiDroneSimulator _simulation;

        private SimulationState _simulationState;
        private bool _isSimulationDone = false;
        private bool _isSimulationReady = false;
        private string _simulationName;

        private bool _wasLoadingSuccessful = true;
        private bool _canMultiOrSingleStep = false;
        private bool _isLoading = true;

        public DjiSimulationWindowViewModel(string simulationFile, DjiPacketSniffer packetSniffer)
        {
            _simulationFile = simulationFile;
            _packetSniffer = packetSniffer;

            SimulationName = new FileInfo(_simulationFile).Name;
            _simulation = new DjiDroneSimulator(_packetSniffer);
            _simulation.SimulationStateChanged += (obj, data) => SimulationState = data;
            _ = _simulation.LoadSimulation(_simulationFile).ContinueWith(t =>
            {
                WasLoadingSuccessful = t.Exception == null && t.Result;
                IsSimulationReady = WasLoadingSuccessful;
                IsLoading = false;
            });

            this.WhenAnyValue(instance => instance.SimulationState).Subscribe(simulationState => CanMultiOrSingleStep =
                (simulationState == SimulationState.Loaded ||
                simulationState == SimulationState.Pause));

            this.WhenAnyValue(instance => instance.SimulationState).Subscribe(simulationState => IsSimulationDone =
                (simulationState == SimulationState.Cancelled ||
                simulationState == SimulationState.Complete));
        }

        public string SimulationName
        {
            get => _simulationName;
            set => this.RaiseAndSetIfChanged(ref _simulationName, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }

        public bool WasLoadingSuccessful
        {
            get => _wasLoadingSuccessful;
            set => this.RaiseAndSetIfChanged(ref _wasLoadingSuccessful, value);
        }

        public bool IsSimulationReady
        {
            get => _isSimulationReady;
            set => this.RaiseAndSetIfChanged(ref _isSimulationReady, value);
        }

        public bool IsSimulationDone
        {
            get => _isSimulationDone;
            set => this.RaiseAndSetIfChanged(ref _isSimulationDone, value);
        }

        public bool CanMultiOrSingleStep
        {
            get => _canMultiOrSingleStep;
            set => this.RaiseAndSetIfChanged(ref _canMultiOrSingleStep, value);
        }

        public SimulationState SimulationState
        {
            get => _simulationState;
            set => this.RaiseAndSetIfChanged(ref _simulationState, value);
        }

        public void PauseContinueSimulation()
        {
            if (SimulationState == SimulationState.Simulate)
                _simulation.PauseSimulation();
            else if (SimulationState == SimulationState.Pause ||
                SimulationState == SimulationState.Loaded)
                _simulation.ContinueSimulation();
            else throw new InvalidOperationException($"Can't pause nor continue while in {SimulationState} state");
        }

        public void NextStepSimulation() => _simulation.SingleStepSimulation();

        public void CompleteSimulation() => _simulation.MultiStepSimulation();

        public void Dispose() => _simulation.Dispose();
    }
}