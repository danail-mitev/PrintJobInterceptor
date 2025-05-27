using Domain.Models;
using Service;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PrintJobInterceptor.ViewModels
{
    public class ViewModelMain : ViewModelBase, IDisposable
    {
        private readonly PrintJobCoordinator _coordinator = new();

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
            PauseCommand = new RelayCommand(_ => _coordinator.Pause(SelectedJob!), _ => SelectedJob != null);
            ResumeCommand = new RelayCommand(_ => _coordinator.Resume(SelectedJob!), _ => SelectedJob != null);
            CancelCommand = new RelayCommand(_ => _coordinator.Cancel(SelectedJob!), _ => SelectedJob != null);
        }

        public void Dispose()
        {
            _coordinator.Dispose();
        }
    }
}
