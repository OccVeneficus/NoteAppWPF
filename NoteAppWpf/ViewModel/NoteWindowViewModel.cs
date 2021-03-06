using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NoteApp;
using NoteApp.Annotations;
using NoteAppWpf.View;
using GalaSoft.MvvmLight;

namespace NoteAppWpf.ViewModel
{
    public class NoteWindowViewModel : ViewModelNotifyDataError,INotifyPropertyChanged
    {
        private Window _window;

        private Note _note;

        public bool DialogResult { get; private set; }

        public Note Note { get; set; }

        private string _newNoteTitle;

        public string NewNoteTitle
        {
            get
            {
                return _newNoteTitle;
            }
            set
            {
                if (_newNoteTitle != value)
                {
                    _newNoteTitle = value;
                    RaisePropertyChanged(nameof(NewNoteTitle));
                    ValidateProperty(nameof(NewNoteTitle));
                }
            }
        }

        private readonly List<NoteCategory> _noteCategories = Enum.GetValues(typeof(NoteCategory)).Cast<NoteCategory>().ToList();

        /// <summary>
        /// Категории, доступные для выбора в качестве категории заметки
        /// </summary>
        public List<NoteCategory> NoteCategories
        {
            get
            {
                _noteCategories.Remove(NoteCategory.All);
                return _noteCategories;
            }
        }

        public NoteWindowViewModel(Note note)
        {
            Note = note;
            NewNoteTitle = note.Name;
            _window = new NoteWindow(this);
            _window.ShowDialog();
            note.Name = NewNoteTitle;
            if (_window.DialogResult != null)
            {
                DialogResult = (bool) _window.DialogResult;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        private ICommand _okCommand = null;

        public ICommand OkCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    _okCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(
                        () =>
                        {
                            ValidateProperty(nameof(NewNoteTitle));
                        });
                }

                return _okCommand;
            }
        }

        protected override IEnumerable<string> ValidatePropertySpecialized(string propertyName)
        {
            List<string> errorMessages = new List<string>();
            if (!string.IsNullOrEmpty(propertyName))
            {
                object propertyValue =
                    this.GetType()
                        .GetProperty(propertyName)
                        .GetValue(this, null);
                if (propertyValue == null ||
                    propertyValue.ToString()
                        .Trim()
                        .Equals(string.Empty))
                {
                    errorMessages.Add(string.Format("{0} is required.", propertyName));
                }
            }

            if (propertyName == nameof(NewNoteTitle))
            {
                try
                {
                    Note.Name = NewNoteTitle;
                }
                catch (ArgumentException e)
                {
                    errorMessages.Add("Wrong note title size: it must be less than 50 characters.");
                }
            }

            return errorMessages.Count > 0 ?
                errorMessages.AsEnumerable<string>() : null;
        }
    }
}
