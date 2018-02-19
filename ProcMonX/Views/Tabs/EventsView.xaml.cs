using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using ProcMonX.ViewModels.Tabs;
using Syncfusion.UI.Xaml.ScrollAxis;

namespace ProcMonX.Views.Tabs {
    public partial class EventsView  {
        public EventsView() {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            var vm = e.NewValue as EventsViewModel;
            if (vm != null) {
                var collection = vm.Events as INotifyCollectionChanged;
                Debug.Assert(collection != null);
                collection.CollectionChanged += delegate {
                    if (vm.AutoScroll && vm.Events.Count > 0) {
                        _dataGrid.ScrollInView(new RowColumnIndex(vm.Events.Count - 1, 0));
                    }
                };
            }
        }

    }
}
