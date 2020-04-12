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
using System.Threading;

namespace NTag.Models
{
    public class MainWindowModel
    {
        private IConfiguration _configuration;
        private CancellationTokenSource _cancellationTokenSource;

        public Action ProcessingStarted { get; set; }
        public Action ProcessingFinished { get; set; }
        public Action<int, int> ProgressChanged { get; set; }

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

        private Task<TrackModel> CreateTrackModelAsync(string filePath, int num)
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
                    TrackNumber = num,
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

        private void ProcessTrack(TrackModel track)
        {
            try
            {
                if (!track.OriginalFileName.Equals(track.ModifiedFileName))
                {
                    System.IO.File.Move(Path.Combine(track.FileDir, track.OriginalFileName),
                        Path.Combine(track.FileDir, track.ModifiedFileName));
                }

                var tagfile = TagLib.File.Create(Path.Combine(track.FileDir, track.ModifiedFileName));
                tagfile.RemoveTags(TagTypes.AllTags);
                tagfile.Save();

                tagfile = TagLib.File.Create(Path.Combine(track.FileDir, track.ModifiedFileName));
                tagfile.Tag.Album = track.ModifiedAlbum;
                tagfile.Tag.Performers = new string[] { track.ModifiedPerformer };
                tagfile.Tag.Title = track.ModifiedTitle;

                if (track.ModifiedImage != null)
                {
                    tagfile.Tag.Pictures = new IPicture[] { track.ModifiedImage };
                }

                tagfile.Save();
                track.ResultCode = TrackModel.SuccessfulCode;
            }
            catch (Exception ex)
            {
                track.ResultCode = TrackModel.ErrorCode;
                track.ProcessingException = ex;
            }
        }

        public async void OpenFolder(string folderPath)
        {
            var supportedFormats = _configuration.SupportedFormats;
            var mediaFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(x => supportedFormats.Contains(Path.GetExtension(x).ToLower())).ToArray();

            TrackModels.Clear();

            foreach (var mediaFile in mediaFiles)
            {
                var trackModel = await CreateTrackModelAsync(mediaFile, TrackModels.Count + 1);
                TrackModels.Add(trackModel);
            }
        }

        public void SetPicture(TrackModel trackModel, IPicture pic)
        {
            trackModel.ModifiedImage = pic;
        }

        public async void SetPictureFromFile(TrackModel trackModel, string imgPath)
        {
            var pic = await LoadPictureAsync(imgPath);
            SetPicture(trackModel, pic);
        }

        public void TitleFromFileName(TrackModel trackModel)
        {
            var title = string.Empty;

            if (trackModel.ModifiedFileName.Contains("-"))
            {
                title = trackModel.ModifiedFileName.Split("-").LastOrDefault()?.Trim();
            }

            if (string.IsNullOrEmpty(title))
            {
                title = trackModel.ModifiedFileName;
            }

            trackModel.ModifiedTitle = Path.GetFileNameWithoutExtension(title);
        }

        public void PerformerFromFileName(TrackModel trackModel)
        {
            var performer = string.Empty;

            if (trackModel.ModifiedFileName.Contains("-"))
            {
                performer = trackModel.ModifiedFileName.Split("-").FirstOrDefault()?.Trim();
            }

            if (string.IsNullOrEmpty(performer))
            {
                performer = trackModel.ModifiedFileName;
            }

            trackModel.ModifiedPerformer = performer;
        }

        public void FileNameFromTags(TrackModel trackModel)
        {
            if (!string.IsNullOrEmpty(trackModel.ModifiedPerformer) &&
                !string.IsNullOrEmpty(trackModel.ModifiedTitle))
            {
                var ext = Path.GetExtension(trackModel.ModifiedFileName);
                trackModel.ModifiedFileName = $"{trackModel.ModifiedPerformer.Trim()} - {trackModel.ModifiedTitle.Trim()}{ext}";
            }
        }

        public void FileNameFromTagsAll()
        {
            foreach (var tm in TrackModels)
            {
                FileNameFromTags(tm);
            }
        }

        public void DuplicatePictureAll(TrackModel trackModel)
        {
            var pic = trackModel.ModifiedImage;

            foreach (var tm in TrackModels.Where(x => !x.Equals(trackModel)))
            {
                SetPicture(tm, pic);
            }
        }

        public void DuplicateAlbumAll(TrackModel trackModel)
        {
            foreach (var tm in TrackModels.Where(x => !x.Equals(trackModel)))
            {
                tm.ModifiedAlbum = trackModel.ModifiedAlbum;
            }
        }

        public void DuplicatePerformerAll(TrackModel trackModel)
        {
            foreach (var tm in TrackModels.Where(x => !x.Equals(trackModel)))
            {
                tm.ModifiedPerformer = trackModel.ModifiedPerformer;
            }
        }

        public void DuplicateTitleAll(TrackModel trackModel)
        {
            foreach (var tm in TrackModels.Where(x => !x.Equals(trackModel)))
            {
                tm.ModifiedTitle = trackModel.ModifiedTitle;
            }
        }

        public void Start(CancellationToken cancellationToken)
        {
            ProcessingStarted?.Invoke();

            try
            {
                for (int i = 0; i < TrackModels.Count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    ProcessTrack(TrackModels[i]);
                    ProgressChanged?.Invoke(TrackModels.Count, i + 1);
                }
            }
            catch (OperationCanceledException)
            { }
        }

        public Task StartAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            return Task.Run(() => Start(_cancellationTokenSource.Token));
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
