using System;
using System.Windows.Input;

namespace NoteAppWpf.ViewModel
{
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Определяет, может ли команда выполнятся
        /// </summary>
        /// <param name="parameter">команда</param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        /// <summary>
        /// Действия команды
        /// </summary>
        /// <param name="parameter">команда</param>
        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
