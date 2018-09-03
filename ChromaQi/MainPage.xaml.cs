using System;
using ChromaQi.Helpers;
using ChromaQi.Views;
using Windows.Foundation;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ChromaQi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void CameraButton_Click(object sender, RoutedEventArgs e)
        {
            StorageFile photo;

            bool isInTestMode = await ApplicationData.Current.LocalSettings.ReadAsync<bool>("isInTestMode");
            if (isInTestMode)
            {
                photo = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/TestImage.jpg"));
            }
            else
            {
                CameraCaptureUI captureUI = new CameraCaptureUI();
                captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
                captureUI.PhotoSettings.AllowCropping = false;
                //captureUI.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.SmallVga;
                //captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 300);

                photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

                if (photo == null)
                {
                    // User cancelled photo capture
                    return;
                }
            }

            Frame.Navigate(typeof(ChromaPickerView), photo);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsView));
        }
    }
}
