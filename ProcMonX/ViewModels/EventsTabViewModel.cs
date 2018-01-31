using Microsoft.Diagnostics.Tracing;
using ProcMonX.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.ComponentModel;

namespace ProcMonX.ViewModels {
	enum TabKind {
		None = 0,
		AllEvents,
		Processes,
		Threads,
		Modules,
		Alpc,
		Registry,
		Files,
		Memory,
		Driver,
		Network,
		Custom = 99
	}

	class EventsTabViewModel : TabViewModelBase {
		readonly CollectionViewSource _cvs;
		Func<TraceEventDataViewModel, bool> _baseFilter;
		ListCollectionView _view;

		public TabKind Kind { get; }

		public ICollectionView Items => _view;

		public EventsTabViewModel(TabKind kind, IEnumerable<TraceEventDataViewModel> events, Func<TraceEventDataViewModel, bool> filter = null) {
			Kind = kind;
			_baseFilter = filter;
			_cvs = new CollectionViewSource() {
				Source = events
			};
			_cvs.View.Filter = filter == null ? default(Predicate<object>) : o => filter((TraceEventDataViewModel)o);
			_view = _cvs.View as ListCollectionView;
		}

		public IEnumerable<TraceEventDataViewModel> Events { get; }

		private string _searchText, _lowerSearchText;

		public string SearchText {
			get => _searchText;
			set {
				if (SetProperty(ref _searchText, value)) {
					_lowerSearchText = value.ToLower();
					if (string.IsNullOrWhiteSpace(value)) {
						if (_baseFilter == null)
							_cvs.View.Filter = null;
						else
							_cvs.View.Filter = obj => _baseFilter((TraceEventDataViewModel)obj);
					}
					else
						_cvs.View.Filter = obj => {
							var vm = (TraceEventDataViewModel)obj;
							if (_baseFilter != null && !_baseFilter(vm))
								return false;
							return vm.Data.ProcessName.ToLower().Contains(_lowerSearchText)
								|| vm.MoreInfo.ToLower().Contains(_lowerSearchText);
						};
				}
			}
		}

		public bool IsGeneric => Kind == TabKind.Custom || Kind == TabKind.AllEvents;

		public bool IsProcessesOnly => Kind == TabKind.Processes;
		public bool IsThreadsOnly => Kind == TabKind.Threads;
		public bool IsModulesOnly => Kind == TabKind.Modules;
		public bool IsRegistryOnly => Kind == TabKind.Registry;
		public bool IsAlpcOnly => Kind == TabKind.Alpc;

		public int EventCount => _view.Count;

		public void Refresh() {
			RaisePropertyChanged(nameof(EventCount));
		}
	}

}
