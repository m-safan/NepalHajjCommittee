using NepalHajjCommittee.Database;
using NepalHajjCommittee.Database.EDMX;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace NepalHajjCommittee.ViewModels
{
    public class PersonPageViewModel : ViewModelBase
    {
        private readonly INepalHajjCommitteeRepository _repository;
        private ICommand _chooseImage;
        private ICommand _backCommand;
        private ICommand _saveCommand;
        private Person _personModel;
        private Batch _batchModel;
        private HaajiGroup _groupModel;
        private bool _fromDatabase;
        private List<string> _genders;
        private int _genderSelectedIndex;

        public PersonPageViewModel(IRegionManager regionManager, INepalHajjCommitteeRepository repository) : base(regionManager)
        {
            _repository = repository;

            Genders = new List<string> { "Male", "Female" };
        }

        public BitmapSource Photo
        {
            get
            {
                try
                {
                    if (PersonModel == null || string.IsNullOrEmpty(PersonModel.Photo)) return null;

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(PersonModel.Photo);
                    bitmap.EndInit();
                    return bitmap;
                }
                catch
                {
                    return null;
                }
            }
        }

        public int GenderSelectedIndex { get => _genderSelectedIndex; set => SetProperty(ref _genderSelectedIndex, value); }

        public List<string> Genders { get => _genders; set => SetProperty(ref _genders, value); }

        public ICommand ChooseImage => _chooseImage ?? (_chooseImage = new DelegateCommand(ExecuteChooseImage));

        public Person PersonModel { get => _personModel; set { SetProperty(ref _personModel, value); RaisePropertyChanged(nameof(Photo)); } }

        public Batch BatchModel { get => _batchModel; set { SetProperty(ref _batchModel, value); RaisePropertyChanged(nameof(Photo)); } }

        public HaajiGroup GroupModel { get => _groupModel; set => SetProperty(ref _groupModel, value); }

        public ICommand BackCommand => _backCommand ?? (_backCommand = new DelegateCommand(ExecuteBackCommand));

        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(ExecuteSaveCommand));

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            GenderSelectedIndex = -1;

            if (navigationContext.Parameters.TryGetValue("GroupModel", out HaajiGroup groupModel))
                GroupModel = groupModel;

            if (navigationContext.Parameters.TryGetValue("BatchModel", out Batch batchModel))
                BatchModel = batchModel;

            if (navigationContext.Parameters.TryGetValue("Model", out Person personModel))
                FetchFromDatabase(personModel.ID);
            else
                CreateBlankModel();
        }

        private void FetchFromDatabase(int id)
        {
            var personModel = _repository.PersonRepository.GetFirstOrDefault(x => x.ID == id);
            if (personModel == null)
            {
                CreateBlankModel();
                return;
            }

            _fromDatabase = true;
            PersonModel = personModel;
        }

        private void CreateBlankModel()
        {
            _fromDatabase = false;
            PersonModel = new Person
            {
                MadinahToAirportBusNumber = string.Empty,
                MakkahToAirportBusNumber = string.Empty,
                MadinahToMakkahBusNumber = string.Empty,
                MakkahToMadinahBusNumber = string.Empty,
                Gender = string.Empty,
                Name = string.Empty,
                PassportNo = string.Empty,
                FK_ID_Batch = BatchModel.ID,
                Photo = string.Empty
            };
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
                    _repository.PersonRepository.Insert(PersonModel);
                else
                    _repository.PersonRepository.Update(PersonModel);

                _repository.Commit();

                FetchFromDatabase(PersonModel.ID);

                MessageBox.Show("Person information saved successfully", Constants.Success, MessageBoxButton.OK,
                    MessageBoxImage.Information);

                ExecuteBackCommand();
            }
            catch
            {
                MessageBox.Show("Could not save person information", Constants.Error, MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(PersonModel.Name))
                return false;
            if (string.IsNullOrEmpty(PersonModel.Gender))
                return false;
            if (string.IsNullOrEmpty(PersonModel.PassportNo))
                return false;
            return true;
        }

        private void ExecuteBackCommand()
        {
            RegionManager.RequestNavigate(Constants.ContentRegion, Constants.BatchPage, new NavigationParameters { { "Model", BatchModel }, { "GroupModel", GroupModel } });
        }

        private void ExecuteChooseImage()
        {
            var fileDialog = new OpenFileDialog { Filter = "Image files (*.png)|*.png" };
            if (fileDialog.ShowDialog() != true) return;

            const string folderSeparator = @"\";
            var newFilepath = $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}{folderSeparator}{Constants.MainFolder}{folderSeparator}{Constants.ImageFolder}{folderSeparator}{Guid.NewGuid()}.png";

            File.Copy(fileDialog.FileName, newFilepath);
            PersonModel.Photo = newFilepath;

            RaisePropertyChanged(nameof(Photo));
        }
    }
}
