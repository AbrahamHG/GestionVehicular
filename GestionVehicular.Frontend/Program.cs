using GestionVehicular.Frontend.Data;
using GestionVehicular.Frontend.Services;
using GestionVehicular.Core.Validators;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using FluentValidation;
using Blazored.FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Servicios personalizados
// Configuración única de HttpClient
builder.Services.AddHttpClient("GestionVehicularAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7147"); // Ajusta al puerto real de tu API
});

// Registro de servicios usando el HttpClient configurado
builder.Services.AddScoped<ConductorService>(sp =>
    new ConductorService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("GestionVehicularAPI")));

builder.Services.AddScoped<VehiculoService>(sp =>
    new VehiculoService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("GestionVehicularAPI")));

builder.Services.AddScoped<AsignacionService>(sp =>
    new AsignacionService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("GestionVehicularAPI")));


// FluentValidation: solo registrar validadores
builder.Services.AddValidatorsFromAssemblyContaining<ConductorValidator>();

// Servicios Blazor
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7147"); // Ajusta el puerto si tu API usa otro
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();