using Prism.Mvvm;
using ProcMonX.Models;
using ProcMonX.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcMonX.ViewModels {
    sealed class EventTypeViewModel : BindableBase {
        public EventInfo Info { get; }

        public EventTypeViewModel(EventInfo info) {
            Info = info;
        }

        public string Name => Info.AsString;
        public string Icon => $"/icons/events/{Info.EventType.ToString()}.ico";
        public EventCategory Category => Info.Category;

        public bool IsMonitoring {
            get => _isMonitoring;
            set => SetProperty(ref _isMonitoring, value);
        }

        bool _isMonitoring;
    }
}
