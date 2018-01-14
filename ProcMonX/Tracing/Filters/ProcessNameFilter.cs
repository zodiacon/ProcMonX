using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;

namespace ProcMonX.Tracing.Filters {
	class ProcessNameFilter : IFilterRule {
		public string[] Names { get; }
		public bool Include { get; }

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
