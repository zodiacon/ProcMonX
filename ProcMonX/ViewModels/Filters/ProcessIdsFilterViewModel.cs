using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProcMonX.ViewModels.Filters {
    sealed class ProcessIdsFilterViewModel : FilterDialogViewModelBase {
        public ProcessIdsFilterViewModel(Window dialog) : base(dialog) {
        }

        public string Title => "Process IDs Filter";

    }
}
