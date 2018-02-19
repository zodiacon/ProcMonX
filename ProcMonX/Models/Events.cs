using Microsoft.Diagnostics.Tracing.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMonX.Models {
    enum EventType {
        None,
        ProcessStart = 100, ProcessStop, ProcessDCStart, ProcessDCStop,
        ThreadStart = 200, ThreadStop, ThreadDCStart, ThreadDCStop,
        MemoryAlloc = 300, MemoryFree, HeapRangeCreate, HeapRangeDestory, HeapRangeReserve, VirtualAllocDCStart, VirtualAllocDCStop,
        RegistryOpenKey = 400, RegistryQueryValue, RegistrySetValue, RegistryCreateKey,
        RegistryCloseKey, RegistryEnumerateKey, RegistryEnumerateValues, RegistryFlush,
        RegistryDeleteKey, RegistryDeleteValue, RegistryQueryMultipleValues,
        AlpcSendMessage = 500, AlpcReceiveMessage, AlpcWaitForReply, ALPCWaitForNewMessage,
        ModuleLoad = 600, ModuleUnload, ModuleDCLoad, ModuleDCUnload,
        FileRead = 700, FileWrite, FileCreate, FileRename, FileDelete, FileQueryInfo, FileClose, FileFlush,
            FileMapDCStart, FileMapDCStop, FileMap, FileUnmap,
        DiskRead = 800, DiskWrite, DriverMajorFunctionCall, DriverMajorFunctionReturn, DriverCompletionRoutine,
        TcpIpReceive = 900, TcpIpSend, TcpIpConnect, TcpIpDisconnect, TcpIpAccept,
        MemoryMemInfo = 950, MemoryInMemory, MemorySystemMemoryInfo, MemoryInMemoryActive, ProcessMemoryInfo,
        Custom = 0x10000000
    }

    enum EventCategory {
        None,
        Processes,
        Threads,
        Registry,
        Files,
        Modules,
        ALPC,
        Network,
        Driver,
        Memory,
        MemoryManager,
        Disk,
        Other = 999
    }

    class CategoryInfo {
        public string Name { get; }
        public EventCategory Category { get; }

        public CategoryInfo(EventCategory category, string name = null) {
            Category = category;
            Name = name ?? category.ToString();
        }

        public static readonly IReadOnlyList<CategoryInfo> AllCategories = new List<CategoryInfo> {
            new CategoryInfo(EventCategory.Processes),
            new CategoryInfo(EventCategory.Threads),
            new CategoryInfo(EventCategory.Registry),
            new CategoryInfo(EventCategory.Files),
            new CategoryInfo(EventCategory.Network),
            new CategoryInfo(EventCategory.ALPC),
            new CategoryInfo(EventCategory.Modules),
            new CategoryInfo(EventCategory.Memory),
            new CategoryInfo(EventCategory.MemoryManager, "Memory Manager"),
            new CategoryInfo(EventCategory.Driver),
            new CategoryInfo(EventCategory.Disk),
            new CategoryInfo(EventCategory.Other)
        };

        public static readonly IReadOnlyDictionary<EventCategory, CategoryInfo> CategoriesByType = AllCategories.ToDictionary(cat => cat.Category);

        public static CategoryInfo GetCategory(EventCategory category) {
            return CategoriesByType[category];
        }

        public override string ToString() {
            return Name;
        }
    }

    class EventInfo {
        public EventType EventType { get; private set; }
        public string AsString { get; private set; }
        public KernelTraceEventParser.Keywords Keyword { get; private set; }
        public string Description { get; private set; }
        public CategoryInfo Category { get; private set; }

        public static readonly IReadOnlyList<EventInfo> AllEvents =
            new List<EventInfo> {
                new EventInfo {
                    EventType = EventType.ProcessStart,
                    AsString = "Process Start",
                    Keyword = KernelTraceEventParser.Keywords.Process,
                    Category = CategoryInfo.GetCategory(EventCategory.Processes),
                    Description = "Process is created"
                },
                new EventInfo {
                    EventType = EventType.ProcessDCStart,
                    AsString = "Process DC Start",
                    Keyword = KernelTraceEventParser.Keywords.Process,
                    Category = CategoryInfo.GetCategory(EventCategory.Processes),
                    Description = "Existing set of processes on the system"
                },
                new EventInfo {
                    EventType = EventType.ProcessStop,
                    AsString = "Process Stop",
                    Keyword = KernelTraceEventParser.Keywords.Process,
                    Category = CategoryInfo.GetCategory(EventCategory.Processes),
                    Description = "Process is terminated (for whatever reason)"
                },
                new EventInfo {
                    EventType = EventType.ThreadStart,
                    AsString = "Thread Start",
                    Keyword = KernelTraceEventParser.Keywords.Thread,
                    Category = CategoryInfo.GetCategory(EventCategory.Threads),
                    Description = "Thread is created"
                },
                new EventInfo {
                    EventType = EventType.ThreadDCStart,
                    AsString = "Thread DC Start",
                    Keyword = KernelTraceEventParser.Keywords.Thread,
                    Category = CategoryInfo.GetCategory(EventCategory.Threads),
                    Description = "Existing threads on the system"
                },
                new EventInfo {
                    EventType = EventType.ThreadStop,
                    AsString = "Thread Stop",
                    Keyword = KernelTraceEventParser.Keywords.Thread,
                    Category = CategoryInfo.GetCategory(EventCategory.Threads),
                    Description = "Thread is terminated (for whatever reason)"
                },
                new EventInfo {
                    EventType = EventType.RegistryOpenKey,
                    AsString = "Registry Open",
                    Keyword = KernelTraceEventParser.Keywords.Registry,
                    Category = CategoryInfo.GetCategory(EventCategory.Registry),
                    Description = "Open registry key"
                },
                new EventInfo {
                    EventType = EventType.RegistryCreateKey,
                    AsString = "Registry Create Key",
                    Keyword = KernelTraceEventParser.Keywords.Registry,
                    Category = CategoryInfo.GetCategory(EventCategory.Registry)
                },
                new EventInfo {
                    EventType = EventType.RegistryQueryValue,
                    AsString = "Registry Query Value",
                    Keyword = KernelTraceEventParser.Keywords.Registry,
                    Category = CategoryInfo.GetCategory(EventCategory.Registry)
                },
                new EventInfo {
                    EventType = EventType.RegistryQueryMultipleValues,
                    AsString = "Registry Query Multiple Values",
                    Keyword = KernelTraceEventParser.Keywords.Registry,
                    Category = CategoryInfo.GetCategory(EventCategory.Registry)
                },
                new EventInfo {
                    EventType = EventType.RegistryEnumerateKey,
                    AsString = "Registry Enumerate Keys",
                    Keyword = KernelTraceEventParser.Keywords.Registry,
                    Category = CategoryInfo.GetCategory(EventCategory.Registry)
                },
                new EventInfo {
                    EventType = EventType.RegistryEnumerateValues,
                    AsString = "Registry Enumerate Values",
                    Keyword = KernelTraceEventParser.Keywords.Registry,
                    Category = CategoryInfo.GetCategory(EventCategory.Registry)
                },
                new EventInfo {
                    EventType = EventType.RegistrySetValue,
                    AsString = "Registry Set Value",
                    Keyword = KernelTraceEventParser.Keywords.Registry,
                    Category = CategoryInfo.GetCategory(EventCategory.Registry)
                },
                new EventInfo {
                    EventType = EventType.RegistryFlush,
                    AsString = "Registry Flush",
                    Keyword = KernelTraceEventParser.Keywords.Registry,
                    Category = CategoryInfo.GetCategory(EventCategory.Registry)
                },
                new EventInfo {
                    EventType = EventType.RegistryDeleteKey,
                    AsString = "Registry Delete Key",
                    Keyword = KernelTraceEventParser.Keywords.Registry,
                    Category = CategoryInfo.GetCategory(EventCategory.Registry)
                },
                new EventInfo {
                    EventType = EventType.RegistryDeleteValue,
                    AsString = "Registry Delete Value",
                    Keyword = KernelTraceEventParser.Keywords.Registry,
                    Category = CategoryInfo.GetCategory(EventCategory.Registry)
                },
                new EventInfo {
                    EventType = EventType.ModuleLoad,
                    AsString = "Module Load",
                    Keyword = KernelTraceEventParser.Keywords.ImageLoad,
                    Category = CategoryInfo.GetCategory(EventCategory.Modules)
                },
                new EventInfo {
                    EventType = EventType.ModuleUnload,
                    AsString = "Module Unload",
                    Keyword = KernelTraceEventParser.Keywords.ImageLoad,
                    Category = CategoryInfo.GetCategory(EventCategory.Modules)
                },
                new EventInfo {
                    EventType = EventType.ModuleDCLoad,
                    AsString = "Module DC Load",
                    Keyword = KernelTraceEventParser.Keywords.ImageLoad,
                    Category = CategoryInfo.GetCategory(EventCategory.Modules)
                },
                new EventInfo {
                    EventType = EventType.ModuleDCUnload,
                    AsString = "Module DC Unload",
                    Keyword = KernelTraceEventParser.Keywords.ImageLoad,
                    Category = CategoryInfo.GetCategory(EventCategory.Modules)
                },
                new EventInfo {
                    EventType = EventType.AlpcSendMessage,
                    AsString = "ALPC Send Message",
                    Keyword = KernelTraceEventParser.Keywords.AdvancedLocalProcedureCalls,
                    Category = CategoryInfo.GetCategory(EventCategory.ALPC)
                },
                new EventInfo {
                    EventType = EventType.AlpcReceiveMessage,
                    AsString = "ALPC Receive Message",
                    Keyword = KernelTraceEventParser.Keywords.AdvancedLocalProcedureCalls,
                    Category = CategoryInfo.GetCategory(EventCategory.ALPC)
                },
                new EventInfo {
                    EventType = EventType.ALPCWaitForNewMessage,
                    AsString = "ALPC Wait for New Message",
                    Keyword = KernelTraceEventParser.Keywords.AdvancedLocalProcedureCalls,
                    Category = CategoryInfo.GetCategory(EventCategory.ALPC)
                },
                new EventInfo {
                    EventType = EventType.AlpcWaitForReply,
                    AsString = "ALPC Wait for Reply",
                    Keyword = KernelTraceEventParser.Keywords.AdvancedLocalProcedureCalls,
                    Category = CategoryInfo.GetCategory(EventCategory.ALPC)
                },
                new EventInfo {
                    EventType = EventType.FileRead,
                    AsString = "File Read",
                    Keyword = KernelTraceEventParser.Keywords.FileIO | KernelTraceEventParser.Keywords.FileIOInit,
                    Category = CategoryInfo.GetCategory(EventCategory.Files)
                },
                new EventInfo {
                    EventType = EventType.FileWrite,
                    AsString = "File Write",
                    Keyword = KernelTraceEventParser.Keywords.FileIO | KernelTraceEventParser.Keywords.FileIOInit,
                    Category = CategoryInfo.GetCategory(EventCategory.Files)
                },
                new EventInfo {
                    EventType = EventType.FileCreate,
                    AsString = "File Create",
                    Keyword = KernelTraceEventParser.Keywords.FileIO | KernelTraceEventParser.Keywords.FileIOInit,
                    Category = CategoryInfo.GetCategory(EventCategory.Files)
                },
                new EventInfo {
                    EventType = EventType.FileClose,
                    AsString = "File Close",
                    Keyword = KernelTraceEventParser.Keywords.FileIO | KernelTraceEventParser.Keywords.FileIOInit,
                    Category = CategoryInfo.GetCategory(EventCategory.Files)
                },
                new EventInfo {
                    EventType = EventType.FileFlush,
                    AsString = "File Flush",
                    Keyword = KernelTraceEventParser.Keywords.FileIO | KernelTraceEventParser.Keywords.FileIOInit,
                    Category = CategoryInfo.GetCategory(EventCategory.Files)
                },
                new EventInfo {
                    EventType = EventType.FileDelete,
                    AsString = "File Delete",
                    Keyword = KernelTraceEventParser.Keywords.FileIO | KernelTraceEventParser.Keywords.FileIOInit,
                    Category = CategoryInfo.GetCategory(EventCategory.Files)
                },
                new EventInfo {
                    EventType = EventType.FileRename,
                    AsString = "File Rename",
                    Keyword = KernelTraceEventParser.Keywords.FileIO | KernelTraceEventParser.Keywords.FileIOInit,
                    Category = CategoryInfo.GetCategory(EventCategory.Files)
                },
                new EventInfo {
                    EventType = EventType.MemoryAlloc,
                    AsString = "Memory Allocate",
                    Keyword = KernelTraceEventParser.Keywords.VirtualAlloc,
                    Category = CategoryInfo.GetCategory(EventCategory.Memory)
                },
                new EventInfo {
                    EventType = EventType.MemoryFree,
                    AsString = "Memory Free",
                    Keyword = KernelTraceEventParser.Keywords.VirtualAlloc,
                    Category = CategoryInfo.GetCategory(EventCategory.Memory)
                },
                new EventInfo {
                    EventType = EventType.DiskWrite,
                    AsString = "Disk Write",
                    Keyword = KernelTraceEventParser.Keywords.DiskIO | KernelTraceEventParser.Keywords.DiskIOInit,
                    Category = CategoryInfo.GetCategory(EventCategory.Disk)
                },
                new EventInfo {
                    EventType = EventType.DiskRead,
                    AsString = "Disk Read",
                    Keyword = KernelTraceEventParser.Keywords.DiskIO | KernelTraceEventParser.Keywords.DiskIOInit,
                    Category = CategoryInfo.GetCategory(EventCategory.Disk)
                },
                new EventInfo {
                    EventType = EventType.TcpIpConnect,
                    AsString = "TCP/IP Connect",
                    Keyword = KernelTraceEventParser.Keywords.NetworkTCPIP,
                    Category = CategoryInfo.GetCategory(EventCategory.Network)
                },
                new EventInfo {
                    EventType = EventType.TcpIpDisconnect,
                    AsString = "TCP/IP Disconnect",
                    Keyword = KernelTraceEventParser.Keywords.NetworkTCPIP,
                    Category = CategoryInfo.GetCategory(EventCategory.Network)
                },
                new EventInfo {
                    EventType = EventType.TcpIpAccept,
                    AsString = "TCP/IP Accept",
                    Keyword = KernelTraceEventParser.Keywords.NetworkTCPIP,
                    Category = CategoryInfo.GetCategory(EventCategory.Network)
                },
                new EventInfo {
                    EventType = EventType.TcpIpSend,
                    AsString = "TCP/IP Send",
                    Keyword = KernelTraceEventParser.Keywords.NetworkTCPIP,
                    Category = CategoryInfo.GetCategory(EventCategory.Network)
                },
                new EventInfo {
                    EventType = EventType.TcpIpReceive,
                    AsString = "TCP/IP Receive",
                    Keyword = KernelTraceEventParser.Keywords.NetworkTCPIP,
                    Category = CategoryInfo.GetCategory(EventCategory.Network)
                },
                new EventInfo {
                    EventType = EventType.FileMapDCStart,
                    AsString = "File Mapping DC Start",
                    Keyword = KernelTraceEventParser.Keywords.VAMap,
                    Category = CategoryInfo.GetCategory(EventCategory.Files)
                },
                new EventInfo {
                    EventType = EventType.FileMap,
                    AsString = "File Mapped",
                    Keyword = KernelTraceEventParser.Keywords.VAMap,
                    Category = CategoryInfo.GetCategory(EventCategory.Files)
                },
                new EventInfo {
                    EventType = EventType.FileMapDCStop,
                    AsString = "File Mapping DC Stop",
                    Keyword = KernelTraceEventParser.Keywords.VAMap,
                    Category = CategoryInfo.GetCategory(EventCategory.Files)
                },
                new EventInfo {
                    EventType = EventType.FileUnmap,
                    AsString = "File Unmapped",
                    Keyword = KernelTraceEventParser.Keywords.VAMap,
                    Category = CategoryInfo.GetCategory(EventCategory.Files)
                },
                //new EventInfo {
                //    EventType = EventType.Custom,
                //    AsString = "Custom",
                //    Category = CategoryInfo.GetCategory(EventCategory.Other)
                //},
                //new EventInfo {
                //    EventType = EventType.DriverMajorFunctionCall,
                //    AsString = "Driver Major Function Call",
                //    Keyword = KernelTraceEventParser.Keywords.Driver,
                //    Category = EventCategory.Driver
                //},
            }.OrderBy(evt => evt.AsString).ToList();

        public static readonly IDictionary<EventType, EventInfo> AllEventsByType = AllEvents.ToDictionary(evt => evt.EventType);
        public static readonly IEnumerable<IGrouping<EventCategory, EventInfo>> AllEventsByCategory = AllEvents.GroupBy(evt => evt.Category.Category).OrderBy(g => g.Key.ToString()).ToList();
    }
}
