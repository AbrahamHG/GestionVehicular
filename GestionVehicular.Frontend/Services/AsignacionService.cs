using GestionVehicular.Core.Dtos;
using System.Net.Http;
using System.Net.Http.Json;

public class AsignacionService
{
    private readonly HttpClient _http;

    public AsignacionService(HttpClient http)
    {
        _http = http;
    }

   
    public async Task<List<AsignacionDto>> GetAsignaciones()
        => await _http.GetFromJsonAsync<List<AsignacionDto>>("api/asignacion");

    
    public async Task<AsignacionDto> GetAsignacionById(int id)
        => await _http.GetFromJsonAsync<AsignacionDto>($"api/asignacion/{id}");

   
    public async Task<HttpResponseMessage> CrearAsignacion(AsignacionDto asignacion)
        => await _http.PostAsJsonAsync("api/asignacion", asignacion);

  
    public async Task<HttpResponseMessage> ActualizarAsignacion(AsignacionDto asignacion)
        => await _http.PutAsJsonAsync($"api/asignacion/{asignacion.Id}", asignacion);

   
    public async Task<HttpResponseMessage> EliminarAsignacion(int id)
        => await _http.DeleteAsync($"api/asignacion/{id}");

    public async Task<List<ConductorDto>> GetConductores()
    {
        return await _http.GetFromJsonAsync<List<ConductorDto>>("api/conductor");
    }

    public async Task<List<VehiculoDto>> GetVehiculosDisponibles()
    {
        return await _http.GetFromJsonAsync<List<VehiculoDto>>("api/vehiculo/disponible");
    }

}