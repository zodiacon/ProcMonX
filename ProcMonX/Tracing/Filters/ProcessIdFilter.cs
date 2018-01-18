using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;

namespace ProcMonX.Tracing.Filters {
    [DebuggerDisplay("Active: {IsActive} Include: {Include} Names: {Pids}")]
    [Filter("Process IDs")]
    sealed class ProcessIdFilter : IFilterRule {
		public int[] Pids { get; }
		public bool Include { get; set; }

        public string Name => "Process IDs";

        public bool IsActive { get; set; } = true;

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
