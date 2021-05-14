namespace Dji.UI.ViewModels.Controls.Inspectors
{
    public interface IBinaryComparable<T>
    {
        void ResetUniqueness();

        void DetermineUniqueness(T other);
    }
}
