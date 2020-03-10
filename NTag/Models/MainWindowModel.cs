using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnidecodeSharpFork;
using TagLib;
using TagLib.Id3v2;
using NTag.Interfaces;
using System.Threading.Tasks;

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

            //for (int i = 0; i < 10; i++)
            //{
            //    TrackModels.Add(new TrackModel()
            //    {
            //        FileDir = @"C:\",
            //        OriginalFileName = "Original file name" + $" #{i}",
            //        OriginalAlbum = "Original album" + $" #{i}",
            //        OriginalPerformer = "Original performer" + $" #{i}",
            //        OriginalTitle = "Original title" + $" #{i}",
            //        ModifiedFileName = "Modified file name" + $" #{i}",
            //        ModifiedAlbum = "Modified album" + $" #{i}",
            //        ModifiedPerformer = "Modified performer" + $" #{i}",
            //        ModifiedTitle = "Modified title" + $" #{i}"
            //    });
            //}
        }

        private Task<TrackModel> CreateTrackModelAsync(string filePath)
        {
            return Task.Run(() =>
            {
                var tagFile = TagLib.File.Create(filePath);                
                var trackModel = new TrackModel()
                {
                    FileDir = Path.GetDirectoryName(filePath),
                    OriginalFileName = Path.GetFileName(filePath),
                    OriginalAlbum = tagFile.Tag.Album,
                    OriginalPerformer = tagFile.Tag.Performers.FirstOrDefault(),
                    OriginalTitle = tagFile.Tag.Title,
                    ModifiedFileName = Path.GetFileName(filePath).Unidecode(),
                    ModifiedAlbum = Directory.GetParent(filePath).Name,
                    //ModifiedPerformer = "Modified performer" + $" #{i}",
                    //ModifiedTitle = "Modified title" + $" #{i}"
                };

                return trackModel;
            });
        }

        public async void OpenFolder(string folderPath)
        {
            var supportedFormats = _configuration.SupportedFormats;
            var mediaFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(x => supportedFormats.Contains(Path.GetExtension(x).ToLower())).ToArray();

            TrackModels.Clear();

            foreach (var mediaFile in mediaFiles)
            {
                var trackModel = await CreateTrackModelAsync(mediaFile);
                TrackModels.Add(trackModel);
            }
        }
    }
}
