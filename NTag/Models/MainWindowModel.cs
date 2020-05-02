using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using UnidecodeSharpFork;
using TagLib;
using NTag.Interfaces;
using NTag.Exceptions;

namespace NTag.Models
{
    public class MainWindowModel
    {
        private IConfiguration _configuration;
        private CancellationTokenSource _cancellationTokenSource;

        public Action ProcessingStarted { get; set; }
        public Action ProcessingFinished { get; set; }
        public Action<int, int> ProgressChanged { get; set; }
        public Action<string> Notification { get; set; }

        public ObservableCollection<TrackModel> TrackModels { get; set; }

        public MainWindowModel(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            TrackModels = new ObservableCollection<TrackModel>();
        }

        private Task<bool> CheckPictureAsync(string imgPath)
        {
            return Task.Run(() =>
            {
                if (!System.IO.File.Exists(imgPath))
                {
                    throw new FileNotFoundException(imgPath);
                }

                var bitmapSize = default(System.Drawing.Size);

                using (var bitmap = new System.Drawing.Bitmap(imgPath))
                {
                    bitmapSize = bitmap.Size;
                }

                if (bitmapSize.Width > _configuration.AllowedTagImageSize.Width ||
                    bitmapSize.Height > _configuration.AllowedTagImageSize.Height ||
                    bitmapSize.Width != bitmapSize.Height)
                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.Append("Unsupported image size. Max size: ");
                    stringBuilder.Append(_configuration.AllowedTagImageSize.Width.ToString());
                    stringBuilder.Append("x");
                    stringBuilder.Append(_configuration.AllowedTagImageSize.Height.ToString());
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.Append("Width should be equal height!");

                    throw new UnsupportedImageSizeException(stringBuilder.ToString());
                }

                return true;
            });
        }

        private Task<IPicture> LoadPictureAsync(string imgPath)
        {
            return Task.Run(() =>
            {
                var pic = new Picture(imgPath);
                pic.Type = PictureType.FrontCover;

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

        private char[] CheckTrackTags(TrackModel track)
        {
            var result = track.ModifiedAlbum.Where(x => !_configuration.AllowedChars.Contains(x))
                .Union(track.ModifiedPerformer.Where(x => !_configuration.AllowedChars.Contains(x)))
                .Union(track.ModifiedTitle.Where(x => !_configuration.AllowedChars.Contains(x)));

            return result?.ToArray();
        }

        [Conditional("DEBUG")]
        private void DebugTimeout(int timeout)
        {
            Thread.Sleep(1000);
        }

        private void ProcessTrack(TrackModel track)
        {
            DebugTimeout(1000);

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

        public async Task OpenFolder(string folderPath)
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

        private void SetPicture(TrackModel trackModel, IPicture pic)
        {
            trackModel.ModifiedImage = pic;
        }

        public async Task SetPictureFromFile(TrackModel trackModel, string imgPath)
        {
            var checkResult = false;

            try
            {
                checkResult = await CheckPictureAsync(imgPath);
            }
            catch (UnsupportedImageSizeException ex)
            {
                Notification?.Invoke(ex.Message);
            }

            if (checkResult)
            {
                var pic = await LoadPictureAsync(imgPath);
                SetPicture(trackModel, pic);
            }
        }

        public void TitleFromFileName(TrackModel trackModel)
        {
            var title = string.Empty;

            if (trackModel.ModifiedFileName.Contains(_configuration.PerformerTitleDelimiter))
            {
                title = trackModel.ModifiedFileName.Split(_configuration.PerformerTitleDelimiter).LastOrDefault()?.Trim();
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

            if (trackModel.ModifiedFileName.Contains(_configuration.PerformerTitleDelimiter))
            {
                performer = trackModel.ModifiedFileName.Split(_configuration.PerformerTitleDelimiter).FirstOrDefault()?.Trim();
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
                trackModel.ModifiedFileName = $"{trackModel.ModifiedPerformer.Trim()} {_configuration.PerformerTitleDelimiter} {trackModel.ModifiedTitle.Trim()}{ext}";
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

        public void SaveTagImageToFile(TrackModel trackModel, string filePath)
        {
            var bitmapFrame = trackModel.GetBitmapFrameFromPicture(trackModel.OriginalImage);

            if (bitmapFrame != null)
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    var encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(bitmapFrame);
                    encoder.Save(fileStream);
                }
            }
        }

        public void Start(CancellationToken cancellationToken)
        {
            for (int i = 0; i < TrackModels.Count; i++)
            {
                var checkRes = CheckTrackTags(TrackModels[i]);
                if (checkRes != null && checkRes.Length > 0)
                {
                    var msg = $"Forbidden symbols in track #{TrackModels[i].TrackNumber}:{Environment.NewLine}{new string(checkRes)}";
                    Notification?.Invoke(msg);
                    return;
                }
            }

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

            ProcessingFinished?.Invoke();
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
