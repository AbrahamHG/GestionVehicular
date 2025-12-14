using GestionVehicular.Core.Dtos;


namespace GestionVehicular.Frontend.Services
{
    public class ConductorService
    {
        private readonly HttpClient _httpClient;

        public ConductorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Obtener lista de conductor
        public async Task<IEnumerable<ConductorDto>> GetconductorAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ConductorDto>>("api/conductor");
        }

        // Obtener un conductor por ID
        public async Task<ConductorDto?> GetConductorByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ConductorDto>($"api/conductor/{id}");
        }

        // Crear un nuevo conductor
        public async Task<HttpResponseMessage> CreateConductorAsync(ConductorDto conductor)
        {
            return await _httpClient.PostAsJsonAsync("api/conductor", conductor);
        }

        // Actualizar un conductor existente
        public async Task<HttpResponseMessage> UpdateConductorAsync(int id, ConductorDto conductor)
        {
            return await _httpClient.PutAsJsonAsync($"api/conductor/{id}", conductor);
        }

        // Eliminar un conductor
        public async Task<HttpResponseMessage> DeleteConductorAsync(int id)
        {
            return await _httpClient.DeleteAsync($"api/conductor/{id}");
        }
    }
}