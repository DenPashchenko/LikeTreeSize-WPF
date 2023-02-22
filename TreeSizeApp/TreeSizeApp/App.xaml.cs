using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TreeSizeApp.View;
using TreeSizeApp.Services;
using SizeConverter = TreeSizeApp.Services.SizeConverter;
using TreeSizeApp.Services.Interfaces;

namespace TreeSizeApp
{
    public partial class App : Application
    {
        private ServiceProvider serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddTransient<ISizeConverter, SizeConverter>();
            services.AddTransient<IDirectoryService, DirectoryService>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = serviceProvider.GetService<MainWindow>();
            mainWindow!.Show();
        }
    }
}
