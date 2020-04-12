using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using Prism.Mvvm;
using Prism.Commands;
using NTag.Models;
using NTag.Interfaces;

namespace NTag.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IConfiguration _configuration;
        private string _currentFolderName;
        private double _progressValue;
        private Visibility _progressVisibility;
        private bool _isInProgress;
        private MainWindowModel _mainWindowModel;

        private DelegateCommandBase _openFolder;
        private DelegateCommandBase _exit;
        private DelegateCommandBase _startStop;
        private DelegateCommandBase _setPictureFromFile;
        private DelegateCommandBase _applyPictureForAll;
        private DelegateCommandBase _applyAlbumForAll;
        private DelegateCommandBase _applyPerformerForAll;
        private DelegateCommandBase _applyTitleForAll;
        private DelegateCommandBase _titleFromFileName;
        private DelegateCommandBase _performerFromFileName;
        private DelegateCommandBase _fileNameFromTags;
        private DelegateCommandBase _fileNameFromTagsAll;

        public ObservableCollection<TrackModel> TrackModels => _mainWindowModel?.TrackModels;

        public string CurrentFolderName
        {
            get { return _currentFolderName; }
            set { SetProperty(ref _currentFolderName, value); }
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
            set 
            { 
                SetProperty(ref _isInProgress, value);
                RaiseCanExecuteChanged();
            }
        }

        public DelegateCommandBase OpenFolder => _openFolder ??
                    (_openFolder = new DelegateCommand(OpenFolderExecute, OpenFolderCanExecute));

        public DelegateCommandBase Exit => _exit ??
                    (_exit = new DelegateCommand(ExitExecute, ExitCanExecute));

        public DelegateCommandBase StartStop => _startStop ??
                    (_startStop = new DelegateCommand(StartStopExecute, StartStopCanExecute));

        public DelegateCommandBase SetPictureFromFile => _setPictureFromFile ??
                    (_setPictureFromFile = new DelegateCommand<object>(SetPictureFromFileExecute, TrackContextMenuCanExecute));

        public DelegateCommandBase ApplyPictureForAll => _applyPictureForAll ??
                    (_applyPictureForAll = new DelegateCommand<object>(ApplyPictureForAllExecute, TrackContextMenuCanExecute));

        public DelegateCommandBase ApplyAlbumForAll => _applyAlbumForAll ??
                    (_applyAlbumForAll = new DelegateCommand<object>(ApplyAlbumForAllExecute, TrackContextMenuCanExecute));

        public DelegateCommandBase ApplyPerformerForAll => _applyPerformerForAll ??
                    (_applyPerformerForAll = new DelegateCommand<object>(ApplyPerformerForAllExecute, TrackContextMenuCanExecute));

        public DelegateCommandBase ApplyTitleForAll => _applyTitleForAll ??
                    (_applyTitleForAll = new DelegateCommand<object>(ApplyTitleForAllExecute, TrackContextMenuCanExecute));

        public DelegateCommandBase TitleFromFileName => _titleFromFileName ??
                    (_titleFromFileName = new DelegateCommand<object>(TitleFromFileNameExecute, TrackContextMenuCanExecute));

        public DelegateCommandBase PerformerFromFileName => _performerFromFileName ??
                    (_performerFromFileName = new DelegateCommand<object>(PerformerFromFileNameExecute, TrackContextMenuCanExecute));

        public DelegateCommandBase FileNameFromTags => _fileNameFromTags ??
                    (_fileNameFromTags = new DelegateCommand<object>(FileNameFromTagsExecute, TrackContextMenuCanExecute));

        public DelegateCommandBase FileNameFromTagsAll => _fileNameFromTagsAll ??
                    (_fileNameFromTagsAll = new DelegateCommand(FileNameFromTagsAllExecute, FileNameFromTagsAllCanExecute));

        public MainWindowViewModel(IConfiguration configuration)
        {
            Init(configuration);
        }

        private void Init(IConfiguration configuration)
        {
            _configuration = configuration;
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

        private void RaiseCanExecuteChanged()
        {
            OpenFolder.RaiseCanExecuteChanged();
            Exit.RaiseCanExecuteChanged();
            StartStop.RaiseCanExecuteChanged();
            SetPictureFromFile.RaiseCanExecuteChanged();
            ApplyPictureForAll.RaiseCanExecuteChanged();
            ApplyAlbumForAll.RaiseCanExecuteChanged();
            ApplyPerformerForAll.RaiseCanExecuteChanged();
            ApplyTitleForAll.RaiseCanExecuteChanged();
            TitleFromFileName.RaiseCanExecuteChanged();
            PerformerFromFileName.RaiseCanExecuteChanged();
            FileNameFromTags.RaiseCanExecuteChanged();
            FileNameFromTagsAll.RaiseCanExecuteChanged();
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
            return IsInProgress == false;
        }

        private void ExitExecute()
        {
            if (IsInProgress)
            {
                _mainWindowModel.Stop();
            }

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
            }
        }

        private bool StartStopCanExecute()
        {
            return true;
        }

        private bool ConfirmApplyForAll(TrackModel trackModel, string tagName)
        {
            var msg = $"Apply {tagName} from track {trackModel.TrackNumber} for All?";
            if (MessageBox.Show(msg, "Batch Dialog", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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

        private bool TrackContextMenuCanExecute(object sender)
        {
            return IsInProgress == false;
        }

        private void ApplyPictureForAllExecute(object sender)
        {
            if (sender is TrackModel trackModel && ConfirmApplyForAll(trackModel, "Picture"))
            {
                _mainWindowModel.DuplicatePictureAll(trackModel);
            }
        }

        private void ApplyAlbumForAllExecute(object sender)
        {
            if (sender is TrackModel trackModel && ConfirmApplyForAll(trackModel, "Album"))
            {
                _mainWindowModel.DuplicateAlbumAll(trackModel);
            }
        }

        private void ApplyPerformerForAllExecute(object sender)
        {
            if (sender is TrackModel trackModel && ConfirmApplyForAll(trackModel, "Performer"))
            {
                _mainWindowModel.DuplicatePerformerAll(trackModel);
            }
        }

        private void ApplyTitleForAllExecute(object sender)
        {
            if (sender is TrackModel trackModel && ConfirmApplyForAll(trackModel, "Title"))
            {
                _mainWindowModel.DuplicateTitleAll(trackModel);
            }
        }

        private void TitleFromFileNameExecute(object sender)
        {
            if (sender is TrackModel trackModel)
            {
                _mainWindowModel.TitleFromFileName(trackModel);
            }
        }

        private void PerformerFromFileNameExecute(object sender)
        {
            if (sender is TrackModel trackModel)
            {
                _mainWindowModel.PerformerFromFileName(trackModel);
            }
        }

        private void FileNameFromTagsExecute(object sender)
        {
            if (sender is TrackModel trackModel)
            {
                _mainWindowModel.FileNameFromTags(trackModel);
            }
        }

        private void FileNameFromTagsAllExecute()
        {
            var msg = "Apply file names to file names from tags for All?";
            if (MessageBox.Show(msg, "Batch Dialog", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            _mainWindowModel.FileNameFromTagsAll();
        }

        private bool FileNameFromTagsAllCanExecute()
        {
            return IsInProgress == false;
        }

        private void OnProcessingStarted()
        {
            UIBeginInvoke(() =>
            {
                IsInProgress = true;
                ProgressValue = 0;
                ProgressVisibility = Visibility.Visible;
            });
        }

        private void OnProcessingFinished()
        {
            UIBeginInvoke(() =>
            {
                MessageBox.Show("Done!");
                IsInProgress = false;
                ProgressVisibility = Visibility.Hidden;
            });
        }

        private void OnProgressChanged(int totalCount, int doneCount)
        {
            var progressValue = Math.Truncate((double)doneCount / totalCount * 100D);
            UIBeginInvoke(() =>
            {
                ProgressValue = progressValue;
            });
        }
    }
}
