using ProcMonX.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Zodiacon.WPF;

namespace ProcMonX {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public const string Title = "Process Monitor X";

        public App() {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            ShowFatalError((Exception)e.ExceptionObject);
        }

        private void ShowFatalError(Exception ex) {
            MessageBox.Show($"Fatal error: {ex.Message}.\n{ex.StackTrace}", Title);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            ShowFatalError(e.Exception);
        }

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            var ui = new UIServicesDefaults();
            var vm = new MainViewModel(ui);
            var win = new MainWindow();
            ui.MessageBoxService.SetOwner(win);
            win.DataContext = vm;
            win.Show();
        }

    }
}
