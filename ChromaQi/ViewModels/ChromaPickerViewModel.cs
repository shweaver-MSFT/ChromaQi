using ChromaQi.Common;
using ChromaQi.Helpers;
using ChromaQi.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

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

        private ObservableCollection<BackgroundImageItem> _backgroundImages;
        public ObservableCollection<BackgroundImageItem> BackgroundImages
        {
            get => _backgroundImages;
            set => Set(ref _backgroundImages, value);
        }

        public ChromaPickerViewModel()
        {
            ApplyChromaKeyCommand = new RelayCommand(ApplyChromaKey);
            ResetPhotoImageSourceCommand = new RelayCommand(ResetPhotoImageSource);

            _photoImageSource = new WriteableBitmap(PhotoWidth, PhotoHeight);

            _keyColor = Colors.Green;
            _chromaRange = 0;

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender,PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(KeyColor):
                case nameof(ChromaRange):
                    ApplyChromaKey();
                    break;
            }
        }

        public override async Task LoadAsync(object data = null)
        {
            if (data != null)
            {
                var photo = data as StorageFile;

                // resize the photo and get bytes
                var imageBytes = await ImageHelpers.ReadImageBytesAsync(photo);
                imageBytes = await ImageHelpers.ResizeImageAsync(imageBytes, PhotoWidth, PhotoHeight);

                // save bytes as new photo to tempFolder
                _photo = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("workingImage", CreationCollisionOption.ReplaceExisting);
                await ImageHelpers.WriteImageBytesAsync(_photo, imageBytes);

                await ResetPhotoImageSourceAsync();
            }

            KeyColor = await ApplicationData.Current.LocalSettings.ReadAsync<Color>("keyColor");
            ChromaRange = await ApplicationData.Current.LocalSettings.ReadAsync<int>("chromaRange");

            
        }

        private bool _processing = false;

        private async void ApplyChromaKey()
        {
            if (_processing) return;

            _processing = true;
            await ResetPhotoImageSourceAsync();
            await ProcessPhotoImageSourceAsync();
            _processing = false;
        }

        private async void ResetPhotoImageSource()
        {
            await ResetPhotoImageSourceAsync();
        }

        private async Task ResetPhotoImageSourceAsync()
        {
            if (_photo == null) return;

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
            // Early exit if range is zero
            if (_chromaRange == 0) return;

            //var pixels = new byte[_photoImageSource.PixelBuffer.Capacity];
            //_photoImageSource.PixelBuffer.CopyTo(pixels);

            if (_photo != null)
            {
                using (IRandomAccessStream fileStream = await _photo.OpenAsync(FileAccessMode.Read))
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);
                    //BitmapTransform transform = new BitmapTransform()
                    //{
                    //    ScaledWidth = Convert.ToUInt32(_photoImageSource.PixelWidth),
                    //    ScaledHeight = Convert.ToUInt32(_photoImageSource.PixelHeight)
                    //};

                    PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
                        BitmapPixelFormat.Bgra8,
                        BitmapAlphaMode.Straight, 
                        new BitmapTransform(),
                        ExifOrientationMode.IgnoreExifOrientation,
                        ColorManagementMode.DoNotColorManage);

                    byte[] pixels = pixelData.DetachPixelData();
                    

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
                    //using (Stream stream = _photoImageSource.PixelBuffer.AsStream())
                    //{
                    //    await stream.WriteAsync(pixels, 0, (int)pixels.AsBuffer().Capacity);
                    //}

                    using (Stream stream = _photoImageSource.PixelBuffer.AsStream())
                    {
                        await stream.WriteAsync(pixels, 0, pixels.Length);
                    }

                    // Redraw the WriteableBitmap 
                    _photoImageSource.Invalidate();
                }
            }
        }

        public override async Task SaveAsync()
        {
           await ApplicationData.Current.LocalSettings.SaveAsync("keyColor", _keyColor);
           await ApplicationData.Current.LocalSettings.SaveAsync("chromaRange", _chromaRange);
        }
    }
}
