using ChromaQi.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ChromaQi.Views
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class ChromaPickerView : BaseView
    {
        private bool _eyeDropperMode = false;
        private CoreCursor _defaultCursor = null;

        public ChromaPickerView()
        {
            InitializeComponent();

            ChromaPickerPivot.SelectionChanged += ChromaPickerPivot_SelectionChanged;
        }

        private void ChromaPickerPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Contains(BackgroundPivotItem))
            {
                var vm = ViewModel as ChromaPickerViewModel;
                BackgroundImageListView.ItemsSource = vm.BackgroundImages;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void EyeDropButton_Click(object sender, RoutedEventArgs e)
        {
            _eyeDropperMode = !_eyeDropperMode;
            if (_eyeDropperMode)
            {
                _defaultCursor = CoreWindow.GetForCurrentThread().PointerCursor;
                CoreWindow.GetForCurrentThread().PointerCursor = new CoreCursor(CoreCursorType.Custom, 101);
            }
            else
            {
                CoreWindow.GetForCurrentThread().PointerCursor = _defaultCursor;
            }
        }

        private bool GetShareContent(DataRequest request)
        {
            bool succeeded = false;

            //if (this.imageFile != null)
            //{
            //    DataPackage requestData = request.Data;
            //    requestData.Properties.Title = TitleInputBox.Text;
            //    requestData.Properties.Description = DescriptionInputBox.Text; // The description is optional.
            //    requestData.Properties.ContentSourceApplicationLink = ApplicationLink;

            //    // It's recommended to use both SetBitmap and SetStorageItems for sharing a single image
            //    // since the target app may only support one or the other.

            //    List<IStorageItem> imageItems = new List<IStorageItem>();
            //    imageItems.Add(this.imageFile);
            //    requestData.SetStorageItems(imageItems);

            //    RandomAccessStreamReference imageStreamRef = RandomAccessStreamReference.CreateFromFile(this.imageFile);
            //    requestData.Properties.Thumbnail = imageStreamRef;
            //    requestData.SetBitmap(imageStreamRef);
            //    succeeded = true;
            //}
            //else
            //{
            //    request.FailWithDisplayText("Select an image you would like to share and try again.");
            //}
            return succeeded;
        }
    }
}
