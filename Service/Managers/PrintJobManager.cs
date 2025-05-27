using Domain.Models;
using System.Runtime.InteropServices;

namespace Service.Managers
{
    public class PrintJobManager
    {
        private const int JOB_ACCESS_ADMINISTER = 0x00000010;
        private const int JOB_ACCESS_READ = 0x00000020;
        private const int JOB_CONTROL_PAUSE = 1;
        private const int JOB_CONTROL_RESUME = 2;
        private const int JOB_CONTROL_CANCEL = 3;

        [DllImport("winspool.drv", SetLastError = true)]
        static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

        [DllImport("winspool.drv", SetLastError = true)]
        static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        static extern bool GetJob(IntPtr hPrinter, int JobId, int Level, IntPtr pJob, int cbBuf, out int pcbNeeded);

        [DllImport("winspool.drv", SetLastError = true)]
        static extern bool SetJob(IntPtr hPrinter, int JobId, int Level, IntPtr pJob, int Command);

        [StructLayout(LayoutKind.Sequential)]
        private struct JOB_INFO_1
        {
            public int JobId;
            public string pPrinterName;
            public string pMachineName;
            public string pUserName;
            public string pDocument;
            public string pDatatype;
            public string pStatus;
            public int Status;
            public int Priority;
            public int Position;
            public int TotalPages;
            public int PagesPrinted;
            public System.Runtime.InteropServices.ComTypes.FILETIME Submitted;
        }

        public PrintJobInfo? GetJobInfo(string printerName, int jobId)
        {
            if (!OpenPrinter(printerName, out var hPrinter, IntPtr.Zero))
                return null;

            try
            {
                GetJob(hPrinter, jobId, 1, IntPtr.Zero, 0, out int needed);
                IntPtr ptr = Marshal.AllocHGlobal(needed);

                if (GetJob(hPrinter, jobId, 1, ptr, needed, out _))
                {
                    var job = Marshal.PtrToStructure<JOB_INFO_1>(ptr);
                    Marshal.FreeHGlobal(ptr);

                    return new PrintJobInfo
                    {
                        JobId = job.JobId.ToString(),
                        DocumentName = job.pDocument,
                        User = job.pUserName,
                        PrinterName = job.pPrinterName,
                        TotalPages = job.TotalPages,
                        Status = job.pStatus ?? job.Status.ToString(),
                        TimeSubmitted = DateTime.Now // We can enhance this later with FILETIME
                    };
                }

                Marshal.FreeHGlobal(ptr);
                return null;
            }
            finally
            {
                ClosePrinter(hPrinter);
            }
        }

        public bool PauseJob(string printerName, int jobId) =>
            ControlJob(printerName, jobId, JOB_CONTROL_PAUSE);

        public bool ResumeJob(string printerName, int jobId) =>
            ControlJob(printerName, jobId, JOB_CONTROL_RESUME);

        public bool CancelJob(string printerName, int jobId) =>
            ControlJob(printerName, jobId, JOB_CONTROL_CANCEL);

        private bool ControlJob(string printerName, int jobId, int command)
        {
            if (!OpenPrinter(printerName, out var hPrinter, IntPtr.Zero))
                return false;

            try
            {
                return SetJob(hPrinter, jobId, 0, IntPtr.Zero, command);
            }
            finally
            {
                ClosePrinter(hPrinter);
            }
        }
    }
}
