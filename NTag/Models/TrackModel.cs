using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Prism.Mvvm;
using Prism.Commands;
using UnidecodeSharpFork;
using TagLib;

namespace NTag.Models
{
    public class TrackModel : BindableBase
    {
        private string _fileDir;        

        private string _originalFileName;
        private string _originalAlbum;
        private string _originalPerformer;
        private string _originalTitle;
        private IPicture _originalImage;
        private BitmapFrame _originalImageVisible;

        private string _modifiedFileName;
        private string _modifiedAlbum;
        private string _modifiedPerformer;
        private string _modifiedTitle;
        private IPicture _modifiedImage;
        private BitmapFrame _modifiedImageVisible;

        public string FileDir
        {
            get { return _fileDir; }
            set { _fileDir = value; }
        }

        public string OriginalFileName
        {
            get { return _originalFileName; }
            set { SetProperty(ref _originalFileName, value); }
        }

        public string OriginalAlbum
        {
            get { return _originalAlbum; }
            set { SetProperty(ref _originalAlbum, value); }
        }

        public string OriginalPerformer
        {
            get { return _originalPerformer; }
            set { SetProperty(ref _originalPerformer, value); }
        }

        public string OriginalTitle
        {
            get { return _originalTitle; }
            set { SetProperty(ref _originalTitle, value); }
        }

        public IPicture OriginalImage
        {
            get { return _originalImage; }
            set { SetOriginalImage(value); }
        }

        public BitmapFrame OriginalImageVisible
        {
            get { return _originalImageVisible; }
            set { SetProperty(ref _originalImageVisible, value); }
        }

        public string ModifiedFileName
        {
            get { return _modifiedFileName; }
            set { SetProperty(ref _modifiedFileName, value); }
        }

        public string ModifiedAlbum
        {
            get { return _modifiedAlbum; }
            set { SetProperty(ref _modifiedAlbum, value); }
        }

        public string ModifiedPerformer
        {
            get { return _modifiedPerformer; }
            set { SetProperty(ref _modifiedPerformer, value); }
        }

        public string ModifiedTitle
        {
            get { return _modifiedTitle; }
            set { SetProperty(ref _modifiedTitle, value); }
        }

        public IPicture ModifiedImage
        {
            get { return _modifiedImage; }
            set { SetModifiedImage(value); }
        }

        public BitmapFrame ModifiedImageVisible
        {
            get { return _modifiedImageVisible; }
            set { SetProperty(ref _modifiedImageVisible, value); }
        }

        private BitmapFrame GetBitmapFrameFromPicture(IPicture picture)
        {
            if (picture == null || picture.Type == PictureType.NotAPicture)
            {
                return null;
            }

            var pictureMemoryStream = new System.IO.MemoryStream(picture.Data.ToArray());
            pictureMemoryStream.Position = 0;
            var bitmapFrame = BitmapFrame.Create(pictureMemoryStream);
            return bitmapFrame;
        }

        private void SetOriginalImage(IPicture picture)
        {
            _originalImage = picture;
            OriginalImageVisible = GetBitmapFrameFromPicture(picture);
        }

        private void SetModifiedImage(IPicture picture)
        {
            _modifiedImage = picture;
            ModifiedImageVisible = GetBitmapFrameFromPicture(picture);
        }
    }
}
