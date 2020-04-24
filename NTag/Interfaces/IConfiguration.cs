using System.Windows;
using System.Collections.Generic;

namespace NTag.Interfaces
{
    public interface IConfiguration
    {
        public IEnumerable<string> SupportedFormats { get; set; }
        public IEnumerable<string> SupportedImageFormats { get; set; }
        public IEnumerable<char> AllowedChars { get; set; }
        public Size AllowedTagImageSize { get; set; }
        public string PerformerTitleDelimiter { get; set; }
    }
}
