using Prism.Mvvm;
using ProcMonX.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMonX.ViewModels {
    sealed class FilterRuleViewModel : BindableBase {
        public readonly IFilterRule Rule;
        FilterTypeViewModel _type;

        public FilterRuleViewModel(FilterTypeViewModel type, IFilterRule rule) {
            Rule = rule;
            _type = type;
            Details = FilterFactory.GetRuleDetails(rule);
        }

        public string Name => _type.Name;

        public bool IsActive {
            get => Rule.IsActive;
            set {
                Rule.IsActive = value;
                RaisePropertyChanged(nameof(IsActive));
            }
        }

        public bool Include {
            get => Rule.Include;
            set {
                Rule.Include = value;
                RaisePropertyChanged(nameof(Include));
            }
        }

        public string Icon { get; }
        public string Details { get; }
    }
}
