using Domain.Models;
using Service.Managers;
using System.Management;

namespace Service.Monitors
{
    public class PrintMonitorService : IDisposable
    {
        private ManagementEventWatcher? _watcher;
        private readonly PrintJobManager _jobManager = new();

        public event EventHandler<PrintJobInfo>? PrintJobDetected;

        public void StartMonitoring()
        {
            var query = new WqlEventQuery("__InstanceCreationEvent", TimeSpan.FromSeconds(1), "TargetInstance ISA 'Win32_PrintJob'");

            _watcher = new ManagementEventWatcher(query);
            _watcher.EventArrived += OnPrintJobArrived;
            _watcher.Start();
        }

        private void OnPrintJobArrived(object sender, EventArrivedEventArgs e)
        {
            var printJob = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            string? name = printJob["Name"]?.ToString(); // format: PrinterName, JobID

            if (string.IsNullOrWhiteSpace(name) || !name.Contains(','))
                return;

            var parts = name.Split(',');
            string printerName = parts[0].Trim();
            if (!int.TryParse(parts[1], out int jobId))
                return;

            var fullInfo = _jobManager.GetJobInfo(printerName, jobId);
            if (fullInfo != null)
            {
                PrintJobDetected?.Invoke(this, fullInfo);
            }
        }

        public void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.Stop();
                _watcher.Dispose();
                _watcher = null;
            }
        }
    }
}
