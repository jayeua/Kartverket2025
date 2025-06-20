using kartverket2025.Data;
using kartverket2025.Models.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace kartverket2025.Repositories
{
    public class MapReportRepository : IMapReportRepository
    {
        private readonly ApplicationDbContext kartDbContext;   
        public MapReportRepository(ApplicationDbContext kartDbContext)
        {
            this.kartDbContext = kartDbContext;
        }

        public async Task<MapReportModel> AddReportAsync(MapReportModel mapReport)
        {
            await kartDbContext.MapReport.AddAsync(mapReport);
            await kartDbContext.SaveChangesAsync();
            return mapReport;
        }

        public async Task<MapReportModel?> DeleteReportAsync(int id)
        {
            var mapReport = await kartDbContext.MapReport
                .Include(x => x.MapReportStatusModel) // Include related entities if needed
                .FirstOrDefaultAsync(x => x.Id == id);

            if (mapReport == null)
            {
                return null;
            }

            try
            {
                kartDbContext.MapReport.Remove(mapReport);
                await kartDbContext.SaveChangesAsync();
                return mapReport;
            }
            catch (DbUpdateException ex)
            {
                // Log ex or handle as needed
                // Optionally, rethrow or return null
                throw new InvalidOperationException("Failed to delete MapReport. It may be referenced by other data.", ex);
            }
        }

        public async Task<IEnumerable<MapReportModel>> GetAllReportAsync()
        {
            return await kartDbContext.MapReport.Include(x => x.MapReportStatusModel).ToListAsync();
        }

        public async Task<MapReportModel?> FindCaseById(int id)
        {
            return await kartDbContext.MapReport.Include(x => x.MapReportStatusModel).Where(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<MapReportModel?> UpdateReportAsync(MapReportModel mapReport)
        {
            kartDbContext.MapReport.Update(mapReport);
            await kartDbContext.SaveChangesAsync();
            return mapReport;
        }

    }
}
