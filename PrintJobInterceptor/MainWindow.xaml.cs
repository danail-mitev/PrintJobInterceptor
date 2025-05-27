using Domain.ViewModels;
using Service.Monitors;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrintJobInterceptor
{
    public partial class MainWindow : Window
    {
        private readonly PrintMonitorService _monitor = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModelMain();
        }
    }
}