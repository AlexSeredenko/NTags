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
        }

        private Task<IPicture> LoadPictureAsync(string imgPath)
        {
            return Task.Run(() =>
            {
                if (!System.IO.File.Exists(imgPath))
                {
                    throw new FileNotFoundException(imgPath);
                }

                var pic = new Picture(imgPath);
                pic.Type = PictureType.BackCover;

                return (IPicture)pic;
            });
        }

        private Task<TrackModel> CreateTrackModelAsync(string filePath)
        {
            return Task.Run(() =>
            {
                var tagFile = TagLib.File.Create(filePath);
                var modifiedFileName = Path.GetFileName(filePath).Unidecode();
                var modifiedAlbum = Directory.GetParent(filePath).Name.Unidecode();
                var modifiedPerformer = tagFile.Tag.Performers.FirstOrDefault().Unidecode();
                var modifiedTitle = tagFile.Tag.Title.Unidecode();

                var trackModel = new TrackModel()
                {
                    FileDir = Path.GetDirectoryName(filePath),
                    OriginalFileName = Path.GetFileName(filePath),
                    OriginalAlbum = tagFile.Tag.Album,
                    OriginalPerformer = tagFile.Tag.Performers.FirstOrDefault(),
                    OriginalTitle = tagFile.Tag.Title,
                    OriginalImage = tagFile.Tag.Pictures.FirstOrDefault(),
                    ModifiedFileName = modifiedFileName,
                    ModifiedAlbum = modifiedAlbum,
                    ModifiedPerformer = modifiedPerformer,
                    ModifiedTitle = modifiedTitle,
                    ModifiedImage = tagFile.Tag.Pictures.FirstOrDefault()
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

        public void SetPicture(TrackModel trackModel, IPicture pic)
        {
            trackModel.ModifiedImage = pic;
        }

        public void DuplicatePictureAll(TrackModel trackModel)
        {
            var pic = trackModel.ModifiedImage;

            foreach (var tm in TrackModels.Where(x => !x.Equals(trackModel)))
            {
                SetPicture(tm, pic);
            }
        }

        public async void SetPictureFromFile(TrackModel trackModel, string imgPath)
        {
            var pic = await LoadPictureAsync(imgPath);
            SetPicture(trackModel, pic);
        }

        public async void SetPictureFromFileAll(string imgPath)
        {
            var pic = await LoadPictureAsync(imgPath);

            foreach (var trackModel in TrackModels)
            {
                SetPicture(trackModel, pic);
            }
        }
    }
}
