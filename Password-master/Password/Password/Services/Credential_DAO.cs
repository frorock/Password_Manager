using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Password.Models;
using Password.Services;

namespace Password.Services
{
    internal class Credential_DAO
    {
        private readonly string _baseApiUrl = "http://iliana.alwaysdata.net"; 
        private readonly HttpClient _httpClient;
        public Credential_DAO()
        {
            _httpClient = new HttpClient();
        }
  

        public async Task<bool> AddCredential(Credential credential)
        {
            var jsonData = JsonConvert.SerializeObject(credential);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseApiUrl}/AddCredentials", content);
            return response.IsSuccessStatusCode;
        }
        public async Task<List<Credential>> GetAllCredentials()
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/GetAllCredentials");
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Credential>>(jsonResponse);
            }
            else
            {
                    throw new Exception($"Erreur lors de la récupération des données. Statut : {response.StatusCode}");
            }
        }

        public async Task<bool> EditCredential(Credential credential)
        {
            var jsonData = JsonConvert.SerializeObject(credential);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseApiUrl}/EditCredentials/{credential.Id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCredential(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseApiUrl}/DelCredentials/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
