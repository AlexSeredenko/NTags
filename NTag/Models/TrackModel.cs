using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows;
using Prism.Mvvm;
using Prism.Commands;
using UnidecodeSharpFork;
using TagLib;

namespace NTag.Models
{
    public class TrackModel : BindableBase
    {
        private string _filePath;

        private string _originalFileName;
        private string _originalAlbum;
        private string _originalPerformer;
        private string _originalTitle;
        private IPicture _originalImage;

        private string _modifiedFileName;
        private string _modifiedAlbum;
        private string _modifiedPerformer;
        private string _modifiedTitle;
        private IPicture _modifiedImage;


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
            set { SetProperty(ref _originalImage, value); }
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
            set { SetProperty(ref _modifiedImage, value); }
        }
    }
}
