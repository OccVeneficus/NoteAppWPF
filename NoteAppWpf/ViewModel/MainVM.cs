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
using NoteAppWpf.MessageBoxServicing;
using NoteAppWpf.WindowServicing;

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
        /// VM для окна About
        /// </summary>
        private AboutWindowViewModel _aboutWindowViewModel;

        /// <summary>
        /// VM для окна Note
        /// </summary>
        private NoteWindowViewModel _noteWindowViewModel;

        private ObservableCollection<Note> _selectedNotes;

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
                var note = _selectedNotes.FirstOrDefault(x => x.Equals(_selectedNote));
                return note;
            }
            set 
            {
                var note = _selectedNotes.FirstOrDefault(x => x.Equals(value));
                _selectedNote = note;             
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

        public MainVM(IMessageBoxServise messageBoxServise, IWindowServise windowServise)
        {
            _windowServise = windowServise;
            _messageBoxServise = messageBoxServise;
            Project = ProjectManager.LoadFromFile(ProjectManager.DefaultFilePath);
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

        /// <summary>
        /// Поле для хранения выбранной категории в ComboBox
        /// </summary>
        private NoteCategory _selectedCategory;

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

        /// <summary>
        /// Команда добавления новой записки
        /// </summary>
        private RelayCommand _addCommand;

        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ??
                       (_addCommand = new RelayCommand(obj =>
                           {
                               Note newNote = new Note(DateTime.Now, DateTime.Now,
                                   "New note","",NoteCategory.Other);

                               _noteWindowViewModel = new NoteWindowViewModel
                                   (newNote, _messageBoxServise, _windowServise);

                               if (_noteWindowViewModel.DialogResult.Equals(true))
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

        /// <summary>
        /// Команда для удаления записи
        /// </summary>
        private RelayCommand _removeCommand;

        public RelayCommand RemoveCommand
        {
            get
            {
                return _removeCommand ??
                       (_removeCommand = new RelayCommand(obj =>
                       {
                           if (_messageBoxServise.Show(
                               "You sure you want to delete this note",
                               "Remove",
                               _messageBoxButtons[MyMessageBoxButton.YesNo],
                               _messageBoxImages[MyMessageBoxImage.Warning]) == false)
                           {
                               return;
                           }
                           Note note = obj as Note;
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

        /// <summary>
        /// Команда для редактирования записи
        /// </summary>
        private RelayCommand _editCommand;

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
                            _noteWindowViewModel = new NoteWindowViewModel
                                (newNote, _messageBoxServise, _windowServise);
                        }
                        else
                        {
                            _messageBoxServise.Show("Note isn't chosen", "Editing error",
                                _messageBoxButtons[MyMessageBoxButton.OK],
                                _messageBoxImages[MyMessageBoxImage.Warning]);
                            return;
                        }
                        if (_noteWindowViewModel.DialogResult.Equals(true))
                        {
                            _project.CurrentNote.Name = _noteWindowViewModel.Note.Name;
                            _project.CurrentNote.Text = _noteWindowViewModel.Note.Text;
                            _project.CurrentNote.ModifiedDate = DateTime.Now;
                            _project.CurrentNote.Category = _noteWindowViewModel.Note.Category;
                            Project.Notes = Project.SortNotesByModifiedDate(Project.Notes);
                            SelectedCategory = _selectedCategory;
                            ProjectManager.SaveToFile(Project,ProjectManager.DefaultFilePath);
                        }
                    }));
            }
        }

        /// <summary>
        /// Команда для показа окна "О программе"
        /// </summary>
        private RelayCommand _aboutWindowCommand;

        public RelayCommand AboutWindowCommand
        {
            get
            {
                return _aboutWindowCommand ?? (
                    _aboutWindowCommand = new RelayCommand(obj =>
                    {
                        _aboutWindowViewModel = new AboutWindowViewModel(_windowServise);
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
