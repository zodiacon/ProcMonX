using Prism.Commands;
using ProcMonX.Models;
using ProcMonX.Tracing;
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

        public ICommand MonitorSelectedCommand => new DelegateCommand(() => MonitorSelected(true));

        public ICommand UnmonitorSelectedCommand => new DelegateCommand(() => MonitorSelected(false));

        public ICommand ToggleSelectionCommand => new DelegateCommand(() => {
            foreach (EventTypeViewModel item in SelectedItems) {
                item.IsMonitoring = !item.IsMonitoring;
            }
        });

        private void MonitorSelected(bool monitor) {
            if (SelectedItems == null)
                return;

            foreach (EventTypeViewModel item in SelectedItems) {
                item.IsMonitoring = monitor;
            }
        }

        public bool IsMonitoring => _mainViewModel.IsMonitoring;
        public bool IsNotMonitoring => !IsMonitoring;

        private void MonitorAll(bool monitor) {
            foreach (var info in _eventTypes)
                info.IsMonitoring = monitor;
        }

        public ICollectionViewAdv View { get; set; }

        public string FilterText {
            get => _filterText;
            set {
                if (SetProperty(ref _filterText, value)) {
                    if (string.IsNullOrWhiteSpace(value))
                        View.Filter = null;
                    else {
                        var text = value.ToLower();
                        View.Filter = obj => {
                            var item = (EventTypeViewModel)obj;
                            return item.Name.ToLower().Contains(text) || item.Category.ToString().ToLower().Contains(text);
                        };
                    }
                    View.RefreshFilter();
                }
            }
        }
        string _filterText;

        public ObservableCollection<object> SelectedItems { get; set; }
    }
}
