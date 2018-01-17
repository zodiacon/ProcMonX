using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;
using ProcMonX.ViewModels.Filters;

namespace ProcMonX.Tracing.Filters {
    [DebuggerDisplay("Active: {IsActive} Include: {Include} Names: {Names}")]
    [Filter("Process Names", ViewModelType = typeof(ProcessNamesFilterViewModel))]
	class ProcessNameFilter : IFilterRule {
		public string[] Names { get; }
		public bool Include { get; set; }

        public bool IsActive { get; set; } = true;

        public ProcessNameFilter(bool include, params string[] names) {
			Names = names.Select(n => n.ToLower()).ToArray();
			Include = include;
		}

		public FilterRuleResult Evaluate(TraceEvent evt) {
			var processName = evt.ProcessName.ToLower();
			if (!Names.Contains(processName))
				return FilterRuleResult.Skip;

			return Include ? FilterRuleResult.Include : FilterRuleResult.Exclude;
		}
	}
}
