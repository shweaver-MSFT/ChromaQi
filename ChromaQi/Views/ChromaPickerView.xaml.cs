﻿using ChromaQi.ViewModels;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void EyeDropButton_Click(object sender, RoutedEventArgs e)
        {

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
