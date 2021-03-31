using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Hi5WpfEntry.Constants;
using Hi5WpfEntry.Core.Contracts.Services;
using Hi5WpfEntry.Core.Models;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace Hi5WpfEntry.ViewModels
{
    public class ContentGridViewModel : BindableBase, INavigationAware
    {
        private readonly ISampleDataService _sampleDataService;
        private readonly IRegionManager _regionManager;
        private IRegionNavigationService _navigationService;

        private ICommand _navigateToDetailCommand;

        public ICommand NavigateToDetailCommand => _navigateToDetailCommand ?? (_navigateToDetailCommand = new DelegateCommand<SampleOrder>(NavigateToDetail));

        public ObservableCollection<SampleOrder> Source { get; } = new ObservableCollection<SampleOrder>();

        public ContentGridViewModel(ISampleDataService sampleDataService, IRegionManager regionManager)
        {
            _sampleDataService = sampleDataService;
            _regionManager = regionManager;
            if (regionManager.Regions.ContainsRegionWithName(Regions.Main))
            {
                _navigationService = regionManager.Regions[Regions.Main].NavigationService;
            }
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            Source.Clear();

            // TODO WTS: Replace this with your actual data
            var data = await _sampleDataService.GetContentGridDataAsync();
            foreach (var item in data)
            {
                Source.Add(item);
            }
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private void NavigateToDetail(SampleOrder order)
        {
            _navigationService.RequestNavigate(PageKeys.ContentGridDetail, new NavigationParameters() { { "DetailID", order.OrderID } });
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
            => true;
    }
}
