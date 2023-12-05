using System.Globalization;
using AccountOperationUtilities.Formatting;
using Avalonia.Data.Converters;

namespace AccountOperationUtilities.Converters;

public class BytesConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return "0 B";

        var bytes = System.Convert.ToDouble(value);

        // TODO: I18N
        return UnitFormatting.FormatBytes(bytes) ?? "Error";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
