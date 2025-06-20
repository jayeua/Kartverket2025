using Azure.Identity;
using kartverket2025.Models.DomainModels;
using kartverket2025.Models.ViewModels;
using kartverket2025.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
                    UserName = currentUser.UserName,
                    Email = currentUser.Email,
                    Kommunenavn = mapReportViewModel.ReportKommunenavn,
                    Fylkenavn = mapReportViewModel.ReportFylkenavn,
                    Description = mapReportViewModel.ReportDescription,
                    AreaJson = mapReportViewModel.ReportAreaJson,
                    MapReportStatusId = 1,
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
                    UserName = currentUser.UserName,
                    Email = currentUser.Email,
                    Kommunenavn = mapReportViewModel.ReportKommunenavn,
                    Fylkenavn = mapReportViewModel.ReportFylkenavn,
                    Description = mapReportViewModel.ReportDescription,
                    AreaJson = mapReportViewModel.ReportAreaJson,
                    MapReportStatusId = 1,
                    Date = DateTime.Now
                };

                await _mapReportRepository.AddReportAsync(newMapReport);

                return RedirectToAction("SuccessReport");
            }

            ModelState.AddModelError("", "An error has occurred. Please try again.");
            return View("AddReport", mapReportViewModel);
        }

        //[Authorize(Roles = "Map User, Case Handler")]
        //[HttpGet]
        //public async Task<IActionResult> UserReportHistory()
        //{
        //    var getReportHistory = await _mapReportRepository.GetAllReportAsync();
        //    var userReporter = await _userManager.GetUserAsync(User);

        //    if (getReportHistory != null && userReporter != null)
        //    {
        //        var mapReportViewModel = getReportHistory.Where(e => e.Email == userReporter.Email)
        //            .Select(u => new MapReportViewModel
        //            {
        //                CaseHandler = u.CaseHandler ?? "No Casehandler",
        //                ReportDescription = u.Description,
        //                ReportAreaJson = u.AreaJson,
        //                ReportFylkenavn = u.Fylkenavn,
        //                ReportKommunenavn = u.Kommunenavn,
        //                Id = u.Id,
        //                Status = u.MapReportStatusModel.Status,
        //                ReportDate = u.Date
        //            }
        //        );
        //        return View(mapReportViewModel);
        //    }
        //    return View(new List<MapReportViewModel>());

        //}

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
        //[Authorize(Roles = "Case Handler")]
        //[HttpGet]
        //public async Task<IActionResult> AllMapReportsOverview()
        //{
        //    var getOverView = await _mapReportRepository.GetAllReportAsync();
        //    if (getOverView != null)
        //    {
        //        var mapReportViewModel = getOverView.Select(u => new MapReportViewModel
        //        {
        //            ReportKommunenavn = u.Kommunenavn,
        //            ReportFylkenavn = u.Fylkenavn,
        //            ReportDescription = u.Description,
        //            ReportAreaJson = u.AreaJson,
        //            ReportDate = u.Date,
        //            Email = u.Email,
        //            Id = u.Id,
        //            Status = u.MapReportStatusModel.Status
        //        }).ToList();
        //        return View(mapReportViewModel);
        //    }
        //    return NotFound();
        //}

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
                        (r.Description != null && r.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                        (r.Kommunenavn != null && r.Kommunenavn.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                        (r.Fylkenavn != null && r.Fylkenavn.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)))
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
                    ReportKommunenavn = u.Kommunenavn,
                    ReportFylkenavn = u.Fylkenavn,
                    ReportDescription = u.Description,
                    ReportAreaJson = u.AreaJson,
                    ReportDate = u.Date,
                    Email = u.Email,
                    Id = u.Id,
                    Status = u.MapReportStatusModel.Status
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
                    ReportDescription = updateCurrentMap.Description,
                    ReportKommunenavn = updateCurrentMap.Kommunenavn,
                    ReportFylkenavn = updateCurrentMap.Fylkenavn,
                    ReportAreaJson = updateCurrentMap.AreaJson,
                    ReportDate = updateCurrentMap.Date,
                    CaseHandler = updateCurrentMap.CaseHandler
                };

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
                // Re-render form with validation errors
                return View(mapReportViewModel);
            }

            var mapToUpdateReport = await _mapReportRepository.FindCaseById(mapReportViewModel.Id);
            var casehandler = await _userManager.GetUserAsync(User);
            if (mapToUpdateReport == null)
            {
                return NotFound($"Map report with ID {mapReportViewModel.Id} is not found.");
            }

            // Update properties
            mapToUpdateReport.Description = mapReportViewModel.ReportDescription;
            mapToUpdateReport.AreaJson = mapReportViewModel.ReportAreaJson;
            mapToUpdateReport.Kommunenavn = mapReportViewModel.ReportKommunenavn;
            mapToUpdateReport.Fylkenavn = mapReportViewModel.ReportFylkenavn;
            mapToUpdateReport.Date = DateTime.Now;
            mapToUpdateReport.CaseHandler = $"{casehandler.FirstName} {casehandler.LastName}";

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
                Email = mapReport.Email,
                ReportDescription = mapReport.Description,
                ReportKommunenavn = mapReport.Kommunenavn,
                ReportFylkenavn = mapReport.Fylkenavn,
                Status = mapReport.MapReportStatusModel.Status
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
                return RedirectToAction("AllMapReportsOverview");
            }
        }

        [HttpGet]
        public IActionResult SuccessReport()
        {
            return View();
        }
    }
}