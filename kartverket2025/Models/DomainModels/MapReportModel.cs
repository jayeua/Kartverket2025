namespace kartverket2025.Models.DomainModels
{
    public class MapReportModel
    {
        public int Id {  get; set; }
        public string? UserName { get; set; }
        public DateTime? Date { get ; set; } = DateTime.Now;
        public string? Email { get; set; }
        public string? CaseHandler { get; set; }
        public string? AreaJson { get; set; }
        public string? Description { get; set; }
        public string? Kommunenavn { get; set; }
        public string? Fylkenavn { get; set; }

        public MapReportStatus? MapReportStatusModel { get; set; }
        public int MapReportStatusId { get; set; }
        
    }
}
