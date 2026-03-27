using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
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
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
            LoadComboxes();
            
        }

        private void LoadComboxes()
        {
            try
            {
                familyStatusCombo.ItemsSource = ConnectionClass.comfortEntities.FamilyStatus.ToList();
                healthCombo.ItemsSource = ConnectionClass.comfortEntities.Health.ToList();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void registration_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(login.Text) || string.IsNullOrEmpty(password.Password) 
                || string.IsNullOrEmpty(surname.Text) ||
                string.IsNullOrEmpty(name.Text)
                || birthday.SelectedDate == null ||
                string.IsNullOrWhiteSpace(passportSeries.Text) || 
                string.IsNullOrWhiteSpace(passportNumber.Text) ||
                familyStatusCombo.SelectedItem == null || 
                healthCombo.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля", "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if(ConnectionClass.comfortEntities.Logins.Count(x => x.Login == login.Text) > 0)
                {
                    MessageBox.Show("Такой пользователь уже существует", "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                try
                {
                    Employee employeeObj = new Employee
                    {
                        Surname = surname.Text.Trim(),
                        Name = name.Text.Trim(),
                        Patronumic = string.IsNullOrWhiteSpace(patronymic.Text) ? null : patronymic.Text.Trim(),
                        Birthday = birthday.SelectedDate,
                        PassportNumber = passportNumber.Text.Trim(),
                        PassportSeria = passportSeries.Text.Trim(),
                        Id_family = (int)familyStatusCombo.SelectedItem,
                        Id_health = (int)healthCombo.SelectedItem,
                        Id_role = 3

                    };

                    ConnectionClass.comfortEntities.Employee.Add(employeeObj);
                    ConnectionClass.comfortEntities.SaveChanges();

                    Logins loginsObj = new Logins
                    {
                        Login = login.Text.Trim(),
                        Password = password.Password.Trim(),
                        Id_user = employeeObj.Id_employee
                    };



                    MessageBox.Show("Успешная регистрация", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.Navigate(new LoginPage());
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении данных {ex.Message}", "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
