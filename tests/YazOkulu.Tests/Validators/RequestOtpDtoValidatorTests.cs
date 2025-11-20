using FluentAssertions;
using YazOkulu.Application.DTOs.Auth;
using YazOkulu.Application.Validators;

namespace YazOkulu.Tests.Validators;

public class RequestOtpDtoValidatorTests
{
    private readonly RequestOtpValidator _validator;

    public RequestOtpDtoValidatorTests()
    {
        _validator = new RequestOtpValidator();
    }

    [Theory]
    [InlineData("05001234567")]
    [InlineData("5001234567")]
    [InlineData("+905001234567")]
    public void Validate_ValidPhoneNumber_PassesValidation(string phoneNumber)
    {
        // Arrange
        var dto = new RequestOtpDto { PhoneNumber = phoneNumber };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("abc")]
    [InlineData("12345")]
    public void Validate_InvalidPhoneNumber_FailsValidation(string phoneNumber)
    {
        // Arrange
        var dto = new RequestOtpDto { PhoneNumber = phoneNumber };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }
}
