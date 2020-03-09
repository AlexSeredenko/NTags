// -----------------------------------------------------------------------
// <copyright file="OpenDialogModelBase.cs" company="">
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
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using Gat.Controls.Utilities;

	internal class OpenDialogModelBase
	{
		public OpenFolderRoot GetComputer()
		{
			OpenFolderRoot root = new OpenFolderRoot();
			root.Name = "Computer";
			root.Image = FileInfoHelper.GetComputerImage(false);

			List<OpenFolderItem> items = new List<OpenFolderItem>();
			DriveInfo[] drives = DriveInfo.GetDrives();
			foreach(var drive in drives)
			{
				if(drive.IsReady)
				{
					StringBuilder sb = new StringBuilder();
					sb.Append(drive.DriveType);
					sb.Append(" (");
					sb.Append(drive.Name, 0, drive.Name.Length - 1);
					sb.Append(") ");
					sb.Append(drive.VolumeLabel);

					OpenFolderItem item = new OpenFolderItem(drive.Name);
					item.Name = sb.ToString();
					item.Image = FileInfoHelper.GetSmallImage(item.Path, false);
					items.Add(item);
				}
			}

			root.Children = items;

			return root;
		}
	}
}
