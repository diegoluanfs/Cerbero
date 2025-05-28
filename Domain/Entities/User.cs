namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Nome completo do usuário</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Email do usuário</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Hash da senha do usuário</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>URL da imagem de perfil do usuário (pode ser nulo)</summary>
    public string? Picture { get; set; }

    /// <summary>ID do provedor externo (Firebase, etc.)</summary>
    public string? UserId { get; set; }

    /// <summary>Indica se o email do usuário foi verificado</summary>
    public bool EmailVerified { get; set; }

    /// <summary>Provedor de autenticação (ex: google, facebook, etc.)</summary>
    public string SignInProvider { get; set; } = string.Empty;

    /// <summary>Data de criação do registro do usuário</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Data da última atualização dos dados do usuário</summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>Lista de roles associadas ao usuário</summary>
    public ICollection<string> Roles { get; set; } = new List<string>();

    /// <summary> Id do tenant ao qual o usuário pertence</summary>
    public Guid TenantId { get; set; }
}