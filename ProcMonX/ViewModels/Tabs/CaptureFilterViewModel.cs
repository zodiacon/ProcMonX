using Prism.Commands;
using ProcMonX.Tracing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Zodiacon.WPF;

namespace ProcMonX.ViewModels.Tabs {
    [TabItem(Text = "Filters", Icon = "/icons/tabs/filter.ico")]
    sealed class CaptureFilterViewModel : TabItemViewModelBase {
        TraceManager _traceManager;
        TraceEventFilter _filter = new TraceEventFilter();
        ObservableCollection<FilterRuleViewModel> _filters = new ObservableCollection<FilterRuleViewModel>();
        IUIServices UI;

        internal override bool CanClose => false;

        public CaptureFilterViewModel(MainViewModel vm) {
            _traceManager = vm.TraceManager;
            UI = vm.UI;
            _traceManager.Filter = _filter;
        }

        public IEnumerable<FilterRuleViewModel> Filters => _filters;

        FilterTypeViewModel[] _filterTypes;
        public FilterTypeViewModel[] FilterTypes {
            get {
                if (_filterTypes == null)
                    _filterTypes = FilterFactory.GetFilterTypes();
                return _filterTypes;
            }
        }

        public ICommand NewFilterCommand => new DelegateCommand<FilterTypeViewModel>(type => {
            Debug.Assert(type != null);

            var vm = FilterFactory.CreateFilterDialog(type, UI.DialogService);
            if (vm.ShowDialog() == true) {
                _filters.Add(new FilterRuleViewModel(type, vm.Filter));
                _filter.FilterRules.Add(vm.Filter);
            }
        });

        ObservableCollection<object> _selectedItems = new ObservableCollection<object>();
        public ObservableCollection<object> SelectedItems {
            get => _selectedItems;
            set {
                _selectedItems = value;
                RaisePropertyChanged(nameof(SelectedItems));
            }
        }

        public ICommand DeleteCommand => new DelegateCommand(() => {
            var items = SelectedItems.Cast<FilterRuleViewModel>().ToArray();
            foreach (var filter in items) {
                _filter.FilterRules.Remove(filter.Rule);
                _filters.Remove(filter);
            }
        }, () => SelectedItems.Count > 0).ObservesProperty(() => SelectedItems);

        public ICommand EditCommand => new DelegateCommand(() => {
            if (SelectedItems.Count != 1)
                return;

            // TODO: edit filter
        }, () => SelectedItems.Count == 1).ObservesProperty(() => SelectedItems);

        public bool DefaultResult {
            get => _filter.DefaultResult == FilterRuleResult.Include;
            set {
                _filter.DefaultResult = value ? FilterRuleResult.Include : FilterRuleResult.Exclude;
                RaisePropertyChanged(nameof(DefaultResult));
            }
        }
    }
}
