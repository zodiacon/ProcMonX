using CsvHelper;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using Prism.Commands;
using Prism.Mvvm;
using ProcMonX.Models;
using ProcMonX.Tracing;
using ProcMonX.Tracing.Filters;
using ProcMonX.ViewModels.Tabs;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
        public readonly TraceManager TraceManager = new TraceManager();
        ObservableCollection<TabItemViewModelBase> _tabs = new ObservableCollection<TabItemViewModelBase>();
        ObservableCollection<TraceEventDataViewModel> _events = new ObservableCollection<TraceEventDataViewModel>();
        Dictionary<string, TabItemViewModelBase> _views = new Dictionary<string, TabItemViewModelBase>(8);

        EventType[] _eventTypes;
        List<TraceEventDataViewModel> _tempEvents = new List<TraceEventDataViewModel>(8192);
        DispatcherTimer _updateTimer;
        CaptureViewModel _captureSettings;
        CaptureFilterViewModel _filterSettings;
        EventsViewModel _allEventsViewModel;

        public Options Options { get; } = new Options();

        public IList<TabItemViewModelBase> Tabs => _tabs;

        public EventType[] EventTypes => _eventTypes;

        public IList<TraceEventDataViewModel> Events => _events;

        public readonly IUIServices UI;

        public MainViewModel(IUIServices ui) {
            UI = ui;

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            HookupEvents();

            _captureSettings = new CaptureViewModel(this);
            AddTab(_captureSettings, true);

            _filterSettings = new CaptureFilterViewModel(this);
            AddTab(_filterSettings);

            AddTab(_allEventsViewModel = new EventsViewModel(this, Events));
            _views.Add(_allEventsViewModel.Text, _allEventsViewModel);
            _views.Add(_captureSettings.Text, _captureSettings);

            _updateTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(1500), DispatcherPriority.Background, (_, __) => Update(), 
                Dispatcher.CurrentDispatcher);
            _updateTimer.Start();

        }

        private void HookupEvents() {
            var dispatcher = Dispatcher.CurrentDispatcher;
            TraceManager.EventTrace += (evt, type) => {
                var info = GetDetails(evt);
                var data = new TraceEventDataViewModel(evt, type, info);
                lock (_tempEvents)
                    _tempEvents.Add(data);
            };

        }

        private string GetDetails(TraceEvent evt) {
            switch (evt) {
                case ProcessTraceData data:
                    return $"Parent PID:;; {data.ParentID};; Flags:;; {data.Flags};; Image Path:;; {data.ImageFileName};; Command Line:;; {data.CommandLine}";

                case ThreadTraceData data:
                    return $"Win32 Start Address:;; 0x{data.Win32StartAddr:X};; Kernel Stack Base:;; 0x{data.StackBase:X}" + 
                        $" User Stack Base:;; 0x{data.UserStackBase:X};; TEB:;; 0x{data.TebBase:X};; Parent PID:;; {data.ParentProcessID}";

                case RegistryTraceData data:
                    return $"Key:;; {data.KeyName};; Value Name:;; {data.ValueName};; Status:;; 0x{data.Status:X};; Handle:;; 0x{data.KeyHandle:X}";

                case ImageLoadTraceData data:
                    return $"Name:;; {data.FileName};; Address:;; 0x{data.ImageBase:X};; Base:;; 0x{data.DefaultBase:X};; size:;; 0x{data.ImageSize:X}";

                case ALPCSendMessageTraceData data:
                    return $"Message ID: ;;{data.MessageID}";

                case ALPCReceiveMessageTraceData alpc:
                    return $"Message ID: ;;{alpc.MessageID}";

                case ALPCWaitForReplyTraceData data:
                    return $"Message ID:;; {data.MessageID}";

                case ALPCWaitForNewMessageTraceData data:
                    return $"Server:;; {Convert.ToBoolean(data.IsServerPort)};; Port Name:;; {data.PortName}";

                case FileIOReadWriteTraceData data:
                    return $"Filename:;; {data.FileName};; Offset:;; {data.Offset:X};; Size:;; 0x{data.IoSize:X};; IRP:;; 0x{data.IrpPtr:X}";

                case FileIOSimpleOpTraceData data:
                    return $"Filename:;; {data.FileName};; File Object:;; 0x{data.FileObject:X};; IRP:;; 0x{data.IrpPtr:X}";

                case FileIOCreateTraceData data:
                    return $"Attributes:;; {data.FileAttributes};; Options:;; {data.CreateOptions};; Sharing:;; {data.ShareAccess};; File Object:;; 0x{data.FileObject:X};; IRP:;; 0x{data.IrpPtr}";

                case VirtualAllocTraceData data:
                    return $"Address:;; 0x{data.BaseAddr:X};; Size:;; 0x{data.Length:X};; Flags:;; {(VirtualAllocFlags)(data.Flags)}";

                case TcpIpConnectTraceData data:
                    return $"Src Address:;; {data.saddr.ToString()};; Dst Address:;; {data.daddr};; Dst Port:;; {data.dport};; Src Port:;; {data.sport};; Connection ID:;; {data.connid}";

                case TcpIpTraceData data:
                    return $"Src Address:;; {data.saddr.ToString()};; Dst Address:;; {data.daddr};; Dst Port:;; {data.dport};; Src Port:;; {data.sport};; Size:;; {data.size};; Connection ID:;; {data.connid}";

                case TcpIpV6TraceData data:
                    return $"Src Address:;; {data.saddr.ToString()};; Dst Address:;; {data.daddr};; Dst Port:;; {data.dport};; Src Port:;; {data.sport};; Size:;; {data.size};; Connection ID:;; {data.connid}";

                case TcpIpSendTraceData data:
                    return $"Src Address:;; {data.saddr.ToString()};; Dst Address:;; {data.daddr};; Dst Port:;; {data.dport};; Src Port:;; {data.sport};;" + 
                        $" Size:;; {data.size};; Seq:;; {data.seqnum};; Start:;; {data.startime};; End:;; {data.endtime};; Connection ID:;; {data.connid}";

                case TcpIpV6SendTraceData data:
                    return $"Src Address:;; {data.saddr.ToString()};; Dst Address:;; {data.daddr};; Dst Port:;; {data.dport};; Src Port:;; {data.sport};;" +
                        $" Size:;; {data.size};; Seq:;; {data.seqnum};; Start:;; {data.startime};; End:;; {data.endtime};; Connection ID:;; {data.connid}";

                case DiskIOTraceData data:
                    return $"Disk:;; {data.DiskNumber};; Offset:;; {data.ByteOffset};; Size:;; {data.TransferSize};; Priority:;; {data.Priority};; IRP:;;" + 
                        $" 0x{data.Irp:X};; IRP Flags:;; {data.IrpFlags};; File Key:;; 0x{data.FileKey:X};; Filename:;; {data.FileName}";

                case MapFileTraceData data:
                    return $"Filename:;; {data.FileName};; View Base:;; 0x{data.ViewBase:X};; Offset:;; 0x{data.ByteOffset:X};; Size:;; 0x{data.ViewSize:X}";

                case FileIONameTraceData data:
                    return $"Filename:;; {data.FileName};; File Key:;; 0x{data.FileKey:X}";

                case DriverMajorFunctionCallTraceData data:
                    return $"Major:;; {data.MajorFunction};; Minor:;; {data.MinorFunction};; IRP:;; 0x{data.Irp:X};; Routine:;; 0x{data.RoutineAddr:X};; Unique ID:;; 0x{data.UniqMatchID:X}";

                case MemInfoTraceData data:
                    return $"Zero Pages:;; {data.ZeroPageCount};; Free Pages:;; {data.FreePageCount};; Modified Pages:;; {data.ModifiedPageCount};; Modified No Write Pages:;; {data.ModifiedNoWritePageCount};; Bad Pages:;; {data.BadPageCount}";

                case MemoryPageAccessTraceData data:
                    return $"Page Kind:;; {data.PageKind};; Page List:;; {data.PageList};; PFN:;; {data.PageFrameIndex};; Virtual Address:;; 0x{data.VirtualAddress:X};; File Key:;; {data.FileKey:X};; Filename:;; {data.FileName}";

                case MemorySystemMemInfoTraceData data:
                    return $"Free Pages: {data.FreePages}";

                case MemoryPageFaultTraceData data:
                    return $"Virtual Address:;; 0x{data.VirtualAddress};; Program Counter:;; 0x{data.ProgramCounter}";

            }
            var sb = new StringBuilder(128);
            foreach (var name in evt.PayloadNames)
                sb.Append(name).Append(":;; ").Append(evt.PayloadStringByName(name)).Append(";; ");
            return sb.ToString();
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
            set {
                if (SetProperty(ref _isMonitoring, value)) {
                    RaisePropertyChanged(nameof(IsNotMonitoring));
                }
            }
        }

        public bool IsNotMonitoring {
            get => !IsMonitoring;
            set => IsMonitoring = !value;
        }

        private TabItemViewModelBase _selectedTab;

        public TabItemViewModelBase SelectedTab {
            get => _selectedTab; 
            set {
                var current = _selectedTab;
                if (SetProperty(ref _selectedTab, value)) {
                    current?.OnActivate(false);
                    value?.OnActivate(true);
                }
            }
        }

        public TabItemViewModelBase AddTab(TabItemViewModelBase item, bool activate = false) {
            _tabs.Add(item);
            if (activate)
                SelectedTab = item;
            return item;
        }

        public void RemoveTab(TabItemViewModelBase tab) {
            _tabs.Remove(tab);
        }

        public ICommand ViewTabCommand => new DelegateCommand<string>(name => {
            if (_views.TryGetValue(name, out var view))
                SelectedTab = view;
            else {
                var tab = CreateTab(name);
                if (tab != null)
                    _views.Add(tab.Text, tab);
            }
        }, name => !IsMonitoring).ObservesProperty(() => IsMonitoring);      

        private TabItemViewModelBase CreateTab(string name) {
            TabItemViewModelBase tab = null;
            switch (name) {
                case "Processes":
                    tab = new ProcessesViewModel(this, Events);
                    break;
            }

            if (tab != null)
                AddTab(tab, true);
            return tab;
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
            var sw = Stopwatch.StartNew();
            if (!SuspendUpdates) {
                lock (_tempEvents) {
                    int count = Math.Min(_tempEvents.Count, IsMonitoring ? 3072 : 8192);
                    for (int i = 0; i < count; i++)
                        _events.Add(_tempEvents[i]);
                    _tempEvents.RemoveRange(0, count);
                    IsBusy = _tempEvents.Count > 0;
                }
            }
            sw.Stop();
            if (sw.ElapsedMilliseconds > 800) {
                SuspendUpdates = true;
            }
            RaisePropertyChanged(nameof(LostEvents));
            RaisePropertyChanged(nameof(EventCount));

            _updateTimer.Start();
        }

        public string Title => $"{App.Title} v0.22 Beta (C)2017-2018 by Pavel Yosifovich";

        public ICommand ExitCommand => new DelegateCommand(() => Application.Current.Shutdown());

        public DelegateCommandBase ClearAllCommand => new DelegateCommand(() => _events.Clear());

        private bool _isBusy;

        public bool IsBusy {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private void StopMonitoring() {
            TraceManager.Stop();
            IsMonitoring = false;
            SuspendUpdates = false;
        }

        private void ResumeMonitoring() {
            _eventTypes = _captureSettings.EventTypes.Where(type => type.IsMonitoring).Select(type => type.Info.EventType).ToArray();
            if (_eventTypes.Length == 0) {
                UI.MessageBoxService.ShowMessage("No events selected to monitor", App.Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            SelectedTab = _allEventsViewModel;

            TraceManager.Start(EventTypes);
            IsMonitoring = true;
        }

        public int LostEvents => TraceManager.LostEvents;
        public int EventCount => _events.Count + _tempEvents.Count;

        public ICommand TabClosingCommand => new DelegateCommand<CancelingRoutedEventArgs>(args => {
            if (!SelectedTab.CanClose)
                args.Cancel = true;
        });

        public ICommand TabClosedCommand => new DelegateCommand<CloseTabEventArgs>(args => {
            var tab = args.TargetTabItem.DataContext as TabItemViewModelBase;
            tab.OnClose();
            _views.Remove(tab.Text);
        });

        public ICommand SaveCommand => new DelegateCommand(() => {
            var filename = UI.FileDialogService.GetFileForSave("CSV Files (*.csv)|*.csv", "Select File");
            if (filename == null)
                return;

            SaveInternal(filename);
        }, () => !IsMonitoring).ObservesProperty(() => IsMonitoring);

        public ICommand AlwaysOnTopCommand => new DelegateCommand<FrameworkElement>(element => {
            var window = Window.GetWindow(element);
            window.Topmost = Options.AlwaysOnTop;
        });

        public bool AutoScroll {
            get => SelectedTab != null ? SelectedTab.AutoScroll : false;
            set {
                if (SelectedTab != null)
                    SelectedTab.AutoScroll = value;
            }
        }

        private void SaveInternal(string filename) {
            using (var writer = new StreamWriter(filename, append: false, encoding: Encoding.Unicode)) {
                using (var csvWriter = new CsvWriter(writer)) {
                    var data = _events.Select(evt => new EventData {
                        Index = evt.Index,
                        ProcessId = evt.ProcessId,
                        ProcessName = evt.ProcessName,
                        Details = evt.Details.Replace(";;", string.Empty),
                        ThreadId = evt.ThreadId,
                        CPU = evt.Processor,
                        Opcode = evt.Opcode,
                        EventType = evt.TypeAsString,
                        Category = evt.Category.ToString(),
                        Time = evt.TimeStamp
                    });

                    csvWriter.WriteHeader<EventData>();
                    csvWriter.NextRecord();
                    foreach (var evt in data) {
                        csvWriter.WriteRecord(evt);
                        csvWriter.NextRecord();
                    }
                }
            }
        }
    }
}
