using System.IO.Abstractions;

namespace TreeSizeApp.Services.Interfaces
{
    public interface IDirectoryService
    {
        IDirectoryInfo[] GetDirectories(string directoryInfo);
        IFileInfo[] GetFiles(string directoryInfo);
    }
}