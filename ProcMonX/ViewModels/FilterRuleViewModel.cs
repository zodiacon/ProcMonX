using Prism.Mvvm;
using ProcMonX.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMonX.ViewModels {
    sealed class FilterRuleViewModel : BindableBase {
        public IFilterRule Rule { get; set; }
        public readonly FilterTypeViewModel Type;

        public FilterRuleViewModel(FilterTypeViewModel type, IFilterRule rule) {
            Rule = rule;
            Type = type;
            Details = FilterFactory.GetRuleDetails(rule);
        }

        public string Name => Type.Name;

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
        public string Details { get => _details; set => SetProperty(ref _details, value); }

        string _details;

        public void Refresh() {
            Details = FilterFactory.GetRuleDetails(Rule);
        }
    }
}
