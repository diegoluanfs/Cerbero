using Support.Application.DTOs;
using Support.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PasswordVerificationResult = Microsoft.AspNetCore.Identity.PasswordVerificationResult;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly UserDomainService _userDomainService;
    private readonly IConfiguration _configuration;

    public AuthService(UserDomainService userDomainService, IConfiguration configuration)
    {
        _userDomainService = userDomainService ?? throw new ArgumentNullException(nameof(userDomainService));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<AuthResponseDto> AuthenticateAsync(LoginDto loginDto)
    {
        var user = await _userDomainService.GetUserByEmailAsync(loginDto.Email);
        if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Credenciais inválidas.");
        }

        var token = GenerateJwtToken(user);
        return new AuthResponseDto
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(1)
        };
    }

    public async Task RegisterUserAsync(SignupDto signupDto)
    {
        var existingUser = await _userDomainService.GetUserByEmailAsync(signupDto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Usuário já registrado.");
        }

        var user = new User
        {
            Name = signupDto.Name,
            Email = signupDto.Email,
            PasswordHash = HashPassword(signupDto.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Roles = new List<string> { "Viewer", "Requester" }
        };

        await _userDomainService.CreateUserAsync(user);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        // Adicionar roles como claims
        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string HashPassword(string password)
    {
        return new PasswordHasher<object?>().HashPassword(null, password);
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        var result = new PasswordHasher<object?>().VerifyHashedPassword(null, passwordHash, password);
        return result == PasswordVerificationResult.Success;
    }
}