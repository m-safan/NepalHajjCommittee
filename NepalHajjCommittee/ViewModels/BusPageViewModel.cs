using NepalHajjCommittee.Database;
using NepalHajjCommittee.Database.EDMX;
using NepalHajjCommittee.Models;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NepalHajjCommittee.ViewModels
{
    public class BusPageViewModel : ViewModelBase
    {
        private INepalHajjCommitteeRepository _repository;
        private List<BusDetails> _busDetails;
        private int _availableSeats;
        private int _requiredSeats;
        private List<int> _years;
        private int _selectedYear;
        private List<HaajiGroup> _groups;
        private DelegateCommand _allocateBus;
        private HaajiGroup _selectedGroup;
        private List<Person> _people;
        private DelegateCommand _addBus;
        private string _busNumber;
        private string _capacity;

        public BusPageViewModel(IRegionManager regionManager, INepalHajjCommitteeRepository repository) : base(regionManager)
        {
            _repository = repository;
            Years = new List<int> { 2019, 2020, 2021, 2022, 2023, 2024, 2025, 2026, 2027, 2028, 2029, 2030 };
        }

        public ICommand AddBus => _addBus ?? (_addBus = new DelegateCommand(ExecuteAddBus));

        public ICommand AllocateBus => _allocateBus ?? (_allocateBus = new DelegateCommand(ExecuteAllocateBus));

        public string Capacity
        {
            get { return _capacity; }
            set { SetProperty(ref _capacity, value); }
        }
        public string BusNumber
        {
            get { return _busNumber; }
            set { SetProperty(ref _busNumber, value); }
        }

        public HaajiGroup SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                SetProperty(ref _selectedGroup, value);
                GroupChanged();
            }
        }

        public List<HaajiGroup> Groups
        {
            get { return _groups; }
            set { SetProperty(ref _groups, value); }
        }

        public int SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                SetProperty(ref _selectedYear, value);
                YearChanged();
            }
        }

        public List<int> Years
        {
            get { return _years; }
            set { SetProperty(ref _years, value); }
        }

        public int RequiredSeats
        {
            get { return _requiredSeats; }
            set { SetProperty(ref _requiredSeats, value); }
        }
        public int AvailableSeats
        {
            get { return _availableSeats; }
            set { SetProperty(ref _availableSeats, value); }
        }
        public List<BusDetails> BusDetails
        {
            get { return _busDetails; }
            set { SetProperty(ref _busDetails, value); }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            SelectedYear = DateTime.Now.Year;
            BusNumber = null;
            Capacity = null;
            SelectedGroup = null;
            AvailableSeats = 0;
            RequiredSeats = 0;
            BusDetails = null;

            YearChanged();
        }

        private void ExecuteAllocateBus()
        {
            if (AvailableSeats < RequiredSeats)
                return;
            try
            {
                var peopleCounter = 0;
                for (int i = 0; i < BusDetails.Count; i++)
                {
                    if (peopleCounter == _people.Count) break;
                    for (int j = 0; j < BusDetails[i].Capacity; j++)
                    {
                        if (peopleCounter == _people.Count) break;
                        _people[peopleCounter].BusNumber = BusDetails[i].BusNumber;
                        _repository.PersonRepository.Update(_people[peopleCounter++]);
                    }
                }

                _repository.Commit();
                MessageBox.Show("Alloted bus number", Constants.Success, MessageBoxButton.OK, MessageBoxImage.Information);
                RegionManager.RequestNavigate(Constants.ContentRegion, Constants.Welcome);
            }
            catch
            {
                MessageBox.Show("Could not allot bus number", Constants.Success, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void GroupChanged()
        {
            if (SelectedGroup == null) return;
            var batchIDs = _repository.BatchRepository.GetAllQueryable().Where(x => x.FK_ID_HaajiGroup == SelectedGroup.ID).Select(x => x.ID).ToList();
            _people = _repository.PersonRepository.GetMany(x => batchIDs.Contains(x.FK_ID_Batch)).ToList();

            RequiredSeats = _people.Count;
        }

        private void YearChanged()
        {
            Groups = _repository.HaajiGroupRepository.GetMany(x => x.VisitYear == SelectedYear).ToList();
        }

        private void ExecuteAddBus()
        {
            if (string.IsNullOrEmpty(BusNumber))
                return;

            if (!int.TryParse(Capacity, out int capacity))
                return;

            var busDetails = new List<BusDetails>();
            if (BusDetails != null)
                busDetails.AddRange(BusDetails);
            busDetails.Add(new BusDetails { BusNumber = BusNumber, Capacity = capacity });

            BusDetails = busDetails;

            AvailableSeats = BusDetails.Sum(x => x.Capacity);
        }
    }
}
