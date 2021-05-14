using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Dji.UI.Extensions.MVVM
{
    /// <summary>
    /// PerformanceCollection is a ObservableCollection with some extensions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PerformanceCollection<T> : ObservableCollection<T>
    {
        public PerformanceCollection(IEnumerable<T> collection) : base(collection) { }

        public PerformanceCollection() { }

        public void AddRange(IEnumerable<T> items)
        {
            if (items == null || !items.Any())
                return;

            CheckReentrancy();

            int startIndex = Count;

            foreach (var item in items)
                Items.Insert(0, item);

            NotifyChanges(NotifyCollectionChangedAction.Add, items, 0);
        }

        public new void Clear()
        {
            Action raiseChanges = () => base.Clear();

            if (!Dispatcher.UIThread.CheckAccess())
                Dispatcher.UIThread.Post(raiseChanges, DispatcherPriority.ContextIdle);
            else raiseChanges();
        }

        private void NotifyChanges(NotifyCollectionChangedAction changeAction, IEnumerable<T> items, int index)
        {
            Action raiseChanges = () =>
            {
                try
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(changeAction, new List<T>(items), index));
                    OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                    OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                }
                catch(ArgumentOutOfRangeException) { }
            };

            if (!Dispatcher.UIThread.CheckAccess())
                Dispatcher.UIThread.Post(raiseChanges, DispatcherPriority.ContextIdle);
            else raiseChanges();
        }
    }
}