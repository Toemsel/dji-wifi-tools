using ReactiveUI;

namespace Dji.UI.ViewModels.StatusBar
{
    public abstract class StatusBarItemViewModel : ReactiveObject
    {
        private bool _status;
        private string _description;

        protected abstract bool CanToggle { get; }

        public bool Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }

        public virtual string Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        public void Toggle()
        {
            if (!CanToggle)
                return;

            if (Status) Off();
            else On();

            Status = !Status;
        }

        public abstract void On();

        public abstract void Off();
    }
}
