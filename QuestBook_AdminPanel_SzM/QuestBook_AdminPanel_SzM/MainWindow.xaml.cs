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
        private async void AddLocation_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Window
            {
                Title = "Helyszín hozzáadása",
                Width = 400,
                Height = 350,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };

            var stack = new StackPanel { Margin = new Thickness(10) };

            var txtName = new TextBox { Margin = new Thickness(0, 0, 0, 5) };
            var txtAdress = new TextBox { Margin = new Thickness(0, 0, 0, 5) };
            var txtDescription = new TextBox { Margin = new Thickness(0, 0, 0, 5) };

            // Image file picker
            var txtImagePath = new TextBox
            {
                Margin = new Thickness(0, 0, 0, 5),
                IsReadOnly = true
            };
            var btnBrowse = new Button
            {
                Content = "Tallózás...",
                Width = 100,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 10)
            };
            string? selectedFilePath = null;

            btnBrowse.Click += (s, args) =>
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "Válassz ki egy képet",
                    Filter = "Képfájlok (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Minden fájl (*.*)|*.*"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    selectedFilePath = openFileDialog.FileName;
                    txtImagePath.Text = openFileDialog.FileName;
                }
            };

            var btnOk = new Button { Content = "Hozzáadás", Width = 100 };

            stack.Children.Add(new TextBlock { Text = "Helyszín neve:" });
            stack.Children.Add(txtName);
            stack.Children.Add(new TextBlock { Text = "Cím:" });
            stack.Children.Add(txtAdress);
            stack.Children.Add(new TextBlock { Text = "Leírás:" });
            stack.Children.Add(txtDescription);
            stack.Children.Add(new TextBlock { Text = "Kép (opcionális):" });
            stack.Children.Add(txtImagePath);
            stack.Children.Add(btnBrowse);
            stack.Children.Add(btnOk);

            btnOk.Click += (s, args) => { dialog.DialogResult = true; };
            dialog.Content = stack;

            if (dialog.ShowDialog() != true)
                return;

            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtAdress.Text))
            {
                MessageBox.Show("Helyszín neve és cím megadása kötelező.");
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);

                    // Convert image file to Base64 data URI string
                    string imageValue = "N/A";
                    if (!string.IsNullOrWhiteSpace(selectedFilePath) && System.IO.File.Exists(selectedFilePath))
                    {
                        var fileBytes = await System.IO.File.ReadAllBytesAsync(selectedFilePath);
                        var base64 = Convert.ToBase64String(fileBytes);
                        var extension = System.IO.Path.GetExtension(selectedFilePath).TrimStart('.').ToLower();
                        var mimeType = extension switch
                        {
                            "jpg" or "jpeg" => "image/jpeg",
                            "png" => "image/png",
                            "gif" => "image/gif",
                            "bmp" => "image/bmp",
                            _ => "application/octet-stream"
                        };
                        imageValue = $"data:{mimeType};base64,{base64}";
                    }

                    var body = new
                    {
                        LocationName = txtName.Text,
                        Adress = txtAdress.Text,
                        Description = txtDescription.Text,
                        Image = imageValue
                    };

                    var json = JsonConvert.SerializeObject(body);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    var addLocationApiUrl = $"{url}/api/Auth/AddLocation";
                    HttpResponseMessage response = await client.PostAsync(addLocationApiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Helyszín sikeresen hozzáadva: {txtName.Text}");
                        await LoadLocations();
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