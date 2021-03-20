using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using NoteApp.Properties;

namespace NoteApp
{
    /// <summary>
    /// Класс заметки, хранящий ее название, категорию, текст, дату создания и последнего
    /// изменения
    /// </summary>
    public class Note : ICloneable, INotifyPropertyChanged
    {
        /// <summary>
        /// Название заметки
        /// </summary>
        private string _name;
    
        /// <summary>
        /// Задает/возвращает название заметки размером не более 50 символов
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (value.Length > 50)
                {
                    throw new ArgumentException("Name contains more than 50 characters");
                }
    
                _name = value;
                ModifiedDate = DateTime.Now;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _text;

        /// <summary>
        /// Задает/возвращает текст заметки
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
    
        /// <summary>
        /// Категория заметки
        /// </summary>
        private NoteCategory _category;
    
        /// <summary>
        /// Задает/возвращает категорию заметки
        /// </summary>
        public NoteCategory Category
        {
            get => _category;
            set
            {
                _category = value;
                ModifiedDate = DateTime.Now;
                OnPropertyChanged(nameof(Category));
            }
        }
    
        /// <summary>
        /// Дата создания заметки
        /// </summary>
        private DateTime _createdDate;
    
        /// <summary>
        /// Возвращает дату создания заметки
        /// </summary>
        public DateTime CreatedDate
        {
            get => _createdDate;
            private set
            {
                _createdDate = value;
                OnPropertyChanged(nameof(CreatedDate));
            }
        }
    
        /// <summary>
        /// Дата последнего изменения заметки
        /// </summary>
        private DateTime _modifiedDate;

        /// <summary>
        /// Возвращает дату последнего изменения параметров заметки
        /// </summary>

        public DateTime ModifiedDate
        {
            get => _modifiedDate;
            set
            {
                _modifiedDate = value;
                OnPropertyChanged(nameof(ModifiedDate));
            }
        }
    
        /// <summary>
        /// Реализация интерфейса IClone
        /// </summary>
        /// <returns>Возвращает новый экземпляр-копию текущего объекта</returns>
        public object Clone()
        {
            var noteClone = new Note(this.CreatedDate, this.ModifiedDate,
                this.Name, this.Text, this.Category);
            return noteClone;
        }
    
        /// <summary>
        /// Конструктор класса. Используется сериализатором JSON
        /// </summary>
        /// <param name="createdDate">Дата создания заметки</param>
        /// <param name="modifiedDate">Дата последнего изменения заметки</param>
        /// <param name="name">Название заметки</param>
        /// <param name="text">Текст заметки</param>
        /// <param name="category">Категория <see cref="NoteCategory"/> заметки</param>
        [JsonConstructor]
        public Note(DateTime createdDate, DateTime modifiedDate, string name,
            string text, NoteCategory category)
        {
            CreatedDate = createdDate;
            Name = name;
            Text = text;
            Category = category;
            ModifiedDate = modifiedDate;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
