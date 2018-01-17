using ProcMonX.Models;
using Syncfusion.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMonX.ViewModels.Tabs {
    [TabItem(Text = "Processes", Icon = "/icons/tabs/processes.ico")]
    sealed class ProcessesViewModel : TabItemViewModelBase {
        IEnumerable<TraceEventDataViewModel> _events;

        public IEnumerable<TraceEventDataViewModel> Events => _events;

        public ProcessesViewModel(IEnumerable<TraceEventDataViewModel> events) {
            _events = events;
        }

        ICollectionViewAdv _view;
        public ICollectionViewAdv View {
            get => _view;
            set {
                _view = value;
                if (_view != null) {
                    _view.Filter = obj => ((TraceEventDataViewModel)obj).Category == EventCategory.Processes;
                    _view.RefreshFilter();
                }
            }
        }
    }
}
