using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ChromaQi.Helpers
{
    public static class ImageHelpers
    {
        /// <summary>
        /// https://stackoverflow.com/questions/35111635/how-can-i-convert-a-image-into-a-byte-array-in-uwp-platform
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static async Task<byte[]> ReadImageBytesAsync(StorageFile image)
        {
            using (var inputStream = await image.OpenSequentialReadAsync())
            {
                var readStream = inputStream.AsStreamForRead();
                var imageBytes = new byte[readStream.Length];
                await readStream.ReadAsync(imageBytes, 0, imageBytes.Length);
                return imageBytes;
            }
        }

        public static async Task WriteImageBytesAsync(byte[] imageBytes)
        {
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(imageBytes);
                    await writer.StoreAsync();
                }
            }
        }

        /// <summary>
        /// https://stackoverflow.com/questions/36009021/uwp-how-to-resize-an-image
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="reqWidth"></param>
        /// <param name="reqHeight"></param>
        /// <returns></returns>
        public static async Task<byte[]> ResizeImageAsync(byte[] imageData, int reqWidth, int reqHeight)
        {
            var memStream = new MemoryStream(imageData);

            IRandomAccessStream imageStream = memStream.AsRandomAccessStream();
            var decoder = await BitmapDecoder.CreateAsync(imageStream);
            if (decoder.PixelHeight > reqHeight || decoder.PixelWidth > reqWidth)
            {
                using (imageStream)
                {
                    var resizedStream = new InMemoryRandomAccessStream();

                    BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);
                    double widthRatio = (double)reqWidth / decoder.PixelWidth;
                    double heightRatio = (double)reqHeight / decoder.PixelHeight;

                    double scaleRatio = Math.Min(widthRatio, heightRatio);

                    if (reqWidth == 0)
                        scaleRatio = heightRatio;

                    if (reqHeight == 0)
                        scaleRatio = widthRatio;

                    uint aspectHeight = (uint)Math.Floor(decoder.PixelHeight * scaleRatio);
                    uint aspectWidth = (uint)Math.Floor(decoder.PixelWidth * scaleRatio);

                    encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;

                    encoder.BitmapTransform.ScaledHeight = aspectHeight;
                    encoder.BitmapTransform.ScaledWidth = aspectWidth;

                    await encoder.FlushAsync();
                    resizedStream.Seek(0);
                    var outBuffer = new byte[resizedStream.Size];
                    //uint x = await resizedStream.WriteAsync(outBuffer.AsBuffer());
                    await resizedStream.ReadAsync(outBuffer.AsBuffer(), (uint)resizedStream.Size, InputStreamOptions.None);
                    return outBuffer;
                }
            }
            return imageData;
        }
    }
}
