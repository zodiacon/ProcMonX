using Prism.Commands;
using ProcMonX.Tracing;
using ProcMonX.ViewModels.Filters;
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
            var vm = FilterFactory.CreateFilterDialog(type, UI.DialogService);
            if (vm == null) {
                UI.MessageBoxService.ShowMessage("Filter type UI not yet implemented", App.Title);
                return;
            }
            if (vm.ShowDialog() == true) {
                _filters.Add(new FilterRuleViewModel(type, vm.Filter));
                _filter.FilterRules.Add(vm.Filter);
                RaisePropertyChanged(nameof(SelectedItems));
            }
        });

        ObservableCollection<object> _selectedItems = new ObservableCollection<object>();
        public ObservableCollection<object> SelectedItems {
            get => _selectedItems;
            set {
                if (_selectedItems == null)
                    _selectedItems.Clear();
                else
                    _selectedItems = value;
                RaisePropertyChanged(nameof(SelectedItems));
            }
        }

        FilterRuleViewModel _selectedItem;

        public ICommand DeleteCommand => new DelegateCommand(() => {
            var items = SelectedItems.OfType<FilterRuleViewModel>().ToArray();
            foreach (var filter in items) {
                _filter.FilterRules.Remove(filter.Rule);
                _filters.Remove(filter);
            }
            RaisePropertyChanged(nameof(SelectedItems));
        }, () => SelectedItems.OfType<FilterRuleViewModel>().Any()).ObservesProperty(() => SelectedItems);

        public ICommand EditCommand => new DelegateCommand(() => {
            var filterItem = SelectedItem;

            var vm = FilterFactory.CreateFilterDialog(filterItem.Type, UI.DialogService);
            vm.Filter = filterItem.Rule;
            vm.Include = filterItem.Include;
            vm.Refresh();

            if (vm.ShowDialog() == true) {
                filterItem.Rule = vm.Filter;
                filterItem.Include = vm.Include;
                filterItem.Refresh();
            }

        }, () => SelectedItem != null).ObservesProperty(() => SelectedItem);

        public bool DefaultResult {
            get => _filter.DefaultResult == FilterRuleResult.Include;
            set {
                _filter.DefaultResult = value ? FilterRuleResult.Include : FilterRuleResult.Exclude;
                RaisePropertyChanged(nameof(DefaultResult));
            }
        }

        public FilterRuleViewModel SelectedItem {
            get => _selectedItem;
            set {
                if (SetProperty(ref _selectedItem, value)) {
                    RaisePropertyChanged(nameof(SelectedItems));
                }
            }
        }
    }
}
