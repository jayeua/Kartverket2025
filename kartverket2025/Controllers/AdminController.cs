using kartverket2025.Models.DomainModels;
using kartverket2025.Models.ViewModels;
using kartverket2025.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace kartverket2025.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUsersListRepository usersListRepository;

        public AdminController(UserManager<ApplicationUser> userManager, IUsersListRepository usersListRepository)
        {
            this.userManager = userManager;
            this.usersListRepository = usersListRepository;
        }

        [Authorize(Roles = "System Admin")]
        public async Task<IActionResult> UsersList(string? searchQuery, int pageSize = 10, int pageNumber = 1)
        {
            var allUsers = await GetUsersAsync();

            // Search/filter
            if (!string.IsNullOrEmpty(searchQuery))
            {
                allUsers = allUsers.Where(u =>
                    (u.Email != null && u.Email.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (u.FullName != null && u.FullName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (u.Id != null && u.Id.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            var totalRecords = allUsers.Count();
            var totalPages = (int)Math.Ceiling((decimal)totalRecords / pageSize);

            if (pageNumber < 1) pageNumber = 1;
            if (pageNumber > totalPages) pageNumber = totalPages == 0 ? 1 : totalPages;

            // Paging
            var pagedUsers = allUsers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Get roles for users
            var usersViewModel = new UsersListViewModel
            {
                Users = new List<User>(),
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchQuery = searchQuery
            };

            foreach (var user in pagedUsers)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                usersViewModel.Users.Add(new User
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Roles = userRoles
                });
            }

            return View(usersViewModel);
        }

        [Authorize(Roles = "System Admin")]
        [HttpPost]
        public async Task<IActionResult> DelUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest($"User ID {id} cannot be null or empty.");
            }
            var userDeleted = await usersListRepository.DeleteUserThroughId(id);
            if (userDeleted)
            {
                return RedirectToAction("UsersList");
            }
            else
            {
                return NotFound("User not found.");
            }
        }
        private async Task<List<ApplicationUser>> GetUsersAsync()
        {
            return await userManager.Users.ToListAsync(); // ✅ Fixed casing
        }
    }
}
