using Dji.UI.ViewModels.Controls.Inspectors;
using System.Collections.ObjectModel;
using Dji.Network.Packet;
using Dji.UI.Extensions;
using System.Linq;
using ReactiveUI;
using WeakEvent;
using System;

namespace Dji.UI.ViewModels
{
    public class DjiHexComparisonWindowViewModel : ReactiveObject, IBinaryComparable<ObservableCollection<HexControlViewModel>>
    {
        private static readonly WeakEventSource<DjiHexComparisonWindowViewModel> _djiHexComparisonAdded = new WeakEventSource<DjiHexComparisonWindowViewModel>();
        private static readonly WeakEventSource<DjiHexComparisonWindowViewModel> _djiHexComparisonRemoved = new WeakEventSource<DjiHexComparisonWindowViewModel>();

        private string _title;

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        public static event EventHandler<DjiHexComparisonWindowViewModel> HexComparisonAdded
        {
            add { _djiHexComparisonAdded.Subscribe(value); }
            remove { _djiHexComparisonAdded.Unsubscribe(value); }
        }

        public static event EventHandler<DjiHexComparisonWindowViewModel> HexComparisonRemoved
        {
            add { _djiHexComparisonRemoved.Subscribe(value); }
            remove { _djiHexComparisonRemoved.Unsubscribe(value); }
        }

        public void Add(NetworkPacket networkPacket)
        {
            // if the comparison window already contains this packet, ignore it
            if (HexControlViewModels.Any(h => h.NetworkPacket == networkPacket)) return;

            // add the new networkPacket and sort based on their identifier
            HexControlViewModels.Add(new HexControlViewModel(networkPacket));
            HexControlViewModels.Sort((a, b) => a.NetworkPacket.Id.CompareTo(b.NetworkPacket.Id));

            RefreshTitle();
            // determine the new uniqueness after receiving a new packet.
            ResetUniqueness();
            DetermineUniqueness(HexControlViewModels);

            // notify all subscribers that a new item has been added
            _djiHexComparisonAdded?.Raise(this, this);
        }

        public void Remove(HexControlViewModel viewModel)
        {
            // if the comparison window doesn't contains this model, ignore it
            if (!HexControlViewModels.Any(h => h == viewModel)) return;

            HexControlViewModels.Remove(viewModel);

            RefreshTitle();
            ResetUniqueness();
            DetermineUniqueness(HexControlViewModels);

            // notify all subscribers that this window isn't available anymore
            _djiHexComparisonRemoved?.Raise(this, this);
        }

        public void OnControlWindowClose()
        {
            HexControlViewModels.Clear(); 
            _djiHexComparisonRemoved?.Raise(this, this);
        }

        private void RefreshTitle() => Title = string.Join(" ", HexControlViewModels.ToList().Select(h => $"#{h.NetworkPacket.Id}"));

        public void ResetUniqueness()
        {
            foreach (var currentViewModel in HexControlViewModels)
                currentViewModel.ResetUniqueness();
        }

        public void DetermineUniqueness(ObservableCollection<HexControlViewModel> other)
        {
            foreach (var outerViewModel in other)
                foreach (var innerViewModel in other.Where(h => h != outerViewModel))
                    outerViewModel.DetermineUniqueness(innerViewModel);
        }

        public ObservableCollection<HexControlViewModel> HexControlViewModels { get; set; } = new ObservableCollection<HexControlViewModel>();
    }
}