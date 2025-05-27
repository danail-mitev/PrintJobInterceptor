using Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class ViewModelMain : ViewModelBase
    {
        public ObservableCollection<PrintJobInfo> PrintJobs { get; set; } = new();
    }
}
