using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using NoteApp;
using NoteAppWpf.Services.MessageBoxServices;
using NoteAppWpf.Services.WindowServices;

namespace NoteAppWpf.ViewModel
{
    /// <summary>
    /// View Model главного окна
    /// </summary>
    public class MainVM : ViewModelBase
    {
        #region Константы

        /// <summary>
        /// Лист категорий для ComboBox
        /// </summary>
        private readonly List<NoteCategory> _noteCategories =
            Enum.GetValues(typeof(NoteCategory)).Cast<NoteCategory>().ToList();

        private readonly IWindowServise _windowServise;

        private readonly IMessageBoxServise _messageBoxServise;

        #endregion

        #region Поля

        /// <summary>
        /// Поле для хранения проекта
        /// </summary>
        private readonly Project _project = ProjectManager.LoadFromFile(ProjectManager.DefaultFilePath);

        /// <summary>
        /// Выбранная заметка
        /// </summary>
        private Note _selectedNote;

        /// <summary>
        /// VM для окна About
        /// </summary>
        private AboutWindowVM _aboutWindowVm;

        /// <summary>
        /// VM для окна Note
        /// </summary>
        private NoteWindowVM _noteWindowVm;

        private ObservableCollection<Note> _selectedNotes;

        private RelayCommand _removeCommand;

        private RelayCommand _addCommand;

        private RelayCommand _editCommand;

        private RelayCommand _aboutWindowCommand;

        private NoteCategory _selectedCategory;

        #endregion

        #region Свойства

        /// <summary>
        /// Свойство для доступа к категориям комбобокса
        /// </summary>
        public List<NoteCategory> NoteCategories => _noteCategories;

        /// <summary>
        /// Заметки выбранной категории
        /// </summary>
        public ObservableCollection<Note> SelectedNotes
        {
            get => _selectedNotes;
            set => Set(ref _selectedNotes, value);
        }

        public Note SelectedNote
        {
            get => _selectedNote;
            set => Set(ref _selectedNote, value);
        }

        /// <summary>
        /// Свойство для хранения выбранной категории в ComboBox
        /// </summary>
        public NoteCategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (value != NoteCategory.All)
                {
                    SelectedNotes = _project.SortNotesByModifiedDate(_project.Notes, value);
                }
                else
                {
                    SelectedNotes = _project.SortNotesByModifiedDate(_project.Notes);
                    _selectedCategory = value;
                }

                SelectedNote = SelectedNotes.Count != 0 ? SelectedNotes[0] : null;
            }
        }

        #region Команды

        /// <summary>
        /// Команда добавления новой записки
        /// </summary>
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ??
                       (_addCommand = new RelayCommand(obj =>
                           {
                               Note newNote = new Note(DateTime.Now, DateTime.Now,
                                   "New note", "", NoteCategory.Other);

                               _noteWindowVm = new NoteWindowVM
                                   (newNote, _messageBoxServise, _windowServise);

                               if (_noteWindowVm.DialogResult.Equals(true))
                               {
                                   _project.Notes.Add(newNote);
                                   SelectedCategory = _selectedCategory;
                                   ProjectManager.SaveToFile(_project, ProjectManager.DefaultFilePath);
                               }
                           }
                       ));
            }
        }
        public RelayCommand<IClosable> CloseWindowCommand { get; private set; }

        /// <summary>
        /// Команда для удаления записи
        /// </summary>
        public RelayCommand RemoveCommand
        {
            get
            {
                return _removeCommand ??
                       (_removeCommand = new RelayCommand(obj =>
                       {
                           Note note = obj as Note;
                           if (_project.Notes.Count == 0 || note == null)
                           {
                               _messageBoxServise.Show(
                                   "There is no notes to remove, or note isn't chosen.",
                                   "Remove",
                                   MyMessageBoxButton.OK,
                                   MyMessageBoxImage.Warning);
                               return;
                           }
                           if (_messageBoxServise.Show(
                               "You sure you want to delete this note?",
                               "Remove",
                               MyMessageBoxButton.YesNo,
                               MyMessageBoxImage.Warning) == false)
                           {
                               return;
                           }

                           _project.Notes.Remove(SelectedNote);

                           SelectedCategory = _selectedCategory;
                           ProjectManager.SaveToFile(_project, ProjectManager.DefaultFilePath);

                           if (_project.Notes.Count != 0)
                           {
                               SelectedNote = _project.Notes[0];
                           }
                       }));
            }
        }

        /// <summary>
        /// Команда для редактирования записи
        /// </summary>
        public RelayCommand EditCommand
        {
            get
            {
                return _editCommand ?? (
                    _editCommand = new RelayCommand(obj =>
                    {
                        Note note = obj as Note;
                        if (note == null)
                        {
                            _messageBoxServise.Show("Note isn't chosen", "Editing error",
                                MyMessageBoxButton.OK,
                                MyMessageBoxImage.Warning);
                            return;
                        }
                        Note newNote = (Note)note.Clone();
                        _noteWindowVm = new NoteWindowVM
                                (newNote, _messageBoxServise, _windowServise);
                        if (_noteWindowVm.DialogResult.Equals(true))
                        {
                            SelectedNote.Name = _noteWindowVm.Note.Name;
                            SelectedNote.Text = _noteWindowVm.Note.Text;
                            SelectedNote.ModifiedDate = DateTime.Now;
                            SelectedNote.Category = _noteWindowVm.Note.Category;
                            _project.Notes = _project.SortNotesByModifiedDate(_project.Notes);
                            SelectedCategory = _selectedCategory;
                            ProjectManager.SaveToFile(_project, ProjectManager.DefaultFilePath);
                        }
                    }));
            }
        }

        /// <summary>
        /// Команда для показа окна "О программе"
        /// </summary>
        public RelayCommand AboutWindowCommand
        {
            get
            {
                return _aboutWindowCommand ?? (
                    _aboutWindowCommand = new RelayCommand(obj =>
                    {
                        _aboutWindowVm = new AboutWindowVM(_windowServise);
                    }));
            }
        }

        #endregion

        #endregion

        #region Конструкторы

        public MainVM(IMessageBoxServise messageBoxServise, IWindowServise windowServise)
        {
            _windowServise = windowServise;
            _messageBoxServise = messageBoxServise;
        }

        #endregion

        #region Приватные методы

        /// <summary>
        /// Метод для закрытия главного окна
        /// </summary>
        /// <param name="window">окно для закрытия</param>
        private void CloseWindow(IClosable window)
        {
            window?.Close();
        }

        #endregion

        #region Публичные методы

        /// <summary>
        /// Обработчик закрытия приложения
        /// </summary>
        /// <param name="sender">главное окно приложения</param>
        /// <param name="e">аргументы</param>
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            ProjectManager.SaveToFile(_project, ProjectManager.DefaultFilePath);
        }

        /// <summary>
        /// Обработчик события загрузки приложения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        public void OnWindowLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _project.Notes = _project.SortNotesByModifiedDate(_project.Notes);
            SelectedNote = _project.Notes.Count == 0 ? null :_project.Notes[0];
            SelectedCategory = NoteCategory.All;
            CloseWindowCommand = new RelayCommand<IClosable>(CloseWindow);
        }

        #endregion
    }
}
