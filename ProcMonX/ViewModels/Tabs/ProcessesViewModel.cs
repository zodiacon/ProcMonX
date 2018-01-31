using ProcMonX.Models;
using ProcMonX.ViewModels.EventCategories;
using Syncfusion.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMonX.ViewModels.Tabs {
    [TabItem(Text = "Processes", Icon = "/icons/tabs/processes.ico")]
    sealed class ProcessesViewModel : TabItemViewModelBase {
        IEnumerable<TraceEventDataViewModel> _events;
        readonly MainViewModel _mainViewModel;

        public IEnumerable<ProcessTraceEventViewModel> ProcessEvents => _events.Select(evt => new ProcessTraceEventViewModel(evt));

        public ProcessesViewModel(MainViewModel vm, IEnumerable<TraceEventDataViewModel> events) {
            _mainViewModel = vm;
            _events = events.Where(evt => evt.Category.Category == EventCategory.Processes);
            _mainViewModel.PropertyChanged += _mainViewModel_PropertyChanged;
        }

        private void _mainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(MainViewModel.IsMonitoring)) {
                if (_mainViewModel.IsMonitoring)
                    _events = null;
                else
                    _events = _mainViewModel.Events.Where(evt => evt.Category.Category == EventCategory.Processes);
                RaisePropertyChanged(nameof(ProcessEvents));
            }
        }

        public ICollectionViewAdv View { get; set; }

        public override void OnActivate(bool activate) {
                
        }

        public override void OnClose() {
            _mainViewModel.PropertyChanged -= _mainViewModel_PropertyChanged;
        }
    }
}
