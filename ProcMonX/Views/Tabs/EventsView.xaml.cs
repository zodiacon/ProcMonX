using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProcMonX.ViewModels.Tabs;

namespace ProcMonX.Views.Tabs {
    /// <summary>
    /// Interaction logic for EventsView.xaml
    /// </summary>
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
                    if (vm.AutoScroll && vm.Events.Count > 0)
                        _dataGrid.ScrollInView(new Syncfusion.UI.Xaml.ScrollAxis.RowColumnIndex(vm.Events.Count - 1, 0));
                };
            }
        }

    }
}
