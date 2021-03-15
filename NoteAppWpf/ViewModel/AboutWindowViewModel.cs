using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using GalaSoft.MvvmLight;
using NoteApp.Properties;
using NoteAppWpf.WindowServicing;

namespace NoteAppWpf.ViewModel
{
    /// <summary>
    /// View Model окна About
    /// </summary>
    public class AboutWindowViewModel : ViewModelBase,INotifyPropertyChanged
    {
        private readonly IWindowServise _windowServise;

        public event PropertyChangedEventHandler PropertyChanged;

        public AboutWindowViewModel(IWindowServise windowServise)
        {
            _windowServise = windowServise;
            _windowServise.ShowDialog(WindowType.About,this);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
