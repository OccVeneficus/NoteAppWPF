using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using NoteApp;
using NoteAppWpf.Services.MessageBoxServices;
using NoteAppWpf.Services.WindowServices;
using NoteAppWpf.View;
using NoteAppWpf.ViewModel;

namespace NoteAppWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Container = ConfigureDependencyInjection();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainView = new MainWindow();
            var container = Container;
            var mainVM = (MainVM)ActivatorUtilities.GetServiceOrCreateInstance(container, typeof(MainVM));
            mainVM.Project = ProjectManager.LoadFromFile(ProjectManager.DefaultFilePath);
            mainView.Closing += mainVM.OnWindowClosing;
            mainView.Loaded += mainVM.OnWindowLoaded;
            mainView.DataContext= mainVM;
            mainView.Show();
        }

        public IServiceProvider Container { get; }

        IServiceProvider ConfigureDependencyInjection()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<IWindowServise, WindowServise>();
            serviceCollection.AddTransient<IMessageBoxServise, MessageBoxServise>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
