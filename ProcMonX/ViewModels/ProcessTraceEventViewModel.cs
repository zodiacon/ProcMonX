using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using ProcMonX.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMonX.ViewModels {
    sealed class ProcessTraceEventViewModel : TraceEventDataViewModel {
        ProcessTraceData _data;
        internal ProcessTraceEventViewModel(TraceEventDataViewModel evt) : base(evt.Data, evt.Type, evt.Details) {
            _data = (ProcessTraceData)evt.Data;
            Debug.Assert(_data != null);
        }

        public string CommandLine => _data.CommandLine;
        public int Session => _data.SessionID;
        public ulong Key => _data.UniqueProcessKey;
        public string ImageFileName => _data.ImageFileName;
        public int ParentID => _data.ParentID;
        public string PackageFullName => _data.PackageFullName;
        public int ExitCode => _data.ExitStatus;
        public ProcessFlags Flags => _data.Flags;
        public string AppId => _data.ApplicationID;
    }
}
