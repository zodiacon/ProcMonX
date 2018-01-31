using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMonX.ViewModels {
	class CapturesViewModel : TabViewModelBase {
		ObservableCollection<CaptureViewModel> _captures = new ObservableCollection<CaptureViewModel>();

		public IList<CaptureViewModel> Captures => _captures;

		public CapturesViewModel() {
			Captures.Add(new CaptureViewModel());
			Captures.Add(new CaptureViewModel());
			Captures.Add(new CaptureViewModel());
		}
	}
}
