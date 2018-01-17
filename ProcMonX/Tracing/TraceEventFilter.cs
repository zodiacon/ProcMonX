using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMonX.Tracing {
    enum FilterRuleResult {
        Skip,
        Include,
        Exclude
    }

    enum CompareType {
        Equals,
        NotEquals,
        Contains,
        NotContains
    }

    interface IFilterRule {
        FilterRuleResult Evaluate(TraceEvent evt);
        bool IsActive { get; set; }
        bool Include { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    sealed class FilterAttribute : Attribute {
        public string Name { get; }

        public FilterAttribute(string name) {
            Name = name;
        }

        public string Description { get; set; }
    }

    class TraceEventFilter {
        ObservableCollection<IFilterRule> _filterRules = new ObservableCollection<IFilterRule>();

        public FilterRuleResult DefaultResult { get; set; } = FilterRuleResult.Include;

        public IList<IFilterRule> FilterRules => _filterRules;

        public virtual FilterRuleResult EvaluateEvent(TraceEvent evt) {
            if (FilterRules.Count == 0)
                return FilterRuleResult.Include;

            foreach (var rule in FilterRules) {
                if (rule.IsActive) {
                    var result = rule.Evaluate(evt);
                    if (result != FilterRuleResult.Skip)
                        return result;
                }
            }
            return DefaultResult;
        }
    }
}
