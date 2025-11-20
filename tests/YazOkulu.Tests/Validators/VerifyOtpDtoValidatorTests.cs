using FluentAssertions;
using YazOkulu.Application.DTOs.Auth;
using YazOkulu.Application.Validators;

namespace YazOkulu.Tests.Validators;

public class VerifyOtpDtoValidatorTests
{
    private readonly VerifyOtpValidator _validator;

    public VerifyOtpDtoValidatorTests()
    {
        _validator = new VerifyOtpValidator();
    }

    [Fact]
    public void Validate_ValidData_PassesValidation()
    {
        // Arrange
        var dto = new VerifyOtpDto
        {
            PhoneNumber = "05001234567",
            Code = "123456"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "123456")]
    [InlineData("05001234567", "")]
    [InlineData("05001234567", "12345")]
    [InlineData("05001234567", "1234567")]
    [InlineData("05001234567", "abcdef")]
    public void Validate_InvalidData_FailsValidation(string phoneNumber, string code)
    {
        // Arrange
        var dto = new VerifyOtpDto
        {
            PhoneNumber = phoneNumber,
            Code = code
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }
}
