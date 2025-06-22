namespace kartverket2025.Models.ViewModels
{
    public class MapReportViewModel
    {
        public int Id { get; set; }
        public string? ReportTitle { get; set; }
        public string? ReportDescription {  get; set; }
        public string? ReportKommunenavn { get; set; }
        public string? ReportFylkenavn { get; set; }
        public string? ReportAreaJson { get; set; }
        public string? Email { get; set; }
        public DateTime? ReportDate { get; set; }
        public string? CaseHandler { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
    }
}
