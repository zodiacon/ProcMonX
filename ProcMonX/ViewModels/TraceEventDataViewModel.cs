using Microsoft.Diagnostics.Tracing;
using ProcMonX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcMonX.ViewModels {
    sealed class TraceEventDataViewModel {
        static int _globalIndex;
        public int Index { get; }
        public TraceEvent Data { get; }
        public EventType Type { get; }
        public string TypeAsString { get; }

        public string Icon { get; }

        public int? ThreadId => Data.ThreadID < 0 ? (int?)null : Data.ThreadID;

        public string Details { get; }
        public EventCategory Category { get; }
        public string ProcessName { get; }

        public TraceEventDataViewModel(TraceEvent evt, EventType type, string details = null) {
            Data = evt;
            Type = type;
            var info = EventInfo.AllEventsByType[type];
            ProcessName = string.Intern(evt.ProcessName);
            TypeAsString = info.AsString ?? type.ToString();
            Index = Interlocked.Increment(ref _globalIndex);
            Icon = string.Intern($"/icons/events/{type.ToString()}.ico");
            Category = info.Category;
            Details = details ?? string.Empty;
        }
    }
}
