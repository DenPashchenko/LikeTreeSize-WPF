using System.IO.Abstractions.TestingHelpers;
using TreeSizeApp.ViewModel;
using TreeSizeApp.Services;
using System.Windows;
using SizeConverter = TreeSizeApp.Services.SizeConverter;

namespace TreeSizeAppTests
{
    //This class is created so that any class under test, which access to Application.Current.Dispatcher, will find a dispatcher.

    [TestClass]
    public class ApplicationInitializer
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            var waitForApplicationRun = new TaskCompletionSource<bool>();
            Task.Run(() =>
            {
                var application = new Application();
                application.Startup += (s, e) => { waitForApplicationRun.SetResult(true); };
                application.Run();
            });
            waitForApplicationRun.Task.Wait();
        }
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
        }
    }

    [TestClass]
    public class NodeViewModelTest
    {
        MockFileSystem fileSystem = new();

        [TestMethod]
        public async Task LoadDirectoryDataAsync_RootDir_LoadedDirectoryTreeData()
        {
            var directoryService = new DirectoryService(fileSystem);
            var sizeConverter = new SizeConverter();
            var viewModel = new NodeViewModel(directoryService, sizeConverter);
            var fileContent = $"3 lines{Environment.NewLine}6 words{Environment.NewLine}26 bytes";
            var fileData = new MockFileData(fileContent);
            fileSystem.AddDirectory(@"C:\MyDir1");
            fileSystem.AddDirectory(@"C:\MyDir2");
            fileSystem.AddFile(@"C:\MyDir1\testFile1.txt", fileData);

            await viewModel.LoadDirectoryDataAsync(new DirectoryInfo("C:\\"), new CancellationTokenSource().Token);

            Assert.IsNotNull(viewModel.Nodes);
            Assert.AreEqual(1, viewModel.Nodes.Count);
            Assert.AreEqual("C:\\", viewModel.Nodes[0].Name);
            Assert.AreEqual(@"\Images\hdd_icon.png", viewModel.Nodes[0].Icon);

            Assert.AreEqual(3, viewModel.Nodes[0].Nodes.Count);
            Assert.AreEqual("temp", viewModel.Nodes[0].Nodes[0].Name);
            Assert.AreEqual(@"\Images\folder_icon.png", viewModel.Nodes[0].Nodes[0].Icon);

            Assert.AreEqual("MyDir1", viewModel.Nodes[0].Nodes[1].Name);
            Assert.AreEqual(@"\Images\folder_icon.png", viewModel.Nodes[0].Nodes[1].Icon);

            Assert.AreEqual(1, viewModel.Nodes[0].Nodes[1].Nodes.Count);
            Assert.AreEqual("testFile1.txt", viewModel.Nodes[0].Nodes[1].Nodes[0].Name);
            Assert.AreEqual(@"\Images\file_icon.png", viewModel.Nodes[0].Nodes[1].Nodes[0].Icon);
            Assert.AreEqual(26, viewModel.Nodes[0].Nodes[1].Nodes[0].Size);
            Assert.AreEqual("26 bytes", viewModel.Nodes[0].Nodes[1].Nodes[0].SutableSize);

            Assert.AreEqual(1, viewModel.Nodes[0].FileCount);
            Assert.AreEqual(3, viewModel.Nodes[0].FolderCount);
            Assert.AreEqual(26, viewModel.Nodes[0].Size);
        }
    }
}