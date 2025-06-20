using kartverket2025.Models.DomainModels;

namespace kartverket2025.Repositories
{
    public interface IMapReportRepository
    {
        Task<IEnumerable<MapReportModel>> GetAllReportAsync();

        Task<MapReportModel?> FindCaseById(int id);

        Task<MapReportModel?> AddReportAsync(MapReportModel mapReportRepository);

        Task<MapReportModel?> UpdateReportAsync(MapReportModel mapReportRepository);
        Task<MapReportModel?> DeleteReportAsync(int id);
    }
}
