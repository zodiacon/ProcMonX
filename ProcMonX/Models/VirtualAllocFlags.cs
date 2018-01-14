using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMonX.Models {
    [Flags]
    enum VirtualAllocFlags : uint {
        Commit = 0x1000,
        Reserve = 0x2000,
        Reset = 0x80000,
        ResetUndo = 0x1000000,
        LargePages = 0x20000000,
        Physical = 0x400000,
        TopDown = 0x100000,
        WriteWatch = 0x200000,
        Release = 0x8000,
        Decommit = 0x4000
    }
}
