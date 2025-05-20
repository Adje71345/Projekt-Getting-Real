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
using GettingRealWPF.ViewModel;

namespace GettingRealWPF.View.UserControls
{
    public partial class SearchBar : UserControl
    {
        public SearchBar()
        {
            InitializeComponent();
            UpdatePlaceHolderVisibility();
        }


        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtInput.Clear();
            txtInput.Focus();
        }

        private void OnCategorySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void OnLocationSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void UpdateComboBoxText(string searchText, string selectedCategory, string selectedLocation)
        {
            // Kategori
            if (!string.IsNullOrEmpty(selectedCategory) && selectedCategory != "Alle kategorier")
            {
                cmbCategory.Text = selectedCategory;
            }
            else
            {
                cmbCategory.Text = "Alle kategorier";
            }

            // Lokation
            if (!string.IsNullOrEmpty(selectedLocation) && selectedLocation != "Alle lokationer")
            {
                cmbLocation.Text = selectedLocation;
            }
            else
            {
                cmbLocation.Text = "Alle lokationer";
            }
        }

        private void UpdatePlaceHolderVisibility()
        {
            txtPlaceholder.Visibility = string.IsNullOrEmpty(txtInput.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePlaceHolderVisibility();
        }
    }
}
