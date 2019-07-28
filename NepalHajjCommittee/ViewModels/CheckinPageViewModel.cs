using MoreLinq;
using NepalHajjCommittee.Database;
using NepalHajjCommittee.Database.EDMX;
using NepalHajjCommittee.Models;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NepalHajjCommittee.ViewModels
{
    public class CheckinPageViewModel : ViewModelBase
    {
        private INepalHajjCommitteeRepository _dbRepository;
        private List<int> _years;
        private List<string> _cities;
        private List<HaajiGroupDTO> _groups;
        private int _selectedYear;
        private string _selectedCity;
        private HaajiGroupDTO _selectedGroup;
        private int _requiredBeds;
        private int _availableBeds;
        private DelegateCommand _allocateCommand;

        public CheckinPageViewModel(IRegionManager regionManager, INepalHajjCommitteeRepository repository) : base(regionManager)
        {
            _dbRepository = repository;

            Years = new List<int> { 2019, 2020, 2021, 2022, 2023, 2024, 2025, 2026, 2027, 2028, 2029, 2030 };
            _selectedYear = DateTime.Now.Year;

            Cities = new List<string> { "Makkah", "Madinah" };

            PropertyChanged += This_PropertyChanged;
        }

        public ICommand AllocateCommand => _allocateCommand ?? (_allocateCommand = new DelegateCommand(ExecuteAllocateCommand));

        public int AvailableBeds
        {
            get { return _availableBeds; }
            set { SetProperty(ref _availableBeds, value); }
        }

        public int RequiredBeds
        {
            get { return _requiredBeds; }
            set { SetProperty(ref _requiredBeds, value); }
        }

        public HaajiGroupDTO SelectedGroup
        {
            get { return _selectedGroup; }
            set { SetProperty(ref _selectedGroup, value); }
        }
        public string SelectedCity
        {
            get { return _selectedCity; }
            set { SetProperty(ref _selectedCity, value); }
        }
        public int SelectedYear
        {
            get { return _selectedYear; }
            set { SetProperty(ref _selectedYear, value); }
        }

        public List<HaajiGroupDTO> Groups
        {
            get { return _groups; }
            set { SetProperty(ref _groups, value); }
        }
        public List<string> Cities
        {
            get { return _cities; }
            set { SetProperty(ref _cities, value); }
        }
        public List<int> Years
        {
            get { return _years; }
            set { SetProperty(ref _years, value); }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            SelectedYear = DateTime.Now.Year;
            SelectedCity = null;
            SelectedGroup = null;
            AvailableBeds = 0;
            RequiredBeds = 0;
        }

        private void This_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedYear))
                YearChanged();

            if (e.PropertyName == nameof(SelectedCity))
                CityChanged();

            if (e.PropertyName == nameof(SelectedGroup))
                GroupChanged();
        }

        private void GroupChanged()
        {
            if (SelectedGroup == null)
            {
                RequiredBeds = 0;
                return;
            }
            var batchIds = _dbRepository.BatchRepository.GetMany(x => x.FK_ID_HaajiGroup == SelectedGroup.ID).Select(x => x.ID);
            RequiredBeds = _dbRepository.PersonRepository.GetMany(x => batchIds.Contains(x.FK_ID_Batch)).Count();
        }

        private void CityChanged()
        {
            GetGroupsBasedOnCity();

            if (!string.IsNullOrEmpty(SelectedCity))
                try
                {
                    AvailableBeds = _dbRepository.RoomRepository.GetAllQueryable().Where(x => x.City == SelectedCity).Sum(x => x.Beds.Count(y => y.IsAvailable));
                }
                catch { AvailableBeds = 0; }
        }

        private void YearChanged()
        {
            GetGroupsBasedOnCity();
        }

        private void GetGroupsBasedOnCity()
        {
            if (string.IsNullOrEmpty(SelectedCity))
            {
                RequiredBeds = 0;
                Groups = new List<HaajiGroupDTO>();
                return;
            }

            if (SelectedCity == "Makkah")
                Groups = _dbRepository.HaajiGroupRepository.GetAllQueryable().
                    Where(x => !x.IsRoomAllotedMakkah && x.VisitYear == SelectedYear).
                    Select(x => new HaajiGroupDTO
                    {
                        ArrivalDateMadinah = x.ArrivalDateMadinah,
                        ArrivalDateMakkah = x.ArrivalDateMakkah,
                        DepartureDateMadinah = x.DepartureDateMadinah,
                        DepartureDateMakkah = x.DepartureDateMakkah,
                        ID = x.ID,
                        IncomingFlight = x.IncomingFlight,
                        IsRoomAllotedMadinah = x.IsRoomAllotedMadinah,
                        IsRoomAllotedMakkah = x.IsRoomAllotedMakkah,
                        Name = x.Name,
                        OutgoingFlight = x.OutgoingFlight,
                        StateName = x.StateName,
                        VisitYear = x.VisitYear
                    }).ToList();
            else
                Groups = _dbRepository.HaajiGroupRepository.GetAllQueryable().
                    Where(x => !x.IsRoomAllotedMadinah && x.VisitYear == SelectedYear).
                    Select(x => new HaajiGroupDTO
                    {
                        ArrivalDateMadinah = x.ArrivalDateMadinah,
                        ArrivalDateMakkah = x.ArrivalDateMakkah,
                        DepartureDateMadinah = x.DepartureDateMadinah,
                        DepartureDateMakkah = x.DepartureDateMakkah,
                        ID = x.ID,
                        IncomingFlight = x.IncomingFlight,
                        IsRoomAllotedMadinah = x.IsRoomAllotedMadinah,
                        IsRoomAllotedMakkah = x.IsRoomAllotedMakkah,
                        Name = x.Name,
                        OutgoingFlight = x.OutgoingFlight,
                        StateName = x.StateName,
                        VisitYear = x.VisitYear
                    }).ToList();
        }

        private void ExecuteAllocateCommand()
        {
            var group = _dbRepository.HaajiGroupRepository.GetFirstOrDefault(x => x.ID == SelectedGroup.ID);
            if ((SelectedCity == "Makkah" && group.IsRoomAllotedMakkah) || (SelectedCity == "Madinah" && group.IsRoomAllotedMadinah))
            {
                MessageBox.Show("Rooms for this group has already assigned", Constants.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (RequiredBeds > AvailableBeds)
            {
                MessageBox.Show("You do not have enough beds to allocate.", Constants.Error, MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            var batches = _dbRepository.BatchRepository.GetAllQueryable().
                Where(x => x.FK_ID_HaajiGroup == group.ID).
                Select(x => new BatchDTO
                {
                    BatchName = x.BatchName,
                    ContactNo = x.ContactNo,
                    FK_ID_HaajiGroup = x.FK_ID_HaajiGroup,
                    ID = x.ID,
                    Photo = x.Photo,
                    PeopleCount = x.People.Count
                }).ToList();

            var batchIds = batches.Select(x => x.ID);
            var people = _dbRepository.PersonRepository.GetMany(x => batchIds.Contains(x.FK_ID_Batch)).ToList();

            var rooms = _dbRepository.RoomRepository.GetAllQueryable().
                 Where(x => x.City == SelectedCity && x.IsAvailable).
                 Select(x => new RoomDTO
                 {
                     City = x.City,
                     HotelName = x.HotelName,
                     ID = x.ID,
                     IsAvailable = x.IsAvailable,
                     IsDirty = x.IsDirty,
                     RoomNumber = x.RoomNumber,
                     Capacity = x.Beds.Count.ToString(),
                     AvailableBeds = x.Beds.Count(b => b.IsAvailable)
                 }).ToList();

            var beds = _dbRepository.BedRepository.GetAllQueryable().Where(x => x.IsAvailable).ToList();

            SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString);
            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            try
            {
                while (people.Any())
                {
                    var peopleCount = people.Count();

                    AllotRooms(rooms, beds, batches, people, false, sqlConnection, sqlTransaction);
                    AllotRooms(rooms, beds, batches, people, true, sqlConnection, sqlTransaction);

                    if (peopleCount == people.Count)
                        break;
                }

                if (people.Any())
                {
                    if (MessageBox.Show("Could not allocate room to everybody, Do you still want to continue with room allocation? However you can allocate rooms manually for them.",
                    Constants.Error, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                        return;
                }

                //if (SelectedCity == "Makkah")
                //    group.IsRoomAllotedMakkah = true;
                //else
                //    group.IsRoomAllotedMadinah = true;
                //_repository.HaajiGroupRepository.Update(group);

                if (SelectedCity == "Makkah")
                    new SqlCommand("update HaajiGroup set IsRoomAllotedMakkah = 1 where ID = " + group.ID, sqlConnection, sqlTransaction).ExecuteNonQuery();
                else
                    new SqlCommand("update HaajiGroup set IsRoomAllotedMadinah = 1 where ID = " + group.ID, sqlConnection, sqlTransaction).ExecuteNonQuery();

                //_repository.Commit();
                sqlTransaction.Commit();
                MessageBox.Show("Successfully allocatated rooms", Constants.Success, MessageBoxButton.OK, MessageBoxImage.Information);

                RegionManager.RequestNavigate(Constants.ContentRegion, Constants.Welcome);
            }
            catch
            {
                sqlTransaction.Rollback();
                MessageBox.Show("Something went wrong while alocating roos from the group", Constants.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private void AllotRooms(List<RoomDTO> rooms, List<Bed> beds, List<BatchDTO> batches, List<Person> people, bool usedRooms, SqlConnection sqlConnection, SqlTransaction sqlTransaction)
        {
            var roomsToRemve = new List<RoomDTO>();

            // Exact Match
            rooms.Where(room => room.IsDirty == usedRooms).ForEach(room =>
              {
                  var batchInSingleRoom = batches.FirstOrDefault(batch => batch.PeopleCount == room.AvailableBeds);
                  if (batchInSingleRoom != null)
                  {
                      var peopleInBatch = people.FindAll(person => person.FK_ID_Batch == batchInSingleRoom.ID).ToList();

                      if (usedRooms)
                      {
                          //var peopleAlreadyInRoom = _repository.BedRepository.GetAllQueryableWithTracking().
                          //Where(x => x.FK_ID_Room == room.ID && !x.IsAvailable).
                          //Select(x => x.People.FirstOrDefault()).
                          //Where(x => x != null).ToList();

                          var peopleAlreadyInRoom = new List<Person>();
                          SqlDataReader dataReader = new SqlCommand("select * from person where FK_ID_Bed_Makkah in (select ID from Bed where FK_ID_Room = " + room.ID + ") " +
                              "or FK_ID_Bed_Madinah in (select ID from Bed where FK_ID_Room = " + room.ID + ")",
                              sqlConnection, sqlTransaction).ExecuteReader();
                          while (dataReader.Read())
                              peopleAlreadyInRoom.Add(new Person { Gender = dataReader["Gender"].ToString() });
                          dataReader.Close();

                          var femalesExistsInRoom = peopleAlreadyInRoom.Any(x => x.Gender == "Female");
                          var malesExistsInRoom = peopleAlreadyInRoom.Any(x => x.Gender == "Male");
                          var femalesExistsInBatch = peopleInBatch.Any(x => x.Gender == "Female");
                          var malesExistsInBatch = peopleInBatch.Any(x => x.Gender == "Male");

                          if ((femalesExistsInRoom && malesExistsInBatch) || (malesExistsInRoom && femalesExistsInBatch))
                              return;
                      }

                      people.RemoveAll(person => person.FK_ID_Batch == batchInSingleRoom.ID);

                      roomsToRemve.Add(room);
                      //var roomToUpdate = _repository.RoomRepository.GetFirstOrDefaultWithTracking(r => r.ID == room.ID);
                      //roomToUpdate.IsDirty = true;
                      //roomToUpdate.IsAvailable = false;
                      //_repository.RoomRepository.Update(roomToUpdate);
                      new SqlCommand("update Room set IsDirty = 1, IsAvailable = 0 where ID = " + room.ID, sqlConnection, sqlTransaction).ExecuteNonQuery();

                      batches.Remove(batchInSingleRoom);

                      var bedsInRoom = beds.Where(x => x.FK_ID_Room == room.ID).ToList();
                      beds.RemoveAll(x => x.FK_ID_Room == room.ID);

                      peopleInBatch.ForEach((person, index) =>
                      {
                          //if (SelectedCity == "Makkah")
                          //    person.FK_ID_Bed_Makkah = bedsInRoom[index].ID;
                          //else
                          //    person.FK_ID_Bed_Madinah = bedsInRoom[index].ID;
                          //_repository.PersonRepository.Update(person);

                          if (SelectedCity == "Makkah")
                              new SqlCommand("update Person set FK_ID_Bed_Makkah =" + bedsInRoom[index].ID + " where ID = " + person.ID, sqlConnection, sqlTransaction).ExecuteNonQuery();
                          else
                              new SqlCommand("update Person set FK_ID_Bed_Madinah =" + bedsInRoom[index].ID + " where ID = " + person.ID, sqlConnection, sqlTransaction).ExecuteNonQuery();


                          //bedsInRoom[index].IsAvailable = false;
                          //_repository.BedRepository.Update(bedsInRoom[index]);
                          new SqlCommand("update Bed set IsAvailable = 0 where ID = " + bedsInRoom[index].ID, sqlConnection, sqlTransaction).ExecuteNonQuery();
                      });
                  }
              });

            roomsToRemve.ForEach(x => rooms.Remove(x));
            roomsToRemve.Clear();

            // Split batch to multiple rooms fresh room
            rooms.Where(room => room.IsDirty == usedRooms).OrderByDescending(room => room.AvailableBeds).ForEach(room =>
            {
                var batchInSingleRoom = batches.OrderByDescending(batch => batch.PeopleCount).FirstOrDefault(batch => batch.PeopleCount > room.AvailableBeds);
                if (batchInSingleRoom != null)
                {
                    var females = people.Where(person => person.FK_ID_Batch == batchInSingleRoom.ID && person.Gender == "Female");
                    var males = people.Where(person => person.FK_ID_Batch == batchInSingleRoom.ID && person.Gender == "Male");

                    var peopleSelected = new List<Person>();
                    if (females.Count() > room.AvailableBeds)
                    {
                        peopleSelected = females.Take(room.AvailableBeds).ToList();
                    }
                    else if (females.Count() == room.AvailableBeds)
                    {
                        peopleSelected = females.Where(x => true).ToList();
                    }
                    else if (females.Count() < room.AvailableBeds)
                    {
                        peopleSelected = females.Where(x => true).ToList();
                        var extraMalesRequired = room.AvailableBeds - females.Count();
                        var extraPerson = males.Take(extraMalesRequired).ToList();
                        peopleSelected.AddRange(extraPerson);
                    }

                    if (usedRooms)
                    {
                        //var peopleAlreadyInRoom = _repository.BedRepository.GetAllQueryableWithTracking().
                        //Where(x => x.FK_ID_Room == room.ID && !x.IsAvailable).
                        //Select(x => x.People.FirstOrDefault()).
                        //Where(x => x != null).ToList();

                        var peopleAlreadyInRoom = new List<Person>();
                        SqlDataReader dataReader = new SqlCommand("select * from person where FK_ID_Bed_Makkah in (select ID from Bed where FK_ID_Room = " + room.ID + ") " +
                            "or FK_ID_Bed_Madinah in (select ID from Bed where FK_ID_Room = " + room.ID + ")",
                            sqlConnection, sqlTransaction).ExecuteReader();
                        while (dataReader.Read())
                            peopleAlreadyInRoom.Add(new Person { Gender = dataReader["Gender"].ToString() });
                        dataReader.Close();

                        var femalesExistsInRoom = peopleAlreadyInRoom.Any(x => x.Gender == "Female");
                        var malesExistsInRoom = peopleAlreadyInRoom.Any(x => x.Gender == "Male");
                        var femalesExistsInBatch = peopleSelected.Any(x => x.Gender == "Female");
                        var malesExistsInBatch = peopleSelected.Any(x => x.Gender == "Male");

                        if ((femalesExistsInRoom && malesExistsInBatch) || (malesExistsInRoom && femalesExistsInBatch))
                            return;
                    }

                    roomsToRemve.Add(room);
                    //var roomToUpdate = _repository.RoomRepository.GetFirstOrDefaultWithTracking(r => r.ID == room.ID);
                    //roomToUpdate.IsDirty = true;
                    //roomToUpdate.IsAvailable = false;
                    //_repository.RoomRepository.Update(roomToUpdate);
                    new SqlCommand("update Room set IsDirty = 1, IsAvailable = 0 where ID = " + room.ID, sqlConnection, sqlTransaction).ExecuteNonQuery();

                    peopleSelected.ForEach(x => people.Remove(x));
                    batchInSingleRoom.PeopleCount -= room.AvailableBeds;

                    var bedsInRoom = beds.Where(x => x.FK_ID_Room == room.ID).ToList();
                    beds.RemoveAll(x => x.FK_ID_Room == room.ID);

                    peopleSelected.ForEach((person, index) =>
                    {
                        //if (SelectedCity == "Makkah")
                        //    person.FK_ID_Bed_Makkah = bedsInRoom[index].ID;
                        //else
                        //    person.FK_ID_Bed_Madinah = bedsInRoom[index].ID;
                        //_repository.PersonRepository.Update(person);

                        if (SelectedCity == "Makkah")
                            new SqlCommand("update Person set FK_ID_Bed_Makkah =" + bedsInRoom[index].ID + " where ID = " + person.ID, sqlConnection, sqlTransaction).ExecuteNonQuery();
                        else
                            new SqlCommand("update Person set FK_ID_Bed_Madinah =" + bedsInRoom[index].ID + " where ID = " + person.ID, sqlConnection, sqlTransaction).ExecuteNonQuery();

                        //bedsInRoom[index].IsAvailable = false;
                        //_repository.BedRepository.Update(bedsInRoom[index]);
                        new SqlCommand("update Bed set IsAvailable = 0 where ID = " + bedsInRoom[index].ID, sqlConnection, sqlTransaction).ExecuteNonQuery();

                    });
                }
            });

            roomsToRemve.ForEach(x => rooms.Remove(x));
            roomsToRemve.Clear();

            // small group bigger room
            rooms.Where(room => room.IsDirty == usedRooms).OrderByDescending(room => room.AvailableBeds).ForEach(room =>
              {
                  var batchInSingleRoom = batches.OrderByDescending(batch => batch.PeopleCount).FirstOrDefault(batch => batch.PeopleCount < room.AvailableBeds);
                  if (batchInSingleRoom != null)
                  {
                      var peopleInBatch = people.FindAll(person => person.FK_ID_Batch == batchInSingleRoom.ID).ToList();

                      if (usedRooms)
                      {
                          //var peopleAlreadyInRoom = _repository.BedRepository.GetAllQueryableWithTracking().
                          //Where(x => x.FK_ID_Room == room.ID && !x.IsAvailable).
                          //Select(x => x.People.FirstOrDefault()).
                          //Where(x => x != null).ToList();

                          var peopleAlreadyInRoom = new List<Person>();
                          SqlDataReader dataReader = new SqlCommand("select * from person where FK_ID_Bed_Makkah in (select ID from Bed where FK_ID_Room = " + room.ID + ") " +
                              "or FK_ID_Bed_Madinah in (select ID from Bed where FK_ID_Room = " + room.ID + ")",
                              sqlConnection, sqlTransaction).ExecuteReader();
                          while (dataReader.Read())
                              peopleAlreadyInRoom.Add(new Person { Gender = dataReader["Gender"].ToString() });
                          dataReader.Close();

                          var femalesExistsInRoom = peopleAlreadyInRoom.Any(x => x.Gender == "Female");
                          var malesExistsInRoom = peopleAlreadyInRoom.Any(x => x.Gender == "Male");
                          var femalesExistsInBatch = peopleInBatch.Any(x => x.Gender == "Female");
                          var malesExistsInBatch = peopleInBatch.Any(x => x.Gender == "Male");

                          if ((femalesExistsInRoom && malesExistsInBatch) || (malesExistsInRoom && femalesExistsInBatch))
                              return;
                      }

                      batches.Remove(batchInSingleRoom);

                      room.AvailableBeds -= batchInSingleRoom.PeopleCount;

                      //var roomToUpdate = _repository.RoomRepository.GetFirstOrDefaultWithTracking(r => r.ID == room.ID);
                      //roomToUpdate.IsDirty = true;
                      //roomToUpdate.IsAvailable = true;
                      //_repository.RoomRepository.Update(roomToUpdate);
                      new SqlCommand("update Room set IsDirty = 1 where ID = " + room.ID, sqlConnection, sqlTransaction).ExecuteNonQuery();

                      if (room.AvailableBeds <= 0)
                      {
                          new SqlCommand("update Room set IsAvailable = 0  where ID = " + room.ID, sqlConnection, sqlTransaction).ExecuteNonQuery();
                          roomsToRemve.Add(room);
                      }

                      var bedsInRoom = beds.Where(x => x.FK_ID_Room == room.ID).Take(batchInSingleRoom.PeopleCount).ToList();
                      bedsInRoom.ForEach(x => beds.Remove(x));

                      people.RemoveAll(person => person.FK_ID_Batch == batchInSingleRoom.ID);

                      peopleInBatch.ForEach((person, index) =>
                      {
                          //if (SelectedCity == "Makkah")
                          //    person.FK_ID_Bed_Makkah = bedsInRoom[index].ID;
                          //else
                          //    person.FK_ID_Bed_Madinah = bedsInRoom[index].ID;
                          //_repository.PersonRepository.Update(person);

                          if (SelectedCity == "Makkah")
                              new SqlCommand("update Person set FK_ID_Bed_Makkah =" + bedsInRoom[index].ID + " where ID = " + person.ID, sqlConnection, sqlTransaction).ExecuteNonQuery();
                          else
                              new SqlCommand("update Person set FK_ID_Bed_Madinah =" + bedsInRoom[index].ID + " where ID = " + person.ID, sqlConnection, sqlTransaction).ExecuteNonQuery();

                          //bedsInRoom[index].IsAvailable = false;
                          //_repository.BedRepository.Update(bedsInRoom[index]);
                          new SqlCommand("update Bed set IsAvailable = 0 where ID = " + bedsInRoom[index].ID, sqlConnection, sqlTransaction).ExecuteNonQuery();
                      });
                  }
              });

            roomsToRemve.ForEach(x => rooms.Remove(x));
            roomsToRemve.Clear();
        }
    }
}
