using ProcMonX.Tracing.Filters;
using ProcMonX.ViewModels;
using ProcMonX.ViewModels.Filters;
using ProcMonX.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Zodiacon.WPF;

namespace ProcMonX.Tracing {
    static class FilterFactory {
        public static string GetRuleDetails(IFilterRule rule) {
            switch (rule) {
                case ProcessNameFilter filter:
                    return $"Process Names: {string.Join(",", filter.Names)}";

                case ProcessIdFilter filter:
                    return $"Process IDs: {string.Join(",", filter.Pids.Select(pid => pid.ToString()))} Include: {filter.Include}";
            }
            return string.Empty;
        }

        public static FilterTypeViewModel[] GetFilterTypes() {
            var filters = from type in Assembly.GetExecutingAssembly().GetTypes()
                          let filterAttribute = type.GetCustomAttribute<FilterAttribute>()
                          where filterAttribute != null
                          select new FilterTypeViewModel {
                              Name = filterAttribute.Name,
                              Type = type
                          };
            return filters.ToArray();
        }

        public static FilterDialogViewModelBase CreateFilterDialog(FilterTypeViewModel type, IDialogService dialogService) {
            var creator = dialogService.GetType();
            var method = creator.GetMethod("CreateDialog", new Type[] { typeof(object[]) });
            var createMethod = method.MakeGenericMethod(type.Type.GetCustomAttribute<FilterAttribute>().ViewModelType, typeof(FilterDialogWindow));
            var vm = (FilterDialogViewModelBase) createMethod.Invoke(dialogService, new object[1] { new object[0] });
            return vm;
        }

    }
}
