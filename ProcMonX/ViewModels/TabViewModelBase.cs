using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProcMonX.ViewModels {

	abstract class TabViewModelBase : BindableBase {
		public string Header { get; set; }
		public string Icon { get; set; }

		public bool CanClose { get; set; } = true;

	}
}
