using System.Windows;
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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var viewModel = new MainVM(new MessageBoxServise(), new WindowServise(),
                ProjectManager.LoadFromFile(ProjectManager.DefaultFilePath));
            var mainView = new MainWindow();
            mainView.DataContext = viewModel;
            mainView.Closing += viewModel.OnWindowClosing;
            mainView.Show();
        }
    }
}
