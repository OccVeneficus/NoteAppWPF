using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using GalaSoft.MvvmLight.CommandWpf;
using NoteApp;
using NoteApp.Properties;
using NoteAppWpf.Services.MessageBoxServices;
using NoteAppWpf.Services.WindowServices;

namespace NoteAppWpf.ViewModel
{
    /// <summary>
    /// View Model главного окна
    /// </summary>
    public class MainVM : INotifyPropertyChanged
    {
        /// <summary>
        /// Поле для хранения проекта
        /// </summary>
        private Project _project;

        /// <summary>
        /// Лист категорий для ComboBox
        /// </summary>
        private readonly List<NoteCategory> _noteCategories =
            Enum.GetValues(typeof(NoteCategory)).Cast<NoteCategory>().ToList();

        /// <summary>
        /// Свойство для доступа к категориям комбобокса
        /// </summary>
        public  List<NoteCategory> NoteCategories => _noteCategories;

        /// <summary>
        /// VM для окна About
        /// </summary>
        private AboutWindowVM _aboutWindowVm;

        /// <summary>
        /// VM для окна Note
        /// </summary>
        private NoteWindowVM _noteWindowVm;

        private ObservableCollection<Note> _selectedNotes;

        /// <summary>
        /// Заметки выбранной категории
        /// </summary>
        public ObservableCollection<Note> SelectedNotes
        {
            get => _selectedNotes;
            set
            {
                _selectedNotes = value;
                OnPropertyChanged(nameof(SelectedNotes));
            }
        }

        private Note _selectedNote;

        public Note SelectedNote
        {
            get
            {
                return _selectedNote;
                var note = _selectedNotes.FirstOrDefault(x => x.Equals(_selectedNote));
                return note;
            }
            set 
            {
                //var note = _selectedNotes.FirstOrDefault(x => x.Equals(value));
                _selectedNote = value;             
                OnPropertyChanged(nameof(SelectedNote));
            }
        }

        /// <summary>
        /// Свойство для хранения экземпляра проекта
        /// </summary>
        public Project Project
        {
            get => _project;
            set
            {
                _project = value;
                OnPropertyChanged(nameof(Project));
            }
        }

        private readonly IWindowServise _windowServise;

        private readonly IMessageBoxServise _messageBoxServise;
        public RelayCommand<IClosable> CloseWindowCommand { get; private set; }

        public MainVM(IMessageBoxServise messageBoxServise, IWindowServise windowServise, Project project)
        {
            _windowServise = windowServise;
            _messageBoxServise = messageBoxServise;
            Project = project;
            Project.Notes = Project.SortNotesByModifiedDate(Project.Notes);
            SelectedCategory = NoteCategory.All;
            CloseWindowCommand = new RelayCommand<IClosable>(CloseWindow);
        }

        /// <summary>
        /// Метод для закрытия главного окна
        /// </summary>
        /// <param name="window">окно для закрытия</param>
        private void CloseWindow(IClosable window)
        {
            window?.Close();
        }

        private NoteCategory _selectedCategory;

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
                    SelectedNotes = Project.SortNotesByModifiedDate(Project.Notes, value);
                }
                else
                {
                    SelectedNotes = Project.SortNotesByModifiedDate(Project.Notes);
                    _selectedCategory = value;
                }
            }
        }

        private RelayCommand _addCommand;

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
                                   "New note","",NoteCategory.Other);

                               _noteWindowVm = new NoteWindowVM
                                   (newNote, _messageBoxServise, _windowServise);

                               if (_noteWindowVm.DialogResult.Equals(true))
                               {
                                   _project.Notes.Add(newNote);
                                   SelectedCategory = _selectedCategory;
                                   ProjectManager.SaveToFile(Project, ProjectManager.DefaultFilePath);
                               }
                           }
                       ));
            }
        }

        private readonly Dictionary<MyMessageBoxButton, MessageBoxButton> _messageBoxButtons =
            new Dictionary<MyMessageBoxButton, MessageBoxButton>
            {
                {MyMessageBoxButton.OK, MessageBoxButton.OK},
                {MyMessageBoxButton.YesNo, MessageBoxButton.YesNo}
            };

        private readonly Dictionary<MyMessageBoxImage, MessageBoxImage> _messageBoxImages =
            new Dictionary<MyMessageBoxImage, MessageBoxImage>
            {
                {MyMessageBoxImage.Warning, MessageBoxImage.Warning}
            };

        private RelayCommand _removeCommand;

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
                                   _messageBoxButtons[MyMessageBoxButton.OK],
                                   _messageBoxImages[MyMessageBoxImage.Warning]);
                               return;
                           }
                           if (_messageBoxServise.Show(
                               "You sure you want to delete this note?",
                               "Remove",
                               _messageBoxButtons[MyMessageBoxButton.YesNo],
                               _messageBoxImages[MyMessageBoxImage.Warning]) == false)
                           {
                               return;
                           }
                           if (note != null)
                           {
                               _project.Notes.Remove(note);
                               SelectedCategory = _selectedCategory;
                               ProjectManager.SaveToFile(Project, ProjectManager.DefaultFilePath);
                           }

                           if (_project.Notes.Count != 0)
                           {
                               Project.CurrentNote = _project.Notes[0];
                           }
                       }));
            }
        }

        private RelayCommand _editCommand;

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
                        if (note != null)
                        {
                            Note newNote = (Note)note.Clone();
                            _noteWindowVm = new NoteWindowVM
                                (newNote, _messageBoxServise, _windowServise);
                        }
                        else
                        {
                            _messageBoxServise.Show("Note isn't chosen", "Editing error",
                                _messageBoxButtons[MyMessageBoxButton.OK],
                                _messageBoxImages[MyMessageBoxImage.Warning]);
                            return;
                        }
                        if (_noteWindowVm.DialogResult.Equals(true))
                        {
                            _project.CurrentNote.Name = _noteWindowVm.Note.Name;
                            _project.CurrentNote.Text = _noteWindowVm.Note.Text;
                            _project.CurrentNote.ModifiedDate = DateTime.Now;
                            _project.CurrentNote.Category = _noteWindowVm.Note.Category;
                            Project.Notes = Project.SortNotesByModifiedDate(Project.Notes);
                            SelectedCategory = _selectedCategory;
                            ProjectManager.SaveToFile(Project,ProjectManager.DefaultFilePath);
                        }
                    }));
            }
        }

        private RelayCommand _aboutWindowCommand;

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

        /// <summary>
        /// Обработчик закрытия приложения
        /// </summary>
        /// <param name="sender">главное окно приложения</param>
        /// <param name="e">аргументы</param>
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            ProjectManager.SaveToFile(Project, ProjectManager.DefaultFilePath);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
