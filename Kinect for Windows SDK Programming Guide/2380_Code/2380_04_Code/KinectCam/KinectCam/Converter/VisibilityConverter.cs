// ------------------------------------------------------------------------------
// <copyright file="VisibilityConverter.cs" 
// </copyright>
// ------------------------------------------------------------------------------

namespace KinectCam
{
    using System;
    using System.Windows;
    using System.Windows.Data;


    /// <summary>
    /// Visibility Converter
    /// </summary>
    public class VisibilityConverter : IValueConverter
    {
      
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility visible = Visibility.Visible;
            Visibility collapsed = Visibility.Collapsed;
   
            if (value is bool)
            {
                if ((bool)value)
                {
                    return visible;
                }
                else
                {
                    return collapsed;
                }
            }

            return collapsed;
     
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
