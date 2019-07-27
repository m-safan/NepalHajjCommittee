using Prism.Mvvm;
using Prism.Regions;

namespace NepalHajjCommittee.ViewModels
{
    public abstract class ViewModelBase : BindableBase, INavigationAware
    {
        private string _title = "Nepal Hajj Committee";

        protected ViewModelBase(IRegionManager regionManager)
        {
            RegionManager = regionManager;
        }

        public string Title { get => _title; set => SetProperty(ref _title, value); }

        protected IRegionManager RegionManager { get; }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}
