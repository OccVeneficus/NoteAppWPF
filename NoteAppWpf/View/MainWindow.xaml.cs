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
        public MainWindow()
        {
            InitializeComponent();
            ApplicationViewModel viewModel = new ApplicationViewModel();
            DataContext = viewModel;
            Closing += viewModel.OnWindowClosing;
        }
    }
}
