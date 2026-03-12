using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuestBook_AdminPanel_SzM
{
    public partial class LoginWindow : Window
    {
        private readonly string url = "https://localhost:7231";
        public string AccessToken { get; private set; }
        public bool IsAdminAuthenticated { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            IsAdminAuthenticated = false;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Text = string.Empty;

            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrorMessage.Text = "Kérlek töltsd ki az összes mezõt!";
                return;
            }

            await LoginAsync(username, password);
        }

        private async Task LoginAsync(string username, string password)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var loginData = new
                    {
                        Name = username,
                        Gmail = "", // If your API requires Gmail, you might need to add a field
                        Password = password
                    };

                    string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(loginData);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync($"{url}/api/Auth/login", content);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        JObject tokenData = JObject.Parse(jsonResponse);
                        AccessToken = tokenData["accesToken"]?.ToString();

                        if (!string.IsNullOrEmpty(AccessToken))
                        {
                            // Verify user role
                            bool isAdmin = await VerifyAdminRoleAsync(AccessToken);
                            
                            if (isAdmin)
                            {
                                IsAdminAuthenticated = true;
                                DialogResult = true;
                                Close();
                            }
                            else
                            {
                                ErrorMessage.Text = "Hozzáférés megtagadva! Csak admin jogosultsággal rendelkezõ felhasználók léphetnek be.";
                            }
                        }
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        ErrorMessage.Text = $"Bejelentkezési hiba: {response.ReasonPhrase}";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = $"Hiba történt: {ex.Message}";
            }
        }

        private async Task<bool> VerifyAdminRoleAsync(string accessToken)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                    HttpResponseMessage response = await client.GetAsync($"{url}/api/Auth/CurrentUser");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        JObject userData = JObject.Parse(jsonResponse);
                        string role = userData["role"]?.ToString();

                        return role?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = $"Jogosultság ellenõrzési hiba: {ex.Message}";
            }

            return false;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}