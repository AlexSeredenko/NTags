using System.Linq;
using System.Windows;
using System.Collections.Generic;
using NTag.Interfaces;

namespace NTagApp.Models
{
    internal class ConfigurationModel : IConfiguration
    {
        public IEnumerable<string> SupportedFormats { get; set; }
        public IEnumerable<string> SupportedImageFormats { get; set; }
        public IEnumerable<char> AllowedChars { get; set; }
        public Size AllowedTagImageSize { get; set; }

        public ConfigurationModel()
        {
            SupportedFormats = new string[] { ".mp3" };
            SupportedImageFormats = new string[] { ".jpg" };

            AllowedChars = Enumerable.Range('a', 'z' - 'a' + 1).Select(x => (char)x)
            .Union(Enumerable.Range('A', 'Z' - 'A' + 1).Select(x => (char)x))
            .Union(Enumerable.Range('0', '9' - '0' + 1).Select(x => (char)x))
            .Union(new char[] { ' ', '.', ',', ':', '-', '_', '\'', '"', '!', '+', '&', '$', '*', '#', '(', ')' })
            .ToArray();

            AllowedTagImageSize = new Size(200, 200);
        }
    }
}
