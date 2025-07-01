using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PL
{
    //    class ConvertUpdateToTrueKey : IValueConverter
    //    {
    //        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //        { BO.Year year = (BO.Year)value; switch (year) { case BO.Year.FirstYear: return Brushes.Yellow; case BO.Year.SecondYear: return Brushes.Orange; case BO.Year.ThirdYear: return Brushes.Green; case BO.Year.ExtraYear: return Brushes.PaleVioletRed; case BO.Year.None: return Brushes.White; default: return Brushes.White; } }
    //        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
    //            throw new NotImplementedException(); 
    //        }
    //    }
    //}

    //public class UpdateToReadOnlyConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value?.ToString() == "Update"; // אם הכפתור במצב Update, הפקד יהיה לקריאה בלבד
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
    //        throw new NotImplementedException();
    //}

    public class ConvertUpdateToCollapsedKey : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string buttonText = value as string;
            return buttonText == "Add" ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace(value as string) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

    public class EnumerableToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumerable = value as IEnumerable;
            if (enumerable == null)
                return Visibility.Collapsed;

            foreach (var _ in enumerable)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                BO.AssignmentStatus.Completed => Brushes.LightGreen,
                BO.AssignmentStatus.Expired => Brushes.LightSalmon,
                _ => Brushes.White
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

  }