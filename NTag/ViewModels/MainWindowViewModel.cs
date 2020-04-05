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
using NTag.Interfaces;
using System.Windows.Threading;
using System.Threading;

namespace NTag.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IConfiguration _configuration;
        private string _currentFolderName;
        private string _startStopText;
        private double _progressValue;
        private Visibility _progressVisibility;
        private bool _isInProgress;
        private MainWindowModel _mainWindowModel;

        private ICommand _openFolder;
        private ICommand _exit;
        private ICommand _startStop;
        private ICommand _setPictureFromFile;
        private ICommand _applyPictureForAll;
        private ICommand _applyAlbumForAll;
        private ICommand _applyPerformerForAll;
        private ICommand _applyTitleForAll;
        private ICommand _titleFromFileName;
        private ICommand _performerFromFileName;
        private ICommand _fileNameFromTags;
        private ICommand _fileNameFromTagsAll;

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

        public double ProgressValue
        {
            get { return _progressValue; }
            set { SetProperty(ref _progressValue, value); }
        }

        public Visibility ProgressVisibility
        {
            get { return _progressVisibility; }
            set { SetProperty(ref _progressVisibility, value); }
        }

        public bool IsInProgress
        {
            get { return _isInProgress; }
            set { SetProperty(ref _isInProgress, value); }
        }

        public ICommand OpenFolder => _openFolder ??
                    (_openFolder = new DelegateCommand(OpenFolderExecute, OpenFolderCanExecute));

        public ICommand Exit => _exit ??
                    (_exit = new DelegateCommand(ExitExecute, ExitCanExecute));

        public ICommand StartStop => _startStop ??
                    (_startStop = new DelegateCommand(StartStopExecute, StartStopCanExecute));

        public ICommand SetPictureFromFile => _setPictureFromFile ??
                    (_setPictureFromFile = new DelegateCommand<object>(SetPictureFromFileExecute, SetPictureFromFileCanExecute));

        public ICommand ApplyPictureForAll => _applyPictureForAll ??
                    (_applyPictureForAll = new DelegateCommand<object>(ApplyPictureForAllExecute, ApplyPictureForAllCanExecute));

        public ICommand ApplyAlbumForAll => _applyAlbumForAll ??
                    (_applyAlbumForAll = new DelegateCommand<object>(ApplyAlbumForAllExecute, ApplyAlbumForAllCanExecute));

        public ICommand ApplyPerformerForAll => _applyPerformerForAll ??
                    (_applyPerformerForAll = new DelegateCommand<object>(ApplyPerformerForAllExecute, ApplyPerformerForAllCanExecute));

        public ICommand ApplyTitleForAll => _applyTitleForAll ??
                    (_applyTitleForAll = new DelegateCommand<object>(ApplyTitleForAllExecute, ApplyTitleForAllCanExecute));

        public ICommand TitleFromFileName => _titleFromFileName ??
                    (_titleFromFileName = new DelegateCommand<object>(TitleFromFileNameExecute, TitleFromFileNameCanExecute));

        public ICommand PerformerFromFileName => _performerFromFileName ??
                    (_performerFromFileName = new DelegateCommand<object>(PerformerFromFileNameExecute, PerformerFromFileNameCanExecute));

        public ICommand FileNameFromTags => _fileNameFromTags ??
                    (_fileNameFromTags = new DelegateCommand<object>(FileNameFromTagsExecute, FileNameFromTagsCanExecute));

        public ICommand FileNameFromTagsAll => _fileNameFromTagsAll ??
                    (_fileNameFromTagsAll = new DelegateCommand(FileNameFromTagsAllExecute, FileNameFromTagsAllCanExecute));

        public MainWindowViewModel(IConfiguration configuration)
        {
            Init(configuration);
        }

        private void Init(IConfiguration configuration)
        {
            _configuration = configuration;
            _startStopText = "Start";
            _progressValue = 0;
            _progressVisibility = Visibility.Hidden;
            _mainWindowModel = new MainWindowModel(configuration)
            {
                ProgressChanged = OnProgressChanged,
                ProcessingStarted = OnProcessingStarted,
                ProcessingFinished = OnProcessingFinished
            };

            //for debug
            CurrentFolderName = @"C:\Data\muz\Bi-2";
            _mainWindowModel.OpenFolder(CurrentFolderName);
            //--
        }

        private void UIBeginInvoke(Action callback)
        {
            Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.Normal, callback);
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

        private async void StartStopExecute()
        {
            if (IsInProgress)
            {
                _mainWindowModel.Stop();
            }
            else
            {
                await _mainWindowModel.StartAsync();
                IsInProgress = false;
                MessageBox.Show("Done!");
            }
        }

        private bool StartStopCanExecute()
        {
            return true;
        }

        private bool ConfirmApplyForAll(TrackModel trackModel, string tagName)
        {
            var msg = $"Apply {tagName} from track {trackModel.TrackNumber} for All?";
            if (MessageBox.Show(msg, "Batch Dialog", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                return true;
            }

            return false;
        }

        private void SetPictureFromFileExecute(object sender)
        {
            if (sender is TrackModel trackModel)
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Images |" + string.Join($";", _configuration.SupportedImageFormats.Select(x => $"*{x}"));
                if (openFileDialog.ShowDialog() == true)
                {
                    _mainWindowModel.SetPictureFromFile(trackModel, openFileDialog.FileName);
                }
            }
        }

        private bool SetPictureFromFileCanExecute(object sender)
        {
            return true;
        }

        private void ApplyPictureForAllExecute(object sender)
        {
            if (sender is TrackModel trackModel && ConfirmApplyForAll(trackModel, "Picture"))
            {
                _mainWindowModel.DuplicatePictureAll(trackModel);
            }
        }

        private bool ApplyPictureForAllCanExecute(object sender)
        {
            return true;
        }

        private void ApplyAlbumForAllExecute(object sender)
        {
            if (sender is TrackModel trackModel && ConfirmApplyForAll(trackModel, "Album"))
            {
                _mainWindowModel.DuplicateAlbumAll(trackModel);
            }
        }

        private bool ApplyAlbumForAllCanExecute(object sender)
        {
            return true;
        }

        private void ApplyPerformerForAllExecute(object sender)
        {
            if (sender is TrackModel trackModel && ConfirmApplyForAll(trackModel, "Performer"))
            {
                _mainWindowModel.DuplicatePerformerAll(trackModel);
            }
        }

        private bool ApplyPerformerForAllCanExecute(object sender)
        {
            return true;
        }

        private void ApplyTitleForAllExecute(object sender)
        {
            if (sender is TrackModel trackModel && ConfirmApplyForAll(trackModel, "Title"))
            {
                _mainWindowModel.DuplicateTitleAll(trackModel);
            }
        }

        private bool ApplyTitleForAllCanExecute(object sender)
        {
            return true;
        }


        private void TitleFromFileNameExecute(object sender)
        {
            if (sender is TrackModel trackModel)
            {
                _mainWindowModel.TitleFromFileName(trackModel);
            }
        }

        private bool TitleFromFileNameCanExecute(object sender)
        {
            return true;
        }

        private void PerformerFromFileNameExecute(object sender)
        {
            if (sender is TrackModel trackModel)
            {
                _mainWindowModel.PerformerFromFileName(trackModel);
            }
        }

        private bool PerformerFromFileNameCanExecute(object sender)
        {
            return true;
        }

        private void FileNameFromTagsExecute(object sender)
        {
            if (sender is TrackModel trackModel)
            {
                _mainWindowModel.FileNameFromTags(trackModel);
            }
        }

        private bool FileNameFromTagsCanExecute(object sender)
        {
            return true;
        }

        private void FileNameFromTagsAllExecute()
        {
            var msg = "Apply file names to file names from tags for All?";
            if (MessageBox.Show(msg, "Batch Dialog", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            _mainWindowModel.FileNameFromTagsAll();
        }

        private bool FileNameFromTagsAllCanExecute()
        {
            return true;
        }

        private void OnProcessingStarted()
        {
            ProgressVisibility = Visibility.Visible;
        }

        private void OnProcessingFinished()
        {
        }

        private void OnProgressChanged(int totalCount, int doneCount)
        {
            var progressValue = Math.Truncate((double)doneCount / totalCount * 100D);
            UIBeginInvoke(() =>
            {
                IsInProgress = true;
                ProgressValue = progressValue;
            });
        }
    }
}
