using ProcMonX.Tracing.Filters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProcMonX.ViewModels.Filters {
    sealed class ProcessNamesFilterViewModel : FilterDialogViewModelBase {
        public string[] ProcessNames { get; private set; }

        public ProcessNamesFilterViewModel(Window dialog) : base(dialog) {
            CanExecuteOKCommand = () => Names.Length > 0;
            OKCommand = OKCommand.ObservesProperty(() => Names);
        }

        public string Title => "Process Names Filter";

        public string Names { get => _names; set => SetProperty(ref _names, value); }
        public bool Include { get => _include; set => SetProperty(ref _include, value); }

        string _names = string.Empty;
        bool _include = true;

        protected override void OnOK() {
            ProcessNames = Names.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var rule = new ProcessNameFilter(Include, ProcessNames);
            Filter = rule;

            base.OnOK();
        }
    }
}
