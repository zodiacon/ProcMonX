using Microsoft.Diagnostics.Tracing;
using ProcMonX.Models;
using ProcMonX.ViewModels.EventCategories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zodiacon.ManagedWindows.Processes;

namespace ProcMonX.ViewModels {
    class TraceEventDataViewModel {
        static int _globalIndex;
        public int Index { get; }
        public DateTime TimeStamp => Data.TimeStamp;

        public TraceEvent Data { get; }
        public EventType Type { get; }
        public string TypeAsString { get; }
        public CategoryInfo Category { get; }
        public string Opcode => Data.OpcodeName;

        public int ProcessId => Data.ProcessID;
        public string ProcessName { get; }
        public int ThreadId => Data.ThreadID;
        public int Processor => Data.ProcessorNumber;
        public TraceEventLevel Level => Data.Level;

        public string Details { get; }

        internal TraceEventDataViewModel(TraceEvent evt, EventType type, string details = null) {
            Data = evt;
            Type = type;
            var info = EventInfo.AllEventsByType[type];
            ProcessName = string.Intern(string.IsNullOrEmpty(evt.ProcessName) ? QueryProcessName(evt.ProcessID) : evt.ProcessName);
            TypeAsString = info.AsString ?? type.ToString();
            Index = Interlocked.Increment(ref _globalIndex);
            Category = info.Category;
            Details = details ?? string.Empty;
        }

        private string QueryProcessName(int processID) {
            using (var process = NativeProcess.TryOpen(ProcessAccessMask.QueryLimitedInformation, processID)) {
                if (process == null)
                    return string.Empty;
                var name = process.TryGetFullImageName();
                if (name == null)
                    return string.Empty;

                return "(" + Path.GetFileNameWithoutExtension(name) + ")";
            }
        }
    }
}
