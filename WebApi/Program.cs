using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Application.Services;
using WebApi.Middlewares;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Domain.Interfaces.Services;
using Infrastructure.Data;
using Domain.Interfaces.Repositories;
using Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ------------------- CONFIGURAÇÃO DE LOG ------------------- //
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// ------------------- CONFIGURAÇÃO SWAGGER ------------------- //
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT no formato: Bearer {seu token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ------------------- CONFIGURAÇÃO JWT ------------------- //
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JwtSettings:SecretKey está ausente.");
var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JwtSettings:Issuer está ausente.");
var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JwtSettings:Audience está ausente.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
    
    // Customização para ler o token do cookie "jwt_token"
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            try
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(token))
                {
                    token = context.Request.Cookies["jwt_token"];
                }
                context.Token = token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[JwtBearer] Erro ao extrair token: {ex.Message}");
            }
            return Task.CompletedTask;
        }
    };
});

// ------------------- CONFIGURAÇÃO BANCO DE DADOS ------------------- //
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ??
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found."),
        sql => sql.MigrationsAssembly("backend_api.Data")
    )
);

// ------------------- CONFIGURAÇÃO CORS ------------------- //
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactAppPolicy", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "http://localhost:3000",
            "http://62.171.151.220"
            )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ------------------- DEPENDÊNCIAS ------------------- //
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UserService>();

// ------------------- CONFIGURAÇÃO JSON ------------------- //
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

var app = builder.Build();

// ------------------- AMBIENTE ------------------- //
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error"); // Crie um endpoint genérico de erro se necessário
    app.UseHsts(); // HTTPS estrito
}

// ------------------- MIDDLEWARES ------------------- //
app.UseCors("ReactAppPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RequestLoggingMiddleware>();
app.MapControllers();

// ------------------- APLICA MIGRATIONS AUTOMATICAMENTE ------------------- //
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        if (dbContext.Database.GetPendingMigrations().Any())
        {
            dbContext.Database.Migrate();
            Console.WriteLine("✅ Migrations aplicadas com sucesso.");
        }
        else
        {
            Console.WriteLine("ℹ️ Nenhuma migration pendente.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao aplicar migrations: {ex.Message}");
        throw;
    }
}

app.Run();
