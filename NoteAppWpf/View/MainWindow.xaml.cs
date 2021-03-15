using System.Windows;
using NoteAppWpf.MessageBoxServicing;
using NoteAppWpf.ViewModel;
using NoteAppWpf.WindowServicing;

namespace NoteAppWpf.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,IClosable
    {
        public MainWindow()
        {
            InitializeComponent();
            MainVM viewModel = new MainVM(new MessageBoxServise(), new WindowServise());
            DataContext = viewModel;
            Closing += viewModel.OnWindowClosing;
        }
    }
}
