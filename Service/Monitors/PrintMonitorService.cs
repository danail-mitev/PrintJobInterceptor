using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Service.Monitors
{
    public class PrintMonitorService : IDisposable
    {
        private ManagementEventWatcher? _watcher;

        public event EventHandler<PrintJobInfo>? PrintJobDetected;

        public void StartMonitoring()
        {
            var query = new WqlEventQuery("__InstanceCreationEvent",
                TimeSpan.FromSeconds(1),
                "TargetInstance ISA 'Win32_PrintJob'");

            _watcher = new ManagementEventWatcher(query);
            _watcher.EventArrived += OnPrintJobArrived;
            _watcher.Start();
        }

        private void OnPrintJobArrived(object sender, EventArrivedEventArgs e)
        {
            var printJob = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            var jobInfo = new PrintJobInfo
            {
                JobId = printJob["JobId"]?.ToString() ?? "",
                DocumentName = printJob["Document"]?.ToString() ?? "",
                PrinterName = printJob["Name"]?.ToString()?.Split(',')[0] ?? "",
                User = printJob["Owner"]?.ToString() ?? "",
                TotalPages = int.TryParse(printJob["TotalPages"]?.ToString(), out int pages) ? pages : 0,
                Status = printJob["Status"]?.ToString() ?? "",
                TimeSubmitted = DateTime.Now
            };

            PrintJobDetected?.Invoke(this, jobInfo);
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
