using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using NoteApp.Annotations;
using NoteAppWpf.View;

namespace NoteAppWpf.ViewModel
{
    /// <summary>
    /// View Model окна About
    /// </summary>
    public class AboutWindowViewModel : INotifyPropertyChanged
    {
        private Window _aboutWindow;

        public Window AboutWindow
        {
            get;
        }

        public AboutWindowViewModel()
        {
            _aboutWindow = new About();
            _aboutWindow.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
