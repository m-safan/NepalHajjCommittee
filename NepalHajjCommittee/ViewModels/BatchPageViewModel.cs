using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using NepalHajjCommittee.Database;
using NepalHajjCommittee.Database.EDMX;
using Prism.Commands;
using Prism.Regions;

namespace NepalHajjCommittee.ViewModels
{
    public class BatchPageViewModel : ViewModelBase
    {
        private readonly INepalHajjCommitteeRepository _repository;
        private ICommand _chooseImage;
        private Batch _batchModel;
        private GridLength _buttonPanelHeight;
        private GridLength _peopleGridHeight;
        private Person _selectedPerson;
        private ICommand _saveCommand;
        private ICommand _addPerson;
        private ICommand _deletePerson;
        private ICommand _editPerson;
        private bool _fromDatabase;
        private HaajiGroup _groupModel;
        private ICommand _backCommand;

        public BatchPageViewModel(IRegionManager regionManager, INepalHajjCommitteeRepository repository) : base(regionManager)
        {
            _repository = repository;
        }

        public BitmapSource Photo
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(BatchModel.Photo)) return null;

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(BatchModel.Photo);
                    bitmap.EndInit();
                    return bitmap;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ICommand ChooseImage => _chooseImage ?? (_chooseImage = new DelegateCommand(ExecuteChooseImage));

        public Batch BatchModel { get => _batchModel; set { SetProperty(ref _batchModel, value); RaisePropertyChanged(nameof(Photo)); } }

        public HaajiGroup GroupModel { get => _groupModel; set => SetProperty(ref _groupModel, value); }

        public GridLength ButtonPanelHeight { get => _buttonPanelHeight; set => SetProperty(ref _buttonPanelHeight, value); }

        public GridLength PeopleGridHeight { get => _peopleGridHeight; set => SetProperty(ref _peopleGridHeight, value); }

        public Person SelectedPerson { get => _selectedPerson; set { SetProperty(ref _selectedPerson, value); RaisePropertyChanged(nameof(IsPersonSelected)); } }

        public bool IsPersonSelected => SelectedPerson != null;

        public ICommand BackCommand => _backCommand ?? (_backCommand = new DelegateCommand(ExecuteBackCommand));

        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(ExecuteSaveCommand));

        public ICommand AddPerson => _addPerson ?? (_addPerson = new DelegateCommand(ExecuteAddPerson));

        public ICommand EditPerson => _editPerson ?? (_editPerson = new DelegateCommand(ExecuteEditPerson));

        public ICommand DeletePerson => _deletePerson ?? (_deletePerson = new DelegateCommand(ExecuteDeletePerson));

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            if (navigationContext.Parameters.TryGetValue("GroupModel", out HaajiGroup groupModel))
                GroupModel = groupModel;

            if (navigationContext.Parameters.TryGetValue("Model", out Batch batchModel))
                FetchFromDatabase(batchModel.ID);
            else
                CreateBlankModel();
        }

        private void FetchFromDatabase(int id)
        {
            var batchModel = _repository.BatchRepository.GetFirstOrDefault(x => x.ID == id);
            if (batchModel == null)
            {
                CreateBlankModel();
                return;
            }

            _fromDatabase = true;
            BatchModel = batchModel;
            ButtonPanelHeight = GridLength.Auto;
            PeopleGridHeight = new GridLength(1, GridUnitType.Star);
        }

        private void CreateBlankModel()
        {
            _fromDatabase = false;
            ButtonPanelHeight = new GridLength(0);
            PeopleGridHeight = new GridLength(0);
            BatchModel = new Batch
            {
                FK_ID_HaajiGroup = GroupModel.ID,
                BatchName = string.Empty,
                Photo = string.Empty
            };
        }

        private void ExecuteChooseImage()
        {
            var fileDialog = new OpenFileDialog { Filter = "Image files (*.png)|*.png" };
            if (fileDialog.ShowDialog() != true) return;

            const string folderSeparator = @"\";
            var newFilepath = $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}{folderSeparator}{Constants.MainFolder}{folderSeparator}{Constants.ImageFolder}{folderSeparator}{Guid.NewGuid()}.png";

            File.Copy(fileDialog.FileName, newFilepath);
            BatchModel.Photo = newFilepath;

            RaisePropertyChanged(nameof(Photo));
        }

        private void ExecuteBackCommand()
        {
            RegionManager.RequestNavigate(Constants.ContentRegion, Constants.GroupPage, new NavigationParameters { { "Model", GroupModel } });
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
                    _repository.BatchRepository.Insert(BatchModel);
                else
                    _repository.BatchRepository.Update(BatchModel);

                _repository.Commit();

                FetchFromDatabase(BatchModel.ID);

                MessageBox.Show("Batch information saved successfully", Constants.Success, MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Could not save Batch information", Constants.Error, MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(BatchModel.BatchName))
                return false;
            return true;
        }

        private void ExecuteAddPerson()
        {
            RegionManager.RequestNavigate(Constants.ContentRegion, Constants.PersonPage, new NavigationParameters { { "BatchModel", BatchModel }, { "GroupModel", GroupModel } });
        }

        private void ExecuteEditPerson()
        {
            RegionManager.RequestNavigate(Constants.ContentRegion, Constants.PersonPage, new NavigationParameters { { "Model", SelectedPerson }, { "BatchModel", BatchModel }, { "GroupModel", GroupModel } });
        }

        private void ExecuteDeletePerson()
        {
            if (MessageBox.Show(
                    "Are you sure you want to delete person,",
                    Constants.Confirmation, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            try
            {
                _repository.PersonRepository.Delete(x => SelectedPerson.ID == x.ID);

                _repository.Commit();

                MessageBox.Show("Person deleted successfully", Constants.Success, MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Could not delete the person", Constants.Error, MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                FetchFromDatabase(BatchModel.ID);
            }
        }

    }
}
