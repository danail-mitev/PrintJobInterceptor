using PrintJobInterceptor.ViewModels;
using Service.Monitors;
using System.Windows;

namespace PrintJobInterceptor
{
    public partial class MainWindow : Window
    {
        private readonly PrintMonitorService _monitor = new();

        public MainWindow()
        {
            InitializeComponent();

            //_monitor.PrintJobDetected += (s, job) =>
            //{
            //    Dispatcher.Invoke(() =>
            //    {
            //        MessageBox.Show($"Detected: {job.DocumentName} by {job.User} on {job.PrinterName}");
            //    });
            //};
            //
            //_monitor.StartMonitoring();
            DataContext = new ViewModelMain();
        }
    }
}