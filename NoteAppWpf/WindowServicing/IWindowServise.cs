using GalaSoft.MvvmLight;

namespace NoteAppWpf.WindowServicing
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
        bool? ShowDialog(WindowType windowType, ViewModelBase viewModel);

        /// <summary>
        /// Метод для установки результата диалога по нажатию кнопок
        /// </summary>
        /// <param name="result">Результат из кнопки</param>
        void SetDialogResult(bool result);
    }
}
