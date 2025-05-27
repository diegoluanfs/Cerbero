using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Application.DTOs;
using Application.Services;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Autentica o usuário e retorna um token JWT.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (loginDto == null)
        {
            _logger.LogWarning("Tentativa de login com dados inválidos.");
            return BadRequest(new ApiResponse<string>(
                code: StatusCodes.Status400BadRequest,
                message: "Dados de login inválidos."
            ));
        }

        try
        {
            _logger.LogInformation("Tentativa de login para o usuário {Email}.", loginDto.Email);
            var authResponse = await _authService.AuthenticateAsync(loginDto);
            _logger.LogInformation("Login bem-sucedido para o usuário {Email}.", loginDto.Email);

            // Adiciona o token JWT em um cookie HTTP-only
            Response.Cookies.Append(
                "jwt_token",
                authResponse.Token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // Use true em produção (HTTPS)
                    SameSite = SameSiteMode.Strict, // Ou Lax, conforme necessidade
                    Expires = authResponse.Expiration
                }
            );

            return Ok(new ApiResponse<AuthResponseDto>(
                code: StatusCodes.Status200OK,
                message: "Autenticação realizada com sucesso.",
                data: authResponse
            ));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Falha na autenticação para o usuário {Email}. Motivo: {Message}", loginDto.Email, ex.Message);
            return Unauthorized(new ApiResponse<string>(
                code: StatusCodes.Status401Unauthorized,
                message: "Falha na autenticação.",
                errors: new List<string> { ex.Message }
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado durante o login para o usuário {Email}.", loginDto.Email);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>(
                code: StatusCodes.Status500InternalServerError,
                message: "Erro interno no servidor."
            ));
        }
    }

    /// <summary>
    /// Registra um novo usuário.
    /// </summary>
    [HttpPost("signup")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Signup([FromBody] SignupDto signupDto)
    {
        if (signupDto == null)
        {
            _logger.LogWarning("Tentativa de registro com dados inválidos.");
            return BadRequest(new ApiResponse<string>(
                code: StatusCodes.Status400BadRequest,
                message: "Dados de registro inválidos."
            ));
        }

        try
        {
            _logger.LogInformation("Tentativa de registro para o usuário {Email}.", signupDto.Email);
            await _authService.RegisterUserAsync(signupDto);
            _logger.LogInformation("Usuário {Email} registrado com sucesso.", signupDto.Email);
            return Created(string.Empty, new ApiResponse<string>(
                code: StatusCodes.Status201Created,
                message: "Usuário registrado com sucesso."
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar o usuário {Email}.", signupDto.Email);
            return BadRequest(new ApiResponse<string>(
                code: StatusCodes.Status400BadRequest,
                message: "Erro ao registrar o usuário.",
                errors: new List<string> { ex.Message }
            ));
        }
    }

    /// <summary>
    /// Realiza o logout do usuário, removendo o cookie JWT.
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Logout()
    {
        // Remove o cookie JWT
        Response.Cookies.Append(
            "jwt_token",
            "",
            new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Use true em produção (HTTPS)
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(-1) // Expira imediatamente
            }
        );

        return Ok(new ApiResponse<string>(
            code: StatusCodes.Status200OK,
            message: "Logout realizado com sucesso."
        ));
    }

    /// <summary>
    /// Retorna informações do usuário autenticado com base no JWT do cookie.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Unauthorized(new ApiResponse<string>(
                code: StatusCodes.Status401Unauthorized,
                message: "Usuário não autenticado."
            ));
        }

        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var roles = User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        return Ok(new ApiResponse<object>(
            code: StatusCodes.Status200OK,
            message: "Usuário autenticado.",
            data: new { userId, email, roles }
        ));
    }
}