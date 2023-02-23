using System.IO;
using System.IO.Abstractions;
using TreeSizeApp.Services.Interfaces;

namespace TreeSizeApp.Services
{
    public class DirectoryService : IDirectoryService
    {
        private readonly IFileSystem _fileSystem;

        public DirectoryService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public IFileInfo[] GetFiles(string directoryInfo)
        {
            return _fileSystem.DirectoryInfo.New(directoryInfo).GetFiles();
        }

        public IDirectoryInfo[] GetDirectories(string directoryInfo)
        {
            return _fileSystem.DirectoryInfo.New(directoryInfo).GetDirectories();
        }
    }
}