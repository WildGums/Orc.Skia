namespace Orc.Skia
{
    using System;
    using Catel.MVVM.Converters;

    [System.Windows.Data.ValueConversion(typeof(string), typeof(Uri))]
    public class StringToUriConverter : ValueConverterBase<string, Uri>
    {
        protected override object Convert(string value, Type targetType, object parameter)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return new Uri(value, UriKind.RelativeOrAbsolute);
        }

        protected override object ConvertBack(Uri value, Type targetType, object parameter)
        {
            return value?.ToString();
        }
    }
}
