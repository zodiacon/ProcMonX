using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMonX.Models {
    public sealed class EventData {
        public int Index { get; set; }
        public DateTime Time { get; set; }
        public string EventType { get; set; }
        public string Category { get; set; }
        public int ProcessId { get; set; }
        public string ProcessName { get; set; }
        public int? ThreadId { get; set; }
        public int CPU { get; set; }
        public string Opcode { get; set; }
        public string Details { get; set; }
    }
}
