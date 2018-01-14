using Prism.Commands;
using ProcMonX.Models;
using Syncfusion.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Zodiacon.WPF;

namespace ProcMonX.ViewModels.Tabs {
    [TabItem(Text = "Capture Settings", Icon = "/icons/tabs/camera.ico")]
    sealed class CaptureViewModel : TabItemViewModelBase {
        EventTypeViewModel[] _eventTypes;
        PropertyFollower<MainViewModel, CaptureViewModel> _isMonitoringProperty;
        MainViewModel _mainViewModel;

        public CaptureViewModel(MainViewModel vm) {
            _isMonitoringProperty = new PropertyFollower<MainViewModel, CaptureViewModel>(vm, this, nameof(IsMonitoring));
            _isMonitoringProperty.Add(nameof(IsMonitoring), _ => RaisePropertyChanged(nameof(IsNotMonitoring)));
            _mainViewModel = vm;
            _eventTypes = EventInfo.AllEvents.Select(info => new EventTypeViewModel(info)).ToArray();
        }

        internal override bool CanClose => false;

        public IEnumerable<EventTypeViewModel> EventTypes => _eventTypes;

        public ICommand MonitorAllCommand => new DelegateCommand(() => MonitorAll(true), () => !IsMonitoring).ObservesProperty(() => IsMonitoring);

        public ICommand MonitorNoneCommand => new DelegateCommand(() => MonitorAll(false), () => !IsMonitoring).ObservesProperty(() => IsMonitoring);

        public bool IsMonitoring => _mainViewModel.IsMonitoring;
        public bool IsNotMonitoring => !IsMonitoring;

        private void MonitorAll(bool monitor) {
            foreach (var info in _eventTypes)
                info.IsMonitoring = monitor;
        }

        public ICollectionViewAdv View { get; set; }
    }
}
