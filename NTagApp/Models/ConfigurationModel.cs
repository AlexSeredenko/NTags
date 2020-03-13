using System.Collections.Generic;
using NTag.Interfaces;

namespace NTagApp.Models
{
    internal class ConfigurationModel : IConfiguration
    {
        public IEnumerable<string> SupportedFormats { get; set; }
        public IEnumerable<string> SupportedImageFormats { get; set; }

        public ConfigurationModel()
        {
            SupportedFormats = new string[] { ".mp3" };
            SupportedImageFormats = new string[] {".png", ".jpg", ".bmp" };
        }
    }
}
