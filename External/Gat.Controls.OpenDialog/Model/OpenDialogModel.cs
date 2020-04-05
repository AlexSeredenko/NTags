// -----------------------------------------------------------------------
// <copyright file="OpenDialogModel.cs" company="">
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
	using System.IO;
	using Gat.Controls.Utilities;

	internal class OpenDialogModel : OpenDialogModelBase
	{

		public OpenFolderRoot GetFavorites()
		{
			OpenFolderRoot root = new OpenFolderRoot();
			root.Name = "Favorites";
			root.Image = FileInfoHelper.GetFavoritesImage(false);
			root.Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Links");

			if(Directory.Exists(root.Path))
			{
				List<OpenFolderItem> items = new List<OpenFolderItem>();
				foreach(string file in Directory.GetFiles(root.Path))
				{
					if(ShortcutHelper.IsShortcut(file))
					{
						OpenFolderItem item = new OpenFolderItem();
						string resolved = ShortcutHelper.ResolveShortcut(file);
						item.Path = resolved;
						item.Name = Path.GetFileNameWithoutExtension(file);
						item.Image = FileInfoHelper.GetImage(file, false);
						items.Add(item);
					}
				}
				root.Children = items;
			}
			return root;
		}

		public OpenFolderRoot GetLibraries()
		{
			OpenFolderRoot root = new OpenFolderRoot();
			root.Name = "Libraries";
			root.Image = FileInfoHelper.GetLibrariesImage(false);

			if(PlatformHelper.IsMinWin7)
			{
				//IKnownFolder folder = ShellLibrary.LibrariesKnownFolder;

				//root.Path = folder.Path;

				//List<OpenFolderItem> items = new List<OpenFolderItem>();
				//foreach(var lib in folder)
				//{
				//	OpenFolderItem item = new OpenFolderItem();
				//	item.Name = lib.Name;
				//	item.Path = Path.Combine(root.Path, item.Name);
				//	//item.Image = ImageLoader.GetSmallImage(item.Path);
				//	items.Add(item);
				//}
				//root.Children = items;

			}

			return root;

		}

		public OpenFolderRoot GetNetwork()
		{
			OpenFolderRoot root = new OpenFolderRoot();
			root.Name = "Network";
			root.Image = FileInfoHelper.GetNetworkNeighborhoodImage(false);

			return root;

		}
	}
}
