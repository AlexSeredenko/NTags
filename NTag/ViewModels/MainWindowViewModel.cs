using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Prism.Mvvm;
using Prism.Commands;
using UnidecodeSharpFork;
using TagLib;
using TagLib.Id3v2;
using NTag.Models;

namespace NTag.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _currentFolderName;
        private string _startStopText;

        private ICommand _openFolder;
        private ICommand _exit;
        private ICommand _startStop;

        public ObservableCollection<TrackModel> TrackModels { get; set; }

        private BitmapFrame _songImage;
        public BitmapFrame SongImage
        {
            get { return _songImage; }
            set { SetProperty(ref _songImage, value); }
        }


        public string CurrentFolderName
        {
            get { return _currentFolderName; }
            set { SetProperty(ref _currentFolderName, value); }
        }

        public string StartStopText
        {
            get { return _startStopText; }
            set { SetProperty(ref _startStopText, value); }
        }

        public ICommand OpenFolder => _openFolder ?? (_openFolder = new DelegateCommand(OpenFolderExecute, OpenFolderCanExecute));
        public ICommand Exit => _exit ?? (_exit = new DelegateCommand(ExitExecute, ExitCanExecute));
        public ICommand StartStop => _startStop ?? (_startStop = new DelegateCommand(StartStopExecute, StartStopCanExecute));


        public MainWindowViewModel()
        {
            Init();
        }

        private void Init()
        {
            _startStopText = "Start";

            TrackModels = new ObservableCollection<TrackModel>();

            for (int i = 0; i < 10; i++)
            {
                TrackModels.Add(new TrackModel()
                {
                    FileDir = @"C:\",
                    OriginalFileName = "Original file name" + $" #{i}",
                    OriginalAlbum = "Original album" + $" #{i}",
                    OriginalPerformer = "Original performer" + $" #{i}",
                    OriginalTitle = "Original title" + $" #{i}",
                    ModifiedFileName = "Modified file name" + $" #{i}",
                    ModifiedAlbum = "Modified album" + $" #{i}",
                    ModifiedPerformer = "Modified performer" + $" #{i}",
                    ModifiedTitle = "Modified title" + $" #{i}"
                });
            }
        }

        private void OpenFolderExecute()
        {
            CurrentFolderName = @"C:\Data";
        }

        private bool OpenFolderCanExecute()
        {
            return true;
        }

        private void ExitExecute()
        {
            Application.Current?.Shutdown();
        }

        private bool ExitCanExecute()
        {
            return true;
        }

        private void StartStopExecute()
        {
            StartStopText = "Stop";

            //var tfile = File.Create(@"C:\Users\oleksandr.seredenko\Downloads\billie-eilish-bellyache(mp3name.net).mp3");
            //tfile.RemoveTags(TagTypes.AllTags);
            //tfile.Save();

            var tfile = File.Create(@"C:\Users\oleksandr.seredenko\Downloads\billie-eilish-bellyache(mp3name.net).mp3");
            tfile.Tag.Album = "Downloads".Unidecode();
            tfile.Tag.Performers = new string[] { "billie eilish" };
            tfile.Tag.Title = "bellyache";
            var img = tfile.Tag.Pictures.FirstOrDefault();

            var imgData = img.Data.ToArray();
            var ms = new System.IO.MemoryStream(imgData);

            var imgExt = img.MimeType.Split("/").LastOrDefault();

            if (!string.IsNullOrEmpty(imgExt))
            {
                using (var fs = new System.IO.FileStream(@"D:\123." + imgExt, System.IO.FileMode.CreateNew))
                {
                    fs.Write(imgData, 0, imgData.Length);
                }
            }

            var t = img.GetType();
            AttachmentFrame attachmentFrame;

            var pic = new Picture(@"D:\tagimg.jpg");
            pic.Type = PictureType.BackCover;

            tfile.Tag.Pictures = new IPicture[] { pic };

            System.Diagnostics.Trace.WriteLine("");
            //tfile.Save();
            ms.Position = 0;
            var bitmapImage = BitmapFrame.Create(ms);
            var t2 = bitmapImage.GetType();

            System.Diagnostics.Trace.WriteLine("");

            SongImage = bitmapImage;
        }

        private bool StartStopCanExecute()
        {
            return true;
        }
    }
}
