// -----------------------------------------------------------------------
// <copyright file="ImageLoader.cs" company="">
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
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Windows;
	using System.Windows.Media;
	using System.Windows.Media.Imaging;

	internal static class FileInfoHelper
	{
		private static IDictionary<string, ImageSource> _Cache = new Dictionary<string, ImageSource>();

		// http://support.microsoft.com/kb/319350
		[StructLayout(LayoutKind.Sequential)]
		internal struct SHFILEINFO
		{
			public IntPtr hIcon;
			public IntPtr iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		};

		internal class Win32
		{
			public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
			public const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;
			public const uint SHGFI_TYPENAME = 0x000000400;
			public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;

			internal const uint SHGFI_SYSICONINDEX = 0x000004000;
			internal const int ILD_TRANSPARENT = 0x1;

			internal const uint SHGFI_ICON = 0x100;
			internal const uint SHGFI_LARGEICON = 0x0; // 'Large icon
			internal const uint SHGFI_SMALLICON = 0x1; // 'Small icon

			[DllImport("shell32.dll")]
			internal static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

			[DllImport("shell32.dll", CharSet = CharSet.Auto)]
			internal static extern int ExtractIconEx(string stExeFileName, int nIconIndex, ref IntPtr phiconLarge, ref IntPtr phiconSmall, int nIcons);

			[DllImport("comctl32.dll", SetLastError = true)]
			internal static extern IntPtr ImageList_GetIcon(IntPtr himl, int i, int flags);

			[DllImport("user32.dll")]
			internal static extern bool DestroyIcon(IntPtr hIcon);
		}

		#region Special Images

		internal static ImageSource GetComputerImage(bool large)
		{
			return GetImage("shell32.dll", 15, large);
		}

		internal static ImageSource GetFavoritesImage(bool large)
		{
			return GetImage("shell32.dll", 43, large);
		}

		internal static ImageSource GetFolderImage(bool large)
		{
			if(_Cache.ContainsKey("!!!"))
			{
				return _Cache["!!!"];
			}
			ImageSource image = GetImage("shell32.dll", 4, large);
			_Cache.Add("!!!", image);
			return image;
		}

		internal static ImageSource GetNetworkNeighborhoodImage(bool large)
		{
			return GetImage("shell32.dll", 17, large);
		}

		internal static ImageSource GetWarningImage(bool large)
		{
			return GetImage("user32.dll", 1, large);
		}

		internal static ImageSource GetQuestionImage(bool large)
		{
			return GetImage("user32.dll", 2, large);
		}

		internal static ImageSource GetErrorImage(bool large)
		{
			return GetImage("user32.dll", 3, large);
		}

		internal static ImageSource GetInformationImage(bool large)
		{
			return GetImage("user32.dll", 4, large);
		}

		internal static ImageSource GetLibrariesImage(bool large)
		{
			return GetImage("imageres.dll", 202, large);
		}

		#endregion

		private static ImageSource GetImage(string file, int index, bool large)
		{
			IntPtr hImgLarge = IntPtr.Zero; //the handle to the system image list
			IntPtr hImgSmall = IntPtr.Zero; //the handle to the system image list

			int count = Win32.ExtractIconEx(file, index, ref hImgLarge, ref hImgSmall, 1);
			ImageSource image;
			if(large)
			{
				image = GetImage(hImgLarge);
			}
			else
			{
				image = GetImage(hImgSmall);
			}

			Win32.DestroyIcon(hImgLarge);
			Win32.DestroyIcon(hImgSmall);

			return image;
		}

		internal static ImageSource GetSmallImage(string filename, bool useCache)
		{
			string extension = Path.GetExtension(filename).ToLower();
			if(useCache)
			{
				if(_Cache.ContainsKey(filename))
				{
					return _Cache[filename];
				}

				if(_Cache.ContainsKey(extension))
				{
					return _Cache[extension];
				}

				FileAttributes attributes = File.GetAttributes(filename);
				if((attributes & FileAttributes.Directory) == FileAttributes.Directory)
				{
					if(!ShortcutHelper.IsShortcut(filename))
					{
						if(_Cache.ContainsKey("!!!"))
						{
							return _Cache["!!!"];
						}
						else
						{
							ImageSource folderImage = GetFolderImage(false);
							_Cache.Add("!!!", folderImage);
							return folderImage;
						}
					}
				}
			}

			IntPtr hImgSmall; //the handle to the system image list
			SHFILEINFO shinfo = new SHFILEINFO();

			//Use this to get the small Icon
			hImgSmall = Win32.SHGetFileInfo(filename, Win32.FILE_ATTRIBUTE_NORMAL, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON | Win32.SHGFI_USEFILEATTRIBUTES);

			ImageSource image = GetImage(shinfo.hIcon);
			Win32.DestroyIcon(hImgSmall);

			if(useCache)
			{
				if(extension == ".exe" || extension == ".ico" || extension == string.Empty || ShortcutHelper.IsShortcut(filename))
				{
					_Cache.Add(filename, image);
				}
				else
				{
					_Cache.Add(extension, image);
				}
			}

			return image;
		}

		internal static ImageSource GetLargeImage(string filename, bool useCache)
		{
			string extension = Path.GetExtension(filename).ToLower();
			if(useCache)
			{
				if(_Cache.ContainsKey(filename))
				{
					return _Cache[filename];
				}
				if(_Cache.ContainsKey(extension))
				{
					return _Cache[extension];
				}

				FileAttributes attributes = File.GetAttributes(filename);
				if((attributes & FileAttributes.Directory) == FileAttributes.Directory)
				{
					if(!ShortcutHelper.IsShortcut(filename))
					{
						if(_Cache.ContainsKey("!!!"))
						{
							return _Cache["!!!"];
						}
						else
						{
							ImageSource folderImage = GetFolderImage(false);
							_Cache.Add("!!!", folderImage);
							return folderImage;
						}
					}
				}
			}

			IntPtr hImgLarge; //the handle to the system image list
			SHFILEINFO shinfo = new SHFILEINFO();

			//Use this to get the large Icon
			hImgLarge = Win32.SHGetFileInfo(filename, Win32.FILE_ATTRIBUTE_NORMAL, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON | Win32.SHGFI_USEFILEATTRIBUTES);

			ImageSource image = GetImage(shinfo.hIcon);
			Win32.DestroyIcon(hImgLarge);

			if(useCache)
			{
				if(extension == ".exe" || extension == ".ico" || extension == string.Empty || ShortcutHelper.IsShortcut(filename))
				{
					_Cache.Add(filename, image);
				}
				else
				{
					_Cache.Add(extension, image);
				}
			}

			return image;
		}

		private static ImageSource GetImage(IntPtr hIcon)
		{
			try
			{
				//The icon is returned in the hIcon member of the shinfo struct
				System.Drawing.Icon myIcon = System.Drawing.Icon.FromHandle(hIcon);
				BitmapSource image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(myIcon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

				return image;
			}
			catch
			{
				return null;
			}
		}

		internal static ImageSource GetImage(string filename, bool useCache)
		{
			string extension = Path.GetExtension(filename).ToLower();
			if(useCache)
			{
				if(_Cache.ContainsKey(filename))
				{
					return _Cache[filename];
				}
				if(_Cache.ContainsKey(extension))
				{
					return _Cache[extension];
				}

				FileAttributes attributes = File.GetAttributes(filename);
				if((attributes & FileAttributes.Directory) == FileAttributes.Directory)
				{
					if(!ShortcutHelper.IsShortcut(filename))
					{
						if(_Cache.ContainsKey("!!!"))
						{
							return _Cache["!!!"];
						}
						else
						{
							ImageSource folderImage = GetFolderImage(false);
							_Cache.Add("!!!", folderImage);
							return folderImage;
						}
					}
				}
			}

			SHFILEINFO shinfo = new SHFILEINFO();

			IntPtr ptrIconList = Win32.SHGetFileInfo(filename, Win32.FILE_ATTRIBUTE_NORMAL, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_SYSICONINDEX | Win32.SHGFI_SMALLICON | Win32.SHGFI_USEFILEATTRIBUTES);
			IntPtr ptrIcon = Win32.ImageList_GetIcon(ptrIconList, shinfo.iIcon.ToInt32(), Win32.ILD_TRANSPARENT);
			ImageSource image = GetImage(ptrIcon);

			Win32.DestroyIcon(ptrIcon);
			Win32.DestroyIcon(ptrIconList);

			if(useCache)
			{
				if(extension == ".exe" || extension == ".ico" || extension == string.Empty || ShortcutHelper.IsShortcut(filename))
				{
					_Cache.Add(filename, image);
				}
				else
				{
					_Cache.Add(extension, image);
				}
			}

			return image;
		}

		internal static string GetFileType(string filename)
		{
			SHFILEINFO shinfo = new SHFILEINFO();
			Win32.SHGetFileInfo(filename, Win32.FILE_ATTRIBUTE_NORMAL, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_TYPENAME | Win32.SHGFI_USEFILEATTRIBUTES);

			return shinfo.szTypeName;
		}
	}
}
