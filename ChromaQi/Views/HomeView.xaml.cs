using ChromaQi.Helpers;
using System;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ChromaQi.Views
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class HomeView : BaseView
    {
        public HomeView()
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

        private void VideoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
