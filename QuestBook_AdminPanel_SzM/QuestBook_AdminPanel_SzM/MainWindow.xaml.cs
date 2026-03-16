using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace QuestBook_AdminPanel_SzM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string url = "https://localhost:7231";
        private readonly string accessToken;
        private List<UserModel> playerList = [];
        private List<Lobbies> lobbyList = [];
        private List<LocationModel> locationList = [];

        public MainWindow(string token)
        {
            InitializeComponent();
            accessToken = token;
        }
        
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await LoadUsersAsync();
        }
        private async Task LoadUsersAsync()
        {
            playerList = [];
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);

                    var getUsersApiUrl = $"{url}/api/Auth/GetAllUsers";
                    HttpResponseMessage response = await client.GetAsync(getUsersApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var users = JsonConvert.DeserializeObject<List<UserModel>>(jsonResponse);
                        playerList = users ?? [];
                        PlayerResult.ItemsSource = playerList;
                    }
                    else
                    {
                        MessageBox.Show($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }

        
        private async void GetLobbies(object sender, RoutedEventArgs e)
        {
            await LoadLobbies();
        }
        private async Task LoadLobbies()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);

                    var getLobbiesApiUrl = $"{url}/api/Auth/GetAllLobbies";
                    HttpResponseMessage response = await client.GetAsync(getLobbiesApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var lobbies = JsonConvert.DeserializeObject<List<Lobbies>>(jsonResponse);
                        lobbyList = lobbies ?? [];
                        LobbyResult.ItemsSource = lobbyList;
                    }
                    else
                    {
                        MessageBox.Show($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }
        private void GetLocations(object sender, RoutedEventArgs e)
        {
            LoadLocations();
        }
        private async Task LoadLocations()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);

                    var getLocationsApiUrl = $"{url}/api/Auth/GetLocations";
                    HttpResponseMessage response = await client.GetAsync(getLocationsApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var locations = JsonConvert.DeserializeObject<List<LocationModel>>(jsonResponse);
                        locationList = locations ?? [];
                        LocationResult.ItemsSource = locations;
                    }
                    else
                    {
                        MessageBox.Show($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }
        private void UserTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            UserModel selected = PlayerResult.SelectedItem as UserModel;
            nametext.Text = selected.Name;


        }
        private async void ChangeRoleToAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerResult.SelectedItem is not UserModel selectedUser)
            {
                MessageBox.Show("Válassz ki egy felhasználót az admin ranghoz.");
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);

                    var ChangeRoleApiUrl = $"{url}/api/Auth/ChangeToAdmin?Id={selectedUser.Id}";
                    HttpResponseMessage response = await client.PatchAsync(ChangeRoleApiUrl, null);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Felhasználó sikeresen admin lett {selectedUser.Name}");
                        await LoadUsersAsync();
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(string.IsNullOrWhiteSpace(errorContent)
                            ? $"Error: {response.StatusCode} - {response.ReasonPhrase}"
                            : errorContent);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }
        private async void ChangeRoleToUser_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerResult.SelectedItem is not UserModel selectedUser)
            {
                MessageBox.Show("Válassz ki egy felhasználót az admin ranghoz.");
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);

                    var ChangeRoleApiUrl = $"{url}/api/Auth/ChangeToUser?Id={selectedUser.Id}";
                    HttpResponseMessage response = await client.PatchAsync(ChangeRoleApiUrl, null);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Felhasználó sikeresen felhasználó lett {selectedUser.Name}");
                        await LoadUsersAsync();
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(string.IsNullOrWhiteSpace(errorContent)
                            ? $"Error: {response.StatusCode} - {response.ReasonPhrase}"
                            : errorContent);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }
        private async void GiveWarning_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerResult.SelectedItem is not UserModel selectedUser)
            {
                MessageBox.Show("Válassz ki egy felhasználót a figyelmeztetéshez.");
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);

                    var giveWarningApiUrl = $"{url}/api/Auth/GiveWarning?Id={selectedUser.Id}";
                    HttpResponseMessage response = await client.PatchAsync(giveWarningApiUrl, null);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Figyelmeztetés sikeresen kiosztva: {selectedUser.Name}");
                        await LoadUsersAsync();
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(string.IsNullOrWhiteSpace(errorContent)
                            ? $"Error: {response.StatusCode} - {response.ReasonPhrase}"
                            : errorContent);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }

    
    private async void SuspendUser_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerResult.SelectedItem is not UserModel selectedUser)
            {
                MessageBox.Show("Válassz ki egy felhasználót a figyelmeztetéshez.");
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);

                    var SuspendUserApiUrl = $"{url}/api/Auth/SuspendUser?Id={selectedUser.Id}";
                    HttpResponseMessage response = await client.PatchAsync(SuspendUserApiUrl, null);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Felhasználó sikeresen felfüggesztve {selectedUser.Name}");
                        await LoadUsersAsync();
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(string.IsNullOrWhiteSpace(errorContent)
                            ? $"Error: {response.StatusCode} - {response.ReasonPhrase}"
                            : errorContent);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }

        private async void UnSuspendUser_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerResult.SelectedItem is not UserModel selectedUser)
            {
                MessageBox.Show("Válassz ki egy felhasználót a felmentéshez.");
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);

                    var UnSuspendUserApiUrl = $"{url}/api/Auth/UnSuspendUser?Id={selectedUser.Id}";
                    HttpResponseMessage response = await client.PatchAsync(UnSuspendUserApiUrl, null);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Felhasználó sikeresen felmentve {selectedUser.Name}");
                        await LoadUsersAsync();
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(string.IsNullOrWhiteSpace(errorContent)
                            ? $"Error: {response.StatusCode} - {response.ReasonPhrase}"
                            : errorContent);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }
        private async void Ban_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerResult.SelectedItem is not UserModel selectedUser)
            {
                MessageBox.Show("Válassz ki egy lobbyt a törléshez.");
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);

                    var banUserApiUrl = $"{url}/api/Auth/BanUser/{selectedUser.Id}";
                    HttpResponseMessage response = await client.DeleteAsync(banUserApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Lobby törölve{selectedUser.Name}");
                        await LoadUsersAsync();
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(string.IsNullOrWhiteSpace(errorContent)
                            ? $"Error: {response.StatusCode} - {response.ReasonPhrase}"
                            : errorContent);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }
        
        private async void DeleteLobby_Click(object sender, RoutedEventArgs e)
        {
            if (LobbyResult.SelectedItem is not Lobbies selectedLobby)
            {
                MessageBox.Show("Válassz ki egy felhasználót a bannoláshoz.");
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);

                    var LobbyApiUrl = $"{url}/api/Auth/DeleteLobby/{selectedLobby.Id}";
                    HttpResponseMessage response = await client.DeleteAsync(LobbyApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Lobby törölve {selectedLobby.LobbyName}");
                        await LoadUsersAsync();
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(string.IsNullOrWhiteSpace(errorContent)
                            ? $"Error: {response.StatusCode} - {response.ReasonPhrase}"
                            : errorContent);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }

        private async void DeleteLocation_Click(object sender, RoutedEventArgs e)
        {
            if (LocationResult.SelectedItem is not LocationModel selectedLocations)
            {
                MessageBox.Show("Válassz ki egy helyszínt a törléshez.");
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);

                    var LocationApiUrl = $"{url}/api/Auth/DeleteLocation/{selectedLocations.Id}";
                    HttpResponseMessage response = await client.DeleteAsync(LocationApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Helyszín törölve {selectedLocations.LocationName}");
                        await LoadUsersAsync();
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(string.IsNullOrWhiteSpace(errorContent)
                            ? $"Error: {response.StatusCode} - {response.ReasonPhrase}"
                            : errorContent);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }
        private async void RemoveUserFromLobby_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerResult.SelectedItem is not UserModel selectedUser)
            {
                MessageBox.Show("Válassz ki egy felhasználót.");
                return;
            }

            if (LobbyResult.SelectedItem is not Lobbies selectedLobby)
            {
                MessageBox.Show("Válassz ki egy lobbyt.");
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);

                    var removeUserApiUrl = $"{url}/api/Auth/RemovePlayerFromLobby?lobbyId={selectedLobby.Id}&userId={selectedUser.Id}";
                    HttpResponseMessage response = await client.DeleteAsync(removeUserApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"{selectedUser.Name} el lett távolítva a(z) {selectedLobby.LobbyName} lobbyból.");
                        await LoadUsersAsync();
                        await LoadLobbies();
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(string.IsNullOrWhiteSpace(errorContent)
                            ? $"Error: {response.StatusCode} - {response.ReasonPhrase}"
                            : errorContent);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }
    }
    }