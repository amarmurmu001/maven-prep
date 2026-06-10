using System.Security.Claims;
using System.Security.Cryptography;
using AutoMapper;
using LeaveManagementSystem.Application.DTOs.Auth;
using LeaveManagementSystem.Application.Interfaces;
using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Domain.Enums;
using LeaveManagementSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace LeaveManagementSystem.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IMapper mapper,
        ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var userRepo = _unitOfWork.Repository<User>();

        var existingUser = await userRepo.FindAsync(u => u.Email == request.Email.ToLower(), cancellationToken);
        if (existingUser.Any())
        {
            return new AuthResponse(false, "Email already registered");
        }

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email.ToLower(),
            PasswordHash = BCryptHash(request.Password),
            Role = UserRole.Employee
        };

        await userRepo.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User registered: {Email}", user.Email);

        var (accessToken, expiresAt) = GenerateTokens(user);
        return new AuthResponse(true, "Registration successful", accessToken, user.RefreshToken, expiresAt, _mapper.Map<UserDto>(user));
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var userRepo = _unitOfWork.Repository<User>();
        var users = await userRepo.FindAsync(u => u.Email == request.Email.ToLower(), cancellationToken);
        var user = users.FirstOrDefault();

        if (user is null || !BCryptVerify(request.Password, user.PasswordHash))
        {
            return new AuthResponse(false, "Invalid email or password");
        }

        if (!user.IsActive)
        {
            return new AuthResponse(false, "Account is deactivated. Contact administrator.");
        }

        var (accessToken, expiresAt) = GenerateTokens(user);
        user.RefreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User logged in: {Email}", user.Email);

        return new AuthResponse(true, "Login successful", accessToken, user.RefreshToken, expiresAt, _mapper.Map<UserDto>(user));
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal is null)
        {
            return new AuthResponse(false, "Invalid access token");
        }

        var userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            return new AuthResponse(false, "Invalid token payload");
        }

        var userRepo = _unitOfWork.Repository<User>();
        var user = await userRepo.GetByIdAsync(userId, cancellationToken);

        if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return new AuthResponse(false, "Invalid or expired refresh token");
        }

        var (accessToken, expiresAt) = GenerateTokens(user);
        user.RefreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse(true, "Token refreshed successfully", accessToken, user.RefreshToken, expiresAt, _mapper.Map<UserDto>(user));
    }

    public async Task LogoutAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userRepo = _unitOfWork.Repository<User>();
        var user = await userRepo.GetByIdAsync(userId, cancellationToken);

        if (user is not null)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("User logged out: {UserId}", userId);
        }
    }

    private (string accessToken, DateTime expiresAt) GenerateTokens(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        return _jwtService.GenerateAccessToken(claims);
    }

    private static string BCryptHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private static bool BCryptVerify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
