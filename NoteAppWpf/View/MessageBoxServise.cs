using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NoteAppWpf.ViewModel;

namespace NoteAppWpf.View
{
    public class MessageBoxServise : IMessageBoxServise
    {
        public void Show(string message, string caption, MessageBoxButton button, MessageBoxImage image)
        {
            MessageBox.Show(message, caption, button, image);
        }
    }
}
