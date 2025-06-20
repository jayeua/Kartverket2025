using kartverket2025.Data;
using kartverket2025.Models.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace kartverket2025.Repositories
{
    public class UsersListRepository : IUsersListRepository
    {
        private readonly ApplicationDbContext kartDbContext;

        public UsersListRepository(ApplicationDbContext kartDbContext)
        {
            this.kartDbContext = kartDbContext;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            return await kartDbContext.Users.ToListAsync();
        }

        public async Task<bool> DeleteUserThroughId(string id)
        {
            var user = await kartDbContext.Users.FindAsync(id);
            if (user == null)
            {
                return false; // User not found
            }
            kartDbContext.Users.Remove(user);
            await kartDbContext.SaveChangesAsync();
            return true; // User deleted successfully
        }
    }
}
