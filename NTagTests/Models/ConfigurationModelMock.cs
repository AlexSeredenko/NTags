using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using NTag.Interfaces;

namespace NTagTests.Models
{
    internal class ConfigurationModelMock : IConfiguration
    {
        public IEnumerable<string> SupportedFormats { get; set; }
        public IEnumerable<string> SupportedImageFormats { get; set; }
        public IEnumerable<char> AllowedChars { get; set; }
        public Size AllowedTagImageSize { get; set; }
        public string PerformerTitleDelimiter { get; set; }

        public ConfigurationModelMock()
        {
            SupportedFormats = new string[] { ".mp3" };
            SupportedImageFormats = new string[] { ".jpg" };
            AllowedChars = Enumerable.Range(Char.MinValue, Char.MaxValue - 1).Select(x => (char)x).ToArray();
            AllowedTagImageSize = new Size(200, 200);
            PerformerTitleDelimiter = "-";
    }
    }
}
