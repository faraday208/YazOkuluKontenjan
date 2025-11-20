using FluentAssertions;
using YazOkulu.Application.DTOs.Applications;
using YazOkulu.Application.Validators;
using YazOkulu.Domain.Enums;

namespace YazOkulu.Tests.Validators;

public class UpdateApplicationStatusDtoValidatorTests
{
    private readonly UpdateApplicationStatusValidator _validator;

    public UpdateApplicationStatusDtoValidatorTests()
    {
        _validator = new UpdateApplicationStatusValidator();
    }

    [Theory]
    [InlineData(ApplicationStatus.Pending)]
    [InlineData(ApplicationStatus.Approved)]
    [InlineData(ApplicationStatus.Rejected)]
    public void Validate_ValidStatus_PassesValidation(ApplicationStatus status)
    {
        // Arrange
        var dto = new UpdateApplicationStatusDto
        {
            Status = status,
            ReviewNotes = "Test notes"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_NullReviewNotes_PassesValidation()
    {
        // Arrange
        var dto = new UpdateApplicationStatusDto
        {
            Status = ApplicationStatus.Approved,
            ReviewNotes = null
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_TooLongReviewNotes_FailsValidation()
    {
        // Arrange
        var dto = new UpdateApplicationStatusDto
        {
            Status = ApplicationStatus.Approved,
            ReviewNotes = new string('A', 501) // 501 karakter
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateApplicationStatusDto.ReviewNotes));
    }
}
