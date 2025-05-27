using Domain.Models;
using Service;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PrintJobInterceptor.ViewModels
{
    public class ViewModelMain : ViewModelBase, IDisposable
    {
        private readonly PrintJobCoordinator _coordinator;

        public ObservableCollection<PrintJobInfo> PrintJobs => _coordinator.Jobs;

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
            _coordinator = new PrintJobCoordinator(OnJobDiscovered);

            PauseCommand = new RelayCommand(_ => _coordinator.Pause(SelectedJob!), _ => SelectedJob != null);
            ResumeCommand = new RelayCommand(_ => _coordinator.Resume(SelectedJob!), _ => SelectedJob != null);
            CancelCommand = new RelayCommand(_ => _coordinator.Cancel(SelectedJob!), _ => SelectedJob != null);
        }

        private void OnJobDiscovered(PrintJobInfo job)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (!_coordinator.Jobs.Any(j => j.JobId == job.JobId && j.PrinterName == job.PrinterName))
                {
                    _coordinator.Jobs.Add(job);
                }
            });
        }

        public void Dispose()
        {
            _coordinator.Dispose();
        }
    }
}
