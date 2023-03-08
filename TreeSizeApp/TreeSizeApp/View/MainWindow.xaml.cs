using System.Windows;
using TreeSizeApp.Services;
using TreeSizeApp.ViewModel;
using FileSystem = System.IO.Abstractions.FileSystem;
using SizeConverter = TreeSizeApp.Services.SizeConverter;

namespace TreeSizeApp.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new NodeViewModel(new DirectoryService(new FileSystem()), new SizeConverter());
        }
    }
}