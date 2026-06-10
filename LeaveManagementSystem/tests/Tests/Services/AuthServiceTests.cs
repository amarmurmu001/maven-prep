using AutoMapper;
using Xunit;
using FluentAssertions;
using LeaveManagementSystem.Application.DTOs.Auth;
using LeaveManagementSystem.Application.Interfaces;
using LeaveManagementSystem.Application.Mappings;
using LeaveManagementSystem.Application.Services;
using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Domain.Enums;
using LeaveManagementSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace LeaveManagementSystem.Tests.Services;

public sealed class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _jwtServiceMock = new Mock<IJwtService>();
        _loggerMock = new Mock<ILogger<AuthService>>();
        _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()));
        _sut = new AuthService(_unitOfWorkMock.Object, _jwtServiceMock.Object, _mapper, _loggerMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ReturnsSuccess()
    {
        var request = new RegisterRequest("John", "Doe", "john@test.com", "Password123!", "Password123!");
        var userRepoMock = new Mock<IRepository<User>>();
        userRepoMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        userRepoMock.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User());
        _unitOfWorkMock.Setup(x => x.Repository<User>()).Returns(userRepoMock.Object);
        _jwtServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<IEnumerable<System.Security.Claims.Claim>>()))
            .Returns(("token", DateTime.UtcNow.AddHours(1)));
        _jwtServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("refresh-token");

        var result = await _sut.RegisterAsync(request);

        result.Success.Should().BeTrue();
        result.Message.Should().Be("Registration successful");
        result.AccessToken.Should().NotBeNull();
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ReturnsFailure()
    {
        var request = new RegisterRequest("John", "Doe", "existing@test.com", "Password123!", "Password123!");
        var userRepoMock = new Mock<IRepository<User>>();
        userRepoMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new User()]);
        _unitOfWorkMock.Setup(x => x.Repository<User>()).Returns(userRepoMock.Object);

        var result = await _sut.RegisterAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Email already registered");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ReturnsFailure()
    {
        var request = new LoginRequest("wrong@test.com", "wrongpass");
        var userRepoMock = new Mock<IRepository<User>>();
        userRepoMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _unitOfWorkMock.Setup(x => x.Repository<User>()).Returns(userRepoMock.Object);

        var result = await _sut.LoginAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid email or password");
    }
}
