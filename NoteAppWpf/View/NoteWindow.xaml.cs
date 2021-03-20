using GalaSoft.MvvmLight;
using NoteAppWpf.ViewModel;

namespace NoteAppWpf.View
{
    /// <summary>
    /// Interaction logic for NoteWindow.xaml
    /// </summary>
    public partial class NoteWindow
    {
        public NoteWindow(ViewModelBase viewModel)
        {
            DataContext = (NoteWindowVM)viewModel;
            InitializeComponent();
        }
    }
}
