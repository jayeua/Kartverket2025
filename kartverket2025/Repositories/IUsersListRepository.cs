using kartverket2025.Models.DomainModels;

namespace kartverket2025.Repositories
{
    public interface IUsersListRepository
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsers();
        Task<bool> DeleteUserThroughId(string id);
    }
}
