using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using Prism.Commands;
using Prism.Mvvm;
using ProcMonX.Models;
using ProcMonX.Tracing;
using ProcMonX.ViewModels.Tabs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Zodiacon.WPF;

namespace ProcMonX.ViewModels {
    sealed class MainViewModel : BindableBase {
        TraceManager _traceManager = new TraceManager();
        ObservableCollection<TabItemViewModelBase> _tabs = new ObservableCollection<TabItemViewModelBase>();
        ObservableCollection<TraceEventDataViewModel> _events = new ObservableCollection<TraceEventDataViewModel>();
        ObservableCollection<EventType> _eventTypes = new ObservableCollection<EventType>();
        List<TraceEventDataViewModel> _tempEvents = new List<TraceEventDataViewModel>(8192);
        DispatcherTimer _updateTimer;

        public IList<TabItemViewModelBase> Tabs => _tabs;

        public IList<EventType> EventTypes => _eventTypes;

        public IList<TraceEventDataViewModel> Events => _events;

        public readonly IUIServices UI;

        public MainViewModel(IUIServices ui) {
            UI = ui;

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            AddEventTypes(
                EventType.ProcessStart, EventType.ProcessStop,
                EventType.ModuleLoad, EventType.ModuleUnload,
                EventType.RegistrySetValue, EventType.RegistryDeleteKey, EventType.RegistryDeleteValue,
                EventType.ThreadStart, EventType.ThreadStop,
                EventType.FileRead, EventType.FileWrite, EventType.FileCreate, EventType.FileRename, EventType.FileDelete,
                EventType.MemoryAlloc, EventType.MemoryFree,
                EventType.AlpcSendMessage, EventType.AlpcReceiveMessage
                );

            HookupEvents();
            Init();

            AddTab(new EventsViewModel(Events), true);

            _updateTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(1500), DispatcherPriority.Background, (_, __) => Update(), 
                Dispatcher.CurrentDispatcher);
            _updateTimer.Start();

        }

        public void AddEventTypes(params EventType[] types) {
            foreach (var type in types)
                _eventTypes.Add(type);
        }

        private void HookupEvents() {
            var dispatcher = Dispatcher.CurrentDispatcher;
            _traceManager.EventTrace += (evt, type) => {
                var info = GetDetails(evt);
                //dispatcher.InvokeAsync(() => _events.Add(new TraceEventDataViewModel(evt, type, info)), DispatcherPriority.Background);
                var data = new TraceEventDataViewModel(evt, type, info);
                lock (_tempEvents)
                    _tempEvents.Add(data);
            };

        }

        private string GetDetails(TraceEvent evt) {
            switch (evt) {
                case ProcessTraceData p:
                    return $"Parent PID: {p.ParentID}; Command Line={p.CommandLine}; Process Flags: {p.Flags}; Bitness: {p.PointerSize * 4}";

                case ThreadTraceData t:
                    return $"Win32 Start Address: 0x{t.Win32StartAddr:X}; Kernel Stack Base: 0x{t.StackBase:X}; User Stack Base: 0x{t.UserStackBase:X}; TEB: 0x{t.TebBase:X}";

                case RegistryTraceData r:
                    return $"Key: {r.KeyName}; Value Name={r.ValueName}; Status={r.Status}; Handle: 0x{r.KeyHandle:X}";

                case ImageLoadTraceData m:
                    return $"Name: {m.FileName}; Address: 0x{m.ImageBase:X}; Base: 0x{m.DefaultBase:X}; size: 0x{m.ImageSize:X}";

                case ALPCSendMessageTraceData alpc:
                    return $"Message ID: {alpc.MessageID}";

                case ALPCReceiveMessageTraceData alpc:
                    return $"Message ID: {alpc.MessageID}";

                case FileIOReadWriteTraceData file:
                    return $"Filename: {file.FileName}; Size: {file.IoSize};";

                case VirtualAllocTraceData mem:
                    return $"Address: 0x{mem.BaseAddr:X}; Size: 0x{mem.Length:X}; Flags: {mem.Flags}";
            }
            return string.Empty;
        }

        bool _suspendUpdates;

        public bool SuspendUpdates {
            get => _suspendUpdates;
            set {
                if (SetProperty(ref _suspendUpdates, value)) {
                    SuspendText = value ? "Suspending UI updates" : string.Empty;
                }
            }
        }

        string _suspendText;

        public string SuspendText {
            get => _suspendText;
            set => SetProperty(ref _suspendText, value);
        }

        private bool _isMonitoring;

        public bool IsMonitoring {
            get => _isMonitoring; 
            set => SetProperty(ref _isMonitoring, value);
        }

        private TabItemViewModelBase _selectedTab;

        public TabItemViewModelBase SelectedTab {
            get => _selectedTab; 
            set => SetProperty(ref _selectedTab, value); 
        }

        public void AddTab(TabItemViewModelBase item, bool activate = false) {
            _tabs.Add(item);
            if (activate)
                SelectedTab = item;
        }

        public DelegateCommandBase GoCommand => new DelegateCommand(
            () => ResumeMonitoring(),
            () => !IsMonitoring)
            .ObservesProperty(() => IsMonitoring);

        public DelegateCommandBase StopCommand => new DelegateCommand(
            () => StopMonitoring(),
            () => IsMonitoring)
            .ObservesProperty(() => IsMonitoring).ObservesProperty(() => IsBusy);

        void Update() {
            _updateTimer.Stop();
            if (!SuspendUpdates) {
                Debug.WriteLine($"{Environment.TickCount} Updating collection");

                var sw = Stopwatch.StartNew();

                lock (_tempEvents) {
                    int count = Math.Min(_tempEvents.Count, IsMonitoring ? 3072 : 8192);
                    for (int i = 0; i < count; i++)
                        _events.Add(_tempEvents[i]);
                    _tempEvents.RemoveRange(0, count);
                    IsBusy = _tempEvents.Count > 0;
                }
                sw.Stop();
                //if (IsMonitoring && sw.ElapsedMilliseconds > 800) {
                //    // update is taking too long, suspend updates
                //    SuspendUpdates = true;
                //}
            }
            RaisePropertyChanged(nameof(LostEvents));
            RaisePropertyChanged(nameof(EventCount));

            _updateTimer.Start();
        }

        private void Init() {
        }

        public string Title => "Process Monitor X v0.1 (C)2018 by Pavel Yosifovich";

        public ICommand ExitCommand => new DelegateCommand(() => Application.Current.Shutdown());

        public DelegateCommandBase ClearAllCommand => new DelegateCommand(() => _events.Clear());

        private bool _isBusy;

        public bool IsBusy {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private async void StopMonitoring() {
            IsBusy = true;
            await Task.Run(() => _traceManager.Stop());
            IsBusy = false;
            IsMonitoring = false;
            SuspendUpdates = false;
        }

        private void ResumeMonitoring() {
            _traceManager.Start(EventTypes, false);
            IsMonitoring = true;
        }

        public int LostEvents => _traceManager.LostEvents;
        public int EventCount => _events.Count + _tempEvents.Count;

    }
}
