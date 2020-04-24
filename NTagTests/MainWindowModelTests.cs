using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTag.Interfaces;
using NTag.Models;
using NTagTests.Models;

namespace NTagTests
{
    [TestClass]
    public class MainWindowModelTests
    {
        private static string _deploymentDirectory;
        private static string _testDataFolder = @"TestsData";
        private static string _testDataMusicFolder = @"TestsData\mp3";
        private static string _testDataImageFolder = @"TestsData\jpg";

        private MainWindowModel _mainWindowModel;
        private IConfiguration _configurationModel;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            _deploymentDirectory = context.DeploymentDirectory;
            _testDataFolder = Path.Combine(_deploymentDirectory, _testDataFolder);
            _testDataMusicFolder = Path.Combine(_deploymentDirectory, _testDataMusicFolder);
            _testDataImageFolder = Path.Combine(_deploymentDirectory, _testDataImageFolder);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _configurationModel = new ConfigurationModelMock();
            _mainWindowModel = new MainWindowModel(_configurationModel);
            Assert.IsNotNull(_mainWindowModel.TrackModels);
            Assert.AreEqual(0, _mainWindowModel.TrackModels.Count);
            _mainWindowModel.OpenFolder(_testDataMusicFolder).Wait();
        }

        [TestMethod]
        public void OpenFolderTest()
        {
            var tracksCount = Directory.GetFiles(_testDataMusicFolder, "*.mp3").Length;
            Assert.AreEqual(tracksCount, _mainWindowModel.TrackModels.Count);
        }

        [TestMethod]
        public void SetPictureFromFileAndSaveTest()
        {
            var imagePath = Directory.GetFiles(_testDataImageFolder, "*.jpg").First();
            var track = _mainWindowModel.TrackModels.First();
            var oldTrackPicture = track.ModifiedImage;
            _mainWindowModel.SetPictureFromFile(track, imagePath).Wait();
            Assert.IsNotNull(track.ModifiedImage);
            Assert.AreNotEqual(oldTrackPicture, track.ModifiedImage);

            var newImagePath = Guid.NewGuid().ToString().Replace(_configurationModel.PerformerTitleDelimiter, string.Empty);
            newImagePath = Path.Combine(_deploymentDirectory, newImagePath);
            _mainWindowModel.SaveTagImageToFile(track, newImagePath);
            Assert.IsTrue(System.IO.File.Exists(newImagePath));
            var imgFileInfo = new FileInfo(newImagePath);
            Assert.AreNotEqual(0, imgFileInfo.Length);
        }

        [TestMethod]
        public void TitleFromFileNameTest()
        {
            var performer = Guid.NewGuid().ToString().Replace(_configurationModel.PerformerTitleDelimiter, string.Empty);
            var title = Guid.NewGuid().ToString().Replace(_configurationModel.PerformerTitleDelimiter, string.Empty);
            var track = _mainWindowModel.TrackModels.First();
            var oldTitle = track.ModifiedTitle;
            Assert.AreNotEqual(oldTitle, title);
            track.ModifiedFileName = $"{performer} {_configurationModel.PerformerTitleDelimiter} {title}{Path.GetExtension(track.ModifiedFileName)}";
            _mainWindowModel.TitleFromFileName(track);
            Assert.AreEqual(title, track.ModifiedTitle);
        }

        [TestMethod]
        public void PerformerFromFileNameTest()
        {
            var performer = Guid.NewGuid().ToString().Replace(_configurationModel.PerformerTitleDelimiter, string.Empty);
            var title = Guid.NewGuid().ToString().Replace(_configurationModel.PerformerTitleDelimiter, string.Empty);
            var track = _mainWindowModel.TrackModels.First();
            var oldPerformer = track.ModifiedPerformer;
            Assert.AreNotEqual(oldPerformer, performer);
            track.ModifiedFileName = $"{performer} {_configurationModel.PerformerTitleDelimiter} {title}{Path.GetExtension(track.ModifiedFileName)}";
            _mainWindowModel.PerformerFromFileName(track);
            Assert.AreEqual(performer, track.ModifiedPerformer);
        }

        [TestMethod]
        public void FileNameFromTagsTest()
        {
            var performer = Guid.NewGuid().ToString().Replace(_configurationModel.PerformerTitleDelimiter, string.Empty);
            var title = Guid.NewGuid().ToString().Replace(_configurationModel.PerformerTitleDelimiter, string.Empty);
            var track = _mainWindowModel.TrackModels.First();
            var expectedFileName = $"{performer} {_configurationModel.PerformerTitleDelimiter} {title}{Path.GetExtension(track.ModifiedFileName)}";
            track.ModifiedFileName = $"_{Path.GetExtension(track.ModifiedFileName)}";
            track.ModifiedPerformer = performer;
            track.ModifiedTitle = title;
            _mainWindowModel.FileNameFromTags(track);
            Assert.AreEqual(expectedFileName, track.ModifiedFileName);
        }

        [TestMethod]
        public void DuplicatePictureAllTest()
        {
            foreach (var trackModel in _mainWindowModel.TrackModels)
            {
                trackModel.ModifiedImage = null;
            }

            var imagePath = Directory.GetFiles(_testDataImageFolder, "*.jpg").First();
            var track = _mainWindowModel.TrackModels.First();
            _mainWindowModel.SetPictureFromFile(track, imagePath).Wait();
            Assert.IsNotNull(track.ModifiedImage);
            var expectedPicture = track.ModifiedImage;
            _mainWindowModel.DuplicatePictureAll(track);

            foreach (var trackModel in _mainWindowModel.TrackModels)
            {
                Assert.AreEqual(expectedPicture, trackModel.ModifiedImage);
            }
        }

        [TestMethod]
        public void DuplicateAlbumAllTest()
        {
            foreach (var trackModel in _mainWindowModel.TrackModels)
            {
                trackModel.ModifiedAlbum = null;
            }

            var expectedAlbum = "ExpectedAlbum";
            var track = _mainWindowModel.TrackModels.First();
            track.ModifiedAlbum = expectedAlbum;
            _mainWindowModel.DuplicateAlbumAll(track);

            foreach (var trackModel in _mainWindowModel.TrackModels)
            {
                Assert.AreEqual(expectedAlbum, trackModel.ModifiedAlbum);
            }
        }

        [TestMethod]
        public void DuplicatePerformerAllTest()
        {
            foreach (var trackModel in _mainWindowModel.TrackModels)
            {
                trackModel.ModifiedPerformer = null;
            }

            var expectedPerformer = "ExpectedPerformer";
            var track = _mainWindowModel.TrackModels.First();
            track.ModifiedPerformer = expectedPerformer;
            _mainWindowModel.DuplicatePerformerAll(track);

            foreach (var trackModel in _mainWindowModel.TrackModels)
            {
                Assert.AreEqual(expectedPerformer, trackModel.ModifiedPerformer);
            }
        }

        [TestMethod]
        public void DuplicateTitleAllTest()
        {
            foreach (var trackModel in _mainWindowModel.TrackModels)
            {
                trackModel.ModifiedTitle = null;
            }

            var expectedTitle = "ExpectedTitle";
            var track = _mainWindowModel.TrackModels.First();
            track.ModifiedTitle = expectedTitle;
            _mainWindowModel.DuplicateTitleAll(track);

            foreach (var trackModel in _mainWindowModel.TrackModels)
            {
                Assert.AreEqual(expectedTitle, trackModel.ModifiedTitle);
            }
        }

        [TestMethod]
        public void ProcessingTracksTest()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var md5Hash = MD5.Create();

            var tracksHashesOriginal = _mainWindowModel.TrackModels.Select(x =>
            {
                using (var fs = new FileStream(Path.Combine(_testDataMusicFolder, x.OriginalFileName), FileMode.Open))
                {
                    return BitConverter.ToString(md5Hash.ComputeHash(fs));
                }
            }).ToArray();

            var imagePath = Directory.GetFiles(_testDataImageFolder, "*.jpg").First();
            var track = _mainWindowModel.TrackModels.First();
            track.ModifiedAlbum = Guid.NewGuid().ToString().Replace(_configurationModel.PerformerTitleDelimiter, string.Empty);
            track.ModifiedPerformer = Guid.NewGuid().ToString().Replace(_configurationModel.PerformerTitleDelimiter, string.Empty);
            track.ModifiedTitle = Guid.NewGuid().ToString().Replace(_configurationModel.PerformerTitleDelimiter, string.Empty);
            _mainWindowModel.SetPictureFromFile(track, imagePath).Wait();
            _mainWindowModel.DuplicatePictureAll(track);
            _mainWindowModel.DuplicateAlbumAll(track);
            _mainWindowModel.DuplicatePerformerAll(track);
            _mainWindowModel.DuplicateTitleAll(track);

            var processingStartedInvoked = false;
            var pocessingFinishedInvoked = false;
            var progressChangedInvoked = false;

            _mainWindowModel.ProcessingStarted = () => { processingStartedInvoked = true; };
            _mainWindowModel.ProcessingFinished = () => { pocessingFinishedInvoked = true; };
            _mainWindowModel.ProgressChanged = (_, __) => { progressChangedInvoked = true; };

            _mainWindowModel.Start(cancellationTokenSource.Token);

            Assert.IsTrue(processingStartedInvoked);
            Assert.IsTrue(pocessingFinishedInvoked);
            Assert.IsTrue(progressChangedInvoked);

            var tracksHashesModified = _mainWindowModel.TrackModels.Select(x =>
            {
                using (var fs = new FileStream(Path.Combine(_testDataMusicFolder, x.ModifiedFileName), FileMode.Open))
                {
                    return BitConverter.ToString(md5Hash.ComputeHash(fs));
                }
            }).ToArray();

            CollectionAssert.AreNotEqual(tracksHashesOriginal, tracksHashesModified, "The track was not processed. Hashes are equal.");
        }
    }
}
