using Xunit;
using Moq;
using kartverket2025.Controllers;
using kartverket2025.Repositories;
using kartverket2025.Models.DomainModels;
using kartverket2025.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace kartverket2025.unitTests
{
    public class MapReportControllerTests
    {
        // Mocks for repository and user manager dependencies
        private readonly MapReportController _controller;
        private readonly Mock<IMapReportRepository> _mockMapReportRepository;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;

        // Test class constructor: sets up controller and common mocks
        public MapReportControllerTests()
        {
            // Mocking the repository and user manager dependencies
            _mockMapReportRepository = new Mock<IMapReportRepository>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<ApplicationUser>>(),
                new List<IUserValidator<ApplicationUser>> { Mock.Of<IUserValidator<ApplicationUser>>() },
                new List<IPasswordValidator<ApplicationUser>> { Mock.Of<IPasswordValidator<ApplicationUser>>() },
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<ApplicationUser>>>()
            );

            // Set up a default mock user for most tests
            var mockUser = new ApplicationUser { Id = "testId", Email = "test@example.com", FirstName = "Test", LastName = "User" };
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(mockUser);

            // Instantiate the controller with the mocks
            _controller = new MapReportController(_mockMapReportRepository.Object, _mockUserManager.Object);

            // Set up a default mock ISession (required for controller session usage)
            var httpContext = new DefaultHttpContext();
            var sessionMock = new Mock<ISession>();
            httpContext.Session = sessionMock.Object;
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public void AddReport_Get_ReturnsView()
        {
            // Test that GET returns a ViewResult
            var result = _controller.AddReport();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task AddReport_Post_ModelOrUserNull_ReturnsViewWithError()
        {
            // Test when the model is null: should return a ViewResult with null model
            MapReportViewModel model = null;
            var result = await _controller.AddReport(model);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);

            // Test when the user is null: should return a ViewResult with the same model
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((ApplicationUser)null);
            var validModel = new MapReportViewModel { ReportTitle = "Test" };
            var result2 = await _controller.AddReport(validModel);
            var viewResult2 = Assert.IsType<ViewResult>(result2);
            Assert.Equal(validModel, viewResult2.Model);
        }

        [Fact]
        public async Task AddReport_Post_ValidModelAndUser_RedirectsToSuccessReportAndSetsSession()
        {
            // Arrange: set up mock session and HTTP context for the controller
            var sessionMock = new Mock<ISession>();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = sessionMock.Object;
            _controller.ControllerContext.HttpContext = httpContext;

            // Prepare a valid view model for the test
            var model = new MapReportViewModel
            {
                ReportTitle = "Title",
                ReportDescription = "Description",
                ReportKommunenavn = "Kommune",
                ReportFylkenavn = "Fylke",
                ReportAreaJson = "{}",
                TileLayerId = 1
            };

            // Mock repository to "save" the report
            _mockMapReportRepository
                .Setup(r => r.AddReportAsync(It.IsAny<MapReportModel>()))
                .ReturnsAsync((MapReportModel)null);

            // Act: call the controller action
            var result = await _controller.AddReport(model);

            // Assert: should redirect to SuccessReport and set session key "JustReported" to "true"
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SuccessReport", redirect.ActionName);
            _mockMapReportRepository.Verify(r => r.AddReportAsync(It.IsAny<MapReportModel>()), Times.Once);

            // Verify ISession.Set called with the expected value ("true" as a UTF8 byte array)
            sessionMock.Verify(s => s.Set(
                "JustReported",
                It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == "true")
            ), Times.Once);
        }

        [Fact]
        public void PreviewReport_ReturnsPreviewView()
        {
            // Test that PreviewReport returns the "PreviewReport" view with the right model
            var viewModel = new MapReportViewModel { ReportTitle = "Preview" };
            var result = _controller.PreviewReport(viewModel);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("PreviewReport", viewResult.ViewName);
            Assert.Equal(viewModel, viewResult.Model);
        }

        [Fact]
        public async Task ConfirmReport_ValidModelAndUser_RedirectsToSuccessReport()
        {
            // Arrange: mock session and HTTP context
            var sessionMock = new Mock<ISession>();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = sessionMock.Object;
            _controller.ControllerContext.HttpContext = httpContext;

            // Prepare valid model
            var model = new MapReportViewModel
            {
                ReportTitle = "Title",
                ReportDescription = "Description",
                ReportKommunenavn = "Kommune",
                ReportFylkenavn = "Fylke",
                ReportAreaJson = "{}",
                TileLayerId = 1
            };

            // Mock repo for report creation
            _mockMapReportRepository
                .Setup(r => r.AddReportAsync(It.IsAny<MapReportModel>()))
                .ReturnsAsync((MapReportModel)null);

            // Act
            var result = await _controller.ConfirmReport(model);

            // Assert: Should redirect and set session value
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SuccessReport", redirect.ActionName);
            _mockMapReportRepository.Verify(r => r.AddReportAsync(It.IsAny<MapReportModel>()), Times.Once);
            sessionMock.Verify(s => s.Set(
                "JustReported",
                It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == "true")
            ), Times.Once);
        }

        [Fact]
        public async Task UserReportHistory_ReturnsViewWithPagedModel()
        {
            // Arrange: setup user and mock repo to return one report
            var user = new ApplicationUser { Id = "testId", Email = "test@example.com" };
            _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var reports = new List<MapReportModel>
            {
                new MapReportModel { Id = 1, Email = user.Email, Title = "Test", Description = "Desc", Kommunenavn = "K", Fylkenavn = "F", Date = System.DateTime.Now }
            };
            _mockMapReportRepository.Setup(r => r.GetAllReportAsync()).ReturnsAsync(reports);

            // Act: call the controller action
            var result = await _controller.UserReportHistory(null);

            // Assert: Should return a view with one report in the model
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<MapReportOverviewPagedViewModel>(viewResult.Model);
            Assert.Single(model.Reports);
        }

        [Fact]
        public async Task UpdateMapReport_Get_ReportNotFound_ReturnsNotFound()
        {
            // Arrange: mock repo to not find the case
            _mockMapReportRepository
                .Setup(r => r.FindCaseById(It.IsAny<int>()))
                .ReturnsAsync((MapReportModel)null);

            // Act/Assert: Should return NotFound
            var result = await _controller.UpdateMapReport(42);
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("not found", notFound.Value.ToString());
        }

        [Fact]
        public async Task DeleteMapReport_Get_ReportNotFound_ReturnsNotFound()
        {
            // Arrange: mock repo to not find report
            _mockMapReportRepository
                .Setup(r => r.FindCaseById(It.IsAny<int>()))
                .ReturnsAsync((MapReportModel)null);

            // Act/Assert: Should return NotFound
            var result = await _controller.DeleteMapReport(1);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("not valid or not found", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteMapReportConfirmed_DeletesAndRedirects()
        {
            // Arrange: mock repo for delete
            _mockMapReportRepository
                .Setup(r => r.DeleteReportAsync(It.IsAny<int>()))
                .ReturnsAsync((MapReportModel)null);

            // Set up a fake user in the context
            var httpContext = new DefaultHttpContext();
            var claims = new List<Claim> { new Claim(ClaimTypes.Role, "Map User") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            httpContext.User = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext = httpContext;

            // Act: Call delete
            var result = await _controller.DeleteMapReportConfirmed(1);

            // Assert: Should redirect to history
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("UserReportHistory", redirect.ActionName);
        }

        [Fact]
        public void SuccessReport_SessionSet_ReturnsView()
        {
            // Arrange: Simulate session containing "JustReported" == "true"
            var sessionMock = new Mock<ISession>();
            var key = "JustReported";
            var value = Encoding.UTF8.GetBytes("true");
            sessionMock.Setup(s => s.TryGetValue(key, out value)).Returns(true);
            sessionMock.Setup(s => s.Remove(key));

            var httpContext = new DefaultHttpContext();
            httpContext.Session = sessionMock.Object;
            _controller.ControllerContext.HttpContext = httpContext;

            // Act: Call SuccessReport
            var result = _controller.SuccessReport();
            Assert.IsType<ViewResult>(result);

            // Assert: Session key should be removed
            sessionMock.Verify(s => s.Remove(key), Times.Once);
        }

        [Fact]
        public void SuccessReport_NoSession_RedirectsToAddReport()
        {
            // Arrange: Simulate session where "JustReported" is not set
            var sessionMock = new Mock<ISession>();
            var key = "JustReported";
            byte[] value = null;
            sessionMock.Setup(s => s.TryGetValue(key, out value)).Returns(false);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = sessionMock.Object;
            _controller.ControllerContext.HttpContext = httpContext;

            // Act: Call SuccessReport
            var result = _controller.SuccessReport();

            // Assert: Should redirect to AddReport
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddReport", redirect.ActionName);
        }
    }
}