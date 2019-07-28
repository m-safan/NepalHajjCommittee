using System;
using System.IO;
using NepalHajjCommittee.Database;
using NepalHajjCommittee.Views;
using Prism.Ioc;
using System.Windows;
using NepalHajjCommittee.ViewModels;

namespace NepalHajjCommittee
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            if (!Directory.Exists(folder + @"\" + Constants.MainFolder))
                Directory.CreateDirectory(folder + @"\" + Constants.MainFolder);
            if (!Directory.Exists(folder + @"\" + Constants.MainFolder + @"\" + Constants.ImageFolder))
                Directory.CreateDirectory(folder + @"\" + Constants.MainFolder + @"\" + Constants.ImageFolder);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<INepalHajjCommitteeRepository, NepalHajjCommitteeRepository>();

            containerRegistry.RegisterForNavigation<WelcomePage, WelcomePageViewModel>(Constants.Welcome);
            containerRegistry.RegisterForNavigation<ManageGroup, ManageGroupViewModel>(Constants.GroupManagement);
            containerRegistry.RegisterForNavigation<GroupPage, GroupPageViewModel>(Constants.GroupPage);
            containerRegistry.RegisterForNavigation<BatchPage, BatchPageViewModel>(Constants.BatchPage);
            containerRegistry.RegisterForNavigation<PersonPage, PersonPageViewModel>(Constants.PersonPage);
            containerRegistry.RegisterForNavigation<ReportPage, ReportPageViewModel>(Constants.ReportPage);
            containerRegistry.RegisterForNavigation<ReportExportPage, ReportExportPageViewModel>(Constants.ReportExportPage);
            containerRegistry.RegisterForNavigation<RoomPage, RoomPageViewModel>(Constants.RoomPage);
            containerRegistry.RegisterForNavigation<CheckinPage, CheckinPageViewModel>(Constants.CheckInGroup);
        }
    }
}
