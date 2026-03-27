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
    /// Логика взаимодействия для PartnersListPage.xaml
    /// </summary>
    public partial class PartnersListPage : Page
    {
        private List<Partners> allPartners;
        private Partners selectedPartner;

        public PartnersListPage()
        {
            InitializeComponent();
            PartnersLW.ItemsSource = ConnectionClass.comfortEntities.Partners.ToList();
        }

        private void LoadPartners()
        {
            try
            {
                allPartners = ConnectionClass.comfortEntities.Partners.ToList();
                UpdateListPartner();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateListPartner()
        {
            if (allPartners == null) return;

            var filteredPartners = allPartners.ToList();

            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                string searchText = SearchTextBox.Text.ToLower();
                filteredPartners = filteredPartners.Where(p => (p.NamePartner != null && p.NamePartner.ToLower().Contains(searchText)) ||
                    (p.TypeOfBusiness != null && p.TypeOfBusiness.NameBusiness != null && p.TypeOfBusiness.NameBusiness.ToLower().Contains(searchText)) ||
                    (p.Phone != null && p.Phone.ToLower().Contains(searchText)) || (p.SurnameDirector != null && p.SurnameDirector.ToLower().Contains(searchText))).ToList();
            }
            if (SortComboBox.SelectedItem != null)
            {
                string selectedSort = (SortComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                if (selectedSort == "По названию (A-Z)")
                {
                    filteredPartners = filteredPartners.OrderBy(p => p.NamePartner).ToList();
                }
                else if (selectedSort == "По названию (Z-A)")
                {
                    filteredPartners = filteredPartners.OrderByDescending(p => p.NamePartner).ToList();
                }
                else
                {
                    filteredPartners = filteredPartners.OrderBy(p => p.Id_partner).ToList();
                }
            }

            PartnersLW.ItemsSource = filteredPartners;
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var selPartner = PartnersLW.SelectedItem as Partners;
            if (PartnersLW.SelectedItems != null)
            {
                NavigationService.Navigate(new PartnersAddEditPage(selPartner));
            }
            else
            {
                MessageBox.Show("Не выбран партнер для редактрирвоания!");
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            
            NavigationService.Navigate(new PartnersAddEditPage(new Partners()));
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {

            if (selectedPartner != null)
            {
                NavigationService.Navigate(new PartnerHistoryPage(selectedPartner));
            }
            else
            {
                MessageBox.Show("ВЫБЕРИ ПАРТНЕРА", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPartner == null)
            {
                MessageBox.Show("Сначала выберите партнера!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MessageBoxResult result = MessageBox.Show($"Вы точно хотите удалить партнера \"{selectedPartner.NamePartner}\"?", "Подтверждение удаления",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    ConnectionClass.comfortEntities.Partners.Remove(selectedPartner);
                    ConnectionClass.comfortEntities.SaveChanges();

                    MessageBox.Show("Партнер удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadPartners();
                    selectedPartner = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateListPartner();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateListPartner();
        }
    }
}
