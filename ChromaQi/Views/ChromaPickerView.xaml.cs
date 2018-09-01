using ChromaQi.ViewModels;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace ChromaQi.Views
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class ChromaPickerView : BaseView
    {
        public ChromaPickerView()
        {
            InitializeComponent();
        }

        private void ChromaRangeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var vm = (ViewModel as ChromaPickerViewModel);
                vm.ChromaRange = Convert.ToInt32(ChromaRangeTextBox.Text);
            }
            catch { }
        }

        private void CloseFlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            KeyColorPickerButton.Background = new SolidColorBrush(KeyColorPicker.Color);
            KeyColorPickerButton.Flyout.Hide();
        }
    }
}
