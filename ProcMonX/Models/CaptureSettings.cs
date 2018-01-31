using ProcMonX.Tracing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMonX.Models {
	class CaptureSettings {
		ObservableCollection<EventType> _eventTypes = new ObservableCollection<EventType> {
			EventType.ProcessStart, EventType.ProcessStop, EventType.ModuleLoad, EventType.ModuleUnload
		};

		public IList<EventType> EventTypes => _eventTypes;

		public TraceFilter Filter { get; set; }

		public string Name { get; set; } = "Capture1";
		public string FileName { get; set; }
	}
}
