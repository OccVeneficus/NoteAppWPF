using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NoteApp.Annotations;
using NoteAppWpf.View;

namespace NoteAppWpf.ViewModel
{
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
