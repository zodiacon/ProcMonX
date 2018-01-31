using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMonX.ViewModels {
	class EventViewModel : BindableBase {
		public string Icon { get; set; }
		public string Name { get; set; }
		public string Category { get; set; }

		private bool _isSelected;

		public bool IsSelected {
			get => _isSelected; 
			set => SetProperty(ref _isSelected, value); 
		}

	}
}
