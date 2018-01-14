using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using Microsoft.Diagnostics.Tracing.Session;
using ProcMonX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcMonX.Tracing {
    sealed class TraceManager : IDisposable {
        TraceEventSession _kernelSession;
        KernelTraceEventParser _kernelParser;
        //ClrTraceEventParser _clrParser;

        Thread _processingThread;

        public event Action<TraceEvent, EventType> EventTrace;

        public TraceManager() {
            TraceEventSession.SetDebugPrivilege();

            _handlers = new Dictionary<EventType, Action<TraceEvent>> {
                { EventType.ProcessStart, obj => HandleEvent(obj, EventType.ProcessStart) },
            };
        }

        public void Dispose() {
            _kernelSession.Dispose();
        }

        public void Start(IEnumerable<EventType> types) {
            if (EventTrace == null)
                throw new InvalidOperationException("Must register for event notifications");

            _kernelSession = new TraceEventSession(KernelTraceEventParser.KernelSessionName) {
                BufferSizeMB = 128,
                CpuSampleIntervalMSec = 10
            };

            var keywords = KernelTraceEventParser.Keywords.None;
            foreach (var type in types)
                keywords |= EventInfo.AllEventsByType[type].Keyword;

            //_kernelSession.EnableKernelProvider(keywords | KernelTraceEventParser.Keywords.Job);

            _processingThread = new Thread(() => {
                _kernelSession.EnableKernelProvider(keywords);
                _kernelParser = new KernelTraceEventParser(_kernelSession.Source);
                SetupCallbacks(types);
                _kernelSession.Source.Process();
            });
            _processingThread.Priority = ThreadPriority.Lowest;
            _processingThread.IsBackground = true;
            _processingThread.Start();

        }

        public TraceFilter Filter { get; set; }

        public void Stop() {
            _kernelSession.Flush();
            _kernelSession.Stop();
        }

        void HandleEvent(TraceEvent evt, EventType type) {
            var include = Filter?.EvaluateEvent(evt);
            if (include == null || include == FilterRuleResult.Include) {
                EventTrace(evt.Clone(), type);
            }
        }

        Dictionary<EventType, Action<TraceEvent>> _handlers;

        public void SetupCallback(EventType type, bool add) {
            if (_kernelParser == null)
                return;

            switch (type) {
                case EventType.ProcessStart:
                    if (add)
                        _kernelParser.ProcessStart += _handlers[EventType.ProcessStart];
                    else
                        _kernelParser.ProcessStart -= _handlers[EventType.ProcessStart];
                    break;

                case EventType.ProcessDCStart:
                    _kernelParser.ProcessDCStart += obj => HandleEvent(obj, EventType.ProcessDCStart);
                    break;

                case EventType.ProcessStop:
                    _kernelParser.ProcessStop += obj => HandleEvent(obj, EventType.ProcessStop);
                    break;

                case EventType.ThreadStart:
                    _kernelParser.ThreadStart += obj => HandleEvent(obj, EventType.ThreadStart);
                    break;

                case EventType.ThreadDCStart:
                    _kernelParser.ThreadDCStart += obj => HandleEvent(obj, EventType.ThreadDCStart);
                    break;

                case EventType.ThreadStop:
                    _kernelParser.ThreadStop += obj => HandleEvent(obj, EventType.ThreadStop);
                    break;

                case EventType.RegistryCreateKey:
                    _kernelParser.RegistryCreate += obj => HandleEvent(obj, EventType.RegistryCreateKey);
                    break;

                case EventType.RegistryOpenKey:
                    _kernelParser.RegistryOpen += obj => HandleEvent(obj, EventType.RegistryOpenKey);
                    break;

                case EventType.RegistryQueryValue:
                    _kernelParser.RegistryQueryValue += obj => HandleEvent(obj, EventType.RegistryQueryValue);
                    break;

                case EventType.RegistryQueryMultipleValues:
                    _kernelParser.RegistryQueryMultipleValue += obj => HandleEvent(obj, EventType.RegistryQueryMultipleValues);
                    break;

                case EventType.RegistrySetValue:
                    _kernelParser.RegistrySetValue += obj => HandleEvent(obj, EventType.RegistrySetValue);
                    break;

                case EventType.RegistryDeleteKey:
                    _kernelParser.RegistryDelete += obj => HandleEvent(obj, EventType.RegistryDeleteKey);
                    break;

                case EventType.RegistryDeleteValue:
                    _kernelParser.RegistryDeleteValue += obj => HandleEvent(obj, EventType.RegistryDeleteValue);
                    break;

                case EventType.ModuleLoad:
                    _kernelParser.ImageLoad += obj => HandleEvent(obj, EventType.ModuleLoad);
                    break;

                case EventType.ModuleUnload:
                    _kernelParser.ImageUnload += obj => HandleEvent(obj, EventType.ModuleUnload);
                    break;

                case EventType.AlpcSendMessage:
                    _kernelParser.ALPCSendMessage += obj => HandleEvent(obj, EventType.AlpcSendMessage);
                    break;

                case EventType.AlpcReceiveMessage:
                    _kernelParser.ALPCReceiveMessage += obj => HandleEvent(obj, EventType.AlpcReceiveMessage);
                    break;

                case EventType.FileRead:
                    _kernelParser.FileIORead += obj => HandleEvent(obj, EventType.FileRead);
                    break;

                case EventType.FileWrite:
                    _kernelParser.FileIOWrite += obj => HandleEvent(obj, EventType.FileWrite);
                    break;

                case EventType.FileRename:
                    _kernelParser.FileIORename += obj => HandleEvent(obj, EventType.FileRename);
                    break;

                case EventType.FileCreate:
                    _kernelParser.FileIOFileCreate += obj => HandleEvent(obj, EventType.FileCreate);
                    break;

                case EventType.FileDelete:
                    _kernelParser.FileIOFileDelete += obj => HandleEvent(obj, EventType.FileDelete);
                    break;

                case EventType.MemoryAlloc:
                    _kernelParser.VirtualMemAlloc += obj => HandleEvent(obj, EventType.MemoryAlloc);
                    break;

                case EventType.MemoryFree:
                    _kernelParser.VirtualMemFree += obj => HandleEvent(obj, EventType.MemoryFree);
                    break;

            }
        }

        private void SetupCallbacks(IEnumerable<EventType> types, bool add = true) {
            foreach (var type in types) {
                SetupCallback(type, add);
            }
        }

        public int LostEvents => _kernelSession?.IsActive == true ? _kernelSession.EventsLost : 0;

    }
}
