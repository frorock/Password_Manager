using Password.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BCrypt.Net;
using System.Linq;
using Xamarin.Essentials;

namespace Password.Services
{
    public class UserData_DAO
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "http://iliana.alwaysdata.net"; 

        public UserData_DAO()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<UserData>> GetAllUser()
        {
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/GetAllUsers");
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<UserData>>(jsonString);
        }

        public async Task AddUser(UserData user)
        {
            var jsonData = JsonSerializer.Serialize(user);
            var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
            await _httpClient.PostAsync($"{ApiBaseUrl}/AddUsers", content);
        }

        public async Task EditUser(UserData user)
        {
            var jsonData = JsonSerializer.Serialize(user);
            var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
            await _httpClient.PutAsync($"{ApiBaseUrl}/EditUsers/{user.Id}", content);
        }

        public async Task DeleteUser(int userId)
        {
            await _httpClient.DeleteAsync($"{ApiBaseUrl}/DelUsers/{userId}");
        }

        public async Task<bool> CheckLogin(string user, string password)
        {
            var users = await GetAllUser();
            var userData = users.FirstOrDefault(u => u.UserName == user);

            if (userData != null && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(userData.PasswordHash))
            {
                if (BCrypt.Net.BCrypt.Verify(password, userData.PasswordHash))
                {
                    // Définissez la préférence avant de renvoyer true
                    Preferences.Set("IdUser", userData.Id.ToString());
                    Preferences.Set("IV", userData.IV);

                    return true;
                }
            }

            return false;
        }



    }
}

