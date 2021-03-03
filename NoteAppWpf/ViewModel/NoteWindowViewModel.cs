using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NoteApp;
using NoteApp.Annotations;
using NoteAppWpf.View;

namespace NoteAppWpf.ViewModel
{
    public class NoteWindowViewModel : INotifyPropertyChanged
    {
        private Window _window;

        private Note _note;

        public bool DialogResult { get; private set; }

        public Note Note { get; set; }

        private readonly List<NoteCategory> _noteCategories = Enum.GetValues(typeof(NoteCategory)).Cast<NoteCategory>().ToList();
        public List<NoteCategory> NoteCategories => _noteCategories;

        public NoteWindowViewModel(Note note)
        {
            Note oldNote = (Note)note.Clone();
            Note = note;
            _window = new NoteWindow(this);
            _window.ShowDialog();
            if (_window.DialogResult != null) DialogResult = (bool) _window.DialogResult;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
