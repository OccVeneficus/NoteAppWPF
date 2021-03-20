using System.Windows;

namespace NoteAppWpf.Services.MessageBoxServices
{
    public class MessageBoxServise : IMessageBoxServise
    {
        public bool Show(string message, string caption, MessageBoxButton button, MessageBoxImage image)
        {
           MessageBoxResult result = MessageBox.Show(message, caption, button, image);
           if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
           {
               return true;
           }
           else
           {
               return false;
           }
        }
    }
}
