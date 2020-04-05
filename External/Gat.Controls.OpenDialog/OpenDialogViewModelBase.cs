// -----------------------------------------------------------------------
// <copyright file="OpenDialogViewModelBase.cs" company="">
//
// The MIT License (MIT)
// 
// Copyright (c) 2012 Christoph Gattnar
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of
// the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
// BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// </copyright>
// -----------------------------------------------------------------------

namespace Gat.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using System.IO;
	using System.Windows.Input;
	using Gat.Controls.Framework;
	using Gat.Controls.Model;
	using Gat.Controls.Utilities;

	public class OpenDialogViewModelBase : ViewModelBase
	{
		#region Fields

		private ObservableCollection<OpenFolderRoot> _Items;
		private ICollection<FileItem> _Folder;
		private OpenFolderItem _SelectedFolder;
		private FileItem _SelectedFile;
		private ObservableCollection<string> _FileFilterExtensions;
		private string _SelectedFilePath;
		private string _SelectedFileFilterExtension;
		private string _OpenText;
		private string _SaveText;
		private string _CancelText;
		private string _FileNameText;
		private string _FileFilterText;
		private string _TypeText;
		private string _SizeText;
		private string _DateText;
		private string _NameText;
		private bool _OpenVisibility;
		private bool _SaveVisibility;
		private bool _IsSaveDialog;

		private const string _DefaultSave = "Save";
		private const string _DefaultOpen = "Open";
		private const string _DefaultCancel = "Cancel";
		private const string _DefaultFileName = "File name: ";
		private const string _DefaultFileFilter = "File filter: ";
		private const string _DefaultName = "Name";
		private const string _DefaultType = "Type";
		private const string _DefaultDate = "Date";
		private const string _DefaultSize = "Size [KB]";

		public static readonly string DIN1355_1_DateFormat = "dd.MM.yyyy HH:mm";
		public static readonly string ISO8601_DateFormat = "yyyy-MM-dd HH:mm:ss";
		public static readonly string US_DateFormat = "M/d/yyyy h:mm tt";

		#endregion

		#region Events

		public event EventHandler<OpenDialogEventArgs> ShowOpenDialogEventHandler;
		public event EventHandler<OpenDialogEventArgs> CloseOpenDialogEventHandler;

		#endregion

		#region Constructors

		public OpenDialogViewModelBase()
		{
			DateFormat = DIN1355_1_DateFormat;
			IsDirectoryChooser = false;
			OpenVisibility = true;
			SaveVisibility = false;
			IsSaveDialog = false;

			Items = new ObservableCollection<OpenFolderRoot>();
			Folder = new List<FileItem>();
			FileFilterExtensions = new ObservableCollection<string>();
			FileFilterExtensions.Add(string.Empty);

			OpenCommand = new RelayCommand(param => Open());
			CancelCommand = new RelayCommand(param => Cancel());

			Caption = _DefaultOpen;
			OpenText = _DefaultOpen;
			CancelText = _DefaultCancel;
			SaveText = _DefaultSave;
			FileNameText = _DefaultFileName;
			FileFilterText = _DefaultFileFilter;
			NameText = _DefaultName;
			DateText = _DefaultDate;
			TypeText = _DefaultType;
			SizeText = _DefaultSize;
		}

		#endregion

		#region Properties

		public ICommand OpenCommand
		{
			get;
			private set;
		}

		public ICommand CancelCommand
		{
			get;
			private set;
		}

		public ObservableCollection<OpenFolderRoot> Items
		{
			get
			{
				return _Items;
			}
			set
			{
				_Items = value;
				OnPropertyChanged("Items");
			}
		}

		public OpenFolderItem SelectedFolder
		{
			get
			{
				return _SelectedFolder;
			}
			internal set
			{
				_SelectedFolder = value;
				UpdateFolder();
			}
		}

		public FileItem SelectedFile
		{
			get
			{
				return _SelectedFile;
			}
			set
			{
				_SelectedFile = value;

				if(_SelectedFile != null)
				{
					SelectedFilePath = _SelectedFile.Path;
				}
			}
		}

		public ICollection<FileItem> Folder
		{
			get
			{
				return _Folder;
			}
			set
			{
				_Folder = value;
				OnPropertyChanged("Folder");
			}
		}

		public string SelectedFilePath
		{
			get
			{
				return _SelectedFilePath;
			}
			set
			{
				_SelectedFilePath = value;
				OnPropertyChanged("SelectedFilePath");
			}
		}

		public ObservableCollection<string> FileFilterExtensions
		{
			get
			{
				return _FileFilterExtensions;
			}
			set
			{
				_FileFilterExtensions = value;
				OnPropertyChanged("FileFilterExtensions");
			}
		}

		public string SelectedFileFilterExtension
		{
			get
			{
				return _SelectedFileFilterExtension;
			}
			set
			{
				_SelectedFileFilterExtension = value;
				UpdateFolder();
				OnPropertyChanged("SelectedFileFilterExtension");
			}
		}

		public string OpenText
		{
			get
			{
				return _OpenText;
			}
			set
			{
				_OpenText = value;
				OnPropertyChanged("OpenText");
			}
		}

		public string SaveText
		{
			get
			{
				return _SaveText;
			}
			set
			{
				_SaveText = value;
				OnPropertyChanged("SaveText");
			}
		}

		public string CancelText
		{
			get
			{
				return _CancelText;
			}
			set
			{
				_CancelText = value;
				OnPropertyChanged("CancelText");
			}
		}

		public string FileNameText
		{
			get
			{
				return _FileNameText;
			}
			set
			{
				_FileNameText = value;
				OnPropertyChanged("FileNameText");
			}
		}

		public string FileFilterText
		{
			get
			{
				return _FileFilterText;
			}
			set
			{
				_FileFilterText = value;
				OnPropertyChanged("FileFilterText");
			}
		}

		public string TypeText
		{
			get
			{
				return _TypeText;
			}
			set
			{
				_TypeText = value;
				OnPropertyChanged("TypeText");
			}
		}

		public string SizeText
		{
			get
			{
				return _SizeText;
			}
			set
			{
				_SizeText = value;
				OnPropertyChanged("SizeText");
			}
		}

		public string DateText
		{
			get
			{
				return _DateText;
			}
			set
			{
				_DateText = value;
				OnPropertyChanged("DateText");
			}
		}

		public string NameText
		{
			get
			{
				return _NameText;
			}
			set
			{
				_NameText = value;
				OnPropertyChanged("NameText");
			}
		}

		public bool OpenVisibility
		{
			get
			{
				return _OpenVisibility;
			}
			set
			{
				_OpenVisibility = value;
				OnPropertyChanged("OpenVisibility");
			}
		}

		public bool SaveVisibility
		{
			get
			{
				return _SaveVisibility;
			}
			set
			{
				_SaveVisibility = value;
				OnPropertyChanged("SaveVisibility");
			}
		}

		public bool SelectFolder
		{
			get;
			set;
		}

		public bool? Result
		{
			get;
			set;
		}

		public string Caption
		{
			get;
			set;
		}

		public string DateFormat
		{
			get;
			set;
		}

		public bool IsDirectoryChooser
		{
			get;
			set;
		}

		public bool IsSaveDialog
		{
			get
			{
				return _IsSaveDialog;
			}
			set
			{
				_IsSaveDialog = value;
				if(_IsSaveDialog)
				{
					OpenVisibility = false;
					SaveVisibility = true;
					Caption = _DefaultSave;
				}
				else
				{
					SaveVisibility = false;
					OpenVisibility = true;
					Caption = _DefaultOpen;
				}
			}
		}

		public System.Windows.WindowStartupLocation StartupLocation
		{
			get;
			set;
		}

		public System.Windows.Window Owner
		{
			get;
			set;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Add extension of file including ".".
		/// </summary>
		/// <param name="extension"></param>
		public void AddFileFilterExtension(string extension)
		{
			FileFilterExtensions.Add(extension);
		}

		private void UpdateFolder()
		{
			List<FileItem> folder = new List<FileItem>();
			try
			{
				if(_SelectedFolder == null)
				{
					return;
				}

				string path = _SelectedFolder.Path ?? string.Empty;
				if(!string.IsNullOrEmpty(path))
				{
					if(IsDirectoryChooser)
					{
						string[] directories = Directory.GetDirectories(path);

						foreach(string directory in directories)
						{
							DirectoryInfo di = new DirectoryInfo(directory);

							FileItem item = new FileItem();
							item.Path = directory;
							item.Name = Path.GetFileName(directory);
							item.Date = di.LastWriteTime.ToString(DateFormat, CultureInfo.InvariantCulture);
							item.Image = FileInfoHelper.GetSmallImage(directory, true);
							folder.Add(item);
						}
					}
					else
					{
						string[] files = Directory.GetFiles(path);

						foreach(string file in files)
						{
							if(!string.IsNullOrEmpty(SelectedFileFilterExtension))
							{
								if(Path.GetExtension(file) != SelectedFileFilterExtension)
								{
									continue;
								}
							}

							FileInfo fi = new FileInfo(file);

							FileItem item = new FileItem();
							item.Path = file;
							item.Name = Path.GetFileName(file);
							item.Date = fi.LastWriteTime.ToString(DateFormat, CultureInfo.InvariantCulture);
							item.Size = string.Format("{0}", fi.Length / 1024);
							item.Type = FileInfoHelper.GetFileType(file);
							item.Image = FileInfoHelper.GetSmallImage(file, true);
							folder.Add(item);
						}
					}
				}
			}
			catch(UnauthorizedAccessException)
			{

			}

			Folder = new ReadOnlyCollection<FileItem>(folder);
		}

		private void Open()
		{
			Result = true;
			Close();
		}

		private void Cancel()
		{
			SelectedFilePath = null;
			Result = false;
			Close();
		}

		private void Close()
		{
			if(CloseOpenDialogEventHandler != null)
			{
				CloseOpenDialogEventHandler(this, new OpenDialogEventArgs());
			}
		}

		public bool? Show()
		{
			if(ShowOpenDialogEventHandler != null)
			{
				ShowOpenDialogEventHandler(this, new OpenDialogEventArgs() { Caption = Caption, StartupLocation = StartupLocation, Owner = Owner });
			}
			return Result;
		}

		#endregion
	}
}
