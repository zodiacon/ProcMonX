using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
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
	}
}
