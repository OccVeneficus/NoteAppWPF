using System;
using System.ComponentModel;
using System.Windows;
using NoteAppWpf.ViewModel;

namespace NoteAppWpf.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,IClosable
    {
        //TODO: MessageBoxes "with are you sure" stuff
        //TODO: XML (done(?))
        //TODO: Validation

        public MainWindow()
        {
            InitializeComponent();
            ApplicationViewModel viewModel = new ApplicationViewModel();
            DataContext = viewModel;
            Closing += viewModel.OnWindowClosing;
        }
    }
}
