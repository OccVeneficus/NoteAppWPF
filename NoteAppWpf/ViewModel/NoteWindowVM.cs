using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using NoteApp;
using NoteAppWpf.Services.MessageBoxServices;
using NoteAppWpf.Services.WindowServices;

namespace NoteAppWpf.ViewModel
{
    /// <summary>
    /// VM окна редактирования заметки
    /// </summary>
    public class NoteWindowVM : ViewModelBase,INotifyDataErrorInfo
    {
        #region Поля

        /// <summary>
        /// Словарь для хранения ошибок проверки свойств 
        /// </summary>
        private readonly Dictionary<string, List<string>> _errorsByPropertyName =
            new Dictionary<string, List<string>>();

        private Note _note;

        private string _newNoteTitle;

        private readonly List<NoteCategory> _noteCategories =
            Enum.GetValues(typeof(NoteCategory)).Cast<NoteCategory>().ToList();

        private readonly IMessageBoxServise _messageBoxServise;

        private readonly IWindowServise _windowServise;

        private ICommand _cancelCommand = null;

        private RelayCommand _okCommand = null;

        #endregion

        #region Свойства

        /// <summary>
        /// Поле для хранения результата диалогового окна
        /// </summary>
        public bool DialogResult { get; private set; }

        /// <summary>
        /// Поле для хранения редактируемой заметки
        /// </summary>
        public Note Note { get; set; }

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
                    Set(ref _newNoteTitle, value);
                    ValidateNoteName();
                }
            }
        }

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

        /// <summary>
        /// Возвращает наличие ошибок
        /// </summary>
        public bool HasErrors => _errorsByPropertyName.Any();

        #endregion

        #region События

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        #endregion

        #region Конструкторы

        public NoteWindowVM(Note note, IMessageBoxServise messageBoxServise,
            IWindowServise windowServise)
        {

            Note = note;
            NewNoteTitle = note.Name;
            _messageBoxServise = messageBoxServise;
            _windowServise = windowServise;
            ShowDialogWindow();
        }

        #endregion

        #region Приватные методы

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
                AddError(nameof(NewNoteTitle), "Wrong title length");
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

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        #endregion

        #region Публичные методы

        /// <summary>
        /// Метод для отображения диалога окна
        /// </summary>
        public void ShowDialogWindow()
        {
            bool? dialogResult = _windowServise.ShowDialog(WindowType.Note, this);
            if (dialogResult != null)
            {
                DialogResult = (bool)dialogResult;
            }
        }

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

        #region Команды

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
                                MyMessageBoxButton.OK,
                                MyMessageBoxImage.Warning);
                        }
                    });
                }
                return _okCommand;
            }
        }

        #endregion

        #endregion
    }
}
