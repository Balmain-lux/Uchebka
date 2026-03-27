using System;
using System.Collections.Generic;
using System.Linq;
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
using Test942.Model;

namespace Test942.Pages
{
    /// <summary>
    /// Логика взаимодействия для PartnerHistoryPage.xaml
    /// </summary>
    public partial class PartnerHistoryPage : Page
    {
        private Partners currentPartner;

        public class SaleHistoryItem
        {
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public DateTime? SaleDate { get; set; }
            public decimal? Amount { get; set; }
            public string SalePoint { get; set; }
        }

        public PartnerHistoryPage()
        {
            InitializeComponent();
            LoadPartnerInfo();
            LoadSaleHistory();
        }

        private void LoadPartnerInfo()
        {
            txtPartnerName.Text = currentPartner.NamePartner;
            txtPartnerInfo.Text = $"{currentPartner.TypeOfBusiness?.NameBusiness} | Директор: {currentPartner.SurnameDirector} {currentPartner.NameDirector} | Телефон: {currentPartner.Phone}";
        }

        private void LoadSaleHistory()
        {
            try
            {

                var salePoints = ConnectionClass.comfortEntities.SalePoint.Where(sp => sp.Id_partner == currentPartner.Id_partner).Select(sp => sp.Id_point).ToList();
                var sales = ConnectionClass.comfortEntities.SaleHistory.Where(sh => salePoints.Contains(sh.Id_point ?? 0)).ToList();
                var historyList = new List<SaleHistoryItem>();
                decimal totalAmount = 0;
                foreach (var sale in sales)
                {
                    var product = ConnectionClass.comfortEntities.Products.FirstOrDefault(p => p.Id_product == sale.Id_product);
                    var salePoint = ConnectionClass.comfortEntities.SalePoint.FirstOrDefault(sp => sp.Id_point == sale.Id_point);
                    var historyItem = new SaleHistoryItem
                    {
                        ProductName = product?.NameProduct ?? "Неизвестный товар",
                        Quantity = sale.Quantity ?? 0,
                        SaleDate = null,
                        Amount = sale.Amount,
                        SalePoint = salePoint?.NamePoint ?? "Неизвестная точка"
                    };
                    historyList.Add(historyItem);
                    if (sale.Amount.HasValue)
                        totalAmount += sale.Amount.Value;
                }
                if (historyList.Count == 0)
                {
                    LoadRequestsHistory(historyList, ref totalAmount);
                }
                HistoryLv.ItemsSource = historyList;
                txtTotalAmount.Text = $"{totalAmount:N2} руб.";
                if (historyList.Count == 0)
                {
                    txtTotalAmount.Text = "0.00 руб.";
                    MessageBox.Show("У данного партнера нет истории продаж", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки истории: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRequestsHistory(List<SaleHistoryItem> historyList, ref decimal totalAmount)
        {
            var requests = ConnectionClass.comfortEntities.Request.Where(r => r.Id_partner == currentPartner.Id_partner && r.Id_status == 5).ToList();
            foreach (var request in requests)
            {
                var details = ConnectionClass.comfortEntities.RequestDetails.Where(rd => rd.Id_request == request.Id_request).ToList();
                foreach (var detail in details)
                {
                    var product = ConnectionClass.comfortEntities.Products.FirstOrDefault(p => p.Id_product == detail.Id_product);
                    var historyItem = new SaleHistoryItem
                    {
                        ProductName = product?.NameProduct ?? "Неизвестный товар",
                        Quantity = detail.Quantity ?? 0,
                        SaleDate = request.RequestDate,
                        Amount = request.TotalAmountReq,
                        SalePoint = "Заказ"
                    };
                    historyList.Add(historyItem);

                    if (request.TotalAmountReq.HasValue)
                        totalAmount += request.TotalAmountReq.Value;
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PartnersListPage());
        }
    }
}
