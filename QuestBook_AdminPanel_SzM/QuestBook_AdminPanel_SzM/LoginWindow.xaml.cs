using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuestBook_AdminPanel_SzM
{
    public partial class LoginWindow : Window
    {
        private readonly string url = "https://localhost:7231";
        public string? AccessToken { get; private set; }
        public string? UserRole { get; private set; }
        public bool IsAdminAuthenticated { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            IsAdminAuthenticated = false;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            

            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string gmail = GmailTextBox.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(gmail))
            {
                ErrorMessage.Text = "Kérlek töltsd ki az összes mezõt!";
                return;
            }

            await LoginAsync(username, password, gmail);
        }

        private async Task LoginAsync(string username, string password, string gmail)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var loginData = new
                    {
                        Name = username,
                        Gmail = gmail, 
                        Password = password
                        
                    };

                    string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(loginData);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync($"{url}/api/Auth/login", content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Bejelentkezés sikeres!");
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        JObject tokenData = JObject.Parse(jsonResponse);
                        AccessToken = tokenData["AccesToken"]?.ToString() ?? tokenData["accesToken"]?.ToString();

                        if (string.IsNullOrEmpty(AccessToken))
                        {
                            ErrorMessage.Text = "A bejelentkezés sikerült, de a token nem érkezett meg.";
                            return;
                        }

                        UserRole = GetUserRole(AccessToken);
                        IsAdminAuthenticated = string.Equals(UserRole, "Admin", StringComparison.OrdinalIgnoreCase);

                        if (!IsAdminAuthenticated)
                        {
                            ErrorMessage.Text = "Csak admin felhasználó tud belépni.";
                            AccessToken = null;
                            return;
                        }

                        DialogResult = true;
                        
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        ErrorMessage.Text = string.IsNullOrWhiteSpace(errorContent)
                            ? $"Bejelentkezési hiba: {response.ReasonPhrase}"
                            : $"Bejelentkezési hiba: {errorContent}";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = $"Hiba történt: {ex.Message}";
            }
        }

        private string? GetUserRole(string accessToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(accessToken);

                var roleClaim = token.Claims.FirstOrDefault(c =>
                    c.Type == ClaimTypes.Role);

                return roleClaim?.Value;
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = $"Token feldolgozási hiba: {ex.Message}";
                return null;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            
        }
    }
}