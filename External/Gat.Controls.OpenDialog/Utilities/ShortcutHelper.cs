// -----------------------------------------------------------------------
// <copyright file="ShortcutHelper.cs" company="">
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

namespace Gat.Controls.Utilities
{
	using System.IO;

	internal static class ShortcutHelper
	{
		internal static bool IsShortcut(string path)
		{
			if(!File.Exists(path))
			{
				return false;
			}

			string directory = Path.GetDirectoryName(path);
			string file = Path.GetFileName(path);

			try
			{
				Shell32.Shell shell = new Shell32.Shell();
				Shell32.Folder folder = shell.NameSpace(directory);
				Shell32.FolderItem folderItem = folder.ParseName(file);

				if (folderItem != null)
				{
					return folderItem.IsLink;
				}
			}
			catch { }

			return false;
		}

		internal static string ResolveShortcut(string path)
		{
			if(IsShortcut(path))
			{
				string directory = Path.GetDirectoryName(path);
				string file = Path.GetFileName(path);

				Shell32.Shell shell = new Shell32.Shell();
				Shell32.Folder folder = shell.NameSpace(directory);
				Shell32.FolderItem folderItem = folder.ParseName(file);

				Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)folderItem.GetLink;
				return link.Path;
			}
			return string.Empty;
		}
	}
}
