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
    /// Логика взаимодействия для PartnersAddEditPage.xaml
    /// </summary>
    public partial class PartnersAddEditPage : Page
    {
        Partners partner;
        public PartnersAddEditPage(Partners _partner)
        {
            InitializeComponent();
            partner = _partner;
            this.DataContext = partner;
            TypeCb.ItemsSource = ConnectionClass.comfortEntities.TypeOfBusiness.ToList();
            TypeCb.DisplayMemberPath = "NameBusiness";
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                partner.Id_type = (TypeCb.SelectedItem as TypeOfBusiness).Id_type;
                if(partner.Id_partner == 0)
                {
                    ConnectionClass.comfortEntities.Partners.Add(partner);
                }
                ConnectionClass.comfortEntities.SaveChanges();
                MessageBox.Show("Операция прошла успешно");
                NavigationService.Navigate(new PartnersListPage());
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Отмена операции", "Отмена", MessageBoxButton.OK, MessageBoxImage.Information);
            NavigationService.Navigate(new PartnersListPage());
        }
    }
}
