using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NTag.Views;
using NTag.ViewModels;
using System.Reflection;
using System.Diagnostics;

namespace NTagApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindowViewModel = new MainWindowViewModel();
            var mainWindow = new MainWindowView(mainWindowViewModel);            
            mainWindow.Title = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).ProductName;
            Application.Current.MainWindow = mainWindow;
            
            base.OnStartup(e);
            Application.Current.MainWindow.Show();
        }
    }
}
