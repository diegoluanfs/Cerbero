using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    // Construtor sem parâmetros (APENAS PARA MIGRATIONS E DESENVOLVIMENTO)
    public AppDbContext() { }

    // Construtor principal (usado pela aplicação)
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSets
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
        
        // Configuração de seeding para a tabela Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin", Description = "Administrador do sistema." },
            new Role { Id = 2, Name = "SupportManager", Description = "Gerente da equipe de suporte." },
            new Role { Id = 3, Name = "SupportAgent", Description = "Agente de suporte que resolve problemas." },
            new Role { Id = 4, Name = "Requester", Description = "Usuário que cria tickets." },
            new Role { Id = 5, Name = "Viewer", Description = "Usuário com acesso somente leitura." }
        );
    }
}