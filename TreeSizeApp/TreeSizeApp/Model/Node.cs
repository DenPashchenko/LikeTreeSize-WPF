using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TreeSizeApp.Model
{
    public class Node : INotifyPropertyChanged
    {
        #region Fields
        private string _name;
        private long _size;
        private int _fileCount;
        private int _folderCount;
        private string _sutableSize;
        private string _icon;
        #endregion

        #region Properties
        public string? Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public long Size
        {
            get => _size;
            set
            {
                _size = value;
            }
        }

        public string SutableSize
        {
            get => _sutableSize;
            set
            {
                _sutableSize = value;
                OnPropertyChanged(nameof(SutableSize));
            }
        }

        public string Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        public int FileCount
        {
            get => _fileCount;
            set
            {
                _fileCount = value;
                OnPropertyChanged(nameof(FileCount));
            }
        }

        public int FolderCount
        {
            get => _folderCount;
            set
            {
                _folderCount = value;
                OnPropertyChanged(nameof(FolderCount));
            }
        }

        public ObservableCollection<Node>? Nodes { get; set; }
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
