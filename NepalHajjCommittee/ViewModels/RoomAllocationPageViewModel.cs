using NepalHajjCommittee.Database;
using NepalHajjCommittee.Database.EDMX;
using NepalHajjCommittee.Models;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NepalHajjCommittee.ViewModels
{
    public class RoomAllocationPageViewModel : ViewModelBase
    {
        private readonly INepalHajjCommitteeRepository _repository;
        private List<int> _years;
        private int _selectedYear;
        private List<string> _cities;
        private string _selectedCity;
        private List<SearchedResults> _unallocatedPeople;
        private List<SearchedResults> _allocatedPeople;
        private SearchedResults _selectedUnallocatedPerson;
        private SearchedResults _selectedAllocatedPerson;
        private DelegateCommand _unallocateRoom;
        private DelegateCommand _allocateRoom;
        private List<RoomDTO> _rooms;
        private RoomDTO _selectedRoom;
        private List<Bed> _beds;
        private Bed _selectedBed;

        public RoomAllocationPageViewModel(IRegionManager regionManager, INepalHajjCommitteeRepository repository) : base(regionManager)
        {
            _repository = repository;
            Years = new List<int> { 2019, 2020, 2021, 2022, 2023, 2024, 2025, 2026, 2027, 2028, 2029, 2030 };
            Cities = new List<string> { "Makkah", "Madinah" };
            PropertyChanged += This_PropertyChanged;
        }
        public Bed SelectedBed
        {
            get { return _selectedBed; }
            set { SetProperty(ref _selectedBed, value); }
        }
        public List<Bed> Beds
        {
            get { return _beds; }
            set { SetProperty(ref _beds, value); }
        }
        public RoomDTO SelectedRoom
        {
            get { return _selectedRoom; }
            set { SetProperty(ref _selectedRoom, value); }
        }
        public List<RoomDTO> Rooms
        {
            get { return _rooms; }
            set { SetProperty(ref _rooms, value); }
        }

        public ICommand UnallocateRoom => _unallocateRoom ?? (_unallocateRoom = new DelegateCommand(ExecuteUnallocateRoom));


        public ICommand AllocateRoom => _allocateRoom ?? (_allocateRoom = new DelegateCommand(ExecuteAllocateRoom));

        public SearchedResults SelectedAllocatedPerson
        {
            get { return _selectedAllocatedPerson; }
            set
            {
                SetProperty(ref _selectedAllocatedPerson, value);
                RaisePropertyChanged(nameof(IsAllocatedPersonSelected));
            }
        }

        public SearchedResults SelectedUnallocatedPerson
        {
            get { return _selectedUnallocatedPerson; }
            set
            {
                SetProperty(ref _selectedUnallocatedPerson, value);
                RaisePropertyChanged(nameof(IsUnallocatedPersonSelected));
            }
        }

        public List<SearchedResults> AllocatedPeople
        {
            get { return _allocatedPeople; }
            set { SetProperty(ref _allocatedPeople, value); }
        }

        public List<SearchedResults> UnallocatedPeople
        {
            get { return _unallocatedPeople; }
            set { SetProperty(ref _unallocatedPeople, value); }
        }

        public string SelectedCity
        {
            get { return _selectedCity; }
            set { SetProperty(ref _selectedCity, value); }
        }

        public List<string> Cities
        {
            get { return _cities; }
            set { SetProperty(ref _cities, value); }
        }

        public int SelectedYear
        {
            get { return _selectedYear; }
            set { SetProperty(ref _selectedYear, value); }
        }

        public List<int> Years
        {
            get { return _years; }
            set { SetProperty(ref _years, value); }
        }

        public bool IsUnallocatedPersonSelected
        {
            get
            {
                if (SelectedUnallocatedPerson == null)
                    return false;

                if (SelectedCity == "Makkah" && SelectedUnallocatedPerson.MakkahRoomNo != " /  / ")
                    return false;
                if (SelectedCity == "Madinah" && SelectedUnallocatedPerson.MadinahRoomNo != " /  / ")
                    return false;

                return true;
            }
        }

        public bool IsAllocatedPersonSelected
        {
            get
            {
                if (SelectedAllocatedPerson == null)
                    return false;

                if (SelectedCity == "Makkah" && SelectedAllocatedPerson.MakkahRoomNo == " /  / ")
                    return false;
                if (SelectedCity == "Madinah" && SelectedAllocatedPerson.MadinahRoomNo == " /  / ")
                    return false;

                return true;
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            SelectedYear = DateTime.Now.Year;
            SelectedCity = null;

            SelectedUnallocatedPerson = null;
            SelectedAllocatedPerson = null;

            SelectedBed = null;
            SelectedRoom = null;

            UnallocatedPeople = null;
            AllocatedPeople = null;
        }

        private void This_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedYear))
                FetchRecords();

            if (e.PropertyName == nameof(SelectedCity))
                FetchRecords();

            if (e.PropertyName == nameof(SelectedRoom))
            {
                if (SelectedRoom == null)
                    Beds = null;
                else
                    Beds = _repository.BedRepository.GetMany(x => x.FK_ID_Room == SelectedRoom.ID && x.IsAvailable).ToList();
            }
        }

        private void FetchRecords()
        {
            if (string.IsNullOrEmpty(SelectedCity))
                return;

            IQueryable<Person> people = null;
            if (SelectedCity == "Makkah")
                people = _repository.PersonRepository.GetAllQueryable().Where(x => x.FK_ID_Bed_Makkah == null);
            else
                people = _repository.PersonRepository.GetAllQueryable().Where(x => x.FK_ID_Bed_Madinah == null);

            people.Where(x => x.Batch.HaajiGroup.VisitYear == SelectedYear);

            UnallocatedPeople = people.Select(x => new SearchedResults
            {
                ID = x.ID,
                ArrivalDateMadinah = x.Batch.HaajiGroup.ArrivalDateMadinah,
                ArrivalDateMakkah = x.Batch.HaajiGroup.ArrivalDateMakkah,
                BatchName = x.Batch.BatchName,
                MadinahToAirportBusNumber = x.MadinahToAirportBusNumber,
                MadinahToMakkahBusNumber = x.MadinahToMakkahBusNumber,
                MakkahToAirportBusNumber = x.MakkahToAirportBusNumber,
                MakkahToMadinahBusNumber = x.MakkahToMadinahBusNumber,
                ContactNo = x.ContactNo,
                DepartureDateMadinah = x.Batch.HaajiGroup.DepartureDateMadinah,
                DepartureDateMakkah = x.Batch.HaajiGroup.DepartureDateMakkah,
                Gender = x.Gender,
                GroupName = x.Batch.HaajiGroup.Name,
                Name = x.Name,
                PassportNo = x.PassportNo,
                StateName = x.Batch.HaajiGroup.StateName,
                MadinahRoomNo = x.Bed.Room.HotelName + " / " + x.Bed.Room.RoomNumber + " / " + x.Bed.BedNumber,
                MakkahRoomNo = x.Bed1.Room.HotelName + " / " + x.Bed1.Room.RoomNumber + " / " + x.Bed1.BedNumber,
            }).ToList();

            if (SelectedCity == "Makkah")
                people = _repository.PersonRepository.GetAllQueryable().Where(x => x.FK_ID_Bed_Makkah != null);
            else
                people = _repository.PersonRepository.GetAllQueryable().Where(x => x.FK_ID_Bed_Madinah != null);

            people.Where(x => x.Batch.HaajiGroup.VisitYear == SelectedYear);

            AllocatedPeople = people.Select(x => new SearchedResults
            {
                ID = x.ID,
                ArrivalDateMadinah = x.Batch.HaajiGroup.ArrivalDateMadinah,
                ArrivalDateMakkah = x.Batch.HaajiGroup.ArrivalDateMakkah,
                BatchName = x.Batch.BatchName,
                MadinahToAirportBusNumber = x.MadinahToAirportBusNumber,
                MadinahToMakkahBusNumber = x.MadinahToMakkahBusNumber,
                MakkahToAirportBusNumber = x.MakkahToAirportBusNumber,
                MakkahToMadinahBusNumber = x.MakkahToMadinahBusNumber,
                ContactNo = x.ContactNo,
                DepartureDateMadinah = x.Batch.HaajiGroup.DepartureDateMadinah,
                DepartureDateMakkah = x.Batch.HaajiGroup.DepartureDateMakkah,
                Gender = x.Gender,
                GroupName = x.Batch.HaajiGroup.Name,
                Name = x.Name,
                PassportNo = x.PassportNo,
                StateName = x.Batch.HaajiGroup.StateName,
                MadinahRoomNo = x.Bed.Room.HotelName + " / " + x.Bed.Room.RoomNumber + " / " + x.Bed.BedNumber,
                MakkahRoomNo = x.Bed1.Room.HotelName + " / " + x.Bed1.Room.RoomNumber + " / " + x.Bed1.BedNumber,
            }).ToList();

            Rooms = _repository.RoomRepository.GetAllQueryable().Where(x => x.City == SelectedCity && x.IsAvailable).Select(x => new RoomDTO
            {
                ID = x.ID,
                City = x.City,
                HotelName = x.HotelName,
                IsAvailable = x.IsAvailable,
                IsDirty = x.IsDirty,
                RoomNumber = x.RoomNumber
            }).ToList();
        }

        private void ExecuteAllocateRoom()
        {
            if (SelectedBed == null)
                return;

            var person = _repository.PersonRepository.GetFirstOrDefault(x => x.ID == SelectedUnallocatedPerson.ID);
            if (SelectedCity == "Makkah")
                person.FK_ID_Bed_Makkah = SelectedBed.ID;
            else
                person.FK_ID_Bed_Madinah = SelectedBed.ID;
            _repository.PersonRepository.Update(person);

            var availableBedCount = _repository.BedRepository.GetMany(x => x.FK_ID_Room == SelectedBed.FK_ID_Room && x.IsAvailable).Count();
            var bed = _repository.BedRepository.GetFirstOrDefault(x => x.ID == SelectedBed.ID);
            bed.IsAvailable = false;
            _repository.BedRepository.Update(bed);

            var room = _repository.RoomRepository.GetFirstOrDefault(x => x.ID == bed.FK_ID_Room);
            room.IsDirty = true;
            room.IsAvailable = availableBedCount != 1;
            _repository.RoomRepository.Update(room);

            _repository.Commit();
            FetchRecords();
        }

        private void ExecuteUnallocateRoom()
        {
            var person = _repository.PersonRepository.GetFirstOrDefault(x => x.ID == SelectedAllocatedPerson.ID);
            int? bedId = null;
            if (SelectedCity == "Makkah")
            {
                bedId = person.FK_ID_Bed_Makkah;
                person.FK_ID_Bed_Makkah = null;
            }
            else
            {
                bedId = person.FK_ID_Bed_Madinah;
                person.FK_ID_Bed_Madinah = null;
            }

            var bed = _repository.BedRepository.GetFirstOrDefault(x => x.ID == bedId);
            var availableBedCount = _repository.BedRepository.GetMany(x => x.FK_ID_Room == bed.FK_ID_Room && x.IsAvailable).Count();
            bed.IsAvailable = true;
            _repository.BedRepository.Update(bed);

            var room = _repository.RoomRepository.GetFirstOrDefault(x => x.ID == bed.FK_ID_Room);
            room.IsAvailable = true;
            room.IsDirty = availableBedCount != 1;
            _repository.RoomRepository.Update(room);

            _repository.PersonRepository.Update(person);
            _repository.Commit();

            FetchRecords();
        }
    }
}
