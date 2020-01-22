using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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
using CarAPI.Model;

namespace CarWPF
{
    public partial class MainWindow : Window
    {
        private static HttpClient HttpClient = new HttpClient() {
            BaseAddress = new Uri("http://localhost:5000/api/cars") 
        };

        public ObservableCollection<Car> Cars { get; set; } = new ObservableCollection<Car>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadAllCars(null, null);
        }

        private async Task<List<Car>> loadCars(int year, int month, int day)
        {
            HttpResponseMessage carBookingResponse;
            if (year != 0 && month != 0 && day != 0)
            {
                carBookingResponse = await HttpClient.GetAsync("cars?year=" + year + "&month=" + month + "&day=" + day);
            }
            else
            {
                carBookingResponse = await HttpClient.GetAsync("cars");
            }
            carBookingResponse.EnsureSuccessStatusCode();
            var responseBody = await carBookingResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Car>>(responseBody);
        }

        private async void LoadCarsByDate(object sender, RoutedEventArgs e)
        {
            MessageBox.Text = "";
            if (DateFilter.DisplayDate != default)
            {
                UpdateList(await loadCars(DateFilter.DisplayDate.Year, DateFilter.DisplayDate.Month, DateFilter.DisplayDate.Day));
            }
        }

        private async void LoadAllCars(object sender, RoutedEventArgs e)
        {
            MessageBox.Text = "";
            UpdateList(await loadCars(0, 0, 0));
        }

        private void UpdateList(IEnumerable<Car> carArray)
        {
            Cars.Clear();
            foreach (Car car in carArray)
            {
                Cars.Add(car);
            }
        }

        public DateTime BookingDate { get; set; } = DateTime.Now.Date;

        private readonly Car car;

        private async void BookCar(object sender, RoutedEventArgs e)
        {

            Booking request = new Booking
            {
                BookedDate = BookingDate,
                CarId = car.CarId,
                Car = car
            };

            HttpClient httpClient2 = new HttpClient();
            var client = httpClient2;
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:5000/api/Bookings", content);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                MessageBox.Text ="Can't book car";
            }
            Close();
        }
    }
}
