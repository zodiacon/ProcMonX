using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProcMonX.ViewModels {
    [AttributeUsage(AttributeTargets.Class)]
    sealed class TabItemAttribute : Attribute {
        public string Text { get; set; }
        public string Icon { get; set; }
    }

    abstract class TabItemViewModelBase : BindableBase {
        string _text, _icon;

        public string Text { get => _text; set => SetProperty(ref _text, value); }
        public string Icon { get => _icon; set => SetProperty(ref _icon, value); }

        protected TabItemViewModelBase() {
            var tabItemAttribute = GetType().GetCustomAttribute<TabItemAttribute>();
            if (tabItemAttribute != null) {
                Text = tabItemAttribute.Text;
                Icon = tabItemAttribute.Icon;
            }
        }

        internal virtual bool CanClose => true;

        public virtual void Refresh() { }
    }
}
