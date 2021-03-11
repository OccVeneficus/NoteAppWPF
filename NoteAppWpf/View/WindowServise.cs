using System;
using System.Collections.Generic;
using System.Windows;
using GalaSoft.MvvmLight;
using NoteAppWpf.ViewModel;

namespace NoteAppWpf.View
{
    public class WindowServise : IWindowServise
    {
        /// <summary>
        /// Текущее показываемое окно
        /// </summary>
        private Window _window;

        public bool? ShowDialog(string windowType, ViewModelBase viewModel)
        {
            switch (windowType)
            {
                case "NoteWindow":
                {
                    _window = new NoteWindow(viewModel);
                    return _window.ShowDialog();
                }
                case "AboutWindow":
                {
                    _window = new About();
                    return _window.ShowDialog();
                }
                default:
                {
                    throw new ArgumentException("Unexpected window type");
                }
            }
        }

        public void SetDialogResult(bool result)
        {
            _window.DialogResult = result;
        }
    }
}
