using Domain.Models;
using Service.Managers;
using Service.Monitors;
using System.Collections.ObjectModel;

namespace Service
{
    public class PrintJobCoordinator : IDisposable
    {
        private readonly PrintMonitorService _monitorService = new();
        private readonly PrintJobManager _jobManager = new();

        private readonly TimeSpan _groupingTimeout = TimeSpan.FromSeconds(10);
        private readonly Dictionary<string, (DateTime timestamp, Guid groupId)> _recentJobs = new();

        private readonly Action<PrintJobInfo> _jobCallback;

        public ObservableCollection<PrintJobInfo> Jobs { get; } = new();

        public PrintJobCoordinator(Action<PrintJobInfo> onJobDiscovered)
        {
            _jobCallback = onJobDiscovered;

            _monitorService.PrintJobDetected += OnPrintJobDetected;
            _monitorService.StartMonitoring();
        }

        private void OnPrintJobDetected(object? sender, PrintJobInfo job)
        {
            string key = $"{job.User}_{job.DocumentName}";

            if (_recentJobs.TryGetValue(key, out var entry) &&
                (DateTime.Now - entry.timestamp) <= _groupingTimeout)
            {
                job.GroupId = entry.groupId;
            }
            else
            {
                job.GroupId = Guid.NewGuid();
                _recentJobs[key] = (DateTime.Now, job.GroupId);
            }

            _jobCallback.Invoke(job);
        }

        public void Pause(PrintJobInfo job)
        {
            if (int.TryParse(job.JobId, out int id))
                _jobManager.PauseJob(job.PrinterName, id);
        }

        public void Resume(PrintJobInfo job)
        {
            if (int.TryParse(job.JobId, out int id))
                _jobManager.ResumeJob(job.PrinterName, id);
        }

        public void Cancel(PrintJobInfo job)
        {
            if (int.TryParse(job.JobId, out int id))
            {
                _jobManager.CancelJob(job.PrinterName, id);
                Jobs.Remove(job);
            }
        }

        public void Dispose() => _monitorService.Dispose();
    }
}
