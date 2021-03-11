using System.Windows;

namespace NoteAppWpf.ViewModel
{
    /// <summary>
    /// Интерфейс для сервисного класса показа окон с сообщениями 
    /// </summary>
    public interface IMessageBoxServise
    {
        /// <summary>
        /// Метод показа окна с сообщением
        /// </summary>
        /// <param name="message">Сообщение на форме</param>
        /// <param name="caption">Заголовок формы</param>
        /// <param name="button">Кнопки формы</param>
        /// <param name="image">Изображение на кнопке</param>
        void Show(string message, string caption, MessageBoxButton button, MessageBoxImage image);
    }
}
