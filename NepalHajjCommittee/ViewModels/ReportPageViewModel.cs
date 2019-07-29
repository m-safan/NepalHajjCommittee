using NepalHajjCommittee.Database;
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
    public class ReportPageViewModel : ViewModelBase
    {
        private INepalHajjCommitteeRepository _repository;
        private FilterModel _filterModel;
        private List<SearchedResults> _searchedResults;
        private List<int> _years;
        private List<string> _genders;
        private ICommand _searchCommand;
        private ICommand _exportCommand;
        private ColumnVisibility _columnVisibility;

        public ReportPageViewModel(IRegionManager regionManager, INepalHajjCommitteeRepository repository) : base(regionManager)
        {
            _repository = repository;

            Years = new List<int> { 2019, 2020, 2021, 2022, 2023, 2024, 2025, 2026, 2027, 2028, 2029, 2030 };
            Genders = new List<string> { "Male", "Female" };
            FilterModel = new FilterModel { VisitYear = DateTime.Now.Year };
            ColumnVisibility = new ColumnVisibility();
        }

        public bool IsExportEnabled => SearchedResults != null && SearchedResults.Any();
        public FilterModel FilterModel { get => _filterModel; set => SetProperty(ref _filterModel, value); }
        public ColumnVisibility ColumnVisibility { get => _columnVisibility; set => SetProperty(ref _columnVisibility, value); }
        public List<SearchedResults> SearchedResults { get => _searchedResults; set { SetProperty(ref _searchedResults, value); RaisePropertyChanged(nameof(IsExportEnabled)); } }
        public List<int> Years { get => _years; set => SetProperty(ref _years, value); }
        public List<string> Genders { get => _genders; set => SetProperty(ref _genders, value); }
        public ICommand SearchCommand => _searchCommand ?? (_searchCommand = new DelegateCommand(ExecuteSearchCommand));
        public ICommand ExportCommand => _exportCommand ?? (_exportCommand = new DelegateCommand(ExecuteExportCommand));

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            FetchResults();
        }

        private void FetchResults()
        {
            var people = _repository.PersonRepository.GetAllQueryable();

            people = people.Where(x => x.Batch.HaajiGroup.VisitYear == FilterModel.VisitYear);

            if (!string.IsNullOrEmpty(FilterModel.Name))
                people = people.Where(x => x.Name.ToLower().Contains(FilterModel.Name.ToLower())); ;

            if (!string.IsNullOrEmpty(FilterModel.ContactNo))
                people = people.Where(x => x.ContactNo.ToLower().Contains(FilterModel.ContactNo.ToLower())); ;

            if (!string.IsNullOrEmpty(FilterModel.BatchName))
                people = people.Where(x => x.Batch.BatchName.ToLower().Contains(FilterModel.BatchName.ToLower()));

            if (!string.IsNullOrEmpty(FilterModel.Gender))
                people = people.Where(x => x.Gender == FilterModel.Gender);

            if (!string.IsNullOrEmpty(FilterModel.GroupName))
                people = people.Where(x => x.Batch.HaajiGroup.Name.ToLower().Contains(FilterModel.GroupName.ToLower()));

            if (!string.IsNullOrEmpty(FilterModel.IncomingFlight))
                people = people.Where(x => x.Batch.HaajiGroup.IncomingFlight.ToLower().Contains(FilterModel.IncomingFlight.ToLower()));

            if (!string.IsNullOrEmpty(FilterModel.OutgoingFlight))
                people = people.Where(x => x.Batch.HaajiGroup.OutgoingFlight.ToLower().Contains(FilterModel.OutgoingFlight.ToLower()));

            if (!string.IsNullOrEmpty(FilterModel.PassportNo))
                people = people.Where(x => x.PassportNo.ToLower().Contains(FilterModel.PassportNo.ToLower()));

            if (!string.IsNullOrEmpty(FilterModel.State))
                people = people.Where(x => x.Batch.HaajiGroup.StateName.ToLower().Contains(FilterModel.State.ToLower()));

            if (FilterModel.ArrivalDateMakkah != null)
                people = people.Where(x => FilterModel.ArrivalDateMakkah == x.Batch.HaajiGroup.ArrivalDateMakkah);

            if (FilterModel.DepartureDateMakkah != null)
                people = people.Where(x => FilterModel.DepartureDateMakkah == x.Batch.HaajiGroup.DepartureDateMakkah);

            if (FilterModel.ArrivalDateMadinah != null)
                people = people.Where(x => FilterModel.ArrivalDateMadinah == x.Batch.HaajiGroup.ArrivalDateMadinah);

            if (FilterModel.DepartureDateMadinah != null)
                people = people.Where(x => FilterModel.DepartureDateMadinah == x.Batch.HaajiGroup.DepartureDateMadinah);

            SearchedResults = people.Select(x => new SearchedResults
            {
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

            if (!SearchedResults.Any())
                MessageBox.Show("No records found", Constants.Error, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ExecuteSearchCommand()
        {
            FetchResults();
        }

        private void ExecuteExportCommand()
        {
            //System.Diagnostics.Process.Start("file:///C:/ProgramData/NepalHaajiCommittee/printing.html");
            RegionManager.RequestNavigate(Constants.ContentRegion, Constants.ReportExportPage, new NavigationParameters { { "Model", SearchedResults } });
        }
    }
}
