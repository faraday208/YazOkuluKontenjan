using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using AutoMapper;
using YazOkulu.Application.Services.Implementations;
using YazOkulu.Application.DTOs.Applications;
using YazOkulu.Domain.Entities;
using YazOkulu.Domain.Enums;
using YazOkulu.Domain.Interfaces;
using YazOkulu.Tests.Helpers;
using YazOkulu.Application.Mappings;
using System.Linq.Expressions;

namespace YazOkulu.Tests.Services;

public class ApplicationServiceTests
{
    private readonly Mock<IRepository<CourseApplication>> _applicationRepoMock;
    private readonly Mock<IRepository<Course>> _courseRepoMock;
    private readonly Mock<IRepository<Student>> _studentRepoMock;
    private readonly Mock<ILogger<ApplicationService>> _loggerMock;
    private readonly IMapper _mapper;
    private readonly ApplicationService _applicationService;

    public ApplicationServiceTests()
    {
        _applicationRepoMock = new Mock<IRepository<CourseApplication>>();
        _courseRepoMock = new Mock<IRepository<Course>>();
        _studentRepoMock = new Mock<IRepository<Student>>();
        _loggerMock = new Mock<ILogger<ApplicationService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _applicationService = new ApplicationService(
            _applicationRepoMock.Object,
            _courseRepoMock.Object,
            _studentRepoMock.Object,
            _mapper,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task CreateApplicationAsync_ValidRequest_CreatesApplication()
    {
        // Arrange
        var studentId = 1;
        var courseId = 1;
        var dto = new CreateApplicationDto { CourseId = courseId };

        var course = MockData.CreateCourse(courseId, quota: 30);
        var student = MockData.CreateStudent(studentId);

        _studentRepoMock.Setup(x => x.GetByIdAsync(studentId)).ReturnsAsync(student);
        _courseRepoMock.Setup(x => x.GetByIdAsync(courseId)).ReturnsAsync(course);
        _applicationRepoMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<CourseApplication, bool>>>()))
            .ReturnsAsync(false); // No existing application
        _applicationRepoMock.Setup(x => x.AddAsync(It.IsAny<CourseApplication>()))
            .ReturnsAsync((CourseApplication app) => app); // Return the same application

        // Act
        var result = await _applicationService.CreateApplicationAsync(studentId, dto);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Status.Should().Be(ApplicationStatus.Pending);
        _applicationRepoMock.Verify(x => x.AddAsync(It.IsAny<CourseApplication>()), Times.Once);
    }

    [Fact]
    public async Task CreateApplicationAsync_DuplicateApplication_ReturnsFailure()
    {
        // Arrange
        var studentId = 1;
        var courseId = 1;
        var dto = new CreateApplicationDto { CourseId = courseId };

        var student = MockData.CreateStudent(studentId);
        var existingApplication = MockData.CreateApplication(1, studentId, courseId);
        var course = MockData.CreateCourse(courseId);

        _studentRepoMock.Setup(x => x.GetByIdAsync(studentId)).ReturnsAsync(student);
        _courseRepoMock.Setup(x => x.GetByIdAsync(courseId)).ReturnsAsync(course);
        _applicationRepoMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<CourseApplication, bool>>>()))
            .ReturnsAsync(true); // Application exists

        // Act
        var result = await _applicationService.CreateApplicationAsync(studentId, dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Bu derse zaten başvurdunuz");
        _applicationRepoMock.Verify(x => x.AddAsync(It.IsAny<CourseApplication>()), Times.Never);
    }

    [Fact]
    public async Task CreateApplicationAsync_QuotaFull_ReturnsFailure()
    {
        // Arrange
        var studentId = 1;
        var courseId = 1;
        var dto = new CreateApplicationDto { CourseId = courseId };

        var student = MockData.CreateStudent(studentId);
        var course = MockData.CreateCourse(courseId, quota: 1);
        var approvedApp = MockData.CreateApplication(2, 2, courseId, ApplicationStatus.Approved);
        course.Applications.Add(approvedApp);

        _studentRepoMock.Setup(x => x.GetByIdAsync(studentId)).ReturnsAsync(student);
        _courseRepoMock.Setup(x => x.GetByIdAsync(courseId)).ReturnsAsync(course);
        _applicationRepoMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<CourseApplication, bool>>>()))
            .ReturnsAsync(false);

        // Act
        var result = await _applicationService.CreateApplicationAsync(studentId, dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Bu dersin kontenjanı dolmuştur");
    }

    [Fact]
    public async Task CreateApplicationAsync_NonExistingCourse_ReturnsFailure()
    {
        // Arrange
        var studentId = 1;
        var dto = new CreateApplicationDto { CourseId = 999 };

        var student = MockData.CreateStudent(studentId);
        _studentRepoMock.Setup(x => x.GetByIdAsync(studentId)).ReturnsAsync(student);
        _courseRepoMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Course?)null);

        // Act
        var result = await _applicationService.CreateApplicationAsync(studentId, dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Ders bulunamadı");
    }

    [Fact]
    public async Task UpdateApplicationStatusAsync_ApproveApplication_UpdatesStatus()
    {
        // Arrange
        var applicationId = 1;
        var dto = new UpdateApplicationStatusDto
        {
            Status = ApplicationStatus.Approved,
            ReviewNotes = "Onaylandı"
        };

        var application = MockData.CreateApplication(applicationId);
        var course = MockData.CreateCourse(1, quota: 30);
        var student = MockData.CreateStudent(1);

        _applicationRepoMock.Setup(x => x.GetByIdAsync(applicationId)).ReturnsAsync(application);
        _courseRepoMock.Setup(x => x.GetByIdAsync(application.CourseId)).ReturnsAsync(course);
        _studentRepoMock.Setup(x => x.GetByIdAsync(application.StudentId)).ReturnsAsync(student);

        // Act
        var result = await _applicationService.UpdateApplicationStatusAsync(applicationId, dto);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Status.Should().Be(ApplicationStatus.Approved);
        result.Data.ReviewNotes.Should().Be("Onaylandı");
        _applicationRepoMock.Verify(x => x.UpdateAsync(It.IsAny<CourseApplication>()), Times.Once);
    }

    [Fact]
    public async Task UpdateApplicationStatusAsync_RejectApplication_UpdatesStatus()
    {
        // Arrange
        var applicationId = 1;
        var dto = new UpdateApplicationStatusDto
        {
            Status = ApplicationStatus.Rejected,
            ReviewNotes = "Reddedildi"
        };

        var application = MockData.CreateApplication(applicationId);
        var course = MockData.CreateCourse(1);
        var student = MockData.CreateStudent(1);

        _applicationRepoMock.Setup(x => x.GetByIdAsync(applicationId)).ReturnsAsync(application);
        _courseRepoMock.Setup(x => x.GetByIdAsync(application.CourseId)).ReturnsAsync(course);
        _studentRepoMock.Setup(x => x.GetByIdAsync(application.StudentId)).ReturnsAsync(student);

        // Act
        var result = await _applicationService.UpdateApplicationStatusAsync(applicationId, dto);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Status.Should().Be(ApplicationStatus.Rejected);
    }

    [Fact]
    public async Task UpdateApplicationStatusAsync_NonExistingApplication_ReturnsFailure()
    {
        // Arrange
        var dto = new UpdateApplicationStatusDto { Status = ApplicationStatus.Approved };

        _applicationRepoMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((CourseApplication?)null);

        // Act
        var result = await _applicationService.UpdateApplicationStatusAsync(999, dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Başvuru bulunamadı");
    }

    [Fact]
    public async Task GetStudentApplicationsAsync_ReturnsStudentApplications()
    {
        // Arrange
        var studentId = 1;
        var applications = new List<CourseApplication>
        {
            MockData.CreateApplication(1, studentId, 1),
            MockData.CreateApplication(2, studentId, 2)
        };

        _applicationRepoMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<CourseApplication, bool>>>()))
            .ReturnsAsync(applications);

        // Act
        var result = await _applicationService.GetStudentApplicationsAsync(studentId);

        // Assert
        result.Should().HaveCount(2);
        result.All(a => a.StudentId == studentId).Should().BeTrue();
    }

    [Fact]
    public async Task GetCourseApplicationsAsync_ReturnsAllApplicationsForCourse()
    {
        // Arrange
        var courseId = 1;
        var applications = new List<CourseApplication>
        {
            MockData.CreateApplication(1, 1, courseId),
            MockData.CreateApplication(2, 2, courseId),
            MockData.CreateApplication(3, 3, courseId)
        };

        _applicationRepoMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<CourseApplication, bool>>>()))
            .ReturnsAsync(applications);

        // Act
        var result = await _applicationService.GetCourseApplicationsAsync(courseId);

        // Assert
        result.Should().HaveCount(3);
        result.All(a => a.CourseId == courseId).Should().BeTrue();
    }
}
