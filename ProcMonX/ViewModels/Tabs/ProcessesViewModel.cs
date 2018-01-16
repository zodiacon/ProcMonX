using ProcMonX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMonX.ViewModels.Tabs {
    [TabItem(Text = "Processes", Icon = "/icons/tabs/processes.ico")]
    sealed class ProcessesViewModel : TabItemViewModelBase {
        IEnumerable<TraceEventDataViewModel> _original;

        public IEnumerable<ProcessTraceEventViewModel> Events => _original.Where(evt => evt.Category == EventCategory.Processes).Select(evt => new ProcessTraceEventViewModel(evt));

        public ProcessesViewModel(IEnumerable<TraceEventDataViewModel> events) {
            _original = events;
            Refresh();
        }

        public override void Refresh() {
            RaisePropertyChanged(nameof(Events));
        }
    }
}
