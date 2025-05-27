namespace Application.DTOs;

public class UpdateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid? SectorId { get; set; } // Exemplo de campo opcional
}