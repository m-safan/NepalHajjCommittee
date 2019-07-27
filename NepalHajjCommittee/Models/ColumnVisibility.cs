namespace NepalHajjCommittee.Models
{
    public class ColumnVisibility : PropertyChangeNotifier
    {
        private bool _visitYear = true;
        private bool _groupName = true;
        private bool _batchName = true;
        private bool _name = true;
        private bool _gender = true;
        private bool _passportNo = true;
        private bool _arrivalDateMakkah = true;
        private bool _departureDateMakkah = true;
        private bool _arrivalDateMadinah = true;
        private bool _departureDateMadinah = true;
        private bool _incomingFlight = true;
        private bool _outgoingFlight = true;
        private bool _state = true;
        private bool _contactNo = true;
        private bool _makkahRoomNo = true;
        private bool _madinahRoomNo = true;
        private bool _busNumber = true;

        public bool BusNumber
        {
            get { return _busNumber; }
            set { SetProperty(ref _busNumber, value); }
        }

        public bool MadinahRoomNo
        {
            get { return _madinahRoomNo; }
            set { SetProperty(ref _madinahRoomNo, value); }
        }

        public bool MakkahRoomNo
        {
            get { return _makkahRoomNo; }
            set { SetProperty(ref _makkahRoomNo, value); }
        }

        public bool ContactNo
        {
            get { return _contactNo; }
            set { SetProperty(ref _contactNo, value); }
        }

        public bool State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }
        public bool OutgoingFlight
        {
            get { return _outgoingFlight; }
            set { SetProperty(ref _outgoingFlight, value); }
        }
        public bool IncomingFlight
        {
            get { return _incomingFlight; }
            set { SetProperty(ref _incomingFlight, value); }
        }
        public bool DepartureDateMadinah
        {
            get { return _departureDateMadinah; }
            set { SetProperty(ref _departureDateMadinah, value); }
        }
        public bool ArrivalDateMadinah
        {
            get { return _arrivalDateMadinah; }
            set { SetProperty(ref _arrivalDateMadinah, value); }
        }
        public bool DepartureDateMakkah
        {
            get { return _departureDateMakkah; }
            set { SetProperty(ref _departureDateMakkah, value); }
        }

        public bool ArrivalDateMakkah
        {
            get { return _arrivalDateMakkah; }
            set { SetProperty(ref _arrivalDateMakkah, value); }
        }
        public bool PassportNo
        {
            get { return _passportNo; }
            set { SetProperty(ref _passportNo, value); }
        }
        public bool Gender
        {
            get { return _gender; }
            set { SetProperty(ref _gender, value); }
        }
        public bool Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        public bool BatchName
        {
            get { return _batchName; }
            set { SetProperty(ref _batchName, value); }
        }
        public bool GroupName
        {
            get { return _groupName; }
            set { SetProperty(ref _groupName, value); }
        }
        public bool VisitYear
        {
            get { return _visitYear; }
            set { SetProperty(ref _visitYear, value); }
        }
    }
}
