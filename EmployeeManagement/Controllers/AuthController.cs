using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Data;
using EmployeeManagement.Dtos;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    // POST: api/Auth/register
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto dto)
    {
        // Check if the email is already registered
        var email = dto.Email.Trim().ToLowerInvariant();
        var exists = await _context.Users.AnyAsync(u => u.Email == email);
        if (exists)
        {
            return BadRequest("Email is already registered.");
        }

        // Create a new user with hashed password and salt
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

        // For security reasons, do not return the password hash or salt in the response.
        return Ok(new { user.Id, user.DisplayName, user.Email });
    }

    [AllowAnonymous]
    // POST: api/Auth/login
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDto dto)
    {
        // 1) Retrieve the user by email
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }
        if (!user.IsActive)
        {
            return Unauthorized("User account is inactive.");
        }

        // 2) Verify password
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

        // 3) Compare hashes in constant time
        if (!CryptographicOperations.FixedTimeEquals(computedHash, user.PasswordHash))
        {
            return Unauthorized("Invalid email or password.");
        }

        // 4) Generate a token (access token)
        user.LoginAt = DateTime.UtcNow;
        var token = CreateToken();
        _context.UserTokens.Add(new UserToken
        {
            UserId = user.Id,
            Token = token,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(8) // Token valid for 8 hours
        });
        await _context.SaveChangesAsync();

        // 5) Return the token to the client
        return Ok(new
        {
            // standard OAuth / protocol style
            accesstoken = token,
            tokentype = "bearer",
            expiresat = DateTime.UtcNow.AddHours(8),
            user = new { user.Id, user.DisplayName, user.Email }
        });
    }

    private static string CreateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
        .Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }
}