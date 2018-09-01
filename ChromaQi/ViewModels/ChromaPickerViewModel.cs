using ChromaQi.Helpers;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using ChromaQi.Common;

namespace ChromaQi.ViewModels
{
    public class ChromaPickerViewModel : BaseViewModel
    {
        private const int DEFAULT_PHOTO_HEIGHT = 300;
        private const int DEFAULT_PHOTO_WIDTH = 300;

        private StorageFile _photo;

        public readonly int PhotoHeight = DEFAULT_PHOTO_HEIGHT;
        public readonly int PhotoWidth = DEFAULT_PHOTO_WIDTH;

        public RelayCommand ApplyChromaKeyCommand { get; }
        public RelayCommand ResetPhotoImageSourceCommand { get; }

        private WriteableBitmap _photoImageSource;
        public WriteableBitmap PhotoImageSource
        {
            get => _photoImageSource;
            set => Set(ref _photoImageSource, value);
        }

        private Color _keyColor;
        public Color KeyColor
        {
            get => _keyColor;
            set => Set(ref _keyColor, value);
        }

        private int _chromaRange;
        public int ChromaRange
        {
            get => _chromaRange;
            set => Set(ref _chromaRange, value);
        }

        public ChromaPickerViewModel()
        {
            ApplyChromaKeyCommand = new RelayCommand(ApplyChromaKey);
            ResetPhotoImageSourceCommand = new RelayCommand(ResetPhotoImageSource);

            _photoImageSource = new WriteableBitmap(PhotoWidth, PhotoHeight);

            _keyColor = Colors.Green;
            _chromaRange = 0;
        }

        public override async Task LoadAsync(object data = null)
        {
            KeyColor = await ApplicationData.Current.LocalSettings.ReadAsync<Color>("keyColor");
            ChromaRange = await ApplicationData.Current.LocalSettings.ReadAsync<int>("chromaRange");

            if (data != null)
            {
                _photo = data as StorageFile;

                var imageBytes = await ImageHelpers.ReadImageBytesAsync(_photo);
                imageBytes = await ImageHelpers.ResizeImageAsync(imageBytes, PhotoWidth, PhotoHeight);
                await ImageHelpers.WriteImageBytesAsync(imageBytes);

                await ResetPhotoImageSourceAsync();
                await ProcessPhotoImageSourceAsync();
            }
        }

        private async void ApplyChromaKey()
        {
            await ResetPhotoImageSourceAsync();
            await ProcessPhotoImageSourceAsync();
        }

        private async void ResetPhotoImageSource()
        {
            await ResetPhotoImageSourceAsync();
        }

        private async Task ResetPhotoImageSourceAsync()
        {
            using (IRandomAccessStream fileStream = await _photo.OpenAsync(FileAccessMode.Read))
            {
                try
                {
                    await _photoImageSource.SetSourceAsync(fileStream);
                }
                catch (TaskCanceledException)
                {
                    // The async action to set the WriteableBitmap's source may be canceled if the source is changed again while the action is in progress 
                }
            }

            _photoImageSource.Invalidate();
        }

        private async Task ProcessPhotoImageSourceAsync()
        {
            var pixels = new byte[_photoImageSource.PixelBuffer.Capacity];
            _photoImageSource.PixelBuffer.CopyTo(pixels);

            var pixelUpdateCount = 0;
            for (var i = 0; i < pixels.Length - 4; i += 4)
            {
                byte r = pixels[i],
                     g = pixels[i + 1],
                     b = pixels[i + 2],
                     a = pixels[i + 3];

                // Find pixels within color range
                if (Math.Abs(r - _keyColor.R) < _chromaRange &&
                    Math.Abs(g - _keyColor.G) < _chromaRange &&
                    Math.Abs(b - _keyColor.B) < _chromaRange)
                {
                    // Remove all color from this pixel
                    pixels[i] = 0;
                    pixels[i + 1] = 0;
                    pixels[i + 2] = 0;
                    pixels[i + 3] = 0;
                    pixelUpdateCount++;
                }
            }

            System.Diagnostics.Debug.WriteLine($"Pixels Updated: {pixelUpdateCount}");

            // Open a stream to copy to the WriteableBitmap's pixel buffer 
            using (Stream stream = _photoImageSource.PixelBuffer.AsStream())
            {
                await stream.WriteAsync(pixels, 0, (int)pixels.AsBuffer().Capacity);
            }

            // Redraw the WriteableBitmap 
            _photoImageSource.Invalidate();
        }

        public override async Task SaveAsync()
        {
           await ApplicationData.Current.LocalSettings.SaveAsync("keyColor", _keyColor);
           await ApplicationData.Current.LocalSettings.SaveAsync("chromaRange", _chromaRange);
        }
    }
}
