using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace ООО_Ткани_Сессия1_2
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        public AuthorizationWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataBaseClass.USERS_ID = "sa";
            DataBaseClass.Password = "12345";
            DataBaseClass.ConnectionString = String.Format(DataBaseClass.ConnectionString, DataBaseClass.USERS_ID, DataBaseClass.Password);
            DataBaseClass dataBaseClass = new DataBaseClass();
            try
            {
                Visibility = Visibility.Hidden;

                SqlCommand command = new SqlCommand("select [ROLE_ID] from [dbo].[USER] where EMAIL_USER = @Email AND PASSWORD_USER = @Password");
                command.Parameters.AddWithValue("@Email", pbEmail.Text);
                command.Parameters.AddWithValue("@Password", pbPassword.Password);
                command.Connection = dataBaseClass.connection;
                dataBaseClass.connection.Open();
                int role = (int)command.ExecuteScalar();

                if (role != null)
                {
                    if(role == 1)
                    {
                        MainWindow window = new MainWindow();
                        window.Show();
                    }
                    else if (role == 1)
                    {
                        ClientWindow window = new ClientWindow();
                        window.Show();
                    }
                    Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль");
                }
            }
            catch
            {
                MessageBox.Show("Неверный логин или пароль");
            }
            finally
            {
                dataBaseClass.connection.Close();
                pbEmail.Clear();
                pbPassword.Clear(); 
            }
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
            Close();

        }
    }
}
