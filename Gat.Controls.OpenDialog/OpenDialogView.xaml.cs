// -----------------------------------------------------------------------
// <copyright file="OpenDialogView.xaml.cs" company="">
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
	using System.Windows;
	using System.Windows.Controls;

	/// <summary>
	/// Interaction logic for OpenDialogView.xaml
	/// </summary>
	public partial class OpenDialogView : UserControl
	{
		private Window _Window;

		public OpenDialogView()
		{
			InitializeComponent();
		}

		private void Folders_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
		{
			ViewModel.SelectedFolder = e.NewValue as Gat.Controls.Model.OpenFolderItem;
		}

		private void OnShow(object sender, OpenDialogEventArgs e)
		{
			_Window = new Window();
			_Window.Content = this;
			_Window.SizeToContent = SizeToContent.Manual;
			_Window.ResizeMode = ResizeMode.CanResizeWithGrip;
			_Window.WindowStyle = WindowStyle.ToolWindow;
			_Window.Title = e.Caption;
			_Window.ShowInTaskbar = false;
			_Window.Topmost = true;
			_Window.Height = 420;
			_Window.Width = 700;
			_Window.Owner = e.Owner;
			_Window.WindowStartupLocation = e.StartupLocation;
			_Window.ShowDialog();
		}

		private void OnClose(object sender, OpenDialogEventArgs e)
		{
			_Window.Close();
		}
	}
}
