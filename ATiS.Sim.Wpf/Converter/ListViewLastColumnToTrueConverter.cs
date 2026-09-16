using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace ATiS.Sim.Wpf
{
    public class ListViewLastColumnToTrueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is GridViewColumnHeader header && values[1] is GridView gridView)
            {
                var column = header.Column;
                if (column != null)
                {
                    int index = gridView.Columns.IndexOf(column);
                    return !(index >= 0 && index < gridView.Columns.Count - 1);
                }
            }
            return true; 
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
