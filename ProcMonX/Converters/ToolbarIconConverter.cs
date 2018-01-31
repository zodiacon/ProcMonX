using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ProcMonX.Converters {
	class ToolbarIconConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var icon = (string)value;
			switch (parameter.ToString()) {
				case "normal": return $"/icons/normal/{icon}.ico";
				case "hot": return $"/icons/hot/{icon}.ico";
				case "disabled": return $"/icons/disabled/{icon}.ico";
			}

			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
