using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using NoteApp.Properties;

namespace NoteApp
{
    /// <summary>
    /// Класс, содержащий коллекцию всех созданных заметок
    /// </summary>
    public class Project : INotifyPropertyChanged
    {
        private ObservableCollection<Note> _notes = new ObservableCollection<Note>();
        /// <summary>
        /// Доступ к коллекции записок
        /// </summary>
        public ObservableCollection<Note> Notes {
            get => _notes;
            set
            {
                _notes = value;
                OnPropertyChanged(nameof(Notes));
            }
        }


        private Note _currentNote;
        /// <summary>
        /// Свойство для хранения текущей выбранной заметки
        /// </summary>
        public Note CurrentNote 
        {
            get => _currentNote;
            set
            {
                _currentNote = value;
                OnPropertyChanged(nameof(CurrentNote));
            }
        }

        /// <summary>
        /// Сортировка заметок по убыванию даты последнего изменения
        /// </summary>
        /// <param name="notes">Сортируемый список заметок</param>
        /// <returns>Возвращает отсортированный список заметок</returns>
        public ObservableCollection<Note> SortNotesByModifiedDate(ObservableCollection<Note> notes)
        {
            return new ObservableCollection<Note>(notes.OrderByDescending(note => note.ModifiedDate).ToList());
        }


        /// <summary>
        /// Сортировка по убыванию даты последнего изменения с выборкой по категории
        /// </summary>
        /// <param name="notes">Сортируемый список заметок</param>
        /// <param name="category">Категория выборки заметок</param>
        /// <returns>Возвращает отсортированный список заметок указанной категории</returns>
        public ObservableCollection<Note> SortNotesByModifiedDate(ObservableCollection<Note> notes, NoteCategory category)
        {
            ObservableCollection<Note> categoryNotes = new ObservableCollection<Note>();
            foreach (var note in notes)
            {
                if (note.Category == category)
                {
                    categoryNotes.Add(note);
                }
            }

            return SortNotesByModifiedDate(categoryNotes);
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
