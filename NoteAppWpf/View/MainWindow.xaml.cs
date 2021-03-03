using System;
using System.ComponentModel;
using System.Windows;
using NoteAppWpf.ViewModel;

namespace NoteAppWpf.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //TODO: Combobox logic (done(?))
        //TODO: Exit menu button
        //TODO: MessageBoxes with are you sure stuff
        //TODO: XML (done(?))
        public MainWindow()
        {
            InitializeComponent();
            ApplicationViewModel viewModel = new ApplicationViewModel();
            DataContext = viewModel;
            Closing += viewModel.OnWindowClosing;
        }
    }
}
