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

        public ObservableCollection<PrintJobInfo> Jobs { get; } = new();

        public PrintJobCoordinator()
        {
            _monitorService.PrintJobDetected += OnPrintJobDetected;
            _monitorService.StartMonitoring();
        }

        private void OnPrintJobDetected(object? sender, PrintJobInfo job)
        {
            App.Current.Dispatcher.Invoke(() =>
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

                if (!Jobs.Any(j => j.JobId == job.JobId && j.PrinterName == job.PrinterName))
                {
                    Jobs.Add(job);
                }
            });
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

        public void Dispose()
        {
            _monitorService.Dispose();
        }
    }
}
