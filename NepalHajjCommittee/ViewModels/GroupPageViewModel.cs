using System;
using NepalHajjCommittee.Database;
using Prism.Regions;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using NepalHajjCommittee.Database.EDMX;
using Prism.Commands;

namespace NepalHajjCommittee.ViewModels
{
    public class GroupPageViewModel : ViewModelBase
    {
        private List<int> _years;
        private readonly INepalHajjCommitteeRepository _repository;
        private HaajiGroup _groupModel;
        private GridLength _buttonPanelHeight;
        private GridLength _batchGridHeight;
        private ICommand _saveCommand;
        private bool _fromDatabase;
        private ICommand _addBatch;
        private ICommand _editBatch;
        private ICommand _deleteBatch;
        private Batch _selectedBatch;
        private ICommand _backCommand;

        public GroupPageViewModel(IRegionManager regionManager, INepalHajjCommitteeRepository repository) : base(regionManager)
        {
            _repository = repository;

            Years = new List<int> { 2019, 2020, 2021, 2022, 2023, 2024, 2025, 2026, 2027, 2028, 2029, 2030 };
        }

        public List<int> Years { get => _years; set => SetProperty(ref _years, value); }

        public HaajiGroup GroupModel { get => _groupModel; set => SetProperty(ref _groupModel, value); }

        public GridLength ButtonPanelHeight { get => _buttonPanelHeight; set => SetProperty(ref _buttonPanelHeight, value); }

        public GridLength BatchGridHeight { get => _batchGridHeight; set => SetProperty(ref _batchGridHeight, value); }

        public Batch SelectedBatch { get => _selectedBatch; set { SetProperty(ref _selectedBatch, value); RaisePropertyChanged(nameof(IsBatchSelected)); } }

        public bool IsBatchSelected => SelectedBatch != null;

        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(ExecuteSaveCommand));

        public ICommand BackCommand => _backCommand ?? (_backCommand = new DelegateCommand(ExecuteBackCommand));

        public ICommand AddBatch => _addBatch ?? (_addBatch = new DelegateCommand(ExecuteAddBatch));

        public ICommand EditBatch => _editBatch ?? (_editBatch = new DelegateCommand(ExecuteEditBatch));

        public ICommand DeleteBatch => _deleteBatch ?? (_deleteBatch = new DelegateCommand(ExecuteDeleteBatch));

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            if (navigationContext.Parameters.TryGetValue("Model", out HaajiGroup groupModel))
                FetchFromDatabase(groupModel.ID);
            else
                CreateBlankModel();
        }

        private void FetchFromDatabase(int id)
        {
            var groupModel = _repository.HaajiGroupRepository.GetFirstOrDefault(x => x.ID == id);
            if (groupModel == null)
            {
                CreateBlankModel();
                return;
            }

            _fromDatabase = true;
            GroupModel = groupModel;
            ButtonPanelHeight = GridLength.Auto;
            BatchGridHeight = new GridLength(1, GridUnitType.Star);
        }

        private void CreateBlankModel()
        {
            _fromDatabase = false;
            ButtonPanelHeight = new GridLength(0);
            BatchGridHeight = new GridLength(0);
            GroupModel = new HaajiGroup
            {
                VisitYear = DateTime.Now.Year,
                Name = string.Empty,
                ArrivalDateMakkah = DateTime.Today,
                DepartureDateMadinah = DateTime.Today,
                ArrivalDateMadinah = DateTime.Today,
                DepartureDateMakkah = DateTime.Today,
                IncomingFlight = string.Empty,
                OutgoingFlight = string.Empty,
                StateName = string.Empty
            };
        }

        private void ExecuteBackCommand()
        {
            RegionManager.RequestNavigate(Constants.ContentRegion, Constants.GroupManagement);
        }

        private void ExecuteSaveCommand()
        {
            try
            {
                if (!ValidateData())
                {
                    MessageBox.Show("Something wrong with the data provided", Constants.Error, MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                if (!_fromDatabase)
                    _repository.HaajiGroupRepository.Insert(GroupModel);
                else
                    _repository.HaajiGroupRepository.Update(GroupModel);

                _repository.Commit();

                FetchFromDatabase(GroupModel.ID);

                MessageBox.Show("Group information saved successfully", Constants.Success, MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Could not save group information", Constants.Error, MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(GroupModel.Name))
                return false;
            if (string.IsNullOrEmpty(GroupModel.StateName))
                return false;
            if (string.IsNullOrEmpty(GroupModel.IncomingFlight))
                return false;
            if (GroupModel.DepartureDateMakkah < GroupModel.ArrivalDateMakkah)
                return false;
            //if (GroupModel.ArrivalDateMadinah < GroupModel.DepartureDateMakkah)
            //    return false;
            if (GroupModel.DepartureDateMadinah < GroupModel.ArrivalDateMadinah)
                return false;
            return true;
        }

        private void ExecuteAddBatch()
        {
            RegionManager.RequestNavigate(Constants.ContentRegion, Constants.BatchPage, new NavigationParameters { { "GroupModel", GroupModel } });
        }

        private void ExecuteEditBatch()
        {
            RegionManager.RequestNavigate(Constants.ContentRegion, Constants.BatchPage, new NavigationParameters { { "Model", SelectedBatch }, { "GroupModel", GroupModel } });
        }

        private void ExecuteDeleteBatch()
        {
            if (MessageBox.Show(
                    "Are you sure you want to delete batch, this will also delete all the people associated with this batch",
                    Constants.Confirmation, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            try
            {
                var batchToDelete =
                    _repository.BatchRepository.GetFirstOrDefault(x => x.ID == SelectedBatch.ID);
                if (batchToDelete == null)
                    throw new Exception();

                var peopleIds = batchToDelete.People.Select(person => person.ID);
                _repository.PersonRepository.Delete(x => peopleIds.Contains(x.ID));

                _repository.BatchRepository.Delete(x => x.ID == batchToDelete.ID);
                _repository.Commit();

                MessageBox.Show("Batch deleted successfully", Constants.Success, MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Could not delete the batch", Constants.Error, MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                FetchFromDatabase(GroupModel.ID);
            }
        }
    }
}
