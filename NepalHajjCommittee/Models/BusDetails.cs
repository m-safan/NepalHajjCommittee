namespace NepalHajjCommittee.Models
{
    public class BusDetails : PropertyChangeNotifier
    {
        private string _busNumber;
        private int _capacity;

        public int Capacity
        {
            get { return _capacity; }
            set { SetProperty(ref _capacity, value); }
        }

        public string BusNumber
        {
            get { return _busNumber; }
            set { SetProperty(ref _busNumber, value); }
        }
    }
}
