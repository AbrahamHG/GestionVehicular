using System.Net.Http.Json;
using GestionVehicular.Core.Dtos;

public class VehiculoService
{
    private readonly HttpClient _http;

    public VehiculoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<VehiculoDto>> GetVehiculos()
        => await _http.GetFromJsonAsync<List<VehiculoDto>>("api/vehiculo");

    public async Task<VehiculoDto> GetVehiculoById(int id)
        => await _http.GetFromJsonAsync<VehiculoDto>($"api/vehiculo/{id}");

    public async Task<HttpResponseMessage> CrearVehiculo(VehiculoDto vehiculo)
        => await _http.PostAsJsonAsync("api/vehiculo", vehiculo);

    public async Task<HttpResponseMessage> ActualizarVehiculo(VehiculoDto vehiculo)
        => await _http.PutAsJsonAsync($"api/vehiculo/{vehiculo.Id}", vehiculo);

    public async Task<HttpResponseMessage> EliminarVehiculo(int id)
        => await _http.DeleteAsync($"api/vehiculo/{id}");
}