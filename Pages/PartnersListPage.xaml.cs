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
using Test942.Pages;

namespace Test942.Pages
{
    /// <summary>
    /// Логика взаимодействия для PartnersListPage.xaml
    /// </summary>
    public partial class PartnersListPage : Page
    {
        

        public PartnersListPage()
        {
            InitializeComponent();

            try
            {
                PartnersLW.ItemsSource = ConnectionClass.comfortEntities.Partners.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedPartner = PartnersLW.SelectedItem as Partners;
            if (selectedPartner != null)
            {
                NavigationService.Navigate(new PartnersAddEditPage(selectedPartner));
            }
            else
            {
                MessageBox.Show("Выберите партнера для редактирования!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            
            NavigationService.Navigate(new PartnersAddEditPage(new Partners()));
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {

            var selectedPartner = PartnersLW.SelectedItem as Partners;
            if (selectedPartner != null)
            {
                NavigationService.Navigate(new PartnerHistoryPage(selectedPartner));
            }
            else
            {
                MessageBox.Show("Выберите партнера!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedPartner = PartnersLW.SelectedItem as Partners;
            if (selectedPartner == null)
            {
                MessageBox.Show("Выберите партнера!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить партнера \"{selectedPartner.NamePartner}\"?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    ConnectionClass.comfortEntities.Partners.Remove(selectedPartner);
                    ConnectionClass.comfortEntities.SaveChanges();
                    MessageBox.Show("Партнер удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    PartnersLW.ItemsSource = ConnectionClass.comfortEntities.Partners.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var partners = ConnectionClass.comfortEntities.Partners.ToList();

            string searchText = SearchTextBox.Text?.ToLower().Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                partners = partners.Where(p =>
                    (p.NamePartner != null && p.NamePartner.ToLower().Contains(searchText)) ||
                    (p.TypeOfBusiness != null && p.TypeOfBusiness.NameBusiness != null && p.TypeOfBusiness.NameBusiness.ToLower().Contains(searchText)) ||
                    (p.Phone != null && p.Phone.ToLower().Contains(searchText)) ||
                    (p.SurnameDirector != null && p.SurnameDirector.ToLower().Contains(searchText))
                ).ToList();
            }

            string selectedSort = (SortComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (selectedSort == "По названию (A-Z)")
            {
                partners = partners.OrderBy(p => p.NamePartner).ToList();
            }
            else if (selectedSort == "По названию (Z-A)")
            {
                partners = partners.OrderByDescending(p => p.NamePartner).ToList();
            }
            else
            {
                partners = partners.OrderBy(p => p.Id_partner).ToList();
            }

            PartnersLW.ItemsSource = partners;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var partners = ConnectionClass.comfortEntities.Partners.ToList();

            string searchText = SearchTextBox.Text?.ToLower().Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                partners = partners.Where(p =>
                    (p.NamePartner != null && p.NamePartner.ToLower().Contains(searchText)) ||
                    (p.TypeOfBusiness != null && p.TypeOfBusiness.NameBusiness != null && p.TypeOfBusiness.NameBusiness.ToLower().Contains(searchText)) ||
                    (p.Phone != null && p.Phone.ToLower().Contains(searchText)) ||
                    (p.SurnameDirector != null && p.SurnameDirector.ToLower().Contains(searchText))
                ).ToList();
            }

            string selectedSort = (SortComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (selectedSort == "По названию (A-Z)")
            {
                partners = partners.OrderBy(p => p.NamePartner).ToList();
            }
            else if (selectedSort == "По названию (Z-A)")
            {
                partners = partners.OrderByDescending(p => p.NamePartner).ToList();
            }
            else
            {
                partners = partners.OrderBy(p => p.Id_partner).ToList();
            }

            PartnersLW.ItemsSource = partners;
        }
    
    }
}
