using Azure.Identity;
using kartverket2025.Models.DomainModels;
using kartverket2025.Models.ViewModels;
using kartverket2025.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace kartverket2025.Controllers
{
    public class MapReportController : Controller
    {
        private readonly IMapReportRepository _mapReportRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public MapReportController(IMapReportRepository mapReportRepository, UserManager<ApplicationUser> userManager)
        {
            _mapReportRepository = mapReportRepository;
            _userManager = userManager;
        }

        [Authorize (Roles = "Map User, Case Handler")]
        [HttpGet]
        public IActionResult AddReport()
        {
            return View();
        }

        [Authorize(Roles = "Map User, Case Handler")]
        [HttpPost]
        public async Task<IActionResult> AddReport(MapReportViewModel mapReportViewModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (mapReportViewModel != null && currentUser != null)
            {
                var newMapReport = new MapReportModel
                {
                    ApplicationUserId = currentUser.Id,
                    Email = currentUser.Email,
                    Kommunenavn = mapReportViewModel.ReportKommunenavn,
                    Fylkenavn = mapReportViewModel.ReportFylkenavn,
                    Title = mapReportViewModel.ReportTitle,
                    Description = mapReportViewModel.ReportDescription,
                    AreaJson = mapReportViewModel.ReportAreaJson,
                    TileLayerId = mapReportViewModel.TileLayerId,
                    MapReportStatusId = 1,
                    MapPriorityStatusId = 1,
                    Date = DateTime.Now 
                };

                await _mapReportRepository.AddReportAsync(newMapReport);

                // Redirect to the success report page
                return RedirectToAction("SuccessReport");
            }

            // If model or user is null, show the form again with an error
            ModelState.AddModelError("", "An error has occurred. Please try again.");
            return View(mapReportViewModel);
        }

        [Authorize(Roles = "Map User, Case Handler")]
        [HttpPost]
        public IActionResult PreviewReport(MapReportViewModel mapReportViewModel)
        {
            // You might want to validate here, but for now just show the preview
            return View("PreviewReport", mapReportViewModel);
        }

        [Authorize(Roles = "Map User, Case Handler")]
        [HttpPost]
        public async Task<IActionResult> ConfirmReport(MapReportViewModel mapReportViewModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (mapReportViewModel != null && currentUser != null)
            {
                var newMapReport = new MapReportModel
                {
                    ApplicationUserId = currentUser.Id,
                    Email = currentUser.Email,
                    Kommunenavn = mapReportViewModel.ReportKommunenavn,
                    Fylkenavn = mapReportViewModel.ReportFylkenavn,
                    Title = mapReportViewModel.ReportTitle,
                    Description = mapReportViewModel.ReportDescription,
                    AreaJson = mapReportViewModel.ReportAreaJson,
                    TileLayerId = mapReportViewModel.TileLayerId,
                    MapReportStatusId = 1,
                    MapPriorityStatusId = 1,
                    Date = DateTime.Now
                };

                await _mapReportRepository.AddReportAsync(newMapReport);

                return RedirectToAction("SuccessReport");
            }

            ModelState.AddModelError("", "An error has occurred. Please try again.");
            return View("AddReport", mapReportViewModel);
        }

        [Authorize(Roles = "Map User, Case Handler")]
        [HttpGet]
        public async Task<IActionResult> UserReportHistory(string? searchQuery, int pageSize = 5, int pageNumber = 1)
        {
            var allReports = await _mapReportRepository.GetAllReportAsync();
            var user = await _userManager.GetUserAsync(User);

            // Only current user's reports
            var userReports = allReports.Where(r => r.Email == user.Email);

            // Search filter
            if (!string.IsNullOrEmpty(searchQuery))
            {
                userReports = userReports.Where(r =>
                    (r.Title != null && r.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (r.Description != null && r.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (r.Kommunenavn != null && r.Kommunenavn.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (r.Fylkenavn != null && r.Fylkenavn.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (r.CaseHandler != null && r.CaseHandler.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (r.MapReportStatusModel != null && r.MapReportStatusModel.Status != null && r.MapReportStatusModel.Status.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                );
            }

            var totalRecords = userReports.Count();
            var totalPages = (int)Math.Ceiling((decimal)totalRecords / pageSize);

            if (pageNumber < 1) pageNumber = 1;
            if (pageNumber > totalPages) pageNumber = totalPages == 0 ? 1 : totalPages;

            var pagedReports = userReports
                .OrderByDescending(r => r.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new MapReportViewModel
                {
                    Id = r.Id,
                    ReportTitle = r.Title ?? "No Title",
                    ReportDescription = r.Description,
                    ReportKommunenavn = r.Kommunenavn,
                    ReportFylkenavn = r.Fylkenavn,
                    CaseHandler = r.CaseHandler ?? "No Casehandler",
                    Status = r.MapReportStatusModel != null ? r.MapReportStatusModel.Status : "",
                    ReportDate = r.Date
                })
                .ToList();

            var pagedModel = new MapReportOverviewPagedViewModel
            {
                Reports = pagedReports,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchQuery = searchQuery ?? ""
            };

            return View(pagedModel);
        }

        [Authorize(Roles = "Case Handler")]
        [HttpGet]
        public async Task<IActionResult> AllMapReportsOverview(string? searchQuery, int pageSize = 5, int pageNumber = 1)
        {
            // Get all reports (or filter in repository if possible)
            var allReports = await _mapReportRepository.GetAllReportAsync();

            // Filter by search query if provided (e.g., search in email, description, etc.)
            if (!string.IsNullOrEmpty(searchQuery))
            {
                allReports = allReports
                .Where(r =>
                    (r.Email != null && r.Email.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (r.Title != null && r.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (r.Kommunenavn != null && r.Kommunenavn.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (r.Fylkenavn != null && r.Fylkenavn.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    // adding more properties here
                    (r.Description != null && r.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (r.MapReportStatusModel != null && r.MapReportStatusModel.Status != null && r.MapReportStatusModel.Status.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (r.MapPriorityStatusModel != null && r.MapPriorityStatusModel.PriorityStatus != null && r.MapPriorityStatusModel.PriorityStatus.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            }

            var totalRecords = allReports.Count();
            var totalPages = (int)Math.Ceiling((decimal)totalRecords / pageSize);

            if (pageNumber < 1) pageNumber = 1;
            if (pageNumber > totalPages) pageNumber = totalPages == 0 ? 1 : totalPages;

            var pagedReports = allReports
                .OrderByDescending(u => u.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new MapReportViewModel
                {
                    Id = u.Id,
                    ReportTitle = u.Title,
                    Email = u.Email,
                    ReportKommunenavn = u.Kommunenavn,
                    ReportFylkenavn = u.Fylkenavn,
                    ReportAreaJson = u.AreaJson,
                    ReportDate = u.Date,
                    FullName = u.ApplicationUserModel != null ? u.ApplicationUserModel.FullName : "",
                    Status = u.MapReportStatusModel.Status,
                    Priority = u.MapPriorityStatusModel != null ? u.MapPriorityStatusModel.PriorityStatus : ""
                })
                .ToList();

            var overviewPaged = new MapReportOverviewPagedViewModel
            {
                Reports = pagedReports,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchQuery = searchQuery
            };

            return View(overviewPaged);
        }

        [Authorize(Roles ="Case Handler")]
        [HttpGet]
        public async Task<IActionResult> UpdateMapReport(int id)
        {
            var updateCurrentMap = await _mapReportRepository.FindCaseById(id);

            if (updateCurrentMap != null)
            {
                MapReportViewModel mapReportViewModel = new MapReportViewModel
                {
                    Id = updateCurrentMap.Id,
                    ReportTitle = updateCurrentMap.Title,
                    ReportDescription = updateCurrentMap.Description,
                    ReportKommunenavn = updateCurrentMap.Kommunenavn,
                    ReportFylkenavn = updateCurrentMap.Fylkenavn,
                    ReportAreaJson = updateCurrentMap.AreaJson,
                    ReportDate = updateCurrentMap.Date,
                    CaseHandler = updateCurrentMap.CaseHandler,
                    MapReportStatusId = updateCurrentMap.MapReportStatusId,
                    MapPriorityStatusId = updateCurrentMap.MapPriorityStatusId ?? 1,
                    TileLayerId = updateCurrentMap.TileLayerId
                };

                var statuses = await _mapReportRepository.GetAllStatusesAsync();
                mapReportViewModel.StatusOptions = statuses
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Status,
                        Selected = s.Id == mapReportViewModel.MapReportStatusId
                    });

                var priorities = await _mapReportRepository.GetAllPriorityStatusesAsync();
                mapReportViewModel.PriorityOptions = priorities.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.PriorityStatus,
                    Selected = p.Id == mapReportViewModel.MapPriorityStatusId
                });

                return View(mapReportViewModel); // This will use UpdateMapReport.cshtml
            }
            return NotFound($"Map report with ID {id} not found");
        }

        [Authorize(Roles = "Case Handler")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMapReport(MapReportViewModel mapReportViewModel)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns!
                var statuses = await _mapReportRepository.GetAllStatusesAsync();
                mapReportViewModel.StatusOptions = statuses.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Status,
                    Selected = s.Id == mapReportViewModel.MapReportStatusId
                });

                var priorities = await _mapReportRepository.GetAllPriorityStatusesAsync();
                mapReportViewModel.PriorityOptions = priorities.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.PriorityStatus,
                    Selected = p.Id == mapReportViewModel.MapPriorityStatusId
                });

                return View(mapReportViewModel);
            }

            var mapToUpdateReport = await _mapReportRepository.FindCaseById(mapReportViewModel.Id);
            var casehandler = await _userManager.GetUserAsync(User);
            if (mapToUpdateReport == null)
            {
                return NotFound($"Map report with ID {mapReportViewModel.Id} is not found.");
            }

            // Update properties
            //mapToUpdateReport.Description = mapReportViewModel.ReportDescription;
            //mapToUpdateReport.AreaJson = mapReportViewModel.ReportAreaJson;
            //mapToUpdateReport.Kommunenavn = mapReportViewModel.ReportKommunenavn;
            //mapToUpdateReport.Fylkenavn = mapReportViewModel.ReportFylkenavn;
            mapToUpdateReport.Date = DateTime.Now;
            mapToUpdateReport.CaseHandler = $"{casehandler.FirstName} {casehandler.LastName}";
            mapToUpdateReport.MapReportStatusId = mapReportViewModel.MapReportStatusId;
            mapToUpdateReport.MapPriorityStatusId = mapReportViewModel.MapPriorityStatusId;
            mapToUpdateReport.TileLayerId = mapReportViewModel.TileLayerId;

            await _mapReportRepository.UpdateReportAsync(mapToUpdateReport);

            // Redirect to overview or success page
            return RedirectToAction("AllMapReportsOverview");
        }

        [Authorize(Roles = "Case Handler")]
        [HttpPost]
        public async Task<IActionResult> FinishReport(MapReportViewModel mapReportViewModel)
        {
            var mapToUpdate = await _mapReportRepository.FindCaseById(mapReportViewModel.Id);
            var casehandler = await _userManager.GetUserAsync(User);

            if (mapToUpdate != null && mapToUpdate.MapReportStatusId != 3)
            {
                mapToUpdate.MapReportStatusId = 3; // Set status to Finished
                mapToUpdate.Date = DateTime.Now;
                mapToUpdate.CaseHandler = $"{casehandler.FirstName} {casehandler.LastName}"; // Set case handler

                await _mapReportRepository.UpdateReportAsync(mapToUpdate);
                return RedirectToAction("AllMapReportsOverview");


            }
            return BadRequest("The case has already been finished or does not exist.");
        }

        // --- ADD THIS: GET: DeleteMapReport (confirmation page) ---
        [Authorize(Roles = "Map User, Case Handler")]
        [HttpGet]
        public async Task<IActionResult> DeleteMapReport(int id)
        {
            var mapReport = await _mapReportRepository.FindCaseById(id);
            if (mapReport == null)
            {
                return NotFound($"Map report with ID {id} is not valid or not found");
            }

            // Map to ViewModel for the view
            var viewModel = new MapReportViewModel
            {
                Id = mapReport.Id,
                FullName = mapReport.ApplicationUserModel != null ? mapReport.ApplicationUserModel.FullName : "",
                ReportDescription = mapReport.Description,
                ReportKommunenavn = mapReport.Kommunenavn,
                ReportFylkenavn = mapReport.Fylkenavn,
                Status = mapReport.MapReportStatusModel.Status,
                ReportAreaJson = mapReport.AreaJson,
                TileLayerId = mapReport.TileLayerId

            };

            return View(viewModel); // This will use DeleteMapReport.cshtml
        }

        [Authorize(Roles = "Map User, Case Handler")]
        [HttpPost, ActionName("DeleteMapReport")]
        public async Task<IActionResult> DeleteMapReportConfirmed(int id)
        {
            await _mapReportRepository.DeleteReportAsync(id);

            // Redirect based on user role
            if (User.IsInRole("Map User"))
            {
                return RedirectToAction("UserReportHistory");
            }
            else if (User.IsInRole("Case Handler"))
            {
                return RedirectToAction("AllMapReportsOverview");
            }
            else
            {
                // fallback (shouldn’t happen if you only allow above roles)
                return RedirectToAction("UserReportHistory");
            }
        }

        [HttpGet]
        public IActionResult SuccessReport()
        {
            return View();
        }
    }
}