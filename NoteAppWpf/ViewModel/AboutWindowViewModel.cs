using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using GalaSoft.MvvmLight;
using NoteApp.Annotations;

namespace NoteAppWpf.ViewModel
{
    /// <summary>
    /// View Model окна About
    /// </summary>
    public class AboutWindowViewModel : ViewModelBase,INotifyPropertyChanged
    {
        private IWindowServise _windowServise;

        public AboutWindowViewModel(IWindowServise windowServise)
        {
            _windowServise = windowServise;
            _windowServise.ShowDialog("AboutWindow",this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
