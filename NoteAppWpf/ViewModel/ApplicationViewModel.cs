using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight.CommandWpf;
using NoteApp;
using NoteApp.Annotations;

namespace NoteAppWpf.ViewModel
{
    /// <summary>
    /// View Model главного окна
    /// </summary>
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Поле для хранения проекта
        /// </summary>
        private Project _project;

        /// <summary>
        /// Лист категорий для ComboBox
        /// </summary>
        private readonly List<NoteCategory> _noteCategories = Enum.GetValues(typeof(NoteCategory)).Cast<NoteCategory>().ToList();

        public  List<NoteCategory> NoteCategories => _noteCategories;

        public RelayCommand<IClosable> CloseWindowCommand { get; private set; }

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
            get
            {
                return _selectedNotes;
            }
            set
            {
                _selectedNotes = value;
                OnPropertyChanged(nameof(SelectedNotes));
            }
        }

        /// <summary>
        /// Свойство для хранения экземпляра проекта
        /// </summary>
        public Project Project
        {
            get
            {
                return _project;
            }
            set
            {
                _project = value;
                OnPropertyChanged(nameof(Project));
            }
        }

        private readonly IWindowServise _windowServise;

        private readonly IMessageBoxServise _messageBoxServise;

        public ApplicationViewModel(IMessageBoxServise messageBoxServise, IWindowServise windowServise)
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
            if (window != null)
            {
                window.Close();
            }
        }

        /// <summary>
        /// Поле для хранения выбранной категории в ComboBox
        /// </summary>
        private NoteCategory _selectedCategory;

        public NoteCategory SelectedCategory
        {
            get { return _selectedCategory; }
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
