using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Data;
using EmployeeManagement.Dtos;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text;
using EmployeeManagement.Models;
using EmployeeManagement.Service;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;

namespace EmployeeManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IValidator<RegisterDto> _registerValidator;
    private readonly IValidator<LoginDto> _loginValidator;
    private readonly JwtService _jwtService;

    public AuthController(
        ApplicationDbContext context,
        IValidator<RegisterDto> registerValidator,
        IValidator<LoginDto> loginValidator,
        JwtService jwtService)
    {
        _context = context;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _jwtService = jwtService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto dto)
    {
        var registerResult = await _registerValidator.ValidateAsync(dto);
        if (!registerResult.IsValid)
            return BadRequest(registerResult.Errors);

        var email = dto.Email.Trim().ToLowerInvariant();
        var exists = await _context.Users.AnyAsync(u => u.Email == email);
        if (exists)
            return BadRequest("Email is already registered.");

        using var hmac = new HMACSHA512();
        var user = new User
        {
            DisplayName = dto.DisplayName,
            Email = email,
            PasswordSalt = hmac.Key,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
            IsActive = true,
            RegisteredAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { user.Id, user.DisplayName, user.Email });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDto dto)
    {
        var loginResult = await _loginValidator.ValidateAsync(dto);
        if (!loginResult.IsValid)
            return BadRequest(loginResult.Errors);

        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null)
            return Unauthorized("Invalid email or password.");
        if (!user.IsActive)
            return Unauthorized("User account is inactive.");

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));
        if (!CryptographicOperations.FixedTimeEquals(computedHash, user.PasswordHash))
            return Unauthorized("Invalid email or password.");

        user.LoginAt = DateTime.UtcNow;

        // Access token: JWT, not stored in DB
        var accessToken = _jwtService.GenerateAccessToken(user);

        // Refresh token: opaque, stored in UserToken
        var refreshToken = CreateToken();
        _context.UserTokens.Add(new UserToken
        {
            UserId = user.Id,
            Token = refreshToken,
            Type = "Refresh",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        await _context.SaveChangesAsync();

        return Ok(new
        {
            accesstoken = accessToken,
            refreshtoken = refreshToken,
            tokentype = "bearer",
            user = new { user.Id, user.DisplayName, user.Email }
        });
    }

    private static string CreateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh(RefreshTokenDto dto)
    {
        var existing = await _context.UserTokens
            .FirstOrDefaultAsync(t => t.Token == dto.RefreshToken && t.Type == "Refresh");

        if (existing == null)
            return Unauthorized("Invalid refresh token.");

        // Reuse detection: token was already revoked before -> possible theft
        if (existing.RevokedAt != null)
        {
            var allUserTokens = await _context.UserTokens
                .Where(t => t.UserId == existing.UserId && t.Type == "Refresh" && t.RevokedAt == null)
                .ToListAsync();

            foreach (var t in allUserTokens)
                t.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Unauthorized("Refresh token reuse detected. All sessions have been revoked.");
        }

        if (existing.ExpiresAt < DateTime.UtcNow)
            return Unauthorized("Refresh token expired.");

        var user = await _context.Users.FindAsync(existing.UserId);
        if (user == null || !user.IsActive)
            return Unauthorized("User not found or inactive.");

        // Rotate: revoke the old refresh token
        existing.RevokedAt = DateTime.UtcNow;

        var newAccessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshToken = CreateToken();

        _context.UserTokens.Add(new UserToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            Type = "Refresh",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        await _context.SaveChangesAsync();

        return Ok(new
        {
            accesstoken = newAccessToken,
            refreshtoken = newRefreshToken,
            tokentype = "bearer",
            user = new { user.Id, user.DisplayName, user.Email }
        });
    }
}