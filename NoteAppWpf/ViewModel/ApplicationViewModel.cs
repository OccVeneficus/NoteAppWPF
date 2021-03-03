using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NoteApp;
using NoteApp.Annotations;

namespace NoteAppWpf.ViewModel
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private Project _project;

        private readonly List<NoteCategory> _noteCategories = Enum.GetValues(typeof(NoteCategory)).Cast<NoteCategory>().ToList();

        private AboutWindowViewModel _aboutWindowViewModel;
        private NoteWindowViewModel _noteWindowViewModel;

        public  List<NoteCategory> NoteCategories => _noteCategories;

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

        public ApplicationViewModel()
        {
            Project = ProjectManager.LoadFromFile(ProjectManager.DefaultFilePath);
            Project.Notes = Project.SortNotesByModifiedDate(Project.Notes);
        }

        private RelayCommand _addCommand;

        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ??
                       (_addCommand = new RelayCommand(obj =>
                           {
                               Note newNote = new Note(DateTime.Now, DateTime.Now, "New note","",NoteCategory.Other);
                               _noteWindowViewModel = new NoteWindowViewModel(newNote);
                               if (_noteWindowViewModel.DialogResult.Equals(true))
                               {
                                   _project.Notes.Add(newNote);
                                   Project.Notes = Project.SortNotesByModifiedDate(Project.Notes);
                                   ProjectManager.SaveToFile(Project, ProjectManager.DefaultFilePath);
                               }
                           }
                       ));
            }
        }

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
                               Project.Notes = Project.SortNotesByModifiedDate(Project.Notes);
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
                            _noteWindowViewModel = new NoteWindowViewModel(newNote);
                        }

                        if (_noteWindowViewModel.DialogResult.Equals(true))
                        {
                            _project.CurrentNote.Name = _noteWindowViewModel.Note.Name;
                            _project.CurrentNote.Text = _noteWindowViewModel.Note.Text;
                            _project.CurrentNote.ModifiedDate = DateTime.Now;
                            _project.CurrentNote.Category = _noteWindowViewModel.Note.Category;
                            Project.Notes = Project.SortNotesByModifiedDate(Project.Notes);
                            ProjectManager.SaveToFile(Project,ProjectManager.DefaultFilePath);
                        }
                    }));
            }
        }

        private RelayCommand _aboutWindowCommand;

        public RelayCommand AboutWindowCommand
        {
            get
            {
                return _aboutWindowCommand ?? (
                    _aboutWindowCommand = new RelayCommand(obj =>
                    {
                        _aboutWindowViewModel = new AboutWindowViewModel();
                    }));
            }
        }

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
