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

        private static void OnViewModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var baseView = d as BaseView;
            baseView.DataContext = e.NewValue;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel?.LoadAsync(e.Parameter);
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            await ViewModel.SaveAsync();
            base.OnNavigatingFrom(e);
        }
    }
}
