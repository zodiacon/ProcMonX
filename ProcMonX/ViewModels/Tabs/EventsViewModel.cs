using ProcMonX.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMonX.ViewModels.Tabs {
    [TabItem(Text = "All Events", Icon = "/icons/tabs/event.ico")]
    sealed class EventsViewModel : TabItemViewModelBase {
        public EventsViewModel(IList<TraceEventDataViewModel> events) {
            Events = events;
        }

        internal override bool CanClose => false;

        public IList<TraceEventDataViewModel> Events { get; }
    }
}
