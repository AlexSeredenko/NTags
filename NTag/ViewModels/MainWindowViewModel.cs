using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
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
        private MainWindowModel _mainWindowModel;

        private ICommand _openFolder;
        private ICommand _exit;
        private ICommand _startStop;

        public ObservableCollection<TrackModel> TrackModels => _mainWindowModel?.TrackModels;

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
            _mainWindowModel = new MainWindowModel();

        }

        private void OpenFolderExecute()
        {
            var openDialog = new Gat.Controls.OpenDialogView();
            var dialogViewModel = (Gat.Controls.OpenDialogViewModel)openDialog.DataContext;
            dialogViewModel.DateFormat = Gat.Controls.OpenDialogViewModel.ISO8601_DateFormat;
            dialogViewModel.IsDirectoryChooser = true;

            var result = dialogViewModel.Show();
            if (result == true && dialogViewModel.SelectedFolder != null)
            {
                CurrentFolderName = dialogViewModel.SelectedFolder.Path;
                _mainWindowModel.OpenFolder(CurrentFolderName);
            }
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
        }

        private bool StartStopCanExecute()
        {
            return true;
        }
    }
}
