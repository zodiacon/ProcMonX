using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProcMonX.Extensions {
	static class IconProperties {
		public static string GetIcon(DependencyObject obj) {
			return (string)obj.GetValue(IconProperty);
		}

		public static void SetIcon(DependencyObject obj, string value) {
			obj.SetValue(IconProperty, value);
		}

		public static readonly DependencyProperty IconProperty =
			DependencyProperty.RegisterAttached("Icon", typeof(string), typeof(IconProperties), new PropertyMetadata(null));


	}
}
