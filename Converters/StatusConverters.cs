using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace TrolleybusApp.Converters
{
    public class StatusToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TrolleybusStatus status)
            {
                return status switch
                {
                    TrolleybusStatus.Ready => "Готов к работе",
                    TrolleybusStatus.Broken => "Сломан",
                    TrolleybusStatus.PolesOff => "Штанги сняты",
                    _ => "Неизвестно"
                };
            }
            return "Неизвестно";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TrolleybusStatus status)
            {
                return status switch
                {
                    TrolleybusStatus.Ready => new SolidColorBrush(Colors.Green),
                    TrolleybusStatus.Broken => new SolidColorBrush(Colors.Red),
                    TrolleybusStatus.PolesOff => new SolidColorBrush(Colors.Orange),
                    _ => new SolidColorBrush(Colors.Gray)
                };
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 