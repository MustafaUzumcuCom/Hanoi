using System.IO;

namespace Hanoi
{
    public static class FilePath
    {
        private static string _directory;
        public static string Directory
        {
            get
            {
                if (!System.IO.Directory.Exists(_directory))
                    System.IO.Directory.CreateDirectory(_directory);
                return _directory;
            }
            set { _directory = value; }
        }
    }
}
