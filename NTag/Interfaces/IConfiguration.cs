using System.Collections.Generic;

namespace NTag.Interfaces
{
    public interface IConfiguration
    {
        public IEnumerable<string> SupportedFormats { get; set; }
    }
}
