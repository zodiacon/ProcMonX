using ProcMonX.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Zodiacon.WPF;

namespace ProcMonX.ViewModels {
    abstract class FilterDialogViewModelBase : DialogViewModelBase {
        protected FilterDialogViewModelBase(Window dialog) : base(dialog) {
            dialog.Owner = Application.Current.MainWindow;
        }

        public IFilterRule Filter { get; set; }

        bool _include = true;
        public bool Include { get => _include; set => SetProperty(ref _include, value); }

        public ResizeMode ResizeMode => ResizeMode.NoResize;

        public SizeToContent SizeToContent => SizeToContent.WidthAndHeight;

        public virtual void Refresh() { }
    }
}
