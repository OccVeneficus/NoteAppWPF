﻿using System.Windows;
using Microsoft.Extensions.DependencyInjection;
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
        }
    }
}
