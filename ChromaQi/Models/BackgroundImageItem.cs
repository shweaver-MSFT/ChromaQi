using System;

namespace ChromaQi.Models
{
    public struct BackgroundImageItem
    {
        public Uri ImageUri { get; set; }
        public string Title { get; set; }

        public BackgroundImageItem(string title, Uri imageUri)
        {
            Title = title;
            ImageUri = imageUri;
        }
    }
}
