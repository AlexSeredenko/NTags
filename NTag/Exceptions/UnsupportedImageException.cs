using System;

namespace NTag.Exceptions
{
    public class UnsupportedImageSizeException : ApplicationException
    {
        public UnsupportedImageSizeException(string message) : base(message)
        {
        }
    }
}
