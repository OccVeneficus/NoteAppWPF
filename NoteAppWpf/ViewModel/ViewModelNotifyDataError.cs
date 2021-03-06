using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight;

namespace NoteAppWpf.ViewModel
{
    public abstract class ViewModelNotifyDataError : ViewModelBase, INotifyDataErrorInfo
    {
        private ObservableCollection<ValidationError> _errors;

        public ObservableCollection<ValidationError> Errors
        {
            get
            {
                if (_errors == null)
                {
                    _errors = new ObservableCollection<ValidationError>();
                }

                return _errors;
            }
        }

        protected Dictionary<string, List<string>> PropertyDependencies = new Dictionary<string, List<string>>();



        public IEnumerable GetErrors(string propertyName)
        {
            IEnumerable<string> ret = null;
            ret = Errors.Where<ValidationError>(e => e.PropertyName == propertyName)
                .Select<ValidationError, string>(e => e.ErrorMessage);
            return ret;
        }

        public bool HasErrors
        {
            get
            {
                return PropertyDependencies.Any();
            }
        }

        protected void ValidateProperty(string propertyName)
        {
            VerifyPropertyName(propertyName);
            var worker = new BackgroundWorker();
            worker.DoWork += (o, e) =>
            {
                Thread.Sleep(4000);
                e.Result = ValidatePropertySpecialized(propertyName);
            };
            worker.RunWorkerCompleted += (o, e) =>
            {
                IEnumerable<string> messages =
                    e.Error == null ? (IEnumerable<string>) e.Result : Enumerable.Repeat<string>(e.Error.Message, 1);
                UpdateErrors(propertyName, messages);
            };
            worker.RunWorkerAsync();
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
            if (PropertyDependencies.ContainsKey(propertyName))
            {
                foreach (var item in PropertyDependencies[propertyName])
                {
                    ValidateProperty(item);
                }
            }
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected abstract IEnumerable<string> ValidatePropertySpecialized(string propertyName);

        protected virtual void UpdateErrors(string propertyName, IEnumerable<string> errors)
        {
            Errors.Where<ValidationError>(e => e.PropertyName == propertyName)
                .ToList<ValidationError>()
                .ForEach((element) =>
                {
                    Errors.Remove(element);
                });
            if (errors != null)
            {
                foreach (var item in errors)
                {
                    Errors.Add(new ValidationError()
                    {
                        PropertyName = propertyName, ErrorMessage = item
                    });                    
                }
            }
            RaiseErrorsChanged(propertyName);
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    }
}
