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

       

        public PartnerHistoryPage(Partners partner)
        {
            InitializeComponent();


            currentPartner = partner;

            txtPartnerName.Text = currentPartner.NamePartner;
            txtPartnerInfo.Text = $"{currentPartner.TypeOfBusiness?.NameBusiness} | Директор: {currentPartner.SurnameDirector} {currentPartner.NameDirector} | Телефон: {currentPartner.Phone}";

            try
            {
                var historyList = new List<dynamic>();
                decimal totalAmount = 0;

                var salePoints = ConnectionClass.comfortEntities.SalePoint
                    .Where(sp => sp.Id_partner == currentPartner.Id_partner)
                    .Select(sp => sp.Id_point)
                    .ToList();

                var sales = ConnectionClass.comfortEntities.SaleHistory
                    .Where(sh => salePoints.Contains(sh.Id_point ?? 0))
                    .ToList();

                foreach (var sale in sales)
                {
                    var product = ConnectionClass.comfortEntities.Products
                        .FirstOrDefault(p => p.Id_product == sale.Id_product);
                    var salePoint = ConnectionClass.comfortEntities.SalePoint
                        .FirstOrDefault(sp => sp.Id_point == sale.Id_point);

                    historyList.Add(new
                    {
                        ProductName = product?.NameProduct ?? "Неизвестный товар",
                        Quantity = sale.Quantity ?? 0,
                        Amount = sale.Amount,
                        SalePoint = salePoint?.NamePoint ?? "Неизвестная точка"
                    });

                    if (sale.Amount.HasValue)
                        totalAmount += sale.Amount.Value;
                }

                var requests = ConnectionClass.comfortEntities.Request
                    .Where(r => r.Id_partner == currentPartner.Id_partner && r.Id_status == 5)
                    .ToList();

                foreach (var request in requests)
                {
                    var details = ConnectionClass.comfortEntities.RequestDetails
                        .Where(rd => rd.Id_request == request.Id_request)
                        .ToList();

                    foreach (var detail in details)
                    {
                        var product = ConnectionClass.comfortEntities.Products
                            .FirstOrDefault(p => p.Id_product == detail.Id_product);

                        historyList.Add(new
                        {
                            ProductName = product?.NameProduct ?? "Неизвестный товар",
                            Quantity = detail.Quantity ?? 0,
                            SaleDate = request.RequestDate,
                            Amount = request.TotalAmountReq,
                            SalePoint = "Заказ"
                        });

                        if (request.TotalAmountReq.HasValue)
                            totalAmount += request.TotalAmountReq.Value;
                    }
                }

                HistoryLv.ItemsSource = historyList;
                txtTotalAmount.Text = $"{totalAmount:N2} руб.";

                if (historyList.Count == 0)
                {
                    MessageBox.Show("У данного партнера нет истории продаж", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки истории: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

            public int CalculateRequiredMaterial(int productTypeId, int materialTypeId, int productQuantity, double productParam1, double productParam2)
        {
            try
            {
                if (productQuantity <= 0 || productParam1 <= 0 || productParam2 <= 0)
                {
                    return -1;
                }

                var productType = ConnectionClass.comfortEntities.ProductType
                    .FirstOrDefault(pt => pt.Id_prodtype == productTypeId);

                if (productType == null)
                {
                    return -1;
                }

                var materialType = ConnectionClass.comfortEntities.TypeMaterial
                    .FirstOrDefault(mt => mt.Id_type_material == materialTypeId);

                if (materialType == null)
                {
                    return -1;
                }

                double coefficient = double.Parse(productType.Coefficient);
                double lostPercent = (double)materialType.LostProcent;

                double materialPerUnit = productParam1 * productParam2 * coefficient;

                double totalMaterialWithoutWaste = materialPerUnit * productQuantity;

                double wastePercentage = lostPercent / 100.0;
                double totalMaterialWithWaste = totalMaterialWithoutWaste * (1 + wastePercentage);

                int result = (int)Math.Ceiling(totalMaterialWithWaste);

                return result;
            }
            catch (Exception)
            {
                return -1;
            }
        
        }



        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PartnersListPage());
        }
    }
}
