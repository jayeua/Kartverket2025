namespace kartverket2025.Models.ViewModels
{
    public class MapReportOverviewPagedViewModel
    {
        public List<MapReportViewModel> Reports { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public string SearchQuery { get; set; }
    }
}
