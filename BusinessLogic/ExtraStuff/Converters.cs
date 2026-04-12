using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace ProjectHellsParadise.BusinessLogic.ExtraStuff;

/// <summary>
/// Returns true when the bound bool is false, and false whne it is true
/// </summary>
public class InvertedBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;
}

/// <summary>
/// Returns true when the bound int is greater than zero
/// used to show recently opened headero only when there is history
/// </summary>
public class IntToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is int count && count > 0;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
