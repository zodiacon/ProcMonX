using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;

namespace ProcMonX.Tracing.Filters {
	class ProcessIdFilter : IFilterRule {
		public int[] Pids { get; }
		public bool Include { get; }

		public ProcessIdFilter(bool include, params int[] pids) {
			Pids = pids;
			Include = include;
		}

		public FilterRuleResult Evaluate(TraceEvent evt) {
			if (!Pids.Contains(evt.ProcessID))
				return FilterRuleResult.Skip;

			return Include ? FilterRuleResult.Include : FilterRuleResult.Exclude;
		}
	}
}
