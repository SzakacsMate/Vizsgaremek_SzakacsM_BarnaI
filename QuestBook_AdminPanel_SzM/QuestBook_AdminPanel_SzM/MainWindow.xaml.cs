using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuestBook_AdminPanel_SzM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            RequestAPI();
            var url = "https://localhost:7231";
        }

        private async Task Button_Click(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            var getUsersApiUrl = await client.GetAsync(url+"/api/Auth/Users");
        }

        public async void RequestAPI()
        {
            using HttpClient client = new HttpClient();
            
            HttpResponseMessage latLenResponse = await client.GetAsync(url);
            string latLenResponseData = await latLenResponse.Content.ReadAsStringAsync();
            JObject latLenJsonObj = JObject.Parse(latLenResponseData);
        }
    }
}