using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NoteAppWpf.ViewModel
{
    public class NoteNameLengthRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int length = 0;
            try
            {
                if (((string) value).Length > 0)
                {
                    length = Int32.Parse((String)value);
                }
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"Wrong title size");
            }
            if (length < 0 || length > 50)
            {
                return new ValidationResult(false,$"Wrong title size");
            }
            return ValidationResult.ValidResult;
        }
    }
}
