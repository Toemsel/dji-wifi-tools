using Dji.Network.Packet;
using Dji.UI.Extensions;
using Dji.UI.Extensions.MVVM;
using LinqKit;
using ReactiveUI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Dji.UI.Pooling
{
    public class DjiNetworkPacketPool : ReactiveObject, IDisposable
    {
        private const int MAX_NETWORK_PACKETS = 100_000_000;
        private const int MAX_FILTER_PACKETS = 100_000_000;

        private readonly List<NetworkPacket> _networkPackets = new List<NetworkPacket>();
        private readonly SemaphoreSlim _filterPacketAccessSemaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _filterSemaphore = new SemaphoreSlim(1, 1);
        private readonly IDisposable _filterSubscriptionDisposable;
        private readonly IDisposable _freezeSubscriptionDisposable;

        private Expression<Func<NetworkPacket, bool>> _baseFilter = PredicateBuilder.New<NetworkPacket>(true);
        private CancellationTokenSource _filterCancellationToken;
        private Thread _filterThread;
        private bool _isFiltering;

        private readonly CancellationTokenSource _uiIntegrationCancellationToken = new CancellationTokenSource();
        private readonly ConcurrentBag<NetworkPacket> _pendingUiNetworkPackets = new ConcurrentBag<NetworkPacket>();
        private readonly SemaphoreSlim _uiNetworkPacketsIntegrationSemaphore = new SemaphoreSlim(1, 1);

        private bool _freezeNetworkPackets = true;
        private bool _forceUiRebuild = false;

        public DjiNetworkPacketPool()
        {
            // if our filter-expression changes, apply the filter to the database
            _filterSubscriptionDisposable = this.WhenAnyValue(instance => instance.Filter).Subscribe(e => EvaluateFilterOnPackets());
            _freezeSubscriptionDisposable = this.WhenAnyValue(instance => instance.FreezeNetworkPackets).Subscribe(e => _forceUiRebuild = true);
            _freezeSubscriptionDisposable = this.WhenAnyValue(instance => instance.FreezeNetworkPackets).Subscribe(e => UnlockUiNetworkPacketIntegration());

            // work off all packets which are still waiting for the UI insertion
            _ = UiNetworkPacketIntegration();
        }

        public Expression<Func<NetworkPacket, bool>> Filter
        {
            get => _baseFilter;
            private set => this.RaiseAndSetIfChanged(ref _baseFilter, value);
        }

        public bool IsFiltering
        {
            get => _isFiltering;
            set => this.RaiseAndSetIfChanged(ref _isFiltering, value);
        }

        public bool FreezeNetworkPackets
        {
            get => _freezeNetworkPackets;
            set => this.RaiseAndSetIfChanged(ref _freezeNetworkPackets, value);
        }

        public int MaxPoolSize => MAX_NETWORK_PACKETS;

        public int CurrentPoolSize => _networkPackets.Count;

        public int MaxFilterSize => MAX_FILTER_PACKETS;

        public ObservableCollection<NetworkPacket> NetworkPackets { get; private set; } = new ObservableCollection<NetworkPacket>();

        public void AddFilter(Expression<Func<NetworkPacket, bool>> filter) => Filter = Filter.And(filter);

        public void Store(params NetworkPacket[] networkPackets)
        {
            // check our boundaries. If we have more than MAX_NETWORK_PACKETS packets stored, we need to get rid of some packets.
            int removeCount = (_networkPackets.Count + networkPackets.Length) - MAX_NETWORK_PACKETS;

            if (removeCount > 0)
                _networkPackets.RemoveRange(0, removeCount);

            // add the packets to the base-collection.
            _networkPackets.AddRange(networkPackets);
            this.RaisePropertyChanged(nameof(CurrentPoolSize));

            // singal the UI-Thread that data is available
            _pendingUiNetworkPackets.AddRange(networkPackets);
            UnlockUiNetworkPacketIntegration();
        }

        private void UnlockUiNetworkPacketIntegration()
        {
            if (!FreezeNetworkPackets && _uiNetworkPacketsIntegrationSemaphore.CurrentCount == 0)
                _uiNetworkPacketsIntegrationSemaphore.Release();
        }

        private async Task UiNetworkPacketIntegration()
        {
            while(!_uiIntegrationCancellationToken.IsCancellationRequested)
            {
                List<NetworkPacket> networkPackets = new List<NetworkPacket>();

                while (_pendingUiNetworkPackets.Count > 0)
                    if (_pendingUiNetworkPackets.TryTake(out NetworkPacket networkPacket))
                        networkPackets.Add(networkPacket);
                    else await Task.Delay(1);

                // do whatever we want todo with the UI packets
                await HandleUiNetworkPackets(networkPackets, _forceUiRebuild);
                // we only do require to 'force-refresh' the UI once
                _forceUiRebuild = false;

                // let the UI some space to breath
                await Task.Delay(8);

                // wait for new ui-packets to arrive
                await _uiNetworkPacketsIntegrationSemaphore.WaitAsync(_uiIntegrationCancellationToken.Token);
            }
        }

        protected virtual async Task HandleUiNetworkPackets(IEnumerable<NetworkPacket> networkPackets, bool requiresUiRebuild)
        {
            try
            {
                // while we can filter in background, we still have some specific
                // time-frames where we aren't allowed to modify the collection anymore
                await _filterPacketAccessSemaphore.WaitAsync();

                // if the packets match our filter, add it to the 'filtered' collection
                networkPackets = networkPackets.Where(Filter.Compile()).ToList();

                // check our boundaries. If we have more than MAX_FILTER_PACKETS packets stored, we need to get rid of some packets
                var removeCount = NetworkPackets.Count + ((List<NetworkPacket>)networkPackets).Count - MAX_FILTER_PACKETS;

                if (removeCount > 0 || requiresUiRebuild)
                {
                    NetworkPackets = new ObservableCollection<NetworkPacket>(networkPackets.Reverse().Concat(NetworkPackets).Take(MAX_FILTER_PACKETS).ToList());
                    this.RaisePropertyChanged(nameof(NetworkPackets));
                }
                else
                {
                    // add the packets to the filtered-collection
                    foreach (var networkPacket in networkPackets)
                        NetworkPackets.Insert(0, networkPacket);
                }
            }
            finally
            {
                // since we are done with the changes, release the lock
                _filterPacketAccessSemaphore.Release();
            }
        }

        public void EvaluateFilterOnPackets()
        {
            // only one thread may pass this boundary
            _filterSemaphore.Wait();

            if (_filterCancellationToken != null)
            {
                _filterCancellationToken.Cancel();
                _filterThread.Join();

                _filterCancellationToken = null;
                _filterThread = null;
            }

            _filterCancellationToken = new CancellationTokenSource();
            _filterThread = new Thread(FilterNetworkPackets);
            _filterThread.IsBackground = true;
            _filterThread.Start();

            // let other threads pass the semaphore
            _filterSemaphore.Release();
        }

        private void FilterNetworkPackets()
        {
            bool Filter()
            {
                // compile the new filter - it might have changed
                var predicate = _baseFilter.Compile();
                var relevantPackets = new List<NetworkPacket>();
                var semaphoreAccess = false;

                try
                {
                    // materialize, such that we wait for all threads to complete
                    relevantPackets = _networkPackets.AsParallel().Where(networkPacket =>
                    {
                        _filterCancellationToken.Token.ThrowIfCancellationRequested();
                        return predicate.Invoke(networkPacket);
                    }).ToList();

                    // as we NOW have all relevant packets, we need to add the packets
                    // which have been added WHILE applying the predicate. From this
                    // point on, we MUST NOT allow other threads to add values
                    _filterPacketAccessSemaphore.Wait();
                    // remember that we did set the semaphore. This is important,
                    // as this filter function may (but not always) consume the
                    // semaphore. Hence, we must not release the semaphore if we
                    // didn't reach this point -> Remember our action!
                    semaphoreAccess = true;
                    // while filtering, we could have new values within our collection
                    relevantPackets.AddRange(NetworkPackets.Where(predicate));

                    // check again if the thread should abort, before doing all the linqs
                    _filterCancellationToken.Token.ThrowIfCancellationRequested();

                    // Remove duplicates, sort and cap at MAX_FILTER_PACKETS
                    relevantPackets = relevantPackets
                        .Distinct()
                        .OrderBy(networkPacket => networkPacket.Id)
                        .Take(MAX_FILTER_PACKETS)
                        .Reverse()
                        .ToList();
                }
                catch (OperationCanceledException) { }
                catch (InvalidOperationException) { }
                catch (AggregateException) { }

                // if we did cancel the filter operation, skip changes
                if (_filterCancellationToken.IsCancellationRequested) return semaphoreAccess;

                this.OnUIThread(() =>
                {
                    NetworkPackets = new ObservableCollection<NetworkPacket>(relevantPackets);
                    this.RaisePropertyChanged(nameof(NetworkPackets));
                });

                return semaphoreAccess;
            }

            var requireSemaphoreRelease = false;

            try
            {
                this.OnUIThread(() => IsFiltering = true);
                requireSemaphoreRelease = Filter();
            }
            finally
            {
                if (requireSemaphoreRelease)
                    _filterPacketAccessSemaphore.Release();

                this.OnUIThread(() => IsFiltering = false);
            }
        }

        public void Dispose()
        {
            _uiIntegrationCancellationToken.Cancel();
            _filterSubscriptionDisposable.Dispose();
            _freezeSubscriptionDisposable.Dispose();
        }
    }
}