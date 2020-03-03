using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows;
using Prism.Mvvm;
using Prism.Commands;
using UnidecodeSharpFork;

namespace NTag.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _currentFolderName;
        private string _startStopText;

        private ICommand _openFolder;
        private ICommand _exit;
        private ICommand _startStop;

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
        }

        private bool StartStopCanExecute()
        {
            return true;
        }
    }
}
