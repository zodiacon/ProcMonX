using ProcMonX.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Zodiacon.WPF;

namespace ProcMonX.ViewModels.Filters {
    abstract class FilterDialogViewModelBase : DialogViewModelBase {
        protected FilterDialogViewModelBase(Window dialog) : base(dialog) {
            dialog.Owner = Application.Current.MainWindow;
        }

        public IFilterRule Filter { get; protected set; }

        public ResizeMode ResizeMode => ResizeMode.NoResize;

        public SizeToContent SizeToContent => SizeToContent.WidthAndHeight;
    }
}
