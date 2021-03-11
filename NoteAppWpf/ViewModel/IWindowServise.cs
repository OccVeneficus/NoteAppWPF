using GalaSoft.MvvmLight;

namespace NoteAppWpf.ViewModel
{
    /// <summary>
    /// Интерфейс для сервисного класса для диалоговых окон
    /// </summary>
    public interface IWindowServise
    {
        /// <summary>
        /// Метод для показа диалога окна
        /// </summary>
        /// <param name="windowType">Тип пользовательского окна</param>
        /// <param name="viewModel">Контекст</param>
        /// <returns></returns>
        bool? ShowDialog(string windowType, ViewModelBase viewModel);

        /// <summary>
        /// Метод для установки результата диалога по нажатию кнопок
        /// </summary>
        /// <param name="result">Результат из кнопки</param>
        void SetDialogResult(bool result);
    }
}
