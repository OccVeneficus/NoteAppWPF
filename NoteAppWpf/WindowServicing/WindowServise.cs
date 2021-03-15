using System;
using System.Windows;
using GalaSoft.MvvmLight;
using NoteAppWpf.View;

namespace NoteAppWpf.WindowServicing
{
    public class WindowServise : IWindowServise
    {
        /// <summary>
        /// Текущее показываемое окно
        /// </summary>
        private Window _window;

        public bool? ShowDialog(WindowType windowType, ViewModelBase viewModel)
        {
            switch (windowType)
            {
                case WindowType.Note:
                {
                    _window = new NoteWindow(viewModel);
                    return _window.ShowDialog();
                }
                case WindowType.About:
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
