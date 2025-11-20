using FluentAssertions;
using YazOkulu.Application.DTOs.Applications;
using YazOkulu.Application.Validators;

namespace YazOkulu.Tests.Validators;

public class CreateApplicationDtoValidatorTests
{
    private readonly CreateApplicationValidator _validator;

    public CreateApplicationDtoValidatorTests()
    {
        _validator = new CreateApplicationValidator();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999)]
    public void Validate_ValidCourseId_PassesValidation(int courseId)
    {
        // Arrange
        var dto = new CreateApplicationDto { CourseId = courseId };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_InvalidCourseId_FailsValidation(int courseId)
    {
        // Arrange
        var dto = new CreateApplicationDto { CourseId = courseId };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }
}
