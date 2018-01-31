using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProcMonX.Models {
	public class AppOptions : BindableBase {
		private bool _alwaysOnTop;

		public bool AlwaysOnTop {
			get { return _alwaysOnTop; }
			set {	SetProperty(ref _alwaysOnTop, value); }
		}
	}
}
