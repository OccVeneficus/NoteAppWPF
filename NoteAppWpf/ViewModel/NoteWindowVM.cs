using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using NoteApp;
using NoteApp.Properties;
using NoteAppWpf.Services.MessageBoxServices;
using NoteAppWpf.Services.WindowServices;

namespace NoteAppWpf.ViewModel
{
    /// <summary>
    /// VM окна редактирования заметки
    /// </summary>
    public class NoteWindowVM : ViewModelBase,INotifyDataErrorInfo,INotifyPropertyChanged
    {
        private Dictionary<MyMessageBoxButton, MessageBoxButton> _messageBoxButtons =
            new Dictionary<MyMessageBoxButton, MessageBoxButton>
        {
            {MyMessageBoxButton.OK, MessageBoxButton.OK}
        };

        private Dictionary<MyMessageBoxImage, MessageBoxImage> _messageBoxImages = 
            new Dictionary<MyMessageBoxImage, MessageBoxImage>
        {
            {MyMessageBoxImage.Warning, MessageBoxImage.Warning}
        };

        /// <summary>
        /// Словарь для хранения ошибок проверки свойств 
        /// </summary>
        private readonly Dictionary<string, List<string>> _errorsByPropertyName =
            new Dictionary<string, List<string>>();

        private Note _note;

        /// <summary>
        /// Поле для хранения результата диалогового окна
        /// </summary>
        public bool DialogResult { get; private set; }

        /// <summary>
        /// Поле для хранения редактируемой заметки
        /// </summary>
        public Note Note { get; set; }

        private string _newNoteTitle;
        /// <summary>
        /// Свойство для нового имени заметки
        /// </summary>
        public string NewNoteTitle
        {
            get => _newNoteTitle;
            set
            {
                if (_newNoteTitle != value)
                {
                    _newNoteTitle = value;
                    ValidateNoteName();
                    OnPropertyChanged(nameof(NewNoteTitle));
                }
            }
        }

        private readonly List<NoteCategory> _noteCategories = 
            Enum.GetValues(typeof(NoteCategory)).Cast<NoteCategory>().ToList();

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

        private readonly IMessageBoxServise _messageBoxServise;

        private readonly IWindowServise _windowServise;

        public NoteWindowVM(Note note, IMessageBoxServise messageBoxServise,
            IWindowServise windowServise)
        {

            Note = note;
            NewNoteTitle = note.Name;
            _messageBoxServise = messageBoxServise;
            _windowServise = windowServise;
            ShowDialogWindow();
        }

        /// <summary>
        /// Метод для отображения диалога окна
        /// </summary>
        public void ShowDialogWindow()
        {
            bool? dialogResult = _windowServise.ShowDialog(WindowType.Note, this);
            if (dialogResult != null)
            {
                DialogResult = (bool) dialogResult;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName]
            string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ICommand _cancelCommand = null;
        /// <summary>
        /// Команда для кнопки Cancel
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(
                        () =>
                        {
                            _windowServise.SetDialogResult(false);
                        });
                }

                return _cancelCommand;
            }
        }

        private RelayCommand _okCommand = null;
        /// <summary>
        /// Команда для кнопки ОК
        /// </summary>
        public RelayCommand OkCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    _okCommand = new RelayCommand(obj =>
                    {
                        ValidateNoteName();
                        if (!HasErrors)
                        {
                            _windowServise.SetDialogResult(true);
                        }
                        else
                        {
                            _messageBoxServise.Show(
                                "Wrong title size",
                                "Validation Error",
                                _messageBoxButtons[MyMessageBoxButton.OK],
                                _messageBoxImages[MyMessageBoxImage.Warning]);
                        }
                    });
                }
                return _okCommand;
            }
        }

        /// <summary>
        /// Возвращает наличие ошибок
        /// </summary>
        public bool HasErrors => _errorsByPropertyName.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Возвращает ошибки для указанного свойства
        /// </summary>
        /// <param name="propertyName">Проверяемое свойство</param>
        /// <returns></returns>
        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsByPropertyName.ContainsKey(propertyName) ?
                _errorsByPropertyName[propertyName] : null;
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Метод проверки свойства имени новой заметки
        /// </summary>
        private void ValidateNoteName()
        {
            ClearErrors(nameof(NewNoteTitle));
            try
            {
                Note.Name = NewNoteTitle;
            }
            catch (ArgumentException)
            {
                AddError(nameof(NewNoteTitle),"Wrong title length");
            }
        }

        /// <summary>
        /// Добавить ошибку в словарь
        /// </summary>
        /// <param name="propertyName">Имя проверяемого свойства</param>
        /// <param name="error">Описание ошибки</param>
        private void AddError(string propertyName, string error)
        {
            if (!_errorsByPropertyName.ContainsKey(propertyName))
                _errorsByPropertyName[propertyName] = new List<string>();

            if (!_errorsByPropertyName[propertyName].Contains(error))
            {
                _errorsByPropertyName[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        /// <summary>
        /// Очистить словарь ошибок
        /// </summary>
        /// <param name="propertyName"></param>
        private void ClearErrors(string propertyName)
        {
            if (_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }
    }
}
