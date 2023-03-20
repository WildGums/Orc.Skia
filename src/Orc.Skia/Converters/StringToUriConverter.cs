namespace Orc.Skia;

using System;
using Catel.MVVM.Converters;

[System.Windows.Data.ValueConversion(typeof(string), typeof(Uri))]
public class StringToUriConverter : ValueConverterBase<string, Uri>
{
    protected override object? Convert(string? value, Type targetType, object? parameter)
    {
        return !string.IsNullOrEmpty(value)
            ? new Uri(value, UriKind.RelativeOrAbsolute)
            : null;
    }

    protected override object? ConvertBack(Uri? value, Type targetType, object? parameter)
    {
        return value?.ToString();
    }
}
