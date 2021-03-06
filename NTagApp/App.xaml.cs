﻿using System;
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
using NTagApp.Models;

namespace NTagApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindowViewModel = default(MainWindowViewModel);

            if (e.Args != null && e.Args.Length > 0)
            {
                mainWindowViewModel = new MainWindowViewModel(new ConfigurationModel(), e.Args[0]);
            }
            else
            {
                mainWindowViewModel = new MainWindowViewModel(new ConfigurationModel());
            }

            var mainWindow = new MainWindowView(mainWindowViewModel);
            mainWindow.Title = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).ProductName;
            Application.Current.MainWindow = mainWindow;

            base.OnStartup(e);
            Application.Current.MainWindow.Show();
        }
    }
}
