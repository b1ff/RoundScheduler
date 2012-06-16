using System;
using System.Collections;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace RoundScheduler.Utils
{
    public class IndexConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var currItem = values[0];
            var source = values[1] as IList;
            if (source == null)
                return string.Empty;

            var index = source.IndexOf(currItem);
            return index;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    [ValueConversion(typeof(DataGrid), typeof(DataGridRow))]
    public class DataGridIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = (DataGridRow)value;
            var dataGrid = ItemsControl.ItemsControlFromItemContainer(item) as DataGrid;
            if (dataGrid == null)
                return string.Empty;
            var index = dataGrid.ItemContainerGenerator.IndexFromContainer(item);
            return index + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
