using System;
using ChromaQi.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ChromaQi.Views
{
    public abstract class BaseView : Page
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(IViewModel),
            typeof(BaseView),
            new PropertyMetadata(null, OnViewModelPropertyChanged));

        public IViewModel ViewModel
        {
            get { return (IViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        private async static void OnViewModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var baseView = d as BaseView;
            await baseView.Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => baseView.DataContext = e.NewValue);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => ViewModel?.LoadAsync(e.Parameter));
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            await ViewModel.SaveAsync();
            base.OnNavigatingFrom(e);
        }
    }
}
