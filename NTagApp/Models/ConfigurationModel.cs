using System.Collections.Generic;
using System.Linq;
using NTag.Interfaces;

namespace NTagApp.Models
{
    internal class ConfigurationModel : IConfiguration
    {
        public IEnumerable<string> SupportedFormats { get; set; }
        public IEnumerable<string> SupportedImageFormats { get; set; }
        public IEnumerable<char> AllowedChars { get; set; }

        public ConfigurationModel()
        {
            SupportedFormats = new string[] { ".mp3" };
            SupportedImageFormats = new string[] { ".png", ".jpg", ".bmp" };

            AllowedChars = Enumerable.Range('a', 'z' - 'a' + 1).Select(x => (char)x)
            .Union(Enumerable.Range('A', 'Z' - 'A' + 1).Select(x => (char)x))
            .Union(Enumerable.Range('0', '9' - '0' + 1).Select(x => (char)x))
            .Union(new char[] { ' ', '.', ',', ':', '-', '_', '\'', '"', '!', '+', '&', '$', '*', '#', '(', ')' })
            .ToArray();
        }
    }
}
