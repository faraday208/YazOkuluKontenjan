using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using AutoMapper;
using YazOkulu.Application.DTOs.Auth;
using YazOkulu.Application.Services.Implementations;
using YazOkulu.Application.Services.Interfaces;
using YazOkulu.Application.Mappings;
using YazOkulu.Domain.Entities;
using YazOkulu.Domain.Enums;
using YazOkulu.Domain.Interfaces;
using YazOkulu.Tests.Helpers;
using System.Linq.Expressions;

namespace YazOkulu.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IRepository<Student>> _studentRepoMock;
    private readonly Mock<IRepository<OtpCode>> _otpRepoMock;
    private readonly Mock<ISmsService> _smsServiceMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _studentRepoMock = new Mock<IRepository<Student>>();
        _otpRepoMock = new Mock<IRepository<OtpCode>>();
        _smsServiceMock = new Mock<ISmsService>();
        _loggerMock = new Mock<ILogger<AuthService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _authService = new AuthService(
            _studentRepoMock.Object,
            _otpRepoMock.Object,
            _smsServiceMock.Object,
            _mapper,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task RequestOtpAsync_ValidPhone_ReturnsSuccess()
    {
        // Arrange
        var dto = new RequestOtpDto { PhoneNumber = "05001234567" };
        _smsServiceMock.Setup(x => x.SendOtpAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.RequestOtpAsync(dto);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("Doğrulama kodu");
        _otpRepoMock.Verify(x => x.AddAsync(It.IsAny<OtpCode>()), Times.Once);
        _smsServiceMock.Verify(x => x.SendOtpAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task RequestOtpAsync_SmsServiceFails_ReturnsFailure()
    {
        // Arrange
        var dto = new RequestOtpDto { PhoneNumber = "05001234567" };
        _smsServiceMock.Setup(x => x.SendOtpAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("SMS service error"));

        // Act
        var result = await _authService.RequestOtpAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("hata");
    }

    [Fact]
    public async Task VerifyOtpAsync_CorrectCode_ReturnsSuccessWithToken()
    {
        // Arrange
        var phone = "5001234567";
        var code = "123456";
        var student = MockData.CreateStudent(1, phone);
        var otpCode = MockData.CreateOtpCode(phone, code, OtpStatus.Active);

        var dto = new VerifyOtpDto { PhoneNumber = phone, Code = code };

        _otpRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<OtpCode, bool>>>()))
            .ReturnsAsync(otpCode);
        _studentRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Student, bool>>>()))
            .ReturnsAsync(student);

        // Act
        var result = await _authService.VerifyOtpAsync(dto);

        // Assert
        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        result.Student.Should().NotBeNull();
        result.Student!.PhoneNumber.Should().Be(phone);
        _otpRepoMock.Verify(x => x.UpdateAsync(It.IsAny<OtpCode>()), Times.Once);
    }

    [Fact]
    public async Task VerifyOtpAsync_WrongCode_ReturnsFailure()
    {
        // Arrange
        var dto = new VerifyOtpDto { PhoneNumber = "5001234567", Code = "999999" };

        _otpRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<OtpCode, bool>>>()))
            .ReturnsAsync((OtpCode?)null);

        // Act
        var result = await _authService.VerifyOtpAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Geçersiz doğrulama kodu");
    }

    [Fact]
    public async Task VerifyOtpAsync_ExpiredCode_ReturnsFailure()
    {
        // Arrange
        var phone = "5001234567";
        var code = "123456";
        var expiredOtp = MockData.CreateOtpCode(
            phone,
            code,
            OtpStatus.Active,
            DateTime.UtcNow.AddMinutes(-10)
        );

        var dto = new VerifyOtpDto { PhoneNumber = phone, Code = code };

        _otpRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<OtpCode, bool>>>()))
            .ReturnsAsync(expiredOtp);

        // Act
        var result = await _authService.VerifyOtpAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Doğrulama kodunun süresi dolmuş");
    }

    [Fact]
    public async Task VerifyOtpAsync_AlreadyUsedCode_ReturnsFailure()
    {
        // Arrange
        var phone = "5001234567";
        var code = "123456";

        // Used code won't be found by FirstOrDefaultAsync (Status == Active filter)
        var dto = new VerifyOtpDto { PhoneNumber = phone, Code = code };

        _otpRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<OtpCode, bool>>>()))
            .ReturnsAsync((OtpCode?)null);

        // Act
        var result = await _authService.VerifyOtpAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Geçersiz doğrulama kodu");
    }

    [Fact]
    public async Task VerifyOtpAsync_NewStudent_CreatesStudentAutomatically()
    {
        // Arrange
        var phone = "5009999999";
        var code = "123456";
        var otpCode = MockData.CreateOtpCode(phone, code, OtpStatus.Active);

        var dto = new VerifyOtpDto { PhoneNumber = phone, Code = code };

        _otpRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<OtpCode, bool>>>()))
            .ReturnsAsync(otpCode);
        _studentRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Student, bool>>>()))
            .ReturnsAsync((Student?)null); // Student yok

        // Act
        var result = await _authService.VerifyOtpAsync(dto);

        // Assert
        result.Success.Should().BeTrue();
        result.Student.Should().NotBeNull();
        _studentRepoMock.Verify(x => x.AddAsync(It.IsAny<Student>()), Times.Once);
    }
}
