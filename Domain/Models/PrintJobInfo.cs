namespace Domain.Models
{
    public class PrintJobInfo
    {
        public string JobId { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string PrinterName { get; set; } = string.Empty;
        public int TotalPages { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime TimeSubmitted { get; set; }
        public Guid GroupId { get; set; } = Guid.Empty;
    }
}
