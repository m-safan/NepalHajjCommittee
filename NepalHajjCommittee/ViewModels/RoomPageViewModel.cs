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
    public class RoomPageViewModel : ViewModelBase
    {
        private INepalHajjCommitteeRepository _repository;
        private List<RoomDTO> _availableRooms;
        private RoomDTO _roomModel;
        private ICommand _saveCommand;
        private bool _fromDatabase;
        private RoomDTO _selectedRoom;
        private List<string> _cities;
        private DelegateCommand _editRoom;
        private DelegateCommand _deleteRoom;

        public RoomPageViewModel(IRegionManager regionManager, INepalHajjCommitteeRepository repository) : base(regionManager)
        {
            _repository = repository;

            Cities = new List<string> { "Makkah", "Madinah" };
        }

        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(ExecuteSaveCommand));
        public ICommand EditRoom => _editRoom ?? (_editRoom = new DelegateCommand(ExecuteEditRoom));

        public ICommand DeleteRoom => _deleteRoom ?? (_deleteRoom = new DelegateCommand(ExecuteDeleteRoom));

        public List<string> Cities
        {
            get { return _cities; }
            set { SetProperty(ref _cities, value); }
        }

        public RoomDTO SelectedRoom
        {
            get { return _selectedRoom; }
            set
            {
                SetProperty(ref _selectedRoom, value);
                RaisePropertyChanged(nameof(IsRoomSelected));
            }
        }

        public bool IsRoomSelected => SelectedRoom != null && !SelectedRoom.IsDirty;

        public int AvailableRoomCount => AvailableRooms != null ? AvailableRooms.Count(x => x.IsAvailable) : 0;

        public int AvailableBedCount => AvailableRooms != null ? AvailableRooms.Sum(x => x.AvailableBeds) : 0;

        public RoomDTO RoomModel
        {
            get { return _roomModel; }
            set { SetProperty(ref _roomModel, value); }
        }

        public List<RoomDTO> AvailableRooms
        {
            get { return _availableRooms; }
            set
            {
                SetProperty(ref _availableRooms, value);
                RaisePropertyChanged(nameof(AvailableRoomCount));
                RaisePropertyChanged(nameof(AvailableBedCount));
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            GetBlankModel();
            FetchRooms();
        }

        private void GetBlankModel(RoomDTO room = null)
        {
            _fromDatabase = false;
            var roomModel = new RoomDTO();
            if (room != null)
            {
                roomModel.ID = room.ID;
                roomModel.HotelName = room.HotelName;
                roomModel.City = room.City;
                roomModel.RoomNumber = room.RoomNumber;
                roomModel.Capacity = room.Capacity;
            }
            RoomModel = roomModel;
        }

        private void FetchRooms()
        {
            AvailableRooms = _repository.RoomRepository.GetAllQueryable().
                Where(x => x.IsAvailable).
                Select(x => new RoomDTO()
                {
                    City = x.City,
                    HotelName = x.HotelName,
                    ID = x.ID,
                    IsAvailable = x.IsAvailable,
                    IsDirty = x.IsDirty,
                    RoomNumber = x.RoomNumber,
                    Capacity = x.Beds.Count.ToString(),
                    AvailableBeds = x.Beds.Count(b => b.IsAvailable)
                }).
                ToList();
        }

        private void ExecuteSaveCommand()
        {
            if (!ValidateData())
            {
                MessageBox.Show("Something wrong with the data provided", Constants.Error, MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            var capacity = int.Parse(RoomModel.Capacity);
            var beds = new List<Bed>();
            for (int i = 0; i < capacity; i++)
                beds.Add(new Bed() { BedNumber = (i + 1).ToString(), IsAvailable = true });

            Room room = new Room()
            {
                Beds = beds,
                City = RoomModel.City,
                HotelName = RoomModel.HotelName,
                IsAvailable = true,
                IsDirty = false,
                ID = 0,
                RoomNumber = RoomModel.RoomNumber
            };

            try
            {
                if (!_fromDatabase)
                {
                    _repository.RoomRepository.Insert(room);
                    GetBlankModel(new RoomDTO() { HotelName = room.HotelName, City = room.City });
                }
                else
                {
                    _repository.BedRepository.Delete(x => x.FK_ID_Room == RoomModel.ID);
                    _repository.RoomRepository.Delete(x => x.ID == RoomModel.ID);
                    _repository.RoomRepository.Insert(room);
                    GetBlankModel();
                }

                _repository.Commit();

                MessageBox.Show("Room information saved successfully", Constants.Success, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Could not save room information", Constants.Success, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            {
                FetchRooms();
            }
        }

        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(RoomModel.HotelName))
                return false;
            if (string.IsNullOrEmpty(RoomModel.City))
                return false;
            if (string.IsNullOrEmpty(RoomModel.RoomNumber))
                return false;
            if (!(int.TryParse(RoomModel.Capacity, out int capacity) && capacity > 0))
                return false;
            return true;
        }

        private void ExecuteDeleteRoom()
        {
            if (MessageBox.Show(
                    "Are you sure you want to delete room, this will also delete all the beds associated with this room",
                    Constants.Confirmation, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                _repository.BedRepository.Delete(x => x.FK_ID_Room == SelectedRoom.ID);
                _repository.RoomRepository.Delete(x => x.ID == SelectedRoom.ID);
                _repository.Commit();

                MessageBox.Show("Batch deleted successfully", Constants.Success, MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Could not delete the room", Constants.Error, MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                FetchRooms();
            }
        }

        private void ExecuteEditRoom()
        {
            GetBlankModel(SelectedRoom);
            _fromDatabase = true;
        }
    }
}
