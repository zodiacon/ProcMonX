using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
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
        //TraceEventSession _customSession;
        KernelTraceEventParser _kernelParser;
        //ClrTraceEventParser _clrParser;

        Thread _processingThread;
        bool _includeInit;

        public event Action<TraceEvent, EventType> EventTrace;

        public TraceManager() {
            TraceEventSession.SetDebugPrivilege();
        }

        public void Dispose() {
            _kernelSession.Dispose();
        }

        public void Start(IEnumerable<EventType> types, bool includeInit) {
            if (EventTrace == null)
                throw new InvalidOperationException("Must register for event notifications");

            _includeInit = includeInit;
            _kernelSession = new TraceEventSession(KernelTraceEventParser.KernelSessionName) {
                BufferSizeMB = 64,
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

        private void SetupCallbacks(IEnumerable<EventType> types) {
            foreach (var type in types) {
                switch (type) {
                    case EventType.ProcessStart:
                        _kernelParser.ProcessStart += obj => HandleEvent(obj, EventType.ProcessStart);
                        break;

                    case EventType.ProcessStop:
                        _kernelParser.ProcessStop += obj => HandleEvent(obj, EventType.ProcessStop);
                        break;

                    case EventType.ThreadStart:
                        _kernelParser.ThreadStart += obj => HandleEvent(obj, EventType.ThreadStart);
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
                        _kernelParser.MemoryRangeAccess += obj => HandleEvent(obj, EventType.MemoryFree);
                        break;

                    case EventType.MemoryFree:
                        //_kernelParser.VirtualMemFree += obj => HandleEvent(obj, EventType.MemoryFree);
                        break;

                }
            }
        }

        public int LostEvents => _kernelSession?.IsActive == true ? _kernelSession.EventsLost : 0;

    }
}
