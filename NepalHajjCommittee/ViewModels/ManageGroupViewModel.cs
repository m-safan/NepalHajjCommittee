using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MoreLinq;
using NepalHajjCommittee.Database;
using NepalHajjCommittee.Database.EDMX;
using Prism.Commands;
using Prism.Regions;

namespace NepalHajjCommittee.ViewModels
{
    public class ManageGroupViewModel : ViewModelBase
    {
        private int _selectedYear;
        private List<int> _years;
        private List<HaajiGroup> _availableGroups;
        private readonly INepalHajjCommitteeRepository _repository;
        private HaajiGroup _selectedGroup;
        private ICommand _addGroup;
        private ICommand _editGroup;
        private ICommand _deleteGroup;

        public ManageGroupViewModel(IRegionManager regionManager, INepalHajjCommitteeRepository repository) : base(regionManager)
        {
            _repository = repository;

            PropertyChanged += This_PropertyChanged;

            Years = new List<int> { 2019, 2020, 2021, 2022, 2023, 2024, 2025, 2026, 2027, 2028, 2029, 2030 };
        }

        public List<int> Years { get => _years; set => SetProperty(ref _years, value); }

        public List<HaajiGroup> AvailableGroups { get => _availableGroups; set => SetProperty(ref _availableGroups, value); }

        public int SelectedYear { get => _selectedYear; set => SetProperty(ref _selectedYear, value); }

        public HaajiGroup SelectedGroup { get => _selectedGroup; set { SetProperty(ref _selectedGroup, value); RaisePropertyChanged(nameof(IsGroupSelected)); } }

        public bool IsGroupSelected => SelectedGroup != null;

        public ICommand AddGroup => _addGroup ?? (_addGroup = new DelegateCommand(ExecuteAddGroup));

        public ICommand EditGroup => _editGroup ?? (_editGroup = new DelegateCommand(ExecuteEditGroup));

        public ICommand DeleteGroup => _deleteGroup ?? (_deleteGroup = new DelegateCommand(ExecuteDeleteGroup));

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            SelectedYear = DateTime.Now.Year;
            FetchGroups();
        }

        private void ExecuteAddGroup()
        {
            RegionManager.RequestNavigate(Constants.ContentRegion, Constants.GroupPage);
        }

        private void ExecuteEditGroup()
        {
            RegionManager.RequestNavigate(Constants.ContentRegion, Constants.GroupPage, new NavigationParameters { { "Model", SelectedGroup } });
        }

        private void ExecuteDeleteGroup()
        {
            if (MessageBox.Show(
                    "Are you sure you want to delete group, this will also delete all the batches and people associated with this group",
                    Constants.Confirmation, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            try
            {
                var groupToDelete =
                    _repository.HaajiGroupRepository.GetFirstOrDefault(x => x.ID == SelectedGroup.ID);
                if (groupToDelete == null)
                    throw new Exception();

                groupToDelete.Batches.ForEach(batch =>
                {
                    var peopleIds = batch.People.Select(person => person.ID);
                    _repository.PersonRepository.Delete(x => peopleIds.Contains(x.ID));
                });
                var batchIds = groupToDelete.Batches.Select(batch => batch.ID);
                _repository.BatchRepository.Delete(x => batchIds.Contains(x.ID));

                _repository.HaajiGroupRepository.Delete(x => x.ID == groupToDelete.ID);
                _repository.Commit();

                MessageBox.Show("Group deleted successfully", Constants.Success, MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Could not delete the group", Constants.Error, MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                FetchGroups();
            }
        }

        private void FetchGroups()
        {
            AvailableGroups = _repository.HaajiGroupRepository.GetMany(x => x.VisitYear == SelectedYear).ToList();
        }

        private void This_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedYear))
                FetchGroups();
        }
    }
}
