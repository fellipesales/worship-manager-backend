using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WorshipManager.Application.DTOs;
using WorshipManager.Application.Interfaces;
using WorshipManager.Core.Entities;

namespace WorshipManager.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOrganizationService _organizationService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    private static readonly HashSet<string> SuperAdminEmails = new(StringComparer.OrdinalIgnoreCase)
    {
        "fellipesales@gmail.com"
    };

    public AuthController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IOrganizationService organizationService,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _organizationService = organizationService;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Login with email/password, returns JWT
    /// </summary>
    [HttpPost("token")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return Unauthorized(new { message = "Email ou senha inválidos" });

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Email ou senha inválidos" });

        user.LastLoginAt = DateTime.UtcNow;
        var shouldBeSuperAdmin = SuperAdminEmails.Contains(user.Email ?? "");
        if (user.IsSuperAdmin != shouldBeSuperAdmin)
            user.IsSuperAdmin = shouldBeSuperAdmin;
        await _userManager.UpdateAsync(user);

        var tokenResponse = await GenerateTokenResponseAsync(user);
        return Ok(tokenResponse);
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponseDto>> Register([FromBody] RegisterRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return BadRequest(new { message = "Este email já está cadastrado" });

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow,
            IsActive = true,
            IsSuperAdmin = SuperAdminEmails.Contains(dto.Email)
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new { message = errors });
        }

        _logger.LogInformation("New user registered: {Email}", dto.Email);
        var tokenResponse = await GenerateTokenResponseAsync(user);
        return CreatedAtAction(nameof(GetMe), tokenResponse);
    }

    /// <summary>
    /// Refresh an expired access token
    /// </summary>
    [HttpPost("token/refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Validate the refresh token by finding the user
        var user = await FindUserByRefreshTokenAsync(dto.RefreshToken);
        if (user == null)
            return Unauthorized(new { message = "Token de refresh inválido" });

        var tokenResponse = await GenerateTokenResponseAsync(user);
        return Ok(tokenResponse);
    }

    /// <summary>
    /// Initiate external OAuth login
    /// </summary>
    [HttpGet("external-login")]
    [AllowAnonymous]
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
        var redirectUrl = Url.Action("ExternalLoginCallback", "Auth", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    /// <summary>
    /// Handle external OAuth callback
    /// </summary>
    [HttpGet("external-login-callback")]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
        if (remoteError != null)
        {
            _logger.LogWarning("External provider error: {Error}", remoteError);
            return BadRequest(new { message = $"Erro do provedor: {remoteError}" });
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return BadRequest(new { message = "Erro ao obter informações de login" });

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var name = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email;
        var photoUrl = info.Principal.FindFirstValue("picture") ??
                       info.Principal.FindFirstValue("urn:google:picture");

        if (string.IsNullOrEmpty(email))
            return BadRequest(new { message = "Email não fornecido pelo provedor" });

        // Try to find existing user
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            // Link external login if not already linked
            var logins = await _userManager.GetLoginsAsync(user);
            if (!logins.Any(l => l.LoginProvider == info.LoginProvider))
                await _userManager.AddLoginAsync(user, info);

            user.LastLoginAt = DateTime.UtcNow;
            user.PhotoUrl ??= photoUrl;
            var shouldBeSuperAdmin = SuperAdminEmails.Contains(email);
            if (user.IsSuperAdmin != shouldBeSuperAdmin)
                user.IsSuperAdmin = shouldBeSuperAdmin;
            await _userManager.UpdateAsync(user);
        }
        else
        {
            // Create new user
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = name ?? "Usuário",
                PhotoUrl = photoUrl,
                AuthProvider = info.LoginProvider,
                ExternalId = info.ProviderKey,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                IsActive = true,
                IsSuperAdmin = SuperAdminEmails.Contains(email)
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                return BadRequest(new { message = "Erro ao criar conta" });

            await _userManager.AddLoginAsync(user, info);
            _logger.LogInformation("New user created via {Provider}: {Email}", info.LoginProvider, email);
        }

        var tokenResponse = await GenerateTokenResponseAsync(user);

        // Redirect to frontend with token
        var frontendUrl = returnUrl ?? _configuration["Cors:AllowedOrigins"]?.Split(',').FirstOrDefault() ?? "http://localhost:3000";
        return Redirect($"{frontendUrl}/auth/callback?token={tokenResponse.AccessToken}&refresh={tokenResponse.RefreshToken}");
    }

    /// <summary>
    /// Exchange external OAuth code for app JWT
    /// </summary>
    [HttpPost("external/token")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponseDto>> ExchangeExternalToken([FromBody] ExternalTokenRequestDto dto)
    {
        // This endpoint is used by the frontend after OAuth redirect
        // The frontend sends the provider and authorization code
        // For now, this is handled via the callback flow above
        return BadRequest(new { message = "Use the /auth/external-login flow instead" });
    }

    /// <summary>
    /// Get current user info from JWT
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        string? orgRole = null;
        if (user.CurrentOrganizationId.HasValue)
        {
            var role = await _organizationService.GetUserRoleAsync(userId, user.CurrentOrganizationId.Value);
            orgRole = role?.ToString();
        }

        return Ok(new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? "",
            PhotoUrl = user.PhotoUrl,
            IsSuperAdmin = user.IsSuperAdmin,
            CurrentOrganizationId = user.CurrentOrganizationId,
            OrganizationRole = orgRole
        });
    }

    /// <summary>
    /// Logout (invalidate refresh token)
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Clear refresh token
                await _userManager.RemoveAuthenticationTokenAsync(user, "WorshipManager", "RefreshToken");
            }
        }
        return Ok(new { message = "Logout realizado com sucesso" });
    }

    private async Task<TokenResponseDto> GenerateTokenResponseAsync(ApplicationUser user)
    {
        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        // Store refresh token
        await _userManager.SetAuthenticationTokenAsync(user, "WorshipManager", "RefreshToken", refreshToken);

        string? orgRole = null;
        if (user.CurrentOrganizationId.HasValue)
        {
            var role = await _organizationService.GetUserRoleAsync(user.Id, user.CurrentOrganizationId.Value);
            orgRole = role?.ToString();
        }

        var expiresInMinutes = int.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "60");

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes),
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                PhotoUrl = user.PhotoUrl,
                IsSuperAdmin = user.IsSuperAdmin,
                CurrentOrganizationId = user.CurrentOrganizationId,
                OrganizationRole = orgRole
            }
        };
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var secret = _configuration["Jwt:Secret"]!;
        var issuer = _configuration["Jwt:Issuer"] ?? "WorshipManager";
        var audience = _configuration["Jwt:Audience"] ?? "WorshipManagerApp";
        var expiresInMinutes = int.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "60");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? ""),
            new(ClaimTypes.Name, user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (user.CurrentOrganizationId.HasValue)
            claims.Add(new Claim("OrganizationId", user.CurrentOrganizationId.Value.ToString()));

        if (user.IsSuperAdmin)
            claims.Add(new Claim("IsSuperAdmin", "true"));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private async Task<ApplicationUser?> FindUserByRefreshTokenAsync(string refreshToken)
    {
        // Search through all users for the matching refresh token
        // In production, consider storing refresh tokens in a dedicated table for efficiency
        var users = _userManager.Users.ToList();
        foreach (var user in users)
        {
            var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "WorshipManager", "RefreshToken");
            if (storedToken == refreshToken)
                return user;
        }
        return null;
    }
}
