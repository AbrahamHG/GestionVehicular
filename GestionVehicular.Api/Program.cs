using FluentValidation;
using FluentValidation.AspNetCore;
using GestionVehicular.Core.Dtos;
using GestionVehicular.Core.Validators;
using GestionVehicular.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

//Logs
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning) // ignora info/debug de ASP.NET
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)    // ignora info/debug del sistema
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();


// Add services to the container.

//Servicios
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "API Gestión Vehicular", Version = "v1" });
    c.ExampleFilters();
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<VehiculoDto>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<ConductorDto>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<AsignacionDto>();

// Registrar DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("sql_connection"),
        sqlServerOptionsAction: sqloption =>
        {
            sqloption.EnableRetryOnFailure(
                maxRetryCount: 20,
                maxRetryDelay: TimeSpan.FromSeconds(15),
                errorNumbersToAdd: null);
        }
    )
);
//FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<VehiculoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ConductorValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AsignacionValidator>();
builder.Services.AddFluentValidationAutoValidation();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gestión Vehicular v1");
        c.RoutePrefix = string.Empty; // Swagger en raíz
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
