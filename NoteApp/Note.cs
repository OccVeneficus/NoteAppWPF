using System;
using Newtonsoft.Json;

namespace NoteApp
{
    /// <summary>
    /// Класс заметки, хранящий ее название, категорю, текст, дату создания и последнего
    /// изменения
    /// </summary>
    public class Note : ICloneable
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
            get
            {
                return _name;
            }
            set
            {
                if (value.Length > 50)
                {
                    throw new Exception("Name contains more than 50 characters");
                }
    
                _name = value;
                ModifidedDate = DateTime.Now;
            }
        }
    
        /// <summary>
        /// Задает/возвращает текст заметки
        /// </summary>
        public string Text { get; set; }
    
        /// <summary>
        /// Категория заметки
        /// </summary>
        private NoteCategory _category;
    
        /// <summary>
        /// Задает/возвращает категорию заметки
        /// </summary>
        public NoteCategory Category
        {
            get
            {
                return _category;
            }
            set
            {
                _category = value;
                ModifidedDate = DateTime.Now;
            }
        }
    
        /// <summary>
        /// Дата создания заметки
        /// </summary>
        private DateTime _createdDate;
    
        /// <summary>
        /// Возвращает дату создания заметки
        /// </summary>
        public DateTime CreatedDate { get; private set; }
    
        /// <summary>
        /// Дата последнего изменения заметки
        /// </summary>
        private DateTime _modifiedDate;
    
        /// <summary>
        /// Возвращает дату последнего изменения параметров заметки
        /// </summary>
    
        public DateTime ModifidedDate { get; set; }
    
        /// <summary>
        /// Реализация интерфейса IClone
        /// </summary>
        /// <returns>Возвращает новый экземпляр-копию текущего объекта</returns>
        public object Clone()
        {
            var noteClone = new Note(this.CreatedDate, this.ModifidedDate,
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
            this.CreatedDate = createdDate;
            this.ModifidedDate = modifiedDate;
            this.Name = name;
            this.Text = text;
            this.Category = category;
        }
    
    }
}
