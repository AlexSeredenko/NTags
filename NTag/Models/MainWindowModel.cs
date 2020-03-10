using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using NTag.Interfaces;

namespace NTag.Models
{
    public class MainWindowModel
    {
        private IConfiguration _configuration;

        public ObservableCollection<TrackModel> TrackModels { get; set; }

        public MainWindowModel(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            TrackModels = new ObservableCollection<TrackModel>();

            for (int i = 0; i < 10; i++)
            {
                TrackModels.Add(new TrackModel()
                {
                    FileDir = @"C:\",
                    OriginalFileName = "Original file name" + $" #{i}",
                    OriginalAlbum = "Original album" + $" #{i}",
                    OriginalPerformer = "Original performer" + $" #{i}",
                    OriginalTitle = "Original title" + $" #{i}",
                    ModifiedFileName = "Modified file name" + $" #{i}",
                    ModifiedAlbum = "Modified album" + $" #{i}",
                    ModifiedPerformer = "Modified performer" + $" #{i}",
                    ModifiedTitle = "Modified title" + $" #{i}"
                });
            }
        }

        public void OpenFolder(string folderPath)
        {
            var supportedFormats = _configuration.SupportedFormats;
            var mediaFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(x => supportedFormats.Contains(Path.GetExtension(x).ToLower())).ToArray();
        }
    }
}
