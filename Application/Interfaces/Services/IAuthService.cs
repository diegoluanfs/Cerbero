using Application.DTOs;

public interface IAuthService
{
    /// <summary>
    /// Autentica um usuário com base nas credenciais fornecidas.
    /// </summary>
    /// <param name="loginDto">DTO contendo e-mail e senha do usuário.</param>
    /// <returns>Um objeto AuthResponseDto contendo o token JWT e a data de expiração.</returns>
    Task<AuthResponseDto> AuthenticateAsync(LoginDto loginDto);

    /// <summary>
    /// Registra um novo usuário no sistema.
    /// </summary>
    /// <param name="signupDto">DTO contendo os dados do usuário a ser registrado.</param>
    Task RegisterUserAsync(SignupDto signupDto);
}