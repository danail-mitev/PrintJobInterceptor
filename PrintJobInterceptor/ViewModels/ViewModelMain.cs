using Domain.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Service.Managers;
using Service.Monitors;

namespace PrintJobInterceptor.ViewModels
{
    public class ViewModelMain : ViewModelBase
    {
        private readonly PrintMonitorService _monitorService = new();
        private readonly PrintJobManager _jobManager = new();

        public ObservableCollection<PrintJobInfo> PrintJobs { get; } = [];

        private PrintJobInfo? _selectedJob;
        public PrintJobInfo? SelectedJob
        {
            get => _selectedJob;
            set
            {
                _selectedJob = value;
                OnPropertyChanged();
            }
        }

        public ICommand PauseCommand { get; }
        public ICommand ResumeCommand { get; }
        public ICommand CancelCommand { get; }

        public ViewModelMain()
        {
            PauseCommand = new RelayCommand(_ => PauseSelectedJob(), _ => SelectedJob != null);
            ResumeCommand = new RelayCommand(_ => ResumeSelectedJob(), _ => SelectedJob != null);
            CancelCommand = new RelayCommand(_ => CancelSelectedJob(), _ => SelectedJob != null);

            _monitorService.PrintJobDetected += OnPrintJobDetected;
            _monitorService.StartMonitoring();
        }

        private void OnPrintJobDetected(object? sender, PrintJobInfo job)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (!PrintJobs.Any(j => j.JobId == job.JobId && j.PrinterName == job.PrinterName))
                {
                    PrintJobs.Add(job);
                }
            });
        }

        private void PauseSelectedJob()
        {
            if (SelectedJob != null && int.TryParse(SelectedJob.JobId, out int jobId))
            {
                _jobManager.PauseJob(SelectedJob.PrinterName, jobId);
            }
        }

        private void ResumeSelectedJob()
        {
            if (SelectedJob != null && int.TryParse(SelectedJob.JobId, out int jobId))
            {
                _jobManager.ResumeJob(SelectedJob.PrinterName, jobId);
            }
        }

        private void CancelSelectedJob()
        {
            if (SelectedJob != null && int.TryParse(SelectedJob.JobId, out int jobId))
            {
                _jobManager.CancelJob(SelectedJob.PrinterName, jobId);
                PrintJobs.Remove(SelectedJob);
            }
        }
    }
}
