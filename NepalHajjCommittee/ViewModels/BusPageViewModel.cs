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
        private List<Batch> _batches;
        private Batch _selectedBatch;
        private BusDetails _selectedBus;
        private List<string> _routes;
        private string _selectedRoute;

        public BusPageViewModel(IRegionManager regionManager, INepalHajjCommitteeRepository repository) : base(regionManager)
        {
            _repository = repository;
            Years = new List<int> { 2019, 2020, 2021, 2022, 2023, 2024, 2025, 2026, 2027, 2028, 2029, 2030 };

            Routes = new List<string> { "Makkah to Madinah", "Makkah to Airport", "Madinah to Airport", "Madinah To Makkah" };
        }

        public ICommand AddBus => _addBus ?? (_addBus = new DelegateCommand(ExecuteAddBus));

        public ICommand AllocateBus => _allocateBus ?? (_allocateBus = new DelegateCommand(ExecuteAllocateBus));


        public string SelectedRoute
        {
            get { return _selectedRoute; }
            set { SetProperty(ref _selectedRoute, value); }
        }
        public List<string> Routes
        {
            get { return _routes; }
            set { SetProperty(ref _routes, value); }
        }
        public BusDetails SelectedBus
        {
            get { return _selectedBus; }
            set
            {
                SetProperty(ref _selectedBus, value);
                BusChanged();
            }
        }
        public Batch SelectedBatch
        {
            get { return _selectedBatch; }
            set
            {
                SetProperty(ref _selectedBatch, value);
                BatchChanged();
            }
        }
        public List<Batch> Batches
        {
            get { return _batches; }
            set { SetProperty(ref _batches, value); }
        }

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
            SelectedBus = null;
            SelectedBatch = null;
            SelectedRoute = null;

            YearChanged();
        }

        private void ExecuteAllocateBus()
        {
            if (AvailableSeats < RequiredSeats)
                return;
            try
            {
                _people.ForEach(x =>
                {
                    if (SelectedRoute == "Makkah to Madinah")
                        x.MakkahToMadinahBusNumber = SelectedBus.BusNumber;
                    else if (SelectedRoute == "Makkah to Airport")
                        x.MakkahToAirportBusNumber = SelectedBus.BusNumber;
                    if (SelectedRoute == "Madinah to Airport")
                        x.MadinahToAirportBusNumber = SelectedBus.BusNumber;
                    if (SelectedRoute == "Madinah To Makkah")
                        x.MadinahToMakkahBusNumber = SelectedBus.BusNumber;
                    _repository.PersonRepository.Update(x);
                });
                _repository.Commit();

                var bus = BusDetails.FirstOrDefault(x => x == SelectedBus);
                bus.Capacity -= RequiredSeats;
                if (bus.Capacity == 0)
                    BusDetails.Remove(bus);

                BusDetails = BusDetails.Where(x => true).ToList();
                MessageBox.Show("Alloted bus number", Constants.Success, MessageBoxButton.OK, MessageBoxImage.Information);

                Batches.Remove(SelectedBatch);
                Batches = Batches.Where(x => true).ToList();

                SelectedBus = null;
            }
            catch
            {
                MessageBox.Show("Could not allot bus number", Constants.Success, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void GroupChanged()
        {
            if (SelectedGroup == null) return;

            Batches = _repository.BatchRepository.GetMany(x => x.FK_ID_HaajiGroup == SelectedGroup.ID).ToList();
        }

        private void BatchChanged()
        {
            if (SelectedBatch == null) { RequiredSeats = 0; return; }

            _people = _repository.PersonRepository.GetMany(x => x.FK_ID_Batch == SelectedBatch.ID).ToList();
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

        private void BusChanged()
        {
            if (SelectedBus == null)
                AvailableSeats = 0;
            else
                AvailableSeats = SelectedBus.Capacity;
        }
    }
}
