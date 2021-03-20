using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using GalaSoft.MvvmLight;
using NoteApp.Properties;
using NoteAppWpf.Services.WindowServices;

namespace NoteAppWpf.ViewModel
{
    /// <summary>
    /// View Model окна About
    /// </summary>
    public class AboutWindowVM : ViewModelBase,INotifyPropertyChanged
    {
        private readonly IWindowServise _windowServise;

        public event PropertyChangedEventHandler PropertyChanged;

        public AboutWindowVM(IWindowServise windowServise)
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
