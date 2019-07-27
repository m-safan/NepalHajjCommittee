using System;

namespace NepalHajjCommittee.Models
{
    public class FilterModel : PropertyChangeNotifier
    {
        private int _visitYear;
        private string _groupName;
        private string _batchName;
        private string _name;
        private string _gender;
        private string _passportNo;
        private DateTime? _arrivalDateMakkah;
        private DateTime? _departureDateMakkah;
        private DateTime? _arrivalDateMadinah;
        private DateTime? _departureDateMadinah;
        private string _incomingFlight;
        private string _outgoingFlight;
        private string _state;
        private string _contactNo;

        public string ContactNo
        {
            get { return _contactNo; }
            set { SetProperty(ref _contactNo, value); }
        }

        public string State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }
        public string OutgoingFlight
        {
            get { return _outgoingFlight; }
            set { SetProperty(ref _outgoingFlight, value); }
        }
        public string IncomingFlight
        {
            get { return _incomingFlight; }
            set { SetProperty(ref _incomingFlight, value); }
        }
        public DateTime? DepartureDateMadinah
        {
            get { return _departureDateMadinah; }
            set { SetProperty(ref _departureDateMadinah, value); }
        }
        public DateTime? ArrivalDateMadinah
        {
            get { return _arrivalDateMadinah; }
            set { SetProperty(ref _arrivalDateMadinah, value); }
        }
        public DateTime? DepartureDateMakkah
        {
            get { return _departureDateMakkah; }
            set { SetProperty(ref _departureDateMakkah, value); }
        }

        public DateTime? ArrivalDateMakkah
        {
            get { return _arrivalDateMakkah; }
            set { SetProperty(ref _arrivalDateMakkah, value); }
        }
        public string PassportNo
        {
            get { return _passportNo; }
            set { SetProperty(ref _passportNo, value); }
        }
        public string Gender
        {
            get { return _gender; }
            set { SetProperty(ref _gender, value); }
        }
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        public string BatchName
        {
            get { return _batchName; }
            set { SetProperty(ref _batchName, value); }
        }
        public string GroupName
        {
            get { return _groupName; }
            set { SetProperty(ref _groupName, value); }
        }
        public int VisitYear
        {
            get { return _visitYear; }
            set { SetProperty(ref _visitYear, value); }
        }
    }
}
