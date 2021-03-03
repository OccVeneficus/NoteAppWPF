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

namespace NoteAppViewModel
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private Project _project;

        private readonly List<NoteCategory> _noteCategories = Enum.GetValues(typeof(NoteCategory)).Cast<NoteCategory>().ToList();

        private AboutWindowViewModel _aboutWindowViewModel;

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
            //Project = ProjectManager.LoadFromFile(ProjectManager.DefaultFilePath);
            Project = new Project
            {
                Notes = new ObservableCollection<Note>
                {
                    new Note(new DateTime(2001, 01,01), new DateTime(2001, 02,03), "Test1","test1", NoteCategory.Home),
                    new Note(new DateTime(2002, 02,02), new DateTime(2001, 03,04), "Test2","test2", NoteCategory.HealthAndSport),
                    new Note(new DateTime(2003, 03,03), new DateTime(2001, 04,05), "Test3","test3", NoteCategory.Documents),
                    new Note(new DateTime(2004, 04,04), new DateTime(2001, 05,06), "Test4","test4", NoteCategory.Other)
                }
            };
        }

        private RelayCommand _removeCommand;

        public RelayCommand RemoveCommand
        {
            get
            {
                return _removeCommand ??
                       (_removeCommand = new RelayCommand(obj =>
                       {
                           _project.Notes.Remove(_project.CurrentNote);
                           if (_project.Notes.Count != 0)
                           {
                               _project.CurrentNote = _project.Notes[0];
                           }
                       }));
            }
        }

        private RelayCommand _editCommand;

        public RelayCommand EditCommand
        {
            get
            {
                return _editCommand ??
                       (_editCommand = new RelayCommand(obj =>
                       {
                           _aboutWindowViewModel = new AboutWindowViewModel();
                       }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
