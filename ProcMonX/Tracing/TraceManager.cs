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
                CpuSampleIntervalMSec = 10,
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

        public TraceEventFilter Filter { get; set; }

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

        void SetupCallback(EventType type) {
            switch (type) {
                case EventType.ProcessStart:
                    _kernelParser.ProcessStart += _handlers[EventType.ProcessStart];
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

                case EventType.ModuleDCLoad:
                    _kernelParser.ImageDCStart += obj => HandleEvent(obj, EventType.ModuleDCLoad);
                    break;

                case EventType.ModuleDCUnload:
                    _kernelParser.ImageDCStop += obj => HandleEvent(obj, EventType.ModuleDCUnload);
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

                case EventType.ALPCWaitForNewMessage:
                    _kernelParser.ALPCWaitForNewMessage += obj => HandleEvent(obj, EventType.ALPCWaitForNewMessage);
                    break;

                case EventType.AlpcWaitForReply:
                    _kernelParser.ALPCWaitForReply += obj => HandleEvent(obj, EventType.AlpcWaitForReply);
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
                    _kernelParser.FileIOCreate += obj => HandleEvent(obj, EventType.FileCreate);
                    break;

                case EventType.FileClose:
                    _kernelParser.FileIOClose += obj => HandleEvent(obj, EventType.FileClose);
                    break;

                case EventType.FileFlush:
                    _kernelParser.FileIOFlush += obj => HandleEvent(obj, EventType.FileFlush);
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

                case EventType.DiskRead:
                    _kernelParser.DiskIORead += obj => HandleEvent(obj, EventType.DiskRead);
                    break;

                case EventType.DiskWrite:
                    _kernelParser.DiskIOWrite += obj => HandleEvent(obj, EventType.DiskWrite);
                    break;

                case EventType.TcpIpConnect:
                    _kernelParser.TcpIpConnect += obj => HandleEvent(obj, EventType.TcpIpConnect);
                    _kernelParser.TcpIpConnectIPV6 += obj => HandleEvent(obj, EventType.TcpIpConnect);
                    break;

                case EventType.TcpIpDisconnect:
                    _kernelParser.TcpIpDisconnect += obj => HandleEvent(obj, EventType.TcpIpDisconnect);
                    _kernelParser.TcpIpDisconnectIPV6 += obj => HandleEvent(obj, EventType.TcpIpDisconnect);
                    break;

                case EventType.TcpIpAccept:
                    _kernelParser.TcpIpAccept += obj => HandleEvent(obj, EventType.TcpIpAccept);
                    _kernelParser.TcpIpAcceptIPV6 += obj => HandleEvent(obj, EventType.TcpIpAccept);
                    break;

                case EventType.TcpIpSend:
                    _kernelParser.TcpIpSend += obj => HandleEvent(obj, EventType.TcpIpSend);
                    _kernelParser.TcpIpSendIPV6 += obj => HandleEvent(obj, EventType.TcpIpSend);
                    break;

                case EventType.TcpIpReceive:
                    _kernelParser.TcpIpRecv += obj => HandleEvent(obj, EventType.TcpIpReceive);
                    _kernelParser.TcpIpRecvIPV6 += obj => HandleEvent(obj, EventType.TcpIpReceive);
                    break;

                case EventType.FileMapDCStart:
                    _kernelParser.FileIOMapFileDCStart += obj => HandleEvent(obj, EventType.FileMapDCStart);
                    break;

                case EventType.FileMapDCStop:
                    _kernelParser.FileIOMapFileDCStop+= obj => HandleEvent(obj, EventType.FileMapDCStop);
                    break;

                case EventType.FileMap:
                    _kernelParser.FileIOMapFile += obj => HandleEvent(obj, EventType.FileMap);
                    break;

                case EventType.FileUnmap:
                    _kernelParser.FileIOUnmapFile += obj => HandleEvent(obj, EventType.FileUnmap);
                    break;

                case EventType.DriverMajorFunctionCall:
                    _kernelParser.DiskIODriverMajorFunctionCall += obj => HandleEvent(obj, EventType.DriverMajorFunctionCall);
                    break;
            }
        }

        private void SetupCallbacks(IEnumerable<EventType> types, bool add = true) {
            foreach (var type in types) {
                SetupCallback(type);
            }
        }

        public int LostEvents => _kernelSession?.IsActive == true ? _kernelSession.EventsLost : 0;

    }
}
