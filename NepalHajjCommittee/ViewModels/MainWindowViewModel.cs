using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Prism.Commands;
using Prism.Regions;

namespace NepalHajjCommittee.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Visibility _loginWindowVisibility = Visibility.Visible;
        private Visibility _workWindowVisibility = Visibility.Collapsed;
        private ICommand _loginCommand;
        private string _userName;
        private ICommand _logoutCommand;
        private ICommand _navigateToPage;

        public MainWindowViewModel(IRegionManager regionManager) : base(regionManager)
        {

        }

        public string UserName { get => _userName; set => SetProperty(ref _userName, value); }

        public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand<PasswordBox>(ExecuteLoginCommand));

        public ICommand LogoutCommand => _logoutCommand ?? (_logoutCommand = new DelegateCommand(ExecuteLogoutCommand));

        public ICommand NavigateToPage => _navigateToPage ?? (_navigateToPage = new DelegateCommand<string>(ExecuteNavigateToPage));

        public Visibility LoginWindowVisibility { get => _loginWindowVisibility; set => SetProperty(ref _loginWindowVisibility, value); }

        public Visibility WorkWindowVisibility { get => _workWindowVisibility; set => SetProperty(ref _workWindowVisibility, value); }

        private void ExecuteLoginCommand(PasswordBox obj)
        {
            if (string.Equals(UserName, "admin", StringComparison.InvariantCultureIgnoreCase) &&
                string.Equals(obj.Password, "admin"))
            {
                LoginWindowVisibility = Visibility.Collapsed;
                WorkWindowVisibility = Visibility.Visible;

                obj.Password = UserName = string.Empty;

                RegionManager.RequestNavigate(Constants.ContentRegion, Constants.Welcome);
            }
            else
            {
                obj.Password = string.Empty;
                MessageBox.Show("Username and password did not match", Constants.Error, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void ExecuteLogoutCommand()
        {
            LoginWindowVisibility = Visibility.Visible;
            WorkWindowVisibility = Visibility.Collapsed;
        }

        private void ExecuteNavigateToPage(string obj)
        {
            RegionManager.RequestNavigate(Constants.ContentRegion, obj);
        }
    }
}
