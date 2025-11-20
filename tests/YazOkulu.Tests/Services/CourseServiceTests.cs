using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using AutoMapper;
using YazOkulu.Application.Services.Implementations;
using YazOkulu.Domain.Entities;
using YazOkulu.Domain.Interfaces;
using YazOkulu.Tests.Helpers;
using YazOkulu.Application.Mappings;
using System.Linq.Expressions;

namespace YazOkulu.Tests.Services;

public class CourseServiceTests
{
    private readonly Mock<IRepository<Course>> _courseRepoMock;
    private readonly Mock<IRepository<CourseApplication>> _applicationRepoMock;
    private readonly Mock<ILogger<CourseService>> _loggerMock;
    private readonly IMapper _mapper;
    private readonly CourseService _courseService;

    public CourseServiceTests()
    {
        _courseRepoMock = new Mock<IRepository<Course>>();
        _applicationRepoMock = new Mock<IRepository<CourseApplication>>();
        _loggerMock = new Mock<ILogger<CourseService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _courseService = new CourseService(
            _courseRepoMock.Object,
            _applicationRepoMock.Object,
            _mapper,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetAllCoursesAsync_WithStudentId_ReturnsCoursesWithApplicationStatus()
    {
        // Arrange
        var studentId = 1;
        var courses = MockData.CreateCourseList();
        var application = MockData.CreateApplication(1, studentId, 1);

        _courseRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(courses);
        _applicationRepoMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<CourseApplication, bool>>>()))
            .ReturnsAsync(new List<CourseApplication> { application });

        // Act
        var result = await _courseService.GetAllCoursesAsync(studentId);

        // Assert
        result.Should().HaveCount(3);
        result.First(c => c.Id == 1).HasApplied.Should().BeTrue();
        result.First(c => c.Id == 2).HasApplied.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllCoursesAsync_WithoutStudentId_ReturnsCoursesOnly()
    {
        // Arrange
        var courses = MockData.CreateCourseList();

        _courseRepoMock.Setup(x => x.GetAllAsync()).ReturnsAsync(courses);

        // Act
        var result = await _courseService.GetAllCoursesAsync(null);

        // Assert
        result.Should().HaveCount(3);
        result.All(c => !c.HasApplied).Should().BeTrue();
        _applicationRepoMock.Verify(x => x.FindAsync(It.IsAny<Expression<Func<CourseApplication, bool>>>()), Times.Never);
    }

    [Fact]
    public async Task GetCourseByIdAsync_ExistingCourse_ReturnsCourseDto()
    {
        // Arrange
        var course = MockData.CreateCourse(1, "CS101");

        _courseRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(course);

        // Act
        var result = await _courseService.GetCourseByIdAsync(1, null);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Code.Should().Be("CS101");
    }

    [Fact]
    public async Task GetCourseByIdAsync_NonExistingCourse_ReturnsNull()
    {
        // Arrange
        _courseRepoMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Course?)null);

        // Act
        var result = await _courseService.GetCourseByIdAsync(999, null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCourseByIdAsync_WithStudentId_IncludesHasAppliedFlag()
    {
        // Arrange
        var studentId = 1;
        var courseId = 1;
        var course = MockData.CreateCourse(courseId);
        var application = MockData.CreateApplication(1, studentId, courseId);

        _courseRepoMock.Setup(x => x.GetByIdAsync(courseId)).ReturnsAsync(course);
        _applicationRepoMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<CourseApplication, bool>>>()))
            .ReturnsAsync(true); // Student has applied

        // Act
        var result = await _courseService.GetCourseByIdAsync(courseId, studentId);

        // Assert
        result.Should().NotBeNull();
        result!.HasApplied.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateCourseAsync_ValidUpdate_UpdatesCourseSuccessfully()
    {
        // Arrange
        var courseId = 1;
        var existingCourse = MockData.CreateCourse(courseId, "CS101", 30);
        var updateDto = MockData.CreateUpdateCourseDto(
            code: "CS102",
            name: "Advanced Programming",
            quota: 25,
            department: "Software Engineering",
            faculty: "Engineering Faculty",
            instructor: "Dr. Smith",
            credits: 4
        );

        _courseRepoMock.Setup(x => x.GetByIdAsync(courseId)).ReturnsAsync(existingCourse);
        _courseRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Course>())).Returns(Task.CompletedTask);

        // Act
        var result = await _courseService.UpdateCourseAsync(courseId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be("CS102");
        result.Name.Should().Be("Advanced Programming");
        result.Quota.Should().Be(25);
        result.Department.Should().Be("Software Engineering");
        result.Faculty.Should().Be("Engineering Faculty");
        result.Instructor.Should().Be("Dr. Smith");
        result.Credits.Should().Be(4);

        _courseRepoMock.Verify(x => x.UpdateAsync(It.IsAny<Course>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCourseAsync_NonExistingCourse_ReturnsNull()
    {
        // Arrange
        var courseId = 999;
        var updateDto = MockData.CreateUpdateCourseDto();

        _courseRepoMock.Setup(x => x.GetByIdAsync(courseId)).ReturnsAsync((Course?)null);

        // Act
        var result = await _courseService.UpdateCourseAsync(courseId, updateDto);

        // Assert
        result.Should().BeNull();
        _courseRepoMock.Verify(x => x.UpdateAsync(It.IsAny<Course>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCourseAsync_ValidUpdate_CallsRepositoryUpdateOnce()
    {
        // Arrange
        var courseId = 1;
        var existingCourse = MockData.CreateCourse(courseId);
        var updateDto = MockData.CreateUpdateCourseDto();

        _courseRepoMock.Setup(x => x.GetByIdAsync(courseId)).ReturnsAsync(existingCourse);
        _courseRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Course>())).Returns(Task.CompletedTask);

        // Act
        await _courseService.UpdateCourseAsync(courseId, updateDto);

        // Assert
        _courseRepoMock.Verify(
            x => x.UpdateAsync(It.Is<Course>(c =>
                c.Id == courseId &&
                c.Code == updateDto.Code &&
                c.Name == updateDto.Name
            )),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateCourseAsync_ValidUpdate_LogsInformation()
    {
        // Arrange
        var courseId = 1;
        var existingCourse = MockData.CreateCourse(courseId, "CS101");
        var updateDto = MockData.CreateUpdateCourseDto(code: "CS102");

        _courseRepoMock.Setup(x => x.GetByIdAsync(courseId)).ReturnsAsync(existingCourse);
        _courseRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Course>())).Returns(Task.CompletedTask);

        // Act
        await _courseService.UpdateCourseAsync(courseId, updateDto);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Ders güncellendi")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
