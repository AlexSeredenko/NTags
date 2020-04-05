// -----------------------------------------------------------------------
// <copyright file="OpenFolderItem.cs" company="">
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

namespace Gat.Controls.Model
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.IO;
	using System.Windows.Media;
	using Gat.Controls.Framework;
	using Gat.Controls.Utilities;

	public class OpenFolderItem : ViewModelBase
	{
		#region Fields

		ICollection<OpenFolderItem> _Children;
		bool _IsSelected;
		bool _IsExpanded;

		#endregion

		#region Constructors

		public OpenFolderItem()
		{

		}

		public OpenFolderItem(string path)
		{
			Path = path;
			//Children = LoadChildren();
		}


		#endregion

		public string Name
		{
			get;
			set;
		}

		public string Path
		{
			get;
			set;
		}

		public ImageSource Image
		{
			get;
			set;
		}

		public virtual ICollection<OpenFolderItem> Children
		{
			get
			{
				if(_Children == null)
				{
					_Children = LoadChildren();
				}

				return _Children;
			}
			set
			{

			}
		}

		private ICollection<OpenFolderItem> LoadChildren()
		{
			List<OpenFolderItem> items = new List<OpenFolderItem>();
			try
			{
				foreach(string directory in Directory.GetDirectories(Path))
				{
					OpenFolderItem item = new OpenFolderItem(directory);
					item.Name = System.IO.Path.GetFileName(directory);
					item.Image = FileInfoHelper.GetFolderImage(false);
					items.Add(item);
				}
			}
			catch(UnauthorizedAccessException) { }
			catch(ArgumentException) { }
			catch(DirectoryNotFoundException) { }

			return new ReadOnlyCollection<OpenFolderItem>(items);
		}
	}
}
