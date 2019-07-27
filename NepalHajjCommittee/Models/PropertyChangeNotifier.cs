using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NepalHajjCommittee.Models
{
    public class PropertyChangeNotifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return;

            storage = value;
            RaisePropertyChanged(propertyName);
        }
    }
}