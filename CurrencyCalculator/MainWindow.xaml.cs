using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace CurrencyCalculator
{
    public partial class MainWindow : Window
    {
        private const string ApiKey = "625021f5c79546009b68625548219f35";
        private const string ApiUrl = "https://open.er-api.com/v1/latest/";

        public MainWindow()
        {
            InitializeComponent();
            LoadCurrencies();
        }

        private void LoadCurrencies()
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    string response = client.DownloadString(ApiUrl + "?app_id=" + ApiKey);
                    dynamic data = JsonConvert.DeserializeObject(response);

                    foreach (var currency in data.rates)
                    {
                        comboBoxFrom.Items.Add(currency.Name);
                        comboBoxTo.Items.Add(currency.Name);
                    }
                }
                catch (WebException)
                {
                    MessageBox.Show("Не удалось загрузить данные о валюте. Проверьте подключение к интернету и ключ API.");
                }
            }
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(textBoxAmount.Text, out double amount))
            {
                double fromRate = GetCurrencyRate(comboBoxFrom.Text);
                double toRate = GetCurrencyRate(comboBoxTo.Text);

                if (fromRate != 0 && toRate != 0)
                {
                    double result = (amount / fromRate) * toRate;
                    labelResult.Content = $"{result:F2} {comboBoxTo.Text}";
                }
                else
                {
                    MessageBox.Show("Не удалось конвертировать валюту. Проверьте, выбрана ли валюта и доступно ли подключение к Интернету");
                }
            }
            else
            {
                MessageBox.Show("Недопустимая сумма. Пожалуйста, введите действительный номер.");
            }
        }

        private double GetCurrencyRate(string currency)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    string response = client.DownloadString(ApiUrl + "?app_id=" + ApiKey);
                    dynamic data = JsonConvert.DeserializeObject(response);
                    return data.rates[currency];
                }
                catch (WebException)
                {
                    MessageBox.Show("Не удалось узнать курс валюты. Проверьте свое интернет-соединение и API-ключ.");
                    return 0;
                }
            }
        }
    }
}
