using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartApiary.Application.Common.Behaviors;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure;
using SmartApiary.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

var jwtIssuer = builder.Configuration["JwtOptions:Issuer"]
    ?? throw new InvalidOperationException("JwtOptions:Issuer is not configured.");
var jwtAudience = builder.Configuration["JwtOptions:Audience"]
    ?? throw new InvalidOperationException("JwtOptions:Audience is not configured.");
var jwtKey = builder.Configuration["JwtOptions:Key"]
    ?? throw new InvalidOperationException("JwtOptions:Key is not configured.");

builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token."
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(SmartApiary.Application.Features.Users.Commands.CreateUserCommand).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(SmartApiary.Application.Features.Users.Commands.CreateUserCommand).Assembly);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

    var adminEmail = "admin@test.com";

    var existingAdmin = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);

    if (existingAdmin is null)
    {
        var admin = User.Create(
            "Admin",
            "Test",
            adminEmail,
            UserRole.Admin,
            Guid.NewGuid().ToString(),
            DateTime.UtcNow.AddDays(1));

        admin.Activate(passwordHasher.Hash("Admin123!"));

        dbContext.Users.Add(admin);
        await dbContext.SaveChangesAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ReactApp");
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();