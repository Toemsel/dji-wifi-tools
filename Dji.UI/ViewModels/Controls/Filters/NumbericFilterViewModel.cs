using Avalonia.Data;
using ReactiveUI;
using System;

namespace Dji.UI.ViewModels.Controls.Filters
{
    public abstract class NumbericFilterViewModel : FilterControlViewModel
    {
        private int _numeric;
        private string _text;
        private bool _hasValue;

        public NumbericFilterViewModel()
        {
            this.WhenAnyValue(instance => instance.Numeric).Subscribe(numeric => DjiNetworkPacketPool?.EvaluateFilterOnPackets());
            this.WhenAnyValue(instance => instance.HasValue).Subscribe(hasValue => DjiNetworkPacketPool?.EvaluateFilterOnPackets());
        }

        public string Text
        {
            get => _text;
            set 
            {
                if(_hasValue = int.TryParse(value, out int numeric))
                    _numeric = numeric;
                else if (!string.IsNullOrWhiteSpace(value))
                    throw new DataValidationException(string.Empty);
                
                this.RaisePropertyChanged(nameof(Numeric));
                this.RaisePropertyChanged(nameof(HasValue));
                this.RaiseAndSetIfChanged(ref _text, value);
            }
        }

        public int Numeric => _numeric;

        public bool HasValue => _hasValue;
    }
}