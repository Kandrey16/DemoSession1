using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace ООО_Ткани_Сессия1
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

        private void BtnAuth_Click(object sender, RoutedEventArgs e)
        {
            DataBaseClass.Users_ID = "sa";
            DataBaseClass.Password = "12345";
            DataBaseClass.ConnectionString = string.Format(DataBaseClass.ConnectionString, DataBaseClass.Users_ID, DataBaseClass.Password);
            DataBaseClass dataBaseClass = new DataBaseClass();
            try
            {
                Visibility = Visibility.Hidden;

                SqlCommand command = new SqlCommand("Select ROLE_ID from [USER] where EMAIL_User = @Login and PASSWORD_USER = @Password");
                command.Parameters.AddWithValue("@Login", pbEmail.Text);
                command.Parameters.AddWithValue("@Password", pbPassword.Password);
                command.Connection = dataBaseClass.connection;
                dataBaseClass.connection.Open();
                int role = (int)command.ExecuteScalar();

                if (role != null)
                {
                    if(role == 1)
                    {
                        MainWindow main = new MainWindow();
                        main.Show();
                    } else
                    {
                        ClientWindow client = new ClientWindow();
                        client.Show();
                    }
                    Close();
                }
                else
                {
                    MessageBox.Show("Неверный Логин или пароль");
                }

            }
            catch
            {
                MessageBox.Show("Неверный логин или пароль!", DataBaseClass.App_name);
                pbEmail.Focus();
            }
            finally
            {
                dataBaseClass.connection.Close();
                pbEmail.Clear();
                pbPassword.Clear();
            }


        }
    }
}
