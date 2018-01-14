using ProcMonX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMonX.ViewModels.Tabs {
    [TabItem(Text = "Processes", Icon = "/icons/tabs/processes.ico")]
    sealed class ProcessesViewModel : TabItemViewModelBase {
        public IEnumerable<ProcessTraceEventViewModel> Events { get; }

        public ProcessesViewModel(IEnumerable<TraceEventDataViewModel> events) {
            Events = events.Where(evt => evt.Category == EventCategory.Processes).Select(evt => new ProcessTraceEventViewModel(evt));
        }

        public override void Refresh() {
            RaisePropertyChanged(nameof(Events));
        }
    }
}
