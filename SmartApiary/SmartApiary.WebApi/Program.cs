using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartApiary.Application.Common.Behaviors;
using SmartApiary.Application.Interfaces.Email;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Application.Interfaces.Services;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure;
using SmartApiary.Application;
using SmartApiary.Infrastructure.Persistence;
using SmartApiary.Infrastructure.Services.Email;
using SmartApiary.WebApi.Extensions;
using SmartApiary.WebApi.Hubs;
using SmartApiary.WebApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddWebServices()
    .AddIdentityServices(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

var app = builder.Build();

await app.SeedDatabaseAsync();

app.UseCors("ReactApp");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<SmartScaleHub>("/smartScaleHub");

app.Run();