using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ООО_Ткани_Сессия1.Models;

namespace ООО_Ткани_Сессия1
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
            FillCreatorCombobox();
        }

        private void Product_Fill()
        {
            Action action = () =>
            {
                DataBaseClass dataBaseClass = new DataBaseClass();
                dataBaseClass.sqlExecute("select [ID_PRODUCT], [Name_product], [DESCRIPTION_PRODUCT],[CREATOR_PRODUCT],[PRICE_PRODUCT], [IMG_PRODUCT], [IS_ENABLED] from [dbo].[PRODUCT]", DataBaseClass.act.select);
                dataBaseClass.dependency.OnChange += Dependency_OnChange_Product;

                if(dataBaseClass.resultTable.Rows.Count > 0)
                {
                    var products = new List<Product>();

                    foreach (DataRow row in dataBaseClass.resultTable.Rows)
                    {
                        var product = new Product
                        {
                            ID_PRODUCT = Convert.ToInt32(row["ID_PRODUCT"]),
                            NAME_PRODUCT = row["NAME_PRODUCT"].ToString(),
                            DESCRIPTION_PRODUCT = row["DESCRIPTION_PRODUCT"].ToString(),
                            CREATOR_PRODUCT = row["CREATOR_PRODUCT"].ToString(),
                            PRICE_PRODUCT = Convert.ToSingle(row["PRICE_PRODUCT"]),
                            IMG_PRODUCT = row["IMG_PRODUCT"].ToString(),
                            IS_ENABLED = Convert.ToBoolean(row["IS_ENABLED"])
                        };
                    products.Add(product);
                    }
                    ProductItemsControl.ItemsSource = products;
                }
            };
            Dispatcher.Invoke(action);
        }

        private void Dependency_OnChange_Product(object sender, SqlNotificationEventArgs e)
        {
            if (e.Info != SqlNotificationInfo.Invalid)
                Product_Fill();
        }

        private void Searchproduct(string searchText)
        {
            /*string creatorFilter = "";
            if (creator != "Все")
            {
                creator = string.Format("AND [CREATOR_PRODUCT] = '{0}'", creator);
            }*/

            DataBaseClass dataBaseClass = new DataBaseClass();
            dataBaseClass.sqlExecute(string.Format("select [ID_PRODUCT], [Name_product], [DESCRIPTION_PRODUCT],[CREATOR_PRODUCT],[PRICE_PRODUCT], [IMG_PRODUCT], [IS_ENABLED] from [dbo].[PRODUCT] where [NAME_PRODUCT] like '%{0}%' or [DESCRIPTION_PRODUCT] like '%{0}%' or [CREATOR_PRODUCT] like '%{0}%'", searchText), DataBaseClass.act.select);
            dataBaseClass.dependency.OnChange += Dependency_OnChange_Product;

            if (dataBaseClass.resultTable.Rows.Count > 0)
            {
                var products = new List<Product>();

                foreach (DataRow row in dataBaseClass.resultTable.Rows)
                {
                    var product = new Product
                    {
                        ID_PRODUCT = Convert.ToInt32(row["ID_PRODUCT"]),
                        NAME_PRODUCT = row["NAME_PRODUCT"].ToString(),
                        DESCRIPTION_PRODUCT = row["DESCRIPTION_PRODUCT"].ToString(),
                        CREATOR_PRODUCT = row["CREATOR_PRODUCT"].ToString(),
                        PRICE_PRODUCT = Convert.ToSingle(row["PRICE_PRODUCT"]),
                        IMG_PRODUCT = row["IMG_PRODUCT"].ToString(),
                        IS_ENABLED = Convert.ToBoolean(row["IS_ENABLED"])
                    };
                    products.Add(product);
                }
                ProductItemsControl.ItemsSource = products;
            }
        }
        private void FillCreatorCombobox()
        {
            DataBaseClass dataBaseClass = new DataBaseClass();
            dataBaseClass.sqlExecute("SELECT DISTINCT [CREATOR_PRODUCT] FROM [dbo].[PRODUCT]", DataBaseClass.act.select);

            if(dataBaseClass.resultTable.Rows.Count > 0)
            {
                cbCreatorFilter.Items.Clear();

                foreach(DataRow row in dataBaseClass.resultTable.Rows)
                {
                    string creator = row["CREATOR_PRODUCT"].ToString();
                    cbCreatorFilter.Items.Add(creator);
                }
            }
        }
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = tbxSearch.Text;
            /*ComboBoxItem selectedCreator = (ComboBoxItem)cbCreatorFilter.SelectedItem;
            string creator = selectedCreator.Content.ToString();*/
            Searchproduct(searchText);
        }
        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            DataBaseClass dataBaseClass = new DataBaseClass();
            dataBaseClass.sqlExecute(String.Format("Insert into [dbo].[Product] ([NAME_PRODUCT],[DESCRIPTION_PRODUCT],[CREATOR_PRODUCT],[PRICE_PRODUCT],[IMG_PRODUCT], [IS_ENABLED]) values ('{0}','{1}','{2}','{3}','{4}','{5}')", tbxNameProduct.Text, tbxDescriptionProduct.Text, tbxCreatorProduct.Text, tbxPriceProduct.Text, tbxImgProduct.Text, cbIsEnabledProduct.IsChecked), DataBaseClass.act.select);
            tbxNameProduct.Clear();
            tbxDescriptionProduct.Clear();
            tbxCreatorProduct.Clear();
            tbxPriceProduct.Clear();
            tbxImgProduct.Clear();
            cbIsEnabledProduct.IsChecked = false;
        }

        private void btnUpdProduct_Click(object sender, RoutedEventArgs e)
        {
            Product selectedProduct = (Product)ProductItemsControl.SelectedItem;
            DataBaseClass dataBaseClass = new DataBaseClass();

            bool isEnabled = cbIsEnabledProduct.IsChecked ?? false;

            string query = String.Format("update [dbo].[Product] set [NAME_PRODUCT] = '{0}',[DESCRIPTION_PRODUCT] = '{1}',[CREATOR_PRODUCT] = '{2}',[PRICE_PRODUCT] = '{3}',[IMG_PRODUCT] = '{4}', [IS_ENABLED] = '{5}' where [ID_PRODUCT] = {6}", tbxNameProduct.Text, tbxDescriptionProduct.Text, tbxCreatorProduct.Text, tbxPriceProduct.Text, tbxImgProduct.Text, isEnabled, selectedProduct.ID_PRODUCT);

            dataBaseClass.sqlExecute(query, DataBaseClass.act.manipulation);

            tbxNameProduct.Clear();
            tbxDescriptionProduct.Clear();
            tbxCreatorProduct.Clear();
            tbxPriceProduct.Clear();
            tbxImgProduct.Clear();
            cbIsEnabledProduct.IsChecked = false;

            Product_Fill();
        }

        private void btnDelProduct_Click(object sender, RoutedEventArgs e)
        {
            switch(MessageBox.Show("Вы действительно хотите удалить данную запись?", DataBaseClass.App_name, MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                case MessageBoxResult.Yes:
                        
                Product selectedProduct = (Product)ProductItemsControl.SelectedItem;
                DataBaseClass dataBaseClass = new DataBaseClass();

                string query = String.Format("delete [dbo].[Product] where [ID_PRODUCT] = {0}", selectedProduct.ID_PRODUCT);
                dataBaseClass.sqlExecute(query, DataBaseClass.act.manipulation);

                Product_Fill();
                break;
            }
        }


        private void ProductItemsControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ProductItemsControl.SelectedItem != null)
            {
                Product selectedProduct = (Product)ProductItemsControl.SelectedItem;
                tbxNameProduct.Text = selectedProduct.NAME_PRODUCT;
                tbxDescriptionProduct.Text = selectedProduct.DESCRIPTION_PRODUCT;
                tbxCreatorProduct.Text = selectedProduct.CREATOR_PRODUCT;
                tbxPriceProduct.Text = selectedProduct.PRICE_PRODUCT.ToString();
                tbxImgProduct.Text = selectedProduct.IMG_PRODUCT.ToString();
                cbIsEnabledProduct.IsChecked = selectedProduct.IS_ENABLED;

                btnUpdProduct.IsEnabled = true;
            }
        }

        
    }
}
