using Dji.Network.Packet.Extensions;
using ReactiveUI;
using System;
using System.Collections;
using System.Linq;

namespace Dji.UI.ViewModels.Controls.Inspectors
{
    public class HexValueControlViewModel : ReactiveObject, IBinaryComparable<HexValueControlViewModel>
    {
        private readonly byte _data;
        private BitArray _isUnique;

        public HexValueControlViewModel(byte data)
        {
            _data = data;

            Value = _data.ToHexString(false).ToUpper();
            ValueAsShort = Convert.ToUInt16(_data);
            Bits = _data.ToBits().Select(b => b ? '1' : '0').ToArray();
            _isUnique = new BitArray(Bits.Length, true);
        }

        public byte Data => _data;

        public string Value { get; init; }

        public ushort ValueAsShort { get; init; }

        public bool IsValueUnique { get; private set; }

        public char[] Bits { get; init; }

        public BitArray IsBitUnique => _isUnique;

        public void ResetUniqueness()
        {
            _isUnique = new BitArray(Bits.Length, true);
            IsValueUnique = true;

            this.RaisePropertyChanged(nameof(IsBitUnique));
            this.RaisePropertyChanged(nameof(IsValueUnique));
        }

        public void DetermineUniqueness(HexValueControlViewModel otherViewModel)
        {
            for (int index = 0; index < IsBitUnique.Length; index++)
            {
                var isUnique = _isUnique[index] && otherViewModel.Bits[index] == Bits[index];
                IsValueUnique = IsValueUnique && isUnique;
                _isUnique.Set(index, isUnique);
            }

            this.RaisePropertyChanged(nameof(IsBitUnique));
            this.RaisePropertyChanged(nameof(IsValueUnique));
        }
    }
}