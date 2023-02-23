﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TreeSizeApp.Commands;
using TreeSizeApp.Model;
using TreeSizeApp.ViewModel.Base;
using System.IO.Abstractions;
using TreeSizeApp.Services.Interfaces;

namespace TreeSizeApp.ViewModel
{
    public class NodeViewModel : BaseViewModel
    {
        #region Commands

        #region StartScanningCommand

        public ICommand StartScanningCommand { get; }

        private async void OnStartScanningCommandExecuted(object parameter)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;
            IsNotScanning = false;
            IsRefreshAllowed = false;

            ArgumentNullException.ThrowIfNull(SelectedItem);
            await Task.Run(() => LoadDirectoryDataAsync(new DirectoryInfo(SelectedItem.ToString()), cancellationToken));
        }

        private bool CanStartScanningCommandExecuted(object parameter) => true;
        #endregion

        #region CancelScanningCommand

        public ICommand CancelScanningCommand { get; }

        private void OnCancelScanningCommandExecuted(object parameter)
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
            }
            IsNotScanning = true;
            IsRefreshAllowed = true;
        }

        private bool CanCancelScanningCommandExecuted(object parameter) => true;
        #endregion

        #endregion

        #region Fields
        private const int MaxProgressVolume = 100;
        private const string FolderIcon = @"\Images\folder_icon.png";
        private const string FileIcon = @"\Images\file_icon.png";
        private const string DriveIcon = @"\Images\hdd_icon.png";
        private ObservableCollection<Node>? _nodes;
        private long totalNodesSize;
        private long currentSize;
        private double _currentProgress;
        private DriveInfo _selectedItem;
        private DirectoryInfo _rootDir;
        private bool _isNotScanning = true;
        private bool _isRefreshAllowed;
        private double _refreshOpacityValue = 0.5;
        private double _stopOpacityValue = 0.5;
        private CancellationTokenSource _cancellationTokenSource;
        ISizeConverter _sizeConverter;
        private IDirectoryService _directoryService;

        #endregion

        #region Properties
        public double CurrentProgress
        {
            get => _currentProgress;
            set
            {
                _currentProgress = value;
                OnPropertyChanged(nameof(CurrentProgress));
            }
        }

        public bool IsNotScanning
        {
            get => _isNotScanning;
            set
            {
                _isNotScanning = value;
                if (_isNotScanning)
                {
                    StopOpacityValue = 0.5;
                }
                else
                {
                    StopOpacityValue = 1.0;
                }
                OnPropertyChanged(nameof(IsNotScanning));
            }
        }

        public bool IsRefreshAllowed
        {
            get => _isRefreshAllowed;
            set
            {
                _isRefreshAllowed = IsNotScanning && (SelectedItem != null);
                if (_isRefreshAllowed)
                {
                    RefreshOpacityValue = 1.0;
                }
                else
                {
                    RefreshOpacityValue = 0.5;
                }
                OnPropertyChanged(nameof(IsRefreshAllowed));
            }
        }

        public double RefreshOpacityValue
        {
            get { return _refreshOpacityValue; }
            set
            {
                _refreshOpacityValue = value;
                OnPropertyChanged(nameof(RefreshOpacityValue));
            }
        }

        public double StopOpacityValue
        {
            get { return _stopOpacityValue; }
            set
            {
                _stopOpacityValue = value;
                OnPropertyChanged(nameof(StopOpacityValue));
            }
        }

        public List<DriveInfo> Drives { get; } = DriveInfo.GetDrives().ToList();

        public DriveInfo SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public ObservableCollection<Node>? Nodes
        {
            get => _nodes;
            set
            {
                _nodes = value;
                OnPropertyChanged(nameof(Nodes));
            }
        }
        #endregion

        public NodeViewModel(IDirectoryService directoryService, ISizeConverter sizeConverter)
        {
            _directoryService = directoryService;
            _sizeConverter = sizeConverter;

            StartScanningCommand = new RelayCommand(OnStartScanningCommandExecuted, CanStartScanningCommandExecuted);
            CancelScanningCommand = new RelayCommand(OnCancelScanningCommandExecuted, CanCancelScanningCommandExecuted);
        }

        public async Task LoadDirectoryDataAsync(DirectoryInfo rootDir, CancellationToken cancellationToken)
        {
            _rootDir = rootDir;
            CurrentProgress = 0;
            currentSize = 0;

            DriveInfo di = new(_rootDir.ToString());
            totalNodesSize = di.TotalSize - di.TotalFreeSpace;

            _nodes = new ObservableCollection<Node>();

            Node rootNode = new Node
            {
                Name = _rootDir.Name,
                Nodes = new ObservableCollection<Node>(),
                Icon = DriveIcon,
                IsExpanded = true
            };
            Nodes.Add(rootNode);

            await GetFoldersAndFilesAsync(rootNode, _rootDir.Name, cancellationToken);
            if (!cancellationToken.IsCancellationRequested)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    CurrentProgress = MaxProgressVolume;
                    IsNotScanning = true;
                    IsRefreshAllowed = true;
                    OnPropertyChanged(nameof(Nodes));
                });
            }
        }

        private async Task GetFoldersAndFilesAsync(Node parentNode, string parentDirectory, CancellationToken cancellationToken)
        {
            IFileInfo[]? files = Array.Empty<IFileInfo>();
            IDirectoryInfo[]? subdirectories = Array.Empty<IDirectoryInfo>();

            try
            {
                subdirectories = _directoryService.GetDirectories(parentDirectory);
                foreach (var subdirectory in subdirectories)
                {
                    Node node = new()
                    {
                        Name = subdirectory.Name,
                        Nodes = new ObservableCollection<Node>(),
                        Icon = FolderIcon,
                        IsExpanded = false
                    };
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        parentNode.Nodes.Add(node);
                    });

                    await GetFoldersAndFilesAsync(node, subdirectory.FullName.ToString(), cancellationToken);

                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        parentNode.Size += node.Size;
                        parentNode.FileCount += node.FileCount;
                        parentNode.FolderCount += node.FolderCount;
                        parentNode.SutableSize = _sizeConverter.Convert(parentNode.Size);
                    });
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    parentNode.FolderCount += subdirectories.Length;
                    OnPropertyChanged(nameof(Nodes));
                });

            }
            catch (UnauthorizedAccessException) { }
            catch (DirectoryNotFoundException) { }

            try
            {
                files = _directoryService.GetFiles(parentDirectory);
                foreach (var file in files)
                {
                    Node fileNode = new()
                    {
                        Name = file.Name,
                        Size = file.Length,
                        SutableSize = _sizeConverter.Convert(file.Length),
                        Icon = FileIcon,
                        FileCount = 1
                    };
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        parentNode.Nodes.Add(fileNode);
                        parentNode.Size += file.Length;
                        parentNode.SutableSize = _sizeConverter.Convert(parentNode.Size);

                        currentSize += file.Length;
                        GetProgress();
                    });
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    parentNode.FileCount += files.Length;
                    OnPropertyChanged(nameof(Nodes));
                });
            }
            catch (UnauthorizedAccessException) { }
            catch (FileNotFoundException) { }
        }

        private void GetProgress()
        {
            if (totalNodesSize != 0)
            {
                CurrentProgress = (double)currentSize / (double)totalNodesSize * 100;
            }
        }
    }
}

