using System.Collections.Generic;
using System.Windows;
using NoteAppWpf.ViewModel;

namespace NoteAppWpf.Services.MessageBoxServices
{
    public class MessageBoxServise : IMessageBoxServise
    {
        private readonly Dictionary<MyMessageBoxButton, MessageBoxButton> _messageBoxButtons =
            new Dictionary<MyMessageBoxButton, MessageBoxButton>
            {
                {MyMessageBoxButton.OK, MessageBoxButton.OK},
                {MyMessageBoxButton.YesNo, MessageBoxButton.YesNo}
            };

        private readonly Dictionary<MyMessageBoxImage, MessageBoxImage> _messageBoxImages =
            new Dictionary<MyMessageBoxImage, MessageBoxImage>
            {
                {MyMessageBoxImage.Warning, MessageBoxImage.Warning}
            };

        public bool Show(string message, string caption, MyMessageBoxButton button, MyMessageBoxImage image)
        {
           MessageBoxResult result = MessageBox.Show(message, caption, _messageBoxButtons[button], _messageBoxImages[image]);
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
