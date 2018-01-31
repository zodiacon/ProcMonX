using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing.Parsers;

namespace ProcMonX.Models {

	enum EventType {
		None,
		ProcessStart = 100, ProcessStop, ProcessDCStart, ProcessDCStop,
		ThreadStart = 200, ThreadStop, ThreadDCStart, ThreadDCStop,
		MemoryAlloc = 300, MemoryFree,
		RegistryOpenKey = 400, RegistryQueryValue, RegistrySetValue, RegistryCreateKey,
			RegistryCloseKey, RegistryEnumerateKey, RegistryEnumerateValues, RegistryFlush,
			RegistryDeleteKey, RegistryDeleteValue,
		AlpcSendMessage = 500, AlpcReceiveMessage,
		ModuleLoad = 600, ModuleUnload,
		FileRead = 700, FileWrite, FileCreate, FileRename, FileDelete, FileQueryInfo,
		DiskRead = 800, DiskWrite,
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
	}

	class EventInfo {
		public EventType EventType { get; private set; }
		public string AsString { get; private set; }
		public KernelTraceEventParser.Keywords Keyword { get; private set; }

		public EventCategory Category { get; private set; }

		public static readonly IReadOnlyList<EventInfo> AllEvents =
			new List<EventInfo> {
				new EventInfo {
					EventType = EventType.ProcessStart,
					AsString = "Process Start",
					Keyword = KernelTraceEventParser.Keywords.Process,
					Category = EventCategory.Processes
				},
				new EventInfo {
					EventType = EventType.ProcessDCStart,
					AsString = "Process DC Start",
					Keyword = KernelTraceEventParser.Keywords.Process,
					Category = EventCategory.Processes
				},
				new EventInfo {
					EventType = EventType.ProcessStop,
					AsString = "Process Stop",
					Keyword = KernelTraceEventParser.Keywords.Process,
					Category = EventCategory.Processes
				},
				new EventInfo {
					EventType = EventType.ThreadStart,
					AsString = "Thread Start",
					Keyword = KernelTraceEventParser.Keywords.Thread,
					Category = EventCategory.Threads
				},
				new EventInfo {
					EventType = EventType.ThreadDCStart,
					AsString = "Thread DC Start",
					Keyword = KernelTraceEventParser.Keywords.Thread,
					Category = EventCategory.Threads
				},
				new EventInfo {
					EventType = EventType.ThreadStop,
					AsString = "Thread Stop",
					Keyword = KernelTraceEventParser.Keywords.Thread,
					Category = EventCategory.Threads
				},
				new EventInfo {
					EventType = EventType.RegistryOpenKey,
					AsString = "Registry Open",
					Keyword = KernelTraceEventParser.Keywords.Registry,
					Category = EventCategory.Registry
				},
				new EventInfo {
					EventType = EventType.RegistryCreateKey,
					AsString = "Registry Create Key",
					Keyword = KernelTraceEventParser.Keywords.Registry,
					Category = EventCategory.Registry
				},
				new EventInfo {
					EventType = EventType.RegistryQueryValue,
					AsString = "Registry Query Value",
					Keyword = KernelTraceEventParser.Keywords.Registry,
					Category = EventCategory.Registry
				},
				new EventInfo {
					EventType = EventType.RegistryEnumerateKey,
					AsString = "Registry Enumerate Keys",
					Keyword = KernelTraceEventParser.Keywords.Registry,
					Category = EventCategory.Registry
				},
				new EventInfo {
					EventType = EventType.RegistryEnumerateValues,
					AsString = "Registry Enumerate Values",
					Keyword = KernelTraceEventParser.Keywords.Registry,
					Category = EventCategory.Registry
				},
				new EventInfo {
					EventType = EventType.RegistrySetValue,
					AsString = "Registry Set Value",
					Keyword = KernelTraceEventParser.Keywords.Registry,
					Category = EventCategory.Registry
				},
				new EventInfo {
					EventType = EventType.RegistryFlush,
					AsString = "Registry Flush",
					Keyword = KernelTraceEventParser.Keywords.Registry,
					Category = EventCategory.Registry
				},
				new EventInfo {
					EventType = EventType.RegistryDeleteKey,
					AsString = "Registry Delete Key",
					Keyword = KernelTraceEventParser.Keywords.Registry,
					Category = EventCategory.Registry
				},
				new EventInfo {
					EventType = EventType.RegistryDeleteValue,
					AsString = "Registry Delete Value",
					Keyword = KernelTraceEventParser.Keywords.Registry,
					Category = EventCategory.Registry
				},
				new EventInfo {
					EventType = EventType.ModuleLoad,
					AsString = "Module Load",
					Keyword = KernelTraceEventParser.Keywords.ImageLoad,
					Category = EventCategory.Modules
				},
				new EventInfo {
					EventType = EventType.ModuleUnload,
					AsString = "Module Unload",
					Keyword = KernelTraceEventParser.Keywords.ImageLoad,
					Category = EventCategory.Modules
				},
				new EventInfo {
					EventType = EventType.AlpcSendMessage,
					AsString = "ALPC Send Message",
					Keyword = KernelTraceEventParser.Keywords.AdvancedLocalProcedureCalls,
					Category = EventCategory.ALPC
				},
				new EventInfo {
					EventType = EventType.AlpcReceiveMessage,
					AsString = "ALPC Receive Message",
					Keyword = KernelTraceEventParser.Keywords.AdvancedLocalProcedureCalls,
					Category = EventCategory.ALPC
				},
				new EventInfo {
					EventType = EventType.FileRead,
					AsString = "File Read",
					Keyword = KernelTraceEventParser.Keywords.FileIO | KernelTraceEventParser.Keywords.FileIOInit,
					Category = EventCategory.Files
				},
				new EventInfo {
					EventType = EventType.FileWrite,
					AsString = "File Write",
					Keyword = KernelTraceEventParser.Keywords.FileIO | KernelTraceEventParser.Keywords.FileIOInit,
					Category = EventCategory.Files
				},
				new EventInfo {
					EventType = EventType.FileCreate,
					AsString = "File Create",
					Keyword = KernelTraceEventParser.Keywords.FileIO | KernelTraceEventParser.Keywords.FileIOInit,
					Category = EventCategory.Files
				},
				new EventInfo {
					EventType = EventType.FileDelete,
					AsString = "File Delete",
					Keyword = KernelTraceEventParser.Keywords.FileIO | KernelTraceEventParser.Keywords.FileIOInit,
					Category = EventCategory.Files
				},
				new EventInfo {
					EventType = EventType.FileRename,
					AsString = "File Rename",
					Keyword = KernelTraceEventParser.Keywords.FileIO | KernelTraceEventParser.Keywords.FileIOInit,
					Category = EventCategory.Files
				},
				new EventInfo {
					EventType = EventType.MemoryAlloc,
					AsString = "Memory Allocate",
					Keyword = KernelTraceEventParser.Keywords.VirtualAlloc,
					Category = EventCategory.Memory
				},
				new EventInfo {
					EventType = EventType.MemoryFree,
					AsString = "Memory Free",
					Keyword = KernelTraceEventParser.Keywords.VirtualAlloc,
					Category = EventCategory.Memory
				},
			};

		public static readonly IDictionary<EventType, EventInfo> AllEventsByType = AllEvents.ToDictionary(evt => evt.EventType);
		public static readonly IEnumerable<IGrouping<EventCategory, EventInfo>> AllEventsByCategory = AllEvents.GroupBy(evt => evt.Category).OrderBy(g => g.Key.ToString()).ToList();
	}
}
