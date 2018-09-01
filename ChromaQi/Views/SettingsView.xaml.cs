using Windows.UI.Xaml;

namespace ChromaQi.Views
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class SettingsView : BaseView
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
