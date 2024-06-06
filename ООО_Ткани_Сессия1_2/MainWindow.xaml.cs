using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ООО_Ткани_Сессия1_2.Models;

namespace ООО_Ткани_Сессия1_2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Product_Fill();
        }
        private void dgProduct_Loaded(object sender, RoutedEventArgs e)
        {
            Product_Fill();
        }

        public void Product_Fill(string searchQuery = "")
        {
            Action action = () =>
            {
                DataBaseClass dataBaseClass = new DataBaseClass();
                string query = "SELECT * from [dbo].[PRODUCT]";

                // Если есть поисковый запрос, добавляем условие WHERE
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query += " WHERE [NAME_PRODUCT] LIKE '%' + @searchQuery + '%' " +
                             "OR [DESCRIPTION_PRODUCT] LIKE '%' + @searchQuery + '%' " +
                             "OR [CREATOR_PRODUCT] LIKE '%' + @searchQuery + '%' " +
                             "OR [PRICE_PRODUCT] LIKE '%' + @searchQuery + '%' " +
                             "OR [IMG_PRODUCT] LIKE '%' + @searchQuery + '%' " +
                             "OR [IS_ENABLED] LIKE '%' + @searchQuery + '%'";
                }

                // Создаем команду и добавляем параметр
                dataBaseClass.command.CommandText = query;
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    dataBaseClass.command.Parameters.Clear();
                    dataBaseClass.command.Parameters.AddWithValue("@searchQuery", searchQuery);
                }

                dataBaseClass.sqlExecute(query, DataBaseClass.act.select);
                dataBaseClass.dependency.OnChange += Dependency_OnChange_Product;
                dgProduct.ItemsSource = dataBaseClass.resultTable.DefaultView;
                dgProduct.Columns[0].Header = "ID";
                dgProduct.Columns[1].Header = "Название";
                dgProduct.Columns[2].Header = "Описание";
                dgProduct.Columns[3].Header = "Производитель";
                dgProduct.Columns[4].Header = "Цена";
                dgProduct.Columns[5].Header = "Изображение";
                dgProduct.Columns[6].Header = "Доступность";
            };
            Dispatcher.Invoke(action);
        }

        private void Dependency_OnChange_Product(object sender, SqlNotificationEventArgs e)
        {
            if (e.Info != SqlNotificationInfo.Invalid)
            Product_Fill();
        }

        private void dgProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgProduct.Items.Count != 0 && dgProduct.SelectedItems.Count != 0)
            {
                DataRowView dataRow = (DataRowView)dgProduct.SelectedItems[0];
                tbxNameProduct.Text = dataRow[1].ToString();
                tbxDescriptionProduct.Text = dataRow[2].ToString();
                tbxCreatorProduct.Text = dataRow[3].ToString();
                tbxPriceProduct.Text = dataRow[4].ToString();
                tbxImgProduct.Text = dataRow[5].ToString();
            }
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataBaseClass dataBaseClass = new DataBaseClass();

                decimal price;
                if (!decimal.TryParse(tbxPriceProduct.Text, out price))
                {
                    MessageBox.Show("Некорректное значение цены.", DataBaseClass.App_name, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string priceString = price.ToString(System.Globalization.CultureInfo.InvariantCulture);

                string isEnabledValue = cbIsEnabledProduct.IsChecked == true ? "1" : "0";

                string query = String.Format("INSERT INTO [dbo].[Product] " +
                    "([NAME_PRODUCT], [DESCRIPTION_PRODUCT], [CREATOR_PRODUCT], [PRICE_PRODUCT], [IMG_PRODUCT], [IS_ENABLED]) " +
                    "VALUES ('{0}', '{1}', '{2}', {3}, '{4}', {5})",
                    tbxNameProduct.Text, tbxDescriptionProduct.Text, tbxCreatorProduct.Text, priceString, tbxImgProduct.Text, isEnabledValue);

                dataBaseClass.sqlExecute(query, DataBaseClass.act.manipulation);

                // Очистка полей после добавления записи
                tbxNameProduct.Clear();
                tbxDescriptionProduct.Clear();
                tbxCreatorProduct.Clear();
                tbxPriceProduct.Clear();
                tbxImgProduct.Clear();
                cbIsEnabledProduct.IsChecked = false;

                Product_Fill();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", DataBaseClass.App_name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }


        private void btnUpdProduct_Click(object sender, RoutedEventArgs e)
        {
            if (dgProduct.Items.Count != 0 && dgProduct.SelectedItems.Count != 0)
            {
                DataBaseClass dataBaseClass = new DataBaseClass();
                DataRowView dataRowView = (DataRowView)dgProduct.SelectedItems[0];

                decimal price;
                if (!decimal.TryParse(tbxPriceProduct.Text, out price))
                {
                    MessageBox.Show("Некорректное значение цены.", DataBaseClass.App_name, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string priceString = price.ToString(System.Globalization.CultureInfo.InvariantCulture);

                string isChecked = cbIsEnabledProduct.IsChecked == true ? "1" : "0";

                string query = String.Format("update [dbo].[Product] " +
                    "set [NAME_PRODUCT] = '{0}',[DESCRIPTION_PRODUCT] = '{1}',[CREATOR_PRODUCT] = '{2}',[PRICE_PRODUCT] = '{3}',[IMG_PRODUCT] = '{4}', [IS_ENABLED] = '{5}' where [ID_PRODUCT] = {6}", tbxNameProduct.Text, tbxDescriptionProduct.Text, tbxCreatorProduct.Text, priceString, tbxImgProduct.Text, isChecked, dataRowView[0]);

                dataBaseClass.sqlExecute(query, DataBaseClass.act.manipulation);
                tbxNameProduct.Clear();
                tbxDescriptionProduct.Clear();
                tbxCreatorProduct.Clear();
                tbxPriceProduct.Clear();
                tbxImgProduct.Clear();
                cbIsEnabledProduct.IsChecked = false;

                Product_Fill();
            }
        }

        private void btnDelProduct_Click(object sender, RoutedEventArgs e)
        {
            switch (MessageBox.Show("Вы действительно хотите удалить данную запись?", DataBaseClass.App_name, MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                case MessageBoxResult.Yes:
                    if (dgProduct.Items.Count != 0 && dgProduct.SelectedItems.Count != 0)
                    {
                        DataBaseClass dataBaseClass = new DataBaseClass();
                        DataRowView dataRowView = (DataRowView)dgProduct.SelectedItems[0];
                        string query = String.Format("delete [dbo].[Product] where [ID_PRODUCT] = {0}", dataRowView[0]);
                        dataBaseClass.sqlExecute(query, DataBaseClass.act.manipulation);
                        Product_Fill();
                    }
                    break;
            }
        }

        private void tbxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Product_Fill(tbxSearch.Text);
        }
    }
}
