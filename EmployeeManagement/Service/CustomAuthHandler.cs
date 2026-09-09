using System;
using System.Collections.Generic;
using System.Security.Claims;
using EmployeeManagement.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EmployeeManagement.Service;

public class CustomAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{

    private readonly ApplicationDbContext _context;
    public CustomAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ApplicationDbContext context)
        : base(options, logger, encoder)
    {
        _context = context;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string token = null;

        // 1) Try Authorization header first
        if (Request.Headers.TryGetValue("Authorization", out var headerValues))
        {
            var header = headerValues.ToString();
            if (header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = header["Bearer ".Length..].Trim();
            }
        }

        // 2) If not found in header, try query string
        if (string.IsNullOrWhiteSpace(token))
        {
            token = Request.Query["access_token"].ToString();
        }

        // 3)If token is still not found, return error message
        if (string.IsNullOrWhiteSpace(token))
        {
            return AuthenticateResult.Fail("No token provided.");
        }

        // 4)Get current time since we need to check expiry
        var now = DateTime.UtcNow;

        // 5) Create session: user token must exist, not revoked, and not expired
        var session = await _context.UserTokens
            .FirstOrDefaultAsync(t =>
            t.Token == token &&
            t.RevokedAt == null &&
            t.ExpiresAt > now);

        // 6) If session is null, return error message
        if (session == null)
        {
            return AuthenticateResult.Fail("Invalid or expired token.");
        }

        // 7) Get user and check if user is active
        var user = await _context.Users.FindAsync(session.UserId);

        if (user == null || !user.IsActive)
            return AuthenticateResult.Fail("User not found or inactive.");

        // 8) Create ClaimsPrincipal for the authenticated user
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.DisplayName),
            new Claim(ClaimTypes.Email, user.Email)
        };


        // 9) Create identity and principal
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);


        // 10) Return success result with the authentication ticket
        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }
}
